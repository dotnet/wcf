// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Description
{
    [DebuggerDisplay("Address={_address}")]
    [DebuggerDisplay("Name={_name}")]
    public class ServiceEndpoint
    {
        private EndpointAddress _address;
        private Binding _binding;
        private ContractDescription _contract;
        private Uri _listenUri;
        private ListenUriMode _listenUriMode = ListenUriMode.Explicit;
        private KeyedByTypeCollection<IEndpointBehavior> _behaviors;
        private string _id;
        private XmlName _name;
        private bool _isEndpointFullyConfigured = false;

        public ServiceEndpoint(ContractDescription contract)
        {
            if (contract == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contract");
            _contract = contract;
        }

        public ServiceEndpoint(ContractDescription contract, Binding binding, EndpointAddress address)
        {
            if (contract == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contract");

            _contract = contract;
            _binding = binding;
            _address = address;
        }

        public EndpointAddress Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public KeyedCollection<Type, IEndpointBehavior> EndpointBehaviors
        {
            get { return this.Behaviors; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public KeyedByTypeCollection<IEndpointBehavior> Behaviors
        {
            get
            {
                if (_behaviors == null)
                {
                    _behaviors = new KeyedByTypeCollection<IEndpointBehavior>();
                }

                return _behaviors;
            }
        }

        public Binding Binding
        {
            get { return _binding; }
            set { _binding = value; }
        }

        public ContractDescription Contract
        {
            get { return _contract; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                _contract = value;
            }
        }

        public bool IsSystemEndpoint
        {
            get;
            set;
        }

        public string Name
        {
            get
            {
                if (!XmlName.IsNullOrEmpty(_name))
                {
                    return _name.EncodedName;
                }
                else if (_binding != null)
                {
                    return String.Format(CultureInfo.InvariantCulture, "{0}_{1}", new XmlName(Binding.Name).EncodedName, Contract.Name);
                }
                else
                {
                    return Contract.Name;
                }
            }
            set
            {
                _name = new XmlName(value, true /*isEncoded*/);
            }
        }

        public Uri ListenUri
        {
            get
            {
                if (_listenUri == null)
                {
                    if (_address == null)
                    {
                        return null;
                    }
                    else
                    {
                        return _address.Uri;
                    }
                }
                else
                {
                    return _listenUri;
                }
            }
            set
            {
                if (value != null && !value.IsAbsoluteUri)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", SRServiceModel.UriMustBeAbsolute);
                }
                _listenUri = value;
            }
        }

        public ListenUriMode ListenUriMode
        {
            get { return _listenUriMode; }
            set
            {
                if (!ListenUriModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                _listenUriMode = value;
            }
        }

        internal string Id
        {
            get
            {
                if (_id == null)
                    _id = Guid.NewGuid().ToString();
                return _id;
            }
        }

        internal Uri UnresolvedAddress
        {
            get;
            set;
        }

        internal Uri UnresolvedListenUri
        {
            get;
            set;
        }

        // This method ensures that the description object graph is structurally sound and that none
        // of the fundamental SFx framework assumptions have been violated.
        internal void EnsureInvariants()
        {
            if (Binding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.AChannelServiceEndpointSBindingIsNull0));
            }
            if (Contract == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.AChannelServiceEndpointSContractIsNull0));
            }
            this.Contract.EnsureInvariants();
            this.Binding.EnsureInvariants(this.Contract.Name);
        }

        internal void ValidateForClient()
        {
            Validate(true, false);
        }

        internal void ValidateForService(bool runOperationValidators)
        {
            Validate(runOperationValidators, true);
        }

        internal bool IsFullyConfigured
        {
            get { return _isEndpointFullyConfigured; }
            set { _isEndpointFullyConfigured = value; }
        }

        // This method runs validators (both builtin and ones in description).  
        // Precondition: EnsureInvariants() should already have been called.
        private void Validate(bool runOperationValidators, bool isForService)
        {
            // contract behaviors
            ContractDescription contract = this.Contract;
            for (int j = 0; j < contract.Behaviors.Count; j++)
            {
                IContractBehavior iContractBehavior = contract.Behaviors[j];
                iContractBehavior.Validate(contract, this);
            }
            // endpoint behaviors
            if (!isForService)
            {
            }
            for (int j = 0; j < this.Behaviors.Count; j++)
            {
                IEndpointBehavior ieb = this.Behaviors[j];
                ieb.Validate(this);
            }
            // operation behaviors
            if (runOperationValidators)
            {
                for (int j = 0; j < contract.Operations.Count; j++)
                {
                    OperationDescription op = contract.Operations[j];


                    for (int k = 0; k < op.Behaviors.Count; k++)
                    {
                        IOperationBehavior iob = op.Behaviors[k];
                        iob.Validate(op);
                    }
                }
            }
        }
    }
}
