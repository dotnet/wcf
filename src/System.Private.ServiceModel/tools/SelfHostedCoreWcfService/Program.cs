// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using WcfService;

namespace SelfHostedWCFService
{
    public class Parameters
    {
        public const string ServiceBootstrap = "bootstrap";
    }

    public class SelfHostedWCFService
    {
        private static bool s_serviceBootstrap = false;

        public static async Task Main(string[] args)
        {
            ParseArgs(args);

            if (s_serviceBootstrap)
            {
                await ServiceHostHelper.ServiceBootstrap();
                Environment.Exit(1);
            }

            Console.WriteLine("Installing certificates...");
            string testserverbase = string.Empty;
            TimeSpan validatePeriod = TimeSpan.FromDays(1);
            string crlFileLocation = "c:\\WCFTest\\test.crl";
            int DefaultHttpPort = 8081;

            CertificateGeneratorLibrary.SetupCerts(testserverbase, validatePeriod, crlFileLocation, DefaultHttpPort);

            Console.WriteLine("Starting services...");
            var webHost = await TestDefinitionHelper.StartHosts(false);
            var webHostWebSocket = await TestDefinitionHelper.StartHosts(true);

            Console.WriteLine("All services started.");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
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
            else
            {
                //Linux and MacOS
                Console.WriteLine("Use Shutdown endpoint to terminate the self service Host.");
                Console.WriteLine("http://localhost:8081/TestHost.svc/shutdown");

                Thread.Sleep(Timeout.Infinite);
            }

            GC.KeepAlive(webHost);
            GC.KeepAlive(webHostWebSocket);
        }

        private static bool ParseArgs(string[] args)
        {
            foreach (string s in args)
            {
                string[] p = s.Split(new char[] { ':' }, count: 2);
                if (p.Length != 2)
                {
                    continue;
                }

                switch (p[0].ToLower())
                {
                    case Parameters.ServiceBootstrap:
                        bool.TryParse(p[1], out s_serviceBootstrap);
                        break;
                    default:
                        Console.WriteLine("unknown argument: " + s);
                        continue;
                }
            }

            return true;
        }

    }
}
