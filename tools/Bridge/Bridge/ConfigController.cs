// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Bridge
{
    public class ConfigController : ApiController
    {
        internal static config Config = new config();
        internal static string CurrentAppDomain;

        public HttpResponseMessage POST(HttpRequestMessage request)
        {
            try
            {
                // Handle deserialization explicitly to bypass MediaTypeFormatter use
                string nameValuePairs = request.Content.ReadAsStringAsync().Result;
                Dictionary<string, string> configInfo = JsonSerializer.DeserializeDictionary(nameValuePairs);

                config config = new config(configInfo);

                Trace.WriteLine("POST config: " + Environment.NewLine + config);
                if (!config.isValidProbingPath())
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                        String.Format("config.resourcesDirectory {0} does not exist.", config.BridgeResourceFolder));
                }

                string friendlyName = config.UpdateApp();
                CurrentAppDomain = friendlyName;

                var configResponse = new configResponse
                {
                    types = TypeCache.Cache[friendlyName]
                };
                Trace.WriteLine("POST config: " + configResponse);

                // Directly return a json string to avoid use of MediaTypeFormatters
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(configResponse.ToString());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(JsonSerializer.JsonMediaType);
                return response;
            }
            catch (Exception ex)
            {
                var exceptionResponse = ex.Message;
                Trace.WriteLine("POST config exception: " + ex);

                return Request.CreateResponse(HttpStatusCode.BadRequest, exceptionResponse);
            }
        }
    }
}
