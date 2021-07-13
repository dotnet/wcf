using Benchmarks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFPerfClient
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
            Program test = new Program();

            if (test.ProcessRunOptions(args))
            {
                var startTime = DateTime.Now;
                int request = 0;
                StringBuilder sb = new StringBuilder();

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

                        ChannelFactory<ISayHello> factory = new ChannelFactory<ISayHello>(binding, new EndpointAddress(test._paramServiceUrl));
                        var stopwatchChannelOpen = new Stopwatch();
                        stopwatchChannelOpen.Start();
                        factory.Open();
                        sb.Append("channelopen:" + stopwatchChannelOpen.ElapsedMilliseconds);

                        var client = factory.CreateChannel();
                        var stopwatchFirstReq = new Stopwatch();
                        stopwatchFirstReq.Start();
                        var result = client.HelloAsync("helloworld").Result;

                        sb.Append(" firstrequest:" + stopwatchFirstReq.ElapsedMilliseconds);

                        while (DateTime.Now <= startTime.Add(test._paramPerfMeasurementDuration))
                        {
                            var rtnResult = client.HelloAsync("helloworld").Result;
                            request++;
                        }
                        sb.Append(" bombardier/requests:" + request);
                        sb.Append(" bombardier/rps/max:" + request / test._paramPerfMeasurementDuration.TotalSeconds);
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
                        sb.Append("channelopen:" + stopwatchWSHttpChannelOpen.ElapsedMilliseconds);

                        var clientWSHttp = wsHttpFactory.CreateChannel();
                        var stopwatchWSHttpFirstReq = new Stopwatch();
                        stopwatchWSHttpFirstReq.Start();
                        var wsResults=clientWSHttp.HelloAsync("helloworld").Result;
                        sb.Append(" firstrequest:" + stopwatchWSHttpFirstReq.ElapsedMilliseconds);

                        while (DateTime.Now <= startTime.Add(test._paramPerfMeasurementDuration))
                        {
                            var rtnResult = clientWSHttp.HelloAsync("helloworld").Result;
                            request++;
                        }

                        sb.Append(" bombardier/requests:" + request);
                        sb.Append(" bombardier/rps/max:" + request / test._paramPerfMeasurementDuration.TotalSeconds);
                        break;
                    case TestBinding.NetTcp:
                        NetTcpBinding netTcpBinding = new NetTcpBinding(SecurityMode.None);
                        ChannelFactory<IService1> netTcpFactory = new ChannelFactory<IService1>(netTcpBinding, new EndpointAddress(test._paramServiceUrl));

                        var stopwatchNetTcpChannelOpen = new Stopwatch();
                        stopwatchNetTcpChannelOpen.Start();
                        netTcpFactory.Open();
                        sb.Append("channelopen:" + stopwatchNetTcpChannelOpen.ElapsedMilliseconds);

                        var clientNetTcp = netTcpFactory.CreateChannel();
                        var stopwatchNetTcpFirstReq = new Stopwatch();
                        stopwatchNetTcpFirstReq.Start();
                        var netTcpResult = clientNetTcp.GetDataAsync(1).Result;
                        sb.Append(" firstrequest:" + stopwatchNetTcpFirstReq.ElapsedMilliseconds);

                        while (DateTime.Now <= startTime.Add(test._paramPerfMeasurementDuration))
                        {
                            var rtnResult = clientNetTcp.GetDataAsync(1).Result;
                            request++;
                        }

                        sb.Append(" bombardier/requests:" + request);
                        sb.Append(" bombardier/rps/max:" + request / test._paramPerfMeasurementDuration.TotalSeconds);
                        break;
                }

                Console.WriteLine(sb.ToString());
            }
        }

        private bool ProcessRunOptions(string[] args)
        {
            foreach (string s in args)
            {
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
