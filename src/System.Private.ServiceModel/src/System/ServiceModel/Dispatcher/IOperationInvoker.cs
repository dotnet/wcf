// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.ServiceModel.Dispatcher
{
    public interface IOperationInvoker
    {
        object[] AllocateInputs();
        IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state);
        object InvokeEnd(object instance, out object[] outputs, IAsyncResult result);
    }
}
