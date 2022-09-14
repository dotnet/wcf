// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace System.ServiceModel.Channels
{
    internal static class HttpTransportDefaults
    {
        internal const bool AllowCookies = false;
        internal const AuthenticationSchemes AuthenticationScheme = AuthenticationSchemes.Anonymous;
        internal const bool BypassProxyOnLocal = false;
        internal const bool DecompressionEnabled = true;
        internal const HostNameComparisonMode HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
        internal const bool KeepAliveEnabled = true;
        internal const IWebProxy Proxy = null;
        internal const Uri ProxyAddress = null;
        internal const AuthenticationSchemes ProxyAuthenticationScheme = AuthenticationSchemes.Anonymous;
        internal const string Realm = "";
        internal const TransferMode TransferMode = System.ServiceModel.TransferMode.Buffered;
        internal const bool UnsafeConnectionNtlmAuthentication = false;
        internal const bool UseDefaultWebProxy = true;
        internal const string UpgradeHeader = "Upgrade";
        internal const string ConnectionHeader = "Connection";
        internal const HttpMessageHandlerFactory MessageHandlerFactory = null;

        internal static TimeSpan RequestInitializationTimeout => TimeSpanHelper.FromMilliseconds(0, RequestInitializationTimeoutString);
        internal const string RequestInitializationTimeoutString = "00:00:00";

        internal const int DefaultMaxPendingAccepts = 0;
        internal const int MaxPendingAcceptsUpperLimit = 100000;

        internal static WebSocketTransportSettings GetDefaultWebSocketTransportSettings()
        {
            return new WebSocketTransportSettings();
        }

        internal static MessageEncoderFactory GetDefaultMessageEncoderFactory()
        {
            return new TextMessageEncoderFactory(MessageVersion.Default, TextEncoderDefaults.Encoding, EncoderDefaults.MaxReadPoolSize, EncoderDefaults.MaxWritePoolSize, EncoderDefaults.ReaderQuotas);
        }
    }

    internal static class BasicHttpBindingDefaults
    {
        public const BasicHttpMessageCredentialType MessageSecurityClientCredentialType = BasicHttpMessageCredentialType.UserName;
        public const WSMessageEncoding MessageEncoding = WSMessageEncoding.Text;
        public const TransferMode TransferMode = System.ServiceModel.TransferMode.Buffered;
        public static Encoding TextEncoding
        {
            get { return TextEncoderDefaults.Encoding; }
        }
    }

    internal static class WebSocketDefaults
    {
        public const WebSocketTransportUsage TransportUsage = WebSocketTransportUsage.Never;
        public const bool CreateNotificationOnConnection = false;
        public const string DefaultKeepAliveIntervalString = "00:00:00";
        public static readonly TimeSpan DefaultKeepAliveInterval = TimeSpanHelper.FromSeconds(0, DefaultKeepAliveIntervalString);

        public const int BufferSize = 16 * 1024;
        public const int MinReceiveBufferSize = 256;
        public const int MinSendBufferSize = 16;
        internal const WebSocketMessageType DefaultWebSocketMessageType = WebSocketMessageType.Binary;

        public const string SubProtocol = null;

        public const string WebSocketConnectionHeaderValue = "Upgrade";
        public const string WebSocketUpgradeHeaderValue = "websocket";
    }

    internal static class NetHttpBindingDefaults
    {
        public const NetHttpMessageEncoding MessageEncoding = NetHttpMessageEncoding.Binary;
        public const WebSocketTransportUsage TransportUsage = WebSocketTransportUsage.WhenDuplex;
    }
}
