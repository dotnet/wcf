// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net.Security;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Xml;

namespace System.ServiceModel.Channels
{
    public abstract class SecurityBindingElement : BindingElement
    {
        internal static readonly SecurityAlgorithmSuite defaultDefaultAlgorithmSuite = SecurityAlgorithmSuite.Default;
        internal const bool defaultIncludeTimestamp = true;
        internal const bool defaultAllowInsecureTransport = false;
        internal const bool defaultRequireSignatureConfirmation = false;
        internal const bool defaultEnableUnsecuredResponse = false;
        internal const bool defaultProtectTokens = false;

        private SecurityAlgorithmSuite _defaultAlgorithmSuite;
        private SecurityKeyEntropyMode _keyEntropyMode;
        private Dictionary<string, SupportingTokenParameters> _operationSupportingTokenParameters;
        private Dictionary<string, SupportingTokenParameters> _optionalOperationSupportingTokenParameters;
        private MessageSecurityVersion _messageSecurityVersion;
        private SecurityHeaderLayout _securityHeaderLayout;
        private bool _protectTokens = defaultProtectTokens;

        internal SecurityBindingElement()
            : base()
        {
            _messageSecurityVersion = MessageSecurityVersion.Default;
            _keyEntropyMode = AcceleratedTokenProvider.defaultKeyEntropyMode;
            IncludeTimestamp = defaultIncludeTimestamp;
            _defaultAlgorithmSuite = defaultDefaultAlgorithmSuite;
            LocalClientSettings = new LocalClientSecuritySettings();
            EndpointSupportingTokenParameters = new SupportingTokenParameters();
            OptionalEndpointSupportingTokenParameters = new SupportingTokenParameters();
            _operationSupportingTokenParameters = new Dictionary<string, SupportingTokenParameters>();
            _optionalOperationSupportingTokenParameters = new Dictionary<string, SupportingTokenParameters>();
            _securityHeaderLayout = SecurityProtocolFactory.defaultSecurityHeaderLayout;
        }

        internal SecurityBindingElement(SecurityBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            if (elementToBeCloned == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(elementToBeCloned));
            }

            _defaultAlgorithmSuite = elementToBeCloned._defaultAlgorithmSuite;
            IncludeTimestamp = elementToBeCloned.IncludeTimestamp;
            _keyEntropyMode = elementToBeCloned._keyEntropyMode;
            _messageSecurityVersion = elementToBeCloned._messageSecurityVersion;
            _securityHeaderLayout = elementToBeCloned._securityHeaderLayout;
            EndpointSupportingTokenParameters = elementToBeCloned.EndpointSupportingTokenParameters.Clone();
            OptionalEndpointSupportingTokenParameters = elementToBeCloned.OptionalEndpointSupportingTokenParameters.Clone();
            _operationSupportingTokenParameters = new Dictionary<string, SupportingTokenParameters>();
            foreach (string key in elementToBeCloned._operationSupportingTokenParameters.Keys)
            {
                _operationSupportingTokenParameters[key] = elementToBeCloned._operationSupportingTokenParameters[key].Clone();
            }
            
            _optionalOperationSupportingTokenParameters = new Dictionary<string, SupportingTokenParameters>();
            foreach (string key in elementToBeCloned._optionalOperationSupportingTokenParameters.Keys)
            {
                _optionalOperationSupportingTokenParameters[key] = elementToBeCloned._optionalOperationSupportingTokenParameters[key].Clone();
            }
            
            LocalClientSettings = elementToBeCloned.LocalClientSettings.Clone();
            MaxReceivedMessageSize = elementToBeCloned.MaxReceivedMessageSize;
            ReaderQuotas = elementToBeCloned.ReaderQuotas;
            EnableUnsecuredResponse = elementToBeCloned.EnableUnsecuredResponse;
        }

        public SupportingTokenParameters EndpointSupportingTokenParameters { get; }

        public SupportingTokenParameters OptionalEndpointSupportingTokenParameters { get; }

        public IDictionary<string, SupportingTokenParameters> OperationSupportingTokenParameters => _operationSupportingTokenParameters;

        public IDictionary<string, SupportingTokenParameters> OptionalOperationSupportingTokenParameters => _optionalOperationSupportingTokenParameters;

        public SecurityHeaderLayout SecurityHeaderLayout
        {
            get
            {
                return _securityHeaderLayout;
            }
            set
            {
                if (!SecurityHeaderLayoutHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }

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
                _messageSecurityVersion = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(value)));
            }
        }

        public bool EnableUnsecuredResponse { get; set; }

        public bool IncludeTimestamp { get; set; }

        public SecurityAlgorithmSuite DefaultAlgorithmSuite
        {
            get
            {
                return _defaultAlgorithmSuite;
            }
            set
            {
                _defaultAlgorithmSuite = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(value)));
            }
        }

        public LocalClientSecuritySettings LocalClientSettings { get; }

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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
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

        internal long MaxReceivedMessageSize { get; set; } = TransportDefaults.MaxReceivedMessageSize;

        internal XmlDictionaryReaderQuotas ReaderQuotas { get; set; }

        private void GetSupportingTokensCapabilities(ICollection<SecurityTokenParameters> parameters, out bool supportsClientAuth, out bool supportsWindowsIdentity)
        {
            supportsClientAuth = false;
            supportsWindowsIdentity = false;
            foreach (SecurityTokenParameters p in parameters)
            {
                if (p.SupportsClientAuthentication)
                {
                    supportsClientAuth = true;
                }

                if (p.SupportsClientWindowsIdentity)
                {
                    supportsWindowsIdentity = true;
                }
            }
        }

        private void GetSupportingTokensCapabilities(SupportingTokenParameters requirements, out bool supportsClientAuth, out bool supportsWindowsIdentity)
        {
            supportsClientAuth = false;
            supportsWindowsIdentity = false;
            bool tmpSupportsClientAuth;
            bool tmpSupportsWindowsIdentity;
            GetSupportingTokensCapabilities(requirements.Endorsing, out tmpSupportsClientAuth, out tmpSupportsWindowsIdentity);
            supportsClientAuth = supportsClientAuth || tmpSupportsClientAuth;
            supportsWindowsIdentity = supportsWindowsIdentity || tmpSupportsWindowsIdentity;

            GetSupportingTokensCapabilities(requirements.SignedEndorsing, out tmpSupportsClientAuth, out tmpSupportsWindowsIdentity);
            supportsClientAuth = supportsClientAuth || tmpSupportsClientAuth;
            supportsWindowsIdentity = supportsWindowsIdentity || tmpSupportsWindowsIdentity;

            GetSupportingTokensCapabilities(requirements.SignedEncrypted, out tmpSupportsClientAuth, out tmpSupportsWindowsIdentity);
            supportsClientAuth = supportsClientAuth || tmpSupportsClientAuth;
            supportsWindowsIdentity = supportsWindowsIdentity || tmpSupportsWindowsIdentity;
        }

        internal void GetSupportingTokensCapabilities(out bool supportsClientAuth, out bool supportsWindowsIdentity)
        {
            GetSupportingTokensCapabilities(EndpointSupportingTokenParameters, out supportsClientAuth, out supportsWindowsIdentity);
        }

        protected static void SetIssuerBindingContextIfRequired(SecurityTokenParameters parameters, BindingContext issuerBindingContext)
        {
            // Only needed for SslSecurityTokenParameters and SspiSecurityTokenParameters which aren't supported
        }

        private static void SetIssuerBindingContextIfRequired(SupportingTokenParameters supportingParameters, BindingContext issuerBindingContext)
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

        private void SetIssuerBindingContextIfRequired(BindingContext issuerBindingContext)
        {
            SetIssuerBindingContextIfRequired(EndpointSupportingTokenParameters, issuerBindingContext);
        }

        internal bool RequiresChannelDemuxer(SecurityTokenParameters parameters)
        {
            return (parameters is SecureConversationSecurityTokenParameters);
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

        private void SetPrivacyNoticeUriIfRequired(SecurityProtocolFactory factory, Binding binding)
        {
            // Only used for WS-Fed and configures a PrivacyNoticeBindingElement if present.
            // Not currently supported on .NET Core
        }

        internal void ConfigureProtocolFactory(SecurityProtocolFactory factory, SecurityCredentialsManager credentialsManager, bool isForService, BindingContext issuerBindingContext, Binding binding)
        {
            if (factory == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(factory)));
            }

            if (credentialsManager == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(credentialsManager)));
            }

            factory.AddTimestamp = IncludeTimestamp;
            factory.IncomingAlgorithmSuite = DefaultAlgorithmSuite;
            factory.OutgoingAlgorithmSuite = DefaultAlgorithmSuite;
            factory.SecurityHeaderLayout = SecurityHeaderLayout;

            if (!isForService)
            {
                factory.TimestampValidityDuration = LocalClientSettings.TimestampValidityDuration;
                factory.DetectReplays = LocalClientSettings.DetectReplays;
                factory.MaxCachedNonces = LocalClientSettings.ReplayCacheSize;
                factory.MaxClockSkew = LocalClientSettings.MaxClockSkew;
                factory.ReplayWindow = LocalClientSettings.ReplayWindow;

                if (LocalClientSettings.DetectReplays)
                {
                    factory.NonceCache = LocalClientSettings.NonceCache;
                }
            }
            else
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            factory.SecurityBindingElement = (SecurityBindingElement)Clone();
            factory.SecurityBindingElement.SetIssuerBindingContextIfRequired(issuerBindingContext);
            factory.SecurityTokenManager = credentialsManager.CreateSecurityTokenManager();
            SecurityTokenSerializer tokenSerializer = factory.SecurityTokenManager.CreateSecurityTokenSerializer(_messageSecurityVersion.SecurityTokenVersion);
            factory.StandardsManager = new SecurityStandardsManager(_messageSecurityVersion, tokenSerializer);
            if (!isForService)
            {
                SetPrivacyNoticeUriIfRequired(factory, binding);
            }
        }

        internal abstract SecurityProtocolFactory CreateSecurityProtocolFactory<TChannel>(BindingContext context, SecurityCredentialsManager credentialsManager,
            bool isForService, BindingContext issuanceBindingContext);

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }

            if (!CanBuildChannelFactory<TChannel>(context))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.Format(SRP.ChannelTypeNotSupported, typeof(TChannel)), nameof(TChannel)));
            }

            ReaderQuotas = context.GetInnerProperty<XmlDictionaryReaderQuotas>();
            if (ReaderQuotas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.EncodingBindingElementDoesNotHandleReaderQuotas));
            }

            TransportBindingElement transportBindingElement = null;

            if (context.RemainingBindingElements != null)
            {
                transportBindingElement = context.RemainingBindingElements.Find<TransportBindingElement>();
            }

            if (transportBindingElement != null)
            {
                MaxReceivedMessageSize = transportBindingElement.MaxReceivedMessageSize;
            }

            IChannelFactory<TChannel> result = BuildChannelFactoryCore<TChannel>(context);

            return result;
        }

        protected abstract IChannelFactory<TChannel> BuildChannelFactoryCore<TChannel>(BindingContext context);

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }

            if (SessionMode)
            {
                return CanBuildSessionChannelFactory<TChannel>(context);
            }

            if (!context.CanBuildInnerChannelFactory<TChannel>())
            {
                return false;
            }

            return typeof(TChannel) == typeof(IOutputChannel) || typeof(TChannel) == typeof(IOutputSessionChannel) ||
                (SupportsDuplex && (typeof(TChannel) == typeof(IDuplexChannel) || typeof(TChannel) == typeof(IDuplexSessionChannel))) ||
                (SupportsRequestReply && (typeof(TChannel) == typeof(IRequestChannel) || typeof(TChannel) == typeof(IRequestSessionChannel)));
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
            {
                return false;
            }

            return true;
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T)(object)GetSecurityCapabilities(context);
            }
            else if (typeof(T) == typeof(IdentityVerifier))
            {
                return (T)(object)LocalClientSettings.IdentityVerifier;
            }
            else
            {
                return context.GetInnerProperty<T>();
            }
        }

        internal abstract ISecurityCapabilities GetIndividualISecurityCapabilities();

        private ISecurityCapabilities GetSecurityCapabilities(BindingContext context)
        {
            ISecurityCapabilities thisSecurityCapability = GetIndividualISecurityCapabilities();
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

        internal void ApplyPropertiesOnDemuxer(ChannelBuilder builder, BindingContext context)
        {
            // Only used for services
        }

        static public SecurityBindingElement CreateMutualCertificateBindingElement()
        {
            return CreateMutualCertificateBindingElement(MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11);
        }

        static public SecurityBindingElement CreateMutualCertificateBindingElement(MessageSecurityVersion version)
        {
            return CreateMutualCertificateBindingElement(version, false);
        }

        static public SecurityBindingElement CreateMutualCertificateBindingElement(MessageSecurityVersion version, bool allowSerializedSigningTokenOnReply)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(version));
            }

            throw ExceptionHelper.PlatformNotSupported("SecurityBindingElement.CreateMutualCertificateBindingElement is not supported.");
        }

        static public TransportSecurityBindingElement CreateUserNameOverTransportBindingElement()
        {
            var result = new TransportSecurityBindingElement();
            result.EndpointSupportingTokenParameters.SignedEncrypted.Add(
                new UserNameSecurityTokenParameters());
            result.IncludeTimestamp = true;
            result.LocalClientSettings.DetectReplays = false;
            return result;
        }

        static public TransportSecurityBindingElement CreateCertificateOverTransportBindingElement()
        {
            return CreateCertificateOverTransportBindingElement(MessageSecurityVersion.Default);
        }

        static public TransportSecurityBindingElement CreateCertificateOverTransportBindingElement(MessageSecurityVersion version)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(version));
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
            //result.LocalServiceSettings.DetectReplays = false;
            result.MessageSecurityVersion = version;

            return result;
        }

        public static TransportSecurityBindingElement CreateIssuedTokenOverTransportBindingElement(IssuedSecurityTokenParameters issuedTokenParameters)
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
            result.IncludeTimestamp = true;

            return result;
        }

        static public SecurityBindingElement CreateSecureConversationBindingElement(SecurityBindingElement bootstrapSecurity)
        {
            return CreateSecureConversationBindingElement(bootstrapSecurity, SecureConversationSecurityTokenParameters.defaultRequireCancellation, null);
        }

        static public SecurityBindingElement CreateSecureConversationBindingElement(SecurityBindingElement bootstrapSecurity, bool requireCancellation)
        {
            return CreateSecureConversationBindingElement(bootstrapSecurity, requireCancellation, null);
        }

        static public SecurityBindingElement CreateSecureConversationBindingElement(SecurityBindingElement bootstrapSecurity, bool requireCancellation, ChannelProtectionRequirements bootstrapProtectionRequirements)
        {
            if (bootstrapSecurity == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(bootstrapSecurity));
            }

            SecurityBindingElement result;

            if (bootstrapSecurity is TransportSecurityBindingElement)
            {
                // there is no need to do replay detection or key derivation for transport bindings
                var primary = new TransportSecurityBindingElement();
                var scParameters = new SecureConversationSecurityTokenParameters(
                        bootstrapSecurity,
                        requireCancellation,
                        bootstrapProtectionRequirements);
                scParameters.RequireDerivedKeys = false;
                primary.EndpointSupportingTokenParameters.Endorsing.Add(
                    scParameters);
                primary.LocalClientSettings.DetectReplays = false;
                primary.IncludeTimestamp = true;
                result = primary;
            }
            else // Symmetric- or AsymmetricSecurityBindingElement
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}:", GetType().ToString()));
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "DefaultAlgorithmSuite: {0}", _defaultAlgorithmSuite.ToString()));
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "IncludeTimestamp: {0}", IncludeTimestamp.ToString()));
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "KeyEntropyMode: {0}", _keyEntropyMode.ToString()));
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "MessageSecurityVersion: {0}", MessageSecurityVersion.ToString()));
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "SecurityHeaderLayout: {0}", _securityHeaderLayout.ToString()));
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "ProtectTokens: {0}", _protectTokens.ToString()));
            sb.AppendLine("EndpointSupportingTokenParameters:");
            sb.AppendLine("  " + EndpointSupportingTokenParameters.ToString().Trim().Replace("\n", "\n  "));
            sb.AppendLine("OptionalEndpointSupportingTokenParameters:");
            sb.AppendLine("  " + OptionalEndpointSupportingTokenParameters.ToString().Trim().Replace("\n", "\n  "));

            if (_operationSupportingTokenParameters.Count == 0)
            {
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "OperationSupportingTokenParameters: none"));
            }
            else
            {
                foreach (string requestAction in OperationSupportingTokenParameters.Keys)
                {
                    sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "OperationSupportingTokenParameters[\"{0}\"]:", requestAction));
                    sb.AppendLine("  " + OperationSupportingTokenParameters[requestAction].ToString().Trim().Replace("\n", "\n  "));
                }
            }

            if (_optionalOperationSupportingTokenParameters.Count == 0)
            {
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "OptionalOperationSupportingTokenParameters: none"));
            }
            else
            {
                foreach (string requestAction in OptionalOperationSupportingTokenParameters.Keys)
                {
                    sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "OptionalOperationSupportingTokenParameters[\"{0}\"]:", requestAction));
                    sb.AppendLine("  " + OptionalOperationSupportingTokenParameters[requestAction].ToString().Trim().Replace("\n", "\n  "));
                }
            }

            return sb.ToString().Trim();
        }
    }
}
