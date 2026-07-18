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
            else if (CurrentOperatingSystem.IsMacOS())
            {
                // macOS doesn't have proper per-store separation. .NET's X509Store(TrustedPeople|Root, CurrentUser)
                // on macOS does not enumerate certs imported into the user's default keychain via the
                // 'security' CLI. Route all store names through StoreName.My so adds and lookups land in
                // the same place — the user's default keychain (which is our custom WCF test keychain).
                store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            }
            else
            {
                // On Linux, use CurrentUser scope as LocalMachine is not supported.
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

            // Set as default keychain so .NET's X509Store(My, CurrentUser) searches it
            RunSecurityCommand(string.Format("default-keychain -s \"{0}\"", s_macOSKeychainPath));

            s_macOSKeychainInitialized = true;
            Trace.WriteLine(string.Format("[CertificateHelper] macOS keychain initialized at: {0}", s_macOSKeychainPath));
        }

        /// <summary>
        /// Imports a PFX (PKCS12) file into the macOS custom keychain
        /// using the 'security import' CLI, avoiding user interaction prompts.
        /// </summary>
        public static bool ImportCertToMacOSKeychain(byte[] pfxBytes, string pfxPassword)
        {
            EnsureMacOSKeychainInitialized();

            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".pfx");
            try
            {
                File.WriteAllBytes(tempFile, pfxBytes);

                // -A allows any application to access the imported key without prompting
                string output = RunSecurityCommand(string.Format(
                    "import \"{0}\" -k \"{1}\" -P \"{2}\" -A -T /usr/bin/security",
                    tempFile, s_macOSKeychainPath, pfxPassword));

                Trace.WriteLine("[CertificateHelper] Imported PFX to macOS keychain.");

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
            return AddTrustedCertOnMacOS(certificate.Export(X509ContentType.Cert));
        }

        /// <summary>
        /// Adds trust for a certificate (from raw DER bytes) on macOS using the 'security' CLI.
        /// Writes trust settings into the admin trust domain (System.keychain) via sudo so that
        /// macOS's TLS chain evaluator honors the root system-wide. Helix macOS runners are
        /// configured with passwordless sudo, so this is non-interactive.
        /// </summary>
        public static bool AddTrustedCertOnMacOS(byte[] certDerBytes)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".cer");
            try
            {
                File.WriteAllBytes(tempFile, certDerBytes);

                // sudo -n: non-interactive; fail rather than prompt.
                // -d: admin trust domain (system-wide), requires root.
                // -r trustRoot: this cert is a trust root.
                // NO -p flag: empty trust settings array means "trusted for all uses" - broader
                // than "-p ssl" which constrains trust to the sslServer policy only. Without -p
                // SecTrust treats the root as a universal trust anchor.
                // -k System.keychain: store the cert in the system keychain.
                var psi = new ProcessStartInfo
                {
                    FileName = "sudo",
                    Arguments = string.Format(
                        "-n security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain \"{0}\"",
                        tempFile),
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
                        Console.Error.WriteLine(string.Format(
                            "[CertificateHelper] sudo security add-trusted-cert failed (exit {0}): {1}",
                            process.ExitCode, stderr));
                        return false;
                    }

                    Console.WriteLine("[CertificateHelper] Added root cert to macOS System.keychain admin trust domain.");

                    return true;
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
                    Console.Error.WriteLine(string.Format("[CertificateHelper] security {0} failed (exit code {1}): {2}",
                        arguments, process.ExitCode, stderr));
                }

                return stdout;
            }
        }
    }
}
