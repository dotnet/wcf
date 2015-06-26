using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService.TestResources
{
    internal class TcpNoSecurityTextResource : ResourceController<WcfService, IWcfService>
    {
        protected override string Protocol { get { return "net.tcp"; } }

        protected override string Address { get { return "tcp-custombinding-nosecurity-text"; } }

        protected override string Port { get { return "809"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(), new TcpTransportBindingElement() { PortSharingEnabled = false });
        }
    }
}
