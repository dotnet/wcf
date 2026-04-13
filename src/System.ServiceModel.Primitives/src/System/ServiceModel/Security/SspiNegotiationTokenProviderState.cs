// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;

namespace System.ServiceModel.Security
{
    internal class SspiNegotiationTokenProviderState : IssuanceTokenProviderState
    {
        private ISspiNegotiation _sspiNegotiation;
        private IncrementalHash _negotiationDigest;

        public SspiNegotiationTokenProviderState(ISspiNegotiation sspiNegotiation)
            : base()
        {
            _sspiNegotiation = sspiNegotiation ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(sspiNegotiation));
            _negotiationDigest = IncrementalHash.CreateHash(HashAlgorithmName.SHA1);
        }

        public ISspiNegotiation SspiNegotiation
        {
            get
            {
                return _sspiNegotiation;
            }
        }

        internal IncrementalHash NegotiationDigest
        {
            get
            {
                return _negotiationDigest;
            }
        }

        public override void Dispose()
        {
            try
            {
                if (_sspiNegotiation != null)
                {
                    _sspiNegotiation.Dispose();
                    _sspiNegotiation = null;

                    _negotiationDigest?.Dispose();
                    _negotiationDigest = null;
                }
            }
            finally
            {
                base.Dispose();
            }
        }
    }
}
