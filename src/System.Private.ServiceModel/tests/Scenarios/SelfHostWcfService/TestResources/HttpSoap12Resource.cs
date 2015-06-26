using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfService.TestResources
{
    internal class HttpSoap12Resource : ResourceController<WcfService, IWcfService>
    {
        protected override string Protocol { get { return "http"; } }

        protected override string Address { get { return "http-soap12"; } }

        protected override string Port { get { return "8081"; } }

        protected override Binding GetBinding()
        {
            var binding = new CustomBinding();
            binding.Elements.Add(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8));
            binding.Elements.Add(new HttpTransportBindingElement());
            return binding;
        }
    }
}
