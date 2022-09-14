// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    internal class AcceleratedTokenProviderState : IssuanceTokenProviderState
    {
        private byte[] _entropy;

        public AcceleratedTokenProviderState(byte[] value)
        {
            _entropy = value;
        }

        public byte[] GetRequestorEntropy()
        {
            return _entropy;
        }
    }
}
