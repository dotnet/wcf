// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net.Security;
using System.Runtime;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class SslStreamSecurityUpgradeProvider : StreamSecurityUpgradeProvider, IStreamUpgradeChannelBindingProvider
    {
        private SecurityTokenAuthenticator _clientCertificateAuthenticator;
        private SecurityTokenManager _clientSecurityTokenManager;
        private SecurityTokenProvider _serverTokenProvider;
        private EndpointIdentity _identity;
        private IdentityVerifier _identityVerifier;
        private X509Certificate2 _serverCertificate;
        private bool _requireClientCertificate;
        private string _scheme;
        private SslProtocols _sslProtocols;
        private bool _enableChannelBinding;

        private SslStreamSecurityUpgradeProvider(IDefaultCommunicationTimeouts timeouts, SecurityTokenManager clientSecurityTokenManager, bool requireClientCertificate, string scheme, IdentityVerifier identityVerifier, SslProtocols sslProtocols)
            : base(timeouts)
        {
            _identityVerifier = identityVerifier;
            _scheme = scheme;
            _clientSecurityTokenManager = clientSecurityTokenManager;
            _requireClientCertificate = requireClientCertificate;
            _sslProtocols = sslProtocols;
        }

        private SslStreamSecurityUpgradeProvider(IDefaultCommunicationTimeouts timeouts, SecurityTokenProvider serverTokenProvider, bool requireClientCertificate, SecurityTokenAuthenticator clientCertificateAuthenticator, string scheme, IdentityVerifier identityVerifier, SslProtocols sslProtocols)
            : base(timeouts)
        {
            _serverTokenProvider = serverTokenProvider;
            _requireClientCertificate = requireClientCertificate;
            _clientCertificateAuthenticator = clientCertificateAuthenticator;
            _identityVerifier = identityVerifier;
            _scheme = scheme;
            _sslProtocols = sslProtocols;
        }

        public static SslStreamSecurityUpgradeProvider CreateClientProvider(
            SslStreamSecurityBindingElement bindingElement, BindingContext context)
        {
            SecurityCredentialsManager credentialProvider = context.BindingParameters.Find<SecurityCredentialsManager>();

            if (credentialProvider == null)
            {
                credentialProvider = ClientCredentials.CreateDefaultCredentials();
            }
            SecurityTokenManager tokenManager = credentialProvider.CreateSecurityTokenManager();

            return new SslStreamSecurityUpgradeProvider(
                context.Binding,
                tokenManager,
                bindingElement.RequireClientCertificate,
                context.Binding.Scheme,
                bindingElement.IdentityVerifier,
                bindingElement.SslProtocols);
        }

        public override EndpointIdentity Identity
        {
            get
            {
                if ((_identity == null) && (_serverCertificate != null))
                {
                    // this.identity = SecurityUtils.GetServiceCertificateIdentity(this.serverCertificate);
                    throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeProvider.Identity - server certificate");
                }

                return _identity;
            }
        }

        public IdentityVerifier IdentityVerifier
        {
            get
            {
                return _identityVerifier;
            }
        }

        public bool RequireClientCertificate
        {
            get
            {
                return _requireClientCertificate;
            }
        }

        public X509Certificate2 ServerCertificate
        {
            get
            {
                return _serverCertificate;
            }
        }

        public SecurityTokenAuthenticator ClientCertificateAuthenticator
        {
            get
            {
                if (_clientCertificateAuthenticator == null)
                {
                    _clientCertificateAuthenticator = new X509SecurityTokenAuthenticator(X509ClientCertificateAuthentication.DefaultCertificateValidator);
                }

                return _clientCertificateAuthenticator;
            }
        }

        public SecurityTokenManager ClientSecurityTokenManager
        {
            get
            {
                return _clientSecurityTokenManager;
            }
        }

        public string Scheme
        {
            get { return _scheme; }
        }

        public SslProtocols SslProtocols
        {
            get { return _sslProtocols; }
        }

        public override T GetProperty<T>()
        {
#if FEATURE_CORECLR
            if (typeof(T) == typeof(IChannelBindingProvider) || typeof(T) == typeof(IStreamUpgradeChannelBindingProvider))
            {
                return (T)(object)this;
            }
#endif //FEATURE_CORECLR
            return base.GetProperty<T>();
        }

#if FEATURE_CORECLR
        ChannelBinding IStreamUpgradeChannelBindingProvider.GetChannelBinding(StreamUpgradeInitiator upgradeInitiator, ChannelBindingKind kind)
        {
            if (upgradeInitiator == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("upgradeInitiator");
            }

            SslStreamSecurityUpgradeInitiator sslUpgradeInitiator = upgradeInitiator as SslStreamSecurityUpgradeInitiator;

            if (sslUpgradeInitiator == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("upgradeInitiator", string.Format(SRServiceModel.UnsupportedUpgradeInitiator, upgradeInitiator.GetType()));
            }

            if (kind != ChannelBindingKind.Endpoint)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("kind", string.Format(SRServiceModel.StreamUpgradeUnsupportedChannelBindingKind, this.GetType(), kind));
            }

            return sslUpgradeInitiator.ChannelBinding;
        }

        ChannelBinding IStreamUpgradeChannelBindingProvider.GetChannelBinding(StreamUpgradeAcceptor upgradeAcceptor, ChannelBindingKind kind)
        {
            if (upgradeAcceptor == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("upgradeAcceptor");
            }

            SslStreamSecurityUpgradeAcceptor sslupgradeAcceptor = upgradeAcceptor as SslStreamSecurityUpgradeAcceptor;

            if (sslupgradeAcceptor == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("upgradeAcceptor", string.Format(SRServiceModel.UnsupportedUpgradeAcceptor, upgradeAcceptor.GetType()));
            }

            if (kind != ChannelBindingKind.Endpoint)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("kind", string.Format(SRServiceModel.StreamUpgradeUnsupportedChannelBindingKind, this.GetType(), kind));
            }

            return sslupgradeAcceptor.ChannelBinding;
        }
#endif // FEATURE_CORECLR

        void IChannelBindingProvider.EnableChannelBindingSupport()
        {
            _enableChannelBinding = true;
        }

        bool IChannelBindingProvider.IsChannelBindingSupportEnabled
        {
            get
            {
                return _enableChannelBinding;
            }
        }

        public override StreamUpgradeAcceptor CreateUpgradeAcceptor()
        {
            ThrowIfDisposedOrNotOpen();
            return new SslStreamSecurityUpgradeAcceptor(this);
        }

        public override StreamUpgradeInitiator CreateUpgradeInitiator(EndpointAddress remoteAddress, Uri via)
        {
            ThrowIfDisposedOrNotOpen();
            return new SslStreamSecurityUpgradeInitiator(this, remoteAddress, via);
        }

        protected override void OnAbort()
        {
            if (_clientCertificateAuthenticator != null)
            {
                SecurityUtils.AbortTokenAuthenticatorIfRequired(_clientCertificateAuthenticator);
            }
            CleanupServerCertificate();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            if (_clientCertificateAuthenticator != null)
            {
                SecurityUtils.CloseTokenAuthenticatorIfRequired(_clientCertificateAuthenticator, timeout);
            }
            CleanupServerCertificate();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeProvider async path");
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeProvider async path");
        }

        private void SetupServerCertificate(SecurityToken token)
        {
            X509SecurityToken x509Token = token as X509SecurityToken;
            if (x509Token == null)
            {
                SecurityUtils.AbortTokenProviderIfRequired(_serverTokenProvider);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(
                    SRServiceModel.InvalidTokenProvided, _serverTokenProvider.GetType(), typeof(X509SecurityToken))));
            }
            _serverCertificate = new X509Certificate2(x509Token.Certificate.Handle);
        }

        private void CleanupServerCertificate()
        {
            if (_serverCertificate != null)
            {
                _serverCertificate.Dispose();
                _serverCertificate = null;
            }
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource(timeout))
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                SecurityUtils.OpenTokenAuthenticatorIfRequired(this.ClientCertificateAuthenticator, timeoutHelper.RemainingTime());

                if (_serverTokenProvider != null)
                {
                    SecurityUtils.OpenTokenProviderIfRequired(_serverTokenProvider, timeoutHelper.RemainingTime());
                    SecurityToken token = _serverTokenProvider.GetTokenAsync(cts.Token).GetAwaiter().GetResult();
                    SetupServerCertificate(token);
                    SecurityUtils.CloseTokenProviderIfRequired(_serverTokenProvider, timeoutHelper.RemainingTime());
                    _serverTokenProvider = null;
                }
            }
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeProvider async path");
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeProvider async path");
        }
    }

    internal class SslStreamSecurityUpgradeAcceptor : StreamSecurityUpgradeAcceptorBase
    {
        private SslStreamSecurityUpgradeProvider _parent;
        private SecurityMessageProperty _clientSecurity;
        // for audit
        private X509Certificate2 _clientCertificate = null;
        private ChannelBinding _channelBindingToken;

        public SslStreamSecurityUpgradeAcceptor(SslStreamSecurityUpgradeProvider parent)
            : base(FramingUpgradeString.SslOrTls)
        {
            _parent = parent;
            _clientSecurity = new SecurityMessageProperty();
        }


        internal ChannelBinding ChannelBinding
        {
            get
            {
                Fx.Assert(this.IsChannelBindingSupportEnabled, "A request for the ChannelBinding is not permitted without enabling ChannelBinding first (through the IChannelBindingProvider interface)");
                return _channelBindingToken;
            }
        }

        internal bool IsChannelBindingSupportEnabled
        {
            get
            {
                return ((IChannelBindingProvider)_parent).IsChannelBindingSupportEnabled;
            }
        }

        protected override Stream OnAcceptUpgrade(Stream stream, out SecurityMessageProperty remoteSecurity)
        {
#if FEATURE_NETNATIVE
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeAcceptor.OnAcceptUpgrade");
#else // !FEATURE_NETNATIVE

            if (WcfEventSource.Instance.SslOnAcceptUpgradeIsEnabled())
            {
                WcfEventSource.Instance.SslOnAcceptUpgrade(this.EventTraceActivity);
            }

            SslStream sslStream = new SslStream(stream, false, this.ValidateRemoteCertificate);

            try
            {
                sslStream.AuthenticateAsServerAsync(_parent.ServerCertificate, _parent.RequireClientCertificate,
                    _parent.SslProtocols, false).GetAwaiter().GetResult();
            }
            catch (AuthenticationException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(exception.Message,
                    exception));
            }
            catch (IOException ioException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(
                    string.Format(SRServiceModel.NegotiationFailedIO, ioException.Message), ioException));
            }

            remoteSecurity = _clientSecurity;

            if (this.IsChannelBindingSupportEnabled)
            {
                _channelBindingToken = ChannelBindingUtility.GetToken(sslStream);
            }

            return sslStream;
#endif // !FEATURE_NETNATIVE
        }

        protected override IAsyncResult OnBeginAcceptUpgrade(Stream stream, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeProvider async path");
        }

        protected override Stream OnEndAcceptUpgrade(IAsyncResult result, out SecurityMessageProperty remoteSecurity)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeProvider async path");
        }

        // callback from schannel
        private bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (_parent.RequireClientCertificate)
            {
                if (certificate == null)
                {
                    Contract.Assert(certificate != null, "certificate MUST NOT be null");
                    return false;
                }
                // Note: add ref to handle since the caller will reset the cert after the callback return.
                X509Certificate2 certificate2 = new X509Certificate2(certificate.Handle);
                _clientCertificate = certificate2;
                try
                {
                    SecurityToken token = new X509SecurityToken(certificate2, false);
                    ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies = _parent.ClientCertificateAuthenticator.ValidateToken(token);
                    _clientSecurity = new SecurityMessageProperty();
                    _clientSecurity.TransportToken = new SecurityTokenSpecification(token, authorizationPolicies);
                    _clientSecurity.ServiceSecurityContext = new ServiceSecurityContext(authorizationPolicies);
                }
                catch (SecurityTokenException)
                {
                    return false;
                }
            }
            return true;
        }

        public override SecurityMessageProperty GetRemoteSecurity()
        {
            if (_clientSecurity.TransportToken != null)
            {
                return _clientSecurity;
            }
            if (_clientCertificate != null)
            {
                SecurityToken token = new X509SecurityToken(_clientCertificate);
                ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies = SecurityUtils.NonValidatingX509Authenticator.ValidateToken(token);
                _clientSecurity = new SecurityMessageProperty();
                _clientSecurity.TransportToken = new SecurityTokenSpecification(token, authorizationPolicies);
                _clientSecurity.ServiceSecurityContext = new ServiceSecurityContext(authorizationPolicies);
                return _clientSecurity;
            }
            return base.GetRemoteSecurity();
        }
    }

    internal class SslStreamSecurityUpgradeInitiator : StreamSecurityUpgradeInitiatorBase
    {
        private SslStreamSecurityUpgradeProvider _parent;
        private SecurityMessageProperty _serverSecurity;
        private SecurityTokenProvider _clientCertificateProvider;
        private X509SecurityToken _clientToken;
        private SecurityTokenAuthenticator _serverCertificateAuthenticator;
        private ChannelBinding _channelBindingToken;
#if !FEATURE_NETNATIVE
        private static LocalCertificateSelectionCallback s_clientCertificateSelectionCallback;
#endif // !FEATURE_NETNATIVE

        public SslStreamSecurityUpgradeInitiator(SslStreamSecurityUpgradeProvider parent,
            EndpointAddress remoteAddress, Uri via)
            : base(FramingUpgradeString.SslOrTls, remoteAddress, via)
        {
            _parent = parent;

            InitiatorServiceModelSecurityTokenRequirement serverCertRequirement = new InitiatorServiceModelSecurityTokenRequirement();
            serverCertRequirement.TokenType = SecurityTokenTypes.X509Certificate;
            serverCertRequirement.RequireCryptographicToken = true;
            serverCertRequirement.KeyUsage = SecurityKeyUsage.Exchange;
            serverCertRequirement.TargetAddress = remoteAddress;
            serverCertRequirement.Via = via;
            serverCertRequirement.TransportScheme = _parent.Scheme;
            serverCertRequirement.PreferSslCertificateAuthenticator = true;

            SecurityTokenResolver dummy;
            _serverCertificateAuthenticator = (parent.ClientSecurityTokenManager.CreateSecurityTokenAuthenticator(serverCertRequirement, out dummy));

            if (parent.RequireClientCertificate)
            {
                InitiatorServiceModelSecurityTokenRequirement clientCertRequirement = new InitiatorServiceModelSecurityTokenRequirement();
                clientCertRequirement.TokenType = SecurityTokenTypes.X509Certificate;
                clientCertRequirement.RequireCryptographicToken = true;
                clientCertRequirement.KeyUsage = SecurityKeyUsage.Signature;
                clientCertRequirement.TargetAddress = remoteAddress;
                clientCertRequirement.Via = via;
                clientCertRequirement.TransportScheme = _parent.Scheme;
                _clientCertificateProvider = parent.ClientSecurityTokenManager.CreateSecurityTokenProvider(clientCertRequirement);
                if (_clientCertificateProvider == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.ClientCredentialsUnableToCreateLocalTokenProvider, clientCertRequirement)));
                }
            }
        }

#if !FEATURE_NETNATIVE
        private static LocalCertificateSelectionCallback ClientCertificateSelectionCallback
        {
            get
            {
                if (s_clientCertificateSelectionCallback == null)
                {
                    s_clientCertificateSelectionCallback = new LocalCertificateSelectionCallback(SelectClientCertificate);
                }

                return s_clientCertificateSelectionCallback;
            }
        }
#endif //!FEATURE_NETNATIVE

        internal ChannelBinding ChannelBinding
        {
            get
            {
                Fx.Assert(this.IsChannelBindingSupportEnabled, "A request for the ChannelBinding is not permitted without enabling ChannelBinding first (through the IChannelBindingProvider interface)");
                return _channelBindingToken;
            }
        }

        internal bool IsChannelBindingSupportEnabled
        {
            get
            {
                return ((IChannelBindingProvider)_parent).IsChannelBindingSupportEnabled;
            }
        }

        internal override void Open(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            base.Open(timeoutHelper.RemainingTime());
            if (_clientCertificateProvider != null)
            {
                SecurityUtils.OpenTokenProviderIfRequired(_clientCertificateProvider, timeoutHelper.RemainingTime());
                using (CancellationTokenSource cts = new CancellationTokenSource(timeoutHelper.RemainingTime()))
                {
                    _clientToken = (X509SecurityToken)_clientCertificateProvider.GetTokenAsync(cts.Token).GetAwaiter().GetResult();
                }
            }
        }

        internal override async Task OpenAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await base.OpenAsync(timeoutHelper.RemainingTime());
            if (_clientCertificateProvider != null)
            {
                SecurityUtils.OpenTokenProviderIfRequired(_clientCertificateProvider, timeoutHelper.RemainingTime());
                using (CancellationTokenSource cts = new CancellationTokenSource(timeoutHelper.RemainingTime()))
                {
                    _clientToken = (X509SecurityToken)(await _clientCertificateProvider.GetTokenAsync(cts.Token));
                }
            }
        }

        internal override void Close(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            base.Close(timeoutHelper.RemainingTime());
            if (_clientCertificateProvider != null)
            {
                SecurityUtils.CloseTokenProviderIfRequired(_clientCertificateProvider, timeoutHelper.RemainingTime());
            }
        }

        internal override async Task CloseAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await base.CloseAsync(timeoutHelper.RemainingTime());
            if (_clientCertificateProvider != null)
            {
                SecurityUtils.CloseTokenProviderIfRequired(_clientCertificateProvider, timeoutHelper.RemainingTime());
            }
        }

        protected override Stream OnInitiateUpgrade(Stream stream, out SecurityMessageProperty remoteSecurity)
        {
            OutWrapper<SecurityMessageProperty> remoteSecurityWrapper = new OutWrapper<SecurityMessageProperty>();
            Stream retVal = this.OnInitiateUpgradeAsync(stream, remoteSecurityWrapper).GetAwaiter().GetResult();
            remoteSecurity = remoteSecurityWrapper.Value;

            return retVal;
        }

#if FEATURE_NETNATIVE
        protected override Task<Stream> OnInitiateUpgradeAsync(Stream stream, OutWrapper<SecurityMessageProperty> remoteSecurityWrapper)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeInitiator.InInitiateUpgradeAsync");
#else // !FEATURE_NETNATIVE
        protected override async Task<Stream> OnInitiateUpgradeAsync(Stream stream, OutWrapper<SecurityMessageProperty> remoteSecurityWrapper)
        {
            if (WcfEventSource.Instance.SslOnInitiateUpgradeIsEnabled())
            {
                WcfEventSource.Instance.SslOnInitiateUpgrade();
            }

            X509CertificateCollection clientCertificates = null;
            LocalCertificateSelectionCallback selectionCallback = null;

            if (_clientToken != null)
            {
                clientCertificates = new X509CertificateCollection();
                clientCertificates.Add(_clientToken.Certificate);
                selectionCallback = ClientCertificateSelectionCallback;
            }

            SslStream sslStream = new SslStream(stream, false, this.ValidateRemoteCertificate, selectionCallback);

            try
            {
                await sslStream.AuthenticateAsClientAsync(string.Empty, clientCertificates, _parent.SslProtocols, false);
            }
            catch (SecurityTokenValidationException tokenValidationException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(tokenValidationException.Message,
                    tokenValidationException));
            }
            catch (AuthenticationException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(exception.Message,
                    exception));
            }
            catch (IOException ioException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(
                    string.Format(SRServiceModel.NegotiationFailedIO, ioException.Message), ioException));
            }

            remoteSecurityWrapper.Value = _serverSecurity;

            if (this.IsChannelBindingSupportEnabled)
            {
                _channelBindingToken = ChannelBindingUtility.GetToken(sslStream);
            }

            return sslStream;
#endif //!FEATURE_NETNATIVE
        }

        private static X509Certificate SelectClientCertificate(object sender, string targetHost,
            X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return localCertificates[0];
        }

        private bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            // Note: add ref to handle since the caller will reset the cert after the callback return.
            X509Certificate2 certificate2 = new X509Certificate2(certificate.Handle);
            SecurityToken token = new X509SecurityToken(certificate2, false);
            ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies = _serverCertificateAuthenticator.ValidateToken(token);
            _serverSecurity = new SecurityMessageProperty();
            _serverSecurity.TransportToken = new SecurityTokenSpecification(token, authorizationPolicies);
            _serverSecurity.ServiceSecurityContext = new ServiceSecurityContext(authorizationPolicies);

            AuthorizationContext authzContext = _serverSecurity.ServiceSecurityContext.AuthorizationContext;
            _parent.IdentityVerifier.EnsureOutgoingIdentity(this.RemoteAddress, this.Via, authzContext);

            return true;
        }
    }
}
