// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTPS, BasePath = "HttpsTransSecMessCredsUserName.svc")]
    internal class HttpsTransportSecurityMessageCredentialsUserNameTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "https-message-credentials-username"; } }

        protected override Binding GetBinding()
        {
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;

            return binding;
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
            AuthenticationResourceHelper.ConfigureServiceHostUserNameAuth(this);
        }

        public HttpsTransportSecurityMessageCredentialsUserNameTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
