using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Bridge.Tests
{
    class BridgeLaunchFixture
    {
        public BridgeLaunchFixture()
        {
            EnsureBridge();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void EnsureBridge()
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo("powershell.exe",
                        "-ExecutionPolicy Bypass -File " +
                        Path.GetFullPath("artifacts\\ensureBridge.ps1") +
                        " " + Constants.BasePortNumber);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            var proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.WaitForExit();
            string result = proc.StandardOutput.ReadToEnd();
            Console.WriteLine("Result from Test: " + result);

            if (proc.ExitCode != 0)
            {
                throw new InvalidOperationException("Could not launch Bridge process.");
            }
        }
    }
}
