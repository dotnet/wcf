// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

        private bool _allowSerializedSigningTokenOnReply;
        private SecurityTokenParameters _initiatorTokenParameters;
        private MessageProtectionOrder _messageProtectionOrder;
        private SecurityTokenParameters _recipientTokenParameters;
        private bool _requireSignatureConfirmation;
        private bool _isCertificateSignatureBinding;

        private AsymmetricSecurityBindingElement(AsymmetricSecurityBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            if (elementToBeCloned._initiatorTokenParameters != null)
                _initiatorTokenParameters = (SecurityTokenParameters)elementToBeCloned._initiatorTokenParameters.Clone();
            _messageProtectionOrder = elementToBeCloned._messageProtectionOrder;
            if (elementToBeCloned._recipientTokenParameters != null)
                _recipientTokenParameters = (SecurityTokenParameters)elementToBeCloned._recipientTokenParameters.Clone();
            _requireSignatureConfirmation = elementToBeCloned._requireSignatureConfirmation;
            _allowSerializedSigningTokenOnReply = elementToBeCloned._allowSerializedSigningTokenOnReply;
            _isCertificateSignatureBinding = elementToBeCloned._isCertificateSignatureBinding;
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
            _messageProtectionOrder = SecurityBindingElement.defaultMessageProtectionOrder;
            _requireSignatureConfirmation = SecurityBindingElement.defaultRequireSignatureConfirmation;
            _initiatorTokenParameters = initiatorTokenParameters;
            _recipientTokenParameters = recipientTokenParameters;
            _allowSerializedSigningTokenOnReply = allowSerializedSigningTokenOnReply;
            _isCertificateSignatureBinding = false;
        }

        public bool AllowSerializedSigningTokenOnReply
        {
            get
            {
                return _allowSerializedSigningTokenOnReply;
            }
            set
            {
                _allowSerializedSigningTokenOnReply = value;
            }
        }

        public SecurityTokenParameters InitiatorTokenParameters
        {
            get
            {
                return _initiatorTokenParameters;
            }
            set
            {
                _initiatorTokenParameters = value;
            }
        }

        public MessageProtectionOrder MessageProtectionOrder
        {
            get
            {
                return _messageProtectionOrder;
            }
            set
            {
                if (!MessageProtectionOrderHelper.IsDefined(value))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                _messageProtectionOrder = value;
            }
        }

        public SecurityTokenParameters RecipientTokenParameters
        {
            get
            {
                return _recipientTokenParameters;
            }
            set
            {
                _recipientTokenParameters = value;
            }
        }

        public bool RequireSignatureConfirmation
        {
            get
            {
                return _requireSignatureConfirmation;
            }
            set
            {
                _requireSignatureConfirmation = value;
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
            get { return !_isCertificateSignatureBinding; }
        }

        internal override bool SupportsRequestReply
        {
            get
            {
                return !_isCertificateSignatureBinding;
            }
        }

        internal bool IsCertificateSignatureBinding
        {
            get { return _isCertificateSignatureBinding; }
            set { _isCertificateSignatureBinding = value; }
        }

        public override void SetKeyDerivation(bool requireDerivedKeys)
        {
            base.SetKeyDerivation(requireDerivedKeys);
            if (_initiatorTokenParameters != null)
                _initiatorTokenParameters.RequireDerivedKeys = requireDerivedKeys;
            if (_recipientTokenParameters != null)
                _recipientTokenParameters.RequireDerivedKeys = requireDerivedKeys;
        }

        internal override bool IsSetKeyDerivation(bool requireDerivedKeys)
        {
            if (!base.IsSetKeyDerivation(requireDerivedKeys))
                return false;
            if (_initiatorTokenParameters != null && _initiatorTokenParameters.RequireDerivedKeys != requireDerivedKeys)
                return false;
            if (_recipientTokenParameters != null && _recipientTokenParameters.RequireDerivedKeys != requireDerivedKeys)
                return false;
            return true;
        }

        private bool HasProtectionRequirements(ScopedMessagePartSpecification scopedParts)
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

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "MessageProtectionOrder: {0}", _messageProtectionOrder.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "RequireSignatureConfirmation: {0}", _requireSignatureConfirmation.ToString()));
            sb.Append("InitiatorTokenParameters: ");
            if (_initiatorTokenParameters != null)
                sb.AppendLine(_initiatorTokenParameters.ToString().Trim().Replace("\n", "\n  "));
            else
                sb.AppendLine("null");
            sb.Append("RecipientTokenParameters: ");
            if (_recipientTokenParameters != null)
                sb.AppendLine(_recipientTokenParameters.ToString().Trim().Replace("\n", "\n  "));
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
