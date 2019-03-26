// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

public class TestTypeConstants_4_4_0
{
    public const string MessageContract_RequestReply_OperationName = "MessageContractRequestReply";

    public const string MessageContract_Namespace = "http://www.contoso.com";
    public const string MessageContract_Request_Action = "http://tempuri.org/IWcfService/MessageContractRequestReply";
    public const string MessageContract_Reply_Action = "http://tempuri.org/IWcfService/MessageContractRequestReplyResponse";
    public const string MessageContract_RoundTrip_Request_Action = "http://tempuri.org/IWcfService/MessageContractRoundTrip";
    public const string MessageContract_RoundTrip_Reply_Action = "http://tempuri.org/IWcfService/MessageContractRoundTrip";
    public const string MessageContract_Request_WrapperName = "CustomRequestWrapperName";
    public const string MessageContract_Reply_WrapperName = "CustomReplyWrapperName";

    public const string MessageContract_Request_TransactionDateName = "Request_Date_of_Request";
    public const string MessageContract_Request_CustomerName = "Request_Customer_Name";
    public const string MessageContract_Request_SingleValueName = "Request_SingleRequestValue";
    public const string MessageContract_Request_MultipleValueName = "Request_MultipleRequestValue";
    public const string MessageContract_Request_MultipleArrayValueName = "Request_MultipleArrayRequestValue";
    public const string MessageContract_Request_PropertyName = "requestTestProperty";

    public const string MessageContract_Reply_TransactionDateName = "Reply_Date_of_Request";
    public const string MessageContract_Reply_TransactionAmountName = "Reply_Amount";
    public const string MessageContract_Reply_CustomerName = "Reply_Customer_Name";
    public const string MessageContract_Reply_SingleValueName = "Reply_SingleRequestValue";
    public const string MessageContract_Reply_MultipleValueName = "Reply_MultipleRequestValue";
    public const string MessageContract_Reply_MultipleArrayValueName = "Reply_MultipleArrayRequestValue";
    public const string MessageContract_Reply_PropertyName = "replyTestProperty";
}

// This service exposes a MessageContract containing attribute types
// that were added to the public API only after 1.1.0
// (MessageHeaderArrayAttribute and MessageProperty)
[ServiceContract]
public interface IMessageContract_4_4_0
{
    [OperationContract(Action = TestTypeConstants_4_4_0.MessageContract_Request_Action, 
                       ReplyAction = TestTypeConstants_4_4_0.MessageContract_Reply_Action)]
    ReplyBankingData_4_4_0 MessageContractRequestReply(RequestBankingData_4_4_0 bt);
}

[ServiceContract]
public interface IMessageContractRoundTrip_4_4_0
{
    [OperationContract(Action = TestTypeConstants_4_4_0.MessageContract_RoundTrip_Request_Action,
                   ReplyAction = TestTypeConstants_4_4_0.MessageContract_RoundTrip_Reply_Action)]
    RequestBankingData_4_4_0 MessageContractRoundTrip(RequestBankingData_4_4_0 bt);
}

[MessageContract(IsWrapped = true, 
                 WrapperName = TestTypeConstants_4_4_0.MessageContract_Request_WrapperName, 
                 WrapperNamespace = TestTypeConstants_4_4_0.MessageContract_Namespace)]
public class RequestBankingData_4_4_0
{
    [MessageProperty]
    public string requestTestProperty;

    [MessageBodyMember(Order = 1, 
                       Name = TestTypeConstants_4_4_0.MessageContract_Request_TransactionDateName)]
    public DateTime transactionDate;

    [MessageBodyMember(Name = TestTypeConstants_4_4_0.MessageContract_Request_CustomerName, 
                       Namespace = TestTypeConstants_4_4_0.MessageContract_Namespace, 
                       Order = 3)]
    public string accountName;

    [MessageHeader(Name = TestTypeConstants_4_4_0.MessageContract_Request_SingleValueName, 
                   Namespace = TestTypeConstants_4_4_0.MessageContract_Namespace, 
                   MustUnderstand = false)]
    public string testValue;

    [MessageHeader(Name = TestTypeConstants_4_4_0.MessageContract_Request_MultipleValueName, 
                   Namespace = TestTypeConstants_4_4_0.MessageContract_Namespace, 
                   MustUnderstand = false)]
    public string[] testValues;

    [MessageHeaderArray(Name = TestTypeConstants_4_4_0.MessageContract_Request_MultipleArrayValueName, 
                        Namespace = TestTypeConstants_4_4_0.MessageContract_Namespace, 
                        MustUnderstand = false)]
    public string[] testValuesArray;
}

[MessageContract(IsWrapped = true, 
                 WrapperName = TestTypeConstants_4_4_0.MessageContract_Reply_WrapperName, 
                 WrapperNamespace = TestTypeConstants_4_4_0.MessageContract_Request_WrapperName)]
public class ReplyBankingData_4_4_0
{
    [MessageProperty]
    public string replyTestProperty;

    [MessageBodyMember(Order = 1, 
                       Name = TestTypeConstants_4_4_0.MessageContract_Reply_TransactionDateName)]
    public DateTime transactionDate;

    [MessageBodyMember(Name = TestTypeConstants_4_4_0.MessageContract_Reply_CustomerName, 
                       Namespace = TestTypeConstants_4_4_0.MessageContract_Namespace, 
                       Order = 3)]
    public string accountName;

    [MessageBodyMember(Order = 2, 
                       Name = TestTypeConstants_4_4_0.MessageContract_Reply_TransactionAmountName)]
    public decimal amount;

    [MessageHeader(Name = TestTypeConstants_4_4_0.MessageContract_Reply_SingleValueName, 
                   Namespace = TestTypeConstants_4_4_0.MessageContract_Namespace, 
                   MustUnderstand = false)]
    public string testValue;

    [MessageHeader(Name = TestTypeConstants_4_4_0.MessageContract_Reply_MultipleValueName, 
                   Namespace = TestTypeConstants_4_4_0.MessageContract_Namespace, 
                   MustUnderstand = false)]
    public string[] testValues;

    [MessageHeaderArray(Name = TestTypeConstants_4_4_0.MessageContract_Reply_MultipleArrayValueName, 
                        Namespace = TestTypeConstants_4_4_0.MessageContract_Namespace, 
                        MustUnderstand = false)]
    public string[] testValuesArray;
}

public class NonSerializableType
{
    public string Name { get; private set; }
    public int Index { get; private set; }

    public NonSerializableType(string name, int index)
    {
        this.Name = name;
        this.Index = index;
    }
}

[DataContract]
public class NonSerializableTypeSurrogate
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public int Index { get; set; }
}

public class SurrogateTestType
{
    public NonSerializableType[] Members { get; set; }
}

