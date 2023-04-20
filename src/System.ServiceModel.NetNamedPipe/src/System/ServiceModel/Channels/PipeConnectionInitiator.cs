// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceModel.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    [SupportedOSPlatform("windows")]
    internal class PipeConnectionInitiator : IConnectionInitiator
    {
        private readonly int _bufferSize;

        public PipeConnectionInitiator(int bufferSize)
        {
            _bufferSize = bufferSize;
        }

        private Exception CreateConnectFailedException(Uri remoteUri, PipeException innerException)
        {
            return new CommunicationException(
                SR.Format(SR.PipeConnectFailed, remoteUri.AbsoluteUri), innerException);
        }

        public async ValueTask<IConnection> ConnectAsync(Uri remoteUri, TimeSpan timeout)
        {
            string resolvedAddress;
            BackoffTimeoutHelper backoffHelper;
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            PrepareConnect(remoteUri, timeoutHelper.RemainingTime(), out resolvedAddress, out backoffHelper);

            IConnection connection = null;
            while (connection == null)
            {
                connection =  TryConnect(remoteUri, resolvedAddress, backoffHelper);
                if (connection == null)
                {
                    await backoffHelper.WaitAndBackoffAsync();
                    if (DiagnosticUtility.ShouldTraceInformation)
                    {
                        TraceUtility.TraceEvent(
                            TraceEventType.Information,
                            TraceCode.FailedPipeConnect,
                            SR.Format(
                                SR.TraceCodeFailedPipeConnect,
                                timeoutHelper.RemainingTime(),
                                remoteUri));
                    }
                }
            }

            return connection;
        }

        internal const string UseBestMatchNamedPipeUriString = "wcf:useBestMatchNamedPipeUri";
        internal static bool s_useBestMatchNamedPipeUri = AppContext.TryGetSwitch(UseBestMatchNamedPipeUriString, out bool enabled) && enabled;

        internal static string GetPipeName(Uri uri)
        {
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
                    // walk up the path hierarchy, looking for match
                    string path = PipeUri.GetPath(uri);

                    while (path.Length > 0)
                    {

                        string sharedMemoryName = PipeUri.BuildSharedMemoryName(hostChoices[i], path, globalChoices[iGlobal]);
                        try
                        {
                            PipeSharedMemory sharedMemory = PipeSharedMemory.Open(sharedMemoryName, uri);
                            if (sharedMemory != null)
                            {
                                try
                                {
                                    string pipeName = sharedMemory.PipeName;
                                    if (pipeName != null)
                                    {
                                        // Found a matching pipe name. 
                                        // If the best match app setting is enabled, save the match if it is the best so far and continue.
                                        // Otherwise, just return the first match we find.
                                        if (s_useBestMatchNamedPipeUri)
                                        {
                                            if (path.Length > matchPath.Length)
                                            {
                                                matchPath = path;
                                                matchPipeName = pipeName;
                                            }
                                        }
                                        else
                                        {
                                            return pipeName;
                                        }
                                    }
                                }
                                finally
                                {
                                    sharedMemory.Dispose();
                                }
                            }
                        }
                        catch (AddressAccessDeniedException exception)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new EndpointNotFoundException(SR.Format(
                                SR.EndpointNotFound, uri.AbsoluteUri), exception));
                        }

                        path = PipeUri.GetParentPath(path);
                    }
                }
            }

            if (string.IsNullOrEmpty(matchPipeName))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new EndpointNotFoundException(SR.Format(SR.EndpointNotFound, uri.AbsoluteUri),
                    new PipeException(SR.Format(SR.PipeEndpointNotFound, uri.AbsoluteUri))));
            }

            return matchPipeName;
        }

        private void PrepareConnect(Uri remoteUri, TimeSpan timeout, out string resolvedAddress, out BackoffTimeoutHelper backoffHelper)
        {
            PipeUri.Validate(remoteUri);
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.InitiatingNamedPipeConnection,
                    SR.TraceCodeInitiatingNamedPipeConnection,
                    new StringTraceRecord("Uri", remoteUri.ToString()), this, null);
            }
            resolvedAddress = GetPipeName(remoteUri);
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

        private IConnection TryConnect(Uri remoteUri, string resolvedAddress, BackoffTimeoutHelper backoffHelper)
        {
            bool lastAttempt = backoffHelper.IsExpired();
            // NamedPipeClientStream opens a pipe with the name "\\{serverName}\pipe\{pipeName}". As we only connect
            // to local pipes, we pass "." as the serverName.
            // PipeDirection means the pipe is opened READ/WRITE
            // PipeOptions.Asynchronous sets the flag FILE_FLAG_OVERLAPPED and binds the handle to the threadpool IOCP
            // TokenImpersonationLevel.Anonymous sets the SECURITY_QOS_PRESENT flag and disables impersonation 
            // HandleInheritability.None sets the security attributes to null
            NamedPipeClientStream namedPipeClient;
            try
            {
                namedPipeClient = new NamedPipeClientStream(".", resolvedAddress, PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.Anonymous, HandleInheritability.None);
                // Don't use ConnectAsync as it uses Task.Factory.StartNew to call the synchronous Connect code on a background thread.
                // We pass a timeout of 1 as NamedPipeClient might call WaitNamedPipe which synchronously blocks waiting for the service
                // to call ConnectNamedPipe on the named pipe. NamedPipeClientStream calls CreateFile to connect, and if it can't,
                // will call WaitNamedPipe passing in the timeout. It waits this much time for the service to call ConnectNamedPipe. If
                // we pass a value of 0, it will use the default timeout configured on the named pipe. WCF services specify zero which
                // means use the API default which is 50ms. As we can't prevent NamedPipeClientStream from calling WaitNamedPipe, the
                // best we can do is pass 1ms.
                namedPipeClient.Connect(1);
            }
            catch (Exception ex)
            {
                int error = PipeError.GetErrorFromHResult(ex.HResult);

                if (error == UnsafeNativeMethods.ERROR_FILE_NOT_FOUND || error == UnsafeNativeMethods.ERROR_PIPE_BUSY)
                {
                    if (lastAttempt)
                    {
                        Exception innerException = new PipeException(SR.Format(SR.PipeConnectAddressFailed,
                            resolvedAddress, ex.Message), ex.HResult);

                        TimeoutException timeoutException;
                        string endpoint = remoteUri.AbsoluteUri;

                        if (error == UnsafeNativeMethods.ERROR_PIPE_BUSY)
                        {
                            timeoutException = new TimeoutException(SR.Format(SR.PipeConnectTimedOutServerTooBusy,
                                endpoint, backoffHelper.OriginalTimeout), innerException);
                        }
                        else
                        {
                            timeoutException = new TimeoutException(SR.Format(SR.PipeConnectTimedOut,
                                endpoint, backoffHelper.OriginalTimeout), innerException);
                        }

                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(timeoutException);
                    }
                }
                else
                {
                    PipeException innerException = new PipeException(SR.Format(SR.PipeConnectAddressFailed,
                        resolvedAddress, ex.Message), error);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        CreateConnectFailedException(remoteUri, innerException));
                }

                return null;
            }

            try
            {
                namedPipeClient.ReadMode = PipeTransmissionMode.Message;
            }
            catch(Exception ex)
            {
                PipeException innerException = new PipeException(SR.Format(SR.PipeModeChangeFailed, ex.Message), ex.HResult);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    CreateConnectFailedException(remoteUri, innerException));

            }

            return new PipeConnection(namedPipeClient, _bufferSize);
        }
    }

    [SupportedOSPlatform("windows")]
    internal unsafe class PipeSharedMemory : IDisposable
    {
        internal const string PipeLocalPrefix = @"Local\";
        private MemoryMappedFile _fileMapping;
        private string _pipeName;
        private readonly Uri _pipeUri;

        private PipeSharedMemory(MemoryMappedFile fileMapping, Uri pipeUri)
            : this(fileMapping, pipeUri, null)
        {
        }

        private PipeSharedMemory(MemoryMappedFile fileMapping, Uri pipeUri, string pipeName)
        {
            _pipeName = pipeName;
            _fileMapping = fileMapping;
            _pipeUri = pipeUri;
        }

        public static PipeSharedMemory Open(string sharedMemoryName, Uri pipeUri)
        {
            MemoryMappedFile memoryMappedFile;
            try
            {
                memoryMappedFile = MemoryMappedFile.OpenExisting(sharedMemoryName, MemoryMappedFileRights.Read,
                                                                        HandleInheritability.None);
            }
            catch (FileNotFoundException)
            {
                try
                {
                    memoryMappedFile = MemoryMappedFile.OpenExisting("Global\\" + sharedMemoryName, MemoryMappedFileRights.Read,
                                                                            HandleInheritability.None);
                }
                catch(FileNotFoundException)
                {
                    return null;
                }
                catch(IOException ioe)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreatePipeNameCannotBeAccessedException(ioe, pipeUri));
                }
            }

            return new PipeSharedMemory(memoryMappedFile, pipeUri);
        }

        public void Dispose()
        {
            if (_fileMapping != null)
            {
                _fileMapping.Dispose();
                _fileMapping = null;
            }
        }

        public string PipeName
        {
            get
            {
                if (_pipeName == null)
                {
                    using (MemoryMappedViewStream view = GetView(false))
                    {
                        SharedMemoryContents contents = view.SafeMemoryMappedViewHandle.Read<SharedMemoryContents>(0);
                        _pipeName = contents.pipeGuid.ToString();
                    }
                }

                return _pipeName;
            }
        }

        private static Exception CreatePipeNameCannotBeAccessedException(IOException ioe, Uri pipeUri)
        {
            Exception innerException = new PipeException(SR.Format(SR.PipeNameCanNotBeAccessed,
                PipeError.GetErrorString(ioe)), ioe.HResult & 0x7FF8FFFF);
            return new AddressAccessDeniedException(SR.Format(SR.PipeNameCanNotBeAccessed2, pipeUri.AbsoluteUri), innerException);
        }

        private MemoryMappedViewStream GetView(bool writable)
        {
            try
            {
                return _fileMapping.CreateViewStream(0, sizeof(SharedMemoryContents), writable ? MemoryMappedFileAccess.Write : MemoryMappedFileAccess.Read);
            }
            catch(IOException ioe)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreatePipeNameCannotBeAccessedException(ioe, _pipeUri));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SharedMemoryContents
        {
            public bool isInitialized;
            public Guid pipeGuid;
        }
    }

    internal static class PipeUri
    {
        public static string BuildSharedMemoryName(string hostName, string path, bool global)
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

        internal const string UseSha1InPipeConnectionGetHashAlgorithmString = "Switch.System.ServiceModel.UseSha1InPipeConnectionGetHashAlgorithm";
        internal static bool s_useSha1InPipeConnectionGetHashAlgorithm = AppContext.TryGetSwitch(UseSha1InPipeConnectionGetHashAlgorithmString, out bool enabled) && enabled;

        private static HashAlgorithm GetHashAlgorithm()
        {
            if (s_useSha1InPipeConnectionGetHashAlgorithm)
            {
                return SHA1.Create(); // CodeQL [SM02196] Here SHA1 is not used for cryptographic purposes, it's for compatibility.
            }
            else
            {
                return SHA256.Create();
            }
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(nameof(uri), SR.PipeUriSchemeWrong);
        }
    }

    internal static class PipeError
    {
        public static string GetErrorString(IOException exception)
        {
            int originalErrorCode = GetErrorFromHResult(exception.HResult);
            return SR.Format(
                SR.PipeKnownWin32Error,
                exception.Message,
                originalErrorCode.ToString(CultureInfo.InvariantCulture),
                Convert.ToString(originalErrorCode, 16));
        }

        public static int GetErrorFromHResult(int hResult)
        {
            return hResult & 0x7FF8FFFF;
        }
    }
}
