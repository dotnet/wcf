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

        public override void Flush()
        {
            _dataAvail.TrySetResult(0);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
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
            Contract.EndContractBlock();

            if (_disposed)
            {
                return 0;
            }

            _buffer.SetResult(new WriteBufferWrapper(buffer, offset, count));
            int totalBytesRead = 0;
            do
            {
                int bytesRead = _dataAvail.Task.Result;
                _dataAvail = new TaskCompletionSource<int>();
                totalBytesRead += bytesRead;
                count -= bytesRead;
                if (bytesRead == 0)
                {
                    break;
                }
            } while (count > 0);
            return totalBytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
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
            Contract.EndContractBlock();

            if (_disposed)
            {
                throw new ObjectDisposedException("ProducerConsumerStream");
            }

            while (count > 0)
            {
                if (_currentBuffer == WriteBufferWrapper.EmptyContainer)
                {
                    _currentBuffer = _buffer.Task.Result;
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

        public override bool CanRead
        {
            get { return !_disposed; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return !_disposed; }
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
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
            Contract.EndContractBlock();

            if (_disposed)
            {
                throw new ObjectDisposedException("ProducerConsumerStream");
            }

            while (count > 0)
            {
                if (_currentBuffer == WriteBufferWrapper.EmptyContainer)
                {
                    _currentBuffer = await _buffer.Task;
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
            if (count < 0 || count > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            Contract.EndContractBlock();

            if (_disposed)
            {
                return 0;
            }

            _buffer.TrySetResult(new WriteBufferWrapper(buffer, offset, count));
            int totalBytesRead = 0;
            do
            {
                int bytesRead = await _dataAvail.Task;
                _dataAvail = new TaskCompletionSource<int>();
                if (bytesRead == 0)
                {
                    break;
                }

                totalBytesRead += bytesRead;
                count -= bytesRead;
            } while (count > 0);

            return totalBytesRead;
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

        private struct WriteBufferWrapper : IEquatable<WriteBufferWrapper>
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
                set
                {
                    if (_buffer == value)
                    {
                        return;
                    }
                    if (_readOnly)
                    {
                        throw new ArgumentException("value");
                    }
                    _buffer = value;
                }
            }

            public int Offset
            {
                get { return _offset; }
                set
                {
                    if (_offset == value)
                    {
                        return;
                    }
                    if (_readOnly)
                    {
                        throw new ArgumentException("value");
                    }
                    _offset = value;
                }
            }

            public int Count
            {
                get { return _count; }
                set
                {
                    if (_count == value)
                    {
                        return;
                    }
                    if (_readOnly)
                    {
                        throw new ArgumentException("value");
                    }
                    _count = value;
                }
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
                    throw new InvalidOperationException("BufferContainer.Write");
                }

                // TODO: Parameter validation
                int bytesToCopy = Math.Min(_count, srcCount);
                Buffer.BlockCopy(srcBuffer, srcOffset, _buffer, _offset, bytesToCopy);
                _offset += bytesToCopy;
                _count -= bytesToCopy;
                return bytesToCopy;
            }
        }
    }
}
