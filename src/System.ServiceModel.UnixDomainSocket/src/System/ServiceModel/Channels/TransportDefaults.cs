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
    internal static class UnixDomainSocketTransportDefaults
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
        public const string ConnectionPoolGroupName = "default";
        public static TimeSpan IdleTimeout => TimeSpan.FromMinutes(2);
        public const int MaxOutboundConnectionsPerEndpoint = 10;
        public const TransferMode TransferMode = ServiceModel.TransferMode.Buffered;
    }
}
