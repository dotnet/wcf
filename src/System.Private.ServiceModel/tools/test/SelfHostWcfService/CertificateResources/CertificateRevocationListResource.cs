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

            ResourceResponse response = new ResourceResponse();
            response.RawResponse = certGenerator.CrlEncoded;

            return response;
        }

        public override ResourceResponse Put(ResourceRequestContext context)
        {
            var certGenerator = CertificateResourceHelpers.GetCertificateGeneratorInstance(context.BridgeConfiguration);

            ResourceResponse response = new ResourceResponse();
            response.Properties.Add("crlUri", certGenerator.CrlUri);

            return response; 
        }
    }
}
