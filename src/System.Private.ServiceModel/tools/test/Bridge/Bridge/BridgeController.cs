// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WcfTestBridgeCommon;

namespace Bridge
{
    public class BridgeController : ApiController
    {
        private static object BridgeLock { get; set; }
        public static BridgeState BridgeState { get; private set; }

        static BridgeController()
        {
            BridgeLock = new object();
            BridgeState = BridgeState.Running;
        }

        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            Dictionary<string, string> dictionary = ConfigController.BridgeConfiguration.ToDictionary();

            string configResponse = JsonSerializer.SerializeDictionary(dictionary);

            Trace.WriteLine(String.Format("{0:T} - GET bridge returning raw content:{1}{2}",
                                          DateTime.Now, Environment.NewLine, configResponse),
                            typeof(BridgeController).Name);

            // Bridge GET response is the current Bridge configuration
            HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(configResponse);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(JsonSerializer.JsonMediaType);
            return response;
        }

        // The DELETE Http verb means stop the Bridge cleanly
        public HttpResponseMessage Delete(HttpRequestMessage request)
        {
            Trace.WriteLine(String.Format("{0:T} - received DELETE request", DateTime.Now),
                            typeof(BridgeController).Name);

            lock(BridgeLock)
            {
                if (BridgeState == BridgeState.Running)
                {
                    // 'Stopping' is the Bridge's terminal state because
                    // the process itself will terminate during this response.
                    BridgeState = BridgeState.Stopping;
                    try
                    {
                        HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new ExitOnDisposeStringContent("\"The Bridge has closed.\"");
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue(JsonSerializer.JsonMediaType);
                        return response;
                    }
                    catch (Exception ex)
                    {
                        var exceptionResponse = ex.Message;
                        Trace.WriteLine(String.Format("{0:T} - DELETE config exception:{1}{2}",
                                                        DateTime.Now, Environment.NewLine, ex),
                                        typeof(BridgeController).Name);

                        return request.CreateResponse(HttpStatusCode.BadRequest, exceptionResponse);
                    }
                }
                else
                {
                    // Multiple concurrent DELETE requests are blocked by the monitor.
                    // But in case the process has not yet terminated from the first request,
                    // send back BADREQUEST for any others.
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Bridge is already stopping.");
                }
            }
        }

        public static void ReleaseAllResources()
        {
            // Cleanly shutdown all AppDomains we own so they have
            // the chance to release resources they've acquired or installed
            AppDomainManager.ShutdownAllAppDomains();

            // Uninstall any certificates added or used by the Bridge itself
            CertificateManager.UninstallAllCertificates(force: true);

            // Force the removal of the SSL cert that may have been added
            // by another AppDomain or left from a prior run
            int httpsPort = ConfigController.BridgeConfiguration.BridgeHttpsPort;
            if (httpsPort != 0)
            {
                CertificateManager.UninstallSslPortCertificate(httpsPort);
            }

            // Finally remove all firewall rules we added for the ports
            PortManager.RemoveAllBridgeFirewallRules();
        }

        public static void StopBridgeProcess(int exitCode)
        {
            ReleaseAllResources();
            Environment.Exit(exitCode);
        }

        // This class exists to release all Bridge resources
        // in this class's Dispose().  WebAPI guarantees the
        // HttpResponseMessage and its content will be disposed
        // only after the response has been sent, allowing the
        // Bridge to provide a valid 200 response for the DELETE
        // and then immediately terminate the process.
        class ExitOnDisposeStringContent : StringContent
        {
            public ExitOnDisposeStringContent(string content) : base(content)
            {
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                StopBridgeProcess(0);
            }
        }
    }
}
