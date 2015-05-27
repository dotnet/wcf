// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        public const string wrapperName = "CustomWrapperName";
        public const string wrapperNamespace = "http://www.contoso.com";
        public const string dateElementName = "Date_of_Request";
        public static string dateElementValue = "";
        public const string transactionElementName = "Transaction_Amount";
        public const string transactionElementValue = "1000000";
        public const string customerElementName = "Customer_Name";
        public const string customerElementNamespace = "http://www.contoso.com";
        public const string customerElementValue = "Michael Jordan";
    }

    public class MessageContractHelpers
    {
        public static XmlDictionaryReader SetupMessageContractTests(bool isWrapped)
        {
            MyInspector inspector = new MyInspector();
            BasicHttpBinding binding = new BasicHttpBinding();
            ChannelFactory<IMessageContract> factory = new ChannelFactory<IMessageContract>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            factory.Endpoint.EndpointBehaviors.Add(inspector);
            IMessageContract clientProxy = factory.CreateChannel();
            MessageContractTypes.RequestBankingData transaction = new MessageContractTypes.RequestBankingData();
            transaction.accountName = MessageContractConstants.customerElementValue;
            transaction.transactionDate = DateTime.Now;
            MessageContractConstants.dateElementValue = transaction.transactionDate.TimeOfDay.ToString();
            transaction.amount = Convert.ToInt32(MessageContractConstants.transactionElementValue);

            if (isWrapped)
            {
                MessageContractTypes.ReplyBankingData responseData = clientProxy.MessageContractRequestReply(transaction);
            }
            else
            {
                MessageContractTypes.ReplyBankingDataNotWrapped responseData = clientProxy.MessageContractRequestReplyNotWrapped(transaction);
            }

            XmlDictionaryReader reader = inspector.ReceivedMessage.GetReaderAtBodyContents();
            return reader;
        }

        [ServiceContract]
        public interface IMessageContract
        {
            [OperationContract(Action = "http://tempuri.org/IWcfService/MessageContractRequestReply", ReplyAction = "http://tempuri.org/IWcfService/MessageContractRequestReplyResponse")]
            MessageContractTypes.ReplyBankingData MessageContractRequestReply(MessageContractTypes.RequestBankingData bt);

            [OperationContract(Action = "http://tempuri.org/IWcfService/MessageContractRequestReplyNotWrapped", ReplyAction = "http://tempuri.org/IWcfService/MessageContractRequestReplyNotWrappedResponse")]
            MessageContractTypes.ReplyBankingDataNotWrapped MessageContractRequestReplyNotWrapped(MessageContractTypes.RequestBankingData bt);
        }
    }

    public class MessageContractTypes
    {
        [MessageContract(IsWrapped = true, WrapperName = "CustomWrapperName", WrapperNamespace = "http://www.contoso.com")]
        public class RequestBankingData
        {
            [MessageBodyMember(Order = 1, Name = "Date_of_Request")]
            public DateTime transactionDate;
            [MessageBodyMember(Name = "Customer_Name", Namespace = "CustomNamespace", Order = 3)]
            public string accountName;
            [MessageBodyMember(Order = 2, Name = "Transaction_Amount")]
            public int amount;
        }

        [MessageContract(IsWrapped = true, WrapperName = "CustomWrapperName", WrapperNamespace = "http://www.contoso.com")]
        public class ReplyBankingData
        {
            [MessageBodyMember(Order = 1, Name = "Date_of_Request")]
            public DateTime transactionDate;
            [MessageBodyMember(Name = "Customer_Name", Namespace = "http://www.contoso.com", Order = 3)]
            public string accountName;
            [MessageBodyMember(Order = 2, Name = "Transaction_Amount")]
            public int amount;
        }

        [MessageContract(IsWrapped = false, WrapperName = "CustomWrapperName", WrapperNamespace = "http://www.contoso.com")]
        public class ReplyBankingDataNotWrapped
        {
            [MessageBodyMember(Order = 1, Name = "Date_of_Request")]
            public DateTime transactionDate;
            [MessageBodyMember(Name = "Customer_Name", Namespace = "http://www.contoso.com", Order = 3)]
            public string accountName;
            [MessageBodyMember(Order = 2, Name = "Transaction_Amount")]
            public int amount;
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
