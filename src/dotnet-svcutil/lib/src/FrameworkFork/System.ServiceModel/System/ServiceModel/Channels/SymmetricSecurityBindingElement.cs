// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel.Description;
    using Microsoft.Xml;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Security;
    using System.ServiceModel.Security.Tokens;
    using System.ServiceModel.Dispatcher;
    using System.Net.Security;
    using System.Text;

    public sealed class SymmetricSecurityBindingElement : SecurityBindingElement, IPolicyExportExtension
    {
        MessageProtectionOrder messageProtectionOrder;
        SecurityTokenParameters protectionTokenParameters;
        bool requireSignatureConfirmation;

        SymmetricSecurityBindingElement(SymmetricSecurityBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            this.messageProtectionOrder = elementToBeCloned.messageProtectionOrder;
            if (elementToBeCloned.protectionTokenParameters != null)
                this.protectionTokenParameters = (SecurityTokenParameters)elementToBeCloned.protectionTokenParameters.Clone();
            this.requireSignatureConfirmation = elementToBeCloned.requireSignatureConfirmation;
        }

        public SymmetricSecurityBindingElement()
            : this((SecurityTokenParameters)null)
        {
            // empty
        }

        public SymmetricSecurityBindingElement(SecurityTokenParameters protectionTokenParameters)
            : base()
        {
            this.messageProtectionOrder = SecurityBindingElement.defaultMessageProtectionOrder;
            this.requireSignatureConfirmation = SecurityBindingElement.defaultRequireSignatureConfirmation;
            this.protectionTokenParameters = protectionTokenParameters;
        }

        public bool RequireSignatureConfirmation
        {
            get
            {
                return this.requireSignatureConfirmation;
            }
            set
            {
                this.requireSignatureConfirmation = value;
            }
        }

        public MessageProtectionOrder MessageProtectionOrder
        {
            get
            {
                return this.messageProtectionOrder;
            }
            set
            {
                if (!MessageProtectionOrderHelper.IsDefined(value))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                this.messageProtectionOrder = value;
            }
        }

        public SecurityTokenParameters ProtectionTokenParameters
        {
            get
            {
                return this.protectionTokenParameters;
            }
            set
            {
                this.protectionTokenParameters = value;
            }
        }

        internal override ISecurityCapabilities GetIndividualISecurityCapabilities()
        {
            bool supportsServerAuthentication = false;
            bool supportsClientAuthentication;
            bool supportsClientWindowsIdentity;
            GetSupportingTokensCapabilities(out supportsClientAuthentication, out supportsClientWindowsIdentity);
            if (ProtectionTokenParameters != null)
            {
                supportsClientAuthentication = supportsClientAuthentication || ProtectionTokenParameters.SupportsClientAuthentication;
                supportsClientWindowsIdentity = supportsClientWindowsIdentity || ProtectionTokenParameters.SupportsClientWindowsIdentity;

                if (ProtectionTokenParameters.HasAsymmetricKey)
                {
                    supportsServerAuthentication = ProtectionTokenParameters.SupportsClientAuthentication;
                }
                else
                {
                    supportsServerAuthentication = ProtectionTokenParameters.SupportsServerAuthentication;
                }
            }

            return new SecurityCapabilities(supportsClientAuthentication, supportsServerAuthentication, supportsClientWindowsIdentity,
                ProtectionLevel.EncryptAndSign, ProtectionLevel.EncryptAndSign);
        }

        internal override bool SessionMode
        {
            get
            {
                SecureConversationSecurityTokenParameters secureConversationTokenParameters = this.ProtectionTokenParameters as SecureConversationSecurityTokenParameters;
                if (secureConversationTokenParameters != null)
                    return secureConversationTokenParameters.RequireCancellation;
                else
                    return false;
            }
        }

        internal override bool SupportsDuplex
        {
            get { return this.SessionMode; }
        }

        internal override bool SupportsRequestReply
        {
            get { return true; }
        }

        public override void SetKeyDerivation(bool requireDerivedKeys)
        {
            base.SetKeyDerivation(requireDerivedKeys);
            if (this.protectionTokenParameters != null)
                this.protectionTokenParameters.RequireDerivedKeys = requireDerivedKeys;
        }

        internal override bool IsSetKeyDerivation(bool requireDerivedKeys)
        {
            if (!base.IsSetKeyDerivation(requireDerivedKeys))
                return false;

            if (this.protectionTokenParameters != null && this.protectionTokenParameters.RequireDerivedKeys != requireDerivedKeys)
                return false;

            return true;
        }

        protected override IChannelFactory<TChannel> BuildChannelFactoryCore<TChannel>(BindingContext context)
        {
            throw new NotImplementedException();
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");

            if (typeof(T) == typeof(ChannelProtectionRequirements))
            {
                AddressingVersion addressing = MessageVersion.Default.Addressing;
#pragma warning disable 56506
                MessageEncodingBindingElement encoding = context.Binding.Elements.Find<MessageEncodingBindingElement>();
                if (encoding != null)
                {
                    addressing = encoding.MessageVersion.Addressing;
                }
                ChannelProtectionRequirements myRequirements = base.GetProtectionRequirements(addressing, ProtectionLevel.EncryptAndSign);
                myRequirements.Add(context.GetInnerProperty<ChannelProtectionRequirements>() ?? new ChannelProtectionRequirements());
                return (T)(object)myRequirements;
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "MessageProtectionOrder: {0}", this.messageProtectionOrder.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "RequireSignatureConfirmation: {0}", this.requireSignatureConfirmation.ToString()));
            sb.Append("ProtectionTokenParameters: ");
            if (this.protectionTokenParameters != null)
                sb.AppendLine(this.protectionTokenParameters.ToString().Trim().Replace("\n", "\n  "));
            else
                sb.AppendLine("null");

            return sb.ToString().Trim();
        }

        public override BindingElement Clone()
        {
            return new SymmetricSecurityBindingElement(this);
        }

        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
        {
            //SecurityBindingElement.ExportPolicy(exporter, context);
            throw new NotImplementedException();
        }
    }
}
