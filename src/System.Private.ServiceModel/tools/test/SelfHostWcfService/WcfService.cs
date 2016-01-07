// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfService
{
    internal class WcfService : IWcfService
    {
        public String EchoWithTimeout(String message, TimeSpan serviceOperationTimeout)
        {
            System.Threading.Thread.Sleep(serviceOperationTimeout);
            return message;
        }

        public Message MessageRequestReply(Message message)
        {
            var reader = message.GetReaderAtBodyContents();

            Message requestmessage = Message.CreateMessage(
                message.Version,
                "http://tempuri.org/IWcfService/MessageRequestReplyResponse",
                reader.ReadString() + "[service] Request received, this is my Reply.");

            return requestmessage;
        }

        public String Echo(String message)
        {
            return message;
        }

        public ComplexCompositeType EchoComplex(ComplexCompositeType message)
        {
            return message;
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public MahojongTypes.ResultObject<string> UserGetAuthToken(string liveId)
        {
            MahojongTypes.ResultObject<string> result = new MahojongTypes.ResultObject<string>();
            result.Result = "Received request from the client.";
            return result;
        }

        public Dictionary<string, string> ValidateMessagePropertyHeaders()
        {
            Dictionary<string, string> headerCollection = new Dictionary<string, string>();
            try
            {
                HttpRequestMessageProperty property;
                object obj;
                MessageProperties properties = new MessageProperties(OperationContext.Current.IncomingMessageProperties);
                if (properties.TryGetValue(HttpRequestMessageProperty.Name, out obj))
                {
                    property = obj as HttpRequestMessageProperty;
                    WebHeaderCollection collection = property.Headers;

                    string[] headers = collection.AllKeys;
                    foreach (string s in headers)
                    {
                        string[] values = collection.GetValues(s);
                        headerCollection.Add(s, String.Join(",", values));
                    }
                }
                else
                {
                    headerCollection.Add("ERROR", "No HttpRequestMessageProperty was found!");
                }
            }
            catch (Exception ex)
            {
                headerCollection.Add("ERROR", string.Format("An exception was thrown: {0}", ex.Message));
            }

            return headerCollection;
        }

        // This operation is called by the Mahjong async scenario
        public MahojongTypes.ResultObject<IEnumerable<MahojongTypes.UserGamePlay>> UserGamePlayGetList(string gameKey, string keys)
        {
            MahojongTypes.ResultObject<IEnumerable<MahojongTypes.UserGamePlay>> result = new MahojongTypes.ResultObject<IEnumerable<MahojongTypes.UserGamePlay>>();
            result.ErrorCode = (int)MahojongTypes.ErrorCode.Ok;
            result.ErrorMessage = MahojongTypes.ErrorMessage.GetErrorDescription((MahojongTypes.ErrorCode)result.ErrorCode);
            result.HttpStatusCode = HttpStatusCode.OK;

            MahojongTypes.UserGamePlay gameData = new MahojongTypes.UserGamePlay();
            gameData.GameKey = "This is the GameKey property.";
            gameData.Key = "This is the Key property.";
            gameData.TimeStamp = "This is the TimeStamp property.";
            gameData.UserId = "This is the UserId property.";
            gameData.Value = "This is the Value property.";

            result.Result = new List<MahojongTypes.UserGamePlay>() { gameData };
            return result;
        }

        public void TestFault(string faultMsg)
        {
            throw new FaultException<FaultDetail>(new FaultDetail(faultMsg));
        }
        public void TestFaultInt(int faultCode)
        {
            throw new FaultException<int>(faultCode);
        }

        public void ThrowInvalidOperationException(string message)
        {
            throw new InvalidOperationException(message);
        }

        public ReplyBankingData MessageContractRequestReply(RequestBankingData bt)
        {
            ReplyBankingData bankingData = new ReplyBankingData();
            bankingData.accountName = bt.accountName;
            bankingData.transactionDate = bt.transactionDate;
            bankingData.amount = bt.amount;

            return bankingData;
        }

        public ReplyBankingDataNotWrapped MessageContractRequestReplyNotWrapped(RequestBankingData bt)
        {
            ReplyBankingDataNotWrapped bankingData = new ReplyBankingDataNotWrapped();
            bankingData.accountName = bt.accountName;
            bankingData.transactionDate = bt.transactionDate;
            bankingData.amount = bt.amount;

            return bankingData;
        }

        public ReplyBankingDataWithMessageHeader MessageContractRequestReplyWithMessageHeader(RequestBankingData bt)
        {
            ReplyBankingDataWithMessageHeader bankingData = new ReplyBankingDataWithMessageHeader();
            bankingData.accountName = bt.accountName;
            bankingData.transactionDate = bt.transactionDate;
            bankingData.amount = bt.amount;
            bankingData.extraValues = bt.accountName;

            return bankingData;
        }

        public ReplyBankingDataWithMessageHeaderNotNecessaryUnderstood MessageContractRequestReplyWithMessageHeaderNotNecessaryUnderstood(RequestBankingData bt)
        {
            ReplyBankingDataWithMessageHeaderNotNecessaryUnderstood bankingData = new ReplyBankingDataWithMessageHeaderNotNecessaryUnderstood();
            bankingData.accountName = bt.accountName;
            bankingData.transactionDate = bt.transactionDate;
            bankingData.amount = bt.amount;
            bankingData.extraValues = bt.accountName;

            return bankingData;
        }

        public TestHttpRequestMessageProperty EchoHttpRequestMessageProperty()
        {
            object obj;
            MessageProperties properties = new MessageProperties(OperationContext.Current.IncomingMessageProperties);
            if (properties.TryGetValue(HttpRequestMessageProperty.Name, out obj))
            {
                HttpRequestMessageProperty property = (HttpRequestMessageProperty)obj;
                if (property != null)
                {
                    TestHttpRequestMessageProperty testProperty = new TestHttpRequestMessageProperty();
                    testProperty.SuppressEntityBody = property.SuppressEntityBody;
                    testProperty.Method = property.Method;
                    testProperty.QueryString = property.QueryString;

                    WebHeaderCollection collection = property.Headers;
                    foreach (string s in collection.AllKeys)
                    {
                        string[] values = collection.GetValues(s);
                        testProperty.Headers.Add(s, String.Join(",", values));
                    }
                    return testProperty;
                }
            }

            return null;
        }

        public string GetRestartServiceEndpoint()
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            Guid guid = Guid.NewGuid();
            string localHost = "http://localhost";
            string path = "/WindowsCommunicationFoundation/" + guid.ToString();

            ServiceHost host = new ServiceHost(typeof(WcfRestartService));
            host.AddServiceEndpoint(typeof(IWcfRestartService), binding, localHost + path);
            host.Open();

            // Add the ServiceHost instance to a static dictionary so that it can be used by restart service operation to close the ServiceHost
            WcfRestartService.serviceHostDictionary.Add(guid, host);

            // Return the unique endpoint for this ServiceHost instance of the WcfRestartService
            return "http://[HOST]" + path;
        }

        public string GetRequestCustomHeader(string customHeaderName, string customHeaderNamespace)
        {
            string value = null;
            MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;
            // look at headers on incoming message
            for (int i = 0; i < headers.Count; ++i)
            {
                MessageHeaderInfo h = headers[i];
                if (h.Name == customHeaderName &&
                    h.Namespace == customHeaderNamespace)
                {
                    System.Xml.XmlReader xr = headers.GetReaderAtHeader(i);
                    value = xr.ReadElementContentAsString();
                    return value;
                }
            }

            return value;
        }

        public Dictionary<string, string> GetIncomingMessageHeaders()
        {
            Dictionary<string, string> infos = new Dictionary<string, string>();
            MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;
            // look at headers on incoming message
            for (int i = 0; i < headers.Count; ++i)
            {
                MessageHeaderInfo h = headers[i];
                System.Xml.XmlReader xr = headers.GetReaderAtHeader(i);
                string value = xr.ReadElementContentAsString();
                infos.Add(string.Format("{0}//{1}", h.Namespace, h.Name), value);
            }

            return infos;
        }

        public string EchoXmlSerializerFormat(string message)
        {
            return message;
        }

        public string EchoXmlSerializerFormatSupportFaults(string message, bool pleaseThrowException)
        {
            if (!pleaseThrowException)
            {
                return message;
            }
            else
            {
                throw new Exception(message);
            }
        }

        public string EchoXmlSerializerFormatUsingRpc(string message)
        {
            return message;
        }

        public XmlCompositeType GetDataUsingXmlSerializer(XmlCompositeType composite)
        {
            composite.StringValue = composite.StringValue;
            return composite;
        }
        
        public LoginResponse Login(LoginRequest request)
        {
            var response = new LoginResponse();
            response.@return = request.clientId + request.user + request.pwd;
            return response;
        }

        public object[] EchoItems(object[] objects)
        {
            return objects;
        }

        public object[] EchoItems_Xml(object[] objects)
        {
            return objects;
        }

        public XmlVeryComplexType EchoXmlVeryComplexType(XmlVeryComplexType complex)
        {
            return complex;
        }

        public Stream GetStreamFromString(string data)
        {
            return StringToStream(data);
        }

        public string GetStringFromStream(Stream stream)
        {
            string data = StreamToString(stream);

            return data;
        }

        public Stream EchoStream(Stream stream)
        {
            string data = StreamToString(stream);
            return StringToStream(data);            
        }

        public string EchoMessageParameter(string name)
        {
            return string.Format("Hello {0}", name);
        }

        private static string StreamToString(Stream stream)
        {
            var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        private static Stream StringToStream(string str)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms, Encoding.UTF8);

            sw.Write(str);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }
    }
}
