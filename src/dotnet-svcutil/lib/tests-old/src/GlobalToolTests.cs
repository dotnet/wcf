// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SvcutilTest
{
    public partial class E2ETest
    {
        private void TestGlobalSvcutil(string options, bool expectSuccess = true)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(this_TestCaseName), $"{nameof(this_TestCaseName)} not initialized!");
            Assert.IsFalse(options == null, $"{nameof(options)} not initialized!");

            // this sets the current directory to the project's.
            var processResult = this_TestCaseProject.RunSvcutil(options, expectSuccess, this_TestCaseLogger, globalTool: true);
            var outputText = $"{processResult.OutputText}{Environment.NewLine}{((TestLogger)this_TestCaseLogger)}";

            ValidateTest(options, this_TestCaseProject.DirectoryPath, processResult.ExitCode, processResult.OutputText, expectSuccess);
        }

        [TestCategory("BVT")]
        [DataTestMethod]
        [DataRow("silent", DisplayName = "helpSilent")]
        [DataRow("normal", DisplayName = "helpMinimal")]
        [DataRow("verbose", DisplayName = "helpVerbose")]
        [DataRow("debug", DisplayName = "helpDebug")]
        public void HelpGlobal(string verbosity)
        {
            var testCaseName = $"HelpGlobal_{verbosity}";
            InitializeGlobal(testCaseName);

            var options = $"-h -v {verbosity}";
            TestGlobalSvcutil(options);
        }

        [TestCategory("BVT")]
        //[DataTestMethod]
        [DataRow("Project", "--toolContext Project -ntr -tf netcoreapp2.0 -nb", DisplayName = "Project")]
        [DataRow("Global", "--toolcontext Global -ntr -tf netcoreapp2.0 -nb", DisplayName = "Global")]
        [DataRow("Infrastructure", "-tc Infrastructure -ntr -tf netcoreapp2.0 -nb", DisplayName = "Infrastructure")]
        [DataRow("None", "-ntr -tf netcoreapp2.0 -nb", DisplayName = "None")]
        public void ToolContextGlobal(string testCaseName, string param)
        {
            testCaseName = $"ToolContext_{testCaseName}";
            InitializeGlobal(testCaseName);

            var options = $"{param}";
            TestGlobalSvcutil(options, expectSuccess: false);
        }

        [TestCategory("Test")]
        //[DataTestMethod]
        [DataRow("tfmGlobalDefault", null, DisplayName = "tfmGlobalDefault")]
        [DataRow("tfmGlobalCoreapp20", "netcoreapp2.0", DisplayName = "tfmGlobalCoreapp20")]
        [DataRow("tfmGlobalCoreapp10", "netcoreapp1.0", DisplayName = "tfmGlobalCoreapp10")]
        [DataRow("tfmGlobalNetstd20", "netstandard2.0", DisplayName = "tfmGlobalNetstd20")]
        [DataRow("tfmGlobal100", "netcoreapp100.0", DisplayName = "tfmGlobal100")]
        public void TFMBootstrapGlobal(string testCaseName, string targetFramework)
        {
            InitializeGlobal(testCaseName, targetFramework: "netcoreapp2.1", sdkVersion: "2.1.801");

            // set bootstrapper dir the same as the test output dir to validate generated files.
            this_TestCaseBootstrapDir = this_TestCaseOutputDir;
            // the boostrapper won't delete the folder if not created by it or with the -v Debug option 
            Directory.CreateDirectory(Path.Combine(this_TestCaseOutputDir, "SvcutilBootstrapper"));

            var uri = $"\"{Path.Combine(g_TestCasesDir, "wsdl/simple.wsdl")}\"";
            var tf = string.IsNullOrEmpty(targetFramework) ? string.Empty : $"-tf {targetFramework}";
            var tr = $"-r \"{{Newtonsoft.Json, *}}\" -bd {this_TestCaseBootstrapDir}";
            var options = $"{uri} {tf} {tr} -nl -tc global -v minimal -d {this_TestCaseName} -n \"*, {this_TestCaseName}_NS\"";

            TestGlobalSvcutil(options);
        }
    }
}
