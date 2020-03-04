// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections;

namespace System.ServiceModel.Dispatcher
{
    public interface IParameterInspector
    {
        object BeforeCall(string operationName, object[] inputs);
        void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState);
    }
}
