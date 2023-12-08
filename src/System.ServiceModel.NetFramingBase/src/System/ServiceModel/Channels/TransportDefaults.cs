// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Security;
using SSAuth = System.Security.Authentication;

namespace System.ServiceModel.Channels
{
    internal class NFTransportDefaults
    {
        public const bool ExtractGroupsForWindowsAccounts = true;
        public const long MaxReceivedMessageSize = 65536;
        public const int MaxBufferSize = (int)MaxReceivedMessageSize;
        public const bool RequireClientCertificate = false;
#pragma warning disable SYSLIB0039
        public const SSAuth.SslProtocols SslProtocols =
        // SSL3 is not supported in CoreFx.
        SSAuth.SslProtocols.Tls |
                                           SSAuth.SslProtocols.Tls11 |
                                           SSAuth.SslProtocols.Tls12;
#pragma warning restore SYSLIB0039
        public static MessageEncoderFactory GetDefaultMessageEncoderFactory()
        {
            return new BinaryMessageEncodingBindingElement().CreateMessageEncoderFactory();
        }
    }

    internal static class ConnectionOrientedTransportDefaults
    {
        public const bool AllowNtlm = true;
        public const int ConnectionBufferSize = 8192;
        //public const string ConnectionPoolGroupName = "default";
        //public const HostNameComparisonMode HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
        //public static TimeSpan IdleTimeout { get { return TimeSpanHelper.FromMinutes(2, IdleTimeoutString); } }
        //public const string IdleTimeoutString = "00:02:00";
        //public static TimeSpan ChannelInitializationTimeout { get { return TimeSpanHelper.FromSeconds(30, ChannelInitializationTimeoutString); } }
        //public const string ChannelInitializationTimeoutString = "00:00:30";
        //public const int MaxContentTypeSize = 256;
        //public const int MaxOutboundConnectionsPerEndpoint = 10;
        //public const int MaxPendingConnectionsConst = 0;
        public static TimeSpan MaxOutputDelay => TimeSpan.FromMilliseconds(200);
        //public const int MaxPendingAcceptsConst = 0;
        //public const int MaxViaSize = 2048;
        public const ProtectionLevel ProtectionLevel = Net.Security.ProtectionLevel.EncryptAndSign;
        public const TransferMode TransferMode = ServiceModel.TransferMode.Buffered;

        //public static int GetMaxConnections()
        //{
        //    return GetMaxPendingConnections();
        //}

        //public static int GetMaxPendingConnections()
        //{
        //    return 12 * Environment.ProcessorCount;
        //}

        //public static int GetMaxPendingAccepts()
        //{
        //    return 2 * Environment.ProcessorCount;
        //}
    }
}
