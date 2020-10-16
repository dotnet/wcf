// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public sealed class BasicHttpsSecurity
    {
        internal const BasicHttpsSecurityMode DefaultMode = BasicHttpsSecurityMode.Transport;
        private BasicHttpSecurity _basicHttpSecurity;

        public BasicHttpsSecurity()
            : this(DefaultMode, new HttpTransportSecurity())
        {
        }

        private BasicHttpsSecurity(BasicHttpsSecurityMode mode, HttpTransportSecurity transportSecurity)
        {
            if (!BasicHttpsSecurityModeHelper.IsDefined(mode))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("mode"));
            }
            HttpTransportSecurity httpTransportSecurity = transportSecurity == null ? new HttpTransportSecurity() : transportSecurity;
            BasicHttpSecurityMode basicHttpSecurityMode = BasicHttpsSecurityModeHelper.ToBasicHttpSecurityMode(mode);
            _basicHttpSecurity = new BasicHttpSecurity()
            {
                Mode = basicHttpSecurityMode,
                Transport = httpTransportSecurity
            };
        }

        public BasicHttpsSecurityMode Mode
        {
            get
            {
                return BasicHttpsSecurityModeHelper.ToBasicHttpsSecurityMode(_basicHttpSecurity.Mode);
            }

            set
            {
                if (!BasicHttpsSecurityModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }

                _basicHttpSecurity.Mode = BasicHttpsSecurityModeHelper.ToBasicHttpSecurityMode(value);
            }
        }

        public HttpTransportSecurity Transport
        {
            get
            {
                return _basicHttpSecurity.Transport;
            }

            set
            {
                _basicHttpSecurity.Transport = value;
            }
        }

        internal BasicHttpSecurity BasicHttpSecurity
        {
            get
            {
                return _basicHttpSecurity;
            }
        }

        internal static BasicHttpSecurity ToBasicHttpSecurity(BasicHttpsSecurity basicHttpsSecurity)
        {
            Fx.Assert(basicHttpsSecurity != null, "Cannot pass in a null value for basicHttpsSecurity");

            BasicHttpSecurity basicHttpSecurity = new BasicHttpSecurity()
            {
                Transport = basicHttpsSecurity.Transport,
                Mode = BasicHttpsSecurityModeHelper.ToBasicHttpSecurityMode(basicHttpsSecurity.Mode)
            };

            return basicHttpSecurity;
        }

        internal static BasicHttpsSecurity ToBasicHttpsSecurity(BasicHttpSecurity basicHttpSecurity)
        {
            Fx.Assert(basicHttpSecurity != null, "basicHttpSecurity cannot be null");

            BasicHttpsSecurity basicHttpsSecurity = new BasicHttpsSecurity()
            {
                Transport = basicHttpSecurity.Transport,
                Mode = BasicHttpsSecurityModeHelper.ToBasicHttpsSecurityMode(basicHttpSecurity.Mode)
            };

            return basicHttpsSecurity;
        }

        internal static void EnableTransportSecurity(HttpsTransportBindingElement https, HttpTransportSecurity transportSecurity)
        {
            BasicHttpSecurity.EnableTransportSecurity(https, transportSecurity);
        }

        internal static bool IsEnabledTransportAuthentication(HttpTransportBindingElement http, HttpTransportSecurity transportSecurity)
        {
            return BasicHttpSecurity.IsEnabledTransportAuthentication(http, transportSecurity);
        }

        internal void EnableTransportSecurity(HttpsTransportBindingElement https)
        {
            _basicHttpSecurity.EnableTransportSecurity(https);
        }

        internal void EnableTransportAuthentication(HttpTransportBindingElement http)
        {
            _basicHttpSecurity.EnableTransportAuthentication(http);
        }

        internal void DisableTransportAuthentication(HttpTransportBindingElement http)
        {
            _basicHttpSecurity.DisableTransportAuthentication(http);
        }

        internal SecurityBindingElement CreateMessageSecurity()
        {
            return _basicHttpSecurity.CreateMessageSecurity();
        }
    }
}
