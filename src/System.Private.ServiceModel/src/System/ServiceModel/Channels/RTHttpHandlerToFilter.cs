// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


#if FEATURE_NETNATIVE
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;

using HttpRequestMessage = System.Net.Http.HttpRequestMessage;
using HttpResponseMessage = System.Net.Http.HttpResponseMessage;
using RTHttpMethod = Windows.Web.Http.HttpMethod;
using RTHttpRequestMessage = Windows.Web.Http.HttpRequestMessage;
using RTHttpRequestHeaderCollection = Windows.Web.Http.Headers.HttpRequestHeaderCollection;
using RTHttpResponseMessage = Windows.Web.Http.HttpResponseMessage;
using RTHttpBufferContent = Windows.Web.Http.HttpBufferContent;
using RTHttpStreamContent = Windows.Web.Http.HttpStreamContent;
using RTHttpVersion = Windows.Web.Http.HttpVersion;
using RTIHttpContent = Windows.Web.Http.IHttpContent;
using RTIInputStream = Windows.Storage.Streams.IInputStream;
using RTHttpBaseProtocolFilter = Windows.Web.Http.Filters.HttpBaseProtocolFilter;

namespace System.ServiceModel.Channels
{
    internal class RTHttpHandlerToFilter : DelegatingHandler
    {
        private readonly RTHttpBaseProtocolFilter _next;
        private int _filterMaxVersionSet;

        internal RTHttpHandlerToFilter(RTHttpBaseProtocolFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            _next = filter;
            _filterMaxVersionSet = 0;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            cancel.ThrowIfCancellationRequested();

            RTHttpRequestMessage rtRequest = await ConvertRequestAsync(request).ConfigureAwait(false);
            RTHttpResponseMessage rtResponse = await _next.SendRequestAsync(rtRequest).AsTask(cancel).ConfigureAwait(false);

            // Update in case of redirects
            request.RequestUri = rtRequest.RequestUri;

            HttpResponseMessage response = ConvertResponse(rtResponse);
            response.RequestMessage = request;
            return response;
        }

        private async Task<RTHttpRequestMessage> ConvertRequestAsync(HttpRequestMessage request)
        {
            RTHttpRequestMessage rtRequest = new RTHttpRequestMessage(new RTHttpMethod(request.Method.Method), request.RequestUri);

            // We can only control the Version on the first request message since the WinRT API
            // has this property designed as a filter/handler property. In addition the overall design
            // of HTTP/2.0 is such that once the first request is using it, all the other requests
            // to the same endpoint will use it as well.
            if (Interlocked.Exchange(ref _filterMaxVersionSet, 1) == 0)
            {
                    _next.MaxVersion = RTHttpVersion.Http11;
            }

            // Headers
            foreach (KeyValuePair<string, IEnumerable<string>> headerPair in request.Headers)
            {
                foreach (string value in headerPair.Value)
                {
                    bool success = rtRequest.Headers.TryAppendWithoutValidation(headerPair.Key, value);
                    Debug.Assert(success);
                }
            }

            // Properties
            foreach (KeyValuePair<string, object> propertyPair in request.Properties)
            {
                rtRequest.Properties.Add(propertyPair.Key, propertyPair.Value);
            }

            // Content
            if (request.Content != null)
            {
                rtRequest.Content = await CreateRequestContentAsync(request, rtRequest.Headers).ConfigureAwait(false);
            }

            return rtRequest;
        }

        private static async Task<RTIHttpContent> CreateRequestContentAsync(HttpRequestMessage request, RTHttpRequestHeaderCollection rtHeaderCollection)
        {
            HttpContent content = request.Content;

            RTIHttpContent rtContent;
            ArraySegment<byte> buffer;

            // If we are buffered already, it is more efficient to send the data directly using the buffer with the
            // WinRT HttpBufferContent class than using HttpStreamContent. This also avoids issues caused by
            // a design limitation in the System.Runtime.WindowsRuntime System.IO.NetFxToWinRtStreamAdapter.
            Stream contentStream = await content.ReadAsStreamAsync().ConfigureAwait(false);

            if (contentStream is RTIInputStream)
            {
                rtContent = new RTHttpStreamContent((RTIInputStream)contentStream);
            }
            else if (contentStream is MemoryStream)
            {
                var memStream = contentStream as MemoryStream;
                if (memStream.TryGetBuffer(out buffer))
                {
                    rtContent = new RTHttpBufferContent(buffer.Array.AsBuffer(), (uint)buffer.Offset, (uint)buffer.Count);
                }
                else
                {
                    byte[] byteArray = memStream.ToArray();
                    rtContent = new RTHttpBufferContent(byteArray.AsBuffer(), 0, (uint)byteArray.Length);
                }
            }
            else
            {
                rtContent = new RTHttpStreamContent(contentStream.AsInputStream());
            }

            // RTHttpBufferContent constructor automatically adds a Content-Length header. RTHttpStreamContent does not.
            // Clear any 'Content-Length' header added by the RTHttp*Content objects. We need to clear that now
            // and decide later whether we need 'Content-Length' or 'Transfer-Encoding: chunked' headers based on the
            // .NET HttpRequestMessage and Content header collections.
            rtContent.Headers.ContentLength = null;

            // Deal with conflict between 'Content-Length' vs. 'Transfer-Encoding: chunked' semantics.
            // Desktop System.Net allows both headers to be specified but ends up stripping out
            // 'Content-Length' and using chunked semantics.  The WinRT APIs throw an exception so
            // we need to manually strip out the conflicting header to maintain app compatibility.
            if (request.Headers.TransferEncodingChunked.HasValue && request.Headers.TransferEncodingChunked.Value)
            {
                content.Headers.ContentLength = null;
            }
            else
            {
                // Trigger delayed header generation via TryComputeLength. This code is needed due to an outstanding
                content.Headers.ContentLength = content.Headers.ContentLength;
            }

            foreach (KeyValuePair<string, IEnumerable<string>> headerPair in content.Headers)
            {
                foreach (string value in headerPair.Value)
                {
                    if (!rtContent.Headers.TryAppendWithoutValidation(headerPair.Key, value))
                    {
                        // rtContent headers are restricted to a white-list of allowed headers, while System.Net.HttpClient's content headers 
                        // will allow custom headers.  If something is not successfully added to the content headers, try adding them to the standard headers.
                        bool success = rtHeaderCollection.TryAppendWithoutValidation(headerPair.Key, value);
                        Debug.Assert(success);
                    }
                }
            }
            return rtContent;
        }

        private static HttpResponseMessage ConvertResponse(RTHttpResponseMessage rtResponse)
        {
            HttpResponseMessage response = new HttpResponseMessage((Net.HttpStatusCode)rtResponse.StatusCode);
            response.ReasonPhrase = rtResponse.ReasonPhrase;

            // Version
            if (rtResponse.Version == RTHttpVersion.Http11)
            {
                response.Version = new Version(1, 1);
            }
            else if (rtResponse.Version == RTHttpVersion.Http10)
            {
                response.Version = new Version(1, 0);
            }
            else if (rtResponse.Version == RTHttpVersion.Http20)
            {
                response.Version = new Version(2, 0);
            }
            else
            {
                response.Version = new Version(0, 0);
            }

            bool success;

            // Headers
            foreach (KeyValuePair<string, string> headerPair in rtResponse.Headers)
            {
                if (headerPair.Key.Equals(RTHttpClientHandler.HttpKnownHeaderNames.SetCookie, StringComparison.OrdinalIgnoreCase))
                {
                    // The Set-Cookie header always comes back with all of the cookies concatenated together. 
                    // For example if the response contains the following:
                    //     Set-Cookie A=1
                    //     Set-Cookie B=2
                    // Then we will have a single header KeyValuePair of Key=”Set-Cookie”, Value=”A=1, B=2”. 
                    // However clients expect these headers to be separated(i.e. 
                    // httpResponseMessage.Headers.GetValues("Set-Cookie") should return two cookies not one 
                    // concatenated together).
                    success = response.Headers.TryAddWithoutValidation(headerPair.Key, GetCookiesFromHeader(headerPair.Value, rtResponse.RequestMessage.RequestUri));
                }
                else
                {
                    success = response.Headers.TryAddWithoutValidation(headerPair.Key, headerPair.Value);
                }

                Debug.Assert(success);
            }

            // Content
            if (rtResponse.Content != null)
            {
                var rtResponseStream = rtResponse.Content.ReadAsInputStreamAsync().AsTask().Result;
                response.Content = new StreamContent(rtResponseStream.AsStreamForRead());

                foreach (KeyValuePair<string, string> headerPair in rtResponse.Content.Headers)
                {
                    success = response.Content.Headers.TryAddWithoutValidation(headerPair.Key, headerPair.Value);
                    Debug.Assert(success);
                }
            }

            return response;
        }

        private static IEnumerable<string> GetCookiesFromHeader(string setCookieHeader, Uri requestUri)
        {
            List<string> cookieStrings = null;

            try
            {
                var container = new CookieContainer();
                container.SetCookies(requestUri, setCookieHeader);
                var collection = container.GetCookies(requestUri);
                cookieStrings = new List<string>(collection.Count);
                foreach (var cookieObj in collection)
                {
                    Cookie cookie = (Cookie)cookieObj;
                    cookieStrings.Add(cookie.ToString());
                }
            }
            catch (Exception)
            {
                // TODO: We should log this.  But there isn't much we can do about it other
                // than to drop the rest of the cookies.
            }

            return cookieStrings;
        }
    }
}
#endif // FEATURE_NETNATIVE