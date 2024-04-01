﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using CoreWCF.Configuration;
using CoreWCF.Description;
using idunno.Authentication.Basic;
#else
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
#endif

namespace WcfService
{
    internal class TestDefinitionHelper
    {
        private static IDictionary<ServiceSchema, string> s_baseAddresses = null;
        private const int DefaultHttpPort = 8081;
        private const int DefaultHttpsPort = 44285;
        private const int DefaultTcpPort = 8808;
        private const int DefaultWebSocketPort = 8083;
        private const int DefaultWebSocketSPort = 8084;

        internal static IDictionary<ServiceSchema, string> BaseAddresses
        {
            get
            {
                if (s_baseAddresses == null)
                {
                    IDictionary<ServiceSchema, string> dict = new Dictionary<ServiceSchema, string>();
                    var httpPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("httpPort")) ? DefaultHttpPort : int.Parse(Environment.GetEnvironmentVariable("httpPort"));
                    dict[ServiceSchema.HTTP] = string.Format(@"http://localhost:{0}", httpPort);
                    var httpsPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("httpsPort")) ? DefaultHttpsPort : int.Parse(Environment.GetEnvironmentVariable("httpsPort"));
                    dict[ServiceSchema.HTTPS] = string.Format(@"https://localhost:{0}", httpsPort);
                    var tcpPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("tcpPort")) ? DefaultTcpPort : int.Parse(Environment.GetEnvironmentVariable("tcpPort"));
                    dict[ServiceSchema.NETTCP] = string.Format(@"net.tcp://localhost:{0}", tcpPort);
#if NET
                    var websocketPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("websocketPort")) ? DefaultWebSocketPort : int.Parse(Environment.GetEnvironmentVariable("websocketPort"));
                    dict[ServiceSchema.WS] = string.Format(@"ws://localhost:{0}", websocketPort);
                    var websocketsPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("websocketsPort")) ? DefaultWebSocketSPort : int.Parse(Environment.GetEnvironmentVariable("websocketsPort"));
                    dict[ServiceSchema.WSS] = string.Format(@"wss://localhost:{0}", websocketsPort);
#else
                    var websocketPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("websocketPort")) ? DefaultWebSocketPort : int.Parse(Environment.GetEnvironmentVariable("websocketPort"));
                    dict[ServiceSchema.WS] = string.Format(@"http://localhost:{0}", websocketPort);
                    var websocketsPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("websocketsPort")) ? DefaultWebSocketSPort : int.Parse(Environment.GetEnvironmentVariable("websocketsPort"));
                    dict[ServiceSchema.WSS] = string.Format(@"https://localhost:{0}", websocketsPort);
#endif
                    s_baseAddresses = dict;
                    dict[ServiceSchema.NETPIPE] = @"net.pipe://localhost";
                    Console.WriteLine("Using base addresses:");
                    foreach (var ba in dict.Values)
                    {
                        Console.WriteLine("\t" + ba);
                    }
                }

                return s_baseAddresses;
            }
        }

#if NET
        internal class ServiceTestHostOptions
        {
            public List<Uri> serviceBaseAddresses = new List<Uri>();
            public Dictionary<Enum, string> endpointBasePath = new Dictionary<Enum, string>();
        }

        internal static async Task StartHosts()
        {
            bool success = true;
            var serviceTestHostOptionsDict = new Dictionary<string, ServiceTestHostOptions>();

            var webHost = new WebHostBuilder()
                .ConfigureLogging((ILoggingBuilder logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddFilter("Default", LogLevel.Debug);
                    logging.AddFilter("Microsoft", LogLevel.Debug);
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.IPv6Any, new Uri(BaseAddresses[ServiceSchema.HTTP]).Port);
                    options.Listen(IPAddress.IPv6Any, new Uri(BaseAddresses[ServiceSchema.HTTPS]).Port, listenOptions =>
                    {
                        X509Certificate2 cert = TestHost.CertificateFromFriendlyName(StoreName.My, StoreLocation.LocalMachine, "WCF Bridge - Machine certificate generated by the CertificateManager");
                        Debug.Assert(cert != null, "couldn't find the server certificate");

                        listenOptions.UseHttps(cert);

                        if (Debugger.IsAttached)
                        {
                            listenOptions.UseConnectionLogging();
                        }
                    });
                    options.Listen(IPAddress.IPv6Any, new Uri(BaseAddresses[ServiceSchema.WS]).Port);
                    options.Listen(IPAddress.IPv6Any, new Uri(BaseAddresses[ServiceSchema.WSS]).Port, listenOptions =>
                    {
                        X509Certificate2 cert = TestHost.CertificateFromFriendlyName(StoreName.My, StoreLocation.LocalMachine, "WCF Bridge - Machine certificate generated by the CertificateManager");
                        listenOptions.UseHttps(cert);

                        if (Debugger.IsAttached)
                        {
                            listenOptions.UseConnectionLogging();
                        }
                    });
                })
                .UseNetTcp(IPAddress.IPv6Any, new Uri(BaseAddresses[ServiceSchema.NETTCP]).Port)
                .ConfigureServices(services =>
                {
                    services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                        .AddBasic(options =>
                        {
                            options.Realm = "Basic Authentication";
                            options.Events = new BasicAuthenticationEvents
                            {
                                OnValidateCredentials = context =>
                                {
                                    if (context.Username == context.Password)
                                    {
                                        var claims = new[]
                                            {
                                            new Claim(ClaimTypes.NameIdentifier, context.Username, ClaimValueTypes.String, context.Options.ClaimsIssuer),
                                            new Claim(ClaimTypes.Name, context.Username, ClaimValueTypes.String, context.Options.ClaimsIssuer)
                                            };

                                        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                                        context.Success();
                                    }

                                    return Task.CompletedTask;
                                }
                            };
                        });
                    services.AddServiceModelServices()
                    .AddServiceModelMetadata()
                    .AddServiceModelWebServices()
                    .AddRouting()
                    .AddAuthorization();
                })
                .Configure(app =>
                {
                    app.UseAuthentication();
                    app.UseAuthorization();                    
                    app.UseServiceModel(serviceBuilder =>
                    {
                        foreach (var serviceTestHost in GetAttributedServiceHostTypes())
                        {
                            var serviceTestHostOptions = new ServiceTestHostOptions();
                            foreach (TestServiceDefinitionAttribute attr in serviceTestHost.GetCustomAttributes(typeof(TestServiceDefinitionAttribute), false))
                            {
                                Uri serviceBaseAddress = null;
                                try
                                {
                                    foreach (Enum schema in Enum.GetValues(typeof(ServiceSchema)))
                                    {
                                        if (attr.Schema.HasFlag(schema))
                                        {
                                            serviceTestHostOptions.endpointBasePath.Add(schema, attr.BasePath);
                                            serviceBaseAddress = new Uri(BaseAddresses[(ServiceSchema)schema]);
                                            serviceTestHostOptions.serviceBaseAddresses.Add(serviceBaseAddress);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    ConsoleColor bg = Console.BackgroundColor;
                                    ConsoleColor fg = Console.ForegroundColor;
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Problem creating base address for servicehost type " + serviceTestHost.Name + " with schema " + attr.Schema + " and address " + serviceBaseAddress == null ? string.Empty : serviceBaseAddress.ToString());
                                    Console.WriteLine(e);
                                    Console.BackgroundColor = bg;
                                    Console.ForegroundColor = fg;
                                    success = false;
                                    break;
                                }
                            }

                            serviceTestHostOptionsDict.Add(serviceTestHost.Name, serviceTestHostOptions);
                            try
                            {
                                if (success)
                                {
                                    string serviceHostTypeName = serviceTestHost.Name;
                                    var serviceHost = (ServiceHost)Activator.CreateInstance(serviceTestHost, serviceTestHostOptionsDict[serviceHostTypeName].serviceBaseAddresses.ToArray());
                                    serviceBuilder.AddService(serviceHost.ServiceType, options =>
                                    {
                                        var localHostTypeName = serviceHostTypeName;
                                        options.BaseAddresses.Clear();
                                        foreach (var baseAddress in BaseAddresses.Values)
                                        {
                                            //if (!options.BaseAddresses.Contains(baseAddress))
                                                options.BaseAddresses.Add(new Uri(baseAddress));
                                        }
                                    });

                                    foreach (var endpoint in serviceHost.Endpoints)
                                    {
                                        Enum schema = ServiceHostHelper.ToServiceSchema(endpoint.Binding.Scheme);
                                        string basePath = serviceTestHostOptionsDict[serviceHostTypeName].endpointBasePath[schema];
                                        string endpointAddress = string.Format("{0}/{1}", basePath, endpoint.Address);
                                        serviceBuilder.AddServiceEndpoint(serviceHost.ServiceType, endpoint.ContractType, endpoint.Binding, new Uri(endpointAddress, UriKind.RelativeOrAbsolute), null);
                                    }

                                    serviceBuilder.ConfigureServiceHostBase(serviceHost.ServiceType, serviceHostBase =>
                                    {
                                        var localHostTypeName = serviceHostTypeName;
                                        var smb = serviceHostBase.Description.Behaviors.Find<ServiceMetadataBehavior>();
                                        if (serviceTestHostOptionsDict[localHostTypeName].serviceBaseAddresses.Where(uri => uri.Scheme == Uri.UriSchemeHttps).Any())
                                        {
                                            smb.HttpsGetEnabled = true;
                                        }

                                        smb.HttpGetEnabled = true;
                                        serviceHost.ApplyConfig(serviceHostBase);
                                    });
                                }
                            }
                            catch (Exception e)
                            {
                                ConsoleColor bg = Console.BackgroundColor;
                                ConsoleColor fg = Console.ForegroundColor;
                                Console.BackgroundColor = ConsoleColor.Black;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Problem starting servicehost type " + serviceTestHost.Name);
                                Console.WriteLine(e);
                                Console.BackgroundColor = bg;
                                Console.ForegroundColor = fg;
                            }
                        }

                    });
                }).Build();

            await webHost.StartAsync();
        }

        internal static IEnumerable<Type> GetAttributedServiceHostTypes()
        {
            var allTypes = typeof(TestDefinitionHelper).Assembly.GetTypes();
            var serviceHostTypes = from t in allTypes where (typeof(ServiceHost).IsAssignableFrom(t)) select t;
            return from sht in serviceHostTypes where (sht.GetCustomAttributes(typeof(TestServiceDefinitionAttribute), false).Length > 0) select sht;
        }
#else
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
#endif

    }
}
