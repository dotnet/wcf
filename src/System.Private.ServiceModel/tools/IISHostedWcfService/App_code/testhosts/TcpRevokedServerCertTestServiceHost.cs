// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class TcpRevokedServerCertTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            TcpRevokedServerCertTestServiceHost serviceHost = new TcpRevokedServerCertTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class TcpRevokedServerCertTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "tcp-RevokedServerCert"; } }

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

            string thumbprint = Util.CertificateFromFridendlyName(StoreName.My, StoreLocation.LocalMachine, "WCF Bridge - TcpRevokedServerCertResource").Thumbprint;

            this.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine,
                                                        StoreName.My,
                                                        X509FindType.FindByThumbprint,
                                                        thumbprint);
        }

        public TcpRevokedServerCertTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
