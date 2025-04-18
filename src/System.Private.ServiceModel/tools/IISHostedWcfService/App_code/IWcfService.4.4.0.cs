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
    // This contract relies on features added post-1.1.0
    [ServiceContract]
    public interface IWcfService_4_4_0
    {
        [OperationContract(Action = "http://tempuri.org/IWcfService_4_4_0/MessageContractRequestReply")]
        ReplyBankingData_4_4_0 MessageContractRequestReply(RequestBankingData_4_4_0 bt);
        
        [OperationContract(Action = "http://tempuri.org/IWcfService_4_4_0/SendRequestWithXmlElementMessageHeader")]
        [XmlSerializerFormat(SupportFaults = true)]
        XmlElementMessageHeaderResponse SendRequestWithXmlElementMessageHeader(XmlElementMessageHeaderRequest request);
    }
}
