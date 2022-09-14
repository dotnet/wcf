// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
    internal sealed class NonceToken : BinarySecretSecurityToken
    {
        public NonceToken(byte[] key)
            : this(SecurityUniqueId.Create().Value, key)
        {
        }

        public NonceToken(string id, byte[] key)
            : base(id, key, false)
        {
        }

        public NonceToken(int keySizeInBits)
            : base(SecurityUniqueId.Create().Value, keySizeInBits, false)
        {
        }
    }
}
