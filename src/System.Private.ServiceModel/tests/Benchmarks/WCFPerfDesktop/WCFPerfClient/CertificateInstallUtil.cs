using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WCFPerfClient
{
    public static class CertificateInstallUtil
    {
        private static object s_certificateLock = new object();
        public static X509Certificate2 InstallClientCertificateFromServer(string serviceUrl, string cerResoure)
        {
            X509KeyStorageFlags storageFlags = X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.UserKeySet;

            X509Certificate2 clientCertificate = new X509Certificate2(GetResourceFromServiceAsByteArray(serviceUrl, cerResoure), "test", storageFlags);
            return InstallCertificateToTrustedPeopleStore(clientCertificate);
        }

        public static X509Certificate2 InstallCertificateToRootStore(X509Certificate2 certificate)
        {
            // See explanation of StoreLocation selection at PlatformSpecificRootStoreLocation
            certificate = AddToStoreIfNeeded(StoreName.Root, StoreLocation.CurrentUser, certificate);
            return certificate;
        }

        private static X509Certificate2 InstallRootCertificateFromServer(string serviceUrl, string cerResoure)
        {
            X509Certificate2 rootCertificate = new X509Certificate2(GetResourceFromServiceAsByteArray(serviceUrl, cerResoure));
            return InstallCertificateToRootStore(rootCertificate);
        }

        public static void EnsureClientCertificateInstalled(string serviceUrl, string cerResoure)
        {
            try
            {
                var rootCertificate = InstallRootCertificateFromServer(serviceUrl, "RootCert");
            }
            catch
            {
                // Exceptions installing the root certificate are captured and
                // will be reported if it is requested.  But allow the attempt
                // to install the client certificate to succeed or fail independently.
            }

            try
            {
                // Once only, we interrogate the service utility endpoint
                // for the client certificate and install it locally if it
                // is not already in the store.
                var clientCertificate = InstallClientCertificateFromServer(serviceUrl, cerResoure);
            }
            catch
            {

            }
        }

        public static byte[] GetResourceFromServiceAsByteArray(string serviceUrl, string resource)
        {
            string requestUri = GetResourceAddress(serviceUrl, resource);

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = httpClient.GetAsync(requestUri).GetAwaiter().GetResult();
                return response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            }
        }

        private static string GetResourceAddress(string serviceUrl, string resource, string protocol = "http")
        {
            string host = new Uri(serviceUrl).Host;
            return string.Format(@"{0}://{1}/{2}/{3}/{4}", protocol, host, "testhost", "testhost.svc", resource);
        }

        public static X509Certificate2 InstallCertificateToTrustedPeopleStore(X509Certificate2 certificate)
        {
            certificate = AddToStoreIfNeeded(StoreName.TrustedPeople, StoreLocation.CurrentUser, certificate);
            return certificate;
        }

        public static X509Certificate2 AddToStoreIfNeeded(StoreName storeName, StoreLocation storeLocation, X509Certificate2 certificate)
        {
            X509Certificate2 resultCert = null;
            lock (s_certificateLock)
            {
                // Open the store as ReadOnly first, as it prevents the need for elevation if opening
                // a LocalMachine store
                using (X509Store store = new X509Store(storeName, storeLocation))
                {
                    store.Open(OpenFlags.ReadWrite);
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

        public static X509Certificate2 CertificateFromThumbprint(X509Store store, string thumbprint, bool validOnly)
        {
            X509Certificate2Collection foundCertificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly);
            return foundCertificates.Count == 0 ? null : foundCertificates[0];
        }
    }
}
