using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService.TestResources
{
    internal class BasicHttpsResource : HttpsResource
    {
        protected override string Address { get { return "basicHttps"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpsBinding();
        }
    }
}
