// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using Microsoft.Tools.ServiceModel.Svcutil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

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
                if (File.Exists(dstProgramFile)) File.Delete(dstProgramFile);
                FileUtil.TryDeleteDirectory(Path.Combine(Path.GetDirectoryName(filePath), "obj"));
            }

            if (File.Exists(filePath))
            {
                project = MSBuildProj.FromPathAsync(filePath, logger, token).Result;
            }
            else
            {
                project = MSBuildProj.DotNetNewAsync(filePath, logger, token).Result;
                File.Move(srcProgramFile, dstProgramFile);
            }

            Assert.IsNotNull(project, $"Could not create project \"{filePath}\":\r\n{logger}");

            if (!string.IsNullOrEmpty(targetFramework))
            {
                project.TargetFramework = targetFramework;
            }

            if (!globalTool)
            {
                var svcutilPkgVersion = E2ETest.GetSvcutilPkgVersionAndFeed();
                var svcutilPkgRef = ProjectDependency.FromCliTool("dotnet-svcutil-lib", svcutilPkgVersion);
                if (!project.Dependencies.Any(d => d.Equals(svcutilPkgRef)))
                {
                    bool success = project.AddDependency(svcutilPkgRef);
                    Assert.IsTrue(success, $"Could not add tool package dependency: dotnet-svcutil-lib.{svcutilPkgVersion}");
                }
            }

            var ret = project.RestoreAsync(logger, token).Result;
            Assert.IsTrue(ret.ExitCode == 0, $"Project package restore failed:{Environment.NewLine}{ret.OutputText}{logger}");

            if (build)
            {
                ret = project.BuildAsync(logger, token).Result;
                Assert.IsTrue(ret.ExitCode == 0, $"Project build failed:{Environment.NewLine}{ret.OutputText}{logger}");
            }

            return project;
        }

        public static ProcessRunner.ProcessResult RunSvcutil(this MSBuildProj project, string options, bool expectSuccess, ILogger logger, bool globalTool = false)
        {
            Assert.IsFalse(string.IsNullOrEmpty(options), $"{nameof(options)} not initialized!");
            Assert.IsTrue(File.Exists(project?.FullPath), $"{nameof(project)} is not initialized!");

            var envVars = new Dictionary<string, string> { { AppInsightsTelemetryClient.OptOutVariable, (!AppInsightsTelemetryClient.IsUserOptedIn).ToString() } };

            ProcessRunner.ProcessResult result;

            if (globalTool)
            {
                result = ProcessRunner.RunAsync("dotnet-svcutil", options, project.DirectoryPath, redirectOutput: true, throwOnError: false, environmentVariables: envVars, logger: logger, cancellationToken: CancellationToken.None).Result;
            }
            else
            {
                result = ProcessRunner.RunAsync("dotnet", $"svcutil-lib {options}", project.DirectoryPath, redirectOutput: true, throwOnError: false, environmentVariables: envVars, logger: logger, cancellationToken: CancellationToken.None).Result;
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
            Assert.IsFalse(string.IsNullOrEmpty(srcParamsFile), $"{nameof(srcParamsFile)} not initialized!");
            Assert.IsFalse(string.IsNullOrEmpty(referenceFolderName), $"{nameof(referenceFolderName)} not initialized!");
            Assert.IsTrue(File.Exists(project?.FullPath), $"{nameof(project)} is not initialized!");

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
