// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Buffers;
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
        private Memory<byte> _rawBuffer;
        private Memory<byte> _buffer;
        private int _envelopeSize;
        private Message _pendingMessage;
        private Exception _pendingException;
        private SecurityMessageProperty _security;
        // Raw connection that we will revert to after end handshake
        private IConnection _rawConnection;

        protected SessionConnectionReader(IConnection connection, IConnection rawConnection, SecurityMessageProperty security)
        {
            _rawBuffer = Fx.AllocateByteArray(connection.ConnectionBufferSize);
            _buffer = Memory<byte>.Empty;
            _connection = connection;
            _rawConnection = rawConnection;
            _security = security;
        }

        private Message DecodeMessage(TimeSpan timeout)
        {
            return DecodeMessage(ref _buffer, ref _isAtEOF, timeout);
        }

        protected abstract Message DecodeMessage(ref Memory<byte> buffer, ref bool isAtEof, TimeSpan timeout);

        protected byte[] EnvelopeBuffer { get; set; }

        protected int EnvelopeOffset { get; set; }

        protected int EnvelopeSize
        {
            get { return _envelopeSize; }
            set { _envelopeSize = value; }
        }

        public IConnection GetRawConnection()
        {
            IConnection result = null;
            if (_rawConnection != null)
            {
                result = _rawConnection;
                _rawConnection = null;
                if (!_buffer.IsEmpty)
                {
                    PreReadConnection preReadConnection = result as PreReadConnection;
                    if (preReadConnection != null) // make sure we don't keep wrapping
                    {
                        preReadConnection.AddPreReadData(_buffer);
                    }
                    else
                    {
                        result = new PreReadConnection(result, _buffer);
                    }
                }
            }

            return result;
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

                if (!_buffer.IsEmpty)
                {
                    message = DecodeMessage(timeoutHelper.RemainingTime());

                    if (message != null)
                    {
                        PrepareMessage(message);
                        return message;
                    }
                    else if (_isAtEOF) // could have read the END record under DecodeMessage
                    {
                        return null;
                    }
                }

                if (!_buffer.IsEmpty)
                {
                    throw new Exception("Receive: DecodeMessage() should consume the outstanding buffer or return a message.");
                }

                int bytesRead;

                if (EnvelopeBuffer != null && (EnvelopeSize - EnvelopeOffset) >= _rawBuffer.Length)
                {
                    bytesRead = await _connection.ReadAsync(new Memory<byte>(EnvelopeBuffer, EnvelopeOffset, EnvelopeSize - EnvelopeOffset), timeoutHelper.RemainingTime());
                    HandleReadComplete(bytesRead, true);
                }
                else
                {
                    bytesRead = await _connection.ReadAsync(_rawBuffer, timeoutHelper.RemainingTime());
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
            if (bytesRead == 0)
            {
                EnsureDecoderAtEof();
                _isAtEOF = true;
            }
            else
            {
                if (readIntoEnvelopeBuffer)
                {
                    _buffer = new Memory<byte>(EnvelopeBuffer, EnvelopeOffset, bytesRead);
                }
                else
                {
                    _buffer = _rawBuffer.Slice(0, bytesRead);
                }
            }
        }

        protected virtual void PrepareMessage(Message message)
        {
            if (_security != null)
            {
                message.Properties.Security = (SecurityMessageProperty)_security.CreateCopy();
            }
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
            : base(connection, null, null)
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

        protected override Message DecodeMessage(ref Memory<byte> buffer, ref bool isAtEOF, TimeSpan timeout)
        {
            bool copyToEnvelope = false;
            while (!buffer.IsEmpty)
            {
                int bytesRead = _decoder.Decode(new ReadOnlySequence<byte>(buffer));
                if (bytesRead > 0)
                {
                    if (copyToEnvelope)
                    {
                        buffer.CopyTo(new Memory<byte>(EnvelopeBuffer, EnvelopeOffset, bytesRead));
                        EnvelopeOffset += bytesRead;
                    }

                    buffer = buffer.Slice(bytesRead);
                }

                switch (_decoder.CurrentState)
                {
                    case ClientFramingDecoderState.Fault:
                        _channel.Session.CloseOutputSession(_channel.InternalCloseTimeout);
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(FaultStringDecoder.GetFaultException(_decoder.Fault, _channel.RemoteAddress.Uri.ToString(), _messageEncoder.ContentType));

                    case ClientFramingDecoderState.End:
                        isAtEOF = true;
                        return null; // we're done

                    case ClientFramingDecoderState.EnvelopeStart:
                        int envelopeSize = _decoder.EnvelopeSize;
                        if (envelopeSize > _maxBufferSize)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                                MaxMessageSizeStream.CreateMaxReceivedMessageSizeExceededException(_maxBufferSize));
                        }
                        EnvelopeBuffer = _bufferManager.TakeBuffer(envelopeSize);
                        EnvelopeSize = envelopeSize;
                        EnvelopeOffset = 0;
                        copyToEnvelope = true;
                        break;

                    case ClientFramingDecoderState.EnvelopeEnd:
                        if (EnvelopeBuffer != null)
                        {
                            Message message = null;
                            try
                            {
                                IDisposable activity = ClientDuplexConnectionReader.CreateProcessActionActivity();
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
