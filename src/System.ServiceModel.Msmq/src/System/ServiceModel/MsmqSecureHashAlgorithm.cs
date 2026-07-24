// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel
{
    public enum MsmqSecureHashAlgorithm
    {
        MD5,
        Sha1,
        Sha256,
        Sha512
    }

    internal static class MsmqSecureHashAlgorithmHelper
    {
        public static bool IsDefined(MsmqSecureHashAlgorithm algorithm)
        {
            return algorithm == MsmqSecureHashAlgorithm.MD5 ||
                algorithm == MsmqSecureHashAlgorithm.Sha1 ||
                algorithm == MsmqSecureHashAlgorithm.Sha256 ||
                algorithm == MsmqSecureHashAlgorithm.Sha512;
        }
    }
}
