// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.Threading.Tasks;

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
    }
}
