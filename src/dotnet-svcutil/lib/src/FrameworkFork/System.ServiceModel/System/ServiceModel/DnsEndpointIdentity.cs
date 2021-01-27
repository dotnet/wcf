// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using Microsoft.Xml;
using Microsoft.Xml.Serialization;

namespace System.ServiceModel
{
    public class DnsEndpointIdentity : EndpointIdentity
    {
        public DnsEndpointIdentity(string dnsName)
        {
            if (dnsName == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("dnsName");

            base.Initialize(Claim.CreateDnsClaim(dnsName));
        }

        public DnsEndpointIdentity(Claim identity)
        {
            if (identity == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("identity");

            if (!identity.ClaimType.Equals(ClaimTypes.Dns))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(string.Format(SRServiceModel.UnrecognizedClaimTypeForIdentity, identity.ClaimType, ClaimTypes.Dns));

            base.Initialize(identity);
        }

        internal override void WriteContentsTo(XmlDictionaryWriter writer)
        {
            if (writer == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");

            writer.WriteElementString(XD.AddressingDictionary.Dns, XD.AddressingDictionary.IdentityExtensionNamespace, (string)this.IdentityClaim.Resource);
        }
    }
}
