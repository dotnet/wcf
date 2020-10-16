// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Net;
    using System.Net.Security;
    using System.Runtime.Serialization;
    using System.Security.Principal;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using Microsoft.Xml;

    public class WSHttpBinding : WSHttpBindingBase
    {
        private static readonly MessageSecurityVersion s_WSMessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;

        private WSHttpSecurity _security = new WSHttpSecurity();

        public WSHttpBinding(string configName)
            : this()
        {
            // TODO: ApplyConfiguration(configName);
        }

        public WSHttpBinding()
            : base()
        {
        }

        public WSHttpBinding(SecurityMode securityMode)
            : this(securityMode, false)
        {
        }

        public WSHttpBinding(SecurityMode securityMode, bool reliableSessionEnabled)
            : base(reliableSessionEnabled)
        {
            _security.Mode = securityMode;
        }

        internal WSHttpBinding(WSHttpSecurity security, bool reliableSessionEnabled)
            : base(reliableSessionEnabled)
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
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                }
                _security = value;
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingParameterCollection parameters)
        {
            if ((_security.Mode == SecurityMode.Transport) &&
                _security.Transport.ClientCredentialType == HttpClientCredentialType.InheritedFromHost)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.HttpClientCredentialTypeInvalid, _security.Transport.ClientCredentialType)));
            }

            return base.BuildChannelFactory<TChannel>(parameters);
        }

        public override BindingElementCollection CreateBindingElements()
        {
            if (ReliableSession.Enabled)
            {
                if (_security.Mode == SecurityMode.Transport)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.WSHttpDoesNotSupportRMWithHttps));
            }

            return base.CreateBindingElements();
        }

        // if you make changes here, see also WS2007HttpBinding.TryCreate()
        internal static bool TryCreate(SecurityBindingElement sbe, TransportBindingElement transport, ReliableSessionBindingElement rsbe, TransactionFlowBindingElement tfbe, out Binding binding)
        {
            bool isReliableSession = (rsbe != null);
            binding = null;

            // reverse GetTransport
            HttpTransportSecurity transportSecurity = WSHttpSecurity.GetDefaultHttpTransportSecurity();
            UnifiedSecurityMode mode;
            if (!GetSecurityModeFromTransport(transport, transportSecurity, out mode))
            {
                return false;
            }

            HttpsTransportBindingElement httpsBinding = transport as HttpsTransportBindingElement;
            if (httpsBinding != null && httpsBinding.MessageSecurityVersion != null)
            {
                if (httpsBinding.MessageSecurityVersion.SecurityPolicyVersion != s_WSMessageSecurityVersion.SecurityPolicyVersion)
                {
                    return false;
                }
            }

            WSHttpSecurity security;
            if (TryCreateSecurity(sbe, mode, transportSecurity, isReliableSession, out security))
            {
                WSHttpBinding wsHttpBinding = new WSHttpBinding(security, isReliableSession);

                bool allowCookies;
                if (!TryGetAllowCookiesFromTransport(transport, out allowCookies))
                {
                    return false;
                }

                wsHttpBinding.AllowCookies = allowCookies;
                binding = wsHttpBinding;
            }

            if (rsbe != null && rsbe.ReliableMessagingVersion != ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                return false;
            }

            if (tfbe != null && tfbe.TransactionProtocol != TransactionProtocol.WSAtomicTransactionOctober2004)
            {
                return false;
            }

            return binding != null;
        }

        protected override TransportBindingElement GetTransport()
        {
            if (_security.Mode == SecurityMode.None || _security.Mode == SecurityMode.Message)
            {
                this.HttpTransport.ExtendedProtectionPolicy = _security.Transport.ExtendedProtectionPolicy;
                return this.HttpTransport;
            }
            else
            {
                _security.ApplyTransportSecurity(this.HttpsTransport);
                return this.HttpsTransport;
            }
        }

        internal static bool GetSecurityModeFromTransport(TransportBindingElement transport, HttpTransportSecurity transportSecurity, out UnifiedSecurityMode mode)
        {
            mode = UnifiedSecurityMode.None;
            if (transport is HttpsTransportBindingElement)
            {
                mode = UnifiedSecurityMode.Transport | UnifiedSecurityMode.TransportWithMessageCredential;
                WSHttpSecurity.ApplyTransportSecurity((HttpsTransportBindingElement)transport, transportSecurity);
            }
            else if (transport is HttpTransportBindingElement)
            {
                mode = UnifiedSecurityMode.None | UnifiedSecurityMode.Message;
            }
            else
            {
                return false;
            }
            return true;
        }

        internal static bool TryGetAllowCookiesFromTransport(TransportBindingElement transport, out bool allowCookies)
        {
            HttpTransportBindingElement httpTransportBindingElement = transport as HttpTransportBindingElement;
            if (httpTransportBindingElement == null)
            {
                allowCookies = false;
                return false;
            }
            else
            {
                allowCookies = httpTransportBindingElement.AllowCookies;
                return true;
            }
        }

        protected override SecurityBindingElement CreateMessageSecurity()
        {
            return _security.CreateMessageSecurity(this.ReliableSession.Enabled, s_WSMessageSecurityVersion);
        }

        // if you make changes here, see also WS2007HttpBinding.TryCreateSecurity()
        private static bool TryCreateSecurity(SecurityBindingElement sbe, UnifiedSecurityMode mode, HttpTransportSecurity transportSecurity, bool isReliableSession, out WSHttpSecurity security)
        {
            if (!WSHttpSecurity.TryCreate(sbe, mode, transportSecurity, isReliableSession, out security))
                return false;
            // the last check: make sure that security binding element match the incoming security
            return System.ServiceModel.Configuration.SecurityElement.AreBindingsMatching(security.CreateMessageSecurity(isReliableSession, s_WSMessageSecurityVersion), sbe);
        }
    }
}
