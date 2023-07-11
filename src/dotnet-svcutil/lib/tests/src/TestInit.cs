// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Tools.ServiceModel.Svcutil;
using Xunit;

namespace SvcutilTest
{
    public class ClassFixture : IDisposable
    {
        #region fields and properties

        // global variables common to all tests.
        protected static string g_TestCasesDir;
        protected static string g_TestResultsDir;
        protected static string g_TestBootstrapDir;
        protected static string g_TestOutputDir;
        protected static string g_BaselinesDir;
        protected static string g_ServiceUrl;
        protected static string g_ServiceId;
        protected static string g_SvcutilNugetFeed;
        protected static string g_SvcutilPkgVersion;
        protected static string g_RepositoryRoot;
        protected static string g_SdkVersion;

        protected static bool containerAlreadyRunning;

        // test group-specific variables.
        protected string this_TestGroupBaselinesDir;
        protected string this_TestGroupOutputDir;
        protected string this_TestGroupBootstrapDir;
        protected string this_TestGroupProjDir;

        // test case-specific variables
        internal MSBuildProj this_TestCaseProject;
        protected string this_TestCaseName;
        protected string this_TestCaseBaselinesDir;
        protected string this_TestCaseOutputDir;
        protected string this_TestCaseBootstrapDir;
        protected string this_TestCaseSdkVersion;
        protected string this_TestCaseLogFile;
        internal ILogger this_TestCaseLogger;
        internal FixupUtil this_FixupUtil;

        internal static MSBuildProj __g_StarterProject;
        internal static MSBuildProj g_StarterProject
        {
            get
            {
                if (__g_StarterProject == null)
                {
                    File.WriteAllText(Path.Combine(g_TestOutputDir, "Directory.Build.props"), "<Project xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\" />");
                    File.WriteAllText(Path.Combine(g_TestOutputDir, "Directory.Build.targets"), "<Project></Project>");
                    var projectPath = Path.Combine(g_TestOutputDir, "TestStarterProject", "TestStarterProject.csproj");
                    __g_StarterProject = ProjectUtils.GetProject(projectPath, targetFramework: null, forceNew: true, build: false, logger: null);
                }
                return __g_StarterProject;
            }
        }

        internal static string buildType
        {
            get
            {
#if DEBUG
                return "Debug";
#else
                return "Release";
#endif
            }
        }

        // special-name TestGroup projects to identify them when updating baselines as they are not not tracked.
        // because they are not specific to any test.
        protected string this_TestGroupProjectName { get { return $"{this_TestCaseName}_TestGroup.csproj"; } }

        internal MSBuildProj __this_TestGroupProject;
        internal MSBuildProj this_TestGroupProject
        {
            get
            {
                if (__this_TestGroupProject == null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(this_TestGroupProjectName);
                    var filePath = Path.Combine(this_TestGroupProjDir, this_TestGroupProjectName);

                    if (!File.Exists(filePath))
                    {
                        // copy project to avoid 'dotnet new + restore' and save a few seconds.
                        CopyProject(g_StarterProject, this_TestGroupProjDir, fileName);
                    }

                    __this_TestGroupProject = ProjectUtils.GetProject(filePath, targetFramework: null, forceNew: false, build: false, logger: this_TestCaseLogger);
                }

                return __this_TestGroupProject as MSBuildProj;
            }
        }

        internal static string CopyProject(MSBuildProj project, string outputDir, string projectName)
        {
            // no dotnet new/no build/no restore
            var projectPath = Path.Combine(outputDir, $"{projectName}.csproj");
            var programPath = Path.Combine(outputDir, $"{projectName}.cs");
            var srcProgramPath = Directory.GetFiles(project.DirectoryPath, "*.cs", SearchOption.TopDirectoryOnly).First();

            File.Copy(project.FullPath, projectPath);
            File.Copy(srcProgramPath, programPath);

            File.WriteAllText(programPath, File.ReadAllText(programPath)
                .Replace(Path.GetFileNameWithoutExtension(project.FileName), projectName));

            return projectPath;
        }
        #endregion

        #region Initialization
        public ClassFixture()
        {
            AppInsightsTelemetryClient.IsUserOptedIn = false;

            g_RepositoryRoot = FindRepositoryRoot();

            var vsTestsRoot = GetSTestRootDir();

            g_SdkVersion = "5.0.100";
            ProcessRunner.ProcessResult procResult = ProcessRunner.TryRunAsync("dotnet", "--version", Directory.GetCurrentDirectory(), null, new CancellationToken()).ConfigureAwait(false).GetAwaiter().GetResult();

            if (procResult.ExitCode == 0)
            {
                g_SdkVersion = procResult.OutputText.Trim();
            }

            g_ServiceUrl = "http://wcfcoresrv53.westus3.cloudapp.azure.com/wcftestservice1";
            g_TestOutputDir = Path.Combine(g_RepositoryRoot, "artifacts", "TestOutput");
            g_TestResultsDir = Path.Combine(g_TestOutputDir, "TestResults");
            g_TestBootstrapDir = Path.Combine(g_TestOutputDir, "TestBootstrap");
            g_TestCasesDir = Path.Combine(vsTestsRoot, "TestCases");
            g_BaselinesDir = Path.Combine(vsTestsRoot, "Baselines");

            Directory.CreateDirectory(g_TestOutputDir);
            Directory.CreateDirectory(g_TestResultsDir);
            Directory.CreateDirectory(g_TestBootstrapDir);
            Directory.CreateDirectory(g_TestCasesDir);
            Directory.CreateDirectory(g_BaselinesDir);

            Assert.True(Directory.Exists(g_RepositoryRoot), $"{nameof(g_RepositoryRoot)} is not initialized!");
            Assert.True(Directory.Exists(g_TestCasesDir), $"{nameof(g_TestCasesDir)} is not initialized!");
            Assert.True(Directory.Exists(g_TestResultsDir), $"{nameof(g_TestResultsDir)} is not initialized!");
            Assert.True(Directory.Exists(g_TestBootstrapDir), $"{nameof(g_TestBootstrapDir)} is not initialized!");
            Assert.True(Directory.Exists(g_TestOutputDir), $"{nameof(g_TestOutputDir)} is not initialized!");
            Assert.True(Directory.Exists(g_BaselinesDir), $"{nameof(g_BaselinesDir)} is not initialized!");

            GetSvcutilPkgVersionAndFeed();
            CreateNuGetConfig();
            CreateGlobalJson(g_TestOutputDir, g_SdkVersion);

            // force creation of common project.
            Assert.True(File.Exists(g_StarterProject.FullPath), $"{nameof(g_StarterProject)} is not initialized!");

            Assert.False(string.IsNullOrEmpty(g_SvcutilNugetFeed), $"{nameof(g_SvcutilNugetFeed)} is not initialized!");
            Assert.False(string.IsNullOrEmpty(g_SvcutilPkgVersion), $"{nameof(g_SvcutilPkgVersion)} is not initialized!");

            // copy MSBuild global props/targets file to test folder to disable build customizations for test projects.
            File.WriteAllText(Path.Combine(g_TestOutputDir, "Directory.Build.props"), "<Project xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\" />");
            File.WriteAllText(Path.Combine(g_TestOutputDir, "Directory.Build.targets"), "<Project></Project>");

            // Uninstall the global tool and install the current version.
            var currentDirectory = Directory.GetCurrentDirectory();
            ProcessRunner.ProcessResult ret = ProcessRunner.TryRunAsync("dotnet", "tool uninstall -g dotnet-svcutil", currentDirectory, null, CancellationToken.None).Result;
            string pkgPath = Path.Combine(g_RepositoryRoot, "artifacts", "packages", buildType, "Shipping");
            if (!Directory.Exists(pkgPath) || !Directory.GetFiles(pkgPath, "dotnet-svcutil-lib*.nupkg").Any())
            {
                pkgPath = Path.Combine(g_RepositoryRoot, "artifacts", "packages", buildType, "NonShipping");
            }
            Assert.True(Directory.GetFiles(pkgPath, "dotnet-svcutil-lib.*.nupkg").Any(), $"dotnet-svcutil-lib*.nupkg not found under {pkgPath}!");
            ret = ProcessRunner.TryRunAsync("dotnet", $"tool install --global --add-source {pkgPath} dotnet-svcutil --version {g_SvcutilPkgVersion}", currentDirectory, null, CancellationToken.None).Result;
            Assert.True(ret.ExitCode == 0, "Could not install the global tool." + Environment.NewLine + ret.OutputText);
        }

        public void Dispose()
        {
        }

        protected void InitializeE2E(string testCaseName, bool createUniqueProject = false, string targetFramework = null, string sdkVersion = null)
        {
            InitializeUnitTest(testCaseName, createProject: false);
            this_TestCaseLogger = new TestLogger() { Verbose = true };

            Directory.CreateDirectory(this_TestCaseBootstrapDir);

            this_TestCaseSdkVersion = sdkVersion ?? g_SdkVersion;

            if (sdkVersion != null)
            {
                CreateGlobalJson(createUniqueProject ? this_TestCaseOutputDir : this_TestGroupOutputDir, sdkVersion);
            }

            if (createUniqueProject)
            {
                // create a project specific to the test case.
                var projectPath = Path.Combine(this_TestCaseOutputDir, $"{testCaseName}.csproj");
                this_TestCaseProject = ProjectUtils.GetProject(projectPath, targetFramework, forceNew: true, build: true, logger: this_TestCaseLogger);
            }
            else
            {
                this_TestCaseProject = this_TestGroupProject;
            }
        }

        public void InitializeGlobal(string testCaseName, string targetFramework = null, string sdkVersion = null)
        {
            InitializeUnitTest(testCaseName, createProject: false);
            this_TestCaseLogger = new TestLogger() { Verbose = true };

            Directory.CreateDirectory(this_TestCaseBootstrapDir);

            this_TestCaseSdkVersion = sdkVersion ?? g_SdkVersion;

            if (sdkVersion != null)
            {
                CreateGlobalJson(this_TestCaseOutputDir, sdkVersion);
            }
            
            // create a project specific to the test case.
            var projectPath = Path.Combine(this_TestCaseOutputDir, $"{testCaseName}.csproj");
            this_TestCaseProject = ProjectUtils.GetProject(projectPath, targetFramework, forceNew: true, build: true, logger: this_TestCaseLogger, globalTool: true);
        }

        public void InitializeUnitTest(string testCaseName, bool createProject = true, string sdkVersion = null)
        {
            Assert.False(string.IsNullOrWhiteSpace(testCaseName), $"{nameof(testCaseName)} is not initialized!");

            this_TestCaseOutputDir = Path.Combine(this_TestGroupOutputDir, testCaseName);
            FileUtil.TryDeleteDirectory(this_TestCaseOutputDir);
            Directory.CreateDirectory(this_TestCaseOutputDir);

            // do not create directory as it is not expected to be used by a unit-test.
            this_TestCaseBootstrapDir = Path.Combine(this_TestGroupBootstrapDir, testCaseName);

            this_TestCaseLogger = new DebugLogger();
            this_TestCaseLogFile = Path.Combine(this_TestGroupOutputDir, $"{testCaseName}.log");

            this_TestCaseSdkVersion = sdkVersion;

            if (createProject == true)
            {
                var project = (sdkVersion == null || g_StarterProject.SdkVersion == sdkVersion) ? g_StarterProject : this_TestGroupProject;
                var projectPath = CopyProject(g_StarterProject, this_TestCaseOutputDir, testCaseName);

                this_TestCaseProject = MSBuildProj.FromPathAsync(projectPath, null, CancellationToken.None).Result;
            }
            else
            {
                this_TestCaseProject = g_StarterProject;
            }

            this_TestCaseBaselinesDir = Path.Combine(this_TestGroupBaselinesDir, testCaseName);
            var linuxBaselinePath = Path.Combine(this_TestCaseBaselinesDir, "Linux");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && File.Exists(Path.Combine(linuxBaselinePath, Path.GetFileName(this_TestCaseLogFile))))
            {
                this_TestCaseBaselinesDir = linuxBaselinePath;
            }
            Directory.CreateDirectory(this_TestCaseBaselinesDir);

            this_FixupUtil = new FixupUtil();
            this_FixupUtil.Init(g_TestResultsDir, g_TestCasesDir, this_TestCaseOutputDir, g_ServiceUrl, g_ServiceId, g_RepositoryRoot);

            Assert.True(Directory.Exists(this_TestCaseOutputDir), $"{nameof(this_TestGroupOutputDir)} is not initialized!");
            Assert.True(Directory.Exists(this_TestCaseBaselinesDir), $"{nameof(this_TestCaseBaselinesDir)} is not initialized!");
            Assert.True(this_TestCaseBootstrapDir != null, $"{nameof(this_TestCaseBootstrapDir)} is not initialized!");
            Assert.True(this_TestCaseLogger != null, $"{nameof(this_TestGroupOutputDir)} is not initialized!");
            Assert.True(this_FixupUtil != null, $"{nameof(this_FixupUtil)} is not initialized!");
        }
        #endregion

        protected string AppendCommonOptions(string options)
        {
            // add the namespace option to avoid type clashing in the generated code and be able to compile the project when more than one service ref is added.
            options = $"{options} -v Minimal -d ../{this_TestCaseName} -n \"\"*,{this_TestCaseName}_NS\"\" -ntr";
            return options;
        }

        #region validation methods
        static readonly string g_DiffBaselineErrorFormat =
                Environment.NewLine + Environment.NewLine + "Error: The generated file doesn't match the baseline! Different lines:" +
                Environment.NewLine + "< \"{0}\"" +
                Environment.NewLine + "> \"{1}\"" + Environment.NewLine +
                Environment.NewLine + "  windiff \"{2}\" \"{3}\"" + Environment.NewLine +
                Environment.NewLine + "To update the baseline run the following command:" +
                Environment.NewLine + "  copy /y \"{3}\" \"{2}\"";

        static readonly string g_NoBaselineErrorFormat =
                Environment.NewLine + Environment.NewLine + "Error: A file was generated but no baseline exists!" +
                Environment.NewLine + "The \"{0}\" file was created and need to be committed into the source repo. Rerunning the test should succeed if no other error occurred!";

        static readonly string g_UnvalidatedBaselineMessageFormat =
                 Environment.NewLine + Environment.NewLine + "No generated file was found matching the following baselines:" + Environment.NewLine + "{0}" +
                 Environment.NewLine + "If no longer valid, run the following command script to delete them:" + Environment.NewLine + "  {1}";

        public static readonly string g_ToReproduceProblemMessageFormat =
                Environment.NewLine + Environment.NewLine + "To reproduce the problem run the following commands:" + Environment.NewLine +
                Environment.NewLine + "  pushd  {0}" + Environment.NewLine +
                Environment.NewLine + "  dotnet svcutil {1}";

        public static readonly string g_CommandAndWorkdirMessageFormat =
                Environment.NewLine + "Command: dotnet svcutil {0}" +
                Environment.NewLine + "Workdir: {1}";

        public static readonly string g_GeneralErrMsg = $"{Environment.NewLine}{Environment.NewLine}Click the Output link for a full report.";

        // get test case generated files. NOTE: common test project is not considered nor fixed up.
        public static readonly string[] g_GeneratedExtensions = new string[] { ".cs", ".log", ".csproj", ".json", ".config", ".wsdl" };

        protected void ValidateTest(string options, string workingDir, int exitCode, string outputText, bool expectSuccess = true)
        {
            var isSuccess = exitCode == 0 || exitCode == 6;
            var actualText = isSuccess ? "Success" : "Failure";
            var expectedText = expectSuccess ? "Success" : "Failure";
            var resultsMsg = Environment.NewLine + $"Svcutil Result: {actualText}. Expected: {expectedText}";
            var isTestSucess = !(isSuccess ^ expectSuccess);

            var commandMsg = string.Format(g_CommandAndWorkdirMessageFormat, options, workingDir);
            var logText = $"{Environment.NewLine}{outputText}{Environment.NewLine}{Environment.NewLine}{commandMsg}{resultsMsg}";

            WriteLog(logText, options);
            File.WriteAllText(this.this_TestCaseLogFile, logText);

            Assert.True(isTestSucess, $"{Environment.NewLine}Test failed:{Environment.NewLine}{outputText}{g_GeneralErrMsg}");

            // validate test outputs only if test succeeded not to fix up files and be able to reproduce the problem.
            var validationSuccess = ValidateOutputs(options, out var failureMessage);
            WriteLog(failureMessage);

            Assert.True(validationSuccess, $"{Environment.NewLine}Test failed validation!{failureMessage}{g_GeneralErrMsg}");
        }

        protected bool ValidateOutputs(string options, out string failureMessage)
        {
            return ValidateOutputs(options, true, out failureMessage);
        }

        protected bool ValidateOutputs(string options, bool isSuccess, out string failureMessage)
        {
            var linuxBaselinePath = Path.Combine(this_TestCaseBaselinesDir, "Linux");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && File.Exists(Path.Combine(linuxBaselinePath, Path.GetFileName(this_TestCaseLogFile))))
            {
                this_TestCaseBaselinesDir = linuxBaselinePath;
            }

            var excludeDirs = new string[] { "bin", "obj", "lib" };

            // get test case baselines.
            var baselineFiles = Directory.GetFiles(this_TestCaseBaselinesDir, "*", SearchOption.AllDirectories)
                                    .Where(f => !PathHelper.PathHasFolder(f, excludeDirs, this_TestCaseBaselinesDir)).Where(f => !f.ToLower().EndsWith("nuget.config")).ToList();

            var nonRefbaselineFiles = baselineFiles.Where(f => f.EndsWith(".cs") && !f.EndsWith(Path.DirectorySeparatorChar + "reference.cs", StringComparison.OrdinalIgnoreCase)).ToList();

            baselineFiles = baselineFiles.Except(nonRefbaselineFiles).ToList();
            
            // we don't check the bootstrapping directory, for those tests where it is relevant the test should 
            // place the bootstrapping dir under the test case's output dir.
            var generatedFiles = Directory.GetFiles(this_TestCaseOutputDir, "*.*", SearchOption.AllDirectories)
                                    .Where(f => !PathHelper.PathHasFolder(f, excludeDirs, this_TestCaseOutputDir))
                                    .Where(f => !f.ToLower().EndsWith("nuget.config"))
                                    .Where(f => g_GeneratedExtensions
                                    .Any(e => e.Equals(Path.GetExtension(f), RuntimeEnvironmentHelper.FileStringComparison))).ToList();
            
            var nonRefGeneratedFiles = generatedFiles.Where(f => f.EndsWith(".cs") && !f.EndsWith(Path.DirectorySeparatorChar + "reference.cs", StringComparison.OrdinalIgnoreCase)).ToList();
            generatedFiles = generatedFiles.Except(nonRefGeneratedFiles).ToList();

            generatedFiles = generatedFiles.Distinct().Select(g => this_FixupUtil.FixupFile(g)).ToList();

            ValidateFiles(this_TestGroupOutputDir, this_TestGroupBaselinesDir, this_TestGroupBootstrapDir, generatedFiles, baselineFiles, out failureMessage);

            return string.IsNullOrEmpty(failureMessage);
        }

        protected bool ValidateFiles(string outputDir, string baselineDir, string bootstrapDir, List<string> generatedFiles, List<string> baselineFiles, out string failureMessage)
        {
            failureMessage = string.Empty;

            // populate tuple of generated files and baselines (if exist)
            var testFileTable = generatedFiles.Select(g => new Tuple<string, string>(g, baselineFiles.FirstOrDefault(b =>
            {
                var bRel = PathHelper.GetRelativePath(b, new DirectoryInfo(baselineDir));
                var gRel = PathHelper.GetRelativePath(g, new DirectoryInfo(outputDir));
                return bRel.Equals(gRel, RuntimeEnvironmentHelper.FileStringComparison);
            })));

            var generatedWithBaseline = testFileTable.Where(t => t.Item2 != null);
            var generatedWithNoBaseline = testFileTable.Where(t => t.Item2 == null).Select(t => t.Item1);

            var baselineWithNoGenerated = baselineFiles.Where(b => !generatedFiles.Any(g =>
            {
                var bRel = PathHelper.GetRelativePath(b, new DirectoryInfo(baselineDir));
                var gRel = PathHelper.GetRelativePath(g, new DirectoryInfo(outputDir));
                return bRel.Equals(gRel, RuntimeEnvironmentHelper.FileStringComparison);
            }));

            // Compare with baselines.
            foreach (var tuple in generatedWithBaseline)
            {
                var generated = tuple.Item1;
                var baseline = tuple.Item2;
                if (!CompareFiles(baseline, generated, out var diffLines))
                {
                    failureMessage += string.Format(g_DiffBaselineErrorFormat, diffLines.Value.Key, diffLines.Value.Value, baseline, generated);
                }
            }

            // copy generated files with no baseline.
            foreach (var generated in generatedWithNoBaseline)
            {
                PathHelper.GetRelativePath(generated, new DirectoryInfo(outputDir), out var relativePath);
                if (string.IsNullOrEmpty(relativePath))
                {
                    PathHelper.GetRelativePath(generated, new DirectoryInfo(bootstrapDir), out relativePath);
                }
                var expected = Path.Combine(baselineDir, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(expected));
                File.Copy(generated, expected);
                failureMessage += string.Format(g_NoBaselineErrorFormat, expected);
            }

            // generate delete script for unvalidated baselines in case they are no longer valid.
            if (baselineWithNoGenerated.Count() > 0)
            {
                failureMessage += GenerateBaselineDeleteScript(baselineWithNoGenerated);
            }

            return string.IsNullOrEmpty(failureMessage);
        }

        protected string GenerateBaselineDeleteScript(IEnumerable<string> unvalidatedBaselines)
        {
            var scriptPath = Path.Combine(g_TestOutputDir, @"TestScripts", $"{this_TestCaseName}.cmd");
            var globalScriptPath = Path.Combine(g_TestOutputDir, @"TestScripts\deleteAll.cmd");

            var errMsg = new StringBuilder();
            var cmdStr = new StringBuilder();

            foreach (var baseline in unvalidatedBaselines)
            {
                errMsg.AppendLine($"+ {baseline}");
                cmdStr.AppendLine($"del /s {baseline}");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(scriptPath));
            File.WriteAllText(scriptPath, cmdStr.ToString());

            return string.Format(g_UnvalidatedBaselineMessageFormat, unvalidatedBaselines
                .Aggregate((msg, b) => $"{msg}{Environment.NewLine}{b}"), scriptPath);
        }

        protected bool CompareFiles(string baselineFile, string generatedFile, out KeyValuePair<string, string>? diffLines)
        {
            diffLines = null;

            var fileLines1 = new List<string>();
            var fileLines2 = new List<string>();
            var exceptLines = new List<string>() { "<ImplicitUsings>enable</ImplicitUsings>", "<Nullable>enable</Nullable>", "<Content CopyToOutputDirectory" };

            fileLines1.AddRange(File.ReadAllLines(baselineFile));
            fileLines2.AddRange(File.ReadAllLines(generatedFile));

            fileLines1.RemoveAll(l => exceptLines.Any(ex => l.Contains(ex)));
            fileLines2.RemoveAll(l => exceptLines.Any(ex => l.Contains(ex)));

            // to reduce noise, let's ignore empty lines in log files (only).
            var isLogFile = Path.GetExtension(baselineFile).Equals(".log", StringComparison.OrdinalIgnoreCase);
            if (isLogFile && fileLines1.Count != fileLines2.Count)
            {
                fileLines1 = fileLines1.Where(f => !string.IsNullOrWhiteSpace(f)).ToList();
                fileLines2 = fileLines2.Where(f => !string.IsNullOrWhiteSpace(f)).ToList();
            }

            if (fileLines1.Count != fileLines2.Count)
            {
                diffLines = new KeyValuePair<string, string>($"Line count file1: {fileLines1.Count}", $"Line count file2: {fileLines2.Count}");
                return false;
            }

            // some source lines get generated in different order (mostly cs files), to reduce noise let's compare source lines.
            fileLines1.Sort();
            fileLines2.Sort();

            for (int i = 0; i < fileLines1.Count; i++)
            {
                if (fileLines1[i] != fileLines2[i])
                {
                    diffLines = new KeyValuePair<string, string>(fileLines1[i], fileLines2[i]);
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region helper methods
        protected void WriteLog(string msg, string options = null)
        {
            if (options != null)
            {
                WriteParams(options);
            }
            
        }

        protected void WriteParams(string options)
        {
            WriteLog("Test name: " + this_TestCaseName);
            WriteLog("Parameters: " + options);
            WriteLog("Root path: " + g_TestCasesDir);
            WriteLog("Working dir: " + this_TestCaseProject?.DirectoryPath);
            WriteLog("Results dir: " + this_TestCaseOutputDir);
            WriteLog("Baslines dir: " + this_TestCaseBaselinesDir);
            WriteLog("Bootstrap dir: " + this_TestCaseBootstrapDir);
            WriteLog("Test project: " + this_TestCaseProject?.FullPath);
            WriteLog("Log file: " + this_TestCaseLogFile);
        }

        protected static string GetSTestRootDir()
        {
            // Root path is current working directory if TestCases exists in it. Otherwise set it 
            // based on the repository root.
            var vstestDir = Directory.GetCurrentDirectory();
            if (!Directory.Exists(Path.Combine(vstestDir, "TestCases")))
            {
                vstestDir = new DirectoryInfo(Path.Combine(g_RepositoryRoot, "src", "dotnet-svcutil", "lib", "tests")).FullName;
                Assert.True(Directory.Exists(vstestDir), $"{nameof(vstestDir)} is not initialized!");
            }
            return vstestDir;
        }

        internal static string GetSvcutilPkgVersionAndFeed()
        {
            if (g_SvcutilPkgVersion == null)
            {
                string nugetId;
                string[] nugetFiles = Directory.GetFiles(g_TestCasesDir, "*.nupkg", SearchOption.TopDirectoryOnly);
                Assert.True(nugetFiles.Length <= 1, "There should be one and only one nuget package for testing.");

                if (nugetFiles.Length == 0)
                {
                    var binDir = Path.Combine(g_RepositoryRoot, "artifacts", "packages", buildType, "Shipping");
                    if (!Directory.Exists(binDir) || !Directory.GetFiles(binDir, "dotnet-svcutil-lib.*.nupkg").Any())
                    {
                        binDir = Path.Combine(g_RepositoryRoot, "artifacts", "packages", buildType, "NonShipping");
                    }

                    DirectoryInfo binDirectory = new DirectoryInfo(binDir);

                    FileInfo nugetFile = binDirectory.GetFiles("dotnet-svcutil-lib.*.nupkg", SearchOption.AllDirectories).Where(f => !f.Name.Contains("symbols")).OrderByDescending(f => f.LastWriteTime).FirstOrDefault();

                    Assert.NotNull(nugetFile);

                    g_SvcutilNugetFeed = nugetFile.DirectoryName;
                    nugetId = Path.GetFileNameWithoutExtension(nugetFile.Name);
                }
                else
                {
                    g_SvcutilNugetFeed = g_TestCasesDir;
                    nugetId = Path.GetFileNameWithoutExtension(nugetFiles[0]);
                }

                g_SvcutilPkgVersion = nugetId.Substring("dotnet-svcutil-lib.".Length);
            }

            return g_SvcutilPkgVersion;
        }

        internal static string FindRepositoryRoot()
        {
            if (g_RepositoryRoot == null)
            {
                if (string.IsNullOrEmpty(g_RepositoryRoot))
                {
                    var parentDir = Directory.GetCurrentDirectory();
                    var rootSolutionFolder = PathHelper.TryFindFileAsync("dotnet-svcutil.sln", parentDir, null, CancellationToken.None).Result;
                    Assert.True(rootSolutionFolder != null && File.Exists(rootSolutionFolder), $"Unable to find dotnet-svcutil.sln file, current dir: {parentDir}");

                    g_RepositoryRoot = Path.GetDirectoryName(rootSolutionFolder);
                }
            }

            return g_RepositoryRoot;
        }

        // Need to fix the dotnet SDK version as newer versions report different messages, making it hard to keep up with the log baselines 
        protected const string globalConfigFormat = "{{ \"sdk\": {{ \"version\": \"{0}\" }} }}";

        internal static void CreateGlobalJson(string directory, string sdkVersion)
        {
            var globalConfig = string.Format(globalConfigFormat, sdkVersion);
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, "global.json"), globalConfig);
        }

        // The build system require access to VSTS feeds which cannot be provided by the tests, need to remove that feed from nuget.config.
        protected static string nugetConfigText =
"<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine +
"<configuration>" + Environment.NewLine +
"  <packageSources>" + Environment.NewLine +
"    <clear />" + Environment.NewLine +
"    <add key = \"svcutilTestFeed\" value=\"$svcutilTestFeed$\" />" + Environment.NewLine +
"    <add key = \"dotnet-public\" value=\"https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-public/nuget/v3/index.json\" />" + Environment.NewLine +
"  </packageSources>" + Environment.NewLine +
"</configuration>" + Environment.NewLine;

        internal static void CreateNuGetConfig()
        {
            GetSvcutilPkgVersionAndFeed();
            Directory.CreateDirectory(g_TestOutputDir);
            File.WriteAllText(Path.Combine(g_TestOutputDir, "nuget.config"), nugetConfigText.Replace("$svcutilTestFeed$", g_SvcutilNugetFeed));
        }
        #endregion

        protected class CurrentDirectorySetter : IDisposable
        {
            protected string _cashedWorkingDirectory;

            public CurrentDirectorySetter(string currentDirectory)
            {
                this._cashedWorkingDirectory = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(currentDirectory);
            }

            public void Dispose()
            {
                Directory.SetCurrentDirectory(this._cashedWorkingDirectory);
            }
        }
    }
}
