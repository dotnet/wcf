// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Sockets;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class SocketConnection : IConnection
    {
        static AsyncCallback s_onReceiveCompleted;
        static EventHandler<SocketAsyncEventArgs> s_onReceiveAsyncCompleted;
        static EventHandler<SocketAsyncEventArgs> s_onSocketSendCompleted;

        // common state
        private Socket _socket;
        private TimeSpan _asyncSendTimeout;
        private TimeSpan _readFinTimeout;
        private TimeSpan _asyncReceiveTimeout;

        // Socket.SendTimeout/Socket.ReceiveTimeout only work with the synchronous API calls and therefore they
        // do not get updated when asynchronous Send/Read operations are performed.  In order to make sure we 
        // Set the proper timeouts on the Socket itself we need to keep these two additional fields.
        private TimeSpan _socketSyncSendTimeout;
        private TimeSpan _socketSyncReceiveTimeout;

        private CloseState _closeState;
        private bool _isShutdown;
        private bool _noDelay = false;
        private bool _aborted;

        // close state
        private TimeoutHelper _closeTimeoutHelper;
        private static Action<object> s_onWaitForFinComplete = new Action<object>(OnWaitForFinComplete);

        // read state
        private int _asyncReadSize;
        private SocketAsyncEventArgs _asyncReadEventArgs;
        private byte[] _readBuffer;
        private int _asyncReadBufferSize;
        private object _asyncReadState;
        private Action<object> _asyncReadCallback;
        private Exception _asyncReadException;
        private bool _asyncReadPending;

        // write state
        private SocketAsyncEventArgs _asyncWriteEventArgs;
        private object _asyncWriteState;
        private Action<object> _asyncWriteCallback;
        private Exception _asyncWriteException;
        private bool _asyncWritePending;

        private IOTimer<SocketConnection> _receiveTimer;
        private static Action<object> s_onReceiveTimeout;
        private IOTimer<SocketConnection> _sendTimer;
        private static Action<object> s_onSendTimeout;
        private string _timeoutErrorString;
        private TransferOperation _timeoutErrorTransferOperation;
        private IPEndPoint _remoteEndpoint;
        private ConnectionBufferPool _connectionBufferPool;
        private string _remoteEndpointAddress;

        public SocketConnection(Socket socket, ConnectionBufferPool connectionBufferPool, bool autoBindToCompletionPort)
        {
            _connectionBufferPool = connectionBufferPool ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(connectionBufferPool));
            _socket = socket ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(socket));
            _closeState = CloseState.Open;
            _readBuffer = connectionBufferPool.Take();
            _asyncReadBufferSize = _readBuffer.Length;
            _socket.SendBufferSize = _socket.ReceiveBufferSize = _asyncReadBufferSize;
            _asyncSendTimeout = _asyncReceiveTimeout = TimeSpan.MaxValue;
            _socketSyncSendTimeout = _socketSyncReceiveTimeout = TimeSpan.MaxValue;

            _remoteEndpoint = null;

            if (autoBindToCompletionPort)
            {
                _socket.UseOnlyOverlappedIO = false;
            }

            // In SMSvcHost, sockets must be duplicated to the target process. Binding a handle to a completion port
            // prevents any duplicated handle from ever binding to a completion port. The target process is where we
            // want to use completion ports for performance. This means that in SMSvcHost, socket.UseOnlyOverlappedIO
            // must be set to true to prevent completion port use.
            if (_socket.UseOnlyOverlappedIO)
            {
                // Init BeginRead state
                if (s_onReceiveCompleted == null)
                {
                    s_onReceiveCompleted = Fx.ThunkCallback(new AsyncCallback(OnReceiveCompleted));
                }
            }
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

        private object ThisLock
        {
            get { return this; }
        }

        public IPEndPoint RemoteIPEndPoint
        {
            get
            {
                // this property should only be called on the receive path
                if (_remoteEndpoint == null && _closeState == CloseState.Open)
                {
                    try
                    {
                        _remoteEndpoint = (IPEndPoint)_socket.RemoteEndPoint;
                    }
                    catch (SocketException socketException)
                    {
                        // will never be a timeout error, so TimeSpan.Zero is ok
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            ConvertReceiveException(socketException, TimeSpan.Zero, TimeSpan.Zero));
                    }
                    catch (ObjectDisposedException objectDisposedException)
                    {
                        Exception exceptionToThrow = ConvertObjectDisposedException(objectDisposedException, TransferOperation.Undefined);
                        if (ReferenceEquals(exceptionToThrow, objectDisposedException))
                        {
                            throw;
                        }
                        else
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exceptionToThrow);
                        }
                    }
                }

                return _remoteEndpoint;
            }
        }

        private IOTimer<SocketConnection> SendTimer
        {
            get
            {
                if (_sendTimer == null)
                {
                    if (s_onSendTimeout == null)
                    {
                        s_onSendTimeout = new Action<object>(OnSendTimeout);
                    }

                    _sendTimer = new IOTimer<SocketConnection>(s_onSendTimeout, this);
                }

                return _sendTimer;
            }
        }

        private IOTimer<SocketConnection> ReceiveTimer
        {
            get
            {
                if (_receiveTimer == null)
                {
                    if (s_onReceiveTimeout == null)
                    {
                        s_onReceiveTimeout = new Action<object>(OnReceiveTimeout);
                    }

                    _receiveTimer = new IOTimer<SocketConnection>(s_onReceiveTimeout, this);
                }

                return _receiveTimer;
            }
        }


        private string RemoteEndpointAddress
        {
            get
            {
                if (_remoteEndpointAddress == null)
                {
                    try
                    {
                        if (TryGetEndpoints(out IPEndPoint local, out IPEndPoint remote))
                        {
                            _remoteEndpointAddress = remote.Address + ":" + remote.Port;
                        }
                        else
                        {
                            //null indicates not initialized.
                            _remoteEndpointAddress = string.Empty;
                        }
                    }
                    catch (Exception exception)
                    {
                        if (Fx.IsFatal(exception))
                        {
                            throw;
                        }

                    }
                }
                return _remoteEndpointAddress;
            }
        }

        private static void OnReceiveTimeout(object state)
        {
            SocketConnection thisPtr = (SocketConnection)state;
            thisPtr.Abort(SR.Format(SR.SocketAbortedReceiveTimedOut, thisPtr._asyncReceiveTimeout), TransferOperation.Read);
        }

        private static void OnSendTimeout(object state)
        {
            SocketConnection thisPtr = (SocketConnection)state;
            thisPtr.Abort(4,	// TraceEventType.Warning
                SR.Format(SR.SocketAbortedSendTimedOut, thisPtr._asyncSendTimeout), TransferOperation.Write);
        }

        private static void OnReceiveCompleted(IAsyncResult result)
        {
            ((SocketConnection)result.AsyncState).OnReceive(result);
        }

        private static void OnReceiveAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            ((SocketConnection)e.UserToken).OnReceiveAsync(sender, e);
        }

        private static void OnSendAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            ((SocketConnection)e.UserToken).OnSendAsync(sender, e);
        }

        public void Abort()
        {
            Abort(null, TransferOperation.Undefined);
        }

        private void Abort(string timeoutErrorString, TransferOperation transferOperation)
        {
            int traceEventType = 4;	// TraceEventType.Warning;

            // we could be timing out a cached connection

            Abort(traceEventType, timeoutErrorString, transferOperation);
        }

        private void Abort(int traceEventType)
        {
            Abort(traceEventType, null, TransferOperation.Undefined);
        }

        private void Abort(int traceEventType, string timeoutErrorString, TransferOperation transferOperation)
        {
            lock (ThisLock)
            {
                if (_closeState == CloseState.Closed)
                {
                    return;
                }

                _timeoutErrorString = timeoutErrorString;
                _timeoutErrorTransferOperation = transferOperation;
                _aborted = true;
                _closeState = CloseState.Closed;

                if (_asyncReadPending)
                {
                    CancelReceiveTimer();
                }
                else
                {
                    DisposeReadEventArgs();
                }

                if (_asyncWritePending)
                {
                    CancelSendTimer();
                }
                else
                {
                    DisposeWriteEventArgs();
                }
            }

            _socket.Close(0);
        }

        private void AbortRead()
        {
            lock (ThisLock)
            {
                if (_asyncReadPending)
                {
                    if (_closeState != CloseState.Closed)
                    {
                        SetUserToken(_asyncReadEventArgs, null);
                        _asyncReadPending = false;
                        CancelReceiveTimer();
                    }
                    else
                    {
                        DisposeReadEventArgs();
                    }
                }
            }
        }

        private void CancelReceiveTimer()
        {
            if (_receiveTimer != null)
            {
                _receiveTimer.Cancel();
            }
        }

        private void CancelSendTimer()
        {
            _sendTimer?.Cancel();
        }

        private void CloseAsyncAndLinger()
        {
            _readFinTimeout = _closeTimeoutHelper.RemainingTime();

            try
            {
                // A FIN (shutdown) packet has already been sent to the remote host and we're waiting for the remote
                // host to send a FIN back. A pending read on a socket will complete returning zero bytes when a FIN
                // packet is received.
                if (BeginReadCore(0, 1, _readFinTimeout, s_onWaitForFinComplete, this) == AsyncCompletionResult.Queued)
                {
                    return;
                }

                int bytesRead = EndRead();

                if (bytesRead > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new CommunicationException(SR.Format(SR.SocketCloseReadReceivedData, _socket.RemoteEndPoint)));
                }
            }
            catch (TimeoutException timeoutException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                    SR.Format(SR.SocketCloseReadTimeout, _socket.RemoteEndPoint, _readFinTimeout), timeoutException));
            }

            ContinueClose(_closeTimeoutHelper.RemainingTime());
        }

        private static void OnWaitForFinComplete(object state)
        {
            SocketConnection thisPtr = (SocketConnection)state;

            try
            {
                int bytesRead;

                try
                {
                    bytesRead = thisPtr.EndRead();

                    if (bytesRead > 0)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new CommunicationException(SR.Format(SR.SocketCloseReadReceivedData, thisPtr._socket.RemoteEndPoint)));
                    }
                }
                catch (TimeoutException timeoutException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                        SR.Format(SR.SocketCloseReadTimeout, thisPtr._socket.RemoteEndPoint, thisPtr._readFinTimeout),
                        timeoutException));
                }

                thisPtr.ContinueClose(thisPtr._closeTimeoutHelper.RemainingTime());
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                // The user has no opportunity to clean up the connection in the async and linger
                // code path, ensure cleanup finishes.
                thisPtr.Abort();
            }
        }

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

            // first we shutdown our send-side
            _closeTimeoutHelper = new TimeoutHelper(timeout);
            Shutdown(_closeTimeoutHelper.RemainingTime());

            if (asyncAndLinger)
            {
                CloseAsyncAndLinger();
            }
            else
            {
                CloseSync();
            }
        }

        private void CloseSync()
        {
            byte[] dummy = new byte[1];

            // then we check for a FIN from the other side (i.e. read zero)
            int bytesRead;
            _readFinTimeout = _closeTimeoutHelper.RemainingTime();

            try
            {
                bytesRead = ReadCore(dummy, 0, 1, _readFinTimeout, true);

                if (bytesRead > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new CommunicationException(SR.Format(SR.SocketCloseReadReceivedData, _socket.RemoteEndPoint)));
                }
            }
            catch (TimeoutException timeoutException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                    SR.Format(SR.SocketCloseReadTimeout, _socket.RemoteEndPoint, _readFinTimeout), timeoutException));
            }

            // finally we call Close with whatever time is remaining
            ContinueClose(_closeTimeoutHelper.RemainingTime());
        }

        public void ContinueClose(TimeSpan timeout)
        {
            _socket.Close(TimeoutHelper.ToMilliseconds(timeout));

            lock (ThisLock)
            {
                // Abort could have been called on a separate thread and cleaned up 
                // our buffers/completion here
                if (_closeState != CloseState.Closed)
                {
                    if (!_asyncReadPending)
                    {
                        DisposeReadEventArgs();
                    }

                    if (!_asyncWritePending)
                    {
                        DisposeWriteEventArgs();
                    }
                }

                _closeState = CloseState.Closed;
            }
        }

        public void Shutdown(TimeSpan timeout)
        {
            lock (ThisLock)
            {
                if (_isShutdown)
                {
                    return;
                }

                _isShutdown = true;
            }

            try
            {
                _socket.Shutdown(SocketShutdown.Send);
            }
            catch (SocketException socketException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    ConvertSendException(socketException, TimeSpan.MaxValue, _socketSyncSendTimeout));
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Exception exceptionToThrow = ConvertObjectDisposedException(objectDisposedException, TransferOperation.Undefined);
                if (ReferenceEquals(exceptionToThrow, objectDisposedException))
                {
                    throw;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exceptionToThrow);
                }
            }
        }

        private void ThrowIfNotOpen()
        {
            if (_closeState == CloseState.Closing || _closeState == CloseState.Closed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    ConvertObjectDisposedException(new ObjectDisposedException(
                    GetType().ToString(), SR.SocketConnectionDisposed), TransferOperation.Undefined));
            }
        }

        private void ThrowIfClosed()
        {
            if (_closeState == CloseState.Closed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    ConvertObjectDisposedException(new ObjectDisposedException(
                    GetType().ToString(), SR.SocketConnectionDisposed), TransferOperation.Undefined));
            }
        }

        private bool TryGetEndpoints(out IPEndPoint localIPEndpoint, out IPEndPoint remoteIPEndpoint)
        {
            localIPEndpoint = null;
            remoteIPEndpoint = null;

            if (_closeState == CloseState.Open)
            {
                try
                {
                    remoteIPEndpoint = _remoteEndpoint ?? (IPEndPoint)_socket.RemoteEndPoint;
                    localIPEndpoint = (IPEndPoint)_socket.LocalEndPoint;
                }
                catch (Exception exception)
                {
                    if (Fx.IsFatal(exception))
                    {
                        throw;
                    }

                }
            }

            return localIPEndpoint != null && remoteIPEndpoint != null;
        }

        public object GetCoreTransport()
        {
            return _socket;
        }

        public IAsyncResult BeginValidate(Uri uri, AsyncCallback callback, object state)
        {
            return new CompletedAsyncResult<bool>(true, callback, state);
        }

        public bool EndValidate(IAsyncResult result)
        {
            return CompletedAsyncResult<bool>.End(result);
        }

        private Exception ConvertSendException(SocketException socketException, TimeSpan remainingTime, TimeSpan timeout)
        {
            return ConvertTransferException(socketException, timeout, socketException,
                TransferOperation.Write, _aborted, _timeoutErrorString, _timeoutErrorTransferOperation, this, remainingTime);
        }

        private Exception ConvertReceiveException(SocketException socketException, TimeSpan remainingTime, TimeSpan timeout)
        {
            return ConvertTransferException(socketException, timeout, socketException,
                TransferOperation.Read, _aborted, _timeoutErrorString, _timeoutErrorTransferOperation, this, remainingTime);
        }

        internal static Exception ConvertTransferException(SocketException socketException, TimeSpan timeout, Exception originalException)
        {
            return ConvertTransferException(socketException, timeout, originalException,
                TransferOperation.Undefined, false, null, TransferOperation.Undefined, null, TimeSpan.MaxValue);
        }

        private Exception ConvertObjectDisposedException(ObjectDisposedException originalException, TransferOperation transferOperation)
        {
            if (_timeoutErrorString != null)
            {
                return ConvertTimeoutErrorException(originalException, transferOperation, _timeoutErrorString, _timeoutErrorTransferOperation);
            }
            else if (_aborted)
            {
                return new CommunicationObjectAbortedException(SR.SocketConnectionDisposed, originalException);
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
            if (socketException.ErrorCode == UnsafeNativeMethods.ERROR_INVALID_HANDLE)
            {
                return new CommunicationObjectAbortedException(socketException.Message, socketException);
            }

            if (timeoutErrorString != null)
            {
                return ConvertTimeoutErrorException(originalException, transferOperation, timeoutErrorString, timeoutErrorTransferOperation);
            }

            // 10053 can occur due to our timeout sockopt firing, so map to TimeoutException in that case
            if (socketException.ErrorCode == UnsafeNativeMethods.WSAECONNABORTED &&
                remainingTime <= TimeSpan.Zero)
            {
                TimeoutException timeoutException = new TimeoutException(SR.Format(SR.TcpConnectionTimedOut, timeout), originalException);
                return timeoutException;
            }

            if (socketException.ErrorCode == UnsafeNativeMethods.WSAENETRESET ||
                socketException.ErrorCode == UnsafeNativeMethods.WSAECONNABORTED ||
                socketException.ErrorCode == UnsafeNativeMethods.WSAECONNRESET)
            {
                if (aborted)
                {
                    return new CommunicationObjectAbortedException(SR.TcpLocalConnectionAborted, originalException);
                }
                else
                {
                    CommunicationException communicationException = new CommunicationException(SR.Format(SR.TcpConnectionResetError, timeout), originalException);
                    return communicationException;
                }
            }
            else if (socketException.ErrorCode == UnsafeNativeMethods.WSAETIMEDOUT)
            {
                TimeoutException timeoutException = new TimeoutException(SR.Format(SR.TcpConnectionTimedOut, timeout), originalException);
                return timeoutException;
            }
            else
            {
                if (aborted)
                {
                    return new CommunicationObjectAbortedException(SR.Format(SR.TcpTransferError, socketException.ErrorCode, socketException.Message), originalException);
                }
                else
                {
                    CommunicationException communicationException = new CommunicationException(SR.Format(SR.TcpTransferError, socketException.ErrorCode, socketException.Message), originalException);
                    return communicationException;
                }
            }
        }

        private static Exception ConvertTimeoutErrorException(Exception originalException,
            TransferOperation transferOperation, string timeoutErrorString, TransferOperation timeoutErrorTransferOperation)
        {
            if (timeoutErrorString == null)
            {
                Fx.Assert("Argument timeoutErrorString must not be null.");
            }

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
            ConnectionUtilities.ValidateBufferBounds(buffer, offset, size);
            bool abortWrite = true;

            try
            {
                if (WcfEventSource.Instance.SocketAsyncWriteStartIsEnabled())
                {
                    TraceWriteStart(size, true);
                }

                lock (ThisLock)
                {
                    Fx.Assert(!_asyncWritePending, "Called BeginWrite twice.");
                    ThrowIfClosed();
                    EnsureWriteEventArgs();
                    SetImmediate(immediate);
                    SetWriteTimeout(timeout, false);
                    SetUserToken(_asyncWriteEventArgs, this);
                    _asyncWritePending = true;
                    _asyncWriteCallback = callback;
                    _asyncWriteState = state;
                }

                _asyncWriteEventArgs.SetBuffer(buffer, offset, size);

                if (_socket.SendAsync(_asyncWriteEventArgs))
                {
                    abortWrite = false;
                    return AsyncCompletionResult.Queued;
                }

                HandleSendAsyncCompleted();
                abortWrite = false;
                return AsyncCompletionResult.Completed;
            }
            catch (SocketException socketException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    ConvertSendException(socketException, TimeSpan.MaxValue, _asyncSendTimeout));
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Exception exceptionToThrow = ConvertObjectDisposedException(objectDisposedException, TransferOperation.Write);
                if (ReferenceEquals(exceptionToThrow, objectDisposedException))
                {
                    throw;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exceptionToThrow);
                }
            }
            finally
            {
                if (abortWrite)
                {
                    AbortWrite();
                }
            }
        }

        public void EndWrite()
        {
            if (_asyncWriteException != null)
            {
                AbortWrite();
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(_asyncWriteException);
            }

            lock (ThisLock)
            {
                if (!_asyncWritePending)
                {
                    throw Fx.AssertAndThrow("SocketConnection.EndWrite called with no write pending.");
                }

                SetUserToken(_asyncWriteEventArgs, null);
                _asyncWritePending = false;

                if (_closeState == CloseState.Closed)
                {
                    DisposeWriteEventArgs();
                }
            }
        }

        private void OnSendAsync(object sender, SocketAsyncEventArgs eventArgs)
        {
            Fx.Assert(eventArgs != null, "Argument 'eventArgs' cannot be NULL.");
            CancelSendTimer();

            try
            {
                HandleSendAsyncCompleted();
                Fx.Assert(eventArgs.BytesTransferred == _asyncWriteEventArgs.Count, "The socket SendAsync did not send all the bytes.");
            }
            catch (SocketException socketException)
            {
                _asyncWriteException = ConvertSendException(socketException, TimeSpan.MaxValue, _asyncSendTimeout);
            }
            catch (Exception exception)
            {
                if (Fx.IsFatal(exception))
                {
                    throw;
                }

                _asyncWriteException = exception;
            }

            FinishWrite();
        }

        private void HandleSendAsyncCompleted()
        {
            if (_asyncWriteEventArgs.SocketError == SocketError.Success)
            {
                return;
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SocketException((int)_asyncWriteEventArgs.SocketError));
        }

        // This method should be called inside ThisLock
        private void DisposeWriteEventArgs()
        {
            if (_asyncWriteEventArgs != null)
            {
                _asyncWriteEventArgs.Completed -= s_onSocketSendCompleted;
                _asyncWriteEventArgs.Dispose();
            }
        }

        private void AbortWrite()
        {
            lock (ThisLock)
            {
                if (_asyncWritePending)
                {
                    if (_closeState != CloseState.Closed)
                    {
                        SetUserToken(_asyncWriteEventArgs, null);
                        _asyncWritePending = false;
                        CancelSendTimer();
                    }
                    else
                    {
                        DisposeWriteEventArgs();
                    }
                }
            }
        }

        private void FinishWrite()
        {
            Action<object> asyncWriteCallback = _asyncWriteCallback;
            object asyncWriteState = _asyncWriteState;

            _asyncWriteState = null;
            _asyncWriteCallback = null;

            asyncWriteCallback(asyncWriteState);
        }

        public void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout)
        {
            // as per http://support.microsoft.com/default.aspx?scid=kb%3ben-us%3b201213
            // we shouldn't write more than 64K synchronously to a socket
            const int maxSocketWrite = 64 * 1024;

            ConnectionUtilities.ValidateBufferBounds(buffer, offset, size);

            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            try
            {
                SetImmediate(immediate);
                int bytesToWrite = size;

                while (bytesToWrite > 0)
                {
                    SetWriteTimeout(timeoutHelper.RemainingTime(), true);
                    size = Math.Min(bytesToWrite, maxSocketWrite);
                    _socket.Send(buffer, offset, size, SocketFlags.None);
                    bytesToWrite -= size;
                    offset += size;
                    timeout = timeoutHelper.RemainingTime();
                }
            }
            catch (SocketException socketException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    ConvertSendException(socketException, timeoutHelper.RemainingTime(), _socketSyncSendTimeout));
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Exception exceptionToThrow = ConvertObjectDisposedException(objectDisposedException, TransferOperation.Write);
                if (ReferenceEquals(exceptionToThrow, objectDisposedException))
                {
                    throw;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exceptionToThrow);
                }
            }
        }

        private void TraceWriteStart(int size, bool async)
        {
            if (!async)
            {
                WcfEventSource.Instance.SocketWriteStart(_socket.GetHashCode(), size, RemoteEndpointAddress);
            }
            else
            {
                WcfEventSource.Instance.SocketAsyncWriteStart(_socket.GetHashCode(), size, RemoteEndpointAddress);
            }
        }

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
            return ReadCore(buffer, offset, size, timeout, false);
        }

        private int ReadCore(byte[] buffer, int offset, int size, TimeSpan timeout, bool closing)
        {
            int bytesRead = 0;
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            try
            {
                SetReadTimeout(timeoutHelper.RemainingTime(), true, closing);
                bytesRead = _socket.Receive(buffer, offset, size, SocketFlags.None);
            }
            catch (SocketException socketException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    ConvertReceiveException(socketException, timeoutHelper.RemainingTime(), _socketSyncReceiveTimeout));
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Exception exceptionToThrow = ConvertObjectDisposedException(objectDisposedException, TransferOperation.Read);
                if (ReferenceEquals(exceptionToThrow, objectDisposedException))
                {
                    throw;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exceptionToThrow);
                }
            }

            return bytesRead;
        }

        private void TraceSocketReadStop(int bytesRead, bool async)
        {
            if (!async)
            {
                WcfEventSource.Instance.SocketReadStop((_socket != null) ? _socket.GetHashCode() : -1, bytesRead, RemoteEndpointAddress);
            }
            else
            {
                WcfEventSource.Instance.SocketAsyncReadStop((_socket != null) ? _socket.GetHashCode() : -1, bytesRead, RemoteEndpointAddress);
            }
        }

        public virtual AsyncCompletionResult BeginRead(int offset, int size, TimeSpan timeout,
            Action<object> callback, object state)
        {
            ConnectionUtilities.ValidateBufferBounds(AsyncReadBufferSize, offset, size);
            ThrowIfNotOpen();
            return BeginReadCore(offset, size, timeout, callback, state);
        }

        private AsyncCompletionResult BeginReadCore(int offset, int size, TimeSpan timeout,
            Action<object> callback, object state)
        {
            bool abortRead = true;

            lock (ThisLock)
            {
                ThrowIfClosed();
                EnsureReadEventArgs();
                _asyncReadState = state;
                _asyncReadCallback = callback;
                SetUserToken(_asyncReadEventArgs, this);
                _asyncReadPending = true;
                SetReadTimeout(timeout, false, false);
            }

            try
            {
                if (_socket.UseOnlyOverlappedIO)
                {
                    // ReceiveAsync does not respect UseOnlyOverlappedIO but BeginReceive does.
                    IAsyncResult result = _socket.BeginReceive(AsyncReadBuffer, offset, size, SocketFlags.None, s_onReceiveCompleted, this);

                    if (!result.CompletedSynchronously)
                    {
                        abortRead = false;
                        return AsyncCompletionResult.Queued;
                    }

                    _asyncReadSize = _socket.EndReceive(result);
                }
                else
                {
                    if (offset != _asyncReadEventArgs.Offset ||
                        size != _asyncReadEventArgs.Count)
                    {
                        _asyncReadEventArgs.SetBuffer(offset, size);
                    }

                    if (ReceiveAsync())
                    {
                        abortRead = false;
                        return AsyncCompletionResult.Queued;
                    }

                    HandleReceiveAsyncCompleted();
                    _asyncReadSize = _asyncReadEventArgs.BytesTransferred;
                }

                if (WcfEventSource.Instance.SocketReadStopIsEnabled())
                {
                    TraceSocketReadStop(_asyncReadSize, true);
                }

                abortRead = false;
                return AsyncCompletionResult.Completed;
            }
            catch (SocketException socketException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertReceiveException(socketException, TimeSpan.MaxValue, _asyncReceiveTimeout));
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Exception exceptionToThrow = ConvertObjectDisposedException(objectDisposedException, TransferOperation.Read);
                if (ReferenceEquals(exceptionToThrow, objectDisposedException))
                {
                    throw;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exceptionToThrow);
                }
            }
            finally
            {
                if (abortRead)
                {
                    AbortRead();
                }
            }
        }

        private bool ReceiveAsync()
        {
            return _socket.ReceiveAsync(_asyncReadEventArgs);
        }

        private void OnReceive(IAsyncResult result)
        {
            CancelReceiveTimer();
            if (result.CompletedSynchronously)
            {
                return;
            }

            try
            {
                _asyncReadSize = _socket.EndReceive(result);

                if (WcfEventSource.Instance.SocketReadStopIsEnabled())
                {
                    TraceSocketReadStop(_asyncReadSize, true);
                }
            }
            catch (SocketException socketException)
            {
                _asyncReadException = ConvertReceiveException(socketException, TimeSpan.MaxValue, _asyncReceiveTimeout);
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                _asyncReadException = ConvertObjectDisposedException(objectDisposedException, TransferOperation.Read);
            }
            catch (Exception exception)
            {
                if (Fx.IsFatal(exception))
                {
                    throw;
                }
                _asyncReadException = exception;
            }

            FinishRead();
        }

        private void OnReceiveAsync(object sender, SocketAsyncEventArgs eventArgs)
        {
            Fx.Assert(eventArgs != null, "Argument 'eventArgs' cannot be NULL.");
            CancelReceiveTimer();

            try
            {
                HandleReceiveAsyncCompleted();
                _asyncReadSize = eventArgs.BytesTransferred;

                if (WcfEventSource.Instance.SocketReadStopIsEnabled())
                {
                    TraceSocketReadStop(_asyncReadSize, true);
                }
            }
            catch (SocketException socketException)
            {
                _asyncReadException = ConvertReceiveException(socketException, TimeSpan.MaxValue, _asyncReceiveTimeout);
            }
            catch (Exception exception)
            {
                if (Fx.IsFatal(exception))
                {
                    throw;
                }
                _asyncReadException = exception;
            }

            FinishRead();
        }

        private void HandleReceiveAsyncCompleted()
        {
            if (_asyncReadEventArgs.SocketError == SocketError.Success)
            {
                return;
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SocketException((int)_asyncReadEventArgs.SocketError));
        }

        private void FinishRead()
        {
            Action<object> asyncReadCallback = _asyncReadCallback;
            object asyncReadState = _asyncReadState;

            _asyncReadState = null;
            _asyncReadCallback = null;

            asyncReadCallback(asyncReadState);
        }

        // Both BeginRead/ReadAsync paths completed themselves. EndRead's only job is to deliver the result.
        public int EndRead()
        {
            if (_asyncReadException != null)
            {
                AbortRead();
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(_asyncReadException);
            }

            lock (ThisLock)
            {
                if (!_asyncReadPending)
                {
                    throw Fx.AssertAndThrow("SocketConnection.EndRead called with no read pending.");
                }

                SetUserToken(_asyncReadEventArgs, null);
                _asyncReadPending = false;

                if (_closeState == CloseState.Closed)
                {
                    DisposeReadEventArgs();
                }
            }

            return _asyncReadSize;
        }

        // This method should be called inside ThisLock
        private void DisposeReadEventArgs()
        {
            if (_asyncReadEventArgs != null)
            {
                _asyncReadEventArgs.Completed -= s_onReceiveAsyncCompleted;
                _asyncReadEventArgs.Dispose();
            }

            // We release the buffer only if there is no outstanding I/O
            TryReturnReadBuffer();
        }

        private void TryReturnReadBuffer()
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

        private void SetUserToken(SocketAsyncEventArgs args, object userToken)
        {
            // The socket args can be pinned by the overlapped callback. Ensure SocketConnection is
            // only pinned when there is outstanding IO.
            if (args != null)
            {
                args.UserToken = userToken;
            }
        }

        private void SetImmediate(bool immediate)
        {
            if (immediate != _noDelay)
            {
                lock (ThisLock)
                {
                    ThrowIfNotOpen();
                    _socket.NoDelay = immediate;
                }
                _noDelay = immediate;
            }
        }

        private void SetReadTimeout(TimeSpan timeout, bool synchronous, bool closing)
        {
            if (synchronous)
            {
                CancelReceiveTimer();

                // 0 == infinite for winsock timeouts, so we should preempt and throw
                if (timeout <= TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new TimeoutException(SR.Format(SR.TcpConnectionTimedOut, timeout)));
                }

                if (ShouldUpdateTimeout(_socketSyncReceiveTimeout, timeout))
                {
                    lock (ThisLock)
                    {
                        if (!closing || _closeState != CloseState.Closing)
                        {
                            ThrowIfNotOpen();
                        }
                        _socket.ReceiveTimeout = TimeoutHelper.ToMilliseconds(timeout);
                    }
                    _socketSyncReceiveTimeout = timeout;
                }
            }
            else
            {
                _asyncReceiveTimeout = timeout;
                if (timeout == TimeSpan.MaxValue)
                {
                    CancelReceiveTimer();
                }
                else
                {
                    ReceiveTimer.ScheduleAfter(timeout);
                }
            }
        }

        private void SetWriteTimeout(TimeSpan timeout, bool synchronous)
        {
            if (synchronous)
            {
                CancelSendTimer();

                // 0 == infinite for winsock timeouts, so we should preempt and throw
                if (timeout <= TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new TimeoutException(SR.Format(SR.TcpConnectionTimedOut, timeout)));
                }

                if (ShouldUpdateTimeout(_socketSyncSendTimeout, timeout))
                {
                    lock (ThisLock)
                    {
                        ThrowIfNotOpen();
                        _socket.SendTimeout = TimeoutHelper.ToMilliseconds(timeout);
                    }
                    _socketSyncSendTimeout = timeout;
                }
            }
            else
            {
                _asyncSendTimeout = timeout;
                if (timeout == TimeSpan.MaxValue)
                {
                    CancelSendTimer();
                }
                else
                {
                    SendTimer.ScheduleAfter(timeout);
                }
            }
        }

        private bool ShouldUpdateTimeout(TimeSpan oldTimeout, TimeSpan newTimeout)
        {
            if (oldTimeout == newTimeout)
            {
                return false;
            }

            long threshold = oldTimeout.Ticks / 10;
            long delta = Math.Max(oldTimeout.Ticks, newTimeout.Ticks) - Math.Min(oldTimeout.Ticks, newTimeout.Ticks);

            return delta > threshold;
        }

        // This method should be called inside ThisLock
        private void EnsureReadEventArgs()
        {
            if (_asyncReadEventArgs == null)
            {
                // Init ReadAsync state
                if (s_onReceiveAsyncCompleted == null)
                {
                    s_onReceiveAsyncCompleted = new EventHandler<SocketAsyncEventArgs>(OnReceiveAsyncCompleted);
                }

                _asyncReadEventArgs = new SocketAsyncEventArgs();
                _asyncReadEventArgs.SetBuffer(_readBuffer, 0, _readBuffer.Length);
                _asyncReadEventArgs.Completed += s_onReceiveAsyncCompleted;
            }
        }

        // This method should be called inside ThisLock
        private void EnsureWriteEventArgs()
        {
            if (_asyncWriteEventArgs == null)
            {
                // Init SendAsync state
                if (s_onSocketSendCompleted == null)
                {
                    s_onSocketSendCompleted = new EventHandler<SocketAsyncEventArgs>(OnSendAsyncCompleted);
                }

                _asyncWriteEventArgs = new SocketAsyncEventArgs();
                _asyncWriteEventArgs.Completed += s_onSocketSendCompleted;
            }
        }

        private enum CloseState
        {
            Open,
            Closing,
            Closed,
        }

        private enum TransferOperation
        {
            Write,
            Read,
            Undefined,
        }
    }

    internal class SocketConnectionInitiator : IConnectionInitiator
    {
        private int _bufferSize;
        private ConnectionBufferPool _connectionBufferPool;

        public SocketConnectionInitiator(int bufferSize)
        {
            _bufferSize = bufferSize;
            _connectionBufferPool = new ConnectionBufferPool(bufferSize);
        }

        private IConnection CreateConnection(IPAddress address, int port)
        {
            Socket socket = null;
            try
            {
                AddressFamily addressFamily = address.AddressFamily;
                socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(address, port));
                return new SocketConnection(socket, _connectionBufferPool, false);
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        }

        private async Task<IConnection> CreateConnectionAsync(IPAddress address, int port)
        {
            Socket socket = null;
            try
            {
                AddressFamily addressFamily = address.AddressFamily;
                socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(new IPEndPoint(address, port));
                return new SocketConnection(socket, _connectionBufferPool, false);
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        }

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
                    return new EndpointNotFoundException(SR.Format(SR.TcpConnectError, remoteUri.AbsoluteUri, (int)socketException.SocketErrorCode, socketException.Message), innerException);
                }
                else
                {
                    return new EndpointNotFoundException(SR.Format(SR.TcpConnectErrorWithTimeSpan, remoteUri.AbsoluteUri, (int)socketException.SocketErrorCode, socketException.Message, timeSpent), innerException);
                }
            }
            else if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAENOBUFS)
            {
                return new OutOfMemoryException(SR.TcpConnectNoBufs, innerException);
            }
            else if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.ERROR_NOT_ENOUGH_MEMORY ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.ERROR_NO_SYSTEM_RESOURCES ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.ERROR_OUTOFMEMORY)
            {
                return new OutOfMemoryException(SR.InsufficentMemory, socketException);
            }
            else
            {
                if (timeSpent == TimeSpan.MaxValue)
                {
                    return new CommunicationException(SR.Format(SR.TcpConnectError, remoteUri.AbsoluteUri, (int)socketException.SocketErrorCode, socketException.Message), innerException);
                }
                else
                {
                    return new CommunicationException(SR.Format(SR.TcpConnectErrorWithTimeSpan, remoteUri.AbsoluteUri, (int)socketException.SocketErrorCode, socketException.Message, timeSpent), innerException);
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
                    new EndpointNotFoundException(SR.Format(SR.UnableToResolveHost, uri.Host), socketException));
            }

            if (addresses.Length == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new EndpointNotFoundException(SR.Format(SR.UnableToResolveHost, uri.Host)));
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
                SR.Format(SR.TcpConnectingToViaTimedOut, uri.AbsoluteUri, timeout.ToString(),
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
                    new EndpointNotFoundException(SR.Format(SR.NoIPEndpointsFoundForHost, uri.Host)));
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
                    new EndpointNotFoundException(SR.Format(SR.NoIPEndpointsFoundForHost, uri.Host)));
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
