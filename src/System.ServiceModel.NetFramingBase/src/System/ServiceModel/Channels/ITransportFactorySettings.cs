// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ServiceModel.Channels
{
    internal interface IConnectionOrientedConnectionSettings
    {
        int ConnectionBufferSize { get; }
        TimeSpan MaxOutputDelay { get; }
        TimeSpan IdleTimeout { get; }
    }

    internal interface IConnectionOrientedTransportFactorySettings : ITransportFactorySettings, IConnectionOrientedConnectionSettings
    {
        int MaxBufferSize { get; }
        StreamUpgradeProvider Upgrade { get; }
        TransferMode TransferMode { get; }
    }

    internal interface IConnectionOrientedTransportChannelFactorySettings : IConnectionOrientedTransportFactorySettings
    {
        string ConnectionPoolGroupName { get; }
        int MaxOutboundConnectionsPerEndpoint { get; }
        string GetConnectionPoolKey(EndpointAddress address, Uri via);
    }
}
