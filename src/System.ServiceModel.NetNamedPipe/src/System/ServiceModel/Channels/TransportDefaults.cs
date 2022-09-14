// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Security;

namespace System.ServiceModel.Channels
{
    internal static class NPTransportDefaults
    {
        public const long MaxReceivedMessageSize = 65536;
        public const long MaxBufferPoolSize = 512 * 1024;
        public const int MaxBufferSize = (int)MaxReceivedMessageSize;
    }

    internal static class ConnectionOrientedTransportDefaults
    {
        public const string ConnectionPoolGroupName = "default";
        public static TimeSpan IdleTimeout => TimeSpan.FromMinutes(2);
        public const int MaxOutboundConnectionsPerEndpoint = 10;
        public const ProtectionLevel ProtectionLevel = Net.Security.ProtectionLevel.EncryptAndSign;
        public const TransferMode TransferMode = ServiceModel.TransferMode.Buffered;
    }
}
