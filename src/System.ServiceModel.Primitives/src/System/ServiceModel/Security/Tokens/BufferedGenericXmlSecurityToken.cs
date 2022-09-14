// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.Xml;

namespace System.ServiceModel.Security.Tokens
{
    internal class BufferedGenericXmlSecurityToken : GenericXmlSecurityToken
    {
        public BufferedGenericXmlSecurityToken(
            XmlElement tokenXml,
            SecurityToken proofToken,
            DateTime effectiveTime,
            DateTime expirationTime,
            SecurityKeyIdentifierClause internalTokenReference,
            SecurityKeyIdentifierClause externalTokenReference,
            ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies,
            XmlBuffer tokenXmlBuffer
            )
            : base(tokenXml, proofToken, effectiveTime, expirationTime, internalTokenReference, externalTokenReference, authorizationPolicies)
        {
            TokenXmlBuffer = tokenXmlBuffer;
        }

        public XmlBuffer TokenXmlBuffer { get; }
    }
}
