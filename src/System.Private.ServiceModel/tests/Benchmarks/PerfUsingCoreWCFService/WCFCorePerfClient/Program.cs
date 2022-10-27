using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using Microsoft.Crank.EventSources;

namespace WCFCorePerfClient
{
    public class Parameters
    {
        public const string Binding = "binding";
        public const string ServiceUrl = "serviceurl";
        public const string TransferMode = "transfermode";
        public const string ReportingUrl = "reportingurl";
        public const string PerfMeasurementDuration = "perfmeasurementduration";
    }

    public enum TestBinding { BasicHttp, WSHttp, NetTcp }

    class Program
    {
        private TestBinding _paramBinding = TestBinding.BasicHttp;
        private TimeSpan _paramPerfMeasurementDuration = s_defaultPerfMeasurementDuration;
        private string _paramServiceUrl = "";
        private readonly static TimeSpan s_defaultPerfMeasurementDuration = TimeSpan.FromSeconds(10);
        private string _paramTransferMode = "Buffered";

        static async Task Main(string[] args)
        {
            Console.WriteLine("WCFCorePerf Client.");

            Program test = new Program();

            if (test.ProcessRunOptions(args))
            {
                BenchmarksEventSource.Register("wcfcoreperf/channelopensync", Operations.Max, Operations.Max, "Channel Open Sync Time (ms)", "Time to Open Sync Channel in ms", "n0");
                BenchmarksEventSource.Register("wcfcoreperf/firstsyncrequest", Operations.Max, Operations.Max, "First Sync Request (ms)", "Time to first sync request in ms", "n0");
                BenchmarksEventSource.Register("wcfcoreperf/syncrequests", Operations.Max, Operations.Sum, "Sync Requests (" + test._paramPerfMeasurementDuration.TotalMilliseconds + " ms)", "Total number of syncrequests", "n0");
                BenchmarksEventSource.Register("wcfcoreperf/rps/maxsync", Operations.Max, Operations.Sum, "Requests/sec (maxsync)", "Max sync requests per second", "n0");

                BenchmarksEventSource.Register("wcfcoreperf/channelopenasync", Operations.Max, Operations.Max, "Channel Open Async Time (ms)", "Time to Open Async Channel in ms", "n0");
                BenchmarksEventSource.Register("wcfcoreperf/firstasyncrequest", Operations.Max, Operations.Max, "First Async Request (ms)", "Time to first request in ms", "n0");
                BenchmarksEventSource.Register("wcfcoreperf/asyncrequests", Operations.Max, Operations.Sum, "Async Requests (" + test._paramPerfMeasurementDuration.TotalMilliseconds + " ms)", "Total number of async requests", "n0");
                BenchmarksEventSource.Register("wcfcoreperf/rps/maxasync", Operations.Max, Operations.Sum, "Requests/sec (maxasync)", "Max async requests per second", "n0");

                var stopwatch = new Stopwatch();
                stopwatch.Reset();
                switch (test._paramBinding)
                {
                    case TestBinding.BasicHttp:
                        BasicHttpBinding binding = new BasicHttpBinding();
                        switch (test._paramTransferMode.ToLower())
                        {
                            case "buffered":
                                binding.TransferMode = TransferMode.Buffered;
                                break;
                            case "streamed":
                                binding.TransferMode = TransferMode.Streamed;
                                break;
                            case "streamedrequest":
                                binding.TransferMode = TransferMode.StreamedRequest;
                                break;
                            case "streamedresponse":
                                binding.TransferMode = TransferMode.StreamedResponse;
                                break;
                            default:
                                break;
                        }

                        Console.WriteLine($"Testing TransferMode: {binding.TransferMode}");
                        var factory = await test.RunFirstChannelOpen(test._paramBinding, binding, true, test._paramServiceUrl, stopwatch);
                        await test.RunFirstChannelOpen(test._paramBinding, binding, false, test._paramServiceUrl, stopwatch);

                        var client = factory.CreateChannel();

                        await test.RunFirstIteration(client, true, stopwatch);
                        await test.RunFirstIteration(client, false, stopwatch);

                        await test.RunMaxThroughput(client, true, test._paramPerfMeasurementDuration);
                        await test.RunMaxThroughput(client, false, test._paramPerfMeasurementDuration);

                        break;
                    case TestBinding.WSHttp:
                        WSHttpBinding wsHttpBinding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
                        wsHttpBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;

                        var wsHttpFactory = await test.RunFirstChannelOpen(test._paramBinding, wsHttpBinding, true, test._paramServiceUrl, stopwatch);
                        await test.RunFirstChannelOpen(test._paramBinding, wsHttpBinding, false, test._paramServiceUrl, stopwatch);

                        var clientWSHttp = wsHttpFactory.CreateChannel();

                        await test.RunFirstIteration(clientWSHttp, true, stopwatch);
                        await test.RunFirstIteration(clientWSHttp, false, stopwatch);

                        await test.RunMaxThroughput(clientWSHttp, true, test._paramPerfMeasurementDuration);
                        await test.RunMaxThroughput(clientWSHttp, false, test._paramPerfMeasurementDuration);
                        break;
                    case TestBinding.NetTcp:
                        NetTcpBinding netTcpBinding = new NetTcpBinding(SecurityMode.None);
                        netTcpBinding.SendTimeout = TimeSpan.FromSeconds(300);
                        netTcpBinding.ReceiveTimeout = TimeSpan.FromSeconds(300);
                        var netTcpFactory = await test.RunFirstChannelOpen(test._paramBinding, netTcpBinding, true, test._paramServiceUrl, stopwatch);
                        await test.RunFirstChannelOpen(test._paramBinding, netTcpBinding, false, test._paramServiceUrl, stopwatch);

                        var clientNetTcp = netTcpFactory.CreateChannel();

                        await test.RunFirstIteration(clientNetTcp, true, stopwatch);
                        await test.RunMaxThroughput(clientNetTcp, true, test._paramPerfMeasurementDuration);

                        await test.RunFirstIteration(clientNetTcp, false, stopwatch);
                        await test.RunMaxThroughput(clientNetTcp, false, test._paramPerfMeasurementDuration);
                        break;
                }
            }
        }

        private ChannelFactory<ISayHello> GetChannelFactory(TestBinding testBinding, Binding binding, string address)
        {
            ChannelFactory<ISayHello> factory = null;
            switch (testBinding)
            {
                case TestBinding.BasicHttp:
                case TestBinding.NetTcp:
                    factory = new ChannelFactory<ISayHello>(binding, new EndpointAddress(address));
                    break;
                case TestBinding.WSHttp:
                    factory = new ChannelFactory<ISayHello>(binding, new EndpointAddress(address));
                    factory.Credentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication
                    {
                        CertificateValidationMode = X509CertificateValidationMode.None,
                        RevocationMode = X509RevocationMode.NoCheck
                    };
                    factory.Credentials.UserName.UserName = "abc";
                    factory.Credentials.UserName.Password = "[PLACEHOLDER]";
                    break;
            }

            return factory;
        }

        private async Task RunMaxThroughput(ISayHello client, bool sync, TimeSpan time)
        {
            DateTime iterationStartTime, iterationEndTime, iterationDurationTime;
            int request = 0;
            iterationStartTime = DateTime.Now;
            iterationDurationTime = iterationStartTime.Add(time);
            while (DateTime.Now <= iterationDurationTime)
            {
                if (sync)
                {
                    Console.WriteLine($"Get sync result:{client.Hello("helloworld")}");
                }
                else
                {
                    Console.WriteLine($"Get async result:{await client.HelloAsync("helloworld")}");
                }
                request++;
            }

            iterationEndTime = DateTime.Now;
            if (sync)
            {
                BenchmarksEventSource.Measure("wcfcoreperf/syncrequests", request);
                BenchmarksEventSource.Measure("wcfcoreperf/rps/maxsync", request / (iterationEndTime - iterationStartTime).TotalSeconds);
            }
            else
            {
                BenchmarksEventSource.Measure("wcfcoreperf/asyncrequests", request);
                BenchmarksEventSource.Measure("wcfcoreperf/rps/maxasync", request / (iterationEndTime - iterationStartTime).TotalSeconds);
            }
        }

        private async Task RunFirstIteration(ISayHello client, bool sync, Stopwatch stopwatch)
        {
            stopwatch.Restart();
            if (sync)
            {
                Console.WriteLine($"Get the first sync request: {client.Hello("helloworld")}");
                BenchmarksEventSource.Measure("wcfcoreperf/firstsyncrequest", stopwatch.ElapsedMilliseconds);
            }
            else
            {
                Console.WriteLine($"Get the first async request: {await client.HelloAsync("helloworld")}");
                BenchmarksEventSource.Measure("wcfcoreperf/firstasyncrequest", stopwatch.ElapsedMilliseconds);
            }
        }

        private async Task<ChannelFactory<ISayHello>> RunFirstChannelOpen(TestBinding testbinding, Binding binding, bool sync, string address, Stopwatch stopwatch)
        {
            stopwatch.Restart();
            ChannelFactory<ISayHello> factory = null;
            if (sync)
            {
                factory = GetChannelFactory(testbinding, binding, address);
                factory.Open();
                BenchmarksEventSource.Measure("wcfcoreperf/channelopensync", stopwatch.ElapsedMilliseconds);
            }
            else
            {
                factory = GetChannelFactory(testbinding, binding, address);
                await Task.Factory.FromAsync(factory.BeginOpen, factory.EndClose, null);
                BenchmarksEventSource.Measure("wcfcoreperf/channelopenasync", stopwatch.ElapsedMilliseconds);
            }

            return factory;
        }

        private bool ProcessRunOptions(string[] args)
        {
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
                    case Parameters.Binding:
                        if (!Enum.TryParse<TestBinding>(p[1], ignoreCase: true, result: out _paramBinding))
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

                    case Parameters.ServiceUrl:
                        _paramServiceUrl = p[1];
                        break;

                    case Parameters.TransferMode:
                        _paramTransferMode = p[1];
                        break;
                    default:
                        Console.WriteLine("unknown argument: " + s);
                        continue;
                }
            }

            return true;
        }

        private bool ReportWrongArgument(string arg)
        {
            Console.WriteLine("Wrong parameter: " + arg);
            return false;
        }
    }
}
