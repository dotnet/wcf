using Benchmarks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WCFPerfDesktopCrank
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
        private int _paramPerfMeasurementDuration = s_defaultPerfMeasurementDuration;
        private string _paramServiceUrl = "";
        private readonly static int s_defaultPerfMeasurementDuration = 10;
        private string _paramTransferMode = "Buffered";

        static async Task Main(string[] args)
        {
            Program test = new Program();
            var bombardierUrl = "https://wcfcrank.blob.core.windows.net/app/WCFPerfClient.exe";
            var bombardierFileName = Path.GetFileName(bombardierUrl);
            var _httpClient = new HttpClient();

            using (var downloadStream = await _httpClient.GetStreamAsync(bombardierUrl))

            using (var fileStream = File.Create(bombardierFileName))
            {
                await downloadStream.CopyToAsync(fileStream);
            }

            if (test.ProcessRunOptions(args))
            {
                BenchmarksEventSource.Log.Metadata("channelopen", "max", "max", "Channel Open Time (ms)", "Time to Open Channel in ms", "n0");
                BenchmarksEventSource.Log.Metadata("firstrequest", "max", "max", "First Request (ms)", "Time to first request in ms", "n0");
                BenchmarksEventSource.Log.Metadata("bombardier/requests", "max", "sum", "Requests (" + test._paramPerfMeasurementDuration * 1000 + " ms)", "Total number of requests", "n0");
                BenchmarksEventSource.Log.Metadata("bombardier/rps/max", "max", "sum", "Requests/sec (max)", "Max requests per second", "n0");

                var process = new Process()
                {
                    StartInfo = {
                    FileName = bombardierFileName,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                },
                    EnableRaisingEvents = true
                };

                var stringBuilder = new StringBuilder();

                process.OutputDataReceived += (_, e) =>
                {
                    if (e != null && e.Data != null)
                    {
                        Console.WriteLine(e.Data);

                        lock (stringBuilder)
                        {
                            stringBuilder.AppendLine(e.Data);
                        }
                    }
                };

                string arg = "binding:" + test._paramBinding + " transfermode:" + test._paramTransferMode + " perfmeasurementduration:" + test._paramPerfMeasurementDuration + " serviceurl:" + test._paramServiceUrl;
                process.StartInfo.Arguments = arg;
                process.Start();
                BenchmarksEventSource.SetChildProcessId(process.Id);
                process.BeginOutputReadLine();
                process.WaitForExit();

                var dict = test.ProcessOutPut(stringBuilder.ToString());
                BenchmarksEventSource.Measure("channelopen", dict["channelopen"]);
                BenchmarksEventSource.Measure("firstrequest", dict["firstrequest"]);
                BenchmarksEventSource.Measure("bombardier/requests", dict["bombardier/requests"]);
                BenchmarksEventSource.Measure("bombardier/rps/max", dict["bombardier/rps/max"]);
            }
        }

        private Dictionary<string, string> ProcessOutPut(string value)
        {
            var results = value.Split(' ');
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (results.Length < 1)
            {
                throw new Exception("the output value is wrong");
            }
            else
            {
                foreach (string v in results)
                {
                    dict.Add(v.Split(':')[0], v.Split(':')[1]);
                }
            }
            return dict;
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
                        _paramPerfMeasurementDuration = perfPerfMeasurementDurationSeconds;
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
