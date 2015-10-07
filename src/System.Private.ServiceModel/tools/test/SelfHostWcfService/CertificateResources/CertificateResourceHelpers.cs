// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Security.Cryptography.X509Certificates;
using WcfTestBridgeCommon;

namespace WcfService.CertificateResources
{
    // Provides certificate generation capability to the test service
    // We use this as CertificateGenerator is non-static, and we don't really want to make that class static
    // as its functionaity is dependent on how it is initialized
    // we want to provide one certificate generator instance only to every IResource requring this capability 
    // but at the same time need to ensure that initialization is taken care of in one place, 
    // and we can only initialize once we know the BridgeConfiguration of requests

    public static class CertificateResourceHelpers
    {
        private static object s_certificateHelperLock = new object();
        private volatile static CertificateGenerator s_certificateGenerator;
        private static string s_crlUriRelativePath = "/resource/WcfService.CertificateResources.CertificateRevocationListResource";
        
        static CertificateResourceHelpers()
        {
            CertificateManager.BridgeConfigurationChanged += (object s, EventArgs a) => OnResourceFolderChanged(s, a);
        }

        internal static CertificateGenerator GetCertificateGeneratorInstance(BridgeConfiguration config)
        {
            if (s_certificateGenerator == null)
            {
                lock (s_certificateHelperLock)
                {
                    if (s_certificateGenerator == null)
                    {
                        s_certificateGenerator = new CertificateGenerator()
                        {
                            CertificatePassword = config.BridgeCertificatePassword,
                            CrlUriBridgeHost = string.Format("http://{0}:{1}", config.BridgeHost, config.BridgePort),
                            CrlUriRelativePath = s_crlUriRelativePath,
                            ValidityPeriod = config.BridgeCertificateValidityPeriod
                        };

                        // Upon creation, we want to immediately get the authority certificate and install it 
                        // as it means we are about to run a test requiring certs
                        CertificateManager.InstallCertificateToRootStore(s_certificateGenerator.AuthorityCertificate.Certificate);
                    }
                }
            }

            return s_certificateGenerator;
        }

        internal static void ResetCertificateGenerator(ResourceRequestContext context)
        {
            var config = context.BridgeConfiguration;

            if (s_certificateGenerator == null)
            {
                lock (s_certificateHelperLock)
                {
                    if (s_certificateGenerator == null)
                    {
                        s_certificateGenerator = new CertificateGenerator()
                        {
                            CertificatePassword = config.BridgeCertificatePassword,
                            CrlUriBridgeHost = string.Format("http://{0}:{1}", config.BridgeHost, config.BridgePort),
                            CrlUriRelativePath = s_crlUriRelativePath,
                            ValidityPeriod = config.BridgeCertificateValidityPeriod
                        };

                        // Upon creation, we want to immediately get the authority certificate and install it 
                        // as it means we are about to run a test requiring certs
                        CertificateManager.InstallCertificateToRootStore(s_certificateGenerator.AuthorityCertificate.Certificate);
                    }
                }
            }
        }

        internal static string EnsureSslPortCertificateInstalled(BridgeConfiguration configuration)
        {
            // Ensure the https certificate is installed before this endpoint resource is used
            X509Certificate2 cert = CertificateManager.CreateAndInstallLocalMachineCertificates(GetCertificateGeneratorInstance(configuration));

            // Ensure http.sys has been told to use this certificate on the https port
            CertificateManager.InstallSSLPortCertificate(cert.Thumbprint, configuration.BridgeHttpsPort);

            return cert.Thumbprint;
        }

        private static void OnResourceFolderChanged(object sender, EventArgs args)
        {
            lock (s_certificateHelperLock)
            {
                s_certificateGenerator = null;
            }
        }
    }
}
