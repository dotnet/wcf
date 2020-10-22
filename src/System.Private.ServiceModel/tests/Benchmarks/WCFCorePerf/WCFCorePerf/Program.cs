﻿using System;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using Microsoft.Crank.EventSources;

namespace WCFCorePerf
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

        static void Main(string[] args)
        {
            Console.WriteLine("WCFCorePerf Client.");

            Program test = new Program();

            if (test.ProcessRunOptions(args))
            {
                var startTime = DateTime.Now;
                int request = 0;

                BenchmarksEventSource.Log.Metadata("channelopen", "max", "max", "Channel Open Time (ms)", "Time to Open Channel in ms", "n0");
                BenchmarksEventSource.Log.Metadata("firstrequest", "max", "max", "First Request (ms)", "Time to first request in ms", "n0");
                BenchmarksEventSource.Log.Metadata("bombardier/requests", "max", "sum", "Requests (" + test._paramPerfMeasurementDuration.TotalMilliseconds + " ms)", "Total number of requests", "n0");
                BenchmarksEventSource.Log.Metadata("bombardier/rps/max", "max", "sum", "Requests/sec (max)", "Max requests per second", "n0");

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
                        ChannelFactory<ISayHello> factory = new ChannelFactory<ISayHello>(binding, new EndpointAddress(test._paramServiceUrl));
                        var stopwatchChannelOpen = new Stopwatch();
                        stopwatchChannelOpen.Start();
                        factory.Open();
                        BenchmarksEventSource.Measure("channelopen", stopwatchChannelOpen.ElapsedMilliseconds);

                        var client = factory.CreateChannel();
                        var stopwatchFirstReq = new Stopwatch();
                        stopwatchFirstReq.Start();
                        var result = client.HelloAsync("helloworld").Result;
                        BenchmarksEventSource.Measure("firstrequest", stopwatchFirstReq.ElapsedMilliseconds);

                        while (DateTime.Now <= startTime.Add(test._paramPerfMeasurementDuration))
                        {
                            var rtnResult = client.HelloAsync("helloworld").Result;
                            request++;
                        }

                        BenchmarksEventSource.Measure("bombardier/requests", request);
                        BenchmarksEventSource.Measure("bombardier/rps/max", request / test._paramPerfMeasurementDuration.TotalSeconds);
                        break;
                    case TestBinding.WSHttp:                        
                        CertificateInstallUtil.EnsureClientCertificateInstalled(test._paramServiceUrl, "machinecert");                        
                        WSHttpBinding wsHttpBinding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
                        wsHttpBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
                        ChannelFactory<ISayHello> wsHttpFactory = new ChannelFactory<ISayHello>(wsHttpBinding, new EndpointAddress(test._paramServiceUrl));
                        wsHttpFactory.Credentials.UserName.UserName = "abc";
                        wsHttpFactory.Credentials.UserName.Password = "abc";

                        var stopwatchWSHttpChannelOpen = new Stopwatch();
                        stopwatchWSHttpChannelOpen.Start();
                        wsHttpFactory.Open();
                        BenchmarksEventSource.Measure("channelopen", stopwatchWSHttpChannelOpen.ElapsedMilliseconds);

                        var clientWSHttp = wsHttpFactory.CreateChannel();
                        var stopwatchWSHttpFirstReq = new Stopwatch();
                        stopwatchWSHttpFirstReq.Start();
                        Console.WriteLine(clientWSHttp.HelloAsync("helloworld").Result);
                        BenchmarksEventSource.Measure("firstrequest", stopwatchWSHttpFirstReq.ElapsedMilliseconds);

                        while (DateTime.Now <= startTime.Add(test._paramPerfMeasurementDuration))
                        {
                            var rtnResult = clientWSHttp.HelloAsync("helloworld").Result;
                            request++;
                        }

                        BenchmarksEventSource.Measure("bombardier/requests", request);
                        BenchmarksEventSource.Measure("bombardier/rps/max", request / test._paramPerfMeasurementDuration.TotalSeconds);
                        break;
                    case TestBinding.NetTcp:
                        NetTcpBinding netTcpBinding = new NetTcpBinding(SecurityMode.None);
                        ChannelFactory<IService1> netTcpFactory = new ChannelFactory<IService1>(netTcpBinding, new EndpointAddress(test._paramServiceUrl));

                        var stopwatchNetTcpChannelOpen = new Stopwatch();
                        stopwatchNetTcpChannelOpen.Start();
                        netTcpFactory.Open();
                        BenchmarksEventSource.Measure("channelopen", stopwatchNetTcpChannelOpen.ElapsedMilliseconds);

                        var clientNetTcp = netTcpFactory.CreateChannel();
                        var stopwatchNetTcpFirstReq = new Stopwatch();
                        stopwatchNetTcpFirstReq.Start();
                        var netTcpResult = clientNetTcp.GetDataAsync(1).Result;
                        BenchmarksEventSource.Measure("firstrequest", stopwatchNetTcpFirstReq.ElapsedMilliseconds);

                        while (DateTime.Now <= startTime.Add(test._paramPerfMeasurementDuration))
                        {
                            var rtnResult = clientNetTcp.GetDataAsync(1).Result;
                            request++;
                        }

                        BenchmarksEventSource.Measure("bombardier/requests", request);
                        BenchmarksEventSource.Measure("bombardier/rps/max", request / test._paramPerfMeasurementDuration.TotalSeconds);
                        break;
                }
            }
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
