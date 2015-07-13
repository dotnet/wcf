using System.ServiceModel.Channels;
using System.Text;

namespace WcfService.TestResources
{
    internal class HttpSoap12Resource : HttpResource
    {
        protected override string Address { get { return "http-soap12"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8), new HttpTransportBindingElement());
        }
    }
}
