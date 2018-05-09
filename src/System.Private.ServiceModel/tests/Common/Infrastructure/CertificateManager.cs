// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Security.Cryptography; 
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Infrastructure.Common
{
    // This class manages the adding and removal of certificates.
    // It also handles informing http.sys of which certificate to associate with SSL ports.
    internal static class CertificateManager
    {
        private static object s_certificateLock = new object();
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
        internal static StoreLocation PlatformSpecificRootStoreLocation
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
                    catch(CryptographicException)
                    {
                        // Linux
                        s_PlatformSpecificRootStoreLocation = StoreLocation.CurrentUser;
                    }

                    s_PlatformSpecificStoreLocationIsSet = true;
                }
                return s_PlatformSpecificRootStoreLocation;
            }
        }

        internal static string OSXCustomKeychainFilePath
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, "wcfLocal.keychain");
            }
        }

        internal static string OSXCustomKeychainPassword
        {
            get
            {
                return "WCFKeychainFilePassword";
            }
        }

        // Adds the given certificate to the given store unless it is
        // already present.  Returns the  certificate either already in
        // the store or the one requested.
        public static X509Certificate2 AddToStoreIfNeeded(StoreName storeName,
                                                          StoreLocation storeLocation,
                                                          X509Certificate2 certificate)
        {
            X509Certificate2 resultCert = null;
            lock (s_certificateLock)
            {
                // Open the store as ReadOnly first, as it prevents the need for elevation if opening
                // a LocalMachine store
                using (X509Store store = new X509Store(storeName, storeLocation))
                {
                    store.Open(OpenFlags.ReadOnly);
                    resultCert = CertificateFromThumbprint(store, certificate.Thumbprint, validOnly: false);
                }

                // Not already in store.  We need to add it.
                if (resultCert == null)
                {
                    using (X509Store store = new X509Store(storeName, storeLocation))
                    {
                        try
                        {
                            store.Open(OpenFlags.ReadWrite);
                            store.Add(certificate);
                            resultCert = certificate;
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
                    }
                }
            }

            return resultCert;
        }

        // Adds the given certificate to the given keychain unless it is
        // already present.  Returns the  certificate either already in
        // the store or the one requested.
        public static X509Certificate2 AddToOSXKeyChainIfNeeded(SafeKeychainHandle keychain,
                                                          X509Certificate2 certificate)
        {
            X509Certificate2 resultCert = null;
            lock (s_certificateLock)
            {
                using (X509Store store = new X509Store(keychain.DangerousGetHandle()))
                {
                    // No need to open X509Store as it is already opened with mode of MaxAllowed 
                    resultCert = CertificateFromThumbprint(store, certificate.Thumbprint, validOnly: false);
                    // Not already in store.  We need to add it.
                    if (resultCert == null)
                    {
                        try
                        {
                            // We don't need the private key and the X509Certificate2 instance wasn't created as exportable.
                            // This creates a new certificate instance with only the public key so that it can be added to keychain.
                            var publicOnly = new X509Certificate2(certificate.RawData);
                            store.Add(publicOnly);
                            resultCert = publicOnly;
                        }
                        catch (CryptographicException inner)
                        {
                            throw new InvalidOperationException($"Error while attempting to install cert with thumbprint '{certificate.Thumbprint}' into OSX custom keychain.", inner);
                        }
                    }
                }
            }

            return resultCert;
        }

        // Returns the certificate matching the given thumbprint from the given store.
        // Returns null if not found.
        private static X509Certificate2 CertificateFromThumbprint(X509Store store, string thumbprint, bool validOnly)
        {
            X509Certificate2Collection foundCertificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly);
            return foundCertificates.Count == 0 ? null : foundCertificates[0];
        }

        private static X509Certificate2 CertificateFromThumbprint(StoreName storeName,
                                                                  StoreLocation storeLocation,
                                                                  string thumbprint,
                                                                  bool validOnly)
        {
            X509Certificate2 resultCert = null;
            using (X509Store store = new X509Store(storeName, storeLocation))
            {
                store.Open(OpenFlags.ReadOnly);
                resultCert = CertificateFromThumbprint(store, thumbprint, validOnly);
            }

            return resultCert;
        }

        private static X509Certificate2 KeychainCertificateFromThumbprint(string thumbprint, bool validOnly)
        {
            X509Certificate2 resultCert = null;
            using (SafeKeychainHandle handle = SafeKeychainHandle.Open(CertificateManager.OSXCustomKeychainFilePath, CertificateManager.OSXCustomKeychainPassword))
            {
                using (X509Store store = new X509Store(handle.DangerousGetHandle()))
                {
                    resultCert = CertificateFromThumbprint(store, thumbprint, validOnly);
                }
            }

            return resultCert;
        }

        // Retrieves a root certificate matching the given thumbprint from the root store
        public static X509Certificate2 RootCertificateFromThumprint(string thumbprint, bool validOnly)
        {
            return CertificateFromThumbprint(StoreName.Root, PlatformSpecificRootStoreLocation, thumbprint, validOnly);
        }

        // Retrieves a client certificate matching the given thumbprint from the certificate store
        public static X509Certificate2 ClientCertificateFromThumprint(string thumbprint, bool validOnly)
        {
            return CertificateFromThumbprint(StoreName.My, StoreLocation.CurrentUser, thumbprint, validOnly);
        }

        // Retrieves a server certificate matching the given thumbprint from the certificate store
        public static X509Certificate2 PeerCertificateFromThumprint(string thumbprint, bool validOnly)
        {
            return CertificateFromThumbprint(StoreName.TrustedPeople, StoreLocation.CurrentUser, thumbprint, validOnly);
        }

        // Retrieves a server certificate matching the given thumbprint from a OSX local Keychain store
        public static X509Certificate2 OSXLocalKeychainCertificateFromThumprint(string thumbprint, bool validOnly)
        {
            return KeychainCertificateFromThumbprint(thumbprint, validOnly);
        }

        // Install the certificate into the Root store and returns its thumbprint.
        // It will not install the certificate if it is already present in the store.
        // It returns the thumbprint of the certificate, regardless whether it was added or found.
        public static X509Certificate2 InstallCertificateToRootStore(X509Certificate2 certificate)
        {
            // See explanation of StoreLocation selection at PlatformSpecificRootStoreLocation
            certificate = AddToStoreIfNeeded(StoreName.Root, PlatformSpecificRootStoreLocation, certificate);
            return certificate;
        }

        // Install the certificate into the My store.
        // It will not install the certificate if it is already present in the store.
        // It returns the thumbprint of the certificate, regardless whether it was added or found.
        public static X509Certificate2 InstallCertificateToMyStore(X509Certificate2 certificate)
        {
            // Always install client certs to CurrentUser
            // StoreLocation.CurrentUser is supported on both Linux and Windows 
            // Furthermore, installing this cert to this location does not require sudo or admin elevation
            certificate = AddToStoreIfNeeded(StoreName.My, StoreLocation.CurrentUser, certificate);

            return certificate;
        }

        // Install the certificate into the TrustedPeople store.
        // It will not install the certificate if it is already present in the store.
        // It returns the thumbprint of the certificate, regardless whether it was added or found.
        public static X509Certificate2 InstallCertificateToTrustedPeopleStore(X509Certificate2 certificate)
        {
            // Always install certs to CurrentUser
            // StoreLocation.CurrentUser is supported on both Linux and Windows 
            // Furthermore, installing this cert to this location does not require sudo or admin elevation
            certificate = AddToStoreIfNeeded(StoreName.TrustedPeople, StoreLocation.CurrentUser, certificate);

            return certificate;
        }

        // Install the certificate into a custom keychain on OSX. The TrustedPeople store isn't supported
        // on OSX but a similar mechanism can be achieved by creating a custom keychain and using it in
        // the same way as the TrustedPeople store.
        // It will not install the certificate if it is already present in the store.
        // It returns the thumbprint of the certificate, regardless whether it was added or found.
        public static X509Certificate2 InstallCertificateToOSXKeychainStore(X509Certificate2 certificate)
        {
            SafeKeychainHandle keychain;

            if (!File.Exists(OSXCustomKeychainFilePath))
            {
                keychain = SafeKeychainHandle.Create(OSXCustomKeychainFilePath, OSXCustomKeychainPassword);
            }
            else
            {
                keychain = SafeKeychainHandle.Open(OSXCustomKeychainFilePath, OSXCustomKeychainPassword);
            }

            certificate = AddToOSXKeyChainIfNeeded(keychain, certificate);

            return certificate;
        }
    }
}
