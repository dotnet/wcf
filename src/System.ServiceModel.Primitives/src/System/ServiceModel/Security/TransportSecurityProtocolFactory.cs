// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    internal class TransportSecurityProtocolFactory : SecurityProtocolFactory
    {
        public TransportSecurityProtocolFactory() : base()
        {
        }

        internal TransportSecurityProtocolFactory(TransportSecurityProtocolFactory factory) : base(factory)
        {
        }

        public override bool SupportsDuplex
        {
            get
            {
                return true;
            }
        }

        public override bool SupportsReplayDetection
        {
            get
            {
                return false;
            }
        }

        protected override SecurityProtocol OnCreateSecurityProtocol(EndpointAddress target, Uri via, object listenerSecurityState, TimeSpan timeout)
        {
            return new TransportSecurityProtocol(this, target, via);
        }
    }
}
