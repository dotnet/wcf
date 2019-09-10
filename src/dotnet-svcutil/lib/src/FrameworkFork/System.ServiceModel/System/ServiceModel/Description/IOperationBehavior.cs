// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
