// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Text;
using static System.ServiceModel.Channels.UnsafeNativeMethods;

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// This class provides the entry points into Application Container related functionality. 
    /// Callers are expected to check if the application is running in an AppContainer before 
    /// invoking any of the methods in this class. 
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal class AppContainerInfo
    {
        private static object s_thisLock = new object();
        private static bool s_isRunningInAppContainer;
        private static volatile bool s_isRunningInAppContainerSet;
        private static object s_isRunningInAppContainerLock = new object();
        private static int? s_currentSessionId;

        internal AppContainerInfo(int sessionId, string namedObjectPath)
        {
            SessionId = sessionId;
            NamedObjectPath = namedObjectPath;
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

        private static bool RunningInAppContainer()
        {
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

        private static string GetAppContainerNamedObjectPath(string name)
        {
            // 1. Derive the PackageFamilyName(PFN) from the PackageFullName
            // 2. Get the AppContainerSID from the PFN
            // 3. Get the NamedObjectPath from the AppContainerSID
            IntPtr appContainerSid = IntPtr.Zero;

            // Package Full Name => Package family name
            uint packageFamilyNameLength = UnsafeNativeMethods.MAX_PATH;
            StringBuilder packageFamilyNameBuilder = new StringBuilder((int)UnsafeNativeMethods.MAX_PATH);
            string packageFamilyName;
            int errorCode = UnsafeNativeMethods.PackageFamilyNameFromFullName(name, ref packageFamilyNameLength, packageFamilyNameBuilder);
            if (errorCode != UnsafeNativeMethods.ERROR_SUCCESS)
            {
                throw FxTrace.Exception.AsError(new Win32Exception(errorCode, SR.Format(SR.PackageFullNameInvalid, name)));
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

        private static int GetCurrentSessionId()
        {
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
        private static SafeCloseHandle GetCurrentProcessToken()
        {
            SafeCloseHandle tokenHandle = null;
            if (!UnsafeNativeMethods.OpenProcessToken(
                            UnsafeNativeMethods.GetCurrentProcess(),
                            TokenAccessLevels.Query,
                            out tokenHandle))
            {
                int error = Marshal.GetLastWin32Error();
                throw FxTrace.Exception.AsError(new Win32Exception(error));
            }

            return tokenHandle;
        }
    }
}
