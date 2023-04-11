// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime;
using System.ServiceModel.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Tools.ServiceModel.Svcutil.FrameworkFork.System.ServiceModel.Resources;

namespace System.ServiceModel.Channels
{
    internal sealed class PipeConnection : IConnection
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
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
    using System.Security.AccessControl;
    //using SafeCloseHandle = System.ServiceModel.Activation.SafeCloseHandle;
    using Microsoft.Xml;
    using MS.Internal.Xml.XPath;
    using static System.ServiceModel.Channels.SingletonMessageDecoder;
    using System.Drawing;
    using System.Runtime.InteropServices.ComTypes;

    sealed class PipeConnection : IConnection
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
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
    using System.Security.AccessControl;
    using System.ComponentModel;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Permissions;
        public async ValueTask<int> ReadAsync(Memory<byte> buffer, TimeSpan timeout)
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
        private readonly NamedPipeClientStream _pipe;
        private CloseState _closeState;
        private bool _aborted;
        private TraceEventType _exceptionEventType;
        private static readonly byte[] s_zeroBuffer = Array.Empty<byte>();
        byte[] _asyncReadBuffer;
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
        int _asyncBytesRead;
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
            this._asyncReadBuffer = DiagnosticUtility.Utility.AllocateByteArray(connectionBufferSize);
            _pipe = namedPipeClient;
            _closeState = CloseState.Open;
        public async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, bool immediate, TimeSpan timeout)
        {
            ValidateBufferBounds(buffer);
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
            finally
            {
                lock (_writeLock)
            if (_timeoutErrorString != null)
                    ExitWritingState();
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

        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        unsafe AsyncCompletionResult IConnection.BeginRead(int offset, int size, TimeSpan timeout,
            /*WaitCallback callback */Action<object> callback, object state)
        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                                new PipeException(SRServiceModel.PipeCantCloseWithPendingWrite), ExceptionEventType);
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

                    return this._isReadOutstanding ? AsyncCompletionResult.Queued : AsyncCompletionResult.Completed;
                }
                catch (PipeException e)
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
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            var cancellationToken = await timeoutHelper.GetCancellationTokenAsync();

            lock (_writeLock)
            {
                ValidateEnterWritingState(true);
                EnterWritingState();
            }

            try
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                            new TimeoutException(SRServiceModel.PipeShutdownReadError, e), ExceptionEventType);
                    if (this._isWriteOutstanding)
                    {
                        throw Fx.AssertAndThrow("Write I/O already pending when BeginWrite called.");
                    }

                    try
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                            new TimeoutException(SRServiceModel.PipeShutdownReadError), ExceptionEventType);

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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                    new TimeoutException(SRServiceModel.PipeCloseFailed, e), ExceptionEventType);
                        if (writeException != null)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(writeException);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                    ConvertPipeException(SRServiceModel.PipeCloseFailed, e, TransferOperation.Undefined), ExceptionEventType);
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

        public async ValueTask CloseAsync(TimeSpan timeout)
        {
            bool existingReadIsPending = false;
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            var cancellationToken = await timeoutHelper.GetCancellationTokenAsync();

            bool shouldClosePipe = false;
                    _timeoutErrorString = timeoutErrorString;
                    _timeoutErrorTransferOperation = transferOperation;
                    _aborted = abort;
                    _closeState = CloseState.PipeClosed;
                    _pipe.Close();
                    _atEOFTask.TrySetResult(true);
                            {
                                existingReadIsPending = true;

            if (abort)
            {
                TraceEventType traceEventType = TraceEventType.Warning;

                // we could be timing out a cached connection
                if (ExceptionEventType == TraceEventType.Information)
                {
                    traceEventType = ExceptionEventType;
                }
                            _isShutdownWritten = true;
                //if (DiagnosticUtility.ShouldTrace(traceEventType))
                //{
                //    TraceUtility.TraceEvent(traceEventType, TraceCode.PipeConnectionAbort, SR.TraceCodePipeConnectionAbort, this);
                //}
                {
                    DiagnosticUtility.TraceHandledException(e, TraceEventType.Information);
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
                if (shouldClosePipe)
                {
                    ClosePipe(false, null, TransferOperation.Undefined);
                }
            }
        }

        private void ClosePipe(bool abort, string timeoutErrorString, TransferOperation transferOperation)
        {
        private bool IsBrokenPipeError(int error)
        //    }
        //    try
        //    {
        //        // no need to close this handle, it's a pseudo handle. expected value is -1.
        //        IntPtr sourceProcessHandle = ListenerUnsafeNativeMethods.GetCurrentProcess();
        //        if (sourceProcessHandle == IntPtr.Zero)
        //        {
            return ConvertPipeException(new PipeException(SRServiceModel.PipeClosed), transferOperation);
        //                CreatePipeDuplicationFailedException(Marshal.GetLastWin32Error()), ExceptionEventType);
        //        }
        private ValueTask<int> StartReadZeroAsync(CancellationToken token)
        //        if (!success)
        //        {
        //            throw DiagnosticUtility.ExceptionUtility.ThrowHelper(
                ValidateEnterReadingState(false);
                EnterReadingState();
                return _pipe.ReadAsync(ZeroBuffer, token);
            }
        }
                        {
        private ValueTask StartWriteZeroAsync(CancellationToken token)
        {
            lock (_writeLock)
            {
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
                Abort(string.Format(SRServiceModel.PipeConnectionAbortedWriteTimedOut, timeout), TransferOperation.Write);
                    {
                        try
                        {
                            if (success)
                            {
                                bytesRead = FinishSyncRead(true);
                                HandleReadComplete(bytesRead);
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

        private async ValueTask WaitForWriteZero(ValueTask writeTask, TimeSpan timeout, bool traceExceptionsAsErrors)
        {
            try
            {
                await writeTask;
            }
            catch(Exception)
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SRServiceModel.PipeClosed), ExceptionEventType);
                            WriteIOCompleted();
                        }
                    }
                }
            }
        }

        public void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout)
        {
            WriteHelper(buffer, offset, size, immediate, timeout, ref this._writeOverlapped.Holder[0]);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SRServiceModel.PipeAlreadyShuttingDown), ExceptionEventType);
        // The holder is a perf optimization that lets us avoid repeatedly indexing into the array.
        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        unsafe void WriteHelper(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, ref object holder)
        {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SRServiceModel.PipeAlreadyClosing), ExceptionEventType);
                FinishPendingWrite(timeout);

                ConnectionUtilities.ValidateBufferBounds(buffer, offset, size);

                int bytesToWrite = size;
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SRServiceModel.PipeWritePending), ExceptionEventType);
            }

            if (_closeState == CloseState.PipeClosed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SRServiceModel.PipeClosed), ExceptionEventType);
                    try
                    {
                        shouldReturnBuffer = false;
        internal static void ValidateBufferBounds(ReadOnlyMemory<byte> buffer)
        {
            if (buffer.IsEmpty)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(buffer));
            }
        }
            {
                if (_closeState == CloseState.Closing)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SRServiceModel.PipeAlreadyClosing), ExceptionEventType);
                }
            }

        AsyncCompletionResult IConnection.BeginWrite(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, Action<object> callback, object state) => throw new NotImplementedException();
        public void EndWrite()
        {
            if (checkShutdown)
        }
        void IConnection.Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout) => throw new NotImplementedException();
        void IConnection.Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, BufferManager bufferManager) => throw new NotImplementedException();
        int IConnection.Read(byte[] buffer, int offset, int size, TimeSpan timeout) => throw new NotImplementedException();
        AsyncCompletionResult IConnection.BeginRead(int offset, int size, TimeSpan timeout, Action<object> callback, object state) => throw new NotImplementedException();
       public  int EndRead()
        {
            return _asyncBytesRead;
                    //throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new PipeException(SR.GetString(SR.PipeAlreadyClosing)), ExceptionEventType);
  
                throw new Exception("SR.PipeWritePending");
        private enum CloseState
        {
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
