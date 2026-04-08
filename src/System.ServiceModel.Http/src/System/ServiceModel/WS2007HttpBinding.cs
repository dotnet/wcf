// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public class WS2007HttpBinding : WSHttpBinding
    {
        private static readonly ReliableMessagingVersion s_ws2007ReliableMessagingVersion = ReliableMessagingVersion.WSReliableMessaging11;
        private static readonly TransactionProtocol s_ws2007TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;
        private static readonly MessageSecurityVersion s_ws2007MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;

        public WS2007HttpBinding() : base()
        {
            ReliableSessionBindingElement.ReliableMessagingVersion = s_ws2007ReliableMessagingVersion;
            TransactionFlowBindingElement.TransactionProtocol = s_ws2007TransactionProtocol;
            HttpsTransport.MessageSecurityVersion = s_ws2007MessageSecurityVersion;
        }

        public WS2007HttpBinding(SecurityMode securityMode)
            : this(securityMode, false)
        {
        }

        public WS2007HttpBinding(SecurityMode securityMode, bool reliableSessionEnabled)
            : base(securityMode, reliableSessionEnabled)
        {
            ReliableSessionBindingElement.ReliableMessagingVersion = s_ws2007ReliableMessagingVersion;
            TransactionFlowBindingElement.TransactionProtocol = s_ws2007TransactionProtocol;
            HttpsTransport.MessageSecurityVersion = s_ws2007MessageSecurityVersion;
        }

        internal WS2007HttpBinding(WSHttpSecurity security, bool reliableSessionEnabled)
            : base(security, reliableSessionEnabled)
        {
            ReliableSessionBindingElement.ReliableMessagingVersion = s_ws2007ReliableMessagingVersion;
            TransactionFlowBindingElement.TransactionProtocol = s_ws2007TransactionProtocol;
            HttpsTransport.MessageSecurityVersion = s_ws2007MessageSecurityVersion;
        }

        protected override SecurityBindingElement CreateMessageSecurity()
        {
            return Security.CreateMessageSecurity(ReliableSession.Enabled, s_ws2007MessageSecurityVersion);
        }
    }
}
