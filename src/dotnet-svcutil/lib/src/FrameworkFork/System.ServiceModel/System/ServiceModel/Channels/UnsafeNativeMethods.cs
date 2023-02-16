// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#define WSARECV

using System.ComponentModel;
using System.Runtime;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace System.ServiceModel.Channels
{
    using SafeCloseHandle = System.ServiceModel.Activation.SafeCloseHandle;
    using TOKEN_INFORMATION_CLASS = System.ServiceModel.Activation.ListenerUnsafeNativeMethods.TOKEN_INFORMATION_CLASS;

    internal static class UnsafeNativeMethods
    {
        public const string KERNEL32 = "kernel32.dll";
        public const string ADVAPI32 = "advapi32.dll";
        public const string BCRYPT = "bcrypt.dll";
        public const string MQRT = "mqrt.dll";
        public const string SECUR32 = "secur32.dll";
        public const string USERENV = "userenv.dll";

#if WSARECV
        public const string WS2_32 = "ws2_32.dll";
#endif

        public const int ERROR_SUCCESS = 0;
        public const int ERROR_FILE_NOT_FOUND = 2;
        public const int ERROR_ACCESS_DENIED = 5;
        public const int ERROR_INVALID_HANDLE = 6;
        public const int ERROR_NOT_ENOUGH_MEMORY = 8;
        public const int ERROR_OUTOFMEMORY = 14;
        public const int ERROR_SHARING_VIOLATION = 32;
        public const int ERROR_NETNAME_DELETED = 64;
        public const int ERROR_INVALID_PARAMETER = 87;
        public const int ERROR_BROKEN_PIPE = 109;
        public const int ERROR_ALREADY_EXISTS = 183;
        public const int ERROR_PIPE_BUSY = 231;
        public const int ERROR_NO_DATA = 232;
        public const int ERROR_MORE_DATA = 234;
        public const int WAIT_TIMEOUT = 258;
        public const int ERROR_PIPE_CONNECTED = 535;
        public const int ERROR_OPERATION_ABORTED = 995;
        public const int ERROR_IO_PENDING = 997;
        public const int ERROR_SERVICE_ALREADY_RUNNING = 1056;
        public const int ERROR_SERVICE_DISABLED = 1058;
        public const int ERROR_NO_TRACKING_SERVICE = 1172;
        public const int ERROR_ALLOTTED_SPACE_EXCEEDED = 1344;
        public const int ERROR_NO_SYSTEM_RESOURCES = 1450;

        public const uint MAX_PATH = 260;

        // When querying for the token length
        private const int ERROR_INSUFFICIENT_BUFFER = 122;

        public const int STATUS_PENDING = 0x103;

        // socket errors
        public const int WSAACCESS = 10013;
        public const int WSAEMFILE = 10024;
        public const int WSAEMSGSIZE = 10040;
        public const int WSAEADDRINUSE = 10048;
        public const int WSAEADDRNOTAVAIL = 10049;
        public const int WSAENETDOWN = 10050;
        public const int WSAENETUNREACH = 10051;
        public const int WSAENETRESET = 10052;
        public const int WSAECONNABORTED = 10053;
        public const int WSAECONNRESET = 10054;
        public const int WSAENOBUFS = 10055;
        public const int WSAESHUTDOWN = 10058;
        public const int WSAETIMEDOUT = 10060;
        public const int WSAECONNREFUSED = 10061;
        public const int WSAEHOSTDOWN = 10064;
        public const int WSAEHOSTUNREACH = 10065;

        // VirtualAlloc constants
        public const uint MEM_COMMIT = 0x1000;
        public const uint MEM_DECOMMIT = 0x4000;
        public const int PAGE_READWRITE = 4;

        public const int FILE_MAP_WRITE = 2;
        public const int FILE_MAP_READ = 4;

        public const int GENERIC_ALL = 0x10000000;
        public const int GENERIC_READ = unchecked((int)0x80000000);
        public const int GENERIC_WRITE = 0x40000000;

        public const int OPEN_EXISTING = 3;

        public const int FILE_FLAG_OVERLAPPED = 0x40000000;
        public const int FILE_FLAG_FIRST_PIPE_INSTANCE = 0x00080000;
        public const int FILE_CREATE_PIPE_INSTANCE = 0x00000004;
        public const int FILE_WRITE_ATTRIBUTES = 0x00000100;
        public const int FILE_WRITE_DATA = 0x00000002;
        public const int FILE_WRITE_EA = 0x00000010;

        public const int SECURITY_ANONYMOUS = 0x00000000;
        public const int SECURITY_QOS_PRESENT = 0x00100000;
        public const int SECURITY_IDENTIFICATION = 0x00010000;

        public const int PIPE_READMODE_MESSAGE = 2;
        public const int PIPE_ACCESS_DUPLEX = 3;
        public const int PIPE_UNLIMITED_INSTANCES = 255;
        public const int PIPE_TYPE_MESSAGE = 4;

        public const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        public const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
        public const int FORMAT_MESSAGE_FROM_STRING = 0x00000400;
        public const int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
        public const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000;
        public const int FORMAT_MESSAGE_FROM_HMODULE = 0x00000800;

        //
        // Summary:
        //     Defines the privileges of the user account associated with the access token.
        [ComVisible(true)]
        [Flags]
        public enum TokenAccessLevels
        {
            //
            // Summary:
            //     The user can attach a primary token to a process.
            AssignPrimary = 1,
            //
            // Summary:
            //     The user can duplicate the token.
            Duplicate = 2,
            //
            // Summary:
            //     The user can impersonate a client.
            Impersonate = 4,
            //
            // Summary:
            //     The user can query the token.
            Query = 8,
            //
            // Summary:
            //     The user can query the source of the token.
            QuerySource = 16,
            //
            // Summary:
            //     The user can enable or disable privileges in the token.
            AdjustPrivileges = 32,
            //
            // Summary:
            //     The user can change the attributes of the groups in the token.
            AdjustGroups = 64,
            //
            // Summary:
            //     The user can change the default owner, primary group, or discretionary access
            //     control list (DACL) of the token.
            AdjustDefault = 128,
            //
            // Summary:
            //     The user can adjust the session identifier of the token.
            AdjustSessionId = 256,
            //
            // Summary:
            //     The user has standard read rights and the System.Security.Principal.TokenAccessLevels.Query
            //     privilege for the token.
            Read = 131080,
            //
            // Summary:
            //     The user has standard write rights and the System.Security.Principal.TokenAccessLevels.AdjustPrivileges,F:System.Security.Principal.TokenAccessLevels.AdjustGroups,
            //     and System.Security.Principal.TokenAccessLevels.AdjustDefault privileges for
            //     the token.
            Write = 131296,
            //
            // Summary:
            //     The user has all possible access to the token.
            AllAccess = 983551,
            //
            // Summary:
            //     The maximum value that can be assigned for the System.Security.Principal.TokenAccessLevels
            //     enumeration.
            MaximumAllowed = 33554432
        }

        [DllImport(KERNEL32), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [ResourceExposure(ResourceScope.None)]
        internal static extern int CloseHandle
        (
            IntPtr handle
        );

        // This p/invoke is for perf-sensitive codepaths which can guarantee a valid handle via custom locking.
        [DllImport(KERNEL32, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        unsafe internal static extern int ReadFile
        (
            IntPtr handle,
            byte* bytes,
            int numBytesToRead,
            IntPtr numBytesRead_mustBeZero,
            NativeOverlapped* overlapped
        );

        [DllImport(KERNEL32, ExactSpelling = true),
        ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [ResourceExposure(ResourceScope.None)]
        internal static extern int UnmapViewOfFile
        (
            IntPtr lpBaseAddress
        );

        // This p/invoke is for perf-sensitive codepaths which can guarantee a valid handle via custom locking.
        [DllImport(KERNEL32, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        internal static unsafe extern int WriteFile
        (
            IntPtr handle,
            byte* bytes,
            int numBytesToWrite,
            IntPtr numBytesWritten_mustBeZero,
            NativeOverlapped* lpOverlapped
        );

        // This p/invoke is for perf-sensitive codepaths which can guarantee a valid handle via custom locking.
        [DllImport(KERNEL32, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        unsafe internal static extern int GetOverlappedResult
        (
            IntPtr handle,
            NativeOverlapped* overlapped,
            out int bytesTransferred,
            int wait
        );

        [DllImport(KERNEL32, CharSet = CharSet.Unicode, SetLastError = true)]
        [ResourceExposure(ResourceScope.Machine)]
        internal static extern PipeHandle CreateFile
        (
            string lpFileName,
            int dwDesiredAccess,
            int dwShareMode,
            IntPtr lpSECURITY_ATTRIBUTES,
            int dwCreationDisposition,
            int dwFlagsAndAttributes,
            IntPtr hTemplateFile
        );

        [StructLayout(LayoutKind.Sequential)]
        internal class SECURITY_ATTRIBUTES
        {
            internal int _nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
            internal IntPtr _lpSecurityDescriptor = IntPtr.Zero;
            internal bool _bInheritHandle = false;
        }

        [DllImport(KERNEL32, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        internal unsafe static extern int ConnectNamedPipe
        (
            PipeHandle handle,
            NativeOverlapped* lpOverlapped
        );

        [DllImport(KERNEL32, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        internal unsafe static extern int DisconnectNamedPipe
        (
            PipeHandle handle
        );

        [DllImport(KERNEL32, CharSet = CharSet.Unicode, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        internal static extern SafeFileMappingHandle CreateFileMapping(
            IntPtr fileHandle,
            SECURITY_ATTRIBUTES securityAttributes,
            int protect,
            int sizeHigh,
            int sizeLow,
            string name
        );

        [DllImport(KERNEL32, SetLastError = true, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.Machine)]
        internal static extern SafeFileMappingHandle OpenFileMapping
        (
            int access,
            bool inheritHandle,
            string name
        );

        [DllImport(KERNEL32, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        internal static extern int FormatMessage
        (
            int dwFlags,
            IntPtr lpSource,
            int dwMessageId,
            int dwLanguageId,
            StringBuilder lpBuffer,
            int nSize,
            IntPtr arguments
        );

        [DllImport(KERNEL32, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        internal static extern SafeViewOfFileHandle MapViewOfFile
        (
            SafeFileMappingHandle handle,
            int dwDesiredAccess,
            int dwFileOffsetHigh,
            int dwFileOffsetLow,
            IntPtr dwNumberOfBytesToMap
        );

        [DllImport(KERNEL32, CharSet = CharSet.Unicode, SetLastError = true)]
        [ResourceExposure(ResourceScope.Machine)]
        internal unsafe static extern PipeHandle CreateNamedPipe
       (
           string name,
           int openMode,
           int pipeMode,
           int maxInstances,
           int outBufSize,
           int inBufSize,
           int timeout,
           SECURITY_ATTRIBUTES securityAttributes
       );

        [DllImport(KERNEL32, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        internal static extern int SetNamedPipeHandleState
       (
           PipeHandle handle,
           ref int mode,
           IntPtr collectionCount,
           IntPtr collectionDataTimeout
       );

        // [DllImport(ADVAPI32, SetLastError = true)]
        // [ResourceExposure(ResourceScope.None)]
        // internal static extern bool OpenProcessToken
        //(
        //    IntPtr ProcessHandle,
        //    TokenAccessLevels DesiredAccess,
        //    out SafeCloseHandle TokenHandle
        //);

        // Token marshalled as byte[]
        [DllImport(ADVAPI32, SetLastError = true)]
        static extern unsafe bool GetTokenInformation
        (
            SafeCloseHandle tokenHandle,
            TOKEN_INFORMATION_CLASS tokenInformationClass,
            byte[] tokenInformation,
            uint tokenInformationLength,
            out uint returnLength
        );

        // Token marshalled as uint
        [DllImport(ADVAPI32, SetLastError = true)]
        static extern bool GetTokenInformation
        (
            SafeCloseHandle tokenHandle,
            TOKEN_INFORMATION_CLASS tokenInformationClass,
            out uint tokenInformation,
            uint tokenInformationLength,
            out uint returnLength
        );

        internal static int GetSessionId(SafeCloseHandle tokenHandle)
        {
            uint sessionId;
            uint returnLength;

            if (!UnsafeNativeMethods.GetTokenInformation(
                                            tokenHandle,
                                            TOKEN_INFORMATION_CLASS.TokenSessionId,
                                            out sessionId,
                                            sizeof(uint),
                                            out returnLength))
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw FxTrace.Exception.AsError(new Win32Exception(errorCode));
            }

            return (int)sessionId;
        }

        // If the function succeeds, the return value is ERROR_SUCCESS and 'packageFamilyNameLength' contains the size of the data copied 
        // to 'packageFamilyName' (in WCHARs, including the null-terminator). If the function fails, the return value is a Win32 error code.
        [DllImport(KERNEL32)]
        internal static extern int PackageFamilyNameFromFullName
        (
            [In, MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
            ref uint packageFamilyNameLength,
            [In, Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder packageFamilyName
        );

        [DllImport(ADVAPI32, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        internal static extern bool OpenProcessToken
        (
            IntPtr ProcessHandle,
            TokenAccessLevels DesiredAccess,
            out SafeCloseHandle TokenHandle
        );

        [DllImport(KERNEL32, SetLastError = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport(ADVAPI32, SetLastError = true)]
        internal static extern IntPtr FreeSid
       (
           IntPtr pSid
       );

        [DllImport(USERENV, SetLastError = true)]
        internal static extern int DeriveAppContainerSidFromAppContainerName
       (
           [In, MarshalAs(UnmanagedType.LPWStr)] string appContainerName,
           out IntPtr appContainerSid
       );

        [DllImport(KERNEL32, SetLastError = true)]
        internal static extern bool GetAppContainerNamedObjectPath
        (
            IntPtr token,
            IntPtr appContainerSid,
            uint objectPathLength,
            [In, Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder objectPath,
            ref uint returnLength
        );

        internal static bool RunningInAppContainer(SafeCloseHandle tokenHandle)
        {
            uint runningInAppContainer;
            uint returnLength;
            if (!UnsafeNativeMethods.GetTokenInformation(
                                        tokenHandle,
                                        TOKEN_INFORMATION_CLASS.TokenIsAppContainer,
                                        out runningInAppContainer,
                                        sizeof(uint),
                                        out returnLength))
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw FxTrace.Exception.AsError(new Win32Exception(errorCode));
            }

            return runningInAppContainer == 1;
        }

        internal static unsafe SecurityIdentifier GetAppContainerSid(SafeCloseHandle tokenHandle)
        {
            // Get length of buffer needed for sid.
            uint returnLength = UnsafeNativeMethods.GetTokenInformationLength(
                                                        tokenHandle,
                                                        TOKEN_INFORMATION_CLASS.TokenAppContainerSid);

            byte[] tokenInformation = new byte[returnLength];
            fixed (byte* pTokenInformation = tokenInformation)
            {
                if (!UnsafeNativeMethods.GetTokenInformation(
                                                tokenHandle,
                                                TOKEN_INFORMATION_CLASS.TokenAppContainerSid,
                                                tokenInformation,
                                                returnLength,
                                                out returnLength))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw FxTrace.Exception.AsError(new Win32Exception(errorCode));
                }

                TokenAppContainerInfo* ptg = (TokenAppContainerInfo*)pTokenInformation;
                return new SecurityIdentifier(ptg->psid);
            }
        }

        // NOTE: a macro in win32
        //[PermissionSet(SecurityAction.Demand, Unrestricted = true), SecuritySafeCritical]
        internal unsafe static bool HasOverlappedIoCompleted(
            NativeOverlapped* overlapped)
        {
            return overlapped->InternalLow != (IntPtr)STATUS_PENDING;
        }

        [DllImport(KERNEL32, ExactSpelling = true)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool SetWaitableTimer(SafeWaitHandle handle, ref long dueTime, int period, IntPtr mustBeZero, IntPtr mustBeZeroAlso, bool resume);

        [DllImport(KERNEL32, BestFitMapping = false, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.Machine)]
        public static extern SafeWaitHandle CreateWaitableTimer(IntPtr mustBeZero, bool manualReset, string timerName);

        static uint GetTokenInformationLength(SafeCloseHandle token, TOKEN_INFORMATION_CLASS tokenInformationClass)
        {
            uint lengthNeeded;
            bool success;
            if (!(success = GetTokenInformation(
                                       token,
                                       tokenInformationClass,
                                       null,
                                       0,
                                       out lengthNeeded)))
            {
                int error = Marshal.GetLastWin32Error();
                if (error != UnsafeNativeMethods.ERROR_INSUFFICIENT_BUFFER)
                {
                    throw FxTrace.Exception.AsError(new Win32Exception(error));
                }
            }

            Fx.Assert(!success, "Retreving the length should always fail.");

            return lengthNeeded;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct TokenAppContainerInfo
    {
        public IntPtr psid;
    }

    [SuppressUnmanagedCodeSecurity]
    class PipeHandle : SafeHandleMinusOneIsInvalid
    {
        internal PipeHandle() : base(true) { }

        // This is unsafe, but is useful for a duplicated handle, which is inherently unsafe already.
        internal PipeHandle(IntPtr handle)
            : base(true)
        {
            SetHandle(handle);
        }

        //        internal int GetClientPid()
        //        {
        //            int pid;
        //#pragma warning suppress 56523 // elliotw, Win32Exception ctor calls Marshal.GetLastWin32Error()
        //            bool success = UnsafeNativeMethods.GetNamedPipeClientProcessId(this, out pid);
        //            if (!success)
        //            {
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new Win32Exception());
        //            }
        //            return pid;
        //        }

        protected override bool ReleaseHandle()
        {
            return UnsafeNativeMethods.CloseHandle(handle) != 0;
        }
    }

    [SuppressUnmanagedCodeSecurityAttribute()]
    sealed class SafeFileMappingHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeFileMappingHandle()
            : base(true)
        {
        }

        override protected bool ReleaseHandle()
        {
            return UnsafeNativeMethods.CloseHandle(handle) != 0;
        }
    }

    [SuppressUnmanagedCodeSecurityAttribute]
    sealed class SafeViewOfFileHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeViewOfFileHandle()
            : base(true)
        {
        }

        override protected bool ReleaseHandle()
        {
            if (UnsafeNativeMethods.UnmapViewOfFile(handle) != 0)
            {
                handle = IntPtr.Zero;
                return true;
            }
            return false;
        }
    }
}
