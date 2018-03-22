// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public sealed class BasicHttpSecurity
    {
        internal const BasicHttpSecurityMode DefaultMode = BasicHttpSecurityMode.None;
        private BasicHttpSecurityMode _mode;
        private HttpTransportSecurity _transportSecurity;

        public BasicHttpSecurity()
            : this(DefaultMode, new HttpTransportSecurity())
        {
        }

        private BasicHttpSecurity(BasicHttpSecurityMode mode, HttpTransportSecurity transportSecurity)
        {
            Fx.Assert(BasicHttpSecurityModeHelper.IsDefined(mode), string.Format("Invalid BasicHttpSecurityMode value: {0}.", mode.ToString()));
            this.Mode = mode;
            _transportSecurity = transportSecurity == null ? new HttpTransportSecurity() : transportSecurity;
        }

        public BasicHttpSecurityMode Mode
        {
            get { return _mode; }
            set
            {
                if (value == BasicHttpSecurityMode.Message ||
                    value == BasicHttpSecurityMode.TransportWithMessageCredential)
                {
                    throw ExceptionHelper.PlatformNotSupported(SR.Format(SR.UnsupportedSecuritySetting, nameof(value), value));
                }

                if (!BasicHttpSecurityModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }
                _mode = value;
            }
        }

        public HttpTransportSecurity Transport
        {
            get { return _transportSecurity; }
            set
            {
                _transportSecurity = (value == null) ? new HttpTransportSecurity() : value;
            }
        }

        internal void EnableTransportSecurity(HttpsTransportBindingElement https)
        {
            if (_mode == BasicHttpSecurityMode.TransportWithMessageCredential)
            {
                _transportSecurity.ConfigureTransportProtectionOnly(https);
            }
            else
            {
                _transportSecurity.ConfigureTransportProtectionAndAuthentication(https);
            }
        }

        internal static void EnableTransportSecurity(HttpsTransportBindingElement https, HttpTransportSecurity transportSecurity)
        {
            HttpTransportSecurity.ConfigureTransportProtectionAndAuthentication(https, transportSecurity);
        }

        internal void EnableTransportAuthentication(HttpTransportBindingElement http)
        {
            _transportSecurity.ConfigureTransportAuthentication(http);
        }

        internal static bool IsEnabledTransportAuthentication(HttpTransportBindingElement http, HttpTransportSecurity transportSecurity)
        {
            return HttpTransportSecurity.IsConfiguredTransportAuthentication(http, transportSecurity);
        }

        internal void DisableTransportAuthentication(HttpTransportBindingElement http)
        {
            _transportSecurity.DisableTransportAuthentication(http);
        }

        internal SecurityBindingElement CreateMessageSecurity()
        {
            if (_mode == BasicHttpSecurityMode.Message
                || _mode == BasicHttpSecurityMode.TransportWithMessageCredential)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            return null;
        }
    }
}
