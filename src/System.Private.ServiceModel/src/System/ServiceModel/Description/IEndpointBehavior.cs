// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Description
{
    public interface IEndpointBehavior
    {
        void Validate(ServiceEndpoint endpoint);
        void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters);
        void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher);
        void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime);
    }
}
