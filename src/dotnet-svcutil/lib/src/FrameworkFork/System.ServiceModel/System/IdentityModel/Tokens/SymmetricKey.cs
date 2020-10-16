// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Selectors;
using System.Security.Cryptography;
using System.ServiceModel;

namespace System.IdentityModel.Tokens
{
    internal class InMemorySymmetricSecurityKey : SymmetricSecurityKey
    {
        private int _keySize;
        private byte[] _symmetricKey;

        public InMemorySymmetricSecurityKey(byte[] symmetricKey)
            : this(symmetricKey, true)
        {
        }

        public InMemorySymmetricSecurityKey(byte[] symmetricKey, bool cloneBuffer)
        {
            if (symmetricKey == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("symmetricKey"));
            }

            if (symmetricKey.Length == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(string.Format(SRServiceModel.SymmetricKeyLengthTooShort, symmetricKey.Length)));
            }
            _keySize = symmetricKey.Length * 8;

            if (cloneBuffer)
            {
                _symmetricKey = new byte[symmetricKey.Length];
                Buffer.BlockCopy(symmetricKey, 0, _symmetricKey, 0, symmetricKey.Length);
            }
            else
            {
                _symmetricKey = symmetricKey;
            }
        }

        public override int KeySize
        {
            get { return _keySize; }
        }

        public override byte[] DecryptKey(string algorithm, byte[] keyData)
        {
            return CryptoHelper.UnwrapKey(_symmetricKey, keyData, algorithm);
        }

        public override byte[] EncryptKey(string algorithm, byte[] keyData)
        {
            return CryptoHelper.WrapKey(_symmetricKey, keyData, algorithm);
        }

        public override byte[] GenerateDerivedKey(string algorithm, byte[] label, byte[] nonce, int derivedKeyLength, int offset)
        {
            return CryptoHelper.GenerateDerivedKey(_symmetricKey, algorithm, label, nonce, derivedKeyLength, offset);
        }

#pragma warning disable 0436 // ICryptoTransform conflicts with imported types 
        public override ICryptoTransform GetDecryptionTransform(string algorithm, byte[] iv)
        {
            return CryptoHelper.CreateDecryptor(_symmetricKey, iv, algorithm);
        }

        public override ICryptoTransform GetEncryptionTransform(string algorithm, byte[] iv)
        {
            return CryptoHelper.CreateEncryptor(_symmetricKey, iv, algorithm);
        }
#pragma warning restore 0436 

        public override int GetIVSize(string algorithm)
        {
            return CryptoHelper.GetIVSize(algorithm);
        }

#pragma warning disable 0436 // KeyedHashAlgorithm, SymmetricAlgorithm conflict with imported types 
        public override KeyedHashAlgorithm GetKeyedHashAlgorithm(string algorithm)
        {
            return CryptoHelper.CreateKeyedHashAlgorithm(_symmetricKey, algorithm);
        }

        public override SymmetricAlgorithm GetSymmetricAlgorithm(string algorithm)
        {
            return CryptoHelper.GetSymmetricAlgorithm(_symmetricKey, algorithm);
        }
#pragma warning restore 0436

        public override byte[] GetSymmetricKey()
        {
            byte[] local = new byte[_symmetricKey.Length];
            Buffer.BlockCopy(_symmetricKey, 0, local, 0, _symmetricKey.Length);

            return local;
        }

        public override bool IsAsymmetricAlgorithm(string algorithm)
        {
            return (CryptoHelper.IsAsymmetricAlgorithm(algorithm));
        }

        public override bool IsSupportedAlgorithm(string algorithm)
        {
            return (CryptoHelper.IsSymmetricSupportedAlgorithm(algorithm, this.KeySize));
        }

        public override bool IsSymmetricAlgorithm(string algorithm)
        {
            return CryptoHelper.IsSymmetricAlgorithm(algorithm);
        }
    }
}
