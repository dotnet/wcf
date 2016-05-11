// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal abstract class EndpointResource<ServiceType, ContractType> : IResource where ServiceType : class, ContractType
    {
        private const string ResourceResponseUriKeyName = "uri";
        private const string ResourceResponseFQHNKeyName = "hostname";

        protected static int SixtyFourMB = 64 * 1024 * 1024;
        private static Dictionary<string, ServiceHost> s_currentHosts = new Dictionary<string, ServiceHost>();
        private static object s_currentHostLock = new object();
        protected static string s_fqdn = Dns.GetHostEntry("127.0.0.1").HostName;
        protected static string s_hostname = Dns.GetHostEntry("127.0.0.1").HostName.Split('.')[0];

        #region Host Listen Uri components

        protected abstract string Protocol { get; }

        protected abstract string Address { get; }

        protected virtual string GetHost(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeHost;
        }
        protected abstract int GetPort(ResourceRequestContext context);

        #endregion Host Listen Uri components

        public ResourceResponse Put(ResourceRequestContext context)
        {
            ResourceResponse response = new ResourceResponse();
            ServiceHost host;
            if (!s_currentHosts.TryGetValue(Address, out host))
            {
                lock (s_currentHostLock)
                {
                    if (!s_currentHosts.TryGetValue(Address, out host))
                    {
                        host = new ServiceHost(typeof(ServiceType));

                        host.AddServiceEndpoint(
                            typeof(ContractType),
                            GetBinding(),
                            BuildUri(context));
                        ModifyBehaviors(host.Description);
                        ModifyHost(host, context);
                        host.Open();
                        s_currentHosts.Add(Address, host);
                    }
                }
            }

            if (host.Description.Endpoints.Count == 1)
            {
                response.Properties.Add(ResourceResponseUriKeyName, host.Description.Endpoints[0].ListenUri.ToString());
                response.Properties.Add(ResourceResponseFQHNKeyName, Dns.GetHostEntry("127.0.0.1").HostName);
            }

            return response;
        }

        public ResourceResponse Get(ResourceRequestContext context)
        {
            ResourceResponse response = new ResourceResponse();
            ServiceHost host;
            if (s_currentHosts.TryGetValue(Address, out host) && (host.Description.Endpoints.Count == 1))
            {
                response.Properties.Add(ResourceResponseUriKeyName, host.Description.Endpoints[0].ListenUri.ToString());
                response.Properties.Add(ResourceResponseFQHNKeyName, s_hostname);
            }

            return response;
        }

        protected abstract Binding GetBinding();

        protected virtual void ModifyHost(ServiceHost serviceHost, ResourceRequestContext context)
        {
        }

        protected virtual void ModifyBehaviors(ServiceDescription desc)
        {
            ServiceDebugBehavior debug = desc.Behaviors.Find<ServiceDebugBehavior>();
            if (debug == null)
            {
                debug = new ServiceDebugBehavior();
                desc.Behaviors.Add(debug);
            }

            debug.IncludeExceptionDetailInFaults = true;
        }

        private Uri BuildUri(ResourceRequestContext context)
        {
            var builder = new UriBuilder();
            builder.Host = GetHost(context);
            builder.Port = GetPort(context);
            PortManager.OpenPortInFirewall(builder.Port);
            builder.Path = AppDomain.CurrentDomain.FriendlyName + "/" + Address;
            builder.Scheme = Protocol;
            return builder.Uri;
        }
    }
}
