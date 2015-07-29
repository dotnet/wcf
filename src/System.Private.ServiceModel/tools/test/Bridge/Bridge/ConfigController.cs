// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WcfTestBridgeCommon;

namespace Bridge
{
    public class ConfigController : ApiController
    {
        private const int Default_BridgeIdleTimeoutMinutes = 20;

        private static BridgeConfiguration s_bridgeConfiguration = new BridgeConfiguration
        {
            BridgeIdleTimeoutMinutes = Default_BridgeIdleTimeoutMinutes   // initialize so is accessible during app startup
        };

        internal static string CurrentAppDomain;

        public static BridgeConfiguration BridgeConfiguration
        {
            get
            {
                return s_bridgeConfiguration;
            }
            set
            {
                s_bridgeConfiguration = value;
            }
        }
        public HttpResponseMessage POST(HttpRequestMessage request)
        {
            try
            {
                // Handle deserialization explicitly to bypass MediaTypeFormatter use
                string nameValuePairs = request.Content.ReadAsStringAsync().Result;
                Dictionary<string, string> configInfo = JsonSerializer.DeserializeDictionary(nameValuePairs);

                // Create a new configuration combining the existing one with any provided properties.
                BridgeConfiguration newConfiguration = new BridgeConfiguration(BridgeConfiguration, configInfo);

                Trace.WriteLine("POST config request: " + Environment.NewLine + newConfiguration);

                string friendlyName = newConfiguration.UpdateApp();
                CurrentAppDomain = friendlyName;

                string configResponse = PrepareConfigResponse(TypeCache.Cache[friendlyName]);
                Trace.WriteLine("POST config response: " + Environment.NewLine + configResponse);

                // Directly return a json string to avoid use of MediaTypeFormatters
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(configResponse);
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

        private static string PrepareConfigResponse(IEnumerable<string> types)
        {
            return string.Format(@"{{
    types : [
        ""{0}""
    ]
}}",
                string.Join("\",\n        \"", types));
        }
    }
}
