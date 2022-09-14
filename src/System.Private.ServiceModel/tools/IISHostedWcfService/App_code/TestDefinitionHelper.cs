// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace WcfService
{
    internal class TestDefinitionHelper
    {
        private static IDictionary<ServiceSchema, string> s_baseAddresses = null;
        private const int DefaultHttpPort = 8081;
        private const int DefaultHttpsPort = 44285;
        private const int DefaultTcpPort = 809;
        private const int DefaultWebSocketPort = 8083;
        private const int DefaultWebSocketSPort = 8084;

        private static IDictionary<ServiceSchema, string> BaseAddresses
        {
            get
            {
                if (s_baseAddresses == null)
                {
                    IDictionary<ServiceSchema, string> dict = new Dictionary<ServiceSchema, string>();
                    var httpPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("httpPort")) ? DefaultHttpPort : int.Parse(Environment.GetEnvironmentVariable("httpPort"));
                    dict[ServiceSchema.HTTP] = string.Format(@"http://localhost:{0}", httpPort);
                    var httpsPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("httpsPort")) ? DefaultHttpsPort : int.Parse(Environment.GetEnvironmentVariable("httpsPort"));
                    dict[ServiceSchema.HTTPS]= string.Format(@"https://localhost:{0}", httpsPort);
                    var tcpPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("tcpPort")) ? DefaultTcpPort : int.Parse(Environment.GetEnvironmentVariable("tcpPort"));
                    dict[ServiceSchema.NETTCP] = string.Format(@"net.tcp://localhost:{0}", tcpPort);
                    var websocketPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("websocketPort")) ? DefaultWebSocketPort : int.Parse(Environment.GetEnvironmentVariable("websocketPort"));
                    dict[ServiceSchema.WS] = string.Format(@"http://localhost:{0}", websocketPort);
                    var websocketsPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("websocketsPort")) ? DefaultWebSocketSPort : int.Parse(Environment.GetEnvironmentVariable("websocketsPort"));
                    dict[ServiceSchema.WSS] = string.Format(@"https://localhost:{0}", websocketsPort);
                    s_baseAddresses = dict;
                    dict[ServiceSchema.NETPIPE] = @"net.pipe://localhost";
                    Console.WriteLine("Using base addresses:");
                    foreach(var ba in dict.Values)
                    {
                        Console.WriteLine("\t" + ba);
                    }
                }

                return s_baseAddresses;
            }
        }

        internal static void StartHosts()
        {
            foreach (var sht in GetAttributedServiceHostTypes())
            {
                var serviceBaseAddresses = new List<Uri>();
                bool success = true;
                foreach(TestServiceDefinitionAttribute attr in sht.GetCustomAttributes(typeof(TestServiceDefinitionAttribute), false))
                {
                    Uri serviceBaseAddress = null;
                    try
                    {
                        foreach (Enum schema in Enum.GetValues(typeof(ServiceSchema)))
                        {
                            if (attr.Schema.HasFlag(schema))
                            {
                                serviceBaseAddress = new Uri(string.Format("{0}/{1}", BaseAddresses[(ServiceSchema)schema], attr.BasePath));
                                serviceBaseAddresses.Add(serviceBaseAddress);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ConsoleColor bg = Console.BackgroundColor;
                        ConsoleColor fg = Console.ForegroundColor;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Problem creating base address for servicehost type " + sht.Name + " with schema " + attr.Schema + " and address " + serviceBaseAddress == null ? string.Empty : serviceBaseAddress.ToString());
                        Console.WriteLine(e);
                        Console.BackgroundColor = bg;
                        Console.ForegroundColor = fg;
                        success = false;
                        break;
                    }
                }

                try
                {
                    if (success)
                    {
                        var serviceHost = (ServiceHostBase)Activator.CreateInstance(sht, serviceBaseAddresses.ToArray());
                        serviceHost.Open();
                        Console.Write("  {0} at ", sht.Name);
                        bool first = true;
                        foreach (var endpoint in serviceHost.Description.Endpoints)
                        {
                            if (endpoint.IsSystemEndpoint)
                                continue;
                            if(first)
                                first = false;
                            else
                                Console.Write(", ");

                            Console.Write(endpoint.Address);
                        }
                        Console.WriteLine();
                    }
                }
                catch (Exception e)
                {
                    ConsoleColor bg = Console.BackgroundColor;
                    ConsoleColor fg = Console.ForegroundColor;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Problem starting servicehost type " + sht.Name);
                    Console.WriteLine(e);
                    Console.BackgroundColor = bg;
                    Console.ForegroundColor = fg;
                }
            }
        }

        internal static IEnumerable<Type> GetAttributedServiceHostTypes()
        {
            var allTypes = typeof(TestDefinitionHelper).Assembly.GetTypes();
            var serviceHostTypes = from t in allTypes where (typeof(ServiceHostBase).IsAssignableFrom(t)) select t;
            return from sht in serviceHostTypes where (sht.GetCustomAttributes(typeof(TestServiceDefinitionAttribute), false).Length > 0) select sht;
        }
    }
}
