// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTPS, BasePath = "NetHttps.svc")]
    public class NetHttpsTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "NetHttps"; } }

        protected override Binding GetBinding()
        {
            return new NetHttpsBinding();
        }

        public NetHttpsTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WSS, BasePath = "NetHttpsWebSockets.svc")]
    public class NetHttpsTestServiceHostUsingWebSockets : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "NetHttpsWebSockets"; } }

        protected override Binding GetBinding()
        {
            var binding = new NetHttpsBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            return binding;
        }

        public NetHttpsTestServiceHostUsingWebSockets(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
