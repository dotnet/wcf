// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime;
using System.Runtime.InteropServices;
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
        byte[] _asyncReadBuffer;
        private int _asyncBytesRead;

        // read state
        private readonly object _readLock = new object();
        private bool _inReadingState;     // This keeps track of the state machine (IConnection interface).
        private readonly int _connectionBufferSize;
        private readonly TaskCompletionSource<bool> _atEOFTask;
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

            _asyncReadBuffer = DiagnosticUtility.Utility.AllocateByteArray(connectionBufferSize);
            _pipe = namedPipeClient;
            _closeState = CloseState.Open;
            _exceptionEventType = TraceEventType.Error;
            _connectionBufferSize = connectionBufferSize;
            _atEOFTask = new TaskCompletionSource<bool>();
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

        byte[] IConnection.AsyncReadBuffer
        {
            get
            {
                return _asyncReadBuffer; ;
            }
        }

        int IConnection.AsyncReadBufferSize
        {
            get { return ConnectionBufferSize; }
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

        public async Task<int> ReadAsync(Memory<byte> buffer, TimeSpan timeout)
        {
            ValidateBufferBounds(buffer);
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
                _asyncBytesRead = bytesRead;
                if (bytesRead == 0)
                {
                    _isAtEOF = true;
                    _atEOFTask.TrySetResult(true);
                }

                if (_closeState == CloseState.PipeClosed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreatePipeClosedException(TransferOperation.Read));
                }

                return bytesRead;
            }
            catch (OperationCanceledException)
            {
                Abort(string.Format(SRServiceModel.PipeConnectionAbortedReadTimedOut, timeout), TransferOperation.Read);
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

        public async Task WriteAsync(ReadOnlyMemory<byte> buffer, bool immediate, TimeSpan timeout)
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
                Abort(string.Format(SRServiceModel.PipeConnectionAbortedWriteTimedOut, timeout), TransferOperation.Write);
                throw;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                lock (_writeLock)
                {
                    ExitWritingState();
                }
            }
        }

        public async Task CloseAsync(TimeSpan timeout)
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
                                new PipeException(SRServiceModel.PipeCantCloseWithPendingWrite), ExceptionEventType);
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

                Task writeValueTask = default;
                Task<int> readValueTask = default;
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
                            new TimeoutException(SRServiceModel.PipeShutdownWriteError, e), ExceptionEventType);
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
                            new TimeoutException(SRServiceModel.PipeShutdownReadError, e), ExceptionEventType);
                    }
                }
                else if (existingReadIsPending)
                {
                    if (!await _atEOFTask.Task.AwaitWithTimeout(timeoutHelper.RemainingTime()))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                            new TimeoutException(SRServiceModel.PipeShutdownReadError), ExceptionEventType);
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
                    new TimeoutException(SRServiceModel.PipeCloseFailed, e), ExceptionEventType);
            }
            catch (PipeException e)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                    ConvertPipeException(SRServiceModel.PipeCloseFailed, e, TransferOperation.Undefined), ExceptionEventType);
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
                    _atEOFTask.TrySetResult(true);
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
            return ConvertPipeException(new PipeException(SRServiceModel.PipeClosed), transferOperation);
        }

        private Task<int> StartReadZeroAsync(CancellationToken token)
        {
            lock (_readLock)
            {
                ValidateEnterReadingState(false);
                EnterReadingState();
                return _pipe.ReadAsync(ZeroBuffer, token);
            }
        }

        private Task StartWriteZeroAsync(CancellationToken token)
        {
            lock (_writeLock)
            {
                ValidateEnterWritingState(false);
                EnterWritingState();
                return _pipe.WriteAsync(ZeroBuffer, token);
            }
        }

        private async Task WaitForReadZero(Task<int> readTask, TimeSpan timeout, bool traceExceptionsAsErrors)
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
                                Exception exception = ConvertPipeException(new PipeException(SRServiceModel.PipeSignalExpected), TransferOperation.Read);
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

        private async Task WaitForWriteZero(Task writeTask, TimeSpan timeout, bool traceExceptionsAsErrors)
        {
            try
            {
                await writeTask;
            }
            catch(Exception)
            {
                Abort(string.Format(SRServiceModel.PipeConnectionAbortedWriteTimedOut, timeout), TransferOperation.Write);

                Exception timeoutException = new TimeoutException(string.Format(SRServiceModel.PipeWriteTimedOut, timeout));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SRServiceModel.PipeAlreadyClosing), ExceptionEventType);
                }
            }

            if (_inReadingState)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SRServiceModel.PipeReadPending), ExceptionEventType);
            }

            if (_closeState == CloseState.PipeClosed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SRServiceModel.PipeClosed), ExceptionEventType);
            }
        }

        private void ValidateEnterWritingState(bool checkShutdown)
        {
            Fx.Assert(Monitor.IsEntered(_writeLock), "_writeLock must be entered");
            if (checkShutdown)
            {
                if (_isShutdownWritten)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SRServiceModel.PipeAlreadyShuttingDown), ExceptionEventType);
                }

                if (_closeState == CloseState.Closing)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SRServiceModel.PipeAlreadyClosing), ExceptionEventType);
                }
            }

            if (_inWritingState)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SRServiceModel.PipeWritePending), ExceptionEventType);
            }

            if (_closeState == CloseState.PipeClosed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SRServiceModel.PipeClosed), ExceptionEventType);
            }
        }

        internal static void ValidateBufferBounds(ReadOnlyMemory<byte> buffer)
        {
            if (buffer.IsEmpty)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(buffer));
            }
        }

        void IConnection.Close(TimeSpan timeout, bool asyncAndLinger) => throw new NotImplementedException();
        AsyncCompletionResult IConnection.BeginWrite(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, Action<object> callback, object state) => throw new NotImplementedException();
        void IConnection.EndWrite() { }
        void IConnection.Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout) => throw new NotImplementedException();
        void IConnection.Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, BufferManager bufferManager) => throw new NotImplementedException();
        int IConnection.Read(byte[] buffer, int offset, int size, TimeSpan timeout) => throw new NotImplementedException();
        AsyncCompletionResult IConnection.BeginRead(int offset, int size, TimeSpan timeout, Action<object> callback, object state) => throw new NotImplementedException();
        int IConnection.EndRead()
        {
            return _asyncBytesRead;
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

    /// <summary>
    /// Helpers to write Memory<byte> to Stream on netstandard 2.0
    /// </summary>
    internal static class StreamExtensions
    {
        public static Task<int> ReadAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> array))
            {
                return stream.ReadAsync(array.Array, array.Offset, array.Count, cancellationToken);
            }
            else
            {
                byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
                return FinishReadAsync(stream.ReadAsync(sharedBuffer, 0, buffer.Length, cancellationToken), sharedBuffer, buffer);               
            }
        }

        static async Task<int> FinishReadAsync(Task<int> readTask, byte[] localBuffer, Memory<byte> localDestination)
        {
            try
            {
                int result = await readTask.ConfigureAwait(false);
                new Span<byte>(localBuffer, 0, result).CopyTo(localDestination.Span);
                return result;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(localBuffer);
            }
        }

        public static void Write(this Stream stream, ReadOnlyMemory<byte> buffer)
        {
            if (MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> array))
            {
                stream.Write(array.Array, array.Offset, array.Count);
            }
            else
            {
                byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
                try
                {
                    buffer.Span.CopyTo(sharedBuffer);
                    stream.Write(sharedBuffer, 0, buffer.Length);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(sharedBuffer);
                }
            }
        }

        public static Task WriteAsync(this Stream stream, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> array))
            {
                return stream.WriteAsync(array.Array, array.Offset, array.Count, cancellationToken);
            }
            else
            {
                byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
                buffer.Span.CopyTo(sharedBuffer);
                return FinishWriteAsync(stream.WriteAsync(sharedBuffer, 0, buffer.Length, cancellationToken), sharedBuffer);
            }
        }

        private static async Task FinishWriteAsync(Task writeTask, byte[] localBuffer)
        {
            try
            {
                await writeTask.ConfigureAwait(false);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(localBuffer);
            }
        }

        public static Task CopyToAsync(this Stream source, Stream destination, CancellationToken cancellationToken = default)
        {
            const int DefaultBufferSize = 81920;
            return source.CopyToAsync(destination, DefaultBufferSize, cancellationToken);
        }
    }
}
