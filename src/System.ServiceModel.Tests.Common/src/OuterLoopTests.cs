using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Text;

namespace System.ServiceModel.Tests.Common
{
    public abstract class OuterLoopTests : IDisposable
    {
        protected string EndpointAddress
        {
            get;
            private set;
        }

        private string BaseAddress
        {
            // Would like to pull this address either from msbuild properties, env vars, or 
            // configuration passed into xunit.
            get { return "http://localhost:44283"; }
        }

        public OuterLoopTests(string resourceName)
        {
            MakeConfigRequest();
            EndpointAddress = MakeResourceRequest(resourceName);
        }

        public void Dispose()
        {
            // Placeholder for releasing the resource
        }

        private void MakeConfigRequest()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseAddress);
                var content = new StringContent(
                    @"{ resourcesDirectory : ""C:\\dotnet\\wcf\\src\\System.Private.ServiceModel\\tests\\Scenarios\\SelfHostWcfService\\bin\\Debug"" }",
                    Encoding.UTF8,
                    "application/json");
                try
                {
                    var response = httpClient.PostAsync("/config/", content).Result;
                    if (!response.IsSuccessStatusCode)
                        throw new Exception("Unexpected status code: " + response.StatusCode);
                }
                catch (Exception exc)
                {
                    throw new Exception("Bridge is not running", exc);
                }
            }
        }

        private string MakeResourceRequest(string resourceName)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseAddress);
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
    }
}
