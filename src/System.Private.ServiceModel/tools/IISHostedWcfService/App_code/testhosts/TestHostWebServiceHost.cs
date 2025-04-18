// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace WcfService
{
    public class TestHostWebServiceHostFactory : WebServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            TestHostWebServiceHost serviceHost = new TestHostWebServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class TestHostWebServiceHost : WebServiceHost
    {
        public TestHostWebServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            var binding = new WebHttpBinding();
            this.AddServiceEndpoint(typeof(ITestHost), binding, "");
        }
    }
}
#endif
