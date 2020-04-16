// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime;

namespace System.ServiceModel.Channels
{
    internal static class HttpResponseMessageExtensionMethods
    {
        internal static void CopyPropertiesFromMessage(this HttpResponseMessage httpResponseMessage, Message message)
        {
            Fx.Assert(httpResponseMessage != null, "The 'httpRequestMessage' parameter should not be null.");
            Fx.Assert(message != null, "The 'message' parameter should not be null.");

            HttpRequestMessage request = httpResponseMessage.RequestMessage;
            if (request != null)
            {
                request.CopyPropertiesFromMessage(message);
            }
        }

        // It is possible to receive an HttpResponseMessage without Content set from HttpClient. We presume that we have an HttpContent
        // in many places so this ensures we have one and removes the need for special casing in many places.
        internal static bool CreateContentIfNull(this HttpResponseMessage httpResponseMessage)
        {
            Fx.Assert(httpResponseMessage != null, "The 'httpResponseMessage' parameter should never be null.");

            if (httpResponseMessage.Content == null)
            {
                httpResponseMessage.Content = new ByteArrayContent(Array.Empty<byte>());
                return true;
            }

            return false;
        }

        internal static void MergeWebHeaderCollection(this HttpResponseMessage responseMessage, WebHeaderCollection headersToMerge)
        {
            responseMessage.CreateContentIfNull();
            HttpRequestMessageExtensionMethods.MergeWebHeaderCollectionWithHttpHeaders(headersToMerge, responseMessage.Headers, responseMessage.Content.Headers);
        }

        internal static WebHeaderCollection ToWebHeaderCollection(this HttpResponseMessage httpResponse)
        {
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers = httpResponse.Headers;
            if (httpResponse.Content != null)
            {
                headers = headers.Concat(httpResponse.Content.Headers);
            }
            return headers.ToWebHeaderCollection();
        }
    }
}
