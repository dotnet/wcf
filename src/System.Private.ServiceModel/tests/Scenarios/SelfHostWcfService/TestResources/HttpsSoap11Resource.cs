using System.ServiceModel.Channels;
using System.Text;

namespace WcfService.TestResources
{
    internal class HttpsSoap11Resource : HttpsResource
    {
        protected override string Address { get { return "https-soap11"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8), new HttpsTransportBindingElement());
        }
    }
}
