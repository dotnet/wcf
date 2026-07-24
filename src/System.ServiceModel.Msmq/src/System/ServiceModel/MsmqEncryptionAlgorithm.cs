// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel
{
    public enum MsmqEncryptionAlgorithm
    {
        RC4Stream,
        Aes
    }

    internal static class MsmqEncryptionAlgorithmHelper
    {
        public static bool IsDefined(MsmqEncryptionAlgorithm algorithm)
        {
            return algorithm == MsmqEncryptionAlgorithm.RC4Stream || algorithm == MsmqEncryptionAlgorithm.Aes;
        }
    }
}
