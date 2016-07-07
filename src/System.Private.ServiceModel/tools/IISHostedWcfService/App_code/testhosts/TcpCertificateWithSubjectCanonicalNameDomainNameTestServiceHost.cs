// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class TcpCertificateWithSubjectCanonicalNameDomainNameTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            TcpCertificateWithSubjectCanonicalNameDomainNameTestServiceHost serviceHost = new TcpCertificateWithSubjectCanonicalNameDomainNameTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class TcpCertificateWithSubjectCanonicalNameDomainNameTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "tcp-server-subject-cn-domainname-cert"; } }

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

            string certThumprint = TestHost.CertificateFromFriendlyName(StoreName.My, StoreLocation.LocalMachine, "WCF Bridge - TcpCertificateWithSubjectCanonicalNameDomainNameResource").Thumbprint;

            this.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine,
                                                        StoreName.My,
                                                        X509FindType.FindByThumbprint,
                                                        certThumprint);
        }

        public TcpCertificateWithSubjectCanonicalNameDomainNameTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
