// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal class HttpResponseMessageHelper
    {
        private readonly HttpChannelFactory<IRequestChannel> _factory;
        private readonly MessageEncoder _encoder;
        private readonly HttpRequestMessage _httpRequestMessage;
        private readonly HttpResponseMessage _httpResponseMessage;
        private string _contentType;
        private long _contentLength;

        public HttpResponseMessageHelper(HttpResponseMessage httpResponseMessage, HttpChannelFactory<IRequestChannel> factory)
        {
            Contract.Assert(httpResponseMessage != null);
            Contract.Assert(httpResponseMessage.RequestMessage != null);
            Contract.Assert(factory != null);
            _httpResponseMessage = httpResponseMessage;
            _httpRequestMessage = httpResponseMessage.RequestMessage;
            _factory = factory;
            _encoder = factory.MessageEncoderFactory.Encoder;
        }

        internal async Task<Message> ParseIncomingResponse(TimeoutHelper timeoutHelper)
        {
            ValidateAuthentication();
            ValidateResponseStatusCode();
            bool hasContent = await ValidateContentTypeAsync(timeoutHelper);
            Message message = null;

            if (!hasContent)
            {
                if (_encoder.MessageVersion == MessageVersion.None)
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
                message = await ReadStreamAsMessageAsync(timeoutHelper);
            }

            var exception = ProcessHttpAddressing(message);
            Contract.Assert(exception == null, "ProcessHttpAddressing should not set an exception after parsing a response message.");

            return message;
        }

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
                    // message.Headers.Action uses an XmlDictionaryReader. If the xml is malformed,
                    // an XmlException might be thrown when trying to parse the response data.
                    // CommunicationException is the base type for any ServiceModel exceptions. If anything went
                    // wrong in any ServiceModel code, an exception deriving from CommunicationException will be
                    // thrown. 
                    // In these cases, be tolerant of the failure and treat it as though the action is absent.
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
                    // message.Headers.To has the same failure modes as for the Action header.
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

            return result;
        }

        private void AddProperties(Message message)
        {
            HttpResponseMessageProperty responseProperty = new HttpResponseMessageProperty(_httpResponseMessage);
            message.Properties.Add(HttpResponseMessageProperty.Name, responseProperty);
            message.Properties.Add("_System.Net.HttpStatusCode", responseProperty.StatusCode);
            message.Properties.Via = message.Version.Addressing.AnonymousUri;
        }

        private async Task<bool> ValidateContentTypeAsync(TimeoutHelper timeoutHelper)
        {
            var content = _httpResponseMessage.Content;
            if (content != null)
            {
                var mediaValueContentType = content.Headers.ContentType;
                _contentType = mediaValueContentType == null ? string.Empty : mediaValueContentType.ToString();
                _contentLength = content.Headers.ContentLength.HasValue ? content.Headers.ContentLength.Value : -1;
            }

            if (string.IsNullOrEmpty(_contentType))
            {
                Stream contentStream = await GetStreamAsync(timeoutHelper);
                if (contentStream != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ProtocolException(SR.HttpContentTypeHeaderRequired));
                }
                return false;
            }
            else if (_contentLength != 0)
            {
                if (!_encoder.IsContentTypeSupported(_contentType))
                {
                    int bytesToRead = (int)_contentLength;
                    Stream contentStream = await GetStreamAsync(timeoutHelper);
                    string responseExcerpt = HttpChannelUtilities.GetResponseStreamExcerptString(contentStream, ref bytesToRead);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(HttpChannelUtilities.TraceResponseException(
                        new ProtocolException(
                            SR.Format(
                                SR.ResponseContentTypeMismatch,
                                _contentType,
                                _encoder.ContentType,
                                bytesToRead,
                                responseExcerpt))));
                }
            }
            return true;
        }

        private Task<Message> ReadStreamAsMessageAsync(TimeoutHelper timeoutHelper)
        {
            var content = _httpResponseMessage.Content;
            Task<Stream> contentStreamTask = GetStreamAsync(timeoutHelper);

            if (TransferModeHelper.IsResponseStreamed(_factory.TransferMode))
            {
                return ReadStreamedMessageAsync(contentStreamTask);
            }

            if (!content.Headers.ContentLength.HasValue)
            {
                return ReadChunkedBufferedMessageAsync(contentStreamTask, timeoutHelper);
            }

            return ReadBufferedMessageAsync(contentStreamTask, timeoutHelper);
        }

        private async Task<Message> ReadChunkedBufferedMessageAsync(Task<Stream> inputStreamTask, TimeoutHelper timeoutHelper)
        {
            try
            {
                return await _encoder.ReadMessageAsync(await inputStreamTask, _factory.BufferManager, _factory.MaxBufferSize, _contentType, await timeoutHelper.GetCancellationTokenAsync());
            }
            catch (XmlException xmlException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ProtocolException(SR.MessageXmlProtocolError, xmlException));
            }
        }

        private async Task<Message> ReadBufferedMessageAsync(Task<Stream> inputStreamTask, TimeoutHelper timeoutHelper)
        {
            var inputStream = await inputStreamTask;
            if (_contentLength > _factory.MaxReceivedMessageSize)
            {
                ThrowMaxReceivedMessageSizeExceeded();
            }

            int bufferSize = (int)_contentLength;

            var messageBuffer = new ArraySegment<byte>(_factory.BufferManager.TakeBuffer(bufferSize), 0, bufferSize);

            byte[] buffer = messageBuffer.Array;
            int offset = 0;
            int count = messageBuffer.Count;
            var ct = await timeoutHelper.GetCancellationTokenAsync();

            while (count > 0)
            {
                int bytesRead = await inputStream.ReadAsync(buffer, offset, count, ct);
                if (bytesRead == 0) // EOF 
                {
                    if (_contentLength != -1)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new ProtocolException(SR.HttpContentLengthIncorrect));
                    }

                    break;
                }
                count -= bytesRead;
                offset += bytesRead;
            }

            return await DecodeBufferedMessageAsync(new ArraySegment<byte>(buffer, 0, offset), inputStream, timeoutHelper);
        }

        private async Task<Message> ReadStreamedMessageAsync(Task<Stream> inputStreamTask)
        {
            var inputStream = await inputStreamTask;
            var bufferedInputStream = inputStream as BufferedReadStream;
            MaxMessageSizeStream maxMessageSizeStream = new MaxMessageSizeStream(inputStream, _factory.MaxReceivedMessageSize);

            try
            {
                var message = await _encoder.ReadMessageAsync(maxMessageSizeStream, _factory.MaxBufferSize, _contentType);
                if (bufferedInputStream != null)
                {
                    message.Properties[BufferedReadStream.BufferedReadStreamPropertyName] = bufferedInputStream;
                }

                return message;
            }
            catch (XmlException xmlException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ProtocolException(SR.MessageXmlProtocolError, xmlException));
            }
        }

        private void ThrowMaxReceivedMessageSizeExceeded()
        {
            if (WcfEventSource.Instance.MaxReceivedMessageSizeExceededIsEnabled())
            {
                WcfEventSource.Instance.MaxReceivedMessageSizeExceeded(SR.Format(SR.MaxReceivedMessageSizeExceeded, _factory.MaxReceivedMessageSize));
            }

            string message = SR.Format(SR.MaxReceivedMessageSizeExceeded, _factory.MaxReceivedMessageSize);
            Exception inner = new QuotaExceededException(message);
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(message, inner));
        }

        private async Task<Message> DecodeBufferedMessageAsync(ArraySegment<byte> buffer, Stream inputStream, TimeoutHelper timeoutHelper)
        {
            try
            {
                var ct = await timeoutHelper.GetCancellationTokenAsync();
                // if we're chunked, make sure we've consumed the whole body
                if (_contentLength == -1 && buffer.Count == _factory.MaxReceivedMessageSize)
                {
                    byte[] extraBuffer = new byte[1];
                    int extraReceived = await inputStream.ReadAsync(extraBuffer, 0, 1, ct);
                    if (extraReceived > 0)
                    {
                        ThrowMaxReceivedMessageSizeExceeded();
                    }
                }

                try
                {
                    return _encoder.ReadMessage(buffer, _factory.BufferManager, _contentType);
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

        private async Task<Stream> GetStreamAsync(TimeoutHelper timeoutHelper)
        {
            var content = _httpResponseMessage.Content;
            Stream contentStream = null;
            _contentLength = -1;
            if (content != null)
            {
                contentStream = await content.ReadAsStreamAsync();
                _contentLength = content.Headers.ContentLength.HasValue ? content.Headers.ContentLength.Value : -1;
                var cancellationToken = await timeoutHelper.GetCancellationTokenAsync();
                if (_contentLength <= 0)
                {
                    var preReadBuffer = new byte[1];
                    if (await contentStream.ReadAsync(preReadBuffer, 0, 1, cancellationToken) == 0)
                    {
                        contentStream.Dispose();
                        contentStream = null;
                    }
                    else
                    {
                        var bufferedStream = new BufferedReadStream(contentStream, _factory.BufferManager);
                        await bufferedStream.PreReadBufferAsync(preReadBuffer[0], cancellationToken);
                        contentStream = bufferedStream;
                    }
                }
                else if (TransferModeHelper.IsResponseStreamed(_factory.TransferMode))
                {
                    // If _contentLength > 0, then the message was sent buffered but we might still
                    // be receiving it streamed. In which case we need a buffered reading stream.
                    var bufferedStream = new BufferedReadStream(contentStream, _factory.BufferManager);
                    await bufferedStream.PreReadBufferAsync(cancellationToken);
                    contentStream = bufferedStream;
                }
            }

            return contentStream;
        }

        private void ValidateResponseStatusCode()
        {
            if (((int)_httpResponseMessage.StatusCode < 200 || (int)_httpResponseMessage.StatusCode >= 300) && _httpResponseMessage.StatusCode != HttpStatusCode.InternalServerError)
            {
                if (_httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new EndpointNotFoundException(SR.Format(SR.EndpointNotFound, _httpRequestMessage.RequestUri.AbsoluteUri)));
                }

                if (_httpResponseMessage.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ServerTooBusyException(SR.Format(SR.HttpServerTooBusy, _httpRequestMessage.RequestUri.AbsoluteUri)));
                }

                if (_httpResponseMessage.StatusCode == HttpStatusCode.UnsupportedMediaType)
                {
                    string statusDescription = _httpResponseMessage.ReasonPhrase;
                    if (!string.IsNullOrEmpty(statusDescription))
                    {
                        if (string.Compare(statusDescription, HttpChannelUtilities.StatusDescriptionStrings.HttpContentTypeMissing, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ProtocolException(SR.Format(SR.MissingContentType, _httpRequestMessage.RequestUri)));
                        }
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ProtocolException(SR.Format(SR.FramingContentTypeMismatch, _httpRequestMessage.Content.Headers.ContentType.ToString(), _httpRequestMessage.RequestUri)));
                }

                if (_httpResponseMessage.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(_httpResponseMessage.StatusCode + " " + _httpResponseMessage.ReasonPhrase));
                }

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(HttpChannelUtilities.CreateUnexpectedResponseException(_httpResponseMessage));
            }
        }

        private void ValidateAuthentication()
        {
            if (_httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                string message = SR.Format(SR.HttpAuthorizationFailed, _factory.AuthenticationScheme,
                    _httpResponseMessage.Headers.WwwAuthenticate.ToString());
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    HttpChannelUtilities.TraceResponseException(new MessageSecurityException(message)));
            }

            if (_httpResponseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                string message = SR.Format(SR.HttpAuthorizationForbidden, _factory.AuthenticationScheme);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    HttpChannelUtilities.TraceResponseException(new MessageSecurityException(message)));
            }
        }
    }
}
