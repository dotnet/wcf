// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Security.Tokens
{
    using System.Collections.ObjectModel;
    using System.IdentityModel;
    using System.IdentityModel.Tokens;
    using System.Runtime.CompilerServices;

    //TODO: [TypeForwardedFrom("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class BinarySecretSecurityToken : SecurityToken
    {
        private string _id;
        private DateTime _effectiveTime;
        private byte[] _key;
        private ReadOnlyCollection<SecurityKey> _securityKeys;

        public BinarySecretSecurityToken(int keySizeInBits)
            : this(SecurityUniqueId.Create().Value, keySizeInBits)
        {
        }

        public BinarySecretSecurityToken(string id, int keySizeInBits)
            : this(id, keySizeInBits, true)
        {
        }

        public BinarySecretSecurityToken(byte[] key)
            : this(SecurityUniqueId.Create().Value, key)
        {
        }

        public BinarySecretSecurityToken(string id, byte[] key)
            : this(id, key, true)
        {
        }

        protected BinarySecretSecurityToken(string id, int keySizeInBits, bool allowCrypto)
        {
            throw new NotImplementedException();
        }

        protected BinarySecretSecurityToken(string id, byte[] key, bool allowCrypto)
        {
            if (key == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("key");

            _id = id;
            _effectiveTime = DateTime.UtcNow;
            _key = new byte[key.Length];
            Buffer.BlockCopy(key, 0, _key, 0, key.Length);
            if (allowCrypto)
            {
                _securityKeys = SecurityUtils.CreateSymmetricSecurityKeys(_key);
            }
            else
            {
                _securityKeys = EmptyReadOnlyCollection<SecurityKey>.Instance;
            }
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

        public int KeySize
        {
            get { return (_key.Length * 8); }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { return _securityKeys; }
        }

        public byte[] GetKeyBytes()
        {
            return SecurityUtils.CloneBuffer(_key);
        }
    }
}
