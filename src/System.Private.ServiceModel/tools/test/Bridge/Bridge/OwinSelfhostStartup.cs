// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;

namespace Bridge
{
    public class OwinSelfhostStartup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            IdleTimeoutHandler.Register(config);
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
