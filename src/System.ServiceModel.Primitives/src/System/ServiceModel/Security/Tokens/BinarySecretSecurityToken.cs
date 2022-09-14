// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.IdentityModel;
using System.IdentityModel.Tokens;

namespace System.ServiceModel.Security.Tokens
{
    public class BinarySecretSecurityToken : SecurityToken
    {
        private string _id;
        private DateTime _effectiveTime;
        private byte[] _key;
        private ReadOnlyCollection<SecurityKey> _securityKeys;

        public BinarySecretSecurityToken(string id, byte[] key)
            : this(id, key, true)
        {
        }

        public BinarySecretSecurityToken(byte[] key)
            : this(SecurityUniqueId.Create().Value, key)
        {
        }

        protected BinarySecretSecurityToken(string id, int keySizeInBits, bool allowCrypto)
        {
            if (keySizeInBits <= 0 || keySizeInBits >= 512)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(keySizeInBits), SRP.Format(SRP.ValueMustBeInRange, 0, 512)));
            }

            if ((keySizeInBits % 8) != 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(keySizeInBits), SRP.Format(SRP.KeyLengthMustBeMultipleOfEight, keySizeInBits)));
            }

            _id = id;
            _effectiveTime = DateTime.UtcNow;
            _key = new byte[keySizeInBits / 8];
            CryptoHelper.FillRandomBytes(_key);

            if (allowCrypto)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
            else
            {
                _securityKeys = EmptyReadOnlyCollection<SecurityKey>.Instance;
            }
        }

        protected BinarySecretSecurityToken(string id, byte[] key, bool allowCrypto)
        {
            if (key == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(key));
            }

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
