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
            int bytesWrittenToBuffer = _currentBuffer.BytesWritten;
            _currentBuffer = WriteBufferWrapper.EmptyContainer;
            _dataAvail.TrySetResult(bytesWrittenToBuffer);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count == 0) return 0;

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
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (count < 0 || count > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (count == 0) return 0;

            // _dataAvail must be set before the disposed check to avoid a race condition
            // when Dispose is called just after the _disposed check which would result in
            // ReadAsync never completing.
            _dataAvail = new TaskCompletionSource<int>();

            if (_disposed)
            {
                return 0;
            }

            using (cancellationToken.Register(CancelAndDispose, this))
            {
                Contract.Assert(!_buffer.Task.IsCompleted, "Buffer task should not already be completed");
                Contract.Assert(_currentBuffer == WriteBufferWrapper.EmptyContainer, "The current buffer should be the EmptyContainer");
                _buffer.TrySetResult(new WriteBufferWrapper(buffer, offset, count));
                return await _dataAvail.Task;
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
                        Flush();
                    }
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
                Flush();
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
            private int _count;
            private readonly bool _readOnly;

            public WriteBufferWrapper(byte[] buffer, int offset, int count) : this(buffer, offset, count, false) { }
            private WriteBufferWrapper(byte[] buffer, int offset, int count, bool isReadOnly) : this()
            {
                ByteBuffer = buffer;
                Offset = offset;
                _count = count;
                _readOnly = isReadOnly;
            }

            public byte[] ByteBuffer { get; }

            public int Offset { get; private set; }

            public int Count
            {
                get { return _count; }
            }

            public int BytesWritten { get; private set; }

            public override bool Equals(object obj)
            {
                if (obj is WriteBufferWrapper)
                {
                    return Equals((WriteBufferWrapper)obj);
                }

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

                if (other.ByteBuffer == ByteBuffer && other.Offset == Offset)
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.ObjectIsReadOnly));
                }

                int bytesToCopy = Math.Min(_count, srcCount);
                Buffer.BlockCopy(srcBuffer, srcOffset, ByteBuffer, Offset, bytesToCopy);
                Offset += bytesToCopy;
                _count -= bytesToCopy;
                BytesWritten += bytesToCopy;
                return bytesToCopy;
            }
        }
    }
}
