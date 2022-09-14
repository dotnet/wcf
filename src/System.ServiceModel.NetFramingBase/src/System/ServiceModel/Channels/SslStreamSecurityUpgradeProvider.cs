// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
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
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class SslStreamSecurityUpgradeProvider : StreamSecurityUpgradeProvider, IStreamUpgradeChannelBindingProvider
    {
        private SecurityTokenAuthenticator _clientCertificateAuthenticator;
        private SecurityTokenProvider _serverTokenProvider;
        private bool _enableChannelBinding;

        private SslStreamSecurityUpgradeProvider(IDefaultCommunicationTimeouts timeouts, SecurityTokenManager clientSecurityTokenManager, bool requireClientCertificate, string scheme, IdentityVerifier identityVerifier, SslProtocols sslProtocols)
            : base(timeouts)
        {
            IdentityVerifier = identityVerifier;
            Scheme = scheme;
            ClientSecurityTokenManager = clientSecurityTokenManager;
            RequireClientCertificate = requireClientCertificate;
            SslProtocols = sslProtocols;
        }

        private SslStreamSecurityUpgradeProvider(IDefaultCommunicationTimeouts timeouts, SecurityTokenProvider serverTokenProvider, bool requireClientCertificate, SecurityTokenAuthenticator clientCertificateAuthenticator, string scheme, IdentityVerifier identityVerifier, SslProtocols sslProtocols)
            : base(timeouts)
        {
            _serverTokenProvider = serverTokenProvider;
            RequireClientCertificate = requireClientCertificate;
            _clientCertificateAuthenticator = clientCertificateAuthenticator;
            IdentityVerifier = identityVerifier;
            Scheme = scheme;
            SslProtocols = sslProtocols;
        }

        public static SslStreamSecurityUpgradeProvider CreateClientProvider(
            SslStreamSecurityBindingElement bindingElement, BindingContext context)
        {
            SecurityCredentialsManager credentialProvider = context.BindingParameters.Find<SecurityCredentialsManager>();

            if (credentialProvider == null)
            {
                credentialProvider = new ClientCredentials();
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

        public IdentityVerifier IdentityVerifier { get; }

        public bool RequireClientCertificate { get; }

        public X509Certificate2 ServerCertificate { get; private set; }

        public SecurityTokenAuthenticator ClientCertificateAuthenticator
        {
            get
            {
                if (_clientCertificateAuthenticator == null)
                {
                    _clientCertificateAuthenticator = new X509SecurityTokenAuthenticator(DefaultCertificateValidator);
                }

                return _clientCertificateAuthenticator;
            }
        }

        public SecurityTokenManager ClientSecurityTokenManager { get; }

        public string Scheme { get; }

        public SslProtocols SslProtocols { get; }

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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(upgradeInitiator));
            }

            SslStreamSecurityUpgradeInitiator sslUpgradeInitiator = upgradeInitiator as SslStreamSecurityUpgradeInitiator;

            if (sslUpgradeInitiator == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("upgradeInitiator", SR.Format(SR.UnsupportedUpgradeInitiator, upgradeInitiator.GetType()));
            }

            if (kind != ChannelBindingKind.Endpoint)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("kind", SR.Format(SR.StreamUpgradeUnsupportedChannelBindingKind, GetType(), kind));
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
            this.ThrowIfDisposedOrNotOpen();
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
            OnCloseAsync(timeout).WaitForCompletion();
            CleanupServerCertificate();
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            if (_clientCertificateAuthenticator != null)
            {
                return SecurityUtilsEx.CloseTokenAuthenticatorIfRequiredAsync(_clientCertificateAuthenticator, timeout);
            }

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

            ServerCertificate = new X509Certificate2(x509Token.Certificate);
        }

        private void CleanupServerCertificate()
        {
            if (ServerCertificate != null)
            {
                ServerCertificate.Dispose();
                ServerCertificate = null;
            }
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            OnOpenAsync(timeout).WaitForCompletion();
        }

        protected internal override async Task OnOpenAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await SecurityUtilsEx.OpenTokenAuthenticatorIfRequiredAsync(ClientCertificateAuthenticator, timeoutHelper.RemainingTime());

            if (_serverTokenProvider != null)
            {
                await SecurityUtils.OpenTokenProviderIfRequiredAsync(_serverTokenProvider, timeoutHelper.RemainingTime());
                SecurityToken token = _serverTokenProvider.GetTokenAsync(timeoutHelper.RemainingTime()).GetAwaiter().GetResult();
                SetupServerCertificate(token);
                await SecurityUtils.CloseTokenProviderIfRequiredAsync(_serverTokenProvider, timeoutHelper.RemainingTime());
                _serverTokenProvider = null;
            }
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnOpenAsync(timeout).ToApm(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        private static X509CertificateValidator s_defaultCertificateValidator;

        internal static X509CertificateValidator DefaultCertificateValidator
        {
            get
            {
                if (s_defaultCertificateValidator == null)
                {
                    X509ChainPolicy chainPolicy = new X509ChainPolicy();
                    chainPolicy.RevocationMode = X509RevocationMode.Online;
                    s_defaultCertificateValidator = X509CertificateValidator.CreateChainTrustValidator(useMachineContext: true, chainPolicy);
                }
                return s_defaultCertificateValidator;
            }
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
        private const string RequirementNamespace = "http://schemas.microsoft.com/ws/2006/05/servicemodel/securitytokenrequirement";
        private const string PreferSslCertificateAuthenticatorProperty = RequirementNamespace + "/PreferSslCertificateAuthenticator";
        private const string SecurityTokenTypesNamespace = "http://schemas.microsoft.com/ws/2006/05/identitymodel/tokens";
        private const string X509CertificateTokenType = SecurityTokenTypesNamespace + "/X509Certificate";
        public SslStreamSecurityUpgradeInitiator(SslStreamSecurityUpgradeProvider parent,
            EndpointAddress remoteAddress, Uri via)
            : base(FramingUpgradeString.SslOrTls, remoteAddress, via)
        {
            _parent = parent;

            InitiatorServiceModelSecurityTokenRequirement serverCertRequirement = new InitiatorServiceModelSecurityTokenRequirement();
            serverCertRequirement.TokenType = X509CertificateTokenType;
            serverCertRequirement.RequireCryptographicToken = true;
            serverCertRequirement.KeyUsage = SecurityKeyUsage.Exchange;
            serverCertRequirement.TargetAddress = remoteAddress;
            serverCertRequirement.Via = via;
            serverCertRequirement.TransportScheme = _parent.Scheme;
            serverCertRequirement.Properties[PreferSslCertificateAuthenticatorProperty] = true;

            SecurityTokenResolver dummy;
            _serverCertificateAuthenticator = parent.ClientSecurityTokenManager.CreateSecurityTokenAuthenticator(serverCertRequirement, out dummy);

            if (parent.RequireClientCertificate)
            {
                InitiatorServiceModelSecurityTokenRequirement clientCertRequirement = new InitiatorServiceModelSecurityTokenRequirement();
                clientCertRequirement.TokenType = X509CertificateTokenType;
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

        internal override async ValueTask OpenAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await base.OpenAsync(timeoutHelper.RemainingTime());
            if (_clientCertificateProvider != null)
            {
                await SecurityUtils.OpenTokenProviderIfRequiredAsync(_clientCertificateProvider, timeoutHelper.RemainingTime());
                _clientToken = (X509SecurityToken)await _clientCertificateProvider.GetTokenAsync(timeoutHelper.RemainingTime());
            }
        }

        internal override async ValueTask CloseAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await base.CloseAsync(timeoutHelper.RemainingTime());
            if (_clientCertificateProvider != null)
            {
                await SecurityUtils.CloseTokenProviderIfRequiredAsync(_clientCertificateProvider, timeoutHelper.RemainingTime());
            }
        }

        protected override async Task<(Stream upgradedStream, SecurityMessageProperty remoteSecurity)> OnInitiateUpgradeAsync(Stream stream)
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

            SslStream sslStream = new SslStream(stream, false, ValidateRemoteCertificate, selectionCallback);

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

            if (IsChannelBindingSupportEnabled)
            {
                _channelBindingToken = ChannelBindingUtility.GetToken(sslStream);
            }

            return (sslStream, _serverSecurity);
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
            SecurityToken token = new X509SecurityToken(certificate2);
            ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies = _serverCertificateAuthenticator.ValidateToken(token);
            _serverSecurity = new SecurityMessageProperty();
            _serverSecurity.TransportToken = new SecurityTokenSpecification(token, authorizationPolicies);
            _serverSecurity.ServiceSecurityContext = new ServiceSecurityContext(authorizationPolicies);

            AuthorizationContext authzContext = _serverSecurity.ServiceSecurityContext.AuthorizationContext;
            EnsureOutgoingIdentity(_parent.IdentityVerifier, RemoteAddress, Via, authzContext);

            return true;
        }

        private static void EnsureOutgoingIdentity(IdentityVerifier verifier, EndpointAddress serviceReference, Uri via, AuthorizationContext authorizationContext)
        {
            // if we don't have an identity and we have differing Uris, we should use the Via
            if (serviceReference.Identity == null && serviceReference.Uri != via)
            {
                serviceReference = new EndpointAddress(via);
            }
            if (authorizationContext == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(authorizationContext));
            }
            EndpointIdentity identity;
            if (!verifier.TryGetIdentity(serviceReference, out identity))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.IdentityCheckFailedForOutgoingMessage, identity, serviceReference)));
            }
            else
            {
                if (!verifier.CheckAccess(identity, authorizationContext))
                {
                    // CheckAccess performs a Trace on failure, no need to do it twice
                    Exception e = CreateIdentityCheckException(identity, authorizationContext, SR.IdentityCheckFailedForOutgoingMessage, serviceReference);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(e);
                }
            }
        }

        private static Exception CreateIdentityCheckException(EndpointIdentity identity, AuthorizationContext authorizationContext, string errorString, EndpointAddress serviceReference)
        {
            Exception result;

            if (identity.IdentityClaim != null
                && identity.IdentityClaim.ClaimType == ClaimTypes.Dns
                && identity.IdentityClaim.Right == Rights.PossessProperty
                && identity.IdentityClaim.Resource is string)
            {
                string expectedDnsName = (string)identity.IdentityClaim.Resource;
                string actualDnsName = null;
                for (int i = 0; i < authorizationContext.ClaimSets.Count; ++i)
                {
                    ClaimSet claimSet = authorizationContext.ClaimSets[i];
                    foreach (Claim claim in claimSet.FindClaims(ClaimTypes.Dns, Rights.PossessProperty))
                    {
                        if (claim.Resource is string)
                        {
                            actualDnsName = (string)claim.Resource;
                            break;
                        }
                    }
                    if (actualDnsName != null)
                    {
                        break;
                    }
                }
                if (actualDnsName == null)
                {
                    result = new MessageSecurityException(SR.Format(SR.DnsIdentityCheckFailedForOutgoingMessageLackOfDnsClaim, expectedDnsName));
                }
                else
                {
                    result = new MessageSecurityException(SR.Format(SR.DnsIdentityCheckFailedForOutgoingMessage, expectedDnsName, actualDnsName));
                }
            }
            else
            {
                result = new MessageSecurityException(SR.Format(errorString, identity, serviceReference));
            }

            return result;
        }
    }
}
