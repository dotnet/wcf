// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Contracts;
using System.Globalization;
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
                return (T)(object)GetHttpCookieContainerManager();
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
                                if (credentials.UserName.UserName == null)
                                {
                                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("userName");
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

            if (MessageVersion.Addressing == AddressingVersion.None && remoteAddress.Uri != via)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateToMustEqualViaException(remoteAddress.Uri, via));
            }
        }

        protected override TChannel OnCreateChannel(EndpointAddress remoteAddress, Uri via)
        {
            return OnCreateChannelCore(remoteAddress, via);
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
            OnClose(timeout);
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

        internal HttpRequestMessage GetHttpRequestMessage(EndpointAddress to, Uri via)
        {
            Uri httpRequestUri = via;

            HttpRequestMessage requestMessage = null;

            requestMessage = new HttpRequestMessage(HttpMethod.Post, httpRequestUri);
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
                    if (State == CommunicationState.Created)
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
                Factory.MapIdentity(RemoteAddress);
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

            internal Task<HttpRequestMessage> GetHttpRequestMessageAsync(EndpointAddress to, Uri via, TimeoutHelper timeoutHelper)
            {
                // This method replaces the method with the following prototyp:
                //      protected HttpWebRequest GetWebRequest(EndpointAddress to, Uri via, SecurityTokenContainer clientCertificateToken, ref TimeoutHelper timeoutHelper)
                // SecurityTokenContainer doesn't exist so was excluded from method signature
                return Task.FromResult(Factory.GetHttpRequestMessage(to, via));
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
                private HttpResponseMessage _httpResponseMessage;
                private HttpAbortReason _abortReason;
                private TimeoutHelper _timeoutHelper;
                private int _httpRequestCompleted;
                private HttpClient _httpClient;

                public HttpClientChannelAsyncRequest(HttpClientRequestChannel channel)
                {
                    _channel = channel;
                    _to = channel.RemoteAddress;
                    _via = channel.Via;
                    _factory = channel.Factory;
                    _httpClient = _factory.GetHttpClient();
                }

                public async Task SendRequestAsync(Message message, TimeoutHelper timeoutHelper)
                {
                    _timeoutHelper = timeoutHelper;
                    _factory.ApplyManualAddressing(ref _to, ref _via, message);
                    _httpRequestMessage = await _channel.GetHttpRequestMessageAsync(_to, _via, _timeoutHelper);

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
                            _httpRequestMessage.Content = MessageContent.Create(_factory, request, _timeoutHelper);
                        }

                        try
                        {
                            // There is a possibility that a HEAD pre-auth request might fail when the actual request
                            // will succeed. For example, when the web service refuses HEAD requests. We don't want
                            // to fail the actual request because of some subtlety which causes the HEAD request.
                            await SendPreauthenticationHeadRequestIfNeeded();
                        }
                        catch { /* ignored */ }

                        bool success = false;

                        try
                        {
                            _httpResponseMessage = await _httpClient.SendAsync(_httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, _timeoutHelper.CancellationToken);
                            // As we have the response message and no exceptions have been thrown, the request message has completed it's job.
                            // Calling Dispose() on the request message to free up resources in HttpContent, but keeping the object around
                            // as we can still query properties once dispose'd.
                            _httpRequestMessage.Dispose();
                            success = true;
                        }
                        catch (HttpRequestException requestException)
                        {
                            HttpChannelUtilities.ProcessGetResponseWebException(requestException, _httpRequestMessage,
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

                private void Cleanup()
                {
                    if (_httpRequestMessage != null)
                    {
                        var httpRequestMessageSnapshot = _httpRequestMessage;
                        _httpRequestMessage = null;
                        _timeoutHelper.CancelCancellationToken(false);
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
                        var replyMessage = await responseHelper.ParseIncomingResponse();
                        TryCompleteHttpRequest(_httpRequestMessage);
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

                private bool PrepareMessageHeaders(Message message)
                {
                    string action = message.Headers.Action;

                    if (action != null)
                    {
                        action = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", UrlUtility.UrlPathEncode(action));
                    }

                    if (message.Version.Addressing == AddressingVersion.None)
                    {
                        message.Headers.Action = null;
                        message.Headers.To = null;
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
                            else if (string.Compare(name, "host", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                // this should be controlled through Via
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
                                _httpRequestMessage.Headers.UserAgent.Add(ProductInfoHeaderValue.Parse(value));
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
                    if (!AuthenticationSchemeMayRequireResend())
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

                    await _httpClient.SendAsync(headHttpRequestMessage, _timeoutHelper.CancellationToken);
                }

                private bool AuthenticationSchemeMayRequireResend()
                {
                    return _factory.AuthenticationScheme != AuthenticationSchemes.Anonymous;
                }
            }
        }
    }
}
