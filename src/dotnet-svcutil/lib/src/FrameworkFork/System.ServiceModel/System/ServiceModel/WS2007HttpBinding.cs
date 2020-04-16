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

    public class WS2007HttpBinding : WSHttpBinding
    {
        private static readonly ReliableMessagingVersion s_WS2007ReliableMessagingVersion = ReliableMessagingVersion.WSReliableMessaging11;
        private static readonly TransactionProtocol s_WS2007TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;
        private static readonly MessageSecurityVersion s_WS2007MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;

        public WS2007HttpBinding(string configName)
            : this()
        {
            // TODO: ApplyConfiguration(configName);
        }

        public WS2007HttpBinding()
            : base()
        {
            this.ReliableSessionBindingElement.ReliableMessagingVersion = s_WS2007ReliableMessagingVersion;
            this.TransactionFlowBindingElement.TransactionProtocol = s_WS2007TransactionProtocol;
            this.HttpsTransport.MessageSecurityVersion = s_WS2007MessageSecurityVersion;
        }

        public WS2007HttpBinding(SecurityMode securityMode)
            : this(securityMode, false)
        {
        }

        public WS2007HttpBinding(SecurityMode securityMode, bool reliableSessionEnabled)
            : base(securityMode, reliableSessionEnabled)
        {
            this.ReliableSessionBindingElement.ReliableMessagingVersion = s_WS2007ReliableMessagingVersion;
            this.TransactionFlowBindingElement.TransactionProtocol = s_WS2007TransactionProtocol;
            this.HttpsTransport.MessageSecurityVersion = s_WS2007MessageSecurityVersion;
        }

        internal WS2007HttpBinding(WSHttpSecurity security, bool reliableSessionEnabled)
            : base(security, reliableSessionEnabled)
        {
            this.ReliableSessionBindingElement.ReliableMessagingVersion = s_WS2007ReliableMessagingVersion;
            this.TransactionFlowBindingElement.TransactionProtocol = s_WS2007TransactionProtocol;
            this.HttpsTransport.MessageSecurityVersion = s_WS2007MessageSecurityVersion;
        }
        protected override SecurityBindingElement CreateMessageSecurity()
        {
            return this.Security.CreateMessageSecurity(this.ReliableSession.Enabled, s_WS2007MessageSecurityVersion);
        }

        // This is effectively just a copy of WSHttpBinding.TryCreate(), only it news up the 2007 version
        internal new static bool TryCreate(SecurityBindingElement sbe, TransportBindingElement transport, ReliableSessionBindingElement rsbe, TransactionFlowBindingElement tfbe, out Binding binding)
        {
            bool isReliableSession = (rsbe != null);
            binding = null;

            // reverse GetTransport
            HttpTransportSecurity transportSecurity = WSHttpSecurity.GetDefaultHttpTransportSecurity();
            UnifiedSecurityMode mode;
            if (!WSHttpBinding.GetSecurityModeFromTransport(transport, transportSecurity, out mode))
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

            WSHttpSecurity security;
            if (WS2007HttpBinding.TryCreateSecurity(sbe, mode, transportSecurity, isReliableSession, out security))
            {
                WS2007HttpBinding ws2007HttpBinding = new WS2007HttpBinding(security, isReliableSession);

                bool allowCookies;
                if (!WSHttpBinding.TryGetAllowCookiesFromTransport(transport, out allowCookies))
                {
                    return false;
                }

                ws2007HttpBinding.AllowCookies = allowCookies;
                binding = ws2007HttpBinding;
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

        // This is effectively just a copy of WSHttpBinding.TryCreateSecurity(), only it passes the 2007 security version
        private static bool TryCreateSecurity(SecurityBindingElement sbe, UnifiedSecurityMode mode, HttpTransportSecurity transportSecurity, bool isReliableSession, out WSHttpSecurity security)
        {
            if (!WSHttpSecurity.TryCreate(sbe, mode, transportSecurity, isReliableSession, out security))
                return false;
            // the last check: make sure that security binding element match the incoming security
            return System.ServiceModel.Configuration.SecurityElement.AreBindingsMatching(security.CreateMessageSecurity(isReliableSession, s_WS2007MessageSecurityVersion), sbe);
        }
    }
}
