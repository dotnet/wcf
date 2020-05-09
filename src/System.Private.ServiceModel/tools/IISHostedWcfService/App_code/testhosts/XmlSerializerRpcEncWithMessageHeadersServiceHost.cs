using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "BasicHttpRpcEncWithHeaders.svc")]
    public class XmlSerializerRpcEncWithMessageHeadersServiceHost : TestServiceHostBase<IEchoRpcEncWithHeadersService>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public XmlSerializerRpcEncWithMessageHeadersServiceHost(params Uri[] baseAddresses)
            : base(typeof(EchoRpcEncWithHeadersService), baseAddresses)
        {
        }
    }
}
