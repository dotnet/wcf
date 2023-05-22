// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using TestTypes;

[ServiceContract]
public interface IWcfService
{
    [OperationContract]
    Message MessageRequestReply(Message message);

    [OperationContract]
    String Echo(String message);

    [OperationContract(Action = "http://tempuri.org/IWcfService/EchoComplex")]
    ComplexCompositeType EchoComplex(ComplexCompositeType message);

    [OperationContract(Action = "http://tempuri.org/IWcfService/EchoWithTimeout")]
    String EchoWithTimeout(String message);

    [OperationContract(Action = "http://tempuri.org/IWcfService/GetDataUsingDataContract")]
    CompositeType GetDataUsingDataContract(CompositeType composite);

    [OperationContract]
    [FaultContract(typeof(FaultDetail), Action = "http://tempuri.org/IWcfService/TestFaultFaultDetailFault", Name = "FaultDetail", Namespace = "http://www.contoso.com/wcfnamespace")]
    void TestFault(string faultMsg);

    [OperationContract]
    [FaultContract(typeof(int), Action = "http://tempuri.org/IWcfService/TestFaultIntFault", Name = "IntFault", Namespace = "http://www.contoso.com/wcfnamespace")]
    void TestFaultInt(int faultCode);

    [OperationContract]
    void ThrowInvalidOperationException(string message);

    [OperationContract]
    void NotExistOnServer();

    [OperationContract(Action = "http://tempuri.org/IWcfService/EchoHttpMessageProperty")]
    TestHttpRequestMessageProperty EchoHttpRequestMessageProperty();

    [OperationContract(Action = "http://tempuri.org/IWcfService/GetRestartServiceEndpoint")]
    string GetRestartServiceEndpoint();

    [OperationContract(Action = "http://tempuri.org/IWcfService/GetRequestCustomHeader", ReplyAction = "*")]
    string GetRequestCustomHeader(string customHeaderName, string customHeaderNamespace);

    [OperationContract]
    Stream GetStreamFromString(string data);

    [OperationContract]
    string GetStringFromStream(Stream stream);

    [OperationContractAttribute(Action = "http://tempuri.org/IWcfService/EchoStream", ReplyAction = "http://tempuri.org/IWcfService/EchoStreamResponse")]
    Stream EchoStream(Stream stream);

    [OperationContractAttribute(Action = "http://tempuri.org/IWcfService/EchoStream", ReplyAction = "http://tempuri.org/IWcfService/EchoStreamResponse")]
    Task<Stream> EchoStreamAsync(Stream stream);

    [OperationContract(Action = "http://tempuri.org/IWcfService/GetIncomingMessageHeaders", ReplyAction = "*")]
    Dictionary<MessageHeaderInfo, string> GetIncomingMessageHeaders();
}

[System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IWcfService")]
public interface IWcfServiceGenerated
{
    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IWcfService/MessageRequestReply", ReplyAction = "http://tempuri.org/IWcfService/MessageRequestReplyResponse")]
    System.ServiceModel.Channels.Message MessageRequestReply(System.ServiceModel.Channels.Message request);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IWcfService/MessageRequestReply", ReplyAction = "http://tempuri.org/IWcfService/MessageRequestReplyResponse")]
    System.Threading.Tasks.Task<System.ServiceModel.Channels.Message> MessageRequestReplyAsync(System.ServiceModel.Channels.Message request);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IWcfService/Echo", ReplyAction = "http://tempuri.org/IWcfService/EchoResponse")]
    string Echo(string message);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IWcfService/Echo", ReplyAction = "http://tempuri.org/IWcfService/EchoResponse")]
    System.Threading.Tasks.Task<string> EchoAsync(string message);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IWcfService/EchoMessageParameter", ReplyAction = "http://tempuri.org/IWcfService/EchoMessageParameterResponse")]
    [return: System.ServiceModel.MessageParameterAttribute(Name = "result")]
    string EchoMessageParameter(string name);
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

[ServiceContract(
    Name = "SampleDuplexHello",
    Namespace = "http://microsoft.wcf.test",
    CallbackContract = typeof(IHelloCallbackContract)
  )]
public interface IDuplexHello
{
    [OperationContract(IsOneWay = true)]
    void Hello(string greeting);
}

public interface IHelloCallbackContract
{
    [OperationContract(IsOneWay = true)]
    void Reply(string responseToGreeting);
}

[ServiceContract(CallbackContract = typeof(IWcfDuplexServiceCallback))]
public interface IWcfDuplexService
{
    [OperationContract]
    void Ping(Guid guid);
}

[ServiceContract(CallbackContract = typeof(IWcfDuplexServiceCallback))]
public interface ICustomOperationBehaviorDuplexService
{
    [OperationContract]
    [MyOperationBehavior]
    void RoundTripGuid(Guid guid);
}

public interface IWcfDuplexServiceCallback
{
    [OperationContract]
    void OnPingCallback(Guid guid);
}

public interface IWcfDuplexTaskReturnCallback
{
    [OperationContract]
    Task<Guid> ServicePingCallback(Guid guid);

    [OperationContract]
    [FaultContract(typeof(FaultDetail), Name = "FaultDetail")]
    Task<Guid> ServicePingFaultCallback(Guid guid);
}

