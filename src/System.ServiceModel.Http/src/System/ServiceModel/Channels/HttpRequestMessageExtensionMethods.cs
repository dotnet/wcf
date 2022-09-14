// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace System.ServiceModel.Channels
{
    public static class HttpRequestMessageExtensionMethods
    {
        private const string MessageHeadersPropertyKey = "System.ServiceModel.Channels.MessageHeaders";

        internal static HashSet<string> WellKnownContentHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                { "Content-Disposition", "Content-Encoding", "Content-Language", "Content-Length", "Content-Location",
                  "Content-MD5", "Content-Range", "Content-Type", "Expires", "Last-Modified" };

        internal static void AddHeaderWithoutValidation(this HttpHeaders httpHeaders, KeyValuePair<string, IEnumerable<string>> header)
        {
            Contract.Assert(httpHeaders != null, "httpHeaders should not be null.");
            if (!httpHeaders.TryAddWithoutValidation(header.Key, header.Value))
            {
                throw FxTrace.Exception.AsError(new InvalidOperationException(SR.Format(
                                SR.CopyHttpHeaderFailed,
                                header.Key,
                                header.Value,
                                httpHeaders.GetType().Name)));
            }
        }

        private static void CopyProperties(MessageProperties messageProperties, IDictionary<string, object> properties)
        {
            Contract.Assert(messageProperties != null, "The 'messageProperties' parameter should not be null.");
            Contract.Assert(properties != null, "The 'properties' parameter should not be null.");

            foreach (KeyValuePair<string, object> property in messageProperties)
            {
                object value = property.Value;
                string key = property.Key;

                if ((value is HttpRequestMessageProperty && string.Equals(key, HttpRequestMessageProperty.Name, StringComparison.OrdinalIgnoreCase)) ||
                    (value is HttpResponseMessageProperty && string.Equals(key, HttpResponseMessageProperty.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                properties[key] = value;
            }
        }

        // We could potentially be passed an HttpRequestMessage without Content set. We presume that we have an HttpContent
        // in many places so this ensures we have one and removes the need for special casing in many places.
        public static bool CreateContentIfNull(this HttpRequestMessage httpRequestMessage)
        {
            Contract.Assert(httpRequestMessage != null, "The 'httpRequestMessage' parameter should never be null.");

            if (httpRequestMessage.Content == null)
            {
                httpRequestMessage.Content = new ByteArrayContent(Array.Empty<byte>());
                return true;
            }

            return false;
        }

        internal static void MergeWebHeaderCollection(this HttpRequestMessage requestMessage, WebHeaderCollection headersToMerge)
        {
            requestMessage.CreateContentIfNull();
            MergeWebHeaderCollectionWithHttpHeaders(headersToMerge, requestMessage.Headers, requestMessage.Content.Headers);
        }

        internal static void MergeWebHeaderCollectionWithHttpHeaders(WebHeaderCollection headersToMerge, HttpHeaders mainHeaders, HttpHeaders contentHeaders)
        {
            foreach (string headerKey in headersToMerge.AllKeys)
            {
                if (WellKnownContentHeaders.Contains(headerKey))
                {
                    contentHeaders.TryAddWithoutValidation(headerKey, headersToMerge[headerKey]);
                }
                else
                {
                    mainHeaders.TryAddWithoutValidation(headerKey, headersToMerge[headerKey]);
                }
            }
        }

        internal static WebHeaderCollection ToWebHeaderCollection(this HttpRequestMessage httpRequest)
        {
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers = httpRequest.Headers;
            if (httpRequest.Content != null)
            {
                headers = headers.Concat(httpRequest.Content.Headers);
            }
            return headers.ToWebHeaderCollection();
        }

        internal static WebHeaderCollection ToWebHeaderCollection(this IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            var webHeaders = new WebHeaderCollection();
            foreach (var header in headers)
            {
                webHeaders[header.Key] = String.Join(",", header.Value);
            }
            return webHeaders;
        }
    }
}
