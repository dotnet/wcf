// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class TcpNoSecurityTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            TcpNoSecurityTestServiceHost serviceHost = new TcpNoSecurityTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class TcpNoSecurityTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "tcp-nosecurity"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }

        public TcpNoSecurityTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
