// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class RuntimeEnvironmentHelper
    {
        public static bool IsWindows
        {
            get
            {
#if NETCORE
                return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
#else
                var platform = (int)Environment.OSVersion.Platform;
                return (platform != 4) && (platform != 6) && (platform != 128);
#endif
            }
        }

#if !NETCORE10
        private static readonly StringComparison s_ignoreCaseComparison = StringComparison.InvariantCultureIgnoreCase;
        private static readonly StringComparison s_caseSensitiveComparison = StringComparison.InvariantCulture;
#endif
        public static StringComparison FileStringComparison
        {
            get
            {
#if NETCORE10
                return IsWindows ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
#else
                return IsWindows ? s_ignoreCaseComparison : s_caseSensitiveComparison;
#endif
            }
        }

        public static async Task TryCopyingConfigFiles(string workingDirectory, string destinationDirectory, ILogger logger, CancellationToken cancellationToken)
        {
            if (!File.Exists(Path.Combine(destinationDirectory, "global.json")))
            {
                // copy any global.json in use so the right dotnet SDK is used for the bootstrapping projects.
                await PathHelper.TryCopyFileIfFoundAsync("global.json", workingDirectory, destinationDirectory, logger, cancellationToken).ConfigureAwait(false);
            }

            if (!File.Exists(Path.Combine(destinationDirectory, "nuget.config")))
            {
                // try copy nuget.config to help reduce restore issues.
                await PathHelper.TryCopyFileIfFoundAsync("nuget.config", workingDirectory, destinationDirectory, logger, cancellationToken).ConfigureAwait(false);
            }
        }


        internal static readonly string NugetConfigFormat =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine +
            "<configuration>" + Environment.NewLine +
            "  <packageSources>" + Environment.NewLine +
            "    <add key = \"svcutilFeed\" value=\"{0}\" />" + Environment.NewLine +
            "  </packageSources>" + Environment.NewLine +
            "</configuration>" + Environment.NewLine;

        public static async Task<bool> TryAddSvcutilNuGetFeedAsync(string nugetConfigPath, string packageFeed, ILogger logger, CancellationToken cancellationToken)
        {
            bool? added = false;

            if (File.Exists(nugetConfigPath))
            {
                using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, "Adding svcutil NuGet feed to existing nuget.config file ...").ConfigureAwait(false))
                {
                    try
                    {
                        using (var stream = File.Open(nugetConfigPath, FileMode.Open, FileAccess.ReadWrite))
                        {
                            var doc = XDocument.Parse(new StreamReader(stream).ReadToEnd());
                            var pkgSourcesNode = doc.Element("configuration")?.Element("packageSources");

                            if (pkgSourcesNode != null)
                            {
                                added = pkgSourcesNode.Elements("add")?
                                    .Select(e => e.Attribute("value").Value.TrimEnd(Path.DirectorySeparatorChar))
                                    .Any(v => v != null && packageFeed.Equals(v, RuntimeEnvironmentHelper.FileStringComparison));

                                if (added != true)
                                {
                                    var feedName = "SvcutilNuGetFeed";
                                    var count = 1;

                                    while (pkgSourcesNode.Elements("add").Any(e => StringComparer.OrdinalIgnoreCase.Compare(e.Attribute("key").Value, feedName) == 0))
                                    {
                                        feedName += count++;
                                    }

                                    var addElement = new XElement("add");
                                    addElement.Add(new XAttribute("key", feedName), new XAttribute("value", packageFeed));

                                    pkgSourcesNode.Add(addElement);

                                    stream.Seek(0, SeekOrigin.Begin);
                                    doc.Save(stream);
                                    added = true;
                                }
                                else
                                {
                                    await safeLogger.WriteMessageAsync("Feed entry already exists.", logToUI: false);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await safeLogger.WriteErrorAsync(ex.Message, logToUI: false);
                    }

                    await safeLogger.WriteMessageAsync($"Feed entry added/exists: {added == true}", logToUI: false);
                }
            }

            return added == true;
        }
    }
}
