// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Channels
{
    public interface IConnectionOrientedConnectionSettings
    {
        int ConnectionBufferSize { get; }
        TimeSpan MaxOutputDelay { get; }
        TimeSpan IdleTimeout { get; }
    }

    internal interface IConnectionOrientedListenerSettings : IConnectionOrientedConnectionSettings
    {
        TimeSpan ChannelInitializationTimeout { get; }
        int MaxPendingConnections { get; }
        int MaxPendingAccepts { get; }
        int MaxPooledConnections { get; }
    }

    public interface ITransportFactorySettings : IDefaultCommunicationTimeouts
    {
        bool ManualAddressing { get; }
        BufferManager BufferManager { get; }
        long MaxReceivedMessageSize { get; }
        MessageEncoderFactory MessageEncoderFactory { get; }
        MessageVersion MessageVersion { get; }
    }

    public interface IConnectionOrientedTransportFactorySettings : ITransportFactorySettings, IConnectionOrientedConnectionSettings
    {
        int MaxBufferSize { get; }
        StreamUpgradeProvider Upgrade { get; }
        TransferMode TransferMode { get; }
        // Audit
        //ServiceSecurityAuditBehavior AuditBehavior { get; }
    }

    public interface IConnectionOrientedTransportChannelFactorySettings : IConnectionOrientedTransportFactorySettings
    {
        string ConnectionPoolGroupName { get; }
        int MaxOutboundConnectionsPerEndpoint { get; }
    }

    public interface ITcpChannelFactorySettings : IConnectionOrientedTransportChannelFactorySettings
    {
        TimeSpan LeaseTimeout { get; }
    }

    internal interface IHttpTransportFactorySettings : ITransportFactorySettings
    {
        int MaxBufferSize { get; }
        TransferMode TransferMode { get; }
    }

    internal interface IPipeTransportFactorySettings : IConnectionOrientedTransportChannelFactorySettings
    {
        NamedPipeSettings PipeSettings { get; }
    }
}
