// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using TestTypes;

[ServiceContract]
public interface IWtfService
{
    [OperationContract]
    Message MessageRequestReply(Message message);

    [OperationContract]
    String Echo(String message);

    [OperationContract(Action = "http://tempuri.org/IWtfService/EchoComplex")]
    ComplexCompositeType EchoComplex(ComplexCompositeType message);

    [OperationContract(Action = "http://tempuri.org/IWtfService/EchoWithTimeout")]
    String EchoWithTimeout(String message);

    [OperationContract(Action = "http://tempuri.org/IWtfService/GetDataUsingDataContract")]
    CompositeType GetDataUsingDataContract(CompositeType composite);

    [OperationContract]
    [FaultContract(typeof(FaultDetail), Action = "http://tempuri.org/IWtfService/TestFaultFaultDetailFault", Name = "FaultDetail", Namespace = "http://www.contoso.com/wtfnamespace")]
    void TestFault(string faultMsg);

    [OperationContract]
    void ThrowInvalidOperationException(string message);

    [OperationContract]
    void NotExistOnServer();

    [OperationContract(Action = "http://tempuri.org/IWtfService/EchoHttpMessageProperty")]
    TestHttpRequestMessageProperty EchoHttpRequestMessageProperty();

    [OperationContract(Action = "http://tempuri.org/IWtfService/GetRestartServiceEndpoint")]
    string GetRestartServiceEndpoint();

    [OperationContract(Action = "http://tempuri.org/IWtfService/GetRequestCustomHeader", ReplyAction = "*")]
    string GetRequestCustomHeader(string customHeaderName, string customHeaderNamespace);
}

[ServiceContract]
public interface IWtfProjectNRestartService
{
    [OperationContract]
    string RestartService();
}

// 



[System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IWtfService")]
public interface IWtfServiceGenerated
{
    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IWtfService/MessageRequestReply", ReplyAction = "http://tempuri.org/IWtfService/MessageRequestReplyResponse")]
    System.ServiceModel.Channels.Message MessageRequestReply(System.ServiceModel.Channels.Message request);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IWtfService/MessageRequestReply", ReplyAction = "http://tempuri.org/IWtfService/MessageRequestReplyResponse")]
    System.Threading.Tasks.Task<System.ServiceModel.Channels.Message> MessageRequestReplyAsync(System.ServiceModel.Channels.Message request);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IWtfService/Echo", ReplyAction = "http://tempuri.org/IWtfService/EchoResponse")]
    string Echo(string message);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IWtfService/Echo", ReplyAction = "http://tempuri.org/IWtfService/EchoResponse")]
    System.Threading.Tasks.Task<string> EchoAsync(string message);
}

// 



[System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IWtfService")]
public interface IWtfServiceBeginEndGenerated
{
    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IWtfService/MessageRequestReply", ReplyAction = "http://tempuri.org/IWtfService/MessageRequestReplyResponse")]
    System.ServiceModel.Channels.Message MessageRequestReply(System.ServiceModel.Channels.Message request);

    [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://tempuri.org/IWtfService/MessageRequestReply", ReplyAction = "http://tempuri.org/IWtfService/MessageRequestReplyResponse")]
    System.IAsyncResult BeginMessageRequestReply(System.ServiceModel.Channels.Message request, System.AsyncCallback callback, object asyncState);

    System.ServiceModel.Channels.Message EndMessageRequestReply(System.IAsyncResult result);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IWtfService/Echo", ReplyAction = "http://tempuri.org/IWtfService/EchoResponse")]
    string Echo(string message);

    [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://tempuri.org/IWtfService/Echo", ReplyAction = "http://tempuri.org/IWtfService/EchoResponse")]
    System.IAsyncResult BeginEcho(string message, System.AsyncCallback callback, object asyncState);

    string EndEcho(System.IAsyncResult result);
}

// Dummy interface used for ContractDescriptionTests
[ServiceContract]
public interface IDescriptionTestsService
{
    [OperationContract]
    Message MessageRequestReply(Message message);

    [OperationContract]
    String Echo(String message);
}

// Dummy interface used for ContractDescriptionTests
// This code is deliberately not cleaned up after svcutil to test that we work with the raw Add Service Reference code.
[System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IDescriptionTestsService")]
public interface IDescriptionTestsServiceGenerated
{
    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IDescriptionTestsService/MessageRequestReply", ReplyAction = "http://tempuri.org/IDescriptionTestsService/MessageRequestReplyResponse")]
    System.ServiceModel.Channels.Message MessageRequestReply(System.ServiceModel.Channels.Message request);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IDescriptionTestsService/MessageRequestReply", ReplyAction = "http://tempuri.org/IDescriptionTestsService/MessageRequestReplyResponse")]
    System.Threading.Tasks.Task<System.ServiceModel.Channels.Message> MessageRequestReplyAsync(System.ServiceModel.Channels.Message request);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IDescriptionTestsService/Echo", ReplyAction = "http://tempuri.org/IDescriptionTestsService/EchoResponse")]
    string Echo(string message);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IDescriptionTestsService/Echo", ReplyAction = "http://tempuri.org/IDescriptionTestsService/EchoResponse")]
    System.Threading.Tasks.Task<string> EchoAsync(string message);
}

// Dummy interface used for ContractDescriptionTests
// This code is deliberately not cleaned up after svcutil to test that we work with the raw Add Service Reference code.
[System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IDescriptionTestsService")]
public interface IDescriptionTestsServiceBeginEndGenerated
{
    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IDescriptionTestsService/MessageRequestReply", ReplyAction = "http://tempuri.org/IDescriptionTestsService/MessageRequestReplyResponse")]
    System.ServiceModel.Channels.Message MessageRequestReply(System.ServiceModel.Channels.Message request);

    [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://tempuri.org/IDescriptionTestsService/MessageRequestReply", ReplyAction = "http://tempuri.org/IDescriptionTestsService/MessageRequestReplyResponse")]
    System.IAsyncResult BeginMessageRequestReply(System.ServiceModel.Channels.Message request, System.AsyncCallback callback, object asyncState);

    System.ServiceModel.Channels.Message EndMessageRequestReply(System.IAsyncResult result);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IDescriptionTestsService/Echo", ReplyAction = "http://tempuri.org/IDescriptionTestsService/EchoResponse")]
    string Echo(string message);

    [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://tempuri.org/IDescriptionTestsService/Echo", ReplyAction = "http://tempuri.org/IDescriptionTestsService/EchoResponse")]
    System.IAsyncResult BeginEcho(string message, System.AsyncCallback callback, object asyncState);

    string EndEcho(System.IAsyncResult result);
}

// Manually constructed service interface to validate MessageContract operations.
// This interface closely matches the one found at http://app.thefreedictionary.com/w8feedback.asmx
// This was done to test that we would work with that real world app.
[ServiceContract(Namespace = "http://app.my.com/MyFeedback", ConfigurationName = "TFDFeedbackService.MyFeedbackSoap")]
public interface IFeedbackService
{
    [OperationContract(Action = "http://app.my.com/MyFeedback/Feedback", ReplyAction = "*")]
    Task<FeedbackResponse> FeedbackAsync(FeedbackRequest request);
}

[ServiceContract]
public interface IUser
{
    [OperationContract]
    string GetData(int value);

    [OperationContract(Action = "http://tempuri.org/IWtfService/UserGetAuthToken")]
    ResultObject<string> UserGetAuthToken(string liveId);

    [OperationContract]
    ResultObject<string> UserGetId();

    [OperationContract(Action = "http://tempuri.org/IWtfService/ValidateMessagePropertyHeaders")]
    Dictionary<string, string> ValidateMessagePropertyHeaders();
}

[ServiceContract]
public interface IWtfRestartService
{
    [OperationContract]
    String RestartService(Guid uniqueIdentifier);
}
