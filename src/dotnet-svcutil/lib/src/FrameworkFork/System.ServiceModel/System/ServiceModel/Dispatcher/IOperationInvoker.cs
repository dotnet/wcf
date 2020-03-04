// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Threading.Tasks;

namespace System.ServiceModel.Dispatcher
{
    public interface IOperationInvoker
    {
        object[] AllocateInputs();
        IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state);
        object InvokeEnd(object instance, out object[] outputs, IAsyncResult result);
    }
}
