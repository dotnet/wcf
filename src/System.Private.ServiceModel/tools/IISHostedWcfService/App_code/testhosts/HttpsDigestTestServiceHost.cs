// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTPS, BasePath = "DigestAuthentication/HttpsDigest.svc")]
    public class HttpsDigestTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "https-digest"; } }

        protected override Binding GetBinding()
        {
            var binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Digest;
            return binding;
        }

        public HttpsDigestTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
