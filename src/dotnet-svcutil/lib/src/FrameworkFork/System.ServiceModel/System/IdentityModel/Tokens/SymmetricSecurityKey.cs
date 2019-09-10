// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Cryptography;

namespace System.IdentityModel.Tokens
{
    internal abstract class SymmetricSecurityKey : SecurityKey
    {
        public abstract byte[] GenerateDerivedKey(string algorithm, byte[] label, byte[] nonce, int derivedKeyLength, int offset);

#pragma warning disable 0436 // ICryptoTransform conflicts with imported types 
        public abstract ICryptoTransform GetDecryptionTransform(string algorithm, byte[] iv);
        public abstract ICryptoTransform GetEncryptionTransform(string algorithm, byte[] iv);
#pragma warning restore 0436

        public abstract int GetIVSize(string algorithm);

#pragma warning disable 0436 // KeyedHashAlgorithm, SymmetricAlgorithm conflict with imported types 
        public abstract KeyedHashAlgorithm GetKeyedHashAlgorithm(string algorithm);
        public abstract SymmetricAlgorithm GetSymmetricAlgorithm(string algorithm);
#pragma warning restore 0436

        public abstract byte[] GetSymmetricKey();
    }
}
