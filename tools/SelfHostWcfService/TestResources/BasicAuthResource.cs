using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal class BasicAuthResource : IResource
    {
        private static ServiceHost currentHost = null;
        private static object currentHostLock = new object();
        private const string Address = "https-basic";

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
                            new Uri(string.Format("{0}://localhost:{1}/{2}/CustomUserName", 
                                BaseAddressResource.Https, 
                                BaseAddressResource.HttpsPort, 
                                AppDomain.CurrentDomain.FriendlyName))
                            );
                        currentHost.AddServiceEndpoint(
                            typeof(IWcfCustomUserNameService),
                            GetBinding(), 
                            Address);
                        ModifyBehaviors(currentHost.Description);
                        currentHost.Open();
                    }
                }
            }

            return currentHost.Description.Endpoints.Count != 1 ? null : currentHost.Description.Endpoints[0].ListenUri.ToString();
        }

        public object Get()
        {
            if (currentHost != null)
            {
                return currentHost.Description.Endpoints.Count != 1 ? null : currentHost.Description.Endpoints[0].ListenUri.ToString();
            }

            return null;
        }

        private Binding GetBinding()
        {
            var binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            return binding;
        }

        private ServiceCredentials GetServiceCredentials()
        {
            var serviceCredentials = new ServiceCredentials();
            serviceCredentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            serviceCredentials.UserNameAuthentication.CustomUserNamePasswordValidator = new CustomUserNameValidator();
            return serviceCredentials;
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

            desc.Behaviors.Remove<ServiceCredentials>();
            desc.Behaviors.Add(GetServiceCredentials());
        }
    }
}
