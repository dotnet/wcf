// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Xml;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace System.IdentityModel.Tokens
{
    public class GenericXmlSecurityToken : SecurityToken
    {
        const int SupportedPersistanceVersion = 1;
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
                throw ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenXml");
            }

            this._id = GetId(tokenXml);
            this._tokenXml = tokenXml;
            this._proofToken = proofToken;
            this._effectiveTime = effectiveTime.ToUniversalTime();
            this._expirationTime = expirationTime.ToUniversalTime();

            this._internalTokenReference = internalTokenReference;
            this._externalTokenReference = externalTokenReference;
            this._authorizationPolicies = authorizationPolicies ?? EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance;
        }

        public override string Id
        {
            get { return this._id; }
        }

        public override DateTime ValidFrom
        {
            get { return this._effectiveTime; }
        }

        public override DateTime ValidTo
        {
            get { return this._expirationTime; }
        }

        public SecurityKeyIdentifierClause InternalTokenReference
        {
            get { return this._internalTokenReference; }
        }

        public SecurityKeyIdentifierClause ExternalTokenReference
        {
            get { return this._externalTokenReference; }
        }

        public XmlElement TokenXml
        {
            get { return this._tokenXml;  }
        }

        public SecurityToken ProofToken
        {
            get { return this._proofToken; }
        }

        public ReadOnlyCollection<IAuthorizationPolicy> AuthorizationPolicies
        {
            get { return this._authorizationPolicies; }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get 
            {
                if (this._proofToken != null)
                    return this._proofToken.SecurityKeys;
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
            if (this._internalTokenReference != null)
                writer.WriteLine("   InternalTokenReference: {0}", this._internalTokenReference);
            if (this._externalTokenReference != null)
                writer.WriteLine("   ExternalTokenReference: {0}", this._externalTokenReference);
            writer.WriteLine("   Token Element: ({0}, {1})", this._tokenXml.LocalName, this._tokenXml.NamespaceURI);
            return writer.ToString();
        }

        static string GetId(XmlElement tokenXml)
        {
            if (tokenXml != null)
            {
                string id = tokenXml.GetAttribute(UtilityStrings.IdAttribute, UtilityStrings.Namespace);
                if ( string.IsNullOrEmpty( id ) )
                {
                    // special case SAML 1.1 as this is the only possible ID as
                    // spec is closed.  SAML 2.0 is xs:ID
                    id = tokenXml.GetAttribute("AssertionID");

                    // if we are still null, "Id"
                    if ( string.IsNullOrEmpty( id ) )
                    {
                        id = tokenXml.GetAttribute("Id");
                    }

                    //This fixes the unecnrypted SAML 2.0 case. Eg: <Assertion ID="_05955298-214f-41e7-b4c3-84dbff7f01b9" 
                    if (string.IsNullOrEmpty(id))
                    {
                        id = tokenXml.GetAttribute("ID");
                    }
                }

                if ( !string.IsNullOrEmpty(id) )
                {
                    return id;
                }
            }

            return null;
        }

        public override bool CanCreateKeyIdentifierClause<T>()
        {
            if (this._internalTokenReference != null && typeof(T) == this._internalTokenReference.GetType())
                return true;

            if (this._externalTokenReference != null && typeof(T) == this._externalTokenReference.GetType())
                return true;

            return false;
        }

        public override T CreateKeyIdentifierClause<T>()
        {
            if (this._internalTokenReference != null && typeof(T) == this._internalTokenReference.GetType())
                return (T)this._internalTokenReference;

            if (this._externalTokenReference != null && typeof(T) == this._externalTokenReference.GetType())
                return (T)this._externalTokenReference;

            throw ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.UnableToCreateTokenReference)));
        }

        public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (this._internalTokenReference != null && this._internalTokenReference.Matches(keyIdentifierClause))
            {
                return true;
            }
            else if (this._externalTokenReference != null && this._externalTokenReference.Matches(keyIdentifierClause))
            {
                return true;
            }
            
            return false;
        }
    }
}

