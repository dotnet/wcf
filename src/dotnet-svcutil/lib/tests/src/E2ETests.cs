// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Tools.ServiceModel.Svcutil;
using Xunit;

namespace SvcutilTest
{
    public partial class E2ETest : ClassFixture
    {
        private void TestFixture()
        {
            this_TestGroupBaselinesDir = Path.Combine(g_BaselinesDir, this_TestCaseName);
            this_TestGroupOutputDir = Path.Combine(g_TestResultsDir, this_TestCaseName);
            this_TestGroupBootstrapDir = Path.Combine(g_TestBootstrapDir, this_TestCaseName);
            this_TestGroupProjDir = Path.Combine(g_TestResultsDir, this_TestCaseName, "Project");

            if (!Directory.Exists(this_TestGroupOutputDir))
            {
                WriteLog("Svcutil version: " + g_SvcutilPkgVersion);
                WriteLog("SdkVersion version: " + g_SdkVersion);

                FileUtil.TryDeleteDirectory(this_TestGroupOutputDir);
                Directory.CreateDirectory(this_TestGroupOutputDir);

                FileUtil.TryDeleteDirectory(this_TestGroupProjDir);
                Directory.CreateDirectory(this_TestGroupProjDir);

                FileUtil.TryDeleteDirectory(this_TestGroupBootstrapDir);
                Directory.CreateDirectory(this_TestGroupBootstrapDir);

                Directory.CreateDirectory(this_TestGroupBaselinesDir);
            }

            Assert.True(Directory.Exists(this_TestGroupOutputDir), $"{nameof(this_TestGroupOutputDir)} is not initialized!");
            Assert.True(Directory.Exists(this_TestGroupBaselinesDir), $"{nameof(this_TestGroupBaselinesDir)} is not initialized!");
        }
        
        private void TestSvcutil(string options, bool expectSuccess = true)
        {
            Assert.False(string.IsNullOrWhiteSpace(this_TestCaseName), $"{nameof(this_TestCaseName)} not initialized!");
            Assert.False(options == null, $"{nameof(options)} not initialized!");

            // this sets the current directory to the project's.
            ProcessRunner.ProcessResult processResult = this_TestCaseProject.RunSvcutil(options, expectSuccess, this_TestCaseLogger);

            _ = $"{processResult.OutputText}{Environment.NewLine}{((TestLogger)this_TestCaseLogger)}";

            ValidateTest(options, this_TestCaseProject.DirectoryPath, processResult.ExitCode, processResult.OutputText, expectSuccess);
        }
        
        [Theory]
        [Trait("Category", "BVT")]
        [InlineData("silent")]
        [InlineData("normal")]
        [InlineData("verbose")]
        [InlineData("debug")]
        public void Help(string verbosity)
        {
            this_TestCaseName = "Help";
            TestFixture();
            var testCaseName = $"Help_{verbosity}";
            InitializeE2E(testCaseName);

            var options = $"-h -v {verbosity}";

            TestSvcutil(options);
        }

        [Trait("Category", "Test")]
        [Theory]
        [InlineData("tfmDefault", null)]
        [InlineData("tfmNet60", "net6.0")]
        [InlineData("tfmNetstd20", "netstandard2.0")]
        [InlineData("tfmNetstd21", "netstandard2.1")]
        public void TFMBootstrap(string testCaseName, string targetFramework)
        {
            this_TestCaseName = "TFMBootstrap";
            TestFixture();
            InitializeE2E(testCaseName, createUniqueProject: true, sdkVersion: g_SdkVersion);

            // set bootstrapper dir the same as the test output dir to validate generated files.
            this_TestCaseBootstrapDir = this_TestCaseOutputDir;
            // the boostrapper won't delete the folder if not created by it or with the -v Debug option 
            Directory.CreateDirectory(Path.Combine(this_TestCaseOutputDir, "SvcutilBootstrapper"));

            var uri = $"\"\"{Path.Combine(g_TestCasesDir, "wsdl", "Simple.wsdl")}\"\"";
            var tf = string.IsNullOrEmpty(targetFramework) ? string.Empty : $"-tf {targetFramework}";
            var tr = $"-r \"\"{{Newtonsoft.Json,*}}\"\" -bd {this_TestCaseBootstrapDir.Replace("\\", "/")}";
            var options = $"{uri.Replace("\\", "/")} {tf} {tr} -nl -tc global -v minimal -d ../{testCaseName} -n \"\"*,{testCaseName}_NS\"\"";

            TestSvcutil(options);
        }

        [Trait("Category", "Test")]
        [Theory]
        [InlineData("XmlSerializer", "-ser XmlSerializer")]
        [InlineData("DataContractSerializer", "-ser DataContractSerializer")]
        [InlineData("Auto", "-ser Auto")]
        [InlineData("MessageContract", "-mc")]
        [InlineData("EnableDataBinding", "-edb")]
        [InlineData("Internal", "-i")]
        [InlineData("CollectionArray", "-ct System.Array")]
        [InlineData("ExcludeType", "-et System.Net.HttpStatusCode")]
        [InlineData("NoStdLib", "-nsl")]
        [InlineData("wrapped", "-wr")]
        [InlineData("Sync", "-syn")]
        [InlineData("None", " ")]
        public void CodeGenOptions(string testCaseName, string optionModifier)
        {
            this_TestCaseName = "CodeGenOptions";
            TestFixture();
            InitializeE2E(testCaseName);

            var uri = Path.Combine(g_TestCasesDir, "wsdl", "WcfProjectNService", "tempuri.org.wsdl");
            var options = $"{uri} {optionModifier} -nl -tf netcoreapp1.0";
            this_TestCaseName = testCaseName;
            TestSvcutil(AppendCommonOptions(options));
        }

        [Trait("Category", "Test")]
        [Theory]
        [InlineData("badUrl", "http://$NO_REPLACEMENT$^$Simple.svc")]
        [InlineData("badwsdl", "$testCasesPath$/errorScenarios/badwsdl.wsdl.txt")]
        [InlineData("collection", "/ct:MyCollectionType")]
        [InlineData("exclude", "/et:MyExcludeType")]
        [InlineData("invalidAddress", "http://invalidaddress -bd $testCaseBootstratDir$")]
        [InlineData("noInputs", " ")]
        [InlineData("ser", "/--serializer MySerializer")]
        [InlineData("refVersionDoubleQuoted", "-r {Newtonsoft.Json, \"\"*\"\"}")]
        public void ErrorScenarios(string testCaseName, string options)
        {
            this_TestCaseName = "ErrorScenarios";
            TestFixture();
            InitializeE2E(testCaseName);

            options = options
                .Replace("$serviceUrl$", g_ServiceUrl)
                .Replace("$testCaseBootstratDir$", $"\"\"{this_TestCaseBootstrapDir}\"\"")
                .Replace("$testCasesPath$", g_TestCasesDir);

            this_TestCaseName = testCaseName;
            TestSvcutil(AppendCommonOptions(options), expectSuccess: false);
        }

        [Trait("Category", "Test")]
        [Theory]
        [InlineData("multiDocFullPath", "$wsdlDir$tempuri.org.wsdl")]
        [InlineData("multiDocAll", "$wsdlDir$*")]
        [InlineData("multiDocWsdlWildcard", "$wsdlDir$*.wsdl")]
        [InlineData("multiDocWsdlRelXsdWildcard", "$wsdlDir$*.wsdl ../wsdl/*.xsd")]
        [InlineData("multiDocFullAndWildcard", "$wsdlDir$tempuri.org.wsdl $wsdlDir$*.xsd")]
        [InlineData("multiDocAllRelative", "../wsdl/*")]
        [InlineData("multiDocWsdlWildcardRelative", "../wsdl/*.wsdl")]
        [InlineData("multiDocWsdlXsdWildcardRelative", "../wsdl/*.wsdl ../wsdl/*.xsd")]
        [InlineData("multiDocRelativeAndWildcard", "../wsdl/tempuri.org.wsdl ../wsdl/*.xsd")]
        [InlineData("multiDocRelativePath", "../wsdl/tempuri.org.wsdl")]
        public void MultipleDocuments(string testCaseName, string uri)
        {
            this_TestCaseName = "MultipleDocuments";
            TestFixture();
            InitializeE2E(testCaseName);

            //copy wsdl files into test project's path to make it easier to pass the params as relative paths.
            var wsdlFile = Path.Combine(this_TestGroupOutputDir, "wsdl", "tempuri.org.wsdl");
            if (!File.Exists(wsdlFile))
            {
                var wsdlDocsSrdDir = Path.Combine(g_TestCasesDir, "wsdl", "WcfProjectNService");
                FileUtil.CopyDirectory(wsdlDocsSrdDir, Path.GetDirectoryName(wsdlFile));
            }

            Assert.True(File.Exists(wsdlFile), $"{wsdlFile} not initialized!");

            uri = uri.Replace("$wsdlDir$", $"{Path.GetDirectoryName(wsdlFile)}{Path.DirectorySeparatorChar}");
            this_TestCaseName = testCaseName;
            TestSvcutil(AppendCommonOptions(uri));
        }

        [Trait("Category", "Test")]
        [Theory]
        [InlineData("wildcardNamespace", "-n *,TestNamespace", true)]
        [InlineData("invalidNamespace", "-n *,Invalid/TestNamespace", false)]
        [InlineData("urlNamespace", "-n http://schemas.datacontract.org/2004/07/WcfProjectNService,TestUrlNamespace", true)]
        public void NamespaceParam(string testCaseName, string options, bool expectSuccess)
        {
            this_TestCaseName = "NamespaceParam";
            TestFixture();
            InitializeE2E(testCaseName);

            var url = $"{Path.Combine(g_TestCasesDir, "wsdl", "WcfProjectNService", "tempuri.org.wsdl")}";
            var dir = $"-d ../{ testCaseName}";

            TestSvcutil(dir + " " + url + " " + options, expectSuccess);
        }

        [Trait("Category", "BVT")]
        [Theory]
        [InlineData("TypeReuse60", "net6.0")]
        public void TypeReuse(string testCaseName, string targetFramework)
        {
            this_TestCaseName = "TypeReuse";
            TestFixture();
            InitializeE2E(testCaseName, createUniqueProject: true, targetFramework: targetFramework, sdkVersion: g_SdkVersion);

            var uri = SetupProjectDependencies();
            var outDir = Path.Combine(this_TestCaseOutputDir, "ServiceReference");
            var options = $"{uri} -nl -v minimal -d {outDir.Replace("\\", "/")} -n \"\"*,{testCaseName}_NS\"\" -bd {this_TestCaseBootstrapDir.Replace("\\", "/")}";

            TestSvcutil(options, expectSuccess: true);
        }

        private string SetupProjectDependencies()
        {
            var libProjPath = Path.Combine(this_TestGroupOutputDir, "TypesLib", "TypesLib.csproj");
            var binProjPath = Path.Combine(this_TestGroupOutputDir, "BinLib", "BinLib.csproj");
            var assemblyPath = Path.Combine(Path.GetDirectoryName(binProjPath), "bin", "Debug", "netstandard1.3", "BinLib.dll");

            if (!File.Exists(assemblyPath))
            {
                var typeReuseProjectsPath = Path.Combine(g_TestCasesDir, "TypeReuse");

                FileUtil.CopyDirectory(typeReuseProjectsPath, this_TestGroupOutputDir);
                CreateGlobalJson(this_TestGroupOutputDir, this_TestCaseProject.SdkVersion);

                MSBuildProj binProj = MSBuildProj.FromPathAsync(binProjPath, null, CancellationToken.None).Result;
                ProcessRunner.ProcessResult result = binProj.BuildAsync(true, null, CancellationToken.None).Result;
                Assert.True(result.ExitCode == 0, result.OutputText);
            }

            Assert.True(File.Exists(binProjPath), $"{nameof(binProjPath)} not initialized!");
            Assert.True(File.Exists(libProjPath), $"{nameof(libProjPath)} not initialized!");

            this_TestCaseProject.AddDependency(ProjectDependency.FromAssembly(assemblyPath));
            this_TestCaseProject.AddDependency(ProjectDependency.FromProject(libProjPath));
            this_TestCaseProject.SaveAsync(this_TestCaseLogger, CancellationToken.None).Wait();

            ProcessRunner.ProcessResult ret = this_TestCaseProject.BuildAsync(true, this_TestCaseLogger, CancellationToken.None).Result;
            Assert.True(ret.ExitCode == 0, ret.OutputText);

            // keep the boostrapper projects in the outputs to be evaluated against baselines.
            this_TestCaseBootstrapDir = this_TestCaseOutputDir;
            Directory.CreateDirectory(Path.Combine(this_TestCaseBootstrapDir, "SvcutilBootstrapper"));

            var uri = PathHelper.GetRelativePath(Path.Combine(this_TestGroupOutputDir, "TypeReuseSvc.wsdl"), new DirectoryInfo(this_TestCaseOutputDir));
            return uri;
        }

        [Trait("Category", "BVT")]
        [Theory]
        [InlineData("UpdateServiceRefDefault", false)]
        [InlineData("UpdateServiceRefBootstrapping", true)]
        public void UpdateServiceRefBasic(string referenceFolderName, bool bootstrapping)
        {
            this_TestCaseName = "UpdateServiceRefBasic";
            TestFixture();
            var testCaseName = referenceFolderName;
            InitializeE2E(testCaseName, createUniqueProject: true, sdkVersion: g_SdkVersion);

            var paramsFile = SetupServiceReferenceFolder("dotnet-svcutil.params.json", referenceFolderName);

            if (bootstrapping)
            {
                var updateOptions = UpdateOptions.FromFile(paramsFile);
                // this forces bootstrapping of svcutil as the project reference is not available at runtime.
                updateOptions.References.Add(ProjectDependency.FromPackage("Newtonsoft.Json", "*"));
                updateOptions.Save(paramsFile);
            }

            var options = "-u -v minimal";
            TestSvcutil(options, expectSuccess: true);
        }

        [Trait("Category", "BVT")]
        [Theory]
        [InlineData("UpdateNetPipeServiceRefDefault", false)]
        [InlineData("UpdateNetPipeServiceRefBootstrapping", true)]
        public void UpdateNetPipeServiceRef(string referenceFolderName, bool bootstrapping)
        {
            this_TestCaseName = "UpdateNetPipeServiceRef";
            TestFixture();
            var testCaseName = referenceFolderName;
            InitializeE2E(testCaseName, createUniqueProject: true, sdkVersion: g_SdkVersion);

            var paramsFile = SetupServiceReferenceFolder("dotnet-svcutil.params.json", referenceFolderName, 1, true, "updateNetPipeServiceReference");

            if (bootstrapping)
            {
                var updateOptions = UpdateOptions.FromFile(paramsFile);
                // this forces bootstrapping of svcutil as the project reference is not available at runtime.
                updateOptions.References.Add(ProjectDependency.FromPackage("Newtonsoft.Json", "*"));
                updateOptions.Save(paramsFile);
            }

            var options = "-u -v minimal";
            TestSvcutil(options, expectSuccess: true);
        }

        [Trait("Category", "Test")]
        [Theory]
        [InlineData("UpdateServiceRefOptionsDefault", 1, null, true)]
        [InlineData("UpdateServiceRefOptions Folder With Spaces", 1, null, true)]
        [InlineData("UpdateServiceRefOptions Folder With Spaces Full", 1, "\"\"UpdateServiceRefOptions Folder With Spaces Full\"\"", true)]
        [InlineData("UpdateServiceRefOptionsRef", 1, "UpdateServiceRefOptionsRef", true)]
        [InlineData("UpdateServiceRefOptionsRef2", 2, "UpdateServiceRefOptionsRef2", true)]
        [InlineData("UpdateServiceRefOptions/Level1/Level2/UpdateRefLevels", 1, null, true)]
        [InlineData("UpdateServiceRefOptions/Level1/Level2/UpdateRefLevelsFull", 1, "UpdateServiceRefOptions/Level1/Level2/UpdateRefLevelsFull", true)]
        [InlineData("UpdateServiceRefOptionsFilePath", 1, "UpdateServiceRefOptionsFilePath/dotnet-svcutil.params.json", true)]
        [InlineData("UpdateServiceRefOptionsFullPath", 1, "$testCaseOutputDir$/UpdateServiceRefOptionsFullPath/dotnet-svcutil.params.json", true)]
        [InlineData("UpdateServiceRefOptionsExtraOptions", 1, "-nl", true)]
        [InlineData("UpdateServiceRefOptionsExtraOptionsWarn", 1, "-edb -nb -elm", true)]
        [InlineData("UpdateServiceRefOptionsDefaultOnMultipleRefs", 3, null, false)]
        [InlineData("UpdateServiceRefOptionsOnMissingRef", 1, "Inexistent", false)]
        public void UpdateServiceRefOptions(string referenceFolderName, int refCount, string cmdOptions, bool expectSuccess)
        {
            this_TestCaseName = "UpdateServiceRefOptions";
            TestFixture();
            var testCaseName = referenceFolderName.Replace(" ", "_").Split('/').Last();
            InitializeE2E(testCaseName, createUniqueProject: true, sdkVersion: g_SdkVersion);

            cmdOptions = cmdOptions?.Replace("$testCaseOutputDir$", this_TestCaseOutputDir.Replace("\\", "/"));
            var paramsFile = SetupServiceReferenceFolder("dotnet-svcutil.params.json", referenceFolderName, refCount, addNamespace: true);

            // disable type reuse (bootstrapping) to speed up test.
            var udpateOptions = UpdateOptions.FromFile(paramsFile);
            udpateOptions.TypeReuseMode = TypeReuseMode.None;
            udpateOptions.Save(paramsFile);

            var options = $"-u {cmdOptions} -v minimal";
            TestSvcutil(options, expectSuccess);
        }

        [Trait("Category", "Test")]
        [Theory]
        [InlineData("Connected Services/CSServiceReference", false)]
        [InlineData("Connected Services/CSServiceReferenceRoundtrip", true)]
        public void UpdateServiceRefWCFCS(string referenceFolderName, bool addNamespace)
        {
            this_TestCaseName = "UpdateServiceRefWCFCS";
            TestFixture();
            var testCaseName = referenceFolderName.Replace(" ", "_").Split('/').Last();
            InitializeE2E(testCaseName, createUniqueProject: true, sdkVersion: g_SdkVersion);

            var paramsFile = SetupServiceReferenceFolder("ConnectedService.json", referenceFolderName, refCount: 1, addNamespace: false);

            if (addNamespace)
            {
                // This will read the params file in WCF CS format but write it in Svcutil format.
                var wcfcsoptions = WCFCSUpdateOptions.FromFile(paramsFile);
                wcfcsoptions.NamespaceMappings.Clear();
                wcfcsoptions.NamespaceMappings.Add(new KeyValuePair<string, string>("*", $"{testCaseName}_NS"));
                wcfcsoptions.Save(paramsFile);
            }

            var options = "-u -v minimal";
            TestSvcutil(options, expectSuccess: true);
        }

        private string SetupServiceReferenceFolder(string paramsFileName, string referenceFolderName, int refCount = 1, bool addNamespace = true, string filePath = "updateServiceReference")
        {
            var srcParamsFilePath = Path.Combine(g_TestCasesDir, filePath, paramsFileName);
            Assert.True(File.Exists(srcParamsFilePath), $"{nameof(srcParamsFilePath)} not initialized!");

            // copy common test group project files into test cases's directory.
            Directory.EnumerateFiles(this_TestGroupOutputDir, "*", SearchOption.TopDirectoryOnly).Where(f =>
            {
                File.Copy(f, Path.Combine(this_TestCaseProject.DirectoryPath, Path.GetFileName(f)));
                return true;
            });

            var dstParamsFile = string.Empty;
            for (int idx = 0; idx < refCount; idx++)
            {
                var suffix = idx == 0 ? string.Empty : $"_{idx}";
                dstParamsFile = this_TestCaseProject.AddFakeServiceReference(srcParamsFilePath, referenceFolderName + suffix, addNamespace);
                File.WriteAllText(dstParamsFile, File.ReadAllText(dstParamsFile).Replace("$testCasesPath$", g_TestCasesDir.Replace("\\", "/")));
            }

            return dstParamsFile;
        }

        [Trait("Category", "Test")]
        [Theory]
        [InlineData("BasicAuth.svc", false)]
        [InlineData("BasicHttps.svc", true)]
        [InlineData("BasicHttp.svc", true)]
        [InlineData("BasicHttp_4_4_0.svc", true)]
        [InlineData("BasicHttpSoap.svc", true)]
        [InlineData("BasicHttpRpcEncSingleNs.svc", true)]
        [InlineData("BasicHttpRpcLitSingleNs.svc", true)]
        [InlineData("BasicHttpDocLitSingleNs.svc", true)]
        [InlineData("BasicHttpRpcEncDualNs.svc", true)]
        [InlineData("BasicHttpRpcLitDualNs.svc", true)]
        [InlineData("BasicHttpDocLitDualNs.svc", true)]
        public void WcfRuntimeBasicSvcs(string serviceName, bool expectSuccess)
        {
            this_TestCaseName = "WcfRuntimeBasicSvcs";
            TestFixture();

            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        [Trait("Category", "Test")]
        [Theory]
        [InlineData("BasicHttpsTransSecMessCredsUserName.svc", true)]
        public void WcfRuntimeBasicHttpsTransSecMessCredsUserName(string serviceName, bool expectSuccess)
        {
            this_TestCaseName = "WcfRuntimeBasicHttpsTransSecMessCredsUserName";
            TestFixture();

            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        [Trait("Category", "Test")]
        [Theory]
        [InlineData("TcpTransSecMessCredsUserName.svc", true)]
        public void WcfRuntimeNettcpTransSecMessCredsUserName(string serviceName, bool expectSuccess)
        {
            this_TestCaseName = "WcfRuntimeNettcpTransSecMessCredsUserName";
            TestFixture();
            var testCaseName = serviceName.Replace(".svc", "");
            InitializeE2E(testCaseName);

            var uri = $"{g_ServiceUrl}/{serviceName}".Replace("http", "net.tcp");
            this_TestCaseName = testCaseName;
            TestSvcutil(AppendCommonOptions(uri), expectSuccess);
        }

        [Trait("Category", "Test")]
        [Theory]
        [InlineData("HttpsTransSecMessCredsUserName.svc", true)]
        public void WsHttpBindingAndws2007HttpBindingTransSecMessCredsUserName(string serviceName, bool expectSuccess)
        {
            this_TestCaseName = "WsHttpBindingAndws2007HttpBindingTransSecMessCredsUserName";
            TestFixture();

            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        [Trait("Category", "Test")]
        [Theory]
        [InlineData("Saml2IssuedToken.svc/mex", true)]
        public void FederationServiceTest(string serviceName, bool expectSuccess)
        {
            this_TestCaseName = "FederationServiceTest";
            TestFixture();

            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        [Trait("Category", "Test")]
        [Theory]
        [InlineData("Duplex.svc", true)]
        public void WcfRuntimeDuplexCallback(string serviceName, bool expectSuccess)
        {
            this_TestCaseName = "DuplexCallback";
            TestFixture();

            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        [Trait("Category", "Test")]
        [Theory]
        [InlineData("NetHttp.svc", true)]
        [InlineData("NetHttpWebSockets.svc", true)]
        [InlineData("NetHttps.svc", true)]
        [InlineData("NetHttpsWebSockets.svc", true)]
        public void WcfRuntimeNetHttpSvcs(string serviceName, bool expectSuccess)
        {
            this_TestCaseName = "WcfRuntimeNetHttpSvcs";
            TestFixture();

            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        [Trait("Category", "Test")]
        [Theory]
        [InlineData("ReliableSessionService.svc", true)]
        public void WcfRuntimeReliableSessionSvc(string serviceName, bool expectSuccess)
        {
            this_TestCaseName = "WcfRuntimeReliableSessionSvc";
            TestFixture();

            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        private void WcfRuntimeSvcs(string serviceName, bool expectSuccess)
        {
            var testCaseName = serviceName.Replace(".svc", "").Replace("/", "_");
            InitializeE2E(testCaseName);

            var uri = $"{g_ServiceUrl}/{serviceName}";
            this_TestCaseName = testCaseName;
            TestSvcutil(AppendCommonOptions(uri), expectSuccess);
        }
        /*
        // TODO:
        // this is not an actual test but it is a way to keep the repo clean of dead-baselines.
        // in order to reliably run this test, it must run at the end, given that this
        // is a partial class it is hard to enforce the order.
        // need to find a way to keep this test running reliably.
        [Trait("Category", "Test")]
        [Fact]
        
        public void CheckBaslines()
        {
            this_TestCaseName = nameof(CheckBaslines);
            TestFixture();
            var allBaselines = Directory.EnumerateFiles(g_BaselinesDir, "*", SearchOption.AllDirectories).ToList();
            var allOutputs = Directory.EnumerateFiles(g_TestResultsDir, "*", SearchOption.AllDirectories).ToList();

            var baselinesWithNoOutputs = allBaselines.Where(b =>
                    !allOutputs.Any(o =>
                    {
                        var relPath1 = PathHelper.GetRelativePath(o, g_TestResultsDir);
                        var relPath2 = PathHelper.GetRelativePath(b, g_BaselinesDir);
                        return relPath1.Equals(relPath2, RuntimeEnvironmentHelper.FileStringComparison);
                    })).ToList();

            // The purpose of this test is to ensure all baselines are validated so no dead-baselines are kept in the repo.
            // This test must be the last to run so it must be the defined after all other tests in this file.
            if (baselinesWithNoOutputs.Count() > 0)
            {
                Assert.True(false, GenerateBaselineDeleteScript(baselinesWithNoOutputs));
            }
        }*/
    }
}
