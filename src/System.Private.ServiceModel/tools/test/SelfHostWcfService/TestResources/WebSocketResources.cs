// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace WcfService.TestResources
{
    internal class DuplexWebSocketResource : EndpointResource<WcfWebSocketService, IWcfDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override string Port { get { return "8083"; } }

        protected override string Address { get { return "http-defaultduplexwebsockets"; } }

        protected override Binding GetBinding()
        {
            return new NetHttpBinding();
        }
    }

    internal class WebSocketTransportResource : EndpointResource<WcfWebSocketTransportUsageAlwaysService, IWcfDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override string Port { get { return "8083"; } }

        protected override string Address { get { return "http-requestreplywebsockets-transportusagealways"; } }

        protected override Binding GetBinding()
        {
            NetHttpBinding binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            return binding;
        }
    }
}
