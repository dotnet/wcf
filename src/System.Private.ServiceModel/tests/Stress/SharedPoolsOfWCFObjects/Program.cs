// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Diagnostics;
using WcfService1;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SharedPoolsOfWCFObjects
{
    public enum ProgramToRun { Stress, Perf }
    public enum TestToRun { HelloWorld, HelloWorldAPM, Streaming, Duplex, DuplexStreaming }
    public enum TestBinding { Http, Https, NetTcp, NetHttpBinding }

    // This class simply provides a namespace for command line parameter names
    public class Parameters
    {
        public const string DebugMode = "debugmode";
        public const string Program2Run = "program2run";
        public const string StressLevel = "stresslevel";
        public const string StressRunDuration = "stressrunduration";
        public const string HostName = "hostname";
        public const string AppName = "appname";
        public const string Binding = "binding";
        public const string UseAsync = "useasync";
        public const string Async = "async";
        public const string Test = "test";
        public const string StreamingScenario = "streamingscenario";
        public const string PoolFactoriesForPerfStartup = "poolfactoriesforperfstartup";
        public const string UseSeparateTaskForEachChannel = "useseparatetaskforeachchannel";
        public const string PerfMaxThroughputTasks = "perfmaxthroughputtasks";
        public const string PerfThroughputTaskStep = "perfthroughputtaskstep";
        public const string PerfMaxStartupTasks = "perfmaxstartuptasks";
        public const string PerfStartupIterations = "perfstartupiterations";
        public const string PerfMeasurementDuration = "perfmeasurementduration";
        public const string SelfHostedPorts = "selfhostedports";
        public const string SelfHostedPortStartNumber = "selfhostedportstartnumber";
        public const string ResultsLog = "resultslog";
        public const string PerfRunName = "perfrunname";
        public const string StressRunName = "stressrunname";
        public const string RecycleFrequencyThrottle = "recyclefrequencythrottle";
        public const string StressIterations = "stressiterations";
        public const string OpenTimeoutMSecs = "opentimeoutmsecs";
        public const string CloseTimeoutMSecs = "closetimeoutmsecs";
        public const string ReceiveTimeoutMSecs = "receivetimeoutmsecs";
        public const string SendTimeoutMSecs = "sendtimeoutmsecs";
        public const string CustomConnectionPoolSize = "customconnectionpoolsize";
        public const string ExitOnSuccess = "exitonsuccess";

        public const string BindingSecurityMode = "bindingsecuritymode";
        public const string HttpClientCredentialType = "httpclientcredentialtype";
        public const string TcpClientCredentialType = "tcpclientcredentialtype";
        public const string ServerDnsEndpointIdentity = "serverdnsendpointidentity";
        public const string ClientCertThumbprint = "clientcertthumbprint";
        public const string ClientCertThumbprintFile = "clientcertthumbprintfile";
        public const string CertStoreName = "certstorename";
        public const string CertStoreLocation = "certstorelocation";

        public const string ReportingUrl = "reportingurl";
    }


    public class Program
    {
        // Parameters and default values
        private ProgramToRun _paramProgram2Run = ProgramToRun.Stress;
        private int _paramStressLevel = DefaultStressLevel;
        private TimeSpan _paramStressRunDuration = TimeSpan.FromMinutes(1);
        private string _paramHostName = "localhost";
        private string _paramAppName = "WcfService1";
        private TestBinding _paramBinding = TestBinding.Http;
        private bool _paramUseAsync = true;
        private TestToRun _paramTestToRun = TestToRun.HelloWorld;
        private StreamingScenarios _paramStreamingScenario = StreamingScenarios.StreamAll;
        private bool _paramPoolFactoriesForPerfStartup = false;
        private bool _paramUseSeparateTaskForEachChannel = false;
        private int _paramPerfMaxStartupTasks = DefaultPerfMaxStartupTasks;
        private int _paramPerfMaxThroughputTasks = DefaultPerfMaxThroughputTasks;
        private int _paramPerfThroughputTaskStep = DefaultPerfThroughputTaskStep;
        private int _paramPerfStartupIterations = DefaultPerfStartupIterations;
        private TimeSpan _paramPerfMeasurementDuration = s_defaultPerfMeasurementDuration;
        private int _paramSHPortNum = 0;
        private int _paramSHPorts = 0;
        private int _paramRecycleFrequencyThrottle = 5000;
        private long _recycleThrottle = 0;
        private long _paramStressIterations = DefaultStressIterations;
        private int _paramOpenTimeoutMSecs = 0;
        private int _paramCloseTimeoutMSecs = 0;
        private int _paramReceiveTimeoutMSecs = 0;
        private int _paramSendTimeoutMSecs = 0;
        private int _customConnectionPoolSize = -1;
        private bool _paramExitOnSuccess = false;

        // Right now this is just a boolean switch to allow more tracing
        private bool _paramDebugMode = false;

        // a few security-related parameters
        private SecurityMode _paramBindingSecurityMode = SecurityMode.None;
        private HttpClientCredentialType _paramHttpClientCredentialType = HttpClientCredentialType.None;
        private TcpClientCredentialType _paramTcpClientCredentialType = TcpClientCredentialType.None;
        private string _paramServerDnsEndpointIdentity;
        private string _paramClientCertThumbprint;
        private StoreName _paramCertStoreName = StoreName.My;
        private StoreLocation _paramCertStoreLocation = StoreLocation.LocalMachine;

        // Results
        private string _paramResultsLog = "PerfResults.xml";
        private string _paramPerfRunName = "defaultPerfRunName";
        private string _paramStressRunName = String.Empty;
        private string _paramReportingUrl = "";

        // These defaults seem to work well to produce stable results in the existing perf tests:
        private const int DefaultPerfMaxStartupTasks = 25;
        private const int DefaultPerfMaxThroughputTasks = 96;
        private const int DefaultPerfThroughputTaskStep = 5;
        private const int DefaultPerfStartupIterations = 1000;
        private readonly static TimeSpan s_defaultPerfMeasurementDuration = TimeSpan.FromSeconds(10);

        private const int DefaultStressLevel = 10;
        private static Task s_completedTask = Task.FromResult(true);
        // Sometimes we might want to execute stress tests a certain number of iterations rather than run for a certain period of time.
        // The default number of iterations is an arbitrary large number that would leave this type of stress running for a very long time.
        private const long DefaultStressIterations = 1024L * 1024L * 1024L * 1024L;

        public static void Main(string[] args)
        {
            var test = new Program();

            if (test.ProcessRunOptions(args))
            {
                if (test._paramProgram2Run == ProgramToRun.Stress)
                {
                    test.RunStress();
                }
                else if (test._paramProgram2Run == ProgramToRun.Perf)
                {
                    test.RunPerf();
                }
            }
        }

        private ReportingService.RunStartupData CreateRunStartupData()
        {
            return new ReportingService.RunStartupData()
            {
                MachineName = System.Environment.MachineName,
                AppName = _paramAppName,
                Binding = (ReportingService.TestBinding)_paramBinding,
                BindingSecurityMode = _paramBindingSecurityMode,
                ClientCertThumbprint = _paramClientCertThumbprint,
                HostName = _paramHostName,
                HttpClientCredentialType = _paramHttpClientCredentialType,
                Iterations = _paramStressIterations,
                PerfMaxStartupTasks = _paramPerfMaxStartupTasks,
                PerfMaxThroughputTasks = _paramPerfMaxThroughputTasks,
                PerfMeasurementDuration = _paramPerfMeasurementDuration,
                RunName = (_paramProgram2Run == ProgramToRun.Perf) ? _paramPerfRunName : _paramStressRunName,
                PerfStartupIterations = _paramPerfStartupIterations,
                PerfThroughputTaskStep = _paramPerfThroughputTaskStep,
                PoolFactoriesForPerfStartup = _paramPoolFactoriesForPerfStartup,
                Program2Run = (ReportingService.ProgramToRun)_paramProgram2Run,
                ResultsLog = _paramResultsLog,
                ServerDnsEndpointIdentity = _paramServerDnsEndpointIdentity,
                SHPortNum = _paramSHPortNum,
                SHPorts = _paramSHPorts,
                StreamingScenario = (ReportingService.StreamingScenarios)_paramStreamingScenario,
                StressLevel = _paramStressLevel,
                StressRunDuration = _paramStressRunDuration,
                TcpClientCredentialType = _paramTcpClientCredentialType,
                TestToRun = (ReportingService.TestToRun)_paramTestToRun,
                UseAsync = _paramUseAsync,
                UseSeparateTaskForEachChannel = _paramUseSeparateTaskForEachChannel
            };
        }
        private bool ProcessRunOptions(string[] args)
        {
            // The following string contains only the most common parameters:
            Console.WriteLine(
                "[HostName:servername] [AppName:serverappname] [Program2Run:Stress|Perf] [Async:true|false] \r\n" +
                "[Binding:Http|Https|NetTcp|NetHttpBinding] [Test:HelloWorld|Streaming|Duplex|DuplexStreaming] \r\n" +
                "[SelfHostedPortStartNumber:port#] [SelfHostedPorts:#ports] [DebugMode:false|true] [ReportingUrl:reportingUrl]\r\n" +
                "[(Open/Close/Receive/Send)TimeoutMSecs:milliseconds] [CustomConnectionPoolSize:size] \r\n" +
                "    Stress parameters:\r\n" +
                "[StressRunDuration:minutes] [StressLevel:#threads] [RecycleFrequencyThrottle:#requests]\r\n" +
                "    Perf parameters: \r\n" +
                "[PoolFactoriesForPerfStartup:false|true] [PerfMaxStartupTasks:#tasks] [PerfStartupIterations:#iterations] \r\n" +
                "[PerfMaxThroughputTasks:#tasks] [PerfThroughputTaskStep:#tasks] [UseSeparateTaskForEachChannel:false|true] \r\n" +
                "[StreamingScenario:StreamAll|StreamOut|StreamIn|StreamEcho] [PerfMeasurementDuration:seconds] \r\n" +
                "[ResultsLog:logFile] [PerfRunName:name]\r\n" +
                "    Security parameters: \r\n" +
                "[BindingSecurityMode:None|Transport] \r\n" +
                "[HttpClientCredentialType:None|Basic|Digest|Ntlm|Windows|Certificate|InheritedFromHost] \r\n" +
                "[TcpClientCredentialType:None|Windows|Certificate] \r\n" +
                "[ServerDnsEndpointIdentity:identity] \r\n" +
                "[ClientCertThumbprint:thumbprint] [ClientCertThumbprintFile:filename]\r\n" +
                "[CertStoreName:ClientCertificateStoreName] [CertStoreLocation:LocalMachine|CurrentUser] \r\n\r\n");

            foreach (string s in args)
            {
                Console.WriteLine(s);
                string[] p = s.Split(new char[] { ':' }, count: 2);
                if (p.Length != 2)
                {
                    continue;
                }

                switch (p[0].ToLower())
                {
                    case Parameters.ResultsLog:
                        _paramResultsLog = p[1];
                        break;
                    case Parameters.PerfRunName:
                        _paramPerfRunName = p[1];
                        break;
                    case Parameters.StressRunName:
                        _paramStressRunName = p[1];
                        break;
                    case Parameters.HostName:
                        _paramHostName = p[1];
                        break;
                    case Parameters.AppName:
                        _paramAppName = p[1];
                        break;
                    case Parameters.Program2Run:
                        if (!Enum.TryParse(p[1], ignoreCase: true, result: out _paramProgram2Run))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.StressRunDuration:
                        int minutes = 0;
                        if (!Int32.TryParse(p[1], out minutes))
                        {
                            return ReportWrongArgument(s);
                        }
                        _paramStressRunDuration = TimeSpan.FromMinutes(minutes);
                        break;
                    case Parameters.StressLevel:
                        if (!Int32.TryParse(p[1], out _paramStressLevel))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.Binding:
                        if (!Enum.TryParse<TestBinding>(p[1], ignoreCase: true, result: out _paramBinding))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.UseAsync:
                    case Parameters.Async:
                        if (!Boolean.TryParse(p[1].ToLower(), out _paramUseAsync))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.Test:
                        if (!Enum.TryParse<TestToRun>(p[1], ignoreCase: true, result: out _paramTestToRun))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.StreamingScenario:
                        if (!Enum.TryParse<StreamingScenarios>(p[1], ignoreCase: true, result: out _paramStreamingScenario))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.UseSeparateTaskForEachChannel:
                        if (!Boolean.TryParse(p[1].ToLower(), out _paramUseSeparateTaskForEachChannel))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.PoolFactoriesForPerfStartup:
                        if (!Boolean.TryParse(p[1].ToLower(), out _paramPoolFactoriesForPerfStartup))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.PerfMaxStartupTasks:
                        if (!Int32.TryParse(p[1], out _paramPerfMaxStartupTasks))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.PerfMaxThroughputTasks:
                        if (!Int32.TryParse(p[1], out _paramPerfMaxThroughputTasks))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.PerfThroughputTaskStep:
                        if (!Int32.TryParse(p[1], out _paramPerfThroughputTaskStep))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.PerfStartupIterations:
                        if (!Int32.TryParse(p[1], out _paramPerfStartupIterations))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.PerfMeasurementDuration:
                        int perfPerfMeasurementDurationSeconds = 0;
                        if (!Int32.TryParse(p[1], out perfPerfMeasurementDurationSeconds))
                        {
                            return ReportWrongArgument(s);
                        }
                        _paramPerfMeasurementDuration = TimeSpan.FromSeconds(perfPerfMeasurementDurationSeconds);
                        break;
                    case Parameters.SelfHostedPorts:
                        if (!Int32.TryParse(p[1], out _paramSHPorts))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.SelfHostedPortStartNumber:
                        if (!Int32.TryParse(p[1], out _paramSHPortNum))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.BindingSecurityMode:
                        if (!Enum.TryParse(p[1], ignoreCase: true, result: out _paramBindingSecurityMode))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.HttpClientCredentialType:
                        if (!Enum.TryParse(p[1], ignoreCase: true, result: out _paramHttpClientCredentialType))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.TcpClientCredentialType:
                        if (!Enum.TryParse(p[1], ignoreCase: true, result: out _paramTcpClientCredentialType))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.ServerDnsEndpointIdentity:
                        _paramServerDnsEndpointIdentity = p[1];
                        break;
                    case Parameters.ClientCertThumbprint:
                        _paramClientCertThumbprint = p[1];
                        break;
                    case Parameters.ClientCertThumbprintFile:
                        var filename = p[1];
                        _paramClientCertThumbprint = File.ReadAllText(filename).Trim();
                        Console.WriteLine("Thumb: " + _paramClientCertThumbprint);
                        break;
                    case Parameters.CertStoreName:
                        if (!Enum.TryParse(p[1], ignoreCase: true, result: out _paramCertStoreName))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.CertStoreLocation:
                        if (!Enum.TryParse(p[1], ignoreCase: true, result: out _paramCertStoreLocation))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.DebugMode:
                        if (!Boolean.TryParse(p[1].ToLower(), out _paramDebugMode))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.RecycleFrequencyThrottle:
                        if (!Int32.TryParse(p[1], out _paramRecycleFrequencyThrottle))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.StressIterations:
                        if (!Int64.TryParse(p[1], out _paramStressIterations))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.OpenTimeoutMSecs:
                        if (!Int32.TryParse(p[1], out _paramOpenTimeoutMSecs))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.CloseTimeoutMSecs:
                        if (!Int32.TryParse(p[1], out _paramCloseTimeoutMSecs))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.ReceiveTimeoutMSecs:
                        if (!Int32.TryParse(p[1], out _paramReceiveTimeoutMSecs))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.SendTimeoutMSecs:
                        if (!Int32.TryParse(p[1], out _paramSendTimeoutMSecs))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.CustomConnectionPoolSize:
                        if (!Int32.TryParse(p[1], out _customConnectionPoolSize))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.ExitOnSuccess:
                        if (!Boolean.TryParse(p[1].ToLower(), out _paramExitOnSuccess))
                        {
                            return ReportWrongArgument(s);
                        }
                        break;
                    case Parameters.ReportingUrl:
                        _paramReportingUrl = p[1];
                        break;
                    default:
                        Console.WriteLine("unknown argument: " + s);
                        continue;
                }
            }

            // If the parameters didn't specify the stress run name
            // then come up with something rather than using an empty string
            // E.g. the current directory name may give a good clue of what is being run
            if (_paramProgram2Run == ProgramToRun.Stress && String.IsNullOrEmpty(_paramStressRunName))
            {
                var dir = Directory.GetCurrentDirectory().Split(new char[] { '\\', '/' });
                if (dir.Length > 0)
                {
                    var defaultRunName = dir[dir.Length - 1];
                    _paramStressRunName = defaultRunName;
                    Console.WriteLine(String.Format("Using {0} as the stress run name.", defaultRunName));
                }
            }

            // Give the test helpers all the parameters at once
            TestHelpers.SetHelperParameters(
                useBinding: _paramBinding,
                hostName: _paramHostName,
                appName: _paramAppName,
                selfHostPortStartingNumber: _paramSHPortNum,
                numSelfHostPorts: _paramSHPorts,
                bindingSecurityMode: _paramBindingSecurityMode,
                httpClientCredentialType: _paramHttpClientCredentialType,
                tcpClientCredentialType: _paramTcpClientCredentialType,
                serverDnsEndpointIdentity: _paramServerDnsEndpointIdentity,
                clientCertThumbprint: _paramClientCertThumbprint,
                clientCertStoreName: _paramCertStoreName,
                clientCertStoreLocation: _paramCertStoreLocation,
                openTimeoutMSecs: _paramOpenTimeoutMSecs,
                closeTimeoutMSecs: _paramCloseTimeoutMSecs,
                receiveTimeoutMSecs: _paramReceiveTimeoutMSecs,
                sendTimeoutMSecs: _paramSendTimeoutMSecs,
                customConnectionPoolSize: _customConnectionPoolSize,
                debugMode: _paramDebugMode);

            // Rather than passing additional parameters to the test via generic types we use a static method
            StreamingPerfTestParamsBase.StreamingScenario = _paramStreamingScenario;

            return true;
        }

        private bool ReportWrongArgument(string arg)
        {
            Console.WriteLine("Wrong parameter: " + arg);
            return false;
        }

        #region Stress
        private void RunStress()
        {
            // The exact behavior below is a good candidate to be controlled by command line parameters:
            // - repeat stress runs indefinitely?
            // - user interactive mode (pause for user input after each iteration)?
            // - call GC / finalizers after each iteration?
            // - automatically cycle through different settings for each iteration?
            // etc... 
            while (true)
            {
                RunStressImpl();
                if (_paramExitOnSuccess)
                {
                    break;
                }
                Console.WriteLine("Stress run is finished. Press Enter to induce a GC.");
                Console.ReadLine();
                GC.Collect(2, mode: GCCollectionMode.Forced, blocking: true);
                GC.WaitForPendingFinalizers();
                GC.Collect(2, mode: GCCollectionMode.Forced, blocking: true);
                Console.WriteLine("After GC. Press Enter to rerun the stress run.");
                Console.ReadLine();
            }
        }
        private void RunStressImpl()
        {
            switch (_paramTestToRun)
            {
                case TestToRun.HelloWorld:
                    DoTheStressRun(
                        RunAllStressTests<IService1, HelloWorldTest<CommonStressTestParams>, CommonStressTestParams>,
                        RunAllStressTestsAsync<IService1, HelloWorldTest<CommonStressTestParams>, CommonStressTestParams>);
                    break;
                case TestToRun.HelloWorldAPM:
                    DoTheStressRun(
                        RunAllStressTests<IService1, HelloWorldAPMTest<CommonStressTestParams>, CommonStressTestParams>,
                        RunAllStressTestsAsync<IService1, HelloWorldAPMTest<CommonStressTestParams>, CommonStressTestParams>);
                    break;
                case TestToRun.Streaming:
                    DoTheStressRun(
                        RunAllStressTests<IStreamingService, StreamingTest<IStreamingService, StreamingStressTestParams>, StreamingStressTestParams>,
                        RunAllStressTestsAsync<IStreamingService, StreamingTest<IStreamingService, StreamingStressTestParams>, StreamingStressTestParams>);
                    break;
                case TestToRun.Duplex:
                    DoTheStressRun(
                        RunAllStressTests<IDuplexService, DuplexTest<DuplexStressTestParams>, DuplexStressTestParams>,
                        RunAllStressTestsAsync<IDuplexService, DuplexTest<DuplexStressTestParams>, DuplexStressTestParams>);
                    break;
                case TestToRun.DuplexStreaming:
                    DoTheStressRun(
                        RunAllStressTests<IDuplexStreamingService, DuplexStreamingTest<IDuplexStreamingService, DuplexStreamingStressTestParams>, DuplexStreamingStressTestParams>,
                        RunAllStressTestsAsync<IDuplexStreamingService, DuplexStreamingTest<IDuplexStreamingService, DuplexStreamingStressTestParams>, DuplexStreamingStressTestParams>);
                    break;
                default:
                    Console.WriteLine("Not implemented: " + _paramTestToRun);
                    return;
            }
        }

        private void DoTheStressRun(Action test, Func<Task> testAsync)
        {
            if (_paramUseAsync)
            {
                DoTheStressRunImpl(testAsync);
            }
            else
            {
                DoTheStressRunImpl(() => { test(); return s_completedTask; });
            }
        }

        private void DoTheStressRunImpl(Func<Task> testAsync)
        {
            var runReporting = new RunReportingService(_paramReportingUrl);

            TestUtils.SetFailureLogger((message, criticalFailure) =>
            {
                if (criticalFailure)
                {
                    runReporting.RunFinished(success: !criticalFailure, message: message);
                }
                else
                {
                    runReporting.LogMessage(message);
                }
            });

            runReporting.RunStarted(CreateRunStartupData());

            var cts = new CancellationTokenSource(_paramStressRunDuration);
            Console.WriteLine("Start");
            var startTime = DateTime.Now;

            Task[] allTasks = new Task[_paramStressLevel];
            for (int t = 0; t < allTasks.Length; t++)
            {
                int taskNum = t;
                allTasks[t] = Task.Run(async () =>
                {
                    for (long i = 0; i < _paramStressIterations / allTasks.Length; i++)
                    {
                        try
                        {
                            await testAsync();
                        }
                        catch (Exception e)
                        {
                            TestUtils.ReportFailure(e.ToString());
                            throw;
                        }

                        if (i % 100 == 0)
                        {
                            Console.WriteLine((string)(DateTime.Now.ToString() + " " + taskNum + " " + i));

                            runReporting.HeartBeat(taskNum, i);
                        }

                        if (cts.Token.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    Console.WriteLine((string)(taskNum + ": done"));
                }, cts.Token);
            }
            Task.WaitAll(allTasks);

            runReporting.RunFinished(success: true, message: string.Empty);
            Console.WriteLine("Dispose all");
            StaticDisposablesHelper.DisposeAll();
        }



        // The following 2 methods call all the stress tests we have.
        // Another good candidate for a command line parameter would be a bitmask to select only certain tests (for more targeted stress)
        public void RunAllStressTests<ChannelType, TestTemplate, TestParams>()
            where ChannelType : class
            where TestTemplate : ITestTemplate<ChannelType, TestParams>, IExceptionPolicy, new()
            where TestParams : IPoolTestParameter
        {
            if (Interlocked.Increment(ref _recycleThrottle) % _paramRecycleFrequencyThrottle == 0)
            {
                CreateAndCloseFactoryAndChannelFullCycleTest<ChannelType, TestTemplate, TestParams>.CreateFactoriesAndChannelsUseAllOnceCloseAll();
            }
            PooledFactories<ChannelType, TestTemplate, TestParams>.CreateUseAndCloseChannels();
            PooledFactoriesAndChannels<ChannelType, TestTemplate, TestParams>.UseAllChannelsInPooledFactoriesAndChannels();
            RecyclablePooledFactories<ChannelType, TestTemplate, TestParams>.RunAllScenariosWithWeights(_paramRecycleFrequencyThrottle, 1);
            RecyclablePooledFactoriesAndChannels_OpenOnce<ChannelType, TestTemplate, TestParams>.RunAllScenariosWithWeights(_paramRecycleFrequencyThrottle, 1, 1);
            RecyclablePooledFactoriesAndChannels<ChannelType, TestTemplate, TestParams>.RunAllScenariosWithWeights(_paramRecycleFrequencyThrottle, 1, 1);
        }

        public async Task RunAllStressTestsAsync<ChannelType, TestTemplate, TestParams>()
            where ChannelType : class
            where TestTemplate : ITestTemplate<ChannelType, TestParams>, IExceptionPolicy, new()
            where TestParams : IPoolTestParameter
        {
            // Short-lived connections can quickly exhaust available ports
            if (Interlocked.Increment(ref _recycleThrottle) % _paramRecycleFrequencyThrottle == 0)
            {
                await CreateAndCloseFactoryAndChannelFullCycleTestAsync<ChannelType, TestTemplate, TestParams>.CreateFactoriesAndChannelsUseAllOnceCloseAllAsync();
            }
            await PooledFactoriesAsync<ChannelType, TestTemplate, TestParams>.CreateUseAndCloseChannelsAsync();
            await PooledFactoriesAndChannelsAsync<ChannelType, TestTemplate, TestParams>.UseAllChannelsInPooledFactoriesAndChannelsAsync();
            await RecyclablePooledFactoriesAsync<ChannelType, TestTemplate, TestParams>.RunAllScenariosWithWeightsAsync(_paramRecycleFrequencyThrottle, 1);
            await RecyclablePooledFactoriesAndChannelsAsync_OpenOnce<ChannelType, TestTemplate, TestParams>.RunAllScenariosWithWeightsAsync(_paramRecycleFrequencyThrottle, 1, 1);
            await RecyclablePooledFactoriesAndChannelsAsync<ChannelType, TestTemplate, TestParams>.RunAllScenariosWithWeightsAsync(_paramRecycleFrequencyThrottle, 1, 1);
        }
        #endregion

        #region Perf
        public void RunPerf()
        {
            switch (_paramTestToRun)
            {
                case TestToRun.HelloWorld:
                    RunAllTests<IService1,
                        HelloWorldTest<CommonPerfStartupTestParams>, CommonPerfStartupTestParams,
                        HelloWorldTest<CommonPerfThroughputTestParams>, CommonPerfThroughputTestParams>();
                    break;
                case TestToRun.HelloWorldAPM:
                    RunAllTests<IService1,
                        HelloWorldAPMTest<CommonPerfStartupTestParams>, CommonPerfStartupTestParams,
                        HelloWorldAPMTest<CommonPerfThroughputTestParams>, CommonPerfThroughputTestParams>();
                    break;
                case TestToRun.Streaming:
                    RunAllTests<IStreamingService,
                        StreamingTest<IStreamingService, StreamingPerfStartupTestParams>, StreamingPerfStartupTestParams,
                        StreamingTest<IStreamingService, StreamingPerfThroughputTestParams>, StreamingPerfThroughputTestParams>();
                    break;
                case TestToRun.Duplex:
                    RunAllTests<IDuplexService,
                        DuplexTest<DuplexPerfStartupTestParams>, DuplexPerfStartupTestParams,
                        DuplexTest<DuplexPerfThroughputTestParams>, DuplexPerfThroughputTestParams>();
                    break;
                case TestToRun.DuplexStreaming:
                    RunAllTests<IDuplexStreamingService,
                        DuplexStreamingTest<IDuplexStreamingService, DuplexStreamingPerfStartupTestParams>, DuplexStreamingPerfStartupTestParams,
                        DuplexStreamingTest<IDuplexStreamingService, DuplexStreamingPerfThroughputTestParams>, DuplexStreamingPerfThroughputTestParams>();
                    break;
                default:
                    Console.WriteLine("Not implemented: " + _paramTestToRun);
                    return;
            }

            var sw = new Stopwatch();
            sw.Start();
            StaticDisposablesHelper.DisposeAll();
            Console.WriteLine("Dispose all done in " + sw.ElapsedMilliseconds);
        }

        private PerfResults _perfResults = null;
        private PerfResults.PerfRunResult _perfRunResult = null;

        public void RunAllTests<ChannelType, StartupTestTemplate, StartupTestParams, ThroughputTestTemplate, ThroughputTestParams>()
            where ChannelType : class
            where StartupTestTemplate : ITestTemplate<ChannelType, StartupTestParams>, new()
            where StartupTestParams : IPoolTestParameter
            where ThroughputTestTemplate : ITestTemplate<ChannelType, ThroughputTestParams>, new()
            where ThroughputTestParams : IPoolTestParameter
        {
            _perfRunResult = new PerfResults.PerfRunResult()
            {
                name = _paramPerfRunName,
                async = _paramUseAsync,
                binding = _paramBinding.ToString(),
                test = _paramTestToRun.ToString(),
                streamingScenario = _paramStreamingScenario.ToString(),
                poolFactoriesForPerfStartup = _paramPoolFactoriesForPerfStartup,
                useSeparateTaskForEachChannel = _paramUseSeparateTaskForEachChannel,
                perfMaxTasks = _paramPerfMaxThroughputTasks,
                perfTaskStep = _paramPerfThroughputTaskStep,
                perfMaxStartupTasks = _paramPerfMaxStartupTasks,
                perfStartupIterations = _paramPerfStartupIterations,
                perfMeasurementDuration = _paramPerfMeasurementDuration.TotalSeconds.ToString(),
                first1K = 0,
                maxThroughput = 0,
                maxThroughputTasks = 0
            };

            if (File.Exists(_paramResultsLog))
            {
                try
                {
                    _perfResults = PerfResults.ReadResults(_paramResultsLog);
                }
                catch (Exception e)
                {
                    // Report and ignore the exception
                    Console.WriteLine("Failed to deserialize " + _paramResultsLog);
                    Console.WriteLine(e.ToString());
                }
            }
            if (_perfResults == null)
            {
                _perfResults = new PerfResults();
                _perfResults.PerfRunResults = new List<PerfResults.PerfRunResult>();
            }

            _perfResults.PerfRunResults.Add(_perfRunResult);

            if (_paramUseAsync)
            {
                RunFirstNIterationsAsyncAndSaveResults(_paramPerfStartupIterations, _paramPerfMaxStartupTasks,
                    testAsync: RunStartupPerfTestsAsync<ChannelType, StartupTestTemplate, StartupTestParams>);

                RunMaxThroughputAsyncAndSaveResults(duration: _paramPerfMeasurementDuration, maxTasks: _paramPerfMaxThroughputTasks, taskStep: _paramPerfThroughputTaskStep,
                    testAsync: RunThroughputPerfTestsAsync<ChannelType, ThroughputTestTemplate, ThroughputTestParams>);
            }
            else
            {
                RunFirstNIterationsAndSaveResults(_paramPerfStartupIterations, _paramPerfMaxStartupTasks,
                    test: RunStartupPerfTests<ChannelType, StartupTestTemplate, StartupTestParams>);

                RunMaxThroughputAndSaveResults(duration: _paramPerfMeasurementDuration, maxTasks: _paramPerfMaxThroughputTasks, taskStep: _paramPerfThroughputTaskStep,
                    test: RunThroughputPerfTests<ChannelType, ThroughputTestTemplate, ThroughputTestParams>);
            }
        }

        public void RunFirstNIterationsAndSaveResults(int iterations, int maxTasks, Func<int> test)
        {
            _perfRunResult.first1K = RunFirstNIterationsAsyncImpl(iterations, maxTasks, () => Task.FromResult<int>(test()));
            PerfResults.SaveResults(_paramResultsLog, _perfResults);
        }
        public void RunFirstNIterationsAsyncAndSaveResults(int iterations, int maxTasks, Func<Task<int>> testAsync)
        {
            _perfRunResult.first1K = RunFirstNIterationsAsyncImpl(iterations, maxTasks, testAsync);
            PerfResults.SaveResults(_paramResultsLog, _perfResults);
        }

        public int RunFirstNIterationsAsyncImpl(int iterations, int maxTasks, Func<Task> testAsync)
        {
            Task[] allTasks = new Task[maxTasks];
            Console.WriteLine("Start RunFirstNIterations with " + iterations + " iterations");
            var sw = new Stopwatch();
            sw.Start();

            for (int t = 0; t < allTasks.Length; t++)
            {
                int taskNum = t;
                allTasks[t] = Task.Run(async () =>
                {
                    for (int i = 0; i < iterations / allTasks.Length; i++)
                    {
                        try
                        {
                            await testAsync();
                        }
                        catch (Exception e)
                        {
                            TestUtils.ReportFailure(e.ToString());
                            throw;
                        }
#if DEBUG
                        if (i % 100 == 0)
                        {
                            Console.WriteLine(DateTime.Now.ToString() + " " + taskNum + " " + i);
                        }
#endif
                    }
#if DEBUG
                    Console.WriteLine(taskNum + ": done");
#endif
                });
            }
            Task.WaitAll(allTasks);
            sw.Stop();
            Console.WriteLine(_paramPerfStartupIterations + " are done in: " + sw.ElapsedMilliseconds);
            return (int)sw.ElapsedMilliseconds;
        }

        public void RunMaxThroughputAndSaveResults(TimeSpan duration, int maxTasks, int taskStep, Func<int, int> test)
        {
            RunMaxThroughputAsyncAndSaveResults(duration, maxTasks, taskStep, (n) => Task.FromResult<int>(test(n)));
        }
        public void RunMaxThroughputAsyncAndSaveResults(TimeSpan duration, int maxTasks, int taskStep, Func<int, Task<int>> testAsync)
        {
            double maxThroughput;
            int maxThroughputTasks;
            double avgThroughputUp, avgThroughputDown;
            RunMaxThroughputAsyncImpl(duration, maxTasks, taskStep, testAsync, out maxThroughput, out maxThroughputTasks, out avgThroughputUp);
            _perfRunResult.maxThroughput = maxThroughput;
            _perfRunResult.maxThroughputTasks = maxThroughputTasks;
            _perfRunResult.avgThroughputUp = avgThroughputUp;

            // Save it in case we crash during the second round
            PerfResults.SaveResults(_paramResultsLog, _perfResults);

            RunMaxThroughputReverseAsyncImpl(duration, maxTasks, taskStep, testAsync, out maxThroughput, out maxThroughputTasks, out avgThroughputDown);
            if (maxThroughput > _perfRunResult.maxThroughput)
            {
                _perfRunResult.maxThroughput = maxThroughput;
                _perfRunResult.maxThroughputTasks = maxThroughputTasks;
            }
            _perfRunResult.avgThroughputDown = avgThroughputDown;
            PerfResults.SaveResults(_paramResultsLog, _perfResults);
        }

        private static long s_requestsCompleted = 0;

        public static void RunMaxThroughputAsyncImpl(TimeSpan duration, int maxTasks, int taskStep, Func<int, Task<int>> testAsync, out double bestThroughput, out int bestThroughputTasks, out double avgThroughputUp)
        {
            bool stopAllTasks = false;
            long iterationStartRequests, iterationEndRequests;
            DateTime iterationStartTime, iterationEndTime;
            Task[] allTasks = new Task[maxTasks];
            bestThroughput = 0;
            bestThroughputTasks = 0;

            Console.WriteLine("Determining maximum throughput. ");

            var sw = new Stopwatch();
            sw.Start();

            for (int t = 0; t < allTasks.Length;)
            {
                iterationStartRequests = s_requestsCompleted;
                iterationStartTime = DateTime.Now;

                // For powerful machines it gets too slow to iterate 1 task at a time
                // So we increase the number of tasks we inject for each measurement
                var step = (t == 0 ? 1 : taskStep); // And we want to start with measuring 1 task first
                for (int tt = 0; tt < step && t < allTasks.Length; tt++)
                {
                    int taskNum = t++;
                    allTasks[taskNum] = Task.Run(async () =>
                    {
                        long l = 0;
                        while (!stopAllTasks)
                        {
                            try
                            {
                                int requests = await testAsync(taskNum);
                                Interlocked.Add(ref s_requestsCompleted, requests);
                                l += requests;
                            }
                            catch (Exception e)
                            {
                                TestUtils.ReportFailure(e.ToString());
                                throw;
                            }
#if DEBUG
                            if (l % 1000 == 0)
                            {
                                Console.WriteLine(DateTime.Now.ToString() + " " + taskNum + " " + l);
                            }
#endif
                        }
#if DEBUG
                        Console.WriteLine(taskNum + ": done");
#endif
                    });
                }
                Task.Delay(duration).Wait();

                iterationEndTime = DateTime.Now;
                iterationEndRequests = s_requestsCompleted;
                double throughtput = ((iterationEndRequests - iterationStartRequests) / ((iterationEndTime - iterationStartTime).TotalSeconds));
                Console.WriteLine(String.Format("!!!          Tasks {0} throughput Rq/s {1}", t, throughtput));
                if (throughtput > bestThroughput)
                {
                    bestThroughput = throughtput;
                    bestThroughputTasks = t;
                }
            }

            stopAllTasks = true;
            Task.WaitAll(allTasks);
            sw.Stop();
            avgThroughputUp = s_requestsCompleted * 1000 / sw.ElapsedMilliseconds;
            Console.WriteLine(s_requestsCompleted + " are done in: " + sw.ElapsedMilliseconds);
            Console.WriteLine(String.Format("\r\n Best throughput {0} with {1} tasks \r\n", bestThroughput, bestThroughputTasks));
        }

        public static void RunMaxThroughputReverseAsyncImpl(TimeSpan duration, int maxTasks, int taskStep, Func<int, Task<int>> testAsync, out double bestThroughput, out int bestThroughputTasks, out double avgThroughputDown)
        {
            bestThroughput = 0;
            bestThroughputTasks = 0;
            bool stopAllTasks = false;
            long iterationStartRequests, iterationEndRequests;
            DateTime iterationStartTime, iterationEndTime;


            Console.WriteLine("Determining maximum throughput. ");
            Task[] allTasks = new Task[maxTasks];
            CancellationTokenSource[] allCancellations = new CancellationTokenSource[maxTasks];

            var sw = new Stopwatch();
            sw.Start();
            for (int t = 0; t < maxTasks; t++)
            {
                int taskNum = t;
                allCancellations[t] = new CancellationTokenSource();

                allTasks[t] = Task.Run(async () =>
                {
                    long l = 0;
                    while (!stopAllTasks)
                    {
                        try
                        {
                            int requests = await testAsync(taskNum);
                            Interlocked.Add(ref s_requestsCompleted, requests);
                            l += requests;
                        }
                        catch (Exception e)
                        {
                            TestUtils.ReportFailure(e.ToString());
                            throw;
                        }
#if DEBUG
                        if (l % 1000 == 0)
                        {
                            Console.WriteLine(DateTime.Now.ToString() + " " + taskNum + " " + l);
                        }
#endif
                        if (allCancellations[taskNum].IsCancellationRequested)
                        {
                            break;
                        }
                    }
#if DEBUG
                    Console.WriteLine(taskNum + ": done");
#endif
                });
            }

            // We don't wait for all the tasks scheduled above to actually start running before we start their cancellations
            // Depending on the platform this may lead to incorrect reporting of the actual number of tasks
            for (int t = maxTasks - 1; t >= 0;)
            {
                iterationStartRequests = s_requestsCompleted;
                iterationStartTime = DateTime.Now;
                Task.Delay(duration).Wait();
                iterationEndTime = DateTime.Now;
                iterationEndRequests = s_requestsCompleted;
                double throughtput = ((iterationEndRequests - iterationStartRequests) / ((iterationEndTime - iterationStartTime).TotalSeconds));
                Console.WriteLine(String.Format("!!!          Tasks {0} throughput Rq/s {1}", t + 1, throughtput));
                if (throughtput > bestThroughput)
                {
                    bestThroughput = throughtput;
                    bestThroughputTasks = t + 1;
                }
#if DEBUG
                Console.WriteLine("Stopping task " + (t + 1));
#endif
                for (int tt = 0; tt < taskStep & t >= 0; tt++)
                {
                    allCancellations[t--].Cancel();
                }
            }

            stopAllTasks = true;
            Task.WaitAll(allTasks);
            sw.Stop();
            avgThroughputDown = s_requestsCompleted * 1000 / sw.ElapsedMilliseconds;
            Console.WriteLine(s_requestsCompleted + " are done in: " + sw.ElapsedMilliseconds);
            Console.WriteLine(String.Format("\r\n Best throughput {0} with {1} tasks \r\n", bestThroughput, bestThroughputTasks));
        }


        // For the startup we use 2 scenarios:
        // - a full cycle of creating a factory, creating a channel, using them, and closing all
        // - create a new channel and close it after each use while pooling all channel factories
        public int RunStartupPerfTests<ChannelType, TestTemplate, TestParams>()
            where ChannelType : class
            where TestTemplate : ITestTemplate<ChannelType, TestParams>, new()
            where TestParams : IPoolTestParameter
        {
            if (_paramPoolFactoriesForPerfStartup)
            {
                return PooledFactories<ChannelType, TestTemplate, TestParams>.CreateUseAndCloseChannels();
            }
            else
            {
                return CreateAndCloseFactoryAndChannelFullCycleTest<ChannelType, TestTemplate, TestParams>.CreateFactoriesAndChannelsUseAllOnceCloseAll();
            }
        }

        // For the throughput scenario we choose to run scenario where both channel factories and channels are pooled and never recycled
        public int RunThroughputPerfTests<ChannelType, TestTemplate, TestParams>(int n)
            where ChannelType : class
            where TestTemplate : ITestTemplate<ChannelType, TestParams>, new()
            where TestParams : IPoolTestParameter
        {
            if (_paramUseSeparateTaskForEachChannel)
            {
                return PooledFactoriesAndChannels<ChannelType, TestTemplate, TestParams>.UseOneChannelInPooledFactoriesAndChannels(n);
            }
            else
            {
                return PooledFactoriesAndChannels<ChannelType, TestTemplate, TestParams>.UseAllChannelsInPooledFactoriesAndChannels();
            }
        }

        public async Task<int> RunStartupPerfTestsAsync<ChannelType, TestTemplate, TestParams>()
            where ChannelType : class
            where TestTemplate : ITestTemplate<ChannelType, TestParams>, new()
            where TestParams : IPoolTestParameter
        {
            if (_paramPoolFactoriesForPerfStartup)
            {
                return await PooledFactoriesAsync<ChannelType, TestTemplate, TestParams>.CreateUseAndCloseChannelsAsync();
            }
            else
            {
                return await CreateAndCloseFactoryAndChannelFullCycleTestAsync<ChannelType, TestTemplate, TestParams>.CreateFactoriesAndChannelsUseAllOnceCloseAllAsync();
            }
        }

        public async Task<int> RunThroughputPerfTestsAsync<ChannelType, TestTemplate, TestParams>(int n)
            where ChannelType : class
            where TestTemplate : ITestTemplate<ChannelType, TestParams>, new()
            where TestParams : IPoolTestParameter
        {
            if (_paramUseSeparateTaskForEachChannel)
            {
                return await PooledFactoriesAndChannelsAsync<ChannelType, TestTemplate, TestParams>.UseOneChannelInPooledFactoriesAndChannelsSerialAsync(n);
            }
            else
            {
                return await PooledFactoriesAndChannelsAsync<ChannelType, TestTemplate, TestParams>.UseAllChannelsInPooledFactoriesAndChannelsSerialAsync();
            }
        }
        #endregion


    }

    public class PerfResults
    {
        [XmlAttribute]
        public string logfile;


        public class PerfRunResult
        {
            [XmlAttribute]
            public string name;

            [XmlAttribute]
            public bool async;

            [XmlAttribute]
            public string binding;

            [XmlAttribute]
            public string test;

            [XmlAttribute]
            public double first1K;

            [XmlAttribute]
            public double maxThroughput;

            [XmlAttribute]
            public int maxThroughputTasks;

            [XmlAttribute]
            public double avgThroughputUp;

            [XmlAttribute]
            public double avgThroughputDown;

            [XmlAttribute]
            public string streamingScenario;

            [XmlAttribute]
            public bool poolFactoriesForPerfStartup;

            [XmlAttribute]
            public bool useSeparateTaskForEachChannel;

            [XmlAttribute]
            public int perfMaxTasks;

            [XmlAttribute]
            public int perfTaskStep;

            [XmlAttribute]
            public int perfMaxStartupTasks;

            [XmlAttribute]
            public int perfStartupIterations;

            [XmlAttribute]
            public string perfMeasurementDuration;
        }

        [System.Xml.Serialization.XmlArrayItem("PerfRunResult", typeof(PerfRunResult))]
        public List<PerfRunResult> PerfRunResults;

        // Static Read/Save
        public static PerfResults ReadResults(string resultsFileName)
        {
            System.Xml.Serialization.XmlSerializer deserializer = new System.Xml.Serialization.XmlSerializer(typeof(PerfResults));
            using (StreamReader r = new StreamReader(new FileStream(resultsFileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read, 4096, false)))
            {
                return (PerfResults)deserializer.Deserialize(r);
            }
        }

        public static void SaveResults(string resultsFileName, PerfResults results)
        {
            Console.WriteLine("Saving the results to " + resultsFileName);
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(PerfResults));
            using (StreamWriter w = new StreamWriter(new FileStream(resultsFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 4096, false)))
            {
                serializer.Serialize(w, results);
            }
        }
    }
}
