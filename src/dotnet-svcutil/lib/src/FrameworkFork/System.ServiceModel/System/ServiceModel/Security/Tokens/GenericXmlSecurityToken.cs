// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IdentityModel.Tokens
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.IdentityModel.Claims;
    using System.IdentityModel.Policy;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using Microsoft.Xml;
    using System.Runtime.Serialization;
    using System.Collections.Generic;

    public class GenericXmlSecurityToken : SecurityToken
    {
        private const int SupportedPersistanceVersion = 1;
        private string _id;
        private SecurityToken _proofToken;
        private SecurityKeyIdentifierClause _internalTokenReference;
        private SecurityKeyIdentifierClause _externalTokenReference;
        private XmlElement _tokenXml;
        private ReadOnlyCollection<IAuthorizationPolicy> _authorizationPolicies;
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
                throw new ArgumentNullException("tokenXml"); // TODO: DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenXml");
            }

            _id = GetId(tokenXml);
            _tokenXml = tokenXml;
            _proofToken = proofToken;
            _effectiveTime = effectiveTime.ToUniversalTime();
            _expirationTime = expirationTime.ToUniversalTime();

            _internalTokenReference = internalTokenReference;
            _externalTokenReference = externalTokenReference;
            _authorizationPolicies = authorizationPolicies ?? EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance;
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

        public SecurityKeyIdentifierClause InternalTokenReference
        {
            get { return _internalTokenReference; }
        }

        public SecurityKeyIdentifierClause ExternalTokenReference
        {
            get { return _externalTokenReference; }
        }

        public XmlElement TokenXml
        {
            get { return _tokenXml; }
        }

        public SecurityToken ProofToken
        {
            get { return _proofToken; }
        }

        public ReadOnlyCollection<IAuthorizationPolicy> AuthorizationPolicies
        {
            get { return _authorizationPolicies; }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                if (_proofToken != null)
                    return _proofToken.SecurityKeys;
                else
                    return EmptyReadOnlyCollection<SecurityKey>.Instance;
            }
        }

        public override string ToString()
        {
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            writer.WriteLine("Generic XML token:");
            writer.WriteLine("   validFrom: {0}", this.ValidFrom);
            writer.WriteLine("   validTo: {0}", this.ValidTo);
            if (_internalTokenReference != null)
                writer.WriteLine("   InternalTokenReference: {0}", _internalTokenReference);
            if (_externalTokenReference != null)
                writer.WriteLine("   ExternalTokenReference: {0}", _externalTokenReference);
            writer.WriteLine("   Token Element: ({0}, {1})", _tokenXml.LocalName, _tokenXml.NamespaceURI);
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

                    //This fixes the unecnrypted SAML 2.0 case. Eg: <Assertion ID="_05955298-214f-41e7-b4c3-84dbff7f01b9" 
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

#if desabled
        public override bool CanCreateKeyIdentifierClause<T>()
        {
            if (this.internalTokenReference != null && typeof(T) == this.internalTokenReference.GetType())
                return true;

            if (this.externalTokenReference != null && typeof(T) == this.externalTokenReference.GetType())
                return true;

            return false;
        }

        public override T CreateKeyIdentifierClause<T>()
        {
            if (this.internalTokenReference != null && typeof(T) == this.internalTokenReference.GetType())
                return (T)this.internalTokenReference;

            if (this.externalTokenReference != null && typeof(T) == this.externalTokenReference.GetType())
                return (T)this.externalTokenReference;

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.UnableToCreateTokenReference));
        }

        public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (this.internalTokenReference != null && this.internalTokenReference.Matches(keyIdentifierClause))
            {
                return true;
            }
            else if (this.externalTokenReference != null && this.externalTokenReference.Matches(keyIdentifierClause))
            {
                return true;
            }

            return false;
        }
#endif
    }
}
