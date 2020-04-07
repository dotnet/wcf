// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
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
        bool transactions;
        TransactionFlowOption issuedTokens;
        TransactionProtocol transactionProtocol;

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
            this.transactions = transactions;
            this.issuedTokens = transactions ? TransactionFlowOption.Allowed : TransactionFlowOption.NotAllowed;

            if (!TransactionProtocol.IsDefined(transactionProtocol))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRServiceModel.Format(SRServiceModel.ConfigInvalidTransactionFlowProtocolValue, transactionProtocol.ToString()));
            }

            this.transactionProtocol = transactionProtocol;
        }

        TransactionFlowBindingElement(TransactionFlowBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            this.transactions = elementToBeCloned.transactions;
            this.issuedTokens = elementToBeCloned.issuedTokens;

            if (!TransactionProtocol.IsDefined(elementToBeCloned.transactionProtocol))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRServiceModel.Format(SRServiceModel.ConfigInvalidTransactionFlowProtocolValue, elementToBeCloned.transactionProtocol.ToString()));
            }

            this.transactionProtocol = elementToBeCloned.transactionProtocol;
            this.AllowWildcardAction = elementToBeCloned.AllowWildcardAction;
        }

        internal bool Transactions
        {
            get
            {
                return this.transactions;
            }
            set
            {
                this.transactions = value;
                this.issuedTokens = value ? TransactionFlowOption.Allowed : TransactionFlowOption.NotAllowed;
            }
        }

        internal TransactionFlowOption IssuedTokens
        {
            get
            {
                return this.issuedTokens;
            }
            set
            {
                ValidateOption(value);
                this.issuedTokens = value;
            }
        }

        public override BindingElement Clone()
        {
            return new TransactionFlowBindingElement(this);
        }

        bool IsFlowEnabled(Dictionary<DirectionalAction, TransactionFlowOption> dictionary)
        {
            if (this.issuedTokens != TransactionFlowOption.NotAllowed)
            {
                return true;
            }

            if (!this.transactions)
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
            if (this.issuedTokens != TransactionFlowOption.NotAllowed)
            {
                return true;
            }

            if (!this.transactions)
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
                return this.transactionProtocol;
            }
            set
            {
                if (!TransactionProtocol.IsDefined(value))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                this.transactionProtocol = value;
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRServiceModel.Format(SRServiceModel.TransactionFlowBadOption)));
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

        Dictionary<DirectionalAction, TransactionFlowOption> GetDictionary(BindingContext context)
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.Format(SRServiceModel.TrustDriverVersionDoesNotSupportIssuedTokens)));
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

        ChannelProtectionRequirements GetProtectionRequirements()
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

        XmlElement GetAssertion(XmlDocument doc, TransactionFlowOption option, string prefix, string name, string ns, string policyNs)
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
                    if (this.transactionProtocol == TransactionProtocol.OleTransactions ||
                        this.transactionProtocol == TransactionProtocol.WSAtomicTransactionOctober2004)
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
            if (this.transactions != txFlow.transactions)
                return false;
            if (this.issuedTokens != txFlow.issuedTokens)
                return false;
            if (this.transactionProtocol != txFlow.transactionProtocol)
                return false;

            return true;
        }
    }
}
