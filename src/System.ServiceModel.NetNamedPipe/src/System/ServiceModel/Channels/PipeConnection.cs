// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime;
using System.ServiceModel.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal sealed class PipeConnection : IConnection
    {
        // common state
        private readonly NamedPipeClientStream _pipe;
        private CloseState _closeState;
        private bool _aborted;
        private TraceEventType _exceptionEventType;
        private static readonly byte[] s_zeroBuffer = Array.Empty<byte>();

        // read state
        private readonly object _readLock = new object();
        private bool _inReadingState;     // This keeps track of the state machine (IConnection interface).
        private readonly int _connectionBufferSize;
        private readonly TaskCompletionSource _atEOFTask;
        private bool _isAtEOF;

        // write state
        private readonly object _writeLock = new object();
        private bool _inWritingState;      // This keeps track of the state machine (IConnection interface).
        private bool _isShutdownWritten;

        // timeout support
        private string _timeoutErrorString;
        private TransferOperation _timeoutErrorTransferOperation;

        public PipeConnection(NamedPipeClientStream namedPipeClient, int connectionBufferSize)
        {
            _pipe = namedPipeClient ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(namedPipeClient));
            if (namedPipeClient == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(namedPipeClient));
            if (!namedPipeClient.IsConnected)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(namedPipeClient));

            _pipe = namedPipeClient;
            _closeState = CloseState.Open;
            _exceptionEventType = TraceEventType.Error;
            _connectionBufferSize = connectionBufferSize;
            _atEOFTask = new TaskCompletionSource();
        }

        public int ConnectionBufferSize
        {
            get
            {
                return _connectionBufferSize;
            }
        }

        private static byte[] ZeroBuffer
        {
            get
            {
                return s_zeroBuffer;
            }
        }

        public TraceEventType ExceptionEventType
        {
            get { return _exceptionEventType; }
            set { _exceptionEventType = value; }
        }

        public void Abort()
        {
            Abort(null, TransferOperation.Undefined);
        }

        private void Abort(string timeoutErrorString, TransferOperation transferOperation)
        {
            ClosePipe(true, timeoutErrorString, transferOperation);
        }

        private Exception ConvertPipeException(PipeException pipeException, TransferOperation transferOperation)
        {
            return ConvertPipeException(pipeException.Message, pipeException, transferOperation);
        }

        private Exception ConvertPipeException(string exceptionMessage, PipeException pipeException, TransferOperation transferOperation)
        {
            if (_timeoutErrorString != null)
            {
                if (transferOperation == _timeoutErrorTransferOperation)
                {
                    return new TimeoutException(_timeoutErrorString, pipeException);
                }
                else
                {
                    return new CommunicationException(_timeoutErrorString, pipeException);
                }
            }
            else if (_aborted)
            {
                return new CommunicationObjectAbortedException(exceptionMessage, pipeException);
            }
            else
            {
                return new CommunicationException(exceptionMessage, pipeException);
            }
        }

        public async ValueTask<int> ReadAsync(Memory<byte> buffer, TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            var cancellationToken = await timeoutHelper.GetCancellationTokenAsync();

            lock (_readLock)
            {
                ValidateEnterReadingState(true);
                EnterReadingState();
            }

            try
            {
                if (_isAtEOF)
                {
                    return 0;
                }

                int bytesRead = await _pipe.ReadAsync(buffer, cancellationToken);
                if (!buffer.IsEmpty && bytesRead == 0)
                {
                    _isAtEOF = true;
                    _atEOFTask.TrySetResult();
                }

                if (_closeState == CloseState.PipeClosed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreatePipeClosedException(TransferOperation.Read));
                }

                return bytesRead;
            }
            catch (OperationCanceledException)
            {
                Abort(SR.Format(SR.PipeConnectionAbortedReadTimedOut, timeout), TransferOperation.Read);
                throw;
            }
            finally
            {
                lock (_readLock)
                {
                    ExitReadingState();
                }
            }
        }

        public async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, bool immediate, TimeSpan timeout)
        {
            ValidateBufferBounds(buffer);
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            var cancellationToken = await timeoutHelper.GetCancellationTokenAsync();

            lock (_writeLock)
            {
                ValidateEnterWritingState(true);
                EnterWritingState();
            }

            try
            {
                await _pipe.WriteAsync(buffer, cancellationToken);

                if (_closeState == CloseState.PipeClosed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreatePipeClosedException(TransferOperation.Write));
                }
            }
            catch(OperationCanceledException)
            {
                Abort(SR.Format(SR.PipeConnectionAbortedWriteTimedOut, timeout), TransferOperation.Write);
                throw;
            }
            finally
            {
                lock (_writeLock)
                {
                    ExitWritingState();
                }
            }
        }

        public async ValueTask CloseAsync(TimeSpan timeout)
        {
            bool existingReadIsPending = false;
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            var cancellationToken = await timeoutHelper.GetCancellationTokenAsync();

            bool shouldClosePipe = false;
            try
            {
                bool shouldReadEOF = false;
                bool shouldWriteEOF = false;

                lock (_readLock)
                {
                    lock (_writeLock)
                    {
                        if (!_isShutdownWritten && _inWritingState)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                                new PipeException(SR.PipeCantCloseWithPendingWrite), ExceptionEventType);
                        }

                        if (_closeState == CloseState.Closing || _closeState == CloseState.PipeClosed)
                        {
                            // already closing or closed, so just return
                            return;
                        }

                        _closeState = CloseState.Closing;

                        shouldClosePipe = true;

                        if (!_isAtEOF)
                        {
                            if (_inReadingState)
                            {
                                existingReadIsPending = true;
                            }
                            else
                            {
                                shouldReadEOF = true;
                            }
                        }

                        if (!_isShutdownWritten)
                        {
                            shouldWriteEOF = true;
                            _isShutdownWritten = true;
                        }
                    }
                }

                ValueTask writeValueTask = default;
                ValueTask<int> readValueTask = default;
                if (shouldWriteEOF)
                {
                    writeValueTask = StartWriteZeroAsync(cancellationToken);
                }

                if (shouldReadEOF)
                {
                    readValueTask = StartReadZeroAsync(cancellationToken);
                }

                // wait for shutdown write to complete
                if (shouldWriteEOF)
                {
                    try
                    {
                        await WaitForWriteZero(writeValueTask, timeout, true);
                    }
                    catch (TimeoutException e)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                            new TimeoutException(SR.PipeShutdownWriteError, e), ExceptionEventType);
                    }
                }

                // ensure we have received EOF signal
                if (shouldReadEOF)
                {
                    try
                    {
                        await WaitForReadZero(readValueTask, timeout, true);
                    }
                    catch (TimeoutException e)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                            new TimeoutException(SR.PipeShutdownReadError, e), ExceptionEventType);
                    }
                }
                else if (existingReadIsPending)
                {
                    if (!await _atEOFTask.Task.AwaitWithTimeout(timeoutHelper.RemainingTime()))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                            new TimeoutException(SR.PipeShutdownReadError), ExceptionEventType);
                    }
                }
                // else we had already seen EOF.

                // at this point, we may get exceptions if the other side closes the pipe first
                try
                {
                    // write an ack for eof
                    writeValueTask = StartWriteZeroAsync(cancellationToken);

                    // read an ack for eof
                    readValueTask = StartReadZeroAsync(cancellationToken);

                    // wait for write to complete/fail
                    await WaitForWriteZero(writeValueTask, timeout, false);

                    // wait for read to complete/fail
                    await WaitForReadZero(readValueTask, timeout, false);
                }
                catch (PipeException e)
                {
                    int errorCode = PipeError.GetErrorFromHResult(e.ErrorCode);
                    if (!IsBrokenPipeError(errorCode))
                    {
                        throw;
                    }
                    DiagnosticUtility.TraceHandledException(e, TraceEventType.Information);
                }
                catch (CommunicationException e)
                {
                    DiagnosticUtility.TraceHandledException(e, TraceEventType.Information);
                }
                catch (TimeoutException e)
                {
                    DiagnosticUtility.TraceHandledException(e, TraceEventType.Information);
                }
            }
            catch (TimeoutException e)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                    new TimeoutException(SR.PipeCloseFailed, e), ExceptionEventType);
            }
            catch (PipeException e)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                    ConvertPipeException(SR.PipeCloseFailed, e, TransferOperation.Undefined), ExceptionEventType);
            }
            finally
            {
                if (shouldClosePipe)
                {
                    ClosePipe(false, null, TransferOperation.Undefined);
                }
            }
        }

        private void ClosePipe(bool abort, string timeoutErrorString, TransferOperation transferOperation)
        {
            lock (_readLock)
            {
                lock (_writeLock)
                {
                    if (_closeState == CloseState.PipeClosed)
                    {
                        return;
                    }

                    _timeoutErrorString = timeoutErrorString;
                    _timeoutErrorTransferOperation = transferOperation;
                    _aborted = abort;
                    _closeState = CloseState.PipeClosed;
                    _pipe.Close();
                    _atEOFTask.TrySetResult();
                }
            }

            if (abort)
            {
                TraceEventType traceEventType = TraceEventType.Warning;

                // we could be timing out a cached connection
                if (ExceptionEventType == TraceEventType.Information)
                {
                    traceEventType = ExceptionEventType;
                }

                if (DiagnosticUtility.ShouldTrace(traceEventType))
                {
                    TraceUtility.TraceEvent(traceEventType, TraceCode.PipeConnectionAbort, SR.TraceCodePipeConnectionAbort, this);
                }
            }
        }

        private void EnterReadingState()
        {
            Fx.Assert(Monitor.IsEntered(_readLock), "_readLock must be entered");
            _inReadingState = true;
        }

        private void EnterWritingState()
        {
            Fx.Assert(Monitor.IsEntered(_writeLock), "_writeLock must be entered");
            _inWritingState = true;
        }

        private void ExitReadingState()
        {
            Fx.Assert(Monitor.IsEntered(_readLock), "_readLock must be entered");
            _inReadingState = false;
        }

        private void ExitWritingState()
        {
            Fx.Assert(Monitor.IsEntered(_writeLock), "_writeLock must be entered");
            _inWritingState = false;
        }

        private bool IsBrokenPipeError(int error)
        {
            return error == UnsafeNativeMethods.ERROR_NO_DATA ||
                error == UnsafeNativeMethods.ERROR_BROKEN_PIPE;
        }

        private Exception CreatePipeClosedException(TransferOperation transferOperation)
        {
            return ConvertPipeException(new PipeException(SR.PipeClosed), transferOperation);
        }

        private ValueTask<int> StartReadZeroAsync(CancellationToken token)
        {
            lock (_readLock)
            {
                ValidateEnterReadingState(false);
                EnterReadingState();
                return _pipe.ReadAsync(ZeroBuffer, token);
            }
        }

        private ValueTask StartWriteZeroAsync(CancellationToken token)
        {
            lock (_writeLock)
            {
                ValidateEnterWritingState(false);
                EnterWritingState();
                return _pipe.WriteAsync(ZeroBuffer, token);
            }
        }

        private async ValueTask WaitForReadZero(ValueTask<int> readTask, TimeSpan timeout, bool traceExceptionsAsErrors)
        {
            bool success = false;
            int bytesRead = -1;
            try
            {
                bytesRead = await readTask;
                success = true;
            }
            finally
            {
                lock (_readLock)
                {
                    try
                    {
                        if (success)
                        {
                            if (bytesRead != 0)
                            {
                                Exception exception = ConvertPipeException(new PipeException(SR.PipeSignalExpected), TransferOperation.Read);
                                TraceEventType traceEventType = TraceEventType.Information;
                                if (traceExceptionsAsErrors)
                                {
                                    traceEventType = TraceEventType.Error;
                                }
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(exception, traceEventType);
                            }
                        }
                    }
                    finally
                    {
                        ExitReadingState();
                    }
                }
            }
        }

        private async ValueTask WaitForWriteZero(ValueTask writeTask, TimeSpan timeout, bool traceExceptionsAsErrors)
        {
            try
            {
                await writeTask;
            }
            catch(Exception)
            {
                Abort(SR.Format(SR.PipeConnectionAbortedWriteTimedOut, timeout), TransferOperation.Write);

                Exception timeoutException = new TimeoutException(SR.Format(SR.PipeWriteTimedOut, timeout));
                TraceEventType traceEventType = TraceEventType.Information;
                if (traceExceptionsAsErrors)
                {
                    traceEventType = TraceEventType.Error;
                }

                // This intentionally doesn't reset isWriteOutstanding, because technically it still is, and we need to not free the buffer.
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(timeoutException, traceEventType);
            }
            finally
            {
                lock (_writeLock)
                {
                    ExitWritingState();
                }
            }
        }

        private void ValidateEnterReadingState(bool checkEOF)
        {
            Fx.Assert(Monitor.IsEntered(_readLock), "_readLock must be entered");
            if (checkEOF)
            {
                if (_closeState == CloseState.Closing)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SR.PipeAlreadyClosing), ExceptionEventType);
                }
            }

            if (_inReadingState)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SR.PipeReadPending), ExceptionEventType);
            }

            if (_closeState == CloseState.PipeClosed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SR.PipeClosed), ExceptionEventType);
            }
        }

        private void ValidateEnterWritingState(bool checkShutdown)
        {
            Fx.Assert(Monitor.IsEntered(_writeLock), "_writeLock must be entered");
            if (checkShutdown)
            {
                if (_isShutdownWritten)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SR.PipeAlreadyShuttingDown), ExceptionEventType);
                }

                if (_closeState == CloseState.Closing)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SR.PipeAlreadyClosing), ExceptionEventType);
                }
            }

            if (_inWritingState)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SR.PipeWritePending), ExceptionEventType);
            }

            if (_closeState == CloseState.PipeClosed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SR.PipeClosed), ExceptionEventType);
            }
        }

        internal static void ValidateBufferBounds(ReadOnlyMemory<byte> buffer)
        {
            if (buffer.IsEmpty)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(buffer));
            }
        }

        private enum CloseState
        {
            Open,
            Closing,
            PipeClosed,
        }

        private enum TransferOperation
        {
            Write,
            Read,
            Undefined,
        }
    }
}
