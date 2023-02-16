// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security;
using System.ServiceModel.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Tools.ServiceModel.Svcutil;

namespace System.ServiceModel.Channels
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Runtime;
    using System.Runtime.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
    using System.Security.AccessControl;
    using System.ComponentModel;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.ServiceModel;
    //using System.ServiceModel.Activation;
    using System.ServiceModel.Diagnostics;
    //using System.ServiceModel.Diagnostics.Application;
    using System.ServiceModel.Security;
    using System.Text;
    using System.Threading;
    //using SafeCloseHandle = System.ServiceModel.Activation.SafeCloseHandle;
    using Microsoft.Xml;
    using MS.Internal.Xml.XPath;
    using static System.ServiceModel.Channels.SingletonMessageDecoder;
    using System.Drawing;
    using System.Runtime.InteropServices.ComTypes;

    sealed class PipeConnection : IConnection
    {
        // common state
        PipeHandle _pipe;
        CloseState _closeState;
        bool _aborted;
        bool _isBoundToCompletionPort;
        bool _autoBindToCompletionPort;
        TraceEventType _exceptionEventType;
        static byte[] s_zeroBuffer;

        // read state
        object _readLock = new object();
        bool _inReadingState;     // This keeps track of the state machine (IConnection interface).
        bool _isReadOutstanding;  // This tracks whether an actual I/O is pending.
        OverlappedContext _readOverlapped;
        byte[] _asyncReadBuffer;
        int _readBufferSize;
        ManualResetEvent _atEOFEvent;
        bool _isAtEOF;
        OverlappedIOCompleteCallback _onAsyncReadComplete;
        Exception _asyncReadException;
        //WaitCallback _asyncReadCallback;
        Action<object> _asyncReadCallback;
        object _asyncReadCallbackState;
        int _asyncBytesRead;

        // write state
        object _writeLock = new object();
        bool _inWritingState;      // This keeps track of the state machine (IConnection interface).
        bool _isWriteOutstanding;  // This tracks whether an actual I/O is pending.
        OverlappedContext _writeOverlapped;
        Exception _asyncWriteException;
        //WaitCallback _asyncWriteCallback;
        Action<object> _asyncWriteCallback;
        object _asyncWriteCallbackState;
        int _asyncBytesToWrite;
        bool _isShutdownWritten;
        int _syncWriteSize;
        byte[] _pendingWriteBuffer;
        BufferManager _pendingWriteBufferManager;
        OverlappedIOCompleteCallback _onAsyncWriteComplete;
        int _writeBufferSize;

        // timeout support
        TimeSpan _readTimeout;
        IOThreadTimer _readTimer;
        static Action<object> s_onReadTimeout;
        string _timeoutErrorString;
        TransferOperation _timeoutErrorTransferOperation;
        TimeSpan _writeTimeout;
        IOThreadTimer _writeTimer;
        static Action<object> s_onWriteTimeout;

        public PipeConnection(PipeHandle pipe, int connectionBufferSize, bool isBoundToCompletionPort, bool autoBindToCompletionPort)
        {
            if (pipe == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("pipe");
            if (pipe.IsInvalid)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("pipe");

            this._closeState = CloseState.Open;
            this._exceptionEventType = TraceEventType.Error;
            this._isBoundToCompletionPort = isBoundToCompletionPort;
            this._autoBindToCompletionPort = autoBindToCompletionPort;
            this._pipe = pipe;
            this._readBufferSize = connectionBufferSize;
            this._writeBufferSize = connectionBufferSize;
            this._readOverlapped = new OverlappedContext();
            this._asyncReadBuffer = DiagnosticUtility.Utility.AllocateByteArray(connectionBufferSize);
            this._writeOverlapped = new OverlappedContext();
            this._atEOFEvent = new ManualResetEvent(false);
            this._onAsyncReadComplete = new OverlappedIOCompleteCallback(OnAsyncReadComplete);
            this._onAsyncWriteComplete = new OverlappedIOCompleteCallback(OnAsyncWriteComplete);
        }

        public int AsyncReadBufferSize
        {
            get
            {
                return this._readBufferSize;
            }
        }

        public byte[] AsyncReadBuffer
        {
            get
            {
                return this._asyncReadBuffer;
            }
        }

        static byte[] ZeroBuffer
        {
            get
            {
                if (PipeConnection.s_zeroBuffer == null)
                {
                    PipeConnection.s_zeroBuffer = new byte[1];
                }
                return PipeConnection.s_zeroBuffer;
            }
        }

        public TraceEventType ExceptionEventType
        {
            get { return this._exceptionEventType; }
            set { this._exceptionEventType = value; }
        }

        public IPEndPoint RemoteIPEndPoint
        {
            get { return null; }
        }

        IOThreadTimer ReadTimer
        {
            get
            {
                if (this._readTimer == null)
                {
                    if (s_onReadTimeout == null)
                    {
                        s_onReadTimeout = new Action<object>(OnReadTimeout);
                    }

                    this._readTimer = new IOThreadTimer(s_onReadTimeout, this, false);
                }

                return this._readTimer;
            }
        }
        IOThreadTimer WriteTimer
        {
            get
            {
                if (this._writeTimer == null)
                {
                    if (s_onWriteTimeout == null)
                    {
                        s_onWriteTimeout = new Action<object>(OnWriteTimeout);
                    }

                    this._writeTimer = new IOThreadTimer(s_onWriteTimeout, this, false);
                }

                return this._writeTimer;
            }
        }

        static void OnReadTimeout(object state)
        {
            PipeConnection thisPtr = (PipeConnection)state;
            //thisPtr.Abort(SR.GetString(SR.PipeConnectionAbortedReadTimedOut, thisPtr._readTimeout), TransferOperation.Read);
            thisPtr.Abort("SR.PipeConnectionAbortedReadTimedOut, thisPtr._readTimeout", TransferOperation.Read);
        }

        static void OnWriteTimeout(object state)
        {
            PipeConnection thisPtr = (PipeConnection)state;
            //thisPtr.Abort(SR.GetString(SR.PipeConnectionAbortedWriteTimedOut, thisPtr._writeTimeout), TransferOperation.Write);
            thisPtr.Abort("SR.PipeConnectionAbortedWriteTimedOut, thisPtr._writeTimeout", TransferOperation.Write);
        }

        public void Abort()
        {
            Abort(null, TransferOperation.Undefined);
        }

        void Abort(string timeoutErrorString, TransferOperation transferOperation)
        {
            CloseHandle(true, timeoutErrorString, transferOperation);
        }

        Exception ConvertPipeException(PipeException pipeException, TransferOperation transferOperation)
        {
            return ConvertPipeException(pipeException.Message, pipeException, transferOperation);
        }

        Exception ConvertPipeException(string exceptionMessage, PipeException pipeException, TransferOperation transferOperation)
        {
            if (this._timeoutErrorString != null)
            {
                if (transferOperation == this._timeoutErrorTransferOperation)
                {
                    return new TimeoutException(this._timeoutErrorString, pipeException);
                }
                else
                {
                    return new CommunicationException(this._timeoutErrorString, pipeException);
                }
            }
            else if (this._aborted)
            {
                return new CommunicationObjectAbortedException(exceptionMessage, pipeException);
            }
            else
            {
                return new CommunicationException(exceptionMessage, pipeException);
            }
        }

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        unsafe AsyncCompletionResult IConnection.BeginRead(int offset, int size, TimeSpan timeout,
            /*WaitCallback callback */Action<object> callback, object state)
        {
            ConnectionUtilities.ValidateBufferBounds(AsyncReadBuffer, offset, size);

            lock (_readLock)
            {
                try
                {
                    ValidateEnterReadingState(true);

                    if (_isAtEOF)
                    {
                        _asyncBytesRead = 0;
                        _asyncReadException = null;
                        return AsyncCompletionResult.Completed;
                    }

                    if (_autoBindToCompletionPort)
                    {
                        if (!_isBoundToCompletionPort)
                        {
                            lock (_writeLock)
                            {
                                // readLock, writeLock acquired in order to prevent deadlock
                                EnsureBoundToCompletionPort();
                            }
                        }
                    }

                    if (this._isReadOutstanding)
                    {
                        throw Fx.AssertAndThrow("Read I/O already pending when BeginRead called.");
                    }
                    try
                    {
                        this._readTimeout = timeout;

                        if (this._readTimeout != TimeSpan.MaxValue)
                        {
                            this.ReadTimer.Set(this._readTimeout);
                        }

                        this._asyncReadCallback = callback;
                        this._asyncReadCallbackState = state;

                        this._isReadOutstanding = true;
                        this._readOverlapped.StartAsyncOperation(AsyncReadBuffer, this._onAsyncReadComplete, this._isBoundToCompletionPort);
                        if (UnsafeNativeMethods.ReadFile(this._pipe.DangerousGetHandle(), this._readOverlapped.BufferPtr + offset, size, IntPtr.Zero, this._readOverlapped.NativeOverlapped) == 0)
                        {
                            int error = Marshal.GetLastWin32Error();
                            if (error != UnsafeNativeMethods.ERROR_IO_PENDING && error != UnsafeNativeMethods.ERROR_MORE_DATA)
                            {
                                this._isReadOutstanding = false;
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(Exceptions.CreateReadException(error));
                            }
                        }
                    }
                    finally
                    {
                        if (!this._isReadOutstanding)
                        {
                            // Unbind the buffer.
                            this._readOverlapped.CancelAsyncOperation();

                            this._asyncReadCallback = null;
                            this._asyncReadCallbackState = null;
                            this.ReadTimer.Cancel();
                        }
                    }

                    if (!this._isReadOutstanding)
                    {
                        int bytesRead;
                        Exception readException = Exceptions.GetOverlappedReadException(this._pipe, this._readOverlapped.NativeOverlapped, out bytesRead);
                        if (readException != null)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(readException);
                        }
                        _asyncBytesRead = bytesRead;
                        HandleReadComplete(_asyncBytesRead);
                    }
                    else
                    {
                        EnterReadingState();
                    }

                    return this._isReadOutstanding ? AsyncCompletionResult.Queued : AsyncCompletionResult.Completed;
                }
                catch (PipeException e)
                {
                    //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(ConvertPipeException(e, TransferOperation.Read), ExceptionEventType);
                    throw e;
                }
            }
        }

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        unsafe AsyncCompletionResult IConnection.BeginWrite(byte[] buffer, int offset, int size, bool immediate, System.TimeSpan timeout, System.Action<object> callback, object state)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            FinishPendingWrite(timeout);

            ConnectionUtilities.ValidateBufferBounds(buffer, offset, size);

            if (_autoBindToCompletionPort && !_isBoundToCompletionPort)
            {
                // Locks must be both taken, and in this order.
                lock (_readLock)
                {
                    lock (_writeLock)
                    {
                        ValidateEnterWritingState(true);

                        EnsureBoundToCompletionPort();
                    }
                }
            }

            lock (_writeLock)
            {
                try
                {
                    ValidateEnterWritingState(true);

                    if (this._isWriteOutstanding)
                    {
                        throw Fx.AssertAndThrow("Write I/O already pending when BeginWrite called.");
                    }

                    try
                    {
                        this._writeTimeout = timeout;
                        this.WriteTimer.Set(timeoutHelper.RemainingTime());

                        this._asyncBytesToWrite = size;
                        this._asyncWriteException = null;
                        this._asyncWriteCallback = callback;
                        this._asyncWriteCallbackState = state;

                        this._isWriteOutstanding = true;
                        this._writeOverlapped.StartAsyncOperation(buffer, this._onAsyncWriteComplete, this._isBoundToCompletionPort);
                        if (UnsafeNativeMethods.WriteFile(this._pipe.DangerousGetHandle(), this._writeOverlapped.BufferPtr + offset, size, IntPtr.Zero, this._writeOverlapped.NativeOverlapped) == 0)
                        {
                            int error = Marshal.GetLastWin32Error();
                            if (error != UnsafeNativeMethods.ERROR_IO_PENDING)
                            {
                                this._isWriteOutstanding = false;
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(Exceptions.CreateWriteException(error));
                            }
                        }
                    }
                    finally
                    {
                        if (!this._isWriteOutstanding)
                        {
                            // Unbind the buffer.
                            this._writeOverlapped.CancelAsyncOperation();

                            this.ResetWriteState();
                            this.WriteTimer.Cancel();
                        }
                    }

                    if (!this._isWriteOutstanding)
                    {
                        int bytesWritten;
                        Exception writeException = Exceptions.GetOverlappedWriteException(
                            this._pipe,
                            this._writeOverlapped.NativeOverlapped,
                            out bytesWritten);
                        if (writeException == null && bytesWritten != size)
                        {
                            //writeException = new PipeException(SR.GetString(SR.PipeWriteIncomplete));
                            writeException = new PipeException("SR.PipeWriteIncomplete");
                        }
                        if (writeException != null)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(writeException);
                        }
                    }
                    else
                    {
                        EnterWritingState();
                    }

                    return this._isWriteOutstanding ? AsyncCompletionResult.Queued : AsyncCompletionResult.Completed;
                }
                catch (PipeException e)
                {
                    //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(ConvertPipeException(e, TransferOperation.Write), ExceptionEventType);
                    throw e;
                }
            }
        }

        // CSDMain 112188: Note asyncAndLinger has no effect here. Async pooling for Tcp was
        // added and NamedPipes currently doesn't obey the async model.
        public void Close(TimeSpan timeout, bool asyncAndLinger)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            FinishPendingWrite(timeout);

            bool shouldCloseHandle = false;
            try
            {
                bool existingReadIsPending = false;
                bool shouldReadEOF = false;
                bool shouldWriteEOF = false;

                lock (_readLock)
                {
                    lock (_writeLock)
                    {
                        if (!_isShutdownWritten && _inWritingState)
                        {
                            //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                            //    new PipeException(SR.GetString(SR.PipeCantCloseWithPendingWrite)), ExceptionEventType);
                            throw new Exception("SR.PipeCantCloseWithPendingWrite");

                        }

                        if (_closeState == CloseState.Closing || _closeState == CloseState.HandleClosed)
                        {
                            // already closing or closed, so just return
                            return;
                        }

                        _closeState = CloseState.Closing;

                        shouldCloseHandle = true;

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

                if (shouldWriteEOF)
                {
                    StartWriteZero(timeoutHelper.RemainingTime());
                }

                if (shouldReadEOF)
                {
                    StartReadZero();
                }

                // wait for shutdown write to complete
                try
                {
                    WaitForWriteZero(timeoutHelper.RemainingTime(), true);
                }
                catch (TimeoutException e)
                {
                    //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                    //    new TimeoutException(SR.GetString(SR.PipeShutdownWriteError), e), ExceptionEventType);
                    throw new Exception("SR.PipeShutdownWriteError");
                }

                // ensure we have received EOF signal
                if (shouldReadEOF)
                {
                    try
                    {
                        WaitForReadZero(timeoutHelper.RemainingTime(), true);
                    }
                    catch (TimeoutException e)
                    {
                        //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                        //    new TimeoutException(SR.GetString(SR.PipeShutdownReadError), e), ExceptionEventType);
                        throw e;
                    }
                }
                else if (existingReadIsPending)
                {
                    if (!TimeoutHelper.WaitOne(_atEOFEvent, timeoutHelper.RemainingTime()))
                    {
                        //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                        //    new TimeoutException(SR.GetString(SR.PipeShutdownReadError)), ExceptionEventType);
                        throw new TimeoutException("SR.PipeShutdownReadError");
                    }
                }
                // else we had already seen EOF.

                // at this point, we may get exceptions if the other side closes the handle first
                try
                {
                    // write an ack for eof
                    StartWriteZero(timeoutHelper.RemainingTime());

                    // read an ack for eof
                    StartReadZero();

                    // wait for write to complete/fail
                    WaitForWriteZero(timeoutHelper.RemainingTime(), false);

                    // wait for read to complete/fail
                    WaitForReadZero(timeoutHelper.RemainingTime(), false);
                }
                catch (PipeException e)
                {
                    if (!IsBrokenPipeError(e.ErrorCode))
                    {
                        throw;
                    }
                    //DiagnosticUtility.TraceHandledException(e, TraceEventType.Information);
                }
                catch (CommunicationException e)
                {
                    //DiagnosticUtility.TraceHandledException(e, TraceEventType.Information);
                }
                catch (TimeoutException e)
                {
                    //DiagnosticUtility.TraceHandledException(e, TraceEventType.Information);
                }
            }
            catch (TimeoutException e)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                //    new TimeoutException(SR.GetString(SR.PipeCloseFailed), e), ExceptionEventType);
                throw e;
            }
            catch (PipeException e)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                //    ConvertPipeException(SR.GetString(SR.PipeCloseFailed), e, TransferOperation.Undefined), ExceptionEventType);
                throw e;
            }
            finally
            {
                if (shouldCloseHandle)
                {
                    CloseHandle(false, null, TransferOperation.Undefined);
                }
            }
        }

        void CloseHandle(bool abort, string timeoutErrorString, TransferOperation transferOperation)
        {
            lock (_readLock)
            {
                lock (_writeLock)
                {
                    if (this._closeState == CloseState.HandleClosed)
                    {
                        return;
                    }

                    this._timeoutErrorString = timeoutErrorString;
                    this._timeoutErrorTransferOperation = transferOperation;
                    this._aborted = abort;
                    this._closeState = CloseState.HandleClosed;
                    this._pipe.Close();
                    this._readOverlapped.FreeOrDefer();
                    this._writeOverlapped.FreeOrDefer();

                    if (this._atEOFEvent != null)
                    {
                        this._atEOFEvent.Close();
                    }

                    // This should only do anything in the abort case.
                    try
                    {
                        FinishPendingWrite(TimeSpan.Zero);
                    }
                    catch (TimeoutException exception)
                    {
                        //if (TD.CloseTimeoutIsEnabled())
                        //{
                        //    TD.CloseTimeout(exception.Message);
                        //}
                        //DiagnosticUtility.TraceHandledException(exception, TraceEventType.Information);
                    }
                    catch (CommunicationException exception)
                    {
                        //DiagnosticUtility.TraceHandledException(exception, TraceEventType.Information);
                    }
                }
            }

            if (abort)
            {
                TraceEventType traceEventType = TraceEventType.Warning;

                // we could be timing out a cached connection
                if (this.ExceptionEventType == TraceEventType.Information)
                {
                    traceEventType = this.ExceptionEventType;
                }

                //if (DiagnosticUtility.ShouldTrace(traceEventType))
                //{
                //    TraceUtility.TraceEvent(traceEventType, TraceCode.PipeConnectionAbort, SR.GetString(SR.TraceCodePipeConnectionAbort), this);
                //}
            }
        }

        CommunicationException CreatePipeDuplicationFailedException(int win32Error)
        {
            Exception innerException = new PipeException("PipeDuplicationFailed", win32Error);
            return new CommunicationException(innerException.Message, innerException);
        }

        //public object DuplicateAndClose(int targetProcessId)
        //{
        //    SafeCloseHandle targetProcessHandle = ListenerUnsafeNativeMethods.OpenProcess(ListenerUnsafeNativeMethods.PROCESS_DUP_HANDLE, false, targetProcessId);
        //    if (targetProcessHandle.IsInvalid)
        //    {
        //        targetProcessHandle.SetHandleAsInvalid();
        //        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
        //            CreatePipeDuplicationFailedException(Marshal.GetLastWin32Error()), ExceptionEventType);
        //    }
        //    try
        //    {
        //        // no need to close this handle, it's a pseudo handle. expected value is -1.
        //        IntPtr sourceProcessHandle = ListenerUnsafeNativeMethods.GetCurrentProcess();
        //        if (sourceProcessHandle == IntPtr.Zero)
        //        {
        //            throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
        //                CreatePipeDuplicationFailedException(Marshal.GetLastWin32Error()), ExceptionEventType);
        //        }
        //        IntPtr duplicatedHandle;
        //        bool success = UnsafeNativeMethods.DuplicateHandle(sourceProcessHandle, this._pipe, targetProcessHandle, out duplicatedHandle, 0, false, UnsafeNativeMethods.DUPLICATE_SAME_ACCESS);
        //        if (!success)
        //        {
        //            throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
        //                CreatePipeDuplicationFailedException(Marshal.GetLastWin32Error()), ExceptionEventType);
        //        }
        //        this.Abort();
        //        return duplicatedHandle;
        //    }
        //    finally
        //    {
        //        targetProcessHandle.Close();
        //    }
        //}

        public object GetCoreTransport()
        {
            return _pipe;
        }

        void EnsureBoundToCompletionPort()
        {
            // Both read and write locks must be acquired before doing this
            if (!_isBoundToCompletionPort)
            {
                ThreadPool.BindHandle(this._pipe);
                _isBoundToCompletionPort = true;
            }
        }

        public int EndRead()
        {
            if (_asyncReadException != null)
            {
                Exception exceptionToThrow = _asyncReadException;
                _asyncReadException = null;
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(exceptionToThrow, ExceptionEventType);
                throw exceptionToThrow;
            }
            return _asyncBytesRead;
        }

        public void EndWrite()
        {
            if (this._asyncWriteException != null)
            {
                Exception exceptionToThrow = this._asyncWriteException;
                this._asyncWriteException = null;
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(exceptionToThrow, ExceptionEventType);
                throw exceptionToThrow;
            }
        }

        void EnterReadingState()
        {
            _inReadingState = true;
        }

        void EnterWritingState()
        {
            _inWritingState = true;
        }

        void ExitReadingState()
        {
            _inReadingState = false;
        }

        void ExitWritingState()
        {
            _inWritingState = false;
        }

        void ReadIOCompleted()
        {
            this._readOverlapped.FreeIfDeferred();
        }

        void WriteIOCompleted()
        {
            this._writeOverlapped.FreeIfDeferred();
        }

        void FinishPendingWrite(TimeSpan timeout)
        {
            if (this._pendingWriteBuffer == null)
            {
                return;
            }

            byte[] buffer;
            BufferManager bufferManager;
            lock (this._writeLock)
            {
                if (this._pendingWriteBuffer == null)
                {
                    return;
                }

                buffer = this._pendingWriteBuffer;
                this._pendingWriteBuffer = null;

                bufferManager = this._pendingWriteBufferManager;
                this._pendingWriteBufferManager = null;
            }

            try
            {
                bool success = false;
                try
                {
                    WaitForSyncWrite(timeout, true);
                    success = true;
                }
                finally
                {
                    lock (this._writeLock)
                    {
                        try
                        {
                            if (success)
                            {
                                FinishSyncWrite(true);
                            }
                        }
                        finally
                        {
                            ExitWritingState();
                            if (!this._isWriteOutstanding)
                            {
                                bufferManager.ReturnBuffer(buffer);
                                WriteIOCompleted();
                            }
                        }
                    }
                }
            }
            catch (PipeException e)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(ConvertPipeException(e, TransferOperation.Write), ExceptionEventType);
                throw e;
            }
        }

#if FUTURE
        ulong GetServerPid()
        {
            ulong id;
#pragma warning suppress 56523 // elliotw, Win32Exception ctor calls Marshal.GetLastWin32Error()
            if (!UnsafeNativeMethods.GetNamedPipeServerProcessId(pipe, out id))
            {
                Win32Exception e = new Win32Exception();
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(e.Message, e));
            }
            return id;
        }

        ulong GetClientPid()
        {
            ulong id;
#pragma warning suppress 56523 // elliotw, Win32Exception ctor calls Marshal.GetLastWin32Error()
            if (!UnsafeNativeMethods.GetNamedPipeServerProcessId(pipe, out id))
            {
                Win32Exception e = new Win32Exception();
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(e.Message, e));
            }
            return id;
        }
#endif

        void HandleReadComplete(int bytesRead)
        {
            if (bytesRead == 0)
            {
                _isAtEOF = true;
                _atEOFEvent.Set();
            }
        }

        bool IsBrokenPipeError(int error)
        {
            return error == UnsafeNativeMethods.ERROR_NO_DATA ||
                error == UnsafeNativeMethods.ERROR_BROKEN_PIPE;
        }

        Exception CreatePipeClosedException(TransferOperation transferOperation)
        {
            return ConvertPipeException(new PipeException("SR.PipeClosed"), transferOperation);
        }

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        unsafe void OnAsyncReadComplete(bool haveResult, int error, int numBytes)
        {
            Action<object> callback;
            object state;

            lock (_readLock)
            {
#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
                try
                {
                    try
                    {
                        if (this._readTimeout != TimeSpan.MaxValue && !this.ReadTimer.Cancel())
                        {
                            //this.Abort(SR.GetString(SR.PipeConnectionAbortedReadTimedOut, this._readTimeout), TransferOperation.Read);
                            this.Abort("SR.PipeConnectionAbortedReadTimedOut, this._readTimeout", TransferOperation.Read);
                        }

                        if (this._closeState == CloseState.HandleClosed)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreatePipeClosedException(TransferOperation.Read));
                        }
                        if (!haveResult)
                        {
                            if (UnsafeNativeMethods.GetOverlappedResult(this._pipe.DangerousGetHandle(), this._readOverlapped.NativeOverlapped, out numBytes, 0) == 0)
                            {
                                error = Marshal.GetLastWin32Error();
                            }
                            else
                            {
                                error = 0;
                            }
                        }

                        if (error != 0 && error != UnsafeNativeMethods.ERROR_MORE_DATA)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(Exceptions.CreateReadException((int)error));
                        }
                        this._asyncBytesRead = numBytes;
                        HandleReadComplete(this._asyncBytesRead);
                    }
                    catch (PipeException e)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertPipeException(e, TransferOperation.Read));
                    }
                }
#pragma warning suppress 56500 // elliotw, transferring exception to caller
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    this._asyncReadException = e;
                }
                finally
                {
                    this._isReadOutstanding = false;
                    ReadIOCompleted();
                    ExitReadingState();
                    callback = this._asyncReadCallback;
                    this._asyncReadCallback = null;
                    state = this._asyncReadCallbackState;
                    this._asyncReadCallbackState = null;
                }
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning
            }

            callback(state);
        }

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        unsafe void OnAsyncWriteComplete(bool haveResult, int error, int numBytes)
        {
            Action<object> callback;
            object state;

            Exception writeException = null;

            this.WriteTimer.Cancel();
            lock (_writeLock)
            {
#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
                try
                {
                    try
                    {
                        if (this._closeState == CloseState.HandleClosed)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreatePipeClosedException(TransferOperation.Write));
                        }
                        if (!haveResult)
                        {
                            if (UnsafeNativeMethods.GetOverlappedResult(this._pipe.DangerousGetHandle(), this._writeOverlapped.NativeOverlapped, out numBytes, 0) == 0)
                            {
                                error = Marshal.GetLastWin32Error();
                            }
                            else
                            {
                                error = 0;
                            }
                        }

                        if (error != 0)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(Exceptions.CreateWriteException(error));
                        }
                        else if (numBytes != this._asyncBytesToWrite)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new PipeException("SR.PipeWriteIncomplete"));
                        }
                    }
                    catch (PipeException e)
                    {
                        //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(ConvertPipeException(e, TransferOperation.Write), ExceptionEventType);
                        throw e;
                    }
                }
#pragma warning suppress 56500 // elliotw, transferring exception to another thread
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    writeException = e;
                }
                finally
                {
                    this._isWriteOutstanding = false;
                    WriteIOCompleted();
                    ExitWritingState();
                    this._asyncWriteException = writeException;
                    callback = this._asyncWriteCallback;
                    state = this._asyncWriteCallbackState;
                    this.ResetWriteState();
                }
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning
            }

            if (callback != null)
            {
                callback(state);
            }
        }

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        unsafe public int Read(byte[] buffer, int offset, int size, TimeSpan timeout)
        {
            ConnectionUtilities.ValidateBufferBounds(buffer, offset, size);

            try
            {
                lock (_readLock)
                {
                    ValidateEnterReadingState(true);
                    if (_isAtEOF)
                    {
                        return 0;
                    }

                    StartSyncRead(buffer, offset, size);
                    EnterReadingState();
                }

                int bytesRead = -1;
                bool success = false;
                try
                {
                    WaitForSyncRead(timeout, true);
                    success = true;
                }
                finally
                {
                    lock (this._readLock)
                    {
                        try
                        {
                            if (success)
                            {
                                bytesRead = FinishSyncRead(true);
                                HandleReadComplete(bytesRead);
                            }
                        }
                        finally
                        {
                            ExitReadingState();
                            if (!this._isReadOutstanding)
                            {
                                ReadIOCompleted();
                            }
                        }
                    }
                }

                Fx.Assert(bytesRead >= 0, "Logic error in Read - bytesRead not set.");
                return bytesRead;
            }
            catch (PipeException e)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(ConvertPipeException(e, TransferOperation.Read), ExceptionEventType);
                throw e;
            }
        }

        public void Shutdown(TimeSpan timeout)
        {
            try
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                FinishPendingWrite(timeoutHelper.RemainingTime());

                lock (_writeLock)
                {
                    ValidateEnterWritingState(true);
                    StartWriteZero(timeoutHelper.RemainingTime());
                    _isShutdownWritten = true;
                }
            }
            catch (PipeException e)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(ConvertPipeException(e, TransferOperation.Undefined), ExceptionEventType);
                throw e;
            }
        }

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        unsafe void StartReadZero()
        {
            lock (this._readLock)
            {
                ValidateEnterReadingState(false);
                StartSyncRead(ZeroBuffer, 0, 1);
                EnterReadingState();
            }
        }

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        unsafe void StartWriteZero(TimeSpan timeout)
        {
            FinishPendingWrite(timeout);

            lock (this._writeLock)
            {
                ValidateEnterWritingState(false);
                StartSyncWrite(ZeroBuffer, 0, 0);
                EnterWritingState();
            }
        }

        void ResetWriteState()
        {
            this._asyncBytesToWrite = -1;
            this._asyncWriteCallback = null;
            this._asyncWriteCallbackState = null;
        }

        public IAsyncResult BeginValidate(Uri uri, AsyncCallback callback, object state)
        {
            return new CompletedAsyncResult<bool>(true, callback, state);
        }

        public bool EndValidate(IAsyncResult result)
        {
            return CompletedAsyncResult<bool>.End(result);
        }

        void WaitForReadZero(TimeSpan timeout, bool traceExceptionsAsErrors)
        {
            bool success = false;
            try
            {
                WaitForSyncRead(timeout, traceExceptionsAsErrors);
                success = true;
            }
            finally
            {
                lock (this._readLock)
                {
                    try
                    {
                        if (success)
                        {
                            if (FinishSyncRead(traceExceptionsAsErrors) != 0)
                            {
                                Exception exception = ConvertPipeException(new PipeException("SR.PipeSignalExpected"), TransferOperation.Read);
                                //TraceEventType traceEventType = TraceEventType.Information;
                                //if (traceExceptionsAsErrors)
                                //{
                                //    traceEventType = TraceEventType.Error;
                                //}
                                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(exception, traceEventType);
                                throw exception;
                            }
                        }
                    }
                    finally
                    {
                        ExitReadingState();
                        if (!this._isReadOutstanding)
                        {
                            ReadIOCompleted();
                        }
                    }
                }
            }
        }

        void WaitForWriteZero(TimeSpan timeout, bool traceExceptionsAsErrors)
        {
            bool success = false;
            try
            {
                WaitForSyncWrite(timeout, traceExceptionsAsErrors);
                success = true;
            }
            finally
            {
                lock (this._writeLock)
                {
                    try
                    {
                        if (success)
                        {
                            FinishSyncWrite(traceExceptionsAsErrors);
                        }
                    }
                    finally
                    {
                        ExitWritingState();
                        if (!this._isWriteOutstanding)
                        {
                            WriteIOCompleted();
                        }
                    }
                }
            }
        }

        public void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout)
        {
            WriteHelper(buffer, offset, size, immediate, timeout, ref this._writeOverlapped.Holder[0]);
        }

        // The holder is a perf optimization that lets us avoid repeatedly indexing into the array.
        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        unsafe void WriteHelper(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, ref object holder)
        {
            try
            {
                FinishPendingWrite(timeout);

                ConnectionUtilities.ValidateBufferBounds(buffer, offset, size);

                int bytesToWrite = size;
                if (size > this._writeBufferSize)
                {
                    size = this._writeBufferSize;
                }

                while (bytesToWrite > 0)
                {
                    lock (this._writeLock)
                    {
                        ValidateEnterWritingState(true);

                        StartSyncWrite(buffer, offset, size, ref holder);
                        EnterWritingState();
                    }

                    bool success = false;
                    try
                    {
                        WaitForSyncWrite(timeout, true, ref holder);
                        success = true;
                    }
                    finally
                    {
                        lock (this._writeLock)
                        {
                            try
                            {
                                if (success)
                                {
                                    FinishSyncWrite(true);
                                }
                            }
                            finally
                            {
                                ExitWritingState();
                                if (!this._isWriteOutstanding)
                                {
                                    WriteIOCompleted();
                                }
                            }
                        }
                    }

                    bytesToWrite -= size;
                    offset += size;
                    if (size > bytesToWrite)
                    {
                        size = bytesToWrite;
                    }
                }
            }
            catch (PipeException e)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(ConvertPipeException(e, TransferOperation.Write), ExceptionEventType);
                throw e;
            }
        }

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        public unsafe void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, BufferManager bufferManager)
        {
            bool shouldReturnBuffer = true;

            try
            {
                if (size > this._writeBufferSize)
                {
                    WriteHelper(buffer, offset, size, immediate, timeout, ref this._writeOverlapped.Holder[0]);
                    return;
                }

                FinishPendingWrite(timeout);

                ConnectionUtilities.ValidateBufferBounds(buffer, offset, size);

                lock (this._writeLock)
                {
                    ValidateEnterWritingState(true);

                    // This method avoids the call to GetOverlappedResult for synchronous completions.  Perf?
                    bool success = false;
                    try
                    {
                        shouldReturnBuffer = false;
                        StartSyncWrite(buffer, offset, size);
                        success = true;
                    }
                    finally
                    {
                        if (!this._isWriteOutstanding)
                        {
                            shouldReturnBuffer = true;
                        }
                        else
                        {
                            if (success)
                            {
                                EnterWritingState();

                                Fx.Assert(this._pendingWriteBuffer == null, "Need to pend a write but one's already pending.");
                                this._pendingWriteBuffer = buffer;
                                this._pendingWriteBufferManager = bufferManager;
                            }
                        }
                    }
                }
            }
            catch (PipeException e)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(ConvertPipeException(e, TransferOperation.Write), ExceptionEventType);
                throw e;
            }
            finally
            {
                if (shouldReturnBuffer)
                {
                    bufferManager.ReturnBuffer(buffer);
                }
            }
        }

        void ValidateEnterReadingState(bool checkEOF)
        {
            if (checkEOF)
            {
                if (_closeState == CloseState.Closing)
                {
                    //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException("SR.PipeAlreadyClosing"), ExceptionEventType);
                    throw new Exception("SR.PipeAlreadyClosing");
                }
            }

            if (_inReadingState)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException("SR.PipeReadPending"), ExceptionEventType);
                throw new Exception("SR.PipeReadPending");
            }

            if (_closeState == CloseState.HandleClosed)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SR.GetString(SR.PipeClosed)), ExceptionEventType);
                throw new Exception("SR.PipeClosed");
            }
        }

        void ValidateEnterWritingState(bool checkShutdown)
        {
            if (checkShutdown)
            {
                if (_isShutdownWritten)
                {
                    //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SR.GetString(SR.PipeAlreadyShuttingDown)), ExceptionEventType);
                    throw new Exception("SR.PipeAlreadyShuttingDown");
                }

                if (_closeState == CloseState.Closing)
                {
                    //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SR.GetString(SR.PipeAlreadyClosing)), ExceptionEventType);
                    throw new Exception("SR.PipeAlreadyClosing");
                }
            }

            if (_inWritingState)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SR.GetString(SR.PipeWritePending)), ExceptionEventType);
                throw new Exception("SR.PipeWritePending");
            }

            if (_closeState == CloseState.HandleClosed)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SR.GetString(SR.PipeClosed)), ExceptionEventType);
                throw new Exception("SR.PipeClosed");
            }
        }

        void StartSyncRead(byte[] buffer, int offset, int size)
        {
            StartSyncRead(buffer, offset, size, ref this._readOverlapped.Holder[0]);
        }

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        unsafe void StartSyncRead(byte[] buffer, int offset, int size, ref object holder)
        {
            if (this._isReadOutstanding)
            {
                throw Fx.AssertAndThrow("StartSyncRead called when read I/O was already pending.");
            }

            try
            {
                this._isReadOutstanding = true;
                this._readOverlapped.StartSyncOperation(buffer, ref holder);
                if (UnsafeNativeMethods.ReadFile(this._pipe.DangerousGetHandle(), this._readOverlapped.BufferPtr + offset, size, IntPtr.Zero, this._readOverlapped.NativeOverlapped) == 0)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != UnsafeNativeMethods.ERROR_IO_PENDING)
                    {
                        this._isReadOutstanding = false;
                        if (error != UnsafeNativeMethods.ERROR_MORE_DATA)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(Exceptions.CreateReadException(error));
                        }
                    }
                }
                else
                {
                    this._isReadOutstanding = false;
                }
            }
            finally
            {
                if (!this._isReadOutstanding)
                {
                    this._readOverlapped.CancelSyncOperation(ref holder);
                }
            }
        }

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        unsafe void WaitForSyncRead(TimeSpan timeout, bool traceExceptionsAsErrors)
        {
            if (this._isReadOutstanding)
            {
                if (!this._readOverlapped.WaitForSyncOperation(timeout))
                {
                    Abort("SR.PipeConnectionAbortedReadTimedOut, this._readTimeout", TransferOperation.Read);

                    Exception timeoutException = new TimeoutException("SR.PipeReadTimedOut, timeout");
                    TraceEventType traceEventType = TraceEventType.Information;
                    if (traceExceptionsAsErrors)
                    {
                        traceEventType = TraceEventType.Error;
                    }

                    // This intentionally doesn't reset isReadOutstanding, because technically it still is, and we need to not free the buffer.
                    //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(timeoutException, traceEventType);
                    throw timeoutException;
                }
                else
                {
                    this._isReadOutstanding = false;
                }
            }
        }

        // Must be called in a lock.
        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        unsafe int FinishSyncRead(bool traceExceptionsAsErrors)
        {
            int bytesRead = -1;
            Exception readException;

            if (this._closeState == CloseState.HandleClosed)
            {
                readException = CreatePipeClosedException(TransferOperation.Read);
            }
            else
            {
                readException = Exceptions.GetOverlappedReadException(this._pipe, this._readOverlapped.NativeOverlapped, out bytesRead);
            }
            if (readException != null)
            {
                TraceEventType traceEventType = TraceEventType.Information;
                if (traceExceptionsAsErrors)
                {
                    traceEventType = TraceEventType.Error;
                }
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(readException, traceEventType);
                throw readException;
            }

            return bytesRead;
        }

        void StartSyncWrite(byte[] buffer, int offset, int size)
        {
            StartSyncWrite(buffer, offset, size, ref this._writeOverlapped.Holder[0]);
        }

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        unsafe void StartSyncWrite(byte[] buffer, int offset, int size, ref object holder)
        {
            if (this._isWriteOutstanding)
            {
                throw Fx.AssertAndThrow("StartSyncWrite called when write I/O was already pending.");
            }

            try
            {
                this._syncWriteSize = size;
                this._isWriteOutstanding = true;
                this._writeOverlapped.StartSyncOperation(buffer, ref holder);
                if (UnsafeNativeMethods.WriteFile(this._pipe.DangerousGetHandle(), this._writeOverlapped.BufferPtr + offset, size, IntPtr.Zero, this._writeOverlapped.NativeOverlapped) == 0)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != UnsafeNativeMethods.ERROR_IO_PENDING)
                    {
                        this._isWriteOutstanding = false;
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(Exceptions.CreateWriteException(error));
                    }
                }
                else
                {
                    this._isWriteOutstanding = false;
                }
            }
            finally
            {
                if (!this._isWriteOutstanding)
                {
                    this._writeOverlapped.CancelSyncOperation(ref holder);
                }
            }
        }

        void WaitForSyncWrite(TimeSpan timeout, bool traceExceptionsAsErrors)
        {
            WaitForSyncWrite(timeout, traceExceptionsAsErrors, ref this._writeOverlapped.Holder[0]);
        }

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        unsafe void WaitForSyncWrite(TimeSpan timeout, bool traceExceptionsAsErrors, ref object holder)
        {
            if (this._isWriteOutstanding)
            {
                if (!this._writeOverlapped.WaitForSyncOperation(timeout, ref holder))
                {
                    Abort("SR.PipeConnectionAbortedWriteTimedOut, this._writeTimeout", TransferOperation.Write);

                    Exception timeoutException = new TimeoutException("SR.PipeWriteTimedOut, timeout");
                    TraceEventType traceEventType = TraceEventType.Information;
                    if (traceExceptionsAsErrors)
                    {
                        traceEventType = TraceEventType.Error;
                    }

                    // This intentionally doesn't reset isWriteOutstanding, because technically it still is, and we need to not free the buffer.
                    //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(timeoutException, traceEventType);
                    throw timeoutException;
                }
                else
                {
                    this._isWriteOutstanding = false;
                }
            }
        }

        // Must be called in a lock.
        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        unsafe void FinishSyncWrite(bool traceExceptionsAsErrors)
        {
            int bytesWritten;
            Exception writeException;

            if (this._closeState == CloseState.HandleClosed)
            {
                writeException = CreatePipeClosedException(TransferOperation.Write);
            }
            else
            {
                writeException = Exceptions.GetOverlappedWriteException(this._pipe, this._writeOverlapped.NativeOverlapped, out bytesWritten);
                if (writeException == null && bytesWritten != this._syncWriteSize)
                {
                    writeException = new PipeException("SR.PipeWriteIncomplete");
                }
            }

            if (writeException != null)
            {
                TraceEventType traceEventType = TraceEventType.Information;
                if (traceExceptionsAsErrors)
                {
                    traceEventType = TraceEventType.Error;
                }
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(writeException, traceEventType);
                throw writeException;
            }
        }

        //AsyncCompletionResult IConnection.BeginRead(int offset, int size, TimeSpan timeout, Action<object> callback, object state) => throw new NotImplementedException();

        enum CloseState
        {
            Open,
            Closing,
            HandleClosed,
        }

        enum TransferOperation
        {
            Write,
            Read,
            Undefined,
        }

        static class Exceptions
        {
            static PipeException CreateException(string resourceString, int error)
            {
                return new PipeException("resourceString, PipeError.GetErrorString(error)", error);
            }

            public static PipeException CreateReadException(int error)
            {
                return CreateException("SR.PipeReadError", error);
            }

            public static PipeException CreateWriteException(int error)
            {
                return CreateException("SR.PipeWriteError", error);
            }

            // Must be called in a lock, after checking for HandleClosed.
            //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
            [SecuritySafeCritical]
            public static unsafe PipeException GetOverlappedWriteException(PipeHandle pipe,
                NativeOverlapped* nativeOverlapped, out int bytesWritten)
            {
                if (UnsafeNativeMethods.GetOverlappedResult(pipe.DangerousGetHandle(), nativeOverlapped, out bytesWritten, 0) == 0)
                {
                    int error = Marshal.GetLastWin32Error();
                    return Exceptions.CreateWriteException(error);
                }
                else
                {
                    return null;
                }
            }

            // Must be called in a lock, after checking for HandleClosed.
            //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
            [SecuritySafeCritical]
            public static unsafe PipeException GetOverlappedReadException(PipeHandle pipe,
                NativeOverlapped* nativeOverlapped, out int bytesRead)
            {
                if (UnsafeNativeMethods.GetOverlappedResult(pipe.DangerousGetHandle(), nativeOverlapped, out bytesRead, 0) == 0)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error == UnsafeNativeMethods.ERROR_MORE_DATA)
                    {
                        return null;
                    }

                    else
                    {
                        return Exceptions.CreateReadException(error);
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }


    class PipeConnectionInitiator : IConnectionInitiator
    {
        int _bufferSize;
        IPipeTransportFactorySettings _pipeSettings;
        PipeConnectionInitiator _connectionInitiator;

        public PipeConnectionInitiator(int bufferSize, IPipeTransportFactorySettings pipeSettings)
        {
            this._bufferSize = bufferSize;
            this._pipeSettings = pipeSettings;
            _connectionInitiator = this;
        }

        Exception CreateConnectFailedException(Uri remoteUri, PipeException innerException)
        {
            return new CommunicationException(
                "SR.PipeConnectFailed, remoteUri.AbsoluteUri", innerException);
        }

        public IConnection Connect(Uri remoteUri, TimeSpan timeout)
        {
            string resolvedAddress;
            BackoffTimeoutHelper backoffHelper;
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            this.PrepareConnect(remoteUri, timeoutHelper.RemainingTime(), out resolvedAddress, out backoffHelper);

            IConnection connection = null;
            while (connection == null)
            {
                connection = this.TryConnect(remoteUri, resolvedAddress, backoffHelper);
                if (connection == null)
                {
                    backoffHelper.WaitAndBackoff();

                    //if (DiagnosticUtility.ShouldTraceInformation)
                    //{
                    //    TraceUtility.TraceEvent(
                    //        TraceEventType.Information,
                    //        TraceCode.FailedPipeConnect,
                    //        SR.GetString(
                    //            SR.TraceCodeFailedPipeConnect,
                    //            timeoutHelper.RemainingTime(),
                    //            remoteUri));
                    //}
                }
            }
            return connection;
        }

        internal static string GetPipeName(Uri uri, IPipeTransportFactorySettings transportFactorySettings)
        {
            AppContainerInfo appInfo = GetAppContainerInfo(transportFactorySettings);

            // for wildcard hostName support, we first try and connect to the StrongWildcard,
            // then the Exact HostName, and lastly the WeakWildcard
            string[] hostChoices = new string[] { "+", uri.Host, "*" };
            bool[] globalChoices = new bool[] { true, false };
            string matchPath = String.Empty;
            string matchPipeName = null;

            for (int i = 0; i < hostChoices.Length; i++)
            {
                for (int iGlobal = 0; iGlobal < globalChoices.Length; iGlobal++)
                {

                    if (appInfo != null && globalChoices[iGlobal])
                    {
                        // Don't look at shared memory to acces pipes 
                        // that are created in the local NamedObjectPath
                        continue;
                    }

                    // walk up the path hierarchy, looking for match
                    string path = PipeUri.GetPath(uri);

                    while (path.Length > 0)
                    {

                        string sharedMemoryName = PipeUri.BuildSharedMemoryName(hostChoices[i], path, globalChoices[iGlobal], appInfo);
                        try
                        {
                            PipeSharedMemory sharedMemory = PipeSharedMemory.Open(sharedMemoryName, uri);
                            if (sharedMemory != null)
                            {
                                try
                                {
                                    string pipeName = sharedMemory.GetPipeName(appInfo);
                                    if (pipeName != null)
                                    {
                                        // Found a matching pipe name. 
                                        // If the best match app setting is enabled, save the match if it is the best so far and continue.
                                        // Otherwise, just return the first match we find.
                                        //if (ServiceModelAppSettings.UseBestMatchNamedPipeUri)
                                        //{
                                        //    if (path.Length > matchPath.Length)
                                        //    {
                                        //        matchPath = path;
                                        //        matchPipeName = pipeName;
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    return pipeName;
                                        //}

                                        return pipeName;
                                    }
                                }
                                finally
                                {
                                    sharedMemory.Dispose();
                                }
                            }
                        }
                        //catch (AddressAccessDeniedException exception)
                        //{
                        //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new EndpointNotFoundException(SR.GetString(
                        //        SR.EndpointNotFound, uri.AbsoluteUri), exception));
                        //}
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        path = PipeUri.GetParentPath(path);
                    }
                }
            }

            if (string.IsNullOrEmpty(matchPipeName))
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                //    new EndpointNotFoundException(SR.GetString(SR.EndpointNotFound, uri.AbsoluteUri),
                //    new PipeException(SR.GetString(SR.PipeEndpointNotFound, uri.AbsoluteUri))));
                throw new Exception("PipeEndpointNotFound");
            }

            return matchPipeName;
        }

        public IAsyncResult BeginConnect(Uri uri, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ConnectAsyncResult(this, uri, timeout, callback, state);
        }

        public IConnection EndConnect(IAsyncResult result)
        {
            return ConnectAsyncResult.End(result);
        }

        void PrepareConnect(Uri remoteUri, TimeSpan timeout, out string resolvedAddress, out BackoffTimeoutHelper backoffHelper)
        {
            PipeUri.Validate(remoteUri);
            //if (DiagnosticUtility.ShouldTraceInformation)
            //{
            //    TraceUtility.TraceEvent(System.Diagnostics.TraceEventType.Information, TraceCode.InitiatingNamedPipeConnection,
            //        SR.GetString(SR.TraceCodeInitiatingNamedPipeConnection),
            //        new StringTraceRecord("Uri", remoteUri.ToString()), this, null);
            //}
            resolvedAddress = GetPipeName(remoteUri, this._pipeSettings);
            const int backoffBufferMilliseconds = 150;
            TimeSpan backoffTimeout;
            if (timeout >= TimeSpan.FromMilliseconds(backoffBufferMilliseconds * 2))
            {
                backoffTimeout = TimeoutHelper.Add(timeout, TimeSpan.Zero - TimeSpan.FromMilliseconds(backoffBufferMilliseconds));
            }
            else
            {
                backoffTimeout = Ticks.ToTimeSpan((Ticks.FromMilliseconds(backoffBufferMilliseconds) / 2) + 1);
            }

            backoffHelper = new BackoffTimeoutHelper(backoffTimeout, TimeSpan.FromMinutes(5));
        }

        [ResourceConsumption(ResourceScope.Machine)]
        IConnection TryConnect(Uri remoteUri, string resolvedAddress, BackoffTimeoutHelper backoffHelper)
        {
            const int access = UnsafeNativeMethods.GENERIC_READ | UnsafeNativeMethods.GENERIC_WRITE;
            bool lastAttempt = backoffHelper.IsExpired();

            int flags = UnsafeNativeMethods.FILE_FLAG_OVERLAPPED;

            // By default Windows named pipe connection is created with impersonation, but we want
            // to create it with anonymous and let WCF take care of impersonation/identification.
            flags |= UnsafeNativeMethods.SECURITY_QOS_PRESENT | UnsafeNativeMethods.SECURITY_ANONYMOUS;

            PipeHandle pipeHandle = UnsafeNativeMethods.CreateFile(resolvedAddress, access, 0, IntPtr.Zero,
                UnsafeNativeMethods.OPEN_EXISTING, flags, IntPtr.Zero);
            int error = Marshal.GetLastWin32Error();
            if (pipeHandle.IsInvalid)
            {
                pipeHandle.SetHandleAsInvalid();
            }
            else
            {
                int mode = UnsafeNativeMethods.PIPE_READMODE_MESSAGE;
                if (UnsafeNativeMethods.SetNamedPipeHandleState(pipeHandle, ref mode, IntPtr.Zero, IntPtr.Zero) == 0)
                {
                    error = Marshal.GetLastWin32Error();
                    pipeHandle.Close();
                    PipeException innerException = new PipeException("SR.PipeModeChangeFailed,PipeError.GetErrorString(error)", error);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        CreateConnectFailedException(remoteUri, innerException));
                }
                return new PipeConnection(pipeHandle, _bufferSize, false, true);
            }

            if (error == UnsafeNativeMethods.ERROR_FILE_NOT_FOUND || error == UnsafeNativeMethods.ERROR_PIPE_BUSY)
            {
                if (lastAttempt)
                {
                    Exception innerException = new PipeException("SR.PipeConnectAddressFailed,resolvedAddress, PipeError.GetErrorString(error)", error);

                    TimeoutException timeoutException;
                    string endpoint = remoteUri.AbsoluteUri;

                    if (error == UnsafeNativeMethods.ERROR_PIPE_BUSY)
                    {
                        timeoutException = new TimeoutException("SR.PipeConnectTimedOutServerTooBusy,endpoint, backoffHelper.OriginalTimeout", innerException);
                    }
                    else
                    {
                        timeoutException = new TimeoutException("SR.PipeConnectTimedOut,endpoint, backoffHelper.OriginalTimeout", innerException);
                    }

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(timeoutException);
                }

                return null;
            }
            else
            {
                PipeException innerException = new PipeException("SR.PipeConnectAddressFailed,resolvedAddress, PipeError.GetErrorString(error)", error);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    CreateConnectFailedException(remoteUri, innerException));
            }
        }

        static AppContainerInfo GetAppContainerInfo(IPipeTransportFactorySettings transportFactorySettings)
        {
            if (AppContainerInfo.IsAppContainerSupported &&
                transportFactorySettings != null &&
                transportFactorySettings.PipeSettings != null)
            {
                ApplicationContainerSettings appSettings = transportFactorySettings.PipeSettings.ApplicationContainerSettings;
                if (appSettings != null && appSettings.TargetingAppContainer)
                {
                    return AppContainerInfo.CreateAppContainerInfo(appSettings.PackageFullName, appSettings.SessionId);
                }
            }

            return null;
        }
        //Task<IConnection> IConnectionInitiator.ConnectAsync(Uri uri, TimeSpan timeout)
        //{
        //    return Task.Factory.FromAsync<IConnection>(BeginConnect, EndConnect, uri, timeout, null);
        //}

        Task<IConnection> IConnectionInitiator.ConnectAsync(Uri uri, TimeSpan timeout)
        {
            return Task<IConnection>.Factory.FromAsync(_connectionInitiator.BeginConnect,
                                               _connectionInitiator.EndConnect, uri,
                                               timeout, null);
        }

        class ConnectAsyncResult : AsyncResult
        {
            PipeConnectionInitiator _parent;
            Uri _remoteUri;
            string _resolvedAddress;
            BackoffTimeoutHelper _backoffHelper;
            TimeoutHelper _timeoutHelper;
            IConnection _connection;
            static Action<object> s_waitCompleteCallback;

            public ConnectAsyncResult(PipeConnectionInitiator parent, Uri remoteUri, TimeSpan timeout,
                AsyncCallback callback, object state)
                : base(callback, state)
            {
                this._parent = parent;
                this._remoteUri = remoteUri;
                this._timeoutHelper = new TimeoutHelper(timeout);
                parent.PrepareConnect(remoteUri, this._timeoutHelper.RemainingTime(), out this._resolvedAddress, out this._backoffHelper);

                if (this.ConnectAndWait())
                {
                    this.Complete(true);
                }
            }

            bool ConnectAndWait()
            {
                this._connection = this._parent.TryConnect(this._remoteUri, this._resolvedAddress, this._backoffHelper);
                bool completed = (this._connection != null);
                if (!completed)
                {
                    if (s_waitCompleteCallback == null)
                    {
                        s_waitCompleteCallback = new Action<object>(OnWaitComplete);
                    }
                    this._backoffHelper.WaitAndBackoff(s_waitCompleteCallback, this);
                }
                return completed;
            }

            public static IConnection End(IAsyncResult result)
            {
                ConnectAsyncResult thisPtr = AsyncResult.End<ConnectAsyncResult>(result);
                return thisPtr._connection;
            }

            static void OnWaitComplete(object state)
            {
                Exception exception = null;
                ConnectAsyncResult thisPtr = (ConnectAsyncResult)state;

                bool completeSelf = true;
                try
                {
                    //if (DiagnosticUtility.ShouldTraceInformation)
                    //{
                    //    TraceUtility.TraceEvent(
                    //        TraceEventType.Information,
                    //        TraceCode.FailedPipeConnect,
                    //        SR.GetString(
                    //            SR.TraceCodeFailedPipeConnect,
                    //            thisPtr._timeoutHelper.RemainingTime(),
                    //            thisPtr._remoteUri));
                    //}

                    completeSelf = thisPtr.ConnectAndWait();
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    exception = e;
                }

                if (completeSelf)
                {
                    thisPtr.Complete(false, exception);
                }
            }
        }
    }

    class PipeConnectionListener : IConnectionListener
    {
        Uri _pipeUri;
        int _bufferSize;
        HostNameComparisonMode _hostNameComparisonMode;
        bool _isDisposed;
        bool _isListening;
        List<PendingAccept> _pendingAccepts;
        bool _anyPipesCreated;
        PipeSharedMemory _sharedMemory;
        List<SecurityIdentifier> _allowedSids;
        bool _useCompletionPort;
        int _maxInstances;

        public PipeConnectionListener(Uri pipeUri, HostNameComparisonMode hostNameComparisonMode, int bufferSize,
            List<SecurityIdentifier> allowedSids, bool useCompletionPort, int maxConnections)
        {
            PipeUri.Validate(pipeUri);
            this._pipeUri = pipeUri;
            this._hostNameComparisonMode = hostNameComparisonMode;
            this._allowedSids = allowedSids;
            this._bufferSize = bufferSize;
            _pendingAccepts = new List<PendingAccept>();
            this._useCompletionPort = useCompletionPort;
            this._maxInstances = Math.Min(maxConnections, UnsafeNativeMethods.PIPE_UNLIMITED_INSTANCES);
        }

        object ThisLock
        {
            get { return this; }
        }

        public string PipeName { get { return _sharedMemory.PipeName; } }

        public IAsyncResult BeginAccept(AsyncCallback callback, object state)
        {
            lock (ThisLock)
            {
                if (_isDisposed)
                {
                    //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException("", SR.GetString(SR.PipeListenerDisposed)));
                    throw new Exception("PipeListenerDisposed");
                }

                if (!_isListening)
                {
                    //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString(SR.PipeListenerNotListening)));
                    throw new Exception("PipeListenerNotListening");
                }

                PipeHandle pipeHandle = CreatePipe();
                PendingAccept pendingAccept = new PendingAccept(this, pipeHandle, _useCompletionPort, callback, state);
                if (!pendingAccept.CompletedSynchronously)
                {
                    this._pendingAccepts.Add(pendingAccept);
                }
                return pendingAccept;
            }
        }

        public IConnection EndAccept(IAsyncResult result)
        {
            PendingAccept pendingAccept = result as PendingAccept;
            if (pendingAccept == null)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("result", SR.GetString(SR.InvalidAsyncResult));
                throw new Exception("InvalidAsyncResult");
            }

            PipeHandle acceptedPipe = pendingAccept.End();

            if (acceptedPipe == null)
            {
                return null;
            }
            else
            {
                return new PipeConnection(acceptedPipe, _bufferSize,
                    pendingAccept.IsBoundToCompletionPort, pendingAccept.IsBoundToCompletionPort);
            }
        }

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        [ResourceConsumption(ResourceScope.Machine)]
        unsafe PipeHandle CreatePipe()
        {
            int openMode = UnsafeNativeMethods.PIPE_ACCESS_DUPLEX | UnsafeNativeMethods.FILE_FLAG_OVERLAPPED;
            if (!_anyPipesCreated)
            {
                openMode |= UnsafeNativeMethods.FILE_FLAG_FIRST_PIPE_INSTANCE;
            }

            byte[] binarySecurityDescriptor;

            try
            {
                binarySecurityDescriptor = SecurityDescriptorHelper.FromSecurityIdentifiers(_allowedSids, UnsafeNativeMethods.GENERIC_READ | UnsafeNativeMethods.GENERIC_WRITE);
            }
            catch (Win32Exception e)
            {
                //// While Win32exceptions are not expected, if they do occur we need to obey the pipe/communication exception model.
                //Exception innerException = new PipeException(e.Message, e);
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(innerException.Message, innerException));
                throw new Exception(e.Message);
            }

            PipeHandle pipeHandle;
            int error;
            string pipeName = null;
            fixed (byte* pinnedSecurityDescriptor = binarySecurityDescriptor)
            {
                UnsafeNativeMethods.SECURITY_ATTRIBUTES securityAttributes = new UnsafeNativeMethods.SECURITY_ATTRIBUTES();
                securityAttributes._lpSecurityDescriptor = (IntPtr)pinnedSecurityDescriptor;

                pipeName = this._sharedMemory.PipeName;
                pipeHandle = UnsafeNativeMethods.CreateNamedPipe(
                                                    pipeName,
                                                    openMode,
                                                    UnsafeNativeMethods.PIPE_TYPE_MESSAGE | UnsafeNativeMethods.PIPE_READMODE_MESSAGE,
                                                    _maxInstances, _bufferSize, _bufferSize, 0, securityAttributes);
                error = Marshal.GetLastWin32Error();
            }

            if (pipeHandle.IsInvalid)
            {
                pipeHandle.SetHandleAsInvalid();

                //Exception innerException = new PipeException(SR.GetString(SR.PipeListenFailed,
                //    _pipeUri.AbsoluteUri, PipeError.GetErrorString(error)), error);

                //if (error == UnsafeNativeMethods.ERROR_ACCESS_DENIED)
                //{
                //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new AddressAccessDeniedException(innerException.Message, innerException));
                //}
                //else if (error == UnsafeNativeMethods.ERROR_ALREADY_EXISTS)
                //{
                //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new AddressAlreadyInUseException(innerException.Message, innerException));
                //}
                //else
                //{
                //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(innerException.Message, innerException));
                //}

                throw new Exception(PipeError.GetErrorString(error));
            }
            else
            {
                //if (TD.NamedPipeCreatedIsEnabled())
                //{
                //    TD.NamedPipeCreated(pipeName);
                //}
            }

            bool closePipe = true;
            try
            {
                if (_useCompletionPort)
                {
                    ThreadPool.BindHandle(pipeHandle);
                }
                _anyPipesCreated = true;
                closePipe = false;
                return pipeHandle;
            }
            finally
            {
                if (closePipe)
                {
                    pipeHandle.Close();
                }
            }
        }

        public void Dispose()
        {
            lock (ThisLock)
            {
                if (!_isDisposed)
                {
                    if (_sharedMemory != null)
                    {
                        _sharedMemory.Dispose();
                    }
                    for (int i = 0; i < _pendingAccepts.Count; i++)
                    {
                        _pendingAccepts[i].Abort();
                    }
                    _isDisposed = true;
                }
            }
        }

        public void Listen()
        {
            lock (ThisLock)
            {
                if (!_isListening)
                {
                    string sharedMemoryName = PipeUri.BuildSharedMemoryName(_pipeUri, _hostNameComparisonMode, true);
                    if (!PipeSharedMemory.TryCreate(_allowedSids, _pipeUri, sharedMemoryName, out this._sharedMemory))
                    {
                        PipeSharedMemory tempSharedMemory = null;

                        // first see if we're in RANU by creating a unique Uri in the global namespace
                        Uri tempUri = new Uri(_pipeUri, Guid.NewGuid().ToString());
                        string tempSharedMemoryName = PipeUri.BuildSharedMemoryName(tempUri, _hostNameComparisonMode, true);
                        if (PipeSharedMemory.TryCreate(_allowedSids, tempUri, tempSharedMemoryName, out tempSharedMemory))
                        {
                            // we're not RANU, throw PipeNameInUse
                            tempSharedMemory.Dispose();
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                                PipeSharedMemory.CreatePipeNameInUseException(UnsafeNativeMethods.ERROR_ACCESS_DENIED, _pipeUri));
                        }
                        else
                        {
                            // try the session namespace since we're RANU
                            sharedMemoryName = PipeUri.BuildSharedMemoryName(_pipeUri, _hostNameComparisonMode, false);
                            this._sharedMemory = PipeSharedMemory.Create(_allowedSids, _pipeUri, sharedMemoryName);
                        }
                    }

                    _isListening = true;
                }
            }
        }

        void RemovePendingAccept(PendingAccept pendingAccept)
        {
            lock (ThisLock)
            {
                Fx.Assert(this._pendingAccepts.Contains(pendingAccept), "An unknown PendingAccept is removing itself.");
                this._pendingAccepts.Remove(pendingAccept);
            }
        }

        class PendingAccept : AsyncResult
        {
            PipeHandle _pipeHandle;
            PipeHandle _result;
            OverlappedIOCompleteCallback _onAcceptComplete;
            static Action<object> s_onStartAccept;
            OverlappedContext _overlapped;
            bool _isBoundToCompletionPort;
            PipeConnectionListener _listener;
            EventTraceActivity _eventTraceActivity;

            //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
            [SecuritySafeCritical]
            public unsafe PendingAccept(PipeConnectionListener listener, PipeHandle pipeHandle, bool isBoundToCompletionPort,
                AsyncCallback callback, object state)
                : base(callback, state)
            {
                this._pipeHandle = pipeHandle;
                this._result = pipeHandle;
                this._listener = listener;
                _onAcceptComplete = new OverlappedIOCompleteCallback(OnAcceptComplete);
                _overlapped = new OverlappedContext();
                this._isBoundToCompletionPort = isBoundToCompletionPort;

                //if (TD.PipeConnectionAcceptStartIsEnabled())
                //{
                //    this._eventTraceActivity = new EventTraceActivity();
                //    TD.PipeConnectionAcceptStart(this._eventTraceActivity, this._listener._pipeUri != null ? this._listener._pipeUri.ToString() : string.Empty);
                //}

                if (!Thread.CurrentThread.IsThreadPoolThread)
                {
                    if (s_onStartAccept == null)
                    {
                        s_onStartAccept = new Action<object>(OnStartAccept);
                    }
                    ActionItem.Schedule(s_onStartAccept, this);
                }
                else
                {
                    StartAccept(true);
                }
            }

            public bool IsBoundToCompletionPort
            {
                get { return this._isBoundToCompletionPort; }
            }

            static void OnStartAccept(object state)
            {
                PendingAccept pendingAccept = (PendingAccept)state;
                pendingAccept.StartAccept(false);
            }

            Exception CreatePipeAcceptFailedException(int errorCode)
            {
                //Exception innerException = new PipeException(SR.GetString(SR.PipeAcceptFailed,
                //    PipeError.GetErrorString(errorCode)), errorCode);
                //return new CommunicationException(innerException.Message, innerException);

                return new Exception(PipeError.GetErrorString(errorCode));
            }

            //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
            [SecuritySafeCritical]
            unsafe void StartAccept(bool synchronous)
            {
                Exception completionException = null;
                bool completeSelf = false;
#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
                try
                {
                    try
                    {
                        this._overlapped.StartAsyncOperation(null, _onAcceptComplete, this._isBoundToCompletionPort);
                        while (true)
                        {
                            if (UnsafeNativeMethods.ConnectNamedPipe(_pipeHandle, _overlapped.NativeOverlapped) == 0)
                            {
                                int error = Marshal.GetLastWin32Error();
                                switch (error)
                                {
                                    case UnsafeNativeMethods.ERROR_NO_DATA:
                                        if (UnsafeNativeMethods.DisconnectNamedPipe(_pipeHandle) != 0)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            completeSelf = true;
                                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreatePipeAcceptFailedException(error));
                                        }
                                    case UnsafeNativeMethods.ERROR_PIPE_CONNECTED:
                                        completeSelf = true;
                                        break;
                                    case UnsafeNativeMethods.ERROR_IO_PENDING:
                                        break;
                                    default:
                                        completeSelf = true;
                                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreatePipeAcceptFailedException(error));
                                }
                            }
                            else
                            {
                                completeSelf = true;
                            }

                            break;
                        }
                    }
                    catch (ObjectDisposedException exception)
                    {
                        // A race with Abort can cause PipeHandle to throw this.
                        Fx.Assert(this._result == null, "Got an ObjectDisposedException but not an Abort!");
                        //DiagnosticUtility.TraceHandledException(exception, TraceEventType.Information);
                        completeSelf = true;
                    }
                    finally
                    {
                        if (completeSelf)
                        {
                            this._overlapped.CancelAsyncOperation();
                            this._overlapped.Free();
                        }
                    }
                }
#pragma warning suppress 56500 // elliotw, transferring exception to another thread
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    completeSelf = true;
                    completionException = e;
                }
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning
                if (completeSelf)
                {
                    if (!synchronous)
                    {
                        this._listener.RemovePendingAccept(this);
                    }
                    base.Complete(synchronous, completionException);
                }
            }

            // Must be called in PipeConnectionListener's lock.
            public void Abort()
            {
                this._result = null; // we need to return null after an abort
                _pipeHandle.Close();
            }

            public PipeHandle End()
            {
                AsyncResult.End<PendingAccept>(this);
                return this._result;
            }

            //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
            [SecuritySafeCritical]
            unsafe void OnAcceptComplete(bool haveResult, int error, int numBytes)
            {
                this._listener.RemovePendingAccept(this);

                if (!haveResult)
                {
                    // No race with Abort here since Abort can't be called once RemovePendingAccept happens.
                    if (this._result != null && UnsafeNativeMethods.GetOverlappedResult(this._pipeHandle.DangerousGetHandle(),
                        this._overlapped.NativeOverlapped, out numBytes, 0) == 0)
                    {
                        error = Marshal.GetLastWin32Error();
                    }
                    else
                    {
                        error = 0;
                    }
                }

                this._overlapped.Free();

                //if (TD.PipeConnectionAcceptStopIsEnabled())
                //{
                //    TD.PipeConnectionAcceptStop(this._eventTraceActivity);
                //}

                if (error != 0)
                {
                    this._pipeHandle.Close();
                    base.Complete(false, CreatePipeAcceptFailedException(error));
                }
                else
                {
                    base.Complete(false);
                }
            }
        }
    }

    static class SecurityDescriptorHelper
    {
        static byte[] s_worldCreatorOwnerWithReadAndWriteDescriptorDenyNetwork;
        static byte[] s_worldCreatorOwnerWithReadDescriptorDenyNetwork;

        static SecurityDescriptorHelper()
        {
            s_worldCreatorOwnerWithReadAndWriteDescriptorDenyNetwork = FromSecurityIdentifiersFull(null, UnsafeNativeMethods.GENERIC_READ | UnsafeNativeMethods.GENERIC_WRITE);
            s_worldCreatorOwnerWithReadDescriptorDenyNetwork = FromSecurityIdentifiersFull(null, UnsafeNativeMethods.GENERIC_READ);
        }

        internal static byte[] FromSecurityIdentifiers(List<SecurityIdentifier> allowedSids, int accessRights)
        {
            if (allowedSids == null)
            {
                if (accessRights == (UnsafeNativeMethods.GENERIC_READ | UnsafeNativeMethods.GENERIC_WRITE))
                {
                    return s_worldCreatorOwnerWithReadAndWriteDescriptorDenyNetwork;
                }

                if (accessRights == UnsafeNativeMethods.GENERIC_READ)
                {
                    return s_worldCreatorOwnerWithReadDescriptorDenyNetwork;
                }
            }

            return FromSecurityIdentifiersFull(allowedSids, accessRights);
        }

        static byte[] FromSecurityIdentifiersFull(List<SecurityIdentifier> allowedSids, int accessRights)
        {
            int capacity = allowedSids == null ? 3 : 2 + allowedSids.Count;
            DiscretionaryAcl dacl = new DiscretionaryAcl(false, false, capacity);

            // add deny ACE first so that we don't get short circuited
            dacl.AddAccess(AccessControlType.Deny, new SecurityIdentifier(WellKnownSidType.NetworkSid, null),
                UnsafeNativeMethods.GENERIC_ALL, InheritanceFlags.None, PropagationFlags.None);

            // clients get different rights, since they shouldn't be able to listen
            int clientAccessRights = GenerateClientAccessRights(accessRights);

            if (allowedSids == null)
            {
                dacl.AddAccess(AccessControlType.Allow, new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    clientAccessRights, InheritanceFlags.None, PropagationFlags.None);
            }
            else
            {
                for (int i = 0; i < allowedSids.Count; i++)
                {
                    SecurityIdentifier allowedSid = allowedSids[i];
                    dacl.AddAccess(AccessControlType.Allow, allowedSid,
                        clientAccessRights, InheritanceFlags.None, PropagationFlags.None);
                }
            }

            dacl.AddAccess(AccessControlType.Allow, GetProcessLogonSid(), accessRights, InheritanceFlags.None, PropagationFlags.None);


            if (AppContainerInfo.IsRunningInAppContainer)
            {
                // NamedPipeBinding requires dacl with current AppContainer SID
                // to setup multiple NamedPipes in the BeginAccept loop.                
                dacl.AddAccess(
                            AccessControlType.Allow,
                            AppContainerInfo.GetCurrentAppContainerSid(),
                            accessRights,
                            InheritanceFlags.None,
                            PropagationFlags.None);
            }

            CommonSecurityDescriptor securityDescriptor =
                new CommonSecurityDescriptor(false, false, ControlFlags.None, null, null, null, dacl);
            byte[] binarySecurityDescriptor = new byte[securityDescriptor.BinaryLength];
            securityDescriptor.GetBinaryForm(binarySecurityDescriptor, 0);
            return binarySecurityDescriptor;
        }

        // Security: We cannot grant rights for FILE_CREATE_PIPE_INSTANCE to clients, otherwise other apps can intercept server side pipes.
        // FILE_CREATE_PIPE_INSTANCE is granted in 2 ways, via GENERIC_WRITE or directly specified. Remove both.
        static int GenerateClientAccessRights(int accessRights)
        {
            int everyoneAccessRights = accessRights;

            if ((everyoneAccessRights & UnsafeNativeMethods.GENERIC_WRITE) != 0)
            {
                everyoneAccessRights &= ~UnsafeNativeMethods.GENERIC_WRITE;

                // Since GENERIC_WRITE grants the permissions to write to a file, we need to add it back.
                const int clientWriteAccess = UnsafeNativeMethods.FILE_WRITE_ATTRIBUTES | UnsafeNativeMethods.FILE_WRITE_DATA | UnsafeNativeMethods.FILE_WRITE_EA;
                everyoneAccessRights |= clientWriteAccess;
            }

            // Future proofing: FILE_CREATE_PIPE_INSTANCE isn't used currently but we need to ensure it is not granted.
            everyoneAccessRights &= ~UnsafeNativeMethods.FILE_CREATE_PIPE_INSTANCE;

            return everyoneAccessRights;
        }

        // The logon sid is generated on process start up so it is unique to this process.
        static SecurityIdentifier GetProcessLogonSid()
        {
            int pid = Process.GetCurrentProcess().Id;
            return System.ServiceModel.Activation.Utility.GetLogonSidForPid(pid);
        }
    }

    unsafe class PipeSharedMemory : IDisposable
    {
        internal const string PipePrefix = @"\\.\pipe\";
        internal const string PipeLocalPrefix = @"\\.\pipe\Local\";
        SafeFileMappingHandle _fileMapping;
        string _pipeName;
        string _pipeNameGuidPart;
        Uri _pipeUri;

        PipeSharedMemory(SafeFileMappingHandle fileMapping, Uri pipeUri)
            : this(fileMapping, pipeUri, null)
        {
        }

        PipeSharedMemory(SafeFileMappingHandle fileMapping, Uri pipeUri, string pipeName)
        {
            this._pipeName = pipeName;
            this._fileMapping = fileMapping;
            this._pipeUri = pipeUri;
        }

        public static PipeSharedMemory Create(List<SecurityIdentifier> allowedSids, Uri pipeUri, string sharedMemoryName)
        {
            PipeSharedMemory result;
            if (TryCreate(allowedSids, pipeUri, sharedMemoryName, out result))
            {
                return result;
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreatePipeNameInUseException(UnsafeNativeMethods.ERROR_ACCESS_DENIED, pipeUri));
            }
        }

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        public unsafe static bool TryCreate(List<SecurityIdentifier> allowedSids, Uri pipeUri, string sharedMemoryName, out PipeSharedMemory result)
        {
            Guid pipeGuid = Guid.NewGuid();
            string pipeName = BuildPipeName(pipeGuid.ToString());
            byte[] binarySecurityDescriptor;
            try
            {
                binarySecurityDescriptor = SecurityDescriptorHelper.FromSecurityIdentifiers(allowedSids, UnsafeNativeMethods.GENERIC_READ);
            }
            catch (Win32Exception e)
            {
                //// While Win32exceptions are not expected, if they do occur we need to obey the pipe/communication exception model.
                //Exception innerException = new PipeException(e.Message, e);
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(innerException.Message, innerException));
                throw new Exception(e.Message);
            }

            SafeFileMappingHandle fileMapping;
            int error;
            result = null;
            fixed (byte* pinnedSecurityDescriptor = binarySecurityDescriptor)
            {
                UnsafeNativeMethods.SECURITY_ATTRIBUTES securityAttributes = new UnsafeNativeMethods.SECURITY_ATTRIBUTES();
                securityAttributes._lpSecurityDescriptor = (IntPtr)pinnedSecurityDescriptor;

                fileMapping = UnsafeNativeMethods.CreateFileMapping((IntPtr)(-1), securityAttributes,
                    UnsafeNativeMethods.PAGE_READWRITE, 0, sizeof(SharedMemoryContents), sharedMemoryName);
                error = Marshal.GetLastWin32Error();
            }

            if (fileMapping.IsInvalid)
            {
                fileMapping.SetHandleAsInvalid();
                if (error == UnsafeNativeMethods.ERROR_ACCESS_DENIED)
                {
                    return false;
                }
                else
                {
                    //Exception innerException = new PipeException(SR.GetString(SR.PipeNameCantBeReserved,
                    //    pipeUri.AbsoluteUri, PipeError.GetErrorString(error)), error);
                    //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new AddressAccessDeniedException(innerException.Message, innerException));
                    throw new Exception(PipeError.GetErrorString(error));
                }
            }

            // now we have a valid file mapping handle
            if (error == UnsafeNativeMethods.ERROR_ALREADY_EXISTS)
            {
                fileMapping.Close();
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreatePipeNameInUseException(error, pipeUri));
            }
            PipeSharedMemory pipeSharedMemory = new PipeSharedMemory(fileMapping, pipeUri, pipeName);
            bool disposeSharedMemory = true;
            try
            {
                pipeSharedMemory.InitializeContents(pipeGuid);
                disposeSharedMemory = false;
                result = pipeSharedMemory;

                //if (TD.PipeSharedMemoryCreatedIsEnabled())
                //{
                //    TD.PipeSharedMemoryCreated(sharedMemoryName);
                //}
                return true;
            }
            finally
            {
                if (disposeSharedMemory)
                {
                    pipeSharedMemory.Dispose();
                }
            }
        }

        [ResourceConsumption(ResourceScope.Machine)]
        public static PipeSharedMemory Open(string sharedMemoryName, Uri pipeUri)
        {
            SafeFileMappingHandle fileMapping = UnsafeNativeMethods.OpenFileMapping(
                UnsafeNativeMethods.FILE_MAP_READ, false, sharedMemoryName);
            if (fileMapping.IsInvalid)
            {
                int error = Marshal.GetLastWin32Error();
                fileMapping.SetHandleAsInvalid();
                if (error == UnsafeNativeMethods.ERROR_FILE_NOT_FOUND)
                {
                    fileMapping = UnsafeNativeMethods.OpenFileMapping(
                        UnsafeNativeMethods.FILE_MAP_READ, false, "Global\\" + sharedMemoryName);
                    if (fileMapping.IsInvalid)
                    {
                        error = Marshal.GetLastWin32Error();
                        fileMapping.SetHandleAsInvalid();
                        if (error == UnsafeNativeMethods.ERROR_FILE_NOT_FOUND)
                        {
                            return null;
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreatePipeNameCannotBeAccessedException(error, pipeUri));
                    }
                    return new PipeSharedMemory(fileMapping, pipeUri);
                }

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreatePipeNameCannotBeAccessedException(error, pipeUri));
            }
            return new PipeSharedMemory(fileMapping, pipeUri);
        }

        public void Dispose()
        {
            if (_fileMapping != null)
            {
                _fileMapping.Close();
                _fileMapping = null;
            }
        }

        public string PipeName
        {
            //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
            [SecuritySafeCritical]
            get
            {
                if (_pipeName == null)
                {
                    SafeViewOfFileHandle view = GetView(false);
                    try
                    {
                        SharedMemoryContents* contents = (SharedMemoryContents*)view.DangerousGetHandle();
                        if (contents->isInitialized)
                        {
                            Thread.MemoryBarrier();
                            this._pipeNameGuidPart = contents->pipeGuid.ToString();
                            this._pipeName = BuildPipeName(this._pipeNameGuidPart);
                        }
                    }
                    finally
                    {
                        view.Close();
                    }
                }
                return _pipeName;
            }
        }

        internal string GetPipeName(AppContainerInfo appInfo)
        {
            if (appInfo == null)
            {
                return this.PipeName;
            }
            else if (this.PipeName != null)
            {
                // Build the PipeName for a pipe inside an AppContainer as follows
                // \\.\pipe\Sessions\<SessionId>\<NamedObjectPath>\<PipeGuid>
                return string.Format(
                            CultureInfo.InvariantCulture,
                            @"\\.\pipe\Sessions\{0}\{1}\{2}",
                            appInfo.SessionId,
                            appInfo.NamedObjectPath,
                            this._pipeNameGuidPart);
            }

            return null;
        }

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        [SecuritySafeCritical]
        void InitializeContents(Guid pipeGuid)
        {
            SafeViewOfFileHandle view = GetView(true);
            try
            {
                SharedMemoryContents* contents = (SharedMemoryContents*)view.DangerousGetHandle();
                contents->pipeGuid = pipeGuid;
                Thread.MemoryBarrier();
                contents->isInitialized = true;
            }
            finally
            {
                view.Close();
            }
        }

        public static Exception CreatePipeNameInUseException(int error, Uri pipeUri)
        {
            Exception innerException = new PipeException($"SR.PipeNameInUse, {pipeUri.AbsoluteUri}", error);
            //return new AddressAlreadyInUseException(innerException.Message, innerException);
            throw innerException;
        }

        static Exception CreatePipeNameCannotBeAccessedException(int error, Uri pipeUri)
        {
            Exception innerException = new PipeException("SR.PipeNameCanNotBeAccessed,PipeError.GetErrorString(error)", error);
            //return new AddressAccessDeniedException(SR.GetString(SR.PipeNameCanNotBeAccessed2, pipeUri.AbsoluteUri), innerException);
            throw innerException;
        }

        SafeViewOfFileHandle GetView(bool writable)
        {
            SafeViewOfFileHandle handle = UnsafeNativeMethods.MapViewOfFile(_fileMapping,
                writable ? UnsafeNativeMethods.FILE_MAP_WRITE : UnsafeNativeMethods.FILE_MAP_READ,
                0, 0, (IntPtr)sizeof(SharedMemoryContents));
            if (handle.IsInvalid)
            {
                int error = Marshal.GetLastWin32Error();
                handle.SetHandleAsInvalid();
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreatePipeNameCannotBeAccessedException(error, _pipeUri));
            }
            return handle;
        }

        static string BuildPipeName(string pipeGuid)
        {
            return (AppContainerInfo.IsRunningInAppContainer ? PipeLocalPrefix : PipePrefix) + pipeGuid;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SharedMemoryContents
        {
            public bool isInitialized;
            public Guid pipeGuid;
        }
    }

    static class PipeUri
    {
        public static string BuildSharedMemoryName(Uri uri, HostNameComparisonMode hostNameComparisonMode, bool global)
        {
            string path = PipeUri.GetPath(uri);
            string host = null;

            switch (hostNameComparisonMode)
            {
                case HostNameComparisonMode.StrongWildcard:
                    host = "+";
                    break;
                case HostNameComparisonMode.Exact:
                    host = uri.Host;
                    break;
                case HostNameComparisonMode.WeakWildcard:
                    host = "*";
                    break;
            }

            return PipeUri.BuildSharedMemoryName(host, path, global);
        }

        internal static string BuildSharedMemoryName(string hostName, string path, bool global, AppContainerInfo appContainerInfo)
        {
            if (appContainerInfo == null)
            {
                return BuildSharedMemoryName(hostName, path, global);
            }
            else
            {
                Fx.Assert(appContainerInfo.SessionId != ApplicationContainerSettingsDefaults.CurrentSession, "Session has not yet been initialized.");
                Fx.Assert(!String.IsNullOrEmpty(appContainerInfo.NamedObjectPath),
                    "NamedObjectPath cannot be empty when creating the SharedMemoryName when running in an AppContainer.");

                //We need to use a session symlink for the lowbox appcontainer.
                // Session\{0}\{1}\{2}\<SharedMemoryName>                
                return string.Format(
                            CultureInfo.InvariantCulture,
                            @"Session\{0}\{1}\{2}",
                            appContainerInfo.SessionId,
                            appContainerInfo.NamedObjectPath,
                            BuildSharedMemoryName(hostName, path, global));
            }
        }

        static string BuildSharedMemoryName(string hostName, string path, bool global)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Uri.UriSchemeNetPipe);
            builder.Append("://");
            builder.Append(hostName.ToUpperInvariant());
            builder.Append(path);
            string canonicalName = builder.ToString();

            byte[] canonicalBytes = Encoding.UTF8.GetBytes(canonicalName);
            byte[] hashedBytes;
            string separator;

            if (canonicalBytes.Length >= 128)
            {
                using (HashAlgorithm hash = GetHashAlgorithm())
                {
                    hashedBytes = hash.ComputeHash(canonicalBytes);
                }
                separator = ":H";
            }
            else
            {
                hashedBytes = canonicalBytes;
                separator = ":E";
            }

            builder = new StringBuilder();
            if (global)
            {
                // we may need to create the shared memory in the global namespace so we work with terminal services+admin 
                builder.Append("Global\\");
            }
            else
            {
                builder.Append("Local\\");
            }
            builder.Append(Uri.UriSchemeNetPipe);
            builder.Append(separator);
            builder.Append(Convert.ToBase64String(hashedBytes));
            return builder.ToString();
        }

        [SuppressMessage("Microsoft.Security.Cryptography", "CA5354:DoNotUseSHA1", Justification = "Cannot change. It will cause compatibility issue. Not used for cryptographic purposes.")]
        static HashAlgorithm GetHashAlgorithm()
        {
            //if (!LocalAppContextSwitches.UseSha1InPipeConnectionGetHashAlgorithm)
            //{
            //    if (SecurityUtilsEx.RequiresFipsCompliance)
            //        return new SHA256CryptoServiceProvider();
            //    else
            //        return new SHA256Managed();
            //}
            //else
            //{
            //    if (SecurityUtilsEx.RequiresFipsCompliance)
            //        return new SHA1CryptoServiceProvider();
            //    else
            //        return new SHA1Managed();
            //}

            return new SHA256Managed();
        }

        public static string GetPath(Uri uri)
        {
            string path = uri.LocalPath.ToUpperInvariant();
            if (!path.EndsWith("/", StringComparison.Ordinal))
                path = path + "/";
            return path;
        }

        public static string GetParentPath(string path)
        {
            if (path.EndsWith("/", StringComparison.Ordinal))
                path = path.Substring(0, path.Length - 1);
            if (path.Length == 0)
                return path;
            return path.Substring(0, path.LastIndexOf('/') + 1);
        }

        public static void Validate(Uri uri)
        {
            if (uri.Scheme != Uri.UriSchemeNetPipe)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("uri", "SR.PipeUriSchemeWrong");
        }
    }

    static class PipeError
    {
        public static string GetErrorString(int error)
        {
            StringBuilder stringBuilder = new StringBuilder(512);
            if (UnsafeNativeMethods.FormatMessage(UnsafeNativeMethods.FORMAT_MESSAGE_IGNORE_INSERTS |
                UnsafeNativeMethods.FORMAT_MESSAGE_FROM_SYSTEM | UnsafeNativeMethods.FORMAT_MESSAGE_ARGUMENT_ARRAY,
                IntPtr.Zero, error, CultureInfo.CurrentCulture.LCID, stringBuilder, stringBuilder.Capacity, IntPtr.Zero) != 0)
            {
                stringBuilder = stringBuilder.Replace("\n", "");
                stringBuilder = stringBuilder.Replace("\r", "");
                //return SR.GetString(
                //    SR.PipeKnownWin32Error,
                //    stringBuilder.ToString(),
                //    error.ToString(CultureInfo.InvariantCulture),
                //    Convert.ToString(error, 16));
                return "SR.PipeKnownWin32Error:"+ Convert.ToString(error, 16) + ":" + stringBuilder.ToString();
            }
            else
            {
                //return SR.GetString(
                //    SR.PipeUnknownWin32Error,
                //    error.ToString(CultureInfo.InvariantCulture),
                //    Convert.ToString(error, 16));
                return "SR.PipeKnownWin32Error:" + Convert.ToString(error, 16);
            }
        }
    }
}
