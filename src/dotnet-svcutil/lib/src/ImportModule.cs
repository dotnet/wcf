// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.CodeDom;
using Microsoft.Tools.ServiceModel.Svcutil.Metadata;
using Microsoft.Xml;
using Microsoft.Xml.Schema;
using Microsoft.Xml.Serialization;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

using DcNS = System.Runtime.Serialization;
using WsdlNS = System.Web.Services.Description;
namespace Microsoft.Tools.ServiceModel.Svcutil
{
    partial class ImportModule
    {
        readonly CodeCompileUnit codeCompileUnit;
        readonly WsdlImporter wsdlImporter;
        readonly ServiceContractGenerator contractGenerator;
        readonly WcfCodeGenerationExtension codegenExtension;

        CommandProcessorOptions options;

        internal CodeCompileUnit CodeCompileUnit
        {
            get
            {
                return codeCompileUnit;
            }
        }

        internal ImportModule(CommandProcessorOptions options, ServiceDescriptor serviceDescriptor, WsdlImporter importer)
        {
            this.codeCompileUnit = new CodeCompileUnit();
            this.options = options;
            this.codegenExtension = new WcfCodeGenerationExtension(options);

            this.wsdlImporter = importer;
            this.contractGenerator = InitializationHelper.CreateServiceContractGenerator(options, codeCompileUnit);
        }

        internal bool ImportServiceContracts(ServiceDescriptor serviceDescriptor)
        {
            bool result = false;

            try
            {
                codegenExtension.ClientGenerating(this.contractGenerator);
            }
            catch (Exception e)
            {
                ToolConsole.WriteError(e);
            }

            HttpBindingTracker httpBindingTracker = FindImportExtension<HttpBindingTracker>();

            foreach (ContractDescription contractDescription in serviceDescriptor.Contracts)
            {
                if (!httpBindingTracker.IsHttpBindingContract(contractDescription) || serviceDescriptor.Endpoints.Any(endpoint => endpoint.Contract == contractDescription))
                {
                    this.contractGenerator.GenerateServiceContractType(contractDescription);
                }
            }

            try
            {
                codegenExtension.ClientGenerated(this.contractGenerator);
                if (codegenExtension.ErrorDetected)
                {
                    ToolConsole.ExitCode = ToolExitCode.ValidationError;
                }

                result = !codegenExtension.ErrorDetected;
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (Exception e)
            {
                ToolConsole.WriteError(e);
            }

            return result;
        }

        T FindImportExtension<T>()
        {
            return wsdlImporter.WsdlImportExtensions.Find<T>();
        }

        int nonWsdlImportErrors;
        public void BeforeImportMetadata(ServiceDescriptor serviceDescriptor)
        {
            nonWsdlImportErrors = this.wsdlImporter.Errors.Count;

            this.wsdlImporter.WsdlImportExtensions.Add(new BindingImportTracker());
            this.wsdlImporter.WsdlImportExtensions.Add(new HttpBindingTracker());

            InitializationHelper.RemoveUnneededSerializers(options, serviceDescriptor, this.wsdlImporter.WsdlImportExtensions);
            InitializationHelper.ConfigureSerializers(options, codeCompileUnit, this.wsdlImporter);

            try
            {
                codegenExtension.WsdlImporting(this.wsdlImporter);
            }
            catch (Exception e)
            {
                ToolConsole.WriteError(e);
            }
        }

        bool validateMetadataImport = true;
        public bool AfterImportMetadata(ServiceDescriptor serviceDescriptor)
        {
            try
            {
                // Convert errors to warnings to workaround the issue that many validation errors from XSD compiler
                // can be ignored.
                for (int idx = this.wsdlImporter.Errors.Count - 1; idx >= nonWsdlImportErrors; idx--)
                {
                    var error = this.wsdlImporter.Errors[idx];
                    if (!error.IsWarning)
                    {
                        ToolConsole.ExitCode = ToolExitCode.ValidationErrorTurnedWarning;
                        var warning = new MetadataConversionError(error.Message, isWarning: true);
                        this.wsdlImporter.Errors[idx] = warning;
                    }
                }

                MarkupTelemetryHelper.SendBindingData(serviceDescriptor.Bindings);

                Collection<ServiceEndpoint> endpoints = new Collection<ServiceEndpoint>(serviceDescriptor.Endpoints.ToList());
                Collection<Binding> bindings = new Collection<Binding>(serviceDescriptor.Bindings.ToList());
                Collection<ContractDescription> contracts = new Collection<ContractDescription>(serviceDescriptor.Contracts.ToList());

                codegenExtension.WsdlImported(this.wsdlImporter, endpoints, bindings, contracts);
            }
            catch (Exception e)
            {
                ToolConsole.WriteError(e);
            }

            ToolConsole.WriteConversionErrors(this.wsdlImporter.Errors);

            ImportServiceContracts(serviceDescriptor);

            ToolConsole.WriteConversionErrors(this.contractGenerator.Errors);

            var contractsResolved = true;

            if (this.validateMetadataImport)
            {
                this.validateMetadataImport = false;
                contractsResolved = ContractsResolved(serviceDescriptor, this.CodeCompileUnit);

                if (!contractsResolved)
                {
                    var importer1 = ServiceDescriptor.DefaultUseMessageFormat ? typeof(DataContractSerializerMessageContractImporter) : typeof(XmlSerializerMessageContractImporter);
                    var importer2 = ServiceDescriptor.DefaultUseMessageFormat ? typeof(XmlSerializerMessageContractImporter) : typeof(DataContractSerializerMessageContractImporter);
                    ToolConsole.WriteWarning(string.Format(CultureInfo.CurrentCulture, SR.WrnCouldNotGenerateContractOperationsFormat, importer1, importer2));
                }
            }

            // on false, ServiceDescriptor will attempt to use a different contract serializer.
            return contractsResolved;
        }

        private static bool ContractsResolved(ServiceDescriptor serviceDescriptor, CodeCompileUnit codeCompileUnit)
        {
            if (serviceDescriptor != null && codeCompileUnit != null)
            {
                foreach (ContractDescription contract in serviceDescriptor.Contracts)
                {
                    foreach (OperationDescription operation in contract.Operations)
                    {
                        foreach (CodeNamespace ns in codeCompileUnit.Namespaces)
                        {
                            foreach (CodeTypeDeclaration type in ns.Types)
                            {
                                if (type.Name == contract.Name)
                                {
                                    if (type.Members.Count == 0)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        static class InitializationHelper
        {
            internal static ServiceContractGenerator CreateServiceContractGenerator(CommandProcessorOptions options, CodeCompileUnit codeCompileUnit)
            {
                ServiceContractGenerator contractGenerator = null;

                try
                {
                    contractGenerator = new ServiceContractGenerator(codeCompileUnit);
                }
                catch (Exception e)
                {
                    if (Utils.IsFatalOrUnexpected(e)) throw;
                    throw new ToolRuntimeException(SR.ErrUnableToLoadInputs, e);
                }
                SetContractGeneratorOptions(options, contractGenerator);

                foreach (KeyValuePair<string, string> namespaceMapping in options.NamespaceMappings)
                    contractGenerator.NamespaceMappings.Add(namespaceMapping.Key, namespaceMapping.Value);
                return contractGenerator;
            }

            static void SetContractGeneratorOptions(CommandProcessorOptions options, ServiceContractGenerator contractGenerator)
            {
                contractGenerator.Options |= ServiceContractGenerationOptions.AsynchronousMethods;
                contractGenerator.Options |= ServiceContractGenerationOptions.TaskBasedAsynchronousMethod;

                if (options.InternalTypeAccess == true)
                {
                    contractGenerator.Options |= ServiceContractGenerationOptions.InternalTypes;
                }
                else
                {
                    contractGenerator.Options &= ~ServiceContractGenerationOptions.InternalTypes;
                }

                if (options.MessageContract == true)
                {
                    contractGenerator.Options |= ServiceContractGenerationOptions.TypedMessages;
                }
                else
                {
                    contractGenerator.Options &= ~ServiceContractGenerationOptions.TypedMessages;
                }
            }

            public static void RemoveUnneededSerializers(CommandProcessorOptions options, ServiceDescriptor serviceDescriptor, Collection<IWsdlImportExtension> wsdlImportExtensions)
            {
                if ((options.SerializerMode == SerializerMode.Auto || options.SerializerMode == SerializerMode.Default) && serviceDescriptor.ContainsHttpBindings())
                {
                    // NOTE: HTTP Get/Post binding indicates an old web service. We use XmlSerializer to prevent generating dup classes.
                    // Please check devdiv bug 94078
                    options.SerializerMode = SerializerMode.XmlSerializer;
                }

                switch (options.SerializerMode)
                {
                    case SerializerMode.Default:
                    case SerializerMode.Auto:
                        break;
                    case SerializerMode.XmlSerializer:
                        RemoveExtension(typeof(DataContractSerializerMessageContractImporter), wsdlImportExtensions);
                        break;
                    case SerializerMode.DataContractSerializer:
                        RemoveExtension(typeof(XmlSerializerMessageContractImporter), wsdlImportExtensions);
                        break;
                    default:
                        Debug.Assert(false, "Unrecognized serializer option!");
                        break;
                }
            }

            public static void ConfigureSerializers(CommandProcessorOptions options, CodeCompileUnit codeCompileUnit, WsdlImporter importer)
            {
                switch (options.SerializerMode)
                {
                    case SerializerMode.Default:
                    case SerializerMode.Auto:
                        AddStateForDataContractSerializerImport(options, importer, codeCompileUnit);
                        AddStateForXmlSerializerImport(options, importer, codeCompileUnit);
                        break;
                    case SerializerMode.XmlSerializer:
                        AddStateForXmlSerializerImport(options, importer, codeCompileUnit);
                        break;
                    case SerializerMode.DataContractSerializer:
                        AddStateForDataContractSerializerImport(options, importer, codeCompileUnit);
                        break;
                    default:
                        Debug.Assert(false, "Unrecognized serializer option!");
                        break;
                }
            }

            internal static DcNS.XsdDataContractImporter CreateDCImporter(CommandProcessorOptions options, CodeCompileUnit codeCompileUnit)
            {
                DcNS.XsdDataContractImporter importer = new DcNS.XsdDataContractImporter(codeCompileUnit);
                DcNS.ImportOptions dcOptions = CreateDCImportOptions(options);
                importer.Options = dcOptions;
                return importer;
            }

            static DcNS.ImportOptions CreateDCImportOptions(CommandProcessorOptions options)
            {
                DcNS.ImportOptions dcOptions = new DcNS.ImportOptions
                {
                    GenerateInternal = options.InternalTypeAccess == true,
                    EnableDataBinding = options.EnableDataBinding == true
                };
                foreach (Type referencedType in options.ReferencedTypes)
                    dcOptions.ReferencedTypes.Add(referencedType);
                foreach (Type referencedCollectionType in options.ReferencedCollectionTypes)
                    dcOptions.ReferencedCollectionTypes.Add(referencedCollectionType);
                foreach (KeyValuePair<string, string> namespaceMapping in options.NamespaceMappings)
                {
                    Debug.Assert(!dcOptions.Namespaces.ContainsKey(namespaceMapping.Key), $"Key '{namespaceMapping.Key}' already added to dictionary!");
                    dcOptions.Namespaces[namespaceMapping.Key] = namespaceMapping.Value;
                }
                dcOptions.CodeProvider = options.CodeProvider;
                return dcOptions;
            }

            static void AddStateForXmlSerializerImport(CommandProcessorOptions options, WsdlImporter importer, CodeCompileUnit codeCompileUnit)
            {
                XmlSerializerImportOptions importOptions = new XmlSerializerImportOptions(codeCompileUnit)
                {
                    WebReferenceOptions = new WsdlNS.WebReferenceOptions()
                };
                importOptions.WebReferenceOptions.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties | CodeGenerationOptions.GenerateOrder;
                if (options.EnableDataBinding == true)
                    importOptions.WebReferenceOptions.CodeGenerationOptions |= CodeGenerationOptions.EnableDataBinding;

                // Right now System.Data API not available in DNX. If it comes available we could consider uncommenting these.
                // importOptions.WebReferenceOptions.SchemaImporterExtensions.Add(typeof(TypedDataSetSchemaImporterExtensionFx35).AssemblyQualifiedName);
                // importOptions.WebReferenceOptions.SchemaImporterExtensions.Add(typeof(DataSetSchemaImporterExtension).AssemblyQualifiedName);

                importOptions.CodeProvider = options.CodeProvider;

                importOptions.ClrNamespace = options.NamespaceMappings.FirstOrDefault(m => m.Key == "*").Value;

                importer.State.Add(typeof(XmlSerializerImportOptions), importOptions);

                if (!importer.State.ContainsKey(typeof(WrappedOptions)))
                    importer.State.Add(typeof(WrappedOptions), new WrappedOptions { WrappedFlag = options.Wrapped == true});

            }

            static void AddStateForDataContractSerializerImport(CommandProcessorOptions options, WsdlImporter importer, CodeCompileUnit codeCompileUnit)
            {
                DcNS.XsdDataContractImporter xsdDataContractImporter = CreateDCImporter(options, codeCompileUnit);
                importer.State.Add(typeof(DcNS.XsdDataContractImporter), xsdDataContractImporter);
                if (!importer.State.ContainsKey(typeof(WrappedOptions)))
                    importer.State.Add(typeof(WrappedOptions), new WrappedOptions { WrappedFlag = options.Wrapped == true});
            }

            static void RemoveExtension(Type extensionType, Collection<IWsdlImportExtension> wsdlImportExtensions)
            {
                for (int i = 0; i < wsdlImportExtensions.Count; i++)
                {
                    if (wsdlImportExtensions[i].GetType() == extensionType)
                        wsdlImportExtensions.RemoveAt(i);
                }
            }
        }

        class BindingImportTracker : IWsdlImportExtension
        {
            readonly Dictionary<Binding, ContractDescription> bindingContractMapping = new Dictionary<Binding, ContractDescription>();
            readonly Dictionary<Binding, XmlQualifiedName> bindingWsdlMapping = new Dictionary<Binding, XmlQualifiedName>();
            readonly Dictionary<XmlQualifiedName, WsdlNS.Port> bindingPortMapping = new Dictionary<XmlQualifiedName, WsdlNS.Port>();

            void IWsdlImportExtension.BeforeImport(WsdlNS.ServiceDescriptionCollection wsdlDocuments, XmlSchemaSet xmlSchemas, ICollection<XmlElement> policy) { }

            void IWsdlImportExtension.ImportContract(WsdlImporter importer, WsdlContractConversionContext context) { }

            void IWsdlImportExtension.ImportEndpoint(WsdlImporter importer, WsdlEndpointConversionContext context)
            {
                if (context != null && context.Endpoint != null && context.Endpoint.Binding != null)
                {
                    if (!bindingContractMapping.ContainsKey(context.Endpoint.Binding))
                    {
                        bindingContractMapping.Add(context.Endpoint.Binding, context.Endpoint.Contract);
                    }

                    XmlQualifiedName wsdlBindingQName = new XmlQualifiedName(context.WsdlBinding.Name, context.WsdlBinding.ServiceDescription.TargetNamespace);
                    bindingWsdlMapping[context.Endpoint.Binding] = wsdlBindingQName;

                    if (context.WsdlPort != null)
                    {
                        bindingPortMapping[wsdlBindingQName] = context.WsdlPort;
                    }
                }
            }
        }

        class HttpBindingTracker : IWsdlImportExtension
        {
            readonly HashSet<ContractDescription> httpBindingContracts = new HashSet<ContractDescription>();

            static bool ContainsHttpBindingExtension(WsdlNS.Binding wsdlBinding)
            {
                //avoiding using wsdlBinding.Extensions.Find(typeof(WsdlNS.HttpBinding)) so the extension won't be marked as handled
                foreach (object extension in wsdlBinding.Extensions)
                {
                    if (extension is WsdlNS.HttpBinding httpBinding)
                    {
                        string httpVerb = httpBinding.Verb;
                        if (httpVerb.Equals("GET", StringComparison.OrdinalIgnoreCase) || httpVerb.Equals("POST", StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public bool IsHttpBindingContract(ContractDescription contract)
            {
                return httpBindingContracts.Contains(contract);
            }

            public void BeforeImport(WsdlNS.ServiceDescriptionCollection wsdlDocuments, XmlSchemaSet xmlSchemas, ICollection<XmlElement> policy) { }
            public void ImportContract(WsdlImporter importer, WsdlContractConversionContext context) { }
            public void ImportEndpoint(WsdlImporter importer, WsdlEndpointConversionContext context)
            {
                if (context != null && ContainsHttpBindingExtension(context.WsdlBinding))
                {
                    httpBindingContracts.Add(context.ContractConversionContext.Contract);
                }
            }
        }
    }
}
