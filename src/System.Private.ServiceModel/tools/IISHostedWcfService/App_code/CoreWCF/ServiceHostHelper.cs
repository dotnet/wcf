// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using System.Diagnostics;

namespace WcfService
{
    public class ServiceHostHelper
    {
        public static async Task<bool> PingAsync(string url)
        {
            Console.WriteLine("Ping service host...");
            using HttpClient httpClient = new HttpClient();

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    using HttpResponseMessage response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Ping succeed...");
                        httpClient.Dispose();
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Ping failed... retrying after 10 seconds. {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ping failed... retrying after 10 seconds. {ex.Message}");
                }
                await Task.Delay(10000);
            }

            Console.WriteLine("Ping failed... Exiting..");
            httpClient.Dispose();
            return false;
        }

        public static async Task ServiceBootstrap()
        {
            string processArgs = "";
            if (Process.GetCurrentProcess().ProcessName == "dotnet")
            {
                processArgs = "exec --roll-forward Major " + String.Join(" ", Environment.GetCommandLineArgs().SkipLast(1));
            }

            var process = new Process()
            {
                StartInfo = {
                    FileName = Environment.ProcessPath,
                    Arguments = processArgs,
                    WindowStyle = ProcessWindowStyle.Normal,
                    WorkingDirectory = Environment.CurrentDirectory,
                    RedirectStandardOutput = false,
                    RedirectStandardInput = false,
                    UseShellExecute = false
                },
                EnableRaisingEvents = false
            };

            process.Start();

            Console.WriteLine($"Starting process in the background: {Path.GetFileName(process.ProcessName)}, ID: {process.Id}.");

            // Ping to make sure the service is started
            bool result = await PingAsync("http://localhost:8081/TestHost.svc/Ping");
            if (result)
            {
                Environment.Exit(0);
            }
            Environment.Exit(1);
        }

        public static Enum ToServiceSchema(string CoreWcfScheme) => CoreWcfScheme switch
        {
            "http" => ServiceSchema.HTTP,
            "https" => ServiceSchema.HTTPS,
            "net.tcp" => ServiceSchema.NETTCP,
            "net.pipe" => ServiceSchema.NETPIPE,
            "ws" => ServiceSchema.WS,
            "wss" => ServiceSchema.WSS,
            _ => throw new ArgumentOutOfRangeException(nameof(CoreWcfScheme), $"Not expected CoreWcfScheme value: {CoreWcfScheme}"),
        };
    }
}
#endif
