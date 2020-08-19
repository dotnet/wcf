﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public partial class WSHttpBinding : WSHttpBindingBase
    {
        private static readonly MessageSecurityVersion s_WSMessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;

        private WSHttpSecurity _security = new WSHttpSecurity();

        public WSHttpBinding() : base() { }

        public WSHttpBinding(SecurityMode securityMode) : this(securityMode, false) { }

        public WSHttpBinding(SecurityMode securityMode, bool reliableSessionEnabled) : base(reliableSessionEnabled)
        {
            _security.Mode = securityMode;
        }

        internal WSHttpBinding(WSHttpSecurity security, bool reliableSessionEnabled) : base(reliableSessionEnabled)
        {
            _security = security == null ? new WSHttpSecurity() : security;
        }

        [DefaultValue(HttpTransportDefaults.AllowCookies)]
        public bool AllowCookies
        {
            get { return HttpTransport.AllowCookies; }
            set
            {
                HttpTransport.AllowCookies = value;
                HttpsTransport.AllowCookies = value;
            }
        }

        public WSHttpSecurity Security
        {
            get { return _security; }
            set
            {
                _security = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(value)));
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingParameterCollection parameters)
        {
            if ((_security.Mode == SecurityMode.Transport) &&
                _security.Transport.ClientCredentialType == HttpClientCredentialType.InheritedFromHost)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.HttpClientCredentialTypeInvalid, _security.Transport.ClientCredentialType)));
            }

            return base.BuildChannelFactory<TChannel>(parameters);
        }

        public override BindingElementCollection CreateBindingElements()
        {
            return base.CreateBindingElements();
        }

        protected override TransportBindingElement GetTransport()
        {
            if (_security.Mode == SecurityMode.None || _security.Mode == SecurityMode.Message)
            {
                HttpTransport.ExtendedProtectionPolicy = _security.Transport.ExtendedProtectionPolicy;
                return HttpTransport;
            }
            else
            {
                _security.ApplyTransportSecurity(HttpsTransport);
                return HttpsTransport;
            }
        }

        protected override SecurityBindingElement CreateMessageSecurity()
        {
            return _security.CreateMessageSecurity(ReliableSession.Enabled, s_WSMessageSecurityVersion);
        }
    }
}
