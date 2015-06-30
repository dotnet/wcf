using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService.TestResources
{
    internal class BasicHttpResource : HttpResource
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }
    }
}
