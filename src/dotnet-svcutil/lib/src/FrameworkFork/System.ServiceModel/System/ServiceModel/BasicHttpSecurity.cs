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
        private BasicHttpMessageSecurity _messageSecurity;

        public BasicHttpSecurity()
            : this(DefaultMode, new HttpTransportSecurity(), new BasicHttpMessageSecurity())
        {
        }

        private BasicHttpSecurity(BasicHttpSecurityMode mode, HttpTransportSecurity transportSecurity, BasicHttpMessageSecurity messageSecurity)
        {
            Fx.Assert(BasicHttpSecurityModeHelper.IsDefined(mode), string.Format("Invalid BasicHttpSecurityMode value: {0}.", mode.ToString()));
            this.Mode = mode;
            _transportSecurity = transportSecurity == null ? new HttpTransportSecurity() : transportSecurity;
            _messageSecurity = messageSecurity == null ? new BasicHttpMessageSecurity() : messageSecurity;
        }


        public BasicHttpSecurityMode Mode
        {
            get { return _mode; }
            set
            {
                if (!BasicHttpSecurityModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
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

        public BasicHttpMessageSecurity Message
        {
            get { return _messageSecurity; }
            set
            {
                _messageSecurity = (value == null) ? new BasicHttpMessageSecurity() : value;
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
                return this._messageSecurity.CreateMessageSecurity(this.Mode == BasicHttpSecurityMode.TransportWithMessageCredential);
            }

            return null;
        }

        internal static bool TryCreate(SecurityBindingElement sbe, UnifiedSecurityMode mode, HttpTransportSecurity transportSecurity, out BasicHttpSecurity security)
        {
            security = null;
            BasicHttpMessageSecurity messageSecurity = null;
            if (sbe != null)
            {
                mode &= UnifiedSecurityMode.Message | UnifiedSecurityMode.TransportWithMessageCredential;
                bool isSecureTransportMode;
                if (!BasicHttpMessageSecurity.TryCreate(sbe, out messageSecurity, out isSecureTransportMode))
                {
                    return false;
                }
            }
            else
            {
                mode &= ~(UnifiedSecurityMode.Message | UnifiedSecurityMode.TransportWithMessageCredential);
            }
            BasicHttpSecurityMode basicHttpSecurityMode = BasicHttpSecurityModeHelper.ToSecurityMode(mode);
            Fx.Assert(BasicHttpSecurityModeHelper.IsDefined(basicHttpSecurityMode), string.Format("Invalid BasicHttpSecurityMode value: {0}.", basicHttpSecurityMode.ToString()));
            security = new BasicHttpSecurity(basicHttpSecurityMode, transportSecurity, messageSecurity);

            return System.ServiceModel.Configuration.SecurityElement.AreBindingsMatching(security.CreateMessageSecurity(), sbe);
        }
    }
}
