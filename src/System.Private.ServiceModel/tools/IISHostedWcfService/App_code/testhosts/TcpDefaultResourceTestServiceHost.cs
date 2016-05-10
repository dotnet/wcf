// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class TcpDefaultResourceServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            TcpDefaultResourceTestServiceHost serviceHost = new TcpDefaultResourceTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class TcpDefaultResourceTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "tcp-default"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding() { PortSharingEnabled = false };
        }

        public TcpDefaultResourceTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
