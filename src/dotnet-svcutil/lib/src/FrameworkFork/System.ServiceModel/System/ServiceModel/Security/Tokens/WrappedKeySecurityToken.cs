// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security.Tokens
{
    using System.Collections.ObjectModel;
    using System.IdentityModel;
    using System.IdentityModel.Tokens;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.ServiceModel.Security;
    using Microsoft.Xml;

    [TypeForwardedFrom("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
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
        private byte[] _wrappedKeyHash;
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
            _wrappingSspiContext = wrappingSspiContext;
            if (wrappedKey == null)
            {
                _wrappedKey = wrappingSspiContext.Encrypt(keyToWrap);
            }
            else
            {
                _wrappedKey = wrappedKey;
            }
            _serializeCarriedKeyName = false;
        }

        // receiver use
        internal WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, SecurityToken wrappingToken, SecurityKeyIdentifier wrappingTokenReference, byte[] wrappedKey, SecurityKey wrappingSecurityKey)
            : this(id, keyToWrap, wrappingAlgorithm, null, wrappingToken, wrappingTokenReference, wrappedKey, wrappingSecurityKey)
        {
        }

        private WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, XmlDictionaryString wrappingAlgorithmDictionaryString, SecurityToken wrappingToken, SecurityKeyIdentifier wrappingTokenReference, byte[] wrappedKey, SecurityKey wrappingSecurityKey)
            : this(id, keyToWrap, wrappingAlgorithm, wrappingAlgorithmDictionaryString)
        {
            throw new NotImplementedException();
        }

        private WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, XmlDictionaryString wrappingAlgorithmDictionaryString)
        {
            if (id == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("id");
            if (wrappingAlgorithm == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("wrappingAlgorithm");
            if (keyToWrap == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("securityKeyToWrap");

            _id = id;
            _effectiveTime = DateTime.UtcNow;
            _securityKey = System.IdentityModel.SecurityUtils.CreateSymmetricSecurityKeys(keyToWrap);
            _wrappingAlgorithm = wrappingAlgorithm;
            _wrappingAlgorithmDictionaryString = wrappingAlgorithmDictionaryString;
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
            // Never expire
            get { return DateTime.MaxValue; }
        }

        internal EncryptedKey EncryptedKey
        {
            get { return _encryptedKey; }
            set { _encryptedKey = value; }
        }

        internal ReferenceList ReferenceList
        {
            get
            {
                return _encryptedKey == null ? null : _encryptedKey.ReferenceList;
            }
        }

        public string WrappingAlgorithm
        {
            get { return _wrappingAlgorithm; }
        }

        internal SecurityKey WrappingSecurityKey
        {
            get { return _wrappingSecurityKey; }
        }

        public SecurityToken WrappingToken
        {
            get { return _wrappingToken; }
        }

        public SecurityKeyIdentifier WrappingTokenReference
        {
            get { return _wrappingTokenReference; }
        }

        internal string CarriedKeyName
        {
            get { return null; }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { return _securityKey; }
        }

        internal void EnsureEncryptedKeySetUp()
        {
            if (_encryptedKey == null)
            {
                EncryptedKey ek = new EncryptedKey();
                ek.Id = this.Id;
                if (_serializeCarriedKeyName)
                {
                    ek.CarriedKeyName = this.CarriedKeyName;
                }
                else
                {
                    ek.CarriedKeyName = null;
                }
                ek.EncryptionMethod = this.WrappingAlgorithm;
                ek.EncryptionMethodDictionaryString = _wrappingAlgorithmDictionaryString;
                ek.SetUpKeyWrap(_wrappedKey);
                if (this.WrappingTokenReference != null)
                {
                    ek.KeyIdentifier = this.WrappingTokenReference;
                }
                _encryptedKey = ek;
            }
        }
    }
}
