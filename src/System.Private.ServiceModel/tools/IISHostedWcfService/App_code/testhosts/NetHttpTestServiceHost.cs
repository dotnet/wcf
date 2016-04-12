// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class NetHttpTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            NetHttpTestServiceHost serviceHost = new NetHttpTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class NetHttpTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "NetHttp"; } }

        protected override Binding GetBinding()
        {
            return new NetHttpBinding();
        }

        public NetHttpTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
