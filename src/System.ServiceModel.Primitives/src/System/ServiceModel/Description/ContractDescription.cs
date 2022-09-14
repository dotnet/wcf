// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Security;
using System.ServiceModel.Security;

namespace System.ServiceModel.Description
{
    [DebuggerDisplay("Name={_name}, Namespace={_ns}, ContractType={ContractType}")]
    public class ContractDescription
    {
        private XmlName _name;
        private string _ns;
        private SessionMode _sessionMode;
        private ProtectionLevel _protectionLevel;

        public ContractDescription(string name)
            : this(name, null)
        {
        }

        public ContractDescription(string name, string ns)
        {
            // the property setter validates given value
            Name = name;
            if (!string.IsNullOrEmpty(ns))
            {
                NamingHelper.CheckUriParameter(ns, "ns");
            }

            Operations = new OperationDescriptionCollection();
            _ns = ns ?? NamingHelper.DefaultNamespace; // ns can be ""
        }

        internal string CodeName
        {
            get { return _name.DecodedName; }
        }

        [DefaultValue(null)]
        public string ConfigurationName { get; set; }

        public Type ContractType { get; set; }

        public Type CallbackContractType { get; set; }

        public string Name
        {
            get { return _name.EncodedName; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                if (value.Length == 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new ArgumentOutOfRangeException(nameof(value), SRP.SFxContractDescriptionNameCannotBeEmpty));
                }
                _name = new XmlName(value, true /*isEncoded*/);
            }
        }

        public string Namespace
        {
            get { return _ns; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    NamingHelper.CheckUriProperty(value, "Namespace");
                }

                _ns = value;
            }
        }

        public OperationDescriptionCollection Operations { get; }

        public ProtectionLevel ProtectionLevel
        {
            get { return _protectionLevel; }
            set
            {
                if (!ProtectionLevelHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }

                _protectionLevel = value;
                HasProtectionLevel = true;
            }
        }

        public bool ShouldSerializeProtectionLevel()
        {
            return HasProtectionLevel;
        }

        public bool HasProtectionLevel { get; private set; }

        [DefaultValue(SessionMode.Allowed)]
        public SessionMode SessionMode
        {
            get { return _sessionMode; }
            set
            {
                if (!SessionModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }

                _sessionMode = value;
            }
        }

        public KeyedCollection<Type, IContractBehavior> ContractBehaviors
        {
            get { return Behaviors; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public KeyedByTypeCollection<IContractBehavior> Behaviors { get; } = new KeyedByTypeCollection<IContractBehavior>();

        public static ContractDescription GetContract(Type contractType)
        {
            if (contractType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(contractType));
            }

            TypeLoader typeLoader = new TypeLoader();
            return typeLoader.LoadContractDescription(contractType);
        }

        public static ContractDescription GetContract(Type contractType, Type serviceType)
        {
            if (contractType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(contractType));
            }

            if (serviceType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(serviceType));
            }

            TypeLoader typeLoader = new TypeLoader();
            ContractDescription description = typeLoader.LoadContractDescription(contractType, serviceType);
            return description;
        }

        public static ContractDescription GetContract(Type contractType, object serviceImplementation)
        {
            if (contractType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(contractType));
            }

            if (serviceImplementation == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(serviceImplementation));
            }

            TypeLoader typeLoader = new TypeLoader();
            Type serviceType = serviceImplementation.GetType();
            ContractDescription description = typeLoader.LoadContractDescription(contractType, serviceType, serviceImplementation);
            return description;
        }

        public Collection<ContractDescription> GetInheritedContracts()
        {
            Collection<ContractDescription> result = new Collection<ContractDescription>();
            for (int i = 0; i < Operations.Count; i++)
            {
                OperationDescription od = Operations[i];
                if (od.DeclaringContract != this)
                {
                    ContractDescription inheritedContract = od.DeclaringContract;
                    if (!result.Contains(inheritedContract))
                    {
                        result.Add(inheritedContract);
                    }
                }
            }
            return result;
        }

        internal void EnsureInvariants()
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SRP.AChannelServiceEndpointSContractSNameIsNull0));
            }
            if (Namespace == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SRP.AChannelServiceEndpointSContractSNamespace0));
            }
            if (Operations.Count == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SRP.Format(SRP.SFxContractHasZeroOperations, Name)));
            }
            bool thereIsAtLeastOneInitiatingOperation = false;
            for (int i = 0; i < Operations.Count; i++)
            {
                OperationDescription operationDescription = Operations[i];
                operationDescription.EnsureInvariants();
                if (operationDescription.IsInitiating)
                {
                    thereIsAtLeastOneInitiatingOperation = true;
                }

                if ((!operationDescription.IsInitiating || operationDescription.IsTerminating)
                    && (SessionMode != SessionMode.Required))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        SRP.Format(SRP.ContractIsNotSelfConsistentItHasOneOrMore2, Name)));
                }
            }
            if (!thereIsAtLeastOneInitiatingOperation)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SRP.Format(SRP.SFxContractHasZeroInitiatingOperations, Name)));
            }
        }

        internal bool IsDuplex()
        {
            for (int i = 0; i < Operations.Count; ++i)
            {
                if (Operations[i].IsServerInitiated())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
