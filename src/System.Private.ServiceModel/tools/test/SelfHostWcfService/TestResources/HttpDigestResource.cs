// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


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
