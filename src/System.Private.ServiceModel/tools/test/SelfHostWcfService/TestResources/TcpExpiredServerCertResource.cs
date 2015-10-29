// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Security.Cryptography.X509Certificates;
using WcfTestBridgeCommon;
using WcfService.CertificateResources;
using System.Collections.Generic;

namespace WcfService.TestResources
{
    internal class TcpExpiredServerCertResource : TcpResource
    {
        protected override string Address { get { return "tcp-ExpiredServerCert"; } }

        protected override Binding GetBinding()
        {
            NetTcpBinding binding = new NetTcpBinding() { PortSharingEnabled = false };
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

            return binding;
        }

        protected override void ModifyHost(ServiceHost serviceHost, ResourceRequestContext context)
        {
            // Ensure the service certificate is installed before this endpoint resource is used
            //Create an expired certificate
            CertificateCreationSettings certificateCreationSettings = new CertificateCreationSettings()
            {
                IsValidCert = false,
                ValidityNotBefore = DateTime.UtcNow - TimeSpan.FromDays(4),
                ValidityNotAfter = DateTime.UtcNow - TimeSpan.FromDays(2),
                //If you specify multiple subjects, the first one becomes the subject, and all of them become Subject Alt Names.
                //In this case, the certificate subject is  CN=fqdn, OU=..., O=... , and SANs will be  fqdn, hostname, localhost
                //We do this so that a single bridge setup can deal with all the possible addresses that a client might use.
                //If we don't put "localhost' here, a long-running bridge will not be able to receive requests from both fqdn  and  localhost
                //because the certs won't match.
                Subjects = new string[] { s_fqdn, s_hostname, "localhost" }
            };

            string thumbprint = CertificateResourceHelpers.EnsureNonDefaultCertificateInstalled(context.BridgeConfiguration, certificateCreationSettings, Address);

            serviceHost.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine,
                                                        StoreName.My,
                                                        X509FindType.FindByThumbprint,
                                                        thumbprint);
        }
    }
}
