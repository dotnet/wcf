// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Web;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting.Server.Features;
#else
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
#endif
using System.Net;
using System.Security.Principal;
using System.Text;

namespace WcfService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class WcfService : IWcfService
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

        public MessageInspector_CustomHeaderAuthentication.ResultObject<string> UserGetAuthToken()
        {
            MessageInspector_CustomHeaderAuthentication.ResultObject<string> resultObject = null;

            try
            {
                HttpRequestMessageProperty property;
                object obj;
                MessageProperties properties = new MessageProperties(OperationContext.Current.IncomingMessageProperties);
                if (properties.TryGetValue(HttpRequestMessageProperty.Name, out obj))
                {
                    property = obj as HttpRequestMessageProperty;
                    WebHeaderCollection collection = property.Headers;
                    string authValue = collection.Get(Enum.GetName(typeof(HttpRequestHeader), HttpRequestHeader.Authorization));

                    if (authValue == "Not Allowed")
                    {
                        resultObject = MessageInspector_CustomHeaderAuthentication.ResultObject<string>.CreateFailureObject<string>();
                        resultObject.Result = resultObject.ResultMessage;
                    }
                    else if (authValue == "Allow")
                    {
                        resultObject = MessageInspector_CustomHeaderAuthentication.ResultObject<string>.CreateSuccessObject<string>();
                        resultObject.Result = resultObject.ResultMessage;
                    }
                }
                else
                {
                    resultObject = MessageInspector_CustomHeaderAuthentication.ResultObject<string>.CreateFailureObject<string>();
                    resultObject.Result = "ERROR";
                    resultObject.ResultMessage = "No HttpRequestMessageProperty was found on the incoming Message.";
                }
            }
            catch (Exception ex)
            {
                resultObject = MessageInspector_CustomHeaderAuthentication.ResultObject<string>.CreateFailureObject<string>();
                resultObject.Result = ex.ToString();
                resultObject.ResultMessage = ex.Message;
            }

            return resultObject;
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

        public void TestFault(string faultMsg)
        {
            throw new FaultException<FaultDetail>(new FaultDetail(faultMsg));
        }

        public void TestFaultInt(int faultCode)
        {
            throw new FaultException<int>(faultCode);
        }
        public void TestFaults(string faultMsg, bool throwFaultDetail)
        {
            if (throwFaultDetail)
            {
                throw new FaultException<FaultDetail>(new FaultDetail(faultMsg));
            }
            else
            {
                throw new FaultException<FaultDetail2>(new FaultDetail2(faultMsg));
            }
        }
        public object[] TestFaultWithKnownType(string faultMsg, object[] objects)
        {
            if (objects == null)
            {
                throw new FaultException<FaultDetail>(new FaultDetail(faultMsg));
            }
            return objects;
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

#if NET
        public string GetRestartServiceEndpoint()
        {
            Guid guid = Guid.NewGuid();
            string localHost = "http://localhost:0";
            string path = "WindowsCommunicationFoundationTest";
            string endpointAddress = $"/{path}/{guid}";

            IWebHost host = WebHost.CreateDefaultBuilder().UseKestrel(options =>
            {
                options.Listen(IPAddress.Loopback, 0);
            }).ConfigureServices(services =>
            {
                services.AddServiceModelServices();

            }).Configure(app =>
            {
                string serviceEndpointAddress = $"{localHost}/{path}/{guid}";
                app.UseServiceModel(builder =>
                {
                    builder.AddService<WcfRestartService>()
                    .AddServiceEndpoint<WcfRestartService, IWcfRestartService>(new BasicHttpBinding(BasicHttpSecurityMode.None), serviceEndpointAddress);
                });
            }).Build();

            host.Start();

            // Add the WebHost instance to a static dictionary so that it can be used by restart service operation to close the WebHost
            WcfRestartService.webHostDictionary.Add(guid, host);

            var address = host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.FirstOrDefault();
            var port = new Uri(address).Port;

            // Return the unique endpoint for this WebHost instance of the WcfRestartService
            return "http://[HOST]:" + port + endpointAddress;
        }

#else
        public string GetRestartServiceEndpoint()
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            Guid guid = Guid.NewGuid();
            string localHost = "http://localhost";
            string path = "/WindowsCommunicationFoundationTest/" + WindowsIdentity.GetCurrent().Name.Split('\\').Last() + "/" + guid.ToString();
            
            ServiceHost host = new ServiceHost(typeof(WcfRestartService));
            host.AddServiceEndpoint(typeof(IWcfRestartService), binding, localHost + path);
            host.Open();

            // Add the ServiceHost instance to a static dictionary so that it can be used by restart service operation to close the ServiceHost
            WcfRestartService.serviceHostDictionary.Add(guid, host);

            // Return the unique endpoint for this ServiceHost instance of the WcfRestartService
            return "http://[HOST]" + path;
        }
#endif

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

        public string GetIncomingMessageHeadersMessage(string customHeaderName, string customHeaderNS)
        {
            MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;
            MesssageHeaderCreateHeaderWithXmlSerializerTestType header = headers.GetHeader<MesssageHeaderCreateHeaderWithXmlSerializerTestType>(customHeaderName, customHeaderNS);
            if (header != null)
            {
                return header.Message;
            }

            return string.Empty;
        }

        public XmlMessageContractTestResponse EchoMessageResponseWithMessageHeader(XmlMessageContractTestRequest request)
        {
            var result = new XmlMessageContractTestResponse(request.Message);
            return result;
        }

        public XmlMessageContractTestResponse EchoMessageResquestWithMessageHeader(XmlMessageContractTestRequestWithMessageHeader request)
        {
            var result = new XmlMessageContractTestResponse(request.Message);
            return result;
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

        public object[] EchoItems_Xml1(object[] objects)
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

        public void ReturnContentType(string contentType)
        {
            var outgoingMessageProperties = OperationContext.Current.OutgoingMessageProperties;
            object httpResponseMessagePropertyObj;
            if (!outgoingMessageProperties.TryGetValue(HttpResponseMessageProperty.Name, out httpResponseMessagePropertyObj))
            {
                httpResponseMessagePropertyObj = new HttpResponseMessageProperty();
                outgoingMessageProperties.Add(HttpResponseMessageProperty.Name, httpResponseMessagePropertyObj);
            }

            var httpRespononseMessageProperty = (HttpResponseMessageProperty)httpResponseMessagePropertyObj;
            httpRespononseMessageProperty.Headers[HttpResponseHeader.ContentType] = contentType;
        }

        public string EchoCookie()
        {
            string cookie = WebOperationContext.Current.IncomingRequest.Headers[HttpRequestHeader.Cookie];
            return cookie;
        }

        // Returns the current time and also sets the cookie named 'name' to be the same time returned.
        public string EchoTimeAndSetCookie(string name)
        {
            string cookie = WebOperationContext.Current.IncomingRequest.Headers[HttpRequestHeader.Cookie];
            string value = DateTime.Now.ToString();
            cookie = string.Format("{0}={1}", name, value);
            WebOperationContext.Current.OutgoingResponse.Headers.Add(string.Format("Set-Cookie: {0};", cookie));
            return value;
        }

        public bool IsHttpKeepAliveDisabled()
        {
            MessageProperties properties = new MessageProperties(OperationContext.Current.IncomingMessageProperties);
            var property = (HttpRequestMessageProperty)properties[HttpRequestMessageProperty.Name];
            WebHeaderCollection collection = property.Headers;
            string connectionValue = collection.Get(Enum.GetName(typeof(HttpRequestHeader), HttpRequestHeader.Connection));
            return connectionValue.Equals("Close", StringComparison.OrdinalIgnoreCase);
        }

        public Dictionary<string, string> GetRequestHttpHeaders()
        {
            var headers = new Dictionary<string, string>();
            object httpReqMessagePropObj;
            if (OperationContext.Current.IncomingMessageProperties.TryGetValue(HttpRequestMessageProperty.Name,
                out httpReqMessagePropObj))
            {
                var httpRequestMessageProperty = (HttpRequestMessageProperty)httpReqMessagePropObj;
                foreach (var headerName in httpRequestMessageProperty.Headers.AllKeys)
                {
                    headers.Add(headerName, httpRequestMessageProperty.Headers[headerName]);
                }
            }

            return headers;
        }

        private static string StreamToString(Stream stream)
        {
            var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        private static Stream StringToStream(string str)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms, new UTF8Encoding(false));

            sw.Write(str);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

        public System.Threading.Tasks.Task EchoReturnTask()
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                System.Threading.Tasks.Task.Delay(1).Wait();
            });
        }
    }
}
