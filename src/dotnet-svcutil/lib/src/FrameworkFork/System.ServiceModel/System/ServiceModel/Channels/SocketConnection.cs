// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Sockets;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal abstract class SocketConnection : IConnection
    {
        // common state
        protected TimeSpan _sendTimeout;
        protected TimeSpan _receiveTimeout;
        protected CloseState _closeState;
        protected bool _aborted;

        // close state
        protected TimeoutHelper _closeTimeoutHelper;
        private bool _isShutdown;

        // read state
        protected int _asyncReadSize;
        protected byte[] _readBuffer;
        protected int _asyncReadBufferSize;
        protected object _asyncReadState;
        protected Action<object> _asyncReadCallback;
        protected Exception _asyncReadException;
        protected bool _asyncReadPending;

        // write state
        protected object _asyncWriteState;
        protected Action<object> _asyncWriteCallback;
        protected Exception _asyncWriteException;
        protected bool _asyncWritePending;

        protected string _timeoutErrorString;
        protected TransferOperation _timeoutErrorTransferOperation;
        private ConnectionBufferPool _connectionBufferPool;

        public SocketConnection(ConnectionBufferPool connectionBufferPool)
        {
            Contract.Assert(connectionBufferPool != null, "Argument connectionBufferPool cannot be null");

            _closeState = CloseState.Open;
            _connectionBufferPool = connectionBufferPool;
            _readBuffer = _connectionBufferPool.Take();
            _asyncReadBufferSize = _readBuffer.Length;
            _sendTimeout = _receiveTimeout = TimeSpan.MaxValue;
        }

        public int AsyncReadBufferSize
        {
            get { return _asyncReadBufferSize; }
        }

        public byte[] AsyncReadBuffer
        {
            get
            {
                return _readBuffer;
            }
        }

        protected object ThisLock
        {
            get { return this; }
        }

        protected abstract IPEndPoint RemoteEndPoint { get; }

        protected static void OnReceiveTimeout(object state)
        {
            SocketConnection thisPtr = (SocketConnection)state;
            thisPtr.Abort(string.Format(SRServiceModel.SocketAbortedReceiveTimedOut, thisPtr._receiveTimeout), TransferOperation.Read);
        }

        protected static void OnSendTimeout(object state)
        {
            SocketConnection thisPtr = (SocketConnection)state;
            thisPtr.Abort(4,	// TraceEventType.Warning
                string.Format(SRServiceModel.SocketAbortedSendTimedOut, thisPtr._sendTimeout), TransferOperation.Write);
        }

        public void Abort()
        {
            Abort(null, TransferOperation.Undefined);
        }

        protected void Abort(string timeoutErrorString, TransferOperation transferOperation)
        {
            int traceEventType = 4;	// TraceEventType.Warning;

            // we could be timing out a cached connection

            Abort(traceEventType, timeoutErrorString, transferOperation);
        }

        protected void Abort(int traceEventType)
        {
            Abort(traceEventType, null, TransferOperation.Undefined);
        }

        protected abstract void Abort(int traceEventType, string timeoutErrorString, TransferOperation transferOperation);

        protected abstract void AbortRead();

        public void Close(TimeSpan timeout, bool asyncAndLinger)
        {
            lock (ThisLock)
            {
                if (_closeState == CloseState.Closing || _closeState == CloseState.Closed)
                {
                    // already closing or closed, so just return
                    return;
                }
                _closeState = CloseState.Closing;
            }

            _closeTimeoutHelper = new TimeoutHelper(timeout);

            // first we shutdown our send-side
            Shutdown(timeout);
            CloseCore(asyncAndLinger);
        }

        protected abstract void CloseCore(bool asyncAndLinger);

        private void Shutdown(TimeSpan timeout)
        {
            lock (ThisLock)
            {
                if (_isShutdown)
                {
                    return;
                }

                _isShutdown = true;
            }

            ShutdownCore(timeout);
        }

        protected abstract void ShutdownCore(TimeSpan timeout);

        protected void ThrowIfNotOpen()
        {
            if (_closeState == CloseState.Closing || _closeState == CloseState.Closed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    ConvertObjectDisposedException(new ObjectDisposedException(
                    this.GetType().ToString(), SRServiceModel.SocketConnectionDisposed), TransferOperation.Undefined));
            }
        }

        protected void ThrowIfClosed()
        {
            if (_closeState == CloseState.Closed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    ConvertObjectDisposedException(new ObjectDisposedException(
                    this.GetType().ToString(), SRServiceModel.SocketConnectionDisposed), TransferOperation.Undefined));
            }
        }

        protected Exception ConvertSendException(SocketException socketException, TimeSpan remainingTime)
        {
            return ConvertTransferException(socketException, _sendTimeout, socketException,
                TransferOperation.Write, _aborted, _timeoutErrorString, _timeoutErrorTransferOperation, this, remainingTime);
        }

        protected Exception ConvertReceiveException(SocketException socketException, TimeSpan remainingTime)
        {
            return ConvertTransferException(socketException, _receiveTimeout, socketException,
                TransferOperation.Read, _aborted, _timeoutErrorString, _timeoutErrorTransferOperation, this, remainingTime);
        }

        internal static Exception ConvertTransferException(SocketException socketException, TimeSpan timeout, Exception originalException)
        {
            return ConvertTransferException(socketException, timeout, originalException,
                TransferOperation.Undefined, false, null, TransferOperation.Undefined, null, TimeSpan.MaxValue);
        }

        protected Exception ConvertObjectDisposedException(ObjectDisposedException originalException, TransferOperation transferOperation)
        {
            if (_timeoutErrorString != null)
            {
                return ConvertTimeoutErrorException(originalException, transferOperation, _timeoutErrorString, _timeoutErrorTransferOperation);
            }
            else if (_aborted)
            {
                return new CommunicationObjectAbortedException(SRServiceModel.SocketConnectionDisposed, originalException);
            }
            else
            {
                return originalException;
            }
        }

        private static Exception ConvertTransferException(SocketException socketException, TimeSpan timeout, Exception originalException,
            TransferOperation transferOperation, bool aborted, string timeoutErrorString, TransferOperation timeoutErrorTransferOperation,
            SocketConnection socketConnection, TimeSpan remainingTime)
        {
            if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.ERROR_INVALID_HANDLE)
            {
                return new CommunicationObjectAbortedException(socketException.Message, socketException);
            }

            if (timeoutErrorString != null)
            {
                return ConvertTimeoutErrorException(originalException, transferOperation, timeoutErrorString, timeoutErrorTransferOperation);
            }

            // 10053 can occur due to our timeout sockopt firing, so map to TimeoutException in that case
            if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAECONNABORTED &&
                remainingTime <= TimeSpan.Zero)
            {
                TimeoutException timeoutException = new TimeoutException(string.Format(SRServiceModel.TcpConnectionTimedOut, timeout), originalException);
                return timeoutException;
            }

            if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAENETRESET ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAECONNABORTED ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAECONNRESET)
            {
                if (aborted)
                {
                    return new CommunicationObjectAbortedException(SRServiceModel.TcpLocalConnectionAborted, originalException);
                }
                else
                {
                    CommunicationException communicationException = new CommunicationException(string.Format(SRServiceModel.TcpConnectionResetError, timeout), originalException);
                    return communicationException;
                }
            }
            else if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAETIMEDOUT)
            {
                TimeoutException timeoutException = new TimeoutException(string.Format(SRServiceModel.TcpConnectionTimedOut, timeout), originalException);
                return timeoutException;
            }
            else
            {
                if (aborted)
                {
                    return new CommunicationObjectAbortedException(string.Format(SRServiceModel.TcpTransferError, (int)socketException.SocketErrorCode, socketException.Message), originalException);
                }
                else
                {
                    CommunicationException communicationException = new CommunicationException(string.Format(SRServiceModel.TcpTransferError, (int)socketException.SocketErrorCode, socketException.Message), originalException);
                    return communicationException;
                }
            }
        }

        private static Exception ConvertTimeoutErrorException(Exception originalException,
            TransferOperation transferOperation, string timeoutErrorString, TransferOperation timeoutErrorTransferOperation)
        {
            Contract.Assert(timeoutErrorString != null, "Argument timeoutErrorString must not be null.");

            if (transferOperation == timeoutErrorTransferOperation)
            {
                return new TimeoutException(timeoutErrorString, originalException);
            }
            else
            {
                return new CommunicationException(timeoutErrorString, originalException);
            }
        }

        public AsyncCompletionResult BeginWrite(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout,
            Action<object> callback, object state)
        {
            if (WcfEventSource.Instance.SocketAsyncWriteStartIsEnabled())
            {
                TraceWriteStart(size, true);
            }

            return BeginWriteCore(buffer, offset, size, immediate, timeout, callback, state);
        }

        protected abstract void TraceWriteStart(int size, bool async);

        protected abstract AsyncCompletionResult BeginWriteCore(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout,
            Action<object> callback, object state);

        public void EndWrite()
        {
            EndWriteCore();
        }

        protected abstract void EndWriteCore();

        protected void FinishWrite()
        {
            Action<object> asyncWriteCallback = _asyncWriteCallback;
            object asyncWriteState = _asyncWriteState;

            _asyncWriteState = null;
            _asyncWriteCallback = null;

            asyncWriteCallback(asyncWriteState);
        }

        public void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout)
        {
            WriteCore(buffer, offset, size, immediate, timeout);
        }

        protected abstract void WriteCore(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout);

        public void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, BufferManager bufferManager)
        {
            try
            {
                Write(buffer, offset, size, immediate, timeout);
            }
            finally
            {
                bufferManager.ReturnBuffer(buffer);
            }
        }

        public int Read(byte[] buffer, int offset, int size, TimeSpan timeout)
        {
            ConnectionUtilities.ValidateBufferBounds(buffer, offset, size);
            ThrowIfNotOpen();
            int bytesRead = ReadCore(buffer, offset, size, timeout, false);
            if (WcfEventSource.Instance.SocketReadStopIsEnabled())
            {
                TraceSocketReadStop(bytesRead, false);
            }

            return bytesRead;
        }

        protected abstract int ReadCore(byte[] buffer, int offset, int size, TimeSpan timeout, bool closing);

        public virtual AsyncCompletionResult BeginRead(int offset, int size, TimeSpan timeout,
            Action<object> callback, object state)
        {
            ConnectionUtilities.ValidateBufferBounds(AsyncReadBufferSize, offset, size);
            this.ThrowIfNotOpen();
            var completionResult = this.BeginReadCore(offset, size, timeout, callback, state);
            if (completionResult == AsyncCompletionResult.Completed && WcfEventSource.Instance.SocketReadStopIsEnabled())
            {
                TraceSocketReadStop(_asyncReadSize, true);
            }

            return completionResult;
        }

        protected abstract void TraceSocketReadStop(int bytesRead, bool async);

        protected abstract AsyncCompletionResult BeginReadCore(int offset, int size, TimeSpan timeout,
            Action<object> callback, object state);

        protected void FinishRead()
        {
            if (_asyncReadException != null && WcfEventSource.Instance.SocketReadStopIsEnabled())
            {
                TraceSocketReadStop(_asyncReadSize, true);
            }

            Action<object> asyncReadCallback = _asyncReadCallback;
            object asyncReadState = _asyncReadState;

            _asyncReadState = null;
            _asyncReadCallback = null;

            asyncReadCallback(asyncReadState);
        }

        // Both BeginRead/ReadAsync paths completed themselves. EndRead's only job is to deliver the result.
        public int EndRead()
        {
            return EndReadCore();
        }

        protected abstract int EndReadCore();

        // This method should be called inside ThisLock
        protected void ReturnReadBuffer()
        {
            // We release the buffer only if there is no outstanding I/O
            this.TryReturnReadBuffer();
        }

        // This method should be called inside ThisLock
        protected void TryReturnReadBuffer()
        {
            // The buffer must not be returned and nulled when an abort occurs. Since the buffer
            // is also accessed by higher layers, code that has not yet realized the stack is
            // aborted may be attempting to read from the buffer.
            if (_readBuffer != null && !_aborted)
            {
                _connectionBufferPool.Return(_readBuffer);
                _readBuffer = null;
            }
        }

        protected enum CloseState
        {
            Open,
            Closing,
            Closed,
        }

        protected enum TransferOperation
        {
            Write,
            Read,
            Undefined,
        }
    }

    internal abstract class SocketConnectionInitiator : IConnectionInitiator
    {
        private int _bufferSize;
        protected ConnectionBufferPool _connectionBufferPool;

        public SocketConnectionInitiator(int bufferSize)
        {
            _bufferSize = bufferSize;
            _connectionBufferPool = new ConnectionBufferPool(bufferSize);
        }

        protected abstract IConnection CreateConnection(IPAddress address, int port);

        protected abstract Task<IConnection> CreateConnectionAsync(IPAddress address, int port);

        public static Exception ConvertConnectException(SocketException socketException, Uri remoteUri, TimeSpan timeSpent, Exception innerException)
        {
            if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.ERROR_INVALID_HANDLE)
            {
                return new CommunicationObjectAbortedException(socketException.Message, socketException);
            }

            if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAEADDRNOTAVAIL ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAECONNREFUSED ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAENETDOWN ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAENETUNREACH ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAEHOSTDOWN ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAEHOSTUNREACH ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAETIMEDOUT)
            {
                if (timeSpent == TimeSpan.MaxValue)
                {
                    return new EndpointNotFoundException(string.Format(SRServiceModel.TcpConnectError, remoteUri.AbsoluteUri, (int)socketException.SocketErrorCode, socketException.Message), innerException);
                }
                else
                {
                    return new EndpointNotFoundException(string.Format(SRServiceModel.TcpConnectErrorWithTimeSpan, remoteUri.AbsoluteUri, (int)socketException.SocketErrorCode, socketException.Message, timeSpent), innerException);
                }
            }
            else if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAENOBUFS)
            {
                return new OutOfMemoryException(SRServiceModel.TcpConnectNoBufs, innerException);
            }
            else if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.ERROR_NOT_ENOUGH_MEMORY ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.ERROR_NO_SYSTEM_RESOURCES ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.ERROR_OUTOFMEMORY)
            {
                return new OutOfMemoryException(SRServiceModel.InsufficentMemory, socketException);
            }
            else
            {
                if (timeSpent == TimeSpan.MaxValue)
                {
                    return new CommunicationException(string.Format(SRServiceModel.TcpConnectError, remoteUri.AbsoluteUri, (int)socketException.SocketErrorCode, socketException.Message), innerException);
                }
                else
                {
                    return new CommunicationException(string.Format(SRServiceModel.TcpConnectErrorWithTimeSpan, remoteUri.AbsoluteUri, (int)socketException.SocketErrorCode, socketException.Message, timeSpent), innerException);
                }
            }
        }

        private static async Task<IPAddress[]> GetIPAddressesAsync(Uri uri)
        {
            if (uri.HostNameType == UriHostNameType.IPv4 ||
                uri.HostNameType == UriHostNameType.IPv6)
            {
                IPAddress ipAddress = IPAddress.Parse(uri.DnsSafeHost);
                return new IPAddress[] { ipAddress };
            }

            IPAddress[] addresses = null;

            try
            {
                addresses = await DnsCache.ResolveAsync(uri);
            }
            catch (SocketException socketException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new EndpointNotFoundException(string.Format(SRServiceModel.UnableToResolveHost, uri.Host), socketException));
            }

            if (addresses.Length == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new EndpointNotFoundException(string.Format(SRServiceModel.UnableToResolveHost, uri.Host)));
            }

            return addresses;
        }

        private static TimeoutException CreateTimeoutException(Uri uri, TimeSpan timeout, IPAddress[] addresses, int invalidAddressCount,
            SocketException innerException)
        {
            StringBuilder addressStringBuilder = new StringBuilder();
            for (int i = 0; i < invalidAddressCount; i++)
            {
                if (addresses[i] == null)
                {
                    continue;
                }

                if (addressStringBuilder.Length > 0)
                {
                    addressStringBuilder.Append(", ");
                }
                addressStringBuilder.Append(addresses[i].ToString());
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                string.Format(SRServiceModel.TcpConnectingToViaTimedOut, uri.AbsoluteUri, timeout.ToString(),
                invalidAddressCount, addresses.Length, addressStringBuilder.ToString()), innerException));
        }

        public IConnection Connect(Uri uri, TimeSpan timeout)
        {
            int port = uri.Port;
            IPAddress[] addresses = SocketConnectionInitiator.GetIPAddressesAsync(uri).GetAwaiter().GetResult();
            IConnection socketConnection = null;
            SocketException lastException = null;

            if (port == -1)
            {
                port = TcpUri.DefaultPort;
            }

            int invalidAddressCount = 0;
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            for (int i = 0; i < addresses.Length; i++)
            {
                if (timeoutHelper.RemainingTime() == TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        CreateTimeoutException(uri, timeoutHelper.OriginalTimeout, addresses, invalidAddressCount, lastException));
                }

                DateTime connectStartTime = DateTime.UtcNow;
                try
                {
                    socketConnection = CreateConnection(addresses[i], port);
                    lastException = null;
                    break;
                }
                catch (SocketException socketException)
                {
                    invalidAddressCount++;
                    lastException = socketException;
                }
            }

            if (socketConnection == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new EndpointNotFoundException(string.Format(SRServiceModel.NoIPEndpointsFoundForHost, uri.Host)));
            }

            if (lastException != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    SocketConnectionInitiator.ConvertConnectException(lastException, uri,
                    timeoutHelper.ElapsedTime(), lastException));
            }

            return socketConnection;
        }

        public async Task<IConnection> ConnectAsync(Uri uri, TimeSpan timeout)
        {
            int port = uri.Port;
            IPAddress[] addresses = await SocketConnectionInitiator.GetIPAddressesAsync(uri);
            IConnection socketConnection = null;
            SocketException lastException = null;

            if (port == -1)
            {
                port = TcpUri.DefaultPort;
            }

            int invalidAddressCount = 0;
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            for (int i = 0; i < addresses.Length; i++)
            {
                if (timeoutHelper.RemainingTime() == TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        CreateTimeoutException(uri, timeoutHelper.OriginalTimeout, addresses, invalidAddressCount, lastException));
                }

                DateTime connectStartTime = DateTime.UtcNow;
                try
                {
                    socketConnection = await CreateConnectionAsync(addresses[i], port);
                    lastException = null;
                    break;
                }
                catch (SocketException socketException)
                {
                    invalidAddressCount++;
                    lastException = socketException;
                }
            }

            if (socketConnection == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new EndpointNotFoundException(string.Format(SRServiceModel.NoIPEndpointsFoundForHost, uri.Host)));
            }

            if (lastException != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    SocketConnectionInitiator.ConvertConnectException(lastException, uri,
                    timeoutHelper.ElapsedTime(), lastException));
            }

            return socketConnection;
        }
    }
}
