// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel.Channels;
using System.Net;

namespace System.ServiceModel
{
    public sealed class HttpTransportSecurity
    {
        internal const HttpClientCredentialType DefaultClientCredentialType = HttpClientCredentialType.None;
        internal const string DefaultRealm = System.ServiceModel.Channels.HttpTransportDefaults.Realm;

        private HttpClientCredentialType _clientCredentialType;
        private string _realm;


        public HttpTransportSecurity()
        {
            _clientCredentialType = DefaultClientCredentialType;
            _realm = DefaultRealm;
        }

        public HttpClientCredentialType ClientCredentialType
        {
            get { return _clientCredentialType; }
            set
            {
                if (!HttpClientCredentialTypeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                _clientCredentialType = value;
            }
        }

        public string Realm
        {
            get { return _realm; }
            set { _realm = value; }
        }


        internal void ConfigureTransportProtectionOnly(HttpsTransportBindingElement https)
        {
            DisableAuthentication(https);
            https.RequireClientCertificate = false;
        }

        private void ConfigureAuthentication(HttpTransportBindingElement http)
        {
            http.AuthenticationScheme = HttpClientCredentialTypeHelper.MapToAuthenticationScheme(_clientCredentialType);
            http.Realm = this.Realm;
        }

        private static void ConfigureAuthentication(HttpTransportBindingElement http, HttpTransportSecurity transportSecurity)
        {
            transportSecurity._clientCredentialType = HttpClientCredentialTypeHelper.MapToClientCredentialType(http.AuthenticationScheme);
            transportSecurity.Realm = http.Realm;
        }

        private void DisableAuthentication(HttpTransportBindingElement http)
        {
            http.AuthenticationScheme = AuthenticationSchemes.Anonymous;
            http.Realm = DefaultRealm;
            //ExtendedProtectionPolicy is always copied - even for security mode None, Message and TransportWithMessageCredential,
            //because the settings for ExtendedProtectionPolicy are always below the <security><transport> element
            //http.ExtendedProtectionPolicy = this.extendedProtectionPolicy;
        }

        private static bool IsDisabledAuthentication(HttpTransportBindingElement http)
        {
            return http.AuthenticationScheme == AuthenticationSchemes.Anonymous && http.Realm == DefaultRealm;
        }

        internal void ConfigureTransportProtectionAndAuthentication(HttpsTransportBindingElement https)
        {
            ConfigureAuthentication(https);
            https.RequireClientCertificate = (_clientCredentialType == HttpClientCredentialType.Certificate);
        }

        internal static void ConfigureTransportProtectionAndAuthentication(HttpsTransportBindingElement https, HttpTransportSecurity transportSecurity)
        {
            ConfigureAuthentication(https, transportSecurity);
            if (https.RequireClientCertificate)
                transportSecurity.ClientCredentialType = HttpClientCredentialType.Certificate;
        }

        internal void ConfigureTransportAuthentication(HttpTransportBindingElement http)
        {
            if (_clientCredentialType == HttpClientCredentialType.Certificate)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.CertificateUnsupportedForHttpTransportCredentialOnly));
            }
            ConfigureAuthentication(http);
        }

        internal static bool IsConfiguredTransportAuthentication(HttpTransportBindingElement http, HttpTransportSecurity transportSecurity)
        {
            if (HttpClientCredentialTypeHelper.MapToClientCredentialType(http.AuthenticationScheme) == HttpClientCredentialType.Certificate)
                return false;
            ConfigureAuthentication(http, transportSecurity);
            return true;
        }

        internal void DisableTransportAuthentication(HttpTransportBindingElement http)
        {
            DisableAuthentication(http);
        }

        internal static bool IsDisabledTransportAuthentication(HttpTransportBindingElement http)
        {
            return IsDisabledAuthentication(http);
        }
    }
}
