using System;
using System.IO;
using System.Net;
using System.Reflection;
using CoreWCF;
using CoreWCF.Configuration;
using CoreWCFPerfService;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace WCFCorePerfService
{
    public class Program
    {        
        static void Main(string[] args)
        {           
            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "CoreWCFPerfService.exe");
            Console.WriteLine(filePath);

            FirewallRulesManager.OpenPortInFirewall("CoreWCFPerfService", filePath, "8080,8443,8088");
           
            Console.WriteLine("Application start.");

            using (var host = CreateWebHostBuilder(args).Build())
            {
                host.Start();

                Console.WriteLine("Service is Ready");
                Console.WriteLine("Press Any Key to Terminate...");
                Console.ReadLine();
            }
            
            Console.WriteLine("Clean up the firewall rule.");
            FirewallRulesManager.RemoveAllBridgeFirewallRules("CoreWCFPerfService");
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
    }
}
