// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Runtime;

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

        internal static HttpProxyCredentialType MapToProxyCredentialType(AuthenticationSchemes authenticationSchemes)
        {
            HttpProxyCredentialType result;
            switch (authenticationSchemes)
            {
                case AuthenticationSchemes.Anonymous:
                    result = HttpProxyCredentialType.None;
                    break;
                case AuthenticationSchemes.Basic:
                    result = HttpProxyCredentialType.Basic;
                    break;
                case AuthenticationSchemes.Digest:
                    result = HttpProxyCredentialType.Digest;
                    break;
                case AuthenticationSchemes.Ntlm:
                    result = HttpProxyCredentialType.Ntlm;
                    break;
                case AuthenticationSchemes.Negotiate:
                    result = HttpProxyCredentialType.Windows;
                    break;
                default:
                    Fx.Assert("unsupported authentication Scheme");
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }
            return result;
        }
    }
}

