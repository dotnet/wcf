using System;
using System.Diagnostics;
using System.IO;
using Web.App_Start;

namespace Bridge
{
    class Program
    {
        private const int DefaultPortNumber = 44283;

        static void Main(string[] args)
        {
            int portNumber = DefaultPortNumber;

            if (args.Length > 0)
            {
                if (!int.TryParse(args[0], out portNumber))
                {
                    portNumber = DefaultPortNumber;
                }
            }

            string baseAddress = "http://localhost:" + portNumber;
            OwinSelfhostStartup.Startup(baseAddress);
            Console.WriteLine("Sample Usage:");
            //curl --request PUT 'http://<baseAddress>/resource' -H "Content-Type:application/json" -H "Accept: application/json" --data "{name:'Bridge.Commands.Hostname'}"
            Console.WriteLine("curl --request PUT '{0}/resource' -H \"Content-Type:application/json\" -H \"Accept: application/json\" --data \"{{name:'Bridge.Commands.Hostname'}}\"", baseAddress);
            Test(portNumber);
            Console.ReadLine();
        }

        [Conditional("DEBUG")]
        static void Test(int portNumber)
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo("powershell.exe", "-ExecutionPolicy Bypass -File " + Path.GetFullPath("ensureBridge.ps1") + " " + portNumber);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            var proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.WaitForExit();
            string result = proc.StandardOutput.ReadToEnd();
            Console.WriteLine("Result from Test: " + result);
        }
    }
}
