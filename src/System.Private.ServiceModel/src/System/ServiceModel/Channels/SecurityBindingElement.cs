// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Selectors;
using System.Net.Security;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Xml;

namespace System.ServiceModel.Channels
{
    public abstract class SecurityBindingElement : BindingElement
    {
        internal const bool defaultIncludeTimestamp = true;
        internal const bool defaultAllowInsecureTransport = false;
        internal const bool defaultRequireSignatureConfirmation = false;
        internal const bool defaultEnableUnsecuredResponse = false;
        internal const bool defaultProtectTokens = false;

        private SecurityAlgorithmSuite _defaultAlgorithmSuite;
        private SupportingTokenParameters _endpointSupportingTokenParameters;
        private SupportingTokenParameters _optionalEndpointSupportingTokenParameters;
        private bool _includeTimestamp;
        Dictionary<string, SupportingTokenParameters> _operationSupportingTokenParameters;
        Dictionary<string, SupportingTokenParameters> _optionalOperationSupportingTokenParameters;
        private LocalClientSecuritySettings _localClientSettings;

        private MessageSecurityVersion _messageSecurityVersion;
        private SecurityHeaderLayout _securityHeaderLayout;
        private long _maxReceivedMessageSize = TransportDefaults.MaxReceivedMessageSize;
        private XmlDictionaryReaderQuotas _readerQuotas;
        private bool _enableUnsecuredResponse;
        private bool _protectTokens = defaultProtectTokens;

        internal SecurityBindingElement()
            : base()
        {
            _messageSecurityVersion = MessageSecurityVersion.Default;
            _includeTimestamp = defaultIncludeTimestamp;
            _defaultAlgorithmSuite = SecurityAlgorithmSuite.Default;
            _localClientSettings = new LocalClientSecuritySettings();
            _endpointSupportingTokenParameters = new SupportingTokenParameters();
            _optionalEndpointSupportingTokenParameters = new SupportingTokenParameters();
            _operationSupportingTokenParameters = new Dictionary<string, SupportingTokenParameters>();
            _optionalOperationSupportingTokenParameters = new Dictionary<string, SupportingTokenParameters>();
            _securityHeaderLayout = SecurityProtocolFactory.defaultSecurityHeaderLayout;
            _enableUnsecuredResponse = defaultEnableUnsecuredResponse;
        }

        internal SecurityBindingElement(SecurityBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            if (elementToBeCloned == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("elementToBeCloned");

            _includeTimestamp = elementToBeCloned._includeTimestamp;
            _defaultAlgorithmSuite = elementToBeCloned._defaultAlgorithmSuite;
            _messageSecurityVersion = elementToBeCloned._messageSecurityVersion;
            _securityHeaderLayout = elementToBeCloned._securityHeaderLayout;
            _endpointSupportingTokenParameters = elementToBeCloned._endpointSupportingTokenParameters.Clone();
            _optionalEndpointSupportingTokenParameters = (SupportingTokenParameters)elementToBeCloned._optionalEndpointSupportingTokenParameters.Clone();
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
            _maxReceivedMessageSize = elementToBeCloned._maxReceivedMessageSize;
            _readerQuotas = elementToBeCloned._readerQuotas;
            _enableUnsecuredResponse = elementToBeCloned._enableUnsecuredResponse;
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

        public LocalClientSecuritySettings LocalClientSettings
        {
            get
            {
                return _localClientSettings;
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

        static BindingContext CreateIssuerBindingContextForNegotiation(BindingContext issuerBindingContext)
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
            //TransportBindingElement transport = issuerBindingContext.RemainingBindingElements.Find<TransportBindingElement>();
            //if (transport == null)
            //{
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.TransportBindingElementNotFound)));
            //}
            //ChannelDemuxerBindingElement demuxer = null;
            //// pick the demuxer above transport (i.e. the last demuxer in the array)
            //for (int i = 0; i < issuerBindingContext.RemainingBindingElements.Count; ++i)
            //{
            //    if (issuerBindingContext.RemainingBindingElements[i] is ChannelDemuxerBindingElement)
            //    {
            //        demuxer = (ChannelDemuxerBindingElement)issuerBindingContext.RemainingBindingElements[i];
            //    }
            //}
            //if (demuxer == null)
            //{
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ChannelDemuxerBindingElementNotFound)));
            //}
            //BindingElementCollection negotiationBindingElements = new BindingElementCollection();
            //negotiationBindingElements.Add(demuxer.Clone());
            //negotiationBindingElements.Add(transport.Clone());
            //CustomBinding binding = new CustomBinding(negotiationBindingElements);
            //binding.OpenTimeout = issuerBindingContext.Binding.OpenTimeout;
            //binding.CloseTimeout = issuerBindingContext.Binding.CloseTimeout;
            //binding.SendTimeout = issuerBindingContext.Binding.SendTimeout;
            //binding.ReceiveTimeout = issuerBindingContext.Binding.ReceiveTimeout;
            //if (issuerBindingContext.ListenUriBaseAddress != null)
            //{
            //    return new BindingContext(binding, new BindingParameterCollection(issuerBindingContext.BindingParameters), issuerBindingContext.ListenUriBaseAddress,
            //        issuerBindingContext.ListenUriRelativeAddress, issuerBindingContext.ListenUriMode);
            //}
            //else
            //{
            //    return new BindingContext(binding, new BindingParameterCollection(issuerBindingContext.BindingParameters));
            //}
        }

        protected static void SetIssuerBindingContextIfRequired(SecurityTokenParameters parameters, BindingContext issuerBindingContext)
        {
            if (parameters is SslSecurityTokenParameters)
            {
                ((SslSecurityTokenParameters)parameters).IssuerBindingContext = CreateIssuerBindingContextForNegotiation(issuerBindingContext);
            }
            else if (parameters is SspiSecurityTokenParameters)
            {
                ((SspiSecurityTokenParameters)parameters).IssuerBindingContext = CreateIssuerBindingContextForNegotiation(issuerBindingContext);
            }
        }

        static void SetIssuerBindingContextIfRequired(SupportingTokenParameters supportingParameters, BindingContext issuerBindingContext)
        {
            for (int i = 0; i < supportingParameters.Endorsing.Count; ++i)
            {
                SetIssuerBindingContextIfRequired(supportingParameters.Endorsing[i], issuerBindingContext);
            }
            for (int i = 0; i < supportingParameters.SignedEndorsing.Count; ++i)
            {
                SetIssuerBindingContextIfRequired(supportingParameters.SignedEndorsing[i], issuerBindingContext);
            }
            for (int i = 0; i < supportingParameters.Signed.Count; ++i)
            {
                SetIssuerBindingContextIfRequired(supportingParameters.Signed[i], issuerBindingContext);
            }
            for (int i = 0; i < supportingParameters.SignedEncrypted.Count; ++i)
            {
                SetIssuerBindingContextIfRequired(supportingParameters.SignedEncrypted[i], issuerBindingContext);
            }
        }

        void SetIssuerBindingContextIfRequired(BindingContext issuerBindingContext)
        {
            SetIssuerBindingContextIfRequired(this.EndpointSupportingTokenParameters, issuerBindingContext);
            SetIssuerBindingContextIfRequired(this.OptionalEndpointSupportingTokenParameters, issuerBindingContext);
            foreach (SupportingTokenParameters parameters in this.OperationSupportingTokenParameters.Values)
            {
                SetIssuerBindingContextIfRequired(parameters, issuerBindingContext);
            }
            foreach (SupportingTokenParameters parameters in this.OptionalOperationSupportingTokenParameters.Values)
            {
                SetIssuerBindingContextIfRequired(parameters, issuerBindingContext);
            }
        }

        internal bool RequiresChannelDemuxer(SecurityTokenParameters parameters)
        {
            throw ExceptionHelper.PlatformNotSupported("RequiresChannelDemuxer is not supported.");
        }

        internal virtual bool RequiresChannelDemuxer()
        {
            foreach (SecurityTokenParameters parameters in EndpointSupportingTokenParameters.Endorsing)
            {
                if (RequiresChannelDemuxer(parameters))
                {
                    return true;
                }
            }
            foreach (SecurityTokenParameters parameters in EndpointSupportingTokenParameters.SignedEndorsing)
            {
                if (RequiresChannelDemuxer(parameters))
                {
                    return true;
                }
            }

            return false;
        }

        internal abstract SecurityProtocolFactory CreateSecurityProtocolFactory<TChannel>(BindingContext context, SecurityCredentialsManager credentialsManager,
                                                                                          bool isForService, BindingContext issuanceBindingContext);

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");

            if (!this.CanBuildChannelFactory<TChannel>(context))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.ChannelTypeNotSupported, typeof(TChannel)), "TChannel"));
            }

            _readerQuotas = context.GetInnerProperty<XmlDictionaryReaderQuotas>();
            if (_readerQuotas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.EncodingBindingElementDoesNotHandleReaderQuotas)));
            }

            TransportBindingElement transportBindingElement = null;

            if (context.RemainingBindingElements != null)
                transportBindingElement = context.RemainingBindingElements.Find<TransportBindingElement>();

            if (transportBindingElement != null)
                _maxReceivedMessageSize = transportBindingElement.MaxReceivedMessageSize;

            IChannelFactory<TChannel> result = this.BuildChannelFactoryCore<TChannel>(context);

            return result;
        }

        protected abstract IChannelFactory<TChannel> BuildChannelFactoryCore<TChannel>(BindingContext context);

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");

            if (this.SessionMode)
            {
                return this.CanBuildSessionChannelFactory<TChannel>(context);
            }

            if (!context.CanBuildInnerChannelFactory<TChannel>())
            {
                return false;
            }

            return typeof(TChannel) == typeof(IOutputChannel) || typeof(TChannel) == typeof(IOutputSessionChannel) ||
                (this.SupportsDuplex && (typeof(TChannel) == typeof(IDuplexChannel) || typeof(TChannel) == typeof(IDuplexSessionChannel))) ||
                (this.SupportsRequestReply && (typeof(TChannel) == typeof(IRequestChannel) || typeof(TChannel) == typeof(IRequestSessionChannel)));
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
            EndpointSupportingTokenParameters.SetKeyDerivation(requireDerivedKeys);
        }

        internal virtual bool IsSetKeyDerivation(bool requireDerivedKeys)
        {
            if (!EndpointSupportingTokenParameters.IsSetKeyDerivation(requireDerivedKeys))
                return false;

            return true;
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

            throw ExceptionHelper.PlatformNotSupported("SecurityBindingElement.CreateMutualCertificateBindingElement is not supported.");
        }


        // this method reverses CreateMutualCertificateBindingElement() logic
        internal static bool IsMutualCertificateBinding(SecurityBindingElement sbe, bool allowSerializedSigningTokenOnReply)
        {
            throw ExceptionHelper.PlatformNotSupported("SecurityBindingElement.IsMutualCertificateBinding is not supported.");
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsUserNameOverTransportBinding() method.
        static public TransportSecurityBindingElement CreateUserNameOverTransportBindingElement()
        {
            TransportSecurityBindingElement result = new TransportSecurityBindingElement();
            result.EndpointSupportingTokenParameters.SignedEncrypted.Add(
                new UserNameSecurityTokenParameters());
            result.IncludeTimestamp = true;

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
        static public TransportSecurityBindingElement CreateCertificateOverTransportBindingElement(MessageSecurityVersion version)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("version");
            }

            throw ExceptionHelper.PlatformNotSupported("SecurityBindingElement.CreateCertificateOverTransportBindingElement is not supported.");
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

            throw ExceptionHelper.PlatformNotSupported("SecurityBindingElement.IsCertificateOverTransportBinding is not supported.");
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsSecureConversationBinding() method.
        static public SecurityBindingElement CreateSecureConversationBindingElement(SecurityBindingElement bootstrapSecurity)
        {
            throw ExceptionHelper.PlatformNotSupported("SecurityBindingElement.CreateSecureConversatationBindingElement is not supported.");
        }

        void SetPrivacyNoticeUriIfRequired(SecurityProtocolFactory factory, Binding binding)
        {
            // Issue #31 in progress (don't throw here to allow further processing)
            //PrivacyNoticeBindingElement privacyElement = binding.CreateBindingElements().Find<PrivacyNoticeBindingElement>();
            //if (privacyElement != null)
            //{
            //    factory.PrivacyNoticeUri = privacyElement.Url;
            //    factory.PrivacyNoticeVersion = privacyElement.Version;
            //}
        }

        internal void ConfigureProtocolFactory(SecurityProtocolFactory factory, SecurityCredentialsManager credentialsManager, bool isForService, BindingContext issuerBindingContext, Binding binding)
        {
            if (factory == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("factory"));
            if (credentialsManager == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("credentialsManager"));

            factory.AddTimestamp = this.IncludeTimestamp;
            factory.IncomingAlgorithmSuite = this.DefaultAlgorithmSuite;
            factory.OutgoingAlgorithmSuite = this.DefaultAlgorithmSuite;
            factory.SecurityHeaderLayout = this.SecurityHeaderLayout;

            if (!isForService)
            {
                factory.TimestampValidityDuration = this.LocalClientSettings.TimestampValidityDuration;
                factory.DetectReplays = this.LocalClientSettings.DetectReplays;
                factory.MaxCachedNonces = this.LocalClientSettings.ReplayCacheSize;
                factory.MaxClockSkew = this.LocalClientSettings.MaxClockSkew;
                factory.ReplayWindow = this.LocalClientSettings.ReplayWindow;

                if (this.LocalClientSettings.DetectReplays)
                {
                    factory.NonceCache = this.LocalClientSettings.NonceCache;
                }
            }
            else
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

                //factory.TimestampValidityDuration = this.LocalServiceSettings.TimestampValidityDuration;
                //factory.DetectReplays = this.LocalServiceSettings.DetectReplays;
                //factory.MaxCachedNonces = this.LocalServiceSettings.ReplayCacheSize;
                //factory.MaxClockSkew = this.LocalServiceSettings.MaxClockSkew;
                //factory.ReplayWindow = this.LocalServiceSettings.ReplayWindow;

                //if (this.LocalServiceSettings.DetectReplays)
                //{
                //    factory.NonceCache = this.LocalServiceSettings.NonceCache;
                //}
            }

            factory.SecurityBindingElement = (SecurityBindingElement)this.Clone();
            factory.SecurityBindingElement.SetIssuerBindingContextIfRequired(issuerBindingContext);
            factory.SecurityTokenManager = credentialsManager.CreateSecurityTokenManager();
            SecurityTokenSerializer tokenSerializer = factory.SecurityTokenManager.CreateSecurityTokenSerializer(_messageSecurityVersion.SecurityTokenVersion);
            factory.StandardsManager = new SecurityStandardsManager(_messageSecurityVersion, tokenSerializer);
            if (!isForService)
            {
                SetPrivacyNoticeUriIfRequired(factory, binding);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0}:", this.GetType().ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "IncludeTimestamp: {0}", _includeTimestamp.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "MessageSecurityVersion: {0}", this.MessageSecurityVersion.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "SecurityHeaderLayout: {0}", _securityHeaderLayout.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "ProtectTokens: {0}", _protectTokens.ToString()));
            sb.AppendLine("EndpointSupportingTokenParameters:");
            sb.AppendLine("  " + this.EndpointSupportingTokenParameters.ToString().Trim().Replace("\n", "\n  "));

            return sb.ToString().Trim();
        }

        internal void ApplyAuditBehaviorSettings(BindingContext context, SecurityProtocolFactory factory)
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
            //ServiceSecurityAuditBehavior auditBehavior = context.BindingParameters.Find<ServiceSecurityAuditBehavior>();
            //if (auditBehavior != null)
            //{
            //    factory.AuditLogLocation = auditBehavior.AuditLogLocation;
            //    factory.SuppressAuditFailure = auditBehavior.SuppressAuditFailure;
            //    factory.ServiceAuthorizationAuditLevel = auditBehavior.ServiceAuthorizationAuditLevel;
            //    factory.MessageAuthenticationAuditLevel = auditBehavior.MessageAuthenticationAuditLevel;
            //}
            //else
            //{
            //    factory.AuditLogLocation = ServiceSecurityAuditBehavior.defaultAuditLogLocation;
            //    factory.SuppressAuditFailure = ServiceSecurityAuditBehavior.defaultSuppressAuditFailure;
            //    factory.ServiceAuthorizationAuditLevel = ServiceSecurityAuditBehavior.defaultServiceAuthorizationAuditLevel;
            //    factory.MessageAuthenticationAuditLevel = ServiceSecurityAuditBehavior.defaultMessageAuthenticationAuditLevel;
            //}
        }
    }
}
