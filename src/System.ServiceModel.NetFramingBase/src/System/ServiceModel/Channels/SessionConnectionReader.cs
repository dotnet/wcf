// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Buffers;
using System.Drawing;
using System.Runtime;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal abstract class SessionConnectionReader : IMessageSource
    {
        private bool _isAtEOF;
        private IConnection _connection;
        private byte[] _buffer;
        private int _offset;
        private int _size;
        private int _envelopeSize;
        private bool _readIntoEnvelopeBuffer;
        private Message _pendingMessage;
        private Exception _pendingException;

        protected SessionConnectionReader(IConnection connection)
        {
            _offset = 0;
            _size = 0;
            _connection = connection;
        }

        private Message DecodeMessage(TimeSpan timeout)
        {
            if (!_readIntoEnvelopeBuffer)
            {
                Fx.Assert(_buffer != null, "_buffer can't be null");
                return DecodeMessage(_buffer, ref _offset, ref _size, ref _isAtEOF, timeout);
            }
            else
            {
                // decode from the envelope buffer
                Fx.Assert(EnvelopeBuffer != null, "EnvelopeBuffer can't be null");
                int dummyOffset = EnvelopeOffset;
                return DecodeMessage(EnvelopeBuffer, ref dummyOffset, ref _size, ref _isAtEOF, timeout);
            }
        }

        protected abstract Message DecodeMessage(byte[] buffer, ref int offset, ref int size, ref bool isAtEof, TimeSpan timeout);

        protected byte[] EnvelopeBuffer { get; set; }

        protected int EnvelopeOffset { get; set; }

        protected int EnvelopeSize
        {
            get { return _envelopeSize; }
            set { _envelopeSize = value; }
        }

        public async Task<Message> ReceiveAsync(TimeSpan timeout)
        {
            Message message = GetPendingMessage();

            if (message != null)
            {
                return message;
            }

            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            for (; ; )
            {
                if (_isAtEOF)
                {
                    return null;
                }

                if (_size > 0)
                {
                    message = DecodeMessage(timeoutHelper.RemainingTime());

                    if (message != null)
                    {
                        PrepareMessage(message);
                        if (_size == 0)
                        {
                            ArrayPool<byte>.Shared.Return(_buffer);
                            _buffer = null;
                        }

                        return message;
                    }
                    else if (_isAtEOF) // could have read the END record under DecodeMessage
                    {
                        return null;
                    }
                }

                if (_size != 0)
                {
                    throw new Exception("Receive: DecodeMessage() should consume the outstanding buffer or return a message.");
                }

                if (_buffer == null)
                {
                    _buffer = ArrayPool<byte>.Shared.Rent(_connection.ConnectionBufferSize);
                }

                int bytesRead;
                if (EnvelopeBuffer != null && (EnvelopeSize - EnvelopeOffset) >= _buffer.Length)
                {
                    // Using IConnection.ConnectionBufferSize as the length for the Memory<byte> as the EnvelopeBuffer is only used when the envelope (SOAP message) is larger
                    // than the connection buffer size and we limit the amount of data read from the connection at a time to ConnectionBufferSize bytes.
                    bytesRead = await _connection.ReadAsync(new Memory<byte>(EnvelopeBuffer, EnvelopeOffset, _connection.ConnectionBufferSize), timeoutHelper.RemainingTime());
                    HandleReadComplete(bytesRead, true);
                }
                else
                {
                    // Using IConnection.ConnectionBufferSize as the length for the Memory<byte> as the leased buffer might be larger than ConnectionBufferSize and we
                    // limit the amount of data read from the connection at a time to ConnectionBufferSize bytes.
                    bytesRead = await _connection.ReadAsync(new Memory<byte>(_buffer, 0, _connection.ConnectionBufferSize), timeoutHelper.RemainingTime());
                    HandleReadComplete(bytesRead, false);
                }
            }
        }

        private Message GetPendingMessage()
        {
            if (_pendingException != null)
            {
                Exception exception = _pendingException;
                _pendingException = null;
                throw exception;
            }

            if (_pendingMessage != null)
            {
                Message message = _pendingMessage;
                _pendingMessage = null;
                return message;
            }

            return null;
        }

        public async Task<bool> WaitForMessageAsync(TimeSpan timeout)
        {
            try
            {
                Message message = await ReceiveAsync(timeout);
                _pendingMessage = message;
                return true;
            }
            catch (TimeoutException e)
            {
                if (WcfEventSource.Instance.ReceiveTimeoutIsEnabled())
                {
                    WcfEventSource.Instance.ReceiveTimeout(e.Message);
                }

                return false;
            }
        }

        protected abstract void EnsureDecoderAtEof();

        private void HandleReadComplete(int bytesRead, bool readIntoEnvelopeBuffer)
        {
            _readIntoEnvelopeBuffer = readIntoEnvelopeBuffer;

            if (bytesRead == 0)
            {
                EnsureDecoderAtEof();
                _isAtEOF = true;
            }
            else
            {
                _offset = 0;
                _size = bytesRead;
            }
        }

        protected virtual void PrepareMessage(Message message)
        {
        }
    }


    internal class ClientDuplexConnectionReader : SessionConnectionReader
    {
        private ClientDuplexDecoder _decoder;
        private int _maxBufferSize;
        private BufferManager _bufferManager;
        private MessageEncoder _messageEncoder;
        private ClientFramingDuplexSessionChannel _channel;

        public ClientDuplexConnectionReader(ClientFramingDuplexSessionChannel channel, IConnection connection, ClientDuplexDecoder decoder,
            IConnectionOrientedTransportFactorySettings settings, MessageEncoder messageEncoder)
            : base(connection)
        {
            _decoder = decoder;
            _maxBufferSize = settings.MaxBufferSize;
            _bufferManager = settings.BufferManager;
            _messageEncoder = messageEncoder;
            _channel = channel;
        }

        protected override void EnsureDecoderAtEof()
        {
            if (!(_decoder.CurrentState == ClientFramingDecoderState.End
                || _decoder.CurrentState == ClientFramingDecoderState.EnvelopeEnd
                || _decoder.CurrentState == ClientFramingDecoderState.ReadingUpgradeRecord
                || _decoder.CurrentState == ClientFramingDecoderState.UpgradeResponse))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(_decoder.CreatePrematureEOFException());
            }
        }

        private static IDisposable CreateProcessActionActivity()
        {
            return null;
        }

        protected override Message DecodeMessage(byte[] buffer, ref int offset, ref int size, ref bool isAtEOF, TimeSpan timeout)
        {
            while (size > 0)
            {
                int bytesRead = _decoder.Decode(buffer, offset, size);
                if (bytesRead > 0)
                {
                    if (EnvelopeBuffer != null)
                    {
                        if (!ReferenceEquals(buffer, EnvelopeBuffer))
                        {
                            Buffer.BlockCopy(buffer, offset, EnvelopeBuffer, EnvelopeOffset, bytesRead);
                        }

                        EnvelopeOffset += bytesRead;
                    }

                    offset += bytesRead;
                    size -= bytesRead;
                }

                switch (_decoder.CurrentState)
                {
                    case ClientFramingDecoderState.Fault:
                        _channel.Session.CloseOutputSession(_channel.GetInternalCloseTimeout());
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(FaultStringDecoder.GetFaultException(_decoder.Fault, _channel.RemoteAddress.Uri.ToString(), _messageEncoder.ContentType));

                    case ClientFramingDecoderState.End:
                        isAtEOF = true;
                        return null; // we're done

                    case ClientFramingDecoderState.EnvelopeStart:
                        int envelopeSize = _decoder.EnvelopeSize;
                        if (envelopeSize > _maxBufferSize)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                                ExceptionHelper.CreateMaxReceivedMessageSizeExceededException(_maxBufferSize));
                        }
                        EnvelopeBuffer = _bufferManager.TakeBuffer(envelopeSize);
                        EnvelopeOffset = 0;
                        EnvelopeSize = envelopeSize;
                        break;

                    case ClientFramingDecoderState.EnvelopeEnd:
                        if (EnvelopeBuffer != null)
                        {
                            Message message = null;
                            try
                            {
                                IDisposable activity = CreateProcessActionActivity();
                                using (activity)
                                {
                                    message = _messageEncoder.ReadMessage(new ArraySegment<byte>(EnvelopeBuffer, 0, EnvelopeSize), _bufferManager);
                                }
                            }
                            catch (XmlException xmlException)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                                    new ProtocolException(SR.MessageXmlProtocolError, xmlException));
                            }
                            EnvelopeBuffer = null;
                            return message;
                        }
                        break;
                }
            }

            return null;
        }
    }
}
