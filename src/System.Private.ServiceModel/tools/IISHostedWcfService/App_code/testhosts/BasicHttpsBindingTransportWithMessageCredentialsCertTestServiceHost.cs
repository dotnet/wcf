// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Security;
#else
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
#endif

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTPS, BasePath = "BasicHttpsBindingTransSecMessCredsCert.svc")]
    internal class BasicHttpsBindingTransportWithMessageCredentialsCertTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "https-transwithmessage-credentials-cert"; } }

        protected override Binding GetBinding()
        {
            BasicHttpsBinding binding = new BasicHttpsBinding(BasicHttpsSecurityMode.TransportWithMessageCredential);
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.Certificate;
            return binding;
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
            Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new MyX509CertificateValidator("DO_NOT_TRUST_WcfBridgeRootCA");
        }

        public BasicHttpsBindingTransportWithMessageCredentialsCertTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
