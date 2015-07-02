using System.ServiceModel.Channels;

namespace WcfService.TestResources
{
    internal class HttpBinaryResource : HttpResource
    {
        protected override string Address { get { return "http-binary"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new BinaryMessageEncodingBindingElement(), new HttpTransportBindingElement());
        }
    }
}
