// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfProjectNService
{
    public class UnderstoodHeadersServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            UnderstoodHeadersServiceHost serviceHost = new UnderstoodHeadersServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class UnderstoodHeadersServiceHost : TestServiceHostBase<IUnderstoodHeaders>
    {
        protected override string Address { get { return "UnderstoodHeaders"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public UnderstoodHeadersServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
