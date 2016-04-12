// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class HttpDigestNoDomainTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            HttpDigestNoDomainTestServiceHost serviceHost = new HttpDigestNoDomainTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class HttpDigestNoDomainTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "http-digest-nodomain"; } }

        protected override Binding GetBinding()
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            return binding;
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
            AuthenticationResourceHelper.ConfigureServiceHostUseDigestAuth(this);
        }

        public HttpDigestNoDomainTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
