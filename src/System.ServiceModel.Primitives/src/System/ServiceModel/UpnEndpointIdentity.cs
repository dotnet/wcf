// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Principal;
using System.IdentityModel.Claims;
using System.Runtime;
using System.ServiceModel.Diagnostics;
using System.Xml;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System.ServiceModel
{
    public class UpnEndpointIdentity : EndpointIdentity
    {
        public UpnEndpointIdentity(string upnName)
        {
            if (upnName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(upnName));
            }

            Initialize(Claim.CreateUpnClaim(upnName));
        }

        internal UpnEndpointIdentity(Claim identity)
        {
            if (identity == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(identity));
            }

            if (!identity.ClaimType.Equals(ClaimTypes.Upn))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.UnrecognizedClaimTypeForIdentity, identity.ClaimType, ClaimTypes.Upn));
            }

            Initialize(identity);
        }

        internal override void WriteContentsTo(XmlDictionaryWriter writer)
        {
            if (writer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(writer));
            }

            writer.WriteElementString(XD.AddressingDictionary.Upn, XD.AddressingDictionary.IdentityExtensionNamespace, (string)IdentityClaim.Resource);
        }
    }
}
