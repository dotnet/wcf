// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Description
{
    public interface IOperationBehavior
    {
        void Validate(OperationDescription operationDescription);
        void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation);
        void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation);
        void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters);
    }
}
