// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using SSAuth = System.Security.Authentication;

namespace System.ServiceModel.Channels
{
    internal static class TcpTransportDefaults
    {
        public const long MaxReceivedMessageSize = 65536;
        public const long MaxBufferPoolSize = 512 * 1024;
        public const int MaxBufferSize = (int)MaxReceivedMessageSize;
#pragma warning disable SYSLIB0039
        public const SslProtocols SslProtocols =
                                           // SSL3 is not supported in CoreFx.
                                           SSAuth.SslProtocols.Tls |
                                           SSAuth.SslProtocols.Tls11 |
                                           SSAuth.SslProtocols.Tls12;
 #pragma warning restore SYSLIB0039
        public static TimeSpan ConnectionLeaseTimeout => TimeSpan.FromMinutes(5);
        public const bool PortSharingEnabled = false;
        public const bool TeredoEnabled = false;
    }

    internal static class ConnectionOrientedTransportDefaults
    {
        //public const bool AllowNtlm = SspiSecurityTokenProvider.DefaultAllowNtlm;
        //public const int ConnectionBufferSize = 8192;
        public const string ConnectionPoolGroupName = "default";
        //public const HostNameComparisonMode HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
        public static TimeSpan IdleTimeout => TimeSpan.FromMinutes(2);
        //public const string IdleTimeoutString = "00:02:00";
        //public static TimeSpan ChannelInitializationTimeout { get { return TimeSpanHelper.FromSeconds(30, ChannelInitializationTimeoutString); } }
        //public const string ChannelInitializationTimeoutString = "00:00:30";
        //public const int MaxContentTypeSize = 256;
        public const int MaxOutboundConnectionsPerEndpoint = 10;
        //public const int MaxPendingConnectionsConst = 0;
        //public static TimeSpan MaxOutputDelay { get { return TimeSpanHelper.FromMilliseconds(200, MaxOutputDelayString); } }
        //public const string MaxOutputDelayString = "00:00:00.2";
        //public const int MaxPendingAcceptsConst = 0;
        //public const int MaxViaSize = 2048;
        //public const ProtectionLevel ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
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
