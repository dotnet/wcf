//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reflection;
    using System.ServiceModel;
    //using System.ServiceModel.Configuration;
    using System.Xml;
    using System.Xml.Schema;
    using System.Collections.ObjectModel;
    using DcNS = System.Runtime.Serialization;
   // using WsdlNS = System.Web.Services.Description;

    class ExportModule
    {
        //readonly WsdlExporter wsdlExporter;
        //readonly DcNS.XsdDataContractExporter dcExporter;
        readonly bool dcOnlyMode;
        readonly string serviceName;
        readonly IsTypeExcludedDelegate isTypeExcluded;
        //readonly TypeResolver typeResolver;

        //readonly Collection<MetadataConversionError> dcExporterErrors;
        internal delegate bool IsTypeExcludedDelegate(Type t);
        internal delegate void TypeLoadErrorEventHandler(Type type, string errorMessage);
        internal delegate void ServiceResolutionErrorEventHandler(string configName, string errorMessage);
        internal delegate void ConfigLoadErrorEventHandler(string fileName, string errorMessage);

        internal ExportModule(Options options)
        {

            switch (options.GetToolMode())
            {
                case ToolMode.MetadataFromAssembly:
                //case ToolMode.Validate:
                //    this.wsdlExporter = new WsdlExporter();
                //    this.dcOnlyMode = false;
                //    this.serviceName = options.ServiceName;
                //    this.isTypeExcluded = options.IsTypeExcluded;
                //    this.typeResolver = options.TypeResolver;
                //    break;
                //case ToolMode.DataContractExport:
                //    this.dcExporterErrors = new Collection<MetadataConversionError>();
                //    XmlSchemaSet schemaSet = new XmlSchemaSet();
                //    schemaSet.XmlResolver = null;
                //    schemaSet.ValidationEventHandler += new ValidationEventHandler(delegate (object sender, ValidationEventArgs args)
                //    {
                //        HandleSchemaValidationError(sender, args, dcExporterErrors, SR.ErrSchemaValidationForExport);
                //    });
                //    this.dcExporter = new DcNS.XsdDataContractExporter(schemaSet);
                //    this.dcOnlyMode = true;
                //    break;
                default:
                    //Tool.Assert(false, "ExportModule does not recognize this tool mode");
                    return;
            }
        }

        internal bool MetadataWasGenerated
        {
            get
            {
                //foreach (MetadataSection doc in this.GetGeneratedMetadata())
                //{
                //    return true;
                //}
                return false;
            }
        }

//        internal IEnumerable<MetadataSection> GetGeneratedMetadata()
//        {
//            if (dcOnlyMode)
//            {
//                return GetGeneratedSchemas();
//            }
//            else
//            {
//                return wsdlExporter.GetGeneratedMetadata().MetadataSections;
//            }
//        }

//        IEnumerable<MetadataSection> GetGeneratedSchemas()
//        {
//            // Note: call to XsdDataContractExporter.Schemas causes schemas to be compiled
//            XmlSchemaSet generatedSchemas = dcExporter.Schemas;
//            ToolConsole.WriteConversionErrors(this.dcExporterErrors);
//            foreach (XmlSchema schema in generatedSchemas.Schemas())
//            {
//                yield return MetadataSection.CreateFromSchema(schema);
//            }
//        }

//        internal ToolExitCodes ValidateTypes(List<Assembly> assemblies)
//        {
//            return ValidateService(assemblies);
//        }

//        ToolExitCodes ValidateService(List<Assembly> assemblies)
//        {
//            ToolExitCodes exitCode = ToolExitCodes.Success;
//            ServiceLoader serviceLoader = new ServiceLoader(assemblies, this.isTypeExcluded, typeResolver);

//            serviceLoader.ConfigLoadErrorCallback = delegate (string fileName, string errorMessage)
//            {
//                ToolConsole.WriteWarning(SR.GetString(SR.WrnCannotLoadConfigFileForValidation, errorMessage));
//            };
//            serviceLoader.ServiceLoadFailureCallback = delegate (Type type, string errorMessage)
//            {
//                ToolConsole.WriteWarning(SR.GetString(SR.WrnCannotLoadServiceForValidation, type.AssemblyQualifiedName, errorMessage));
//                exitCode = ToolExitCodes.RuntimeError;
//            };
//            serviceLoader.ServiceResolutionFailureCallback = delegate (string configName, string errorMessage)
//            {
//                ToolConsole.WriteWarning(SR.GetString(SR.WrnCannotResolveServiceForValidation, configName, errorMessage));
//                exitCode = ToolExitCodes.RuntimeError;
//            };

//            try
//            {
//                SvcutilServiceHost serviceHost = serviceLoader.LoadServiceDescription(this.serviceName);

//                if (serviceHost != null)
//                    serviceHost.Validate();
//            }
//#pragma warning suppress 56500 // covered by FxCOP
//            catch (Exception e)
//            {
//                if (Tool.IsFatal(e))
//                    throw;

//                ToolConsole.WriteValidationError(e);
//                return ToolExitCodes.ValidationError;
//            }
//            ToolConsole.WriteLine(SR.GetString(SR.ValidationWasSuccessful, this.serviceName));
//            return exitCode;
//        }

//        internal ToolExitCodes ExportMetadata(List<Assembly> assemblies)
//        {
//            if (this.serviceName != null)
//            {
//                return ExportService(assemblies);
//            }
//            else
//            {
//                return ExportServiceContracts(assemblies);
//            }
//        }

//        ToolExitCodes ExportService(List<Assembly> assemblies)
//        {
//            ToolExitCodes exitCode = ToolExitCodes.Success;
//            ServiceLoader serviceLoader = new ServiceLoader(assemblies, this.isTypeExcluded, typeResolver);

//            serviceLoader.ConfigLoadErrorCallback = delegate (string fileName, string errorMessage)
//            {
//                ToolConsole.WriteWarning(SR.GetString(SR.WrnCannotLoadConfigFileForExport, errorMessage));
//            };
//            serviceLoader.ServiceLoadFailureCallback = delegate (Type type, string errorMessage)
//            {
//                ToolConsole.WriteWarning(SR.GetString(SR.WrnCannotLoadServiceForExport, type.AssemblyQualifiedName, errorMessage));
//                exitCode = ToolExitCodes.RuntimeError;
//            };
//            serviceLoader.ServiceResolutionFailureCallback = delegate (string configName, string errorMessage)
//            {
//                ToolConsole.WriteWarning(SR.GetString(SR.WrnCannotResolveServiceForExport, configName, errorMessage));
//                exitCode = ToolExitCodes.RuntimeError;
//            };

//            SvcutilServiceHost serviceHost = serviceLoader.LoadServiceDescription(this.serviceName);

//            if (serviceHost != null)
//            {
//                if (serviceHost.Description != null)
//                {
//                    ServiceMetadataBehavior serviceMetadataBehavior = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
//                    if (serviceMetadataBehavior != null)
//                    {
//                        this.wsdlExporter.PolicyVersion = serviceMetadataBehavior.MetadataExporter.PolicyVersion;
//                    }
//                }
//                ExportServiceDescription(serviceHost.Description);
//            }

//            ToolConsole.WriteConversionErrors(this.wsdlExporter.Errors);

//            return exitCode;
//        }

//        ToolExitCodes ExportServiceContracts(List<Assembly> assemblies)
//        {
//            ContractLoader contractLoader = new ContractLoader(assemblies, this.isTypeExcluded);
//            contractLoader.ContractLoadErrorCallback = delegate (Type type, string errorMessage)
//            {
//                ToolConsole.WriteWarning(SR.GetString(SR.WrnUnableToLoadContractForExport, type, errorMessage));
//            };

//            foreach (ContractDescription contract in contractLoader.GetContracts())
//            {
//                ExportServiceContract(contract);
//            }
//            ToolConsole.WriteConversionErrors(this.wsdlExporter.Errors);

//            return ToolExitCodes.Success;
//        }


//        void ExportServiceContract(ContractDescription description)
//        {
//            try
//            {
//                ServiceEndpoint endpoint = new ServiceEndpoint(description);

//                endpoint.Binding = new BasicHttpBinding();
//                endpoint.Binding.Name = "DefaultBinding";
//                endpoint.Binding.Namespace = description.Namespace;

//                wsdlExporter.ExportEndpoint(endpoint);
//            }
//#pragma warning suppress 56500 // covered by FxCOP
//            catch (Exception e)
//            {
//                if (Tool.IsFatal(e))
//                    throw;

//                throw new ToolRuntimeException(SR.GetString(SR.ErrUnableToExportContract, description.ContractType.AssemblyQualifiedName), e);
//            }
//        }

//        void ExportServiceDescription(ServiceDescription description)
//        {
//            try
//            {
//                XmlQualifiedName serviceQName = new XmlQualifiedName(XmlConvert.EncodeLocalName(description.Name), description.Namespace);
//                wsdlExporter.ExportEndpoints(description.Endpoints, serviceQName);
//            }
//#pragma warning suppress 56500 // covered by FxCOP
//            catch (Exception e)
//            {
//                if (Tool.IsFatal(e))
//                    throw;

//                throw new ToolRuntimeException(SR.GetString(SR.ErrUnableToExportEndpoints, description.ServiceType.FullName), e);
//            }
//        }

//        internal void ExportDataContracts(List<Assembly> assemblies)
//        {
//            foreach (Assembly assembly in assemblies)
//            {
//                InputModule.LoadTypes(assembly);
//            }
//            this.dcExporter.Export(assemblies);
//        }

//        internal static void HandleSchemaValidationError(object sender, ValidationEventArgs args, Collection<MetadataConversionError> errors, string validationErrorMessage)
//        {
//            MetadataConversionError warning = null;

//            if (args.Exception != null && args.Exception.SourceUri != null)
//            {
//                XmlSchemaException ex = args.Exception;
//                warning = new MetadataConversionError(SR.GetString(validationErrorMessage, ex.SourceUri, ex.LineNumber, ex.LinePosition, ex.Message));
//            }
//            else
//            {
//                warning = new MetadataConversionError(SR.GetString(SR.ErrGeneralSchemaValidation, args.Message));
//            }

//            if (!errors.Contains(warning))
//                errors.Add(warning);
//        }

        internal class ContractLoader
        {
            readonly List<Type> types = new List<Type>();
            readonly IsTypeExcludedDelegate isTypeExcluded;
            TypeLoadErrorEventHandler contractLoadErrorCallback;

            internal ContractLoader(IEnumerable<Assembly> assemblies, IsTypeExcludedDelegate isTypeExcluded)
            {
                this.isTypeExcluded = isTypeExcluded;
                foreach (Assembly assembly in assemblies)
                    types.AddRange(InputModule.LoadTypes(assembly));
            }

            internal TypeLoadErrorEventHandler ContractLoadErrorCallback
            {
                get { return contractLoadErrorCallback; }
                set { contractLoadErrorCallback = value; }
            }

            internal IEnumerable<ContractDescription> GetContracts()
            {
                foreach (Type type in this.types)
                {
                    if (!isTypeExcluded(type) && IsContractType(type))
                    {
                        ContractDescription contract = LoadContract(type);
                        if (contract != null)
                            yield return contract;
                    }
                }
            }

            ContractDescription LoadContract(Type type)
            {
                try
                {
                    //ContractDescription description = ContractDescription.GetContract(type);
                    MethodInfo md = typeof(ContractDescription).GetMethod("GetContract", new Type[] { typeof(Type) });
                    ContractDescription description = (ContractDescription) md.Invoke(null, new object[] { type });
                    return description;
                }
#pragma warning suppress 56500 // covered by FxCOP
                catch (Exception e)
                {
                    //if (Tool.IsFatal(e))
                    //    throw;

                    if (this.contractLoadErrorCallback != null)
                        this.contractLoadErrorCallback(type, e.Message);

                    return null;
                }
            }

            static bool IsContractType(Type type)
            {
                return (type.IsInterface || type.IsClass) && (type.IsDefined(typeof(ServiceContractAttribute), false));
            }
        }

    }
}
