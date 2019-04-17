// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace WcfService
{
    public class HttpsTransportSecurityMessageCredentialsCertTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            HttpsTransportSecurityMessageCredentialsCertTestServiceHost serviceHost = new HttpsTransportSecurityMessageCredentialsCertTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    internal class HttpsTransportSecurityMessageCredentialsCertTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "https-message-credentials-cert"; } }

        protected override Binding GetBinding()
        {
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;

            return binding;
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
            this.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            this.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new MyX509CertificateValidator("DO_NOT_TRUST_WcfBridgeRootCA");
        }

        public HttpsTransportSecurityMessageCredentialsCertTestServiceHost(Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses)
        {
        }
    }
}
