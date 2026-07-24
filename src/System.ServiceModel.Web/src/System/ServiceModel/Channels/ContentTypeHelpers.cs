// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;

namespace System.ServiceModel.Channels
{
    // Helpers for matching content-type headers to encodings.
    // Mirrors CoreWCF.Runtime.ContentTypeHelpers (client subset).
    internal static class ContentTypeHelpers
    {
        public static Encoding[] GetSupportedEncodings()
        {
            Encoding[] supported = TextEncoderDefaults.SupportedEncodings;
            Encoding[] copy = new Encoding[supported.Length];
            Array.Copy(supported, copy, supported.Length);
            return copy;
        }

        public static Encoding GetEncodingFromContentType(string contentType, ContentEncoding[] contentEncodingMap)
        {
            if (contentType == null)
            {
                return null;
            }

            for (int i = 0; i < contentEncodingMap.Length; i++)
            {
                if (contentType.Equals(contentEncodingMap[i].contentType, StringComparison.OrdinalIgnoreCase))
                {
                    return contentEncodingMap[i].encoding;
                }
            }

            // Fall back to parsing the charset parameter if present.
            try
            {
                System.Net.Http.Headers.MediaTypeHeaderValue parsed =
                    System.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType);
                string charset = parsed.CharSet;
                if (!string.IsNullOrEmpty(charset))
                {
                    if (TextEncoderDefaults.TryGetEncoding(charset, out Encoding encoding))
                    {
                        return encoding;
                    }
                }
            }
            catch (FormatException)
            {
                // Malformed content-type: caller will surface the error.
            }

            return null;
        }
    }
}
