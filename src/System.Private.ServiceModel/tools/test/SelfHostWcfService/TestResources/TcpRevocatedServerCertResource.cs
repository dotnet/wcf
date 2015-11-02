// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Security.Cryptography.X509Certificates;
using WcfTestBridgeCommon;
using WcfService.CertificateResources;

namespace WcfService.TestResources
{
    internal class TcpRevocatedServerCertResource : TcpResource
    {
        protected override string Address { get { return "tcp-RevocatedServerCert"; } }

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
                IsValidCert = false,
                Subjects = new string[] { s_fqdn, s_hostname, "localhost" }
            };

            string thumbprint = CertificateResourceHelpers.EnsureNonDefaultCertificateInstalled(context.BridgeConfiguration, certificateCreationSettings, Address);
            CertificateManager.RevocatingACert(CertificateResourceHelpers.GetCertificateGeneratorInstance(context.BridgeConfiguration), thumbprint);

            serviceHost.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine,
                                                        StoreName.My,
                                                        X509FindType.FindByThumbprint,
                                                        thumbprint);
        }
    }
}
