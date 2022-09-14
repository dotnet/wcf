// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public class WS2007HttpBinding : WSHttpBinding
    {
        static readonly MessageSecurityVersion WS2007MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;

        public WS2007HttpBinding()
            : base()
        {
            HttpsTransport.MessageSecurityVersion = WS2007MessageSecurityVersion;
        }

        public WS2007HttpBinding(SecurityMode securityMode)
            : this(securityMode, false)
        {
        }

        public WS2007HttpBinding(SecurityMode securityMode, bool reliableSessionEnabled)
            : base(securityMode, reliableSessionEnabled)
        {
            HttpsTransport.MessageSecurityVersion = WS2007MessageSecurityVersion;
        }

        internal WS2007HttpBinding(WSHttpSecurity security, bool reliableSessionEnabled)
            : base(security, reliableSessionEnabled)
        {
            HttpsTransport.MessageSecurityVersion = WS2007MessageSecurityVersion;
        }

        protected override SecurityBindingElement CreateMessageSecurity()
        {
            return Security.CreateMessageSecurity(ReliableSession.Enabled, WS2007MessageSecurityVersion);
        }
    }
}
