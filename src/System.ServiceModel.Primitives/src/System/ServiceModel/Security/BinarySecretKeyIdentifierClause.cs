﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens;

namespace System.ServiceModel.Security
{
    public class BinarySecretKeyIdentifierClause : BinaryKeyIdentifierClause
    {
        private InMemorySymmetricSecurityKey _symmetricKey;

        public BinarySecretKeyIdentifierClause(byte[] key)
            : this(key, true)
        {
        }

        public BinarySecretKeyIdentifierClause(byte[] key, bool cloneBuffer)
            : this(key, cloneBuffer, null, 0)
        {
        }

        public BinarySecretKeyIdentifierClause(byte[] key, bool cloneBuffer, byte[] derivationNonce, int derivationLength)
            : base(XD.TrustFeb2005Dictionary.BinarySecretClauseType.Value, key, cloneBuffer, derivationNonce, derivationLength)
        {
        }

        public byte[] GetKeyBytes()
        {
            return GetBuffer();
        }

        public override bool CanCreateKey
        {
            get { return true; }
        }

        public override SecurityKey CreateKey()
        {
            if (_symmetricKey == null)
            {
                _symmetricKey = new InMemorySymmetricSecurityKey(GetBuffer(), false);
            }

            return _symmetricKey;
        }

        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            BinarySecretKeyIdentifierClause that = keyIdentifierClause as BinarySecretKeyIdentifierClause;
            return ReferenceEquals(this, that) || (that != null && that.Matches(GetRawBuffer()));
        }
    }
}
