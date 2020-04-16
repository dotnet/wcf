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

    public class WS2007FederationHttpBinding : WSFederationHttpBinding
    {
        private static readonly ReliableMessagingVersion s_WS2007ReliableMessagingVersion = ReliableMessagingVersion.WSReliableMessaging11;
        private static readonly TransactionProtocol s_WS2007TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;
        private static readonly MessageSecurityVersion s_WS2007MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;

        public WS2007FederationHttpBinding(string configName)
            : this()
        {
            //TODO: ApplyConfiguration(configName);
        }

        public WS2007FederationHttpBinding()
            : base()
        {
            this.ReliableSessionBindingElement.ReliableMessagingVersion = s_WS2007ReliableMessagingVersion;
            this.TransactionFlowBindingElement.TransactionProtocol = s_WS2007TransactionProtocol;
            this.HttpsTransport.MessageSecurityVersion = s_WS2007MessageSecurityVersion;
        }

        public WS2007FederationHttpBinding(WSFederationHttpSecurityMode securityMode)
            : this(securityMode, false)
        {
        }

        public WS2007FederationHttpBinding(WSFederationHttpSecurityMode securityMode, bool reliableSessionEnabled)
            : base(securityMode, reliableSessionEnabled)
        {
            this.ReliableSessionBindingElement.ReliableMessagingVersion = s_WS2007ReliableMessagingVersion;
            this.TransactionFlowBindingElement.TransactionProtocol = s_WS2007TransactionProtocol;
            this.HttpsTransport.MessageSecurityVersion = s_WS2007MessageSecurityVersion;
        }

        private WS2007FederationHttpBinding(WSFederationHttpSecurity security, PrivacyNoticeBindingElement privacy, bool reliableSessionEnabled)
            : base(security, privacy, reliableSessionEnabled)
        {
            this.ReliableSessionBindingElement.ReliableMessagingVersion = s_WS2007ReliableMessagingVersion;
            this.TransactionFlowBindingElement.TransactionProtocol = s_WS2007TransactionProtocol;
            this.HttpsTransport.MessageSecurityVersion = s_WS2007MessageSecurityVersion;
        }

        protected override SecurityBindingElement CreateMessageSecurity()
        {
            return this.Security.CreateMessageSecurity(this.ReliableSession.Enabled, s_WS2007MessageSecurityVersion);
        }

        internal new static bool TryCreate(SecurityBindingElement sbe, TransportBindingElement transport, PrivacyNoticeBindingElement privacy, ReliableSessionBindingElement rsbe, TransactionFlowBindingElement tfbe, out Binding binding)
        {
            bool isReliableSession = (rsbe != null);
            binding = null;

            // reverse GetTransport
            HttpTransportSecurity transportSecurity = new HttpTransportSecurity();
            WSFederationHttpSecurityMode mode;
            if (!WSFederationHttpBinding.GetSecurityModeFromTransport(transport, transportSecurity, out mode))
            {
                return false;
            }

            HttpsTransportBindingElement httpsBinding = transport as HttpsTransportBindingElement;
            if (httpsBinding != null && httpsBinding.MessageSecurityVersion != null)
            {
                if (httpsBinding.MessageSecurityVersion.SecurityPolicyVersion != s_WS2007MessageSecurityVersion.SecurityPolicyVersion)
                {
                    return false;
                }
            }

            WSFederationHttpSecurity security;
            if (WS2007FederationHttpBinding.TryCreateSecurity(sbe, mode, transportSecurity, isReliableSession, out security))
            {
                binding = new WS2007FederationHttpBinding(security, privacy, isReliableSession);
            }

            if (rsbe != null && rsbe.ReliableMessagingVersion != ReliableMessagingVersion.WSReliableMessaging11)
            {
                return false;
            }

            if (tfbe != null && tfbe.TransactionProtocol != TransactionProtocol.WSAtomicTransaction11)
            {
                return false;
            }

            return binding != null;
        }

        private static bool TryCreateSecurity(SecurityBindingElement sbe, WSFederationHttpSecurityMode mode, HttpTransportSecurity transportSecurity, bool isReliableSession, out WSFederationHttpSecurity security)
        {
            if (!WSFederationHttpSecurity.TryCreate(sbe, mode, transportSecurity, isReliableSession, s_WS2007MessageSecurityVersion, out security))
                return false;
            // the last check: make sure that security binding element match the incoming security
            return System.ServiceModel.Configuration.SecurityElement.AreBindingsMatching(security.CreateMessageSecurity(isReliableSession, s_WS2007MessageSecurityVersion), sbe);
        }
    }
}
