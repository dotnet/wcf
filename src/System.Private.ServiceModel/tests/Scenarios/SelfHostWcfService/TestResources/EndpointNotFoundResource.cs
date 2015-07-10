using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal class EndpointNotFoundResource : IResource
    {
        private static ServiceHost currentHost = null;
        private static object currentHostLock = new object();
        private const string Address = "not-found";

        public object Put()
        {
            if (currentHost == null)
            {
                lock (currentHostLock)
                {
                    if (currentHost == null)
                    {
                        currentHost = new ServiceHost(
                            typeof(WcfUserNameService),
                            new Uri(string.Format("{0}://localhost:{1}/{2}/enf-base", 
                                BaseAddressResource.Http, 
                                BaseAddressResource.HttpPort, 
                                AppDomain.CurrentDomain.FriendlyName))
                            );
                        currentHost.AddServiceEndpoint(
                            typeof(IWcfCustomUserNameService),
                            new BasicHttpBinding(), 
                            Address);
                        ModifyBehaviors(currentHost.Description);
                        currentHost.Open();
                    }
                }
            }

            return currentHost.BaseAddresses[0].AbsoluteUri.ToString();
        }

        public object Get()
        {
            return currentHost != null ? currentHost.BaseAddresses[0].AbsoluteUri.ToString() : null;
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
    }
}
