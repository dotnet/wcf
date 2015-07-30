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
        public static EventHandler<Tuple<BridgeConfiguration, BridgeConfiguration>> BridgeConfigurationChanged;

        private static BridgeConfiguration s_bridgeConfiguration = new BridgeConfiguration
        {
            BridgeIdleTimeoutMinutes = IdleTimeoutHandler.Default_Timeout_Minutes
        };

        internal static string CurrentAppDomainName;

        static ConfigController()
        {
            // Register to manage AppDomains in response to changes to configuration
            BridgeConfigurationChanged += (object s, Tuple<BridgeConfiguration, BridgeConfiguration> tuple) =>
            {
                BridgeConfiguration oldConfig = tuple.Item1;
                BridgeConfiguration newConfig = tuple.Item2;
                CurrentAppDomainName = AppDomainManager.UpdateApp(oldConfig, newConfig);
            };
        }

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

                Trace.WriteLine("POST config content: " + Environment.NewLine + nameValuePairs);

                // Create a new configuration combining the existing one with any provided properties.
                BridgeConfiguration newConfiguration = new BridgeConfiguration(BridgeConfiguration, configInfo);
                Trace.WriteLine("POST config properties: " + Environment.NewLine + newConfiguration);

                // Take the new configuration and notify listeners of the change.
                BridgeConfiguration oldConfiguration = BridgeConfiguration;
                BridgeConfiguration = newConfiguration;

                if (BridgeConfigurationChanged != null)
                {
                    Tuple<BridgeConfiguration, BridgeConfiguration> eventArgs = new Tuple<BridgeConfiguration, BridgeConfiguration>(oldConfiguration, newConfiguration);
                    BridgeConfigurationChanged(this, eventArgs);
                }

                string configResponse = PrepareConfigResponse(TypeCache.Cache[CurrentAppDomainName]);
                Trace.WriteLine("POST config response content: " + Environment.NewLine + configResponse);

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
