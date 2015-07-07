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
