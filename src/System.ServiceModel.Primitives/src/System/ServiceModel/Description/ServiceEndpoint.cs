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
    [DebuggerDisplay("Address={Address}")]
    [DebuggerDisplay("Name={_name}")]
    public class ServiceEndpoint
    {
        private ContractDescription _contract;
        private Uri _listenUri;
        private ListenUriMode _listenUriMode = ListenUriMode.Explicit;
        private KeyedByTypeCollection<IEndpointBehavior> _behaviors;
        private string _id;
        private XmlName _name;

        public ServiceEndpoint(ContractDescription contract)
        {
            _contract = contract ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(contract));
        }

        public ServiceEndpoint(ContractDescription contract, Binding binding, EndpointAddress address)
        {
            _contract = contract ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(contract));
            Binding = binding;
            Address = address;
        }

        public EndpointAddress Address { get; set; }

        public KeyedCollection<Type, IEndpointBehavior> EndpointBehaviors
        {
            get { return Behaviors; }
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

        public Binding Binding { get; set; }

        public ContractDescription Contract
        {
            get { return _contract; }
            set
            {
                _contract = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
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
                else if (Binding != null)
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
                    if (Address == null)
                    {
                        return null;
                    }
                    else
                    {
                        return Address.Uri;
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", SRP.UriMustBeAbsolute);
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }
                _listenUriMode = value;
            }
        }

        internal string Id
        {
            get
            {
                if (_id == null)
                {
                    _id = Guid.NewGuid().ToString();
                }

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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.AChannelServiceEndpointSBindingIsNull0));
            }
            if (Contract == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.AChannelServiceEndpointSContractIsNull0));
            }
            Contract.EnsureInvariants();
            Binding.EnsureInvariants(Contract.Name);
        }

        internal void ValidateForClient()
        {
            Validate(true, false);
        }

        internal void ValidateForService(bool runOperationValidators)
        {
            Validate(runOperationValidators, true);
        }

        internal bool IsFullyConfigured { get; set; } = false;

        // This method runs validators (both builtin and ones in description).  
        // Precondition: EnsureInvariants() should already have been called.
        private void Validate(bool runOperationValidators, bool isForService)
        {
            // contract behaviors
            ContractDescription contract = Contract;
            for (int j = 0; j < contract.Behaviors.Count; j++)
            {
                IContractBehavior iContractBehavior = contract.Behaviors[j];
                iContractBehavior.Validate(contract, this);
            }
            // endpoint behaviors
            if (!isForService)
            {
            }
            for (int j = 0; j < Behaviors.Count; j++)
            {
                IEndpointBehavior ieb = Behaviors[j];
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
