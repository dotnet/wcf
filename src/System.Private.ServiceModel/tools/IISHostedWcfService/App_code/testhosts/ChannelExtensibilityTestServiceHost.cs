// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Samples.MessageInterceptor;
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class ChannelExtensibilityServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ChannelExtensibilityServiceHost serviceHost = new ChannelExtensibilityServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class ChannelExtensibilityServiceHost : TestServiceHostBase<IWcfChannelExtensibilityContract>
    {
        protected override string Address { get { return "ChannelExtensibility"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new InterceptingBindingElement(new DroppingServerInterceptor()), new HttpTransportBindingElement());
        }

        public ChannelExtensibilityServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
