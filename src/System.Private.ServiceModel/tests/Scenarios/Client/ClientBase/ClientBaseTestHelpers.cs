// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public static class ClientBaseTestHelpers
{
    public static string GetHeader(string customHeaderName, string customHeaderNamespace, Dictionary<string, string> messageHeaders)
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

    public static void RegisterForEvents(ICommunicationObject co, List<string> eventsCalled, bool deregister = false)
    {
        EventHandler opening = (s, e) =>
        {
            eventsCalled.Add("Opening");
        };

        EventHandler opened = (s, e) =>
        {
            eventsCalled.Add("Opened");
        };

        EventHandler closing = (s, e) =>
        {
            eventsCalled.Add("Closing");
        };

        EventHandler closed = (s, e) =>
        {
            eventsCalled.Add("Closed");
        };

        co.Opening += opening;
        co.Opened += opened;
        co.Closing += closing;
        co.Closed += closed;

        // One test pivot involves ensuring we can both Add and Remove event handlers
        if (deregister)
        {
            co.Opening -= opening;
            co.Opened -= opened;
            co.Closing -= closing;
            co.Closed -= closed;
        }
    }
}

public class ClientMessagePropertyBehavior : IEndpointBehavior
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

public class ClientMessagePropertyInspector : IClientMessageInspector
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
    private ClientBaseMessageInspector _inspector;

    public ClientMessageInspectorBehavior(ClientMessageInspectorData data)
    {
        _inspector = new ClientBaseMessageInspector(data);
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

public class ClientBaseMessageInspector : IClientMessageInspector
{
    private ClientMessageInspectorData _data;
    public ClientBaseMessageInspector(ClientMessageInspectorData data)
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

