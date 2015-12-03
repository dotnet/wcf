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
    internal class TcpCertificateWithServerAltNameResource : TcpResource
    {
        protected override string Address { get { return "tcp-server-alt-name-cert"; } }

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

            // CN=not-real-subject-name means that a cert for "not-real-subject-name" will be installed 
            // Per #422 this shouldn't matter as we now check with SAN
            
            CertificateCreationSettings certificateCreationSettings = new CertificateCreationSettings()
            {
                Subjects = new string[] { "not-real-subject-name", "not-real-subject-name.example.com", s_fqdn, s_hostname, "localhost" }
            };

            X509Certificate2 cert = CertificateResourceHelpers.EnsureCustomCertificateInstalled(context.BridgeConfiguration, certificateCreationSettings, Address);

            serviceHost.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine,
                                                        StoreName.My,
                                                        X509FindType.FindByThumbprint,
                                                        cert.Thumbprint);
        }
    }
}
