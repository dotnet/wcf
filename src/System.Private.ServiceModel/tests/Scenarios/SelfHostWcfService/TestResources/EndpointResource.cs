// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal abstract class EndpointResource<ServiceType, ContractType> : IResource where ServiceType : class, ContractType
    {
        private static Dictionary<string, ServiceHost> s_currentHosts = new Dictionary<string, ServiceHost>();
        private static object s_currentHostLock = new object();

        protected abstract string Protocol { get; }

        protected abstract string Address { get; }

        protected abstract string Port { get; }

        public object Put()
        {
            ServiceHost host;
            if (!s_currentHosts.TryGetValue(Address, out host))
            {
                lock (s_currentHostLock)
                {
                    if (!s_currentHosts.TryGetValue(Address, out host))
                    {
                        host = new ServiceHost(typeof(ServiceType));
                        // URI assumes localhost but that instead should be passed in
                        host.AddServiceEndpoint(
                            typeof(ContractType),
                            GetBinding(),
                            new Uri(string.Format("{0}://localhost:{1}/{2}/{3}", Protocol, Port, AppDomain.CurrentDomain.FriendlyName, Address)));
                        ModifyBehaviors(host.Description);
                        host.Open();
                        s_currentHosts.Add(Address, host);
                    }
                }
            }

            return host.Description.Endpoints.Count != 1 ? null : host.Description.Endpoints[0].ListenUri.ToString();
        }

        public object Get()
        {
            ServiceHost host;
            if (s_currentHosts.TryGetValue(Address, out host))
            {
                return host.Description.Endpoints.Count != 1 ? null : host.Description.Endpoints[0].ListenUri.ToString();
            }

            return null;
        }

        protected abstract Binding GetBinding();

        private void ModifyBehaviors(ServiceDescription desc)
        {
            ServiceDebugBehavior debug = desc.Behaviors.Find<ServiceDebugBehavior>();
            if (debug == null)
            {
                debug = new ServiceDebugBehavior();
                desc.Behaviors.Add(debug);
            }

            debug.IncludeExceptionDetailInFaults = true;
        }
    }

    internal abstract class HttpResource : EndpointResource<WcfService, IWcfService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override string Port { get { return BaseAddressResource.HttpPort; } }
    }

    internal abstract class HttpsResource : EndpointResource<WcfService, IWcfService>
    {
        protected override string Protocol { get { return BaseAddressResource.Https; } }

        protected override string Port { get { return BaseAddressResource.HttpsPort; } }
    }

    internal abstract class TcpResource : EndpointResource<WcfService, IWcfService>
    {
        protected override string Protocol { get { return BaseAddressResource.Tcp; } }

        protected override string Port { get { return BaseAddressResource.TcpPort; } }
    }
}
