// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Infrastructure.Common
{
    public static class BridgeClient 
    {
        private const string EndpointResourceResponseUriKeyName = "uri";
        private const string EndpointResourceRequestNameKeyName = "name";

        private static object s_thisLock = new object();
        private static Dictionary<string, string> _Resources = new Dictionary<string, string>();
        private static BridgeState s_BridgeStatus = BridgeState.NotStarted;
        private static string s_BridgeFaultReason = String.Empty;

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
                    resourceAddress = MakeEndpointResourcePutRequest(resourceName);
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

        private static string MakeEndpointResourcePutRequest(string resourceName)
        {
            var responseContent = MakeResourcePutRequest(resourceName, null);

            string uri = null;
            if (!responseContent.TryGetValue(EndpointResourceResponseUriKeyName, out uri) || String.IsNullOrWhiteSpace(uri))
            {
                throw new Exception("Invalid response from bridge: " + FormatKeyValuePairsAsString(responseContent));
            }

            return uri;
        }

        internal static Dictionary<string,string> MakeResourcePutRequest(string resourceName, Dictionary<string, string> requestParameters)
        {
            EnsureBridgeIsRunning();

            string requestParametersResourceName; 
            if (requestParameters != null 
                && requestParameters.TryGetValue(EndpointResourceRequestNameKeyName, out requestParametersResourceName) 
                && !string.Equals(resourceName, requestParametersResourceName))
            {
                throw new ArgumentException(
                    string.Format("A PUT request to the bridge included parameter 'name' = '{0}' in its requestParameters, but the 'resourceName' = '{1}' specified was different", requestParametersResourceName, resourceName),
                    "requestParameters");
            }

            using (HttpClient httpClient = new HttpClient())
            {
                StringContent content; 
                if (requestParameters != null)
                {
                    content = new StringContent(
                            JsonSerializer.SerializeDictionary(requestParameters),
                            Encoding.UTF8,
                            "application/json");
                }
                else
                {
                    content  = new StringContent("{}", Encoding.UTF8, "application/json");
                }

                try
                {
                    var response = httpClient.PutAsync(BridgeResourceEndpointAddress + resourceName, content).GetAwaiter().GetResult();
                    EnsureSuccessfulResponse(response);

                    var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Dictionary<string, string> properties = JsonSerializer.DeserializeDictionary(responseContent);

                    return properties;
                }
                catch (Exception exc)
                {
                    string failureMessage = String.Format("A PUT request was issued to '{0}' for resource '{1}' but encountered exception {2}",
                    BridgeResourceEndpointAddress, resourceName, exc.Message);
                    throw new Exception(failureMessage, exc);
                }
            }
        }

        internal static Dictionary<string, string> MakeResourceGetRequest(string resourceName, Dictionary<string, string> requestParameters)
        {
            EnsureBridgeIsRunning();

            string requestParametersResourceName;
            if (requestParameters != null
                && requestParameters.TryGetValue(EndpointResourceRequestNameKeyName, out requestParametersResourceName)
                && !string.Equals(resourceName, requestParametersResourceName))
            {
                throw new ArgumentException(
                    string.Format("A GET request to the bridge included parameter 'name' = '{0}' in its requestParameters, but the 'resourceName' = '{1}' specified was different", requestParametersResourceName, resourceName),
                    "requestParameters");
            }

            using (HttpClient httpClient = new HttpClient())
            {
                string uri = CreateResourceUri(resourceName, requestParameters);

                try
                {
                    var response = httpClient.GetAsync(uri).GetAwaiter().GetResult();
                    EnsureSuccessfulResponse(response);

                    var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Dictionary<string, string> properties = JsonSerializer.DeserializeDictionary(responseContent);

                    return properties;
                }
                catch (Exception exc)
                {
                    string failureMessage = String.Format("A GET request was issued to '{0}' for resource '{1}' but encountered exception {2}",
                    uri, resourceName, exc.Message);
                    throw new Exception(failureMessage, exc);
                }
            }
        }

        private static string CreateResourceUri(string resourceName, Dictionary<string, string> requestParameters)
        {
            StringBuilder uriBuilder = new StringBuilder(BridgeResourceEndpointAddress + resourceName);
            if (requestParameters != null)
            {
                uriBuilder.Append("?");
                foreach (var kvp in requestParameters)
                {
                    // we currently rely on the caller to do any URLEncoding
                    uriBuilder.AppendFormat("{0}={1}&", kvp.Key, kvp.Value);
                }
            }

            return uriBuilder.ToString().TrimEnd('&');
        }

        private static string FormatKeyValuePairsAsString(Dictionary<string, string> dictionary)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var pair in dictionary)
            {
                sb.AppendFormat("{0}  {1} : {2}", Environment.NewLine, pair.Key, pair.Value);
            }

            return sb.ToString();
        }

        enum BridgeState
        {
            NotStarted,
            Started,
            Faulted,
        }
    }
}
