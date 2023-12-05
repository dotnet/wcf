// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Sockets;
using System.Net;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing;

namespace System.ServiceModel.Channels
{
    internal class SocketConnection : IConnection
    {
        //// common state
        private Socket _socket;
        private TimeSpan _asyncSendTimeout;
        private TimeSpan _readFinTimeout;
        private TimeSpan _asyncReceiveTimeout;
        private CloseState _closeState;
        private bool _isShutdown;
        private bool _aborted;

        //// close state
        private TimeoutHelper _closeTimeoutHelper;
        //private static Action<object> s_onWaitForFinComplete = new Action<object>(OnWaitForFinComplete);

        //// read state
        //private int _asyncReadSize;
        private SocketAwaitableEventArgs _asyncReadEventArgs;
        private bool _asyncReadPending;

        //// write state
        private SocketAwaitableEventArgs _asyncWriteEventArgs;
        private bool _asyncWritePending;

        private Timer _receiveTimer;
        private bool _receiveTimerEnabled;
        private DateTime _lastReceiveTimeoutDeadline;
        private static TimerCallback s_onReceiveTimeout = OnReceiveTimeout;
        private Timer _sendTimer;
        private bool _sendTimerEnabled;
        private DateTime _lastSendTimeoutDeadline;
        private static TimerCallback s_onSendTimeout = OnSendTimeout;
        private string _timeoutErrorString;
        private TransferOperation _timeoutErrorTransferOperation;
        private UnixDomainSocketEndPoint _remoteEndpoint;
        //private ConnectionBufferPool _connectionBufferPool;
        private string _remoteEndpointAddress;

        public SocketConnection(Socket socket, int bufferSize)
        {
            _socket = socket ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(socket));
            ConnectionBufferSize = bufferSize;
            _closeState = CloseState.Open;
            _socket.SendBufferSize = _socket.ReceiveBufferSize = bufferSize;
            _sendTimer = CreateTimer(s_onSendTimeout);
            _receiveTimer = CreateTimer(s_onReceiveTimeout);
            _asyncReadEventArgs = new SocketAwaitableEventArgs();
            _asyncWriteEventArgs = new SocketAwaitableEventArgs();
            _remoteEndpoint = null;
        }

        private object ThisLock => this;

        public int ConnectionBufferSize { get; }

        public void Abort()
        {
            Abort(null, TransferOperation.Undefined);
        }

        private void Abort(string timeoutErrorString, TransferOperation transferOperation)
        {
            // we could be timing out a cached connection
            Abort(TraceEventType.Warning, timeoutErrorString, transferOperation);
        }

        private void Abort(TraceEventType traceEventType, string timeoutErrorString, TransferOperation transferOperation)
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
                        _asyncReadPending = false;
                        CancelReceiveTimer(dispose: true);
                    }
                    else
                    {
                        DisposeReadEventArgs();
                    }
                }
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
                        _asyncWritePending = false;
                        CancelSendTimer(dispose: true);
                    }
                    else
                    {
                        DisposeWriteEventArgs();
                    }
                }
            }
        }

        public ValueTask<int> ReadAsync(Memory<byte> buffer, TimeSpan timeout)
        {
            if (buffer.Length < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(buffer.Length), buffer.Length, SR.ValueMustBeNonNegative));
            }
            ThrowIfNotOpen();

            return ReadCoreAsync(buffer, timeout, false);
        }

        private async ValueTask<int> ReadCoreAsync(Memory<byte> buffer, TimeSpan timeout, bool closing)
        {
            int bytesRead = 0;
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            bool abortRead = true;

            lock (ThisLock)
            {
                _asyncReadPending = true;
                SetReadTimeout(timeout, closing);
            }

            bool restoreFlow = false;
            try
            {
                if (!ExecutionContext.IsFlowSuppressed())
                {
                    ExecutionContext.SuppressFlow();
                    restoreFlow = true;
                }

                var resultTask = _asyncReadEventArgs.ReceiveAsync(_socket, buffer);
                if (restoreFlow)
                {
                    restoreFlow = false;
                    ExecutionContext.RestoreFlow();
                }
                bytesRead = await resultTask;
                abortRead = false;
                if (WcfEventSource.Instance.SocketReadStopIsEnabled())
                {
                    WcfEventSource.Instance.SocketAsyncReadStop((_socket != null) ? _socket.GetHashCode() : -1, bytesRead, this.RemoteEndpointAddress);
                }
            }
            catch (SocketException socketException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    ConvertReceiveException(socketException, timeoutHelper.RemainingTime(), timeout));
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Exception exceptionToThrow = ConvertObjectDisposedException(objectDisposedException, TransferOperation.Read);
                if (ReferenceEquals(exceptionToThrow, objectDisposedException))
                    throw;
                else
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exceptionToThrow);
            }
            finally
            {
                CancelReceiveTimer();
                // Restore the current ExecutionContext
                if (restoreFlow)
                    ExecutionContext.RestoreFlow();

                if (abortRead)
                {
                    AbortRead();
                }
            }

            lock (ThisLock)
            {
                _asyncReadPending = false;
                if (_closeState == CloseState.Closed)
                {
                    DisposeReadEventArgs();
                }
            }

            return bytesRead;
        }

        public async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, bool immediate, TimeSpan timeout)
        {
            if (buffer.Length <= 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(buffer.Length), buffer.Length, SR.ValueMustBePositive));
            }
            bool abortWrite = true;

            if (WcfEventSource.Instance.SocketAsyncWriteStartIsEnabled())
            {
                WcfEventSource.Instance.SocketAsyncWriteStart(_socket.GetHashCode(), buffer.Length, RemoteEndpointAddress);
            }

            lock (ThisLock)
            {
                Fx.Assert(!_asyncWritePending, "Called BeginWrite twice.");
                ThrowIfClosed();
                SetWriteTimeout(timeout);
                _asyncWritePending = true;
            }

            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            bool restoreFlow = false;
            try
            {
                if (!ExecutionContext.IsFlowSuppressed())
                {
                    ExecutionContext.SuppressFlow();
                    restoreFlow = true;
                }

                var resultTask = _asyncWriteEventArgs.SendAsync(_socket, buffer);
                if (restoreFlow)
                {
                    restoreFlow = false;
                    ExecutionContext.RestoreFlow();
                }

                await resultTask;
                abortWrite = false;
            }
            catch (SocketException socketException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    ConvertSendException(socketException, timeoutHelper.RemainingTime(), timeout));
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
                _asyncWritePending = false;
                CancelSendTimer();
                // Restore the current ExecutionContext
                if (restoreFlow)
                    ExecutionContext.RestoreFlow();

                if (abortWrite)
                {
                    AbortWrite();
                }
            }
        }

        public async ValueTask CloseAsync(TimeSpan timeout)
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
            _readFinTimeout = _closeTimeoutHelper.RemainingTime();

            try
            {
                // A FIN (shutdown) packet has already been sent to the remote host and we're waiting for the remote
                // host to send a FIN back. A pending read on a socket will complete returning zero bytes when a FIN
                // packet is received.
                byte[] dummy = Fx.AllocateByteArray(1);
                int bytesRead = await ReadCoreAsync(dummy, _readFinTimeout, true);

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

            _socket.Close(TimeoutHelper.ToMilliseconds(_closeTimeoutHelper.RemainingTime()));

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

            try
            {
                _socket.Shutdown(SocketShutdown.Send);
            }
            catch (SocketException socketException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    ConvertSendException(socketException, TimeSpan.MaxValue, _asyncSendTimeout));
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

        private string RemoteEndpointAddress
        {
            get
            {
                if (_remoteEndpointAddress == null)
                {
                    try
                    {
                        if (TryGetEndpoints(out UnixDomainSocketEndPoint remote))
                        {
                            _remoteEndpointAddress = remote.ToString();
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

        private bool TryGetEndpoints(out UnixDomainSocketEndPoint remoteIPEndpoint)
        {
            remoteIPEndpoint = null;

            if (_closeState == CloseState.Open)
            {
                try
                {
                    remoteIPEndpoint = _remoteEndpoint ?? (UnixDomainSocketEndPoint)_socket.RemoteEndPoint;
                }
                catch (Exception exception)
                {
                    if (Fx.IsFatal(exception))
                    {
                        throw;
                    }
                }
            }

            return remoteIPEndpoint != null;
        }

        // This method should be called inside ThisLock
        private void DisposeReadEventArgs()
        {
            _asyncReadEventArgs.Dispose();
        }

        // This method should be called inside ThisLock
        private void DisposeWriteEventArgs()
        {
            _asyncWriteEventArgs.Dispose();
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

        private void SetReadTimeout(TimeSpan timeout, bool closing)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new TimeoutException(SR.Format(SR.UDSConnectionTimedOut, timeout)));
            }

            _asyncReceiveTimeout = timeout;
            if (timeout == TimeSpan.MaxValue)
            {
                CancelReceiveTimer();
            }
            else
            {
                if (ShouldUpdateTimeout(_lastReceiveTimeoutDeadline, timeout))
                {
                    lock (ThisLock)
                    {
                        if (!closing || _closeState != CloseState.Closing)
                        {
                            ThrowIfNotOpen();
                        }
                        _lastReceiveTimeoutDeadline = DateTime.UtcNow + timeout;
                        _receiveTimer.Change(timeout, Timeout.InfiniteTimeSpan);
                    }
                }

                _receiveTimerEnabled = true;
            }
        }

        private void CancelReceiveTimer(bool dispose = false)
        {
            // Don't change the timer as modifing the Timer is expensive. It's most likely going to be set
            // to another future time before it actually fires. By using a bool to make the fired timer a
            // no-op we only modify the Timer, if at all. Multiple receives are often part of a single
            // compound operation with the same ultimate final timeout deadline. This means multiple calls
            // to ReceiveAsync will have the same deadline and we can avoid modifying the timer by not
            // changing it when a receive completes and just making it a no-op.
            if (_receiveTimerEnabled)
            {
                _receiveTimerEnabled = false;
            }

            if (dispose)
            {
                _receiveTimer.Dispose();
            }
        }

        private void SetWriteTimeout(TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new TimeoutException(SR.Format(SR.UDSConnectionTimedOut, timeout)));
            }

            _asyncSendTimeout = timeout;
            if (timeout == TimeSpan.MaxValue)
            {
                CancelSendTimer();
            }
            else
            {
                if (ShouldUpdateTimeout(_lastSendTimeoutDeadline, timeout))
                {
                    lock (ThisLock)
                    {
                        ThrowIfNotOpen();
                        _lastSendTimeoutDeadline = DateTime.UtcNow + timeout;
                        _sendTimer.Change(timeout, Timeout.InfiniteTimeSpan);
                    }
                }

                _sendTimerEnabled = true;
            }
        }

        private void CancelSendTimer(bool dispose = false)
        {
            // See CancelReceiveTimer for rationale of not cancelling the underlying timer.
            if (_sendTimerEnabled)
            {
                _sendTimerEnabled = false;
            }

            if (dispose)
            {
                _sendTimer.Dispose();
            }
        }

        private bool ShouldUpdateTimeout(DateTime oldTimeoutDeadline, TimeSpan newTimeout)
        {
            var oldTimeout = oldTimeoutDeadline - DateTime.UtcNow;
            if (oldTimeout < TimeSpan.Zero) // Expired already
                return true;

            long threshold = oldTimeout.Ticks >> 4; // >> 4 is the same as / 16. Are the timeouts within 6.25% of each other
            long delta = oldTimeout.Ticks > newTimeout.Ticks ? oldTimeout.Ticks - newTimeout.Ticks : newTimeout.Ticks - oldTimeout.Ticks;

            return delta > threshold;
        }

        private static void OnReceiveTimeout(object state)
        {
            SocketConnection thisPtr = (SocketConnection)state;
            if (thisPtr._receiveTimerEnabled)
            {
                thisPtr.Abort(SR.Format(SR.SocketAbortedReceiveTimedOut, thisPtr._asyncReceiveTimeout), TransferOperation.Read);
            }
        }

        private static void OnSendTimeout(object state)
        {
            SocketConnection thisPtr = (SocketConnection)state;
            if (thisPtr._sendTimerEnabled)
            {
                thisPtr.Abort(TraceEventType.Warning, SR.Format(SR.SocketAbortedSendTimedOut, thisPtr._asyncSendTimeout), TransferOperation.Write);
            }
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
                TimeoutException timeoutException = new TimeoutException(SR.Format(SR.UDSConnectionTimedOut, timeout), originalException);
                return timeoutException;
            }

            if (socketException.ErrorCode == UnsafeNativeMethods.WSAENETRESET ||
                socketException.ErrorCode == UnsafeNativeMethods.WSAECONNABORTED ||
                socketException.ErrorCode == UnsafeNativeMethods.WSAECONNRESET)
            {
                if (aborted)
                {
                    return new CommunicationObjectAbortedException(SR.UDSLocalConnectionAborted, originalException);
                }
                else
                {
                    CommunicationException communicationException = new CommunicationException(SR.Format(SR.UDSConnectionResetError, timeout), originalException);
                    return communicationException;
                }
            }
            else if (socketException.ErrorCode == UnsafeNativeMethods.WSAETIMEDOUT)
            {
                TimeoutException timeoutException = new TimeoutException(SR.Format(SR.UDSConnectionTimedOut, timeout), originalException);
                return timeoutException;
            }
            else
            {
                if (aborted)
                {
                    return new CommunicationObjectAbortedException(SR.Format(SR.UDSTransferError, socketException.ErrorCode, socketException.Message), originalException);
                }
                else
                {
                    CommunicationException communicationException = new CommunicationException(SR.Format(SR.UDSTransferError, socketException.ErrorCode, socketException.Message), originalException);
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

        // Avoid rooting any values stored in asynclocals.
        private Timer CreateTimer(TimerCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            // Don't capture the current ExecutionContext and its AsyncLocals onto the timer
            bool restoreFlow = false;
            try
            {
                if (!ExecutionContext.IsFlowSuppressed())
                {
                    ExecutionContext.SuppressFlow();
                    restoreFlow = true;
                }

                return new Timer(callback, this, Timeout.Infinite, Timeout.Infinite);
            }
            finally
            {
                // Restore the current ExecutionContext
                if (restoreFlow)
                {
                    ExecutionContext.RestoreFlow();
                }
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

        public SocketConnectionInitiator(int bufferSize)
        {
            _bufferSize = bufferSize;
        }

        private async Task<IConnection> CreateConnectionAsync(Uri uriPath)
        {
            Socket socket = null;
            try
            {
                AddressFamily addressFamily = AddressFamily.Unix;
                socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.IP);
                var endpoint = new UnixDomainSocketEndPoint(uriPath.LocalPath);
                await socket.ConnectAsync(endpoint);
                return new SocketConnection(socket, _bufferSize);
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
                    return new EndpointNotFoundException(SR.Format(SR.UDSConnectError, remoteUri.AbsoluteUri, (int)socketException.SocketErrorCode, socketException.Message), innerException);
                }
                else
                {
                    return new EndpointNotFoundException(SR.Format(SR.UDSConnectErrorWithTimeSpan, remoteUri.AbsoluteUri, (int)socketException.SocketErrorCode, socketException.Message, timeSpent), innerException);
                }
            }
            else if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.WSAENOBUFS)
            {
                return new InsufficientMemoryException(SR.UDSConnectNoBufs, innerException);
            }
            else if ((int)socketException.SocketErrorCode == UnsafeNativeMethods.ERROR_NOT_ENOUGH_MEMORY ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.ERROR_NO_SYSTEM_RESOURCES ||
                (int)socketException.SocketErrorCode == UnsafeNativeMethods.ERROR_OUTOFMEMORY)
            {
                return new InsufficientMemoryException(SR.InsufficentMemory, socketException);
            }
            else
            {
                if (timeSpent == TimeSpan.MaxValue)
                {
                    return new CommunicationException(SR.Format(SR.UDSConnectError, remoteUri.AbsoluteUri, (int)socketException.SocketErrorCode, socketException.Message), innerException);
                }
                else
                {
                    return new CommunicationException(SR.Format(SR.UDSConnectErrorWithTimeSpan, remoteUri.AbsoluteUri, (int)socketException.SocketErrorCode, socketException.Message, timeSpent), innerException);
                }
            }
        }
      
        private static TimeoutException CreateTimeoutException(Uri uri, TimeSpan timeout, SocketException innerException)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException("Timed out while connecting to Unix domain socket", innerException));
        }

        public async ValueTask<IConnection> ConnectAsync(Uri uri, TimeSpan timeout)
        {
            IConnection socketConnection = null;
            SocketException lastException = null;
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            if (timeoutHelper.RemainingTime() == TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    CreateTimeoutException(uri, timeoutHelper.OriginalTimeout, lastException));
            }

            try
            {
                socketConnection = await CreateConnectionAsync(uri);
                lastException = null;
            }
            catch (SocketException socketException)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    SocketConnectionInitiator.ConvertConnectException(lastException, uri,
                    timeoutHelper.ElapsedTime(), socketException));
            }

            return socketConnection;
        }
    }

}
