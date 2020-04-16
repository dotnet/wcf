// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class ProducerConsumerStream : Stream
    {
        private TaskCompletionSource<WriteBufferWrapper> _buffer;
        private TaskCompletionSource<int> _dataAvail;
        private WriteBufferWrapper _currentBuffer;
        private bool _disposed;

        public ProducerConsumerStream()
        {
            _buffer = new TaskCompletionSource<WriteBufferWrapper>();
            _dataAvail = new TaskCompletionSource<int>();
            _currentBuffer = WriteBufferWrapper.EmptyContainer;
        }

        public override bool CanRead { get { return !_disposed; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return !_disposed; } }

        public override void Flush()
        {
            _dataAvail.TrySetResult(0);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                return ReadAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException oce)
            {
                // If WriteAsync is canceled, an OperationCanceledException might get thrown on the Read path.
                // The stream is no longer usable so converting to an ObjectDisposedException.
                throw new ObjectDisposedException("ProducerConsumerStream", oce);
            }
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (count <= 0 || count > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (_disposed)
            {
                return 0;
            }

            using (cancellationToken.Register(CancelAndDispose, this))
            {
                _buffer.TrySetResult(new WriteBufferWrapper(buffer, offset, count));
                int totalBytesRead = 0;
                while (count > 0)
                {
                    int bytesRead = await _dataAvail.Task;
                    _dataAvail = new TaskCompletionSource<int>();
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    totalBytesRead += bytesRead;
                    count -= bytesRead;
                }

                return totalBytesRead;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                WriteAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException oce)
            {
                // If ReadAsync is canceled, an OperationCanceledException might get thrown on the write path.
                // The stream is no longer usable so converting to an ObjectDisposedException.
                throw new ObjectDisposedException("ProducerConsumerStream", oce);
            }
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (count < 0 || count > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (_disposed)
            {
                throw new ObjectDisposedException("ProducerConsumerStream");
            }

            using (cancellationToken.Register(CancelAndDispose, this))
            {
                while (count > 0)
                {
                    if (_currentBuffer == WriteBufferWrapper.EmptyContainer)
                    {
                        _currentBuffer = await _buffer.Task;
                        _buffer = new TaskCompletionSource<WriteBufferWrapper>();
                    }

                    int bytesWritten = _currentBuffer.Write(buffer, offset, count);
                    count -= bytesWritten;
                    offset += bytesWritten;
                    if (_currentBuffer.Count == 0)
                    {
                        _currentBuffer = WriteBufferWrapper.EmptyContainer;
                    }

                    _dataAvail.TrySetResult(bytesWritten);
                }
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                _dataAvail.TrySetResult(0);
            }

            base.Dispose(disposing);
        }

        private static void CancelAndDispose(object state)
        {
            var thisPtr = state as ProducerConsumerStream;
            if (thisPtr != null)
            {
                thisPtr._dataAvail.TrySetCanceled();
                thisPtr._buffer.TrySetCanceled();
                thisPtr.Dispose();
            }
        }

#pragma warning disable 659,661  // Hashcode not needed, Equals overriden for fast path comparison of EmptyContainer
        private struct WriteBufferWrapper : IEquatable<WriteBufferWrapper>
#pragma warning restore 659,661
        {
            public static readonly WriteBufferWrapper EmptyContainer = new WriteBufferWrapper(null, -1, -1, true);

            private byte[] _buffer;
            private int _offset;
            private int _count;
            private readonly bool _readOnly;

            public WriteBufferWrapper(byte[] buffer, int offset, int count) : this(buffer, offset, count, false) { }
            private WriteBufferWrapper(byte[] buffer, int offset, int count, bool isReadOnly) : this()
            {
                _buffer = buffer;
                _offset = offset;
                _count = count;
                _readOnly = isReadOnly;
            }

            public byte[] ByteBuffer
            {
                get { return _buffer; }
            }

            public int Offset
            {
                get { return _offset; }
            }

            public int Count
            {
                get { return _count; }
            }

            public override bool Equals(object obj)
            {
                if (obj is WriteBufferWrapper)
                    return this.Equals((WriteBufferWrapper)obj);
                return false;
            }

            public bool Equals(WriteBufferWrapper other)
            {
                // Doing _readOnly comparison first as main use case is comparing against
                // BufferContainer.EmptyContainer which is the only readonly instance of BufferContainer.
                // If either instance is the read only, then this field is sufficient for comparison.
                if (_readOnly || other._readOnly)
                {
                    return _readOnly == other._readOnly;
                }

                if (other._buffer == _buffer && other._offset == _offset)
                {
                    return other._count == _count;
                }

                return false;
            }

            public static bool operator ==(WriteBufferWrapper a, WriteBufferWrapper b)
            {
                return a.Equals(b);
            }

            public static bool operator !=(WriteBufferWrapper a, WriteBufferWrapper b)
            {
                return !(a == b);
            }

            internal int Write(byte[] srcBuffer, int srcOffset, int srcCount)
            {
                if (_readOnly)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
                }

                int bytesToCopy = Math.Min(_count, srcCount);
                Buffer.BlockCopy(srcBuffer, srcOffset, _buffer, _offset, bytesToCopy);
                _offset += bytesToCopy;
                _count -= bytesToCopy;
                return bytesToCopy;
            }
        }
    }
}
