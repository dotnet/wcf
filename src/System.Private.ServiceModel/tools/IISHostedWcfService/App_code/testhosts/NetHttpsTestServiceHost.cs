// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTPS, BasePath = "NetHttps.svc")]
    public class NetHttpsTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override IList<Binding> GetBindings()
        {
            return new List<Binding> {
                GetNetHttpBinding(NetHttpMessageEncoding.Binary),
                GetNetHttpBinding(NetHttpMessageEncoding.Text),
                GetNetHttpBinding(NetHttpMessageEncoding.Mtom)
            };
        }

        private Binding GetNetHttpBinding(NetHttpMessageEncoding encoding)
        {
            var binding = new NetHttpsBinding();
            binding.MessageEncoding = encoding;
            binding.Name = Enum.GetName(typeof(NetHttpMessageEncoding), encoding);
            return binding;
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
