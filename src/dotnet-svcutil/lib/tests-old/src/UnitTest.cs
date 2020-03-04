// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using Microsoft.Tools.ServiceModel.Svcutil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SvcutilTest
{
    public partial class E2ETest
    {
        // TODO: Add more unit-test cases!

        private void UnitTestSvcutil(string options, bool expectSuccess = true)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(this_TestCaseName), $"{nameof(this_TestCaseName)} not initialized!");
            Assert.IsFalse(options == null, $"{nameof(options)} not initialized!");

            // run in the context of the test-group common project
            using (var cds = new CurrentDirectorySetter(this_TestCaseProject.DirectoryPath))
            {
                Directory.SetCurrentDirectory(this_TestCaseProject.DirectoryPath);

                var exitCode = 0;
                var outputText = string.Empty;

                using (var stream = new MemoryStream())
                {
                    var writer = new System.IO.StreamWriter(stream) { AutoFlush = true };
                    Console.SetOut(writer);
                    var args = options.Split(' ');

                    exitCode = Microsoft.Tools.ServiceModel.Svcutil.Tool.MainAsync(args, this_TestCaseLogger, CancellationToken.None).Result;

                    stream.Position = 0;
                    outputText = new StreamReader(stream).ReadToEnd();
                }

                ValidateTest(options, this_TestCaseProject.DirectoryPath, exitCode, outputText, expectSuccess);
            }
        }

        [TestCategory("UnitTest")]
        //[DataTestMethod]
        [DataRow(null, DisplayName = "verbositDefault")]
        [DataRow("minimal", DisplayName = "verbositMinimal")]
        [DataRow("normal", DisplayName = "verbositNormal")]
        [DataRow("verbose", DisplayName = "verbositVerbose")]
        [DataRow("debug", DisplayName = "verbositDebug")]
        [DataRow("silent", DisplayName = "verbositInvalid")]
        [DataRow("invalid", DisplayName = "verbositInvalid")]
        public void Verbosity(string verbosity)
        {
            var testCaseName = "Verbosity_" + (verbosity ?? "Default");
            InitializeUnitTest(testCaseName, createProject: false);

            // These tests all are expected to fail, we rely on the output log file to check the verbosity.
            var uri = Path.Combine(g_TestCasesDir, "wsdl", "brokenWsdl.wsdl");
            var verbosityOption = string.IsNullOrWhiteSpace(verbosity) ? string.Empty : $" -v {verbosity}";

            // use options that would make the tool show the incorrect tool operational context warning!
            var options = $"{uri} {verbosityOption} -tc project -d {verbosity} -ntr";

            UnitTestSvcutil(options, expectSuccess: false);
        }

        [TestCategory("UnitTest")]
        //[DataTestMethod]
        [DataRow("wsdlQuery", "Service1.svc?wsdl", DisplayName = "wsdlQuery")]
        [DataRow("singleWsdlQuery", "Service1.svc?singlewsdl", DisplayName = "singleWsdlQuery")]
        [DataRow("mexParam", "Service1.svc/mex", DisplayName = "mexParam")]
        [DataRow("noQuery", "Service1.svc", DisplayName = "noQuery")]
        public void MetadataQuery(string testCaseName, string uriPart)
        {
            InitializeUnitTest(testCaseName);

            var uri = $"http://{g_ServiceUrl}/WcfForSvcUtil/{uriPart}";
            var options = $"{uri} -nl";

            UnitTestSvcutil(AppendCommonOptions(options));
        }

        [TestCategory("UnitTest")]
        //[DataTestMethod]
        [DataRow("metadataEpr", DisplayName = "metadataEpr")]
        public void MetadataEpr(string testCaseName)
        {
            InitializeUnitTest(testCaseName);

            var uri = Path.Combine(g_TestCasesDir, "metadataEpr", "epr", "http___WcfProjectNServer_WcfProjectNService_WcfProjectNService.svc.xml");
            var options = $"{uri} -nl";

            UnitTestSvcutil(AppendCommonOptions(options));
        }

        [TestCategory("UnitTest")]
        //[DataTestMethod]
        [DataRow("array", "System.Array", DisplayName = "array")]
        [DataRow("dictionary", "System.Collections.Generic.Dictionary`2,System.Collections", DisplayName = "dictionary")]
        [DataRow("arrayList", "System.Collections.ArrayList,System.Collections.NonGeneric", DisplayName = "arrayList")]
        [DataRow("sortedList", "System.Collections.Generic.SortedList`2,System.Collections", DisplayName = "sortedList")]
        [DataRow("linkedList", "System.Collections.Generic.LinkedList`1,System.Collections", DisplayName = "linkedList")]
        [DataRow("sortedDictionary", "System.Collections.Generic.SortedDictionary`2,System.Collections", DisplayName = "sortedDictionary")]
        [DataRow("list", "System.Collections.Generic.List`1,System.Collections", DisplayName = "list")]
        [DataRow("hashTable", "System.Collections.Hashtable,System.Collections.NonGeneric", DisplayName = "hashTable")]
        [DataRow("sortedListNonGeneric", "System.Collections.SortedList,System.Collections.NonGeneric", DisplayName = "sortedListNonGeneric")]
        [DataRow("collection", "System.Collections.ObjectModel.Collection`1,System.Runtime", DisplayName = "collection")]
        [DataRow("observableCollection", "System.Collections.ObjectModel.ObservableCollection`1,System.ObjectModel", DisplayName = "observableCollection")]
        [DataRow("hybridDictionary", "System.Collections.Specialized.HybridDictionary,System.Collections.Specialized", DisplayName = "hybridDictionary")]
        [DataRow("listDictionary", "System.Collections.Specialized.ListDictionary,System.Collections.Specialized", DisplayName = "listDictionary")]
        [DataRow("orderedDictionary", "System.Collections.Specialized.OrderedDictionary,System.Collections.Specialized", DisplayName = "orderedDictionary")]
        public void Collections(string testCaseName, string collection)
        {
            InitializeUnitTest(testCaseName);

            var uri = Path.Combine(g_TestCasesDir, "wsdl", "WcfProjectNService", "tempuri.org.wsdl");
            var options = $"{uri} -nl -ct \"{collection}\"";

            UnitTestSvcutil(AppendCommonOptions(options));
        }

        [TestCategory("UnitTest")]
        //[DataTestMethod]
        [DataRow("tfmDefault", null, true, DisplayName = "tfmDefault")]
        [DataRow("tfm20", "netcoreapp2.0", true, DisplayName = "tfm20")]
        [DataRow("tfm100", "netcoreapp100.0", true, DisplayName = "tfm100")]
        [DataRow("tfm45", "net45", true, DisplayName = "tfm45")]
        [DataRow("net90", "net90", true, DisplayName = "tfm90")]
        [DataRow("badTFM", "badTFM", false, DisplayName = "badTFM")]
        public void TFM(string testCaseName, string targetFramework, bool expectSuccess)
        {
            InitializeUnitTest(testCaseName, createProject: true, sdkVersion: "2.1.801");

            var uri = Path.Combine(g_TestCasesDir, "wsdl/simple.wsdl");
            var tf = targetFramework == null ? string.Empty : $"-tf {targetFramework}";
            var options = $"{uri} {tf}";
            UnitTestSvcutil(AppendCommonOptions(options), expectSuccess);
        }

        [TestCategory("UnitTest")]
        //[DataTestMethod]
        [DataRow("badParam", "http://www.myhost.com/MyService.svc -badparam", DisplayName = "badParam")]
        [DataRow("xd", "in.wsdl -xd", DisplayName = "xd")]
        [DataRow("emptyD", "in.wsdl -d ", DisplayName = "emptyD")]
        [DataRow("emptyR", "in.wsdl -r", DisplayName = "emptyR")]
        [DataRow("emptyCt", "in.wsdl -ct", DisplayName = "emptyCt")]
        [DataRow("emptyEt", "in.wsdl -et", DisplayName = "emptyEt")]
        [DataRow("emptySer", "in.wsdl --serializer", DisplayName = "emptySer")]
        [DataRow("dir", @"-d:D:\Temp", DisplayName = "dir")]
        [DataRow("out", "-help:MyRef.cs", DisplayName = "out")]
        [DataRow("ref", "--r:MyReference", DisplayName = "ref")]
        [DataRow("singleAndFlag", "in.wsdl -d -nl", DisplayName = "singleAndFlag")]
        [DataRow("invalidAndFlag", "in.wsdl -invalid -nl", DisplayName = "singleAndFlag")]
        [DataRow("invalidAndInput", "in.wsdl -invalid in2.wsdl", DisplayName = "invalidAndInput")]
        public void CommandParserInvalid(string testCaseName, string options)
        {
            options = $"{options} -ntr"; // prevent project reference resolution.
            CommandOptions(testCaseName, options, serializeUpdateOptions: false, expectSuccess: false);
        }

        [TestCategory("UnitTest")]
        //[DataTestMethod]
        [DataRow("basicOptions1", "$testCasesDir$/wsdl/simple.wsdl -tc global", DisplayName = "basicOptions1")]
        [DataRow("basicOptions2", "$testCasesDir$/wsdl/simple.wsdl -pf $projectPath$ -nb ", DisplayName = "basicOptions2")]
        [DataRow("basicOptions3", "$testCasesDir$/wsdl/simple.wsdl -pf $projectPath$ -d OutputDir -o Reference.cs", DisplayName = "basicOptions3")]
        [DataRow("basicOptions4", "$testCasesDir$/wsdl/simple.wsdl -pf $projectPath$ -d OutputDir -o Reference.cs -bd $bootstrapDir$", DisplayName = "basicOptions4")]
        [DataRow("basicOptions5", "$testCasesDir$/wsdl/WcfProjectNService/* -pf $projectPath$ -d OutputDir -o Reference.cs -bd $bootstrapDir$", DisplayName = "basicOptions5")]
        public void CommandOptionsBasic(string testCaseName, string options)
        {
            options = $"{options} -ntr"; // prevent project reference resolution.
            CommandOptions(testCaseName, options, serializeUpdateOptions: false, expectSuccess: true);
        }

        [TestCategory("UnitTest")]
        //[DataTestMethod]
        [DataRow("fwdWild", "../wsdl/* -o ServiceReference/Reference.cs", DisplayName = "fwdWild")]
        [DataRow("fwdWildExt", "../wsdl/*.wsdl -d ServiceReference", DisplayName = "fwdWildExt")]
        [DataRow("bckWild", "..\\wsdl\\* -d OutputDir/ServiceReference -o Reference.cs", DisplayName = "bckWild")]
        [DataRow("backFull", "..\\wsdl\\simple.wsdl -d ..\\backFull\\OutputDir/ServiceReference -o OutputDir\\Reference.cs", DisplayName = "backFull")]
        [DataRow("fwdMultiWild", "../wsdl/WcfProjectNService/* -d ..\\..\\CommandOptionsRelativePaths\\OutputDir", DisplayName = "fwdMultiWild")]
        [DataRow("fwdMultiWildExt", "../wsdl/WcfProjectNService/*.wsdl -d ../../CommandOptionsRelativePaths/OutputDir", DisplayName = "fwdMultiWildExt")]
        public void CommandOptionsFilePaths(string testCaseName, string options)
        {
            var wsdlFilesDstDir = Path.Combine(this_TestGroupOutputDir, "wsdl");
            if (!Directory.Exists(wsdlFilesDstDir))
            {
                var wsdlFilesSrcDir = Path.Combine(g_TestCasesDir, "wsdl", "WcfProjectNService");
                FileUtil.CopyDirectory(wsdlFilesSrcDir.Replace("$testCasesDir$", g_TestCasesDir), Path.Combine(wsdlFilesDstDir, "WcfProjectNService"), overwrite: true);

                Directory.CreateDirectory(Path.Combine(this_TestGroupOutputDir, "wsdl"));
                File.Copy(Path.Combine(g_TestCasesDir, "wsdl/simple.wsdl"), Path.Combine(this_TestGroupOutputDir, "wsdl/simple.wsdl"));
            }

            options = $"-tc global -pf $projectPath$ -ntr {options}";

            CommandOptions(testCaseName, options, serializeUpdateOptions: true, expectSuccess: true);
        }


        public void CommandOptions(string testCaseName, string options, bool serializeUpdateOptions, bool expectSuccess)
        {
            InitializeUnitTest(testCaseName);

            using (var cds = new CurrentDirectorySetter(this_TestCaseOutputDir))
            {
                var args = options
                    .Replace("$projectPath$", this_TestCaseProject.FullPath)
                    .Replace("$testCasesDir$", g_TestCasesDir)
                    .Replace("$testOutputDir$", this_TestGroupOutputDir)
                    .Replace("$testCaseOutputDir$", this_TestCaseOutputDir)
                    .Replace("$testCasesDir$", g_TestCasesDir)
                    .Replace("$bootstrapDir$", this_TestCaseBootstrapDir)
                    .Split(' ');

                var cmdOptions = CommandParser.ParseCommand(args);
                cmdOptions.ProcessBasicOptionsAsync(this_TestCaseLogger, CancellationToken.None).Wait();
                cmdOptions.ResolveAsync(CancellationToken.None).Wait();

                var cmderrors = cmdOptions.Errors.Count() == 0 ? string.Empty :
                        Environment.NewLine + cmdOptions.Errors.Select(e => e.Message).Aggregate((errors, e) => $"{errors}{Environment.NewLine}{e}");
                var cmdwarnings = cmdOptions.Warnings.Count() == 0 ? string.Empty :
                        Environment.NewLine + cmdOptions.Warnings.Aggregate((warnings, w) => $"{warnings}{Environment.NewLine}{w}");
                var optionsString = cmdOptions.ToString();

                var log = $"{Environment.NewLine}Input options:{Environment.NewLine}{options}{Environment.NewLine}" +
                          $"{Environment.NewLine}Output JSON:{Environment.NewLine}{cmdOptions.Json}{Environment.NewLine}" +
                          $"{Environment.NewLine}Output options:{Environment.NewLine}{optionsString}{Environment.NewLine}" +
                          $"{Environment.NewLine}Parsing warnings:{cmdwarnings}{Environment.NewLine}" +
                          $"{Environment.NewLine}Parsing errors:{cmderrors}{Environment.NewLine}";

                if (serializeUpdateOptions)
                {
                    var updateOptions = cmdOptions.CloneAs<UpdateOptions>();
                    updateOptions.MakePathsRelativeTo(new DirectoryInfo(this_TestCaseOutputDir));
                    log += $"{Environment.NewLine}{cmdOptions.GetType().Name} as {updateOptions.GetType().Name}:{Environment.NewLine}{updateOptions.Json}";
                }

                File.WriteAllText(this_TestCaseLogFile, this_FixupUtil.ReplaceText(log));
                WriteLog(log, options);

                Assert.IsFalse(cmdOptions.Errors.Count() == 0 ^ expectSuccess, cmderrors);
                Assert.IsTrue(ValidateOutputs(options, out var failureMessage), failureMessage);
            }
        }

        [TestCategory("UnitTest")]
        //[DataTestMethod]
        [DataRow("safeTelemetry", "-nl -v verbose -cn ES -ntr -tf netcoreapp2.1", DisplayName = "safeTelemetry")]
        [DataRow("gdprSensitive", "http://www.myhost.com/myGdprensitiveUrl -d myGdrpSensitiveOutputDir -o myGdprSensitiveFileName -n \"*,myGdprSenstiveNS\" -bd myTempDir", DisplayName = "gdprSensitive")]
        [DataRow("gdprFiltered", "-r assemblyRef -r {packageRef,*} -ct mycollectionType -et myexludetype", DisplayName = "gdprFiltered")]
        public void CommandOptionsTelemetryString(string testCaseName, string options)
        {
            InitializeUnitTest(testCaseName, createProject: false, sdkVersion: "2.1.801");
            options = $"{options} -tc Infrastructure";

            var args = options.Split(' ');
            var cmdOptions = CommandParser.ParseCommand(args);

            cmdOptions.ProcessBasicOptionsAsync(this_TestCaseLogger, CancellationToken.None).Wait();
            cmdOptions.ResolveAsync(CancellationToken.None).Wait();

            var cmderrors = cmdOptions.Errors.Count() == 0 ? string.Empty :
                    Environment.NewLine + cmdOptions.Errors.Select(e => e.Message).Aggregate((errors, e) => $"{errors}{Environment.NewLine}{e}");
            var cmdwarnings = cmdOptions.Warnings.Count() == 0 ? string.Empty :
                    Environment.NewLine + cmdOptions.Warnings.Aggregate((warnings, w) => $"{warnings}{Environment.NewLine}{w}");
            var optionsString = cmdOptions.ToTelemetryString();

            var log = $"{Environment.NewLine}Input options:{Environment.NewLine}{options}{Environment.NewLine}" +
                      $"{Environment.NewLine}Output options:{Environment.NewLine}{optionsString}{Environment.NewLine}" +
                      $"{Environment.NewLine}{cmdwarnings}" +
                      $"{Environment.NewLine}{cmderrors}";

            File.WriteAllText(this_TestCaseLogFile, this_FixupUtil.ReplaceText(log));
            WriteLog(log, options);

            Assert.IsTrue(ValidateOutputs(options, out var failureMessage), failureMessage);
        }

        [TestCategory("UnitTest")]
        //[DataTestMethod]
        [DataRow("SvcutilOptions", "dotnet-svcutil.svcutil.json", DisplayName = "SvcutilOptions")]
        [DataRow("UpdateOptions", "dotnet-svcutil.update.json", DisplayName = "UpdateOptions")]
        public void OptionsRoundtrip(string optionsType, string jsonFile)
        {
            var testCaseName = optionsType;
            InitializeUnitTest(testCaseName);

            var errorMessage = string.Empty;
            var jsonFilesRoot = Path.Combine(g_TestCasesDir, "options", "JSON");

            var jsonFileSrcPath = Path.Combine(jsonFilesRoot, jsonFile);
            var jsonFileDstPath = Path.Combine(this_TestCaseOutputDir, Path.GetFileName(jsonFileSrcPath));
            File.Copy(jsonFileSrcPath, jsonFileDstPath);
            File.WriteAllText(jsonFileDstPath, File.ReadAllText(jsonFileDstPath).Replace("$testCaseProject$", this_TestCaseProject.FullPath.Replace("\\", "/")));

            ApplicationOptions options = optionsType == "SvcutilOptions" ?
                SvcutilOptions.FromFile(jsonFileDstPath) : UpdateOptions.FromFile(jsonFileDstPath);

            WriteLog("", options.ToString());

            Assert.IsTrue(ValidateJson(options, jsonFileSrcPath, out errorMessage), errorMessage);

            ValidateTest(options.ToString(), this_TestCaseProject.DirectoryPath, 0, string.Empty, true);
        }

        private bool ValidateJson(ApplicationOptions options, string jsonFileSrcPath, out string errorMessage)
        {
            errorMessage = string.Empty;
            var outputDir = Path.GetTempPath();
            var jsonText = File.ReadAllText(jsonFileSrcPath);
            var optionsJson = options.Json.Replace(this_TestCaseProject.FullPath.Replace("\\", "/"), "$testCaseProject$");

            if (optionsJson.Trim() != jsonText.Trim())
            {
                var outJsonFile = Path.Combine(outputDir, Path.GetFileName(jsonFileSrcPath));
                File.WriteAllText(outJsonFile, options.Json);
                errorMessage = $"{options.GetType().Name} did not pass serialization roundtrip!{Environment.NewLine}  windiff {jsonFileSrcPath} {outJsonFile}";
                return false;
            }
            return true;
        }

        // TODO: implement tests for invalid param files (malformed, or not according to the schema) and invalid options (empty options or invalid values).
        [TestCategory("UnitTest")]
        ////[DataTestMethod]
        [DataRow("SvcutilOptions", "dotnet-svcutil.svcutil.json", DisplayName = "SvcutilOptions")]
        [DataRow("UpdateOptions", "dotnet-svcutil.update.json", DisplayName = "UpdateOptions")]
        [DataRow("WCFCSUpdateOptions", "dotnet-svcutil.update.json", DisplayName = "WCFCSUpdateOptions")]
        public void InvalidParamsFileOptions(string optionsType, string jsonFile)
        {
        }
    }
}
