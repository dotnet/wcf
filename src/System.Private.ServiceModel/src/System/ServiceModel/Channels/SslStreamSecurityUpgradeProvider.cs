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
#if FEATURE_NETNATIVE
using System.IdentityModel.Claims;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Diagnostics;

using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Certificates;
#endif

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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("upgradeInitiator", SR.Format(SR.UnsupportedUpgradeInitiator, upgradeInitiator.GetType()));
            }

            if (kind != ChannelBindingKind.Endpoint)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("kind", SR.Format(SR.StreamUpgradeUnsupportedChannelBindingKind, this.GetType(), kind));
            }

            return sslUpgradeInitiator.ChannelBinding;
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

            // dotnet/wcf#1574
            // ORIGINAL CODE: 
            // _serverCertificate = new X509Certificate2(x509Token.Certificate.Handle);
            _serverCertificate = x509Token.Certificate.CloneCertificateInternal(); 
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ClientCredentialsUnableToCreateLocalTokenProvider, clientCertRequirement)));
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

#if FEATURE_NETNATIVE
        protected override async Task<Stream> OnInitiateUpgradeAsync(Stream stream, OutWrapper<SecurityMessageProperty> remoteSecurityWrapper)
        {
            if (WcfEventSource.Instance.SslOnInitiateUpgradeIsEnabled())
            {
                WcfEventSource.Instance.SslOnInitiateUpgrade();
            }

            // There is currently no way to convert a .Net X509Certificate2 to a UWP Certificate. The client certificate
            // needs to be provided by looking it up in the certificate store. E.g.
            //
            //     factory.Credentials.ClientCertificate.SetCertificate(
            //         StoreLocation.CurrentUser,
            //         StoreName.My,
            //         X509FindType.FindByThumbprint,
            //         clientCertThumb);
            //
            // The certificate is retrieved using .Net api's and UWP api's. An artifical X509Extension is used to attach the
            // UWP certificate to the .Net X509Certificate2. This is then retrieved at the point of usage to use with UWP
            // networking api's.

            Certificate clientCertificate = null;

            if (_clientToken != null)
            {
                foreach (var extension in _clientToken.Certificate.Extensions)
                {
                    var attachmentExtension =
                        extension as X509CertificateInitiatorClientCredential.X509UwpCertificateAttachmentExtension;
                    if (attachmentExtension != null && attachmentExtension.AttachedCertificate != null)
                    {
                        clientCertificate = attachmentExtension.AttachedCertificate;
                        break;
                    }
                }

                Contract.Assert(clientCertificate != null, "Missing UWP Certificate as an attachment to X509Certificate2");
            }

            try
            {
                // Fetch the underlying raw transport object. For UWP, this will be a StreamSocket
                var connectionStream = stream as ConnectionStream;
                Contract.Assert(connectionStream !=null, "stream is either null or not a ConnectionStream");
                var rtStreamSocket = connectionStream.Connection.GetCoreTransport() as StreamSocket;
                Contract.Assert(rtStreamSocket != null, "Core transport is either null or not a StreamSocket");
                rtStreamSocket.Control.ClientCertificate = clientCertificate;

                // On CoreClr, we use SslStream which calls a callback with any problems with the server certificate, which
                // returns whether to accept the certificate or not. With SocketStream in UWP, any custom validation needs to
                // happen after the connection has successfully negotiated. Some certificate errors need to be set to be ignored
                // to allow the connection to be established so we can retrieve the server certificate and choose whether to
                // accept the server certificate or not.
                rtStreamSocket.Control.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
                rtStreamSocket.Control.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);
                rtStreamSocket.Control.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
                rtStreamSocket.Control.IgnorableServerCertificateErrors.Add(ChainValidationResult.IncompleteChain);
                rtStreamSocket.Control.IgnorableServerCertificateErrors.Add(
                    ChainValidationResult.RevocationInformationMissing);
                rtStreamSocket.Control.IgnorableServerCertificateErrors.Add(ChainValidationResult.RevocationFailure);

                // SocketStream doesn't take a bitwise field of accepted protocols, but instead accepts a value specifying the highest
                // protocol that can be negotiated. A check is made for each of the protocols in order to see if they've been requested
                // by the binding and set the protection level to the UWP equivalent. This will have the effect of protectionLevel being
                // set to the most secure protocol that was specified by client code. After the connection is established, if a protocol
                // was negotiated which the binding didn't request, the connection needs to be aborted. This could happen for example if
                // the requested SslProtocols was SslProtocols.Tls11 | SslProtocols.Tls12 and the server only supported SSL3 | Tls10. In
                // this case, SocketProtectionLevel would be set to SocketProtectionLevel.Tls12, which would mean Tls10, Tls11 and Tls12
                // are all acceptable protocols to negotiate. As the server is offering SSL3 | Tls10, the connection would be successfully
                // negotiated using Tls10 which isn't allowed according to the binding configuration.
                SocketProtectionLevel protectionLevel = SocketProtectionLevel.PlainSocket;
                if ((_parent.SslProtocols & SslProtocols.Tls) != SslProtocols.None)
                    protectionLevel = SocketProtectionLevel.Tls10;
                if ((_parent.SslProtocols & SslProtocols.Tls11) != SslProtocols.None)
                    protectionLevel = SocketProtectionLevel.Tls11;
                if ((_parent.SslProtocols & SslProtocols.Tls12) != SslProtocols.None)
                    protectionLevel = SocketProtectionLevel.Tls12;

                // With SslStream, the hostname provided in the server certificate is provided to the client and verified in the callback.
                // With UWP StreamSocket, the hostname needs to be provided to the call to UpgradeToSslAsync. The code to fetch the identity
                // lives in the callback for CoreClr but needs to be pulled into this method for UWP.
                EndpointAddress remoteAddress = RemoteAddress;
                if (remoteAddress.Identity == null && remoteAddress.Uri != Via)
                {
                    remoteAddress = new EndpointAddress(Via);
                }
                EndpointIdentity identity;
                if (!_parent.IdentityVerifier.TryGetIdentity(remoteAddress, out identity))
                {
                    SecurityTraceRecordHelper.TraceIdentityVerificationFailure(identity: identity, authContext: null, identityVerifier: GetType());
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(
                        new MessageSecurityException(SR.Format(SR.IdentityCheckFailedForOutgoingMessage, identity,
                            remoteAddress)));
                }
                Contract.Assert(identity.IdentityClaim.ClaimType == ClaimTypes.Dns);
                string dnsHostName = identity.IdentityClaim.Resource as string;

                // This is the actual call to negotiate an SSL connection
                await rtStreamSocket.UpgradeToSslAsync(protectionLevel, new HostName(dnsHostName)).AsTask();

                // Verify that we didn't negotiate a protocol lower than the binding configuration specified. No need to check Tls12 
                // as it will only be negotiated if Tls12 was actually specified. 
                var negotiatedProtectionLevel = rtStreamSocket.Information.ProtectionLevel;
                if ((negotiatedProtectionLevel == SocketProtectionLevel.Tls11 && (_parent.SslProtocols & SslProtocols.Tls11) == SslProtocols.None) ||
                    (negotiatedProtectionLevel == SocketProtectionLevel.Tls10 && (_parent.SslProtocols & SslProtocols.Tls) == SslProtocols.None))
                {
                    // Need to dispose StreamSocket as normally SslStream wouldn't end up in a usable state in this situation. As
                    // post-upgrade validation is required in UWP, the connection needs to be Dispose'd to ensure it isn't used.
                    rtStreamSocket.Dispose();
                    throw new SecurityNegotiationException(SR.Format(SR.SSLProtocolNegotiationFailed, _parent.SslProtocols, negotiatedProtectionLevel));
                }

                X509Certificate2 serverCertificate = null;
                X509Certificate2[] chainCertificates = null;
                X509Chain chain = null;
                try
                {
                    // Convert the UWP Certificate object to a .Net X509Certificate2.
                    byte[] serverCertificateBlob =
                        rtStreamSocket.Information.ServerCertificate.GetCertificateBlob().ToArray();
                    serverCertificate = new X509Certificate2(serverCertificateBlob);

                    // The chain building and validation logic is done by SslStream in CoreClr. This section of code is based
                    // on the SslStream implementation to try to maintain behavior parity.
                    chain = new X509Chain();
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;

                    var serverIntermediateCertificates = rtStreamSocket.Information.ServerIntermediateCertificates;
                    chainCertificates = new X509Certificate2[serverIntermediateCertificates.Count];
                    for (int i = 0; i < chainCertificates.Length; i++)
                    {
                        chainCertificates[i] =
                            new X509Certificate2(serverIntermediateCertificates[i].GetCertificateBlob().ToArray());
                    }
                    chain.ChainPolicy.ExtraStore.AddRange(chainCertificates);

                    chain.Build(serverCertificate);
                    SslPolicyErrors policyErrors = SslPolicyErrors.None;
                    foreach (var serverCertificateError in rtStreamSocket.Information.ServerCertificateErrors)
                    {
                        if (serverCertificateError == ChainValidationResult.InvalidName)
                        {
                            policyErrors |= SslPolicyErrors.RemoteCertificateNameMismatch;
                            continue;
                        }
                        if (serverCertificateError == ChainValidationResult.IncompleteChain)
                        {
                            policyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
                        }
                    }

                    X509ChainStatus[] chainStatusArray = chain.ChainStatus;
                    if (chainStatusArray != null && chainStatusArray.Length != 0)
                    {
                        policyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
                    }

                    if (!ValidateRemoteCertificate(this, serverCertificate, chain, policyErrors))
                    {
                        // Need to dispose StreamSocket as normally SslStream wouldn't end up in a usable state in this situation. As
                        // post-upgrade validation is required in UWP, the connection needs to be Dispose'd to ensure it isn't used.
                        rtStreamSocket.Dispose();
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new SecurityNegotiationException(SR.ssl_io_cert_validation));
                    }
                }
                finally
                {
                    serverCertificate?.Dispose();
                    chain?.Dispose();
                    if (chainCertificates != null)
                    {
                        foreach (var chainCert in chainCertificates)
                        {
                            chainCert?.Dispose();
                        }
                    }
                }
            }
            catch (SecurityTokenValidationException tokenValidationException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new SecurityNegotiationException(tokenValidationException.Message,
                        tokenValidationException));
            }
            catch (IOException ioException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(
                    SR.Format(SR.NegotiationFailedIO, ioException.Message), ioException));
            }
            catch (Exception exception)
            {
                // In NET Native the WinRT API's can throw the base Exception
                // class with an HRESULT indicating the issue.  However, custom
                // validation code can also throw Exception, and to be compatible
                // with the CoreCLR version, we must allow those exceptions to
                // propagate without wrapping them.  We use the simple heuristic
                // that if an HRESULT has been set to other than the default,
                // the exception should be wrapped in SecurityNegotiationException.
                if (exception.HResult == __HResults.COR_E_EXCEPTION)
                {
                    throw;
                }
                
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(
                    exception.Message, exception));
            }

            remoteSecurityWrapper.Value = _serverSecurity;

            return stream;
        }
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
                    SR.Format(SR.NegotiationFailedIO, ioException.Message), ioException));
            }

            remoteSecurityWrapper.Value = _serverSecurity;

            if (this.IsChannelBindingSupportEnabled)
            {
                _channelBindingToken = ChannelBindingUtility.GetToken(sslStream);
            }

            return sslStream;
        }
#endif //!FEATURE_NETNATIVE

        private static X509Certificate SelectClientCertificate(object sender, string targetHost,
            X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return localCertificates[0];
        }

        private bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            // Note: add ref to handle since the caller will reset the cert after the callback return.

            // dotnet/wcf#1574
            // ORIGINAL CODE: 
            // X509Certificate2 certificate2 = new X509Certificate2(certificate.Handle);
            X509Certificate2 certificate2 = certificate.CloneCertificateInternal(); 

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
