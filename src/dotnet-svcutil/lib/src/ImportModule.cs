// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    internal partial class ImportModule
    {
        private readonly CodeCompileUnit _codeCompileUnit;
        private readonly WsdlImporter _wsdlImporter;
        private readonly ServiceContractGenerator _contractGenerator;
        private readonly WcfCodeGenerationExtension _codegenExtension;

        private CommandProcessorOptions _options;

        internal CodeCompileUnit CodeCompileUnit
        {
            get
            {
                return _codeCompileUnit;
            }
        }

        internal ImportModule(CommandProcessorOptions options, ServiceDescriptor serviceDescriptor, WsdlImporter importer)
        {
            _codeCompileUnit = new CodeCompileUnit();
            _options = options;
            _codegenExtension = new WcfCodeGenerationExtension(options);

            _wsdlImporter = importer;
            _contractGenerator = InitializationHelper.CreateServiceContractGenerator(options, _codeCompileUnit);
        }

        internal bool ImportServiceContracts(ServiceDescriptor serviceDescriptor)
        {
            bool result = false;

            try
            {
                _codegenExtension.ClientGenerating(_contractGenerator);
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
                    _contractGenerator.GenerateServiceContractType(contractDescription);
                }
            }

            try
            {
                _codegenExtension.ClientGenerated(_contractGenerator);
                if (_codegenExtension.ErrorDetected)
                {
                    ToolConsole.ExitCode = ToolExitCode.ValidationError;
                }

                result = !_codegenExtension.ErrorDetected;
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

        private T FindImportExtension<T>()
        {
            return _wsdlImporter.WsdlImportExtensions.Find<T>();
        }

        private int _nonWsdlImportErrors;
        public void BeforeImportMetadata(ServiceDescriptor serviceDescriptor)
        {
            _nonWsdlImportErrors = _wsdlImporter.Errors.Count;

            _wsdlImporter.WsdlImportExtensions.Add(new BindingImportTracker());
            _wsdlImporter.WsdlImportExtensions.Add(new HttpBindingTracker());

            InitializationHelper.RemoveUnneededSerializers(_options, serviceDescriptor, _wsdlImporter.WsdlImportExtensions);
            InitializationHelper.ConfigureSerializers(_options, _codeCompileUnit, _wsdlImporter);

            try
            {
                _codegenExtension.WsdlImporting(_wsdlImporter);
            }
            catch (Exception e)
            {
                ToolConsole.WriteError(e);
            }
        }

        private bool _validateMetadataImport = true;
        public bool AfterImportMetadata(ServiceDescriptor serviceDescriptor)
        {
            try
            {
                // Convert errors to warnings to workaround the issue that many validation errors from XSD compiler
                // can be ignored.
                for (int idx = _wsdlImporter.Errors.Count - 1; idx >= _nonWsdlImportErrors; idx--)
                {
                    var error = _wsdlImporter.Errors[idx];
                    if (!error.IsWarning)
                    {
                        ToolConsole.ExitCode = ToolExitCode.ValidationErrorTurnedWarning;
                        var warning = new MetadataConversionError(error.Message, isWarning: true);
                        _wsdlImporter.Errors[idx] = warning;
                    }
                }

                MarkupTelemetryHelper.SendBindingData(serviceDescriptor.Bindings);

                Collection<ServiceEndpoint> endpoints = new Collection<ServiceEndpoint>(serviceDescriptor.Endpoints.ToList());
                Collection<Binding> bindings = new Collection<Binding>(serviceDescriptor.Bindings.ToList());
                Collection<ContractDescription> contracts = new Collection<ContractDescription>(serviceDescriptor.Contracts.ToList());

                _codegenExtension.WsdlImported(_wsdlImporter, endpoints, bindings, contracts);
            }
            catch (Exception e)
            {
                ToolConsole.WriteError(e);
            }

            ToolConsole.WriteConversionErrors(_wsdlImporter.Errors);

            ImportServiceContracts(serviceDescriptor);

            ToolConsole.WriteConversionErrors(_contractGenerator.Errors);

            var contractsResolved = true;

            if (_validateMetadataImport)
            {
                _validateMetadataImport = false;
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

        private static class InitializationHelper
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

            private static void SetContractGeneratorOptions(CommandProcessorOptions options, ServiceContractGenerator contractGenerator)
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

                if (options.ServiceContract == true)
                {
                    contractGenerator.Options &= ~ServiceContractGenerationOptions.ChannelInterface & ~ServiceContractGenerationOptions.ClientClass;
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

            private static DcNS.ImportOptions CreateDCImportOptions(CommandProcessorOptions options)
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

            private static void AddStateForXmlSerializerImport(CommandProcessorOptions options, WsdlImporter importer, CodeCompileUnit codeCompileUnit)
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
                    importer.State.Add(typeof(WrappedOptions), new WrappedOptions { WrappedFlag = options.Wrapped == true });
            }

            private static void AddStateForDataContractSerializerImport(CommandProcessorOptions options, WsdlImporter importer, CodeCompileUnit codeCompileUnit)
            {
                DcNS.XsdDataContractImporter xsdDataContractImporter = CreateDCImporter(options, codeCompileUnit);
                importer.State.Add(typeof(DcNS.XsdDataContractImporter), xsdDataContractImporter);
                if (!importer.State.ContainsKey(typeof(WrappedOptions)))
                    importer.State.Add(typeof(WrappedOptions), new WrappedOptions { WrappedFlag = options.Wrapped == true });
            }

            private static void RemoveExtension(Type extensionType, Collection<IWsdlImportExtension> wsdlImportExtensions)
            {
                for (int i = 0; i < wsdlImportExtensions.Count; i++)
                {
                    if (wsdlImportExtensions[i].GetType() == extensionType)
                        wsdlImportExtensions.RemoveAt(i);
                }
            }
        }

        private class BindingImportTracker : IWsdlImportExtension
        {
            private readonly Dictionary<Binding, ContractDescription> _bindingContractMapping = new Dictionary<Binding, ContractDescription>();
            private readonly Dictionary<Binding, XmlQualifiedName> _bindingWsdlMapping = new Dictionary<Binding, XmlQualifiedName>();
            private readonly Dictionary<XmlQualifiedName, WsdlNS.Port> _bindingPortMapping = new Dictionary<XmlQualifiedName, WsdlNS.Port>();

            void IWsdlImportExtension.BeforeImport(WsdlNS.ServiceDescriptionCollection wsdlDocuments, XmlSchemaSet xmlSchemas, ICollection<XmlElement> policy) { }

            void IWsdlImportExtension.ImportContract(WsdlImporter importer, WsdlContractConversionContext context) { }

            void IWsdlImportExtension.ImportEndpoint(WsdlImporter importer, WsdlEndpointConversionContext context)
            {
                if (context != null && context.Endpoint != null && context.Endpoint.Binding != null)
                {
                    if (!_bindingContractMapping.ContainsKey(context.Endpoint.Binding))
                    {
                        _bindingContractMapping.Add(context.Endpoint.Binding, context.Endpoint.Contract);
                    }

                    XmlQualifiedName wsdlBindingQName = new XmlQualifiedName(context.WsdlBinding.Name, context.WsdlBinding.ServiceDescription.TargetNamespace);
                    _bindingWsdlMapping[context.Endpoint.Binding] = wsdlBindingQName;

                    if (context.WsdlPort != null)
                    {
                        _bindingPortMapping[wsdlBindingQName] = context.WsdlPort;
                    }
                }
            }
        }

        private class HttpBindingTracker : IWsdlImportExtension
        {
            private readonly HashSet<ContractDescription> _httpBindingContracts = new HashSet<ContractDescription>();

            private static bool ContainsHttpBindingExtension(WsdlNS.Binding wsdlBinding)
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
                return _httpBindingContracts.Contains(contract);
            }

            public void BeforeImport(WsdlNS.ServiceDescriptionCollection wsdlDocuments, XmlSchemaSet xmlSchemas, ICollection<XmlElement> policy) { }
            public void ImportContract(WsdlImporter importer, WsdlContractConversionContext context) { }
            public void ImportEndpoint(WsdlImporter importer, WsdlEndpointConversionContext context)
            {
                if (context != null && ContainsHttpBindingExtension(context.WsdlBinding))
                {
                    _httpBindingContracts.Add(context.ContractConversionContext.Contract);
                }
            }
        }
    }
}
