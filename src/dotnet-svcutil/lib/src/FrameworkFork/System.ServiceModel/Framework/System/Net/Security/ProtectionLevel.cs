// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if FEATURE_NETNATIVE
namespace System.Net.Security
{
    // This will request security properties of a NegotiateStream
    public enum ProtectionLevel
    {
        // Used only with Negotiate on Win9x platform
        None = 0,

        // Data integrity only
        Sign = 1,

        // Both data confidentiality and integrity
        EncryptAndSign = 2
    }
}
#endif //FEATURE_NETNATIVE
