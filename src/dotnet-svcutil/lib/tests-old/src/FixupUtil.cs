// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SvcutilTest
{
    internal class FixupUtil
    {
        static readonly string programFilesx64 = Environment.GetEnvironmentVariable("ProgramW6432")?.Replace('\\', '/');

        private List<ReplaceInfo> replacements;

        public void Init(string resultsPath, string testCasesPath, string projectPath, string serviceUrl, string serviceId, string repositoryRoot)
        {
            // Set versions to a valid fixed value to allow for compiling the generated sources.
            this.replacements = new List<ReplaceInfo>
            {
                new ReplaceInfo(serviceUrl, "$serviceUrl$"),
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
                new ReplaceInfo(@"\d+\.\d+\.\d+(\.\d+)*(-\w+)*", "99.99.99") { UseRegex = true }, // valid version value to be able to compile projects.
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
            };

            // The result path passed in includes the directory name. Instead replace the parent.
            var resultPathReplacement = Directory.GetParent(resultsPath).FullName;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                this.replacements.Add(new ReplaceInfo($"{programFilesx64}/dotnet/sdk.*.targets", "$sdkTarget$") { UseRegex = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                this.replacements.Add(new ReplaceInfo(@"/usr/share/dotnet/sdk.*.targets", "$sdkTarget$") { UseRegex = true });
            }

            // Replace paths both with backslashes and forward slashes.
            this.replacements.Add(new ReplaceInfo(resultPathReplacement, "$resultPath$"));
            this.replacements.Add(new ReplaceInfo(resultPathReplacement.Replace('\\', '/'), "$resultPath$"));
            this.replacements.Add(new ReplaceInfo(resultPathReplacement.Replace("\\", "\\\\"), "$resultPath$"));
            this.replacements.Add(new ReplaceInfo(testCasesPath, "$testCasesPath$"));
            this.replacements.Add(new ReplaceInfo(testCasesPath.Replace('\\', '/'), "$testCasesPath$"));
            this.replacements.Add(new ReplaceInfo(testCasesPath.Replace("\\", "\\\\"), "$testCasesPath$"));
            this.replacements.Add(new ReplaceInfo(projectPath, "$projectPath$"));
            this.replacements.Add(new ReplaceInfo(projectPath.Replace('\\', '/'), "$projectPath$"));
            this.replacements.Add(new ReplaceInfo(projectPath.Replace("\\", "\\\\"), "$projectPath$"));
            this.replacements.Add(new ReplaceInfo(repositoryRoot, "$repositoryRoot$"));
            this.replacements.Add(new ReplaceInfo(repositoryRoot.Replace("\\", "/"), "$repositoryRoot$"));
            this.replacements.Add(new ReplaceInfo(repositoryRoot.Replace("\\", "\\\\"), "$repositoryRoot$"));
            this.replacements.Add(new ReplaceInfo("$repositoryRoot$\\bin\\Debug", "$binDir$"));
            this.replacements.Add(new ReplaceInfo("$repositoryRoot$/bin/Debug", "$binDir$"));
            this.replacements.Add(new ReplaceInfo("$repositoryRoot$\\bin\\Release", "$binDir$"));
            this.replacements.Add(new ReplaceInfo("$repositoryRoot$/bin/Release", "$binDir$"));

        }

        public string FixupFile(string fileName)
        {
            var replacements = new List<ReplaceInfo>(this.replacements);

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
                replacements = this.replacements;
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
