// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace System.ServiceModel.Tests.Common
{
    public static class BridgeClient 
    {
        private static Dictionary<string, string> _Resources = new Dictionary<string, string>();
        private static BridgeState _BridgeStatus = BridgeState.NotStarted;
        private static Dictionary<string, string> _BaseAddresses = null;

        private static string BridgeBaseAddress
        {
            // TODO: Pull this address from msbuild props, env vars, or config passed into xunit.
            get { return "http://localhost:44283"; }
        }

        static BridgeClient()
        {
            if (_BridgeStatus == BridgeState.NotStarted)
            {
                MakeConfigRequest();
            }
            else if (_BridgeStatus == BridgeState.Faulted)
            {
                throw new Exception("Bridge is not running");
            }
        }

        public static string GetResourceAddress(string resourceName)
        {
            string resourceAddress = null;
            if (_BridgeStatus == BridgeState.Started && !_Resources.TryGetValue(resourceName, out resourceAddress))
            {
                resourceAddress = MakeResourcePutRequest(resourceName);
                _Resources.Add(resourceName, resourceAddress);
            }

            return resourceAddress;
        }

        public static string GetBaseAddress(string resourceName, string name)
        {
            string resourceAddress = null;
            if (_BridgeStatus == BridgeState.Started)
            {
                if (_BaseAddresses == null)
                {
                    _BaseAddresses = new Dictionary<string, string>();
                    string response = MakeResourceGetRequest(resourceName);

                    var reader = new JsonTextReader(new StringReader(response));
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.String)
                        {
                            _BaseAddresses.Add(reader.Path, reader.Value as string);
                        }
                    }
                }

                _BaseAddresses.TryGetValue(name, out resourceAddress);
            }

            return resourceAddress;
        }

        private static void MakeConfigRequest()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BridgeBaseAddress);
                var content = new StringContent(
                    @"{ resourcesDirectory : ""WcfService"" }",
                    Encoding.UTF8,
                    "application/json");
                try
                {
                    var response = httpClient.PostAsync("/config/", content).Result;
                    if (!response.IsSuccessStatusCode)
                        throw new Exception("Unexpected status code: " + response.StatusCode);
                    _BridgeStatus = BridgeState.Started;
                }
                catch (Exception exc)
                {
                    _BridgeStatus = BridgeState.Faulted;
                    throw new Exception("Bridge is not running", exc);
                }
            }
        }

        private static string MakeResourcePutRequest(string resourceName)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BridgeBaseAddress);
                var content = new StringContent(
                        string.Format(@"{{ name : ""{0}"" }}", resourceName),
                        Encoding.UTF8,
                        "application/json");
                try
                {
                    var response = httpClient.PutAsync("/resource/", content).Result;
                    if (!response.IsSuccessStatusCode)
                        throw new Exception("Unexpected status code: " + response.StatusCode);

                    var reader = new JsonTextReader(new StringReader(response.Content.ReadAsStringAsync().Result));
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.String && reader.Path == "details")
                        {
                            return reader.Value as string;
                        }
                    }
                }
                catch (Exception exc)
                {
                    throw new Exception("Unable to start resource: " + resourceName, exc);
                }
            }
            
            return null;
        }

        private static string MakeResourceGetRequest(string resourceName)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BridgeBaseAddress);
                var response = httpClient.GetAsync("/resource/" + resourceName).Result;
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Unexpected status code: " + response.StatusCode);

                return response.Content.ReadAsStringAsync().Result;
            }
        }

        enum BridgeState
        {
            NotStarted,
            Started,
            Faulted,
        }
    }
}
