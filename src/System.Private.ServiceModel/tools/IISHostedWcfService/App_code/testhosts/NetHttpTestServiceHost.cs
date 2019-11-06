// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "NetHttp.svc")]
    public class NetHttpTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "NetHttp"; } }

        protected override Binding GetBinding()
        {
            return new NetHttpBinding();
        }

        public NetHttpTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WS, BasePath = "NetHttpWebSockets.svc")]
    public class NetHttpTestServiceHostUsingWebSockets : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "NetHttpWebSockets"; } }

        protected override Binding GetBinding()
        {
            var binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            return binding;
        }

        public NetHttpTestServiceHostUsingWebSockets(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
