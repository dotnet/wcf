// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime;
using System.Threading.Tasks;
using System.Linq;

namespace System.ServiceModel.Channels
{
    public abstract class MessageContent : HttpContent
    {
#pragma warning disable 3008	// not CLS-compliant
        protected Message _message;
        protected MessageEncoder _messageEncoder;
        protected Stream _stream = null;
#pragma warning restore 3008	// not CLS-compliant
        private bool _disposed;

        public MessageContent(Message message, MessageEncoder messageEncoder)
        {
            _message = message;
            _messageEncoder = messageEncoder;

            SetContentType(_messageEncoder.ContentType);
            PrepareContentHeaders();
        }

        public Message Message
        {
            get
            {
                return _message;
            }
        }

        private void PrepareContentHeaders()
        {
            bool wasContentTypeSet = false;

            string action = _message.Headers.Action;

            if (action != null)
            {
                action = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", UrlUtility.UrlPathEncode(action));
            }

            object property;
            if (_message.Properties.TryGetValue(HttpRequestMessageProperty.Name, out property))
            {
                HttpRequestMessageProperty requestProperty = (HttpRequestMessageProperty)property;
                WebHeaderCollection requestHeaders = requestProperty.Headers;
                var headerKeys = requestHeaders.AllKeys;
                for (int i = 0; i < headerKeys.Length; i++)
                {
                    string name = headerKeys[i];
                    string value = requestHeaders[name];
                    if (string.Compare(name, "SOAPAction", StringComparison.OrdinalIgnoreCase) == 0)
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
                    else if (string.Compare(name, "content-type", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (SetContentType(value))
                        {
                            wasContentTypeSet = true;
                        }
                    }
                }
            }

            if (action != null)
            {
                if (_message.Version.Envelope == EnvelopeVersion.Soap12)
                {
                    if (_message.Version.Addressing == AddressingVersion.None)
                    {
                        bool shouldSetContentType = true;
                        if (wasContentTypeSet)
                        {
                            var actionParams = (from p in Headers.ContentType.Parameters where p.Name == "action" select p).ToArray();
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
                                        new ProtocolException(SR.Format(SR.HttpContentTypeFormatException, formatException.Message, Headers.ContentType.ToString()), formatException));
                                }
                            }
                        }

                        if (shouldSetContentType)
                        {
                            Headers.ContentType.Parameters.Add(new NameValueHeaderValue("action", action));
                        }
                    }
                }
            }
        }

        private bool SetContentType(string contentType)
        {
            MediaTypeHeaderValue contentTypeHeaderValue;
            if (MediaTypeHeaderValue.TryParse(contentType, out contentTypeHeaderValue))
            {
                Headers.ContentType = contentTypeHeaderValue;
                return true;
            }
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                if (_stream != null)
                {
                    var stream = _stream;
                    _stream = null;
                    stream.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        internal static HttpContent Create(HttpChannelFactory<IRequestChannel> factory, Message request, TimeoutHelper _timeoutHelper)
        {
            if (TransferModeHelper.IsRequestStreamed(factory.TransferMode))
            {
                return new StreamedMessageContent(request, factory.MessageEncoderFactory.Encoder);
            }
            else
            {
                return new BufferedMessageContent(request, factory.MessageEncoderFactory.Encoder, factory.BufferManager);
            }
        }
    }

    public class StreamedMessageContent : MessageContent
    {
        private Task _encodingTask;

        public StreamedMessageContent(Message message, MessageEncoder messageEncoder) : base(message, messageEncoder)
        {
        }


        protected override Task<Stream> CreateContentReadStreamAsync()
        {
            // WriteMessageAsync might run synchronously and try to write to the stream. ProducerConsumerStream
            // will block on the write until the stream is being read from. The WriteMessageAsync method needs
            // to run on a different thread to prevent a deadlock.
            _stream = new ProducerConsumerStream();
            _encodingTask = Task.Run(async () => await _messageEncoder.WriteMessageAsync(_message, _stream));
            return Task.FromResult(_stream);
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return _messageEncoder.WriteMessageAsync(_message, stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }

    internal class BufferedMessageContent : MessageContent
    {
        private bool _disposed;
        private bool _messageEncoded;
        private readonly BufferManager _bufferManager;
        private ArraySegment<byte> _buffer;
        private long? _contentLength;

        public BufferedMessageContent(Message message, MessageEncoder messageEncoder, BufferManager bufferManager) : base(message, messageEncoder)
        {
            Contract.Assert(bufferManager != null);
            _bufferManager = bufferManager;
            _messageEncoded = false;
        }

        protected override Task<Stream> CreateContentReadStreamAsync()
        {
            EnsureMessageEncoded();
            _stream = new MemoryStream(_buffer.Array, _buffer.Offset, _buffer.Count, false, true);
            return Task.FromResult(_stream);
        }

        private void EnsureMessageEncoded()
        {
            if (!_messageEncoded)
            {
                _buffer = _messageEncoder.WriteMessage(_message, int.MaxValue, _bufferManager);
                _contentLength = _buffer.Count;
                _messageEncoded = true;
            }
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            EnsureMessageEncoded();
            await stream.WriteAsync(_buffer.Array, _buffer.Offset, _buffer.Count);
        }

        protected override bool TryComputeLength(out long length)
        {
            EnsureMessageEncoded();
            if (_contentLength.HasValue)
            {
                length = (long)_contentLength;
                return true;
            }
            else
            {
                length = 0;
                return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                if (_buffer.Array != null)
                {
                    var byteArray = _buffer.Array;
                    _buffer = default(ArraySegment<byte>);
                    _bufferManager.ReturnBuffer(byteArray);
                }
            }

            base.Dispose(disposing);
        }
    }
}
