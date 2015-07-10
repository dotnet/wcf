using System.ServiceModel.Channels;

namespace WcfService.TestResources
{
    internal class DefaultCustomHttpResource : HttpResource
    {
        protected override string Address { get { return "default-custom-http"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(), new HttpTransportBindingElement());
        }
    }
}
