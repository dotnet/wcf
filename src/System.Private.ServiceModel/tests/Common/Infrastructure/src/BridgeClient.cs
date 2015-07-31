// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Common
{
    public static class BridgeClient 
    {
        private static object s_thisLock = new object();
        private static Dictionary<string, string> _Resources = new Dictionary<string, string>();
        private static BridgeState _BridgeStatus = BridgeState.NotStarted;
        private static readonly Regex regexResource = new Regex(@"details\s*:\s*""([^""]+)""", RegexOptions.IgnoreCase);

        private static string BridgeBaseAddress
        {
            get { return TestProperties.GetProperty(TestProperties.BridgeUrl_PropertyName); }
        }

        private static void EnsureBridgeIsRunning()
        {
            // Tests run concurrently, so we need the initial configuration
            // request to finish and set state before the rest continue.
            lock (s_thisLock)
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
        }
        public static string GetResourceAddress(string resourceName)
        {
            EnsureBridgeIsRunning();

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
                var content = new StringContent(
                    CreateConfigRequestContentAsJson(),
                    Encoding.UTF8,
                    "application/json");
                try
                {
                    var response = httpClient.PostAsync("/config/", content).GetAwaiter().GetResult();
                    if (!response.IsSuccessStatusCode)
                    {
                        string reason = String.Format("{0}Bridge returned unexpected status code='{1}', reason='{2}'",
                                                    Environment.NewLine, response.StatusCode, response.ReasonPhrase);
                        if (response.Content != null)
                        {
                            string contentAsString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            reason = String.Format("{0}, content:{1}{2}",
                                                    reason, Environment.NewLine, contentAsString);
                        }
                        throw new Exception(reason);
                    }
                    _BridgeStatus = BridgeState.Started;
                }
                catch (Exception exc)
                {
                    _BridgeStatus = BridgeState.Faulted;
                    throw new Exception("Bridge is not running", exc);
                }
            }
        }

        private static string CreateConfigRequestContentAsJson()
        {
            ValidateConfigRequestProperties();

            // Create a Json dictionary of name/value pairs from TestProperties
            StringBuilder sb = new StringBuilder("{ ");
            string[] propertyNames = TestProperties.PropertyNames.ToArray();
            for (int i = 0; i < propertyNames.Length; ++i)
            {
                string propertyName = propertyNames[i];
                sb.Append(String.Format("\"{0}\" : \"{1}\"", propertyName, TestProperties.GetProperty(propertyName)));
                if (i < propertyNames.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(" }");
            return sb.ToString();
        }

        // Validates some of the name/value pairs that will be sent to the Bridge
        // via the /config POST request.
        private static void ValidateConfigRequestProperties()
        {
            string bridgeResourceFolder = TestProperties.GetProperty(TestProperties.BridgeResourceFolder_PropertyName);
            if (String.IsNullOrEmpty(bridgeResourceFolder) || !Directory.Exists(bridgeResourceFolder))
            {
                throw new Exception(String.Format("BridgeResourceFolder '{0}' does not exist", bridgeResourceFolder));
            }
        }

        private static string MakeResourcePutRequest(string resourceName)
        {
            EnsureBridgeIsRunning();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BridgeBaseAddress);
                var content = new StringContent(
                        string.Format(@"{{ name : ""{0}"" }}", resourceName),
                        Encoding.UTF8,
                        "application/json");
                try
                {
                    var response = httpClient.PutAsync("/resource/", content).GetAwaiter().GetResult();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception("Unexpected status code: " + response.StatusCode);

                    var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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
            EnsureBridgeIsRunning();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BridgeBaseAddress);
                var response = httpClient.GetAsync("/resource/" + resourceName).GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Unexpected status code: " + response.StatusCode);

                return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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
