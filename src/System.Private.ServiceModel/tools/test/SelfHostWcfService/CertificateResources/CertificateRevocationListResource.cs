// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using WcfTestBridgeCommon;

namespace WcfService.CertificateResources
{
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

            string thumbprint;

            lock(s_certificateResourceLock)
            {
                if (context.Properties.TryGetValue(revokeSerialNumberResourceString, out thumbprint) && !string.IsNullOrWhiteSpace(thumbprint))
                {
                    certGenerator.RevokeCertificateByThumbprint(thumbprint);
                }


                ResourceResponse response = new ResourceResponse();
                response.Properties.Add(crlUriResourceString, certGenerator.CrlUri);

                response.Properties.Add(
                    revokedCertificatesResourceString, 
                    string.Join<string>(",", certGenerator.RevokedCertificates));

                return response;
            }
        }
    }
}
