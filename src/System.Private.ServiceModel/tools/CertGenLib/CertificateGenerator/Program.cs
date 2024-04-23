// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Configuration;

namespace CertificateGeneratorApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string testserverbase = ApplicationConfiguration.GetSetting("testserverbase") ?? string.Empty;
            TimeSpan validatePeriod = TimeSpan.FromDays(int.Parse(ApplicationConfiguration.GetSetting("CertExpirationInDay")));
            string crlFileLocation = ApplicationConfiguration.GetSetting("CrlFileLocation") ?? string.Empty;

            CertificateGeneratorLibrary.SetupCerts(testserverbase, validatePeriod, crlFileLocation);
        }

        public static class ApplicationConfiguration
        {
            private static readonly IConfigurationRoot s_configuration;
            static ApplicationConfiguration()
            {
                var builder = new ConfigurationBuilder().AddJsonFile("CertificateGeneratorSettings.json");
                s_configuration = builder.Build();
            }
            public static string GetSetting(string key)
            {
                return s_configuration[key] ?? string.Empty;
            }
        }
    }
}
