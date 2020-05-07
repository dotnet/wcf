// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    // Low level abstraction for a socket/pipe
    public interface IConnection
    {
        byte[] AsyncReadBuffer { get; }
        int AsyncReadBufferSize { get; }

        void Abort();
        void Close(TimeSpan timeout, bool asyncAndLinger);

        AsyncCompletionResult BeginWrite(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout,
            Action<object> callback, object state);
        void EndWrite();
        void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout);
        void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, BufferManager bufferManager);

        int Read(byte[] buffer, int offset, int size, TimeSpan timeout);
        AsyncCompletionResult BeginRead(int offset, int size, TimeSpan timeout, Action<object> callback, object state);
        int EndRead();
    }

    // Low level abstraction for connecting a socket/pipe
    public interface IConnectionInitiator
    {
        IConnection Connect(Uri uri, TimeSpan timeout);
        Task<IConnection> ConnectAsync(Uri uri, TimeSpan timeout);
    }

    internal abstract class DelegatingConnection : IConnection
    {
        private IConnection _connection;

        protected DelegatingConnection(IConnection connection)
        {
            _connection = connection;
        }

        public virtual byte[] AsyncReadBuffer
        {
            get { return _connection.AsyncReadBuffer; }
        }

        public virtual int AsyncReadBufferSize
        {
            get { return _connection.AsyncReadBufferSize; }
        }


        protected IConnection Connection
        {
            get { return _connection; }
        }

        public virtual void Abort()
        {
            _connection.Abort();
        }

        public virtual void Close(TimeSpan timeout, bool asyncAndLinger)
        {
            _connection.Close(timeout, asyncAndLinger);
        }

        public virtual AsyncCompletionResult BeginWrite(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout,
            Action<object> callback, object state)
        {
            return _connection.BeginWrite(buffer, offset, size, immediate, timeout, callback, state);
        }

        public virtual void EndWrite()
        {
            _connection.EndWrite();
        }

        public virtual void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout)
        {
            _connection.Write(buffer, offset, size, immediate, timeout);
        }

        public virtual void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, BufferManager bufferManager)
        {
            _connection.Write(buffer, offset, size, immediate, timeout, bufferManager);
        }

        public virtual int Read(byte[] buffer, int offset, int size, TimeSpan timeout)
        {
            return _connection.Read(buffer, offset, size, timeout);
        }

        public virtual AsyncCompletionResult BeginRead(int offset, int size, TimeSpan timeout,
            Action<object> callback, object state)
        {
            return _connection.BeginRead(offset, size, timeout, callback, state);
        }

        public virtual int EndRead()
        {
            return _connection.EndRead();
        }
    }

    internal class PreReadConnection : DelegatingConnection
    {
        private int _asyncBytesRead;
        private byte[] _preReadData;
        private int _preReadOffset;
        private int _preReadCount;

        public PreReadConnection(IConnection innerConnection, byte[] initialData)
            : this(innerConnection, initialData, 0, initialData.Length)
        {
        }

        public PreReadConnection(IConnection innerConnection, byte[] initialData, int initialOffset, int initialSize)
            : base(innerConnection)
        {
            _preReadData = initialData;
            _preReadOffset = initialOffset;
            _preReadCount = initialSize;
        }

        public void AddPreReadData(byte[] initialData, int initialOffset, int initialSize)
        {
            if (_preReadCount > 0)
            {
                byte[] tempBuffer = _preReadData;
                _preReadData = Fx.AllocateByteArray(initialSize + _preReadCount);
                Buffer.BlockCopy(tempBuffer, _preReadOffset, _preReadData, 0, _preReadCount);
                Buffer.BlockCopy(initialData, initialOffset, _preReadData, _preReadCount, initialSize);
                _preReadOffset = 0;
                _preReadCount += initialSize;
            }
            else
            {
                _preReadData = initialData;
                _preReadOffset = initialOffset;
                _preReadCount = initialSize;
            }
        }

        public override int Read(byte[] buffer, int offset, int size, TimeSpan timeout)
        {
            ConnectionUtilities.ValidateBufferBounds(buffer, offset, size);

            if (_preReadCount > 0)
            {
                int bytesToCopy = Math.Min(size, _preReadCount);
                Buffer.BlockCopy(_preReadData, _preReadOffset, buffer, offset, bytesToCopy);
                _preReadOffset += bytesToCopy;
                _preReadCount -= bytesToCopy;
                return bytesToCopy;
            }

            return base.Read(buffer, offset, size, timeout);
        }

        public override AsyncCompletionResult BeginRead(int offset, int size, TimeSpan timeout, Action<object> callback, object state)
        {
            ConnectionUtilities.ValidateBufferBounds(AsyncReadBufferSize, offset, size);

            if (_preReadCount > 0)
            {
                int bytesToCopy = Math.Min(size, _preReadCount);
                Buffer.BlockCopy(_preReadData, _preReadOffset, AsyncReadBuffer, offset, bytesToCopy);
                _preReadOffset += bytesToCopy;
                _preReadCount -= bytesToCopy;
                _asyncBytesRead = bytesToCopy;
                return AsyncCompletionResult.Completed;
            }

            return base.BeginRead(offset, size, timeout, callback, state);
        }

        public override int EndRead()
        {
            if (_asyncBytesRead > 0)
            {
                int retValue = _asyncBytesRead;
                _asyncBytesRead = 0;
                return retValue;
            }

            return base.EndRead();
        }
    }

    internal class ConnectionStream : Stream
    {
        private TimeSpan _closeTimeout;
        private int _readTimeout;
        private int _writeTimeout;
        private IConnection _connection;
        private bool _immediate;
        private static Action<object> s_onWriteComplete = new Action<object>(OnWriteComplete);
        private static Action<object> s_onReadComplete = new Action<object>(OnReadComplete);

        public ConnectionStream(IConnection connection, IDefaultCommunicationTimeouts defaultTimeouts)
        {
            _connection = connection;
            _closeTimeout = defaultTimeouts.CloseTimeout;
            this.ReadTimeout = TimeoutHelper.ToMilliseconds(defaultTimeouts.ReceiveTimeout);
            this.WriteTimeout = TimeoutHelper.ToMilliseconds(defaultTimeouts.SendTimeout);
            _immediate = true;
        }

        public IConnection Connection
        {
            get { return _connection; }
        }

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

        public TimeSpan CloseTimeout
        {
            get { return _closeTimeout; }
            set { _closeTimeout = value; }
        }

        public override int ReadTimeout
        {
            get { return _readTimeout; }
            set
            {
                if (value < -1)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        string.Format(SRServiceModel.ValueMustBeInRange, -1, int.MaxValue)));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        string.Format(SRServiceModel.ValueMustBeInRange, -1, int.MaxValue)));
                }

                _writeTimeout = value;
            }
        }

        public bool Immediate
        {
            get { return _immediate; }
            set { _immediate = value; }
        }

        public override long Length
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRServiceModel.SPS_SeekNotSupported));
            }
        }

        public override long Position
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRServiceModel.SPS_SeekNotSupported));
            }
            set
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRServiceModel.SPS_SeekNotSupported));
            }
        }


        public void Abort()
        {
            _connection.Abort();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection.Close(this.CloseTimeout, false);
            }
        }

        public override void Flush()
        {
            // NOP
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            // There's room for improvement here to avoid allocation in the synchronous completion case. We could store
            // the tcs in ConnectionStream and only allocate if the result is Queued. We would need to return a cached
            // completed Task in the success case to also avoid allocation. The race condition of completing async but 
            // running the callback before the tcs has been allocated would need to be accounted for.
            var tcs = new TaskCompletionSource<bool>(this);
            var asyncCompletionResult = _connection.BeginWrite(buffer, offset, count, this.Immediate,
                TimeoutHelper.FromMilliseconds(this.WriteTimeout), s_onWriteComplete, tcs);
            if (asyncCompletionResult == AsyncCompletionResult.Completed)
            {
                _connection.EndWrite();
                tcs.TrySetResult(true);
            }

            return tcs.Task;
        }

        private static void OnWriteComplete(object state)
        {
            if (state == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("state");
            }

            var tcs = state as TaskCompletionSource<bool>;
            if (tcs == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("state", SRServiceModel.SPS_InvalidAsyncResult);
            }

            var thisPtr = tcs.Task.AsyncState as ConnectionStream;
            if (thisPtr == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("state", SRServiceModel.SPS_InvalidAsyncResult);
            }

            try
            {
                thisPtr._connection.EndWrite();
                tcs.TrySetResult(true);
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _connection.Write(buffer, offset, count, this.Immediate, TimeoutHelper.FromMilliseconds(this.WriteTimeout));
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<int>(this);
            AsyncCompletionResult asyncCompletionResult = _connection.BeginRead(0, Math.Min(count, _connection.AsyncReadBufferSize),
                TimeoutHelper.FromMilliseconds(this.ReadTimeout), s_onReadComplete, tcs);

            if (asyncCompletionResult == AsyncCompletionResult.Completed)
            {
                tcs.TrySetResult(_connection.EndRead());
            }

            return tcs.Task;
        }

        private static void OnReadComplete(object state)
        {
            if (state == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("state");
            }

            var tcs = state as TaskCompletionSource<int>;
            if (tcs == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("state", SRServiceModel.SPS_InvalidAsyncResult);
            }

            var thisPtr = tcs.Task.AsyncState as ConnectionStream;
            if (thisPtr == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("state", SRServiceModel.SPS_InvalidAsyncResult);
            }

            try
            {
                tcs.TrySetResult(thisPtr._connection.EndRead());
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.Read(buffer, offset, count, TimeoutHelper.FromMilliseconds(this.ReadTimeout));
        }

        protected int Read(byte[] buffer, int offset, int count, TimeSpan timeout)
        {
            return _connection.Read(buffer, offset, count, timeout);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRServiceModel.SPS_SeekNotSupported));
        }


        public override void SetLength(long value)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRServiceModel.SPS_SeekNotSupported));
        }
    }

    internal class StreamConnection : IConnection
    {
        private byte[] _asyncReadBuffer;
        private int _bytesRead;
        private ConnectionStream _innerStream;
        private Action<Task<int>, object> _onRead;
        private Action<Task, object> _onWrite;
        private Task<int> _readResult;
        private Task _writeResult;
        private Action<object> _readCallback;
        private Action<object> _writeCallback;
        private Stream _stream;

        public StreamConnection(Stream stream, ConnectionStream innerStream)
        {
            Contract.Assert(stream != null, "StreamConnection: Stream cannot be null.");
            Contract.Assert(innerStream != null, "StreamConnection: Inner stream cannot be null.");

            _stream = stream;
            _innerStream = innerStream;

            _onRead = new Action<Task<int>, object>(OnRead);
            _onWrite = new Action<Task, object>(OnWrite);
        }

        public byte[] AsyncReadBuffer
        {
            get
            {
                if (_asyncReadBuffer == null)
                {
                    lock (ThisLock)
                    {
                        if (_asyncReadBuffer == null)
                        {
                            _asyncReadBuffer = Fx.AllocateByteArray(_innerStream.Connection.AsyncReadBufferSize);
                        }
                    }
                }

                return _asyncReadBuffer;
            }
        }

        public int AsyncReadBufferSize
        {
            get { return _innerStream.Connection.AsyncReadBufferSize; }
        }

        public Stream Stream
        {
            get { return _stream; }
        }

        public object ThisLock
        {
            get { return this; }
        }


        public IPEndPoint RemoteIPEndPoint
        {
            get
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
        }

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
                return new CommunicationException(SRServiceModel.StreamError, ioException);
            }
        }

        public void Close(TimeSpan timeout, bool asyncAndLinger)
        {
            _innerStream.CloseTimeout = timeout;
            try
            {
                _stream.Dispose();
            }
            catch (IOException ioException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertIOException(ioException));
            }
        }

        public AsyncCompletionResult BeginWrite(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout,
            Action<object> callback, object state)
        {
            Contract.Requires(callback != null, "Cannot call BeginWrite without a callback");
            Contract.Requires(_writeCallback == null, "BeginWrite cannot be called twice");

            _writeCallback = callback;
            bool throwing = true;

            try
            {
                _innerStream.Immediate = immediate;
                SetWriteTimeout(timeout);
                Task localTask = _stream.WriteAsync(buffer, offset, size);

                throwing = false;
                if (!localTask.IsCompleted)
                {
                    localTask.ContinueWith(_onWrite, state);
                    return AsyncCompletionResult.Queued;
                }

                localTask.GetAwaiter().GetResult();
            }
            catch (IOException ioException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertIOException(ioException));
            }
            finally
            {
                if (throwing)
                {
                    _writeCallback = null;
                }
            }

            return AsyncCompletionResult.Completed;
        }

        public void EndWrite()
        {
            Task localResult = _writeResult;
            _writeResult = null;
            _writeCallback = null;

            if (localResult != null)
            {
                try
                {
                    localResult.GetAwaiter().GetResult();
                }
                catch (IOException ioException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertIOException(ioException));
                }
            }
        }

        private void OnWrite(Task antecedant, Object state)
        {
            Contract.Requires(_writeResult == null, "StreamConnection: OnWrite called twice.");
            _writeResult = antecedant;
            _writeCallback(state);
        }

        public void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout)
        {
            try
            {
                _innerStream.Immediate = immediate;
                SetWriteTimeout(timeout);
                _stream.Write(buffer, offset, size);
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
            if (_stream.CanTimeout)
            {
                _stream.ReadTimeout = timeoutInMilliseconds;
            }
            _innerStream.ReadTimeout = timeoutInMilliseconds;
        }

        private void SetWriteTimeout(TimeSpan timeout)
        {
            int timeoutInMilliseconds = TimeoutHelper.ToMilliseconds(timeout);
            if (_stream.CanTimeout)
            {
                _stream.WriteTimeout = timeoutInMilliseconds;
            }
            _innerStream.WriteTimeout = timeoutInMilliseconds;
        }

        public int Read(byte[] buffer, int offset, int size, TimeSpan timeout)
        {
            try
            {
                SetReadTimeout(timeout);
                return _stream.Read(buffer, offset, size);
            }
            catch (IOException ioException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertIOException(ioException));
            }
        }

        public AsyncCompletionResult BeginRead(int offset, int size, TimeSpan timeout, Action<object> callback, object state)
        {
            ConnectionUtilities.ValidateBufferBounds(AsyncReadBufferSize, offset, size);
            _readCallback = callback;

            try
            {
                SetReadTimeout(timeout);
                Task<int> localTask = _stream.ReadAsync(AsyncReadBuffer, offset, size);
                //IAsyncResult localResult = stream.BeginRead(AsyncReadBuffer, offset, size, onRead, state);

                if (!localTask.IsCompleted)
                {
                    localTask.ContinueWith(_onRead, state);
                    return AsyncCompletionResult.Queued;
                }

                _bytesRead = localTask.GetAwaiter().GetResult();
            }
            catch (IOException ioException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertIOException(ioException));
            }

            return AsyncCompletionResult.Completed;
        }

        public int EndRead()
        {
            Task<int> localResult = _readResult;
            _readResult = null;

            if (localResult != null)
            {
                try
                {
                    _bytesRead = localResult.GetAwaiter().GetResult();
                }
                catch (IOException ioException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertIOException(ioException));
                }
            }

            return _bytesRead;
        }

        private void OnRead(Task<int> antecedant, object state)
        {
            Contract.Requires(_readResult == null, "StreamConnection: OnRead called twice.");
            _readResult = antecedant;
            _readCallback(state);
        }
    }

    internal class ConnectionMessageProperty
    {
        private IConnection _connection;

        public ConnectionMessageProperty(IConnection connection)
        {
            _connection = connection;
        }

        public static string Name
        {
            get { return "iconnection"; }
        }

        public IConnection Connection
        {
            get { return _connection; }
        }
    }

    internal static class ConnectionUtilities
    {
        internal static void CloseNoThrow(IConnection connection, TimeSpan timeout)
        {
            bool success = false;
            try
            {
                connection.Close(timeout, false);
                success = true;
            }
            catch (TimeoutException)
            {
            }
            catch (CommunicationException)
            {
            }
            finally
            {
                if (!success)
                {
                    connection.Abort();
                }
            }
        }

        internal static void ValidateBufferBounds(ArraySegment<byte> buffer)
        {
            ValidateBufferBounds(buffer.Array, buffer.Offset, buffer.Count);
        }

        internal static void ValidateBufferBounds(byte[] buffer, int offset, int size)
        {
            if (buffer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
            }

            ValidateBufferBounds(buffer.Length, offset, size);
        }

        internal static void ValidateBufferBounds(int bufferSize, int offset, int size)
        {
            if (offset < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", offset, SRServiceModel.ValueMustBeNonNegative));
            }

            if (offset > bufferSize)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", offset, string.Format(SRServiceModel.OffsetExceedsBufferSize, bufferSize)));
            }

            if (size <= 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("size", size, SRServiceModel.ValueMustBePositive));
            }

            int remainingBufferSpace = bufferSize - offset;
            if (size > remainingBufferSpace)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("size", size, string.Format(
                    SRServiceModel.SizeExceedsRemainingBufferSpace, remainingBufferSpace)));
            }
        }
    }
}

namespace System.ServiceModel.Channels.ConnectionHelpers
{
    internal static class IConnectionExtensions
    {
        // This method is a convenience method for the open/close code paths and shouldn't be used on message send/receive.
        internal static async Task WriteAsync(this IConnection connection, byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<bool>();
            var result = connection.BeginWrite(buffer, offset, size, immediate, timeout, OnIoComplete, tcs);
            if (result == AsyncCompletionResult.Completed)
            {
                tcs.SetResult(true);
            }

            await tcs.Task;
            connection.EndWrite();
        }

        // This method is a convenience method for the open/close code paths and shouldn't be used on message send/receive.
        internal static async Task<int> ReadAsync(this IConnection connection, int offset, int size, TimeSpan timeout)
        {
            // read ACK
            var tcs = new TaskCompletionSource<bool>();
            //ackBuffer

            var result = connection.BeginRead(offset, size, timeout, OnIoComplete, tcs);
            if (result == AsyncCompletionResult.Completed)
            {
                tcs.SetResult(true);
            }

            await tcs.Task;
            int ackBytesRead = connection.EndRead();
            return ackBytesRead;
        }

        // This method is a convenience method for the open/close code paths and shouldn't be used on message send/receive.
        internal static async Task<int> ReadAsync(this IConnection connection, byte[] buffer, int offset, int size, TimeSpan timeout)
        {
            int ackBytesRead = await connection.ReadAsync(0, size, timeout);
            Buffer.BlockCopy(connection.AsyncReadBuffer, 0, buffer, offset, ackBytesRead);
            return ackBytesRead;
        }

        private static void OnIoComplete(object state)
        {
            if (state == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("state");
            }

            var tcs = state as TaskCompletionSource<bool>;
            if (tcs == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("state", SRServiceModel.SPS_InvalidAsyncResult);
            }

            tcs.TrySetResult(true);
        }
    }
}
