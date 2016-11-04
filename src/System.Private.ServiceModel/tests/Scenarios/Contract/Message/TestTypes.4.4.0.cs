// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace WcfService
{
    // These classes exist for features added post-1.1.0
    [ServiceContract]
    public interface IWcfService_4_4_0
    {
        [OperationContract(Action = "http://tempuri.org/IWcfService_4_4_0/MessageContractRequestReply", 
                           ReplyAction = "http://tempuri.org/IWcfService_4_4_0/MessageContractRequestReplyResponse")]
        ReplyBankingData_4_4_0 MessageContractRequestReply(RequestBankingData_4_4_0 bt);
    }

    [MessageContract(IsWrapped = true,
                     WrapperName = "CustomWrapperName",
                     WrapperNamespace = "http://www.contoso.com")]
    public class RequestBankingData_4_4_0
    {
        [MessageBodyMember(Order = 1,
                           Name = "Date_of_Request")]
        public DateTime transactionDate;

        [MessageBodyMember(Name = "Customer_Name",
                           Namespace = "http://www.contoso.com",
                           Order = 3)]
        public string accountName;

        [MessageBodyMember(Order = 2,
                           Name = "Transaction_Amount")]
        public decimal amount;

        // The following rely on features added post-1.1.0
        [MessageProperty(Name = "TestProperty")]
        public string requestProperty;

        [MessageHeader(Name = "SingleElement")]
        public string requestSingleValue;

        [MessageHeader(Name = "MultipleElement")]
        public string[] requestMultipleValues;

        [MessageHeaderArray(Name = "ArrayMultipleElement")]
        public string[] requestArrayMultipleValues;
    }

    [MessageContract(IsWrapped = true,
                     WrapperName = "CustomWrapperName",
                     WrapperNamespace = "http://www.contoso.com")]
    public class ReplyBankingData_4_4_0
    {
        [MessageBodyMember(Order = 1, Name = "Date_of_Request")]
        public DateTime transactionDate;

        [MessageBodyMember(Name = "Customer_Name",
                           Namespace = "http://www.contoso.com",
                           Order = 3)]
        public string accountName;

        [MessageBodyMember(Order = 2,
                           Name = "Transaction_Amount")]
        public decimal amount;

        // The following rely on features added post-1.1.0
        [MessageProperty(Name = "TestProperty")]
        public string replyProperty;

        [MessageHeader(Name = "SingleElement")]
        public string replySingleValue;

        [MessageHeader(Name = "MultipleElement")]
        public string[] replyMultipleValues;

        [MessageHeaderArray(Name = "ArrayMultipleElement")]
        public string[] replyArrayMultipleValues;
    }
}
