// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Security.Cryptography; 
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
        private const string UserCertificateResourceName = "WcfService.CertificateResources.UserCertificateResource";

        // key names in request/response keyval pairs
        private const string EndpointResourceRequestNameKeyName = "name";
        private const string SubjectKeyName = "subject";
        private const string ThumbprintKeyName = "thumbprint";
        private const string CertificateKeyName = "certificate";
        private const string IsLocalKeyName = "isLocal";

        private const string ClientCertificateSubject = "WCF Client Certificate";
        private const string ClientCertificatePassword = "test"; // this needs to be kept in sync with the Bridge configuration 

        public static string LocalCertThumbprint { get; private set; }

        // Dictionary of certificates installed by CertificateManager
        // Keyed by the Thumbprint of the certificate
        private static Dictionary<string, CertificateCacheEntry> s_myCertificates = new Dictionary<string, CertificateCacheEntry>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, CertificateCacheEntry> s_rootCertificates = new Dictionary<string, CertificateCacheEntry>(StringComparer.OrdinalIgnoreCase);

        private static bool s_PlatformSpecificStoreLocationIsSet = false;
        private static StoreLocation s_PlatformSpecificRootStoreLocation = StoreLocation.LocalMachine;

        // The location from where to open StoreName.Root
        //
        // Since we don't have access to System.Runtime.InteropServices.IsOSPlatform API, we need to find a novel way to switch
        // the store we use on Linux
        //
        // For Linux: 
        // On Linux, opening stores from the LocalMachine store is not supported yet, so we toggle to use CurrentUser
        // Furthermore, we don't need to sudo in Linux to install a StoreName.Root : StoreLocation.CurrentUser. So this will allow
        // tests requiring certs to pass in Linux. 
        // See dotnet/corefx#3690
        //
        // For Windows: 
        // We don't want to use CurrentUser, as writing to StoreName.Root : StoreLocation.CurrentUser will result in a 
        // modal dialog box that isn't dismissable if the root cert hasn't already been installed previously. 
        // If the cert has already been installed, writing to StoreName.Root : StoreLocation.CurrentUser results in a no-op
        // 
        // In other words, on Windows, we can bypass the modal dialog box, but only if we install to StoreName.Root : StoreLocation.LocalMachine
        // To do this though means that we must run certificate-based tests elevated
        private static StoreLocation PlatformSpecificRootStoreLocation
        {
            get
            {
                if (!s_PlatformSpecificStoreLocationIsSet)
                {
                    try
                    {
                        using (var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                        {
                            store.Open(OpenFlags.ReadWrite);
                        }
                    }
                    catch (PlatformNotSupportedException)
                    {
                        // Linux
                        s_PlatformSpecificRootStoreLocation = StoreLocation.CurrentUser; 
                    }
                    s_PlatformSpecificStoreLocationIsSet = true;
                }
                return s_PlatformSpecificRootStoreLocation;
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
                if (response.TryGetValue(ThumbprintKeyName, out thumbprint))
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
                    // See explanation of StoreLocation selection at PlatformSpecificRootStoreLocation
                    using (X509Store store = new X509Store(StoreName.Root, PlatformSpecificRootStoreLocation))
                    {
                        store.Open(OpenFlags.ReadOnly);
                        var collection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                        if (collection.Count > 0)
                        {
                            // We don't need to add the cert ourselves, because this cert has previously been installed 
                            // Likely BridgeClient is running on the same machine as the Bridge and the Bridge has done the work already.
                            return;
                        }
                    }

                    // Request the certificate from the Bridge
                    string base64Cert = string.Empty;
                    response = BridgeClient.MakeResourceGetRequest(CertificateAuthorityResourceName, null);

                    string certificateAsBase64;
                    if (response.TryGetValue(CertificateKeyName, out certificateAsBase64))
                    {
                        // Root cert coming from Bridge doesn't have a password or private key
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
                                CertificateKeyName,
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

            // PUT the Client Certificate Subject to the Bridge (returns thumbprint)
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            requestParams.Add(SubjectKeyName, ClientCertificateSubject);

            var response = BridgeClient.MakeResourcePutRequest(UserCertificateResourceName, requestParams);

            string thumbprint;
            bool foundUserCertificate = false;

            lock (s_certificateLock)
            {
                if (response.TryGetValue(ThumbprintKeyName, out thumbprint))
                {
                    // Set property with the thumbprint so a test case can use it.
                    LocalCertThumbprint = thumbprint;

                    foundUserCertificate = s_myCertificates.ContainsKey(thumbprint);

                    // The Bridge tells us if the request has been made for a local certificate local to the bridge. 
                    // If it has, then the Bridge itself has already installed that cert as part of the PUT request
                    // There's no need for us to do this in the BridgeClient.
                    string isLocalString;
                    if (response.TryGetValue(IsLocalKeyName, out isLocalString))
                    {
                        bool isLocal = false;
                        if (bool.TryParse(isLocalString, out isLocal) && isLocal)
                        {
                            return;
                        }
                    }
                }

                if (!foundUserCertificate)
                {
                    // GET the cert with thumbprint from the Bridge (returns cert in base64 format)
                    requestParams = new Dictionary<string, string>();
                    requestParams.Add(ThumbprintKeyName, thumbprint);

                    string base64Cert = string.Empty;
                    response = BridgeClient.MakeResourceGetRequest(UserCertificateResourceName, requestParams);

                    string certificateAsBase64;
                    if (response.TryGetValue(CertificateKeyName, out certificateAsBase64))
                    {
                        certificateToInstall = new X509Certificate2(Convert.FromBase64String(certificateAsBase64), ClientCertificatePassword, X509KeyStorageFlags.PersistKeySet);
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var pair in response)
                        {
                            sb.AppendFormat("{0}  {1} : {2}", Environment.NewLine, pair.Key, pair.Value);
                        }

                        throw new Exception(
                            string.Format("Error retrieving '{0}' certificate from Bridge, thumbprint '{1}'.\r\nExpected '{2}' key in response. Response contents:{3}{4}",
                                ClientCertificateSubject,
                                thumbprint,
                                CertificateKeyName,
                                Environment.NewLine,
                                sb.ToString()));
                    }
                }
            }

            // certificateToInstall could be null in the case the user certification exists
            if (certificateToInstall != null)
            {
                InstallCertificateToMyStore(certificateToInstall);
                // We also need to install the root cert if we install a local cert
                InstallRootCertificateFromBridge();
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
            X509Certificate2 existingCert = null;
            lock (s_certificateLock)
            {
                // Open the store as ReadOnly first, as it prevents the need for elevation if opening
                // a LocalMachine store
                using (X509Store store = new X509Store(storeName, storeLocation))
                {
                    store.Open(OpenFlags.ReadOnly);
                    existingCert = CertificateFromThumbprint(store, certificate.Thumbprint);
                }

                if (existingCert == null)
                {
                    using (X509Store store = new X509Store(storeName, storeLocation))
                    {
                        try
                        {
                            store.Open(OpenFlags.ReadWrite);
                        }
                        catch (CryptographicException inner)
                        {
                            StringBuilder exceptionString = new StringBuilder();
                            exceptionString.AppendFormat("Error opening StoreName: '{0}' certificate store from StoreLocation '{1}' in ReadWrite mode ", storeName, storeLocation);
                            exceptionString.AppendFormat("while attempting to install cert with thumbprint '{1}'.", Environment.NewLine, certificate.Thumbprint);
                            exceptionString.AppendFormat("{0}This is usually due to permissions issues if writing to the LocalMachine location", Environment.NewLine);
                            exceptionString.AppendFormat("{0}Try running the test with elevated or superuser permissions.", Environment.NewLine);

                            throw new InvalidOperationException(exceptionString.ToString(), inner);
                        }
                        store.Add(certificate);
                    }
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

                // See explanation of StoreLocation selection at PlatformSpecificRootStoreLocation
                bool added = AddToStoreIfNeeded(StoreName.Root, PlatformSpecificRootStoreLocation, certificate);
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

                // Always install client certs to CurrentUser
                // StoreLocation.CurrentUser is supported on both Linux and Windows 
                // Furthermore, installing this cert to this location does not require sudo or admin elevation
                bool added = AddToStoreIfNeeded(StoreName.My, StoreLocation.CurrentUser, certificate);
                s_myCertificates[certificate.Thumbprint] = new CertificateCacheEntry
                {
                    Thumbprint = certificate.Thumbprint,
                    AddedToStore = added
                };

                return certificate.Thumbprint;
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
