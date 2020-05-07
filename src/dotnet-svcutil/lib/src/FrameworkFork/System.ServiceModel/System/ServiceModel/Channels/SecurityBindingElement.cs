// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net.Security;
using System.Runtime;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Text;
using Microsoft.Xml;

namespace System.ServiceModel.Channels
{
    public abstract class SecurityBindingElement : BindingElement
    {
        internal static readonly SecurityAlgorithmSuite defaultDefaultAlgorithmSuite = SecurityAlgorithmSuite.Default;
        internal const bool defaultIncludeTimestamp = true;
        internal const bool defaultAllowInsecureTransport = false;
        internal const MessageProtectionOrder defaultMessageProtectionOrder = MessageProtectionOrder.SignBeforeEncryptAndEncryptSignature;
        internal const bool defaultRequireSignatureConfirmation = false;
        internal const bool defaultEnableUnsecuredResponse = false;
        internal const bool defaultProtectTokens = false;

        private SecurityAlgorithmSuite _defaultAlgorithmSuite;
        private SupportingTokenParameters _endpointSupportingTokenParameters;
        private SupportingTokenParameters _optionalEndpointSupportingTokenParameters;
        private bool _includeTimestamp;
        private SecurityKeyEntropyMode _keyEntropyMode;
        private Dictionary<string, SupportingTokenParameters> _operationSupportingTokenParameters;
        private Dictionary<string, SupportingTokenParameters> _optionalOperationSupportingTokenParameters;
        private LocalClientSecuritySettings _localClientSettings;
        private LocalServiceSecuritySettings _localServiceSettings;
        private MessageSecurityVersion _messageSecurityVersion;
        private SecurityHeaderLayout _securityHeaderLayout;

        private long _maxReceivedMessageSize = TransportDefaults.MaxReceivedMessageSize;
        private XmlDictionaryReaderQuotas _readerQuotas;
        private bool _doNotEmitTrust = false; // true if user create a basic http standard binding, the custombinding equivalent will not set this flag 
        private bool _supportsExtendedProtectionPolicy;
        private bool _allowInsecureTransport;
        private bool _enableUnsecuredResponse;
        private bool _protectTokens = defaultProtectTokens;

        internal SecurityBindingElement()
            : base()
        {
            _messageSecurityVersion = MessageSecurityVersion.Default;
            _keyEntropyMode = SecurityKeyEntropyMode.CombinedEntropy;  //TODO:? AcceleratedTokenProvider.defaultKeyEntropyMode;
            _includeTimestamp = defaultIncludeTimestamp;
            _defaultAlgorithmSuite = defaultDefaultAlgorithmSuite;
            _localClientSettings = new LocalClientSecuritySettings();
            _localServiceSettings = new LocalServiceSecuritySettings();
            _endpointSupportingTokenParameters = new SupportingTokenParameters();
            _optionalEndpointSupportingTokenParameters = new SupportingTokenParameters();
            _operationSupportingTokenParameters = new Dictionary<string, SupportingTokenParameters>();
            _optionalOperationSupportingTokenParameters = new Dictionary<string, SupportingTokenParameters>();
            _securityHeaderLayout = SecurityProtocolFactory.defaultSecurityHeaderLayout;
            _allowInsecureTransport = defaultAllowInsecureTransport;
            _enableUnsecuredResponse = defaultEnableUnsecuredResponse;
            _protectTokens = defaultProtectTokens;
        }

        internal SecurityBindingElement(SecurityBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            if (elementToBeCloned == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("elementToBeCloned");

            _defaultAlgorithmSuite = elementToBeCloned._defaultAlgorithmSuite;
            _includeTimestamp = elementToBeCloned._includeTimestamp;
            _keyEntropyMode = elementToBeCloned._keyEntropyMode;
            _messageSecurityVersion = elementToBeCloned._messageSecurityVersion;
            _securityHeaderLayout = elementToBeCloned._securityHeaderLayout;
            _endpointSupportingTokenParameters = elementToBeCloned._endpointSupportingTokenParameters.Clone();
            _optionalEndpointSupportingTokenParameters = elementToBeCloned._optionalEndpointSupportingTokenParameters.Clone();
            _operationSupportingTokenParameters = new Dictionary<string, SupportingTokenParameters>();
            foreach (string key in elementToBeCloned._operationSupportingTokenParameters.Keys)
            {
                _operationSupportingTokenParameters[key] = (SupportingTokenParameters)elementToBeCloned._operationSupportingTokenParameters[key].Clone();
            }
            _optionalOperationSupportingTokenParameters = new Dictionary<string, SupportingTokenParameters>();
            foreach (string key in elementToBeCloned._optionalOperationSupportingTokenParameters.Keys)
            {
                _optionalOperationSupportingTokenParameters[key] = (SupportingTokenParameters)elementToBeCloned._optionalOperationSupportingTokenParameters[key].Clone();
            }
            _localClientSettings = elementToBeCloned._localClientSettings.Clone();
            _localServiceSettings = elementToBeCloned._localServiceSettings.Clone();

            _maxReceivedMessageSize = elementToBeCloned._maxReceivedMessageSize;
            _readerQuotas = elementToBeCloned._readerQuotas;
            _doNotEmitTrust = elementToBeCloned._doNotEmitTrust;
            _allowInsecureTransport = elementToBeCloned._allowInsecureTransport;
            _enableUnsecuredResponse = elementToBeCloned._enableUnsecuredResponse;
            _supportsExtendedProtectionPolicy = elementToBeCloned._supportsExtendedProtectionPolicy;
            _protectTokens = elementToBeCloned._protectTokens;
        }

        internal bool SupportsExtendedProtectionPolicy
        {
            get { return _supportsExtendedProtectionPolicy; }
            set { _supportsExtendedProtectionPolicy = value; }
        }

        public SupportingTokenParameters EndpointSupportingTokenParameters
        {
            get
            {
                return _endpointSupportingTokenParameters;
            }
        }

        public SupportingTokenParameters OptionalEndpointSupportingTokenParameters
        {
            get
            {
                return _optionalEndpointSupportingTokenParameters;
            }
        }


        public IDictionary<string, SupportingTokenParameters> OperationSupportingTokenParameters
        {
            get
            {
                return _operationSupportingTokenParameters;
            }
        }

        public IDictionary<string, SupportingTokenParameters> OptionalOperationSupportingTokenParameters
        {
            get
            {
                return _optionalOperationSupportingTokenParameters;
            }
        }

        public SecurityHeaderLayout SecurityHeaderLayout
        {
            get
            {
                return _securityHeaderLayout;
            }
            set
            {
                if (!SecurityHeaderLayoutHelper.IsDefined(value))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));

                _securityHeaderLayout = value;
            }
        }

        public MessageSecurityVersion MessageSecurityVersion
        {
            get
            {
                return _messageSecurityVersion;
            }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                _messageSecurityVersion = value;
            }
        }

        public bool EnableUnsecuredResponse
        {
            get
            {
                return _enableUnsecuredResponse;
            }
            set
            {
                _enableUnsecuredResponse = value;
            }
        }

        public bool IncludeTimestamp
        {
            get
            {
                return _includeTimestamp;
            }
            set
            {
                _includeTimestamp = value;
            }
        }

        public bool AllowInsecureTransport
        {
            get
            {
                return _allowInsecureTransport;
            }
            set
            {
                _allowInsecureTransport = value;
            }
        }

        public SecurityAlgorithmSuite DefaultAlgorithmSuite
        {
            get
            {
                return _defaultAlgorithmSuite;
            }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                _defaultAlgorithmSuite = value;
            }
        }

        public bool ProtectTokens
        {
            get
            {
                return _protectTokens;
            }
            set
            {
                _protectTokens = value;
            }
        }

        public LocalClientSecuritySettings LocalClientSettings
        {
            get
            {
                return _localClientSettings;
            }
        }

        public LocalServiceSecuritySettings LocalServiceSettings
        {
            get
            {
                return _localServiceSettings;
            }
        }

        public SecurityKeyEntropyMode KeyEntropyMode
        {
            get
            {
                return _keyEntropyMode;
            }
            set
            {
                if (!SecurityKeyEntropyModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                _keyEntropyMode = value;
            }
        }

        internal virtual bool SessionMode
        {
            get { return false; }
        }

        internal virtual bool SupportsDuplex
        {
            get { return false; }
        }

        internal virtual bool SupportsRequestReply
        {
            get { return false; }
        }

        internal long MaxReceivedMessageSize
        {
            get { return _maxReceivedMessageSize; }
            set { _maxReceivedMessageSize = value; }
        }

        internal bool DoNotEmitTrust
        {
            get { return _doNotEmitTrust; }
            set { _doNotEmitTrust = value; }
        }

        internal XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return _readerQuotas; }
            set { _readerQuotas = value; }
        }

        private void GetSupportingTokensCapabilities(ICollection<SecurityTokenParameters> parameters, out bool supportsClientAuth, out bool supportsWindowsIdentity)
        {
            supportsClientAuth = false;
            supportsWindowsIdentity = false;
            foreach (SecurityTokenParameters p in parameters)
            {
                if (p.SupportsClientAuthentication)
                    supportsClientAuth = true;
                if (p.SupportsClientWindowsIdentity)
                    supportsWindowsIdentity = true;
            }
        }

        private void GetSupportingTokensCapabilities(SupportingTokenParameters requirements, out bool supportsClientAuth, out bool supportsWindowsIdentity)
        {
            supportsClientAuth = false;
            supportsWindowsIdentity = false;
            bool tmpSupportsClientAuth;
            bool tmpSupportsWindowsIdentity;
            this.GetSupportingTokensCapabilities(requirements.Endorsing, out tmpSupportsClientAuth, out tmpSupportsWindowsIdentity);
            supportsClientAuth = supportsClientAuth || tmpSupportsClientAuth;
            supportsWindowsIdentity = supportsWindowsIdentity || tmpSupportsWindowsIdentity;

            this.GetSupportingTokensCapabilities(requirements.SignedEndorsing, out tmpSupportsClientAuth, out tmpSupportsWindowsIdentity);
            supportsClientAuth = supportsClientAuth || tmpSupportsClientAuth;
            supportsWindowsIdentity = supportsWindowsIdentity || tmpSupportsWindowsIdentity;

            this.GetSupportingTokensCapabilities(requirements.SignedEncrypted, out tmpSupportsClientAuth, out tmpSupportsWindowsIdentity);
            supportsClientAuth = supportsClientAuth || tmpSupportsClientAuth;
            supportsWindowsIdentity = supportsWindowsIdentity || tmpSupportsWindowsIdentity;
        }

        internal void GetSupportingTokensCapabilities(out bool supportsClientAuth, out bool supportsWindowsIdentity)
        {
            this.GetSupportingTokensCapabilities(this.EndpointSupportingTokenParameters, out supportsClientAuth, out supportsWindowsIdentity);
        }
        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            throw new NotImplementedException();
        }

        protected abstract IChannelFactory<TChannel> BuildChannelFactoryCore<TChannel>(BindingContext context);

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            throw new NotImplementedException();
        }

        private bool CanBuildSessionChannelFactory<TChannel>(BindingContext context)
        {
            if (!(context.CanBuildInnerChannelFactory<IRequestChannel>()
                || context.CanBuildInnerChannelFactory<IRequestSessionChannel>()
                || context.CanBuildInnerChannelFactory<IDuplexChannel>()
                || context.CanBuildInnerChannelFactory<IDuplexSessionChannel>()))
            {
                return false;
            }

            if (typeof(TChannel) == typeof(IRequestSessionChannel))
            {
                return (context.CanBuildInnerChannelFactory<IRequestChannel>() || context.CanBuildInnerChannelFactory<IRequestSessionChannel>());
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return (context.CanBuildInnerChannelFactory<IDuplexChannel>() || context.CanBuildInnerChannelFactory<IDuplexSessionChannel>());
            }
            else
            {
                return false;
            }
        }

        public virtual void SetKeyDerivation(bool requireDerivedKeys)
        {
            this.EndpointSupportingTokenParameters.SetKeyDerivation(requireDerivedKeys);
            this.OptionalEndpointSupportingTokenParameters.SetKeyDerivation(requireDerivedKeys);
            foreach (SupportingTokenParameters t in this.OperationSupportingTokenParameters.Values)
                t.SetKeyDerivation(requireDerivedKeys);
            foreach (SupportingTokenParameters t in this.OptionalOperationSupportingTokenParameters.Values)
            {
                t.SetKeyDerivation(requireDerivedKeys);
            }
        }

        internal virtual bool IsSetKeyDerivation(bool requireDerivedKeys)
        {
            if (!this.EndpointSupportingTokenParameters.IsSetKeyDerivation(requireDerivedKeys))
                return false;

            if (!this.OptionalEndpointSupportingTokenParameters.IsSetKeyDerivation(requireDerivedKeys))
                return false;

            foreach (SupportingTokenParameters t in this.OperationSupportingTokenParameters.Values)
            {
                if (!t.IsSetKeyDerivation(requireDerivedKeys))
                    return false;
            }
            foreach (SupportingTokenParameters t in this.OptionalOperationSupportingTokenParameters.Values)
            {
                if (!t.IsSetKeyDerivation(requireDerivedKeys))
                    return false;
            }
            return true;
        }

        internal ChannelProtectionRequirements GetProtectionRequirements(AddressingVersion addressing, ProtectionLevel defaultProtectionLevel)
        {
            if (addressing == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("addressing");

            ChannelProtectionRequirements result = new ChannelProtectionRequirements();
            ProtectionLevel supportedRequestProtectionLevel = this.GetIndividualProperty<ISecurityCapabilities>().SupportedRequestProtectionLevel;
            ProtectionLevel supportedResponseProtectionLevel = this.GetIndividualProperty<ISecurityCapabilities>().SupportedResponseProtectionLevel;

            bool canSupportMoreThanTheDefault =
                (ProtectionLevelHelper.IsStrongerOrEqual(supportedRequestProtectionLevel, defaultProtectionLevel)
                && ProtectionLevelHelper.IsStrongerOrEqual(supportedResponseProtectionLevel, defaultProtectionLevel));
            if (canSupportMoreThanTheDefault)
            {
                MessagePartSpecification signedParts = new MessagePartSpecification();
                MessagePartSpecification encryptedParts = new MessagePartSpecification();
                if (defaultProtectionLevel != ProtectionLevel.None)
                {
                    signedParts.IsBodyIncluded = true;
                    if (defaultProtectionLevel == ProtectionLevel.EncryptAndSign)
                    {
                        encryptedParts.IsBodyIncluded = true;
                    }
                }
                signedParts.MakeReadOnly();
                encryptedParts.MakeReadOnly();
                if (addressing.FaultAction != null)
                {
                    // Addressing faults
                    result.IncomingSignatureParts.AddParts(signedParts, addressing.FaultAction);
                    result.OutgoingSignatureParts.AddParts(signedParts, addressing.FaultAction);
                    result.IncomingEncryptionParts.AddParts(encryptedParts, addressing.FaultAction);
                    result.OutgoingEncryptionParts.AddParts(encryptedParts, addressing.FaultAction);
                }
                if (addressing.DefaultFaultAction != null)
                {
                    // Faults that do not specify a particular action
                    result.IncomingSignatureParts.AddParts(signedParts, addressing.DefaultFaultAction);
                    result.OutgoingSignatureParts.AddParts(signedParts, addressing.DefaultFaultAction);
                    result.IncomingEncryptionParts.AddParts(encryptedParts, addressing.DefaultFaultAction);
                    result.OutgoingEncryptionParts.AddParts(encryptedParts, addressing.DefaultFaultAction);
                }
                // Infrastructure faults
                result.IncomingSignatureParts.AddParts(signedParts, FaultCodeConstants.Actions.NetDispatcher);
                result.OutgoingSignatureParts.AddParts(signedParts, FaultCodeConstants.Actions.NetDispatcher);
                result.IncomingEncryptionParts.AddParts(encryptedParts, FaultCodeConstants.Actions.NetDispatcher);
                result.OutgoingEncryptionParts.AddParts(encryptedParts, FaultCodeConstants.Actions.NetDispatcher);
            }

            return result;
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T)(object)GetSecurityCapabilities(context);
            }
            else if (typeof(T) == typeof(IdentityVerifier))
            {
                return (T)(object)_localClientSettings.IdentityVerifier;
            }
            else
            {
                return context.GetInnerProperty<T>();
            }
        }

        internal abstract ISecurityCapabilities GetIndividualISecurityCapabilities();

        private ISecurityCapabilities GetSecurityCapabilities(BindingContext context)
        {
            ISecurityCapabilities thisSecurityCapability = this.GetIndividualISecurityCapabilities();
            ISecurityCapabilities lowerSecurityCapability = context.GetInnerProperty<ISecurityCapabilities>();
            if (lowerSecurityCapability == null)
            {
                return thisSecurityCapability;
            }
            else
            {
                bool supportsClientAuth = thisSecurityCapability.SupportsClientAuthentication;
                bool supportsClientWindowsIdentity = thisSecurityCapability.SupportsClientWindowsIdentity;
                bool supportsServerAuth = thisSecurityCapability.SupportsServerAuthentication || lowerSecurityCapability.SupportsServerAuthentication;
                ProtectionLevel requestProtectionLevel = ProtectionLevelHelper.Max(thisSecurityCapability.SupportedRequestProtectionLevel, lowerSecurityCapability.SupportedRequestProtectionLevel);
                ProtectionLevel responseProtectionLevel = ProtectionLevelHelper.Max(thisSecurityCapability.SupportedResponseProtectionLevel, lowerSecurityCapability.SupportedResponseProtectionLevel);
                return new SecurityCapabilities(supportsClientAuth, supportsServerAuth, supportsClientWindowsIdentity, requestProtectionLevel, responseProtectionLevel);
            }
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsMutualCertificateBinding() method.
        static public SecurityBindingElement CreateMutualCertificateBindingElement()
        {
            return CreateMutualCertificateBindingElement(MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11);
        }

        // this method reverses CreateMutualCertificateBindingElement() logic
        internal static bool IsMutualCertificateBinding(SecurityBindingElement sbe)
        {
            return IsMutualCertificateBinding(sbe, false);
        }

        static public AsymmetricSecurityBindingElement CreateCertificateSignatureBindingElement()
        {
            AsymmetricSecurityBindingElement result;

            result = new AsymmetricSecurityBindingElement(
                new X509SecurityTokenParameters( // recipient
                    X509KeyIdentifierClauseType.Any,
                    SecurityTokenInclusionMode.Never, false),
                new X509SecurityTokenParameters( // initiator
                    X509KeyIdentifierClauseType.Any,
                    SecurityTokenInclusionMode.AlwaysToRecipient, false));

            // this is a one way binding so the client cannot detect replays
            result.IsCertificateSignatureBinding = true;
            result.LocalClientSettings.DetectReplays = false;
            result.MessageProtectionOrder = MessageProtectionOrder.SignBeforeEncrypt;

            return result;
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsMutualCertificateBinding() method.
        static public SecurityBindingElement CreateMutualCertificateBindingElement(MessageSecurityVersion version)
        {
            return CreateMutualCertificateBindingElement(version, false);
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsMutualCertificateBinding() method.
        static public SecurityBindingElement CreateMutualCertificateBindingElement(MessageSecurityVersion version, bool allowSerializedSigningTokenOnReply)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("version");
            }
            SecurityBindingElement result;

            if (version.SecurityVersion == SecurityVersion.WSSecurity10)
            {
                result = new AsymmetricSecurityBindingElement(
                    new X509SecurityTokenParameters( // recipient
                        X509KeyIdentifierClauseType.Any,
                        SecurityTokenInclusionMode.Never,
                        false),
                    new X509SecurityTokenParameters( // initiator
                        X509KeyIdentifierClauseType.Any,
                        SecurityTokenInclusionMode.AlwaysToRecipient, false),
                    allowSerializedSigningTokenOnReply);
            }
            else
            {
                result = new SymmetricSecurityBindingElement(
                    new X509SecurityTokenParameters( // protection
                        X509KeyIdentifierClauseType.Thumbprint,
                        SecurityTokenInclusionMode.Never));
                result.EndpointSupportingTokenParameters.Endorsing.Add(
                    new X509SecurityTokenParameters(
                        X509KeyIdentifierClauseType.Thumbprint,
                        SecurityTokenInclusionMode.AlwaysToRecipient,
                        false));
                ((SymmetricSecurityBindingElement)result).RequireSignatureConfirmation = true;
            }

            result.MessageSecurityVersion = version;

            return result;
        }

        // this method reverses CreateMutualCertificateDuplexBindingElement() logic

        internal static bool IsMutualCertificateDuplexBinding(SecurityBindingElement sbe)
        {
            // Do not check MessageSecurityVersion: it maybe changed by the wrapper element and gets checked later in the SecuritySection.AreBindingsMatching()

            AsymmetricSecurityBindingElement asbe = sbe as AsymmetricSecurityBindingElement;
            if (asbe != null)
            {
                X509SecurityTokenParameters recipient = asbe.RecipientTokenParameters as X509SecurityTokenParameters;
                if (recipient == null || (recipient.X509ReferenceStyle != X509KeyIdentifierClauseType.Any && recipient.X509ReferenceStyle != X509KeyIdentifierClauseType.Thumbprint) || recipient.InclusionMode != SecurityTokenInclusionMode.AlwaysToInitiator)
                    return false;

                X509SecurityTokenParameters initiator = asbe.InitiatorTokenParameters as X509SecurityTokenParameters;
                if (initiator == null || (initiator.X509ReferenceStyle != X509KeyIdentifierClauseType.Any && initiator.X509ReferenceStyle != X509KeyIdentifierClauseType.Thumbprint) || initiator.InclusionMode != SecurityTokenInclusionMode.AlwaysToRecipient)
                    return false;

                if (!sbe.EndpointSupportingTokenParameters.IsEmpty())
                    return false;

                return true;
            }
            else
            {
                return false;
            }
        }

        // this method reverses CreateMutualCertificateBindingElement() logic
        internal static bool IsMutualCertificateBinding(SecurityBindingElement sbe, bool allowSerializedSigningTokenOnReply)
        {
            // Do not check MessageSecurityVersion: it maybe changed by the wrapper element and gets checked later in the SecuritySection.AreBindingsMatching()

            AsymmetricSecurityBindingElement asbe = sbe as AsymmetricSecurityBindingElement;
            if (asbe != null)
            {
                X509SecurityTokenParameters recipient = asbe.RecipientTokenParameters as X509SecurityTokenParameters;
                if (recipient == null || recipient.X509ReferenceStyle != X509KeyIdentifierClauseType.Any || recipient.InclusionMode != SecurityTokenInclusionMode.Never)
                    return false;

                X509SecurityTokenParameters initiator = asbe.InitiatorTokenParameters as X509SecurityTokenParameters;
                if (initiator == null || initiator.X509ReferenceStyle != X509KeyIdentifierClauseType.Any || initiator.InclusionMode != SecurityTokenInclusionMode.AlwaysToRecipient)
                    return false;

                if (!sbe.EndpointSupportingTokenParameters.IsEmpty())
                    return false;
            }
            else
            {
                SymmetricSecurityBindingElement ssbe = sbe as SymmetricSecurityBindingElement;
                if (ssbe == null)
                    return false;

                X509SecurityTokenParameters x509Parameters = ssbe.ProtectionTokenParameters as X509SecurityTokenParameters;
                if (x509Parameters == null || x509Parameters.X509ReferenceStyle != X509KeyIdentifierClauseType.Thumbprint || x509Parameters.InclusionMode != SecurityTokenInclusionMode.Never)
                    return false;

                SupportingTokenParameters parameters = sbe.EndpointSupportingTokenParameters;
                if (parameters.Signed.Count != 0 || parameters.SignedEncrypted.Count != 0 || parameters.Endorsing.Count != 1 || parameters.SignedEndorsing.Count != 0)
                    return false;

                x509Parameters = parameters.Endorsing[0] as X509SecurityTokenParameters;
                if (x509Parameters == null || x509Parameters.X509ReferenceStyle != X509KeyIdentifierClauseType.Thumbprint || x509Parameters.InclusionMode != SecurityTokenInclusionMode.AlwaysToRecipient)
                    return false;

                if (!ssbe.RequireSignatureConfirmation)
                    return false;
            }
            return true;
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsAnonymousForCertificateBinding() method.
        static public SymmetricSecurityBindingElement CreateAnonymousForCertificateBindingElement()
        {
            SymmetricSecurityBindingElement result;

            result = new SymmetricSecurityBindingElement(
                new X509SecurityTokenParameters( // protection
                    X509KeyIdentifierClauseType.Thumbprint,
                    SecurityTokenInclusionMode.Never));
            result.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11;
            result.RequireSignatureConfirmation = true;

            return result;
        }

        // this method reverses CreateAnonymousForCertificateBindingElement() logic
        internal static bool IsAnonymousForCertificateBinding(SecurityBindingElement sbe)
        {
            SymmetricSecurityBindingElement ssbe = sbe as SymmetricSecurityBindingElement;
            if (ssbe == null)
                return false;

            if (!ssbe.RequireSignatureConfirmation)
                return false;

            // Do not check MessageSecurityVersion: it maybe changed by the wrapper element and gets checked later in the SecuritySection.AreBindingsMatching()

            X509SecurityTokenParameters x509Parameters = ssbe.ProtectionTokenParameters as X509SecurityTokenParameters;
            if (x509Parameters == null || x509Parameters.X509ReferenceStyle != X509KeyIdentifierClauseType.Thumbprint || x509Parameters.InclusionMode != SecurityTokenInclusionMode.Never)
                return false;

            if (!sbe.EndpointSupportingTokenParameters.IsEmpty())
                return false;

            return true;
        }

        static public AsymmetricSecurityBindingElement CreateMutualCertificateDuplexBindingElement()
        {
            return CreateMutualCertificateDuplexBindingElement(MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11);
        }

        static public AsymmetricSecurityBindingElement CreateMutualCertificateDuplexBindingElement(MessageSecurityVersion version)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("version");
            }
            AsymmetricSecurityBindingElement result;

            if (version.SecurityVersion == SecurityVersion.WSSecurity10)
            {
                result = new AsymmetricSecurityBindingElement(
                    new X509SecurityTokenParameters( // recipient
                        X509KeyIdentifierClauseType.Any,
                        SecurityTokenInclusionMode.AlwaysToInitiator,
                        false),
                    new X509SecurityTokenParameters( // initiator
                        X509KeyIdentifierClauseType.Any,
                        SecurityTokenInclusionMode.AlwaysToRecipient,
                        false));
            }
            else
            {
                result = new AsymmetricSecurityBindingElement(
                    new X509SecurityTokenParameters( // recipient
                        X509KeyIdentifierClauseType.Thumbprint,
                        SecurityTokenInclusionMode.AlwaysToInitiator,
                        false),
                    new X509SecurityTokenParameters( // initiator
                        X509KeyIdentifierClauseType.Thumbprint,
                        SecurityTokenInclusionMode.AlwaysToRecipient,
                        false));
            }

            result.MessageSecurityVersion = version;

            return result;
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsUserNameForCertificateBinding() method.
        static public SymmetricSecurityBindingElement CreateUserNameForCertificateBindingElement()
        {
            SymmetricSecurityBindingElement result = new SymmetricSecurityBindingElement(
                new X509SecurityTokenParameters(
                    X509KeyIdentifierClauseType.Thumbprint,
                    SecurityTokenInclusionMode.Never));
            result.EndpointSupportingTokenParameters.SignedEncrypted.Add(
                new UserNameSecurityTokenParameters());
            result.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11;

            return result;
        }

        // this method reverses CreateMutualCertificateBindingElement() logic
        internal static bool IsUserNameForCertificateBinding(SecurityBindingElement sbe)
        {
            // Do not check MessageSecurityVersion: it maybe changed by the wrapper element and gets checked later in the SecuritySection.AreBindingsMatching()

            SymmetricSecurityBindingElement ssbe = sbe as SymmetricSecurityBindingElement;
            if (ssbe == null)
                return false;

            X509SecurityTokenParameters x509Parameters = ssbe.ProtectionTokenParameters as X509SecurityTokenParameters;
            if (x509Parameters == null || x509Parameters.X509ReferenceStyle != X509KeyIdentifierClauseType.Thumbprint || x509Parameters.InclusionMode != SecurityTokenInclusionMode.Never)
                return false;

            SupportingTokenParameters parameters = sbe.EndpointSupportingTokenParameters;
            if (parameters.Signed.Count != 0 || parameters.SignedEncrypted.Count != 1 || parameters.Endorsing.Count != 0 || parameters.SignedEndorsing.Count != 0)
                return false;

            UserNameSecurityTokenParameters userNameParameters = parameters.SignedEncrypted[0] as UserNameSecurityTokenParameters;
            if (userNameParameters == null)
                return false;

            return true;
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsKerberosBinding() method.
        static public SymmetricSecurityBindingElement CreateKerberosBindingElement()
        {
            SymmetricSecurityBindingElement result = new SymmetricSecurityBindingElement(
                new KerberosSecurityTokenParameters());
            result.DefaultAlgorithmSuite = SecurityAlgorithmSuite.KerberosDefault;
            return result;
        }

        // this method reverses CreateMutualCertificateBindingElement() logic
        internal static bool IsKerberosBinding(SecurityBindingElement sbe)
        {
            // do not check DefaultAlgorithmSuite match: it is often changed by the caller of CreateKerberosBindingElement
            SymmetricSecurityBindingElement ssbe = sbe as SymmetricSecurityBindingElement;
            if (ssbe == null)
                return false;

            KerberosSecurityTokenParameters parameters = ssbe.ProtectionTokenParameters as KerberosSecurityTokenParameters;
            if (parameters == null)
                return false;

            if (!sbe.EndpointSupportingTokenParameters.IsEmpty())
                return false;

            return true;
        }

        static public SymmetricSecurityBindingElement CreateSspiNegotiationBindingElement()
        {
            return CreateSspiNegotiationBindingElement(SspiSecurityTokenParameters.defaultRequireCancellation);
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsSspiNegotiationBinding() method.
        static public SymmetricSecurityBindingElement CreateSspiNegotiationBindingElement(bool requireCancellation)
        {
            SymmetricSecurityBindingElement result = new SymmetricSecurityBindingElement(
                new SspiSecurityTokenParameters(requireCancellation));
            return result;
        }

        // this method reverses CreateMutualCertificateBindingElement() logic
        internal static bool IsSspiNegotiationBinding(SecurityBindingElement sbe, bool requireCancellation)
        {
            SymmetricSecurityBindingElement ssbe = sbe as SymmetricSecurityBindingElement;

            if (ssbe == null)
                return false;

            if (!sbe.EndpointSupportingTokenParameters.IsEmpty())
                return false;

            SspiSecurityTokenParameters sspiParameters = ssbe.ProtectionTokenParameters as SspiSecurityTokenParameters;
            if (sspiParameters == null)
                return false;

            return sspiParameters.RequireCancellation == requireCancellation;
        }


        static public SymmetricSecurityBindingElement CreateSslNegotiationBindingElement(bool requireClientCertificate)
        {
            return CreateSslNegotiationBindingElement(requireClientCertificate, SslSecurityTokenParameters.defaultRequireCancellation);
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsSslNegotiationBinding() method.
        static public SymmetricSecurityBindingElement CreateSslNegotiationBindingElement(bool requireClientCertificate, bool requireCancellation)
        {
            SymmetricSecurityBindingElement result = new SymmetricSecurityBindingElement(
                new SslSecurityTokenParameters(requireClientCertificate, requireCancellation));
            return result;
        }

        // this method reverses CreateMutualCertificateBindingElement() logic
        internal static bool IsSslNegotiationBinding(SecurityBindingElement sbe, bool requireClientCertificate, bool requireCancellation)
        {
            SymmetricSecurityBindingElement ssbe = sbe as SymmetricSecurityBindingElement;
            if (ssbe == null)
                return false;

            if (!sbe.EndpointSupportingTokenParameters.IsEmpty())
                return false;

            SslSecurityTokenParameters sslParameters = ssbe.ProtectionTokenParameters as SslSecurityTokenParameters;
            if (sslParameters == null)
                return false;

            return sslParameters.RequireClientCertificate == requireClientCertificate && sslParameters.RequireCancellation == requireCancellation;
        }
        static public SymmetricSecurityBindingElement CreateIssuedTokenBindingElement(IssuedSecurityTokenParameters issuedTokenParameters)
        {
            if (issuedTokenParameters == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("issuedTokenParameters");
            if (issuedTokenParameters.KeyType != SecurityKeyType.SymmetricKey)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRServiceModel.IssuedTokenAuthenticationModeRequiresSymmetricIssuedKey);
            SymmetricSecurityBindingElement result = new SymmetricSecurityBindingElement(issuedTokenParameters);
            return result;
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsIssuedTokenForCertificateBinding() method.
        static public SymmetricSecurityBindingElement CreateIssuedTokenForCertificateBindingElement(IssuedSecurityTokenParameters issuedTokenParameters)
        {
            if (issuedTokenParameters == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("issuedTokenParameters");

            SymmetricSecurityBindingElement result = new SymmetricSecurityBindingElement(
                new X509SecurityTokenParameters(
                    X509KeyIdentifierClauseType.Thumbprint,
                    SecurityTokenInclusionMode.Never));
            if (issuedTokenParameters.KeyType == SecurityKeyType.BearerKey)
            {
                result.EndpointSupportingTokenParameters.SignedEncrypted.Add(issuedTokenParameters);
                result.MessageSecurityVersion = MessageSecurityVersion.WSSXDefault;
            }
            else
            {
                result.EndpointSupportingTokenParameters.Endorsing.Add(issuedTokenParameters);
                result.MessageSecurityVersion = MessageSecurityVersion.Default;
            }
            result.RequireSignatureConfirmation = true;
            return result;
        }

        // this method reverses CreateMutualCertificateBindingElement() logic
        internal static bool IsIssuedTokenForCertificateBinding(SecurityBindingElement sbe, out IssuedSecurityTokenParameters issuedTokenParameters)
        {
            issuedTokenParameters = null;
            SymmetricSecurityBindingElement ssbe = sbe as SymmetricSecurityBindingElement;
            if (ssbe == null)
                return false;

            if (!ssbe.RequireSignatureConfirmation)
                return false;

            // Do not check MessageSecurityVersion: it maybe changed by the wrapper element and gets checked later in the SecuritySection.AreBindingsMatching()

            X509SecurityTokenParameters x509Parameters = ssbe.ProtectionTokenParameters as X509SecurityTokenParameters;
            if (x509Parameters == null || x509Parameters.X509ReferenceStyle != X509KeyIdentifierClauseType.Thumbprint || x509Parameters.InclusionMode != SecurityTokenInclusionMode.Never)
                return false;

            SupportingTokenParameters parameters = ssbe.EndpointSupportingTokenParameters;
            if (parameters.Signed.Count != 0 || (parameters.SignedEncrypted.Count == 0 && parameters.Endorsing.Count == 0) || parameters.SignedEndorsing.Count != 0)
                return false;

            if ((parameters.SignedEncrypted.Count == 1) && (parameters.Endorsing.Count == 0))
            {
                issuedTokenParameters = parameters.SignedEncrypted[0] as IssuedSecurityTokenParameters;
                if (issuedTokenParameters != null && issuedTokenParameters.KeyType != SecurityKeyType.BearerKey)
                    return false;
            }
            else if ((parameters.Endorsing.Count == 1) && (parameters.SignedEncrypted.Count == 0))
            {
                issuedTokenParameters = parameters.Endorsing[0] as IssuedSecurityTokenParameters;
                if (issuedTokenParameters != null && (issuedTokenParameters.KeyType != SecurityKeyType.SymmetricKey && issuedTokenParameters.KeyType != SecurityKeyType.AsymmetricKey))
                    return false;
            }
            return (issuedTokenParameters != null);
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsIssuedTokenForSslBinding() method.
        static public SymmetricSecurityBindingElement CreateIssuedTokenForSslBindingElement(IssuedSecurityTokenParameters issuedTokenParameters)
        {
            return CreateIssuedTokenForSslBindingElement(issuedTokenParameters, SslSecurityTokenParameters.defaultRequireCancellation);
        }

        // this method reverses CreateMutualCertificateBindingElement() logic
        internal static bool IsIssuedTokenForSslBinding(SecurityBindingElement sbe, out IssuedSecurityTokenParameters issuedTokenParameters)
        {
            return IsIssuedTokenForSslBinding(sbe, SslSecurityTokenParameters.defaultRequireCancellation, out issuedTokenParameters);
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsIssuedTokenForSslBinding() method.
        static public SymmetricSecurityBindingElement CreateIssuedTokenForSslBindingElement(IssuedSecurityTokenParameters issuedTokenParameters, bool requireCancellation)
        {
            if (issuedTokenParameters == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("issuedTokenParameters");

            SymmetricSecurityBindingElement result = new SymmetricSecurityBindingElement(
                new SslSecurityTokenParameters(false, requireCancellation));
            if (issuedTokenParameters.KeyType == SecurityKeyType.BearerKey)
            {
                result.EndpointSupportingTokenParameters.SignedEncrypted.Add(issuedTokenParameters);
                result.MessageSecurityVersion = MessageSecurityVersion.WSSXDefault;
            }
            else
            {
                result.EndpointSupportingTokenParameters.Endorsing.Add(issuedTokenParameters);
                result.MessageSecurityVersion = MessageSecurityVersion.Default;
            }
            result.RequireSignatureConfirmation = true;
            return result;
        }

        // this method reverses CreateMutualCertificateBindingElement() logic
        internal static bool IsIssuedTokenForSslBinding(SecurityBindingElement sbe, bool requireCancellation, out IssuedSecurityTokenParameters issuedTokenParameters)
        {
            issuedTokenParameters = null;
            SymmetricSecurityBindingElement ssbe = sbe as SymmetricSecurityBindingElement;
            if (ssbe == null)
                return false;

            if (!ssbe.RequireSignatureConfirmation)
                return false;

            // Do not check MessageSecurityVersion: it maybe changed by the wrapper element and gets checked later in the SecuritySection.AreBindingsMatching()

            SslSecurityTokenParameters sslParameters = ssbe.ProtectionTokenParameters as SslSecurityTokenParameters;
            if (sslParameters == null)
                return false;

            if (sslParameters.RequireClientCertificate || sslParameters.RequireCancellation != requireCancellation)
                return false;

            SupportingTokenParameters parameters = ssbe.EndpointSupportingTokenParameters;
            if (parameters.Signed.Count != 0 || (parameters.SignedEncrypted.Count == 0 && parameters.Endorsing.Count == 0) || parameters.SignedEndorsing.Count != 0)
                return false;

            if ((parameters.SignedEncrypted.Count == 1) && (parameters.Endorsing.Count == 0))
            {
                issuedTokenParameters = parameters.SignedEncrypted[0] as IssuedSecurityTokenParameters;
                if (issuedTokenParameters != null && issuedTokenParameters.KeyType != SecurityKeyType.BearerKey)
                    return false;
            }
            else if ((parameters.Endorsing.Count == 1) && (parameters.SignedEncrypted.Count == 0))
            {
                issuedTokenParameters = parameters.Endorsing[0] as IssuedSecurityTokenParameters;
                if (issuedTokenParameters != null && (issuedTokenParameters.KeyType != SecurityKeyType.SymmetricKey && issuedTokenParameters.KeyType != SecurityKeyType.AsymmetricKey))
                    return false;
            }
            return (issuedTokenParameters != null);
        }

        static public SymmetricSecurityBindingElement CreateUserNameForSslBindingElement()
        {
            return CreateUserNameForSslBindingElement(SslSecurityTokenParameters.defaultRequireCancellation);
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsUserNameForSslBinding() method.
        static public SymmetricSecurityBindingElement CreateUserNameForSslBindingElement(bool requireCancellation)
        {
            SymmetricSecurityBindingElement result = new SymmetricSecurityBindingElement(
                new SslSecurityTokenParameters(false, requireCancellation));
            result.EndpointSupportingTokenParameters.SignedEncrypted.Add(
                new UserNameSecurityTokenParameters());
            result.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11;

            return result;
        }

        // this method reverses CreateMutualCertificateBindingElement() logic
        internal static bool IsUserNameForSslBinding(SecurityBindingElement sbe, bool requireCancellation)
        {
            // Do not check MessageSecurityVersion: it maybe changed by the wrapper element and gets checked later in the SecuritySection.AreBindingsMatching()

            SymmetricSecurityBindingElement ssbe = sbe as SymmetricSecurityBindingElement;
            if (ssbe == null)
                return false;

            SupportingTokenParameters parameters = sbe.EndpointSupportingTokenParameters;
            if (parameters.Signed.Count != 0 || parameters.SignedEncrypted.Count != 1 || parameters.Endorsing.Count != 0 || parameters.SignedEndorsing.Count != 0)
                return false;

            if (!(parameters.SignedEncrypted[0] is UserNameSecurityTokenParameters))
                return false;

            SslSecurityTokenParameters sslParameters = ssbe.ProtectionTokenParameters as SslSecurityTokenParameters;
            if (sslParameters == null)
                return false;

            return sslParameters.RequireCancellation == requireCancellation && !sslParameters.RequireClientCertificate;
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsUserNameOverTransportBinding() method.
        static public TransportSecurityBindingElement CreateUserNameOverTransportBindingElement()
        {
            TransportSecurityBindingElement result = new TransportSecurityBindingElement();
            result.EndpointSupportingTokenParameters.SignedEncrypted.Add(
                new UserNameSecurityTokenParameters());
            result.IncludeTimestamp = true;
            result.LocalClientSettings.DetectReplays = false;
            result.LocalServiceSettings.DetectReplays = false;
            return result;
        }

        // this method reverses CreateMutualCertificateBindingElement() logic
        internal static bool IsUserNameOverTransportBinding(SecurityBindingElement sbe)
        {
            // do not check local settings: sbe.LocalServiceSettings and sbe.LocalClientSettings
            if (!sbe.IncludeTimestamp)
                return false;

            if (!(sbe is TransportSecurityBindingElement))
                return false;

            SupportingTokenParameters parameters = sbe.EndpointSupportingTokenParameters;
            if (parameters.Signed.Count != 0 || parameters.SignedEncrypted.Count != 1 || parameters.Endorsing.Count != 0 || parameters.SignedEndorsing.Count != 0)
                return false;

            UserNameSecurityTokenParameters userNameParameters = parameters.SignedEncrypted[0] as UserNameSecurityTokenParameters;
            if (userNameParameters == null)
                return false;

            return true;
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsCertificateOverTransportBinding() method.
        static public TransportSecurityBindingElement CreateCertificateOverTransportBindingElement()
        {
            return CreateCertificateOverTransportBindingElement(MessageSecurityVersion.Default);
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsCertificateOverTransportBinding() method.
        static public TransportSecurityBindingElement CreateCertificateOverTransportBindingElement(MessageSecurityVersion version)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("version");
            }
            X509KeyIdentifierClauseType x509ReferenceType;

            if (version.SecurityVersion == SecurityVersion.WSSecurity10)
            {
                x509ReferenceType = X509KeyIdentifierClauseType.Any;
            }
            else
            {
                x509ReferenceType = X509KeyIdentifierClauseType.Thumbprint;
            }

            TransportSecurityBindingElement result = new TransportSecurityBindingElement();
            X509SecurityTokenParameters x509Parameters = new X509SecurityTokenParameters(
                    x509ReferenceType,
                    SecurityTokenInclusionMode.AlwaysToRecipient,
                    false);
            result.EndpointSupportingTokenParameters.Endorsing.Add(
                x509Parameters
                );
            result.IncludeTimestamp = true;
            result.LocalClientSettings.DetectReplays = false;
            result.LocalServiceSettings.DetectReplays = false;
            result.MessageSecurityVersion = version;

            return result;
        }

        // this method reverses CreateMutualCertificateBindingElement() logic
        internal static bool IsCertificateOverTransportBinding(SecurityBindingElement sbe)
        {
            // do not check local settings: sbe.LocalServiceSettings and sbe.LocalClientSettings
            if (!sbe.IncludeTimestamp)
                return false;

            if (!(sbe is TransportSecurityBindingElement))
                return false;

            SupportingTokenParameters parameters = sbe.EndpointSupportingTokenParameters;
            if (parameters.Signed.Count != 0 || parameters.SignedEncrypted.Count != 0 || parameters.Endorsing.Count != 1 || parameters.SignedEndorsing.Count != 0)
                return false;

            X509SecurityTokenParameters x509Parameters = parameters.Endorsing[0] as X509SecurityTokenParameters;
            if (x509Parameters == null)
                return false;

            if (x509Parameters.InclusionMode != SecurityTokenInclusionMode.AlwaysToRecipient)
                return false;

            return x509Parameters.X509ReferenceStyle == X509KeyIdentifierClauseType.Any || x509Parameters.X509ReferenceStyle == X509KeyIdentifierClauseType.Thumbprint;
        }

        static public TransportSecurityBindingElement CreateKerberosOverTransportBindingElement()
        {
            TransportSecurityBindingElement result = new TransportSecurityBindingElement();
            KerberosSecurityTokenParameters kerberosParameters = new KerberosSecurityTokenParameters();
            kerberosParameters.RequireDerivedKeys = false;
            result.EndpointSupportingTokenParameters.Endorsing.Add(
                kerberosParameters);
            result.IncludeTimestamp = true;
            result.LocalClientSettings.DetectReplays = false;
            result.LocalServiceSettings.DetectReplays = false;
            result.DefaultAlgorithmSuite = SecurityAlgorithmSuite.KerberosDefault;
            result.SupportsExtendedProtectionPolicy = true;

            return result;
        }
#if NO
        // this is reversing of the CreateKerberosOverTransportBindingElement() logic
        static bool IsKerberosOverTransportBinding(SecurityBindingElement sbe)
        {
            if (sbe.DefaultAlgorithmSuite != SecurityAlgorithmSuite.KerberosDefault)
                return false;

            // do not check local settings: sbe.LocalServiceSettings and sbe.LocalClientSettings

            if (!sbe.IncludeTimestamp)
                return false;

            if (!(sbe is TransportSecurityBindingElement))
                return false;

            SupportingTokenParameters parameters = sbe.EndpointSupportingTokenParameters;
            if (parameters.Signed.Count != 0 || parameters.SignedEncrypted.Count != 0 || parameters.Endorsing.Count != 1 || parameters.SignedEndorsing.Count != 0)
                return false;

            KerberosSecurityTokenParameters kerberosParameters = parameters.Endorsing[0] as KerberosSecurityTokenParameters;
            if (kerberosParameters == null)
                return false;

            if (kerberosParameters.RequireDerivedKeys)
                return false;

            return true;
        }
#endif
        static public TransportSecurityBindingElement CreateSspiNegotiationOverTransportBindingElement()
        {
            return CreateSspiNegotiationOverTransportBindingElement(true);
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsSspiNegotiationOverTransportBinding() method.
        static public TransportSecurityBindingElement CreateSspiNegotiationOverTransportBindingElement(bool requireCancellation)
        {
            TransportSecurityBindingElement result = new TransportSecurityBindingElement();
            SspiSecurityTokenParameters sspiParameters = new SspiSecurityTokenParameters(requireCancellation);
            sspiParameters.RequireDerivedKeys = false;
            result.EndpointSupportingTokenParameters.Endorsing.Add(
                sspiParameters);
            result.IncludeTimestamp = true;
            result.LocalClientSettings.DetectReplays = false;
            result.LocalServiceSettings.DetectReplays = false;
            result.SupportsExtendedProtectionPolicy = true;

            return result;
        }

        // this method reverses CreateSspiNegotiationOverTransportBindingElement() logic
        internal static bool IsSspiNegotiationOverTransportBinding(SecurityBindingElement sbe, bool requireCancellation)
        {
            // do not check local settings: sbe.LocalServiceSettings and sbe.LocalClientSettings

            if (!sbe.IncludeTimestamp)
                return false;

            SupportingTokenParameters parameters = sbe.EndpointSupportingTokenParameters;
            if (parameters.Signed.Count != 0 || parameters.SignedEncrypted.Count != 0 || parameters.Endorsing.Count != 1 || parameters.SignedEndorsing.Count != 0)
                return false;
            SspiSecurityTokenParameters sspiParameters = parameters.Endorsing[0] as SspiSecurityTokenParameters;
            if (sspiParameters == null)
                return false;

            if (sspiParameters.RequireDerivedKeys)
                return false;

            if (sspiParameters.RequireCancellation != requireCancellation)
                return false;

            if (!(sbe is TransportSecurityBindingElement))
                return false;

            return true;
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsIssuedTokenOverTransportBinding() method.
        static public TransportSecurityBindingElement CreateIssuedTokenOverTransportBindingElement(IssuedSecurityTokenParameters issuedTokenParameters)
        {
            if (issuedTokenParameters == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("issuedTokenParameters");

            issuedTokenParameters.RequireDerivedKeys = false;
            TransportSecurityBindingElement result = new TransportSecurityBindingElement();
            if (issuedTokenParameters.KeyType == SecurityKeyType.BearerKey)
            {
                result.EndpointSupportingTokenParameters.Signed.Add(issuedTokenParameters);
                result.MessageSecurityVersion = MessageSecurityVersion.WSSXDefault;
            }
            else
            {
                result.EndpointSupportingTokenParameters.Endorsing.Add(issuedTokenParameters);
                result.MessageSecurityVersion = MessageSecurityVersion.Default;
            }
            result.LocalClientSettings.DetectReplays = false;
            result.LocalServiceSettings.DetectReplays = false;
            result.IncludeTimestamp = true;

            return result;
        }

        // this method reverses CreateIssuedTokenOverTransportBindingElement() logic
        internal static bool IsIssuedTokenOverTransportBinding(SecurityBindingElement sbe, out IssuedSecurityTokenParameters issuedTokenParameters)
        {
            issuedTokenParameters = null;
            if (!(sbe is TransportSecurityBindingElement))
                return false;

            if (!sbe.IncludeTimestamp)
                return false;

            // do not check local settings: sbe.LocalServiceSettings and sbe.LocalClientSettings

            SupportingTokenParameters parameters = sbe.EndpointSupportingTokenParameters;
            if (parameters.SignedEncrypted.Count != 0 || (parameters.Signed.Count == 0 && parameters.Endorsing.Count == 0) || parameters.SignedEndorsing.Count != 0)
                return false;
            if ((parameters.Signed.Count == 1) && (parameters.Endorsing.Count == 0))
            {
                issuedTokenParameters = parameters.Signed[0] as IssuedSecurityTokenParameters;
                if (issuedTokenParameters != null && issuedTokenParameters.KeyType != SecurityKeyType.BearerKey)
                    return false;
            }
            else if ((parameters.Endorsing.Count == 1) && (parameters.Signed.Count == 0))
            {
                issuedTokenParameters = parameters.Endorsing[0] as IssuedSecurityTokenParameters;
                if (issuedTokenParameters != null && (issuedTokenParameters.KeyType != SecurityKeyType.SymmetricKey && issuedTokenParameters.KeyType != SecurityKeyType.AsymmetricKey))
                    return false;
            }
            if (issuedTokenParameters == null)
                return false;
            if (issuedTokenParameters.RequireDerivedKeys)
                return false;

            return true;
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsSecureConversationBinding() method.
        static public SecurityBindingElement CreateSecureConversationBindingElement(SecurityBindingElement bootstrapSecurity)
        {
            return CreateSecureConversationBindingElement(bootstrapSecurity, SecureConversationSecurityTokenParameters.defaultRequireCancellation, null);
        }

        // this method reverses CreateSecureConversationBindingElement() logic
        internal static bool IsSecureConversationBinding(SecurityBindingElement sbe, out SecurityBindingElement bootstrapSecurity)
        {
            return IsSecureConversationBinding(sbe, SecureConversationSecurityTokenParameters.defaultRequireCancellation, out bootstrapSecurity);
        }

        static public SecurityBindingElement CreateSecureConversationBindingElement(SecurityBindingElement bootstrapSecurity, bool requireCancellation)
        {
            return CreateSecureConversationBindingElement(bootstrapSecurity, requireCancellation, null);
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsSecureConversationBinding() method.
        static public SecurityBindingElement CreateSecureConversationBindingElement(SecurityBindingElement bootstrapSecurity, bool requireCancellation, ChannelProtectionRequirements bootstrapProtectionRequirements)
        {
            if (bootstrapSecurity == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bootstrapBinding");

            SecurityBindingElement result;

            if (bootstrapSecurity is TransportSecurityBindingElement)
            {
                // there is no need to do replay detection or key derivation for transport bindings
                TransportSecurityBindingElement primary = new TransportSecurityBindingElement();
                SecureConversationSecurityTokenParameters scParameters = new SecureConversationSecurityTokenParameters(
                        bootstrapSecurity,
                        requireCancellation,
                        bootstrapProtectionRequirements);
                scParameters.RequireDerivedKeys = false;
                primary.EndpointSupportingTokenParameters.Endorsing.Add(
                    scParameters);
                primary.LocalClientSettings.DetectReplays = false;
                primary.LocalServiceSettings.DetectReplays = false;
                primary.IncludeTimestamp = true;
                result = primary;
            }
            else // Symmetric- or AsymmetricSecurityBindingElement
            {
                SymmetricSecurityBindingElement primary = new SymmetricSecurityBindingElement(
                    new SecureConversationSecurityTokenParameters(
                        bootstrapSecurity,
                        requireCancellation,
                        bootstrapProtectionRequirements));
                // there is no need for signature confirmation on the steady state binding
                primary.RequireSignatureConfirmation = false;
                result = primary;
            }

            return result;
        }

        // this method reverses CreateSecureConversationBindingElement() logic
        internal static bool IsSecureConversationBinding(SecurityBindingElement sbe, bool requireCancellation, out SecurityBindingElement bootstrapSecurity)
        {
            bootstrapSecurity = null;
            SymmetricSecurityBindingElement ssbe = sbe as SymmetricSecurityBindingElement;
            if (ssbe != null)
            {
                if (ssbe.RequireSignatureConfirmation)
                    return false;

                SecureConversationSecurityTokenParameters parameters = ssbe.ProtectionTokenParameters as SecureConversationSecurityTokenParameters;
                if (parameters == null)
                    return false;
                if (parameters.RequireCancellation != requireCancellation)
                    return false;
                bootstrapSecurity = parameters.BootstrapSecurityBindingElement;
            }
            else
            {
                if (!sbe.IncludeTimestamp)
                    return false;

                // do not check local settings: sbe.LocalServiceSettings and sbe.LocalClientSettings

                if (!(sbe is TransportSecurityBindingElement))
                    return false;

                SupportingTokenParameters parameters = sbe.EndpointSupportingTokenParameters;
                if (parameters.Signed.Count != 0 || parameters.SignedEncrypted.Count != 0 || parameters.Endorsing.Count != 1 || parameters.SignedEndorsing.Count != 0)
                    return false;
                SecureConversationSecurityTokenParameters scParameters = parameters.Endorsing[0] as SecureConversationSecurityTokenParameters;
                if (scParameters == null)
                    return false;

                if (scParameters.RequireCancellation != requireCancellation)
                    return false;

                bootstrapSecurity = scParameters.BootstrapSecurityBindingElement;
            }

            if (bootstrapSecurity != null && bootstrapSecurity.SecurityHeaderLayout != SecurityProtocolFactory.defaultSecurityHeaderLayout)
                return false;

            return bootstrapSecurity != null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0}:", this.GetType().ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "DefaultAlgorithmSuite: {0}", _defaultAlgorithmSuite.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "IncludeTimestamp: {0}", _includeTimestamp.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "KeyEntropyMode: {0}", _keyEntropyMode.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "MessageSecurityVersion: {0}", this.MessageSecurityVersion.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "SecurityHeaderLayout: {0}", _securityHeaderLayout.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "ProtectTokens: {0}", _protectTokens.ToString()));
            sb.AppendLine("EndpointSupportingTokenParameters:");
            sb.AppendLine("  " + this.EndpointSupportingTokenParameters.ToString().Trim().Replace("\n", "\n  "));
            sb.AppendLine("OptionalEndpointSupportingTokenParameters:");
            sb.AppendLine("  " + this.OptionalEndpointSupportingTokenParameters.ToString().Trim().Replace("\n", "\n  "));
            if (_operationSupportingTokenParameters.Count == 0)
            {
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "OperationSupportingTokenParameters: none"));
            }
            else
            {
                foreach (string requestAction in this.OperationSupportingTokenParameters.Keys)
                {
                    sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "OperationSupportingTokenParameters[\"{0}\"]:", requestAction));
                    sb.AppendLine("  " + this.OperationSupportingTokenParameters[requestAction].ToString().Trim().Replace("\n", "\n  "));
                }
            }
            if (_optionalOperationSupportingTokenParameters.Count == 0)
            {
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "OptionalOperationSupportingTokenParameters: none"));
            }
            else
            {
                foreach (string requestAction in this.OptionalOperationSupportingTokenParameters.Keys)
                {
                    sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "OptionalOperationSupportingTokenParameters[\"{0}\"]:", requestAction));
                    sb.AppendLine("  " + this.OptionalOperationSupportingTokenParameters[requestAction].ToString().Trim().Replace("\n", "\n  "));
                }
            }

            return sb.ToString().Trim();
        }


        internal static ChannelProtectionRequirements ComputeProtectionRequirements(SecurityBindingElement security, BindingParameterCollection parameterCollection, BindingElementCollection bindingElements, bool isForService)
        {
            if (parameterCollection == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameterCollection");
            if (bindingElements == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bindingElements");
            if (security == null)
            {
                return null;
            }

            ChannelProtectionRequirements result = null;
            if ((security is SymmetricSecurityBindingElement) || (security is AsymmetricSecurityBindingElement))
            {
                result = new ChannelProtectionRequirements();
                ChannelProtectionRequirements contractRequirements = parameterCollection.Find<ChannelProtectionRequirements>();

                if (contractRequirements != null)
                    result.Add(contractRequirements);

                AddBindingProtectionRequirements(result, bindingElements, !isForService);
            }

            return result;
        }

        private static void AddBindingProtectionRequirements(ChannelProtectionRequirements requirements, BindingElementCollection bindingElements, bool isForChannel)
        {
            // Gather custom requirements from bindingElements
            CustomBinding binding = new CustomBinding(bindingElements);
            BindingContext context = new BindingContext(binding, new BindingParameterCollection());
            // In theory, we can just do 
            //     context.GetInnerProperty<ChannelProtectionRequirements>()
            // but that relies on each binding element to correctly union-up its own requirements with
            // those of the rest of the stack.  So instead, we ask each BE individually, and we do the 
            // work of combining the results.  This protects us against this scenario: someone authors "FooBE"
            // with a a GetProperty implementation that always returns null (oops), and puts FooBE on the 
            // top of the stack, and so FooBE "hides" important protection requirements that inner BEs
            // require, resulting in an insecure binding.
            foreach (BindingElement bindingElement in bindingElements)
            {
                if (bindingElement != null)
                {
                    // ask each element individually for its requirements
                    context.RemainingBindingElements.Clear();
                    context.RemainingBindingElements.Add(bindingElement);
                    ChannelProtectionRequirements s = context.GetInnerProperty<ChannelProtectionRequirements>();
                    if (s != null)
                    {
                        //if (isForChannel)
                        //{
                        //    requirements.Add(s.CreateInverse());
                        //}
                        //else
                        //{
                        requirements.Add(s);
                        //}
                    }
                }
            }
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (b == null)
                return false;

            SecurityBindingElement security = b as SecurityBindingElement;
            if (security == null)
                return false;
            return System.ServiceModel.Configuration.SecurityElement.AreBindingsMatching(this, security);
        }

        private static void AddAssertionIfNotNull(PolicyConversionContext policyContext, XmlElement assertion)
        {
            if (policyContext != null && assertion != null)
            {
                policyContext.GetBindingAssertions().Add(assertion);
            }
        }

        private static void AddAssertionIfNotNull(PolicyConversionContext policyContext, Collection<XmlElement> assertions)
        {
            if (policyContext != null && assertions != null)
            {
                PolicyAssertionCollection existingAssertions = policyContext.GetBindingAssertions();
                for (int i = 0; i < assertions.Count; ++i)
                    existingAssertions.Add(assertions[i]);
            }
        }

        private static void AddAssertionIfNotNull(PolicyConversionContext policyContext, OperationDescription operation, XmlElement assertion)
        {
            if (policyContext != null && assertion != null)
            {
                policyContext.GetOperationBindingAssertions(operation).Add(assertion);
            }
        }

        private static void AddAssertionIfNotNull(PolicyConversionContext policyContext, OperationDescription operation, Collection<XmlElement> assertions)
        {
            if (policyContext != null && assertions != null)
            {
                PolicyAssertionCollection existingAssertions = policyContext.GetOperationBindingAssertions(operation);
                for (int i = 0; i < assertions.Count; ++i)
                    existingAssertions.Add(assertions[i]);
            }
        }

        private static void AddAssertionIfNotNull(PolicyConversionContext policyContext, MessageDescription message, XmlElement assertion)
        {
            if (policyContext != null && assertion != null)
            {
                policyContext.GetMessageBindingAssertions(message).Add(assertion);
            }
        }

        private static void AddAssertionIfNotNull(PolicyConversionContext policyContext, FaultDescription message, XmlElement assertion)
        {
            if (policyContext != null && assertion != null)
            {
                policyContext.GetFaultBindingAssertions(message).Add(assertion);
            }
        }
    }
}
