// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web.Models;

namespace Web.Controllers
{
    public class ConfigController : ApiController
    {
        internal static string CurrentAppDomain;

        public HttpResponseMessage POST(config config)
        {
            Trace.WriteLine("POST config: " + JsonConvert.SerializeObject(config, Formatting.Indented));

            //

            try
            {
                if (!config.isValidProbingPath())
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                        "config.resourcesDirectory does not exist.");
                }

                string friendlyName = config.UpdateApp();
                CurrentAppDomain = friendlyName;
                var response = new
                {
                    config = WebApiApplication.Config,
                    types = TypeCache.Cache[friendlyName]
                };
                Trace.WriteLine("POST config: " + JsonConvert.SerializeObject(response, Formatting.Indented));
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                var exceptionResponse = new { operation = "config", method = "POST", exception = ex };
                Trace.WriteLine("POST config exception: " +
                            JsonConvert.SerializeObject(exceptionResponse, Formatting.Indented));

                return Request.CreateResponse(HttpStatusCode.BadRequest, exceptionResponse);
            }
        }
    }
}
