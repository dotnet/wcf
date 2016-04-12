// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class TcpTransportSecurityStreamedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            TcpTransportSecuritySslCustomCertValidationTestServiceHost serviceHost = new TcpTransportSecuritySslCustomCertValidationTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class TcpTransportSecurityStreamedTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "tcp-transport-security-streamed"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.Transport)
            {
                PortSharingEnabled = false,
                TransferMode = TransferMode.Streamed
            };
        }
        public TcpTransportSecurityStreamedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
