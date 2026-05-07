// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
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

        /// <summary>
        /// On macOS, the Root and TrustedPeople certificate stores cannot be opened
        /// with ReadWrite via the .NET X509Store API. Use the macOS 'security' CLI
        /// to add trust for a certificate instead.
        /// </summary>
        public static bool AddTrustedCertOnMacOS(X509Certificate2 certificate)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".cer");
            try
            {
                File.WriteAllBytes(tempFile, certificate.Export(X509ContentType.Cert));
                var psi = new ProcessStartInfo
                {
                    FileName = "security",
                    Arguments = string.Format("add-trusted-cert -r trustRoot -p ssl \"{0}\"", tempFile),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                using (var process = Process.Start(psi))
                {
                    process.WaitForExit(30000);
                    if (process.ExitCode != 0)
                    {
                        string error = process.StandardError.ReadToEnd();
                        Trace.WriteLine(string.Format("[CertificateHelper] security add-trusted-cert failed: {0}", error));
                        return false;
                    }
                }
                return true;
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        /// <summary>
        /// On macOS, remove trust for a certificate using the 'security' CLI.
        /// </summary>
        public static bool RemoveTrustedCertOnMacOS(X509Certificate2 certificate)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".cer");
            try
            {
                File.WriteAllBytes(tempFile, certificate.Export(X509ContentType.Cert));
                var psi = new ProcessStartInfo
                {
                    FileName = "security",
                    Arguments = string.Format("remove-trusted-cert \"{0}\"", tempFile),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                using (var process = Process.Start(psi))
                {
                    process.WaitForExit(30000);
                    return process.ExitCode == 0;
                }
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }
}
