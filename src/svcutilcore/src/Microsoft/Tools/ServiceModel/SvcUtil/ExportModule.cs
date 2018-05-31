// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.SvcUtil.XmlSerializer
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

    internal class ExportModule
    {
        private readonly DcNS.XsdDataContractExporter _dcExporter;
        private readonly bool _dcOnlyMode;
        private readonly string _serviceName;
        private readonly IsTypeExcludedDelegate _isTypeExcluded;
        private readonly TypeResolver _typeResolver;

        internal delegate bool IsTypeExcludedDelegate(Type t);
        internal delegate void TypeLoadErrorEventHandler(Type type, string errorMessage);
        internal delegate void ServiceResolutionErrorEventHandler(string configName, string errorMessage);
        internal delegate void ConfigLoadErrorEventHandler(string fileName, string errorMessage);

        internal class ContractLoader
        {
            private readonly List<Type> _types = new List<Type>();
            private readonly IsTypeExcludedDelegate _isTypeExcluded;
            private TypeLoadErrorEventHandler _contractLoadErrorCallback;

            internal ContractLoader(IEnumerable<Assembly> assemblies, IsTypeExcludedDelegate isTypeExcluded)
            {
                _isTypeExcluded = isTypeExcluded;
                foreach (Assembly assembly in assemblies)
                    _types.AddRange(InputModule.LoadTypes(assembly));
            }

            internal TypeLoadErrorEventHandler ContractLoadErrorCallback
            {
                get { return _contractLoadErrorCallback; }
                set { _contractLoadErrorCallback = value; }
            }

            internal IEnumerable<ContractDescription> GetContracts()
            {
                foreach (Type type in _types)
                {
                    if (!_isTypeExcluded(type) && IsContractType(type))
                    {
                        ContractDescription contract = LoadContract(type);
                        if (contract != null)
                            yield return contract;
                    }
                }
            }

            private ContractDescription LoadContract(Type type)
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

                    if (_contractLoadErrorCallback != null)
                        _contractLoadErrorCallback(type, e.Message);

                    return null;
                }
            }

            private static bool IsContractType(Type type)
            {
                return (type.IsInterface || type.IsClass) && (type.IsDefined(typeof(ServiceContractAttribute), false));
            }
        }
    }
}
