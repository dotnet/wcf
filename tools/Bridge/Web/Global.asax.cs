// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Web;
using System.Web.Http;
using Web.Models;

namespace Web
{
    public class WebApiApplication : HttpApplication
    {
        static public config Config = new config();

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
