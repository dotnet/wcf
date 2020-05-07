// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel.Description;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Security;
    using System.ComponentModel;
    //using System.ServiceModel.Transactions;
    using Microsoft.Xml;

    public sealed class TransactionFlowBindingElement : BindingElement
    {
        private bool _transactions;
        private TransactionFlowOption _issuedTokens;
        private TransactionProtocol _transactionProtocol;

        public TransactionFlowBindingElement()
            : this(true, TransactionFlowDefaults.TransactionProtocol)
        {
        }

        public TransactionFlowBindingElement(TransactionProtocol transactionProtocol)
            : this(true, transactionProtocol)
        {
        }

        internal TransactionFlowBindingElement(bool transactions)
            : this(transactions, TransactionFlowDefaults.TransactionProtocol)
        {
        }

        internal TransactionFlowBindingElement(bool transactions, TransactionProtocol transactionProtocol)
        {
            _transactions = transactions;
            _issuedTokens = transactions ? TransactionFlowOption.Allowed : TransactionFlowOption.NotAllowed;

            if (!TransactionProtocol.IsDefined(transactionProtocol))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(string.Format(SRServiceModel.ConfigInvalidTransactionFlowProtocolValue, transactionProtocol.ToString()));
            }

            _transactionProtocol = transactionProtocol;
        }

        private TransactionFlowBindingElement(TransactionFlowBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _transactions = elementToBeCloned._transactions;
            _issuedTokens = elementToBeCloned._issuedTokens;

            if (!TransactionProtocol.IsDefined(elementToBeCloned._transactionProtocol))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(string.Format(SRServiceModel.ConfigInvalidTransactionFlowProtocolValue, elementToBeCloned._transactionProtocol.ToString()));
            }

            _transactionProtocol = elementToBeCloned._transactionProtocol;
            this.AllowWildcardAction = elementToBeCloned.AllowWildcardAction;
        }

        internal bool Transactions
        {
            get
            {
                return _transactions;
            }
            set
            {
                _transactions = value;
                _issuedTokens = value ? TransactionFlowOption.Allowed : TransactionFlowOption.NotAllowed;
            }
        }

        internal TransactionFlowOption IssuedTokens
        {
            get
            {
                return _issuedTokens;
            }
            set
            {
                ValidateOption(value);
                _issuedTokens = value;
            }
        }

        public override BindingElement Clone()
        {
            return new TransactionFlowBindingElement(this);
        }

        private bool IsFlowEnabled(Dictionary<DirectionalAction, TransactionFlowOption> dictionary)
        {
            if (_issuedTokens != TransactionFlowOption.NotAllowed)
            {
                return true;
            }

            if (!_transactions)
            {
                return false;
            }

            foreach (TransactionFlowOption option in dictionary.Values)
            {
                if (option != TransactionFlowOption.NotAllowed)
                {
                    return true;
                }
            }

            return false;
        }

        internal bool IsFlowEnabled(ContractDescription contract)
        {
            if (_issuedTokens != TransactionFlowOption.NotAllowed)
            {
                return true;
            }

            if (!_transactions)
            {
                return false;
            }

            foreach (OperationDescription operation in contract.Operations)
            {
                TransactionFlowAttribute parameter = operation.Behaviors.Find<TransactionFlowAttribute>();
                if (parameter != null)
                {
                    if (parameter.Transactions != TransactionFlowOption.NotAllowed)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public TransactionProtocol TransactionProtocol
        {
            get
            {
                return _transactionProtocol;
            }
            set
            {
                if (!TransactionProtocol.IsDefined(value))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                _transactionProtocol = value;
            }
        }

        [DefaultValue(false)]
        public bool AllowWildcardAction
        {
            get;
            set;
        }

        internal static void ValidateOption(TransactionFlowOption opt)
        {
            if (!TransactionFlowOptionHelper.IsDefined(opt))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(string.Format(SRServiceModel.TransactionFlowBadOption)));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeTransactionProtocol()
        {
            return this.TransactionProtocol != TransactionProtocol.Default;
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("context"));
            }

            if (typeof(TChannel) == typeof(IOutputChannel)
                || typeof(TChannel) == typeof(IDuplexChannel)
                || typeof(TChannel) == typeof(IRequestChannel)
                || typeof(TChannel) == typeof(IOutputSessionChannel)
                || typeof(TChannel) == typeof(IRequestSessionChannel)
                || typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return context.CanBuildInnerChannelFactory<TChannel>();
            }

            return false;
        }

        private Dictionary<DirectionalAction, TransactionFlowOption> GetDictionary(BindingContext context)
        {
            Dictionary<DirectionalAction, TransactionFlowOption> dictionary =
                context.BindingParameters.Find<Dictionary<DirectionalAction, TransactionFlowOption>>();
            if (dictionary == null)
                dictionary = new Dictionary<DirectionalAction, TransactionFlowOption>();
            return dictionary;
        }

        internal static MessagePartSpecification GetIssuedTokenHeaderSpecification(SecurityStandardsManager standardsManager)
        {
            MessagePartSpecification result;

            if (standardsManager.TrustDriver.IsIssuedTokensSupported)
                result = new MessagePartSpecification(new XmlQualifiedName(standardsManager.TrustDriver.IssuedTokensHeaderName, standardsManager.TrustDriver.IssuedTokensHeaderNamespace));
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.TrustDriverVersionDoesNotSupportIssuedTokens)));
            }

            return result;
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(ChannelProtectionRequirements))
            {
                ChannelProtectionRequirements myRequirements = this.GetProtectionRequirements();
                if (myRequirements != null)
                {
                    myRequirements.Add(context.GetInnerProperty<ChannelProtectionRequirements>() ?? new ChannelProtectionRequirements());
                    return (T)(object)myRequirements;
                }
                else
                {
                    return (T)(object)context.GetInnerProperty<ChannelProtectionRequirements>();
                }
            }
            else
            {
                return context.GetInnerProperty<T>();
            }
        }

        private ChannelProtectionRequirements GetProtectionRequirements()
        {
            if (this.Transactions || (this.IssuedTokens != TransactionFlowOption.NotAllowed))
            {
                ChannelProtectionRequirements requirements = new ChannelProtectionRequirements();
                if (this.Transactions)
                {
                    MessagePartSpecification p = new MessagePartSpecification(
                        new XmlQualifiedName(CoordinationExternalStrings.CoordinationContext, CoordinationExternal10Strings.Namespace),
                        new XmlQualifiedName(CoordinationExternalStrings.CoordinationContext, CoordinationExternal11Strings.Namespace),
                        new XmlQualifiedName(OleTxTransactionExternalStrings.OleTxTransaction, OleTxTransactionExternalStrings.Namespace));
                    p.MakeReadOnly();
                    requirements.IncomingSignatureParts.AddParts(p);
                    requirements.OutgoingSignatureParts.AddParts(p);
                    requirements.IncomingEncryptionParts.AddParts(p);
                    requirements.OutgoingEncryptionParts.AddParts(p);
                }
                if (this.IssuedTokens != TransactionFlowOption.NotAllowed)
                {
                    MessagePartSpecification trustParts = GetIssuedTokenHeaderSpecification(SecurityStandardsManager.DefaultInstance);
                    trustParts.MakeReadOnly();
                    requirements.IncomingSignatureParts.AddParts(trustParts);
                    requirements.IncomingEncryptionParts.AddParts(trustParts);
                    requirements.OutgoingSignatureParts.AddParts(trustParts);
                    requirements.OutgoingEncryptionParts.AddParts(trustParts);
                }

                MessagePartSpecification body = new MessagePartSpecification(true);
                body.MakeReadOnly();
                requirements.OutgoingSignatureParts.AddParts(body, FaultCodeConstants.Actions.Transactions);
                requirements.OutgoingEncryptionParts.AddParts(body, FaultCodeConstants.Actions.Transactions);
                return requirements;
            }
            else
            {
                return null;
            }
        }

        private XmlElement GetAssertion(XmlDocument doc, TransactionFlowOption option, string prefix, string name, string ns, string policyNs)
        {
            if (doc == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("doc");

            XmlElement result = null;
            switch (option)
            {
                case TransactionFlowOption.NotAllowed:
                    // Don't generate an assertion
                    break;

                case TransactionFlowOption.Allowed:
                    result = doc.CreateElement(prefix, name, ns);

                    // Always insert the real wsp:Optional attribute
                    XmlAttribute attr = doc.CreateAttribute(TransactionPolicyStrings.OptionalPrefix11,
                        TransactionPolicyStrings.OptionalLocal, policyNs);
                    attr.Value = TransactionPolicyStrings.TrueValue;
                    result.Attributes.Append(attr);

                    // For legacy protocols, also insert the legacy attribute for backward compat
                    if (_transactionProtocol == TransactionProtocol.OleTransactions ||
                        _transactionProtocol == TransactionProtocol.WSAtomicTransactionOctober2004)
                    {
                        XmlAttribute attrLegacy = doc.CreateAttribute(TransactionPolicyStrings.OptionalPrefix10,
                            TransactionPolicyStrings.OptionalLocal, TransactionPolicyStrings.OptionalNamespaceLegacy);
                        attrLegacy.Value = TransactionPolicyStrings.TrueValue;
                        result.Attributes.Append(attrLegacy);
                    }
                    break;

                case TransactionFlowOption.Mandatory:
                    result = doc.CreateElement(prefix, name, ns);
                    break;
            }
            return result;
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (b == null)
                return false;
            TransactionFlowBindingElement txFlow = b as TransactionFlowBindingElement;
            if (txFlow == null)
                return false;
            if (_transactions != txFlow._transactions)
                return false;
            if (_issuedTokens != txFlow._issuedTokens)
                return false;
            if (_transactionProtocol != txFlow._transactionProtocol)
                return false;

            return true;
        }
    }
}
