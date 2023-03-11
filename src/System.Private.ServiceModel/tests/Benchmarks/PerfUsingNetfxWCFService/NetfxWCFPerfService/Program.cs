using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WCFCorePerfService
{
    public class Parameters
    {
        public const string Port = "port";
    }

    class Program
    {
        private string _paramPort = "Port";
        static async Task Main(string[] args)
        {
            string url = "https://wcfcrank.blob.core.windows.net/app/WcfCorePerfCrankService.zip";

            var bombardierFileName = Path.GetFileName(url);
            var _httpClient = new HttpClient();

            using (var downloadStream = await _httpClient.GetStreamAsync(url))
            {
                using (var fileStream = File.Create(bombardierFileName))
                {
                    await downloadStream.CopyToAsync(fileStream);
                }
            }

            ZipFile.ExtractToDirectory(bombardierFileName, @".\", true);
            string filePath = Path.Combine(Environment.CurrentDirectory, "WcfCorePerfCrankService.exe");
            string command = $" advfirewall firewall add rule name=\"WcfCorePerfCrankService\" dir=in protocol=TCP action=allow program=\"{filePath}\" enable=yes";
            ExecuteCommand(command, Environment.CurrentDirectory, TimeSpan.FromSeconds(20));
            Program test = new Program();
            if (test.ProcessRunOptions(args))
            {
                var process = new Process()
                {
                    StartInfo = {
                    FileName = "WcfCorePerfCrankService.exe",
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false
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

                string arg = "port:" + test._paramPort;
                process.StartInfo.Arguments = arg;
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();                
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
                    case Parameters.Port:
                        _paramPort = p[1];
                        break;
                    default:
                        Console.WriteLine("unknown argument: " + s);
                        continue;
                }
            }

            return true;
        }

        private static int ExecuteCommand(string command, string workingDirectory, TimeSpan timeout)
        {
            Process process = new Process();
            process.StartInfo.FileName = "netsh";
            process.StartInfo.Arguments = command;
            if (workingDirectory != null)
            {
                process.StartInfo.WorkingDirectory = workingDirectory;
            }
            process.StartInfo.UseShellExecute = false;
            process.Start();
            bool flag;
            if (timeout.TotalMilliseconds >= Int32.MaxValue)
            {
                flag = process.WaitForExit(Int32.MaxValue);
            }
            else
            {
                flag = process.WaitForExit((int)timeout.TotalMilliseconds);
            }
            if (!flag)
            {
                process.Kill();
            }

            if (!flag)
            {
                throw new TimeoutException(string.Format("Command '{0}' was killed by timeout {1}.", new object[]
                {
                    command,
                    timeout.ToString()
                }));
            }
            return process.ExitCode;
        }
    }
}
