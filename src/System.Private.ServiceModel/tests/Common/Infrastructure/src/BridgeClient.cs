// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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
        private static BridgeState s_BridgeStatus = BridgeState.NotStarted;
        private static string s_BridgeFaultReason = String.Empty;
        private static readonly Regex regexResource = new Regex(@"details\s*:\s*""([^""]+)""", RegexOptions.IgnoreCase);

        private static string BridgeBaseAddress
        {
            get
            {
                return String.Format("http://{0}:{1}",
                                TestProperties.GetProperty(TestProperties.BridgeHost_PropertyName),
                                TestProperties.GetProperty(TestProperties.BridgePort_PropertyName));
            }
        }

        private static string BridgeEndpointAddress
        {
            get
            {
                return String.Format("{0}/{1}/", BridgeBaseAddress, "bridge");
            }
        }

        private static string BridgeConfigEndpointAddress
        {
            get
            {
                return String.Format("{0}/{1}/", BridgeBaseAddress, "config");
            }
        }

        private static string BridgeResourceEndpointAddress
        {
            get
            {
                return String.Format("{0}/{1}/", BridgeBaseAddress, "resource");
            }
        }

        private static void EnsureBridgeIsRunning()
        {
            // Tests run concurrently, so we need the initial configuration
            // request to finish and set state before the rest continue.
            lock (s_thisLock)
            {
                if (s_BridgeStatus == BridgeState.NotStarted)
                {
                    // The initial ping establishes Bridge state
                    PingBridge();

                    // Bridge is known running -- configure with our resources
                    MakeConfigRequest();
                }
                else if (s_BridgeStatus == BridgeState.Faulted)
                {
                    throw new Exception(String.Format("The Bridge is not running, due to this earlier fault:{0}{1}",
                                        Environment.NewLine, s_BridgeFaultReason));
                }
            }
        }
        public static string GetResourceAddress(string resourceName)
        {
            EnsureBridgeIsRunning();

            string resourceAddress = null;
            lock (s_thisLock)
            {
                if (s_BridgeStatus == BridgeState.Started && !_Resources.TryGetValue(resourceName, out resourceAddress))
                {
                    resourceAddress = MakeResourcePutRequest(resourceName);
                    _Resources.Add(resourceName, resourceAddress);
                }
            }

            return resourceAddress;
        }

        // Ensure the given response was successful. If it was not, generate
        // an explanatory message and throw an exception.
        private static void EnsureSuccessfulResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string reason = String.Format("{0}Bridge returned unexpected status code='{1}', reason='{2}'",
                                          Environment.NewLine, response.StatusCode, response.ReasonPhrase);
                if (response.Content != null)
                {
                    string contentAsString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    if (contentAsString.Length > 1000)
                    {
                        contentAsString = contentAsString.Substring(0, 999) + "...";
                    }
                    reason = String.Format("{0}, content:{1}{2}",
                                            reason, Environment.NewLine, contentAsString);
                }

                throw new Exception(reason);
            }
        }

        // Issues a GET request to the Bridge base address to determine
        // whether it is running.  This method also sets state to indicate
        // whether the bridge is running or unavailable. 
        private static void PingBridge()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    var response = httpClient.GetAsync(BridgeEndpointAddress).GetAwaiter().GetResult();
                    EnsureSuccessfulResponse(response);
                    s_BridgeStatus = BridgeState.Started;
                }
                catch (Exception exc)
                {
                    s_BridgeStatus = BridgeState.Faulted;
                    s_BridgeFaultReason = String.Format("A GET request was issued to '{0}' but encountered exception {1}",
                                                            BridgeEndpointAddress, exc.Message);
                    
                    throw new Exception(s_BridgeFaultReason, exc);
                }
            }
        }

        private static void MakeConfigRequest()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var content = new StringContent(
                    CreateConfigRequestContentAsJson(),
                    Encoding.UTF8,
                    "application/json");
                try
                {
                    var response = httpClient.PostAsync(BridgeConfigEndpointAddress, content).GetAwaiter().GetResult();
                    EnsureSuccessfulResponse(response);
                    s_BridgeStatus = BridgeState.Started;
                }
                catch (Exception exc)
                {
                    s_BridgeStatus = BridgeState.Faulted;
                    s_BridgeFaultReason = String.Format("A POST request was issued to '{0}' but encountered exception {1}",
                                        BridgeConfigEndpointAddress, exc.Message);
                    throw new Exception(s_BridgeFaultReason, exc);
                }
            }
        }

        private static string CreateConfigRequestContentAsJson()
        {
            // Create a Json dictionary of name/value pairs from TestProperties
            StringBuilder sb = new StringBuilder("{ ");
            string[] propertyNames = TestProperties.PropertyNames.ToArray();
            for (int i = 0; i < propertyNames.Length; ++i)
            {
                string propertyName = propertyNames[i];
                string propertyValue = TestProperties.GetProperty(propertyName);

                // We do not configure the BridgeResourceFolder because the Bridge
                // is provided one at startup.  Moreover, cross-platform scenarios
                // would require this folder to be shared across OS boundaries.
                if (String.Equals(propertyName, TestProperties.BridgeResourceFolder_PropertyName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                sb.Append(String.Format("\"{0}\" : \"{1}\"", propertyName, propertyValue));
                if (i < propertyNames.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(" }");
            return sb.ToString();
        }

        private static string MakeResourcePutRequest(string resourceName)
        {
            EnsureBridgeIsRunning();

            using (HttpClient httpClient = new HttpClient())
            {
                var content = new StringContent(
                        string.Format(@"{{ name : ""{0}"" }}", resourceName),
                        Encoding.UTF8,
                        "application/json");
                try
                {
                    var response = httpClient.PutAsync(BridgeResourceEndpointAddress, content).GetAwaiter().GetResult();
                    EnsureSuccessfulResponse(response);

                    var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var match = regexResource.Match(responseContent);
                    if (!match.Success || match.Groups.Count != 2)
                        throw new Exception("Invalid response from bridge: " + responseContent);

                    return match.Groups[1].Value;
                }
                catch (Exception exc)
                {
                    string failureMessage = String.Format("A PUT request was issued to '{0}' for resource '{1}' but encountered exception {2}",
                    BridgeResourceEndpointAddress, resourceName, exc.Message);
                    throw new Exception(failureMessage, exc);
                }
            }
        }

        private static string MakeResourceGetRequest(string resourceName)
        {
            EnsureBridgeIsRunning();

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    var response = httpClient.GetAsync(BridgeResourceEndpointAddress + resourceName).GetAwaiter().GetResult();
                    EnsureSuccessfulResponse(response);
                    return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
                catch (Exception exc)
                {
                    string failureMessage = String.Format("A Get request was issued to '{0}{1}' but encountered exception {2}",
                                                           BridgeResourceEndpointAddress, resourceName, exc.Message);
                    throw new Exception(failureMessage, exc);
                }

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
