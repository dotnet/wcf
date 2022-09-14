// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IdentityModel.Policy;
using System.Xml;
using System.ServiceModel;

namespace System.IdentityModel.Tokens
{
    public class GenericXmlSecurityToken : SecurityToken
    {
        private const int SupportedPersistanceVersion = 1;
        private string _id;
        private DateTime _effectiveTime;
        private DateTime _expirationTime;

        public GenericXmlSecurityToken(
            XmlElement tokenXml,
            SecurityToken proofToken,
            DateTime effectiveTime,
            DateTime expirationTime,
            SecurityKeyIdentifierClause internalTokenReference,
            SecurityKeyIdentifierClause externalTokenReference,
            ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies
            )
        {
            if (tokenXml == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenXml));
            }

            _id = GetId(tokenXml);
            TokenXml = tokenXml;
            ProofToken = proofToken;
            _effectiveTime = effectiveTime.ToUniversalTime();
            _expirationTime = expirationTime.ToUniversalTime();

            InternalTokenReference = internalTokenReference;
            ExternalTokenReference = externalTokenReference;
            AuthorizationPolicies = authorizationPolicies ?? EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance;
        }

        public override string Id
        {
            get { return _id; }
        }

        public override DateTime ValidFrom
        {
            get { return _effectiveTime; }
        }

        public override DateTime ValidTo
        {
            get { return _expirationTime; }
        }

        public SecurityKeyIdentifierClause InternalTokenReference { get; }

        public SecurityKeyIdentifierClause ExternalTokenReference { get; }

        public XmlElement TokenXml { get; }

        public SecurityToken ProofToken { get; }

        public ReadOnlyCollection<IAuthorizationPolicy> AuthorizationPolicies { get; }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                if (ProofToken != null)
                {
                    return ProofToken.SecurityKeys;
                }
                else
                {
                    return EmptyReadOnlyCollection<SecurityKey>.Instance;
                }
            }
        }

        public override string ToString()
        {
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            writer.WriteLine("Generic XML token:");
            writer.WriteLine("   validFrom: {0}", ValidFrom);
            writer.WriteLine("   validTo: {0}", ValidTo);
            if (InternalTokenReference != null)
            {
                writer.WriteLine("   InternalTokenReference: {0}", InternalTokenReference);
            }

            if (ExternalTokenReference != null)
            {
                writer.WriteLine("   ExternalTokenReference: {0}", ExternalTokenReference);
            }

            writer.WriteLine("   Token Element: ({0}, {1})", TokenXml.LocalName, TokenXml.NamespaceURI);
            return writer.ToString();
        }

        private static string GetId(XmlElement tokenXml)
        {
            if (tokenXml != null)
            {
                string id = tokenXml.GetAttribute(UtilityStrings.IdAttribute, UtilityStrings.Namespace);
                if (string.IsNullOrEmpty(id))
                {
                    // special case SAML 1.1 as this is the only possible ID as
                    // spec is closed.  SAML 2.0 is xs:ID
                    id = tokenXml.GetAttribute("AssertionID");

                    // if we are still null, "Id"
                    if (string.IsNullOrEmpty(id))
                    {
                        id = tokenXml.GetAttribute("Id");
                    }

                    //This fixes the unencrypted SAML 2.0 case. Eg: <Assertion ID="_05955298-214f-41e7-b4c3-84dbff7f01b9" 
                    if (string.IsNullOrEmpty(id))
                    {
                        id = tokenXml.GetAttribute("ID");
                    }
                }

                if (!string.IsNullOrEmpty(id))
                {
                    return id;
                }
            }

            return null;
        }

        public override bool CanCreateKeyIdentifierClause<T>()
        {
            if (InternalTokenReference != null && typeof(T) == InternalTokenReference.GetType())
            {
                return true;
            }

            if (ExternalTokenReference != null && typeof(T) == ExternalTokenReference.GetType())
            {
                return true;
            }

            return false;
        }

        public override T CreateKeyIdentifierClause<T>()
        {
            if (InternalTokenReference != null && typeof(T) == InternalTokenReference.GetType())
            {
                return (T)InternalTokenReference;
            }

            if (ExternalTokenReference != null && typeof(T) == ExternalTokenReference.GetType())
            {
                return (T)ExternalTokenReference;
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SRP.UnableToCreateTokenReference));
        }

        public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (InternalTokenReference != null && InternalTokenReference.Matches(keyIdentifierClause))
            {
                return true;
            }
            else if (ExternalTokenReference != null && ExternalTokenReference.Matches(keyIdentifierClause))
            {
                return true;
            }

            return false;
        }
    }
}
