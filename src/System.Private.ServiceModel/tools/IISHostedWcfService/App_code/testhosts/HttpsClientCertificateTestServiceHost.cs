// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace WcfService
{
    public class HttpsClientCertificateTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            HttpsClientCertificateTestServiceHost serviceHost = new HttpsClientCertificateTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class HttpsClientCertificateTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "https-client-certificate"; } }

        protected override Binding GetBinding()
        {
            var binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            return binding;
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
            this.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            this.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new MyX509CertificateValidator("DO_NOT_TRUST_WcfBridgeRootCA");
        }

        public HttpsClientCertificateTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}