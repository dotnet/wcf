// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "NetTcpCertValModePeerTrust.svc")]
    public class TcpCertificateValidationPeerTrustTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "nettcp-server-cert-valmode-peertrust"; } }

        protected override Binding GetBinding()
        {
            NetTcpBinding binding = new NetTcpBinding() { PortSharingEnabled = false };
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

            return binding;
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();

            string certThumprint = TestHost.CertificateFromFriendlyName(StoreName.TrustedPeople, StoreLocation.LocalMachine, "WCF Bridge - UserPeerTrustCertificateResource").Thumbprint;

            this.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine,
                                                        StoreName.TrustedPeople,
                                                        X509FindType.FindByThumbprint,
                                                        certThumprint);
        }

        public TcpCertificateValidationPeerTrustTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
