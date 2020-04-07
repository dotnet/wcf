// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
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
        static readonly ReliableMessagingVersion WS2007ReliableMessagingVersion = ReliableMessagingVersion.WSReliableMessaging11;
        static readonly TransactionProtocol WS2007TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;
        static readonly MessageSecurityVersion WS2007MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;

        public WS2007FederationHttpBinding(string configName)
            : this()
        {
            //TODO: ApplyConfiguration(configName);
        }

        public WS2007FederationHttpBinding()
            : base()
        {
            this.ReliableSessionBindingElement.ReliableMessagingVersion = WS2007ReliableMessagingVersion;
            this.TransactionFlowBindingElement.TransactionProtocol = WS2007TransactionProtocol;
            this.HttpsTransport.MessageSecurityVersion = WS2007MessageSecurityVersion;
        }

        public WS2007FederationHttpBinding(WSFederationHttpSecurityMode securityMode)
            : this(securityMode, false)
        {
        }

        public WS2007FederationHttpBinding(WSFederationHttpSecurityMode securityMode, bool reliableSessionEnabled)
            : base(securityMode, reliableSessionEnabled)
        {
            this.ReliableSessionBindingElement.ReliableMessagingVersion = WS2007ReliableMessagingVersion;
            this.TransactionFlowBindingElement.TransactionProtocol = WS2007TransactionProtocol;
            this.HttpsTransport.MessageSecurityVersion = WS2007MessageSecurityVersion;
        }

        WS2007FederationHttpBinding(WSFederationHttpSecurity security, PrivacyNoticeBindingElement privacy, bool reliableSessionEnabled)
            : base(security, privacy, reliableSessionEnabled)
        {
            this.ReliableSessionBindingElement.ReliableMessagingVersion = WS2007ReliableMessagingVersion;
            this.TransactionFlowBindingElement.TransactionProtocol = WS2007TransactionProtocol;
            this.HttpsTransport.MessageSecurityVersion = WS2007MessageSecurityVersion;
        }

        protected override SecurityBindingElement CreateMessageSecurity()
        {
            return this.Security.CreateMessageSecurity(this.ReliableSession.Enabled, WS2007MessageSecurityVersion);
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
                if (httpsBinding.MessageSecurityVersion.SecurityPolicyVersion != WS2007MessageSecurityVersion.SecurityPolicyVersion)
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

        static bool TryCreateSecurity(SecurityBindingElement sbe, WSFederationHttpSecurityMode mode, HttpTransportSecurity transportSecurity, bool isReliableSession, out WSFederationHttpSecurity security)
        {
            if (!WSFederationHttpSecurity.TryCreate(sbe, mode, transportSecurity, isReliableSession, WS2007MessageSecurityVersion, out security))
                return false;
            // the last check: make sure that security binding element match the incoming security
            return System.ServiceModel.Configuration.SecurityElement.AreBindingsMatching(security.CreateMessageSecurity(isReliableSession, WS2007MessageSecurityVersion), sbe);
        }
    }
}
