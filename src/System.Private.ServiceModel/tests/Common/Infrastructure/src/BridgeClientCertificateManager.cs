// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Infrastructure.Common
{
    // This class manages the adding and removal of certificates.
    // It also handles informing http.sys of which certificate to associate with SSL ports.
    public static class BridgeClientCertificateManager
    {
        private static object s_certificateLock = new object();

        // Resource names 
        private const string CertificateAuthorityResourceName = "WcfService.CertificateResources.CertificateAuthorityResource";
        private const string MachineCertificateResourceName = "WcfService.CertificateResources.MachineCertificateResource";

        // key names in request/response keyval pairs
        private const string EndpointResourceRequestNameResourceString = "name";
        private const string SubjectKeyResourceString = "subject";
        private const string ThumbprintKeyResourceString = "thumbprint";
        private const string CertificateKeyResourceString = "certificate";
        private const string IsLocalKeyResourceString = "isLocal";

        // Keyed by the thumbprint of the certificate
        private static string s_LocalFqdn = string.Empty;

        // Dictionary of certificates installed by CertificateManager
        // Keyed by the Thumbprint of the certificate
        private static Dictionary<string, CertificateCacheEntry> s_myCertificates = new Dictionary<string, CertificateCacheEntry>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, CertificateCacheEntry> s_rootCertificates = new Dictionary<string, CertificateCacheEntry>(StringComparer.OrdinalIgnoreCase);

        private static string LocalFqdn
        {
            get
            {
                // Get an FQDN for the local machine and cache it
                if (string.IsNullOrEmpty(s_LocalFqdn))
                {
                    s_LocalFqdn = Dns.GetHostEntryAsync("127.0.0.1").GetAwaiter().GetResult().HostName;
                }
                return s_LocalFqdn;
            }
        }

        // Installs the root certificate from the bridge 
        // Needed for all tests so we trust the incoming certificate coming from the service/bridge
        // We do our best to detect if we're running on the same box as the bridge; if so, we don't try to install the cert
        // as that operation requires admin privileges
        public static void InstallRootCertificateFromBridge()
        {
            // PUT the Authority to the Bridge (returns thumbprint)
            var response = BridgeClient.MakeResourcePutRequest(CertificateAuthorityResourceName, null);

            string thumbprint;
            X509Certificate2 certificateToInstall = null;
            bool rootCertificateAlreadyInstalled = false;

            lock (s_certificateLock)
            {
                if (response.TryGetValue(ThumbprintKeyResourceString, out thumbprint))
                {
                    rootCertificateAlreadyInstalled = s_rootCertificates.ContainsKey(thumbprint);
                }

                if (rootCertificateAlreadyInstalled)
                {
                    // Cert's been installed already, bail out
                    return;
                }
                else
                {
                    using (X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                    {
                        store.Open(OpenFlags.ReadOnly);
                        var collection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                        if (collection.Count > 0)
                        {
                            // We don't need to actually add the cert ourselves, because this cert has previously been installed 
                            // Likely BridgeClient is running on the same machine as the Bridge and the Bridge has done the work already.
                            return;
                        }
                    }

                    // Request the certificate from the Bridge
                    string base64Cert = string.Empty;
                    response = BridgeClient.MakeResourceGetRequest(CertificateAuthorityResourceName, null);

                    string certificateAsBase64;
                    if (response.TryGetValue(CertificateKeyResourceString, out certificateAsBase64))
                    {
                        certificateToInstall = new X509Certificate2(Convert.FromBase64String(certificateAsBase64));
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var pair in response)
                        {
                            sb.AppendFormat("{0}  {1} : {2}", Environment.NewLine, pair.Key, pair.Value);
                        }

                        throw new Exception(
                            string.Format("Error retrieving Authority certificate from Bridge. Expected '{0}' key in response. Response contents:{1}{2}",
                                CertificateKeyResourceString,
                                Environment.NewLine,
                                sb.ToString()));
                    }
                }
            }

            // We return or throw before this point if there is no certificateToInstall
            InstallCertificateToRootStore(certificateToInstall);
        }

        // Installs the local certificate provided by the bridge 
        // Root is installed as part of this call so we trust the incoming certificate coming from the service/bridge 
        // We supply this certificate for bidirectional (tcp) communication
        // 
        // The request to the bridge with the local FQDN concurrently asks the bridge if we are running locally. 
        // if so, we don't try to install the cert as that operation requires admin privileges
        public static void InstallLocalCertificateFromBridge()
        {
            X509Certificate2 certificateToInstall = null; 

            // PUT the Machine name to the Bridge (returns thumbprint)
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            requestParams.Add(SubjectKeyResourceString, LocalFqdn);

            var response = BridgeClient.MakeResourcePutRequest(MachineCertificateResourceName, requestParams);

            string thumbprint;
            bool foundLocalCertificate = false;

            lock(s_certificateLock)
            {
                if (response.TryGetValue(ThumbprintKeyResourceString, out thumbprint))
                {
                    foundLocalCertificate = s_myCertificates.ContainsKey(thumbprint);

                    // The Bridge tells us if the request has been made for a local certificate local to the bridge. 
                    // If it has, then the Bridge itself has already installed that cert as part of the PUT request
                    // There's no need for us to do this in the BridgeClient.
                    string isLocalString;
                    if (response.TryGetValue(IsLocalKeyResourceString, out isLocalString))
                    {
                        bool isLocal = false;
                        if (bool.TryParse(isLocalString, out isLocal) && isLocal)
                        {
                            return;
                        }
                    }
                }

                if (!foundLocalCertificate)
                {
                    // GET the cert with thumbprint from the Bridge (returns cert in base64 format)
                    requestParams = new Dictionary<string, string>();
                    requestParams.Add(ThumbprintKeyResourceString, thumbprint);

                    string base64Cert = string.Empty;
                    response = BridgeClient.MakeResourceGetRequest(MachineCertificateResourceName, requestParams);

                    string certificateAsBase64;
                    if (response.TryGetValue(CertificateKeyResourceString, out certificateAsBase64))
                    {
                        certificateToInstall = new X509Certificate2(Convert.FromBase64String(certificateAsBase64));
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var pair in response)
                        {
                            sb.AppendFormat("{0}  {1} : {2}", Environment.NewLine, pair.Key, pair.Value);
                        }

                        throw new Exception(
                            string.Format("Error retrieving {0} certificate from Bridge. Expected '{1}' key in response. Response contents:{2}{3}",
                                s_LocalFqdn,
                                CertificateKeyResourceString,
                                Environment.NewLine,
                                sb.ToString()));
                    }
                }
            }

            InstallCertificateToMyStore(certificateToInstall);

            // We also need to install the root cert if we install a local cert
            InstallRootCertificateFromBridge();
        }

        // Uninstalls all certificates and SSL port associations
        // added by this process.  If 'force' is true, removes all,
        // whether this process created them or not.
        public static void UninstallAllCertificates(bool force)
        {
            lock (s_certificateLock)
            {
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
                }
            }
            finally
            {
                if (store != null)
                {
                    store.Dispose();
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
                if (s_rootCertificates.TryGetValue(certificate.Thumbprint, out entry))
                {
                    return entry.Thumbprint;
                }

                bool added = AddToStoreIfNeeded(StoreName.Root, StoreLocation.LocalMachine, certificate);
                s_rootCertificates[certificate.Thumbprint] = new CertificateCacheEntry
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
        public static string InstallCertificateToMyStore(X509Certificate2 certificate)
        {
            lock (s_certificateLock)
            {
                CertificateCacheEntry entry = null;
                if (s_myCertificates.TryGetValue(certificate.Thumbprint, out entry))
                {
                    return entry.Thumbprint;
                }

                bool added = AddToStoreIfNeeded(StoreName.My, StoreLocation.LocalMachine, certificate);
                s_myCertificates[certificate.Thumbprint] = new CertificateCacheEntry
                {
                    Thumbprint = certificate.Thumbprint,
                    AddedToStore = added
                };

                return certificate.Thumbprint;
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
                            X509Certificate2 cert = CertificateFromThumbprint(store, pair.Value.Thumbprint);
                            if (cert != null)
                            {
                                store.Remove(cert);
                            }
                        }
                    }
                }
                finally
                {
                    cache.Clear();

                    if (store != null)
                    {
                        store.Dispose();
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
                    store = new X509Store(storeName, storeLocation);
                    store.Open(OpenFlags.ReadWrite);

                    var collection = store.Certificates.Find(X509FindType.FindByIssuerDistinguishedName, issuerDistinguishedName, false);
                    foreach (var cert in collection)
                    {
                        store.Remove(cert);
                    }

                    
                }
                finally
                {
                    cache.Clear();

                    if (store != null)
                    {
                        store.Dispose();
                    }
                }

            }
        }
        
        // Certificates that are used or added by this process are
        // kept in a cache for reuse and eventual removal.
        class CertificateCacheEntry
        {
            public string Thumbprint { get; set; }
            public bool AddedToStore { get; set; }
        }
    }
}
