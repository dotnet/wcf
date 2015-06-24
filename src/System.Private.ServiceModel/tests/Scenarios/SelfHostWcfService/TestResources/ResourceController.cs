using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal abstract class ResourceController<ServiceType, ContractType> : IResource where ServiceType : class, ContractType 
    {
        private static Dictionary<string, ServiceHost> currentHosts = new Dictionary<string, ServiceHost>();
        private static object currentHostLock = new object();

        protected abstract string Protocol { get; }

        protected abstract string Address { get; }

        public string PUT(string port)
        {
            ServiceHost host;
            if (!currentHosts.TryGetValue(Address, out host))
            {
                lock (currentHostLock)
                {
                    if (!currentHosts.TryGetValue(Address, out host))
                    {
                        host = new ServiceHost(typeof(ServiceType));
                        // URI assumes localhost but that instead should be passed in
                        host.AddServiceEndpoint(
                            typeof(ContractType),
                            GetBinding(),
                            new Uri(string.Format("{0}://localhost:{1}/{2}/{3}", Protocol, port, AppDomain.CurrentDomain.FriendlyName, Address)));
                        host.Open();
                        currentHosts.Add(Address, host);
                    }
                }
            }

            return host.Description.Endpoints.Count != 1 ? null : host.Description.Endpoints[0].ListenUri.ToString();
        }

        protected abstract Binding GetBinding();
    }
}
