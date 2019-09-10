// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;

namespace System.IdentityModel.Tokens
{
    // All subclasses are required to be thread-safe and immutable

    // Self-resolving clauses such as RSA and X509 raw data should
    // override CanCreateKey and return true, and implement
    // CreateKey()

    public abstract class SecurityKeyIdentifierClause
    {
        private readonly string _clauseType;
        private byte[] _derivationNonce;
        private int _derivationLength;
        private string _id = null;

        protected SecurityKeyIdentifierClause(string clauseType)
            : this(clauseType, null, 0)
        {
        }

        protected SecurityKeyIdentifierClause(string clauseType, byte[] nonce, int length)
        {
            _clauseType = clauseType;
            _derivationNonce = nonce;
            _derivationLength = length;
        }

        public virtual bool CanCreateKey
        {
            get { return false; }
        }

        public string ClauseType
        {
            get { return _clauseType; }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual SecurityKey CreateKey()
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRServiceModel.KeyIdentifierClauseDoesNotSupportKeyCreation));
        }

        public virtual bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            return ReferenceEquals(this, keyIdentifierClause);
        }

        public byte[] GetDerivationNonce()
        {
            return (_derivationNonce != null) ? (byte[])_derivationNonce.Clone() : null;
        }

        public int DerivationLength
        {
            get { return _derivationLength; }
        }
    }
}
