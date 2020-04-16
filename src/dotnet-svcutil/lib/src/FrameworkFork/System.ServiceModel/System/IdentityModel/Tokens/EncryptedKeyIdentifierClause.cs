// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.ServiceModel;

namespace System.IdentityModel.Tokens
{
    sealed public class EncryptedKeyIdentifierClause : BinaryKeyIdentifierClause
    {
        private readonly string _carriedKeyName;
        private readonly string _encryptionMethod;
        private readonly SecurityKeyIdentifier _encryptingKeyIdentifier;

        public EncryptedKeyIdentifierClause(byte[] encryptedKey, string encryptionMethod)
            : this(encryptedKey, encryptionMethod, null)
        {
        }

        public EncryptedKeyIdentifierClause(byte[] encryptedKey, string encryptionMethod, SecurityKeyIdentifier encryptingKeyIdentifier)
            : this(encryptedKey, encryptionMethod, encryptingKeyIdentifier, null)
        {
        }

        public EncryptedKeyIdentifierClause(byte[] encryptedKey, string encryptionMethod, SecurityKeyIdentifier encryptingKeyIdentifier, string carriedKeyName)
            : this(encryptedKey, encryptionMethod, encryptingKeyIdentifier, carriedKeyName, true, null, 0)
        {
        }

        public EncryptedKeyIdentifierClause(byte[] encryptedKey, string encryptionMethod, SecurityKeyIdentifier encryptingKeyIdentifier, string carriedKeyName, byte[] derivationNonce, int derivationLength)
            : this(encryptedKey, encryptionMethod, encryptingKeyIdentifier, carriedKeyName, true, derivationNonce, derivationLength)
        {
        }

        internal EncryptedKeyIdentifierClause(byte[] encryptedKey, string encryptionMethod, SecurityKeyIdentifier encryptingKeyIdentifier, string carriedKeyName, bool cloneBuffer, byte[] derivationNonce, int derivationLength)
            : base("http://www.w3.org/2001/04/xmlenc#EncryptedKey", encryptedKey, cloneBuffer, derivationNonce, derivationLength)
        {
            if (encryptionMethod == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("encryptionMethod");
            }
            _carriedKeyName = carriedKeyName;
            _encryptionMethod = encryptionMethod;
            _encryptingKeyIdentifier = encryptingKeyIdentifier;
        }

        public string CarriedKeyName
        {
            get { return _carriedKeyName; }
        }

        public SecurityKeyIdentifier EncryptingKeyIdentifier
        {
            get { return _encryptingKeyIdentifier; }
        }

        public string EncryptionMethod
        {
            get { return _encryptionMethod; }
        }

        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            EncryptedKeyIdentifierClause that = keyIdentifierClause as EncryptedKeyIdentifierClause;
            return ReferenceEquals(this, that) || (that != null && that.Matches(this.GetRawBuffer(), _encryptionMethod, _carriedKeyName));
        }

        public bool Matches(byte[] encryptedKey, string encryptionMethod, string carriedKeyName)
        {
            return Matches(encryptedKey) && _encryptionMethod == encryptionMethod && _carriedKeyName == carriedKeyName;
        }

        public byte[] GetEncryptedKey()
        {
            return GetBuffer();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "EncryptedKeyIdentifierClause(EncryptedKey = {0}, Method '{1}')",
                Convert.ToBase64String(GetRawBuffer()), this.EncryptionMethod);
        }
    }
}
