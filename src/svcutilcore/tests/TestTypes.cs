using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Tools.ServiceModel.SvcUtil.Tests
{
    [ServiceContract(ConfigurationName = "ISimpleSvcutilService")]
    public interface ISimpleSvcutilService
    {
        [XmlSerializerFormat]
        [OperationContract(Action = "http://tempuri.org/ISimpleSvcutilService/EchoXmlSerializerFormat", ReplyAction = "http://tempuri.org/ISimpleSvcutilService/EchoXmlSerializerFormatResponse")]
        string EchoXmlSerializerFormat(string message);
    }
}
