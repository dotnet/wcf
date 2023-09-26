// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    public interface IConnection
    {
        int ConnectionBufferSize { get; }
        ValueTask<int> ReadAsync(Memory<byte> buffer, TimeSpan timeout);
        ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, bool immediate, TimeSpan timeout);
        void Abort();
        ValueTask CloseAsync(TimeSpan timeout);
    }

    public interface IConnectionInitiator
    {
        ValueTask<IConnection> ConnectAsync(Uri uri, TimeSpan timeout);
    }

    internal abstract class DelegatingConnection : IConnection
    {
        protected DelegatingConnection(IConnection connection)
        {
            Connection = connection;
        }

        protected IConnection Connection { get; }
        public virtual int ConnectionBufferSize => Connection.ConnectionBufferSize;
        public virtual void Abort() => Connection.Abort();
        public virtual ValueTask<int> ReadAsync(Memory<byte> buffer, TimeSpan timeout) => Connection.ReadAsync(buffer, timeout);
        public virtual ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, bool immediate, TimeSpan timeout) => Connection.WriteAsync(buffer, immediate, timeout);
        public virtual ValueTask CloseAsync(TimeSpan timeout) => Connection.CloseAsync(timeout);
    }

    internal class PreReadConnection : DelegatingConnection
    {
        private Memory<byte> _preReadData;

        public PreReadConnection(IConnection innerConnection, Memory<byte> initialData)
            : base(innerConnection)
        {
            _preReadData = initialData;
        }

        public void AddPreReadData(Memory<byte> initialData)
        {
            if (!_preReadData.IsEmpty)
            {
                Memory<byte> tempBuffer = _preReadData;
                _preReadData = Fx.AllocateByteArray(initialData.Length + _preReadData.Length);
                tempBuffer.CopyTo(_preReadData);
                initialData.CopyTo(_preReadData.Slice(tempBuffer.Length));
            }
            else
            {
                _preReadData = initialData;
            }
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, TimeSpan timeout)
        {
            if (!_preReadData.IsEmpty)
            {
                int bytesToCopy = Math.Min(buffer.Length, _preReadData.Length);
                _preReadData.Slice(0, bytesToCopy).CopyTo(buffer);
                _preReadData = _preReadData.Slice(bytesToCopy);
                return ValueTask.FromResult(bytesToCopy);
            }

            return base.ReadAsync(buffer, timeout);
        }
    }

    internal class ConnectionStream : Stream
    {
        private int _readTimeout;
        private int _writeTimeout;

        public ConnectionStream(IConnection connection, IDefaultCommunicationTimeouts defaultTimeouts)
        {
            Connection = connection;
            CloseTimeout = defaultTimeouts.CloseTimeout;
            ReadTimeout = TimeoutHelper.ToMilliseconds(defaultTimeouts.ReceiveTimeout);
            WriteTimeout = TimeoutHelper.ToMilliseconds(defaultTimeouts.SendTimeout);
            Immediate = true;
        }

        public IConnection Connection { get; }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanTimeout
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public TimeSpan CloseTimeout { get; set; }

        public override int ReadTimeout
        {
            get { return _readTimeout; }
            set
            {
                if (value < -1)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SR.Format(SR.ValueMustBeInRange, -1, int.MaxValue)));
                }

                _readTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get { return _writeTimeout; }
            set
            {
                if (value < -1)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SR.Format(SR.ValueMustBeInRange, -1, int.MaxValue)));
                }

                _writeTimeout = value;
            }
        }

        public bool Immediate { get; set; }

        public override long Length
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.SPS_SeekNotSupported));
            }
        }

        public override long Position
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.SPS_SeekNotSupported));
            }
            set
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.SPS_SeekNotSupported));
            }
        }


        public void Abort()
        {
            Connection.Abort();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Connection.CloseAsync(CloseTimeout).GetAwaiter().GetResult();
            }
        }

        public override void Flush()
        {
            // NOP
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return Connection.WriteAsync(new Memory<byte>(buffer, offset, count), Immediate, TimeoutHelper.FromMilliseconds(WriteTimeout)).AsTask();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Connection.WriteAsync(new Memory<byte>(buffer, offset, count), Immediate, TimeoutHelper.FromMilliseconds(WriteTimeout)).GetAwaiter().GetResult();
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            Memory<byte> memBuffer = new Memory<byte>(sharedBuffer, 0, buffer.Length);
            try
            {
                buffer.CopyTo(memBuffer.Span);
                Connection.WriteAsync(memBuffer, Immediate, TimeoutHelper.FromMilliseconds(WriteTimeout)).GetAwaiter().GetResult();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return Connection.WriteAsync(buffer, Immediate, TimeoutHelper.FromMilliseconds(WriteTimeout));
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return Connection.WriteAsync(new Memory<byte>(buffer, offset, count), Immediate, TimeoutHelper.FromMilliseconds(WriteTimeout))
                .AsTask()
                .ToApm(callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult) => asyncResult.ToApmEnd();

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return Connection.ReadAsync(new Memory<byte>(buffer, offset, count), TimeoutHelper.FromMilliseconds(ReadTimeout)).AsTask();
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return Connection.ReadAsync(buffer, TimeoutHelper.FromMilliseconds(ReadTimeout));
        }

        public override int Read(Span<byte> buffer)
        {
            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            Memory<byte> memBuffer = new Memory<byte>(sharedBuffer, 0, buffer.Length);
            int bytesRead = 0;
            try
            {
                bytesRead = Connection.ReadAsync(memBuffer, TimeoutHelper.FromMilliseconds(ReadTimeout)).GetAwaiter().GetResult();
                memBuffer.Slice(0, bytesRead).Span.CopyTo(buffer);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }

            return bytesRead;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Connection.ReadAsync(new Memory<byte>(buffer, offset, count), TimeoutHelper.FromMilliseconds(ReadTimeout)).GetAwaiter().GetResult();
        }

        protected int Read(byte[] buffer, int offset, int count, TimeSpan timeout)
        {
            return Connection.ReadAsync(new Memory<byte>(buffer, offset, count), timeout).GetAwaiter().GetResult();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.SPS_SeekNotSupported));
        }

        public override void SetLength(long value)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.SPS_SeekNotSupported));
        }
    }

    internal class StreamConnection : IConnection
    {
        private ConnectionStream _innerStream;

        public StreamConnection(Stream stream, ConnectionStream innerStream)
        {
            Contract.Assert(stream != null, "StreamConnection: Stream cannot be null.");
            Contract.Assert(innerStream != null, "StreamConnection: Inner stream cannot be null.");

            Stream = stream;
            _innerStream = innerStream;
        }

        public async ValueTask<int> ReadAsync(Memory<byte> buffer, TimeSpan timeout)
        {
            try
            {
                SetReadTimeout(timeout);
                return await Stream.ReadAsync(buffer);
            }
            catch (IOException ioException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertIOException(ioException));
            }
        }

        public async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, bool immediate, TimeSpan timeout)
        {
            try
            {
                _innerStream.Immediate = immediate;
                SetWriteTimeout(timeout);
                await Stream.WriteAsync(buffer);
            }
            catch (IOException ioException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertIOException(ioException));
            }

        }

        public async ValueTask CloseAsync(TimeSpan timeout)
        {
            _innerStream.CloseTimeout = timeout;
            try
            {
                await Stream.DisposeAsync();
            }
            catch (IOException ioException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertIOException(ioException));
            }
        }

        public Stream Stream { get; }

        public object ThisLock => this;

        public int ConnectionBufferSize => _innerStream.Connection.ConnectionBufferSize;

        public void Abort()
        {
            _innerStream.Abort();
        }

        private Exception ConvertIOException(IOException ioException)
        {
            if (ioException.InnerException is TimeoutException)
            {
                return new TimeoutException(ioException.InnerException.Message, ioException);
            }
            else if (ioException.InnerException is CommunicationObjectAbortedException)
            {
                return new CommunicationObjectAbortedException(ioException.InnerException.Message, ioException);
            }
            else if (ioException.InnerException is CommunicationException)
            {
                return new CommunicationException(ioException.InnerException.Message, ioException);
            }
            else
            {
                return new CommunicationException(SR.StreamError, ioException);
            }
        }

        public void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout)
        {
            try
            {
                _innerStream.Immediate = immediate;
                SetWriteTimeout(timeout);
                Stream.Write(buffer, offset, size);
            }
            catch (IOException ioException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertIOException(ioException));
            }
        }

        public void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, BufferManager bufferManager)
        {
            Write(buffer, offset, size, immediate, timeout);
            bufferManager.ReturnBuffer(buffer);
        }

        private void SetReadTimeout(TimeSpan timeout)
        {
            int timeoutInMilliseconds = TimeoutHelper.ToMilliseconds(timeout);
            if (Stream.CanTimeout)
            {
                Stream.ReadTimeout = timeoutInMilliseconds;
            }
            _innerStream.ReadTimeout = timeoutInMilliseconds;
        }

        private void SetWriteTimeout(TimeSpan timeout)
        {
            int timeoutInMilliseconds = TimeoutHelper.ToMilliseconds(timeout);
            if (Stream.CanTimeout)
            {
                Stream.WriteTimeout = timeoutInMilliseconds;
            }
            _innerStream.WriteTimeout = timeoutInMilliseconds;
        }

        public int Read(byte[] buffer, int offset, int size, TimeSpan timeout)
        {
            try
            {
                SetReadTimeout(timeout);
                return Stream.Read(buffer, offset, size);
            }
            catch (IOException ioException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertIOException(ioException));
            }
        }
    }
}
