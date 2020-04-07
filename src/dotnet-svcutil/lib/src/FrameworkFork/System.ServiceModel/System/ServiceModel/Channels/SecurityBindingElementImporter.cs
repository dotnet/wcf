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

    using System.Net.Security;
    using System.Text;

    public sealed class AsymmetricSecurityBindingElement : SecurityBindingElement, IPolicyExportExtension
    {
        internal const bool defaultAllowSerializedSigningTokenOnReply = false;

        bool allowSerializedSigningTokenOnReply;
        SecurityTokenParameters initiatorTokenParameters;
        MessageProtectionOrder messageProtectionOrder;
        SecurityTokenParameters recipientTokenParameters;
        bool requireSignatureConfirmation;
        bool isCertificateSignatureBinding;

        AsymmetricSecurityBindingElement(AsymmetricSecurityBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            if (elementToBeCloned.initiatorTokenParameters != null)
                this.initiatorTokenParameters = (SecurityTokenParameters)elementToBeCloned.initiatorTokenParameters.Clone();
            this.messageProtectionOrder = elementToBeCloned.messageProtectionOrder;
            if (elementToBeCloned.recipientTokenParameters != null)
                this.recipientTokenParameters = (SecurityTokenParameters)elementToBeCloned.recipientTokenParameters.Clone();
            this.requireSignatureConfirmation = elementToBeCloned.requireSignatureConfirmation;
            this.allowSerializedSigningTokenOnReply = elementToBeCloned.allowSerializedSigningTokenOnReply;
            this.isCertificateSignatureBinding = elementToBeCloned.isCertificateSignatureBinding;
        }

        public AsymmetricSecurityBindingElement()
            : this(null, null)
        {
            // empty
        }

        public AsymmetricSecurityBindingElement(SecurityTokenParameters recipientTokenParameters)
            : this(recipientTokenParameters, null)
        {
            // empty
        }

        public AsymmetricSecurityBindingElement(SecurityTokenParameters recipientTokenParameters, SecurityTokenParameters initiatorTokenParameters)
            : this(recipientTokenParameters, initiatorTokenParameters, AsymmetricSecurityBindingElement.defaultAllowSerializedSigningTokenOnReply)
        {
            // empty
        }

        internal AsymmetricSecurityBindingElement(
            SecurityTokenParameters recipientTokenParameters,
            SecurityTokenParameters initiatorTokenParameters,
            bool allowSerializedSigningTokenOnReply)
            : base()
        {
            this.messageProtectionOrder = SecurityBindingElement.defaultMessageProtectionOrder;
            this.requireSignatureConfirmation = SecurityBindingElement.defaultRequireSignatureConfirmation;
            this.initiatorTokenParameters = initiatorTokenParameters;
            this.recipientTokenParameters = recipientTokenParameters;
            this.allowSerializedSigningTokenOnReply = allowSerializedSigningTokenOnReply;
            this.isCertificateSignatureBinding = false;
        }

        public bool AllowSerializedSigningTokenOnReply
        {
            get
            {
                return this.allowSerializedSigningTokenOnReply;
            }
            set
            {
                this.allowSerializedSigningTokenOnReply = value;
            }
        }

        public SecurityTokenParameters InitiatorTokenParameters
        {
            get
            {
                return this.initiatorTokenParameters;
            }
            set
            {
                this.initiatorTokenParameters = value;
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

        public SecurityTokenParameters RecipientTokenParameters
        {
            get
            {
                return this.recipientTokenParameters;
            }
            set
            {
                this.recipientTokenParameters = value;
            }
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

        internal override ISecurityCapabilities GetIndividualISecurityCapabilities()
        {
            ProtectionLevel requestProtectionLevel = ProtectionLevel.EncryptAndSign;
            ProtectionLevel responseProtectionLevel = ProtectionLevel.EncryptAndSign;
            bool supportsServerAuthentication = false;

            if (IsCertificateSignatureBinding)
            {
                requestProtectionLevel = ProtectionLevel.Sign;
                responseProtectionLevel = ProtectionLevel.None;
            }
            else if (RecipientTokenParameters != null)
            {
                supportsServerAuthentication = RecipientTokenParameters.SupportsServerAuthentication;
            }

            bool supportsClientAuthentication;
            bool supportsClientWindowsIdentity;
            GetSupportingTokensCapabilities(out supportsClientAuthentication, out supportsClientWindowsIdentity);
            if (InitiatorTokenParameters != null)
            {
                supportsClientAuthentication = supportsClientAuthentication || InitiatorTokenParameters.SupportsClientAuthentication;
                supportsClientWindowsIdentity = supportsClientWindowsIdentity || InitiatorTokenParameters.SupportsClientWindowsIdentity;
            }

            return new SecurityCapabilities(supportsClientAuthentication, supportsServerAuthentication, supportsClientWindowsIdentity,
                requestProtectionLevel, responseProtectionLevel);
        }

        internal override bool SupportsDuplex
        {
            get { return !this.isCertificateSignatureBinding; }
        }

        internal override bool SupportsRequestReply
        {
            get
            {
                return !this.isCertificateSignatureBinding;
            }
        }

        internal bool IsCertificateSignatureBinding
        {
            get { return this.isCertificateSignatureBinding; }
            set { this.isCertificateSignatureBinding = value; }
        }

        public override void SetKeyDerivation(bool requireDerivedKeys)
        {
            base.SetKeyDerivation(requireDerivedKeys);
            if (this.initiatorTokenParameters != null)
                this.initiatorTokenParameters.RequireDerivedKeys = requireDerivedKeys;
            if (this.recipientTokenParameters != null)
                this.recipientTokenParameters.RequireDerivedKeys = requireDerivedKeys;
        }

        internal override bool IsSetKeyDerivation(bool requireDerivedKeys)
        {
            if (!base.IsSetKeyDerivation(requireDerivedKeys))
                return false;
            if (this.initiatorTokenParameters != null && this.initiatorTokenParameters.RequireDerivedKeys != requireDerivedKeys)
                return false;
            if (this.recipientTokenParameters != null && this.recipientTokenParameters.RequireDerivedKeys != requireDerivedKeys)
                return false;
            return true;
        }

        bool HasProtectionRequirements(ScopedMessagePartSpecification scopedParts)
        {
            foreach (string action in scopedParts.Actions)
            {
                MessagePartSpecification parts;
                if (scopedParts.TryGetParts(action, out parts))
                {
                    if (!parts.IsEmpty())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override IChannelFactory<TChannel> BuildChannelFactoryCore<TChannel>(BindingContext context)
        {
            throw new NotImplementedException();
        }

        public override T GetProperty<T>(BindingContext context)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "MessageProtectionOrder: {0}", this.messageProtectionOrder.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "RequireSignatureConfirmation: {0}", this.requireSignatureConfirmation.ToString()));
            sb.Append("InitiatorTokenParameters: ");
            if (this.initiatorTokenParameters != null)
                sb.AppendLine(this.initiatorTokenParameters.ToString().Trim().Replace("\n", "\n  "));
            else
                sb.AppendLine("null");
            sb.Append("RecipientTokenParameters: ");
            if (this.recipientTokenParameters != null)
                sb.AppendLine(this.recipientTokenParameters.ToString().Trim().Replace("\n", "\n  "));
            else
                sb.AppendLine("null");

            return sb.ToString().Trim();
        }

        public override BindingElement Clone()
        {
            return new AsymmetricSecurityBindingElement(this);
        }

        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
