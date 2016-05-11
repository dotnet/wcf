// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class DuplexWebSocketServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            DuplexWebSocketServiceHost serviceHost = new DuplexWebSocketServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class DuplexWebSocketServiceHost : TestServiceHostBase<IWcfDuplexService>
    {
        protected override string Address { get { return "WebSocket"; } }

        protected override Binding GetBinding()
        {
            var binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            return binding;
        }
        public DuplexWebSocketServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
