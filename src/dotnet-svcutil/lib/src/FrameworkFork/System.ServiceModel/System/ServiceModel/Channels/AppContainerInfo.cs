// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;

using SafeCloseHandle = System.ServiceModel.Activation.SafeCloseHandle;

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// This class provides the entry points into Application Container related functionality. 
    /// Callers are expected to check if the application is running in an AppContainer before 
    /// invoking any of the methods in this class. 
    /// </summary>
    class AppContainerInfo
    {
        static object s_thisLock = new object();
        static bool s_isAppContainerSupported;
        static bool s_isRunningInAppContainer;
        static volatile bool s_isRunningInAppContainerSet;
        static object s_isRunningInAppContainerLock = new object();
        static int? s_currentSessionId;
        static volatile SecurityIdentifier s_currentAppContainerSid;

        //static AppContainerInfo()
        //{
        //    // AppContainers are supported starting in Win8
        //    s_isAppContainerSupported = OSEnvironmentHelper.IsAtLeast(OSVersion.Win8);

        //    if (!s_isAppContainerSupported)
        //    {
        //        s_isRunningInAppContainerSet = true;
        //    }
        //}

        AppContainerInfo(int sessionId, string namedObjectPath)
        {
            this.SessionId = sessionId;
            this.NamedObjectPath = namedObjectPath;
        }

        internal static bool IsAppContainerSupported
        {
            get
            {
                return s_isAppContainerSupported;
            }
        }

        internal static bool IsRunningInAppContainer
        {
            get
            {                
                // The AppContainerInfo.RunningInAppContainer() API may throw security exceptions,
                // so cannot be used inside the static constructor of the class.
                if (!s_isRunningInAppContainerSet)
                {
                    lock (s_isRunningInAppContainerLock)
                    {
                        if (!s_isRunningInAppContainerSet)
                        {
                            s_isRunningInAppContainer = AppContainerInfo.RunningInAppContainer();
                            s_isRunningInAppContainerSet = true;
                        }
                    }
                }

                return s_isRunningInAppContainer;
            }
        }

        internal int SessionId { get; private set; }

        internal string NamedObjectPath { get; private set; }

        internal static AppContainerInfo CreateAppContainerInfo(string fullName, int sessionId)
        {
            Fx.Assert(IsAppContainerSupported, "AppContainers are not supported.");
            Fx.Assert(!string.IsNullOrEmpty(fullName), "fullName should be provided to initialize an AppContainerInfo.");

            int appSession = sessionId;
            if (appSession == ApplicationContainerSettings.CurrentSession)
            {
                lock (s_thisLock)
                {
                    if (s_currentSessionId == null)
                    {
                        s_currentSessionId = AppContainerInfo.GetCurrentSessionId();
                    }
                }

                appSession = s_currentSessionId.Value;
            }

            string namedObjectPath = AppContainerInfo.GetAppContainerNamedObjectPath(fullName);
            return new AppContainerInfo(appSession, namedObjectPath);
        }

        [SecuritySafeCritical]
        [Fx.Tag.SecurityNote(Critical = "This calls into the SecurityCritical method GetCurrentProcessToken and unsafe GetAppContainerSid.",
            Safe = "All critical token access and dispose is ensured. " +
            " The Sid is a non protected resource and can be obtained only if we have the appropriate process token permissions.")]
        internal static SecurityIdentifier GetCurrentAppContainerSid()
        {
            Fx.Assert(AppContainerInfo.IsAppContainerSupported, "AppContainers are not supported.");
            if (s_currentAppContainerSid == null)
            {
                lock (s_thisLock)
                {
                    if (s_currentAppContainerSid == null)
                    {
                        SafeCloseHandle tokenHandle = null;
                        try
                        {
                            tokenHandle = AppContainerInfo.GetCurrentProcessToken();
                            s_currentAppContainerSid = UnsafeNativeMethods.GetAppContainerSid(tokenHandle);
                        }
                        finally
                        {
                            if (tokenHandle != null)
                            {
                                tokenHandle.Dispose();
                            }
                        }
                    }
                }
            }

            return s_currentAppContainerSid;
        }

        [SecuritySafeCritical]
        [Fx.Tag.SecurityNote(Safe = "Process token handle access and dispose is ensured here and we only return a non-critical flag.")]
        static bool RunningInAppContainer()
        {
            //Fx.Assert(AppContainerInfo.IsAppContainerSupported, "AppContainers are not supported.");
            SafeCloseHandle tokenHandle = null;
            try
            {
                tokenHandle = AppContainerInfo.GetCurrentProcessToken();
                return UnsafeNativeMethods.RunningInAppContainer(tokenHandle);
            }
            finally
            {
                if (tokenHandle != null)
                {
                    tokenHandle.Dispose();
                }
            }
        }

        [SecuritySafeCritical]
        [Fx.Tag.SecurityNote(Critical = "Calls into unsafe native AppContainer package resolution methods.",
            Safe = "Wraps all access and disposal of securityDescriptors and returns a NamedObjectPath, " +
            "which is a non protected resource.")]
        static string GetAppContainerNamedObjectPath(string name)
        {
            // 1. Derive the PackageFamilyName(PFN) from the PackageFullName
            // 2. Get the AppContainerSID from the PFN
            // 3. Get the NamedObjectPath from the AppContainerSID
            Fx.Assert(AppContainerInfo.IsAppContainerSupported, "AppContainers are not supported.");
            IntPtr appContainerSid = IntPtr.Zero;

            // Package Full Name => Package family name
            uint packageFamilyNameLength = UnsafeNativeMethods.MAX_PATH;
            StringBuilder packageFamilyNameBuilder = new StringBuilder((int)UnsafeNativeMethods.MAX_PATH);
            string packageFamilyName;
            int errorCode = UnsafeNativeMethods.PackageFamilyNameFromFullName(name, ref packageFamilyNameLength, packageFamilyNameBuilder);
            if (errorCode != UnsafeNativeMethods.ERROR_SUCCESS)
            {
                throw FxTrace.Exception.AsError(new Win32Exception(errorCode, $"SR.PackageFullNameInvalid, {name}"));
            }

            packageFamilyName = packageFamilyNameBuilder.ToString();

            try
            {
                // PackageFamilyName => AppContainerSID
                int hresult = UnsafeNativeMethods.DeriveAppContainerSidFromAppContainerName(
                                                                    packageFamilyName,
                                                                    out appContainerSid);
                if (hresult != 0)
                {
                    errorCode = Marshal.GetLastWin32Error();
                    throw FxTrace.Exception.AsError(new Win32Exception(errorCode));
                }

                // AppContainerSID => NamedObjectPath
                StringBuilder namedObjectPath = new StringBuilder((int)UnsafeNativeMethods.MAX_PATH);
                uint returnLength = 0;
                if (!UnsafeNativeMethods.GetAppContainerNamedObjectPath(
                                                                IntPtr.Zero,
                                                                appContainerSid,
                                                                UnsafeNativeMethods.MAX_PATH,
                                                                namedObjectPath,
                                                                ref returnLength))
                {
                    errorCode = Marshal.GetLastWin32Error();
                    throw FxTrace.Exception.AsError(new Win32Exception(errorCode));
                }

                return namedObjectPath.ToString();
            }
            finally
            {
                if (appContainerSid != IntPtr.Zero)
                {
                    UnsafeNativeMethods.FreeSid(appContainerSid);
                }
            }
        }

        [SecuritySafeCritical]
        [Fx.Tag.SecurityNote(Critical = "Accesses the native current process token.",
            Safe = "The session id returned is a non-critical resource and " +
            " we ensure that the current process token handle created is disposed here.")]
        static int GetCurrentSessionId()
        {
            Fx.Assert(AppContainerInfo.IsAppContainerSupported, "AppContainers are not supported.");
            SafeCloseHandle tokenHandle = null;
            try
            {
                tokenHandle = AppContainerInfo.GetCurrentProcessToken();
                return UnsafeNativeMethods.GetSessionId(tokenHandle);
            }
            finally
            {
                if (tokenHandle != null)
                {
                    tokenHandle.Dispose();
                }
            }
        }

        /// <summary>
        /// Returns the process token using TokenAccessLevels.Query
        /// </summary>
        /// <returns>ProcessToken as a SafeCloseHandle.</returns>
        [SecurityCritical]
        [Fx.Tag.SecurityNote(Critical = "Returns a process token. The caller is responsible for disposing the SafeCloseHandle.")]
        static SafeCloseHandle GetCurrentProcessToken()
        {
            SafeCloseHandle tokenHandle = null;
            if (!UnsafeNativeMethods.OpenProcessToken(
                            UnsafeNativeMethods.GetCurrentProcess(),
                            /*TokenAccessLevels.Query*/(UnsafeNativeMethods.TokenAccessLevels)8,
                            out tokenHandle))
            {
                int error = Marshal.GetLastWin32Error();
                throw FxTrace.Exception.AsError(new Win32Exception(error));
            }

            return tokenHandle;
        }
    }
}
