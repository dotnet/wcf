// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
#else
using System.ServiceModel;
#endif

namespace WcfService
{
    [ServiceContract]
    [XmlSerializerFormat(Use = OperationFormatUse.Encoded)]
    public interface IWcfSoapService
    {
        [OperationContract(Action = "http://tempuri.org/IWcfService/CombineStringXmlSerializerFormatSoap")]
        [XmlSerializerFormat(Use = OperationFormatUse.Encoded)]
        string CombineStringXmlSerializerFormatSoap(string message1, string message2);

        [OperationContract(Action = "http://tempuri.org/IWcfService/EchoComositeTypeXmlSerializerFormatSoap")]
        [XmlSerializerFormat(Use = OperationFormatUse.Encoded)]
        SoapComplexType EchoComositeTypeXmlSerializerFormatSoap(SoapComplexType c);

        [OperationContract(Action = "http://tempuri.org/IWcfService/ProcessCustomerData")]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, SupportFaults = true, Use = OperationFormatUse.Encoded)]
        [ServiceKnownType(typeof(AdditionalData))]
        [return: MessageParameter(Name = "ProcessCustomerDataReturn")]
        [return: System.Xml.Serialization.SoapElement(DataType = "string")]
        string ProcessCustomerData([MessageParameter(Name = "CustomerData")]CustomerObject customerData);
        
        [OperationContract(Action = "http://tempuri.org/IWcfService/Ping", ReplyAction = "http://tempuri.org/IWcfSoapService/PingResponse")]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, SupportFaults = true, Use = OperationFormatUse.Encoded)]
        PingEncodedResponse Ping(PingEncodedRequest request);
    }
}
