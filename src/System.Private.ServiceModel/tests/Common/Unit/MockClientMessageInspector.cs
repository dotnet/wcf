// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

public delegate void AfterReceiveReplyDelegate(ref Message reply, object correlationState);
public delegate object BeforeSendRequestDelegate(ref Message request, IClientChannel channel);

public class MockClientMessageInspector : IClientMessageInspector, IEndpointBehavior
{
    public MockClientMessageInspector()
    {
        ValidateOverride = DefaultValidate;
        AddBindingParametersOverride = DefaultAddBindingParameters;
        ApplyDispatchBehaviorOverride = DefaultApplyDispatchBehavior;
        ApplyClientBehaviorOverride = DefaultApplyClientBehavior;

        AfterReceiveReplyOverride = DefaultAfterReceiveReply;
        BeforeSendRequestOverride = DefaultBeforeSendRequest;
    }

    public Action<ServiceEndpoint> ValidateOverride { get; set; }
    public Action<ServiceEndpoint, BindingParameterCollection> AddBindingParametersOverride { get; set; }
    public Action<ServiceEndpoint, EndpointDispatcher> ApplyDispatchBehaviorOverride { get; set; }
    public Action<ServiceEndpoint, ClientRuntime> ApplyClientBehaviorOverride { get; set; }

    public AfterReceiveReplyDelegate AfterReceiveReplyOverride { get; set; }
    public BeforeSendRequestDelegate BeforeSendRequestOverride { get; set; }

    public void Validate(ServiceEndpoint endpoint)
    {
        ValidateOverride(endpoint);
    }
    public void DefaultValidate(ServiceEndpoint endpoint)
    {
    }

    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
        AddBindingParametersOverride(endpoint, bindingParameters);
    }

    public void DefaultAddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {
        ApplyDispatchBehaviorOverride(endpoint, endpointDispatcher);
    }

    public void DefaultApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        ApplyClientBehaviorOverride(endpoint, clientRuntime);
    }
    public void DefaultApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
    }

    public void AfterReceiveReply(ref Message reply, object correlationState)
    {
        AfterReceiveReplyOverride(ref reply, correlationState);
    }

    public void DefaultAfterReceiveReply(ref Message reply, object correlationState)
    {
    }

    public object BeforeSendRequest(ref Message request, IClientChannel channel)
    {
        return BeforeSendRequestOverride(ref request, channel);
    }

    public object DefaultBeforeSendRequest(ref Message request, IClientChannel channel)
    {
        return null;
    }
}


