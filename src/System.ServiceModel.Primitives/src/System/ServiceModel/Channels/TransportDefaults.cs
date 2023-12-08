// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Net.Security;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Security.Principal;
using System.ServiceModel.Security;
using System.Text;
using System.Xml;

namespace System.ServiceModel.Channels
{
    // If any of the const's are modified in this file, they must also be modified
    // on the Internal.ServiceModel.Primitives contract and implementation assemblies.

    internal static class EncoderDefaults
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

    internal static class TextEncoderDefaults
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
                {
                    return;
                }
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.Format(SRP.MessageTextEncodingNotSupported, charSet), "encoding"));
        }

        public static string EncodingToCharSet(Encoding encoding)
        {
            string webName = encoding.WebName;
            CharSetEncoding[] charSetEncodings = CharSetEncodings;
            for (int i = 0; i < charSetEncodings.Length; i++)
            {
                Encoding enc = charSetEncodings[i].Encoding;
                if (enc == null)
                {
                    continue;
                }

                if (enc.WebName == webName)
                {
                    return charSetEncodings[i].CharSet;
                }
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
                {
                    continue;
                }

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

    internal static class MtomEncoderDefaults
    {
        internal const int MaxBufferSize = 65536;
    }

    internal static class BinaryEncoderDefaults
    {
        public static EnvelopeVersion EnvelopeVersion { get { return EnvelopeVersion.Soap12; } }
        public static BinaryVersion BinaryVersion { get { return BinaryVersion.Version1; } }
        public const int MaxSessionSize = 2048;
    }

    internal static class TransportDefaults
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
#pragma warning disable SYSLIB0039
        public const SslProtocols SslProtocols =
                                           // SSL3 is not supported in CoreFx.
                                           System.Security.Authentication.SslProtocols.Tls |
                                           System.Security.Authentication.SslProtocols.Tls11 |
                                           System.Security.Authentication.SslProtocols.Tls12;
#pragma warning restore SYSLIB0039

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
        internal const string ReliableMessagingVersionString = "WSReliableMessagingFebruary2005";
    }
}
