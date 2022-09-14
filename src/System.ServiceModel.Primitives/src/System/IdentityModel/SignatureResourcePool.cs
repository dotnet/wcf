// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using System.ServiceModel;

namespace System.IdentityModel
{
    // for sequential use by one thread
    internal sealed class SignatureResourcePool
    {
        private HashStream _hashStream;
        private HashAlgorithm _hashAlgorithm;

        private HashAlgorithm TakeHashAlgorithm(string algorithm)
        {
            if (_hashAlgorithm == null)
            {
                if (string.IsNullOrEmpty(algorithm))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(algorithm, SRP.Format(SRP.EmptyOrNullArgumentString, nameof(algorithm)));
                }

                _hashAlgorithm = CryptoHelper.CreateHashAlgorithm(algorithm);
            }
            else
            {
                _hashAlgorithm.Initialize();
            }

            return _hashAlgorithm;
        }

        private HashStream TakeHashStream(HashAlgorithm hash)
        {
            if (_hashStream == null)
            {
                _hashStream = new HashStream(hash);
            }
            else
            {
                _hashStream.Reset(hash);
            }
            return _hashStream;
        }

        public HashStream TakeHashStream(string algorithm)
        {
            return TakeHashStream(TakeHashAlgorithm(algorithm));
        }
    }
}
