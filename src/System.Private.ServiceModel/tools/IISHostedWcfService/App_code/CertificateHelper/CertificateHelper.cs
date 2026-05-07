// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace WcfTestCommon
{
    public static class CertificateHelper
    {
        public static class CurrentOperatingSystem
        {
            /// <summary>
            /// Returns true if current OS is Windows
            /// </summary>
            public static bool IsWindows()
            {
                return IsOSPlatform(OSPlatform.Windows);
            }

            /// <summary>
            /// Returns true if current OS is Linux
            /// </summary>
            public static bool IsLinux()
            {
                return IsOSPlatform(OSPlatform.Linux);
            }

            /// <summary>
            /// Returns true if current OS is OSX
            /// </summary>
            public static bool IsMacOS()
            {
                return IsOSPlatform(OSPlatform.OSX);
            }

            /// <summary>
            /// Returns true if current OS matches OSPlatform
            /// </summary>
            /// <param name="os">OS Platform to check for</param>
            public static bool IsOSPlatform(OSPlatform osPlatform)
            {
                return RuntimeInformation.IsOSPlatform(osPlatform);
            }
        }

        public static X509Store GetX509Store(StoreName storeName, StoreLocation storeLocation)
        {
            X509Store store;
            if (CurrentOperatingSystem.IsWindows())
            {
                store = new X509Store(storeName, storeLocation);
            }
            else
            {
                // On Linux and macOS, use CurrentUser scope as LocalMachine is not supported.
                store = new X509Store(storeName, StoreLocation.CurrentUser);
            }

            store.Open(OpenFlags.ReadOnly);
            return store;
        }
    }
}
