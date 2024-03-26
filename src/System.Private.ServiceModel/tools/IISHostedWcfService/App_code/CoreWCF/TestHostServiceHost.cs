// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.AspNetCore;

namespace WcfService
{
    public class TestHostServiceHost
    {
        public void StartService()
        {
            IWebHost host = WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
            host.Run();
        }
    }

    internal class Startup
    {
        public const string TestHostRelativeAddress = "/TestHost.svc";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceModelWebServices();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseServiceModel(builder =>
            {
                builder.AddService<TestHost>();
                builder.AddServiceWebEndpoint<TestHost, ITestHost>(new WebHttpBinding(WebHttpSecurityMode.None), TestHostRelativeAddress);
            });
        }
    }
}
#endif
