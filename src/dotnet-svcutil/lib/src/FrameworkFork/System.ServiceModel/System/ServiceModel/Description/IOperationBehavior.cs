// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Collections.Generic;

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
