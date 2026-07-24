// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Net;
using System.ServiceModel.Channels;
using System.Runtime;

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

        internal static void DisableTransportAuthentication(HttpTransportBindingElement http)
        {
            DisableAuthentication(http);
        }

        private static void ConfigureAuthentication(HttpTransportBindingElement http, HttpTransportSecurity transportSecurity)
        {
            http.AuthenticationScheme = MapToAuthenticationScheme(transportSecurity.ClientCredentialType);
            // Also propagate ProxyCredentialType -> ProxyAuthenticationScheme so authenticated
            // corporate proxies (Basic/Digest/Ntlm/Negotiate) work when a ProxyAddress is set on
            // the binding. Mirrors HttpTransportSecurity.ConfigureAuthentication in
            // System.ServiceModel.Http. HttpProxyCredentialTypeHelper is internal to
            // System.ServiceModel.Http but visible to us via [InternalsVisibleTo].
            http.ProxyAuthenticationScheme = HttpProxyCredentialTypeHelper.MapToAuthenticationScheme(transportSecurity.ProxyCredentialType);
            // Realm property is not exposed on dotnet/wcf's HttpTransportBindingElement /
            // HttpTransportSecurity (server-side only). Skip in the client port.
            http.ExtendedProtectionPolicy = transportSecurity.ExtendedProtectionPolicy;
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

        private static void DisableAuthentication(HttpTransportBindingElement http)
        {
            http.AuthenticationScheme = AuthenticationSchemes.Anonymous;
            // Realm property not exposed in dotnet/wcf - see ConfigureAuthentication.
            //ExtendedProtectionPolicy is always copied - even for security mode None, Message and TransportWithMessageCredential,
            //because the settings for ExtendedProtectionPolicy are always below the <security><transport> element
            //http.ExtendedProtectionPolicy = extendedProtectionPolicy;
        }
    }
}
