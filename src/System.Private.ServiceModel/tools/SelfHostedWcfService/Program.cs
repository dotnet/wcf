// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using WcfService;

namespace SelfHostedWCFService
{
    public class SelfHostedWCFService
    {
        private static int s_httpPort = 8081;
        private static int s_httpsPort = 44285;
        private static int s_tcpPort = 8808;
        private static int s_websocketPort = 8083;
        private static int s_websocketsPort = 8084;

        static private void GetPorts()
        {
            s_httpPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("httpPort")) ? s_httpPort : int.Parse(Environment.GetEnvironmentVariable("httpPort"));
            s_httpsPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("httpsPort")) ? s_httpsPort : int.Parse(Environment.GetEnvironmentVariable("httpsPort"));
            s_tcpPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("tcpPort")) ? s_tcpPort : int.Parse(Environment.GetEnvironmentVariable("tcpPort"));
            s_websocketPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("websocketPort")) ? s_websocketPort : int.Parse(Environment.GetEnvironmentVariable("websocketPort"));
            s_websocketsPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("websocketsPort")) ? s_websocketsPort : int.Parse(Environment.GetEnvironmentVariable("websocketsPort"));
        }

        private static void Main()
        {
            GetPorts();

            string httpBaseAddress = string.Format(@"http://localhost:{0}", s_httpPort);
            string httpsBaseAddress = string.Format(@"https://localhost:{0}", s_httpsPort);
            string tcpBaseAddress = string.Format(@"net.tcp://localhost:{0}", s_tcpPort);
            string websocketBaseAddress = string.Format(@"http://localhost:{0}", s_websocketPort);
            string websocketsBaseAddress = string.Format(@"https://localhost:{0}", s_websocketsPort);

            Console.WriteLine("Starting all service hosts...");
            TestDefinitionHelper.StartHosts();

            //Start the crlUrl service last as the client use it to ensure all services have been started
            Uri testHostUrl = new Uri(string.Format("http://localhost/TestHost.svc", s_httpPort));
            WebServiceHost host = new WebServiceHost(typeof(TestHost), testHostUrl);
            WebHttpBinding binding = new WebHttpBinding();
            host.AddServiceEndpoint(typeof(ITestHost), binding, "");
            ServiceDebugBehavior serviceDebugBehavior = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            serviceDebugBehavior.HttpHelpPageEnabled = false;
            host.Open();

            Console.WriteLine("All service hosts have started.");
            do
            {
                Console.WriteLine("Type <Exit> to terminate the self service Host.");
                string input = Console.ReadLine();
                if (string.Compare(input, "exit", true) == 0)
                {
                    return;
                }
            } while (true);
        }
    }
}
