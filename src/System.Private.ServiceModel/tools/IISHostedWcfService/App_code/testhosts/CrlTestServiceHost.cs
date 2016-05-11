// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace WcfService
{
    public class CrlTestServiceHostFactory : WebServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            CrlTestServiceHost serviceHost = new CrlTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class CrlTestServiceHost : WebServiceHost
    {
        public CrlTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            var binding = new WebHttpBinding();
            this.AddServiceEndpoint(typeof(ICrlService), binding, "");
        }
    }
}
