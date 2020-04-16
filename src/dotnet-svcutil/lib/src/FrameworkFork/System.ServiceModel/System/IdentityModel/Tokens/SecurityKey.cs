// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IdentityModel.Tokens
{
    public abstract class SecurityKey
    {
        public abstract int KeySize { get; }
        public abstract byte[] DecryptKey(string algorithm, byte[] keyData);
        public abstract byte[] EncryptKey(string algorithm, byte[] keyData);
        public abstract bool IsAsymmetricAlgorithm(string algorithm);
        public abstract bool IsSupportedAlgorithm(string algorithm);
        public abstract bool IsSymmetricAlgorithm(string algorithm);
    }
}
