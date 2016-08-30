// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.ServiceModel.Security;
using System.Xml;

namespace System.ServiceModel.Security.Tokens
{
    public class WrappedKeySecurityToken : SecurityToken
    {
        private string _id;
        private DateTime _effectiveTime;

        private EncryptedKey _encryptedKey;
        private ReadOnlyCollection<SecurityKey> _securityKey;
        private byte[] _wrappedKey;
        private string _wrappingAlgorithm;
        private ISspiNegotiation _wrappingSspiContext;
        private SecurityToken _wrappingToken;
        private SecurityKey _wrappingSecurityKey;
        private SecurityKeyIdentifier _wrappingTokenReference;
        private bool _serializeCarriedKeyName;
        // byte[] wrappedKeyHash;
        private XmlDictionaryString _wrappingAlgorithmDictionaryString;

        // sender use
        internal WrappedKeySecurityToken(string id, byte[] keyToWrap, ISspiNegotiation wrappingSspiContext)
            : this(id, keyToWrap, (wrappingSspiContext != null) ? (wrappingSspiContext.KeyEncryptionAlgorithm) : null, wrappingSspiContext, null)
        {
        }

        // sender use
        public WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, SecurityToken wrappingToken, SecurityKeyIdentifier wrappingTokenReference)
            : this(id, keyToWrap, wrappingAlgorithm, null, wrappingToken, wrappingTokenReference)
        {
        }

        internal WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, XmlDictionaryString wrappingAlgorithmDictionaryString, SecurityToken wrappingToken, SecurityKeyIdentifier wrappingTokenReference)
            : this(id, keyToWrap, wrappingAlgorithm, wrappingAlgorithmDictionaryString, wrappingToken, wrappingTokenReference, null, null)
        {
        }

        // direct receiver use, chained sender use
        internal WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, ISspiNegotiation wrappingSspiContext, byte[] wrappedKey)
            : this(id, keyToWrap, wrappingAlgorithm, null)
        {
            if (wrappingSspiContext == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("wrappingSspiContext");
            }
            this._wrappingSspiContext = wrappingSspiContext;
            if (wrappedKey == null)
            {
                this._wrappedKey = wrappingSspiContext.Encrypt(keyToWrap);
            }
            else
            {
                this._wrappedKey = wrappedKey;
            }
            this._serializeCarriedKeyName = false;
        }

        // receiver use
        internal WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, SecurityToken wrappingToken, SecurityKeyIdentifier wrappingTokenReference, byte[] wrappedKey, SecurityKey wrappingSecurityKey)
            : this(id, keyToWrap, wrappingAlgorithm, null, wrappingToken, wrappingTokenReference, wrappedKey, wrappingSecurityKey)
        {
        }

        WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, XmlDictionaryString wrappingAlgorithmDictionaryString, SecurityToken wrappingToken, SecurityKeyIdentifier wrappingTokenReference, byte[] wrappedKey, SecurityKey wrappingSecurityKey)
            : this(id, keyToWrap, wrappingAlgorithm, wrappingAlgorithmDictionaryString)
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //if (wrappingToken == null)
            //{
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("wrappingToken");
            //}
            //this.wrappingToken = wrappingToken;
            //this.wrappingTokenReference = wrappingTokenReference;
            //if (wrappedKey == null)
            //{
            //    this.wrappedKey = SecurityUtils.EncryptKey(wrappingToken, wrappingAlgorithm, keyToWrap);
            //}
            //else
            //{
            //    this.wrappedKey = wrappedKey;
            //}
            //this.wrappingSecurityKey = wrappingSecurityKey;
            //this.serializeCarriedKeyName = true;
        }

        WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, XmlDictionaryString wrappingAlgorithmDictionaryString)
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //if (id == null)
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("id");
            //if (wrappingAlgorithm == null)
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("wrappingAlgorithm");
            //if (keyToWrap == null)
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("securityKeyToWrap");

            //this.id = id;
            //this.effectiveTime = DateTime.UtcNow;
            //this.securityKey = SecurityUtils.CreateSymmetricSecurityKeys(keyToWrap);
            //this.wrappingAlgorithm = wrappingAlgorithm;
            //this.wrappingAlgorithmDictionaryString = wrappingAlgorithmDictionaryString;
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
            // Never expire
            get { return DateTime.MaxValue; }
        }

        internal EncryptedKey EncryptedKey
        {
            get { return this._encryptedKey; }
            set { this._encryptedKey = value; }
        }

        internal ReferenceList ReferenceList
        {
            get
            {
                return this._encryptedKey == null ? null : this._encryptedKey.ReferenceList;
            }
        }

        public string WrappingAlgorithm
        {
            get { return this._wrappingAlgorithm; }
        }

        internal SecurityKey WrappingSecurityKey
        {
            get { return this._wrappingSecurityKey; }
        }

        public SecurityToken WrappingToken
        {
            get { return this._wrappingToken; }
        }

        public SecurityKeyIdentifier WrappingTokenReference
        {
            get { return this._wrappingTokenReference; }
        }

        internal string CarriedKeyName
        {
            get { return null; }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { return this._securityKey; }
        }

        internal byte[] GetHash()
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
            //if (this.wrappedKeyHash == null)
            //{
            //    EnsureEncryptedKeySetUp();
            //    using (HashAlgorithm hash = CryptoHelper.NewSha1HashAlgorithm())
            //    {
            //        this.wrappedKeyHash = hash.ComputeHash(this.encryptedKey.GetWrappedKey());
            //    }
            //}
            //return wrappedKeyHash;
        }

        public byte[] GetWrappedKey()
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
            //return SecurityUtils.CloneBuffer(this.wrappedKey);
        }

        internal void EnsureEncryptedKeySetUp()
        {
            if (this._encryptedKey == null)
            {
                EncryptedKey ek = new EncryptedKey();
                ek.Id = this.Id;
                if (this._serializeCarriedKeyName)
                {
                    ek.CarriedKeyName = this.CarriedKeyName;
                }
                else
                {
                    ek.CarriedKeyName = null;
                }
                ek.EncryptionMethod = this.WrappingAlgorithm;
                ek.EncryptionMethodDictionaryString = this._wrappingAlgorithmDictionaryString;
                ek.SetUpKeyWrap(this._wrappedKey);
                if (this.WrappingTokenReference != null)
                {
                    ek.KeyIdentifier = this.WrappingTokenReference;
                }
                this._encryptedKey = ek;
            }
        }

        public override bool CanCreateKeyIdentifierClause<T>()
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //if (typeof(T) == typeof(EncryptedKeyHashIdentifierClause))
            //    return true;

            //return base.CanCreateKeyIdentifierClause<T>();
        }

        public override T CreateKeyIdentifierClause<T>()
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //if (typeof(T) == typeof(EncryptedKeyHashIdentifierClause))
            //    return new EncryptedKeyHashIdentifierClause(GetHash()) as T;

            //return base.CreateKeyIdentifierClause<T>();
        }

        public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //EncryptedKeyHashIdentifierClause encKeyIdentifierClause = keyIdentifierClause as EncryptedKeyHashIdentifierClause;
            //if (encKeyIdentifierClause != null)
            //    return encKeyIdentifierClause.Matches(GetHash());

            //return base.MatchesKeyIdentifierClause(keyIdentifierClause);
        }
    }
}

