﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class HttpsTransportSecurityMessageCredentialsUserNameTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            HttpsTransportSecurityMessageCredentialsUserNameTestServiceHost serviceHost = new HttpsTransportSecurityMessageCredentialsUserNameTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    internal class HttpsTransportSecurityMessageCredentialsUserNameTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override IList<Binding> GetBindings()
        {
            return new List<Binding> { GetWSHttpBinding(), GetWS2007HttpBinding() };
        }

        private Binding GetWSHttpBinding()
        {
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            binding.Name = "https-message-credentials-username";
            return binding;
        }

        private Binding GetWS2007HttpBinding()
        {
            WS2007HttpBinding binding = new WS2007HttpBinding(SecurityMode.TransportWithMessageCredential);
            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            binding.Name = "https2007-message-credentials-username";
            return binding;
        }

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

        public HttpsTransportSecurityMessageCredentialsUserNameTestServiceHost(Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses)
        {
        }
    }
}
