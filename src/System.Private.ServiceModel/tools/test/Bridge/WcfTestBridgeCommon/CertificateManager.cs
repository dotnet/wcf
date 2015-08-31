using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace WcfTestBridgeCommon
{
    // This class manages the adding and removal of certificates.
    // It also handles informing http.sys of which certificate to associate with SSL ports.
    public static class CertificateManager
    {
        private static object s_certificateLock = new object();

        // Keyed by base file name of the certificate
        private static Dictionary<string, CertificateCacheEntry> s_rootCertificates = new Dictionary<string, CertificateCacheEntry>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, CertificateCacheEntry> s_myCertificates = new Dictionary<string, CertificateCacheEntry>(StringComparer.OrdinalIgnoreCase);

        // Keyed by port, value is cert thumbprint
        private static Dictionary<int, string> s_sslPorts = new Dictionary<int, string>();

        // Any change to the Bridge resource folder uninstalls all the certificates
        // installed by those resources.
        public static void OnResourceFolderChanged(string oldFolder, string newFolder)
        {
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

        // Given a certificate base file name, this method will create and
        // return its full path as found in the Bridge Resource Folder.
        private static string CreateCertificateFilePath(BridgeConfiguration configuration, string certificateName)
        {
            if (String.IsNullOrWhiteSpace(configuration.BridgeResourceFolder))
            {
                throw new ArgumentNullException("The BridgeResourceFolder has not been set.", "BridgeResourceFolder");
            }

            string path = Path.Combine(Path.Combine(configuration.BridgeResourceFolder, "Certificates"), certificateName);
            if (!File.Exists(path))
            {
                throw new InvalidOperationException(String.Format("The requested certificate file at {0} does not exist.",
                                                    path));
            }

            return path;
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
        private static bool AddToStoreIfNeeded(StoreName storeName, 
                                               StoreLocation storeLocation, 
                                               X509Certificate2 certificate)
        {
            X509Store store = null;
            X509Certificate2 existingCert = null;
            try
            {
                store = new X509Store(storeName, storeLocation);
                store.Open(OpenFlags.ReadWrite);
                existingCert = CertificateFromThumbprint(store, certificate.Thumbprint);
                if (existingCert == null)
                {
                    store.Add(certificate);
                    Console.WriteLine("Added to store '{0}', location '{1}', certificate '{2}'", 
                                       storeName, storeLocation, certificate.SubjectName.Name);
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

        // Install the certificate in the given file path into the Root store and returns its thumbprint.
        // It will not install the certificate if it is already present in the store.
        // It returns the thumbprint of the certificate, regardless whether it was added or found.
        public static string InstallRootCertificate(BridgeConfiguration configuration, string certificateName)
        {
            lock (s_certificateLock)
            {
                CertificateCacheEntry entry = null;
                if (s_rootCertificates.TryGetValue(certificateName, out entry))
                {
                    return entry.ThumbPrint;
                }

                string certificateFilePath = CreateCertificateFilePath(configuration, certificateName);
                X509Certificate2 cert = new X509Certificate2(certificateFilePath);
                bool added = AddToStoreIfNeeded(StoreName.Root, StoreLocation.LocalMachine, cert);
                s_rootCertificates[certificateName] = new CertificateCacheEntry
                {
                    ThumbPrint = cert.Thumbprint,
                    AddedToStore = added
                };

                return cert.Thumbprint;
            }
        }

        // Install the certificate in the given file path into the My store.
        // It will not install the certificate if it is already present in the store.
        // It returns the thumbprint of the certificate, regardless whether it was added or found.
        public static string InstallMyCertificate(BridgeConfiguration configuration, string certificateName)
        {
            // Installing any MY certificate guarantees the certificate authority is loaded first
            InstallRootCertificate(configuration, configuration.BridgeCertificateAuthority);

            lock (s_certificateLock)
            {
                CertificateCacheEntry entry = null;
                if (s_myCertificates.TryGetValue(certificateName, out entry))
                {
                    return entry.ThumbPrint;
                }

                string certificateFilePath = CreateCertificateFilePath(configuration, certificateName);
                X509Certificate2 cert = new X509Certificate2();
                // "test" is currently the required password to allow exportable private keys
                cert.Import(certificateFilePath, "test", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

                bool added = AddToStoreIfNeeded(StoreName.My, StoreLocation.LocalMachine, cert);
                s_myCertificates[certificateName] = new CertificateCacheEntry
                {
                    ThumbPrint = cert.Thumbprint,
                    AddedToStore = added
                };

                return cert.Thumbprint;
            }
        }

        public static void UninstallAllRootCertificates(bool force)
        {
            UninstallCertificates(StoreName.Root, StoreLocation.LocalMachine, s_rootCertificates, force);
        }

        public static void UninstallAllMyCertificates(bool force)
        {
            UninstallCertificates(StoreName.My, StoreLocation.LocalMachine, s_myCertificates, force);
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
                    store = new X509Store(storeName, storeLocation);
                    store.Open(OpenFlags.ReadWrite);
                    foreach (var pair in cache)
                    {
                        // Remove only if our process was the one that added it
                        // or if 'force' has asked to remove all.
                        if (force || pair.Value.AddedToStore)
                        {
                            X509Certificate2 cert = CertificateFromThumbprint(store, pair.Value.ThumbPrint);
                            if (cert != null)
                            {
                                store.Remove(cert);
                                Console.WriteLine("Uninstalled from store '{0}', location '{1}', cert '{2}'",
                                                    storeName, storeLocation, cert.SubjectName.Name);
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
                Console.WriteLine("Executing: {0} {1}", startInfo.FileName, startInfo.Arguments);
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    Console.WriteLine("Process exit code was {0}", process.ExitCode);
                    string output = process.StandardOutput.ReadToEnd();
                    if (!String.IsNullOrWhiteSpace(output))
                    {
                        Console.WriteLine("stdout was: {0}", output);
                    }

                    output = process.StandardError.ReadToEnd();
                    if (!String.IsNullOrWhiteSpace(output))
                    {
                        Console.WriteLine("stderr was: {0}", output);
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
                Console.WriteLine("Executing: {0} {1}", startInfo.FileName, startInfo.Arguments);
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
                        Console.WriteLine("stdout was: {0}", output);
                    }

                    output = process.StandardError.ReadToEnd();
                    if (!String.IsNullOrWhiteSpace(output))
                    {
                        Console.WriteLine("stderr was: {0}", output);
                    }
                }

                if (exitCode == 0)
                {
                    Console.WriteLine("Removed sslCert for port {0}", port);
                }
                else
                {
                    Console.WriteLine("Did not remove sslCert for port {0}", port);
                }

                s_sslPorts.Remove(port);
            }
        }

        // Certificates that are used or added by this process are
        // kept in a cache for reuse and eventual removal.
        class CertificateCacheEntry
        {
            public string ThumbPrint { get; set; }
            public bool AddedToStore { get; set; }

        }
    }
}
