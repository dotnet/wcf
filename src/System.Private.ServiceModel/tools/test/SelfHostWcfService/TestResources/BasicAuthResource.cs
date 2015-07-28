// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        private static ServiceHost s_currentHost = null;
        private static object s_currentHostLock = new object();
        private const string Address = "https-basic";

        public object Put()
        {
            if (s_currentHost == null)
            {
                lock (s_currentHostLock)
                {
                    if (s_currentHost == null)
                    {
                        s_currentHost = new ServiceHost(
                            typeof(WcfUserNameService),
                            new Uri(string.Format("{0}://localhost:{1}/{2}/CustomUserName",
                                BaseAddressResource.Https,
                                BaseAddressResource.HttpsPort,
                                AppDomain.CurrentDomain.FriendlyName))
                            );
                        s_currentHost.AddServiceEndpoint(
                            typeof(IWcfCustomUserNameService),
                            GetBinding(),
                            Address);
                        ModifyBehaviors(s_currentHost.Description);
                        s_currentHost.Open();
                    }
                }
            }

            return s_currentHost.Description.Endpoints.Count != 1 ? null : s_currentHost.Description.Endpoints[0].ListenUri.ToString();
        }

        public object Get()
        {
            if (s_currentHost != null)
            {
                return s_currentHost.Description.Endpoints.Count != 1 ? null : s_currentHost.Description.Endpoints[0].ListenUri.ToString();
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
