// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    public enum UnixDomainSocketSecurityMode
    {
        None,
        Transport,
        TransportCredentialOnly
    }

    internal static class UnixDomainSocketSecurityModeHelper
    {
        public static bool IsDefined(UnixDomainSocketSecurityMode value)
        {
            return (value == UnixDomainSocketSecurityMode.None ||
                value == UnixDomainSocketSecurityMode.Transport ||
                value == UnixDomainSocketSecurityMode.TransportCredentialOnly);
        }
    }
}
