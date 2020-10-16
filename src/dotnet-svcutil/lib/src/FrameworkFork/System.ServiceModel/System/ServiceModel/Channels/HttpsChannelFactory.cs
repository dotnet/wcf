// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Net.Security;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class HttpsChannelFactory<TChannel> : HttpChannelFactory<TChannel>
    {
        private bool _requireClientCertificate;
        private X509CertificateValidator _sslCertificateValidator;
        private Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> _remoteCertificateValidationCallback;

        internal HttpsChannelFactory(HttpsTransportBindingElement httpsBindingElement, BindingContext context)
            : base(httpsBindingElement, context)
        {
            _requireClientCertificate = httpsBindingElement.RequireClientCertificate;
            ClientCredentials credentials = context.BindingParameters.Find<ClientCredentials>();
            if (credentials != null && credentials.ServiceCertificate.SslCertificateAuthentication != null)
            {
                _sslCertificateValidator = credentials.ServiceCertificate.SslCertificateAuthentication.GetCertificateValidator();
                _remoteCertificateValidationCallback = RemoteCertificateValidationCallback;
            }
        }

        public override string Scheme
        {
            get
            {
                return UriEx.UriSchemeHttps;
            }
        }

        public bool RequireClientCertificate
        {
            get
            {
                return _requireClientCertificate;
            }
        }

        public override bool IsChannelBindingSupportEnabled
        {
            get
            {
                return false;
            }
        }

        public override T GetProperty<T>()
        {
            return base.GetProperty<T>();
        }


        protected override void ValidateCreateChannelParameters(EndpointAddress remoteAddress, Uri via)
        {
            if (string.Compare(via.Scheme, "wss", StringComparison.OrdinalIgnoreCase) != 0)
            {
                ValidateScheme(via);
            }

            if (MessageVersion.Addressing == AddressingVersion.None && remoteAddress.Uri != via)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateToMustEqualViaException(remoteAddress.Uri, via));
            }
        }

        protected override TChannel OnCreateChannelCore(EndpointAddress address, Uri via)
        {
            ValidateCreateChannelParameters(address, via);
            ValidateWebSocketTransportUsage();

            if (typeof(TChannel) == typeof(IRequestChannel))
            {
                return (TChannel)(object)new HttpsClientRequestChannel((HttpsChannelFactory<IRequestChannel>)(object)this, address, via, ManualAddressing);
            }
            else
            {
                return (TChannel)(object)new ClientWebSocketTransportDuplexSessionChannel((HttpChannelFactory<IDuplexSessionChannel>)(object)this, _clientWebSocketFactory, address, via);
            }
        }

        protected override bool IsSecurityTokenManagerRequired()
        {
            return _requireClientCertificate || base.IsSecurityTokenManagerRequired();
        }


        private void OnOpenCore()
        {
            if (_requireClientCertificate)
            {
                throw ExceptionHelper.PlatformNotSupported("Client certificates");
            }
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            base.OnEndOpen(result);
            OnOpenCore();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            base.OnOpen(timeout);
            OnOpenCore();
        }

        internal SecurityTokenProvider CreateAndOpenCertificateTokenProvider(EndpointAddress target, Uri via, ChannelParameterCollection channelParameters, TimeSpan timeout)
        {
            if (!RequireClientCertificate)
            {
                return null;
            }
            SecurityTokenProvider certificateProvider = TransportSecurityHelpers.GetCertificateTokenProvider(
                SecurityTokenManager, target, via, Scheme, channelParameters);
            SecurityUtils.OpenTokenProviderIfRequired(certificateProvider, timeout);
            return certificateProvider;
        }

        internal SecurityTokenContainer GetCertificateSecurityToken(SecurityTokenProvider certificateProvider,
            EndpointAddress to, Uri via, ChannelParameterCollection channelParameters, ref TimeoutHelper timeoutHelper)
        {
            SecurityToken token = null;
            SecurityTokenContainer tokenContainer = null;
            SecurityTokenProvider requestCertificateProvider;
            if (ManualAddressing && RequireClientCertificate)
            {
                requestCertificateProvider = CreateAndOpenCertificateTokenProvider(to, via, channelParameters, timeoutHelper.RemainingTime());
            }
            else
            {
                requestCertificateProvider = certificateProvider;
            }

            if (requestCertificateProvider != null)
            {
                token = requestCertificateProvider.GetTokenAsync(timeoutHelper.GetCancellationToken()).GetAwaiter().GetResult();
            }

            if (ManualAddressing && RequireClientCertificate)
            {
                SecurityUtils.AbortTokenProviderIfRequired(requestCertificateProvider);
            }

            if (token != null)
            {
                tokenContainer = new SecurityTokenContainer(token);
            }

            return tokenContainer;
        }

        private void AddServerCertMappingOrSetRemoteCertificateValidationCallback(ServiceModelHttpMessageHandler messageHandler, EndpointAddress to)
        {
            Fx.Assert(messageHandler != null, "httpMessageHandler should not be null.");
            if (_sslCertificateValidator != null)
            {
                if (!messageHandler.SupportsClientCertificates)
                {
                    throw ExceptionHelper.PlatformNotSupported("Client certificates not supported yet");
                }
                messageHandler.ServerCertificateValidationCallback = _remoteCertificateValidationCallback;
            }
            else
            {
                if (to.Identity is X509CertificateEndpointIdentity)
                {
                    HttpTransportSecurityHelpers.SetServerCertificateValidationCallback(messageHandler);
                }
            }
        }

        private bool RemoteCertificateValidationCallback(HttpRequestMessage sender, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            Fx.Assert(_sslCertificateValidator != null, "sslCertificateAuthentidation should not be null.");

            try
            {
                _sslCertificateValidator.Validate(certificate);
                return true;
            }
            catch (SecurityTokenValidationException ex)
            {
                FxTrace.Exception.AsInformation(ex);
                return false;
            }
            catch (Exception ex)
            {
                if (Fx.IsFatal(ex))
                {
                    throw;
                }

                FxTrace.Exception.AsWarning(ex);
                return false;
            }
        }

        internal override ServiceModelHttpMessageHandler GetHttpMessageHandler(EndpointAddress to, SecurityTokenContainer clientCertificateToken)
        {
            ServiceModelHttpMessageHandler handler = base.GetHttpMessageHandler(to, clientCertificateToken);
            if (RequireClientCertificate)
            {
                SetCertificate(handler, clientCertificateToken);
            }

            AddServerCertMappingOrSetRemoteCertificateValidationCallback(handler, to);
            return handler;
        }

        private static void SetCertificate(ServiceModelHttpMessageHandler handler, SecurityTokenContainer clientCertificateToken)
        {
            if (clientCertificateToken != null)
            {
                if (!handler.SupportsClientCertificates)
                {
                    throw ExceptionHelper.PlatformNotSupported("Client certificates not supported yet");
                }

                X509SecurityToken x509Token = (X509SecurityToken)clientCertificateToken.Token;
                handler.ClientCertificates.Add(x509Token.Certificate);
            }
        }

        protected class HttpsClientRequestChannel : HttpClientRequestChannel
        {
            private SecurityTokenProvider _certificateProvider;
            private HttpsChannelFactory<IRequestChannel> _factory;

            public HttpsClientRequestChannel(HttpsChannelFactory<IRequestChannel> factory, EndpointAddress to, Uri via, bool manualAddressing)
                : base(factory, to, via, manualAddressing)
            {
                _factory = factory;
            }

            public new HttpsChannelFactory<IRequestChannel> Factory
            {
                get { return _factory; }
            }

            private void CreateAndOpenTokenProvider(TimeSpan timeout)
            {
                if (!ManualAddressing && Factory.RequireClientCertificate)
                {
                    _certificateProvider = Factory.CreateAndOpenCertificateTokenProvider(RemoteAddress, Via, ChannelParameters, timeout);
                }
            }

            private void CloseTokenProvider(TimeSpan timeout)
            {
                if (_certificateProvider != null)
                {
                    SecurityUtils.CloseTokenProviderIfRequired(_certificateProvider, timeout);
                }
            }

            private void AbortTokenProvider()
            {
                if (_certificateProvider != null)
                {
                    SecurityUtils.AbortTokenProviderIfRequired(_certificateProvider);
                }
            }

            protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                CreateAndOpenTokenProvider(timeoutHelper.RemainingTime());
                return base.OnBeginOpen(timeoutHelper.RemainingTime(), callback, state);
            }

            protected override void OnOpen(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                CreateAndOpenTokenProvider(timeoutHelper.RemainingTime());
                base.OnOpen(timeoutHelper.RemainingTime());
            }

            protected override void OnAbort()
            {
                AbortTokenProvider();
                base.OnAbort();
            }

            protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                CloseTokenProvider(timeoutHelper.RemainingTime());
                return base.OnBeginClose(timeoutHelper.RemainingTime(), callback, state);
            }

            protected override void OnClose(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                CloseTokenProvider(timeoutHelper.RemainingTime());
                base.OnClose(timeoutHelper.RemainingTime());
            }

            internal override void OnHttpRequestCompleted(HttpRequestMessage request)
            {
            }

            internal override async Task<HttpClient> GetHttpClientAsync(EndpointAddress to, Uri via, TimeoutHelper timeoutHelper)
            {
                SecurityTokenContainer clientCertificateToken = Factory.GetCertificateSecurityToken(_certificateProvider, to, via, this.ChannelParameters, ref timeoutHelper);
                HttpClient httpClient = await base.GetHttpClientAsync(to, via, clientCertificateToken, timeoutHelper);
                return httpClient;
            }
        }
    }
}
