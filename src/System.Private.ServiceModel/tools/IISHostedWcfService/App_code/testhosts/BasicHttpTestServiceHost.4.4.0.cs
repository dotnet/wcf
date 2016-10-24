// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class BasicHttpTestServiceHostFactory_4_4_0 : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            BasicHttpTestServiceHost_4_4_0 serviceHost = new BasicHttpTestServiceHost_4_4_0(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class BasicHttpTestServiceHost_4_4_0 : TestServiceHostBase<IWcfService_4_4_0>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public BasicHttpTestServiceHost_4_4_0(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
