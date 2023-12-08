// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Tools.ServiceModel.Svcutil;
using Xunit;

namespace SvcutilTest
{
    public partial class E2ETest
    {
        private void UnitTestSvcutil(string options, bool expectSuccess = true)
        {
            Assert.False(string.IsNullOrWhiteSpace(this_TestCaseName), $"{nameof(this_TestCaseName)} not initialized!");
            Assert.False(options == null, $"{nameof(options)} not initialized!");

            // run in the context of the test-group common project
            using (var cds = new CurrentDirectorySetter(this_TestCaseProject.DirectoryPath))
            {
                Directory.SetCurrentDirectory(this_TestCaseProject.DirectoryPath);

                var exitCode = 0;
                var outputText = string.Empty;
                var writer = new StringWriter();
                var originalOutput = Console.Out;

                Console.SetOut(writer);
                var args = options.Split(' ');
                exitCode = Tool.MainAsync(args, this_TestCaseLogger, CancellationToken.None).Result;

                outputText = writer.ToString();
                Console.SetOut(originalOutput);
                writer.Close();

                ValidateTest(options, this_TestCaseProject.DirectoryPath, exitCode, outputText, expectSuccess);
            }
        }
        
        [Trait("Category", "UnitTest")]
        [Theory]
        [InlineData(null)]
        [InlineData("minimal")]
        [InlineData("normal")]
        [InlineData("verbose")]
        [InlineData("debug")]
        [InlineData("silent")]
        [InlineData("invalid")]
        public void Verbosity(string verbosity)
        {
            this_TestCaseName = "Verbosity";
            TestFixture();
            var testCaseName = "Verbosity_" + (verbosity ?? "Default");
            InitializeUnitTest(testCaseName, createProject: false);

            // These tests all are expected to fail, we rely on the output log file to check the verbosity.
            var uri = Path.Combine(g_TestCasesDir, "wsdl", "brokenWsdl.wsdl");
            var verbosityOption = string.IsNullOrWhiteSpace(verbosity) ? string.Empty : $" -v {verbosity}";

            // use options that would make the tool show the incorrect tool operational context warning!
            var options = $"{uri} {verbosityOption} -tc project -d ../{testCaseName} -ntr";

            UnitTestSvcutil(options, expectSuccess: false);
        }

        [Trait("Category", "UnitTest")]
        [Theory]
        [InlineData("wsdlQuery", "BasicService1.svc?wsdl")]
        [InlineData("singleWsdlQuery", "BasicService1.svc?singlewsdl")]
        [InlineData("mexParam", "BasicService1.svc/mex")]
        [InlineData("noQuery", "BasicService1.svc")]
        public void MetadataQuery(string testCaseName, string uriPart)
        {
            this_TestCaseName = "MetadataQuery";
            TestFixture();

            InitializeUnitTest(testCaseName);

            var uri = $"{g_ServiceUrl}/{uriPart}";
            var options = $"{uri} -nl";
            this_TestCaseName = testCaseName;
            UnitTestSvcutil(AppendCommonOptions(options));
        }

        //[Trait("Category", "UnitTest")]
        //[Theory]
        //[InlineData("metadataEpr")]
        //public void MetadataEpr(string testCaseName)
        //{
        //    this_TestCaseName = "MetadataEpr";
        //    TestFixture();

        //    InitializeUnitTest(testCaseName);

        //    var uri = Path.Combine(g_TestCasesDir, "metadataEpr", "epr", "http___WcfProjectNServer_WcfProjectNService_WcfProjectNService.svc.xml");
        //    var options = $"{uri} -nl";
        //    this_TestCaseName = testCaseName;
        //    UnitTestSvcutil(AppendCommonOptions(options));
        //}

        [Trait("Category", "UnitTest")]
        [Theory]
        [InlineData("array", "System.Array")]
        [InlineData("dictionary", "System.Collections.Generic.Dictionary`2,System.Collections")]
        [InlineData("arrayList", "System.Collections.ArrayList,System.Collections.NonGeneric")]
        [InlineData("sortedList", "System.Collections.Generic.SortedList`2,System.Collections")]
        [InlineData("linkedList", "System.Collections.Generic.LinkedList`1,System.Collections")]
        [InlineData("sortedDictionary", "System.Collections.Generic.SortedDictionary`2,System.Collections")]
        [InlineData("list", "System.Collections.Generic.List`1,System.Collections")]
        [InlineData("hashTable", "System.Collections.Hashtable,System.Collections.NonGeneric")]
        [InlineData("sortedListNonGeneric", "System.Collections.SortedList,System.Collections.NonGeneric")]
        [InlineData("collection", "System.Collections.ObjectModel.Collection`1,System.Runtime")]
        [InlineData("observableCollection", "System.Collections.ObjectModel.ObservableCollection`1,System.ObjectModel")]
        [InlineData("hybridDictionary", "System.Collections.Specialized.HybridDictionary,System.Collections.Specialized")]
        [InlineData("listDictionary", "System.Collections.Specialized.ListDictionary,System.Collections.Specialized")]
        [InlineData("orderedDictionary", "System.Collections.Specialized.OrderedDictionary,System.Collections.Specialized")]
        public void Collections(string testCaseName, string collection)
        {
            this_TestCaseName = "Collections";
            TestFixture();

            InitializeUnitTest(testCaseName);

            var uri = Path.Combine(g_TestCasesDir, "wsdl", "WcfProjectNService", "tempuri.org.wsdl");
            var options = $"{uri} -nl -ct \"{collection}\"";
            this_TestCaseName = testCaseName;
            UnitTestSvcutil(AppendCommonOptions(options));
        }

        [Trait("Category", "UnitTest")]
        [Theory]
        [InlineData("tfmDefault", null, true)]
        [InlineData("tfm20", "netcoreapp2.0", true)]
        [InlineData("tfm100", "netcoreapp100.0", true)]
        [InlineData("tfm45", "net45", true)]
        [InlineData("net90", "net90", true)]
        [InlineData("badTFM", "badTFM", false)]
        public void TFM(string testCaseName, string targetFramework, bool expectSuccess)
        {
            this_TestCaseName = "TFM";
            TestFixture();

            //InitializeUnitTest(testCaseName, createProject: true, sdkVersion: "3.1.101");
            InitializeUnitTest(testCaseName, createProject: true, sdkVersion: g_SdkVersion);

            var uri = Path.Combine(g_TestCasesDir, "wsdl", "Simple.wsdl");
            var tf = targetFramework == null ? string.Empty : $"-tf {targetFramework}";
            var options = $"{uri} {tf}";
            this_TestCaseName = testCaseName;
            UnitTestSvcutil(AppendCommonOptions(options), expectSuccess);
        }

        [Trait("Category", "UnitTest")]
        [Theory]
        [InlineData("badParam", "http://www.myhost.com/MyService.svc -badparam")]
        [InlineData("xd", "in.wsdl -xd")]
        [InlineData("emptyD", "in.wsdl -d ")]
        [InlineData("emptyR", "in.wsdl -r")]
        [InlineData("emptyCt", "in.wsdl -ct")]
        [InlineData("emptyEt", "in.wsdl -et")]
        [InlineData("emptySer", "in.wsdl --serializer")]
        [InlineData("dir", @"-d:D:\Temp")]
        [InlineData("out", "-help:MyRef.cs")]
        [InlineData("ref", "--r:MyReference")]
        [InlineData("singleAndFlag", "in.wsdl -d -nl")]
        [InlineData("invalidAndFlag", "in.wsdl -invalid -nl")]
        [InlineData("invalidAndInput", "in.wsdl -invalid in2.wsdl")]
        public void CommandParserInvalid(string testCaseName, string options)
        {
            this_TestCaseName = "CommandParserInvalid";
            TestFixture();

            options = $"{options} -ntr"; // prevent project reference resolution.
            CommandOptions(testCaseName, options, serializeUpdateOptions: false, expectSuccess: false);
        }

        [Trait("Category", "UnitTest")]
        [Theory]
        [InlineData("basicOptions1", "$testCasesDir$/wsdl/Simple.wsdl -tc global")]
        [InlineData("basicOptions2", "$testCasesDir$/wsdl/Simple.wsdl -pf $projectPath$ -nb ")]
        [InlineData("basicOptions3", "$testCasesDir$/wsdl/Simple.wsdl -pf $projectPath$ -d OutputDir -o Reference.cs")]
        [InlineData("basicOptions4", "$testCasesDir$/wsdl/Simple.wsdl -pf $projectPath$ -d OutputDir -o Reference.cs -bd $bootstrapDir$")]
        [InlineData("basicOptions5", "$testCasesDir$/wsdl/WcfProjectNService/* -pf $projectPath$ -d OutputDir -o Reference.cs -bd $bootstrapDir$")]
        public void CommandOptionsBasic(string testCaseName, string options)
        {
            this_TestCaseName = "CommandOptionsBasic";
            TestFixture();

            options = $"{options} -ntr"; // prevent project reference resolution.
            CommandOptions(testCaseName, options, serializeUpdateOptions: false, expectSuccess: true);
        }

        [Trait("Category", "UnitTest")]
        [Theory]
        [InlineData("fwdWild", "../wsdl/* -o ServiceReference/Reference.cs")]
        [InlineData("fwdWildExt", "../wsdl/*.wsdl -d ServiceReference")]
        [InlineData("bckWild", "..\\wsdl\\* -d OutputDir/ServiceReference -o Reference.cs")]
        [InlineData("backFull", "..\\wsdl\\Simple.wsdl -d ..\\backFull\\OutputDir/ServiceReference -o OutputDir\\Reference.cs")]
        [InlineData("fwdMultiWild", "../wsdl/WcfProjectNService/* -d ..\\..\\CommandOptionsRelativePaths\\OutputDir")]
        [InlineData("fwdMultiWildExt", "../wsdl/WcfProjectNService/*.wsdl -d ../../CommandOptionsRelativePaths/OutputDir")]
        public void CommandOptionsFilePaths(string testCaseName, string options)
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                if(testCaseName == "bckWild" || testCaseName == "backFull")
                {
                    return; //Linux doesn't support back slash as path separator
                }
            }

            this_TestCaseName = "CommandOptionsFilePaths";
            TestFixture();

            var wsdlFilesDstDir = Path.Combine(this_TestGroupOutputDir, "wsdl");
            if (!Directory.Exists(wsdlFilesDstDir))
            {
                var wsdlFilesSrcDir = Path.Combine(g_TestCasesDir, "wsdl", "WcfProjectNService");
                FileUtil.CopyDirectory(wsdlFilesSrcDir.Replace("$testCasesDir$", g_TestCasesDir), Path.Combine(wsdlFilesDstDir, "WcfProjectNService"), overwrite: true);

                Directory.CreateDirectory(Path.Combine(this_TestGroupOutputDir, "wsdl"));
                File.Copy(Path.Combine(g_TestCasesDir, "wsdl", "Simple.wsdl"), Path.Combine(this_TestGroupOutputDir, "wsdl", "Simple.wsdl"));
            }

            options = $"-tc global -pf $projectPath$ -ntr {options}";

            CommandOptions(testCaseName, options, serializeUpdateOptions: true, expectSuccess: true);
        }

        private void CommandOptions(string testCaseName, string options, bool serializeUpdateOptions, bool expectSuccess)
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

                Assert.False(cmdOptions.Errors.Count() == 0 ^ expectSuccess, cmderrors);
                Assert.True(ValidateOutputs(options, out var failureMessage), failureMessage);
            }
        }

        [Trait("Category", "UnitTest")]
        [Theory]
        [InlineData("safeTelemetry", "-nl -v verbose -cn ES -ntr -tf netcoreapp2.1")]
        [InlineData("gdprSensitive", "http://www.myhost.com/myGdprensitiveUrl -d myGdrpSensitiveOutputDir -o myGdprSensitiveFileName -n \"*,myGdprSenstiveNS\" -bd myTempDir")]
        [InlineData("gdprFiltered", "-r assemblyRef -r {packageRef,*} -ct mycollectionType -et myexludetype")]
        public async Task CommandOptionsTelemetryString(string testCaseName, string options)
        {
            this_TestCaseName = "CommandOptionsTelemetryString";
            TestFixture();

            InitializeUnitTest(testCaseName, createProject: false, sdkVersion: g_SdkVersion);
            options = $"{options} -tc Infrastructure";

            var args = options.Split(' ');
            var cmdOptions = CommandParser.ParseCommand(args);

            await cmdOptions.ProcessBasicOptionsAsync(this_TestCaseLogger, CancellationToken.None);
            await cmdOptions.ResolveAsync(CancellationToken.None);

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

            Assert.True(ValidateOutputs(options, out var failureMessage), failureMessage);
        }

        [Trait("Category", "UnitTest")]
        [Theory]
        [InlineData("SvcutilOptions", "dotnet-svcutil.svcutil.json")]
        [InlineData("UpdateOptions", "dotnet-svcutil.update.json")]
        public void OptionsRoundtrip(string optionsType, string jsonFile)
        {
            this_TestCaseName = "OptionsRoundtrip";
            TestFixture();
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

            Assert.True(ValidateJson(options, jsonFileSrcPath, out errorMessage), errorMessage);

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
    }
}
