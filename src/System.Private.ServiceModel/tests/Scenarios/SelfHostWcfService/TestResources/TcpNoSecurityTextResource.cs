using System.ServiceModel.Channels;

namespace WcfService.TestResources
{
    internal class TcpNoSecurityTextResource : TcpResource
    {
        protected override string Address { get { return "tcp-custombinding-nosecurity-text"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(), new TcpTransportBindingElement() { PortSharingEnabled = false });
        }
    }
}
