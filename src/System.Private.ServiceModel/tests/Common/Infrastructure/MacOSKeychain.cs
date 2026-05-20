// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Infrastructure.Common
{
    // macOS keychain helper used by the test infrastructure. Wraps the `security` CLI to
    //
    //   1) create + unlock a dedicated custom keychain (no GUI prompts, no sudo),
    //   2) make it the default keychain so .NET's X509Store(My, CurrentUser) finds certs,
    //   3) import client/server certs (PFX) with `-A` so the private key is usable from any app,
    //   4) add trust for root certs with `-r trustRoot -p ssl -k <custom>` so SSL chains
    //      validate as fully-trusted instead of the "partial trust" state that breaks TLS
    //      handshakes on macOS (issue #2870).
    //
    // Mirrors the server-side helper in
    // src/System.Private.ServiceModel/tools/IISHostedWcfService/App_code/CertificateHelper/CertificateHelper.cs
    internal static class MacOSKeychain
    {
        private const string KeychainPassword = "WCFKeychainFilePassword";
        private const string PfxImportPassword = "test";

        private static readonly string s_keychainPath = Path.Combine(Environment.CurrentDirectory, "wcfTest.keychain-db");
        private static readonly object s_initLock = new object();
        private static bool s_initialized;

        public static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static string KeychainPath => s_keychainPath;

        public static void EnsureInitialized()
        {
            if (s_initialized)
            {
                return;
            }

            lock (s_initLock)
            {
                if (s_initialized)
                {
                    return;
                }

                if (File.Exists(s_keychainPath))
                {
                    RunSecurity($"delete-keychain \"{s_keychainPath}\"", ignoreFailure: true);
                }

                RunSecurity($"create-keychain -p {KeychainPassword} \"{s_keychainPath}\"");
                RunSecurity($"unlock-keychain -p {KeychainPassword} \"{s_keychainPath}\"");
                // No -t / -u flags = disable auto-lock & timeout so the keychain stays open
                // for the duration of the test run.
                RunSecurity($"set-keychain-settings \"{s_keychainPath}\"");

                // Add to the user keychain search list and make it default so .NET's
                // X509Store(My, CurrentUser) locates certs imported here.
                string existing = RunSecurity("list-keychains -d user").Replace("\"", "").Trim();
                RunSecurity($"list-keychains -d user -s \"{s_keychainPath}\" {existing}");
                RunSecurity($"default-keychain -s \"{s_keychainPath}\"");

                s_initialized = true;
                Trace.WriteLine($"[MacOSKeychain] initialized at: {s_keychainPath}");
            }
        }

        // Imports a PFX (with private key) into the custom keychain. `-A` allows any
        // application to access the private key without prompting.
        public static void ImportPfx(X509Certificate2 certificate)
        {
            EnsureInitialized();

            byte[] pfx = certificate.Export(X509ContentType.Pfx, PfxImportPassword);
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".pfx");
            try
            {
                File.WriteAllBytes(tempFile, pfx);
                RunSecurity($"import \"{tempFile}\" -k \"{s_keychainPath}\" -P \"{PfxImportPassword}\" -A -T /usr/bin/security");
                Trace.WriteLine($"[MacOSKeychain] imported PFX: {certificate.Subject} ({certificate.Thumbprint})");
            }
            finally
            {
                TryDelete(tempFile);
            }
        }

        // Imports a public-key-only certificate.
        public static void ImportPublic(X509Certificate2 certificate)
        {
            EnsureInitialized();

            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".cer");
            try
            {
                File.WriteAllBytes(tempFile, certificate.Export(X509ContentType.Cert));
                RunSecurity($"import \"{tempFile}\" -k \"{s_keychainPath}\" -A -T /usr/bin/security");
                Trace.WriteLine($"[MacOSKeychain] imported public cert: {certificate.Subject} ({certificate.Thumbprint})");
            }
            finally
            {
                TryDelete(tempFile);
            }
        }

        // Marks a root certificate as fully trusted for SSL within the custom keychain.
        // This is what fixes the "partial trust" failure on macOS for the WCF test root CA.
        public static void AddTrustedRoot(X509Certificate2 certificate)
        {
            EnsureInitialized();

            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".cer");
            try
            {
                File.WriteAllBytes(tempFile, certificate.Export(X509ContentType.Cert));
                RunSecurity($"add-trusted-cert -r trustRoot -p ssl -k \"{s_keychainPath}\" \"{tempFile}\"");
                Trace.WriteLine($"[MacOSKeychain] trusted root added: {certificate.Subject} ({certificate.Thumbprint})");
            }
            finally
            {
                TryDelete(tempFile);
            }
        }

        public static void Delete()
        {
            if (!IsMacOS || !File.Exists(s_keychainPath))
            {
                return;
            }

            RunSecurity($"delete-keychain \"{s_keychainPath}\"", ignoreFailure: true);
            s_initialized = false;
        }

        private static string RunSecurity(string arguments, bool ignoreFailure = false)
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
                    string msg = $"[MacOSKeychain] 'security {arguments}' exited {process.ExitCode}: {stderr}";
                    Trace.WriteLine(msg);
                    if (!ignoreFailure)
                    {
                        throw new InvalidOperationException(msg);
                    }
                }

                return stdout;
            }
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch
            {
                // best-effort cleanup
            }
        }
    }
}
