// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;
using System.ServiceModel.Channels;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal class ServiceContractAsyncIntOutResource : EndpointResource<ServiceContractIntOutService, IServiceContractIntOutService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeHttpPort;
        }
        protected override string Address { get { return "ServiceContractIntOut"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }
    }

    internal class ServiceContractAsyncUniqueTypeOutResource : EndpointResource<ServiceContractUniqueTypeOutService, IServiceContractUniqueTypeOutService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeHttpPort;
        }
        protected override string Address { get { return "ServiceContractUniqueTypeOut"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }
    }

    internal class ServiceContractAsyncIntRefResource : EndpointResource<ServiceContractIntRefService, IServiceContractIntRefService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeHttpPort;
        }
        protected override string Address { get { return "ServiceContractIntRef"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }
    }

    internal class ServiceContractAsyncUniqueTypeRefResource : EndpointResource<ServiceContractUniqueTypeRefService, IServiceContractUniqueTypeRefService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeHttpPort;
        }
        protected override string Address { get { return "ServiceContractAsyncUniqueTypeRef"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }
    }

    internal class ServiceContractSyncUniqueTypeOutResource : EndpointResource<ServiceContractUniqueTypeOutSyncService, IServiceContractUniqueTypeOutSyncService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeHttpPort;
        }
        protected override string Address { get { return "ServiceContractUniqueTypeOutSync"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }
    }

    internal class ServiceContractSyncUniqueTypeRefResource : EndpointResource<ServiceContractUniqueTypeRefSyncService, IServiceContractUniqueTypeRefSyncService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeHttpPort;
        }
        protected override string Address { get { return "ServiceContractUniqueTypeRefSync"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }
    }
}
