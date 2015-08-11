// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net.Http;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Bridge.Build.Tasks
{
    // This task will invoke the Bridge to ask it to release all its
    // resources.
    public class ReleaseBridgeResourcesTask : Task
    {
        [Required]
        public string BridgeHost { get; set; }

        [Required]
        public string BridgePort { get; set; }

        public override bool Execute()
        {
            int port = 0;
            if (!int.TryParse(BridgePort, out port))
            {
                Log.LogError("The BridgePort value '{0}' is not a valid integer.", BridgePort);
                return false;
            }

            string bridgeAddress = string.Format("http://{0}:{1}", BridgeHost, BridgePort);

            HttpClient httpClient = new HttpClient();
            bool bridgeIsRunning = false;

            try
            {
                // First, try pinging it.  If it is not currently running, we can safely
                // ignore errors attempting to release its resource folder.
                var response = httpClient.GetAsync(bridgeAddress).GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                {
                    string reason = String.Format("The ping GET request to the Bridge returned unexpected status code='{0}', reason='{1}'",
                            response.StatusCode, response.ReasonPhrase);
                    if (response.Content != null)
                    {
                        string contentAsString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        reason = String.Format("{0}, content:{1}{2}",
                                                reason, Environment.NewLine, contentAsString);
                    }
                    Log.LogMessage(reason);
                }
                else
                {
                    Log.LogMessage("The Bridge responded correctly to a ping GET request at {0}", bridgeAddress);
                }

                bridgeIsRunning = true;

                // A DELETE request to the config endpoint releases all allocated resources.
                response = httpClient.DeleteAsync(bridgeAddress + "/config/").GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                {
                    string reason = String.Format("Failed to release the resources.  The DELETE request to Bridge returned unexpected status code='{0}', reason='{1}'",
                                                  response.StatusCode, response.ReasonPhrase);
                    if (response.Content != null)
                    {
                        string contentAsString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        reason = String.Format("{0}, content:{1}{2}",
                                                reason, Environment.NewLine, contentAsString);
                    }
                    Log.LogError(reason);
                    return false;
                }

                Log.LogMessage("The Bridge resources have been released.");
                return true;
            }
            catch (Exception ex)
            {
                if (bridgeIsRunning)
                {
                    Log.LogError("The Bridge is running but the DELETE request failed:{0}{1}", Environment.NewLine, ex.ToString());
                    return false;
                }

                Log.LogMessage("The Bridge does not appear to be running at {0}, so skipping the request to release its resources.", 
                                bridgeAddress);
                return true;
            }
        }
    }
}
