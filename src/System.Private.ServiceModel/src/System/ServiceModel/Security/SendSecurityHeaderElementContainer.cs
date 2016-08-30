// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Claims;
using System.ServiceModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using System.Xml;
using System.Collections.Generic;

using ISecurityElement = System.IdentityModel.ISecurityElement;

namespace System.ServiceModel.Security
{
    class SendSecurityHeaderElementContainer
    {
        private List<SecurityToken> _signedSupportingTokens = null;
        private List<SendSecurityHeaderElement> _basicSupportingTokens = null;
        private List<SecurityToken> _endorsingSupportingTokens = null;
        private List<SecurityToken> _endorsingDerivedSupportingTokens = null;
        private List<SecurityToken> _signedEndorsingSupportingTokens = null;
        private List<SecurityToken> _signedEndorsingDerivedSupportingTokens = null;
        private List<SendSecurityHeaderElement> _signatureConfirmations = null;
        private List<SendSecurityHeaderElement> _endorsingSignatures = null;
        private Dictionary<SecurityToken, SecurityKeyIdentifierClause> _securityTokenMappedToIdentifierClause = null;

        public SecurityTimestamp Timestamp;
        public SecurityToken PrerequisiteToken;
        public SecurityToken SourceSigningToken;
        public SecurityToken DerivedSigningToken;
        public SecurityToken SourceEncryptionToken;
        public SecurityToken WrappedEncryptionToken;
        public SecurityToken DerivedEncryptionToken;
        public ISecurityElement ReferenceList;
        public SendSecurityHeaderElement PrimarySignature;

        void Add<T>(ref List<T> list, T item)
        {
            if (list == null)
            {
                list = new List<T>();
            }
            list.Add(item);
        }

        public SecurityToken[] GetSignedSupportingTokens()
        {
            return (this._signedSupportingTokens != null) ? this._signedSupportingTokens.ToArray() : null;
        }

        public void AddSignedSupportingToken(SecurityToken token)
        {
            Add<SecurityToken>(ref this._signedSupportingTokens, token);
        }

        public List<SecurityToken> EndorsingSupportingTokens
        {
            get { return this._endorsingSupportingTokens; }
        }

        public SendSecurityHeaderElement[] GetBasicSupportingTokens()
        {
            return (this._basicSupportingTokens != null) ? this._basicSupportingTokens.ToArray() : null;
        }

        public void AddBasicSupportingToken(SendSecurityHeaderElement tokenElement)
        {
            Add<SendSecurityHeaderElement>(ref this._basicSupportingTokens, tokenElement);
        }

        public SecurityToken[] GetSignedEndorsingSupportingTokens()
        {
            return (this._signedEndorsingSupportingTokens != null) ? this._signedEndorsingSupportingTokens.ToArray() : null;
        }

        public void AddSignedEndorsingSupportingToken(SecurityToken token)
        {
            Add<SecurityToken>(ref this._signedEndorsingSupportingTokens, token);
        }

        public SecurityToken[] GetSignedEndorsingDerivedSupportingTokens()
        {
            return (this._signedEndorsingDerivedSupportingTokens != null) ? this._signedEndorsingDerivedSupportingTokens.ToArray() : null;
        }

        public void AddSignedEndorsingDerivedSupportingToken(SecurityToken token)
        {
            Add<SecurityToken>(ref this._signedEndorsingDerivedSupportingTokens, token);
        }

        public SecurityToken[] GetEndorsingSupportingTokens()
        {
            return (this._endorsingSupportingTokens != null) ? this._endorsingSupportingTokens.ToArray() : null;
        }

        public void AddEndorsingSupportingToken(SecurityToken token)
        {
            Add<SecurityToken>(ref this._endorsingSupportingTokens, token);
        }

        public SecurityToken[] GetEndorsingDerivedSupportingTokens()
        {
            return (this._endorsingDerivedSupportingTokens != null) ? this._endorsingDerivedSupportingTokens.ToArray() : null;
        }

        public void AddEndorsingDerivedSupportingToken(SecurityToken token)
        {
            Add<SecurityToken>(ref this._endorsingDerivedSupportingTokens, token);
        }

        public SendSecurityHeaderElement[] GetSignatureConfirmations()
        {
            return (this._signatureConfirmations != null) ? this._signatureConfirmations.ToArray() : null;
        }

        public void AddSignatureConfirmation(SendSecurityHeaderElement confirmation)
        {
            Add<SendSecurityHeaderElement>(ref this._signatureConfirmations, confirmation);
        }

        public SendSecurityHeaderElement[] GetEndorsingSignatures()
        {
            return (this._endorsingSignatures != null) ? this._endorsingSignatures.ToArray() : null;
        }

        public void AddEndorsingSignature(SendSecurityHeaderElement signature)
        {
            Add<SendSecurityHeaderElement>(ref this._endorsingSignatures, signature);
        }

        public void MapSecurityTokenToStrClause(SecurityToken securityToken, SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (this._securityTokenMappedToIdentifierClause == null)
            {
                this._securityTokenMappedToIdentifierClause = new Dictionary<SecurityToken, SecurityKeyIdentifierClause>();
            }

            if (!this._securityTokenMappedToIdentifierClause.ContainsKey(securityToken))
            {
                this._securityTokenMappedToIdentifierClause.Add(securityToken, keyIdentifierClause);
            }
        }

        public bool TryGetIdentifierClauseFromSecurityToken(SecurityToken securityToken, out SecurityKeyIdentifierClause keyIdentifierClause)
        {
            keyIdentifierClause = null;
            if (securityToken == null
                || this._securityTokenMappedToIdentifierClause == null
                || !this._securityTokenMappedToIdentifierClause.TryGetValue(securityToken, out keyIdentifierClause))
            {
                return false;
            }
            return true;
        }       
    }
}

