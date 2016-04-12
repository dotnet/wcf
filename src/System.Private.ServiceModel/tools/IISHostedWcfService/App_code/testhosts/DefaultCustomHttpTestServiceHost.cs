// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class DefaultCustomHttpTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            DefaultCustomHttpTestServiceHost serviceHost = new DefaultCustomHttpTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class DefaultCustomHttpTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "default-custom-http"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(), new HttpTransportBindingElement());
        }

        public DefaultCustomHttpTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
