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
        private static readonly string s_macOSKeychainPath = Path.Combine(Environment.CurrentDirectory, "wcf-test.keychain-db");
        private const string MacOSKeychainPassword = "WcfTestPassword";
        private static bool s_macOSKeychainInitialized;

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
            /// <param name="osPlatform">OS Platform to check for</param>
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
        /// Ensures a custom unlocked keychain exists on macOS for non-interactive cert operations.
        /// The login keychain requires user interaction; this custom keychain does not.
        /// </summary>
        public static void EnsureMacOSKeychainInitialized()
        {
            if (s_macOSKeychainInitialized)
            {
                return;
            }

            if (File.Exists(s_macOSKeychainPath))
            {
                RunSecurityCommand(string.Format("delete-keychain \"{0}\"", s_macOSKeychainPath));
            }

            // Create a new unlocked keychain dedicated to WCF test certificates
            RunSecurityCommand(string.Format("create-keychain -p {0} \"{1}\"", MacOSKeychainPassword, s_macOSKeychainPath));
            RunSecurityCommand(string.Format("unlock-keychain -p {0} \"{1}\"", MacOSKeychainPassword, s_macOSKeychainPath));
            // Disable auto-lock so the keychain stays unlocked for the duration of the test run
            RunSecurityCommand(string.Format("set-keychain-settings \"{0}\"", s_macOSKeychainPath));

            // Add the custom keychain to the search list so certs are discoverable
            string existingKeychains = RunSecurityCommand("list-keychains -d user").Trim().Replace("\"", "");
            RunSecurityCommand(string.Format("list-keychains -d user -s \"{0}\" {1}", s_macOSKeychainPath, existingKeychains));

            s_macOSKeychainInitialized = true;
            Trace.WriteLine(string.Format("[CertificateHelper] macOS keychain initialized at: {0}", s_macOSKeychainPath));
        }

        /// <summary>
        /// Imports a certificate (with private key) into the macOS custom keychain
        /// using the 'security import' CLI, avoiding user interaction prompts.
        /// </summary>
        public static bool ImportCertToMacOSKeychain(X509Certificate2 certificate, string pfxPassword)
        {
            EnsureMacOSKeychainInitialized();

            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".pfx");
            try
            {
                byte[] pfxBytes = certificate.Export(X509ContentType.Pfx, pfxPassword);
                File.WriteAllBytes(tempFile, pfxBytes);

                // -A allows any application to access the imported key without prompting
                string output = RunSecurityCommand(string.Format(
                    "import \"{0}\" -k \"{1}\" -P \"{2}\" -A -T /usr/bin/security",
                    tempFile, s_macOSKeychainPath, pfxPassword));

                Trace.WriteLine(string.Format("[CertificateHelper] Imported certificate to macOS keychain:"));
                Trace.WriteLine(string.Format("    {0} = {1}", "CN", certificate.SubjectName.Name));
                Trace.WriteLine(string.Format("    {0} = {1}", "Thumbprint", certificate.Thumbprint));
                Trace.WriteLine(string.Format("    {0} = {1}", "HasPrivateKey", certificate.HasPrivateKey));
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
        /// Imports a public certificate (no private key) into the macOS custom keychain.
        /// </summary>
        public static bool ImportPublicCertToMacOSKeychain(X509Certificate2 certificate)
        {
            EnsureMacOSKeychainInitialized();

            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".cer");
            try
            {
                File.WriteAllBytes(tempFile, certificate.Export(X509ContentType.Cert));
                RunSecurityCommand(string.Format(
                    "import \"{0}\" -k \"{1}\" -A -T /usr/bin/security",
                    tempFile, s_macOSKeychainPath));

                Trace.WriteLine(string.Format("[CertificateHelper] Imported public certificate to macOS keychain:"));
                Trace.WriteLine(string.Format("    {0} = {1}", "CN", certificate.SubjectName.Name));
                Trace.WriteLine(string.Format("    {0} = {1}", "Thumbprint", certificate.Thumbprint));
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
        /// On macOS, add a certificate as a trusted root using the 'security' CLI.
        /// </summary>
        public static bool AddTrustedCertOnMacOS(X509Certificate2 certificate)
        {
            EnsureMacOSKeychainInitialized();

            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".cer");
            try
            {
                File.WriteAllBytes(tempFile, certificate.Export(X509ContentType.Cert));
                RunSecurityCommand(string.Format(
                    "add-trusted-cert -r trustRoot -p ssl -k \"{0}\" \"{1}\"",
                    s_macOSKeychainPath, tempFile));
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
                RunSecurityCommand(string.Format("remove-trusted-cert \"{0}\"", tempFile));
                return true;
            }
            catch
            {
                return false;
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
        /// Deletes the custom macOS keychain used for WCF test certificates.
        /// </summary>
        public static void DeleteMacOSKeychain()
        {
            if (File.Exists(s_macOSKeychainPath))
            {
                RunSecurityCommand(string.Format("delete-keychain \"{0}\"", s_macOSKeychainPath));
                s_macOSKeychainInitialized = false;
                Trace.WriteLine("[CertificateHelper] macOS keychain deleted.");
            }
        }

        /// <summary>
        /// Removes certificates matching the given issuer from the macOS custom keychain.
        /// </summary>
        public static void RemoveCertsFromMacOSKeychain(string issuerName)
        {
            if (!File.Exists(s_macOSKeychainPath))
            {
                return;
            }

            // Delete the entire keychain — it will be recreated during the next setup
            DeleteMacOSKeychain();
        }

        private static string RunSecurityCommand(string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "security",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using (var process = Process.Start(psi))
            {
                string stdout = process.StandardOutput.ReadToEnd();
                string stderr = process.StandardError.ReadToEnd();
                process.WaitForExit(30000);

                if (process.ExitCode != 0)
                {
                    Trace.WriteLine(string.Format("[CertificateHelper] security {0} failed (exit code {1}): {2}",
                        arguments, process.ExitCode, stderr));
                }

                return stdout;
            }
        }
    }
}
