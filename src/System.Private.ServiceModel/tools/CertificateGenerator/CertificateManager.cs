// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace WcfTestCommon
{
    // This class manages the adding and removal of certificates.
    // It also handles informing http.sys of which certificate to associate with SSL ports.
    public static class CertificateManager
    {
        private static object s_certificateLock = new object();

        // Dictionary of certificates installed by CertificateManager

        // Keyed by the Subject of the certificate - there should be only one valid cert per endpoint
        // Valid certs are shareable across endpoints 
        private static Dictionary<string, CertificateCacheEntry> s_myCertificates = new Dictionary<string, CertificateCacheEntry>(StringComparer.OrdinalIgnoreCase);

        // Keyed by endpoint address - each endpoint can only configure one invalid service certificate,
        // Each endpoint address can have a unique cert
        private static Dictionary<string, CertificateCacheEntry> s_myInvalidCertificates = new Dictionary<string, CertificateCacheEntry>(StringComparer.OrdinalIgnoreCase);

        // Keyed by CA subject
        private static Dictionary<string, CertificateCacheEntry> s_rootCertificates = new Dictionary<string, CertificateCacheEntry>(StringComparer.OrdinalIgnoreCase);

        // Keyed by port, value is cert thumbprint
        private static Dictionary<int, string> s_sslPorts = new Dictionary<int, string>();

        // When we install certificates via CreateAndInstallMachineCertificates, put local cert here 
        // for reference when we need to add the sslport references
        private static X509Certificate2 s_localCertificate = null;

        public static EventHandler BridgeConfigurationChanged;

        // Any change to the Bridge resource folder uninstalls all the certificates
        // installed by those resources.
        public static void OnResourceFolderChanged(string oldFolder, string newFolder)
        {
            BridgeConfigurationChanged(null, new EventArgs());

            // Unintall only the certificates this process added
            UninstallAllCertificates(force: false);
        }

        // Uninstalls all certificates and SSL port associations
        // added by this process.  If 'force' is true, removes all,
        // whether this process created them or not.
        public static void UninstallAllCertificates(bool force)
        {
            lock (s_certificateLock)
            {
                UninstallAllSslPortCertificates();
                UninstallAllMyCertificates(force);
                UninstallAllRootCertificates(force);
            }
        }

        // Returns the certificate matching the given thumbprint from the given store.
        // Returns null if not found.
        private static X509Certificate2 CertificateFromThumbprint(X509Store store, string thumbprint)
        {
            X509Certificate2Collection foundCertificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: true);
            return foundCertificates.Count == 0 ? null : foundCertificates[0];
        }

        // Adds the given certificate to the given store unless it is
        // already present.  Returns 'true' if the certificate was added.
        public static bool AddToStoreIfNeeded(StoreName storeName,
                                               StoreLocation storeLocation,
                                               X509Certificate2 certificate)
        {
            X509Store store = null;
            X509Certificate2 existingCert = null;
            try
            {
                store = new X509Store(storeName, storeLocation);

                // We assume Bridge is running elevated
                store.Open(OpenFlags.ReadWrite);
                existingCert = CertificateFromThumbprint(store, certificate.Thumbprint);
                if (existingCert == null)
                {
                    store.Add(certificate);
                    Trace.WriteLine(string.Format("[CertificateManager] Added certificate to store: "));
                    Trace.WriteLine(string.Format("    {0} = {1}", "StoreName", storeName));
                    Trace.WriteLine(string.Format("    {0} = {1}", "StoreLocation", storeLocation));
                    Trace.WriteLine(string.Format("    {0} = {1}", "CN", certificate.SubjectName.Name));
                    Trace.WriteLine(string.Format("    {0} = {1}", "HasPrivateKey", certificate.HasPrivateKey));
                    Trace.WriteLine(string.Format("    {0} = {1}", "Thumbprint", certificate.Thumbprint));
                }
            }
            finally
            {
                if (store != null)
                {
                    store.Close();
                }
            }

            return existingCert == null;
        }

        // Install the certificate into the Root store and returns its thumbprint.
        // It will not install the certificate if it is already present in the store.
        // It returns the thumbprint of the certificate, regardless whether it was added or found.
        public static string InstallCertificateToRootStore(X509Certificate2 certificate)
        {
            lock (s_certificateLock)
            {
                CertificateCacheEntry entry = null;
                if (s_rootCertificates.TryGetValue(certificate.Subject, out entry))
                {
                    return entry.Thumbprint;
                }

                bool added = AddToStoreIfNeeded(StoreName.Root, StoreLocation.LocalMachine, certificate);
                s_rootCertificates[certificate.Subject] = new CertificateCacheEntry
                {
                    Thumbprint = certificate.Thumbprint,
                    AddedToStore = added
                };

                return certificate.Thumbprint;
            }
        }

        // Install the certificate into the My store.
        // It will not install the certificate if it is already present in the store.
        // It returns the thumbprint of the certificate, regardless whether it was added or found.
        public static string InstallCertificateToMyStore(X509Certificate2 certificate, bool isValidCert = true)
        {
            lock (s_certificateLock)
            {
                bool added = AddToStoreIfNeeded(StoreName.My, StoreLocation.LocalMachine, certificate);

                return certificate.Thumbprint;
            }
        }

        // Install the certificate into the TrustedPeople store.
        // It will not install the certificate if it is already present in the store.
        // It returns the thumbprint of the certificate, regardless whether it was added or found.
        public static string InstallCertificateToTrustedPeopleStore(X509Certificate2 certificate, bool isValidCert = true)
        {
            lock (s_certificateLock)
            {
                bool added = AddToStoreIfNeeded(StoreName.TrustedPeople, StoreLocation.LocalMachine, certificate);

                return certificate.Thumbprint;
            }
        }

        // When called, generates the a cert for the machine DNS name with SAN localhost, and installs both certs
        // returns thumbprint of the machine certs
        public static X509Certificate2 CreateAndInstallLocalMachineCertificates(CertificateGenerator certificateGenerator)
        {
            if (certificateGenerator == null)
            {
                throw new ArgumentNullException("certificateGenerator");
            }

            lock (s_certificateLock)
            {
                if (s_localCertificate != null)
                {
                    return s_localCertificate;
                }

                Trace.WriteLine("[CertificateManager] Installing Root and Machine certificates to machine store.");

                // At this point, we know we haven't generated the certs yet, or the operation is completing on another thread
                // Certificate generation is time-consuming, so we want to make sure that we don't unnecessarily generate a cert

                var rootCertificate = certificateGenerator.AuthorityCertificate.Certificate;

                var fqdn = Dns.GetHostEntry("127.0.0.1").HostName;
                var hostname = fqdn.Split('.')[0];

                // always create a certificate locally for the current machine's fully qualified domain name, 
                // hostname, and "localhost". 
                CertificateCreationSettings certificateCreationSettings = new CertificateCreationSettings()
                {
                    FriendlyName = "WCF Bridge - Machine certificate generated by the CertificateManager",
                    Subject = fqdn,
                    SubjectAlternativeNames = new string[] { fqdn, hostname, "localhost" }
                };
                var hostCert = certificateGenerator.CreateMachineCertificate(certificateCreationSettings).Certificate;

                // Since s_myCertificates keys by subject name, we won't install a cert for the same subject twice
                // only the first-created cert will win
                InstallCertificateToRootStore(rootCertificate);
                InstallCertificateToMyStore(hostCert, certificateCreationSettings.ValidityType == CertificateValidityType.Valid);
                s_localCertificate = hostCert;

                // Create the PeerTrust cert
                certificateCreationSettings = new CertificateCreationSettings()
                {
                    FriendlyName = "WCF Bridge - UserPeerTrustCertificateResource",
                    Subject = fqdn,
                    SubjectAlternativeNames = new string[] { fqdn, hostname, "localhost" }
                };
                var peerCert = certificateGenerator.CreateMachineCertificate(certificateCreationSettings).Certificate;
                InstallCertificateToTrustedPeopleStore(peerCert, certificateCreationSettings.ValidityType == CertificateValidityType.Valid);
            }

            return s_localCertificate;
        }

        // We generate a local machine certificate for common usage. This method is usded to generate certs for non common usage, such as an expired cert.
        public static X509Certificate2 CreateAndInstallNonDefaultMachineCertificates(CertificateGenerator certificateGenerator, CertificateCreationSettings certificateCreationSettings, string resourceAddress)
        {
            if (certificateCreationSettings == null)
            {
                throw new ArgumentException("certificateCreationSettings cannot be null as we are creating a non default certificate");
            }

            if (certificateGenerator == null)
            {
                throw new ArgumentNullException("certificateGenerator");
            }

            lock (s_certificateLock)
            {
                Trace.WriteLine("[CertificateManager] Installing Non default Machine certificates to machine store.");

                var rootCertificate = certificateGenerator.AuthorityCertificate.Certificate;
                var hostCert = certificateGenerator.CreateMachineCertificate(certificateCreationSettings).Certificate;
                InstallCertificateToRootStore(rootCertificate);
                InstallCertificateToMyStore(hostCert, certificateCreationSettings.ValidityType == CertificateValidityType.Valid);
                return hostCert;
            }
        }

        public static void RevokeCertificate(CertificateGenerator certificateGenerator, string serialNum)
        {
            if (certificateGenerator == null)
            {
                throw new ArgumentNullException("certificateGenerator");
            }

            lock (s_certificateLock)
            {
                certificateGenerator.RevokeCertificateBySerialNumber(serialNum);
            }
        }

        public static void UninstallAllRootCertificates(bool force)
        {
            UninstallCertificates(StoreName.Root, StoreLocation.LocalMachine, s_rootCertificates, force);
        }

        public static void UninstallAllMyCertificates(bool force)
        {
            lock (s_certificateLock)
            {
                s_localCertificate = null;
            }

            UninstallCertificates(StoreName.My, StoreLocation.LocalMachine, s_myCertificates, force);
            UninstallCertificates(StoreName.My, StoreLocation.LocalMachine, s_myInvalidCertificates, force);
        }

        // Uninstalls all certificates in the given store and location that
        // were installed by this process.  If 'force' is true, uninstalls
        // all certificates used by this process, whether they already were
        // added by this process or not.
        private static void UninstallCertificates(StoreName storeName,
                                                  StoreLocation storeLocation,
                                                  Dictionary<string, CertificateCacheEntry> cache,
                                                  bool force)
        {
            lock (s_certificateLock)
            {
                if (cache.Count == 0)
                {
                    return;
                }

                X509Store store = null;
                try
                {
                    // We assume Bridge is running elevated
                    store = new X509Store(storeName, storeLocation);
                    store.Open(OpenFlags.ReadWrite);
                    foreach (var pair in cache)
                    {
                        // Remove only if our process was the one that added it
                        // or if 'force' has asked to remove all.
                        if (force || pair.Value.AddedToStore)
                        {
                            X509Certificate2 cert = CertificateFromThumbprint(store, pair.Value.Thumbprint);
                            if (cert != null)
                            {
                                store.Remove(cert);
                                Trace.WriteLine(string.Format("[CertificateManager] Removed certificate from store: "));
                                Trace.WriteLine(string.Format("    {0} = {1}", "StoreName", storeName));
                                Trace.WriteLine(string.Format("    {0} = {1}", "StoreLocation", storeLocation));
                                Trace.WriteLine(string.Format("    {0} = {1}", "CN", cert.SubjectName.Name));
                                Trace.WriteLine(string.Format("    {0} = {1}", "Thumbpint", cert.Thumbprint));
                            }
                        }
                    }
                }
                finally
                {
                    cache.Clear();

                    if (store != null)
                    {
                        store.Close();
                    }
                }
            }
        }

        private static void UninstallAllCertificatesByIssuer(StoreName storeName,
                                                        StoreLocation storeLocation,
                                                        Dictionary<string, CertificateCacheEntry> cache,
                                                        string issuerDistinguishedName)
        {
            lock (s_certificateLock)
            {
                X509Store store = null;

                try
                {
                    // We assume Bridge is running elevated
                    store = new X509Store(storeName, storeLocation);
                    store.Open(OpenFlags.ReadWrite);

                    var collection = store.Certificates.Find(X509FindType.FindByIssuerDistinguishedName, issuerDistinguishedName, false);

                    Trace.WriteLine(string.Format("[CertificateManager] Forcibly removing {0} certificates from store where:", collection.Count));
                    Trace.WriteLine(string.Format("    {0} = {1}", "StoreName", storeName));
                    Trace.WriteLine(string.Format("    {0} = {1}", "StoreLocation", storeLocation));
                    Trace.WriteLine(string.Format("    {0} = {1}", "IssuedBy", issuerDistinguishedName));

                    foreach (var cert in collection)
                    {
                        Trace.WriteLine(string.Format("        {0} = {1}", "CN", cert.SubjectName.Name));
                        Trace.WriteLine(string.Format("        {0} = {1}", "Thumbpint", cert.Thumbprint));
                    }

                    store.RemoveRange(collection);
                }
                finally
                {
                    cache.Clear();

                    if (store != null)
                    {
                        store.Close();
                    }
                }
            }
        }

        public static void InstallSSLPortCertificate(string certThumbprint, int port)
        {
            lock (s_certificateLock)
            {
                if (s_sslPorts.ContainsKey(port))
                {
                    return;
                }

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Arguments = String.Format("http add sslcert ipport=0.0.0.0:{0} certhash={1} appid={2}",
                                                       port, certThumbprint, "{00000000-0000-0000-0000-000000000000}");
                startInfo.FileName = "netsh";
                Console.WriteLine("[CertificateManager] Executing {0} to install an SSL port certificate: ", startInfo.FileName);
                Console.WriteLine("    {0} {1}", startInfo.FileName, startInfo.Arguments);
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    Console.WriteLine("[CertificateManager] Process exit code was {0}", process.ExitCode);
                    string output = process.StandardOutput.ReadToEnd();
                    if (!String.IsNullOrWhiteSpace(output))
                    {
                        Console.WriteLine("[CertificateManager] stdout was: {0}", output);
                    }

                    output = process.StandardError.ReadToEnd();
                    if (!String.IsNullOrWhiteSpace(output))
                    {
                        Console.WriteLine("[CertificateManager] stderr was: {0}", output);
                    }
                }

                s_sslPorts[port] = certThumbprint;
            }
        }

        public static void UninstallAllSslPortCertificates()
        {
            lock (s_certificateLock)
            {
                foreach (int port in s_sslPorts.Keys.ToArray())
                {
                    UninstallSslPortCertificate(port);
                }
            }
        }

        public static void UninstallSslPortCertificate(int port)
        {
            lock (s_certificateLock)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Arguments = String.Format("http delete sslcert ipport=0.0.0.0:{0}",
                                                       port);
                startInfo.FileName = "netsh";
                Console.WriteLine("[CertificateManager] Executing {0} to uninstall an SSL port certificate: ", startInfo.FileName);
                Console.WriteLine("    {0} {1}", startInfo.FileName, startInfo.Arguments);

                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                int exitCode;
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    exitCode = process.ExitCode;
                    string output = process.StandardOutput.ReadToEnd();
                    if (!String.IsNullOrWhiteSpace(output))
                    {
                        Console.WriteLine("[CertificateManager] stdout was: {0}", output);
                    }

                    output = process.StandardError.ReadToEnd();
                    if (!String.IsNullOrWhiteSpace(output))
                    {
                        Console.WriteLine("[CertificateManager] stderr was: {0}", output);
                    }
                }

                if (exitCode == 0)
                {
                    Console.WriteLine("[CertificateManager] Removed sslCert for port {0}", port);
                }
                else
                {
                    Console.WriteLine("[CertificateManager] Did not remove sslCert for port {0}", port);
                }

                s_sslPorts.Remove(port);
            }
        }

        // Certificates that are used or added by this process are
        // kept in a cache for reuse and eventual removal.
        private class CertificateCacheEntry
        {
            public string Thumbprint { get; set; }
            public bool AddedToStore { get; set; }
        }
    }
}
