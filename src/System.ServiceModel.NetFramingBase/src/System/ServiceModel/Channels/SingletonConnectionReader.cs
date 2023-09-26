// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using System.ServiceModel.Security;
using System.ServiceModel.Diagnostics;
using System.Buffers;
using System.Runtime.InteropServices;

namespace System.ServiceModel.Channels
{
    internal abstract class SingletonConnectionReader
    {
        private bool _doneReceiving;
        private bool _doneSending;
        private bool _isAtEof;
        private bool _isClosed;
        private SecurityMessageProperty _security;
        private IConnectionOrientedTransportFactorySettings _transportSettings;
        private Uri _via;
        private Stream _inputStream;

        protected SingletonConnectionReader(IConnection connection, SecurityMessageProperty security,
            IConnectionOrientedTransportFactorySettings transportSettings, Uri via)
        {
            Contract.Assert(connection != null);

            Connection = connection;
            _security = security;
            _transportSettings = transportSettings;
            _via = via;
        }

        protected IConnection Connection { get; }

        protected object ThisLock { get; } = new object();

        protected virtual string ContentType
        {
            get { return null; }
        }

        protected abstract long StreamPosition { get; }

        public void Abort()
        {
            Connection.Abort();
        }

        public void DoneReceiving(bool atEof)
        {
            DoneReceiving(atEof, _transportSettings.CloseTimeout);
        }

        private void DoneReceiving(bool atEof, TimeSpan timeout)
        {
            if (!_doneReceiving)
            {
                _isAtEof = atEof;
                _doneReceiving = true;

                if (_doneSending)
                {
                    Close(timeout);
                }
            }
        }

        public void Close(TimeSpan timeout)
        {
            lock (ThisLock)
            {
                if (_isClosed)
                {
                    return;
                }

                _isClosed = true;
            }

            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            bool success = false;
            try
            {
                // first drain our stream if necessary
                if (_inputStream != null)
                {
                    byte[] dummy = Fx.AllocateByteArray(_transportSettings.ConnectionBufferSize);
                    while (!_isAtEof)
                    {
                        _inputStream.ReadTimeout = TimeoutHelper.ToMilliseconds(timeoutHelper.RemainingTime());
                        int bytesRead = _inputStream.Read(dummy, 0, dummy.Length);
                        if (bytesRead == 0)
                        {
                            _isAtEof = true;
                        }
                    }
                }
                OnClose(timeoutHelper.RemainingTime());
                success = true;
            }
            finally
            {
                if (!success)
                {
                    Abort();
                }
            }
        }

        protected abstract void OnClose(TimeSpan timeout);

        public void DoneSending(TimeSpan timeout)
        {
            _doneSending = true;
            if (_doneReceiving)
            {
                Close(timeout);
            }
        }

        protected abstract bool DecodeBytes(byte[] buffer, ref int offset, ref int size, ref bool isAtEof);

        protected virtual void PrepareMessage(Message message)
        {
            message.Properties.Via = _via;
            message.Properties.Security = (_security != null) ? (SecurityMessageProperty)_security.CreateCopy() : null;
        }

        public async Task<Message> ReceiveAsync(TimeoutHelper timeoutHelper)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(Connection.ConnectionBufferSize);
            int offset = 0, size = 0;
            for (; ; )
            {
                if (DecodeBytes(buffer, ref offset, ref size, ref _isAtEof))
                {
                    break;
                }

                if (_isAtEof)
                {
                    DoneReceiving(true, timeoutHelper.RemainingTime());
                    return null;
                }

                if (size == 0)
                {
                    offset = 0;
                    size = await Connection.ReadAsync(new Memory<byte>(buffer, 0, Connection.ConnectionBufferSize), timeoutHelper.RemainingTime());
                    if (size == 0)
                    {
                        DoneReceiving(true, timeoutHelper.RemainingTime());
                        ArrayPool<byte>.Shared.Return(buffer);
                        return null;
                    }
                }
            }

            // we're ready to read a message
            IConnection singletonConnection = Connection;
            if (size > 0)
            {
                byte[] initialData = Fx.AllocateByteArray(size);
                Buffer.BlockCopy(buffer, offset, initialData, 0, size);
                singletonConnection = new PreReadConnection(singletonConnection, initialData);
            }

            ArrayPool<byte>.Shared.Return(buffer);

            Stream connectionStream = new SingletonInputConnectionStream(this, singletonConnection, _transportSettings);
            _inputStream = new MaxMessageSizeStream(connectionStream, _transportSettings.MaxReceivedMessageSize);
            using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity(true) : null)
            {
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    ServiceModelActivity.Start(activity, SR.Format(SR.ActivityProcessingMessage, TraceUtility.RetrieveMessageNumber()), ActivityType.ProcessMessage);
                }

                Message message = null;
                try
                {
                    message = _transportSettings.MessageEncoderFactory.Encoder.ReadMessage(
                        _inputStream, _transportSettings.MaxBufferSize, ContentType);
                }
                catch (XmlException xmlException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new ProtocolException(SR.Format(SR.MessageXmlProtocolError), xmlException));
                }

                if (DiagnosticUtility.ShouldUseActivity)
                {
                    TraceUtility.TransferFromTransport(message);
                }

                PrepareMessage(message);

                return message;
            }
        }

        // ensures that the reader is notified at end-of-stream, and takes care of the framing chunk headers
        private class SingletonInputConnectionStream : ConnectionStream
        {
            private SingletonMessageDecoder _decoder;
            private SingletonConnectionReader _reader;
            private bool _atEof;
            private byte[] _chunkBuffer; // used for when we have overflow
            private int _chunkBufferOffset;
            private int _chunkBufferSize;
            private int _chunkBytesRemaining;

            public SingletonInputConnectionStream(SingletonConnectionReader reader, IConnection connection,
                IDefaultCommunicationTimeouts defaultTimeouts)
                : base(connection, defaultTimeouts)
            {
                _reader = reader;
                _decoder = new SingletonMessageDecoder(reader.StreamPosition);
                _chunkBytesRemaining = 0;
                _chunkBuffer = new byte[IntEncoder.MaxEncodedSize];
            }

            private void AbortReader()
            {
                _reader.Abort();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _reader.DoneReceiving(_atEof);
                }
            }

            // run chunk data through the decoder
            private void DecodeData(byte[] buffer, int offset, int size)
            {
                while (size > 0)
                {
                    int bytesRead = _decoder.Decode(buffer, offset, size);
                    offset += bytesRead;
                    size -= bytesRead;
                    Fx.Assert(_decoder.CurrentState == SingletonMessageDecoder.State.ReadingEnvelopeBytes || _decoder.CurrentState == SingletonMessageDecoder.State.ChunkEnd, "");
                }
            }

            // run the current data through the decoder to get valid message bytes
            private void DecodeSize(byte[] buffer, ref int offset, ref int size)
            {
                while (size > 0)
                {
                    int bytesRead = _decoder.Decode(buffer, offset, size);

                    if (bytesRead > 0)
                    {
                        offset += bytesRead;
                        size -= bytesRead;
                    }

                    switch (_decoder.CurrentState)
                    {
                        case SingletonMessageDecoder.State.ChunkStart:
                            _chunkBytesRemaining = _decoder.ChunkSize;

                            // if we have overflow and we're not decoding out of our buffer, copy over
                            if (size > 0 && !object.ReferenceEquals(buffer, _chunkBuffer))
                            {
                                Fx.Assert(size <= _chunkBuffer.Length, "");
                                Buffer.BlockCopy(buffer, offset, _chunkBuffer, 0, size);
                                _chunkBufferOffset = 0;
                                _chunkBufferSize = size;
                            }
                            return;

                        case SingletonMessageDecoder.State.End:
                            ProcessEof();
                            return;
                    }
                }
            }

            private int ReadCore(byte[] buffer, int offset, int count)
            {
                int bytesRead = -1;
                try
                {
                    bytesRead = base.Read(buffer, offset, count);
                    if (count != 0 && bytesRead == 0)
                    {
                        ProcessEof();
                    }
                }
                finally
                {
                    if (bytesRead == -1)  // there was an exception
                    {
                        AbortReader();
                    }
                }

                return bytesRead;
            }

            private async ValueTask<int> ReadCoreAsync(Memory<byte> buffer)
            {
                int bytesRead = -1;
                try
                {
                    bytesRead = await base.ReadAsync(buffer);
                    if (!buffer.IsEmpty && bytesRead == 0)
                    {
                        ProcessEof();
                    }
                }
                finally
                {
                    if (bytesRead == -1)  // there was an exception
                    {
                        AbortReader();
                    }
                }

                return bytesRead;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int result = 0;
                while (true)
                {
                    if (count == 0)
                    {
                        return result;
                    }

                    if (_atEof)
                    {
                        return result;
                    }

                    // first deal with any residual carryover
                    if (_chunkBufferSize > 0)
                    {
                        int bytesToCopy = Math.Min(_chunkBytesRemaining,
                            Math.Min(_chunkBufferSize, count));

                        Buffer.BlockCopy(_chunkBuffer, _chunkBufferOffset, buffer, offset, bytesToCopy);
                        // keep decoder up to date
                        DecodeData(_chunkBuffer, _chunkBufferOffset, bytesToCopy);

                        _chunkBufferOffset += bytesToCopy;
                        _chunkBufferSize -= bytesToCopy;
                        _chunkBytesRemaining -= bytesToCopy;
                        if (_chunkBytesRemaining == 0 && _chunkBufferSize > 0)
                        {
                            DecodeSize(_chunkBuffer, ref _chunkBufferOffset, ref _chunkBufferSize);
                        }

                        result += bytesToCopy;
                        offset += bytesToCopy;
                        count -= bytesToCopy;
                    }
                    else if (_chunkBytesRemaining > 0)
                    {
                        // We're in the middle of a chunk. Try and include the next chunk size as well

                        int bytesToRead = count;
                        if (int.MaxValue - _chunkBytesRemaining >= IntEncoder.MaxEncodedSize)
                        {
                            bytesToRead = Math.Min(count, _chunkBytesRemaining + IntEncoder.MaxEncodedSize);
                        }

                        int bytesRead = ReadCore(buffer, offset, bytesToRead);

                        // keep decoder up to date
                        DecodeData(buffer, offset, Math.Min(bytesRead, _chunkBytesRemaining));

                        if (bytesRead > _chunkBytesRemaining)
                        {
                            result += _chunkBytesRemaining;
                            int overflowCount = bytesRead - _chunkBytesRemaining;
                            int overflowOffset = offset + _chunkBytesRemaining;
                            _chunkBytesRemaining = 0;
                            // read at least part of the next chunk, and put any overflow in this.chunkBuffer
                            DecodeSize(buffer, ref overflowOffset, ref overflowCount);
                        }
                        else
                        {
                            result += bytesRead;
                            _chunkBytesRemaining -= bytesRead;
                        }

                        return result;
                    }
                    else
                    {
                        // Final case: we have a new chunk. Read the size, and loop around again
                        if (count < IntEncoder.MaxEncodedSize)
                        {
                            // we don't have space for MaxEncodedSize, so it's worth the copy cost to read into a temp buffer
                            _chunkBufferOffset = 0;
                            _chunkBufferSize = ReadCore(_chunkBuffer, 0, _chunkBuffer.Length);
                            DecodeSize(_chunkBuffer, ref _chunkBufferOffset, ref _chunkBufferSize);
                        }
                        else
                        {
                            int bytesRead = ReadCore(buffer, offset, IntEncoder.MaxEncodedSize);
                            int sizeOffset = offset;
                            DecodeSize(buffer, ref sizeOffset, ref bytesRead);
                        }
                    }
                }
            }

            private void ProcessEof()
            {
                if (!_atEof)
                {
                    _atEof = true;
                    if (_chunkBufferSize > 0 || _chunkBytesRemaining > 0
                        || _decoder.CurrentState != SingletonMessageDecoder.State.End)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(_decoder.CreatePrematureEOFException());
                    }

                    _reader.DoneReceiving(true);
                }
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                int result = 0;
                while (true)
                {
                    if (count == 0)
                    {
                        return result;
                    }

                    if (_atEof)
                    {
                        return result;
                    }

                    // first deal with any residual carryover
                    if (_chunkBufferSize > 0)
                    {
                        int bytesToCopy = Math.Min(_chunkBytesRemaining,
                            Math.Min(_chunkBufferSize, count));

                        Buffer.BlockCopy(_chunkBuffer, _chunkBufferOffset, buffer, offset, bytesToCopy);
                        // keep decoder up to date
                        DecodeData(_chunkBuffer, _chunkBufferOffset, bytesToCopy);

                        _chunkBufferOffset += bytesToCopy;
                        _chunkBufferSize -= bytesToCopy;
                        _chunkBytesRemaining -= bytesToCopy;
                        if (_chunkBytesRemaining == 0 && _chunkBufferSize > 0)
                        {
                            DecodeSize(_chunkBuffer, ref _chunkBufferOffset, ref _chunkBufferSize);
                        }

                        result += bytesToCopy;
                        offset += bytesToCopy;
                        count -= bytesToCopy;
                    }
                    else if (_chunkBytesRemaining > 0)
                    {
                        // We're in the middle of a chunk. Try and include the next chunk size as well

                        int bytesToRead = count;
                        if (int.MaxValue - _chunkBytesRemaining >= IntEncoder.MaxEncodedSize)
                        {
                            bytesToRead = Math.Min(count, _chunkBytesRemaining + IntEncoder.MaxEncodedSize);
                        }

                        int bytesRead = await ReadCoreAsync(new Memory<byte>(buffer, offset, bytesToRead));

                        // keep decoder up to date
                        DecodeData(buffer, offset, Math.Min(bytesRead, _chunkBytesRemaining));

                        if (bytesRead > _chunkBytesRemaining)
                        {
                            result += _chunkBytesRemaining;
                            int overflowCount = bytesRead - _chunkBytesRemaining;
                            int overflowOffset = offset + _chunkBytesRemaining;
                            _chunkBytesRemaining = 0;
                            // read at least part of the next chunk, and put any overflow in this.chunkBuffer
                            DecodeSize(buffer, ref overflowOffset, ref overflowCount);
                        }
                        else
                        {
                            result += bytesRead;
                            _chunkBytesRemaining -= bytesRead;
                        }

                        return result;
                    }
                    else
                    {
                        // Final case: we have a new chunk. Read the size, and loop around again
                        if (count < IntEncoder.MaxEncodedSize)
                        {
                            // we don't have space for MaxEncodedSize, so it's worth the copy cost to read into a temp buffer
                            _chunkBufferOffset = 0;
                            _chunkBufferSize = await ReadCoreAsync(_chunkBuffer);
                            DecodeSize(_chunkBuffer, ref _chunkBufferOffset, ref _chunkBufferSize);
                        }
                        else
                        {
                            int bytesRead = await ReadCoreAsync(new Memory<byte>(buffer, offset, IntEncoder.MaxEncodedSize));
                            int sizeOffset = offset;
                            DecodeSize(buffer, ref sizeOffset, ref bytesRead);
                        }
                    }
                }
            }

            public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            {
                if (buffer.IsEmpty)
                {
                    return new ValueTask<int>(0);
                }

                if (MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> array))
                {
                    return new ValueTask<int>(ReadAsync(array.Array!, array.Offset, array.Count, cancellationToken));
                }

                byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
                return FinishReadAsync(ReadAsync(sharedBuffer, 0, buffer.Length, cancellationToken), sharedBuffer, buffer);

                static async ValueTask<int> FinishReadAsync(Task<int> readTask, byte[] localBuffer, Memory<byte> localDestination)
                {
                    try
                    {
                        int result = await readTask.ConfigureAwait(false);
                        new ReadOnlySpan<byte>(localBuffer, 0, result).CopyTo(localDestination.Span);
                        return result;
                    }
                    finally
                    {
                        ArrayPool<byte>.Shared.Return(localBuffer);
                    }
                }
            }
        }
    }

    internal static class StreamingConnectionHelper
    {
        public async static Task WriteMessageAsync(Message message, IConnection connection, bool isRequest,
            IConnectionOrientedTransportFactorySettings settings, TimeoutHelper timeoutHelper)
        {
            Memory<byte> endBytes = default;
            if (message != null)
            {
                MessageEncoder messageEncoder = settings.MessageEncoderFactory.Encoder;
                Memory<byte> envelopeStartBytes = SingletonEncoder.EnvelopeStartBytes;

                bool writeStreamed;
                if (isRequest)
                {
                    endBytes = SingletonEncoder.EnvelopeEndFramingEndBytes;
                    writeStreamed = TransferModeHelper.IsRequestStreamed(settings.TransferMode);
                }
                else
                {
                    endBytes = SingletonEncoder.EnvelopeEndBytes;
                    writeStreamed = TransferModeHelper.IsResponseStreamed(settings.TransferMode);
                }

                if (writeStreamed)
                {
                    await connection.WriteAsync(envelopeStartBytes, false, timeoutHelper.RemainingTime());
                    Stream connectionStream = new StreamingOutputConnectionStream(connection, settings);
                    Stream writeTimeoutStream = new TimeoutStream(connectionStream, timeoutHelper.RemainingTime());
                    await messageEncoder.WriteMessageAsync(message, writeTimeoutStream);
                }
                else
                {
                    ArraySegment<byte> messageData = await messageEncoder.WriteMessageAsync(message,
                        int.MaxValue, settings.BufferManager, envelopeStartBytes.Length + IntEncoder.MaxEncodedSize);
                    messageData = SingletonEncoder.EncodeMessageFrame(messageData);
                    envelopeStartBytes.CopyTo(new Memory<byte>(messageData.Array, messageData.Offset - envelopeStartBytes.Length, envelopeStartBytes.Length));
                    await connection.WriteAsync(new Memory<byte>(messageData.Array, messageData.Offset - envelopeStartBytes.Length,
                        messageData.Count + envelopeStartBytes.Length), true, timeoutHelper.RemainingTime());
                }
            }
            else if (isRequest) // context handles response end bytes
            {
                endBytes = SingletonEncoder.EndBytes;
            }

            if (!endBytes.IsEmpty)
            {
                await connection.WriteAsync(endBytes,
                    true, timeoutHelper.RemainingTime());
            }
        }

        // overrides ConnectionStream to add a Framing int at the beginning of each record
        private class StreamingOutputConnectionStream : ConnectionStream
        {
            private Memory<byte> _encodedSize;

            public StreamingOutputConnectionStream(IConnection connection, IDefaultCommunicationTimeouts timeouts)
                : base(connection, timeouts)
            {
                _encodedSize = new byte[IntEncoder.MaxEncodedSize];
            }

            private ValueTask WriteChunkSizeAsync(int size)
            {
                if (size > 0)
                {
                    int bytesEncoded = IntEncoder.Encode(size, _encodedSize);
                    return Connection.WriteAsync(_encodedSize.Slice(0, bytesEncoded), false, TimeSpan.FromMilliseconds(WriteTimeout));
                }
                else
                {
                    return default;
                }
            }

            public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
            {
                await WriteChunkSizeAsync(count);
                await base.WriteAsync(buffer, offset, count, cancellationToken);
            }

            public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
            {
                await WriteChunkSizeAsync(buffer.Length);
                await base.WriteAsync(buffer, cancellationToken);
            }

            public override void WriteByte(byte value)
            {
                // We're write the chunk size as a variable length integer before we write the data. For a single byte
                // the length will always be 1 and that's encoded as 0x01 so no need to call WriteChunkSizeAsync. We
                // only need to send 2 bytes so can use _encodedSize to save an extra allocation.
                _encodedSize.Span[0] = 0x01;
                _encodedSize.Span[1] = value;
                base.WriteAsync(_encodedSize.Slice(0, 2)).GetAwaiter().GetResult();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                // Do NOT call base.WriteAsync as the chunk size must be written first.
                WriteAsync(new Memory<byte>(buffer, offset, count)).GetAwaiter().GetResult();
            }
        }
    }

}
