// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        // Keyed by CA subject
        private static Dictionary<string, CertificateCacheEntry> s_rootCertificates = new Dictionary<string, CertificateCacheEntry>(StringComparer.OrdinalIgnoreCase);

        // Keyed by port, value is cert thumbprint
        private static Dictionary<int, string> s_sslPorts = new Dictionary<int, string>();

        // When we install certificates via CreateAndInstallMachineCertificates, put local cert here 
        // for reference when we need to add the sslport references
        private static X509Certificate2 s_localCertificate = null;

        // Returns the certificate matching the given thumbprint from the given store.
        // Returns null if not found.
        private static X509Certificate2 CertificateFromThumbprint(X509Store store, string thumbprint)
        {
            X509Certificate2Collection foundCertificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: true);
            return foundCertificates.Count == 0 ? null : foundCertificates[0];
        }

        // Adds the given certificate to the given store unless it is
        // already present.  Returns 'true' if the certificate was added.
        public static bool AddToStoreIfNeeded(StoreName storeName, StoreLocation storeLocation, X509Certificate2 certificate)
        {
            X509Store store = null;
            X509Certificate2 existingCert = null;
            try
            {
                store = CertificateHelper.GetX509Store(storeName, storeLocation);

                // We assume Bridge is running elevated
                if (!CertificateHelper.CurrentOperatingSystem.IsMacOS())
                {
                    store.Open(OpenFlags.ReadWrite);
                }
                existingCert = CertificateFromThumbprint(store, certificate.Thumbprint);
                if (existingCert == null)
                {
                    store.Add(certificate);
                    Trace.WriteLine(string.Format("[CertificateManager] Added certificate to store: "));
                    Trace.WriteLine(string.Format("    {0} = {1}", "StoreName", store.Name));
                    Trace.WriteLine(string.Format("    {0} = {1}", "StoreLocation", store.Location));
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

        // Certificates that are used or added by this process are
        // kept in a cache for reuse and eventual removal.
        private class CertificateCacheEntry
        {
            public string Thumbprint { get; set; }
            public bool AddedToStore { get; set; }
        }
    }
}
