// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WcfTestBridgeCommon;

namespace WcfService.CertificateResources
{
    // Retrieves a Certificate Authority certificate
    // GET to retrieve the CA cert
    internal class CertificateAuthorityResource : CertificateResource
    {
        public CertificateAuthorityResource() : base() { }

        public override ResourceResponse Get(ResourceRequestContext context)
        {
            X509Certificate2 certificate =
                CertificateResourceHelpers.GetCertificateGeneratorInstance(context.BridgeConfiguration).AuthorityCertificate.Certificate;

            string exportAsPemString = string.Empty;
            bool exportAsPem;

            ResourceResponse response = new ResourceResponse();

            if (context.Properties.TryGetValue(exportAsPemKeyName, out exportAsPemString) && bool.TryParse(exportAsPemString, out exportAsPem) && exportAsPem)
            {
                response.RawResponse = Encoding.ASCII.GetBytes(GetCertificateAsPem(certificate));
            }
            else
            {
                response.Properties.Add(thumbprintKeyName, certificate.Thumbprint);
                response.Properties.Add(certificateKeyName, Convert.ToBase64String(certificate.RawData));
            }

            return response;
        }

        // A bit of a misnomer - you can't really "put" a cert here, and Get will always return you the cert anyway 
        public override ResourceResponse Put(ResourceRequestContext context)
        {
            X509Certificate2 certificate =
                CertificateResourceHelpers.GetCertificateGeneratorInstance(context.BridgeConfiguration).AuthorityCertificate.Certificate;

            ResourceResponse response = new ResourceResponse();
            response.Properties.Add(thumbprintKeyName, certificate.Thumbprint);

            return response;
        }
    }
}
