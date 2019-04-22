// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Channels
{
    internal class TcpChannelFactory<TChannel> : ConnectionOrientedTransportChannelFactory<TChannel>, ITcpChannelFactorySettings
    {
        private static TcpConnectionPoolRegistry s_connectionPoolRegistry = new TcpConnectionPoolRegistry();

        public TcpChannelFactory(TcpTransportBindingElement bindingElement, BindingContext context)
            : base(bindingElement, context,
                    bindingElement.ConnectionPoolSettings.GroupName,
                    bindingElement.ConnectionPoolSettings.IdleTimeout,
                    bindingElement.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint,
                    true)
        {
            LeaseTimeout = bindingElement.ConnectionPoolSettings.LeaseTimeout;
        }

        public TimeSpan LeaseTimeout { get; }

        public override string Scheme
        {
            get { return "net.tcp"; }
        }

        internal override IConnectionInitiator GetConnectionInitiator()
        {
            IConnectionInitiator socketConnectionInitiator = new SocketConnectionInitiator(
                ConnectionBufferSize);

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
