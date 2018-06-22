// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;

// IMockCommunicationObject allows caller to provide delegate to intercept
// every abstract and virtual method.  Each has a corresponding Defaultxxx()
// method that does what the default Communication object would do, allowing
// the caller to do processing before and after the default behavior.
public interface IMockCommunicationObject : ICommunicationObject
{
    // Delegates tests can override
    Func<TimeSpan> DefaultCloseTimeoutOverride { get; set; }
    Func<TimeSpan> DefaultOpenTimeoutOverride { get; set; }
    Action OnAbortOverride { get; set; }
    Func<TimeSpan, AsyncCallback, object, IAsyncResult> OnBeginCloseOverride { get; set; }
    Func<TimeSpan, AsyncCallback, object, IAsyncResult> OnBeginOpenOverride { get; set; }
    Action<TimeSpan> OnOpenOverride { get; set; }
    Action<TimeSpan> OnCloseOverride { get; set; }
    Action<IAsyncResult> OnEndCloseOverride { get; set; }
    Action<IAsyncResult> OnEndOpenOverride { get; set; }

    Action OnOpeningOverride { get; set; }
    Action OnOpenedOverride { get; set; }
    Action OnClosingOverride { get; set; }
    Action OnClosedOverride { get; set; }
    Action OnFaultedOverride { get; set; }

    // Default behaviors tests can call
    void DefaultOnAbort();
    void DefaultOnOpening();
    void DefaultOnOpen(TimeSpan timeout);
    void DefaultOnOpened();
    void DefaultOnClosing();
    void DefaultOnClose(TimeSpan timeout);
    void DefaultOnClosed();

    IAsyncResult DefaultOnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state);
    void DefaultOnEndOpen(IAsyncResult result);

    IAsyncResult DefaultOnBeginClose(TimeSpan timeout, AsyncCallback callback, object state);
    void DefaultOnEndClose(IAsyncResult result);
}