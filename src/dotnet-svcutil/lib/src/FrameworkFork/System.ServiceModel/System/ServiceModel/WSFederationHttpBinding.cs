// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Net;
    using System.Net.Security;
    using System.Runtime.Serialization;
    using System.Security.Principal;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;

    using Microsoft.Xml;
    using System.ComponentModel;

    public class WSFederationHttpBinding : WSHttpBindingBase
    {
        private static readonly MessageSecurityVersion s_WSMessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;

        private Uri _privacyNoticeAt;
        private int _privacyNoticeVersion;
        private WSFederationHttpSecurity _security = new WSFederationHttpSecurity();

        public WSFederationHttpBinding(string configName)
            : this()
        {
            //TODO: ApplyConfiguration(configName);
        }

        public WSFederationHttpBinding()
            : base()
        {
        }

        public WSFederationHttpBinding(WSFederationHttpSecurityMode securityMode)
            : this(securityMode, false)
        {
        }

        public WSFederationHttpBinding(WSFederationHttpSecurityMode securityMode, bool reliableSessionEnabled)
            : base(reliableSessionEnabled)
        {
            _security.Mode = securityMode;
        }


        internal WSFederationHttpBinding(WSFederationHttpSecurity security, PrivacyNoticeBindingElement privacy, bool reliableSessionEnabled)
            : base(reliableSessionEnabled)
        {
            _security = security;
            if (null != privacy)
            {
                _privacyNoticeAt = privacy.Url;
                _privacyNoticeVersion = privacy.Version;
            }
        }

        [DefaultValue(null)]
        public Uri PrivacyNoticeAt
        {
            get { return _privacyNoticeAt; }
            set { _privacyNoticeAt = value; }
        }

        [DefaultValue(0)]
        public int PrivacyNoticeVersion
        {
            get { return _privacyNoticeVersion; }
            set { _privacyNoticeVersion = value; }
        }

        public WSFederationHttpSecurity Security
        {
            get { return _security; }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                _security = value;
            }
        }

        private PrivacyNoticeBindingElement CreatePrivacyPolicy()
        {
            PrivacyNoticeBindingElement privacy = null;

            if (this.PrivacyNoticeAt != null)
            {
                privacy = new PrivacyNoticeBindingElement();
                privacy.Url = this.PrivacyNoticeAt;
                privacy.Version = _privacyNoticeVersion;
            }

            return privacy;
        }

        // if you make changes here, see also WS2007FederationHttpBinding.TryCreate()
        internal static bool TryCreate(SecurityBindingElement sbe, TransportBindingElement transport, PrivacyNoticeBindingElement privacy, ReliableSessionBindingElement rsbe, TransactionFlowBindingElement tfbe, out Binding binding)
        {
            bool isReliableSession = (rsbe != null);
            binding = null;

            // reverse GetTransport
            HttpTransportSecurity transportSecurity = new HttpTransportSecurity();
            WSFederationHttpSecurityMode mode;
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

            WSFederationHttpSecurity security;
            if (TryCreateSecurity(sbe, mode, transportSecurity, isReliableSession, out security))
            {
                binding = new WSFederationHttpBinding(security, privacy, isReliableSession);
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
            if (_security.Mode == WSFederationHttpSecurityMode.None || _security.Mode == WSFederationHttpSecurityMode.Message)
            {
                return this.HttpTransport;
            }
            else
            {
                return this.HttpsTransport;
            }
        }

        internal static bool GetSecurityModeFromTransport(TransportBindingElement transport, HttpTransportSecurity transportSecurity, out WSFederationHttpSecurityMode mode)
        {
            mode = WSFederationHttpSecurityMode.None | WSFederationHttpSecurityMode.Message | WSFederationHttpSecurityMode.TransportWithMessageCredential;
            if (transport is HttpsTransportBindingElement)
            {
                mode = WSFederationHttpSecurityMode.TransportWithMessageCredential;
            }
            else if (transport is HttpTransportBindingElement)
            {
                mode = WSFederationHttpSecurityMode.None | WSFederationHttpSecurityMode.Message;
            }
            else
            {
                return false;
            }
            return true;
        }

        protected override SecurityBindingElement CreateMessageSecurity()
        {
            return _security.CreateMessageSecurity(this.ReliableSession.Enabled, s_WSMessageSecurityVersion);
        }

        // if you make changes here, see also WS2007FederationHttpBinding.TryCreateSecurity()
        private static bool TryCreateSecurity(SecurityBindingElement sbe, WSFederationHttpSecurityMode mode, HttpTransportSecurity transportSecurity, bool isReliableSession, out WSFederationHttpSecurity security)
        {
            if (!WSFederationHttpSecurity.TryCreate(sbe, mode, transportSecurity, isReliableSession, s_WSMessageSecurityVersion, out security))
                return false;
            // the last check: make sure that security binding element match the incoming security
            return System.ServiceModel.Configuration.SecurityElement.AreBindingsMatching(security.CreateMessageSecurity(isReliableSession, s_WSMessageSecurityVersion), sbe);
        }

        public override BindingElementCollection CreateBindingElements()
        {   // return collection of BindingElements
            BindingElementCollection bindingElements = base.CreateBindingElements();
            // order of BindingElements is important

            PrivacyNoticeBindingElement privacy = this.CreatePrivacyPolicy();
            if (privacy != null)
            {
                // This must go first.
                bindingElements.Insert(0, privacy);
            }

            return bindingElements;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeSecurity()
        {
            return this.Security.InternalShouldSerialize();
        }
    }
}
