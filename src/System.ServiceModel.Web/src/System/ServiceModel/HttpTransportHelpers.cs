// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    internal static class HttpTransportHelpers
    {
        private const string DefaultRealm = ""; // HttpTransportDefaults.Realm is not exposed in dotnet/wcf; the .NET FX default is empty string.

        internal static void ConfigureTransportProtectionAndAuthentication(HttpsTransportBindingElement https, HttpTransportSecurity transportSecurity)
        {
            ConfigureAuthentication(https, transportSecurity);
            https.RequireClientCertificate = (transportSecurity.ClientCredentialType == HttpClientCredentialType.Certificate);
        }

        internal static void ConfigureTransportAuthentication(HttpTransportBindingElement http, HttpTransportSecurity transportSecurity)
        {
            if (transportSecurity.ClientCredentialType == HttpClientCredentialType.Certificate)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.CertificateUnsupportedForHttpTransportCredentialOnly));
            }

            ConfigureAuthentication(http, transportSecurity);
        }

        internal static void DisableTransportAuthentication(HttpTransportBindingElement http, HttpTransportSecurity transportSecurity)
        {
            DisableAuthentication(http, transportSecurity);
        }

        private static void ConfigureAuthentication(HttpTransportBindingElement http, HttpTransportSecurity transportSecurity)
        {
            http.AuthenticationScheme = MapToAuthenticationScheme(transportSecurity.ClientCredentialType);
            // Also propagate ProxyCredentialType -> ProxyAuthenticationScheme so authenticated
            // corporate proxies (Basic/Digest/Ntlm/Negotiate) work when a ProxyAddress is set on
            // the binding. Mirrors HttpTransportSecurity.ConfigureAuthentication in
            // System.ServiceModel.Http.
            http.ProxyAuthenticationScheme = MapProxyToAuthenticationScheme(transportSecurity.ProxyCredentialType);
            // Realm property is not exposed on dotnet/wcf's HttpTransportBindingElement /
            // HttpTransportSecurity (server-side only). Skip in the client port.
            http.ExtendedProtectionPolicy = transportSecurity.ExtendedProtectionPolicy;
        }

        private static AuthenticationSchemes MapProxyToAuthenticationScheme(HttpProxyCredentialType proxyCredentialType)
        {
            // Inlined rather than calling System.ServiceModel.Http's internal
            // HttpProxyCredentialTypeHelper so this package doesn't need InternalsVisibleTo.
            switch (proxyCredentialType)
            {
                case HttpProxyCredentialType.None:
                    return AuthenticationSchemes.Anonymous;
                case HttpProxyCredentialType.Basic:
                    return AuthenticationSchemes.Basic;
                case HttpProxyCredentialType.Digest:
                    return AuthenticationSchemes.Digest;
                case HttpProxyCredentialType.Ntlm:
                    return AuthenticationSchemes.Ntlm;
                case HttpProxyCredentialType.Windows:
                    return AuthenticationSchemes.Negotiate;
                default:
                    Fx.Assert("unsupported proxy credential type");
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }
        }

        private static AuthenticationSchemes MapToAuthenticationScheme(HttpClientCredentialType clientCredentialType)
        {
            AuthenticationSchemes result;
            switch (clientCredentialType)
            {
                case HttpClientCredentialType.Certificate:
                // fall through to None case
                case HttpClientCredentialType.None:
                    result = AuthenticationSchemes.Anonymous;
                    break;
                case HttpClientCredentialType.Basic:
                    result = AuthenticationSchemes.Basic;
                    break;
                case HttpClientCredentialType.Digest:
                    result = AuthenticationSchemes.Digest;
                    break;
                case HttpClientCredentialType.Ntlm:
                    result = AuthenticationSchemes.Ntlm;
                    break;
                case HttpClientCredentialType.Windows:
                    result = AuthenticationSchemes.Negotiate;
                    break;
                case HttpClientCredentialType.InheritedFromHost:
                    result = AuthenticationSchemes.None;
                    break;
                default:
                    Fx.Assert("unsupported client credential type");
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }
            return result;
        }

        private static void DisableAuthentication(HttpTransportBindingElement http, HttpTransportSecurity transportSecurity)
        {
            http.AuthenticationScheme = AuthenticationSchemes.Anonymous;
            http.ProxyAuthenticationScheme = AuthenticationSchemes.Anonymous;
            // Realm property not exposed in dotnet/wcf - see ConfigureAuthentication.
            // ExtendedProtectionPolicy is always copied because its settings are
            // under the <security><transport> element, including for mode None.
            http.ExtendedProtectionPolicy = transportSecurity.ExtendedProtectionPolicy;
        }
    }
}
