// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Diagnostics.Application;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Channels
{
    // abstract out the common functionality of an "HttpInput"
    internal abstract class HttpInput
    {
        private const string multipartRelatedMediaType = "multipart/related";
        private const string startInfoHeaderParam = "start-info";
        private const string defaultContentType = "application/octet-stream";

        private BufferManager _bufferManager;
        private bool _isRequest;
        private MessageEncoder _messageEncoder;
        private IHttpTransportFactorySettings _settings;
        private bool _streamed;
        private HttpRequestException _httpRequestException;
        private Stream _inputStream;
        private bool _enableChannelBinding;
        private bool _errorGettingInputStream;

        protected HttpInput(IHttpTransportFactorySettings settings, bool isRequest, bool enableChannelBinding)
        {
            _settings = settings;
            _bufferManager = settings.BufferManager;
            _messageEncoder = settings.MessageEncoderFactory.Encoder;
            _httpRequestException = null;
            _isRequest = isRequest;
            _inputStream = null;
            _enableChannelBinding = enableChannelBinding;

            if (isRequest)
            {
                _streamed = TransferModeHelper.IsRequestStreamed(settings.TransferMode);
            }
            else
            {
                _streamed = TransferModeHelper.IsResponseStreamed(settings.TransferMode);
            }
        }

        internal static HttpInput CreateHttpInput(HttpResponseMessage httpResponse, IHttpTransportFactorySettings settings)
        {
            return new HttpResponseMessageHttpInput(httpResponse, settings);
        }

        internal HttpRequestException HttpRequestException
        {
            get { return _httpRequestException; }
            set { _httpRequestException = value; }
        }

        // Note: This method will return null in the case where throwOnError is false, and a non-fatal error occurs.
        // Please exercice caution when passing in throwOnError = false.  This should basically only be done in error
        // code paths, or code paths where there is very good reason that you would not want this method to throw.
        // When passing in throwOnError = false, please handle the case where this method returns null.
        public Stream GetInputStream(bool throwOnError)
        {
            if (_inputStream == null && (throwOnError || !_errorGettingInputStream))
            {
                try
                {
                    _inputStream = GetInputStream();
                    _errorGettingInputStream = false;
                }
                catch (Exception e)
                {
                    _errorGettingInputStream = true;
                    if (throwOnError || Fx.IsFatal(e))
                    {
                        throw;
                    }
                }
            }

            return _inputStream;
        }

        // -1 if chunked
        public abstract long ContentLength { get; }
        protected abstract string ContentTypeCore { get; }
        protected abstract bool HasContent { get; }
        protected abstract string SoapActionHeader { get; }
        protected abstract Stream GetInputStream();

        protected string ContentType
        {
            get
            {
                string contentType = ContentTypeCore;

                if (string.IsNullOrEmpty(contentType))
                {
                    return defaultContentType;
                }

                return contentType;
            }
        }

        private void ThrowMaxReceivedMessageSizeExceeded()
        {
            if (TD.MaxReceivedMessageSizeExceededIsEnabled())
            {
                TD.MaxReceivedMessageSizeExceeded(SR.Format(SR.MaxReceivedMessageSizeExceeded, _settings.MaxReceivedMessageSize));
            }

            if (_isRequest)
            {
                ThrowHttpProtocolException(SR.Format(SR.MaxReceivedMessageSizeExceeded, _settings.MaxReceivedMessageSize), HttpStatusCode.RequestEntityTooLarge);
            }
            else
            {
                string message = SR.Format(SR.MaxReceivedMessageSizeExceeded, _settings.MaxReceivedMessageSize);
                Exception inner = new QuotaExceededException(message);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(message, inner));
            }
        }

        private async Task<Message> DecodeBufferedMessageAsync(ArraySegment<byte> buffer, Stream inputStream)
        {
            try
            {
                // if we're chunked, make sure we've consumed the whole body
                if (ContentLength == -1 && buffer.Count == _settings.MaxReceivedMessageSize)
                {
                    byte[] extraBuffer = new byte[1];
                    int extraReceived = await inputStream.ReadAsync(extraBuffer, 0, 1);
                    if (extraReceived > 0)
                    {
                        ThrowMaxReceivedMessageSizeExceeded();
                    }
                }

                try
                {
                    return _messageEncoder.ReadMessage(buffer, _bufferManager, ContentType);
                }
                catch (XmlException xmlException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new ProtocolException(SR.MessageXmlProtocolError, xmlException));
                }
            }
            finally
            {
                inputStream.Dispose();
            }
        }

        private async Task<Message> ReadBufferedMessageAsync(Stream inputStream)
        {
            ArraySegment<byte> messageBuffer = GetMessageBuffer();
            byte[] buffer = messageBuffer.Array;
            int offset = 0;
            int count = messageBuffer.Count;

            while (count > 0)
            {
                int bytesRead = await inputStream.ReadAsync(buffer, offset, count);
                if (bytesRead == 0) // EOF 
                {
                    if (ContentLength != -1)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new ProtocolException(SR.HttpContentLengthIncorrect));
                    }

                    break;
                }
                count -= bytesRead;
                offset += bytesRead;
            }

            return await DecodeBufferedMessageAsync(new ArraySegment<byte>(buffer, 0, offset), inputStream);
        }

        private Message ReadChunkedBufferedMessage(Stream inputStream)
        {
            try
            {
                return _messageEncoder.ReadMessage(inputStream, _bufferManager, _settings.MaxBufferSize, ContentType);
            }
            catch (XmlException xmlException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ProtocolException(SR.MessageXmlProtocolError, xmlException));
            }
        }

        private Message ReadStreamedMessage(Stream inputStream)
        {
            MaxMessageSizeStream maxMessageSizeStream = new MaxMessageSizeStream(inputStream, _settings.MaxReceivedMessageSize);

            try
            {
                return _messageEncoder.ReadMessage(maxMessageSizeStream, _settings.MaxBufferSize, ContentType);
            }
            catch (XmlException xmlException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ProtocolException(SR.MessageXmlProtocolError, xmlException));
            }
        }

        protected abstract void AddProperties(Message message);


        // makes sure that appropriate HTTP level headers are included in the received Message
        private Exception ProcessHttpAddressing(Message message)
        {
            Exception result = null;
            AddProperties(message);

            // check if user is receiving WS-1 messages
            if (message.Version.Addressing == AddressingVersion.None)
            {
                bool actionAbsent = false;
                try
                {
                    actionAbsent = (message.Headers.Action == null);
                }
                catch (XmlException)
                {
                }
                catch (CommunicationException)
                {
                }
                if (!actionAbsent)
                {
                    result = new ProtocolException(SR.Format(SR.HttpAddressingNoneHeaderOnWire,
                        XD.AddressingDictionary.Action.Value));
                }

                bool toAbsent = false;
                try
                {
                    toAbsent = (message.Headers.To == null);
                }
                catch (XmlException)
                {
                }
                catch (CommunicationException)
                {
                }

                if (!toAbsent)
                {
                    result = new ProtocolException(SR.Format(SR.HttpAddressingNoneHeaderOnWire,
                        XD.AddressingDictionary.To.Value));
                }
                message.Headers.To = message.Properties.Via;
            }

            if (_isRequest)
            {
                string action = null;

                if (message.Version.Envelope == EnvelopeVersion.Soap11)
                {
                    action = SoapActionHeader;
                }
                else if (message.Version.Envelope == EnvelopeVersion.Soap12 && !String.IsNullOrEmpty(ContentType))
                {
                    MediaTypeHeaderValue parsedContentType = MediaTypeHeaderValue.Parse(ContentType);
                    foreach (NameValueHeaderValue actionParam in parsedContentType.Parameters)
                    {
                        if (actionParam.Name == "action")
                        {
                            action = actionParam.Value;
                            break;
                        }
                    }
                }

                if (action != null)
                {
                    action = UrlUtility.UrlDecode(action, Encoding.UTF8);

                    if (action.Length >= 2 && action[0] == '"' && action[action.Length - 1] == '"')
                    {
                        action = action.Substring(1, action.Length - 2);
                    }

                    if (message.Version.Addressing == AddressingVersion.None)
                    {
                        message.Headers.Action = action;
                    }

                    try
                    {
                        if (action.Length > 0 && string.Compare(message.Headers.Action, action, StringComparison.Ordinal) != 0)
                        {
                            result = new ActionMismatchAddressingException(SR.Format(SR.HttpSoapActionMismatchFault,
                                message.Headers.Action, action), message.Headers.Action, action);
                        }
                    }
                    catch (XmlException)
                    {
                    }
                    catch (CommunicationException)
                    {
                    }
                }
            }

            return result;
        }

        private void ValidateContentType()
        {
            if (!HasContent)
                return;

            if (string.IsNullOrEmpty(ContentType))
            {
                if (MessageLogger.ShouldLogMalformed)
                {
                    // We pass in throwOnError = false below so that the exception which is eventually thrown is the ProtocolException below, with Http status code 415 "UnsupportedMediaType"
                    Stream stream = this.GetInputStream(false);
                    if (stream != null)
                    {
                        MessageLogger.LogMessage(stream, MessageLoggingSource.Malformed);
                    }
                }
                ThrowHttpProtocolException(SR.HttpContentTypeHeaderRequired, HttpStatusCode.UnsupportedMediaType, HttpChannelUtilities.StatusDescriptionStrings.HttpContentTypeMissing);
            }
            if (!_messageEncoder.IsContentTypeSupported(ContentType))
            {
                if (MessageLogger.ShouldLogMalformed)
                {
                    // We pass in throwOnError = false below so that the exception which is eventually thrown is the ProtocolException below, with Http status code 415 "UnsupportedMediaType"
                    Stream stream = this.GetInputStream(false);
                    if (stream != null)
                    {
                        MessageLogger.LogMessage(stream, MessageLoggingSource.Malformed);
                    }
                }
                string statusDescription = string.Format(CultureInfo.InvariantCulture, HttpChannelUtilities.StatusDescriptionStrings.HttpContentTypeMismatch, ContentType, _messageEncoder.ContentType);
                ThrowHttpProtocolException(SR.Format(SR.ContentTypeMismatch, ContentType, _messageEncoder.ContentType), HttpStatusCode.UnsupportedMediaType, statusDescription);
            }
        }

        public Task<Message> ParseIncomingMessageAsync(OutWrapper<Exception> requestException)
        {
            return this.ParseIncomingMessageAsync(null, requestException);
        }

        public async Task<Message> ParseIncomingMessageAsync(HttpRequestMessage httpRequestMessage, OutWrapper<Exception> requestException)
        {
            Message message = null;
            requestException.Value = null;
            bool throwing = true;
            try
            {
                ValidateContentType();


                if (!this.HasContent)
                {
                    if (_messageEncoder.MessageVersion == MessageVersion.None)
                    {
                        message = new NullMessage();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    Stream stream = this.GetInputStream(true);
                    if (_streamed)
                    {
                        message = ReadStreamedMessage(stream);
                    }
                    else if (this.ContentLength == -1)
                    {
                        message = ReadChunkedBufferedMessage(stream);
                    }
                    else
                    {
                        if (httpRequestMessage == null)
                        {
                            message = await ReadBufferedMessageAsync(stream);
                        }
                        else
                        {
                            message = await ReadBufferedMessageAsync(httpRequestMessage);
                        }
                    }
                }

                requestException.Value = ProcessHttpAddressing(message);

                throwing = false;
                return message;
            }
            finally
            {
                if (throwing)
                {
                    Close();
                }
            }
        }

        private async Task<Message> ReadBufferedMessageAsync(HttpRequestMessage httpRequestMessage)
        {
            Contract.Assert(httpRequestMessage != null, "httpRequestMessage cannot be null.");

            Message message;
            using (HttpContent currentContent = httpRequestMessage.Content)
            {
                int length = (int)this.ContentLength;
                byte[] buffer = _bufferManager.TakeBuffer(length);
                bool success = false;
                try
                {
                    MemoryStream ms = new MemoryStream(buffer);
                    await currentContent.CopyToAsync(ms).AsyncWait<CommunicationException>();
                    httpRequestMessage.Content = new ByteArrayContent(buffer, 0, length);

                    foreach (var header in currentContent.Headers)
                    {
                        httpRequestMessage.Content.Headers.Add(header.Key, header.Value);
                    }

                    message = _messageEncoder.ReadMessage(new ArraySegment<byte>(buffer, 0, length), _bufferManager, this.ContentType);
                    success = true;
                }
                finally
                {
                    if (!success)
                    {
                        // We don't have to return it in success case since the buffer will be returned to bufferManager when the message is disposed.
                        _bufferManager.ReturnBuffer(buffer);
                    }
                }
            }
            return message;
        }

        private void ThrowHttpProtocolException(string message, HttpStatusCode statusCode)
        {
            ThrowHttpProtocolException(message, statusCode, null);
        }

        private void ThrowHttpProtocolException(string message, HttpStatusCode statusCode, string statusDescription)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateHttpProtocolException(message, statusCode, statusDescription, _httpRequestException));
        }

        internal static ProtocolException CreateHttpProtocolException(string message, HttpStatusCode statusCode, string statusDescription, Exception innerException)
        {
            ProtocolException exception = new ProtocolException(message, innerException);
            exception.Data.Add(HttpChannelUtilities.HttpStatusCodeExceptionKey, statusCode);
            if (statusDescription != null && statusDescription.Length > 0)
            {
                exception.Data.Add(HttpChannelUtilities.HttpStatusDescriptionExceptionKey, statusDescription);
            }

            return exception;
        }

        protected virtual void Close()
        {
        }

        private ArraySegment<byte> GetMessageBuffer()
        {
            long count = ContentLength;
            int bufferSize;

            if (count > _settings.MaxReceivedMessageSize)
            {
                ThrowMaxReceivedMessageSizeExceeded();
            }

            bufferSize = (int)count;

            return new ArraySegment<byte>(_bufferManager.TakeBuffer(bufferSize), 0, bufferSize);
        }

        internal class HttpResponseMessageHttpInput : HttpInput
        {
            private HttpResponseMessage _httpResponse;
            private byte[] _preReadBuffer;
            private bool _hasContent;

            public HttpResponseMessageHttpInput(HttpResponseMessage httpResponse, IHttpTransportFactorySettings settings)
                : base(settings, false, false)
            {
                _httpResponse = httpResponse;
                if (HttpChannelUtilities.GetContentLength(httpResponse) == -1)
                {
                    _preReadBuffer = new byte[1];

                    if (_httpResponse.Content.ReadAsStreamAsync().GetAwaiter().GetResult().Read(_preReadBuffer, 0, 1) == 0)
                    {
                        _preReadBuffer = null;
                    }
                }

                _hasContent = (_preReadBuffer != null || HttpChannelUtilities.GetContentLength(httpResponse) > 0);
                if (!_hasContent)
                {
                    // Close the response stream to avoid leaking the connection.
                    _httpResponse.Content.ReadAsStreamAsync().GetAwaiter().GetResult().Dispose();
                }
            }


            public override long ContentLength
            {
                get
                {
                    return HttpChannelUtilities.GetContentLength(_httpResponse);
                }
            }

            protected override string ContentTypeCore
            {
                get
                {
                    return HttpChannelUtilities.GetContentTypeString(_httpResponse);
                }
            }

            protected override bool HasContent
            {
                get { return _hasContent; }
            }

            protected override string SoapActionHeader
            {
                get
                {
                    IEnumerable<string> soapActionHeaders;
                    if (_httpResponse.Headers.TryGetValues("SOAPAction", out soapActionHeaders))
                    {
                        return String.Join(", ", soapActionHeaders);
                    }
                    return String.Empty;
                }
            }

            protected override void AddProperties(Message message)
            {
                HttpResponseMessageProperty responseProperty = new HttpResponseMessageProperty(_httpResponse);
                message.Properties.Add(HttpResponseMessageProperty.Name, responseProperty);
                message.Properties.Via = message.Version.Addressing.AnonymousUri;
            }

            protected override void Close()
            {
                try
                {
                    _httpResponse.Dispose();
                }
                catch (Exception exception)
                {
                    if (Fx.IsFatal(exception))
                        throw;
                }
            }

            protected override Stream GetInputStream()
            {
                Contract.Assert(this.HasContent, "this.HasContent must be true.");
                if (_preReadBuffer != null)
                {
                    return new HttpResponseInputStream(_httpResponse, _preReadBuffer);
                }
                else
                {
                    return new HttpResponseInputStream(_httpResponse);
                }
            }

            internal class HttpResponseInputStream : DetectEofStream
            {
                // in order to avoid exceeding kernel buffers, we throttle our reads. http.sys
                // deals with this fine, but System.Net doesn't do any such throttling.
                private const int maxSocketRead = 64 * 1024;
                private HttpResponseMessage _httpResponse;
                private bool _responseClosed;
                private bool _disposed;

                public HttpResponseInputStream(HttpResponseMessage httpResponse)
                    : base(httpResponse.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                {
                    _httpResponse = httpResponse;
                }

                public HttpResponseInputStream(HttpResponseMessage httpResponse, byte[] prereadBuffer)
                    : base(new PreReadStream(httpResponse.Content.ReadAsStreamAsync().GetAwaiter().GetResult(), prereadBuffer))
                {
                    _httpResponse = httpResponse;
                }

                protected override void OnReceivedEof()
                {
                    base.OnReceivedEof();
                    DisposeResponse();
                }

                private void DisposeResponse()
                {
                    if (_responseClosed)
                    {
                        return;
                    }

                    _responseClosed = true;
                    _httpResponse.Dispose();
                }

                public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
                {
                    try
                    {
                        return await base.ReadAsync(buffer, offset, count, cancellationToken);
                    }
                    catch (ObjectDisposedException objectDisposedException)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(objectDisposedException.Message, objectDisposedException));
                    }
                    catch (IOException ioException)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(HttpChannelUtilities.CreateResponseIOException(ioException, TimeoutHelper.FromMilliseconds(this.ReadTimeout)));
                    }
                    catch (HttpRequestException requestException)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(HttpChannelUtilities.CreateResponseHttpRequestException(requestException, _httpResponse));
                    }
                }

                public override int Read(byte[] buffer, int offset, int count)
                {
                    try
                    {
                        return BaseStream.Read(buffer, offset, Math.Min(count, maxSocketRead));
                    }
                    catch (ObjectDisposedException objectDisposedException)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(objectDisposedException.Message, objectDisposedException));
                    }
                    catch (IOException ioException)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(HttpChannelUtilities.CreateResponseIOException(ioException, TimeoutHelper.FromMilliseconds(this.ReadTimeout)));
                    }
                }

                public override int ReadByte()
                {
                    try
                    {
                        return BaseStream.ReadByte();
                    }
                    catch (ObjectDisposedException objectDisposedException)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(objectDisposedException.Message, objectDisposedException));
                    }
                    catch (IOException ioException)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(HttpChannelUtilities.CreateResponseIOException(ioException, TimeoutHelper.FromMilliseconds(this.ReadTimeout)));
                    }
                }

                protected override void Dispose(bool disposing)
                {
                    if (!_disposed)
                    {
                        if (disposing)
                        {
                            DisposeResponse();
                        }

                        _disposed = true;
                    }
                    base.Dispose(disposing);
                }
            }
        }
    }

    // abstract out the common functionality of an "HttpOutput"
    internal abstract class HttpOutput
    {
        private const string DefaultMimeVersion = "1.0";

        protected TimeoutHelper _timeoutHelper;
        private HttpAbortReason _abortReason;
        private bool _isDisposed;
        private bool _isRequest;
        private Message _message;
        private IHttpTransportFactorySettings _settings;
        private byte[] _bufferToRecycle;
        private BufferManager _bufferManager;
        private MessageEncoder _messageEncoder;
        private HttpClient _httpClient;
        private bool _streamed;
        private static Action<object> s_onStreamSendTimeout;
        private AuthenticationSchemes _authenticationScheme = AuthenticationSchemes.None;

        private Stream _outputStream;

        protected HttpOutput(IHttpTransportFactorySettings settings, Message message, bool isRequest)
        {
            _settings = settings;
            _message = message;
            _isRequest = isRequest;
            _bufferManager = settings.BufferManager;
            _messageEncoder = settings.MessageEncoderFactory.Encoder;
            if (isRequest)
            {
                _streamed = TransferModeHelper.IsRequestStreamed(settings.TransferMode);
            }
            else
            {
                _streamed = TransferModeHelper.IsResponseStreamed(settings.TransferMode);
            }
        }

        protected HttpOutput(IHttpTransportFactorySettings settings, Message message, bool isRequest, TimeoutHelper timeoutHelper)
            : this(settings, message, isRequest)
        {
            _timeoutHelper = timeoutHelper;
        }

        protected virtual bool IsChannelBindingSupportEnabled { get { return false; } }


        protected void Abort()
        {
            Abort(HttpAbortReason.Aborted);
        }

        public virtual void Abort(HttpAbortReason reason)
        {
            if (_isDisposed)
            {
                return;
            }

            _abortReason = reason;

            TraceRequestResponseAborted(reason);

            CleanupBuffer();
        }

        private void TraceRequestResponseAborted(HttpAbortReason reason)
        {
        }

        public async Task CloseAsync()
        {
            if (_isDisposed)
            {
                return;
            }

            try
            {
                if (_outputStream != null)
                {
                    await _outputStream.FlushAsync();
                    _outputStream.Dispose();
                }
            }
            finally
            {
                CleanupBuffer();
            }
        }

        private void CleanupBuffer()
        {
            byte[] bufferToRecycleSnapshot = Interlocked.Exchange<byte[]>(ref _bufferToRecycle, null);
            if (bufferToRecycleSnapshot != null)
            {
                _bufferManager.ReturnBuffer(bufferToRecycleSnapshot);
            }

            _isDisposed = true;
        }

        protected abstract void SetContentType(string contentType);
        protected abstract void SetContentEncoding(string contentEncoding);
        protected abstract void SetStatusCode(HttpStatusCode statusCode);
        protected abstract void SetStatusDescription(string statusDescription);
        protected virtual void SetContentLength(int contentLength)
        {
        }

        protected virtual string HttpMethod { get { return null; } }


        protected abstract Task<Stream> GetOutputStreamAsync();

        protected virtual bool WillGetOutputStreamCompleteSynchronously
        {
            get { return true; }
        }

        protected virtual bool PrepareHttpSend(Message message)
        {
            string action = message.Headers.Action;

            if (message.Version.Addressing == AddressingVersion.None)
            {
                message.Headers.Action = null;
                message.Headers.To = null;
            }

            string contentType = null;

            if (message.Version == MessageVersion.None)
            {
                object property = null;
                if (message.Properties.TryGetValue(HttpResponseMessageProperty.Name, out property))
                {
                    HttpResponseMessageProperty responseProperty = (HttpResponseMessageProperty)property;
                    if (responseProperty.HttpResponseMessage.Content != null && responseProperty.HttpResponseMessage.Content.Headers.ContentType != null)
                    {
                        contentType = responseProperty.HttpResponseMessage.Content.Headers.ContentType.ToString();
                        if (!_messageEncoder.IsContentTypeSupported(contentType))
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                                new ProtocolException(SR.Format(SR.ResponseContentTypeNotSupported,
                                contentType)));
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(contentType))
            {
                contentType = _messageEncoder.ContentType;
            }

            SetContentType(contentType);
            return message is NullMessage;
        }

        private ArraySegment<byte> SerializeBufferedMessage(Message message)
        {
            // by default, the HttpOutput should own the buffer and clean it up
            return SerializeBufferedMessage(message, true);
        }

        private ArraySegment<byte> SerializeBufferedMessage(Message message, bool shouldRecycleBuffer)
        {
            ArraySegment<byte> result;

            result = _messageEncoder.WriteMessage(message, int.MaxValue, _bufferManager);


            if (shouldRecycleBuffer)
            {
                // Only set this.bufferToRecycle if the HttpOutput owns the buffer, we will clean it up upon httpOutput.Close()
                // Otherwise, caller of SerializeBufferedMessage assumes responsiblity for returning the buffer to the buffer pool
                _bufferToRecycle = result.Array;
            }
            return result;
        }

        private Stream GetWrappedOutputStream()
        {
            return _outputStream;
        }

        private async Task WriteStreamedMessageAsync()
        {
            _outputStream = GetWrappedOutputStream();

            if (s_onStreamSendTimeout == null)
            {
                s_onStreamSendTimeout = new Action<object>(OnStreamSendTimeout);
            }

            using (_timeoutHelper.CancellationToken.Register(s_onStreamSendTimeout, this))
            {
                await _messageEncoder.WriteMessageAsync(_message, _outputStream);
            }
        }

        private static void OnStreamSendTimeout(object state)
        {
            HttpOutput thisPtr = (HttpOutput)state;
            thisPtr.Abort(HttpAbortReason.TimedOut);
        }

        private void TraceHttpSendStart()
        {
        }

        private void LogMessage()
        {
            if (MessageLogger.LogMessagesAtTransportLevel)
            {
                MessageLogger.LogMessage(ref _message, MessageLoggingSource.TransportSend);
            }
        }

        private bool AuthenticationSchemeMayRequireResend()
        {
            return _authenticationScheme == AuthenticationSchemes.Basic || _authenticationScheme == AuthenticationSchemes.Digest;
        }

        private async Task<HttpResponseMessage> SendHeaddAsync(Uri requestUri)
        {
            // sends a HEAD request to the specificed requestUri for authentication purposes 
            Contract.Assert(requestUri != null);

            HttpRequestMessage headHttpRequestMessage = new HttpRequestMessage()
            {
                Method = new HttpMethod("HEAD"),
                RequestUri = requestUri
            };

            return await _httpClient.SendAsync(headHttpRequestMessage, _timeoutHelper.CancellationToken);
        }

        public async Task<Task<HttpResponseMessage>> SendAsync(HttpRequestMessage httpRequest)
        {
            try
            {
                bool suppressEntityBody = PrepareHttpSend(_message);
                Task<HttpResponseMessage> responseMessageTask = null;

                TraceHttpSendStart();

                if (suppressEntityBody)
                {
                    // requests can't always support an output stream (for GET, etc)
                    if (!_isRequest)
                    {
                        _outputStream = await GetOutputStreamAsync();
                    }
                    else
                    {
                        this.SetContentLength(0);
                        LogMessage();
                    }
                }
                else if (_streamed)
                {
                    _outputStream = await GetOutputStreamAsync();

                    if (this.AuthenticationSchemeMayRequireResend())
                    {
                        Tuple<HttpOutput, HttpRequestMessage> stateTuple = Tuple.Create(this, httpRequest);

                        var sendHeadTask = SendHeaddAsync(httpRequest.RequestUri);
                        responseMessageTask = await sendHeadTask.ContinueWith<Task<HttpResponseMessage>>(async (t, obj) =>
                        {
                            Tuple<HttpOutput, HttpRequestMessage> state = obj as Tuple<HttpOutput, HttpRequestMessage>;

                            var thisPtr = state.Item1;
                            var httpRequestObj = state.Item2;
                            await t;
                            var response = await _httpClient.SendAsync(httpRequestObj, HttpCompletionOption.ResponseHeadersRead, thisPtr._timeoutHelper.CancellationToken);
                            return response;
                        }, stateTuple);
                    }
                    else
                    {
                        responseMessageTask = _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, _timeoutHelper.CancellationToken);
                    }
                    await WriteStreamedMessageAsync();
                }
                else
                {
                    if (this.IsChannelBindingSupportEnabled)
                    {
                        //need to get the Channel binding token (CBT), apply channel binding info to the message and then write the message                    
                        //CBT is only enabled when message security is in the stack, which also requires an HTTP entity body, so we 
                        //should be safe to always get the stream.
                        _outputStream = await GetOutputStreamAsync();

                        if (this.AuthenticationSchemeMayRequireResend())
                        {
                            Tuple<HttpOutput, HttpRequestMessage> stateTuple = Tuple.Create(this, httpRequest);

                            var sendHeadTask = SendHeaddAsync(httpRequest.RequestUri);
                            responseMessageTask = await sendHeadTask.ContinueWith<Task<HttpResponseMessage>>(async (t, obj) =>
                            {
                                Tuple<HttpOutput, HttpRequestMessage> state = obj as Tuple<HttpOutput, HttpRequestMessage>;

                                var thisPtr = state.Item1;
                                var httpRequestObj = state.Item2;
                                await t;
                                var response = await _httpClient.SendAsync(httpRequestObj, HttpCompletionOption.ResponseHeadersRead, thisPtr._timeoutHelper.CancellationToken);
                                return response;
                            }, stateTuple);
                        }
                        else
                        {
                            responseMessageTask = _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, _timeoutHelper.CancellationToken);
                        }
                        //  Channel Binding 
                        //ApplyChannelBinding();

                        ArraySegment<byte> buffer = SerializeBufferedMessage(_message);

                        Contract.Assert(buffer.Count != 0, "We should always have an entity body in this case...");
                        var writeTask = _outputStream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count, _timeoutHelper.CancellationToken);
                        var completedTask = await Task.WhenAny(responseMessageTask, writeTask);
                        if (!responseMessageTask.IsFaulted)
                        {
                            await writeTask;
                        }
                    }
                    else
                    {
                        ArraySegment<byte> buffer = SerializeBufferedMessage(_message);
                        SetContentLength(buffer.Count);

                        // requests can't always support an output stream (for GET, etc)
                        if (!_isRequest || buffer.Count > 0)
                        {
                            _outputStream = await GetOutputStreamAsync();

                            if (this.AuthenticationSchemeMayRequireResend())
                            {
                                Tuple<HttpOutput, HttpRequestMessage> stateTuple = Tuple.Create(this, httpRequest);

                                var sendHeadTask = SendHeaddAsync(httpRequest.RequestUri);
                                responseMessageTask = await sendHeadTask.ContinueWith<Task<HttpResponseMessage>>(async (t, obj) =>
                                {
                                    Tuple<HttpOutput, HttpRequestMessage> state = obj as Tuple<HttpOutput, HttpRequestMessage>;

                                    var thisPtr = state.Item1;
                                    var httpRequestObj = state.Item2;
                                    await t;
                                    var response = await _httpClient.SendAsync(httpRequestObj, HttpCompletionOption.ResponseHeadersRead, thisPtr._timeoutHelper.CancellationToken);
                                    return response;
                                }, stateTuple);
                            }
                            else
                            {
                                responseMessageTask = _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, _timeoutHelper.CancellationToken);
                            }

                            var writeTask = _outputStream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count, _timeoutHelper.CancellationToken);
                            var completedTask = await Task.WhenAny(responseMessageTask, writeTask);
                            if (!responseMessageTask.IsFaulted)
                            {
                                await writeTask;
                            }
                        }
                    }
                }

                TraceSend();
                return responseMessageTask;
            }
            catch (OperationCanceledException)
            {
                if (_timeoutHelper.CancellationToken.IsCancellationRequested)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(SR.Format(
                        SR.HttpRequestTimedOut, httpRequest.RequestUri, _timeoutHelper.OriginalTimeout)));
                }
                else
                {
                    // Cancellation came from somewhere other than timeoutCts and needs to be handled differently.
                    throw;
                }
            }
            finally
            {
                if (_outputStream != null)
                {
                    _outputStream.Dispose();
                }
            }
        }

        private void TraceSend()
        {
        }

        internal static HttpOutput CreateHttpOutput(HttpRequestMessage httpRequestMessage, IHttpTransportFactorySettings settings, Message message, bool enableChannelBindingSupport, HttpClient httpClient, AuthenticationSchemes authenticationScheme, TimeoutHelper timeoutHelper)
        {
            return new HttpRequestMessageHttpOutput(httpRequestMessage, settings, message, enableChannelBindingSupport, httpClient, authenticationScheme, timeoutHelper);
        }

        internal class HttpRequestMessageHttpOutput : HttpOutput
        {
            private HttpRequestMessage _httpRequest;
            private HttpRequestHttpContent _httpContent;
            private bool _enableChannelBindingSupport = false;

            public HttpRequestMessageHttpOutput(HttpRequestMessage httpRequest, IHttpTransportFactorySettings settings, Message message, bool enableChannelBindingSupport, HttpClient httpClient, AuthenticationSchemes authenticationScheme, TimeoutHelper timeoutHelper)
                : base(settings, message, true, timeoutHelper)
            {
                _httpClient = httpClient;
                _httpRequest = httpRequest;
                _httpContent = new HttpRequestHttpContent(base._timeoutHelper.CancellationToken);
                _httpRequest.Content = _httpContent;
                _outputStream = _httpContent.GetOutputStream();
                _authenticationScheme = authenticationScheme;
                Contract.Assert(!enableChannelBindingSupport, "ChannelBinding not supported");
            }

            public override void Abort(HttpAbortReason abortReason)
            {
                base._timeoutHelper.CancelCancellationToken();
                base.Abort(abortReason);
            }

            protected override void SetContentType(string contentType)
            {
                _httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            }

            protected override void SetContentEncoding(string contentEncoding)
            {
                _httpContent.Headers.ContentEncoding.Add(contentEncoding);
            }

            protected override void SetContentLength(int contentLength)
            {
                if (!_enableChannelBindingSupport && contentLength > 0) //When ChannelBinding is enabled, content length isn't supported
                {
                    _httpContent.Headers.ContentLength = contentLength;
                    _httpRequest.Headers.TransferEncodingChunked = false;
                }
                else
                {
                    _httpContent.Headers.ContentLength = null;
                    _httpRequest.Headers.TransferEncodingChunked = true;
                }
            }

            protected override void SetStatusCode(HttpStatusCode statusCode)
            {
            }

            protected override void SetStatusDescription(string statusDescription)
            {
            }

            protected override bool WillGetOutputStreamCompleteSynchronously
            {
                get { return false; }
            }

            protected override bool IsChannelBindingSupportEnabled
            {
                get
                {
                    return _enableChannelBindingSupport;
                }
            }


            protected override Task<Stream> GetOutputStreamAsync()
            {
                try
                {
                    if (this.IsChannelBindingSupportEnabled)
                    {
                        throw ExceptionHelper.PlatformNotSupported();
                    }
                    return Task.FromResult(_outputStream);
                }
                catch (HttpRequestException requestException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(HttpChannelUtilities.CreateHttpRequestException(requestException, _httpRequest, _abortReason));
                }
            }

            protected override bool PrepareHttpSend(Message message)
            {
                bool wasContentTypeSet = false;

                string action = message.Headers.Action;

                if (action != null)
                {
                    action = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", UrlUtility.UrlPathEncode(action));
                }

                bool suppressEntityBody = base.PrepareHttpSend(message);

                object property;
                if (message.Properties.TryGetValue(HttpRequestMessageProperty.Name, out property))
                {
                    HttpRequestMessageProperty requestProperty = (HttpRequestMessageProperty)property;
                    _httpRequest.Method = new HttpMethod(requestProperty.Method);
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
                            _httpRequest.Headers.Accept.TryParseAdd(value);
                        }
                        else if (string.Compare(name, "connection", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            if (value.IndexOf("keep-alive", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                _httpRequest.Headers.ConnectionClose = false;
                            }
                            else
                            {
                                _httpRequest.Headers.Connection.TryParseAdd(value);
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
                            MediaTypeHeaderValue contentType;
                            if (MediaTypeHeaderValue.TryParse(value, out contentType))
                            {
                                _httpContent.Headers.ContentType = contentType;
                                wasContentTypeSet = true;
                            }
                        }
                        else if (string.Compare(name, "expect", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            if (value.ToUpperInvariant().IndexOf("100-CONTINUE", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                _httpRequest.Headers.ExpectContinue = true;
                            }
                            else
                            {
                                _httpRequest.Headers.Expect.TryParseAdd(value);
                            }
                        }
                        else if (string.Compare(name, "host", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            // this should be controlled through Via
                        }
                        else if (string.Compare(name, "referer", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            // referrer is proper spelling, but referer is the what is in the protocol.

                            _httpRequest.Headers.Referrer = new Uri(value);
                        }
                        else if (string.Compare(name, "transfer-encoding", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            if (value.ToUpperInvariant().IndexOf("CHUNKED", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                _httpRequest.Headers.TransferEncodingChunked = true;
                            }
                            else
                            {
                                _httpRequest.Headers.TransferEncoding.TryParseAdd(value);
                            }
                        }
                        else if (string.Compare(name, "user-agent", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            _httpRequest.Headers.UserAgent.Add(ProductInfoHeaderValue.Parse(value));
                        }
                        else if (string.Compare(name, "if-modified-since", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            DateTimeOffset modifiedSinceDate;
                            if (DateTimeOffset.TryParse(value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out modifiedSinceDate))
                            {
                                _httpRequest.Headers.IfModifiedSince = modifiedSinceDate;
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
                            // we don't support ranges in v1.
                        }
                        else
                        {
                            if (!_httpRequest.Headers.TryAddWithoutValidation(name, value))
                            {
                                throw FxTrace.Exception.AsError(new InvalidOperationException(SR.Format(
                                                SR.CopyHttpHeaderFailed,
                                                name,
                                                value,
                                                _httpRequest.Headers.GetType().Name)));
                            }
                        }
                    }
                }

                if (action != null)
                {
                    if (message.Version.Envelope == EnvelopeVersion.Soap11)
                    {
                        _httpRequest.Headers.TryAddWithoutValidation("SOAPAction", action);
                    }
                    else if (message.Version.Envelope == EnvelopeVersion.Soap12)
                    {
                        if (message.Version.Addressing == AddressingVersion.None)
                        {
                            bool shouldSetContentType = true;
                            if (wasContentTypeSet)
                            {
                                var actionParams = (from p in _httpContent.Headers.ContentType.Parameters where p.Name == "action" select p).ToArray();
                                Contract.Assert(actionParams.Length <= 1, "action MUST only appear as a content type parameter at most 1 time");
                                if (actionParams.Length > 0)
                                {
                                    try
                                    {
                                        string value = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", actionParams[0].Value);
                                        if (string.Compare(value, action, StringComparison.Ordinal) != 0)
                                        {
                                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                                                new ProtocolException(SR.Format(SR.HttpSoapActionMismatchContentType, action, value)));
                                        }
                                        shouldSetContentType = false;
                                    }
                                    catch (FormatException formatException)
                                    {
                                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                                            new ProtocolException(SR.Format(SR.HttpContentTypeFormatException, formatException.Message, _httpContent.Headers.ContentType.ToString()), formatException));
                                    }
                                }
                            }

                            if (shouldSetContentType)
                            {
                                _httpContent.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("action", action));
                            }
                        }
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
                    _httpRequest.Headers.TransferEncodingChunked = false;
                }
                else if (this.IsChannelBindingSupportEnabled)
                {
                    //force chunked upload since the length of the message is unknown before encoding.
                    _httpRequest.Headers.TransferEncodingChunked = true;
                }

                return suppressEntityBody;
            }

            internal class HttpRequestHttpContent : HttpContent
            {
                private Stream _requestStream;
                // This TCS is used to provide a task for the forwarding stream class to await on if SerializeToStreamAsync
                // hasn't been called by HttpClient yet. It's possible we could try writing to the stream before HttpClient
                // has proivded it.
                private TaskCompletionSource<Stream> _transportStreamTcs;
                private Task<Stream> _transportStreamTask;
                // This TCS is used to provide a task to SerializeToStreamAsync to return to HttpClient. This allows us to
                // trigger HttpClient to continue after we have finished sending data on the stream.
                private TaskCompletionSource<bool> _streamCompletedTcs;
                private CancellationToken _token;

                public HttpRequestHttpContent(CancellationToken token)
                {
                    _token = token;
                    _transportStreamTcs = new TaskCompletionSource<Stream>(token);
                    _streamCompletedTcs = new TaskCompletionSource<bool>();
                    _transportStreamTask = _transportStreamTcs.Task;
                    _requestStream = new ForwardingAsyncStream(this);
                }

                protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
                {
                    _transportStreamTcs.TrySetResult(stream);
                    return _streamCompletedTcs.Task;
                }

                protected override bool TryComputeLength(out long length)
                {
                    // We can't know how much the Action<Stream> is going to write
                    length = -1;
                    return false;
                }

                public Stream GetOutputStream()
                {
                    return _requestStream;
                }

                private class ForwardingAsyncStream : Stream
                {
                    private HttpRequestHttpContent _httpContent;
                    private bool _disposed;

                    public ForwardingAsyncStream(HttpRequestHttpContent httpContent)
                    {
                        _httpContent = httpContent;
                    }

                    public override bool CanRead { get { return false; } }
                    public override bool CanSeek { get { return false; } }
                    public override bool CanWrite { get { return true; } }

                    protected override void Dispose(bool disposing)
                    {
                        if (!_disposed)
                        {
                            if (disposing)
                            {
                                if (_httpContent._transportStreamTask.Status == TaskStatus.RanToCompletion)
                                {
                                    var forwardStream = _httpContent._transportStreamTask.GetAwaiter().GetResult();
                                    forwardStream.Flush();
                                }
                                else
                                {
                                    _httpContent._transportStreamTcs.TrySetException(new ObjectDisposedException(this.GetType().ToString()));
                                }
                                _httpContent._streamCompletedTcs.TrySetResult(true);
                            }

                            _disposed = true;
                            // Do not call base.Dispose() as we don't want to throw ObjectDisposedException. Close() no longer exists on stream
                            // and Dispose() is what we're supposed to use now. We need to Dispose() so that we can complete the streamCompletedTcs.Task
                            // so that HttpClient knows we're finished sending our request.
                        }
                    }

                    public override void Flush()
                    {
                        // If httpClient hasn't given us a transportStream to write to yet, calling Flush would be a no-op.
                        // It would also additionally cause the thread to block until httpClient did provide a stream. If
                        // we don't have a forward stream yet, just return.
                        if (_httpContent._transportStreamTask.Status == TaskStatus.RanToCompletion)
                        {
                            var forwardStream = _httpContent._transportStreamTask.GetAwaiter().GetResult();
                            forwardStream.Flush();
                        }
                    }

                    public override Task FlushAsync(CancellationToken cancellationToken)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (_httpContent._transportStreamTask.Status == TaskStatus.RanToCompletion)
                        {
                            var forwardStream = _httpContent._transportStreamTask.GetAwaiter().GetResult();
                            return forwardStream.FlushAsync(cancellationToken);
                        }
                        return TaskHelpers.CompletedTask();
                    }

                    public override long Length { get { throw new NotSupportedException(); } }

                    public override long Position
                    {
                        get { throw new NotSupportedException(); }
                        set { throw new NotSupportedException(); }
                    }

                    public override int Read(byte[] buffer, int offset, int count)
                    {
                        throw new NotSupportedException();
                    }

                    public override Task<int> ReadAsync(Byte[] buffer, int offset, int count, CancellationToken cancellationToken)
                    {
                        throw new NotSupportedException();
                    }

                    public override long Seek(long offset, SeekOrigin origin)
                    {
                        throw new NotSupportedException();
                    }

                    public override void SetLength(long value)
                    {
                        throw new NotSupportedException();
                    }

                    public override void Write(byte[] buffer, int offset, int count)
                    {
                        WriteAsyncInternal(buffer, offset, count).WaitForCompletion();
                    }

                    private async Task WriteAsyncInternal(byte[] buffer, int offset, int count)
                    {
                        await TaskHelpers.EnsureDefaultTaskScheduler();
                        await this.WriteAsync(buffer, offset, count);
                    }

                    public override async Task WriteAsync(Byte[] buffer, int offset, int count, CancellationToken cancellationToken)
                    {
                        using (cancellationToken.Register(() => _httpContent._transportStreamTcs.TrySetCanceled()))
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            var forwardStream = await _httpContent._transportStreamTask;
                            cancellationToken.ThrowIfCancellationRequested();
                            await forwardStream.WriteAsync(buffer, offset, count, cancellationToken);
                        }
                    }
                }
            }
        }
    }

    internal enum HttpAbortReason
    {
        None,
        Aborted,
        TimedOut
    }

    internal static class HttpChannelUtilities
    {
        internal static class StatusDescriptionStrings
        {
            internal const string HttpContentTypeMissing = "Missing Content Type";
            internal const string HttpContentTypeMismatch = "Cannot process the message because the content type '{0}' was not the expected type '{1}'.";
            internal const string HttpStatusServiceActivationException = "System.ServiceModel.ServiceActivationException";
        }

        internal const string HttpStatusCodeExceptionKey = "System.ServiceModel.Channels.HttpInput.HttpStatusCode";
        internal const string HttpStatusDescriptionExceptionKey = "System.ServiceModel.Channels.HttpInput.HttpStatusDescription";

        internal const int ResponseStreamExcerptSize = 1024;
        internal const string MIMEVersionHeader = "MIME-Version";
        internal const string ContentEncodingHeader = "Content-Encoding";

        internal const uint WININET_E_NAME_NOT_RESOLVED = 0x80072EE7;
        internal const uint WININET_E_CONNECTION_RESET = 0x80072EFF;
        internal const uint WININET_E_INCORRECT_HANDLE_STATE = 0x80072EF3;

        public static HttpResponseMessage ProcessGetResponseWebException(HttpRequestException requestException, HttpRequestMessage request, HttpAbortReason abortReason)
        {
            var inner = requestException.InnerException;
            if (inner != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertHttpRequestException(requestException, request, abortReason));
            }
            else
            {
                // There is no inner exception so there's not enough information to be able to convert to the correct WCF exception.
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(requestException.Message, requestException));
            }
        }

        public static Exception ConvertHttpRequestException(HttpRequestException exception, HttpRequestMessage request, HttpAbortReason abortReason)
        {
            Contract.Assert(exception.InnerException != null, "InnerException must be set to be able to convert");

            uint hresult = (uint)exception.InnerException.HResult;
            switch (hresult)
            {
                // .Net Native HttpClientHandler sometimes reports an incorrect handle state when a connection is aborted, so we treat it as a connection reset error
                case WININET_E_INCORRECT_HANDLE_STATE:
                    goto case WININET_E_CONNECTION_RESET;
                case WININET_E_CONNECTION_RESET:
                    return new CommunicationException(SR.Format(SR.HttpReceiveFailure, request.RequestUri), exception);
                case WININET_E_NAME_NOT_RESOLVED:
                    return new EndpointNotFoundException(SR.Format(SR.EndpointNotFound, request.RequestUri.AbsoluteUri), exception);
                default:
                    return new CommunicationException(exception.Message, exception);
            }
        }

        public static Exception CreateResponseIOException(IOException ioException, TimeSpan receiveTimeout)
        {
            return new CommunicationException(SR.Format(SR.HttpTransferError, ioException.Message), ioException);
        }

        public static Exception CreateResponseHttpRequestException(HttpRequestException requestException, HttpResponseMessage response)
        {
            var baseException = requestException.GetBaseException();
            return new CommunicationException(baseException.Message, baseException);
        }

        public static Exception CreateRequestCanceledException(Exception httpException, HttpRequestMessage request, HttpAbortReason abortReason)
        {
            switch (abortReason)
            {
                case HttpAbortReason.Aborted:
                    return new CommunicationObjectAbortedException(SR.Format(SR.HttpRequestAborted, request.RequestUri), httpException);
                case HttpAbortReason.TimedOut:
                    return new TimeoutException(CreateRequestTimedOutMessage(request), httpException);
                default:
                    return new CommunicationException(SR.Format(SR.HttpTransferError, httpException.Message), httpException);
            }
        }

        public static Exception CreateRequestIOException(IOException ioException, HttpRequestMessage request)
        {
            return CreateRequestIOException(ioException, request, null);
        }

        public static Exception CreateRequestIOException(IOException ioException, HttpRequestMessage request, Exception originalException)
        {
            Exception exception = originalException == null ? ioException : originalException;

            return new CommunicationException(SR.Format(SR.HttpTransferError, exception.Message), exception);
        }

        private static string CreateRequestTimedOutMessage(HttpRequestMessage request)
        {
            return SR.Format(SR.HttpRequestTimedOut, request.RequestUri, 1000/*TimeSpan.FromMilliseconds(request.Timeout)*/);
        }

        public static Exception CreateHttpRequestException(HttpRequestException requestException, HttpRequestMessage request, HttpAbortReason abortReason)
        {
            var baseException = requestException.GetBaseException();
            return new CommunicationException(baseException.Message, baseException);
        }

        private static Exception CreateUnexpectedResponseException(HttpRequestException responseException, HttpResponseMessage response)
        {
            string statusDescription = response.ReasonPhrase;
            if (string.IsNullOrEmpty(statusDescription))
                statusDescription = response.StatusCode.ToString();

            return TraceResponseException(
                new ProtocolException(SR.Format(SR.UnexpectedHttpResponseCode,
                (int)response.StatusCode, statusDescription), responseException));
        }

        public static Exception CreateNullReferenceResponseException(NullReferenceException nullReferenceException)
        {
            return TraceResponseException(
                new ProtocolException(SR.NullReferenceOnHttpResponse, nullReferenceException));
        }

        private static string GetResponseStreamString(HttpResponseMessage responseMessage, out int bytesRead)
        {
            Stream responseStream = responseMessage.Content.ReadAsStreamAsync().GetAwaiter().GetResult();

            long bufferSize = responseMessage.Content.Headers.ContentLength.HasValue ? responseMessage.Content.Headers.ContentLength.Value : -1;

            if (bufferSize < 0 || bufferSize > ResponseStreamExcerptSize)
            {
                bufferSize = ResponseStreamExcerptSize;
            }

            byte[] responseBuffer = Fx.AllocateByteArray(checked((int)bufferSize));
            bytesRead = responseStream.Read(responseBuffer, 0, (int)bufferSize);
            responseStream.Dispose();

            return System.Text.Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
        }

        private static Exception TraceResponseException(Exception exception)
        {
            return exception;
        }

        private static bool ValidateEmptyContent(HttpResponseMessage response)
        {
            bool responseIsEmpty;

            if (response.Content == null)
            {
                responseIsEmpty = true;
            }
            else if (response.Content.Headers.ContentLength.HasValue && response.Content.Headers.ContentLength.Value > 0)
            {
                responseIsEmpty = false;
            }
            else // chunked 
            {
                Stream responseStream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                byte[] testBuffer = new byte[1];
                responseIsEmpty = (responseStream.Read(testBuffer, 0, 1) != 1);
            }
            return responseIsEmpty;
        }

        private static void ValidateAuthentication(HttpRequestMessage request, HttpResponseMessage response,
    HttpRequestException responseException, HttpChannelFactory<IRequestChannel> factory)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                string message = SR.Format(SR.HttpAuthorizationFailed, factory.AuthenticationScheme,
                    response.Headers.WwwAuthenticate.ToString());
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    TraceResponseException(new MessageSecurityException(message, responseException)));
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                string message = SR.Format(SR.HttpAuthorizationForbidden, factory.AuthenticationScheme);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    TraceResponseException(new MessageSecurityException(message, responseException)));
            }
        }

        // only valid response codes are 500 (if it's a fault) or 200 (iff it's a response message)
        public static HttpInput ValidateRequestReplyResponse(HttpRequestMessage request, HttpResponseMessage response,
          HttpChannelFactory<IRequestChannel> factory, HttpRequestException responseException)
        {
            ValidateAuthentication(request, response, responseException, factory);

            HttpInput httpInput = null;

            // We will close the HttpResponseMessage if we got an error code betwen 200 and 300 and 
            // 1) an exception was thrown out or 
            // 2) it's an empty message and we are using SOAP.
            // For responses with status code above 300, System.Net will close the underlying connection so we don't need to worry about that.
            if ((200 <= (int)response.StatusCode && (int)response.StatusCode < 300) || response.StatusCode == HttpStatusCode.InternalServerError)
            {
                if (response.StatusCode == HttpStatusCode.InternalServerError
                    && string.Compare(response.ReasonPhrase, HttpChannelUtilities.StatusDescriptionStrings.HttpStatusServiceActivationException, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ServiceActivationException(SR.Format(SR.Hosting_ServiceActivationFailed, request.RequestUri)));
                }
                else
                {
                    bool throwing = true;
                    try
                    {
                        string contentType = GetContentTypeString(response);
                        long contentLength = GetContentLength(response);
                        if (string.IsNullOrEmpty(contentType))
                        {
                            if (!ValidateEmptyContent(response))
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(TraceResponseException(
                                    new ProtocolException(
                                        SR.HttpContentTypeHeaderRequired,
                                        responseException)));
                            }
                        }
                        else if (contentLength != 0)
                        {
                            MessageEncoder encoder = factory.MessageEncoderFactory.Encoder;
                            if (!encoder.IsContentTypeSupported(contentType))
                            {
                                int bytesRead;
                                String responseExcerpt = GetResponseStreamString(response, out bytesRead);

                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(TraceResponseException(
                                    new ProtocolException(
                                        SR.Format(
                                            SR.ResponseContentTypeMismatch,
                                            contentType,
                                            encoder.ContentType,
                                            bytesRead,
                                            responseExcerpt), responseException)));
                            }

                            httpInput = HttpInput.CreateHttpInput(response, factory);
                            httpInput.HttpRequestException = responseException;
                        }

                        throwing = false;
                    }
                    finally
                    {
                        if (throwing)
                        {
                            response.Dispose();
                        }
                    }
                }

                if (httpInput == null)
                {
                    if (factory.MessageEncoderFactory.MessageVersion == MessageVersion.None)
                    {
                        httpInput = HttpInput.CreateHttpInput(response, factory);
                        httpInput.HttpRequestException = responseException;
                    }
                    else
                    {
                        // In this case, we got a response with
                        // 1) status code between 200 and 300
                        // 2) Non-empty Content Type string
                        // 3) Zero content length
                        // Since we are trying to use SOAP here, the message seems to be malicious and we should
                        // just close the response directly.
                        response.Dispose();
                    }
                }
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new EndpointNotFoundException(SR.Format(SR.EndpointNotFound, request.RequestUri.AbsoluteUri), responseException));
                }

                if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ServerTooBusyException(SR.Format(SR.HttpServerTooBusy, request.RequestUri.AbsoluteUri), responseException));
                }

                if (response.StatusCode == HttpStatusCode.UnsupportedMediaType)
                {
                    string statusDescription = response.ReasonPhrase;
                    if (!string.IsNullOrEmpty(statusDescription))
                    {
                        if (string.Compare(statusDescription, HttpChannelUtilities.StatusDescriptionStrings.HttpContentTypeMissing, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ProtocolException(SR.Format(SR.MissingContentType, request.RequestUri), responseException));
                        }
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ProtocolException(SR.Format(SR.FramingContentTypeMismatch, request.Content.Headers.ContentType.ToString(), request.RequestUri), responseException));
                }

                if (response.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(response.StatusCode + " " + response.ReasonPhrase, responseException));
                }

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnexpectedResponseException(responseException, response));
            }

            return httpInput;
        }

        // This method returns the entire content type including parameters as it will be sent over the wire, e.g. "application/soap+xml; charset=utf-8"
        public static string GetContentTypeString(HttpResponseMessage response)
        {
            if (response.Content != null)
            {
                return response.Content.Headers.ContentType == null ? null : response.Content.Headers.ContentType.ToString();
            }
            return null;
        }

        public static long GetContentLength(HttpResponseMessage response)
        {
            if (response.Content != null)
            {
                return response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1;
            }
            else
            {
                return 0;
            }
        }
    }

    internal abstract class BytesReadPositionStream : DelegatingStream
    {
        private int _bytesSent = 0;

        protected BytesReadPositionStream(Stream stream)
            : base(stream)
        {
        }

        public override long Position
        {
            get
            {
                return _bytesSent;
            }
            set
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.SPS_SeekNotSupported));
            }
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await base.WriteAsync(buffer, offset, count, cancellationToken);
            _bytesSent += count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);
            _bytesSent += count;
        }

        public override void WriteByte(byte value)
        {
            base.WriteByte(value);
            _bytesSent++;
        }
    }

    internal class PreReadStream : DelegatingStream
    {
        private byte[] _preReadBuffer;

        public PreReadStream(Stream stream, byte[] preReadBuffer)
            : base(stream)
        {
            _preReadBuffer = preReadBuffer;
        }

        private bool ReadFromBuffer(byte[] buffer, int offset, int count, out int bytesRead)
        {
            if (_preReadBuffer != null)
            {
                if (buffer == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
                }

                if (offset >= buffer.Length)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", offset,
                        SR.Format(SR.OffsetExceedsBufferBound, buffer.Length - 1)));
                }

                if (count < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", count,
                        SR.ValueMustBeNonNegative));
                }

                if (count == 0)
                {
                    bytesRead = 0;
                }
                else
                {
                    buffer[offset] = _preReadBuffer[0];
                    _preReadBuffer = null;
                    bytesRead = 1;
                }

                return true;
            }

            bytesRead = -1;
            return false;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int bytesRead;
            if (ReadFromBuffer(buffer, offset, count, out bytesRead))
            {
                return Task.FromResult<int>(bytesRead);
            }
            return base.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead;
            if (ReadFromBuffer(buffer, offset, count, out bytesRead))
            {
                return bytesRead;
            }

            return base.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            if (_preReadBuffer != null)
            {
                byte[] tempBuffer = new byte[1];
                int bytesRead;
                if (ReadFromBuffer(tempBuffer, 0, 1, out bytesRead))
                {
                    return tempBuffer[0];
                }
            }
            return base.ReadByte();
        }
    }
}
