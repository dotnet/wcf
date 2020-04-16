// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    public enum WSFederationHttpSecurityMode
    {
        None,
        Message,
        TransportWithMessageCredential
    }

    internal static class WSFederationHttpSecurityModeHelper
    {
        internal static bool IsDefined(WSFederationHttpSecurityMode value)
        {
            return (value == WSFederationHttpSecurityMode.None ||
                value == WSFederationHttpSecurityMode.Message ||
                value == WSFederationHttpSecurityMode.TransportWithMessageCredential);
        }
    }
}

