// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal class EndpointNotFoundResource : IResource
    {
        private static ServiceHost s_currentHost = null;
        private static object s_currentHostLock = new object();
        private const string Address = "not-found";

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
                            new Uri(string.Format("{0}://localhost:{1}/{2}/enf-base",
                                BaseAddressResource.Http,
                                BaseAddressResource.HttpPort,
                                AppDomain.CurrentDomain.FriendlyName))
                            );
                        s_currentHost.AddServiceEndpoint(
                            typeof(IWcfCustomUserNameService),
                            new BasicHttpBinding(),
                            Address);
                        ModifyBehaviors(s_currentHost.Description);
                        s_currentHost.Open();
                    }
                }
            }

            return s_currentHost.BaseAddresses[0].AbsoluteUri.ToString();
        }

        public object Get()
        {
            return s_currentHost != null ? s_currentHost.BaseAddresses[0].AbsoluteUri.ToString() : null;
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
