// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class ReadAheadWrappingStream : DelegatingStream
    {
        public const string ReadAheadWrappingStreamPropertyName = "ReadAheadWrappingStreamProperty";
        private readonly int _readAheadBytes;
        private readonly ArraySegment<byte>[] _buffers;
        private Task<int> _previousCompletedReadTask;
        private Task<int> _previousCompletedEnsureBufferedTask;
        private bool _eof = false;

        public ReadAheadWrappingStream(Stream innerStream, int readAheadBytes) : base(innerStream)
        {
            _readAheadBytes = readAheadBytes;
            _buffers = new ArraySegment<byte>[2];
            _buffers[0] = new ArraySegment<byte>(new byte[_readAheadBytes], 0, 0);
            _buffers[1] = new ArraySegment<byte>(new byte[_readAheadBytes], 0, 0);
            _previousCompletedReadTask = _previousCompletedEnsureBufferedTask = Task.FromResult(readAheadBytes);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Contract.Assert(_previousCompletedReadTask.IsCompleted, "Calling ReadAsync multiple times concurrently");
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<int>(cancellationToken);
            }

            if (_buffers[0].Count == 0 && _buffers[1].Count == 0)
            {
                if (_eof) // We've reached the end of the stream
                {
                    return Task.FromResult(0);
                }

                _previousCompletedReadTask = base.ReadAsync(buffer, offset, count, cancellationToken);
                return _previousCompletedReadTask;
            }

            int bytesRead = ReadFromBuffer(buffer, offset, count);
            if (bytesRead < count)
            {
                _previousCompletedReadTask = ReadAsyncAfterReadFromBuffer(bytesRead, buffer, offset, count, cancellationToken);
                return _previousCompletedReadTask;
            }

            if (_previousCompletedReadTask.Result != bytesRead)
            {
                _previousCompletedReadTask = Task.FromResult(bytesRead);
            }

            return _previousCompletedReadTask;
        }

        private async Task<int> ReadAsyncAfterReadFromBuffer(int bytesRead, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            // The parameters count and offset are the original values passed to ReadAsync.
            // bytesRead is how many bytes were already filled from the buffer. When reading
            // from the inner stream, we need to adjust offset and count by that number of bytes
            // and then add the previously read count to what was read by ReadAsync.
            bytesRead += await base.ReadAsync(buffer, offset + bytesRead, count - bytesRead, cancellationToken);
            return bytesRead;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (EnsureBuffered(count) == 0)
            {
                if (_eof) // We've reached the end of the stream
                {
                    return 0;
                }

                return base.Read(buffer, offset, count);
            }

            int bytesRead = ReadFromBuffer(buffer, offset, count);
            if (bytesRead < count)
            {
                int extraBytes = base.Read(buffer, offset + bytesRead, count - bytesRead);
                bytesRead += extraBytes;
                if (extraBytes == 0)
                {
                    _eof = true;
                }
            }

            return bytesRead;
        }

        public override int ReadByte()
        {
            if (EnsureBuffered(1) == 0)
            {
                if (_eof) // We've reached the end of the stream
                {
                    return -1;
                }

                return base.ReadByte();
            }

            int retVal = _buffers[0].Array[_buffers[0].Offset];
            UpdateBuffers(1);
            return retVal;
        }

        private int ReadFromBuffer(byte[] buffer, int offset, int count)
        {
            int readBytes;
            int bytesToCopy = readBytes = Math.Min(count, _buffers[0].Count);
            Buffer.BlockCopy(_buffers[0].Array, _buffers[0].Offset, buffer, offset, bytesToCopy);
            UpdateBuffers(bytesToCopy);
            if (count > readBytes)
            {
                bytesToCopy = Math.Min(count - readBytes, _buffers[0].Count);
                Buffer.BlockCopy(_buffers[0].Array, _buffers[0].Offset, buffer, offset + readBytes, bytesToCopy);
                UpdateBuffers(bytesToCopy);
                readBytes += bytesToCopy;
            }

            return readBytes;
        }

        private void UpdateBuffers(int bytesCopied)
        {
            var bytesLeft = _buffers[0].Count - bytesCopied;
            if (bytesLeft == 0)
            {
                var oldArray = _buffers[0].Array;
                _buffers[0] = _buffers[1];
                _buffers[1] = new ArraySegment<byte>(oldArray, 0, 0);
            }
            else
            {
                _buffers[0] = new ArraySegment<byte>(_buffers[0].Array, _buffers[0].Offset + bytesCopied, bytesLeft);
            }
        }

        public Task<int> EnsureBufferedAsync(CancellationToken cancellationToken)
        {
            return EnsureBufferedAsync(_readAheadBytes*2, cancellationToken);
        }

        public Task<int> EnsureBufferedAsync(int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<int>(cancellationToken);
            }

            int availableBuffer = _buffers[0].Count + _buffers[1].Count;

            // If there's enough data in the buffers, we can complete the anticipated read.
            // If there is still data in the second buffer, there's no point trying
            // to get more data as there isn't anywhere to put it. In this case avoid
            // the overhead of running an async method.
            // If we've reached the end of the stream, there's no point trying to read any more.
            if (count <= availableBuffer || _buffers[1].Count > 0 || _eof)
            {
                int returnValue = Math.Min(availableBuffer, count);
                if (_previousCompletedEnsureBufferedTask.Result != returnValue)
                {
                    _previousCompletedEnsureBufferedTask = Task.FromResult(count);
                }

                return _previousCompletedEnsureBufferedTask;
            }

            // The anticipated read needs more data than is currently buffered and
            // the second buffer is empty. Even if the future read can't be fully
            // satisfied after the second buffer is filled, as much data should be
            // buffered as possible to reduce the time a synchronous read might take
            // to complete.
            _previousCompletedEnsureBufferedTask = EnsureBufferedAsyncCore(count, cancellationToken);
            return _previousCompletedEnsureBufferedTask;
        }

        private async Task<int> EnsureBufferedAsyncCore(int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            for (int i = 0; i < 2; i++)
            {
                if (_buffers[i].Count > 0)
                {
                    continue;
                }

                byte[] buffer = _buffers[i].Array;
                int bufferCount = buffer.Length;
                int bytesRead = await base.ReadAsync(buffer, 0, bufferCount, cancellationToken);
                if (bytesRead == 0)
                {
                    _eof = true;
                    break;
                }

                _buffers[i] = new ArraySegment<byte>(buffer, 0, bytesRead);
                if (bytesRead < bufferCount)
                {
                    // If this is the first buffer and we didn't fill it, then the second
                    // ReadAsync call will block waiting for the next bytes. With Net.Tcp,
                    // the stream doesn't end when the message ends so we can't leave a
                    // pending ReadAsync otherwise we will consume bytes meant for the next
                    // message.
                    break;
                }
            }

            return Math.Min(_buffers[0].Count + _buffers[1].Count, count);
        }

        public int EnsureBuffered()
        {
            // The EnsureBuffered method is synchronous, and the reason for this class is to avoid calling
            // Read synchronously whenever possible. This means only one buffer is ever fetched using a single
            // base.Read() call, presuming that any data needed beyond one buffer will be read Asynchronously.
            // This means the default amount to ensure is available should only be a single buffer.
            return EnsureBuffered(_readAheadBytes);
        }

        public int EnsureBuffered(int count)
        {
            int availableBuffer = _buffers[0].Count + _buffers[1].Count;

            // If there's enough data in the buffers, we can complete the anticipated read.
            if (count <= availableBuffer)
            {
                return count;
            }

            if (_eof) // No more data can be read
            {
                return availableBuffer;
            }

            // We only want to do a single synchronous Read as it can cause an async read
            // to happen on the inner stream and block waiting on this thread for it to 
            // complete. We want to avoid this happening, so presume that we only need to
            // do the single sync read as all other reads hopefully will be async.
            int bufferIndex = -1;
            if (_buffers[0].Count == 0)
            {
                bufferIndex = 0;
            }
            else if (_buffers[1].Count == 0)
            {
                bufferIndex = 1;
            }

            // We are here because we didn't have enough data buffered for the request. Data
            // can only be fetched if there's a buffer with space for more data, otherwise we
            // can only return what we have so far.
            if (bufferIndex >= 0)
            {
                byte[] buffer = _buffers[bufferIndex].Array;
                int bufferCount = buffer.Length;
                int bytesRead = base.Read(buffer, 0, bufferCount);
                if (bytesRead == 0)
                {
                    _eof = true;
                }

                _buffers[bufferIndex] = new ArraySegment<byte>(buffer, 0, bytesRead);
            }

            availableBuffer = _buffers[0].Count + _buffers[1].Count;
            return Math.Min(count, availableBuffer);
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override long Position
        {
            get
            {
                // The inner stream might be ahead of where expected because some
                // data is in the buffer.
                return base.Position - (_buffers[0].Count + _buffers[1].Count);
            }
            set
            {
                throw Fx.Exception.AsError(new NotSupportedException(InternalSR.SeekNotSupported));
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw Fx.Exception.AsError(new NotSupportedException(InternalSR.SeekNotSupported));
        }
    }
}