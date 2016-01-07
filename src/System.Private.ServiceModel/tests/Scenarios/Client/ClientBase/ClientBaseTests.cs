// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using Xunit;

public static class ClientBaseTests
{
    [Fact]
    [OuterLoop]
    public static void MessageProperty_HttpRequestMessageProperty_RoundTrip_Verify()
    {
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            MyClientBase<IWcfService> client = new MyClientBase<IWcfService>(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
            client.Endpoint.EndpointBehaviors.Add(new ClientMessagePropertyBehavior());

        try
        {
            IWcfService serviceProxy = client.ChannelFactory.CreateChannel();
            TestHttpRequestMessageProperty property = serviceProxy.EchoHttpRequestMessageProperty();

            Assert.NotNull(property);
            Assert.True(property.SuppressEntityBody == false, "Expected SuppressEntityBody to be 'false'");
            Assert.Equal("POST", property.Method);
            Assert.Equal("My%20address", property.QueryString);
            Assert.True(property.Headers.Count > 0, "TestHttpRequestMessageProperty.Headers should not have empty headers");
            Assert.Equal("my value", property.Headers["customer"]);
            }
        finally
            {
            if (client != null && client.State != CommunicationState.Closed)
                {
                client.Abort(); 
                }
                }
                }

    [Fact]
    [OuterLoop]
    public static void ClientMessageInspector_Verify_Invoke()
    {
        // This test verifies ClientMessageInspector can be added to the client endpoint behaviors
        // and this is it called properly when a message is sent.

            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            MyClientBase client = new MyClientBase(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));

            // Add the ClientMessageInspector and give it an instance where it can record what happens when it is called.
            ClientMessageInspectorData data = new ClientMessageInspectorData();
            client.Endpoint.EndpointBehaviors.Add(new ClientMessageInspectorBehavior(data));

        try
        {
            IWcfServiceGenerated serviceProxy = client.ChannelFactory.CreateChannel();

            // This proxy call should invoke the client message inspector
            string result = serviceProxy.Echo("Hello");

            Assert.Equal("Hello", result);
            Assert.True(data.BeforeSendRequestCalled, "BeforeSendRequest should have been called");
            Assert.True(data.Request != null, "Did not call pass Request to BeforeSendRequest");
            Assert.True(data.Channel != null, "Did not call pass Channel to BeforeSendRequest");
            Assert.True(data.AfterReceiveReplyCalled, "AfterReceiveReplyCalled should have been called");
            Assert.True(data.Reply != null, "Did not call pass Reply to AfterReceiveReplyCalled");
        }
        finally
            {
            if (client != null && client.State != CommunicationState.Closed)
            {
                client.Abort();
            }
        }
    }

    [Fact]
    [OuterLoop]
    public static void ClientBaseOfT_Sync_RoundTrip_Check_CommunicationState()
            {
        CustomBinding customBinding = new CustomBinding();
        customBinding.Elements.Add(new TextMessageEncodingBindingElement());
        customBinding.Elements.Add(new HttpTransportBindingElement());

        MyClientBase client = new MyClientBase(customBinding, new EndpointAddress(Endpoints.HttpSoap12_Address));
        Assert.Equal(CommunicationState.Created, client.State);

        IWcfServiceGenerated serviceProxy = client.ChannelFactory.CreateChannel();
        Assert.Equal(CommunicationState.Opened, client.State);

        try
            {
            string result = serviceProxy.Echo("Hello");
            Assert.Equal(CommunicationState.Opened, client.State);

            ((ICommunicationObject)client).Close();
            Assert.Equal(CommunicationState.Closed, client.State);
            }
        finally
            {
            // normally we'd also check for if (client != null && client.State != CommuncationState.Closed), 
            // but this is a test and it'd be good to have the Abort happen and the channel is still Closed
            if (client != null)
            {
                client.Abort();
                Assert.Equal(CommunicationState.Closed, client.State);
        }
        }
    }

    [Fact]
    [OuterLoop]
    public static void ClientBaseOfT_Sync_RoundTrip_Call_Using_HttpTransport()
        {
        // This test verifies ClientBase<T> can be used to create a proxy and invoke an operation over Http

            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            MyClientBase client = new MyClientBase(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
            IWcfServiceGenerated serviceProxy = client.ChannelFactory.CreateChannel();

        try
        {
            string result = serviceProxy.Echo("Hello");
            Assert.Equal("Hello", result);
        }
        finally
        {
            if (client != null && client.State != CommunicationState.Closed)
            {
                client.Abort();
            }
        }
            }

    [Fact]
    [OuterLoop]
    public static void ClientBaseOfT_Sync_RoundTrip_Call_Using_NetTcpTransport()
    {
        // This test verifies ClientBase<T> can be used to create a proxy and invoke an operation over Tcp
        // (request reply over Tcp) 

        // This test verifies ClientBase<T> can be used to create a proxy and invoke an operation over Http

        CustomBinding binding = new CustomBinding(
                new TextMessageEncodingBindingElement(),
                new TcpTransportBindingElement());

        MyClientBase client = new MyClientBase(binding, new EndpointAddress(Endpoints.Tcp_CustomBinding_NoSecurity_Text_Address));
        IWcfServiceGenerated serviceProxy = client.ChannelFactory.CreateChannel();

        try
        {
            string result = serviceProxy.Echo("Hello");
            Assert.Equal("Hello", result);
        }
        finally
        {
            if (client != null && client.State != CommunicationState.Closed)
        {
                client.Abort();
            }
        }
    }

    [Fact]
    [OuterLoop]
    public static void OperationContextScope_HttpRequestCustomMessageHeader_RoundTrip_Verify()
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        MyClientBase<IWcfService> client = new MyClientBase<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
        IWcfService serviceProxy = client.ChannelFactory.CreateChannel();

        string customHeaderName = "OperationContextScopeCustomHeader";
        string customHeaderNS = "http://tempuri.org/OperationContextScope_HttpRequestCustomMessageHeader_RoundTrip_Verify";
        string customHeaderValue = "CustomHappyValue";
        try
        {
            using (OperationContextScope scope = new OperationContextScope((IContextChannel)serviceProxy))
            {
                MessageHeader header
                  = MessageHeader.CreateHeader(
                  customHeaderName,
                  customHeaderNS,
                  customHeaderValue
                  );
                OperationContext.Current.OutgoingMessageHeaders.Add(header);

                Dictionary<string, string> incomingMessageHeaders = serviceProxy.GetIncomingMessageHeaders();
                string result = GetHeader(customHeaderName, customHeaderNS, incomingMessageHeaders);

                Assert.Equal(customHeaderValue, result);
            }

            //Call outside of scope should not have the custom header
            Dictionary<string, string> outofScopeIncomingMessageHeaders = serviceProxy.GetIncomingMessageHeaders();
            string outofScopeResult = GetHeader(customHeaderName, customHeaderNS, outofScopeIncomingMessageHeaders);
            Assert.True(string.Empty == outofScopeResult, string.Format("Expect call out of the OperationContextScope does not have the custom header {0}", customHeaderName));
            ((ICommunicationObject)client).Close();
        }
        finally
        {
            if (client != null && client.State != CommunicationState.Closed)
            {
                client.Abort();
            }
        }
    }

    private static string GetHeader(string customHeaderName, string customHeaderNamespace, Dictionary<string, string> messageHeaders)
    {
        // look at headers on incoming message
        foreach (KeyValuePair<string, string> keyValue in messageHeaders)
        {
            string headerFullName = keyValue.Key;
            if (headerFullName == string.Format("{0}//{1}", customHeaderNamespace, customHeaderName))
            {
                return keyValue.Value;
            }
        }

        return string.Empty;
    }

    private class ClientMessagePropertyBehavior : IEndpointBehavior
    {
        private ClientMessagePropertyInspector _inspector;

        public ClientMessagePropertyBehavior()
        {
            _inspector = new ClientMessagePropertyInspector();
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(_inspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    private class ClientMessagePropertyInspector : IClientMessageInspector
    {
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequestMessage = new HttpRequestMessageProperty();
            httpRequestMessage.Headers["customer"] = "my value";
            httpRequestMessage.SuppressEntityBody = false;
            httpRequestMessage.Method = "POST";
            httpRequestMessage.QueryString = "My address";

            request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);

            return null;
        }
    }

    public class ClientMessageInspectorBehavior : IEndpointBehavior
    {
        private ClientMessageInspector _inspector;

        public ClientMessageInspectorBehavior(ClientMessageInspectorData data)
        {
            _inspector = new ClientMessageInspector(data);
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(_inspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class ClientMessageInspector : IClientMessageInspector
    {
        private ClientMessageInspectorData _data;
        public ClientMessageInspector(ClientMessageInspectorData data)
        {
            _data = data;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            _data.AfterReceiveReplyCalled = true;
            _data.Reply = reply;
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            _data.BeforeSendRequestCalled = true;
            _data.Request = request;
            _data.Channel = channel;
            return null;
        }
    }

    public class ClientMessageInspectorData
    {
        public bool BeforeSendRequestCalled { get; set; }
        public bool AfterReceiveReplyCalled { get; set; }
        public Message Request { get; set; }
        public Message Reply { get; set; }
        public IClientChannel Channel { get; set; }
    }
}

