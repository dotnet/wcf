// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif
using System.Security.Cryptography.X509Certificates;

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "NetTcpCertValModePeerTrust.svc")]
    public class TcpCertificateValidationPeerTrustTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "nettcp-server-cert-valmode-peertrust"; } }

        protected override Binding GetBinding()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

            return binding;
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();

            X509Certificate2 cert = TestHost.CertificateFromFriendlyName(StoreName.TrustedPeople, StoreLocation.LocalMachine, "WCF Bridge - UserPeerTrustCertificateResource");
            this.Credentials.ServiceCertificate.Certificate = cert;
        }

        public TcpCertificateValidationPeerTrustTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
