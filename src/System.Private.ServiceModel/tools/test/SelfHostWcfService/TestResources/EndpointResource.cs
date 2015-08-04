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
        private string _hostName = "localhost";
        protected string certThumbprint = "1d 85 a3 f6 cd 2c 02 2c 5c a5 4e 5c b2 00 a4 7f 89 ba 0d 3d";

        #region Host Listen Uri components

        protected abstract string Protocol { get; }

        protected virtual string Host {
            get { return _hostName; }
            set { _hostName = value; }
        }

        protected abstract string Address { get; }

        protected abstract string Port { get; }

        #endregion Host Listen Uri components

        public object Put(ResourceRequestContext context)
        {
            ServiceHost host;
            if (!s_currentHosts.TryGetValue(Address, out host))
            {
                lock (s_currentHostLock)
                {
                    if (!s_currentHosts.TryGetValue(Address, out host))
                    {
                        host = new ServiceHost(typeof(ServiceType));

                        // The URL to reach the Bridge is passed in as part of
                        // the Bridge configuration.  The default behavior of
                        // an endpoint resource is to use that for its Host name.
                        string bridgeUrl = context.BridgeConfiguration.BridgeUrl;
                        if (!string.IsNullOrEmpty(bridgeUrl))
                        {
                            Uri uri = new Uri(bridgeUrl);
                            string hostName = uri.Host;
                            Host = hostName;
                        }

                        host.AddServiceEndpoint(
                            typeof(ContractType),
                            GetBinding(),
                            BuildUri());
                        ModifyBehaviors(host.Description);
                        ModifyHost(host);
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

        protected virtual void ModifyHost(ServiceHost serviceHost)
        {
        }

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

        private Uri BuildUri()
        {
            var builder = new UriBuilder();
            builder.Host = Host;
            builder.Port = Int32.Parse(Port);
            builder.Path = AppDomain.CurrentDomain.FriendlyName + "/" + Address;
            builder.Scheme = Protocol;
            return builder.Uri;
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
