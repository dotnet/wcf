using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Tools.ServiceModel.SvcUtil.Tests
{
    [ServiceContract(ConfigurationName = "IWcfService")]
    public interface IWcfServiceXmlGenerated
    {
        [OperationContract(Action = "http://tempuri.org/IWcfService/EchoXmlSerializerFormat", ReplyAction = "http://tempuri.org/IWcfService/EchoXmlSerializerFormatResponse")]
        [XmlSerializerFormat]
        string EchoXmlSerializerFormat(string message);
    }
}
