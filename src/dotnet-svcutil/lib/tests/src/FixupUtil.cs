// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SvcutilTest
{
    internal class FixupUtil
    {
        static readonly string s_programFilesx64 = Environment.GetEnvironmentVariable("ProgramW6432")?.Replace('\\', '/');

        private List<ReplaceInfo> _replacements;

        public void Init(string resultsPath, string testCasesPath, string projectPath, string serviceUrl, string serviceId, string repositoryRoot)
        {
            // Set versions to a valid fixed value to allow for compiling the generated sources.
            _replacements = new List<ReplaceInfo>
            {
                //new ReplaceInfo(serviceUrl, "$serviceUrl$"),
                new ReplaceInfo(@"\[\d+\.\d+\]", "$LOGENTRY$") { UseRegex = true },
                new ReplaceInfo(@"\d+_\w+_\d+_\d+(_\d+)*", "9999_JAN_9_9_9") { UseRegex = true },
                new ReplaceInfo(@"in \d+(\,\d+)* ms", "$TIME$") { UseRegex = true },
                new ReplaceInfo(@"in \d+(\.\d+)* ms", "$TIME$") { UseRegex = true },
                new ReplaceInfo(@"in \d+(\.\d+)* sec", "$TIME$") { UseRegex = true },
                new ReplaceInfo(@"in \d+(\,\d+)* sec", "$TIME$") { UseRegex = true },
                new ReplaceInfo(@"position \d+", "position NNN") { UseRegex = true },
                new ReplaceInfo(@"line \d+", "line NNN") { UseRegex = true },
                new ReplaceInfo(@"Elapsed (\d+\:)+\d+\.\d+", "Elapsed HH:MM:SS.ms") { UseRegex = true },
                new ReplaceInfo(@"elapsed: \d+:\d+:(\d+.)+", "elapsed HH:MM:SS.ms") { UseRegex = true },
                new ReplaceInfo(@"\d+\.\d+\.\d+(\.\d+)*(-\w+)*(\.\d+)*", "99.99.99") { UseRegex = true }, // new, valid version value to be able to compile projects, sample: 5.0.100-preview.7.20366.6                
                new ReplaceInfo(@"targets\(\d+,\d+\)", "targets(NN, NN)") { UseRegex = true },
                new ReplaceInfo(@"\[(.*.csproj?)\]", "[$ProjectFile$.csproj]") { UseRegex = true },
                new ReplaceInfo(@"Found conflicts between different versions of (.*) that could not be resolved.", "Found conflicts between different versions of the same dependent assembly that could not be resolved.") { UseRegex = true },
                new ReplaceInfo("Validation Error: Wildcard '(.*)' allows element '(.*)', and causes the content model to become ambiguous.", "Validation Error: Wildcard '$wildcard%' allows element '%element%', and causes the content model to become ambiguous.") { UseRegex = true },
                new ReplaceInfo(@"\s*at System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1.ConfiguredTaskAwaiter.GetResult\(\)", "") { UseRegex = true }, // Only shows in stack traces on debug builds.
                new ReplaceInfo(Path.GetTempPath(), "$TEMP$"),
                new ReplaceInfo(Path.GetTempPath().Replace("\\", "\\\\"), "$TEMP$"),
                new ReplaceInfo(Path.GetTempPath().Replace("\\", "/"), "$TEMP$"),
                new ReplaceInfo(Environment.GetEnvironmentVariable("USERPROFILE"), "$USERPROFILE$"),
                new ReplaceInfo("/root", "$USERPROFILE$"),
                new ReplaceInfo(@"targetFramework:\[netcoreapp\d+\.\d+\]", "targetFramework:[N.N]") { UseRegex = true },
                new ReplaceInfo(@"""targetFramework"": ""netcoreapp\d+\.\d+""", "\"targetFramework\": \"N.N\"") { UseRegex = true }, //new    
                new ReplaceInfo(@"<TargetFramework>netcoreapp\d+\.\d+</TargetFramework>", "<TargetFramework>N.N</TargetFramework>") { UseRegex = true }, //new    
                new ReplaceInfo(@"""targetFramework"": ""net\d+\.\d+""", "\"targetFramework\": \"N.N\"") { UseRegex = true }, //new    
                new ReplaceInfo(@"<TargetFramework>net\d+\.\d+</TargetFramework>", "<TargetFramework>N.N</TargetFramework>") { UseRegex = true }, //new    
                new ReplaceInfo(@"<PackageReference Include=""System\.ServiceModel\.\w+"" Version="".+"" />", @"<PackageReference Include=""System.ServiceModel.*"", Version=""N.N.N"">") { UseRegex = true } //new
            };

            // The result path passed in includes the directory name. Instead replace the parent.
            var resultPathReplacement = Directory.GetParent(resultsPath).FullName;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _replacements.Add(new ReplaceInfo($"{s_programFilesx64}/dotnet/sdk.*.targets", "$sdkTarget$") { UseRegex = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _replacements.Add(new ReplaceInfo(@"/usr/share/dotnet/sdk.*.targets", "$sdkTarget$") { UseRegex = true });
            }

            // Replace paths both with backslashes and forward slashes.
            _replacements.Add(new ReplaceInfo(resultPathReplacement, "$resultPath$"));
            _replacements.Add(new ReplaceInfo(resultPathReplacement.Replace('\\', '/'), "$resultPath$"));
            _replacements.Add(new ReplaceInfo(resultPathReplacement.Replace("\\", "\\\\"), "$resultPath$"));
            _replacements.Add(new ReplaceInfo(@"$resultPath$\\TestResults\\TypeReuse\\TypeReuse20\\bin\\Debug\\netcoreapp2.0\\BinLib.dll", @"$resultPath$\TestResults\TypeReuse\TypeReuse20\bin\Debug\netcoreapp2.0\BinLib.dll"));
            _replacements.Add(new ReplaceInfo(@"$resultPath$\\TestResults\\TypeReuse\\TypeReuse20\\bin\\Debug\\netcoreapp2.0\\TypesLib.dll", @"$resultPath$\TestResults\TypeReuse\TypeReuse20\bin\Debug\netcoreapp2.0\TypesLib.dll"));
            _replacements.Add(new ReplaceInfo(testCasesPath, "$testCasesPath$"));
            _replacements.Add(new ReplaceInfo(testCasesPath.Replace('\\', '/'), "$testCasesPath$"));
            _replacements.Add(new ReplaceInfo(testCasesPath.Replace("\\", "\\\\"), "$testCasesPath$"));
            _replacements.Add(new ReplaceInfo(projectPath, "$projectPath$"));
            _replacements.Add(new ReplaceInfo(projectPath.Replace('\\', '/'), "$projectPath$"));
            _replacements.Add(new ReplaceInfo(projectPath.Replace("\\", "\\\\"), "$projectPath$"));
            _replacements.Add(new ReplaceInfo(repositoryRoot, "$repositoryRoot$"));
            _replacements.Add(new ReplaceInfo(repositoryRoot.Replace("\\", "/"), "$repositoryRoot$"));
            _replacements.Add(new ReplaceInfo(repositoryRoot.Replace("\\", "\\\\"), "$repositoryRoot$"));
            _replacements.Add(new ReplaceInfo("$repositoryRoot$\\bin\\Debug", "$binDir$"));
            _replacements.Add(new ReplaceInfo("$repositoryRoot$/bin/Debug", "$binDir$"));
            _replacements.Add(new ReplaceInfo("$repositoryRoot$\\bin\\Release", "$binDir$"));
            _replacements.Add(new ReplaceInfo("$repositoryRoot$/bin/Release", "$binDir$"));
            _replacements.Add(new ReplaceInfo(Environment.GetEnvironmentVariable("HOME"), "$USERPROFILE$"));
            _replacements.Add(new ReplaceInfo(@"net(coreapp)?\d+\.\d+/dotnet-svcutil-lib.dll", "DOTNET_VERSION/dotnet-svcutil-lib.dll") { UseRegex = true }); //for linux
            _replacements.Add(new ReplaceInfo(@"net(coreapp)?\d+\.\d+\\dotnet-svcutil-lib.dll", "DOTNET_VERSION\\dotnet-svcutil-lib.dll") { UseRegex = true }); //for windows
            _replacements.Add(new ReplaceInfo(@"net(coreapp)?\d+\.\d+/any/dotnet-svcutil-lib.dll", "DOTNET_VERSION/any/dotnet-svcutil-lib.dll") { UseRegex = true }); //for linux
            _replacements.Add(new ReplaceInfo(@"net(coreapp)?\d+\.\d+\\any\\dotnet-svcutil-lib.dll", "DOTNET_VERSION\\any\\dotnet-svcutil-lib.dll") { UseRegex = true }); //for windows
            _replacements.Add(new ReplaceInfo(@"Release\Shipping", @"RelType/ShipType"));
            _replacements.Add(new ReplaceInfo(@"Release\NonShipping", @"RelType/ShipType"));
            _replacements.Add(new ReplaceInfo(@"Debug\Shipping", @"RelType/ShipType"));
            _replacements.Add(new ReplaceInfo(@"Debug\NonShipping", @"RelType/ShipType"));
        }

        public string FixupFile(string fileName)
        {
            var replacements = new List<ReplaceInfo>(_replacements);

            // Skip replacing backslashes for source code files.
            if (Path.GetExtension(fileName) != ".cs")
            {
                replacements.Add(new ReplaceInfo("\\", "/"));
            }

            var originalText = System.IO.File.ReadAllText(fileName);
            var updatedText = ReplaceText(originalText, replacements);

            if (originalText != updatedText)
            {
                System.IO.File.WriteAllText(fileName, updatedText);
            }

            return fileName;
        }

        public string ReplaceText(string originalText, List<ReplaceInfo> replacements = null)
        {
            if (replacements == null)
            {
                replacements = _replacements;
            }

            originalText = originalText.Replace("\0", "");
            originalText = originalText.Trim(); // ignore spaces at the beginning and end.

            // Normalize line endings. First convert all Windows line endings to Unix, then convert all Unix to Windows.
            // The end result will be all Windows line endings even with mixed files.
            originalText = originalText.Replace("\r\n", "\n");
            originalText = originalText.Replace("\n", "\r\n");

            var updatedText = originalText;

            foreach (ReplaceInfo replaceItem in replacements)
            {
                var target = replaceItem.OriginalValue;
                var value = replaceItem.NewValue;
                bool ignoreCase = replaceItem.IgnoreCase;
                bool useRegex = replaceItem.UseRegex;

                if (string.IsNullOrEmpty(target))
                {
                    continue;
                }

                if (useRegex)
                {
                    var regexOptions = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
                    updatedText = Regex.Replace(updatedText, target, value, regexOptions);
                }
                else if (ignoreCase)
                {
                    int idx = 0;
                    while (true)
                    {
                        idx = updatedText.IndexOf(target, idx, StringComparison.OrdinalIgnoreCase);
                        if (idx == -1) break;
                        updatedText = updatedText.Remove(idx, target.Length);
                        updatedText = updatedText.Insert(idx, value);
                        idx += value.Length;
                    }
                }
                else
                {
                    updatedText = updatedText.Replace(target, value);
                }
            }

            return updatedText;
        }
    }
}
