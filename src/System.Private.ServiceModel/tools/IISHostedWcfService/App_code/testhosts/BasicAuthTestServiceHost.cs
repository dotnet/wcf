// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;

namespace WcfService
{
    public class BasicAuthTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            BasicAuthTestServiceHost serviceHost = new BasicAuthTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class BasicAuthTestServiceHost : TestServiceHostBase<IWcfCustomUserNameService>
    {
        protected override string Address { get { return "https-basic"; } }


        protected override Binding GetBinding()
        {
            var binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            return binding;
        }

        private ServiceCredentials GetServiceCredentials()
        {
            var serviceCredentials = new ServiceCredentials();
            serviceCredentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            serviceCredentials.UserNameAuthentication.CustomUserNamePasswordValidator = new CustomUserNameValidator();
            return serviceCredentials;
        }

        public BasicAuthTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
            this.Description.Behaviors.Remove<ServiceCredentials>();
            this.Description.Behaviors.Add(GetServiceCredentials());
        }
    }
}
