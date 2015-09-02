// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net.Security;
using System.Runtime;
using System.Security.Authentication;
#if FEATURE_CORECLR // X509Certificates
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
#endif 
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics.Application;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Threading;

namespace System.ServiceModel.Channels
{
    internal class SslStreamSecurityUpgradeProvider : StreamSecurityUpgradeProvider, IStreamUpgradeChannelBindingProvider
    {
        private SecurityTokenAuthenticator _clientCertificateAuthenticator;
        private SecurityTokenManager _clientSecurityTokenManager;
        private SecurityTokenProvider _serverTokenProvider;
        private EndpointIdentity _identity;
        private IdentityVerifier _identityVerifier;
#if FEATURE_CORECLR // X509Certificates
        private X509Certificate2 _serverCertificate;
#endif // FEATURE_CORECLR
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
#if FEATURE_CORECLR
                if ((_identity == null) && (_serverCertificate != null))
                {
                    // this.identity = SecurityUtils.GetServiceCertificateIdentity(this.serverCertificate);
                    throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeProvider.Identity - server certificate");
                }
#endif // FEATURE_CORECLR
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

#if FEATURE_CORECLR // X509Certificates
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
#endif // FEATURE_CORECLR

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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("upgradeInitiator", SR.Format(SR.UnsupportedUpgradeInitiator, upgradeInitiator.GetType()));
            }

            if (kind != ChannelBindingKind.Endpoint)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("kind", SR.Format(SR.StreamUpgradeUnsupportedChannelBindingKind, this.GetType(), kind));
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("upgradeAcceptor", SR.Format(SR.UnsupportedUpgradeAcceptor, upgradeAcceptor.GetType()));
            }

            if (kind != ChannelBindingKind.Endpoint)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("kind", SR.Format(SR.StreamUpgradeUnsupportedChannelBindingKind, this.GetType(), kind));
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

#if FEATURE_CORECLR // X509Certificates
        private void SetupServerCertificate(SecurityToken token)
        {
            X509SecurityToken x509Token = token as X509SecurityToken;
            if (x509Token == null)
            {
                SecurityUtils.AbortTokenProviderIfRequired(_serverTokenProvider);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(
                    SR.InvalidTokenProvided, _serverTokenProvider.GetType(), typeof(X509SecurityToken))));
            }
            _serverCertificate = new X509Certificate2(x509Token.Certificate.Handle);
        }
#endif // FEATURE_CORECLR

        private void CleanupServerCertificate()
        {
#if FEATURE_CORECLR // X509Certificates
            if (_serverCertificate != null)
            {
                _serverCertificate.Dispose();
                _serverCertificate = null;
            }
#endif // FEATURE_CORECLR
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource(timeout))
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
#if FEATURE_CORECLR // X509Certificates
                SecurityUtils.OpenTokenAuthenticatorIfRequired(this.ClientCertificateAuthenticator, timeoutHelper.RemainingTime());

                if (_serverTokenProvider != null)
                {
                    SecurityUtils.OpenTokenProviderIfRequired(_serverTokenProvider, timeoutHelper.RemainingTime());
                    SecurityToken token = _serverTokenProvider.GetTokenAsync(cts.Token).GetAwaiter().GetResult();
                    SetupServerCertificate(token);
                    SecurityUtils.CloseTokenProviderIfRequired(_serverTokenProvider, timeoutHelper.RemainingTime());
                    _serverTokenProvider = null;
                }
#endif // FEATURE_CORECLR
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
#if FEATURE_CORECLR // X509Certificates
        private X509Certificate2 _clientCertificate = null;
        private ChannelBinding _channelBindingToken;
#endif // FEATURE_CORECLR

        public SslStreamSecurityUpgradeAcceptor(SslStreamSecurityUpgradeProvider parent)
            : base(FramingUpgradeString.SslOrTls)
        {
            _parent = parent;
            _clientSecurity = new SecurityMessageProperty();
        }

#if FEATURE_CORECLR
        internal ChannelBinding ChannelBinding
        {
            get
            {
                Fx.Assert(this.IsChannelBindingSupportEnabled, "A request for the ChannelBinding is not permitted without enabling ChannelBinding first (through the IChannelBindingProvider interface)");
                return _channelBindingToken;
            }
        }
#endif //FEATURE_CORECLR

        internal bool IsChannelBindingSupportEnabled
        {
            get
            {
                return ((IChannelBindingProvider)_parent).IsChannelBindingSupportEnabled;
            }
        }

        protected override Stream OnAcceptUpgrade(Stream stream, out SecurityMessageProperty remoteSecurity)
        {
#if FEATURE_CORECLR // X509Certificates
            if (TD.SslOnAcceptUpgradeIsEnabled())
            {
                TD.SslOnAcceptUpgrade(this.EventTraceActivity);
            }

            SslStream sslStream = new SslStream(stream, false, this.ValidateRemoteCertificate);

            try
            {
                sslStream.AuthenticateAsServer(_parent.ServerCertificate, _parent.RequireClientCertificate,
                    _parent.SslProtocols, false);

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

            remoteSecurity = _clientSecurity;

            if (this.IsChannelBindingSupportEnabled)
            {
                _channelBindingToken = ChannelBindingUtility.GetToken(sslStream);
            }

            return sslStream;
#else
            throw ExceptionHelper.PlatformNotSupported("SslStream in UAP not supported");
#endif // FEATURE_CORECLR
        }

        protected override IAsyncResult OnBeginAcceptUpgrade(Stream stream, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeProvider async path");
        }

        protected override Stream OnEndAcceptUpgrade(IAsyncResult result, out SecurityMessageProperty remoteSecurity)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeProvider async path");
        }

#if FEATURE_CORECLR // X509Certificates
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
#endif // FEATURE_CORECLR 
    }

    internal class SslStreamSecurityUpgradeInitiator : StreamSecurityUpgradeInitiatorBase
    {
        private SslStreamSecurityUpgradeProvider _parent;
#if FEATURE_CORECLR
        private SecurityMessageProperty _serverSecurity;
#endif 
        private SecurityTokenProvider _clientCertificateProvider;
#if FEATURE_CORECLR
        private X509SecurityToken _clientToken;
#endif
        private SecurityTokenAuthenticator _serverCertificateAuthenticator;
#if FEATURE_CORECLR
        private ChannelBinding _channelBindingToken;
        private static LocalCertificateSelectionCallback s_clientCertificateSelectionCallback;
#endif

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

#if FEATURE_CORECLR // X509Certificates
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
                Fx.Assert(this.IsChannelBindingSupportEnabled, "A request for the ChannelBinding is not permitted without enabling ChannelBinding first (through the IChannelBindingProvider interface)");
                return _channelBindingToken;
            }
        }
#endif // FEATURE_CORECLR

        internal bool IsChannelBindingSupportEnabled
        {
            get
            {
                return ((IChannelBindingProvider)_parent).IsChannelBindingSupportEnabled;
            }
        }

        private IAsyncResult BaseBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return base.BeginOpen(timeout, callback, state);
        }

        private void BaseEndOpen(IAsyncResult result)
        {
            base.EndOpen(result);
        }

        internal override IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeInitiator async path");
        }

        internal override void EndOpen(IAsyncResult result)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeInitiator async path");
        }

        internal override void Open(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            base.Open(timeoutHelper.RemainingTime());
            if (_clientCertificateProvider != null)
            {
                SecurityUtils.OpenTokenProviderIfRequired(_clientCertificateProvider, timeoutHelper.RemainingTime());
#if FEATURE_CORECLR  // X509Certificates
                using (CancellationTokenSource cts = new CancellationTokenSource(timeoutHelper.RemainingTime()))
                {
                    _clientToken = (X509SecurityToken)_clientCertificateProvider.GetTokenAsync(cts.Token).GetAwaiter().GetResult();
                }
#endif // FEATURE_CORECLR
            }
        }

        private IAsyncResult BaseBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return base.BeginClose(timeout, callback, state);
        }

        private void BaseEndClose(IAsyncResult result)
        {
            base.EndClose(result);
        }

        internal override IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeInitiator async path");
        }

        internal override void EndClose(IAsyncResult result)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeInitiator async path");
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

        protected override IAsyncResult OnBeginInitiateUpgrade(Stream stream, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeInitiator async path");
        }

        protected override Stream OnEndInitiateUpgrade(IAsyncResult result,
            out SecurityMessageProperty remoteSecurity)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeInitiator async path");
        }

        protected override Stream OnInitiateUpgrade(Stream stream, out SecurityMessageProperty remoteSecurity)
        {
            if (TD.SslOnInitiateUpgradeIsEnabled())
            {
                TD.SslOnInitiateUpgrade();
            }
#if FEATURE_CORECLR // X509Certificates
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
                sslStream.AuthenticateAsClient(string.Empty, clientCertificates, _parent.SslProtocols, false);
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

            remoteSecurity = _serverSecurity;

            if (this.IsChannelBindingSupportEnabled)
            {
                _channelBindingToken = ChannelBindingUtility.GetToken(sslStream);
            }

            return sslStream;
#else
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityUpgradeInitiator - SslStream in UAP");
#endif // FEATURE_CORECLR
        }

#if FEATURE_CORECLR // X509Certificates
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
#endif // FEATURE_CORECLR
    }
}
