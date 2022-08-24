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
    String EchoWithTimeout(String message, TimeSpan serviceOperationTimeout);

    [OperationContract(Action = "http://tempuri.org/IWcfService/EchoWithTimeout")]
    Task<String> EchoWithTimeoutAsync(String message, TimeSpan serviceOperationTimeout);

    [OperationContract(Action = "http://tempuri.org/IWcfService/GetDataUsingDataContract")]
    CompositeType GetDataUsingDataContract(CompositeType composite);

    [OperationContract]
    [FaultContract(typeof(FaultDetail), Action = "http://tempuri.org/IWcfService/TestFaultFaultDetailFault", Name = "FaultDetail", Namespace = "http://www.contoso.com/wcfnamespace")]
    void TestFault(string faultMsg);

    [OperationContract]
    [FaultContract(typeof(int), Action = "http://tempuri.org/IWcfService/TestFaultIntFault", Name = "IntFault", Namespace = "http://www.contoso.com/wcfnamespace")]
    void TestFaultInt(int faultCode);

    [OperationContract]
    [FaultContract(typeof(FaultDetail), Action = "http://tempuri.org/IWcfService/TestFaultFaultDetailFault", Name = "FaultDetail", Namespace = "http://www.contoso.com/wcfnamespace")]
    [FaultContract(typeof(FaultDetail2), Action = "http://tempuri.org/IWcfService/TestFaultFaultDetailFault2", Name = "FaultDetail2", Namespace = "http://www.contoso.com/wcfnamespace")]
    void TestFaults(string faultMsg, bool throwFaultDetail);

    [OperationContract]
    [FaultContract(typeof(FaultDetail), Action = "http://tempuri.org/IWcfService/TestFaultFaultDetailFault", Name = "FaultDetail", Namespace = "http://www.contoso.com/wcfnamespace")]
    [ServiceKnownType(typeof(KnownTypeA))]
    [ServiceKnownType(typeof(FaultDetail))]
    object[] TestFaultWithKnownType(string faultMsg, object[] objects);

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

    [OperationContract]
    void ReturnContentType(string contentType);

    [OperationContract]
    bool IsHttpKeepAliveDisabled();

    [OperationContract]
    Dictionary<string, string> GetRequestHttpHeaders();
}

[ServiceContract]
public interface IService1
{
    [OperationContract]
    string GetData(int value);

    [OperationContract]
    CompositeType GetDataUsingDataContract(CompositeType composite);
}

[ServiceContract]
public interface IWcfDecompService
{
    [OperationContract]
    bool IsDecompressionEnabled();
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

[ServiceContract(ConfigurationName = "IWcfSoapService")]
public interface IWcfSoapService
{
    [OperationContract(Action = "http://tempuri.org/IWcfService/CombineStringXmlSerializerFormatSoap", ReplyAction = "http://tempuri.org/IWcfService/CombineStringXmlSerializerFormatSoapResponse")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, SupportFaults = true, Use = OperationFormatUse.Encoded)]
    string CombineStringXmlSerializerFormatSoap(string message1, string message2);

    [OperationContract(Action = "http://tempuri.org/IWcfService/EchoComositeTypeXmlSerializerFormatSoap", ReplyAction = "http://tempuri.org/IWcfService/EchoComositeTypeXmlSerializerFormatSoapResponse")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, SupportFaults = true, Use = OperationFormatUse.Encoded)]
    SoapComplexType EchoComositeTypeXmlSerializerFormatSoap(SoapComplexType c);

    [OperationContract(Action = "http://tempuri.org/IWcfService/ProcessCustomerData", ReplyAction = "http://tempuri.org/IWcfSoapService/ProcessCustomerDataResponse")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, SupportFaults = true, Use = OperationFormatUse.Encoded)]
    [ServiceKnownType(typeof(AdditionalData))]
    [return: MessageParameter(Name = "ProcessCustomerDataReturn")]
    string ProcessCustomerData(CustomerObject CustomerData);

    [OperationContract(Action = "http://tempuri.org/IWcfService/Ping", ReplyAction = "http://tempuri.org/IWcfSoapService/PingResponse")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, SupportFaults = true, Use = OperationFormatUse.Encoded)]
    PingEncodedResponse Ping(PingEncodedRequest request);
}

// This type share the same name space with IWcfServiceXmlGenerated.
// And this type contains a method which is also defined in IWcfServiceXmlGenerated.
[ServiceContract(ConfigurationName = "IWcfService")]
public interface ISameNamespaceWithIWcfServiceXmlGenerated
{
    [OperationContractAttribute(Action = "http://tempuri.org/IWcfService/EchoXmlSerializerFormat", ReplyAction = "http://tempuri.org/IWcfService/EchoXmlSerializerFormatResponse"),
    XmlSerializerFormat]
    string EchoXmlSerializerFormat(string message);
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
    [OperationContract(Action = "http://tempuri.org/IWcfService/UserGetAuthToken")]
    ResultObject<string> UserGetAuthToken();

    [OperationContract(Action = "http://tempuri.org/IWcfService/ValidateMessagePropertyHeaders")]
    Dictionary<string, string> ValidateMessagePropertyHeaders();
}

[ServiceContract]
public interface IWcfRestartService
{
    [OperationContract]
    String RestartService(Guid uniqueIdentifier);

    [OperationContract]
    String NonRestartService(Guid uniqueIdentifier);
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

[ServiceContract(CallbackContract = typeof(IWcfDuplexService_CallbackDebugBehavior_Callback))]
public interface IWcfDuplexService_CallbackDebugBehavior
{
    [OperationContract]
    string Hello(string greeting, bool includeExceptionDetailInFaults);

    [OperationContract]
    bool GetResult(bool includeExceptionDetailInFaults);
}

public interface IWcfDuplexService_CallbackDebugBehavior_Callback
{
    [OperationContract]
    void ReplyThrow(string input);
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

[ServiceContract(CallbackContract = typeof(IWcfDuplexService_CallbackConcurrencyMode_Callback))]
public interface IWcfDuplexService_CallbackConcurrencyMode
{
    [OperationContract]
    Task DoWorkAsync();
}

public interface IWcfDuplexService_CallbackConcurrencyMode_Callback
{
    [OperationContract]
    Task CallWithWaitAsync(int delayTime);
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

    [OperationContract]
    void Request2(out UniqueType uniqueTypeResponse, string stringRequest);
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

[ServiceContract(Namespace = "http://www.contoso.com/IXmlMessageContarctTestService")]
public interface IXmlMessageContarctTestService
{
    [OperationContract(
        Action = "http://www.contoso.com/IXmlMessageContarctTestService/EchoMessageResponseWithMessageHeader",
        ReplyAction = "*")]
    [XmlSerializerFormat(SupportFaults = true)]
    XmlMessageContractTestResponse EchoMessageResponseWithMessageHeader(XmlMessageContractTestRequest request);

    [OperationContract(
        Action = "http://www.contoso.com/IXmlMessageContarctTestService/EchoMessageResquestWithMessageHeader",
        ReplyAction = "*")]
    [XmlSerializerFormat(SupportFaults = true)]
    XmlMessageContractTestResponse EchoMessageResquestWithMessageHeader(XmlMessageContractTestRequestWithMessageHeader request);
}

[ServiceContract]
public interface IWcfAspNetCompatibleService
{
    [OperationContract(Action = "http://tempuri.org/IWcfService/EchoCookie", ReplyAction = "http://tempuri.org/IWcfService/EchoCookieResponse")]
    string EchoCookie();

    [OperationContract(Action = "http://tempuri.org/IWcfService/EchoTimeAndSetCookie", ReplyAction = "http://tempuri.org/IIWcfService/EchoTimeAndSetCookieResponse")]
    string EchoTimeAndSetCookie(string name);
}

[ServiceContract(ConfigurationName = "IWcfService")]
public interface IWcfServiceXml_OperationContext
{
    [XmlSerializerFormat]
    [ServiceKnownType(typeof(MesssageHeaderCreateHeaderWithXmlSerializerTestType))]
    [OperationContract(Action = "http://tempuri.org/IWcfService/GetIncomingMessageHeadersMessage", ReplyAction = "*")]
    string GetIncomingMessageHeadersMessage(string customHeaderName, string customHeaderNS);
}

[ServiceContract]
public interface IWcfChannelExtensibilityContract
{
    [OperationContract(IsOneWay = true)]
    void ReportWindSpeed(int speed);
}

[ServiceContract]
public interface IVerifyWebSockets
{
    [OperationContract()]
    bool ValidateWebSocketsUsed();
}

[ServiceContract]
public interface IDataContractResolverService
{
    [OperationContract(Action = "http://tempuri.org/IDataContractResolverService/GetAllEmployees")]
    List<Employee> GetAllEmployees();

    [OperationContract(Action = "http://tempuri.org/IDataContractResolverService/AddEmployee")]
    void AddEmployee(Employee employee);
}


[ServiceContract(SessionMode = SessionMode.Required)]
public interface ISessionTestsDefaultService
{
    [OperationContract(IsInitiating = true, IsTerminating = false)]
    int MethodAInitiating(int a);

    [OperationContract(IsInitiating = false, IsTerminating = false)]
    int MethodBNonInitiating(int b);

    [OperationContract(IsInitiating = false, IsTerminating = true)]
    SessionTestsCompositeType MethodCTerminating();
}

[ServiceContract(SessionMode = SessionMode.Required)]
public interface ISessionTestsShortTimeoutService : ISessionTestsDefaultService
{
}

[ServiceContract(CallbackContract = typeof(ISessionTestsDuplexCallback), SessionMode = SessionMode.Required)]
public interface ISessionTestsDuplexService
{
    [OperationContract]
    int NonTerminatingMethodCallingDuplexCallbacks(
       int callsToClientCallbackToMake,
       int callsToTerminatingClientCallbackToMake,
       int callsToClientSideOnlyTerminatingClientCallbackToMake,
       int callsToNonTerminatingMethodToMakeInsideClientCallback,
       int callsToTerminatingMethodToMakeInsideClientCallback);

    [OperationContract]
    int TerminatingMethodCallingDuplexCallbacks(
        int callsToClientCallbackToMake,
        int callsToTerminatingClientCallbackToMake,
        int callsToClientSideOnlyTerminatingClientCallbackToMake,
        int callsToNonTerminatingMethodToMakeInsideClientCallback,
        int callsToTerminatingMethodToMakeInsideClientCallback);

    [OperationContract(IsInitiating = true, IsTerminating = false)]
    int NonTerminatingMethod();

    [OperationContract(IsInitiating = true, IsTerminating = true)]
    int TerminatingMethod();
}

[ServiceContract(SessionMode = SessionMode.Required)]
public interface ISessionTestsDuplexCallback
{
    [OperationContract(IsInitiating = true, IsTerminating = false)]
    int ClientCallback(int callsToNonTerminatingMethodToMake, int callsToTerminatingMethodToMake);

    [OperationContract(IsInitiating = true, IsTerminating = true)]
    int TerminatingClientCallback(int callsToNonTerminatingMethodToMake, int callsToTerminatingMethodToMake);

    [OperationContract(IsInitiating = true, IsTerminating = true)]
    int ClientSideOnlyTerminatingClientCallback(int callsToNonTerminatingMethodToMake, int callsToTerminatingMethodToMake);
}

[ServiceContract, XmlSerializerFormat]
public interface IXmlSFAttribute
{
    [OperationContract, XmlSerializerFormat(SupportFaults = true)]
    [FaultContract(typeof(FaultDetailWithXmlSerializerFormatAttribute),
        Action = "http://tempuri.org/IWcfService/FaultDetailWithXmlSerializerFormatAttribute",
        Name = "FaultDetailWithXmlSerializerFormatAttribute",
        Namespace = "http://www.contoso.com/wcfnamespace")]
    void TestXmlSerializerSupportsFaults_True();

    [OperationContract, XmlSerializerFormat]
    [FaultContract(typeof(FaultDetailWithXmlSerializerFormatAttribute),
        Action = "http://tempuri.org/IWcfService/FaultDetailWithXmlSerializerFormatAttribute",
        Name = "FaultDetailWithXmlSerializerFormatAttribute",
        Namespace = "http://www.contoso.com/wcfnamespace")]
    void TestXmlSerializerSupportsFaults_False();
}

[ServiceContract(Namespace = "http://contoso.com/calc")]
[XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
public interface ICalculatorRpcEnc
{
    [OperationContract]
    int Sum2(int i, int j);

    [OperationContract]
    int Sum(IntParams par);

    [OperationContract]
    float Divide(FloatParams par);

    [OperationContract]
    string Concatenate(IntParams par);

    [OperationContract]
    void AddIntParams(Guid guid, IntParams par);

    [OperationContract]
    IntParams GetAndRemoveIntParams(Guid guid);

    [OperationContract]
    DateTime ReturnInputDateTime(DateTime dt);

    [OperationContract]
    byte[] CreateSet(ByteParams par);
}

[ServiceContract(Namespace = "http://contoso.com/calc")]
[XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
public interface ICalculatorRpcLit
{
    [OperationContract]
    int Sum2(int i, int j);

    [OperationContract]
    int Sum(IntParams par);

    [OperationContract]
    float Divide(FloatParams par);

    [OperationContract]
    string Concatenate(IntParams par);

    [OperationContract]
    void AddIntParams(Guid guid, IntParams par);

    [OperationContract]
    IntParams GetAndRemoveIntParams(Guid guid);

    [OperationContract]
    DateTime ReturnInputDateTime(DateTime dt);

    [OperationContract]
    byte[] CreateSet(ByteParams par);
}

[ServiceContract(Namespace = "http://contoso.com/calc")]
[XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
public interface ICalculatorDocLit
{
    [OperationContract]
    int Sum2(int i, int j);

    [OperationContract]
    int Sum(IntParams par);

    [OperationContract]
    float Divide(FloatParams par);

    [OperationContract]
    string Concatenate(IntParams par);

    [OperationContract]
    void AddIntParams(Guid guid, IntParams par);

    [OperationContract]
    IntParams GetAndRemoveIntParams(Guid guid);

    [OperationContract]
    DateTime ReturnInputDateTime(DateTime dt);

    [OperationContract]
    byte[] CreateSet(ByteParams par);
}

[ServiceContract]
[XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
public interface IHelloWorldRpcEnc
{
    [OperationContract]
    void AddString(Guid guid, string testString);

    [OperationContract]
    string GetAndRemoveString(Guid guid);
}

[ServiceContract]
[XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
public interface IHelloWorldRpcLit
{
    [OperationContract]
    void AddString(Guid guid, string testString);

    [OperationContract]
    string GetAndRemoveString(Guid guid);
}

[ServiceContract]
[XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
public interface IHelloWorldDocLit
{
    [OperationContract]
    void AddString(Guid guid, string testString);

    [OperationContract]    
    string GetAndRemoveString(Guid guid);
}

[ServiceContract]
public interface IEchoRpcEncWithHeadersService
{
    [OperationContract(Action = "http://tempuri.org/Echo", ReplyAction = "*")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
    EchoResponse Echo(EchoRequest request);
}

[System.Xml.Serialization.SoapType(Namespace = "http://tempuri.org/")]
public partial class StringHeader
{
    public string HeaderValue { get; set; }
}

[MessageContract(WrapperName = "Echo", WrapperNamespace = "http://contoso.com/", IsWrapped = true)]
public partial class EchoRequest
{
    [MessageHeader]
    public StringHeader StringHeader;
    [MessageBodyMember(Namespace = "", Order = 0)]
    public string message;
}

[MessageContract(WrapperName = "EchoResponse", WrapperNamespace = "http://tempuri.org/", IsWrapped = true)]
public partial class EchoResponse
{
    [MessageHeader]
    public StringHeader StringHeader;
    [MessageBodyMember(Namespace = "", Order = 0)]
    public string EchoResult;
}

public class IntParams
{
    public int P1;
    public int P2;
}

public class FloatParams
{
    public float P1;
    public float P2;
}

public class ByteParams
{
    public byte P1;
    public byte P2;
}

// ********************************************************************************

[ServiceContract]
public interface IWcfReliableService
{
    [OperationContract]
    Task<int> GetNextNumberAsync();
    [OperationContract]
    Task<string> EchoAsync(string echo);
}

[ServiceContract]
public interface IOneWayWcfReliableService
{
    [OperationContract(IsOneWay = true)]
    Task OneWayAsync(string text);
}

[ServiceContract(CallbackContract = typeof(IWcfReliableDuplexService))]
public interface IWcfReliableDuplexService
{
    [OperationContract]
    Task<string> DuplexEchoAsync(string echo);
}
