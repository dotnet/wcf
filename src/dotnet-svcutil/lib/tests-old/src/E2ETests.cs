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
    [TestClass]
    public partial class E2ETest
    {
        private void TestSvcutil(string options, bool expectSuccess = true)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(this_TestCaseName), $"{nameof(this_TestCaseName)} not initialized!");
            Assert.IsFalse(options == null, $"{nameof(options)} not initialized!");

            // thi sets the current directory to the project's.
            var processResult = this_TestCaseProject.RunSvcutil(options, expectSuccess, this_TestCaseLogger);
            var outputText = $"{processResult.OutputText}{Environment.NewLine}{((TestLogger)this_TestCaseLogger)}";

            ValidateTest(options, this_TestCaseProject.DirectoryPath, processResult.ExitCode, processResult.OutputText, expectSuccess);
        }


        [TestCategory("BVT")]
        [DataTestMethod]
        [DataRow("silent", DisplayName = "helpSilent")]
        [DataRow("normal", DisplayName = "helpMinimal")]
        [DataRow("verbose", DisplayName = "helpVerbose")]
        [DataRow("debug", DisplayName = "helpDebug")]
        public void Help(string verbosity)
        {
            var testCaseName = $"Help_{verbosity}";
            InitializeE2E(testCaseName);

            var options = $"-h -v {verbosity}";
            TestSvcutil(options);
        }

        [TestCategory("Test")]
        //[DataTestMethod]
        [DataRow("tfmDefault", null, DisplayName = "tfmDefault")]
        [DataRow("tfmCoreapp20", "netcoreapp2.0", DisplayName = "tfmCoreapp20")]
        [DataRow("tfmCoreapp10", "netcoreapp1.0", DisplayName = "tfmCoreapp10")]
        [DataRow("tfmNetstd20", "netstandard2.0", DisplayName = "tfmNetstd20")]
        [DataRow("tfm100", "netcoreapp100.0", DisplayName = "tfm100")]
        public void TFMBootstrap(string testCaseName, string targetFramework)
        {
            InitializeE2E(testCaseName, createUniqueProject: true, targetFramework: "netcoreapp2.1", sdkVersion: "2.1.801");

            // set bootstrapper dir the same as the test output dir to validate generated files.
            this_TestCaseBootstrapDir = this_TestCaseOutputDir;
            // the boostrapper won't delete the folder if not created by it or with the -v Debug option 
            Directory.CreateDirectory(Path.Combine(this_TestCaseOutputDir, "SvcutilBootstrapper"));

            var uri = $"\"{Path.Combine(g_TestCasesDir, "wsdl/simple.wsdl")}\"";
            var tf = string.IsNullOrEmpty(targetFramework) ? string.Empty : $"-tf {targetFramework}";
            var tr = $"-r \"{{Newtonsoft.Json, *}}\" -bd {this_TestCaseBootstrapDir}";
            var options = $"{uri} {tf} {tr} -nl -tc global -v minimal -d {this_TestCaseName} -n \"*, {this_TestCaseName}_NS\"";

            TestSvcutil(options);
        }

        [TestCategory("Test")]
        //[DataTestMethod]
        [DataRow("XmlSerializer", "-ser XmlSerializer", DisplayName = "XmlSerializer")]
        [DataRow("DataContractSerializer", "-ser DataContractSerializer", DisplayName = "DataContractSerializer")]
        [DataRow("Auto", "-ser Auto", DisplayName = "Auto")]
        [DataRow("MessageContract", "-mc", DisplayName = "MessageContract")]
        [DataRow("EnableDataBinding", "-edb", DisplayName = "EnableDataBinding")]
        [DataRow("Internal", "-i", DisplayName = "Internal")]
        [DataRow("CollectionArray", "-ct System.Array", DisplayName = "CollectionArray")]
        [DataRow("ExcludeType", "-et System.Net.HttpStatusCode", DisplayName = "ExcludeType")]
        [DataRow("NoStdLib", "-nsl", DisplayName = "NoStdLib")]
        [DataRow("wrapped", "-wr", DisplayName = "wrapped")]
        [DataRow("Sync", "-syn", DisplayName = "Sync")]
        [DataRow("None", " ", DisplayName = "None")]
        public void CodeGenOptions(string testCaseName, string optionModifier)
        {
            InitializeE2E(testCaseName);

            var uri = Path.Combine(g_TestCasesDir, "wsdl", "WcfProjectNService", "tempuri.org.wsdl");
            var options = $"{uri} {optionModifier} -nl -tf netcoreapp1.0";

            TestSvcutil(AppendCommonOptions(options));
        }

        [TestCategory("Test")]
        //[DataTestMethod]
        [DataRow("badUrl", "http://$NO_REPLACEMENT$^$Simple.svc", DisplayName = "badUrl")]
        [DataRow("badwsdl", "$testCasesPath$/errorScenarios/badwsdl.wsdl.txt", DisplayName = "badwsdl")]
        [DataRow("collection", "/ct:MyCollectionType", DisplayName = "collection")]
        [DataRow("exclude", "/et:MyExcludeType", DisplayName = "exclude")]
        [DataRow("invalidAddress", "http://invalidaddress -bd $testCaseBootstratDir$", DisplayName = "invalidAddress")]
        [DataRow("noInputs", " ", DisplayName = "NoInputs")]
        [DataRow("ser", "/--serializer MySerializer", DisplayName = "ser")]
        [DataRow("refVersionDoubleQuoted", "-r {Newtonsoft.Json, \"*\"}", DisplayName = "refVersionDoubleQuoted")]
        public void ErrorScenarios(string testCaseName, string options)
        {
            InitializeE2E(testCaseName);

            options = options
                .Replace("$serviceUrl$", g_ServiceUrl)
                .Replace("$testCaseBootstratDir$", $"\"{this_TestCaseBootstrapDir}\"")
                .Replace("$testCasesPath$", g_TestCasesDir);

            TestSvcutil(AppendCommonOptions(options), expectSuccess: false);
        }

        [TestCategory("BVT")]
        //[DataTestMethod]
        [DataRow("FullFramework", DisplayName = "FullFramework")]
        public void FullFramework(string testCaseName)
        {
            InitializeE2E(testCaseName, createUniqueProject: true, targetFramework: "net46");

            var uri = Path.Combine(g_TestCasesDir, "wsdl", "WcfProjectNService", "tempuri.org.wsdl");
            var outDir = Path.Combine(this_TestCaseOutputDir, "ServiceReference");
            var options = $"{uri} -nl -d {outDir} -tf netcoreapp1.0";

            this_TestCaseProject.AddDependency(ProjectDependency.FromAssembly("System.ServiceModel"));
            this_TestCaseProject.SaveAsync(this_TestCaseLogger, CancellationToken.None).Wait();

            TestSvcutil(options, expectSuccess: true);
        }

        [TestCategory("Test")]
        //[DataTestMethod]
        [DataRow("multiDocFullPath", "$wsdlDir$tempuri.org.wsdl", DisplayName = "multiDocFullPath")]
        [DataRow("multiDocAll", "$wsdlDir$*", DisplayName = "multiDocAll")]
        [DataRow("multiDocWsdlWildcard", "$wsdlDir$*.wsdl", DisplayName = "multiDocWsdlWildcard")]
        [DataRow("multiDocWsdlRelXsdWildcard", "$wsdlDir$*.wsdl wsdl/*.xsd", DisplayName = "multiDocWsdlRelXsdWildcard")]
        [DataRow("multiDocFullAndWildcard", "$wsdlDir$tempuri.org.wsdl $wsdlDir$*.xsd", DisplayName = "multiDocFullAndWildcard")]
        [DataRow("multiDocAllRelative", "wsdl/*", DisplayName = "multiDocAllRelative")]
        [DataRow("multiDocWsdlWildcardRelative", "wsdl/*.wsdl", DisplayName = "multiDocWsdlWildcardRelative")]
        [DataRow("multiDocWsdlXsdWildcardRelative", "wsdl/*.wsdl wsdl/*.xsd", DisplayName = "multiDocWsdlXsdWildcardRelative")]
        [DataRow("multiDocRelativeAndWildcard", "wsdl/tempuri.org.wsdl wsdl/*.xsd", DisplayName = "multiDocRelativeAndWildcard")]
        [DataRow("multiDocRelativePath", "wsdl/tempuri.org.wsdl", DisplayName = "multiDocRelativePath")]
        public void MultipleDocuments(string testCaseName, string uri)
        {
            InitializeE2E(testCaseName);

            // copy wsdl files into test project's path to make it easier to pass the params as relative paths.
            var wsdlFile = Path.Combine(this_TestGroupOutputDir, "wsdl", "tempuri.org.wsdl");
            if (!File.Exists(wsdlFile))
            {
                var wsdlDocsSrdDir = Path.Combine(g_TestCasesDir, "wsdl", "WcfProjectNService");
                FileUtil.CopyDirectory(wsdlDocsSrdDir, Path.GetDirectoryName(wsdlFile));
            }

            Assert.IsTrue(File.Exists(wsdlFile), $"{wsdlFile} not initialized!");

            uri = uri.Replace("$wsdlDir$", $"{Path.GetDirectoryName(wsdlFile)}{Path.DirectorySeparatorChar}");

            TestSvcutil(AppendCommonOptions(uri));
        }

        [TestCategory("Test")]
        //[DataTestMethod]
        [DataRow("wildcardNamespace", "-n *,TestNamespace", true, DisplayName = "wildcardNamespace")]
        [DataRow("invalidNamespace", "-n *,Invalid/TestNamespace", false, DisplayName = "invalidNamespace")]
        [DataRow("urlNamespace", "-n http://schemas.datacontract.org/2004/07/WcfProjectNService,TestUrlNamespace", true, DisplayName = "urlNamespace")]
        public void NamespaceParam(string testCaseName, string options, bool expectSuccess)
        {
            InitializeE2E(testCaseName);

            var url = $"{Path.Combine(g_TestCasesDir, "wsdl", "WcfProjectNService", "tempuri.org.wsdl")}";
            var dir = $"-d { this_TestCaseName}";

            TestSvcutil(dir + " " + url + " " + options, expectSuccess);
        }

        [TestCategory("BVT")]
        //[DataTestMethod]
        [DataRow("netstandard1.4", "2.1.801", "-v Normal", DisplayName = "netstd_TF20_SDK20")]
        [DataRow("netcoreapp1.0", "2.1.801", "-nl", DisplayName = "netstd_TF21_SDK21")]
        [DataRow("netcoreapp2.0", "2.1.801", "-edb", DisplayName = "netstd_TF20_SDK20")]
        [DataRow("netcoreapp2.1", "2.1.801", "-elm", DisplayName = "netstd_TF21_SDK21")]
        public void ParamsFiles_SDK_TFM(string targetFramework, string sdkVersion, string extraOptions)
        {
            var testCaseName = $"TF{targetFramework}_SDK{sdkVersion}".Replace(".", "_");
            InitializeE2E(testCaseName, true, targetFramework, sdkVersion);

            var url = $"{Path.Combine(g_TestCasesDir, "wsdl", "simple.wsdl")}";
            var ns = testCaseName.Replace(".", "_") + "_NS";

            // generate params file from options
            var paramsFilePath = Path.Combine(this_TestCaseOutputDir, $"{testCaseName}.params.json");
            var options = new SvcutilOptions();
            options.Inputs.Add(new Uri(url));
            options.References.Add(ProjectDependency.FromPackage("Newtonsoft.Json", "9.0.1"));
            options.OutputDir = new DirectoryInfo(this_TestCaseOutputDir);
            options.BootstrapPath = new DirectoryInfo(this_TestCaseBootstrapDir);
            options.NamespaceMappings.Add(new KeyValuePair<string, string>("*", ns));
            options.NoLogo = true;
            options.NoTelemetry = true;
            options.Project = this_TestCaseProject;
            options.ProviderId = testCaseName;
            options.ToolContext = OperationalContext.Global;
            options.Version = g_SvcutilPkgVersion;
            options.Verbosity = Microsoft.Tools.ServiceModel.Svcutil.Verbosity.Minimal;
            options.Save(paramsFilePath);

            // extra options to test warnings on non-allowed user options.
            extraOptions = $"{extraOptions} -tc global";
            var input = $"{paramsFilePath} {extraOptions}";

            TestSvcutil(input);
        }

        [TestCategory("BVT")]
        //[DataTestMethod]
        [DataRow("TypeReuse20", "netcoreapp2.0", DisplayName = "TypeReuse20")]
        [DataRow("TypeReuse10", "netcoreapp1.0", DisplayName = "TypeReuse10")]
        public void TypeReuse(string testCaseName, string targetFramework)
        {
            InitializeE2E(testCaseName, createUniqueProject: true, targetFramework: targetFramework, sdkVersion: "2.1.801");

            var uri = SetupProjectDependencies();
            var outDir = Path.Combine(this_TestCaseOutputDir, "ServiceReference");
            var options = $"{uri} -nl -v minimal -d {outDir} -n \"*, {testCaseName}_NS\" -bd {this_TestCaseBootstrapDir}";

            TestSvcutil(options, expectSuccess: true);
        }

        private string SetupProjectDependencies()
        {
            var libProjPath = Path.Combine(this_TestGroupOutputDir, "TypesLib", "TypesLib.csproj");
            var binProjPath = Path.Combine(this_TestGroupOutputDir, "BinLib", "BinLib.csproj");
            var assemblyPath = Path.Combine(Path.GetDirectoryName(binProjPath), "bin/debug/netstandard1.3/binlib.dll");

            if (!File.Exists(assemblyPath))
            {
                var typeReuseProjectsPath = Path.Combine(g_TestCasesDir, "TypeReuse");

                FileUtil.CopyDirectory(typeReuseProjectsPath, this_TestGroupOutputDir);
                CreateGlobalJson(this_TestGroupOutputDir, this_TestCaseProject.SdkVersion);

                var binProj = MSBuildProj.FromPathAsync(binProjPath, null, CancellationToken.None).Result;
                var result = binProj.BuildAsync(true, null, CancellationToken.None).Result;
                Assert.IsTrue(result.ExitCode == 0, result.OutputText);
            }

            Assert.IsTrue(File.Exists(binProjPath), $"{nameof(binProjPath)} not initialized!");
            Assert.IsTrue(File.Exists(libProjPath), $"{nameof(libProjPath)} not initialized!");

            this_TestCaseProject.AddDependency(ProjectDependency.FromAssembly(assemblyPath));
            this_TestCaseProject.AddDependency(ProjectDependency.FromProject(libProjPath));
            this_TestCaseProject.SaveAsync(this_TestCaseLogger, CancellationToken.None).Wait();

            var ret = this_TestCaseProject.BuildAsync(true, this_TestCaseLogger, CancellationToken.None).Result;
            Assert.IsTrue(ret.ExitCode == 0, ret.OutputText);

            // keep the boostrapper projects in the outputs to be evaluated against baselines.
            this_TestCaseBootstrapDir = this_TestCaseOutputDir;
            Directory.CreateDirectory(Path.Combine(this_TestCaseBootstrapDir, "SvcutilBootstrapper"));

            var uri = PathHelper.GetRelativePath(Path.Combine(this_TestGroupOutputDir, "TypeReuseSvc.wsdl"), new DirectoryInfo(this_TestCaseOutputDir));
            return uri;
        }

        [TestCategory("BVT")]
        //[DataTestMethod]
        [DataRow("UpdateServiceRefDefault", false, DisplayName = "UpdateServiceRefDefault")]
        [DataRow("UpdateServiceRefBootstrapping", true, DisplayName = "UpdateServiceRefBootstrapping")]
        public void UpdateServiceRefBasic(string referenceFolderName, bool bootstrapping)
        {
            var testCaseName = referenceFolderName;
            InitializeE2E(testCaseName, createUniqueProject: true, targetFramework: "netcoreapp2.1", sdkVersion: "2.1.801");

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

        [TestCategory("Test")]
        //[DataTestMethod]
        [DataRow("UpdateServiceRefOptionsDefault", 1, null, true, DisplayName = "UpdateServiceRefOptionsDefault")]
        [DataRow("UpdateServiceRefOptions Folder With Spaces", 1, null, true, DisplayName = "UpdateServiceRefOptions Folder With Spaces")]
        [DataRow("UpdateServiceRefOptions Folder With Spaces Full", 1, "\"UpdateServiceRefOptions Folder With Spaces Full\"", true, DisplayName = "UpdateServiceRefOptions Folder With Spaces Full")]
        [DataRow("UpdateServiceRefOptionsRef", 1, "UpdateServiceRefOptionsRef", true, DisplayName = "UpdateServiceRefOptionsRef")]
        [DataRow("UpdateServiceRefOptionsRef2", 2, "UpdateServiceRefOptionsRef2", true, DisplayName = "UpdateServiceRefOptionsRef2")]
        [DataRow("UpdateServiceRefOptions/Level1/Level2/UpdateRefLevels", 1, null, true, DisplayName = "UpdateRefLevels")]
        [DataRow("UpdateServiceRefOptions/Level1/Level2/UpdateRefLevelsFull", 1, "UpdateServiceRefOptions/Level1/Level2/UpdateRefLevelsFull", true, DisplayName = "UpdateRefLevelsFull")]
        [DataRow("UpdateServiceRefOptionsFilePath", 1, "UpdateServiceRefOptionsFilePath/dotnet-svcutil.params.json", true, DisplayName = "UpdateServiceRefOptionsFilePath")]
        [DataRow("UpdateServiceRefOptionsFullPath", 1, "$testCaseOutputDir$/UpdateServiceRefOptionsFullPath/dotnet-svcutil.params.json", true, DisplayName = "UpdateServiceRefOptionsFullPath")]
        [DataRow("UpdateServiceRefOptionsExtraOptions", 1, "-nl", true, DisplayName = "UpdateServiceRefOptionsRefExtraOptions")]
        [DataRow("UpdateServiceRefOptionsExtraOptionsWarn", 1, "-edb -nb -elm", true, DisplayName = "UpdateServiceRefOptionsExtraOptionsWarn")]
        [DataRow("UpdateServiceRefOptionsDefaultOnMultipleRefs", 3, null, false, DisplayName = "UpdateServiceRefOptionsDefaultOnMultipleRefs")]
        [DataRow("UpdateServiceRefOptionsOnMissingRef", 1, "Inexistent", false, DisplayName = "UpdateServiceRefOptionsOnMissingRef")]
        public void UpdateServiceRefOptions(string referenceFolderName, int refCount, string cmdOptions, bool expectSuccess)
        {
            var testCaseName = referenceFolderName.Replace(" ", "_").Split('/').Last();
            InitializeE2E(testCaseName, createUniqueProject: true, targetFramework: "netcoreapp2.1", sdkVersion: "2.1.801");

            cmdOptions = cmdOptions?.Replace("$testCaseOutputDir$", this_TestCaseOutputDir);
            var paramsFile = SetupServiceReferenceFolder("dotnet-svcutil.params.json", referenceFolderName, refCount, addNamespace: true);

            // disable type reuse (bootstrapping) to speed up test.
            var udpateOptions = UpdateOptions.FromFile(paramsFile);
            udpateOptions.TypeReuseMode = TypeReuseMode.None;
            udpateOptions.Save(paramsFile);

            var options = $"-u {cmdOptions} -v minimal";
            TestSvcutil(options, expectSuccess);
        }

        [TestCategory("Test")]
        //[DataTestMethod]
        [DataRow("Connected Services/CSServiceReference", false, DisplayName = "CSServiceReference")]
        [DataRow("Connected Services/CSServiceReferenceRoundtrip", true, DisplayName = "CSServiceReferenceRoundtrip")]
        public void UpdateServiceRefWCFCS(string referenceFolderName, bool addNamespace)
        {
            var testCaseName = referenceFolderName.Replace(" ", "_").Split('/').Last();
            InitializeE2E(testCaseName, createUniqueProject: true, targetFramework: "netcoreapp2.1", sdkVersion: "2.1.801");

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

        public string SetupServiceReferenceFolder(string paramsFileName, string referenceFolderName, int refCount = 1, bool addNamespace = true)
        {
            var srcParamsFilePath = Path.Combine(g_TestCasesDir, "updateServiceReference", paramsFileName);
            Assert.IsTrue(File.Exists(srcParamsFilePath), $"{nameof(srcParamsFilePath)} not initialized!");

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

        [TestCategory("Test")]
        //[DataTestMethod]
        [DataRow("BasicAuth.svc", false, DisplayName = "BasicAuth")]
        [DataRow("BasicHttps.svc", true, DisplayName = "BasicHttps")]
        [DataRow("BasicHttp.svc", true, DisplayName = "UpdatBasicHttpeOnMissingRef")]
        [DataRow("BasicHttp_4_4_0.svc", true, DisplayName = "BasicHttp_4_4_0")]
        [DataRow("BasicHttpSoap.svc", true, DisplayName = "BasicHttpSoap")]
        [DataRow("BasicHttpRpcEncSingleNs.svc", true, DisplayName = "BasicHttpRpcEncSingleNs")]
        [DataRow("BasicHttpRpcLitSingleNs.svc", true, DisplayName = "BasicHttpRpcLitSingleNs")]
        [DataRow("BasicHttpDocLitSingleNs.svc", true, DisplayName = "BasicHttpDocLitSingleNs")]
        [DataRow("BasicHttpRpcEncDualNs.svc", true, DisplayName = "BasicHttpRpcEncDualNs")]
        [DataRow("BasicHttpRpcLitDualNs.svc", true, DisplayName = "BasicHttpRpcLitDualNs")]
        [DataRow("BasicHttpDocLitDualNs.svc", true, DisplayName = "BasicHttpDocLitDualNs")]

        public void WcfRuntimeBasicSvcs(string serviceName, bool expectSuccess)
        {
            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        // TODO: enable WCF Runtime services: some of these tests take forever, we need to investigate
        // and determine what services to use as part of the tests or whether these tests should be 
        // run as a separate suite like nightly build. Use the Basic and NetHttp tests as guidance.

        [TestCategory("Test")]
        ////[DataTestMethod]  TODO: validate and enable
        [DataRow("Duplex.svc", true, DisplayName = "Duplex")]
        [DataRow("DuplexCallback.svc", true, DisplayName = "DuplexCallback")]
        [DataRow("DuplexChannelCallbackReturn.svc", true, DisplayName = "DuplexChannelCallbackReturn")]
        [DataRow("DuplexCallbackDataContractComplexType.svc", true, DisplayName = "DuplexCallbackDataContractComplexType")]
        [DataRow("DuplexCallbackXmlComplexType.svc", true, DisplayName = "DuplexCallbackXmlComplexType")]
        [DataRow("DuplexCallbackTcpCertificateCredential.svc", true, DisplayName = "DuplexCallbackTcpCertificateCredential")]
        public void WcfRuntimeDuplexSvcs(string serviceName, bool expectSuccess)
        {
            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        [TestCategory("Test")]
        ////[DataTestMethod]  TODO: validate and enable
        [DataRow("HttpBinary.svc", true, DisplayName = "HttpBinary")]
        [DataRow("HttpDigestNoDomain.svc", true, DisplayName = "HttpDigestNoDomain")]
        [DataRow("ClientCertificateAccepted/HttpsClientCertificate.svc", true, DisplayName = "HttpsClientCertificate")]
        [DataRow("DigestAuthentication/HttpsDigest.svc", true, DisplayName = "HttpsDigest")]
        [DataRow("WindowAuthenticationNtlm/HttpsNtlm.svc", true, DisplayName = "HttpsNtlm")]
        [DataRow("HttpSoap11.svc", true, DisplayName = "HttpSoap11")]
        [DataRow("HttpsSoap11.svc", true, DisplayName = "HttpsSoap11")]
        [DataRow("HttpSoap11WSA2004.svc", true, DisplayName = "HttpSoap11WSA2004")]
        [DataRow("HttpsSoap12.svc", true, DisplayName = "HttpsSoap12")]
        [DataRow("HttpSoap12.svc", true, DisplayName = "HttpSoap12")]
        [DataRow("HttpSoap12WSA2004.svc", true, DisplayName = "HttpSoap12WSA2004")]
        [DataRow("WindowAuthenticationNegotiate/HttpsWindows.svc", true, DisplayName = "HttpsWindows")]
        [DataRow("HttpsCertValModePeerTrust.svc", true, DisplayName = "HttpsCertValModePeerTrust")]
        [DataRow("HttpsCertValModeChainTrust.svc", true, DisplayName = "HttpsCertValModeChainTrust")]
        public void WcfRuntimeHttpSvcs(string serviceName, bool expectSuccess)
        {
            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        [TestCategory("Test")]
        //[DataTestMethod]
        [DataRow("NetHttp.svc", true, DisplayName = "NetHttp")]
        [DataRow("NetHttpWebSockets.svc", true, DisplayName = "NetHttpWebSockets")]
        [DataRow("NetHttps.svc", true, DisplayName = "NetHttps")]
        [DataRow("NetHttpsWebSockets.svc", true, DisplayName = "NetHttpsWebSockets")]
        public void WcfRuntimeNetHttpSvcs(string serviceName, bool expectSuccess)
        {
            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        [TestCategory("Test")]
        ////[DataTestMethod]  TODO: validate and enable
        [DataRow("ServiceContractAsyncIntOut.svc", true, DisplayName = "ServiceContractAsyncIntOut")]
        [DataRow("ServiceContractAsyncUniqueTypeOut.svc", true, DisplayName = "ServiceContractAsyncUniqueTypeOut")]
        [DataRow("ServiceContractAsyncIntRef.svc", true, DisplayName = "ServiceContractAsyncIntRef")]
        [DataRow("ServiceContractAsyncUniqueTypeRef.svc", true, DisplayName = "ServiceContractAsyncUniqueTypeRef")]
        [DataRow("ServiceContractSyncUniqueTypeOut.svc", true, DisplayName = "ServiceContractSyncUniqueTypeOut")]
        [DataRow("ServiceContractSyncUniqueTypeRef.svc", true, DisplayName = "ServiceContractSyncUniqueTypeRef")]
        public void WcfRuntimeServiceContractSvcs(string serviceName, bool expectSuccess)
        {
            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        [TestCategory(nameof(WcfRuntimeTcpSvcs))]
        [TestCategory("Test")]
        ////[DataTestMethod]  TODO: validate and enable
        [DataRow("TcpCertificateWithServerAltName.svc", true, DisplayName = "TcpCertificateWithServerAltName")]
        [DataRow("TcpCertificateWithSubjectCanonicalNameDomainName.svc", true, DisplayName = "TcpCertificateWithSubjectCanonicalNameDomainName")]
        [DataRow("TcpCertificateWithSubjectCanonicalNameFqdn.svc", true, DisplayName = "TcpCertificateWithSubjectCanonicalNameFqdn")]
        [DataRow("TcpCertificateWithSubjectCanonicalNameLocalhost.svc", true, DisplayName = "TcpCertificateWithSubjectCanonicalNameLocalhost")]
        [DataRow("TcpExpiredServerCert.svc", true, DisplayName = "TcpExpiredServerCert")]
        [DataRow("TcpDefault.svc", true, DisplayName = "TcpDefault")]
        [DataRow("TcpNoSecurity.svc", true, DisplayName = "TcpNoSecurity")]
        [DataRow("TcpNoSecurityText.svc", true, DisplayName = "TcpNoSecurityText")]
        [DataRow("TcpInvalidEkuServerCert.svc", true, DisplayName = "TcpInvalidEkuServerCert")]
        [DataRow("TcpRevokedServerCert.svc", true, DisplayName = "TcpRevokedServerCert")]
        [DataRow("TcpStreamedNoSecurity.svc", true, DisplayName = "TcpStreamedNoSecurity")]
        [DataRow("TcpTransportSecuritySslCustomCertValidation.svc", true, DisplayName = "TcpTransportSecuritySslCustomCertValidation")]
        [DataRow("WindowAuthenticationNegotiate/TcpTransportSecurityStreamed.svc", true, DisplayName = "TcpTransportSecurityStreamed")]
        [DataRow("TcpTransportSecurityWithSsl.svc", true, DisplayName = "TcpTransportSecurityWithSsl")]
        [DataRow("TcpTransportSecuritySslClientCredentialTypeCertificate.svc", true, DisplayName = "TcpTransportSecuritySslClientCredentialTypeCertificate")]
        [DataRow("TcpVerifyDNS.svc", true, DisplayName = "TcpVerifyDNS")]
        [DataRow("NetTcpCertValModePeerTrust.svc", true, DisplayName = "NetTcpCertValModePeerTrust")]
        public void WcfRuntimeTcpSvcs(string serviceName, bool expectSuccess)
        {
            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        [TestCategory("Test")]
        ////[DataTestMethod]  TODO: validate and enable
        [DataRow("SessionTestsDefaultService.svc", true, DisplayName = "SessionTestsDefaultService")]
        [DataRow("SessionTestsShortTimeoutService.svc", true, DisplayName = "SessionTestsShortTimeoutService")]
        [DataRow("SessionTestsDuplexService.svc", true, DisplayName = "SessionTestsDuplexService")]
        public void WcfRuntimeSessionSvcs(string serviceName, bool expectSuccess)
        {
            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        [TestCategory("Test")]
        ////[DataTestMethod]  TODO: validate and enable
        [DataRow("DuplexWebSocket.svc", true, DisplayName = "DuplexWebSocket")]
        [DataRow("WebSocketTransport.svc", true, DisplayName = "WebSocketTransport")]
        [DataRow("WebSocketHttpDuplexBinaryStreamed.svc", true, DisplayName = "WebSocketHttpDuplexBinaryStreamed")]
        [DataRow("WebSocketHttpRequestReplyBinaryStreamed.svc", true, DisplayName = "WebSocketHttpRequestReplyBinaryStreamed")]
        [DataRow("WebSocketHttpsDuplexBinaryStreamed.svc", true, DisplayName = "WebSocketHttpsDuplexBinaryStreamed")]
        [DataRow("WebSocketHttpsDuplexTextStreamed.svc", true, DisplayName = "WebSocketHttpsDuplexTextStreamed")]
        [DataRow("WebSocketHttpRequestReplyTextStreamed.svc", true, DisplayName = "WebSocketHttpRequestReplyTextStreamed")]
        [DataRow("WebSocketHttpDuplexTextStreamed.svc", true, DisplayName = "WebSocketHttpDuplexTextStreamed")]
        [DataRow("WebSocketHttpRequestReplyTextBuffered.svc", true, DisplayName = "WebSocketHttpRequestReplyTextBuffered")]
        [DataRow("WebSocketHttpRequestReplyBinaryBuffered.svc", true, DisplayName = "WebSocketHttpRequestReplyBinaryBuffered")]
        [DataRow("WebSocketHttpDuplexTextBuffered.svc", true, DisplayName = "WebSocketHttpDuplexTextBuffered")]
        [DataRow("WebSocketHttpDuplexBinaryBuffered.svc", true, DisplayName = "WebSocketHttpDuplexBinaryBuffered")]
        [DataRow("WebSocketHttpsRequestReplyBinaryBuffered.svc", true, DisplayName = "WebSocketHttpsRequestReplyBinaryBuffered")]
        [DataRow("WebSocketHttpsRequestReplyTextBuffered.svc", true, DisplayName = "WebSocketHttpsRequestReplyTextBuffered")]
        [DataRow("WebSocketHttpsDuplexBinaryBuffered.svc", true, DisplayName = "WebSocketHttpsDuplexBinaryBuffered")]
        [DataRow("WebSocketHttpsDuplexTextBuffered.svc", true, DisplayName = "WebSocketHttpsDuplexTextBuffered")]
        [DataRow("WebSocketHttpVerifyWebSocketsUsed.svc", true, DisplayName = "WebSocketHttpVerifyWebSocketsUsed")]
        public void WcfRuntimeWebSocketsvcs(string serviceName, bool expectSuccess)
        {
            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        [TestCategory("Test")]
        ////[DataTestMethod]  TODO: validate and enable
        [DataRow("ChannelExtensibility.svc", true, DisplayName = "ChannelExtensibility")]
        [DataRow("CustomTextEncoderBuffered.svc", true, DisplayName = "CustomTextEncoderBuffered")]
        [DataRow("CustomTextEncoderStreamed.svc", true, DisplayName = "CustomTextEncoderStreamed")]
        [DataRow("DefaultCustomHttp.svc", true, DisplayName = "DefaultCustomHttp")]
        [DataRow("DataContractResolver.svc", true, DisplayName = "ServiceContractSyncUniqueTypeRef")]
        [DataRow("UnderstoodHeaders.svc", true, DisplayName = "ServiceContractSyncUniqueTypeRef")]
        [DataRow("XmlSFAttribute.svc", true, DisplayName = "ServiceContractSyncUniqueTypeRef")]
        public void WcfRuntimeMiscSvcs(string serviceName, bool expectSuccess)
        {
            WcfRuntimeSvcs(serviceName, expectSuccess);
        }

        public void WcfRuntimeSvcs(string serviceName, bool expectSuccess)
        {
            var testCaseName = serviceName.Replace(".svc", "");
            InitializeE2E(testCaseName);

            var uri = $"http://wcfcoresrv5.cloudapp.net/wcftestservice1/{serviceName}";

            TestSvcutil(AppendCommonOptions(uri), expectSuccess);
        }

        // TODO:
        // this is not an actual test but it is a way to keep the repo clean of dead-baselines.
        // in order to reliably run this test, it must run at the end, given that this
        // is a partial class it is hard to enforce the order.
        // need to find a way to keep this test running reliably.
        [TestCategory("Test")]
        ////[DataTestMethod]
        public void CheckBaslines()
        {
            this_TestCaseName = nameof(CheckBaslines);

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
                var assertmsg = GenerateBaselineDeleteScript(baselinesWithNoOutputs);
                Assert.Fail(assertmsg);
            }
        }
    }
}
