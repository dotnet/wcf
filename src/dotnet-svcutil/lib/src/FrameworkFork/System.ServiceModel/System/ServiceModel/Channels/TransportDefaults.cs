// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Net.Security;
using System.Net.WebSockets;
using System.Runtime;
using System.Security.Authentication;
using System.Security.Principal;
using System.ServiceModel.Security;
using System.Text;
using Microsoft.Xml;

namespace System.ServiceModel.Channels
{
    // If any of the const's are modified in this file, they must also be modified
    // on the Internal.ServiceModel.Primitives contract and implementation assemblies.

    internal static class TransactionFlowDefaults
    {
        internal const TransactionFlowOption IssuedTokens = TransactionFlowOption.NotAllowed;
        internal const bool Transactions = false;
        internal static TransactionProtocol TransactionProtocol = System.ServiceModel.TransactionProtocol.OleTransactions;
    }

    public static class EncoderDefaults
    {
        public const int MaxReadPoolSize = 64;
        public const int MaxWritePoolSize = 16;

        public const int MaxDepth = 32;
        public const int MaxStringContentLength = 8192;
        public const int MaxArrayLength = 16384;
        public const int MaxBytesPerRead = 4096;
        public const int MaxNameTableCharCount = 16384;

        public const int BufferedReadDefaultMaxDepth = 128;
        public const int BufferedReadDefaultMaxStringContentLength = Int32.MaxValue;
        public const int BufferedReadDefaultMaxArrayLength = Int32.MaxValue;
        public const int BufferedReadDefaultMaxBytesPerRead = Int32.MaxValue;
        public const int BufferedReadDefaultMaxNameTableCharCount = Int32.MaxValue;

        public const CompressionFormat DefaultCompressionFormat = CompressionFormat.None;

        public static readonly XmlDictionaryReaderQuotas ReaderQuotas = new XmlDictionaryReaderQuotas();

        public static bool IsDefaultReaderQuotas(XmlDictionaryReaderQuotas quotas)
        {
            return quotas.ModifiedQuotas == 0x00;
        }
    }

    public static class TextEncoderDefaults
    {
        public static readonly Encoding Encoding = Encoding.GetEncoding(TextEncoderDefaults.EncodingString);
        public const string EncodingString = "utf-8";
        public static readonly Encoding[] SupportedEncodings = new Encoding[] { Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode };
        public const string MessageVersionString = "Soap12WSAddressing10";
        // Desktop: System.ServiceModel.Configuration.ConfigurationStrings.Soap12WSAddressing10;
        public static readonly CharSetEncoding[] CharSetEncodings = new CharSetEncoding[]
        {
            new CharSetEncoding("utf-8", Encoding.UTF8),
            new CharSetEncoding("utf-16LE", Encoding.Unicode),
            new CharSetEncoding("utf-16BE", Encoding.BigEndianUnicode),
            new CharSetEncoding("utf-16", null),   // Ignore.  Ambiguous charSet, so autodetect.
            new CharSetEncoding(null, null),       // CharSet omitted, so autodetect.
        };

        public static void ValidateEncoding(Encoding encoding)
        {
            string charSet = encoding.WebName;
            Encoding[] supportedEncodings = SupportedEncodings;
            for (int i = 0; i < supportedEncodings.Length; i++)
            {
                if (charSet == supportedEncodings[i].WebName)
                    return;
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(string.Format(SRServiceModel.MessageTextEncodingNotSupported, charSet), "encoding"));
        }

        public static string EncodingToCharSet(Encoding encoding)
        {
            string webName = encoding.WebName;
            CharSetEncoding[] charSetEncodings = CharSetEncodings;
            for (int i = 0; i < charSetEncodings.Length; i++)
            {
                Encoding enc = charSetEncodings[i].Encoding;
                if (enc == null)
                    continue;

                if (enc.WebName == webName)
                    return charSetEncodings[i].CharSet;
            }
            return null;
        }

        public static bool TryGetEncoding(string charSet, out Encoding encoding)
        {
            CharSetEncoding[] charSetEncodings = CharSetEncodings;

            // Quick check for exact equality
            for (int i = 0; i < charSetEncodings.Length; i++)
            {
                if (charSetEncodings[i].CharSet == charSet)
                {
                    encoding = charSetEncodings[i].Encoding;
                    return true;
                }
            }

            // Check for case insensitive match
            for (int i = 0; i < charSetEncodings.Length; i++)
            {
                string compare = charSetEncodings[i].CharSet;
                if (compare == null)
                    continue;

                if (compare.Equals(charSet, StringComparison.OrdinalIgnoreCase))
                {
                    encoding = charSetEncodings[i].Encoding;
                    return true;
                }
            }

            encoding = null;
            return false;
        }

        public class CharSetEncoding
        {
            public string CharSet;
            public Encoding Encoding;

            public CharSetEncoding(string charSet, Encoding enc)
            {
                CharSet = charSet;
                Encoding = enc;
            }
        }
    }

    public static class MtomEncoderDefaults
    {
        internal const int MaxBufferSize = 65536;
    }

    public static class BinaryEncoderDefaults
    {
        public static EnvelopeVersion EnvelopeVersion { get { return EnvelopeVersion.Soap12; } }
        public static BinaryVersion BinaryVersion { get { return BinaryVersion.Version1; } }
        public const int MaxSessionSize = 2048;
    }

    public static class TransportDefaults
    {
        public const bool ExtractGroupsForWindowsAccounts = SspiSecurityTokenProvider.DefaultExtractWindowsGroupClaims;
        public const HostNameComparisonMode HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.Exact;
        public const TokenImpersonationLevel ImpersonationLevel = TokenImpersonationLevel.Identification;
        public const bool ManualAddressing = false;
        public const long MaxReceivedMessageSize = 65536;
        public const int MaxDrainSize = (int)MaxReceivedMessageSize;
        public const long MaxBufferPoolSize = 512 * 1024;
        public const int MaxBufferSize = (int)MaxReceivedMessageSize;
        public const bool RequireClientCertificate = false;
        public const int MaxFaultSize = MaxBufferSize;
        public const int MaxSecurityFaultSize = 16384;
        public const SslProtocols SslProtocols =
                                           // SSL3 is not supported in CoreFx.
                                           System.Security.Authentication.SslProtocols.Tls |
                                           System.Security.Authentication.SslProtocols.Tls11 |
                                           System.Security.Authentication.SslProtocols.Tls12;

        // Calling CreateFault on an incoming message can expose some DoS-related security 
        // vulnerabilities when a service is in streaming mode.  See MB 47592 for more details. 
        // The RM protocol service does not use streaming mode on any of its bindings, so the
        // message we have in hand has already passed the binding’s MaxReceivedMessageSize check.
        // Custom transports can use RM so int.MaxValue is dangerous.
        public const int MaxRMFaultSize = (int)MaxReceivedMessageSize;

        public static MessageEncoderFactory GetDefaultMessageEncoderFactory()
        {
            return new BinaryMessageEncodingBindingElement().CreateMessageEncoderFactory();
        }
    }

    public static class ConnectionOrientedTransportDefaults
    {
        public const bool AllowNtlm = SspiSecurityTokenProvider.DefaultAllowNtlm;
        public const int ConnectionBufferSize = 8192;
        public const string ConnectionPoolGroupName = "default";
        public const HostNameComparisonMode HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
        public static TimeSpan IdleTimeout { get { return TimeSpanHelper.FromMinutes(2, IdleTimeoutString); } }
        public const string IdleTimeoutString = "00:02:00";
        public static TimeSpan ChannelInitializationTimeout { get { return TimeSpanHelper.FromSeconds(30, ChannelInitializationTimeoutString); } }
        public const string ChannelInitializationTimeoutString = "00:00:30";
        public const int MaxContentTypeSize = 256;
        public const int MaxOutboundConnectionsPerEndpoint = 10;
        public const int MaxPendingConnectionsConst = 0;
        public static TimeSpan MaxOutputDelay { get { return TimeSpanHelper.FromMilliseconds(200, MaxOutputDelayString); } }
        public const string MaxOutputDelayString = "00:00:00.2";
        public const int MaxPendingAcceptsConst = 0;
        public const int MaxViaSize = 2048;
        public const ProtectionLevel ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
        public const TransferMode TransferMode = System.ServiceModel.TransferMode.Buffered;

        public static int GetMaxConnections()
        {
            return GetMaxPendingConnections();
        }

        public static int GetMaxPendingConnections()
        {
            return 12 * Environment.ProcessorCount;
        }

        public static int GetMaxPendingAccepts()
        {
            return 2 * Environment.ProcessorCount;
        }
    }

    public static class TcpTransportDefaults
    {
        public const int ListenBacklogConst = 0;
        public static TimeSpan ConnectionLeaseTimeout { get { return TimeSpanHelper.FromMinutes(5, TcpTransportDefaults.ConnectionLeaseTimeoutString); } }
        public const string ConnectionLeaseTimeoutString = "00:05:00";
        public const bool PortSharingEnabled = false;
        public const bool TeredoEnabled = false;

        private const int ListenBacklogPre45 = 10;

        public static int GetListenBacklog()
        {
            return 12 * Environment.ProcessorCount;
        }
    }

    public static class ApplicationContainerSettingsDefaults
    {
        public const string CurrentUserSessionDefaultString = "CurrentSession";
        public const string Session0ServiceSessionString = "ServiceSession";
        public const string PackageFullNameDefaultString = null;

        /// <summary>
        /// The current session will be used for resource lookup.
        /// </summary>
        public const int CurrentSession = -1;

        /// <summary>
        /// Session 0 is the NT Service session
        /// </summary>
        public const int ServiceSession = 0;
    }

    public static class HttpTransportDefaults
    {
        public const bool AllowCookies = false;
        public const AuthenticationSchemes AuthenticationScheme = AuthenticationSchemes.Anonymous;
        public const bool DecompressionEnabled = true;
        public const HostNameComparisonMode HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
        public const bool KeepAliveEnabled = true;
        public const string Realm = "";
        public const TransferMode TransferMode = System.ServiceModel.TransferMode.Buffered;
        public const bool UnsafeConnectionNtlmAuthentication = false;
        public const bool UseDefaultWebProxy = true;
        public const string UpgradeHeader = "Upgrade";
        public const string ConnectionHeader = "Connection";
        public const HttpMessageHandlerFactory MessageHandlerFactory = null;

        public static TimeSpan RequestInitializationTimeout { get { return TimeSpanHelper.FromMilliseconds(0, RequestInitializationTimeoutString); } }
        public const string RequestInitializationTimeoutString = "00:00:00";

        // We use 0 as the default value of the MaxPendingAccepts property on HttpTransportBindingElement. In 4.5 we always
        // use 10 under the hood if the default value is picked. In future releases, we could adjust the underlying default
        // value when we have the dynamic expending pattern of BeginGetContext call implemented and the heap fragmentation issue
        // from NCL layer solved.
        private const int PendingAcceptsConstant = 10;
        public const int DefaultMaxPendingAccepts = 0;
        public const int MaxPendingAcceptsUpperLimit = 100000;
        public static int GetEffectiveMaxPendingAccepts(int maxPendingAccepts)
        {
            return maxPendingAccepts == HttpTransportDefaults.DefaultMaxPendingAccepts ?
                                        PendingAcceptsConstant :
                                        maxPendingAccepts;
        }


        public static WebSocketTransportSettings GetDefaultWebSocketTransportSettings()
        {
            return new WebSocketTransportSettings();
        }

        public static MessageEncoderFactory GetDefaultMessageEncoderFactory()
        {
            return new TextMessageEncoderFactory(MessageVersion.Default, TextEncoderDefaults.Encoding, EncoderDefaults.MaxReadPoolSize, EncoderDefaults.MaxWritePoolSize, EncoderDefaults.ReaderQuotas);
        }
    }

    public static class NetTcpDefaults
    {
        public const MessageCredentialType MessageSecurityClientCredentialType = MessageCredentialType.Windows;
        internal const bool TransactionsEnabled = false;
    }

    public static class OneWayDefaults
    {
        public static TimeSpan IdleTimeout { get { return TimeSpanHelper.FromMinutes(2, IdleTimeoutString); } }
        public const string IdleTimeoutString = "00:02:00";
        public const int MaxOutboundChannelsPerEndpoint = 10;
        public static TimeSpan LeaseTimeout { get { return TimeSpanHelper.FromMinutes(10, LeaseTimeoutString); } }
        public const string LeaseTimeoutString = "00:10:00";
        public const int MaxAcceptedChannels = 10;
        public const bool PacketRoutable = false;
    }

    internal static class ReliableSessionDefaults
    {
        internal const string AcknowledgementIntervalString = "00:00:00.2";
        internal static TimeSpan AcknowledgementInterval { get { return TimeSpanHelper.FromMilliseconds(200, AcknowledgementIntervalString); } }
        internal const bool Enabled = false;
        internal const bool FlowControlEnabled = true;
        internal const string InactivityTimeoutString = "00:10:00";
        internal static TimeSpan InactivityTimeout { get { return TimeSpanHelper.FromMinutes(10, InactivityTimeoutString); } }
        internal const int MaxPendingChannels = 4;
        internal const int MaxRetryCount = 8;
        internal const int MaxTransferWindowSize = 8;
        internal const bool Ordered = true;
        internal static ReliableMessagingVersion ReliableMessagingVersion { get { return System.ServiceModel.ReliableMessagingVersion.WSReliableMessagingFebruary2005; } }
        internal const string ReliableMessagingVersionString = "WSReliableMessagingFebruary2005";  // TODO: System.ServiceModel.Configuration.ConfigurationStrings.WSReliableMessagingFebruary2005;
    }

    public static class BasicHttpBindingDefaults
    {
        public const BasicHttpMessageCredentialType MessageSecurityClientCredentialType = BasicHttpMessageCredentialType.UserName;
        public const WSMessageEncoding MessageEncoding = WSMessageEncoding.Text;
        public const TransferMode TransferMode = System.ServiceModel.TransferMode.Buffered;
        public static Encoding TextEncoding
        {
            get { return TextEncoderDefaults.Encoding; }
        }
    }

    internal static class WSHttpBindingDefaults
    {
        internal const WSMessageEncoding MessageEncoding = WSMessageEncoding.Text;
    }

    public static class WebSocketDefaults
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

    public static class NetHttpBindingDefaults
    {
        public const NetHttpMessageEncoding MessageEncoding = NetHttpMessageEncoding.Binary;
        public const WebSocketTransportUsage TransportUsage = WebSocketTransportUsage.WhenDuplex;
    }
}
