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
    internal class TcpRevokedServerCertResource : TcpResource
    {
        protected override string Address { get { return "tcp-RevokedServerCert"; } }

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
            //Create a certificate and add to the revocation list
            CertificateCreationSettings certificateCreationSettings = new CertificateCreationSettings()
            {
                ValidityType = CertificateValidityType.Revoked,
                Subjects = new string[] { s_fqdn, s_hostname, "localhost" }
            };

            X509Certificate2 cert = CertificateResourceHelpers.EnsureCustomCertificateInstalled(context.BridgeConfiguration, certificateCreationSettings, Address);

            serviceHost.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine,
                                                        StoreName.My,
                                                        X509FindType.FindByThumbprint,
                                                        cert.Thumbprint);
        }
    }
}
