// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService.TestResources
{
    internal class DuplexResource : EndpointResource<WcfDuplexService, IWcfDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Tcp; } }

        protected override string Port { get { return "810"; } }

        protected override string Address {  get { return "tcp-nosecurity-callback"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }
    }

    internal class DuplexCallbackResource : EndpointResource<DuplexCallbackService, IDuplexChannelService>
    {
        protected override string Protocol { get { return BaseAddressResource.Tcp; } }

        protected override string Port { get { return "811"; } }

        protected override string Address { get { return "tcp-nosecurity-typedproxy-duplexcallback"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }
    }

    internal class DuplexChannelCallbackReturnResource : EndpointResource<DuplexChannelCallbackReturnService, IWcfDuplexTaskReturnService>
    {
        protected override string Protocol { get { return BaseAddressResource.Tcp; } }

        protected override string Port { get { return "812"; } }

        protected override string Address { get { return "tcp-nosecurity-taskreturn"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }
    }
}
