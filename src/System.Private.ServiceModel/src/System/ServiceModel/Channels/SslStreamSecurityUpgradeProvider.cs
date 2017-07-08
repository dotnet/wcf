// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
            if (typeof(T) == typeof(IChannelBindingProvider) || typeof(T) == typeof(IStreamUpgradeChannelBindingProvider))
            {
                return (T)(object)this;
            }

            return base.GetProperty<T>();
        }

        ChannelBinding IStreamUpgradeChannelBindingProvider.GetChannelBinding(StreamUpgradeInitiator upgradeInitiator, ChannelBindingKind kind)
        {
            if (upgradeInitiator == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("upgradeInitiator");
            }

            SslStreamSecurityUpgradeInitiator sslUpgradeInitiator = upgradeInitiator as SslStreamSecurityUpgradeInitiator;

            if (sslUpgradeInitiator == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("upgradeInitiator", SR.Format(SR.UnsupportedUpgradeInitiator, upgradeInitiator.GetType()));
            }

            if (kind != ChannelBindingKind.Endpoint)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("kind", SR.Format(SR.StreamUpgradeUnsupportedChannelBindingKind, this.GetType(), kind));
            }

            return sslUpgradeInitiator.ChannelBinding;
        }

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

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            OnClose(timeout);
            return TaskHelpers.CompletedTask();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnCloseAsync(timeout).ToApm(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        private void SetupServerCertificate(SecurityToken token)
        {
            X509SecurityToken x509Token = token as X509SecurityToken;
            if (x509Token == null)
            {
                SecurityUtils.AbortTokenProviderIfRequired(_serverTokenProvider);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(
                    SR.InvalidTokenProvided, _serverTokenProvider.GetType(), typeof(X509SecurityToken))));
            }

            _serverCertificate = new X509Certificate2(x509Token.Certificate);
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
                SecurityUtils.OpenTokenAuthenticatorIfRequired(ClientCertificateAuthenticator, timeoutHelper.RemainingTime());

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

        protected internal override Task OnOpenAsync(TimeSpan timeout)
        {
            OnOpen(timeout);
            return TaskHelpers.CompletedTask();
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnOpenAsync(timeout).ToApm(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            result.ToApmEnd();
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
        private static LocalCertificateSelectionCallback s_clientCertificateSelectionCallback;

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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ClientCredentialsUnableToCreateLocalTokenProvider, clientCertRequirement)));
                }
            }
        }

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

        internal ChannelBinding ChannelBinding
        {
            get
            {
                Fx.Assert(IsChannelBindingSupportEnabled, "A request for the ChannelBinding is not permitted without enabling ChannelBinding first (through the IChannelBindingProvider interface)");
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
            Stream retVal = OnInitiateUpgradeAsync(stream, remoteSecurityWrapper).GetAwaiter().GetResult();
            remoteSecurity = remoteSecurityWrapper.Value;

            return retVal;
        }

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
                    SR.Format(SR.NegotiationFailedIO, ioException.Message), ioException));
            }

            remoteSecurityWrapper.Value = _serverSecurity;

            if (this.IsChannelBindingSupportEnabled)
            {
                _channelBindingToken = ChannelBindingUtility.GetToken(sslStream);
            }

            return sslStream;
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

            X509Certificate2 certificate2 = new X509Certificate2(certificate);

            SecurityToken token = new X509SecurityToken(certificate2, false);
            ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies = _serverCertificateAuthenticator.ValidateToken(token);
            _serverSecurity = new SecurityMessageProperty();
            _serverSecurity.TransportToken = new SecurityTokenSpecification(token, authorizationPolicies);
            _serverSecurity.ServiceSecurityContext = new ServiceSecurityContext(authorizationPolicies);

            AuthorizationContext authzContext = _serverSecurity.ServiceSecurityContext.AuthorizationContext;
            _parent.IdentityVerifier.EnsureOutgoingIdentity(RemoteAddress, Via, authzContext);

            return true;
        }
    }
}
