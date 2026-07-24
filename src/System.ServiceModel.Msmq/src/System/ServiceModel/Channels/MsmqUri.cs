// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Globalization;
using System.Net;
using System.Text;

namespace System.ServiceModel.Channels
{
    // Address translation for net.msmq:// and msmq.formatname: URIs.
    // Active Directory (path-name lookup) and the dead-letter-queue resolver
    // from the .NET Framework reference source are intentionally omitted —
    // both rely on infrastructure (MsmqFormatName, DnsCache) that is not
    // yet ported. They can be added later without breaking changes.
    internal static class MsmqUri
    {
        private static IAddressTranslator s_netMsmq;
        private static IAddressTranslator s_srmp;
        private static IAddressTranslator s_srmps;
        private static IAddressTranslator s_formatName;

        internal static IAddressTranslator NetMsmqAddressTranslator
            => s_netMsmq ??= new NetMsmqTranslator();

        internal static IAddressTranslator SrmpAddressTranslator
            => s_srmp ??= new SrmpTranslator();

        internal static IAddressTranslator SrmpsAddressTranslator
            => s_srmps ??= new SrmpSecureTranslator();

        internal static IAddressTranslator FormatNameAddressTranslator
            => s_formatName ??= new FormatNameTranslator();

        internal static string UriToFormatNameByScheme(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }
            if (uri.Scheme == NetMsmqAddressTranslator.Scheme)
            {
                return NetMsmqAddressTranslator.UriToFormatName(uri);
            }
            if (uri.Scheme == FormatNameAddressTranslator.Scheme)
            {
                return FormatNameAddressTranslator.UriToFormatName(uri);
            }
            throw new ArgumentException(SR.Format(SR.MsmqInvalidScheme), nameof(uri));
        }

        internal interface IAddressTranslator
        {
            string Scheme { get; }
            string UriToFormatName(Uri uri);
        }

        private static void AppendQueueName(StringBuilder builder, string relativePath, string separator)
        {
            const string privatePart = "/private";

            if (relativePath.StartsWith("/private$", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(SR.MsmqWrongPrivateQueueSyntax);
            }

            if (relativePath.StartsWith(privatePart, StringComparison.OrdinalIgnoreCase))
            {
                if (privatePart.Length == relativePath.Length)
                {
                    builder.Append("private$").Append(separator);
                    relativePath = "/";
                }
                else if (relativePath[privatePart.Length] == '/')
                {
                    builder.Append("private$").Append(separator);
                    relativePath = relativePath.Substring(privatePart.Length);
                }
            }
            builder.Append(relativePath.Substring(1));
        }

        private static void ValidateNetMsmqUri(Uri uri, string expectedScheme, bool allowPort)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }
            if (uri.Scheme != expectedScheme)
            {
                throw new ArgumentException(SR.MsmqInvalidScheme, nameof(uri));
            }
            if (string.IsNullOrEmpty(uri.Host))
            {
                throw new ArgumentException(SR.MsmqWrongUri, nameof(uri));
            }
            if (!allowPort && uri.Port != -1)
            {
                throw new ArgumentException(SR.MsmqUnexpectedPort, nameof(uri));
            }
        }

        private sealed class NetMsmqTranslator : IAddressTranslator
        {
            public string Scheme => "net.msmq";

            public string UriToFormatName(Uri uri)
            {
                ValidateNetMsmqUri(uri, Scheme, allowPort: false);

                var builder = new StringBuilder("DIRECT=");
                if (string.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase))
                {
                    builder.Append("OS:.");
                }
                else
                {
                    builder.Append(IPAddress.TryParse(uri.Host, out _) ? "TCP:" : "OS:");
                    builder.Append(uri.Host);
                }
                builder.Append('\\');
                AppendQueueName(builder, Uri.UnescapeDataString(uri.PathAndQuery), "\\");
                return builder.ToString();
            }
        }

        private abstract class SrmpTranslatorBase : IAddressTranslator
        {
            public string Scheme => "net.msmq";
            protected abstract string DirectScheme { get; }

            public string UriToFormatName(Uri uri)
            {
                ValidateNetMsmqUri(uri, Scheme, allowPort: true);

                var builder = new StringBuilder("DIRECT=");
                builder.Append(DirectScheme);
                builder.Append(uri.Host);
                if (uri.Port != -1)
                {
                    builder.Append(':').Append(uri.Port.ToString(CultureInfo.InvariantCulture));
                }
                builder.Append("/msmq/");
                AppendQueueName(builder, Uri.UnescapeDataString(uri.PathAndQuery), "/");
                return builder.ToString();
            }
        }

        private sealed class SrmpTranslator : SrmpTranslatorBase
        {
            protected override string DirectScheme => "http://";
        }

        private sealed class SrmpSecureTranslator : SrmpTranslatorBase
        {
            protected override string DirectScheme => "https://";
        }

        private sealed class FormatNameTranslator : IAddressTranslator
        {
            public string Scheme => "msmq.formatname";

            public string UriToFormatName(Uri uri)
            {
                if (uri == null)
                {
                    throw new ArgumentNullException(nameof(uri));
                }
                if (uri.Scheme != Scheme)
                {
                    throw new ArgumentException(SR.MsmqInvalidScheme, nameof(uri));
                }
                return Uri.UnescapeDataString(uri.AbsoluteUri.Substring(Scheme.Length + 1));
            }
        }
    }
}
