// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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

