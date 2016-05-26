// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


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
            TcpTransportSecurityStreamedTestServiceHost serviceHost = new TcpTransportSecurityStreamedTestServiceHost(serviceType, baseAddresses);
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
