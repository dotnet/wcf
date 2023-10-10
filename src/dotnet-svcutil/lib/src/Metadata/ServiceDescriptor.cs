// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xml;
using WsdlNS = System.Web.Services.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    public class ServiceDescriptor
    {
        public const bool DefaultUseMessageFormat = true;
        internal OperationalContext? OperationalCtx { get; set; }

        protected MetadataDocumentLoader metadataDocumentLoader;

        internal ServiceDescriptor(
            string serviceUri,
            IHttpCredentialsProvider userCredentialsProvider,
            IClientCertificateProvider clientCertificateProvider,
            IServerCertificateValidationProvider serverCertificateValidationProvider)
        {
            if (string.IsNullOrWhiteSpace(serviceUri))
            {
                throw new ArgumentException(nameof(serviceUri));
            }

            this.metadataDocumentLoader = new MetadataDocumentLoader(serviceUri, userCredentialsProvider, clientCertificateProvider, serverCertificateValidationProvider);
        }
        internal ServiceDescriptor(
           List<string> metadataFiles,
           IHttpCredentialsProvider userCredentialsProvider,
           IClientCertificateProvider clientCertificateProvider,
           IServerCertificateValidationProvider serverCertificateValidationProvider)
        {
            if (metadataFiles == null)
            {
                throw new ArgumentException(nameof(metadataFiles));
            }

            this.metadataDocumentLoader = new MetadataDocumentLoader(metadataFiles, false, userCredentialsProvider, clientCertificateProvider, serverCertificateValidationProvider);
        }

        internal ServiceDescriptor(MetadataDocumentLoader metadataDocumentLoader)
        {
            this.metadataDocumentLoader = metadataDocumentLoader ?? throw new ArgumentNullException(nameof(metadataDocumentLoader));
        }

        public Uri MetadataUrl { get { return this.metadataDocumentLoader.MetadataSourceUrl; } }

        public IEnumerable<Uri> MetadataFiles { get { return this.metadataDocumentLoader.MetadataSourceFiles; } }

        public IEnumerable<ServiceEndpoint> Endpoints { get; private set; } = new List<ServiceEndpoint>();

        public IEnumerable<Binding> Bindings { get; private set; } = new List<Binding>();

        public IEnumerable<ContractDescription> Contracts { get; private set; } = new List<ContractDescription>();

        public IEnumerable<Exception> DocumentLoadExceptions { get { return this.metadataDocumentLoader.DocumentLoadExceptions; } }

        public IEnumerable<MetadataConversionError> MetadataConversionErrors { get; private set; } = new List<MetadataConversionError>();

        public bool MetadataImported { get { return this.metadataDocumentLoader.State == MetadataDocumentLoader.LoadState.Successful; } }

        public IEnumerable<ServiceInfo> Services { get; private set; } = new List<ServiceInfo>();

        public IEnumerable<MetadataSection> MetadataDocuments
        {
            get
            {
                CheckMetadataImported();
                return this.metadataDocumentLoader.MetadataSections;
            }
        }

        public async Task ImportMetadataAsync(CancellationToken cancellationToken)
        {
            await ImportMetadataAsync(null, null, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task ImportMetadataAsync(Action<WsdlImporter> onWsdlImporterCreated, Action<ServiceDescriptor> onBeforeMetadataImport, Func<ServiceDescriptor, bool> onAfterMetadataImport, CancellationToken cancellationToken)
        {
            if (this.metadataDocumentLoader.State == MetadataDocumentLoader.LoadState.Successful)
            {
                return;
            }

            //if it's net.pipe url
            if (MetadataUrl != null && MetadataUrl.Scheme.Equals("net.pipe"))
            {
                string tfn;                
                if(OperationalCtx == OperationalContext.Infrastructure)
                {
                    tfn = "net462";
                }
                else
                {
                    tfn = "net6.0";
                }

                string toolPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Assembly assembly = Assembly.LoadFrom($"{toolPath}/internalAssets/{tfn}/Microsoft.Svcutil.NamedPipeMetadataImporter.dll");

                Type type = assembly.GetType("Microsoft.Tools.ServiceModel.Svcutil.NamedPipeMetadataImporter");
                if (type != null)
                {
                    object typeInstance = Activator.CreateInstance(type, null);
                    MethodInfo methodInfo = type.GetMethod("GetMetadatadataAsync", BindingFlags.Public | BindingFlags.Instance);
                    var xmlReader = await (Task<System.Xml.XmlReader>)methodInfo.Invoke(typeInstance, new object[] { MetadataUrl });

                    if (xmlReader != null)
                    {
                        Encoding encoding = Encoding.UTF8;
                        MemoryStream stream = new MemoryStream(encoding.GetBytes(xmlReader.ReadOuterXml()));
                        stream.Position = 0;
                        XmlReader reader = XmlDictionaryReader.CreateTextReader(
                            new MaxMessageSizeStream(stream, int.MaxValue),
                            Encoding.UTF8,
                            EncoderDefaults.ReaderQuotas,
                            null);

                        reader.Read();
                        reader.MoveToContent();

                        MetadataSet newSet = MetadataSet.ReadFrom(reader);
                        (this.metadataDocumentLoader.MetadataSections as List<MetadataSection>).AddRange(newSet.MetadataSections);
                        this.metadataDocumentLoader.State = MetadataDocumentLoader.LoadState.Successful;
                    }
                }
            }
            else
            {
                await this.metadataDocumentLoader.LoadAsync(cancellationToken).ConfigureAwait(false);
            }

            bool useMessageFormat = ServiceDescriptor.DefaultUseMessageFormat;
            bool importSuccess = false;
            WsdlImporter wsdlImporter;

            do
            {
                wsdlImporter = await CreateWsdlImporterAsync(useMessageFormat, cancellationToken).ConfigureAwait(false);
                onWsdlImporterCreated?.Invoke(wsdlImporter);
                onBeforeMetadataImport?.Invoke(this);

                await AsyncHelper.RunAsync(() =>
                {
                    this.Endpoints = wsdlImporter.ImportAllEndpoints();
                    this.Bindings = wsdlImporter.ImportAllBindings();
                    this.Contracts = wsdlImporter.ImportAllContracts();
                }, cancellationToken).ConfigureAwait(false);

                importSuccess = onAfterMetadataImport == null || onAfterMetadataImport.Invoke(this);
                useMessageFormat = !useMessageFormat;
            }
            while (!importSuccess && useMessageFormat != ServiceDescriptor.DefaultUseMessageFormat);

            this.MetadataConversionErrors = wsdlImporter.Errors;

            PopulateServices(wsdlImporter, cancellationToken);
        }

        private void PopulateServices(WsdlImporter wsdlImporter, CancellationToken cancellationToken)
        {
            var orphanContracts = new List<ContractDescription>(this.Contracts);
            var services = new List<ServiceInfo>();
            foreach (WsdlNS.ServiceDescription wsdlDocument in wsdlImporter.WsdlDocuments)
            {
                foreach (WsdlNS.Service wsdlService in wsdlDocument.Services)
                {
                    List<ContractDescription> contracts = new List<ContractDescription>();
                    foreach (WsdlNS.Port port in wsdlService.Ports)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        // Get the contract through the endpoint with binding
                        ServiceEndpoint endpoint = this.Endpoints.FirstOrDefault(ep =>
                            (port.Binding.Name == ep.Binding.Name) &&
                            (port.Binding.Namespace == ep.Binding.Namespace));

                        if (endpoint != null)
                        {
                            // Prevent duplicated item
                            if (!contracts.Contains(endpoint.Contract))
                            {
                                contracts.Add(endpoint.Contract);

                                // Remove the contract which had been associated with endpoint
                                orphanContracts.Remove(endpoint.Contract);
                            }
                        }
                    }

                    services.Add(new ServiceInfo(wsdlService.Name, contracts));
                }
            }

            // Add the contracts without endpoint as service
            foreach (ContractDescription contract in orphanContracts)
            {
                cancellationToken.ThrowIfCancellationRequested();
                services.Add(new ServiceInfo(contract));
            }

            this.Services = services;
        }

        public async Task<MetadataDocumentSaver.SaveResult> SaveMetadataAsync(string directoryPath, CancellationToken cancellationToken)
        {
            return await SaveMetadataAsync(directoryPath, MetadataDocumentSaver.DefaultNamingConvention, MetadataDocumentSaver.DefaultOverwrite, cancellationToken).ConfigureAwait(false);
        }

        public async Task<MetadataDocumentSaver.SaveResult> SaveMetadataAsync(string directoryPath, MetadataFileNamingConvention fileNamingConvention, bool overwrite, CancellationToken cancellationToken)
        {
            await ImportMetadataAsync(cancellationToken).ConfigureAwait(false);
            return await MetadataDocumentSaver.SaveMetadataAsync(directoryPath, this.metadataDocumentLoader.MetadataSections, fileNamingConvention, overwrite, cancellationToken).ConfigureAwait(false);
        }

        public bool ContainsHttpBindings()
        {
            CheckMetadataImported();
            return this.metadataDocumentLoader.MetadataSections.Select((s) => s.Metadata).OfType<WsdlNS.ServiceDescription>().Any((wsdl) => ContainsHttpBindings(wsdl));
        }

        private static bool ContainsHttpBindings(WsdlNS.ServiceDescription wsdl)
        {
            foreach (WsdlNS.Binding binding in wsdl.Bindings)
            {
                if (binding.Extensions.OfType<WsdlNS.HttpBinding>().Count() != 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void CheckMetadataImported()
        {
            if (!this.MetadataImported)
            {
                throw new InvalidOperationException(MetadataResources.ErrServiceMetadataNotImported);
            }
        }

        private static bool IsWsdlImporterQuotasSupported()
        {
            Type wsdlImporterType = typeof(WsdlImporter);
            Assembly smAssembly = wsdlImporterType.GetTypeInfo().Assembly;
            Type importerQuotasType = smAssembly.GetType("System.ServiceModel.Description.MetadataImporterQuotas", throwOnError: false, ignoreCase: true);
            if (importerQuotasType == null)
            {
                return false;
            }

            ConstructorInfo ctor = wsdlImporterType.GetConstructor(
                new Type[4]
                {
                    typeof(MetadataSet),
                    typeof(IEnumerable<IPolicyImportExtension>),
                    typeof(IEnumerable<IWsdlImportExtension>),
                    importerQuotasType
                }
            );

            return (ctor != null);
        }

        private async Task<WsdlImporter> CreateWsdlImporterAsync(bool useMessageFormat, CancellationToken cancellationToken)
        {
            Collection<IWsdlImportExtension> wsdlImportExtensions = LoadWsdlImportExtensions();
            Collection<IPolicyImportExtension> policyImportExtensions = LoadPolicyImportExtensions();

            var metadataSet = new MetadataSet(await AsyncHelper.RunAsync(() => GetImportableMetadataSections(), cancellationToken).ConfigureAwait(false));

            var wsdlImporter = IsWsdlImporterQuotasSupported() ?
                   new WsdlImporter(metadataSet, policyImportExtensions, wsdlImportExtensions, MetadataImporterQuotas.Max) :
                   new WsdlImporter(metadataSet, policyImportExtensions, wsdlImportExtensions);

            // Enable importing faults using  XmlSerializerSchemaImporter instead DataContractSerializerSchemaImporter.
            // This is the default behavior in Svcutil/ASR to support older web services.
            wsdlImporter.State.Add(typeof(FaultImportOptions), new FaultImportOptions() { UseMessageFormat = useMessageFormat });

            return wsdlImporter;
        }

        private static Collection<IWsdlImportExtension> LoadWsdlImportExtensions()
        {
            Collection<IWsdlImportExtension> extensions = new Collection<IWsdlImportExtension>
                {
                new System.ServiceModel.Description.DataContractSerializerMessageContractImporter(),
                new System.ServiceModel.Description.XmlSerializerMessageContractImporter(),
                new System.ServiceModel.Channels.MessageEncodingBindingElementImporter(),
                new System.ServiceModel.Channels.TransportBindingElementImporter(),
                new System.ServiceModel.Channels.StandardBindingImporter(),
                new System.ServiceModel.Channels.UdpTransportImporter(),
                new System.ServiceModel.Channels.ContextBindingElementImporter()
                };

            return extensions;
        }

        private static Collection<IPolicyImportExtension> LoadPolicyImportExtensions()
        {
            Collection<IPolicyImportExtension> extensions = new Collection<IPolicyImportExtension>()
                {
                    new System.ServiceModel.Channels.PrivacyNoticeBindingElementImporter(),
                    new System.ServiceModel.Channels.UseManagedPresentationBindingElementImporter(),
                    new System.ServiceModel.Channels.TransactionFlowBindingElementImporter(),
                    new System.ServiceModel.Channels.ReliableSessionBindingElementImporter(),
                    new System.ServiceModel.Channels.SecurityBindingElementImporter(),
                    new System.ServiceModel.Channels.CompositeDuplexBindingElementImporter(),
                    new System.ServiceModel.Channels.OneWayBindingElementImporter(),
                    new System.ServiceModel.Channels.MessageEncodingBindingElementImporter(),
                    new System.ServiceModel.Channels.TransportBindingElementImporter(),
                    new System.ServiceModel.Channels.UdpTransportImporter(),
                    new System.ServiceModel.Channels.ContextBindingElementImporter()
                };

            return extensions;
        }

        protected virtual IEnumerable<MetadataSection> GetImportableMetadataSections()
        {
#if NETCORE
            // The private Svcutil's RT code doesn't support XML serialization so the metadata cannot be cloned.
            // This is OK because Svcutil doesn't save the metadata into disk.
            return this.metadataDocumentLoader.MetadataSections;
#else
            // clone the wsdl documents as they may get modified by the WSDL importer (extensions). 
            // This is required only when the metadata is meant to be serialized (saved into files).
            return this.metadataDocumentLoader.MetadataSections.Select((section) =>
            {
                object metadata;
                var wsdl = section.Metadata as WsdlNS.ServiceDescription;
                if (wsdl != null)
                {
                    using (var stream = new System.IO.MemoryStream())
                    {
                        wsdl.Write(stream);
                        stream.Position = 0;

                        var wsdlClone = WsdlNS.ServiceDescription.Read(stream);
                        wsdlClone.RetrievalUrl = wsdl.RetrievalUrl;
                        metadata = wsdlClone;
                    }
                }
                else
                {
                    metadata = section.Metadata;
                }

                return new MetadataSection(section.Dialect, section.Identifier, metadata);
            });
#endif
        }

        public class ServiceInfo
        {
            public ServiceInfo(string name, IEnumerable<ContractDescription> contracts)
            {
                this.Name = name;
                this.Contracts = contracts;
            }

            public ServiceInfo(ContractDescription contract)
            {
                this.Name = contract.Name;
                this.Contracts = new List<ContractDescription> { contract };
                this.IsWrapper = true;
            }

            public string Name { get; private set; }

            // Service is used as a contract wrapper and is not a real service.
            public bool IsWrapper { get; private set; }

            public IEnumerable<ContractDescription> Contracts { get; private set; }
        }
    }
}
