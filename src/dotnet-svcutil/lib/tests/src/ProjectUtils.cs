// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Tools.ServiceModel.Svcutil;
using Xunit;

namespace SvcutilTest
{
    internal static class ProjectUtils
    {
        static readonly CancellationToken token = CancellationToken.None;

        public static MSBuildProj GetProject(string filePath, string targetFramework, bool forceNew, bool build, ILogger logger, bool globalTool = false)
        {
            MSBuildProj project = null;

            var projectDir = Path.GetDirectoryName(filePath);
            var srcProgramFile = Path.Combine(projectDir, "Program.cs");
            var dstProgramFile = Path.Combine(projectDir, $"{Path.GetFileNameWithoutExtension(filePath)}.cs");

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            if (forceNew && File.Exists(filePath))
            {
                File.Delete(filePath);
                if (File.Exists(dstProgramFile))
                    File.Delete(dstProgramFile);
                FileUtil.TryDeleteDirectory(Path.Combine(Path.GetDirectoryName(filePath), "obj"));
            }

            if (File.Exists(filePath))
            {
                project = MSBuildProj.FromPathAsync(filePath, logger, token).Result;
            }
            else
            {
                if (!string.IsNullOrEmpty(targetFramework))
                {
                    project = MSBuildProj.DotNetNewAsync(filePath, logger, token, "--langVersion latest --framework " + targetFramework).Result;
                }
                else
                {
                    project = MSBuildProj.DotNetNewAsync(filePath, logger, token).Result;
                }
                    
                File.Move(srcProgramFile, dstProgramFile);
            }

            Assert.NotNull(project);

            if (!string.IsNullOrEmpty(targetFramework))
            {
                project.TargetFramework = targetFramework;
            }

            if (!globalTool)
            {
                var svcutilPkgVersion = E2ETest.GetSvcutilPkgVersionAndFeed();
                var svcutilPkgRef = ProjectDependency.FromPackage("dotnet-svcutil-lib", svcutilPkgVersion);
                if (!project.Dependencies.Any(d => d.Equals(svcutilPkgRef)))
                {
                    bool success = project.AddDependency(svcutilPkgRef, true);
                    Assert.True(success, $"Could not add tool package dependency: dotnet-svcutil-lib.{svcutilPkgVersion}");
                }
            }

            var ret = project.RestoreAsync(logger, token).Result;
            Assert.True(ret.ExitCode == 0, $"Project package restore failed:{Environment.NewLine}{ret.OutputText}{logger}");

            if (build)
            {
                ret = project.BuildAsync(logger, token).Result;
                Assert.True(ret.ExitCode == 0, $"Project build failed:{Environment.NewLine}{ret.OutputText}{logger}");
            }

            return project;
        }

        public static ProcessRunner.ProcessResult RunSvcutil(this MSBuildProj project, string options, bool expectSuccess, ILogger logger, bool globalTool = false)
        {
            Assert.False(string.IsNullOrEmpty(options), $"{nameof(options)} not initialized!");
            Assert.True(File.Exists(project?.FullPath), $"{nameof(project)} is not initialized!");

            var envVars = new Dictionary<string, string> { { AppInsightsTelemetryClient.OptOutVariable, (!AppInsightsTelemetryClient.IsUserOptedIn).ToString() } };

            ProcessRunner.ProcessResult result;

            if (globalTool)
            {
                result = ProcessRunner.RunAsync("dotnet-svcutil", options, project.DirectoryPath, redirectOutput: true, throwOnError: false, environmentVariables: envVars, logger: logger, cancellationToken: CancellationToken.None).Result;
            }
            else
            {
                string csStr = string.Empty;
                string srcPath = project.FullPath.Replace("csproj", "cs");
                using (var sr = new StreamReader(srcPath))
                {
                    csStr = sr.ReadToEnd();
                }

                using (var sw = new StreamWriter(srcPath))
                {
                    if (csStr.Contains("optstring"))
                    {
                        int start = csStr.IndexOf("string optstring");
                        int end = csStr.IndexOf("string[] opts");
                        csStr = csStr.Replace(csStr.Substring(start, end - start), "string optstring = @\"" + options + "\";\r\n");
                        sw.Write(csStr);
                        sw.Flush();
                    }
                    else
                    {
                        string indent = new string(' ', 12);
                        string replacement = "var re = new Regex(@\"'[^\\\"\"]*'|[^\\\"\"^\\s]+|\"\"[^\\\"\"]*\"\"\");\r\n" +
                                    indent + "string optstring = @\"" + options + "\";\r\n" +
                                    indent + "string[] opts = re.Matches(optstring).Cast<Match>().Select(m => m.Value).ToArray();\r\n" +
                                    indent + "return Tool.Main(opts);";

                        if (csStr.Contains("using"))
                        {
                            csStr = csStr.Replace("using System;", "using System;\r\nusing Microsoft.Tools.ServiceModel.Svcutil;\r\nusing System.Linq;\r\nusing System.Text.RegularExpressions;");
                            csStr = csStr.Replace("static void Main", "static int Main");
                            csStr = csStr.Replace("Console.WriteLine(\"Hello World!\");", replacement);
                        }
                        else
                        {
                            replacement = replacement.Insert(0, "using System;\r\nusing Microsoft.Tools.ServiceModel.Svcutil;\r\nusing System.Linq;\r\nusing System.Text.RegularExpressions;\r\n");
                            csStr = csStr.Replace("Console.WriteLine(\"Hello, World!\");", replacement);
                        }

                        sw.Write(csStr);
                        sw.Flush();
                    }
                }

                string csprojStr = string.Empty;
                using (var sr2 = new StreamReader(project.FullPath))
                {
                    csprojStr = sr2.ReadToEnd();
                }

                if (csprojStr.Contains("System.ServiceModel"))
                {
                    using (var sw2 = new StreamWriter(project.FullPath))
                    {
                        sw2.Write(System.Text.RegularExpressions.Regex.Replace(csprojStr, @"<ItemGroup>\s+<PackageReference Include=""System.ServiceModel[\S\s]+ItemGroup>", ""));
                        sw2.Flush();
                    }
                }

                result = ProcessRunner.RunAsync("dotnet", $"run", project.DirectoryPath, redirectOutput: true, throwOnError: false, environmentVariables: envVars, logger: logger, cancellationToken: CancellationToken.None).Result;
            }

            var isSuccess = result.ExitCode == 0 || result.ExitCode == 6;
            var isTestSucess = !(isSuccess ^ expectSuccess);

            if (!isTestSucess)
            {
                var errMsg = string.Format(E2ETest.g_ToReproduceProblemMessageFormat, project.DirectoryPath, options);
                logger?.WriteMessageAsync(errMsg, true).Wait();
            }

            return result;
        }

        public static string AddFakeServiceReference(this MSBuildProj project, string srcParamsFile, string referenceFolderName, bool addNamespace)
        {
            Assert.False(string.IsNullOrEmpty(srcParamsFile), $"{nameof(srcParamsFile)} not initialized!");
            Assert.False(string.IsNullOrEmpty(referenceFolderName), $"{nameof(referenceFolderName)} not initialized!");
            Assert.True(File.Exists(project?.FullPath), $"{nameof(project)} is not initialized!");

            var outputDir = Path.Combine(project.DirectoryPath, referenceFolderName);
            Directory.CreateDirectory(outputDir);

            var dstParamsFile = Path.Combine(outputDir, Path.GetFileName(srcParamsFile));
            File.Copy(srcParamsFile, dstParamsFile);

            if (addNamespace)
            {
                var referenceNamespace = PathHelper.GetRelativePath(referenceFolderName, project.DirectoryPath).Replace("\\", ".").Replace("/", ".").Replace(" ", "_");
                var updateOptions = UpdateOptions.FromFile(dstParamsFile);
                updateOptions.NamespaceMappings.Add(new KeyValuePair<string, string>("*", referenceNamespace));
                updateOptions.Save(dstParamsFile);
            }

            return dstParamsFile;
        }
    }
}
