// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
