// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class HttpChannelFactory<TChannel>
        : TransportChannelFactory<TChannel>,
        IHttpTransportFactorySettings
    {
        private static CacheControlHeaderValue s_requestCacheHeader = new CacheControlHeaderValue { NoCache = true, MaxAge = new TimeSpan(0) };
        private bool _allowCookies;
        private AuthenticationSchemes _authenticationScheme;
        private HttpCookieContainerManager _httpCookieContainerManager;
        private HttpClient _httpClient;
        private SecurityCredentialsManager _channelCredentials;
        private ISecurityCapabilities _securityCapabilities;
        private int _maxBufferSize;
        private TransferMode _transferMode;
        private bool _useDefaultWebProxy;
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

            _allowCookies = bindingElement.AllowCookies;

            if (_allowCookies)
            {
                _httpCookieContainerManager = new HttpCookieContainerManager();
            }

            if (!bindingElement.AuthenticationScheme.IsSingleton())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", SR.Format(SR.HttpRequiresSingleAuthScheme,
                    bindingElement.AuthenticationScheme));
            }
            _authenticationScheme = bindingElement.AuthenticationScheme;
            _maxBufferSize = bindingElement.MaxBufferSize;
            _transferMode = bindingElement.TransferMode;
            _useDefaultWebProxy = bindingElement.UseDefaultWebProxy;

            _channelCredentials = context.BindingParameters.Find<SecurityCredentialsManager>();
            _securityCapabilities = bindingElement.GetProperty<ISecurityCapabilities>(context);
        }

        public bool AllowCookies
        {
            get
            {
                return _allowCookies;
            }
        }

        public AuthenticationSchemes AuthenticationScheme
        {
            get
            {
                return _authenticationScheme;
            }
        }

        public virtual bool IsChannelBindingSupportEnabled
        {
            get
            {
                return false;
            }
        }

        public int MaxBufferSize
        {
            get
            {
                return _maxBufferSize;
            }
        }

        public TransferMode TransferMode
        {
            get
            {
                return _transferMode;
            }
        }

        public override string Scheme
        {
            get
            {
                return UriEx.UriSchemeHttp;
            }
        }


        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T)(object)_securityCapabilities;
            }
            if (typeof(T) == typeof(IHttpCookieContainerManager))
            {
                return (T)(object)this.GetHttpCookieContainerManager();
            }

            return base.GetProperty<T>();
        }

        private HttpCookieContainerManager GetHttpCookieContainerManager()
        {
            return _httpCookieContainerManager;
        }

        internal HttpClient GetHttpClient()
        {
            if (_httpClient == null)
            {
                var clientHandler = new HttpClientHandler();
                clientHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                clientHandler.UseCookies = _allowCookies;
                clientHandler.PreAuthenticate = true;
                if (clientHandler.SupportsProxy)
                {
                    clientHandler.UseProxy = _useDefaultWebProxy;
                }

                ICredentials creds = null;
                clientHandler.UseDefaultCredentials = false;
                if (_authenticationScheme != AuthenticationSchemes.Anonymous)
                {
                    creds = CredentialCache.DefaultCredentials;
                    ClientCredentials credentials = _channelCredentials as ClientCredentials;
                    if (credentials != null)
                    {
                        switch (_authenticationScheme)
                        {
                            case AuthenticationSchemes.Basic:
                                if (credentials.UserName.UserName != string.Empty)
                                {
                                    creds = new NetworkCredential(credentials.UserName.UserName, credentials.UserName.Password);
                                }
                                break;
                            case AuthenticationSchemes.Digest:
                                if (credentials.HttpDigest.ClientCredential.UserName != string.Empty)
                                {
                                    creds = credentials.HttpDigest.ClientCredential;
                                }
                                break;
                            case AuthenticationSchemes.Ntlm:
                                goto case AuthenticationSchemes.Negotiate;
                            case AuthenticationSchemes.Negotiate:
                                if (credentials.Windows.ClientCredential.UserName != string.Empty)
                                {
                                    creds = credentials.Windows.ClientCredential;
                                }
                                break;
                        }
                    }
                }
                if (creds == CredentialCache.DefaultCredentials)
                {
                    clientHandler.UseDefaultCredentials = true;
                }
                else
                {
                    clientHandler.Credentials = creds;
                }

                var client = new HttpClient(clientHandler);
                _httpClient = client;
            }
            return _httpClient;
        }


        internal Exception CreateToMustEqualViaException(Uri to, Uri via)
        {
            return new ArgumentException(SR.Format(SR.HttpToMustEqualVia, to, via));
        }


        public override int GetMaxBufferSize()
        {
            return MaxBufferSize;
        }

        protected virtual void ValidateCreateChannelParameters(EndpointAddress remoteAddress, Uri via)
        {
            base.ValidateScheme(via);

            if (this.MessageVersion.Addressing == AddressingVersion.None && remoteAddress.Uri != via)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateToMustEqualViaException(remoteAddress.Uri, via));
            }
        }

        protected override TChannel OnCreateChannel(EndpointAddress remoteAddress, Uri via)
        {
            return this.OnCreateChannelCore(remoteAddress, via);
        }

        protected virtual TChannel OnCreateChannelCore(EndpointAddress remoteAddress, Uri via)
        {
            ValidateCreateChannelParameters(remoteAddress, via);
            return (TChannel)(object)new HttpClientRequestChannel((HttpChannelFactory<IRequestChannel>)(object)this, remoteAddress, via, ManualAddressing);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InitializeSecurityTokenManager()
        {
            if (_channelCredentials == null)
            {
                _channelCredentials = ClientCredentials.CreateDefaultCredentials();
            }
        }

        protected virtual bool IsSecurityTokenManagerRequired()
        {
            return _authenticationScheme != AuthenticationSchemes.Anonymous;
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
                this.InitializeSecurityTokenManager();
            }

            if (this.AllowCookies &&
                !_httpCookieContainerManager.IsInitialized) // We don't want to overwrite the CookieContainer if someone has set it already.
            {
                _httpCookieContainerManager.CookieContainer = new CookieContainer();
            }
        }

        internal protected override Task OnOpenAsync(TimeSpan timeout)
        {
            this.OnOpen(timeout);
            return TaskHelpers.CompletedTask();
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            this.OnClose(timeout);
            return TaskHelpers.CompletedTask();
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            var httpClientToDispose = _httpClient;
            if (httpClientToDispose != null)
            {
                _httpClient = null;
                httpClientToDispose.Dispose();
            }
        }

        internal static bool IsWindowsAuth(AuthenticationSchemes authScheme)
        {
            Contract.Assert(authScheme.IsSingleton(), "authenticationScheme used in an Http(s)ChannelFactory must be a singleton value.");

            return authScheme == AuthenticationSchemes.Negotiate ||
                authScheme == AuthenticationSchemes.Ntlm;
        }

        internal HttpRequestMessage GetHttpRequestMessage(EndpointAddress to, Uri via, CancellationToken cancelToken, bool isWebSocketRequest)
        {
            Uri httpWebRequestUri = via;

            HttpRequestMessage requestMessage = null;

            if (!isWebSocketRequest)
            {
                requestMessage = new HttpRequestMessage(HttpMethod.Post, httpWebRequestUri);
                if (TransferModeHelper.IsRequestStreamed(TransferMode))
                {
                    requestMessage.Headers.TransferEncodingChunked = true;
                }
            }
            else
            {
                requestMessage = new HttpRequestMessage(HttpMethod.Get, httpWebRequestUri);
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

                if (this.MessageVersion.Addressing == AddressingVersion.None)
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
            return MapIdentity(target, this.AuthenticationScheme);
        }

        protected class HttpClientRequestChannel : RequestChannel
        {
            // Double-checked locking pattern requires volatile for read/write synchronization
            private HttpChannelFactory<IRequestChannel> _factory;

            private ChannelParameterCollection _channelParameters;

            public HttpClientRequestChannel(HttpChannelFactory<IRequestChannel> factory, EndpointAddress to, Uri via, bool manualAddressing)
                : base(factory, to, via, manualAddressing)
            {
                _factory = factory;
            }

            public HttpChannelFactory<IRequestChannel> Factory
            {
                get { return _factory; }
            }



            protected ChannelParameterCollection ChannelParameters
            {
                get
                {
                    return _channelParameters;
                }
            }

            public override T GetProperty<T>()
            {
                if (typeof(T) == typeof(ChannelParameterCollection))
                {
                    if (this.State == CommunicationState.Created)
                    {
                        lock (ThisLock)
                        {
                            if (_channelParameters == null)
                            {
                                _channelParameters = new ChannelParameterCollection();
                            }
                        }
                    }
                    return (T)(object)_channelParameters;
                }
                else
                {
                    return base.GetProperty<T>();
                }
            }

            private void PrepareOpen()
            {
                if (Factory.MapIdentity(RemoteAddress))
                {
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
                return TaskHelpers.CompletedTask();
            }

            private void PrepareClose(bool aborting)
            {
            }

            protected override void OnAbort()
            {
                PrepareClose(true);
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
                await base.WaitForPendingRequestsAsync(timeoutHelper.RemainingTime());
            }

            protected override IAsyncRequest CreateAsyncRequest(Message message)
            {
                return new HttpClientChannelAsyncRequest(this);
            }

            public virtual Task<HttpRequestMessage> GetHttpRequestMessageAsync(EndpointAddress to, Uri via, CancellationToken cancelToken)
            {
                // This method replaces the method with the following prototyp:
                //      protected HttpWebRequest GetWebRequest(EndpointAddress to, Uri via, SecurityTokenContainer clientCertificateToken, ref TimeoutHelper timeoutHelper)
                // SecurityTokenContainer doesn't exist so was excluded from method signature
                return Task.FromResult(this.Factory.GetHttpRequestMessage(to, via, cancelToken, false));
            }


            public virtual bool WillGetWebRequestCompleteSynchronously()
            {
                return true;
            }

            internal virtual void OnHttpRequestCompleted(HttpRequestMessage request)
            {
                // empty
            }

            internal class HttpClientChannelAsyncRequest : IAsyncRequest
            {
                private HttpClientRequestChannel _channel;
                private HttpChannelFactory<IRequestChannel> _factory;
                private EndpointAddress _to;
                private Uri _via;
                private HttpRequestMessage _httpRequestMessage;
                private Task<HttpResponseMessage> _httpResponseMessageTask;
                private HttpAbortReason _abortReason;
                private TimeoutHelper _timeoutHelper;
                private int _httpRequestCompleted;
                public HttpClientChannelAsyncRequest(HttpClientRequestChannel channel)
                {
                    _channel = channel;
                    _to = channel.RemoteAddress;
                    _via = channel.Via;
                    _factory = channel.Factory;
                }

                public async Task SendRequestAsync(Message message, TimeoutHelper timeoutHelper)
                {
                    _timeoutHelper = timeoutHelper;
                    _factory.ApplyManualAddressing(ref _to, ref _via, message);
                    _httpRequestMessage = await _channel.GetHttpRequestMessageAsync(_to, _via, _timeoutHelper.CancellationToken);

                    Message request = message;

                    try
                    {
                        if (_channel.State != CommunicationState.Opened)
                        {
                            // if we were aborted while getting our request or doing correlation, 
                            // we need to abort the web request and bail
                            Cleanup();
                            _channel.ThrowIfDisposedOrNotOpen();
                        }

                        HttpOutput httpOutput = HttpOutput.CreateHttpOutput(_httpRequestMessage, _factory, request, _factory.IsChannelBindingSupportEnabled, _factory.GetHttpClient(), _factory._authenticationScheme, _timeoutHelper);

                        bool success = false;
                        try
                        {
                            _httpResponseMessageTask = await httpOutput.SendAsync(_httpRequestMessage);
                            //this.channelBinding = httpOutput.TakeChannelBinding();
                            await httpOutput.CloseAsync();
                            success = true;
                        }
                        finally
                        {
                            if (!success)
                            {
                                httpOutput.Abort(HttpAbortReason.Aborted);
                            }
                        }
                    }
                    finally
                    {
                        if (!object.ReferenceEquals(request, message))
                        {
                            request.Close();
                        }
                    }
                }

                private void Cleanup()
                {
                    if (_httpRequestMessage != null)
                    {
                        _timeoutHelper.CancelCancellationToken(false);
                        this.TryCompleteHttpRequest(_httpRequestMessage);
                    }
                    //ChannelBindingUtility.Dispose(ref this.channelBinding);
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

                [System.Diagnostics.CodeAnalysis.SuppressMessage(FxCop.Category.ReliabilityBasic, "Reliability104",
                            Justification = "This is an old method from previous release.")]
                public async Task<Message> ReceiveReplyAsync(TimeoutHelper timeoutHelper)
                {
                    _timeoutHelper = timeoutHelper;
                    HttpResponseMessage httpResponse = null;
                    HttpRequestException responseException = null;
                    try
                    {
                        httpResponse = await _httpResponseMessageTask;
                    }
                    catch (HttpRequestException requestException)
                    {
                        responseException = requestException;
                        httpResponse = HttpChannelUtilities.ProcessGetResponseWebException(responseException, _httpRequestMessage,
                            _abortReason);
                    }
                    catch (OperationCanceledException)
                    {
                        if (_timeoutHelper.CancellationToken.IsCancellationRequested)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(SR.Format(
                                SR.HttpRequestTimedOut, _httpRequestMessage.RequestUri, _timeoutHelper.OriginalTimeout)));
                        }
                        else
                        {
                            // Cancellation came from somewhere other than timeoutCts and needs to be handled differently.
                            throw;
                        }
                    }

                    try
                    {
                        HttpInput httpInput = HttpChannelUtilities.ValidateRequestReplyResponse(_httpRequestMessage, httpResponse,
                            _factory, responseException);

                        Message replyMessage = null;
                        if (httpInput != null)
                        {
                            var outException = new OutWrapper<Exception>();
                            replyMessage = await httpInput.ParseIncomingMessageAsync(outException);
                            Exception exception = outException;
                            Contract.Assert(exception == null, "ParseIncomingMessage should not set an exception after parsing a response message.");
                        }

                        this.TryCompleteHttpRequest(_httpRequestMessage);
                        return replyMessage;
                    }
                    catch (OperationCanceledException)
                    {
                        if (_timeoutHelper.CancellationToken.IsCancellationRequested)
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

                public void OnReleaseRequest()
                {
                    this.TryCompleteHttpRequest(_httpRequestMessage);
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
            }
        }
    }
}
