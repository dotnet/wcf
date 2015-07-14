// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Bridge
{
    public class ConfigController : ApiController
    {
        internal static config Config = new config();
        internal static string CurrentAppDomain;

        public HttpResponseMessage POST(config config)
        {
            Trace.WriteLine("POST config: " + config);
            try
            {
                if (!config.isValidProbingPath())
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                        "config.resourcesDirectory does not exist.");
                }

                string friendlyName = config.UpdateApp();
                CurrentAppDomain = friendlyName;
                var response = new configResponse
                {
                    types = TypeCache.Cache[friendlyName]
                };
                Trace.WriteLine("POST config: " + response);
                return Request.CreateResponse(HttpStatusCode.OK, response);
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
