// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using WcfTestBridgeCommon;

namespace Bridge
{
    internal class Program
    {
        private const int DefaultPortNumber = 44283;

        private static void Main(string[] args)
        {
            CommandLineArguments commandLineArgs = new CommandLineArguments(args);

            string hostFormatString = "http://{0}:{1}";
            string owinAddress = String.Format(hostFormatString, commandLineArgs.AllowRemote ? "+" : "localhost", commandLineArgs.Port);
            string visibleHost = (commandLineArgs.AllowRemote) ? Environment.MachineName : "localhost";
            string visibleAddress = String.Format(hostFormatString, visibleHost, commandLineArgs.Port);

            PortManager.OpenPortInFirewall(commandLineArgs.Port);

            OwinSelfhostStartup.Startup(owinAddress);

            Console.WriteLine("The Bridge is listening at {0} with remote access {1}", visibleAddress, commandLineArgs.AllowRemote ? "enabled" : "disabled");

            Test(visibleHost, commandLineArgs.Port);

            while (true)
            {
                Console.WriteLine("Type \"exit\" to stop the Bridge.");
                string answer = Console.ReadLine();
                if (String.Equals(answer, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }

            PortManager.CloseAllOpenedPortsInFireWall();
        }

        [Conditional("DEBUG")]
        private static void Test(string hostName, int portNumber)
        {
            Console.WriteLine("Self-testing the Bridge...");
            string commandLine = String.Format("-ExecutionPolicy Bypass -File {0} -portNumber {1} -hostName {2}", 
                                            Path.GetFullPath("ensureBridge.ps1"), portNumber, hostName);
            ProcessStartInfo procStartInfo = new ProcessStartInfo("powershell.exe", commandLine);
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

        class CommandLineArguments
        {
            private const int DefaultPortNumber = 44283;
            private const bool DefaultAllowRemote = false;

            public CommandLineArguments(string[] args)
            {
                Port = DefaultPortNumber;
                AllowRemote = DefaultAllowRemote;
                bool success = Parse(args);
                if (!success)
                {
                    ShowUsage();
                    Environment.Exit(-1);
                }
            }

            public int Port { get; private set; }
            public bool AllowRemote { get; private set; }

            private bool Parse(string[] args)
            {
                foreach (string arg in args)
                {
                    if (!arg.StartsWith("/") && !arg.StartsWith("-"))
                    {
                        return false;
                    }
                    string[] argAndValue = arg.Substring(1).Split(':');
                    if (argAndValue.Length == 0)
                    {
                        return false;
                    }
                    if (String.Equals(argAndValue[0], "port", StringComparison.OrdinalIgnoreCase))
                    {
                        if (argAndValue.Length < 2)
                        {
                            return false;
                        }
                        int port = 0;
                        if (!int.TryParse(argAndValue[1], out port))
                        {
                            return false;
                        }
                        Port = port;
                    }
                    else if (String.Equals(argAndValue[0], "allowRemote", StringComparison.OrdinalIgnoreCase))
                    {
                        AllowRemote = true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }

            private void ShowUsage()
            {
                Console.WriteLine("Starts a new instance of the Bridge.  Usage is:");
                Console.WriteLine("Bridge.exe [/port:nnn] [/allowRemote]");
                Console.WriteLine("   /port:nnn     Listening port for the bridge (default is {0}", DefaultPortNumber);
                Console.WriteLine("   /allowRemote  If specified, allows access from other than localHost (default is localhost only)");
            }
        }
    }
}
