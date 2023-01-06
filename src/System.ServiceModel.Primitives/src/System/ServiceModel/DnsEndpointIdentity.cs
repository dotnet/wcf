// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Claims;
using System.Xml;

namespace System.ServiceModel
{
    public class DnsEndpointIdentity : EndpointIdentity
    {
        public DnsEndpointIdentity(string dnsName)
        {
            if (dnsName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(dnsName));
            }

            base.Initialize(Claim.CreateDnsClaim(dnsName));
        }

        public DnsEndpointIdentity(Claim identity)
        {
            if (identity == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(identity));
            }

            if (!identity.ClaimType.Equals(ClaimTypes.Dns))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.UnrecognizedClaimTypeForIdentity, identity.ClaimType, ClaimTypes.Dns));
            }

            base.Initialize(identity);
        }

        internal override void WriteContentsTo(XmlDictionaryWriter writer)
        {
            if (writer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(writer));
            }

            writer.WriteElementString(XD.AddressingDictionary.Dns, XD.AddressingDictionary.IdentityExtensionNamespace, (string)IdentityClaim.Resource);
        }
    }
}
