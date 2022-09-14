// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.ServiceModel.Channels
{
    internal sealed class TcpChannelFactory<TChannel> : NetFramingTransportChannelFactory<TChannel>, IConnectionPoolSettings
    {
        public TcpChannelFactory(TcpTransportBindingElement bindingElement, BindingContext context)
            : base(bindingElement, context,
            bindingElement.ConnectionPoolSettings.GroupName,
            bindingElement.ConnectionPoolSettings.IdleTimeout,
            bindingElement.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint)
        {
            LeaseTimeout = bindingElement.ConnectionPoolSettings.LeaseTimeout;
        }

        public TimeSpan LeaseTimeout { get; }

        public override string Scheme
        {
            get { return "net.tcp"; }
        }

        public override IConnectionInitiator GetConnectionInitiator()
        {
            return new SocketConnectionInitiator(ConnectionBufferSize);
        }

        protected override string GetConnectionPoolKey(EndpointAddress address, Uri via)
        {
            int port = via.Port;
            if (port == -1)
            {
                port = TcpUri.DefaultPort;
            }

            string normalizedHost = via.DnsSafeHost.ToUpperInvariant();

            return string.Format(CultureInfo.InvariantCulture, @"[{0}, {1}]", normalizedHost, port);
        }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(IConnectionPoolSettings))
            {
                return (T)(object)this;
            }

            return base.GetProperty<T>();
        }

        T IConnectionPoolSettings.GetConnectionPoolSetting<T>(string settingName)
        {
            if (typeof(T) == typeof(TimeSpan))
            {
                TimeSpan temp;
                switch (settingName)
                {
                    case nameof(LeaseTimeout):
                        temp = LeaseTimeout;
                        break;
                    case nameof(IdleTimeout):
                        temp = IdleTimeout;
                        break;
                    case nameof(MaxOutputDelay):
                        temp = MaxOutputDelay;
                        break;
                    default:
                        return default(T);
                }
                return Unsafe.As<TimeSpan, T>(ref temp);
            }

            if (typeof(T) == typeof(int))
            {
                int temp;
                switch (settingName)
                {
                    case nameof(ConnectionBufferSize):
                        temp = ConnectionBufferSize;
                        break;
                    case nameof(MaxOutboundConnectionsPerEndpoint):
                        temp = MaxOutboundConnectionsPerEndpoint;
                        break;
                    default:
                        return default(T);
                }
                return Unsafe.As<int, T>(ref temp);
            }

            if (typeof(T) == typeof(string))
            {
                if (settingName == nameof(ConnectionPoolGroupName))
                    return (T)(object)ConnectionPoolGroupName;
            }

            return default(T);
        }

        bool IConnectionPoolSettings.IsCompatible(IConnectionPoolSettings other)
        {
            // Other must be a TcpChannelFactory
            if (other.GetType().IsConstructedGenericType)
            {
                if (other.GetType().GetGenericTypeDefinition() != typeof(TcpChannelFactory<>))
                {
                    return false;
                }
            }
             
            return (LeaseTimeout == other.GetConnectionPoolSetting<TimeSpan>(nameof(LeaseTimeout))) &&
                   (ConnectionPoolGroupName == other.GetConnectionPoolSetting<string>(nameof(ConnectionPoolGroupName))) &&
                   (ConnectionBufferSize == other.GetConnectionPoolSetting<int>(nameof(ConnectionBufferSize))) &&
                   (MaxOutboundConnectionsPerEndpoint == other.GetConnectionPoolSetting<int>(nameof(MaxOutboundConnectionsPerEndpoint))) &&
                   (IdleTimeout == other.GetConnectionPoolSetting<TimeSpan>(nameof(IdleTimeout))) &&
                   (MaxOutputDelay == other.GetConnectionPoolSetting<TimeSpan>(nameof(MaxOutputDelay)));
        }
    }

    internal static class TcpUri
    {
        public const int DefaultPort = 808;
    }
}
