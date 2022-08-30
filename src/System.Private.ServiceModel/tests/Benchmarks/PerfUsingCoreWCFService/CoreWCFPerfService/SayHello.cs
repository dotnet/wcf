using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WCFCorePerfService
{
    public class SayHello : ISayHello
    {
        public Task<string> HelloAsync(string name)
        {
            return Task.Factory.StartNew(() => { return name; });
        }

        public bool Cleanup(string name)
        {
            string command = $" advfirewall firewall delete rule name=\"{name}\"";
            bool flag = false;
            int code = ExecuteCommand(command, Environment.CurrentDirectory, TimeSpan.FromSeconds(20));
            if (code == 0)
                flag = true;

            return flag;
        }

        private int ExecuteCommand(string command, string workingDirectory, TimeSpan timeout)
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
