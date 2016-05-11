// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class HttpsDigestTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            HttpsDigestTestServiceHost serviceHost = new HttpsDigestTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class HttpsDigestTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "https-digest"; } }

        protected override Binding GetBinding()
        {
            var binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Digest;
            return binding;
        }

        public HttpsDigestTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
