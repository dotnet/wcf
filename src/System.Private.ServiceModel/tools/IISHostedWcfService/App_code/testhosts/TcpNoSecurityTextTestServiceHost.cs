// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class TcpNoSecurityTextTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            TcpNoSecurityTextTestServiceHost serviceHost = new TcpNoSecurityTextTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class TcpNoSecurityTextTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "tcp-custombinding-nosecurity-text"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(), new TcpTransportBindingElement() { PortSharingEnabled = false });
        }

        public TcpNoSecurityTextTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
