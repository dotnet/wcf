// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;
using System.ServiceModel.Channels;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal class DuplexResource : EndpointResource<WcfDuplexService, IWcfDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Tcp; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeTcpPort + 1;
        }

        protected override string Address { get { return "tcp-nosecurity-callback"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }
    }

    internal class DuplexCallbackResource : EndpointResource<DuplexCallbackService, IDuplexChannelService>
    {
        protected override string Protocol { get { return BaseAddressResource.Tcp; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeTcpPort + 2;
        }

        protected override string Address { get { return "tcp-nosecurity-typedproxy-duplexcallback"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }
    }

    internal class DuplexChannelCallbackReturnResource : EndpointResource<DuplexChannelCallbackReturnService, IWcfDuplexTaskReturnService>
    {
        protected override string Protocol { get { return BaseAddressResource.Tcp; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeTcpPort + 3;
        }

        protected override string Address { get { return "tcp-nosecurity-taskreturn"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }
    }

    internal class DuplexCallbackDataContractComplexTypeResource : EndpointResource<WcfDuplexService, IWcfDuplexService_DataContract>
    {
        protected override string Protocol { get { return BaseAddressResource.Tcp; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeTcpPort + 4;
        }

        protected override string Address { get { return "tcp-nosecurity-callback"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }
    }

    internal class DuplexCallbackXmlComplexTypeResource : EndpointResource<WcfDuplexService, IWcfDuplexService_Xml>
    {
        protected override string Protocol { get { return BaseAddressResource.Tcp; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeTcpPort + 5;
        }

        protected override string Address { get { return "tcp-nosecurity-callback"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }
    }
}
