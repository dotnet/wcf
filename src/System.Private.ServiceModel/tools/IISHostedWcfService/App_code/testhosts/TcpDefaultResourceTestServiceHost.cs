//  Copyright (c) Microsoft Corporation.  All Rights Reserved.
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
        protected override string Address { get { return "tcp-nosecurity"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }

        public TcpDefaultResourceTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
