// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class TcpStreamedNoSecurityTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            TcpStreamedNoSecurityTestServiceHost serviceHost = new TcpStreamedNoSecurityTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class TcpStreamedNoSecurityTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "tcp-streamed-nosecurity"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None)
            {
                PortSharingEnabled = false,
                TransferMode = TransferMode.Streamed
            };
        }

        public TcpStreamedNoSecurityTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
