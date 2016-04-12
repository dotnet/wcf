//  Copyright (c) Microsoft Corporation.  All Rights Reserved.
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class BasicHttpTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            BasicHttpTestServiceHost serviceHost = new BasicHttpTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class BasicHttpTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public BasicHttpTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
