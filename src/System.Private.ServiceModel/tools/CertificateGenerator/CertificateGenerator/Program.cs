// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Configuration;

namespace CertUtil
{
    internal class Program
    {
        private static string s_testserverbase = string.Empty;
        private static string s_crlFileLocation = string.Empty;
        private static TimeSpan s_validatePeriod;
        private static int s_httpPort = 80;

        private static void Usage()
        {
            Console.WriteLine("Supported arguments:");
            Console.WriteLine("  -Uninstall");
            Console.WriteLine("  -help");
            Console.WriteLine("  -httpPort:<port>  or  -httpPort <port>");
        }
        private static int Main(string[] args)
        {
            ApplyAppSettings();

            for (int i = 0; i < args.Length; i++)
            {
                if (string.Compare(args[i], "-Uninstall", true) == 0)
                {
                    CertificateGeneratorLibrary.UninstallAllCerts();
                    return 0;
                }

                if (string.Compare(args[i], "-help", true) == 0)
                {
                    Usage();
                    return 0;
                }

                if (args[i].StartsWith("-httpPort", StringComparison.OrdinalIgnoreCase))
                {
                    string portValue = null;
                    int separatorIndex = args[i].IndexOf(':');
                    if (separatorIndex >= 0)
                    {
                        portValue = args[i].Substring(separatorIndex + 1);
                    }
                    else if (i + 1 < args.Length)
                    {
                        portValue = args[++i];
                    }

                    if (!int.TryParse(portValue, out s_httpPort) || s_httpPort < 1 || s_httpPort > 65535)
                    {
                        Console.WriteLine($"Invalid http port: {portValue ?? "<missing>"}");
                        Usage();
                        return 1;
                    }

                    continue;
                }

                Usage();
                return 1;
            }

            CertificateGeneratorLibrary.SetupCerts(s_testserverbase, s_validatePeriod, s_crlFileLocation, s_httpPort);

            return 0;
        }

        private static void ApplyAppSettings()
        {
            var appSettings = ConfigurationManager.AppSettings;
            s_testserverbase = appSettings["testserverbase"] ?? string.Empty;
            s_validatePeriod = TimeSpan.FromDays(int.Parse(appSettings["CertExpirationInDay"]));
            s_crlFileLocation = appSettings["CrlFileLocation"];
        }
    }
}
