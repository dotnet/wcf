// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class UtilTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            UtilTestServiceHost serviceHost = new UtilTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class UtilTestServiceHost : TestServiceHostBase<IUtil>
    {
        protected override string Address { get { return "Util"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public UtilTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
