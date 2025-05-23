// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Infrastructure.Common;

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
            X509Store store = null;
            if (CurrentOperatingSystem.IsWindows())
            {
                store = new X509Store(storeName, storeLocation);
            }
            else if (CurrentOperatingSystem.IsLinux())
            {
                // Store the certificates in CurrentUser Scope
                store = new X509Store(storeName, StoreLocation.CurrentUser);
            }
            else if (CurrentOperatingSystem.IsMacOS())
            {
                // MacOS SafeKeychainHandle
                store = GetMacOSX509Store();
            }

            store = EnsureStoreIsOpened(store);
            return store;
        }

        private static X509Store EnsureStoreIsOpened(X509Store store)
        {
            try
            {
                // Try opening the store in read-only mode
                store.Open(OpenFlags.ReadOnly);
            }
            catch { }

            return store;
        }

        internal static string OSXCustomKeychainFilePath
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, "wcfLocal.keychain");
            }
        }

        internal static string OSXCustomKeychainPassword
        {
            get
            {
                return "WCFKeychainFilePassword";
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static X509Store GetMacOSX509Store(string storeFilePath = null)
        {
            if (storeFilePath == null)
            {
                storeFilePath = OSXCustomKeychainFilePath;
            }

            SafeKeychainHandle keychain;
            if (!File.Exists(storeFilePath))
            {
                keychain = SafeKeychainHandle.Create(storeFilePath, OSXCustomKeychainPassword);
            }
            else
            {
                keychain = SafeKeychainHandle.Open(storeFilePath, OSXCustomKeychainPassword);
            }

            if (keychain.IsInvalid)
                throw new Exception("Unable to open MacOS Keychain");

            X509Store store = new X509Store(keychain.DangerousGetHandle());
            return store;
        }
    }
}
