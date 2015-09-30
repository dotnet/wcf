// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using WcfTestBridgeCommon;

namespace Bridge
{
    public class ResourceController : ApiController
    {
        /// <summary>
        /// Invoke the <c>Put</c> method of the <see cref="IResource"/>
        /// matching the name specified in the content name/value pairs.
        /// </summary>
        /// <param name="request">The incoming request containing the name/value pair content</param>
        /// <returns>The response to return to the caller.</returns>
        public HttpResponseMessage Put(string name)
        {
            var properties = this.BuildProperites(name);

            StringBuilder sb = new StringBuilder();
            foreach (var pair in properties)
            {
                sb.AppendFormat("{0}  {1} : {2}", Environment.NewLine, pair.Key, pair.Value);
            }

            string resourceName = null;
            if (!properties.TryGetValue("name", out resourceName) || String.IsNullOrWhiteSpace(resourceName))
            {
                string badRequestMessage = "PUT request content did not contain a resource name";
                Trace.WriteLine(String.Format("{0:T} - {1}:{2}",
                                DateTime.Now,
                                badRequestMessage,
                                sb.ToString()),
                this.GetType().Name);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, badRequestMessage);
            }

            Trace.WriteLine(String.Format("{0:T} - PUT request received for resource name = '{1}', properties:{2}",
                                          DateTime.Now,
                                          String.IsNullOrWhiteSpace(resourceName) ? "null" : resourceName,
                                          sb.ToString()),
                            this.GetType().Name);
            try
            {
                ResourceResponse result = ResourceInvoker.DynamicInvokePut(resourceName, properties);
                string contentString = JsonSerializer.SerializeDictionary(result.Properties);

                Trace.WriteLine(String.Format("{0:T} - PUT response for {1} is OK:{2}{3}",
                                              DateTime.Now,
                                              resourceName,
                                              Environment.NewLine,
                                              contentString),
                                this.GetType().Name);

                return BuildJsonContent(contentString);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(String.Format("{0:T} - Exception executing PUT for resource {1}{2}:{3}",
                                                DateTime.Now, resourceName, Environment.NewLine, exception.ToString()),
                                this.GetType().Name);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception.ToString());
            }
        }


        public HttpResponseMessage Get()
        {
            IList<string> resources = ResourceInvoker.DynamicInvokeGetAllResources();
            var items = new List<KeyValuePair<string, string>>();
            foreach (var item in resources)
            {
                var name = item.Substring(item.LastIndexOf('.') + 1);
                var uri = this.Request.RequestUri.GetLeftPart(UriPartial.Path) + "/" + item;
                items.Add(new KeyValuePair<string, string>(name, uri));
            }
            items.Sort((a, b) => string.Compare(a.Key, b.Key));

            string contentString = JsonSerializer.Serialize(items);
            var response = BuildJsonContent(contentString);
            return response;
        }

        /// <summary>
        /// Invoke the <c>Get</c> method of the <see cref="IResource"/>
        /// matching the name specified in the content name/value pairs.
        /// </summary>
        /// <param name="request">The incoming request containing the name/value pair content</param>
        /// <returns>The response to return to the caller.</returns>
        public HttpResponseMessage Get(string name)
        {
            var properties = this.BuildProperites(name);

            StringBuilder sb = new StringBuilder();
            foreach (var pair in properties)
            {
                sb.AppendFormat("{0}  {1} : {2}", Environment.NewLine, pair.Key, pair.Value);
            }

            string resourceName = null;
            if (!properties.TryGetValue("name", out resourceName) || String.IsNullOrWhiteSpace(resourceName))
            {
                string badRequestMessage = "GET request content did not contain a resource name";
                Trace.WriteLine(String.Format("{0:T} - {1}:{2}",
                                DateTime.Now,
                                badRequestMessage,
                                sb.ToString()),
                this.GetType().Name);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, badRequestMessage);
            }

            Trace.WriteLine(String.Format("{0:T} - GET request received for resource name = '{1}', properties:{2}",
                                          DateTime.Now,
                                          String.IsNullOrWhiteSpace(resourceName) ? "null" : resourceName,
                                          sb.ToString()),
                            this.GetType().Name);

            try
            {
                ResourceResponse result = ResourceInvoker.DynamicInvokeGet(resourceName, properties);
                string contentString = JsonSerializer.SerializeDictionary(result.Properties);

                Trace.WriteLine(String.Format("{0:T} - GET response for {1} is OK:{2}{3}",
                                              DateTime.Now, resourceName, Environment.NewLine, contentString),
                                this.GetType().Name);

                return BuildJsonContent(contentString);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(String.Format("{0:T} - Exception executing GET for resource {1}{2}:{3}",
                                                DateTime.Now, resourceName, Environment.NewLine, exception.ToString()),
                                this.GetType().Name);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception.ToString());
            }
        }

        private HttpResponseMessage BuildJsonContent(string contentString)
        {
            // Directly return a json string to avoid use of MediaTypeFormatters
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(contentString);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(JsonSerializer.JsonMediaType);
            return response;
        }

        // The DELETE Http verb means release all resources allocated
        // and return to an initial state
        public HttpResponseMessage Delete(HttpRequestMessage request)
        {
            Trace.WriteLine(String.Format("{0:T} - received DELETE request", DateTime.Now),
                            typeof(ResourceController).Name);

            // A resource change can have wide impact, so we don't allow concurrent use
            lock (ConfigController.ConfigLock)
            {
                try
                {
                    BridgeController.ReleaseAllResources(force: false);
                    HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent("\"Bridge resources have been released.\"");
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(JsonSerializer.JsonMediaType);
                    return response;
                }
                catch (Exception ex)
                {
                    var exceptionResponse = ex.Message;
                    Trace.WriteLine(String.Format("{0:T} - DELETE config exception:{1}{2}",
                                                    DateTime.Now, Environment.NewLine, ex),
                                    typeof(ResourceController).Name);

                    return request.CreateResponse(HttpStatusCode.BadRequest, exceptionResponse);
                }
            }
        }


        private Dictionary<string, string> BuildProperites(string resourceName)
        {
            HttpRequestMessage request = this.Request;

            // GET allows name/value pairs in Uri query parameters only
            Dictionary<string, string> properties = GetNameValuePairsFromQueryParameters(request);

            if (!properties.ContainsKey("name"))
            {
                properties["name"] = resourceName;
            }

            // PUT allows name/value pairs in Uri query parameters or in content.
            // Give precedence to query parameters.
            // If there were no query parameters, allow the content to provide it.
            if (properties.Count == 0 && request.Content != null)
            {
                properties = GetNameValuePairsFromContent(request);
            }

            return properties;
        }

        private static Dictionary<string, string> GetNameValuePairsFromContent(HttpRequestMessage request)
        {
            string nameValuePairs = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Dictionary<string, string> dictionary = JsonSerializer.DeserializeDictionary(nameValuePairs);
            return dictionary;
        }

        private static Dictionary<string, string> GetNameValuePairsFromQueryParameters(HttpRequestMessage request)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var pair in request.GetQueryNameValuePairs())
            {
                dictionary.Add(pair.Key, pair.Value);
            }

            return dictionary;
        }
    }
}
