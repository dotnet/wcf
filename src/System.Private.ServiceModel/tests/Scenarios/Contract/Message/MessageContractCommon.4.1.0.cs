// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace MessageContractCommon
{
    public class MessageContractConstants
    {
        // CONSTANTS
        public const string wrapperNamespace = "http://www.contoso.com";
        public const string dateElementName = "Date_of_Request";
        public static string dateElementValue = "";
        public const string transactionElementName = "Transaction_Amount";
        public const string transactionElementValue = "1000000";
        public const string customerElementName = "Customer_Name";
        public const string customerElementNamespace = "http://www.contoso.com";
        public const string customerElementValue = "Michael Jordan";
        public const string extraValuesNamespace = "http://www.contoso.com";
    }

    public class MessageContractHelpers
    {
        public static XmlDictionaryReader GetResponseBodyReader(MyInspector inspector)
        {
            XmlDictionaryReader reader = inspector.ReceivedMessage.GetReaderAtBodyContents();
            return reader;
        }

        public static MessageHeaders GetHeaders(MyInspector inspector)
        {
            MessageHeaders headers = inspector.ReceivedMessage.Headers;
            return headers;
        }

        public static MyInspector SetupMessageContractTests(out IMessageContract clientProxy,
            out MessageContractTypes.RequestBankingData transaction)
        {
            MyInspector inspector = new MyInspector();
            BasicHttpBinding binding = new BasicHttpBinding();
            ChannelFactory<IMessageContract> factory = new ChannelFactory<IMessageContract>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            factory.Endpoint.EndpointBehaviors.Add(inspector);
            clientProxy = factory.CreateChannel();
            transaction = new MessageContractTypes.RequestBankingData();
            transaction.accountName = MessageContractConstants.customerElementValue;
            transaction.transactionDate = DateTime.Now;
            MessageContractConstants.dateElementValue = transaction.transactionDate.TimeOfDay.ToString();
            transaction.amount = Convert.ToInt32(MessageContractConstants.transactionElementValue);

            return inspector;
        }

        [ServiceContract]
        public interface IMessageContract
        {
            [OperationContract(Action = "http://tempuri.org/IWcfService/MessageContractRequestReply", ReplyAction = "http://tempuri.org/IWcfService/MessageContractRequestReplyResponse")]
            MessageContractTypes.ReplyBankingData MessageContractRequestReply(MessageContractTypes.RequestBankingData bt);

            [OperationContract(Action = "http://tempuri.org/IWcfService/MessageContractRequestReplyNotWrapped", ReplyAction = "http://tempuri.org/IWcfService/MessageContractRequestReplyNotWrappedResponse")]
            MessageContractTypes.ReplyBankingDataNotWrapped MessageContractRequestReplyNotWrapped(MessageContractTypes.RequestBankingData bt);

            [OperationContract(Action = "http://tempuri.org/IWcfService/MessageContractRequestReplyWithMessageHeader", ReplyAction = "http://tempuri.org/IWcfService/MessageContractRequestReplyWithMessageHeaderResponse")]
            MessageContractTypes.ReplyBankingDataWithMessageHeader MessageContractRequestReplyWithMessageHeader(MessageContractTypes.RequestBankingData bt);

            [OperationContract(Action = "http://tempuri.org/IWcfService/MessageContractRequestReplyWithMessageHeaderNotNecessaryUnderstood", ReplyAction = "http://tempuri.org/IWcfService/MessageContractRequestReplyWithMessageHeaderNotNecessaryUnderstoodResponse")]
            MessageContractTypes.ReplyBankingDataWithMessageHeaderNotNecessaryUnderstood MessageContractRequestReplyWithMessageHeaderNotNecessaryUnderstood(MessageContractTypes.RequestBankingData bt);
        }
    }

    public class MessageContractTypes
    {
        [MessageContract(IsWrapped = true, WrapperName = "RequestBankingDataWrapper", WrapperNamespace = "http://www.contoso.com")]
        public class RequestBankingData
        {
            [MessageBodyMember(Order = 1, Name = "Date_of_Request")]
            public DateTime transactionDate;
            [MessageBodyMember(Name = "Customer_Name", Namespace = "CustomNamespace", Order = 3)]
            public string accountName;
            [MessageBodyMember(Order = 2, Name = "Transaction_Amount")]
            public int amount;
        }

        [MessageContract(IsWrapped = true, WrapperName = "ReplyBankingDataWrapper", WrapperNamespace = "http://www.contoso.com")]
        public class ReplyBankingData
        {
            [MessageBodyMember(Order = 1, Name = "Date_of_Request")]
            public DateTime transactionDate;
            [MessageBodyMember(Name = "Customer_Name", Namespace = "http://www.contoso.com", Order = 3)]
            public string accountName;
            [MessageBodyMember(Order = 2, Name = "Transaction_Amount")]
            public int amount;
        }

        [MessageContract(IsWrapped = false)]
        public class ReplyBankingDataNotWrapped
        {
            [MessageBodyMember(Order = 1, Name = "Date_of_Request")]
            public DateTime transactionDate;
            [MessageBodyMember(Name = "Customer_Name", Namespace = "http://www.contoso.com", Order = 3)]
            public string accountName;
            [MessageBodyMember(Order = 2, Name = "Transaction_Amount")]
            public int amount;
        }

        [MessageContract(IsWrapped = true, WrapperName = "ReplyBankingDataWithMessageHeaderWrapper", WrapperNamespace = "http://www.contoso.com")]
        public class ReplyBankingDataWithMessageHeader
        {
            [MessageBodyMember(Order = 1, Name = "Date_of_Request")]
            public DateTime transactionDate;
            [MessageBodyMember(Name = "Customer_Name", Namespace = "http://www.contoso.com", Order = 3)]
            public string accountName;
            [MessageBodyMember(Order = 2, Name = "Transaction_Amount")]
            public int amount;
            [MessageHeader(Name = "ReplyBankingDataWithMessageHeaderExtraValues", Namespace = "http://www.contoso.com", MustUnderstand = true)]
            public string extraValues;
        }

        [MessageContract(IsWrapped = true, WrapperName = "ReplyBankingDataWithMessageHeaderNotNecessaryUnderstoodWrapper", WrapperNamespace = "http://www.contoso.com")]
        public class ReplyBankingDataWithMessageHeaderNotNecessaryUnderstood
        {
            [MessageBodyMember(Order = 1, Name = "Date_of_Request")]
            public DateTime transactionDate;
            [MessageBodyMember(Name = "Customer_Name", Namespace = "http://www.contoso.com", Order = 3)]
            public string accountName;
            [MessageBodyMember(Order = 2, Name = "Transaction_Amount")]
            public int amount;
            [MessageHeader(Name = "ReplyBankingDataWithMessageHeaderNotNecessaryUnderstoodExtraValue", Namespace = "http://www.contoso.com", MustUnderstand = false)]
            public string extraValues;
        }
    }

    public class MyInspector : IClientMessageInspector, IEndpointBehavior
    {
        private Message _receivedMessage;

        public Message ReceivedMessage
        {
            get
            {
                return _receivedMessage;
            }
        }

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            MessageBuffer buffer = reply.CreateBufferedCopy(int.MaxValue);
            _receivedMessage = buffer.CreateMessage();

            reply = buffer.CreateMessage();
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
        {
            return null;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        { }

        public void Validate(ServiceEndpoint endpoint)
        { }
    }
}
