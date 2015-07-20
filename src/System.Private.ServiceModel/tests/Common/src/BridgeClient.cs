// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace System.ServiceModel.Tests.Common
{
    public static class BridgeClient 
    {
        private static Dictionary<string, string> _Resources = new Dictionary<string, string>();
        private static BridgeState _BridgeStatus = BridgeState.NotStarted;
        private static readonly Regex regexResource = new Regex(@"details\s*:\s*""([^""]+)""", RegexOptions.IgnoreCase);

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

        private static void MakeConfigRequest()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BridgeBaseAddress);
                string resourceFolder = TestProperties.GetProperty(TestProperties.BridgeResourceFolder_PropertyName);
                string contentPayload = "{ resourcesDirectory : \"" + resourceFolder + "\" }"; 
                var content = new StringContent(
                    contentPayload,
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

                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    var match = regexResource.Match(responseContent);
                    if (!match.Success || match.Groups.Count != 2)
                        throw new Exception("Invalid response from bridge: " + responseContent);

                    return match.Groups[1].Value;
                }
                catch (Exception exc)
                {
                    throw new Exception("Unable to start resource: " + resourceName, exc);
                }
            }
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
