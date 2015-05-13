// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
