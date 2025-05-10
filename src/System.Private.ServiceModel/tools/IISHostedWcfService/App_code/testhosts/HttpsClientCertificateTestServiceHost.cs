// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Security;
#else
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
#endif

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTPS, BasePath = "ClientCertificateAccepted/HttpsClientCertificate.svc")]
#if NET
    [TestServiceDefinition(Schema = ServiceSchema.WSS, BasePath = "ClientCertificateAccepted/HttpsClientCertificate.svc")]
#endif
    public class HttpsClientCertificateTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override IList<Binding> GetBindings()
        {
            return new List<Binding>
            {
                GetBasicHttpsBinding(),
                GetNetHttpsBindingWithClientCertAuth()
            };
        }

        private Binding GetBasicHttpsBinding()
        {
            var binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            binding.Name = "https-client-certificate";
            return binding;
        }

        private Binding GetNetHttpsBindingWithClientCertAuth()
        {
            NetHttpsBinding binding = new NetHttpsBinding(BasicHttpsSecurityMode.Transport)
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            binding.Name = "WebSocket-client-certificate";
            return binding;
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
            this.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            this.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new MyX509CertificateValidator("DO_NOT_TRUST_WcfBridgeRootCA");
        }

        public HttpsClientCertificateTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
