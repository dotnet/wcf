// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;

namespace System.Runtime
{
    internal static class InternalSR
    {
#pragma warning disable IDE1006 // Naming Styles
        internal static readonly string AsyncCallbackThrewException = "Async Callback threw an exception.";
        internal static readonly string AsyncResultAlreadyEnded = "End cannot be called twice on an AsyncResult.";
        internal static readonly string InvalidAsyncResult = "An incorrect IAsyncResult was provided to an 'End' method. The IAsyncResult object passed to 'End' must be the one returned from the matching 'Begin' or passed to the callback provided to 'Begin'.";
        internal static readonly string InvalidNullAsyncResult = "A null value was returned from an async 'Begin' method or passed to an AsyncCallback. Async 'Begin' implementations must return a non-null IAsyncResult and pass the same IAsyncResult object as the parameter to the AsyncCallback.";
        internal static readonly string InvalidAsyncResultImplementationGeneric = "An incorrect implementation of the IAsyncResult interface may be returning incorrect values from the CompletedSynchronously property or calling the AsyncCallback more than once.";

        internal static string InvalidAsyncResultImplementation(object param0) => $"An incorrect implementation of the IAsyncResult interface may be returning incorrect values from the CompletedSynchronously property or calling the AsyncCallback more than once. The type {param0} could be the incorrect implementation.";
#pragma warning restore IDE1006 // Naming Styles
    }
}
