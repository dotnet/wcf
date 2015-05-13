// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Cryptography;

namespace System.IdentityModel.Tokens
{
    public abstract class SymmetricSecurityKey : SecurityKey
    {
        public abstract byte[] GenerateDerivedKey(string algorithm, byte[] label, byte[] nonce, int derivedKeyLength, int offset);
        public abstract ICryptoTransform GetDecryptionTransform(string algorithm, byte[] iv);
        public abstract ICryptoTransform GetEncryptionTransform(string algorithm, byte[] iv);
        public abstract int GetIVSize(string algorithm);
        public abstract KeyedHashAlgorithm GetKeyedHashAlgorithm(string algorithm);
        public abstract SymmetricAlgorithm GetSymmetricAlgorithm(string algorithm);
        public abstract byte[] GetSymmetricKey();
    }
}
