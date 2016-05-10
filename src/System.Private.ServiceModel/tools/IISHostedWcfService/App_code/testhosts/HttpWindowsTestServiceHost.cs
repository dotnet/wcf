// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class HttpWindowsTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            HttpWindowsTestServiceHost serviceHost = new HttpWindowsTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class HttpWindowsTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "http-windows"; } }

        protected override Binding GetBinding()
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            return binding;
        }

        //REM remember to use host name when start the self host service
        //protected override string GetHost()
        //{
        //    return Environment.MachineName;
        //}

        public HttpWindowsTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}

