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
    using System.Xml;
    using System.Xml.Schema;
    using System.Collections.ObjectModel;
    using DcNS = System.Runtime.Serialization;

    class ExportModule
    {
        readonly DcNS.XsdDataContractExporter dcExporter;
        readonly bool dcOnlyMode;
        readonly string serviceName;
        readonly IsTypeExcludedDelegate isTypeExcluded;
        readonly TypeResolver typeResolver;
        
        internal delegate bool IsTypeExcludedDelegate(Type t);
        internal delegate void TypeLoadErrorEventHandler(Type type, string errorMessage);
        internal delegate void ServiceResolutionErrorEventHandler(string configName, string errorMessage);
        internal delegate void ConfigLoadErrorEventHandler(string fileName, string errorMessage);

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
                    ContractDescription description = ContractDescription.GetContract(type);
                    return description;
                }
#pragma warning suppress 56500 // covered by FxCOP
                catch (Exception e)
                {
                    if (Tool.IsFatal(e))
                        throw;

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
