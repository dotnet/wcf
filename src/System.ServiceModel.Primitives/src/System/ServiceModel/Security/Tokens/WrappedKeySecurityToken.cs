// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace System.ServiceModel.Security.Tokens
{
    internal class WrappedKeySecurityToken : SecurityToken
    {
        private string _id;
        private DateTime _effectiveTime;
        private ReadOnlyCollection<SecurityKey> _securityKeys;
        private byte[] _wrappedKey;
        private string _wrappingAlgorithm;
        private SecurityKey _wrappingSecurityKey;
        private SecurityToken _wrappingToken;
        private KeyInfo _wrappingTokenReference;
        private object _encryptedKey;

        // direct receiver use, chained sender use
        internal WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, ISspiNegotiation wrappingSspiContext, byte[] wrappedKey)
            : this(id, keyToWrap, wrappingAlgorithm)
        {
            if (wrappingSspiContext == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(wrappingSspiContext));

            _wrappedKey = wrappedKey ?? wrappingSspiContext.Encrypt(keyToWrap);
        }

        // receiver use
        internal WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, SecurityToken wrappingToken, KeyInfo wrappingTokenReference, byte[] wrappedKey, SecurityKey wrappingSecurityKey)
            : this(id, keyToWrap, wrappingAlgorithm)
        {
            _wrappingToken = wrappingToken ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(wrappingToken));
            _wrappingTokenReference = wrappingTokenReference;
            _wrappedKey = wrappedKey ?? SecurityUtils.EncryptKey(wrappingToken, wrappingAlgorithm, keyToWrap);
            _wrappingSecurityKey = wrappingSecurityKey;
        }

        private WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm)
        {
            _id = id ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(id));
            _wrappingAlgorithm = wrappingAlgorithm ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(wrappingAlgorithm));
            if (keyToWrap == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(keyToWrap));

            _id = id;
            _effectiveTime = DateTime.UtcNow;
            _securityKeys = SecurityUtils.CreateSymmetricSecurityKeys(keyToWrap);
            _wrappingAlgorithm = wrappingAlgorithm;
        }

        public override string Id => _id;

        public override DateTime ValidFrom => _effectiveTime;

        public override DateTime ValidTo => DateTime.MaxValue;

        internal EncryptedKey EncryptedKey { get; set; }

        internal ReferenceList ReferenceList => EncryptedKey == null ? null : EncryptedKey.ReferenceList;

        public string WrappingAlgorithm => _wrappingAlgorithm;

        internal SecurityKey WrappingSecurityKey
        {
            get { return _wrappingSecurityKey; }
        }

        public SecurityToken WrappingToken
        {
            get { return _wrappingToken; }
        }

        public KeyInfo WrappingTokenReference
        {
            get { return _wrappingTokenReference; }
        }

        internal string CarriedKeyName
        {
            get { return null; }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys => _securityKeys;

        public byte[] GetWrappedKey()
        {
            return SecurityUtils.CloneBuffer(_wrappedKey);
        }

        internal void EnsureEncryptedKeySetUp()
        {
            if (_encryptedKey == null)
            {
                EncryptedKey ek = new EncryptedKey();
                ek.Id = this.Id;
                ek.CarriedKeyName = null;
                ek.EncryptionMethod = new EncryptionMethod(WrappingAlgorithm);
                ek.CipherData.CipherValue = _wrappedKey;
                if (WrappingTokenReference != null)
                {
                    ek.KeyInfo = WrappingTokenReference;
                }
                _encryptedKey = ek;
            }
        }

        public override bool CanCreateKeyIdentifierClause<T>()
        {
            // The type of clause supported is EncryptedKeyHashIdentifierClause, which hasn't been ported
            // so just defer to the base implementation for now.  We can add support for this clause in the future if needed.
            return base.CanCreateKeyIdentifierClause<T>();
        }

        public override T CreateKeyIdentifierClause<T>()
        {
            // The type of clause supported is EncryptedKeyHashIdentifierClause, which hasn't been ported
            // so just defer to the base implementation for now.  We can add support for this clause in the future if needed.
            return base.CreateKeyIdentifierClause<T>();
        }

        public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            // The type of clause supported is EncryptedKeyHashIdentifierClause, which hasn't been ported
            // so just defer to the base implementation for now.  We can add support for this clause in the future if needed.
            return base.MatchesKeyIdentifierClause(keyIdentifierClause);
        }
    }
}
