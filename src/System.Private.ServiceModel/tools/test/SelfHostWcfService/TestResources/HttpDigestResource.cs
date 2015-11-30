// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;
using System.ServiceModel.Channels;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal class HttpDigestNoDomainResource : HttpResource
    {
        protected override string Address { get { return "http-digest-nodomain"; } }

        protected override Binding GetBinding()
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            return binding;
        }

        protected override void ModifyHost(ServiceHost serviceHost, ResourceRequestContext context)
        {
            AuthenticationResourceHelper.ConfigureServiceHostUseDigestAuth(serviceHost);
            base.ModifyHost(serviceHost, context);
        }
    }
}
