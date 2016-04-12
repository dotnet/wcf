// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class BasicHttpsTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            BasicHttpsTestServiceHost serviceHost = new BasicHttpsTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class BasicHttpsTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "basicHttps"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpsBinding();
        }

        public BasicHttpsTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
