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
            
            if (resource == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "resource data { name:'...' } not specified.");
            }

            var correlationId = Guid.NewGuid();

            try
            {
                Trace.WriteLine(String.Format("Received request to create resource: {0}", resource), this.GetType().Name);

                var result = ResourceInvoker.DynamicInvokePut(resource);

                resourceResponse resourceResponse = new resourceResponse
                {
                    id = correlationId,
                    details = result.ToString()
                };

                Trace.WriteLine(String.Format("Resource creation response is: {0}", resourceResponse.ToString()), this.GetType().Name);

                // Directly return a json string to avoid use of MediaTypeFormatters
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(resourceResponse.ToString());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(JsonSerializer.JsonMediaType);
                return response;
            }
            catch (Exception exception)
            {
                Trace.WriteLine(String.Format("Exception when trying to create resource {0}{1}:{2}",
                                                resource.name, Environment.NewLine, exception.ToString()), this.GetType().Name);
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
            if (string.IsNullOrWhiteSpace(name))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "name not specified.");
            }

            try
            {
                Trace.WriteLine(String.Format("Received request to get resource: {0}", name), this.GetType().Name);

                var result = ResourceInvoker.DynamicInvokeGet(name);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(String.Format("Exception when trying to get resource {0}{1}:{2}",
                                                name, Environment.NewLine, exception.ToString()), this.GetType().Name);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new resourceResponse
                {
                    id = Guid.NewGuid(),
                    details = exception.Message
                });
            }
        }
    }
}
