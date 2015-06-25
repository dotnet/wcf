using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace System.ServiceModel.Tests.Common
{
    public class BridgeTestFixture : IDisposable
    {
        private static Dictionary<string, string> _Resources = new Dictionary<string, string>();
        private static BridgeState _BridgeStatus = BridgeState.NotStarted;

        private static string BridgeBaseAddress
        {
            // Would like to pull this address either from msbuild properties, env vars, or 
            // configuration passed into xunit.
            get { return "http://localhost:44283"; }
        }

        public BridgeTestFixture()
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

        public void Dispose()
        {
            // Placeholder for releasing the resource
        }

        public static string GetResourceAddress(string resourceName)
        {
            string resourceAddress = null;
            if (_BridgeStatus == BridgeState.Started && !_Resources.TryGetValue(resourceName, out resourceAddress))
            {
                resourceAddress = MakeResourceRequest(resourceName);
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

        private static string MakeResourceRequest(string resourceName)
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

        enum BridgeState
        {
            NotStarted,
            Started,
            Faulted,
        }
    }
}
