// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace System.ServiceModel.Channels
{
    [SupportedOSPlatform("windows")]
    internal static class UnsafeNativeMethods
    {
        public const string KERNEL32 = "kernel32.dll";
        public const string ADVAPI32 = "advapi32.dll";
        public const string USERENV = "userenv.dll";

        // WinError.h codes:
        internal const int ERROR_SUCCESS = 0;
        internal const int ERROR_FILE_NOT_FOUND = 0x2;
        internal const int ERROR_BROKEN_PIPE = 109;
        internal const int ERROR_PIPE_BUSY = 231;
        internal const int ERROR_NO_DATA = 232;
        // When querying for the token length
        internal const int ERROR_INSUFFICIENT_BUFFER = 122;
        public const uint MAX_PATH = 260;

        [DllImport(USERENV, SetLastError = true)]
        internal static extern int DeriveAppContainerSidFromAppContainerName
        (
            [In, MarshalAs(UnmanagedType.LPWStr)] string appContainerName,
            out IntPtr appContainerSid
        );

        [DllImport(ADVAPI32, SetLastError = true)]
        internal static extern IntPtr FreeSid
        (
            IntPtr pSid
        );

        // If the function succeeds, the return value is ERROR_SUCCESS and 'packageFamilyNameLength' contains the size of the data copied 
        // to 'packageFamilyName' (in WCHARs, including the null-terminator). If the function fails, the return value is a Win32 error code.
        [DllImport(KERNEL32)]
        internal static extern int PackageFamilyNameFromFullName
        (
            [In, MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
            ref uint packageFamilyNameLength,
            [In, Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder packageFamilyName
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

        [DllImport(KERNEL32, SetLastError = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport(ADVAPI32, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        internal static extern bool OpenProcessToken
        (
            IntPtr ProcessHandle,
            TokenAccessLevels DesiredAccess,
            out SafeCloseHandle TokenHandle
        );

        // Token marshalled as byte[]
        [DllImport(ADVAPI32, SetLastError = true)]
        private static unsafe extern bool GetTokenInformation
        (
            SafeCloseHandle tokenHandle,
            TOKEN_INFORMATION_CLASS tokenInformationClass,
            byte[] tokenInformation,
            uint tokenInformationLength,
            out uint returnLength
        );

        // Token marshalled as uint
        [DllImport(ADVAPI32, SetLastError = true)]
        private static extern bool GetTokenInformation
        (
            SafeCloseHandle tokenHandle,
            TOKEN_INFORMATION_CLASS tokenInformationClass,
            out uint tokenInformation,
            uint tokenInformationLength,
            out uint returnLength
        );

        internal static unsafe SecurityIdentifier GetAppContainerSid(SafeCloseHandle tokenHandle)
        {
            // Get length of buffer needed for sid.
            uint returnLength = GetTokenInformationLength(tokenHandle, TOKEN_INFORMATION_CLASS.TokenAppContainerSid);

            byte[] tokenInformation = new byte[returnLength];
            fixed (byte* pTokenInformation = tokenInformation)
            {
                if (!GetTokenInformation(
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

        [StructLayout(LayoutKind.Sequential)]
        private struct TokenAppContainerInfo
        {
            public IntPtr psid;
        }

        internal static int GetSessionId(SafeCloseHandle tokenHandle)
        {
            uint sessionId;
            uint returnLength;

            if (!GetTokenInformation(
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

        private static uint GetTokenInformationLength(SafeCloseHandle token, TOKEN_INFORMATION_CLASS tokenInformationClass)
        {
            uint lengthNeeded;
            if (!GetTokenInformation(
                    token,
                    tokenInformationClass,
                    null,
                    0,
                    out lengthNeeded))
            {
                int error = Marshal.GetLastWin32Error();
                if (error != ERROR_INSUFFICIENT_BUFFER)
                {
                    throw new Win32Exception(error);
                }
            }

            return lengthNeeded;
        }

        internal static bool RunningInAppContainer(SafeCloseHandle tokenHandle)
        {
            uint runningInAppContainer;
            uint returnLength;
            if (!GetTokenInformation(
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

        internal enum TOKEN_INFORMATION_CLASS : int
        {
            TokenUser = 1, // TOKEN_USER structure that contains the user account of the token. = 1,
            TokenGroups, // a TOKEN_GROUPS structure that contains the group accounts associated with the token.,
            TokenPrivileges, // a TOKEN_PRIVILEGES structure that contains the privileges of the token.,
            TokenOwner, // a TOKEN_OWNER structure that contains the default owner security identifier (SID) for newly created objects.,
            TokenPrimaryGroup, // a TOKEN_PRIMARY_GROUP structure that contains the default primary group SID for newly created objects.,
            TokenDefaultDacl, // a TOKEN_DEFAULT_DACL structure that contains the default DACL for newly created objects.,
            TokenSource, // a TOKEN_SOURCE structure that contains the source of the token. TOKEN_QUERY_SOURCE access is needed to retrieve this information.,
            TokenType, // a TOKEN_TYPE value that indicates whether the token is a primary or impersonation token.,
            TokenImpersonationLevel, // a SECURITY_IMPERSONATION_LEVEL value that indicates the impersonation level of the token. If the access token is not an impersonation token, the function fails.,
            TokenStatistics, // a TOKEN_STATISTICS structure that contains various token statistics.,
            TokenRestrictedSids, // a TOKEN_GROUPS structure that contains the list of restricting SIDs in a restricted token.,
            TokenSessionId, // a DWORD value that indicates the Terminal Services session identifier that is associated with the token. If the token is associated with the Terminal Server console session, the session identifier is zero. If the token is associated with the Terminal Server client session, the session identifier is nonzero. In a non-Terminal Services environment, the session identifier is zero. If TokenSessionId is set with SetTokenInformation, the application must have the Act As Part Of the Operating System privilege, and the application must be enabled to set the session ID in a token.
            TokenGroupsAndPrivileges, // a TOKEN_GROUPS_AND_PRIVILEGES structure that contains the user SID, the group accounts, the restricted SIDs, and the authentication ID associated with the token.,
            TokenSessionReference, // Reserved,
            TokenSandBoxInert, // a DWORD value that is nonzero if the token includes the SANDBOX_INERT flag.,
            TokenAuditPolicy,
            TokenOrigin, // a TOKEN_ORIGIN value. If the token  resulted from a logon that used explicit credentials, such as passing a name, domain, and password to the  LogonUser function, then the TOKEN_ORIGIN structure will contain the ID of the logon session that created it. If the token resulted from  network authentication, such as a call to AcceptSecurityContext  or a call to LogonUser with dwLogonType set to LOGON32_LOGON_NETWORK or LOGON32_LOGON_NETWORK_CLEARTEXT, then this value will be zero.
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            TokenIsAppContainer,
            TokenCapabilities,
            TokenAppContainerSid,
            TokenAppContainerNumber,
            TokenUserClaimAttributes,
            TokenDeviceClaimAttributes,
            TokenRestrictedUserClaimAttributes,
            TokenRestrictedDeviceClaimAttributes,
            TokenDeviceGroups,
            TokenRestrictedDeviceGroups,
            MaxTokenInfoClass  // MaxTokenInfoClass should always be the last enum
        }

        internal sealed class SafeCloseHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeCloseHandle() : base(true) { }

            internal SafeCloseHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle) => SetHandle(handle);

            protected override bool ReleaseHandle() => CloseHandle(handle);

            [DllImport(KERNEL32, ExactSpelling = true, SetLastError = true)]
            private static extern bool CloseHandle(IntPtr handle);
        }
    }
}
