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
    internal class SocketConnection : IConnection
    {
        private static EventHandler<SocketAsyncEventArgs> s_onReceiveAsyncCompleted;
        private static EventHandler<SocketAsyncEventArgs> s_onSocketSendCompleted;

        // common state
        private Socket _socket;
        private bool _noDelay = false;
        private TimeSpan _asyncSendTimeout;
        private TimeSpan _asyncReceiveTimeout;

        // Socket.SendTimeout/Socket.ReceiveTimeout only work with the synchronous API calls and therefore they
        // do not get updated when asynchronous Send/Read operations are performed.  In order to make sure we 
        // Set the proper timeouts on the Socket itself we need to keep these two additional fields.
        private TimeSpan _socketSyncSendTimeout;
        private TimeSpan _socketSyncReceiveTimeout;

        private CloseState _closeState;
        private bool _aborted;

        // close state
        private static Action<object> s_onWaitForFinComplete = new Action<object>(OnWaitForFinComplete);
        private TimeoutHelper _closeTimeoutHelper;
        private bool _isShutdown;

        // read state
        private SocketAsyncEventArgs _asyncReadEventArgs;
        private TimeSpan _readFinTimeout;
        private int _asyncReadSize;
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

        private static Action<SocketConnection> s_onSendTimeout;
        private static Action<SocketConnection> s_onReceiveTimeout;
        private IOTimer<SocketConnection> _receiveTimer;
        private IOTimer<SocketConnection> _sendTimer;
        private string _timeoutErrorString;
        private TransferOperation _timeoutErrorTransferOperation;
        private ConnectionBufferPool _connectionBufferPool;
        private string _remoteEndpointAddressString;

        public SocketConnection(Socket socket, ConnectionBufferPool connectionBufferPool)
        {
            _connectionBufferPool = connectionBufferPool ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(connectionBufferPool));
            _socket = socket ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(socket));
            _closeState = CloseState.Open;
            AsyncReadBuffer = _connectionBufferPool.Take();
            AsyncReadBufferSize = AsyncReadBuffer.Length;
            _closeState = CloseState.Open;
            _socket.SendBufferSize = _socket.ReceiveBufferSize = AsyncReadBufferSize;
            _asyncSendTimeout = _asyncReceiveTimeout = TimeSpan.MaxValue;
            _socketSyncSendTimeout = _socketSyncReceiveTimeout = TimeSpan.MaxValue;
        }

        public int AsyncReadBufferSize { get; }

        public byte[] AsyncReadBuffer { get; private set; }

        private object ThisLock
        {
            get { return this; }
        }

        private IOTimer<SocketConnection> SendTimer
        {
            get
            {
                if (_sendTimer == null)
                {
                    if (s_onSendTimeout == null)
                    {
                        s_onSendTimeout = OnSendTimeout;
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
                        s_onReceiveTimeout = OnReceiveTimeout;
                    }

                    _receiveTimer = new IOTimer<SocketConnection>(s_onReceiveTimeout, this);
                }

                return _receiveTimer;
            }
        }

        private IPEndPoint RemoteEndPoint
        {
            get
            {
                if (!_socket.Connected)
                {
                    return null;
                }

                return (IPEndPoint)_socket.RemoteEndPoint;
            }
        }

        private string RemoteEndpointAddressString
        {
            get
            {
                if (_remoteEndpointAddressString == null)
                {
                    IPEndPoint remote = RemoteEndPoint;
                    if (remote == null)
                    {
                        return string.Empty;
                    }
                    _remoteEndpointAddressString = remote.Address + ":" + remote.Port;
                }

                return _remoteEndpointAddressString;
            }
        }

        private static void OnReceiveAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            ((SocketConnection)e.UserToken).OnReceiveAsync(sender, e);
        }

        private static void OnSendAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            ((SocketConnection)e.UserToken).OnSendAsync(sender, e);
        }

        private static void OnWaitForFinComplete(object state)
        {
            // Callback for read on a socket which has had Shutdown called on it. When
            // the response FIN packet is received from the remote host, the pending
            // read will complete with 0 bytes read. If more than 0 bytes has been read,
            // then something has gone wrong as we should have no pending data to be received.
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
                            new CommunicationException(SR.Format(SR.SocketCloseReadReceivedData, thisPtr.RemoteEndPoint)));
                    }
                }
                catch (TimeoutException timeoutException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                        SR.Format(SR.SocketCloseReadTimeout, thisPtr.RemoteEndPoint, thisPtr._readFinTimeout),
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

                Fx.Exception.TraceUnhandledException(e);

                // The user has no opportunity to clean up the connection in the async and linger
                // code path, ensure cleanup finishes.
                thisPtr.Abort();
            }
        }

        private static void OnReceiveTimeout(SocketConnection socketConnection)
        {
            try
            {
                socketConnection.Abort(SR.Format(SR.SocketAbortedReceiveTimedOut, socketConnection._asyncReceiveTimeout), TransferOperation.Read);
            }
            catch (SocketException)
            {
                // Guard against unhandled SocketException in timer callbacks
            }
        }

        private static void OnSendTimeout(SocketConnection socketConnection)
        {
            try
            {
                socketConnection.Abort(4,	// TraceEventType.Warning
                    SR.Format(SR.SocketAbortedSendTimedOut, socketConnection._asyncSendTimeout), TransferOperation.Write);
            }
            catch (SocketException)
            {
                // Guard against unhandled SocketException in timer callbacks
            }
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

                if (!_asyncReadPending)
                {
                    DisposeReadEventArgs();
                }

                if (!_asyncWritePending)
                {
                    DisposeWriteEventArgs();
                }

                DisposeReceiveTimer();
                DisposeSendTimer();
            }

            _socket.LingerState = new LingerOption(true, 0);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Dispose();
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

                // Any NetTcp session handshake will have been completed at this point so if any data is returned, something
                // very wrong has happened.
                if (bytesRead > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new CommunicationException(SR.Format(SR.SocketCloseReadReceivedData, RemoteEndPoint)));
                }
            }
            catch (TimeoutException timeoutException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                    SR.Format(SR.SocketCloseReadTimeout, RemoteEndPoint, _readFinTimeout), timeoutException));
            }

            ContinueClose(_closeTimeoutHelper.RemainingTime());
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

            _closeTimeoutHelper = new TimeoutHelper(timeout);

            // first we shutdown our send-side
            Shutdown(timeout);
            CloseCore(asyncAndLinger);
        }

        private void CloseCore(bool asyncAndLinger)
        {
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

            // A FIN (shutdown) packet has already been sent to the remote host and we're waiting for the remote
            // host to send a FIN back. A pending read on a socket will complete returning zero bytes when a FIN
            // packet is received.

            int bytesRead;
            _readFinTimeout = _closeTimeoutHelper.RemainingTime();

            try
            {
                bytesRead = ReadCore(dummy, 0, 1, _readFinTimeout, true);

                // Any NetTcp session handshake will have been completed at this point so if any data is returned, something
                // very wrong has happened.
                if (bytesRead > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new CommunicationException(SR.Format(SR.SocketCloseReadReceivedData, RemoteEndPoint)));
                }
            }
            catch (TimeoutException timeoutException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                    SR.Format(SR.SocketCloseReadTimeout, RemoteEndPoint, _readFinTimeout), timeoutException));
            }

            // finally we call Close with whatever time is remaining
            ContinueClose(_closeTimeoutHelper.RemainingTime());
        }

        private void ContinueClose(TimeSpan timeout)
        {
            // Use linger to attempt a graceful socket shutdown. Allowing a clean shutdown handshake
            // will allow the service side to close it's socket gracefully too. A hard shutdown would
            // cause the server to receive an exception which affects performance and scalability.
            _socket.LingerState = new LingerOption(true, (int)timeout.TotalSeconds);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Dispose();

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
                DisposeReceiveTimer();
                DisposeSendTimer();
            }
        }

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

        private void ShutdownCore(TimeSpan timeout)
        {
            // Attempt to close the socket gracefully by sending a shutdown (FIN) packet
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

        private Exception ConvertSendException(SocketException socketException, TimeSpan remainingTime, TimeSpan timeout)
        {
            return ConvertTransferException(socketException, timeout, socketException,
                _aborted, _timeoutErrorString, _timeoutErrorTransferOperation, this, remainingTime);
        }

        private Exception ConvertReceiveException(SocketException socketException, TimeSpan remainingTime, TimeSpan timeout)
        {
            return ConvertTransferException(socketException, timeout, socketException,
                _aborted, _timeoutErrorString, _timeoutErrorTransferOperation, this, remainingTime);
        }

        internal static Exception ConvertTransferException(SocketException socketException, TimeSpan timeout, Exception originalException)
        {
            return ConvertTransferException(socketException, timeout, originalException,
                false, null, TransferOperation.Undefined, null, TimeSpan.MaxValue);
        }

        private Exception ConvertObjectDisposedException(ObjectDisposedException originalException, TransferOperation transferOperation)
        {
            if (_timeoutErrorString != null)
            {
                return ConvertTimeoutErrorException(originalException, _timeoutErrorString, _timeoutErrorTransferOperation);
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
            bool aborted, string timeoutErrorString, TransferOperation timeoutErrorTransferOperation,
            SocketConnection socketConnection, TimeSpan remainingTime)
        {
            if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.ERROR_INVALID_HANDLE)
            {
                return new CommunicationObjectAbortedException(socketException.Message, socketException);
            }

            if (timeoutErrorString != null)
            {
                return ConvertTimeoutErrorException(originalException, timeoutErrorString, timeoutErrorTransferOperation);
            }

            // 10053 can occur due to our timeout sockopt firing, so map to TimeoutException in that case
            if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAECONNABORTED &&
                remainingTime <= TimeSpan.Zero)
            {
                TimeoutException timeoutException = new TimeoutException(SR.Format(SR.TcpConnectionTimedOut, timeout), originalException);
                return timeoutException;
            }

            if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAENETRESET ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAECONNABORTED ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAECONNRESET)
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
            else if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAETIMEDOUT)
            {
                TimeoutException timeoutException = new TimeoutException(SR.Format(SR.TcpConnectionTimedOut, timeout), originalException);
                return timeoutException;
            }
            else
            {
                if (aborted)
                {
                    return new CommunicationObjectAbortedException(SR.Format(SR.TcpTransferError, (int)socketException.SocketErrorCode, socketException.Message), originalException);
                }
                else
                {
                    CommunicationException communicationException = new CommunicationException(SR.Format(SR.TcpTransferError, (int)socketException.SocketErrorCode, socketException.Message), originalException);
                    return communicationException;
                }
            }
        }

        private static Exception ConvertTimeoutErrorException(Exception originalException, string timeoutErrorString, TransferOperation timeoutErrorTransferOperation)
        {
            Contract.Assert(timeoutErrorString != null, "Argument timeoutErrorString must not be null.");

            if (timeoutErrorTransferOperation != TransferOperation.Undefined)
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

        private void TraceWriteStart(int size, bool async)
        {
            if (!async)
            {
                WcfEventSource.Instance.SocketWriteStart(_socket.GetHashCode(), size, RemoteEndpointAddressString);
            }
            else
            {
                WcfEventSource.Instance.SocketAsyncWriteStart(_socket.GetHashCode(), size, RemoteEndpointAddressString);
            }
        }

        private AsyncCompletionResult BeginWriteCore(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout,
            Action<object> callback, object state)
        {
            ConnectionUtilities.ValidateBufferBounds(buffer, offset, size);
            bool abortWrite = true;

            try
            {
                lock (ThisLock)
                {
                    Contract.Assert(!_asyncWritePending, "Called BeginWrite twice.");
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
            EndWriteCore();
        }

        private void EndWriteCore()
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
                    Contract.Assert(false, "SocketConnection.EndWrite called with no write pending.");
                    throw new Exception("SocketConnection.EndWrite called with no write pending.");
                }

                SetUserToken(_asyncWriteEventArgs, null);
                _asyncWritePending = false;

                if (_closeState == CloseState.Closed)
                {
                    DisposeWriteEventArgs();
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
            WriteCore(buffer, offset, size, immediate, timeout);
        }

        private void WriteCore(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout)
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

        public virtual AsyncCompletionResult BeginRead(int offset, int size, TimeSpan timeout,
            Action<object> callback, object state)
        {
            ConnectionUtilities.ValidateBufferBounds(AsyncReadBufferSize, offset, size);
            ThrowIfNotOpen();
            var completionResult = BeginReadCore(offset, size, timeout, callback, state);
            if (completionResult == AsyncCompletionResult.Completed && WcfEventSource.Instance.SocketReadStopIsEnabled())
            {
                TraceSocketReadStop(_asyncReadSize, true);
            }

            return completionResult;
        }

        private void TraceSocketReadStop(int bytesRead, bool async)
        {
            if (!async)
            {
                WcfEventSource.Instance.SocketReadStop((_socket != null) ? _socket.GetHashCode() : -1, bytesRead, RemoteEndpointAddressString);
            }
            else
            {
                WcfEventSource.Instance.SocketAsyncReadStop((_socket != null) ? _socket.GetHashCode() : -1, bytesRead, RemoteEndpointAddressString);
            }
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

        private void OnReceiveAsync(object sender, SocketAsyncEventArgs eventArgs)
        {
            Contract.Assert(eventArgs != null, "Argument 'eventArgs' cannot be NULL.");
            CancelReceiveTimer();

            try
            {
                HandleReceiveAsyncCompleted();
                _asyncReadSize = eventArgs.BytesTransferred;
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

        // Both BeginRead/ReadAsync paths completed themselves. EndRead's only job is to deliver the result.
        private int EndReadCore()
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
                    Contract.Assert(false, "SocketConnection.EndRead called with no read pending.");
                    throw new Exception("SocketConnection.EndRead called with no read pending.");
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

        // This method should be called inside ThisLock
        private void DisposeReceiveTimer()
        {
            if (_receiveTimer != null)
            {
                _receiveTimer.Dispose();
            }
        }

        private void OnSendAsync(object sender, SocketAsyncEventArgs eventArgs)
        {
            Contract.Assert(eventArgs != null, "Argument 'eventArgs' cannot be NULL.");
            CancelSendTimer();

            try
            {
                HandleSendAsyncCompleted();
                Contract.Assert(eventArgs.BytesTransferred == _asyncWriteEventArgs.Count, "The socket SendAsync did not send all the bytes.");
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

        // This method should be called inside ThisLock
        private void DisposeSendTimer()
        {
            if (_sendTimer != null)
            {
                _sendTimer.Dispose();
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

        // This method should be called inside ThisLock
        private void ReturnReadBuffer()
        {
            // We release the buffer only if there is no outstanding I/O
            TryReturnReadBuffer();
        }

        // This method should be called inside ThisLock
        private void TryReturnReadBuffer()
        {
            // The buffer must not be returned and nulled when an abort occurs. Since the buffer
            // is also accessed by higher layers, code that has not yet realized the stack is
            // aborted may be attempting to read from the buffer.
            if (AsyncReadBuffer != null && !_aborted)
            {
                _connectionBufferPool.Return(AsyncReadBuffer);
                AsyncReadBuffer = null;
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
                _asyncReadEventArgs.SetBuffer(AsyncReadBuffer, 0, AsyncReadBuffer.Length);
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

        public object GetCoreTransport()
        {
            return _socket;
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
                return new SocketConnection(socket, _connectionBufferPool);
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
                return new SocketConnection(socket, _connectionBufferPool);
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
