// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Bridge
{
    public class ResourceController : ApiController
    {
        /// <summary>
        /// Initialize a test
        /// </summary>
        /// <param name="resource"></param>
        /// <response code="200">Test initialized.</response>
        public HttpResponseMessage Put(HttpRequestMessage request)
        {
            string nameValuePairs = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Dictionary<string, string> resourceInfo = JsonSerializer.DeserializeDictionary(nameValuePairs);
            string resourceName = null;
            resourceInfo.TryGetValue("name", out resourceName);
            resource resource = resourceName == null ? null : new resource { name = resourceName };

            Trace.WriteLine(String.Format("{0:T} - Received PUT request for resource {1}", 
                                          DateTime.Now, String.IsNullOrWhiteSpace(resourceName) ? "null" : resourceName),
                            this.GetType().Name);

            if (String.IsNullOrWhiteSpace(resourceName))
            {
                Trace.WriteLine(String.Format("{0:T} - PUT response is BadRequest due to missing name", DateTime.Now),
                                this.GetType().Name);

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "resource data { name:'...' } not specified.");
            }

            var correlationId = Guid.NewGuid();

            try
            {


                var result = ResourceInvoker.DynamicInvokePut(resource);

                resourceResponse resourceResponse = new resourceResponse
                {
                    id = correlationId,
                    details = result.ToString()
                };

                Trace.WriteLine(String.Format("{0:T} - PUT response for {1} is:{2}{3}", 
                                              DateTime.Now, resourceName, Environment.NewLine, resourceResponse.ToString()), 
                                this.GetType().Name);

                // Directly return a json string to avoid use of MediaTypeFormatters
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(resourceResponse.ToString());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(JsonSerializer.JsonMediaType);
                return response;
            }
            catch (Exception exception)
            {
                Trace.WriteLine(String.Format("{0:T} - Exception executing PUT for resource {1}{2}:{3}",
                                                DateTime.Now, resourceName, Environment.NewLine, exception.ToString()), 
                                this.GetType().Name);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new resourceResponse
                {
                    id = correlationId,
                    details = exception.Message
                });
            }
        }

        /// <summary>
        /// Gets resource information
        /// </summary>
        /// <param name="name">name of the resource</param>
        public HttpResponseMessage Get(string name)
        {
            Trace.WriteLine(String.Format("{0:T} - Received GET request for resource {1}", 
                                          DateTime.Now, String.IsNullOrWhiteSpace(name) ? "null" : name),
                            this.GetType().Name);

            if (string.IsNullOrWhiteSpace(name))
            {
                Trace.WriteLine(String.Format("{0:T} - GET response is BadRequest due to missing name", DateTime.Now),
                                this.GetType().Name);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "name not specified.");
            }

            try
            {


                var result = ResourceInvoker.DynamicInvokeGet(name);

                Trace.WriteLine(String.Format("{0:T} - GET response for resource {1} is:{2}{3}", 
                                              DateTime.Now, name, Environment.NewLine, result),
                                this.GetType().Name);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(String.Format("{0:T} - Exception executing GET for resource {1}{2}:{3}",
                                                DateTime.Now, name, Environment.NewLine, exception.ToString()), 
                                this.GetType().Name);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new resourceResponse
                {
                    id = Guid.NewGuid(),
                    details = exception.Message
                });
            }
        }
    }
}
