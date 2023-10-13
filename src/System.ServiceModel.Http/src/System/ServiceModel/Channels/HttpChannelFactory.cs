// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SecurityUtils = System.ServiceModel.Security.SecurityUtils;

namespace System.ServiceModel.Channels
{
    internal class HttpChannelFactory<TChannel> : TransportChannelFactory<TChannel>
    {
        private static CacheControlHeaderValue s_requestCacheHeader = new CacheControlHeaderValue { NoCache = true, MaxAge = new TimeSpan(0) };
        private HttpCookieContainerManager _httpCookieContainerManager;

        // Double-checked locking pattern requires volatile for read/write synchronization
        private volatile MruCache<Uri, Uri> _credentialCacheUriPrefixCache;
        private volatile MruCache<string, string> _credentialHashCache;
        private volatile MruCache<string, HttpClient> _httpClientCache;
        private SecurityCredentialsManager _channelCredentials;
        private ISecurityCapabilities _securityCapabilities;
        private Func<HttpClientHandler, HttpMessageHandler> _httpMessageHandlerFactory;
        private Lazy<string> _webSocketSoapContentType;
        private SHA512 _hashAlgorithm;
        private bool _keepAliveEnabled;

        internal HttpChannelFactory(HttpTransportBindingElement bindingElement, BindingContext context)
            : base(bindingElement, context, HttpTransportDefaults.GetDefaultMessageEncoderFactory())
        {
            // validate setting interactions
            if (bindingElement.TransferMode == TransferMode.Buffered)
            {
                if (bindingElement.MaxReceivedMessageSize > int.MaxValue)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new ArgumentOutOfRangeException("bindingElement.MaxReceivedMessageSize",
                        SR.MaxReceivedMessageSizeMustBeInIntegerRange));
                }

                if (bindingElement.MaxBufferSize != bindingElement.MaxReceivedMessageSize)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("bindingElement",
                        SR.MaxBufferSizeMustMatchMaxReceivedMessageSize);
                }
            }
            else
            {
                if (bindingElement.MaxBufferSize > bindingElement.MaxReceivedMessageSize)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("bindingElement",
                        SR.MaxBufferSizeMustNotExceedMaxReceivedMessageSize);
                }
            }

            if (TransferModeHelper.IsRequestStreamed(bindingElement.TransferMode) &&
                bindingElement.AuthenticationScheme != AuthenticationSchemes.Anonymous)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("bindingElement",
                    SR.HttpAuthDoesNotSupportRequestStreaming);
            }

            AllowCookies = bindingElement.AllowCookies;

            if (AllowCookies)
            {
                _httpCookieContainerManager = new HttpCookieContainerManager();
            }

            if (!bindingElement.AuthenticationScheme.IsSingleton() && bindingElement.AuthenticationScheme != AuthenticationSchemes.IntegratedWindowsAuthentication)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", SR.Format(SR.HttpRequiresSingleAuthScheme,
                    bindingElement.AuthenticationScheme));
            }

            AuthenticationScheme = bindingElement.AuthenticationScheme;
            DecompressionEnabled = bindingElement.DecompressionEnabled;
            MaxBufferSize = bindingElement.MaxBufferSize;
            TransferMode = bindingElement.TransferMode;
            _keepAliveEnabled = bindingElement.KeepAliveEnabled;

            if (bindingElement.Proxy != null)
            {
                Proxy = bindingElement.Proxy;
            }
            else if (bindingElement.ProxyAddress != null)
            {
                if (bindingElement.UseDefaultWebProxy)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.UseDefaultWebProxyCantBeUsedWithExplicitProxyAddress));
                }

                if (bindingElement.ProxyAuthenticationScheme == AuthenticationSchemes.Anonymous)
                {
                    Proxy = new WebProxy(bindingElement.ProxyAddress, bindingElement.BypassProxyOnLocal);
                }
                else
                {
                    Proxy = null;
                    ProxyFactory =
                        new WebProxyFactory(bindingElement.ProxyAddress, bindingElement.BypassProxyOnLocal,
                        bindingElement.ProxyAuthenticationScheme);
                }
            }
            else if (!bindingElement.UseDefaultWebProxy)
            {
                Proxy = new WebProxy();
            }

            _channelCredentials = context.BindingParameters.Find<SecurityCredentialsManager>();
            _securityCapabilities = bindingElement.GetProperty<ISecurityCapabilities>(context);
            _httpMessageHandlerFactory = context.BindingParameters.Find<Func<HttpClientHandler, HttpMessageHandler>>();

            WebSocketSettings = WebSocketHelper.GetRuntimeWebSocketSettings(bindingElement.WebSocketSettings);
            _webSocketSoapContentType = new Lazy<string>(() => MessageEncoderFactory.CreateSessionEncoder().ContentType, LazyThreadSafetyMode.ExecutionAndPublication);
            _httpClientCache = bindingElement.GetProperty<MruCache<string, HttpClient>>(context);
        }

        public bool AllowCookies { get; }

        public AuthenticationSchemes AuthenticationScheme { get; }

        public bool DecompressionEnabled { get; }

        public virtual bool IsChannelBindingSupportEnabled
        {
            get
            {
                return false;
            }
        }

        public SecurityTokenManager SecurityTokenManager { get; private set; }

        public int MaxBufferSize { get; }

        internal IWebProxy Proxy { get; set; }
        internal WebProxyFactory ProxyFactory { get; set; }

        public TransferMode TransferMode { get; }

        public override string Scheme
        {
            get
            {
                return UriEx.UriSchemeHttp;
            }
        }

        public WebSocketTransportSettings WebSocketSettings { get; }

        internal string WebSocketSoapContentType
        {
            get
            {
                return _webSocketSoapContentType.Value;
            }
        }

        private HashAlgorithm HashAlgorithm
        {
            get
            {
                if (_hashAlgorithm == null)
                {
                    _hashAlgorithm = SHA512.Create();
                }
                else
                {
                    _hashAlgorithm.Initialize();
                }

                return _hashAlgorithm;
            }
        }

        private bool AuthenticationSchemeMayRequireResend()
        {
            return AuthenticationScheme != AuthenticationSchemes.Anonymous;
        }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T)(object)_securityCapabilities;
            }
            if (typeof(T) == typeof(IHttpCookieContainerManager))
            {
                return (T)(object)GetHttpCookieContainerManager();
            }

            return base.GetProperty<T>();
        }

        internal HttpCookieContainerManager GetHttpCookieContainerManager()
        {
            return _httpCookieContainerManager;
        }

        internal Uri GetCredentialCacheUriPrefix(Uri via)
        {
            Uri result;

            if (_credentialCacheUriPrefixCache == null)
            {
                lock (ThisLock)
                {
                    if (_credentialCacheUriPrefixCache == null)
                    {
                        _credentialCacheUriPrefixCache = new MruCache<Uri, Uri>(10);
                    }
                }
            }

            lock (_credentialCacheUriPrefixCache)
            {
                if (!_credentialCacheUriPrefixCache.TryGetValue(via, out result))
                {
                    result = new UriBuilder(via.Scheme, via.Host, via.Port).Uri;
                    _credentialCacheUriPrefixCache.Add(via, result);
                }
            }

            return result;
        }

        internal async Task<HttpClient> GetHttpClientAsync(EndpointAddress to, Uri via,
            SecurityTokenProviderContainer tokenProvider, SecurityTokenProviderContainer proxyTokenProvider,
            SecurityTokenContainer clientCertificateToken, TimeSpan timeout)
        {
            (NetworkCredential credential, TokenImpersonationLevel impersonationLevel, AuthenticationLevel authenticationLevel) = await HttpChannelUtilities.GetCredentialAsync(AuthenticationScheme,
                tokenProvider, timeout);

            HttpClient httpClient;

            string connectionGroupName = GetConnectionGroupName(credential, authenticationLevel, impersonationLevel, clientCertificateToken);

            X509CertificateEndpointIdentity remoteCertificateIdentity = to.Identity as X509CertificateEndpointIdentity;
            if (remoteCertificateIdentity != null)
            {
                connectionGroupName = string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", connectionGroupName,
                    remoteCertificateIdentity.Certificates[0].Thumbprint);
            }

            connectionGroupName = connectionGroupName ?? string.Empty;
            bool foundHttpClient;
            lock (_httpClientCache)
            {
                foundHttpClient = _httpClientCache.TryGetValue(connectionGroupName, out httpClient);
            }

            if (!foundHttpClient)
            {
                var clientHandler = GetHttpClientHandler(to, clientCertificateToken);
                if (DecompressionEnabled)
                {
                    clientHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                }
                else
                {
                    clientHandler.AutomaticDecompression = DecompressionMethods.None;
                }

                if (clientHandler.SupportsProxy)
                {
                    if (Proxy != null)
                    {
                        clientHandler.Proxy = Proxy;
                        clientHandler.UseProxy = true;
                    }
                    else if (ProxyFactory != null)
                    {
                        clientHandler.Proxy = await ProxyFactory.CreateWebProxyAsync(authenticationLevel,
                            impersonationLevel, proxyTokenProvider, timeout);
                        clientHandler.UseProxy = true;
                    }
                }

                clientHandler.UseCookies = AllowCookies;
                if (AllowCookies)
                {
                    clientHandler.CookieContainer = _httpCookieContainerManager.CookieContainer;
                }

                clientHandler.PreAuthenticate = true;

                clientHandler.UseDefaultCredentials = false;
                if (credential == CredentialCache.DefaultCredentials || credential == null)
                {
                    if (AuthenticationScheme != AuthenticationSchemes.Anonymous)
                    {
                        clientHandler.UseDefaultCredentials = true;
                    }
                }
                else
                {
                    if (Fx.IsUap)
                    {
                        clientHandler.Credentials = credential;
                    }
                    else
                    {
                        CredentialCache credentials = new CredentialCache();
                        Uri credentialCacheUriPrefix = GetCredentialCacheUriPrefix(via);
                        if (AuthenticationScheme == AuthenticationSchemes.IntegratedWindowsAuthentication)
                        {
                            credentials.Add(credentialCacheUriPrefix, AuthenticationSchemesHelper.ToString(AuthenticationSchemes.Negotiate),
                                credential);
                            credentials.Add(credentialCacheUriPrefix, AuthenticationSchemesHelper.ToString(AuthenticationSchemes.Ntlm),
                                credential);
                        }
                        else
                        {
                            credentials.Add(credentialCacheUriPrefix, AuthenticationSchemesHelper.ToString(AuthenticationScheme),
                                credential);
                        }

                        clientHandler.Credentials = credentials;
                    }
                }

                HttpMessageHandler handler = clientHandler;
                if (_httpMessageHandlerFactory != null)
                {
                    handler = _httpMessageHandlerFactory(clientHandler);
                }

                httpClient = new HttpClient(handler);

                if (!_keepAliveEnabled)
                {
                    httpClient.DefaultRequestHeaders.ConnectionClose = true;
                }

                if (IsExpectContinueHeaderRequired && !Fx.IsUap)
                {
                    httpClient.DefaultRequestHeaders.ExpectContinue = true;
                }

                // We provide our own CancellationToken for each request. Setting HttpClient.Timeout to -1
                // prevents a call to CancellationToken.CancelAfter that HttpClient does internally which
                // causes TimerQueue contention at high load.
                httpClient.Timeout = Timeout.InfiniteTimeSpan;

                lock (_httpClientCache)
                {
                    HttpClient tempHttpClient;
                    if (_httpClientCache.TryGetValue(connectionGroupName, out tempHttpClient))
                    {
                        httpClient.Dispose();
                        httpClient = tempHttpClient;
                    }
                    else
                    {
                        _httpClientCache.Add(connectionGroupName, httpClient);
                    }
                }
            }

            return httpClient;
        }

        internal virtual bool IsExpectContinueHeaderRequired => AuthenticationSchemeMayRequireResend();

        internal virtual HttpClientHandler GetHttpClientHandler(EndpointAddress to, SecurityTokenContainer clientCertificateToken)
        {
            return new HttpClientHandler();
        }

        internal ICredentials GetCredentials()
        {
            ICredentials creds = null;
            if (AuthenticationScheme != AuthenticationSchemes.Anonymous)
            {
                creds = CredentialCache.DefaultCredentials;
                ClientCredentials credentials = _channelCredentials as ClientCredentials;
                if (credentials != null)
                {
                    switch (AuthenticationScheme)
                    {
                        case AuthenticationSchemes.Basic:
                            if (credentials.UserName.UserName == null)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(ClientCredentials.UserName.UserName));
                            }

                            if (credentials.UserName.UserName == string.Empty)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.UserNameCannotBeEmpty);
                            }

                            creds = new NetworkCredential(credentials.UserName.UserName, credentials.UserName.Password);
                            break;
                        case AuthenticationSchemes.Digest:
                            if (credentials.HttpDigest.ClientCredential.UserName != string.Empty)
                            {
                                creds = credentials.HttpDigest.ClientCredential;
                            }
                            break;
                        case AuthenticationSchemes.Ntlm:
                        case AuthenticationSchemes.IntegratedWindowsAuthentication:
                        case AuthenticationSchemes.Negotiate:
                            if (credentials.Windows.ClientCredential.UserName != string.Empty)
                            {
                                creds = credentials.Windows.ClientCredential;
                            }
                            break;
                    }
                }
            }
            return creds;
        }


        internal Exception CreateToMustEqualViaException(Uri to, Uri via)
        {
            return new ArgumentException(SR.Format(SR.HttpToMustEqualVia, to, via));
        }


        public override int GetMaxBufferSize()
        {
            return MaxBufferSize;
        }

        private async Task<SecurityTokenProviderContainer> CreateAndOpenTokenProviderAsync(TimeSpan timeout, AuthenticationSchemes authenticationScheme,
            EndpointAddress target, Uri via, ChannelParameterCollection channelParameters)
        {
            SecurityTokenProvider tokenProvider = null;
            switch (authenticationScheme)
            {
                case AuthenticationSchemes.Anonymous:
                    break;
                case AuthenticationSchemes.Basic:
                    tokenProvider = TransportSecurityHelpers.GetUserNameTokenProvider(SecurityTokenManager, target, via, Scheme, authenticationScheme, channelParameters);
                    break;
                case AuthenticationSchemes.Negotiate:
                case AuthenticationSchemes.Ntlm:
                case AuthenticationSchemes.IntegratedWindowsAuthentication:
                    tokenProvider = TransportSecurityHelpers.GetSspiTokenProvider(SecurityTokenManager, target, via, Scheme, authenticationScheme, channelParameters);
                    break;
                case AuthenticationSchemes.Digest:
                    tokenProvider = TransportSecurityHelpers.GetDigestTokenProvider(SecurityTokenManager, target, via, Scheme, authenticationScheme, channelParameters);
                    break;
                default:
                    // The setter for this property should prevent this.
                    throw Fx.AssertAndThrow("CreateAndOpenTokenProvider: Invalid authentication scheme");
            }
            SecurityTokenProviderContainer result;
            if (tokenProvider != null)
            {
                result = new SecurityTokenProviderContainer(tokenProvider);
                await result.OpenAsync(timeout);
            }
            else
            {
                result = null;
            }
            return result;
        }

        protected virtual void ValidateCreateChannelParameters(EndpointAddress remoteAddress, Uri via)
        {
            if (string.Compare(via.Scheme, "ws", StringComparison.OrdinalIgnoreCase) != 0)
            {
                ValidateScheme(via);
            }

            if (MessageVersion.Addressing == AddressingVersion.None && remoteAddress.Uri != via)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateToMustEqualViaException(remoteAddress.Uri, via));
            }
        }

        protected override TChannel OnCreateChannel(EndpointAddress remoteAddress, Uri via)
        {
            if (typeof(TChannel) != typeof(IRequestChannel))
            {
                remoteAddress = remoteAddress != null && !WebSocketHelper.IsWebSocketUri(remoteAddress.Uri) ?
                    new EndpointAddress(WebSocketHelper.NormalizeHttpSchemeWithWsScheme(remoteAddress.Uri), remoteAddress) :
                    remoteAddress;
                via = !WebSocketHelper.IsWebSocketUri(via) ? WebSocketHelper.NormalizeHttpSchemeWithWsScheme(via) : via;
            }

            return OnCreateChannelCore(remoteAddress, via);
        }

        protected virtual TChannel OnCreateChannelCore(EndpointAddress remoteAddress, Uri via)
        {
            ValidateCreateChannelParameters(remoteAddress, via);
            ValidateWebSocketTransportUsage();

            if (typeof(TChannel) == typeof(IRequestChannel))
            {
                return (TChannel)(object)new HttpClientRequestChannel((HttpChannelFactory<IRequestChannel>)(object)this, remoteAddress, via, ManualAddressing);
            }
            else
            {
                return (TChannel)(object)new ClientWebSocketTransportDuplexSessionChannel((HttpChannelFactory<IDuplexSessionChannel>)(object)this, remoteAddress, via);
            }
        }

        protected void ValidateWebSocketTransportUsage()
        {
            Type channelType = typeof(TChannel);
            if (channelType == typeof(IRequestChannel) && WebSocketSettings.TransportUsage == WebSocketTransportUsage.Always)
            {
                throw FxTrace.Exception.AsError(new InvalidOperationException(SR.Format(
                            SR.WebSocketCannotCreateRequestClientChannelWithCertainWebSocketTransportUsage,
                            typeof(TChannel),
                            WebSocketTransportSettings.TransportUsageMethodName,
                            typeof(WebSocketTransportSettings).Name,
                            WebSocketSettings.TransportUsage)));
            }

            if (channelType == typeof(IDuplexSessionChannel))
            {
                if (WebSocketSettings.TransportUsage == WebSocketTransportUsage.Never)
                {
                    throw FxTrace.Exception.AsError(new InvalidOperationException(SR.Format(
                                SR.WebSocketCannotCreateRequestClientChannelWithCertainWebSocketTransportUsage,
                                typeof(TChannel),
                                WebSocketTransportSettings.TransportUsageMethodName,
                                typeof(WebSocketTransportSettings).Name,
                                WebSocketSettings.TransportUsage)));
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InitializeSecurityTokenManager()
        {
            if (_channelCredentials == null)
            {
                _channelCredentials = ClientCredentials.CreateDefaultCredentials();
            }
            SecurityTokenManager = _channelCredentials.CreateSecurityTokenManager();
        }

        protected virtual bool IsSecurityTokenManagerRequired()
        {
            if (AuthenticationScheme != AuthenticationSchemes.Anonymous)
            {
                return true;
            }
            if (ProxyFactory != null && ProxyFactory.AuthenticationScheme != AuthenticationSchemes.Anonymous)
            {
                return true;
            }
            else
            {
                return false;
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

        protected override void OnOpen(TimeSpan timeout)
        {
            if (IsSecurityTokenManagerRequired())
            {
                InitializeSecurityTokenManager();
            }

            if (AllowCookies &&
                !_httpCookieContainerManager.IsInitialized) // We don't want to overwrite the CookieContainer if someone has set it already.
            {
                _httpCookieContainerManager.CookieContainer = new CookieContainer();
            }
        }

        internal protected override Task OnOpenAsync(TimeSpan timeout)
        {
            OnOpen(timeout);
            return TaskHelpers.CompletedTask();
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            return base.OnCloseAsync(timeout);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            if (_httpClientCache != null && !_httpClientCache.IsDisposed)
            {
                lock (_httpClientCache)
                {
                    _httpClientCache.Dispose();
                    _httpClientCache = null;
                }
            }
        }

        private string AppendWindowsAuthenticationInfo(string inputString, NetworkCredential credential,
    AuthenticationLevel authenticationLevel, TokenImpersonationLevel impersonationLevel)
        {
            return SecurityUtils.AppendWindowsAuthenticationInfo(inputString, credential, authenticationLevel, impersonationLevel);
        }

        protected virtual string OnGetConnectionGroupPrefix(SecurityTokenContainer clientCertificateToken)
        {
            return string.Empty;
        }

        internal static bool IsWindowsAuth(AuthenticationSchemes authScheme)
        {
            Fx.Assert(authScheme.IsSingleton() || authScheme == AuthenticationSchemes.IntegratedWindowsAuthentication, "authenticationScheme used in an Http(s)ChannelFactory must be a singleton value.");

            return authScheme == AuthenticationSchemes.Negotiate ||
                authScheme == AuthenticationSchemes.Ntlm ||
                authScheme == AuthenticationSchemes.IntegratedWindowsAuthentication;
        }

        private string GetConnectionGroupName(NetworkCredential credential, AuthenticationLevel authenticationLevel,
            TokenImpersonationLevel impersonationLevel, SecurityTokenContainer clientCertificateToken)
        {
            if (_credentialHashCache == null)
            {
                lock (ThisLock)
                {
                    if (_credentialHashCache == null)
                    {
                        _credentialHashCache = new MruCache<string, string>(5);
                    }
                }
            }

            string inputString = TransferModeHelper.IsRequestStreamed(TransferMode) ? "streamed" : string.Empty;

            if (IsWindowsAuth(AuthenticationScheme))
            {
                inputString = AppendWindowsAuthenticationInfo(inputString, credential, authenticationLevel, impersonationLevel);
            }

            inputString = string.Concat(OnGetConnectionGroupPrefix(clientCertificateToken), inputString);

            string credentialHash = null;

            // we have to lock around each call to TryGetValue since the MruCache modifies the
            // contents of it's mruList in a single-threaded manner underneath TryGetValue
            if (!string.IsNullOrEmpty(inputString))
            {
                lock (_credentialHashCache)
                {
                    if (!_credentialHashCache.TryGetValue(inputString, out credentialHash))
                    {
                        byte[] inputBytes = new UTF8Encoding().GetBytes(inputString);
                        byte[] digestBytes = HashAlgorithm.ComputeHash(inputBytes);
                        credentialHash = Convert.ToBase64String(digestBytes);
                        _credentialHashCache.Add(inputString, credentialHash);
                    }
                }
            }

            return credentialHash;
        }

        internal HttpRequestMessage GetHttpRequestMessage(Uri via)
        {
            Uri httpRequestUri = via;

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, httpRequestUri);
            if (TransferModeHelper.IsRequestStreamed(TransferMode))
            {
                requestMessage.Headers.TransferEncodingChunked = true;
            }

            requestMessage.Headers.CacheControl = s_requestCacheHeader;
            return requestMessage;
        }

        private void ApplyManualAddressing(ref EndpointAddress to, ref Uri via, Message message)
        {
            if (ManualAddressing)
            {
                Uri toHeader = message.Headers.To;
                if (toHeader == null)
                {
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SR.ManualAddressingRequiresAddressedMessages), message);
                }

                to = new EndpointAddress(toHeader);

                if (MessageVersion.Addressing == AddressingVersion.None)
                {
                    via = toHeader;
                }
            }

            // now apply query string property
            object property;
            if (message.Properties.TryGetValue(HttpRequestMessageProperty.Name, out property))
            {
                HttpRequestMessageProperty requestProperty = (HttpRequestMessageProperty)property;
                if (!string.IsNullOrEmpty(requestProperty.QueryString))
                {
                    UriBuilder uriBuilder = new UriBuilder(via);

                    if (requestProperty.QueryString.StartsWith("?", StringComparison.Ordinal))
                    {
                        uriBuilder.Query = requestProperty.QueryString.Substring(1);
                    }
                    else
                    {
                        uriBuilder.Query = requestProperty.QueryString;
                    }

                    via = uriBuilder.Uri;
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private async Task<(SecurityTokenProviderContainer tokenProvider, SecurityTokenProviderContainer proxyTokenProvider)> CreateAndOpenTokenProvidersCoreAsync(EndpointAddress to, Uri via, ChannelParameterCollection channelParameters, TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            SecurityTokenProviderContainer tokenProvider = await CreateAndOpenTokenProviderAsync(timeoutHelper.RemainingTime(), AuthenticationScheme, to, via, channelParameters);
            SecurityTokenProviderContainer proxyTokenProvider;
            if (ProxyFactory != null)
            {
                proxyTokenProvider = await CreateAndOpenTokenProviderAsync(timeoutHelper.RemainingTime(), ProxyFactory.AuthenticationScheme, to, via, channelParameters);
            }
            else
            {
                proxyTokenProvider = null;
            }

            return (tokenProvider, proxyTokenProvider);
        }

        internal Task<(SecurityTokenProviderContainer tokenProvider, SecurityTokenProviderContainer proxyTokenProvider)> CreateAndOpenTokenProvidersAsync(EndpointAddress to, Uri via, ChannelParameterCollection channelParameters, TimeSpan timeout)
        {
            if (!IsSecurityTokenManagerRequired())
            {
                (SecurityTokenProviderContainer tokenProvider, SecurityTokenProviderContainer proxyTokenProvider) result = (null, null);
                return Task.FromResult(result);
            }
            else
            {
                return CreateAndOpenTokenProvidersCoreAsync(to, via, channelParameters, timeout);
            }
        }

        internal static bool MapIdentity(EndpointAddress target, AuthenticationSchemes authenticationScheme)
        {
            if (target.Identity == null)
            {
                return false;
            }

            return IsWindowsAuth(authenticationScheme);
        }

        private bool MapIdentity(EndpointAddress target)
        {
            return MapIdentity(target, AuthenticationScheme);
        }

        protected class HttpClientRequestChannel : RequestChannel
        {
            private SecurityTokenProviderContainer _tokenProvider;
            private SecurityTokenProviderContainer _proxyTokenProvider;

            public HttpClientRequestChannel(HttpChannelFactory<IRequestChannel> factory, EndpointAddress to, Uri via, bool manualAddressing)
                : base(factory, to, via, manualAddressing)
            {
                Factory = factory;
            }

            public HttpChannelFactory<IRequestChannel> Factory { get; }

            protected ChannelParameterCollection ChannelParameters { get; private set; }

            public override T GetProperty<T>()
            {
                if (typeof(T) == typeof(ChannelParameterCollection))
                {
                    if (State == CommunicationState.Created)
                    {
                        lock (ThisLock)
                        {
                            if (ChannelParameters == null)
                            {
                                ChannelParameters = new ChannelParameterCollection();
                            }
                        }
                    }
                    return (T)(object)ChannelParameters;
                }

                return base.GetProperty<T>();
            }

            private void PrepareOpen()
            {
                Factory.MapIdentity(RemoteAddress);
            }

            private async Task CreateAndOpenTokenProvidersAsync(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                if (!ManualAddressing)
                {
                    (_tokenProvider, _proxyTokenProvider) = await Factory.CreateAndOpenTokenProvidersAsync(RemoteAddress, Via, ChannelParameters, timeoutHelper.RemainingTime());
                }
            }

            private void CloseTokenProviders(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                if (_tokenProvider != null)
                {
                    _tokenProvider.Close(timeoutHelper.RemainingTime());
                }
            }

            private void AbortTokenProviders()
            {
                if (_tokenProvider != null)
                {
                    _tokenProvider.Abort();
                }
            }

            protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return CommunicationObjectInternal.OnBeginOpen(this, timeout, callback, state);
            }

            protected override void OnEndOpen(IAsyncResult result)
            {
                CommunicationObjectInternal.OnEnd(result);
            }

            protected override void OnOpen(TimeSpan timeout)
            {
                CommunicationObjectInternal.OnOpen(this, timeout);
            }

            internal protected override Task OnOpenAsync(TimeSpan timeout)
            {
                PrepareOpen();
                return CreateAndOpenTokenProvidersAsync(timeout);
            }

            private void PrepareClose(bool aborting)
            {
            }

            protected override void OnAbort()
            {
                PrepareClose(true);
                AbortTokenProviders();
                base.OnAbort();
            }

            protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return CommunicationObjectInternal.OnBeginClose(this, timeout, callback, state);
            }

            protected override void OnEndClose(IAsyncResult result)
            {
                CommunicationObjectInternal.OnEnd(result);
            }

            protected override void OnClose(TimeSpan timeout)
            {
                CommunicationObjectInternal.OnClose(this, timeout);
            }

            protected internal override async Task OnCloseAsync(TimeSpan timeout)
            {
                var timeoutHelper = new TimeoutHelper(timeout);
                PrepareClose(false);
                CloseTokenProviders(timeoutHelper.RemainingTime());
                await WaitForPendingRequestsAsync(timeoutHelper.RemainingTime());
            }

            protected override IAsyncRequest CreateAsyncRequest(Message message)
            {
                return new HttpClientChannelAsyncRequest(this);
            }

            internal virtual Task<HttpClient> GetHttpClientAsync(EndpointAddress to, Uri via, TimeoutHelper timeoutHelper)
            {
                return GetHttpClientAsync(to, via, null, timeoutHelper);
            }

            protected async Task<HttpClient> GetHttpClientAsync(EndpointAddress to, Uri via, SecurityTokenContainer clientCertificateToken, TimeoutHelper timeoutHelper)
            {
                SecurityTokenProviderContainer requestTokenProvider;
                SecurityTokenProviderContainer requestProxyTokenProvider;
                if (ManualAddressing)
                {
                    (requestTokenProvider, requestProxyTokenProvider) = await Factory.CreateAndOpenTokenProvidersAsync(to, via, ChannelParameters, timeoutHelper.RemainingTime());
                }
                else
                {
                    requestTokenProvider = _tokenProvider;
                    requestProxyTokenProvider = _proxyTokenProvider;
                }

                try
                {
                    return await Factory.GetHttpClientAsync(to, via, requestTokenProvider, requestProxyTokenProvider, clientCertificateToken, timeoutHelper.RemainingTime());
                }
                finally
                {
                    if (ManualAddressing)
                    {
                        if (requestTokenProvider != null)
                        {
                            requestTokenProvider.Abort();
                        }
                    }
                }
            }

            internal HttpRequestMessage GetHttpRequestMessage(Uri via)
            {
                return Factory.GetHttpRequestMessage(via);
            }

            internal virtual void OnHttpRequestCompleted(HttpRequestMessage request)
            {
                // empty
            }

            internal class HttpClientChannelAsyncRequest : IAsyncRequest
            {
                private static readonly Action<object> s_cancelCts = state =>
                {
                    try
                    {
                        ((CancellationTokenSource)state).Cancel();
                    }
                    catch (ObjectDisposedException)
                    {
                        // ignore
                    }
                };

                private HttpClientRequestChannel _channel;
                private HttpChannelFactory<IRequestChannel> _factory;
                private EndpointAddress _to;
                private Uri _via;
                private HttpRequestMessage _httpRequestMessage;
                private HttpResponseMessage _httpResponseMessage;
                private HttpAbortReason _abortReason;
                private TimeoutHelper _timeoutHelper;
                private int _httpRequestCompleted;
                private HttpClient _httpClient;
                private readonly CancellationTokenSource _httpSendCts;

                public HttpClientChannelAsyncRequest(HttpClientRequestChannel channel)
                {
                    _channel = channel;
                    _to = channel.RemoteAddress;
                    _via = channel.Via;
                    _factory = channel.Factory;
                    _httpSendCts = new CancellationTokenSource();
                }

                public async Task SendRequestAsync(Message message, TimeoutHelper timeoutHelper)
                {
                    _timeoutHelper = timeoutHelper;
                    if (_channel.Factory.MapIdentity(_to))
                    {
                        HttpTransportSecurityHelpers.AddIdentityMapping(_to, message);
                    }

                    _factory.ApplyManualAddressing(ref _to, ref _via, message);
                    _httpClient = await _channel.GetHttpClientAsync(_to, _via, _timeoutHelper);

                    // The _httpRequestMessage field will be set to null by Cleanup() due to faulting
                    // or aborting, so use a local copy for exception handling within this method.
                    HttpRequestMessage httpRequestMessage = _channel.GetHttpRequestMessage(_via);
                    _httpRequestMessage = httpRequestMessage;
                    Message request = message;

                    try
                    {
                        if (_channel.State != CommunicationState.Opened)
                        {
                            // if we were aborted while getting our request or doing correlation,
                            // we need to abort the request and bail
                            Cleanup();
                            _channel.ThrowIfDisposedOrNotOpen();
                        }

                        bool suppressEntityBody = PrepareMessageHeaders(request);

                        if (!suppressEntityBody)
                        {
                            httpRequestMessage.Content = MessageContent.Create(_factory, request, _timeoutHelper);
                            var contentType = httpRequestMessage.Content.Headers.ContentType;
                            if (contentType!= null &&
                                contentType.MediaType == "multipart/related" &&
                                contentType.Parameters.Contains(new NameValueHeaderValue("type", "\"application/xop+xml\"")))
                            {
                                // For MTOM messages, add a MIME version header
                                AddMimeVersion("1.0");
                                request.Properties.Add("System.ServiceModel.Channel.MtomMessageEncoder.WriteMessageHeaders", false);
                            }

                        }

                        if (Fx.IsUap)
                        {
                            try
                            {
                                // There is a possibility that a HEAD pre-auth request might fail when the actual request
                                // will succeed. For example, when the web service refuses HEAD requests. We don't want
                                // to fail the actual request because of some subtlety which causes the HEAD request.
                                await SendPreauthenticationHeadRequestIfNeeded();
                            }
                            catch { /* ignored */ }
                        }

                        bool success = false;
                        var timeoutToken = await _timeoutHelper.GetCancellationTokenAsync();

                        try
                        {
                            using (timeoutToken.Register(s_cancelCts, _httpSendCts))
                            {
                                _httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, _httpSendCts.Token);
                            }

                            // As we have the response message and no exceptions have been thrown, the request message has completed it's job.
                            // Calling Dispose() on the request message to free up resources in HttpContent, but keeping the object around
                            // as we can still query properties once dispose'd.
                            httpRequestMessage.Dispose();
                            success = true;
                        }
                        catch (HttpRequestException requestException)
                        {
                            HttpChannelUtilities.ProcessGetResponseWebException(requestException, httpRequestMessage,
                                _abortReason);
                        }
                        catch (OperationCanceledException)
                        {
                            if (timeoutToken.IsCancellationRequested)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(SR.Format(
                                    SR.HttpRequestTimedOut, httpRequestMessage.RequestUri, _timeoutHelper.OriginalTimeout)));
                            }
                            else
                            {
                                // Cancellation came from somewhere other than timeoutToken and needs to be handled differently.
                                throw;
                            }
                        }
                        finally
                        {
                            if (!success)
                            {
                                Abort(_channel);
                            }
                        }
                    }
                    finally
                    {
                        if (!ReferenceEquals(request, message))
                        {
                            request.Close();
                        }
                    }
                }

                private void AddMimeVersion(string version)
                {
                    _httpRequestMessage.Headers.Add(HttpChannelUtilities.MIMEVersionHeader, version);
                }

                private void Cleanup()
                {
                    s_cancelCts(_httpSendCts);

                    if (_httpRequestMessage != null)
                    {
                        var httpRequestMessageSnapshot = _httpRequestMessage;
                        _httpRequestMessage = null;
                        TryCompleteHttpRequest(httpRequestMessageSnapshot);
                        httpRequestMessageSnapshot.Dispose();
                    }
                }

                public void Abort(RequestChannel channel)
                {
                    Cleanup();
                    _abortReason = HttpAbortReason.Aborted;
                }

                public void Fault(RequestChannel channel)
                {
                    Cleanup();
                }

                public async Task<Message> ReceiveReplyAsync(TimeoutHelper timeoutHelper)
                {
                    try
                    {
                        _timeoutHelper = timeoutHelper;
                        var responseHelper = new HttpResponseMessageHelper(_httpResponseMessage, _factory);
                        var replyMessage = await responseHelper.ParseIncomingResponse(timeoutHelper);
                        TryCompleteHttpRequest(_httpRequestMessage);
                        return replyMessage;
                    }
                    catch (OperationCanceledException)
                    {
                        var cancelToken = _timeoutHelper.GetCancellationToken();
                        if (cancelToken.IsCancellationRequested)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(SR.Format(
                                SR.HttpResponseTimedOut, _httpRequestMessage.RequestUri, timeoutHelper.OriginalTimeout)));
                        }
                        else
                        {
                            // Cancellation came from somewhere other than timeoutCts and needs to be handled differently.
                            throw;
                        }
                    }
                }

                private bool PrepareMessageHeaders(Message message)
                {
                    string action = message.Headers.Action;

                    if (action != null)
                    {
                        action = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", UrlUtility.UrlPathEncode(action));
                    }

                    bool suppressEntityBody = message is NullMessage;

                    object property;
                    if (message.Properties.TryGetValue(HttpRequestMessageProperty.Name, out property))
                    {
                        HttpRequestMessageProperty requestProperty = (HttpRequestMessageProperty)property;
                        _httpRequestMessage.Method = new HttpMethod(requestProperty.Method);
                        // Query string was applied in HttpChannelFactory.ApplyManualAddressing
                        WebHeaderCollection requestHeaders = requestProperty.Headers;
                        suppressEntityBody = suppressEntityBody || requestProperty.SuppressEntityBody;
                        var headerKeys = requestHeaders.AllKeys;
                        for (int i = 0; i < headerKeys.Length; i++)
                        {
                            string name = headerKeys[i];
                            string value = requestHeaders[name];
                            if (string.Compare(name, "accept", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                _httpRequestMessage.Headers.Accept.TryParseAdd(value);
                            }
                            else if (string.Compare(name, "connection", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                if (value.IndexOf("keep-alive", StringComparison.OrdinalIgnoreCase) != -1)
                                {
                                    _httpRequestMessage.Headers.ConnectionClose = false;
                                }
                                else
                                {
                                    _httpRequestMessage.Headers.Connection.TryParseAdd(value);
                                }
                            }
                            else if (string.Compare(name, "SOAPAction", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                if (action == null)
                                {
                                    action = value;
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(value) && string.Compare(value, action, StringComparison.Ordinal) != 0)
                                    {
                                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                                            new ProtocolException(SR.Format(SR.HttpSoapActionMismatch, action, value)));
                                    }
                                }
                            }
                            else if (string.Compare(name, "content-length", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                // this will be taken care of by System.Net when we write to the content
                            }
                            else if (string.Compare(name, "content-type", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                // Handled by MessageContent
                            }
                            else if (string.Compare(name, "expect", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                if (value.ToUpperInvariant().IndexOf("100-CONTINUE", StringComparison.OrdinalIgnoreCase) != -1)
                                {
                                    _httpRequestMessage.Headers.ExpectContinue = true;
                                }
                                else
                                {
                                    _httpRequestMessage.Headers.Expect.TryParseAdd(value);
                                }
                            }
                            else if (string.Compare(name, "referer", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                // referrer is proper spelling, but referer is the what is in the protocol.

                                _httpRequestMessage.Headers.Referrer = new Uri(value);
                            }
                            else if (string.Compare(name, "transfer-encoding", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                if (value.ToUpperInvariant().IndexOf("CHUNKED", StringComparison.OrdinalIgnoreCase) != -1)
                                {
                                    _httpRequestMessage.Headers.TransferEncodingChunked = true;
                                }
                                else
                                {
                                    _httpRequestMessage.Headers.TransferEncoding.TryParseAdd(value);
                                }
                            }
                            else if (string.Compare(name, "user-agent", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                _httpRequestMessage.Headers.Add(name, value);
                            }
                            else if (string.Compare(name, "if-modified-since", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                DateTimeOffset modifiedSinceDate;
                                if (DateTimeOffset.TryParse(value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out modifiedSinceDate))
                                {
                                    _httpRequestMessage.Headers.IfModifiedSince = modifiedSinceDate;
                                }
                                else
                                {
                                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                                        new ProtocolException(SR.Format(SR.HttpIfModifiedSinceParseError, value)));
                                }
                            }
                            else if (string.Compare(name, "date", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                // this will be taken care of by System.Net when we make the request
                            }
                            else if (string.Compare(name, "proxy-connection", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                throw ExceptionHelper.PlatformNotSupported("proxy-connection");
                            }
                            else if (string.Compare(name, "range", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                // specifying a range doesn't make sense in the context of WCF
                            }
                            else
                            {
                                try
                                {
                                    _httpRequestMessage.Headers.Add(name, value);
                                }
                                catch (Exception addHeaderException)
                                {
                                    throw FxTrace.Exception.AsError(new InvalidOperationException(SR.Format(
                                                    SR.CopyHttpHeaderFailed,
                                                    name,
                                                    value,
                                                    HttpChannelUtilities.HttpRequestHeadersTypeName),
                                                    addHeaderException));
                                }
                            }
                        }
                    }

                    if (action != null)
                    {
                        if (message.Version.Envelope == EnvelopeVersion.Soap11)
                        {
                            _httpRequestMessage.Headers.TryAddWithoutValidation("SOAPAction", action);
                        }
                        else if (message.Version.Envelope == EnvelopeVersion.Soap12)
                        {
                            // Handled by MessageContent
                        }
                        else if (message.Version.Envelope != EnvelopeVersion.None)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                                new ProtocolException(SR.Format(SR.EnvelopeVersionUnknown,
                                message.Version.Envelope.ToString())));
                        }
                    }

                    // since we don't get the output stream in send when retVal == true,
                    // we need to disable chunking for some verbs (DELETE/PUT)
                    if (suppressEntityBody)
                    {
                        _httpRequestMessage.Headers.TransferEncodingChunked = false;
                    }

                    return suppressEntityBody;
                }

                public void OnReleaseRequest()
                {
                    TryCompleteHttpRequest(_httpRequestMessage);
                }

                private void TryCompleteHttpRequest(HttpRequestMessage request)
                {
                    if (request == null)
                    {
                        return;
                    }

                    if (Interlocked.CompareExchange(ref _httpRequestCompleted, 1, 0) == 0)
                    {
                        _channel.OnHttpRequestCompleted(request);
                    }
                }

                private async Task SendPreauthenticationHeadRequestIfNeeded()
                {
                    if (!_factory.AuthenticationSchemeMayRequireResend())
                    {
                        return;
                    }

                    var requestUri = _httpRequestMessage.RequestUri;
                    // sends a HEAD request to the specificed requestUri for authentication purposes
                    Contract.Assert(requestUri != null);

                    HttpRequestMessage headHttpRequestMessage = new HttpRequestMessage()
                    {
                        Method = HttpMethod.Head,
                        RequestUri = requestUri
                    };

                    var cancelToken = await _timeoutHelper.GetCancellationTokenAsync();
                    await _httpClient.SendAsync(headHttpRequestMessage, cancelToken);
                }
            }
        }

        internal class WebProxyFactory
        {
            private Uri _address;
            private bool _bypassOnLocal;

            public WebProxyFactory(Uri address, bool bypassOnLocal, AuthenticationSchemes authenticationScheme)
            {
                _address = address;
                _bypassOnLocal = bypassOnLocal;

                if (!authenticationScheme.IsSingleton() && authenticationScheme != AuthenticationSchemes.IntegratedWindowsAuthentication)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(nameof(authenticationScheme), SR.Format(SR.HttpRequiresSingleAuthScheme,
                        authenticationScheme));
                }

                AuthenticationScheme = authenticationScheme;
            }

            internal AuthenticationSchemes AuthenticationScheme { get; }

            public async Task<IWebProxy> CreateWebProxyAsync(AuthenticationLevel requestAuthenticationLevel, TokenImpersonationLevel requestImpersonationLevel, SecurityTokenProviderContainer tokenProvider, TimeSpan timeout)
            {
                WebProxy result = new WebProxy(_address, _bypassOnLocal);

                if (AuthenticationScheme != AuthenticationSchemes.Anonymous)
                {
                    (NetworkCredential credential, TokenImpersonationLevel impersonationLevel, AuthenticationLevel authenticationLevel) = await HttpChannelUtilities.GetCredentialAsync(AuthenticationScheme,
                        tokenProvider, timeout);

                    // The impersonation level for target auth is also used for proxy auth (by System.Net).  Therefore,
                    // fail if the level stipulated for proxy auth is more restrictive than that for target auth.
                    if (!TokenImpersonationLevelHelper.IsGreaterOrEqual(impersonationLevel, requestImpersonationLevel))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(
                            SR.ProxyImpersonationLevelMismatch, impersonationLevel, requestImpersonationLevel)));
                    }

                    // The authentication level for target auth is also used for proxy auth (by System.Net).
                    // Therefore, fail if proxy auth requires mutual authentication but target auth does not.
                    if ((authenticationLevel == AuthenticationLevel.MutualAuthRequired) &&
                        (requestAuthenticationLevel != AuthenticationLevel.MutualAuthRequired))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(
                            SR.ProxyAuthenticationLevelMismatch, authenticationLevel, requestAuthenticationLevel)));
                    }

                    CredentialCache credentials = new CredentialCache();
                    if (AuthenticationScheme == AuthenticationSchemes.IntegratedWindowsAuthentication)
                    {
                        credentials.Add(_address, AuthenticationSchemesHelper.ToString(AuthenticationSchemes.Negotiate),
                            credential);
                        credentials.Add(_address, AuthenticationSchemesHelper.ToString(AuthenticationSchemes.Ntlm),
                            credential);
                    }
                    else
                    {
                        credentials.Add(_address, AuthenticationSchemesHelper.ToString(AuthenticationScheme),
                            credential);
                    }
                    result.Credentials = credentials;
                }

                return result;
            }
        }
    }
}
