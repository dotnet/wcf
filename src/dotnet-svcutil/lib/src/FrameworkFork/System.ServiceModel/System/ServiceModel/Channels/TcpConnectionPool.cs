// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Channels
{
    internal class TcpConnectionPoolRegistry : ConnectionPoolRegistry
    {
        public TcpConnectionPoolRegistry()
            : base()
        {
        }

        protected override ConnectionPool CreatePool(IConnectionOrientedTransportChannelFactorySettings settings)
        {
            ITcpChannelFactorySettings tcpSettings = (ITcpChannelFactorySettings)settings;
            return new TcpConnectionPool(tcpSettings);
        }

        private class TcpConnectionPool : ConnectionPool
        {
            public TcpConnectionPool(ITcpChannelFactorySettings settings)
                : base(settings, settings.LeaseTimeout)
            {
            }

            protected override string GetPoolKey(EndpointAddress address, Uri via)
            {
                int port = via.Port;
                if (port == -1)
                {
                    port = TcpUri.DefaultPort;
                }

                string normalizedHost = via.DnsSafeHost.ToUpperInvariant();

                return string.Format(CultureInfo.InvariantCulture, @"[{0}, {1}]", normalizedHost, port);
            }

            public override bool IsCompatible(IConnectionOrientedTransportChannelFactorySettings settings)
            {
                ITcpChannelFactorySettings tcpSettings = (ITcpChannelFactorySettings)settings;
                return (
                    (this.LeaseTimeout == tcpSettings.LeaseTimeout) &&
                    base.IsCompatible(settings)
                    );
            }
        }
    }
}


