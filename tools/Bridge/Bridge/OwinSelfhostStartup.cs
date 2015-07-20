// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
            config.Formatters[0] = new JsonStringMediaTypeFormatter();
            appBuilder.UseErrorPage();
            appBuilder.UseWelcomePage("/");
        }

        public static IDisposable Startup(string url)
        {
            return WebApp.Start<OwinSelfhostStartup>(url);
        }
    }
}
