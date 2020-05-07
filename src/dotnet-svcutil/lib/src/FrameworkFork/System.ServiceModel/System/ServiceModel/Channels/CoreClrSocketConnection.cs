// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if FEATURE_CORECLR
using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Sockets;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class CoreClrSocketConnection : SocketConnection
    {
        private static EventHandler<SocketAsyncEventArgs> s_onReceiveAsyncCompleted;
        private static EventHandler<SocketAsyncEventArgs> s_onSocketSendCompleted;

        // common state
        private Socket _socket;
        private bool _noDelay = false;

        // close state
        private static Action<object> s_onWaitForFinComplete = new Action<object>(OnWaitForFinComplete);

        // read state
        private SocketAsyncEventArgs _asyncReadEventArgs;
        private TimeSpan _readFinTimeout;

        // write state
        private SocketAsyncEventArgs _asyncWriteEventArgs;

        private Timer _receiveTimer;
        private static TimerCallback s_onReceiveTimeout;
        private Timer _sendTimer;
        private static TimerCallback s_onSendTimeout;

        public CoreClrSocketConnection(Socket socket, ConnectionBufferPool connectionBufferPool)
            : base(connectionBufferPool)
        {
            if (socket == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("socket");
            }

            _closeState = CloseState.Open;
            _socket = socket;
            _socket.SendBufferSize = _socket.ReceiveBufferSize = _asyncReadBufferSize;
            _sendTimeout = _receiveTimeout = TimeSpan.MaxValue;
        }

        private Timer SendTimer
        {
            get
            {
                if (_sendTimer == null)
                {
                    if (s_onSendTimeout == null)
                    {
                        s_onSendTimeout = new TimerCallback(OnSendTimeout);
                    }

                    _sendTimer = new Timer(s_onSendTimeout, this, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
                }

                return _sendTimer;
            }
        }

        private Timer ReceiveTimer
        {
            get
            {
                if (_receiveTimer == null)
                {
                    if (s_onReceiveTimeout == null)
                    {
                        s_onReceiveTimeout = new TimerCallback(OnReceiveTimeout);
                    }

                    _receiveTimer = new Timer(s_onReceiveTimeout, this, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
                }

                return _receiveTimer;
            }
        }

        protected override IPEndPoint RemoteEndPoint
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

        private static void OnReceiveAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            ((CoreClrSocketConnection)e.UserToken).OnReceiveAsync(sender, e);
        }

        private static void OnSendAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            ((CoreClrSocketConnection)e.UserToken).OnSendAsync(sender, e);
        }

        private static void OnWaitForFinComplete(object state)
        {
            // Callback for read on a socket which has had Shutdown called on it. When
            // the response FIN packet is received from the remote host, the pending
            // read will complete with 0 bytes read. If more than 0 bytes has been read,
            // then something has gone wrong as we should have no pending data to be received.
            CoreClrSocketConnection thisPtr = (CoreClrSocketConnection)state;

            try
            {
                int bytesRead;

                try
                {
                    bytesRead = thisPtr.EndRead();

                    if (bytesRead > 0)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new CommunicationException(string.Format(SRServiceModel.SocketCloseReadReceivedData, thisPtr.RemoteEndPoint)));
                    }
                }
                catch (TimeoutException timeoutException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                        string.Format(SRServiceModel.SocketCloseReadTimeout, thisPtr.RemoteEndPoint, thisPtr._readFinTimeout),
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

        protected override void Abort(int traceEventType, string timeoutErrorString, TransferOperation transferOperation)
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
                    this.DisposeReadEventArgs();
                }

                if (_asyncWritePending)
                {
                    CancelSendTimer();
                }
                else
                {
                    this.DisposeWriteEventArgs();
                }
            }

            _socket.LingerState = new LingerOption(true, 0);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Dispose();
        }

        protected override void AbortRead()
        {
            lock (ThisLock)
            {
                if (_asyncReadPending)
                {
                    if (_closeState != CloseState.Closed)
                    {
                        this.SetUserToken(_asyncReadEventArgs, null);
                        _asyncReadPending = false;
                        CancelReceiveTimer();
                    }
                    else
                    {
                        this.DisposeReadEventArgs();
                    }
                }
            }
        }

        private void CancelReceiveTimer()
        {
            if (_receiveTimer != null)
            {
                _receiveTimer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
            }
        }

        private void CancelSendTimer()
        {
            if (_sendTimer != null)
            {
                _sendTimer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
            }
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
                        new CommunicationException(string.Format(SRServiceModel.SocketCloseReadReceivedData, RemoteEndPoint)));
                }
            }
            catch (TimeoutException timeoutException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                    string.Format(SRServiceModel.SocketCloseReadTimeout, RemoteEndPoint, _readFinTimeout), timeoutException));
            }

            ContinueClose(_closeTimeoutHelper.RemainingTime());
        }

        protected override void CloseCore(bool asyncAndLinger)
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
                        new CommunicationException(string.Format(SRServiceModel.SocketCloseReadReceivedData, RemoteEndPoint)));
                }
            }
            catch (TimeoutException timeoutException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                    string.Format(SRServiceModel.SocketCloseReadTimeout, RemoteEndPoint, _readFinTimeout), timeoutException));
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
                        this.DisposeReadEventArgs();
                    }

                    if (!_asyncWritePending)
                    {
                        this.DisposeWriteEventArgs();
                    }
                }

                _closeState = CloseState.Closed;
            }
        }

        protected override void ShutdownCore(TimeSpan timeout)
        {
            // Attempt to close the socket gracefully by sending a shutdown (FIN) packet
            try
            {
                _socket.Shutdown(SocketShutdown.Send);
            }
            catch (SocketException socketException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    ConvertSendException(socketException, TimeSpan.MaxValue));
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Exception exceptionToThrow = ConvertObjectDisposedException(objectDisposedException, TransferOperation.Undefined);
                if (object.ReferenceEquals(exceptionToThrow, objectDisposedException))
                {
                    throw;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exceptionToThrow);
                }
            }
        }

        protected override AsyncCompletionResult BeginWriteCore(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout,
            Action<object> callback, object state)
        {
            ConnectionUtilities.ValidateBufferBounds(buffer, offset, size);
            bool abortWrite = true;

            try
            {
                lock (ThisLock)
                {
                    Contract.Assert(!_asyncWritePending, "Called BeginWrite twice.");
                    this.ThrowIfClosed();
                    this.EnsureWriteEventArgs();
                    SetImmediate(immediate);
                    SetWriteTimeout(timeout, false);
                    this.SetUserToken(_asyncWriteEventArgs, this);
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

                this.HandleSendAsyncCompleted();
                abortWrite = false;
                return AsyncCompletionResult.Completed;
            }
            catch (SocketException socketException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    ConvertSendException(socketException, TimeSpan.MaxValue));
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Exception exceptionToThrow = ConvertObjectDisposedException(objectDisposedException, TransferOperation.Write);
                if (object.ReferenceEquals(exceptionToThrow, objectDisposedException))
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
                    this.AbortWrite();
                }
            }
        }

        protected override void EndWriteCore()
        {
            if (_asyncWriteException != null)
            {
                this.AbortWrite();
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(_asyncWriteException);
            }

            lock (ThisLock)
            {
                if (!_asyncWritePending)
                {
                    Contract.Assert(false, "SocketConnection.EndWrite called with no write pending.");
                    throw new Exception("SocketConnection.EndWrite called with no write pending.");
                }

                this.SetUserToken(_asyncWriteEventArgs, null);
                _asyncWritePending = false;

                if (_closeState == CloseState.Closed)
                {
                    this.DisposeWriteEventArgs();
                }
            }
        }

        private void OnSendAsync(object sender, SocketAsyncEventArgs eventArgs)
        {
            Contract.Assert(eventArgs != null, "Argument 'eventArgs' cannot be NULL.");
            this.CancelSendTimer();

            try
            {
                this.HandleSendAsyncCompleted();
                Contract.Assert(eventArgs.BytesTransferred == _asyncWriteEventArgs.Count, "The socket SendAsync did not send all the bytes.");
            }
            catch (SocketException socketException)
            {
                _asyncWriteException = ConvertSendException(socketException, TimeSpan.MaxValue);
            }
            catch (Exception exception)
            {
                if (Fx.IsFatal(exception))
                {
                    throw;
                }

                _asyncWriteException = exception;
            }

            this.FinishWrite();
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
                this._asyncWriteEventArgs.Completed -= s_onSocketSendCompleted;
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
                        this.SetUserToken(_asyncWriteEventArgs, null);
                        _asyncWritePending = false;
                        this.CancelSendTimer();
                    }
                    else
                    {
                        this.DisposeWriteEventArgs();
                    }
                }
            }
        }

        protected override void WriteCore(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout)
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
                    ConvertSendException(socketException, timeoutHelper.RemainingTime()));
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Exception exceptionToThrow = ConvertObjectDisposedException(objectDisposedException, TransferOperation.Write);
                if (object.ReferenceEquals(exceptionToThrow, objectDisposedException))
                {
                    throw;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exceptionToThrow);
                }
            }
        }

        protected override void TraceWriteStart(int size, bool async)
        {
            Contract.Assert(_socket != null);
            var remoteEndpoint = (IPEndPoint)_socket.RemoteEndPoint;
            string remoteEndpointAddressString = remoteEndpoint.Address + ":" + remoteEndpoint.Port;
            if (!async)
            {
                WcfEventSource.Instance.SocketWriteStart(_socket.GetHashCode(), size, remoteEndpointAddressString);
            }
            else
            {
                WcfEventSource.Instance.SocketAsyncWriteStart(_socket.GetHashCode(), size, remoteEndpointAddressString);
            }
        }

        protected override int ReadCore(byte[] buffer, int offset, int size, TimeSpan timeout, bool closing)
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
                    ConvertReceiveException(socketException, timeoutHelper.RemainingTime()));
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Exception exceptionToThrow = ConvertObjectDisposedException(objectDisposedException, TransferOperation.Read);
                if (object.ReferenceEquals(exceptionToThrow, objectDisposedException))
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

        protected override void TraceSocketReadStop(int bytesRead, bool async)
        {
            IPEndPoint remote = RemoteEndPoint;
            string remoteEndpointAddressString = string.Empty;
            if (remote != null)
            {
                remoteEndpointAddressString = remote.Address + ":" + remote.Port;
            }

            if (!async)
            {
                WcfEventSource.Instance.SocketReadStop((_socket != null) ? _socket.GetHashCode() : -1, bytesRead, remoteEndpointAddressString);
            }
            else
            {
                WcfEventSource.Instance.SocketAsyncReadStop((_socket != null) ? _socket.GetHashCode() : -1, bytesRead, remoteEndpointAddressString);
            }
        }

        protected override AsyncCompletionResult BeginReadCore(int offset, int size, TimeSpan timeout,
            Action<object> callback, object state)
        {
            bool abortRead = true;

            lock (ThisLock)
            {
                this.ThrowIfClosed();
                this.EnsureReadEventArgs();
                _asyncReadState = state;
                _asyncReadCallback = callback;
                this.SetUserToken(_asyncReadEventArgs, this);
                _asyncReadPending = true;
                this.SetReadTimeout(timeout, false, false);
            }

            try
            {
                if (offset != _asyncReadEventArgs.Offset ||
                    size != _asyncReadEventArgs.Count)
                {
                    _asyncReadEventArgs.SetBuffer(offset, size);
                }

                if (this.ReceiveAsync())
                {
                    abortRead = false;
                    return AsyncCompletionResult.Queued;
                }

                this.HandleReceiveAsyncCompleted();
                _asyncReadSize = _asyncReadEventArgs.BytesTransferred;

                abortRead = false;
                return AsyncCompletionResult.Completed;
            }
            catch (SocketException socketException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertReceiveException(socketException, TimeSpan.MaxValue));
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Exception exceptionToThrow = ConvertObjectDisposedException(objectDisposedException, TransferOperation.Read);
                if (object.ReferenceEquals(exceptionToThrow, objectDisposedException))
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
            this.CancelReceiveTimer();

            try
            {
                this.HandleReceiveAsyncCompleted();
                _asyncReadSize = eventArgs.BytesTransferred;
            }
            catch (SocketException socketException)
            {
                _asyncReadException = ConvertReceiveException(socketException, TimeSpan.MaxValue);
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

        // Both BeginRead/ReadAsync paths completed themselves. EndRead's only job is to deliver the result.
        protected override int EndReadCore()
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

                this.SetUserToken(_asyncReadEventArgs, null);
                _asyncReadPending = false;

                if (_closeState == CloseState.Closed)
                {
                    this.DisposeReadEventArgs();
                }
            }

            return _asyncReadSize;
        }

        // This method should be called inside ThisLock
        private void DisposeReadEventArgs()
        {
            if (_asyncReadEventArgs != null)
            {
                this._asyncReadEventArgs.Completed -= s_onReceiveAsyncCompleted;
                _asyncReadEventArgs.Dispose();
            }

            // We release the buffer only if there is no outstanding I/O
            this.TryReturnReadBuffer();
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
                        new TimeoutException(string.Format(SRServiceModel.TcpConnectionTimedOut, timeout)));
                }

                if (UpdateTimeout(_receiveTimeout, timeout))
                {
                    lock (ThisLock)
                    {
                        if (!closing || _closeState != CloseState.Closing)
                        {
                            ThrowIfNotOpen();
                        }
                        _socket.ReceiveTimeout = TimeoutHelper.ToMilliseconds(timeout);
                    }
                    _receiveTimeout = timeout;
                }
            }
            else
            {
                _receiveTimeout = timeout;
                if (timeout == TimeSpan.MaxValue)
                {
                    CancelReceiveTimer();
                }
                else
                {
                    ReceiveTimer.Change(timeout, TimeSpan.FromMilliseconds(-1));
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
                        new TimeoutException(string.Format(SRServiceModel.TcpConnectionTimedOut, timeout)));
                }

                if (UpdateTimeout(_sendTimeout, timeout))
                {
                    lock (ThisLock)
                    {
                        ThrowIfNotOpen();
                        _socket.SendTimeout = TimeoutHelper.ToMilliseconds(timeout);
                    }
                    _sendTimeout = timeout;
                }
            }
            else
            {
                _sendTimeout = timeout;
                if (timeout == TimeSpan.MaxValue)
                {
                    CancelSendTimer();
                }
                else
                {
                    SendTimer.Change(timeout, TimeSpan.FromMilliseconds(-1));
                }
            }
        }

        private bool UpdateTimeout(TimeSpan oldTimeout, TimeSpan newTimeout)
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
                this._asyncReadEventArgs.Completed += s_onReceiveAsyncCompleted;
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
                this._asyncWriteEventArgs.Completed += s_onSocketSendCompleted;
            }
        }
    }

    internal class CoreClrSocketConnectionInitiator : SocketConnectionInitiator
    {
        public CoreClrSocketConnectionInitiator(int bufferSize) : base(bufferSize) { }

        protected override IConnection CreateConnection(IPAddress address, int port)
        {
            Socket socket = null;
            try
            {
                AddressFamily addressFamily = address.AddressFamily;
                socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(address, port));
                return new CoreClrSocketConnection(socket, _connectionBufferPool);
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        }

        protected override async Task<IConnection> CreateConnectionAsync(IPAddress address, int port)
        {
            Socket socket = null;
            try
            {
                AddressFamily addressFamily = address.AddressFamily;
                socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(new IPEndPoint(address, port));
                return new CoreClrSocketConnection(socket, _connectionBufferPool);
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        }
    }
}
#endif // FEATURE_CORECLR
