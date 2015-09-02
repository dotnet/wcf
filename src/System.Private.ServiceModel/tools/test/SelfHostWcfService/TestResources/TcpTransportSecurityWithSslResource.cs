// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal class TcpTransportSecurityWithSslResource : TcpResource
    {
        protected override string Address { get { return "tcp-server-ssl-security"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(
                new SslStreamSecurityBindingElement(),
                new BinaryMessageEncodingBindingElement(),
                new TcpTransportBindingElement()
                );
        }

        protected override void ModifyHost(ServiceHost serviceHost, ResourceRequestContext context)
        {
            // Ensure the https certificate is installed before this endpoint resource is used
            CertificateManager.InstallMyCertificate(context.BridgeConfiguration,
                                                    context.BridgeConfiguration.BridgeHttpsCertificate);

            string certThumbprint = HttpsResource.EnsureHttpsCertificateInstalled(context.BridgeConfiguration);

            serviceHost.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine,
                                                      StoreName.My,
                                                      X509FindType.FindByThumbprint,
                                                      certThumbprint);

            serviceHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
        }
    }
}
