// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using WcfTestBridgeCommon;

namespace WcfService.CertificateResources
{
    // Resource for generating Certificate Revocation Lists
    // PUT with a serial number to revoke a certificate
    // GET to retrieve the CRL
    public class CertificateRevocationListResource : CertificateResource
    {
        public CertificateRevocationListResource() : base() { }

        public override ResourceResponse Get(ResourceRequestContext context)
        {
            var certGenerator = CertificateResourceHelpers.GetCertificateGeneratorInstance(context.BridgeConfiguration); 

            lock (s_certificateResourceLock)
            {
                ResourceResponse response = new ResourceResponse();
                response.RawResponse = certGenerator.CrlEncoded;
                return response;
            }
        }

        public override ResourceResponse Put(ResourceRequestContext context)
        {
            var certGenerator = CertificateResourceHelpers.GetCertificateGeneratorInstance(context.BridgeConfiguration);

            string serialNumber;

            lock(s_certificateResourceLock)
            {
                if (context.Properties.TryGetValue(revokeSerialNumberKeyName, out serialNumber) && !string.IsNullOrWhiteSpace(serialNumber))
                {
                    certGenerator.RevokeCertificateBySerialNumber(serialNumber);
                }

                ResourceResponse response = new ResourceResponse();
                response.Properties.Add(crlUriKeyName, certGenerator.CrlUri);

                response.Properties.Add(
                    revokedCertificatesKeyName, 
                    string.Join<string>(",", certGenerator.RevokedCertificates));

                return response;
            }
        }
    }
}
