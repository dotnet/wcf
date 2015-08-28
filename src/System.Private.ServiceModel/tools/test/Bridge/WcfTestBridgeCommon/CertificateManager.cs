using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WcfTestBridgeCommon
{
    public static class CertificateManager
    {
        private static object s_certificateLock = new object();
        private static bool s_registeredForProcessExit = false;

        // Keyed by thumbprint, value is file from which it was loaded
        private static Dictionary<string, string> s_rootCertificates = new Dictionary<string, string>();
        private static Dictionary<string, string> s_myCertificates = new Dictionary<string, string>();

        // Keyed by port, value is cert thumbprint
        private static Dictionary<int, string> s_sslPorts = new Dictionary<int, string>();

        // Any change to the Bridge resource folder uninstalls all the certificates
        // installed by those resources.
        public static void OnResourceFolderChanged(string oldFolder, string newFolder)
        {
            UninstallAllCertificates();
        }

        public static void UninstallAllCertificates()
        {
            lock (s_certificateLock)
            {
                UninstallAllSslPortCertificates();
                UninstallAllMyCertificates();
                UninstallAllRootCertificates();
            }
        }

        private static void RegisterForProcessExit()
        {
            lock (s_certificateLock)
            {
                if (!s_registeredForProcessExit)
                {
                    AppDomain.CurrentDomain.ProcessExit += (s, e) =>
                    {
                        UninstallAllCertificates();
                    };
                    s_registeredForProcessExit = true;
                }
            }
        }

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

        private static bool TryFindCertificate(X509Store store, string subjectName, out string thumbprint)
        {
            thumbprint = null;
            foreach (var c in store.Certificates)
            {
                if (String.Equals(c.SubjectName.Name, subjectName, StringComparison.OrdinalIgnoreCase))
                {
                    thumbprint = c.Thumbprint;
                    return true;
                }
            }

            return false;
        }

        // Install the certificate in the given file path into the Root store and returns its thumbprint.
        // It will not install the certificate if there is already one with the same full SubjectName present.
        // It returns the thumbprint of the certificate, regardless whether it was added or found.
        public static string InstallRootCertificate(BridgeConfiguration configuration, string certificateName)
        {
            string certificateFilePath = CreateCertificateFilePath(configuration, certificateName);

            lock (s_certificateLock)
            {
                foreach (var pair in s_rootCertificates)
                {
                    if (string.Equals(certificateName, pair.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        return pair.Key;
                    }
                }

                X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                X509Certificate2 cert = new X509Certificate2(certificateFilePath);
                string thumbprint = null;
                if (!TryFindCertificate(store, cert.SubjectName.Name, out thumbprint))
                {
                    store.Add(cert);
                    thumbprint = cert.Thumbprint;
                    s_rootCertificates[cert.Thumbprint] = certificateName;
                    Console.WriteLine("Added to root store certificate '{0}' : '{1}'", certificateName, cert.SubjectName.Name);
                }
                else
                {
                    Console.WriteLine("Reusing existing root store certificate '{0}' : '{1}'", certificateName, cert.SubjectName.Name);
                }

                store.Close();

                return thumbprint;
            }
        }

        // Install the certificate in the given file path into the My store.
        // It will not install the certificate if there is already one with the same full SubjectName present.
        // It returns the thumbprint of the certificate, regardless whether it was added or found.
        public static string InstallMyCertificate(BridgeConfiguration configuration, string certificateName)
        {
            // Installing any certificate guarantees the certificate authority is loaded first
            InstallRootCertificate(configuration, configuration.BridgeCertificateAuthority);

            string certificateFilePath = CreateCertificateFilePath(configuration, certificateName);

            lock (s_certificateLock)
            {
                foreach (var pair in s_myCertificates)
                {
                    if (string.Equals(certificateName, pair.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        return pair.Key;
                    }
                }

                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);

                X509Certificate2 cert = new X509Certificate2();
                // "test" is currently the required password to allow exportable private keys
                cert.Import(certificateFilePath, "test", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
                string thumbprint = null;
                if (!TryFindCertificate(store, cert.SubjectName.Name, out thumbprint))
                {
                    store.Add(cert);
                    thumbprint = cert.Thumbprint;
                    s_myCertificates[cert.Thumbprint] = certificateName;
                    Console.WriteLine("Added to my store certificate '{0}' : '{1}'", certificateName, cert.SubjectName.Name);
                }
                else
                {
                    Console.WriteLine("Reusing existing my store certificate '{0}' : '{1}'", certificateName, cert.SubjectName.Name);
                }

                store.Close();
                return thumbprint;
            }
        }

        private static void UninstallAllRootCertificates()
        {
            lock (s_certificateLock)
            {
                foreach (var cert in s_rootCertificates.Keys.ToArray())
                {
                    UninstallCertificate(cert, StoreName.Root, s_rootCertificates);
                }
            }
        }

        private static void UninstallAllMyCertificates()
        {
            lock (s_certificateLock)
            {
                foreach (var cert in s_myCertificates.Keys.ToArray())
                {
                    UninstallCertificate(cert, StoreName.My, s_myCertificates);
                }
            }
        }

        private static X509Certificate2 CertificateFromThumbprint(X509Store store, string thumbprint)
        {
            foreach (var cert in store.Certificates)
            {
                if (String.Equals(cert.Thumbprint, thumbprint, StringComparison.OrdinalIgnoreCase))
                {
                    return cert;
                }
            }

            return null;
        }

        private static void UninstallCertificate(string thumbprint, StoreName storeName, Dictionary<string,string> installedCertificates)
        {
            X509Store store = new X509Store(storeName, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            X509Certificate2 cert = CertificateFromThumbprint(store, thumbprint);
            if (cert != null)
            {
                store.Remove(cert);
            }
            store.Close();
            installedCertificates.Remove(thumbprint);
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
                    Console.WriteLine("stdout was: {0}", process.StandardOutput.ReadToEnd());
                    Console.WriteLine("stderr was: {0}", process.StandardError.ReadToEnd());
                }

                s_sslPorts[port] = certThumbprint;
            }
        }

        private static void UninstallAllSslPortCertificates()
        {
            foreach (int port in s_sslPorts.Keys.ToArray())
            {
                UninstallSslPortCertificate(port);
            }
        }

        private static void UninstallSslPortCertificate(int port)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = String.Format("http delete sslcert ipport=0.0.0.0:{0}",
                                                   port);
            startInfo.FileName = "netsh";
            Console.WriteLine("Executing: {0} {1}", startInfo.FileName, startInfo.Arguments);
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                Console.WriteLine("Process exit code was {0}", process.ExitCode);
                Console.WriteLine("stdout was: {0}", process.StandardOutput.ReadToEnd());
                Console.WriteLine("stderr was: {0}", process.StandardError.ReadToEnd());
            }

            s_sslPorts.Remove(port);
        }
    }
}
