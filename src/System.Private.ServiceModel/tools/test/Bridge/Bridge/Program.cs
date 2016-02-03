// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using WcfTestBridgeCommon;

namespace Bridge
{
    internal class Program
    {
        // Exit codes returned by this executable
        private const int Bridge_ExitCode_Success = 0;
        private const int Bridge_ExitCode_Failure = -1;
        // For the -require options 1 means the Bridge was already running, and 0 means it was started
        private const int Bridge_ExitCode_AlreadyRunning = 1;


        internal const bool DefaultAllowRemote = false;
        internal const string DefaultRemoteAddresses = "LocalSubnet";
        internal const string BridgeControllerEndpoint = "Bridge";
        internal static readonly int DefaultRequireBridgeTimeoutSeconds = 60;

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(BridgeUnhandledExceptionHandler);
            CommandLineArguments commandLineArgs = new CommandLineArguments(args);

            Console.WriteLine("Bridge.exe was launched with: {0}{1}", 
                              String.Join(" ", args), Environment.NewLine);

            // If asked to ping (not the default), just ping and return an exit code indicating its state
            // Success means it is running, and Failure means it is not.
            if (commandLineArgs.Ping)
            {
                string errorMessage = null;
                Dictionary<string, string> properties;
                if (PingBridge(commandLineArgs.BridgeConfiguration.BridgeHost,
                               commandLineArgs.BridgeConfiguration.BridgePort,
                               out errorMessage,
                               out properties))
                {
                    Console.WriteLine("The Bridge is running.");
                    if (properties != null)
                    {
                        StringBuilder messageBuilder = new StringBuilder();
                        messageBuilder.AppendLine("Current Bridge configuration is:");
                        foreach (var pair in properties)
                        {
                            messageBuilder.AppendLine(String.Format("  {0} = {1}", pair.Key, pair.Value));
                        }
                        Console.WriteLine(messageBuilder.ToString());
                    }
                    Environment.Exit(Bridge_ExitCode_Success);
                }
                else
                {
                    Console.WriteLine("The Bridge is not running: {0}", errorMessage);
                    Environment.Exit(Bridge_ExitCode_Failure);
                }
            }

            if (commandLineArgs.StopIfLocal)
            {
                StopBridgeIfLocal(commandLineArgs);
                Environment.Exit(Bridge_ExitCode_Success);
            }
            if (commandLineArgs.Stop)
            {
                StopBridge(commandLineArgs);
                Environment.Exit(Bridge_ExitCode_Success);
            }
            if (commandLineArgs.Reset)
            {
                ResetBridge(commandLineArgs);
                Environment.Exit(Bridge_ExitCode_Success);
            }
            if (commandLineArgs.RequireBridgeTimeoutSeconds.HasValue)
            {
                int exitCode = RequireBridge(commandLineArgs);
                Environment.Exit(exitCode);
            }

            // Default action is starting the Bridge
            StartBridge(commandLineArgs);
        }

        // Issues a GET request to the Bridge to determine whether it is alive.
        // A return of 'true' means the Bridge is healthy.  A return of 'false'
        // indicates the Bridge is not healthy, and 'errorMessage' describes the problem.
        // A healthy response will also populate properties with the current Bridge configuration.
        private static bool PingBridge(string host, int port, out string errorMessage, out Dictionary<string, string> properties)
        {
            errorMessage = null;
            properties = null;

            string bridgeUrl = String.Format("http://{0}:{1}/{2}", host, port, BridgeControllerEndpoint);

            using (HttpClient httpClient = new HttpClient())
            {
                Console.WriteLine("Pinging the Bridge by issuing GET request to {0}", bridgeUrl);
                try
                {
                    var response = httpClient.GetAsync(bridgeUrl).GetAwaiter().GetResult();
                    if (!response.IsSuccessStatusCode)
                    {
                        errorMessage = String.Format("{0}Bridge returned unexpected status code='{1}', reason='{2}'",
                                                    Environment.NewLine, response.StatusCode, response.ReasonPhrase);
                        if (response.Content != null)
                        {
                            string contentAsString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            if (contentAsString.Length > 1000)
                            {
                                contentAsString = contentAsString.Substring(0, 999) + "...";
                            }
                            errorMessage = String.Format("{0}, content:{1}{2}",
                                                    errorMessage, Environment.NewLine, contentAsString);

                        }
                        return false;
                    }
                    else if (response.Content != null)
                    {
                        string contentAsString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        properties = JsonSerializer.DeserializeDictionary(contentAsString);
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return false;
                }
            }

            return true;
        }

        private static void StopBridgeIfLocal(CommandLineArguments commandLineArgs)
        {
            if (IsBridgeHostLocal(commandLineArgs.BridgeConfiguration))
            {
                StopBridge(commandLineArgs);
            }
            else
            {
                Console.WriteLine("The Bridge on host {0} is not running locally and will not be stopped.",
                                    commandLineArgs.BridgeConfiguration.BridgeHost);
                Console.WriteLine("Use 'Bridge.exe /stop' to stop a Bridge on another machine.");
            }
        }

        private static int StopBridge(CommandLineArguments commandLineArgs)
        {
            string errorMessage = null;
            Dictionary<string, string> properties;

            if (!PingBridge(commandLineArgs.BridgeConfiguration.BridgeHost,
                                           commandLineArgs.BridgeConfiguration.BridgePort,
                                           out errorMessage,
                                           out properties))
            {
                Console.WriteLine("The Bridge is not running: {0}", errorMessage);
                return Bridge_ExitCode_Success;
            }

            string bridgeUrl = String.Format("http://{0}:{1}/{2}", 
                                             commandLineArgs.BridgeConfiguration.BridgeHost, 
                                             commandLineArgs.BridgeConfiguration.BridgePort,
                                             BridgeControllerEndpoint);
            string problem = null;

            // We stop the Bridge using a DELETE request.
            // If the Bridge is running on localhost, it will be running
            // in a different process on this machine.
            using (HttpClient httpClient = new HttpClient())
            {
                Console.WriteLine("Stopping the Bridge by issuing DELETE request to {0}", bridgeUrl);
                try
                {
                    var response = httpClient.DeleteAsync(bridgeUrl).GetAwaiter().GetResult();
                    if (!response.IsSuccessStatusCode)
                    {
                        problem = String.Format("{0}Bridge returned unexpected status code='{1}', reason='{2}'",
                                                    Environment.NewLine, response.StatusCode, response.ReasonPhrase);
                        if (response.Content != null)
                        {
                            string contentAsString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            if (contentAsString.Length > 1000)
                            {
                                contentAsString = contentAsString.Substring(0, 999) + "...";
                            }
                            problem = String.Format("{0}, content:{1}{2}",
                                                    problem, Environment.NewLine, contentAsString);
                        }
                    }
                }
                catch (Exception ex)
                {
                    problem = ex.ToString();
                }
            }

            if (problem != null)
            {
                Console.WriteLine("A problem was encountered stopping the Bridge:{0}{1}",
                                    Environment.NewLine, problem);
                Console.WriteLine("Forcing local resource cleanup...");
                BridgeController.StopBridgeProcess(Bridge_ExitCode_Success);
            }

            // A successfull DELETE will have cleaned up all firewall rules,
            // certificates, etc.  So when using localhost, this cleanup will
            // be redundant and harmless.  When the Bridge is running remotely,
            // this cleanup will remove all firewall rules and certificates we
            // installed on the current machine to talk with that Bridge.
            BridgeController.StopBridgeProcess(Bridge_ExitCode_Success);

            // The process will be torn down before executing this return
            return Bridge_ExitCode_Success;
        }

        // Asks the Bridge to release all its resources but continue running
        private static void ResetBridge(CommandLineArguments commandLineArgs)
        {
            string errorMessage = null;
            Dictionary<string, string> properties;

            if (!PingBridge(commandLineArgs.BridgeConfiguration.BridgeHost,
                                           commandLineArgs.BridgeConfiguration.BridgePort,
                                           out errorMessage,
                                           out properties))
            {
                Console.WriteLine("The Bridge is not running: {0}", errorMessage);
                Environment.Exit(Bridge_ExitCode_Success);
            }

            string bridgeUrl = String.Format("http://{0}:{1}/Resource", commandLineArgs.BridgeConfiguration.BridgeHost, commandLineArgs.BridgeConfiguration.BridgePort);
            string problem = null;

            Console.WriteLine("Resetting the Bridge by sending DELETE request to {0}", bridgeUrl);

            // We reset the Bridge using a DELETE request to the /resource endpoint.
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    var response = httpClient.DeleteAsync(bridgeUrl).GetAwaiter().GetResult();
                    if (!response.IsSuccessStatusCode)
                    {
                        problem = String.Format("{0}Bridge returned unexpected status code='{1}', reason='{2}'",
                                                    Environment.NewLine, response.StatusCode, response.ReasonPhrase);
                        if (response.Content != null)
                        {
                            string contentAsString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            if (contentAsString.Length > 1000)
                            {
                                contentAsString = contentAsString.Substring(0, 999) + "...";
                            }
                            problem = String.Format("{0}, content:{1}{2}",
                                                    problem, Environment.NewLine, contentAsString);
                        }
                    }
                }
                catch (Exception ex)
                {
                    problem = ex.ToString();
                }
            }

            if (problem != null)
            {
                Console.WriteLine("A problem was encountered resetting the Bridge:{0}{1}",
                                    Environment.NewLine, problem);
                Console.WriteLine("Forcing local resource cleanup...");
            }

            // A successfull DELETE will have cleaned up all firewall rules,
            // certificates, etc.  So when using localhost, this cleanup will
            // be redundant and harmless.  When the Bridge is running remotely,
            // this cleanup will remove all firewall rules and certificates we
            // installed on the current machine to talk with that Bridge.
            BridgeController.ReleaseAllResources(force: false);
        }

        // Checks whether the Bridge is running and starts it if necessary.
        // Returns the exit code to return to the calling script
        private static int RequireBridge(CommandLineArguments commandLineArgs)
        {
            string errorMessage = null;
            Dictionary<string, string> properties;

            // In a test situation, the Bridge may have been launched and is still in the
            // process of starting.  A successful ping means it is healthy, but we will retry
            // if the ping fails and we detect the Bridge process is running.
            int nRetries = 0;
            DateTime startTime = DateTime.Now;
            while (((DateTime.Now - startTime).TotalSeconds) < commandLineArgs.RequireBridgeTimeoutSeconds)
            {
                // Ping first in case the Bridge is running already or on another machine
                if (PingBridge(commandLineArgs.BridgeConfiguration.BridgeHost,
                                               commandLineArgs.BridgeConfiguration.BridgePort,
                                               out errorMessage,
                                               out properties))
                {
                    Console.WriteLine("The Bridge is already running.");
                    return Bridge_ExitCode_AlreadyRunning;
                }

                if (nRetries++ > 0)
                {
                    Console.WriteLine("  -- the ping failure response was: {0}", errorMessage);
                }

                // Ping failed.  If the Bridge process is running, try again until it responds.
                // If the Bridge process is not even running, no need to keep retrying.
                // We look for any other Bridge.exe other than this currently running process.
                int currentPid = Process.GetCurrentProcess().Id;
                bool bridgeRunning = Process.GetProcessesByName("Bridge").Any(p => p.Id != currentPid);
                if (!bridgeRunning)
                {
                    Console.WriteLine("The Bridge.exe process is not running so it will be started now.");
                    break;
                }

                // In the initial ping to see if it is already running, allow a small delay
                // in the case where we see a Bridge.exe is running but not yet responding.
                Thread.Sleep(1000);
            }

            Process bridgeProcess = StartBridgeInNewProcess(commandLineArgs);

            nRetries = 0;
            while (((DateTime.Now - startTime).TotalSeconds) < commandLineArgs.RequireBridgeTimeoutSeconds)
            {
                // Sleep each iteration and before the 1st ping so we report get
                // spurious failures as the new process is being launched.
                Thread.Sleep(2000);

                if (bridgeProcess.HasExited)
                {
                    int exitCode = bridgeProcess.ExitCode;
                    Console.WriteLine("The Bridge process terminated unexpectedly with exit code {0}", bridgeProcess.ExitCode);
                    return Bridge_ExitCode_Failure;
                }

                if (PingBridge(commandLineArgs.BridgeConfiguration.BridgeHost,
                               commandLineArgs.BridgeConfiguration.BridgePort,
                               out errorMessage,
                               out properties))
                {
                    Console.WriteLine("The Bridge has been started successfully.");
                    return Bridge_ExitCode_Success;
                }

                if (nRetries++ > 0)
                {
                    Console.WriteLine("  -- the ping failure response was: {0}", errorMessage);
                }
            }

            Console.WriteLine("The Bridge did not respond in the required {0} seconds.", 
                              commandLineArgs.RequireBridgeTimeoutSeconds);
            return Bridge_ExitCode_Failure;
        }


        // Starts the Bridge locally in a new process
        private static Process StartBridgeInNewProcess(CommandLineArguments commandLineArgs)
        {
            // Pass through the original command line arguments except for the -require switch.
            // Otherwise we would recursively start this same logic in a new process.
            List<string> originalArgList = new List<string>(commandLineArgs.OriginalArgs);
            for (int i = originalArgList.Count-1; i >= 0; --i)
            {
                string[] argParts = (originalArgList[i].Substring(1)).Split(':');
                if (argParts.Length > 0 && String.Equals(argParts[0], "require", StringComparison.OrdinalIgnoreCase))
                {
                    originalArgList.RemoveAt(i);
                }
            }

            string newArguments = String.Join(" ", originalArgList);
            string processExe = typeof(Program).Assembly.Location;
            ProcessStartInfo startInfo = new ProcessStartInfo(processExe, newArguments);
            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;

            Console.WriteLine("Starting new process using {0} {1} ...", processExe, newArguments);
            Process process = Process.Start(startInfo);
            Console.WriteLine("The new Bridge process has been started as PID {0}", process.Id);
            return process;
        }

        // Starts the Bridge locally if it is not already running.
        private static void StartBridge(CommandLineArguments commandLineArgs)
        {
            string errorMessage = null;
            Dictionary<string, string> properties;

            if (PingBridge(commandLineArgs.BridgeConfiguration.BridgeHost,
                                           commandLineArgs.BridgeConfiguration.BridgePort,
                                           out errorMessage,
                                           out properties))
            {
                Console.WriteLine("The Bridge is already running.");
                Environment.Exit(Bridge_ExitCode_Success);
            }

            // The host is not local so we cannot start the Bridge
            if (!IsBridgeHostLocal(commandLineArgs.BridgeConfiguration))
            {
                Console.WriteLine("The Bridge cannot be started from this machine on {0}",
                                  commandLineArgs.BridgeConfiguration.BridgeHost);
                Environment.Exit(Bridge_ExitCode_Failure);
            }

            string resourceFolder = commandLineArgs.BridgeConfiguration.BridgeResourceFolder;
            if (String.IsNullOrWhiteSpace(resourceFolder))
            {
                Console.WriteLine("Starting the Bridge requires the BridgeResourceFolder to be specified.");
                Console.WriteLine("Use either -BridgeResourceFolder:folderName or set it as an environment variable.");
                Environment.Exit(Bridge_ExitCode_Failure);
            }

            resourceFolder = Path.GetFullPath(resourceFolder);
            if (!Directory.Exists(resourceFolder))
            {
                Console.WriteLine("The specified BridgeResourceFolder '{0}' does not exist.");
                Environment.Exit(Bridge_ExitCode_Failure);
            }
            commandLineArgs.BridgeConfiguration.BridgeResourceFolder = resourceFolder;

            int port = commandLineArgs.BridgeConfiguration.BridgePort;

            string hostFormatString = "http://{0}:{1}";
            string owinAddress = String.Format(hostFormatString, commandLineArgs.AllowRemote ? "+" : "localhost", port);
            string visibleHost = (commandLineArgs.AllowRemote) ? Environment.MachineName : "localhost";
            string visibleAddress = String.Format(hostFormatString, visibleHost, port);

            // Configure the remote addresses the firewall rules will accept.
            // If remote access is not allowed, specifically disallow remote addresses
            PortManager.RemoteAddresses = commandLineArgs.AllowRemote ? commandLineArgs.RemoteAddresses : String.Empty;

            // Initialize the BridgeConfiguration from command line.
            ConfigController.BridgeConfiguration = commandLineArgs.BridgeConfiguration;
            ConfigController.BridgeConfiguration.BridgeHost = visibleHost;

            // Remove any pre-existing firewall rules or certificates the Bridge
            // may have added in past runs.  We normally clean them up on exit but
            // it is possible a prior Bridge process was terminated prematurely.
            BridgeController.ReleaseAllResources(force: false);

            Console.WriteLine("Starting the Bridge at {0}", visibleAddress);
            OwinSelfhostStartup.Startup(owinAddress);

            // Now test whether the Bridge is running.  Failure cleans up
            // all resources and terminates the process.
            if (!PingBridge(visibleHost, port, out errorMessage, out properties))
            {
                Console.WriteLine("The Bridge failed to start or is not responding: {0}", errorMessage);
                BridgeController.StopBridgeProcess(1);
            }

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("The Bridge is running");
                Console.WriteLine("    Listening at {0}/{1}", 
                                    visibleAddress, BridgeControllerEndpoint);

                if (commandLineArgs.AllowRemote)
                {
                    Console.WriteLine("    Remote access is allowed from '{0}'", commandLineArgs.RemoteAddresses);
                }
                else
                {
                    Console.WriteLine("    Remote access is disabled.");
                }

                Console.WriteLine("    Commands:");
                Console.WriteLine("    \"cls\" to clear the screen");
                Console.WriteLine("    \"exit\" to stop the Bridge");
                Console.WriteLine(); 
                Console.Write("Bridge> ");

                string answer = Console.ReadLine();
                if (string.Equals(answer, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                else if (string.Equals(answer, "cls", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Clear(); 
                }

                // Key presses to the Bridge restart the idle timeout
                IdleTimeoutHandler.RestartTimer();
            }

            BridgeController.StopBridgeProcess(0);
        }

        // Returns 'true' if the BridgeConfiguration describes a Bridge that
        // would run locally.
        private static bool IsBridgeHostLocal(BridgeConfiguration configuration)
        {
            if (String.Equals("localhost", configuration.BridgeHost, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (String.Equals(Environment.MachineName, configuration.BridgeHost, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private static void BridgeUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception) args.ExceptionObject;
            Console.WriteLine("*** Unhandled exception ***" + Environment.NewLine);
            Console.WriteLine(e + Environment.NewLine);
            Console.WriteLine("***                     ***" + Environment.NewLine);
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        class CommandLineArguments
        {


            public CommandLineArguments(string[] args)
            {
                OriginalArgs = args;
                AllowRemote = DefaultAllowRemote;
                RemoteAddresses = DefaultRemoteAddresses;
                Ping = false;

                bool success = Parse(args);
                if (!success)
                {
                    ShowUsage();
                    Environment.Exit(Bridge_ExitCode_Failure);
                }
            }

            public string[] OriginalArgs { get; private set; }
            public BridgeConfiguration BridgeConfiguration { get; private set; }
            public bool AllowRemote { get; private set; }
            public string RemoteAddresses { get; private set; }
            public bool Ping { get; private set; }
            public bool Stop { get; private set; }
            public bool StopIfLocal { get; private set; }
            public bool Reset { get; private set; }
            public int? RequireBridgeTimeoutSeconds { get; private set; }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder()
                    .AppendLine(String.Format("  -allowRemote = {0}", AllowRemote))
                    .AppendLine(String.Format("  -remoteAddresses = {0}", RemoteAddresses))
                    .AppendLine(String.Format("  -ping = {0}", Ping))
                    .AppendLine(String.Format("  -stop = {0}", Stop))
                    .AppendLine(String.Format("  -stopIfLocal = {0}", StopIfLocal))
                    .AppendLine(String.Format("  -reset = {0}", Reset))
                    .AppendLine(String.Format("  -require = {0}", RequireBridgeTimeoutSeconds))
                    .AppendLine(String.Format("BridgeConfiguration is:{0}{1}", 
                                                Environment.NewLine, BridgeConfiguration.ToString()));
                return sb.ToString();
            }

            private bool Parse(string[] args)
            {
                // Build a dictionary of all command line arguments.
                // This allows us to initialize BridgeConfiguration from it.
                // Precedence of values in the BridgeConfiguration is this:
                //   - Lowest precedence is the BridgeConfiguration ctor defaults
                //   - Next precedence is any value found in a specified configuration file
                //   - Next precedence is environment variables
                //   - Highest precedence is a BridgeConfiguration value explicitly set on the command line

                Dictionary<string, string> argumentDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                foreach (string arg in args)
                {
                    if (!arg.StartsWith("/") && !arg.StartsWith("-"))
                    {
                        return false;
                    }

                    // Cannot use split because some argument values could contain colons
                    int index = arg.IndexOf(':');
                    string argName = (index < 0) ? arg.Substring(1) : arg.Substring(1, index - 1);
                    string argValue = (index < 0) ? String.Empty : arg.Substring(index+1);

                    if (String.Equals(argName, "?", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }

                    argumentDictionary[argName] = argValue;
                }

                BridgeConfiguration = new BridgeConfiguration();

                // If the user specified a configuration file, deserialize it as json
                // and treat each name-value pair as if it had been on the command line.
                // But options explicitly on the command line take precedence over these file options.
                string argumentValue;
                if (argumentDictionary.TryGetValue("bridgeConfig", out argumentValue))
                {
                    if (!File.Exists(argumentValue))
                    {
                        Console.WriteLine("The configuration file '{0}' does not exist.");
                        return false;
                    }

                    // Read the configuration file as json and deserialize it
                    string configurationAsJson = File.ReadAllText(argumentValue);
                    Dictionary<string, string> deserializedConfig = null;

                    try
                    {
                        deserializedConfig = JsonSerializer.DeserializeDictionary(configurationAsJson);
                    }
                    catch (Exception ex)
                    {
                        // Catch all exceptions because any will cause
                        // this application to terminate.
                        Console.WriteLine("Error deserializing {0} : {1}",
                                            argumentValue, ex.Message);
                        return false;
                    }

                    // Every name/value pair in the config file not explicitly set on the command line
                    // is treated as if it had been on the command line.
                    foreach (var pair in deserializedConfig)
                    {
                        if (!argumentDictionary.ContainsKey(pair.Key))
                        {
                            argumentDictionary[pair.Key] = pair.Value;
                        }
                    }
                }

                // For every property in the BridgeConfiguration that has not been explicitly
                // specified on the command line or via the config file, check if there is an
                // Environment variable set for it.  If so, use it as if it had been on the command line.
                foreach (string key in BridgeConfiguration.ToDictionary().Keys)
                {
                    // If the property is explicitly on the command line, it has highest precedence
                    if (!argumentDictionary.ContainsKey(key))
                    {
                        // But if it is not explicitly on the command line but 
                        // an environment variable exists for it, it has higher precedence
                        // than defaults or the config file.
                        string environmentVariable = Environment.GetEnvironmentVariable(key);
                        if (!String.IsNullOrWhiteSpace(environmentVariable))
                        {
                            argumentDictionary[key] = environmentVariable;
                        }
                    }
                }

                // Finally, apply all our command line arguments to the BridgeConfiguration,
                // overwriting any values that were the default or came from the optional config file
                BridgeConfiguration = new BridgeConfiguration(BridgeConfiguration, argumentDictionary);

                // Finish parsing the command line arguments that are not part of BridgeConfiguration
                if (argumentDictionary.ContainsKey("allowRemote"))
                {
                    AllowRemote = true;
                }

                if (argumentDictionary.ContainsKey("ping"))
                {
                    Ping = true;
                }

                if (argumentDictionary.ContainsKey("stop"))
                {
                    Stop = true;
                }

                if (argumentDictionary.ContainsKey("stopiflocal"))
                {
                    StopIfLocal = true;
                }

                string remoteAddresses;
                if (argumentDictionary.TryGetValue("remoteAddresses", out remoteAddresses))
                {
                    RemoteAddresses = remoteAddresses;
                }

                if (argumentDictionary.ContainsKey("reset"))
                {
                    Reset = true;
                }

                string requireTimeoutSecondsString = null;
                if (argumentDictionary.TryGetValue("require", out requireTimeoutSecondsString))
                {
                    int requireTimeoutSeconds;
                    if (String.IsNullOrWhiteSpace(requireTimeoutSecondsString))
                    {
                        RequireBridgeTimeoutSeconds = DefaultRequireBridgeTimeoutSeconds;
                    }
                    else if (int.TryParse(requireTimeoutSecondsString, out requireTimeoutSeconds))
                    {
                        RequireBridgeTimeoutSeconds = requireTimeoutSeconds;
                    }
                    else
                    {
                        Console.WriteLine("The value \"{0}\" is not a valid integer number of seconds.", requireTimeoutSeconds);
                        return false;
                    }
                }

                return true;
            }

            private void ShowUsage()
            {
                Console.WriteLine("Usage is: Bridge.exe -option");
                Console.WriteLine("  Options are:");
                Console.WriteLine("   -ping               Pings the Bridge to check if it is running.");
                Console.WriteLine("                       Exit code 0 indicates it is running.");
                Console.WriteLine("   -require:{maxSeconds} Checks whether the Bridge is running and starts it in a new process if needed.");
                Console.WriteLine("                       Wait up to maxSeconds for the Bridge to start (default {0}).", DefaultRequireBridgeTimeoutSeconds);
                Console.WriteLine("                       Exit code 0 indicates it is running.");
                Console.WriteLine("   -stop               Stops the Bridge if it is running.");
                Console.WriteLine("   -stopIfLocal        Stops the Bridge only if it is running locally.");
                Console.WriteLine("   -allowRemote        If starting the Bridge, allows access from other than localHost (default is localhost only).");
                Console.WriteLine("   -reset              Releases all Bridge resources without stopping the Bridge.");
                Console.WriteLine("   -remoteAddresses:addresses  If starting the Bridge, comma-separated list of addresses firewall rules will accept.");
                Console.WriteLine("                       (default is 'LocalSubnet')");
                Console.WriteLine("   -BridgeConfig:file  Treat file as json name/value pairs to initialize any or all other options.");

                StringBuilder helpBuilder = new StringBuilder().AppendLine(" or any of these known Bridge properties:");
                foreach (string propertyName in new BridgeConfiguration().ToDictionary().Keys)
                {
                    helpBuilder.AppendLine(String.Format("   -{0}:value", propertyName));
                }
                Console.WriteLine(helpBuilder.ToString());
                Console.WriteLine();
                Console.WriteLine("If no other option is specified, and the Bridge is not already running, it will be started.");
                Console.WriteLine("Whenever the Bridge is started, it will block the current process until it is stopped.");
                Console.WriteLine("If the -require option is specified, the Bridge will be started in a new process,");
                Console.WriteLine("and the current process will resume as soon as the Bridge is confirmed running (or the wait times out).");
                Console.WriteLine("Examples");
                Console.WriteLine("  \"Bridge.exe\" -- starts the Bridge and allows access from localhost only.");
                Console.WriteLine("  \"Bridge.exe -allowRemote\" -- starts the Bridge and allows access from the local subnet only.");
                Console.WriteLine("  \"Bridge.exe -allowRemote -remoteAddresses:172.30.168.102\" -- starts the Bridge and allows access from one IP address only.");
                Console.WriteLine("  \"Bridge.exe -require\" -- starts the Bridge in a new process if it is not running and waits up to {0} seconds for it to start.", DefaultRequireBridgeTimeoutSeconds);
                Console.WriteLine("  \"Bridge.exe -reset\" -- if the Bridge is running, tell it to free all resources but continue running.", DefaultRequireBridgeTimeoutSeconds);
                Console.WriteLine("  \"Bridge.exe -stop\" -- if the Bridge is running, tell it to free all resources and terminate.", DefaultRequireBridgeTimeoutSeconds);
            }
        }
    }
}
