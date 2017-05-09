// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    public enum HttpProxyCredentialType
    {
        None,
        Basic,
        Digest,
        Ntlm,
        Windows,
    }

    internal static class HttpProxyCredentialTypeHelper
    {
        internal static bool IsDefined(HttpProxyCredentialType value)
        {
            return (value == HttpProxyCredentialType.None ||
                    value == HttpProxyCredentialType.Basic ||
                    value == HttpProxyCredentialType.Digest ||
                    value == HttpProxyCredentialType.Ntlm ||
                    value == HttpProxyCredentialType.Windows);
        }
    }
}

