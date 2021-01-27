// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if FEATURE_NETNATIVE

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using RTSocketError = Windows.Networking.Sockets.SocketError;

namespace System.ServiceModel.Channels
{
    internal class RTSocketConnection : SocketConnection
    {
        // callback static delegates
        private static Func<Task, object, Task> s_flushWriteImmedaite = new Func<Task, object, Task>(FlushWriteImmediate);
        private static Action<Task, object> s_onSendAsyncCompleted = new Action<Task, object>(OnSendAsyncCompleted);
        private static Action<Task<int>, object> s_onReceiveAsyncCompleted = new Action<Task<int>, object>(OnReceiveAsyncCompleted);

        // common state
        private StreamSocket _socket;
        private Stream _inputStream;
        private Stream _outputStream;

        private CancellationTokenSource _receiveCts;
        private CancellationTokenSource _sendCts;

        public RTSocketConnection(StreamSocket socket, ConnectionBufferPool connectionBufferPool) : base(connectionBufferPool)
        {
            if (socket == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("socket");
            }

            _socket = socket;
            // Not using a buffer wrapped output stream as bytes will sit in the buffer
            // unsent unless the immediate flag is specified.
            _outputStream = _socket.OutputStream.AsStreamForWrite();
            _inputStream = _socket.InputStream.AsStreamForRead(_asyncReadBufferSize);
            _receiveCts = new CancellationTokenSource();
            _sendCts = new CancellationTokenSource();
        }

        protected override IPEndPoint RemoteEndPoint {
            get
            {
                IPAddress ip;
                if(IPAddress.TryParse(_socket.Information.RemoteAddress.CanonicalName, out ip))
                {
                    int port;
                    if(Int32.TryParse(_socket.Information.RemotePort, out port))
                    {
                        return new IPEndPoint(ip, port);
                    }
                }
                return null;
            }
        }

        private static Task FlushWriteImmediate(Task antecedant, object state)
        {
            antecedant.GetAwaiter().GetResult();
            RTSocketConnection thisPtr = (RTSocketConnection)state;
            return thisPtr._outputStream.FlushAsync(thisPtr._sendCts.Token);
        }

        private static void OnReceiveAsyncCompleted(Task<int> antecedant, object state)
        {
            ((RTSocketConnection)state).OnReceiveAsync(antecedant);
        }

        private static void OnSendAsyncCompleted(Task antecedant, object state)
        {
            RTSocketConnection thisPtr = (RTSocketConnection)state;
            thisPtr.OnSendAsync(antecedant);
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
                    this.ReturnReadBuffer();
                }

                if (_asyncWritePending)
                {
                    CancelSendTimer();
                }
            }

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
                        _asyncReadPending = false;
                        CancelReceiveTimer();
                    }
                }
            }
        }

        private void CancelReceiveTimer()
        {
            // This avoids extra allocation by allowing an existing CancellationTokenSource
            // to be reused later if it hasn't been cancelled yet. If the token has already been 
            // canceled, we need to create a new CancellationTokenSource.
            int timeout = Int32.MaxValue;
            _receiveCts.CancelAfter(timeout);
            if (_receiveCts.IsCancellationRequested)
            {
                _receiveCts = new CancellationTokenSource();
            }
        }

        private void CancelSendTimer()
        {
            // This avoids extra allocation by allowing an existing CancellationTokenSource
            // to be reused later if it hasn't been cancelled yet. If the token has already been 
            // canceled, we need to create a new CancellationTokenSource.
            int timeout = Int32.MaxValue;
            _sendCts.CancelAfter(timeout);
            if (_sendCts.IsCancellationRequested)
            {
                _sendCts = new CancellationTokenSource();
            }
        }

        protected override void CloseCore(bool asyncAndLinger)
        {
            // Linger isn't possible with WinRT StreamSocket so ignoring asyncAndLinger parameter
            _socket.Dispose();

            lock (ThisLock)
            {
                // Abort could have been called on a separate thread and cleaned up 
                // our buffers/completion here
                if (_closeState != CloseState.Closed)
                {
                    if (!_asyncReadPending)
                    {
                        this.ReturnReadBuffer();
                    }
                }

                _closeState = CloseState.Closed;
            }
        }

        protected override void ShutdownCore(TimeSpan timeout)
        {
            try
            {
                // All the steps of a socket shutdown are not possible with StreamSocket. As part of a socket shutdown
                // any pending data to write is given the chance to be sent so we'll attempt that.
                var toh = new TimeoutHelper(timeout);
                _outputStream.FlushAsync(toh.GetCancellationToken()).GetAwaiter().GetResult();
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
            catch (Exception exception)
            {
                if (RTSocketError.GetStatus(exception.HResult) != SocketErrorStatus.Unknown)
                {
                    SocketException socketException = new SocketException(exception.HResult & 0x0000FFFF);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        ConvertSendException(socketException, TimeSpan.MaxValue));
                }
                throw;
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
                    SetWriteTimeout(timeout, false);
                    _asyncWritePending = true;
                    _asyncWriteCallback = callback;
                    _asyncWriteState = state;
                }

                Task writeTask = _outputStream.WriteAsync(buffer, offset, size, _sendCts.Token);
                if (immediate)
                {
                    writeTask = writeTask.ContinueWith(s_flushWriteImmedaite, this, CancellationToken.None).Unwrap();
                }

                if (!writeTask.IsCompleted)
                {
                    writeTask.ContinueWith(s_onSendAsyncCompleted, this, CancellationToken.None);
                    abortWrite = false;
                    return AsyncCompletionResult.Queued;
                }

                writeTask.GetAwaiter().GetResult();
                abortWrite = false;
                return AsyncCompletionResult.Completed;
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
            catch (Exception exception) 
            {
                if (RTSocketError.GetStatus(exception.HResult) != SocketErrorStatus.Unknown)
                {
                    SocketException socketException = new SocketException(exception.HResult & 0x0000FFFF);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        ConvertSendException(socketException, TimeSpan.MaxValue));
                }
                throw;
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

                _asyncWritePending = false;
            }
        }

        private void OnSendAsync(Task antecedent)
        {
            Contract.Assert(antecedent != null, "Argument 'antecedent' cannot be NULL.");
            this.CancelSendTimer();

            try
            {
                antecedent.GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                if (RTSocketError.GetStatus(exception.HResult) != SocketErrorStatus.Unknown)
                {
                    SocketException socketException = new SocketException(exception.HResult & 0x0000FFFF);
                    _asyncWriteException = ConvertSendException(socketException, TimeSpan.MaxValue);
                }
                else
                {
                    if (Fx.IsFatal(exception))
                    {
                        throw;
                    }

                    _asyncWriteException = exception;
                }
            }

            this.FinishWrite();
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
                        this.CancelSendTimer();
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
                int bytesToWrite = size;

                using (timeoutHelper.GetCancellationToken().Register(OnSendTimeout, this))
                {
                    while (bytesToWrite > 0)
                    {
                        size = Math.Min(bytesToWrite, maxSocketWrite);
                        _outputStream.Write(buffer, offset, size);
                        if (immediate)
                        {
                            _outputStream.Flush();
                        }

                        bytesToWrite -= size;
                        offset += size;
                    }
                }
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
            catch (Exception exception) 
            {
                if (RTSocketError.GetStatus(exception.HResult) != SocketErrorStatus.Unknown)
                {
                    SocketException socketException = new SocketException(exception.HResult & 0x0000FFFF);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        ConvertSendException(socketException, timeoutHelper.RemainingTime()));
                }
                throw;
            }
        }

        protected override void TraceWriteStart(int size, bool async)
        {
            var socketInfo = _socket.Information;
            string remoteEndpointAddressString = socketInfo.RemoteAddress.ToString() + ":" + socketInfo.RemotePort.ToString();
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
                using (timeoutHelper.GetCancellationToken().Register(OnReceiveTimeout, this))
                {
                    bytesRead = _inputStream.Read(buffer, offset, size);
                }
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
            catch (Exception exception)
            {
                if (RTSocketError.GetStatus(exception.HResult) != SocketErrorStatus.Unknown)
                {
                    SocketException socketException = new SocketException(exception.HResult & 0x0000FFFF);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        ConvertReceiveException(socketException, timeoutHelper.RemainingTime()));
                }
                throw;
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
                _asyncReadState = state;
                _asyncReadCallback = callback;
                _asyncReadPending = true;
                this.SetReadTimeout(timeout, false, false);
            }

            try
            {
                Task<int> readTask = _inputStream.ReadAsync(AsyncReadBuffer, offset, size, _receiveCts.Token);

                if (!readTask.IsCompleted)
                {
                    readTask.ContinueWith(s_onReceiveAsyncCompleted, this, CancellationToken.None);
                    abortRead = false;
                    return AsyncCompletionResult.Queued;
                }

                _asyncReadSize = readTask.Result;
                abortRead = false;
                return AsyncCompletionResult.Completed;
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
            catch (Exception exception) 
            {
                if (RTSocketError.GetStatus(exception.HResult) != SocketErrorStatus.Unknown)
                {
                    SocketException socketException = new SocketException(exception.HResult & 0x0000FFFF);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertReceiveException(socketException, TimeSpan.MaxValue));
                }
                throw;
            }
            finally
            {
                if (abortRead)
                {
                    AbortRead();
                }
            }
        }

        private void OnReceiveAsync(Task<int> antecedent)
        {
            this.CancelReceiveTimer();

            try
            {
                _asyncReadSize = antecedent.Result;
            }
            catch (Exception exception)
            {
                if (RTSocketError.GetStatus(exception.HResult) != SocketErrorStatus.Unknown)
                {
                    SocketException socketException = new SocketException(exception.HResult & 0x0000FFFF);
                    _asyncReadException = ConvertReceiveException(socketException, TimeSpan.MaxValue);
                }
                else
                {
                    if (Fx.IsFatal(exception))
                    {
                        throw;
                    }
                    _asyncReadException = exception;
                }
            }

            FinishRead();
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

                _asyncReadPending = false;

                if (_closeState == CloseState.Closed)
                {
                    this.ReturnReadBuffer();
                }
            }

            return _asyncReadSize;
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

                if (UpdateTimeout(_receiveTimeout, timeout))
                {
                    lock (ThisLock)
                    {
                        if (!closing || _closeState != CloseState.Closing)
                        {
                            ThrowIfNotOpen();
                        }
                        _receiveCts.CancelAfter(timeout);
                        if (_receiveCts.IsCancellationRequested)
                        {
                            _receiveCts = new CancellationTokenSource(timeout);
                        }
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
                    _receiveCts.CancelAfter(timeout);
                    if (_receiveCts.IsCancellationRequested)
                    {
                        _receiveCts = new CancellationTokenSource(timeout);
                    }
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

                if (UpdateTimeout(_sendTimeout, timeout))
                {
                    lock (ThisLock)
                    {
                        ThrowIfNotOpen();
                    }
                    _sendCts.CancelAfter(timeout);
                    if (_sendCts.IsCancellationRequested)
                    {
                        _sendCts = new CancellationTokenSource(timeout);
                    }
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
                    _sendCts.CancelAfter(timeout);
                    if (_sendCts.IsCancellationRequested)
                    {
                        _sendCts = new CancellationTokenSource(timeout);
                    }
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
    }

    internal class RTSocketConnectionInitiator : SocketConnectionInitiator
    {
        public RTSocketConnectionInitiator(int bufferSize) : base(bufferSize) { }
        protected override IConnection CreateConnection(IPAddress address, int port)
        {
            StreamSocket socket = null;
            try
            {
                socket = new StreamSocket();
                socket.Control.OutboundBufferSizeInBytes = (uint)_connectionBufferPool.BufferSize;
                socket.ConnectAsync(new HostName(address.ToString()), port.ToString()).AsTask().GetAwaiter().GetResult();
                return new RTSocketConnection(socket, _connectionBufferPool);
            }
            catch (Exception exception)
            {
                socket.Dispose();
                if (RTSocketError.GetStatus(exception.HResult) != SocketErrorStatus.Unknown)
                {
                    throw new SocketException(exception.HResult & 0x0000FFFF);
                }
                throw;
            }
        }

        protected override async Task<IConnection> CreateConnectionAsync(IPAddress address, int port)
        {
            StreamSocket socket = null;
            try
            {
                socket = new StreamSocket();
                socket.Control.OutboundBufferSizeInBytes = (uint)_connectionBufferPool.BufferSize;
                await socket.ConnectAsync(new HostName(address.ToString()), port.ToString()).AsTask();
                return new RTSocketConnection(socket, _connectionBufferPool);
            }
            catch (Exception exception)
            {
                socket.Dispose();
                if (RTSocketError.GetStatus(exception.HResult) != SocketErrorStatus.Unknown)
                {
                    throw new SocketException(exception.HResult & 0x0000FFFF);
                }
                throw;
            }
        }
    }

}

#endif // FEATURE_NETNATIVE
