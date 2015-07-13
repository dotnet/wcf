// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            MyClientBase<IWcfService> client = new MyClientBase<IWcfService>(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
            client.Endpoint.EndpointBehaviors.Add(new ClientMessagePropertyBehavior());
            IWcfService serviceProxy = client.ChannelFactory.CreateChannel();
            TestHttpRequestMessageProperty property = serviceProxy.EchoHttpRequestMessageProperty();
            if (property == null)
            {
                errorBuilder.AppendLine("Null HttpRequestMessageProperty returned");
            }
            else
            {
                if (property.SuppressEntityBody != false)
                {
                    errorBuilder.AppendLine("Expected SuppressEntityBody: false, actual: " + property.SuppressEntityBody);
                }
                if (property.Method != "POST")
                {
                    errorBuilder.AppendLine("Expected Method: POST, actual: " + property.Method);
                }
                if (property.QueryString != "My%20address")
                {
                    errorBuilder.AppendLine("Expected QueryString: My%20address, actual: " + property.QueryString);
                }
                if (property.Headers.Count == 0)
                {
                    errorBuilder.AppendLine("Headers are empty");
                }
                else if (property.Headers["customer"] != "my value")
                {
                    errorBuilder.AppendLine("Expected customer header: my value, actual: " + property.Headers["customer"]);
                }
            }
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, String.Format("Test Scenario: MessageProperty_HttpRequestMessageProperty_RoundTrip_Verify FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void ClientMessageInspector_Verify_Invoke()
    {
        // This test verifies ClientMessageInspector can be added to the client endpoint behaviors
        // and this is it called properly when a message is sent.
        StringBuilder errorBuilder = new StringBuilder();
        try
        {
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            MyClientBase client = new MyClientBase(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));

            // Add the ClientMessageInspector and give it an instance where it can record what happens when it is called.
            ClientMessageInspectorData data = new ClientMessageInspectorData();
            client.Endpoint.EndpointBehaviors.Add(new ClientMessageInspectorBehavior(data));
            IWcfServiceGenerated serviceProxy = client.ChannelFactory.CreateChannel();

            // This proxy call should invoke the client message inspector
            string result = serviceProxy.Echo("Hello");
            if (!string.Equals(result, "Hello"))
            {
                errorBuilder.AppendLine(String.Format("Expected response from Service: {0} Actual was: {1}", "Hello", result));
            }

            if (!data.BeforeSendRequestCalled)
            {
                errorBuilder.AppendLine(String.Format("Did not call BeforeSendRequest"));
            }

            if (data.Request == null)
            {
                errorBuilder.AppendLine(String.Format("Did not call pass Request to BeforeSendRequest"));
            }

            if (data.Channel == null)
            {
                errorBuilder.AppendLine(String.Format("Did not call pass Channel to BeforeSendRequest"));
            }

            if (!data.AfterReceiveReplyCalled)
            {
                errorBuilder.AppendLine(String.Format("Did not call AfterReceiveReplyCalled"));
            }

            if (data.Reply == null)
            {
                errorBuilder.AppendLine(String.Format("Did not call pass Reply to AfterReceiveReplyCalled"));
            }
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: ClientMessageInspectorScenario FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void ClientBaseOfT_Call()
    {
        // This test verifies ClientBase<T> can be used to create a proxy and invoke an operation
        StringBuilder errorBuilder = new StringBuilder();
        try
        {
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            MyClientBase client = new MyClientBase(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
            IWcfServiceGenerated serviceProxy = client.ChannelFactory.CreateChannel();

            string result = serviceProxy.Echo("Hello");
            if (!string.Equals(result, "Hello"))
            {
                errorBuilder.AppendLine(String.Format("Expected response from Service: {0} Actual was: {1}", "Hello", result));
            }
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: ClientBaseOfTCall FAILED with the following errors: {0}", errorBuilder));
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

