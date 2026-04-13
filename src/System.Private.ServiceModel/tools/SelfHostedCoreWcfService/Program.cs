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
        public const string HttpPort = "httpPort";
    }

    public class SelfHostedWCFService
    {
        private static bool s_serviceBootstrap = false;
        private static int s_httpPort = 8081;

        public static async Task Main(string[] args)
        {
            ParseArgs(args);
            Environment.SetEnvironmentVariable(Parameters.HttpPort, s_httpPort.ToString());

            if (s_serviceBootstrap)
            {
                await ServiceHostHelper.ServiceBootstrap();
                Environment.Exit(1);
            }

            Console.WriteLine("Installing certificates...");
            string testserverbase = string.Empty;
            TimeSpan validatePeriod = TimeSpan.FromDays(1);
            string crlFileLocation;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                crlFileLocation = "c:\\WCFTest\\test.crl";
            }
            else
            {
                crlFileLocation = Path.Combine(Environment.CurrentDirectory, "test.crl");
            }

            CertificateGeneratorLibrary.SetupCerts(testserverbase, validatePeriod, crlFileLocation, s_httpPort);

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
                Console.WriteLine($"http://localhost:{s_httpPort}/TestHost.svc/shutdown");

                Thread.Sleep(Timeout.Infinite);
            }

            GC.KeepAlive(webHost);
            GC.KeepAlive(webHostWebSocket);
        }

        private static bool ParseArgs(string[] args)
        {
            string httpPort = Environment.GetEnvironmentVariable(Parameters.HttpPort);
            if (!string.IsNullOrWhiteSpace(httpPort))
            {
                if (!int.TryParse(httpPort, out s_httpPort) || s_httpPort < 1 || s_httpPort > 65535)
                {
                    throw new ArgumentOutOfRangeException(nameof(args), $"Invalid HTTP port '{httpPort}'.");
                }
            }

            foreach (string s in args)
            {
                string[] p = s.Split(new char[] { ':' }, count: 2);
                if (p.Length != 2)
                {
                    continue;
                }

                switch (p[0].ToLowerInvariant())
                {
                    case Parameters.ServiceBootstrap:
                        bool.TryParse(p[1], out s_serviceBootstrap);
                        break;
                    case "httpport":
                        if (!int.TryParse(p[1], out s_httpPort) || s_httpPort < 1 || s_httpPort > 65535)
                        {
                            throw new ArgumentOutOfRangeException(nameof(args), $"Invalid HTTP port '{p[1]}'.");
                        }
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
