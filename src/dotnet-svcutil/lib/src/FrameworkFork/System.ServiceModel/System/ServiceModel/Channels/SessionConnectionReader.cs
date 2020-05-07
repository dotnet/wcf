// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using Microsoft.Xml;

namespace System.ServiceModel.Channels
{
    internal abstract class SessionConnectionReader : IMessageSource
    {
        private bool _isAtEOF;
        private bool _usingAsyncReadBuffer;
        private IConnection _connection;
        private byte[] _buffer;
        private int _offset;
        private int _size;
        private byte[] _envelopeBuffer;
        private int _envelopeOffset;
        private int _envelopeSize;
        private bool _readIntoEnvelopeBuffer;
        private Message _pendingMessage;
        private Exception _pendingException;
        private SecurityMessageProperty _security;
        // Raw connection that we will revert to after end handshake
        private IConnection _rawConnection;

        protected SessionConnectionReader(IConnection connection, IConnection rawConnection,
            int offset, int size, SecurityMessageProperty security)
        {
            _offset = offset;
            _size = size;
            if (size > 0)
            {
                _buffer = connection.AsyncReadBuffer;
            }
            _connection = connection;
            _rawConnection = rawConnection;
            _security = security;
        }

        private Message DecodeMessage(TimeSpan timeout)
        {
            if (!_readIntoEnvelopeBuffer)
            {
                return DecodeMessage(_buffer, ref _offset, ref _size, ref _isAtEOF, timeout);
            }
            else
            {
                // decode from the envelope buffer
                int dummyOffset = _envelopeOffset;
                return DecodeMessage(_envelopeBuffer, ref dummyOffset, ref _size, ref _isAtEOF, timeout);
            }
        }

        protected abstract Message DecodeMessage(byte[] buffer, ref int offset, ref int size, ref bool isAtEof, TimeSpan timeout);

        protected byte[] EnvelopeBuffer
        {
            get { return _envelopeBuffer; }
            set { _envelopeBuffer = value; }
        }

        protected int EnvelopeOffset
        {
            get { return _envelopeOffset; }
            set { _envelopeOffset = value; }
        }

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
                if (_size > 0)
                {
                    PreReadConnection preReadConnection = result as PreReadConnection;
                    if (preReadConnection != null) // make sure we don't keep wrapping
                    {
                        preReadConnection.AddPreReadData(_buffer, _offset, _size);
                    }
                    else
                    {
                        result = new PreReadConnection(result, _buffer, _offset, _size);
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

                if (_size > 0)
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

                if (_size != 0)
                {
                    throw new Exception("Receive: DecodeMessage() should consume the outstanding buffer or return a message.");
                }

                if (!_usingAsyncReadBuffer)
                {
                    _buffer = _connection.AsyncReadBuffer;
                    _usingAsyncReadBuffer = true;
                }

                int bytesRead;

                var tcs = new TaskCompletionSource<bool>();
                var result = _connection.BeginRead(0, _buffer.Length, timeoutHelper.RemainingTime(), TaskHelpers.OnAsyncCompletionCallback, tcs);
                if (result == AsyncCompletionResult.Completed)
                {
                    tcs.TrySetResult(true);
                }
                await tcs.Task;

                bytesRead = _connection.EndRead();
                HandleReadComplete(bytesRead, false);
            }
        }

        public Message Receive(TimeSpan timeout)
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
                    _buffer = Fx.AllocateByteArray(_connection.AsyncReadBufferSize);
                }

                int bytesRead;

                if (EnvelopeBuffer != null &&
                    (EnvelopeSize - EnvelopeOffset) >= _buffer.Length)
                {
                    bytesRead = _connection.Read(EnvelopeBuffer, EnvelopeOffset, _buffer.Length, timeoutHelper.RemainingTime());
                    HandleReadComplete(bytesRead, true);
                }
                else
                {
                    bytesRead = _connection.Read(_buffer, 0, _buffer.Length, timeoutHelper.RemainingTime());
                    HandleReadComplete(bytesRead, false);
                }
            }
        }

        public Message EndReceive()
        {
            return GetPendingMessage();
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

        public bool WaitForMessage(TimeSpan timeout)
        {
            try
            {
                Message message = Receive(timeout);
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
            : base(connection, null, 0, 0, null)
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
                        if (!object.ReferenceEquals(buffer, EnvelopeBuffer))
                            System.Buffer.BlockCopy(buffer, offset, EnvelopeBuffer, EnvelopeOffset, bytesRead);
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
                                IDisposable activity = ClientDuplexConnectionReader.CreateProcessActionActivity();
                                using (activity)
                                {
                                    message = _messageEncoder.ReadMessage(new ArraySegment<byte>(EnvelopeBuffer, 0, EnvelopeSize), _bufferManager);
                                }
                            }
                            catch (XmlException xmlException)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                                    new ProtocolException(SRServiceModel.MessageXmlProtocolError, xmlException));
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
