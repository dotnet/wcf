using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfService.TestResources
{
    internal class HttpsSoap11Resource : ResourceController<WcfService, IWcfService>
    {
        protected override string Protocol { get { return "https"; } }

        protected override string Address { get { return "https-soap11"; } }

        protected override string Port { get { return "44285"; } }

        protected override Binding GetBinding()
        {
            var binding = new CustomBinding();
            binding.Elements.Add(new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8));
            binding.Elements.Add(new HttpsTransportBindingElement());
            return binding;
        }
    }
}
