// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Net.Security;
using System.ServiceModel.Security;

namespace System.ServiceModel.Description
{
    [DebuggerDisplay("Name={_name}, Namespace={_ns}, ContractType={_contractType}")]
    public class ContractDescription
    {
        private Type _callbackContractType;
        private string _configurationName;
        private Type _contractType;
        private XmlName _name;
        private string _ns;
        private OperationDescriptionCollection _operations;
        private SessionMode _sessionMode;
        private KeyedByTypeCollection<IContractBehavior> _behaviors = new KeyedByTypeCollection<IContractBehavior>();
        private ProtectionLevel _protectionLevel;
        private bool _hasProtectionLevel;

        public ContractDescription(string name)
            : this(name, null)
        {
        }

        public ContractDescription(string name, string ns)
        {
            // the property setter validates given value
            this.Name = name;
            if (!string.IsNullOrEmpty(ns))
                NamingHelper.CheckUriParameter(ns, "ns");

            _operations = new OperationDescriptionCollection();
            _ns = ns ?? NamingHelper.DefaultNamespace; // ns can be ""
        }

        internal string CodeName
        {
            get { return _name.DecodedName; }
        }

        [DefaultValue(null)]
        public string ConfigurationName
        {
            get { return _configurationName; }
            set { _configurationName = value; }
        }

        public Type ContractType
        {
            get { return _contractType; }
            set { _contractType = value; }
        }

        public Type CallbackContractType
        {
            get { return _callbackContractType; }
            set { _callbackContractType = value; }
        }

        public string Name
        {
            get { return _name.EncodedName; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }

                if (value.Length == 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new ArgumentOutOfRangeException("value", SRServiceModel.SFxContractDescriptionNameCannotBeEmpty));
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
                    NamingHelper.CheckUriProperty(value, "Namespace");
                _ns = value;
            }
        }

        public OperationDescriptionCollection Operations
        {
            get { return _operations; }
        }

        public ProtectionLevel ProtectionLevel
        {
            get { return _protectionLevel; }
            set
            {
                if (!ProtectionLevelHelper.IsDefined(value))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                _protectionLevel = value;
                _hasProtectionLevel = true;
            }
        }

        public bool ShouldSerializeProtectionLevel()
        {
            return this.HasProtectionLevel;
        }

        public bool HasProtectionLevel
        {
            get { return _hasProtectionLevel; }
        }

        [DefaultValue(SessionMode.Allowed)]
        public SessionMode SessionMode
        {
            get { return _sessionMode; }
            set
            {
                if (!SessionModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }

                _sessionMode = value;
            }
        }

        public KeyedCollection<Type, IContractBehavior> ContractBehaviors
        {
            get { return this.Behaviors; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public KeyedByTypeCollection<IContractBehavior> Behaviors
        {
            get { return _behaviors; }
        }

        public static ContractDescription GetContract(Type contractType)
        {
            if (contractType == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contractType");

            TypeLoader typeLoader = new TypeLoader();
            return typeLoader.LoadContractDescription(contractType);
        }

        public static ContractDescription GetContract(Type contractType, Type serviceType)
        {
            if (contractType == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contractType");

            if (serviceType == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serviceType");

            TypeLoader typeLoader = new TypeLoader();
            ContractDescription description = typeLoader.LoadContractDescription(contractType, serviceType);
            return description;
        }

        public static ContractDescription GetContract(Type contractType, object serviceImplementation)
        {
            if (contractType == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contractType");

            if (serviceImplementation == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serviceImplementation");

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
            if (string.IsNullOrEmpty(this.Name))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SRServiceModel.AChannelServiceEndpointSContractSNameIsNull0));
            }
            if (this.Namespace == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SRServiceModel.AChannelServiceEndpointSContractSNamespace0));
            }
            if (this.Operations.Count == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    string.Format(SRServiceModel.SFxContractHasZeroOperations, this.Name)));
            }
            bool thereIsAtLeastOneInitiatingOperation = false;
            for (int i = 0; i < this.Operations.Count; i++)
            {
                OperationDescription operationDescription = this.Operations[i];
                operationDescription.EnsureInvariants();
                if (operationDescription.IsInitiating)
                    thereIsAtLeastOneInitiatingOperation = true;
                if ((!operationDescription.IsInitiating)
                    && (this.SessionMode != SessionMode.Required))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        string.Format(SRServiceModel.ContractIsNotSelfConsistentItHasOneOrMore2, this.Name)));
                }
            }
            if (!thereIsAtLeastOneInitiatingOperation)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    string.Format(SRServiceModel.SFxContractHasZeroInitiatingOperations, this.Name)));
            }
        }

        internal bool IsDuplex()
        {
            for (int i = 0; i < _operations.Count; ++i)
            {
                if (_operations[i].IsServerInitiated())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
