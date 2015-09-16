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
using WcfTestBridgeCommon;

namespace Bridge
{
    internal class Program
    {
        internal const bool DefaultAllowRemote = false;
        internal const string DefaultRemoteAddresses = "LocalSubnet";

        private static void Main(string[] args)
        {
            CommandLineArguments commandLineArgs = new CommandLineArguments(args);

            Console.WriteLine("Bridge.exe was launched with:{0}{1}", 
                              Environment.NewLine, commandLineArgs.ToString());

            // If asked to ping (not the default), just ping and return an exit code indicating its state
            if (commandLineArgs.Ping)
            {
                string errorMessage = null;
                if (PingBridge(commandLineArgs.BridgeConfiguration.BridgeHost,
                               commandLineArgs.BridgeConfiguration.BridgePort,
                               out errorMessage))
                {
                    Console.WriteLine("The Bridge is running.");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("The Bridge is not running: {0}", errorMessage);
                    Environment.Exit(1);
                }
            }
            if (commandLineArgs.StopIfLocal)
            {
                StopBridgeIfLocal(commandLineArgs);
                Environment.Exit(0);
            }
            if (commandLineArgs.Stop)
            {
                StopBridge(commandLineArgs);
                Environment.Exit(0);
            }
            if (commandLineArgs.Reset)
            {
                ResetBridge(commandLineArgs);
                Environment.Exit(0);
            }

            // Default action is starting the Bridge
            StartBridge(commandLineArgs);
        }

        // Issues a GET request to the Bridge to determine whether it is alive.
        // A return of 'true' means the Bridge is healthy.  A return of 'false'
        // indicates the Bridge is not healthy, and 'errorMessage' describes the problem.
        private static bool PingBridge(string host, int port, out string errorMessage)
        {
            errorMessage = null;

            string bridgeUrl = String.Format("http://{0}:{1}/Bridge", host, port);

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

        private static void StopBridge(CommandLineArguments commandLineArgs)
        {
            string errorMessage = null;

            if (!PingBridge(commandLineArgs.BridgeConfiguration.BridgeHost,
                                           commandLineArgs.BridgeConfiguration.BridgePort,
                                           out errorMessage))
            {
                Console.WriteLine("The Bridge is not running: {0}", errorMessage);
                Environment.Exit(0);
            }

            string bridgeUrl = String.Format("http://{0}:{1}/Bridge", commandLineArgs.BridgeConfiguration.BridgeHost, commandLineArgs.BridgeConfiguration.BridgePort);
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
                BridgeController.StopBridgeProcess(1);
            }

            // A successfull DELETE will have cleaned up all firewall rules,
            // certificates, etc.  So when using localhost, this cleanup will
            // be redundant and harmless.  When the Bridge is running remotely,
            // this cleanup will remove all firewall rules and certificates we
            // installed on the current machine to talk with that Bridge.
            BridgeController.StopBridgeProcess(0);
        }

        // Asks the Bridge to release all its resources but continue running
        private static void ResetBridge(CommandLineArguments commandLineArgs)
        {
            string errorMessage = null;

            if (!PingBridge(commandLineArgs.BridgeConfiguration.BridgeHost,
                                           commandLineArgs.BridgeConfiguration.BridgePort,
                                           out errorMessage))
            {
                Console.WriteLine("The Bridge is not running: {0}", errorMessage);
                Environment.Exit(0);
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


        // Starts the Bridge locally if it is not already running.
        private static void StartBridge(CommandLineArguments commandLineArgs)
        {
            string errorMessage = null;

            if (PingBridge(commandLineArgs.BridgeConfiguration.BridgeHost,
                                           commandLineArgs.BridgeConfiguration.BridgePort,
                                           out errorMessage))
            {
                Console.WriteLine("The Bridge is already running.");
                Environment.Exit(0);
            }

            // The host is not local so we cannot start the Bridge
            if (!IsBridgeHostLocal(commandLineArgs.BridgeConfiguration))
            {
                Console.WriteLine("The Bridge cannot be started from this machine on {0}",
                                  commandLineArgs.BridgeConfiguration.BridgeHost);
                Environment.Exit(1);
            }

            string resourceFolder = commandLineArgs.BridgeConfiguration.BridgeResourceFolder;
            if (String.IsNullOrWhiteSpace(resourceFolder))
            {
                Console.WriteLine("Starting the Bridge requires the BridgeResourceFolder to be specified.");
                Console.WriteLine("Use either -BridgeResourceFolder:folderName or set it as an environment variable.");
                Environment.Exit(1);
            }

            resourceFolder = Path.GetFullPath(resourceFolder);
            if (!Directory.Exists(resourceFolder))
            {
                Console.WriteLine("The specified BridgeResourceFolder '{0}' does not exist.");
                Environment.Exit(1);
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
            if (!PingBridge(visibleHost, port, out errorMessage))
            {
                Console.WriteLine("The Bridge failed to start or is not responding: {0}", errorMessage);
                BridgeController.StopBridgeProcess(1);
            }

            while (true)
            {
                Console.WriteLine("The Bridge is running and listening at {0}", visibleAddress);
                if (commandLineArgs.AllowRemote)
                {
                    Console.WriteLine("Remote access is allowed from '{0}'", commandLineArgs.RemoteAddresses);
                }
                else
                {
                    Console.WriteLine("Remote access is disabled.");
                }

                Console.WriteLine("Type \"exit\" to stop the Bridge.");
                string answer = Console.ReadLine();
                if (String.Equals(answer, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
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

        class CommandLineArguments
        {
            public CommandLineArguments(string[] args)
            {
                AllowRemote = DefaultAllowRemote;
                RemoteAddresses = DefaultRemoteAddresses;
                Ping = false;

                bool success = Parse(args);
                if (!success)
                {
                    ShowUsage();
                    Environment.Exit(-1);
                }
            }

            public BridgeConfiguration BridgeConfiguration { get; private set; }
            public bool AllowRemote { get; private set; }
            public string RemoteAddresses { get; private set; }
            public bool Ping { get; private set; }
            public bool Stop { get; private set; }
            public bool StopIfLocal { get; private set; }
            public bool Reset { get; private set; }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Options are:")
                    .AppendLine(String.Format("  -allowRemote = {0}", AllowRemote))
                    .AppendLine(String.Format("  -remoteAddresses = {0}", RemoteAddresses))
                    .AppendLine(String.Format("  -ping = {0}", Ping))
                    .AppendLine(String.Format("  -stop = {0}", Stop))
                    .AppendLine(String.Format("  -stopIfLocal = {0}", StopIfLocal))
                    .AppendLine(String.Format("  -reset = {0}", Reset))
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

                return true;
            }

            private void ShowUsage()
            {
                Console.WriteLine("Usage is: Bridge.exe [/ping] [/stop] [/stopIfLocal] [/allowRemote] [/remoteAddresses:x,y,z] [/{BridgeProperty}:value");
                Console.WriteLine("   /ping            Pings the Bridge to check if it is running");
                Console.WriteLine("   /stop            Stops the Bridge if it is running");
                Console.WriteLine("   /stopIfLocal     Stops the Bridge if it is running locally");
                Console.WriteLine("   /allowRemote     If starting the Bridge, allows access from other than localHost (default is localhost only)");
                Console.WriteLine("   /reset           Releases all Brige resources without stopping Bridge");
                Console.WriteLine("   /remoteAddresses If starting the Bridge, comma-separated list of addresses firewall rules will accept (default is 'LocalSubnet')");
                Console.WriteLine("   /BridgeConfig:file  Treat file as json name/value pairs to initialize any or all other options");

                string bridgePropertyList = String.Join(Environment.NewLine + "   /", new BridgeConfiguration().ToDictionary().Keys);
                Console.WriteLine("   /{0}", bridgePropertyList);
                Console.WriteLine();
                Console.WriteLine("If no other action is specified, the Bridge will be started unless it is already running.");
            }
        }
    }
}
