// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.ServiceModel.Channels
{
    internal sealed class UnixDomainSocketChannelFactory<TChannel> : NetFramingTransportChannelFactory<TChannel>, IConnectionPoolSettings
    {
        public UnixDomainSocketChannelFactory(UnixDomainSocketTransportBindingElement bindingElement, BindingContext context)
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
            get { return "net.uds"; }
        }

        public override IConnectionInitiator GetConnectionInitiator()
        {
            return new SocketConnectionInitiator(ConnectionBufferSize);
        }

        ///TODO Check with Matt
        protected override string GetConnectionPoolKey(EndpointAddress address, Uri via)
        {
            return via.AbsolutePath;
        }

        protected override TChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            if(address.Identity == null)
            {
                var hostIdentity = new DnsEndpointIdentity(address.Uri.Host ?? "localhost");
                var uriBuilder = new UriBuilder(address.Uri);
                uriBuilder.Host = null;
                address = new EndpointAddress(uriBuilder.Uri, hostIdentity,address.Headers.ToArray());
            }

            if(via !=null)
            {
                var uriBuilder = new UriBuilder(via);
                uriBuilder.Host = null;
                via = uriBuilder.Uri;
            }

            return base.OnCreateChannel(address,via);
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
                if (other.GetType().GetGenericTypeDefinition() != typeof(UnixDomainSocketChannelFactory<>))
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
}
