// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using WcfService.CertificateResources;
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
            string thumbprint = CertificateResourceHelpers.EnsureSslPortCertificateInstalled(context.BridgeConfiguration);

            serviceHost.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine,
                                                      StoreName.My,
                                                      X509FindType.FindByThumbprint,
                                                      thumbprint);

            serviceHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
        }
    }
}
