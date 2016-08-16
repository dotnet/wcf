// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfService
{
    public class WsHttpTransSecTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WsHttpTranSecTestServiceHost serviceHost = new WsHttpTranSecTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class WsHttpTranSecTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "wshttp-transec"; } }

        protected override Binding GetBinding()
        {
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.Transport, reliableSessionEnabled: false);
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.TransactionFlow = false;
            return binding;
        }

        public WsHttpTranSecTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
