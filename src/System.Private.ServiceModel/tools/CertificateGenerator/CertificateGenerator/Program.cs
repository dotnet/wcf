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

        private static void Usage()
        {
            Console.WriteLine("Supported argument is -Uninstall");
            Console.WriteLine("                      -help");
        }
        private static int Main(string[] args)
        {
            ApplyAppSettings();

            if (args.Length > 0)
            {
                if (string.Compare(args[0], "-Uninstall", true) == 0)
                {
                    CertificateGeneratorLibrary.UninstallAllCerts();
                    return 0;
                }
                else if (string.Compare(args[0], "-help", true) == 0)
                {
                    Usage();
                    return 0;
                }
                else
                {
                    Usage();
                    return 1;
                }
            }

            CertificateGeneratorLibrary.SetupCerts(s_testserverbase, s_validatePeriod, s_crlFileLocation);

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
