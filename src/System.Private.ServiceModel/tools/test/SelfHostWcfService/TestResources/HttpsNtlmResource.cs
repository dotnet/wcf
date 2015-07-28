// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService.TestResources
{
    internal class HttpsNtlmResource : HttpsResource
    {
        protected override string Address { get { return "https-ntlm"; } }

        protected override Binding GetBinding()
        {
            var binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            return binding;
        }
    }
}
