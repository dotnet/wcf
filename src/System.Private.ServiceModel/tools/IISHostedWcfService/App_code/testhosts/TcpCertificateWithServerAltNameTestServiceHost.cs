// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class TcpCertificateWithServerAltNameTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            TcpCertificateWithServerAltNameTestServiceHost serviceHost = new TcpCertificateWithServerAltNameTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class TcpCertificateWithServerAltNameTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "tcp-server-alt-name-cert"; } }

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

            string certThumprint = Util.CertificateFromFridendlyName(StoreName.My, StoreLocation.LocalMachine, "WCF Bridge - TcpCertificateWithServerAltNameResource").Thumbprint;

            this.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine,
                                                        StoreName.My,
                                                        X509FindType.FindByThumbprint,
                                                        certThumprint);
        }

        public TcpCertificateWithServerAltNameTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
