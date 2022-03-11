// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Xunit;

namespace SvcutilTest
{
    public partial class E2ETest
    {
        private void TestGlobalSvcutil(string options, bool expectSuccess = true)
        {
            Assert.False(string.IsNullOrWhiteSpace(this_TestCaseName), $"{nameof(this_TestCaseName)} not initialized!");
            Assert.False(options == null, $"{nameof(options)} not initialized!");

            // this sets the current directory to the project's.
            var processResult = this_TestCaseProject.RunSvcutil(options, expectSuccess, this_TestCaseLogger, globalTool: true);
            var outputText = $"{processResult.OutputText}{Environment.NewLine}{((TestLogger)this_TestCaseLogger)}";

            ValidateTest(options, this_TestCaseProject.DirectoryPath, processResult.ExitCode, processResult.OutputText, expectSuccess);
        }

        [Theory]
        [Trait("Category", "BVT")]
        [InlineData("silent")]
        [InlineData("normal")]
        [InlineData("verbose")]
        [InlineData("debug")]
        public void HelpGlobal(string verbosity)
        {
            this_TestCaseName = "HelpGlobal";
            TestFixture();

            var testCaseName = $"HelpGlobal_{verbosity}";
            InitializeGlobal(testCaseName);

            var options = $"-h -v {verbosity}";
            TestGlobalSvcutil(options);
        }

        [Theory]
        [Trait("Category", "BVT")]
        [InlineData("Project", "--toolContext Project -ntr -tf netcoreapp2.0 -nb")]
        [InlineData("Global", "--toolcontext Global -ntr -tf netcoreapp2.0 -nb")]
        [InlineData("Infrastructure", "-tc Infrastructure -ntr -tf netcoreapp2.0 -nb")]
        [InlineData("None", "-ntr -tf netcoreapp2.0 -nb")]
        public void ToolContextGlobal(string testCaseName, string param)
        {
            this_TestCaseName = "ToolContextGlobal";
            TestFixture();
            testCaseName = $"ToolContext_{testCaseName}";
            InitializeGlobal(testCaseName);

            var options = $"{param}";
            TestGlobalSvcutil(options, expectSuccess: false);
        }

        [Theory]
        [Trait("Category", "Test")]
        [InlineData("tfmGlobalDefault", null)]
        [InlineData("tfmGlobalCoreapp20", "netcoreapp2.0")]
        [InlineData("tfmGlobalNetstd20", "netstandard2.0")]
        [InlineData("tfmGlobal100", "netcoreapp100.0")]
        public void TFMBootstrapGlobal(string testCaseName, string targetFramework)
        {
            this_TestCaseName = "TFMBootstrapGlobal";
            TestFixture();
            InitializeGlobal(testCaseName, sdkVersion: g_SdkVersion);

            // set bootstrapper dir the same as the test output dir to validate generated files.
            this_TestCaseBootstrapDir = this_TestCaseOutputDir;
            // the boostrapper won't delete the folder if not created by it or with the -v Debug option 
            Directory.CreateDirectory(Path.Combine(this_TestCaseOutputDir, "SvcutilBootstrapper"));

            var uri = $"\"{Path.Combine(g_TestCasesDir, "wsdl", "Simple.wsdl")}\"";
            var tf = string.IsNullOrEmpty(targetFramework) ? string.Empty : $"-tf {targetFramework}";
            var tr = $"-r \"{{Newtonsoft.Json, *}}\" -bd {this_TestCaseBootstrapDir.Replace("\\", "/")}";
            var options = $"{uri.Replace("\\", "/")} {tf} {tr} -nl -tc global -v minimal -d ../{testCaseName} -n \"*, {testCaseName}_NS\"";

            TestGlobalSvcutil(options);
        }

        [Trait("Category", "BVT")]
        [Theory]
        [InlineData("FullFramework")]
        public void FullFramework(string testCaseName)
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                return;
            }

            this_TestCaseName = "FullFramework";
            TestFixture();
            InitializeGlobal(testCaseName, targetFramework: "net5.0");
            this_TestCaseProject.TargetFramework = "net46";
            var uri = Path.Combine(g_TestCasesDir, "wsdl", "WcfProjectNService", "tempuri.org.wsdl");
            var outDir = Path.Combine(this_TestCaseOutputDir, "ServiceReference");
            var options = $"{uri} -nl -d {outDir} -tf netcoreapp1.0";

            this_TestCaseProject.AddDependency(Microsoft.Tools.ServiceModel.Svcutil.ProjectDependency.FromAssembly("System.ServiceModel"));
            this_TestCaseProject.SaveAsync(this_TestCaseLogger, System.Threading.CancellationToken.None).Wait();

            TestGlobalSvcutil(options, expectSuccess: true);
        }

        [Trait("Category", "BVT")]
        [Theory]
        [InlineData("netcoreapp3.1", "-edb")]
        [InlineData("net5.0", "-elm")]
        public void ParamsFiles_SDK_TFM(string targetFramework, string extraOptions)
        {
            this_TestCaseName = "ParamsFiles_SDK_TFM";
            TestFixture();
            var testCaseName = $"TF{targetFramework}".Replace(".", "_");
            InitializeGlobal(testCaseName, targetFramework, g_SdkVersion);

            var url = $"{Path.Combine(g_TestCasesDir, "wsdl", "Simple.wsdl")}";
            var ns = testCaseName.Replace(".", "_") + "_NS";

            // generate params file from options
            var paramsFilePath = Path.Combine(this_TestCaseOutputDir, $"{testCaseName}.params.json");
            var options = new Microsoft.Tools.ServiceModel.Svcutil.SvcutilOptions();
            options.Inputs.Add(new Uri(url));
            options.References.Add(Microsoft.Tools.ServiceModel.Svcutil.ProjectDependency.FromPackage("Newtonsoft.Json", "13.0.1"));
            options.OutputDir = new DirectoryInfo(this_TestCaseOutputDir);
            options.BootstrapPath = new DirectoryInfo(this_TestCaseBootstrapDir);
            options.NamespaceMappings.Add(new System.Collections.Generic.KeyValuePair<string, string>("*", ns));
            options.NoLogo = true;
            options.NoTelemetry = true;
            options.Project = this_TestCaseProject;
            options.ProviderId = testCaseName;
            options.ToolContext = Microsoft.Tools.ServiceModel.Svcutil.OperationalContext.Global;
            options.Version = g_SvcutilPkgVersion;
            options.Verbosity = Microsoft.Tools.ServiceModel.Svcutil.Verbosity.Minimal;
            options.Save(paramsFilePath);

            // extra options to test warnings on non-allowed user options.
            extraOptions = $"{extraOptions} -tc global";
            var input = $"{paramsFilePath} {extraOptions}";

            TestGlobalSvcutil(input, expectSuccess: true);
        }
    }
}
