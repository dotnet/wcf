// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Channels
{
    internal class TcpChannelFactory<TChannel> : ConnectionOrientedTransportChannelFactory<TChannel>, ITcpChannelFactorySettings
    {
        private static TcpConnectionPoolRegistry s_connectionPoolRegistry = new TcpConnectionPoolRegistry();
        private TimeSpan _leaseTimeout;

        public TcpChannelFactory(TcpTransportBindingElement bindingElement, BindingContext context)
            : base(bindingElement, context,
                    bindingElement.ConnectionPoolSettings.GroupName,
                    bindingElement.ConnectionPoolSettings.IdleTimeout,
                    bindingElement.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint,
                    true)
        {
            _leaseTimeout = bindingElement.ConnectionPoolSettings.LeaseTimeout;
        }

        public TimeSpan LeaseTimeout
        {
            get
            {
                return _leaseTimeout;
            }
        }

        public override string Scheme
        {
            get { return "net.tcp"; }
        }

        internal override IConnectionInitiator GetConnectionInitiator()
        {
#if FEATURE_NETNATIVE
            IConnectionInitiator socketConnectionInitiator = new RTSocketConnectionInitiator(
                ConnectionBufferSize);
#else
            IConnectionInitiator socketConnectionInitiator = new CoreClrSocketConnectionInitiator(
                ConnectionBufferSize);
#endif

            return new BufferedConnectionInitiator(socketConnectionInitiator,
                MaxOutputDelay, ConnectionBufferSize);
        }

        internal override ConnectionPool GetConnectionPool()
        {
            return s_connectionPoolRegistry.Lookup(this);
        }

        internal override void ReleaseConnectionPool(ConnectionPool pool, TimeSpan timeout)
        {
            s_connectionPoolRegistry.Release(pool, timeout);
        }
    }

    internal static class TcpUri
    {
        public const int DefaultPort = 808;
    }
}
