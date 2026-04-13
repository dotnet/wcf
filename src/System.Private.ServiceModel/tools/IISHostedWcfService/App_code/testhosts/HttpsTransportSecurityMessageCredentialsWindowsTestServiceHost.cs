// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTPS, BasePath = "HttpsTransSecMessCredsWindows.svc")]
    internal class HttpsTransportSecurityMessageCredentialsWindowsTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override IList<Binding> GetBindings()
        {
            return new List<Binding>
            {
                GetWSHttpBinding(establishSecurityContext: true),
                GetWSHttpBinding(establishSecurityContext: false)
            };
        }

        private Binding GetWSHttpBinding(bool establishSecurityContext)
        {
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            binding.Security.Message.EstablishSecurityContext = establishSecurityContext;
            binding.Name = establishSecurityContext
                ? "https-message-credentials-windows"
                : "https-message-credentials-windows-nosc";
            return binding;
        }

        public HttpsTransportSecurityMessageCredentialsWindowsTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
