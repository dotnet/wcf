// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;

namespace Web.App_Start
{
    public class OwinSelfhostStartup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            appBuilder.UseWebApi(config);
            appBuilder.UseErrorPage();
            appBuilder.UseWelcomePage("/");
        }

        public static IDisposable Startup(string url)
        {
            return WebApp.Start<OwinSelfhostStartup>(url);
        }
    }
}
