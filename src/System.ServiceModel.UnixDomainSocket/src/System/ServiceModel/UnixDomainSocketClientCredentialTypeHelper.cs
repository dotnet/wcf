// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ServiceModel
{
    internal static class UnixDomainSocketClientCredentialTypeHelper
    {
        internal static bool IsDefined(UnixDomainSocketClientCredentialType value)
        {
            return (value == UnixDomainSocketClientCredentialType.None ||
                value == UnixDomainSocketClientCredentialType.Windows ||
                value == UnixDomainSocketClientCredentialType.Certificate ||
                value == UnixDomainSocketClientCredentialType.Default ||
                value == UnixDomainSocketClientCredentialType.PosixIdentity
                );
        }
    }
}
