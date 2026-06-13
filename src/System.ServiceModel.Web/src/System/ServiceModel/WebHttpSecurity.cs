// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public sealed class WebHttpSecurity
    {
        internal const WebHttpSecurityMode DefaultMode = WebHttpSecurityMode.None;
        private WebHttpSecurityMode _mode;
        private HttpTransportSecurity _transportSecurity;

        public WebHttpSecurity()
        {
            _transportSecurity = new HttpTransportSecurity();
        }

        public WebHttpSecurityMode Mode
        {
            get { return _mode; }
            set
            {
                if (!WebHttpSecurityModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }

                _mode = value;
                IsModeSet = true;
            }
        }

        internal bool IsModeSet { get; private set; }

        public HttpTransportSecurity Transport
        {
            get { return _transportSecurity; }
            set
            {
                _transportSecurity = (value == null) ? new HttpTransportSecurity() : value;
            }
        }

        internal void DisableTransportAuthentication(HttpTransportBindingElement http)
        {
            HttpTransportHelpers.DisableTransportAuthentication(http);
        }

        internal void EnableTransportAuthentication(HttpTransportBindingElement http)
        {
            HttpTransportHelpers.ConfigureTransportAuthentication(http, _transportSecurity);
        }

        internal void EnableTransportSecurity(HttpsTransportBindingElement https)
        {
            HttpTransportHelpers.ConfigureTransportProtectionAndAuthentication(https, _transportSecurity);
        }

        internal void ApplyAuthorizationPolicySupport(HttpTransportBindingElement httpTransport)
        {
            // AlwaysUseAuthorizationPolicySupport is server-side only; dotnet/wcf's
            // HttpTransportBindingElement and HttpTransportSecurity don't expose it.
            // No-op for the client port.
        }
    }
}
