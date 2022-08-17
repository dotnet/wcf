﻿using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace WCFCorePerfService
{
    public class Program
    {        
        static void Main(string[] args)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "WCFCorePerfService.exe");
            Console.WriteLine(filePath);
            string command = $" advfirewall firewall add rule name=\"WCFCorePerfService\" dir=in protocol=any action=allow program=\"{filePath}\" enable=yes";
            ExecuteCommand(command, Environment.CurrentDirectory, TimeSpan.FromSeconds(20));
            Console.WriteLine("Application start.");

            using (var host = CreateWebHostBuilder(args).Build())
            {
                host.Start();

                Console.WriteLine("Service is Ready");
                Console.WriteLine("Press Any Key to Terminate...");
                Console.ReadLine();
            }

            command = $" advfirewall firewall delete rule name=\"WCFCorePerfService\" program=\"{filePath}\"";
            ExecuteCommand(command, Environment.CurrentDirectory, TimeSpan.FromSeconds(20));
            Console.WriteLine("Clean up the firewall rule.");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseKestrel(options =>
            {
                options.ListenLocalhost(8080);
                options.Listen(IPAddress.Loopback, 8443, listenOptions =>
                {
                    listenOptions.UseHttps();
                }
                );
            })
            .UseNetTcp(8808)
            .UseStartup<Startup>();

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddServiceModelServices();
            }

            public void Configure(IApplicationBuilder app)
            {
                WSHttpBinding serverBinding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
                serverBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
                app.UseServiceModel(builder =>
                {
                    builder.AddService<SayHello>();
                    builder.AddServiceEndpoint<SayHello, ISayHello>(new BasicHttpBinding(), "/WCFCorePerf/TestService.svc/BasicHttp");
                    builder.AddServiceEndpoint<SayHello, ISayHello>(new NetTcpBinding(SecurityMode.None), "/WCFCorePerf/TestService.svc/NetTcp");
                    builder.AddServiceEndpoint<SayHello, ISayHello>(serverBinding, "/WCFCorePerf/TestService.svc/WSHttp");
                    Action<ServiceHostBase> serviceHost = host => ChangeHostBehavior(host);
                    builder.ConfigureServiceHostBase<SayHello>(serviceHost);
                });
            }

            public void ChangeHostBehavior(ServiceHostBase host)
            {
                var srvCredentials = new CoreWCF.Description.ServiceCredentials();
                srvCredentials.UserNameAuthentication.UserNamePasswordValidationMode
                    = CoreWCF.Security.UserNamePasswordValidationMode.Custom;
                srvCredentials.UserNameAuthentication.CustomUserNamePasswordValidator
                    = new MyCustomValidator();
                host.Description.Behaviors.Add(srvCredentials);
            }
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
