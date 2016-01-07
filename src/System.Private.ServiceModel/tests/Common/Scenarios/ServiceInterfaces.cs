// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    String EchoWithTimeout(String message, TimeSpan serviceOperationTimeout);

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

    [OperationContract(Action = "http://tempuri.org/IWcfService/GetIncomingMessageHeaders", ReplyAction = "*")]
    Dictionary<string, string> GetIncomingMessageHeaders();

    [OperationContract]
    Stream GetStreamFromString(string data);

    [OperationContract]
    string GetStringFromStream(Stream stream);

    [OperationContractAttribute(Action = "http://tempuri.org/IWcfService/EchoStream", ReplyAction = "http://tempuri.org/IWcfService/EchoStreamResponse")]
    Stream EchoStream(Stream stream);

    [OperationContractAttribute(Action = "http://tempuri.org/IWcfService/EchoStream", ReplyAction = "http://tempuri.org/IWcfService/EchoStreamResponse")]
    Task<Stream> EchoStreamAsync(Stream stream);
}

[ServiceContract]
public interface IWcfProjectNRestartService
{
    [OperationContract]
    string RestartService();
}

[ServiceContract(ConfigurationName = "IWcfService")]
public interface IWcfServiceXmlGenerated
{
    [OperationContractAttribute(Action = "http://tempuri.org/IWcfService/EchoXmlSerializerFormat", ReplyAction = "http://tempuri.org/IWcfService/EchoXmlSerializerFormatResponse"),
    XmlSerializerFormat]
    string EchoXmlSerializerFormat(string message);

    [OperationContractAttribute(Action = "http://tempuri.org/IWcfService/EchoXmlSerializerFormatSupportFaults", ReplyAction = "http://tempuri.org/IWcfService/EchoXmlSerializerFormatSupportFaultsResponse"),
    XmlSerializerFormat(SupportFaults = true)]
    string EchoXmlSerializerFormatSupportFaults(string message, bool pleaseThrowException);

    [OperationContract(Action = "http://tempuri.org/IWcfService/EchoXmlSerializerFormatUsingRpc", ReplyAction = "http://tempuri.org/IWcfService/EchoXmlSerializerFormatUsingRpcResponse"),
    XmlSerializerFormat(Style = OperationFormatStyle.Rpc)]
    string EchoXmlSerializerFormatUsingRpc(string message);

    [OperationContractAttribute(Action = "http://tempuri.org/IWcfService/EchoXmlSerializerFormat", ReplyAction = "http://tempuri.org/IWcfService/EchoXmlSerializerFormatResponse"),
    XmlSerializerFormat]
    Task<string> EchoXmlSerializerFormatAsync(string message);

    [OperationContract(Action = "http://tempuri.org/IWcfService/GetDataUsingXmlSerializer"),
    XmlSerializerFormat]
    XmlCompositeType GetDataUsingXmlSerializer(XmlCompositeType composite);

    [OperationContract(Action = "http://tempuri.org/IWcfService/EchoXmlVeryComplexType"),
    XmlSerializerFormat]
    XmlVeryComplexType EchoXmlVeryComplexType(XmlVeryComplexType complex);
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

[System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IWcfService")]
public interface IWcfServiceBeginEndGenerated
{
    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IWcfService/MessageRequestReply", ReplyAction = "http://tempuri.org/IWcfService/MessageRequestReplyResponse")]
    System.ServiceModel.Channels.Message MessageRequestReply(System.ServiceModel.Channels.Message request);

    [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://tempuri.org/IWcfService/MessageRequestReply", ReplyAction = "http://tempuri.org/IWcfService/MessageRequestReplyResponse")]
    System.IAsyncResult BeginMessageRequestReply(System.ServiceModel.Channels.Message request, System.AsyncCallback callback, object asyncState);

    System.ServiceModel.Channels.Message EndMessageRequestReply(System.IAsyncResult result);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IWcfService/Echo", ReplyAction = "http://tempuri.org/IWcfService/EchoResponse")]
    string Echo(string message);

    [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://tempuri.org/IWcfService/Echo", ReplyAction = "http://tempuri.org/IWcfService/EchoResponse")]
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

    [OperationContract(Action = "http://tempuri.org/IWcfService/UserGetAuthToken")]
    ResultObject<string> UserGetAuthToken(string liveId);

    [OperationContract]
    ResultObject<string> UserGetId();

    [OperationContract(Action = "http://tempuri.org/IWcfService/ValidateMessagePropertyHeaders")]
    Dictionary<string, string> ValidateMessagePropertyHeaders();
}

[ServiceContract]
public interface IWcfRestartService
{
    [OperationContract]
    String RestartService(Guid uniqueIdentifier);
}

[ServiceContract]
public interface IWcfCustomUserNameService
{
    [OperationContract(Action = "http://tempuri.org/IWcfCustomUserNameService/Echo")]
    String Echo(String message);
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

public interface IWcfDuplexServiceCallback
{
    [OperationContract]
    void OnPingCallback(Guid guid);
}

[ServiceContract(CallbackContract = typeof(IWcfDuplexTaskReturnCallback))]
public interface IWcfDuplexTaskReturnService
{
    [OperationContract]
    Task<Guid> Ping(Guid guid);

    [OperationContract]
    [FaultContract(typeof(FaultDetail), Name = "FaultDetail",
        Action = "http://tempuri.org/IWcfDuplexTaskReturnService/FaultPingFaultDetailFault")]
    Task<Guid> FaultPing(Guid guid);
}

public interface IWcfDuplexTaskReturnCallback
{
    [OperationContract]
    Task<Guid> ServicePingCallback(Guid guid);

    [OperationContract]
    [FaultContract(typeof(FaultDetail), Name = "FaultDetail",
        Action = "http://tempuri.org/IWcfDuplexTaskReturnCallback/ServicePingFaultCallbackFaultDetailFault")]
    Task<Guid> ServicePingFaultCallback(Guid guid);
}

// ********************************************************************************

[ServiceContract(CallbackContract = typeof(IWcfDuplexService_DataContract_Callback))]
public interface IWcfDuplexService_DataContract
{
    [OperationContract]
    void Ping_DataContract(Guid guid);
}

public interface IWcfDuplexService_DataContract_Callback
{
    [OperationContract]
    void OnDataContractPingCallback(ComplexCompositeTypeDuplexCallbackOnly complexCompositeType);
}

// ********************************************************************************

[ServiceContract(CallbackContract = typeof(IWcfDuplexService_Xml_Callback))]
public interface IWcfDuplexService_Xml
{
    [OperationContract]
    void Ping_Xml(Guid guid);
}

public interface IWcfDuplexService_Xml_Callback
{
    [OperationContract, XmlSerializerFormat]
    void OnXmlPingCallback(XmlCompositeTypeDuplexCallbackOnly xmlCompositeType);
}

// WebSocket Interfaces

[ServiceContract]
public interface IWSRequestReplyService
{
    [OperationContract]
    void UploadData(string data);

    [OperationContract]
    string DownloadData();

    [OperationContract]
    void UploadStream(Stream stream);

    [OperationContract]
    Stream DownloadStream();

    [OperationContract]
    Stream DownloadCustomizedStream(TimeSpan readThrottle, TimeSpan streamDuration);

    [OperationContract]
    void ThrowingOperation(Exception exceptionToThrow);

    [OperationContract]
    string DelayOperation(TimeSpan delay);

    // Logging
    [OperationContract]
    List<string> GetLog();
}

[ServiceContract(CallbackContract = typeof(IPushCallback))]
public interface IWSDuplexService
{
    // Request-Reply operations
    [OperationContract]
    string GetExceptionString();

    [OperationContract]
    void UploadData(string data);

    [OperationContract]
    string DownloadData();

    [OperationContract(IsOneWay = true)]
    void UploadStream(Stream stream);

    [OperationContract]
    Stream DownloadStream();

    // Duplex operations
    [OperationContract(IsOneWay = true)]
    void StartPushingData();

    [OperationContract(IsOneWay = true)]
    void StopPushingData();

    [OperationContract(IsOneWay = true)]
    void StartPushingStream();

    [OperationContract(IsOneWay = true)]
    void StartPushingStreamLongWait();

    [OperationContract(IsOneWay = true)]
    void StopPushingStream();

    // Logging
    [OperationContract(IsOneWay = true)]
    void GetLog();
}

public interface IPushCallback
{
    [OperationContract(IsOneWay = true)]
    void ReceiveData(string data);

    [OperationContract(IsOneWay = true)]
    void ReceiveStream(Stream stream);

    [OperationContract(IsOneWay = true)]
    void ReceiveLog(List<string> log);
}

// ********************************************************************************

[ServiceContract]
public interface IServiceContractIntOutService
{
    [OperationContract(AsyncPattern = true)]
    IAsyncResult BeginRequest(string stringRequest, AsyncCallback callback, object asyncState);

    void EndRequest(out int intResponse, IAsyncResult result);
}

[ServiceContract]
public interface IServiceContractUniqueTypeOutService
{
    [OperationContract(AsyncPattern = true)]
    IAsyncResult BeginRequest(string stringRequest, AsyncCallback callback, object asyncState);

    void EndRequest(out UniqueType uniqueTypeResponse, IAsyncResult result);
}

[ServiceContract]
public interface IServiceContractIntRefService
{
    [OperationContract(AsyncPattern = true)]
    IAsyncResult BeginRequest(string stringRequest, ref int referencedInteger, AsyncCallback callback, object asyncState);

    void EndRequest(ref int referencedInteger, IAsyncResult result);
}

[ServiceContract]
public interface IServiceContractUniqueTypeRefService
{
    [OperationContract(AsyncPattern = true)]
    IAsyncResult BeginRequest(string stringRequest, ref UniqueType uniqueTypeResponse, AsyncCallback callback, object asyncState);

    void EndRequest(ref UniqueType uniqueTypeResponse, IAsyncResult result);
}

[ServiceContract]
public interface IServiceContractUniqueTypeOutSyncService
{
    [OperationContract]
    void Request(string stringRequest, out UniqueType uniqueTypeResponse);
}

[ServiceContract]
public interface IServiceContractUniqueTypeRefSyncService
{
    [OperationContract]
    void Request(string stringRequest, ref UniqueType uniqueTypeResponse);
}

[ServiceContract]
public interface ILoginService
{

    [OperationContract(Action = "http://www.contoso.com/MtcRequest/loginRequest", ReplyAction = "http://www.contoso.com/MtcRequest/loginResponse")]
    [XmlSerializerFormat]
    LoginResponse Login(LoginRequest request);
}
