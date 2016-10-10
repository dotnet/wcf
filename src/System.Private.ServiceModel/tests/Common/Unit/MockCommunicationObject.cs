// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

// Mock CommunicationObject allows caller to provide delegate to intercept
// every abstract and virtual method.  Each has a corresponding Defaultxxx()
// method that does what the default Communication object would do, allowing
// the caller to do processing before and after the default behavior.
public class MockCommunicationObject : CommunicationObject, IMockCommunicationObject
{
    public MockCommunicationObject()
    {
        OpenAsyncResult = new MockAsyncResult();
        CloseAsyncResult = new MockAsyncResult();

        // Each overrideable method has a delegate property that
        // can be set to override it, please a default handler.

        // All the abstracts
        DefaultCloseTimeoutOverride = DefaultDefaultCloseTimeout;
        DefaultOpenTimeoutOverride = DefaultDefaultOpenTimeout;

        OnAbortOverride = DefaultOnAbort;
        OnOpenOverride = DefaultOnOpen;
        OnCloseOverride = DefaultOnClose;

        OnBeginOpenOverride = DefaultOnBeginOpen;
        OnEndOpenOverride = DefaultOnEndOpen;

        OnBeginCloseOverride = DefaultOnBeginClose;
        OnEndCloseOverride = DefaultOnEndClose;

        // All the virtuals
        OnOpeningOverride = DefaultOnOpening;
        OnOpenedOverride = DefaultOnOpened;
        OnClosingOverride = DefaultOnClosing;
        OnClosedOverride = DefaultOnClosed;
        OnFaultedOverride = DefaultOnFaulted;
    }

    public MockAsyncResult OpenAsyncResult { get; set; }
    public MockAsyncResult CloseAsyncResult { get; set; }

    // Abstract overrides
    public Func<TimeSpan> DefaultCloseTimeoutOverride { get; set; }
    public Func<TimeSpan> DefaultOpenTimeoutOverride { get; set; }
    public Action OnAbortOverride { get; set; }
    public Func<TimeSpan, AsyncCallback, object, IAsyncResult> OnBeginCloseOverride { get; set; }
    public Func<TimeSpan, AsyncCallback, object, IAsyncResult> OnBeginOpenOverride { get; set; }
    public Action<TimeSpan> OnOpenOverride { get; set; }
    public Action<TimeSpan> OnCloseOverride { get; set; }
    public Action<IAsyncResult> OnEndCloseOverride { get; set; }
    public Action<IAsyncResult> OnEndOpenOverride { get; set; }

    // Virtual overrides
    public Action OnOpeningOverride { get; set; }
    public Action OnOpenedOverride { get; set; }
    public Action OnClosingOverride { get; set; }
    public Action OnClosedOverride { get; set; }
    public Action OnFaultedOverride { get; set; }

    protected override TimeSpan DefaultCloseTimeout
    {
        get
        {
            return DefaultCloseTimeoutOverride();
        }
    }

    public TimeSpan DefaultDefaultCloseTimeout()
    {
        return TimeSpan.FromSeconds(30);
    }

    protected override TimeSpan DefaultOpenTimeout
    {
        get
        {
            return DefaultOpenTimeoutOverride();
        }
    }

    public TimeSpan DefaultDefaultOpenTimeout()
    {
        return TimeSpan.FromSeconds(30);
    }

    protected override void OnAbort()
    {
        OnAbortOverride();
    }

    public void DefaultOnAbort()
    {
        // abstract -- no base to call
    }

    protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
    {
        return OnBeginCloseOverride(timeout, callback, state);
    }

    public IAsyncResult DefaultOnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
    {
        // Modify the placeholder async result we already instantiated.
        CloseAsyncResult.Callback = callback;
        CloseAsyncResult.AsyncState = state;

        // The mock always Completes the IAsyncResult before handing it back.
        // This is done because the sync path has no access to this IAsyncResult
        // that happens behind the scenes.
        CloseAsyncResult.Complete();

        return CloseAsyncResult;
        // abstract -- no base to call
    }

    protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
    {
        return OnBeginOpenOverride(timeout, callback, state);
        // abstract -- no base to call
    }

    public IAsyncResult DefaultOnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
    {
        // Modify the placeholder async result we already instantiated.
        OpenAsyncResult.Callback = callback;
        OpenAsyncResult.AsyncState = state;

        // The mock always Completes the IAsyncResult before handing it back.
        // This is done because the sync path has no access to this IAsyncResult
        // that happens behind the scenes.
        OpenAsyncResult.Complete();

        return OpenAsyncResult;
        // abstract -- no base to call
    }

    protected override void OnClose(TimeSpan timeout)
    {
        OnCloseOverride(timeout);
    }

    public void DefaultOnClose(TimeSpan timeout)
    {
        // abstract -- no base to call
    }

    protected override void OnEndClose(IAsyncResult result)
    {
        OnEndCloseOverride(result);
    }

    public void DefaultOnEndClose(IAsyncResult result)
    {
        ((MockAsyncResult)result).Complete();
        // abstract -- no base to call
    }

    protected override void OnEndOpen(IAsyncResult result)
    {
        OnEndOpenOverride(result);
    }

    public void DefaultOnEndOpen(IAsyncResult result)
    {
        ((MockAsyncResult)result).Complete();
        // abstract -- no base to call
    }

    protected override void OnOpen(TimeSpan timeout)
    {
        OnOpenOverride(timeout);
    }

    public void DefaultOnOpen(TimeSpan timeout)
    {
        // abstract -- no base to call
    }

    // Virtuals
    protected override void OnOpening()
    {
        OnOpeningOverride();
    }

    public void DefaultOnOpening()
    {
        base.OnOpening();
    }

    protected override void OnOpened()
    {
        OnOpenedOverride();
    }

    public void DefaultOnOpened()
    {
        base.OnOpened();
    }

    protected override void OnClosing()
    {
        OnClosingOverride();
    }

    public void DefaultOnClosing()
    {
        base.OnClosing();
    }

    protected override void OnClosed()
    {
        OnClosedOverride();
    }

    public void DefaultOnClosed()
    {
        base.OnClosed();
    }

    protected override void OnFaulted()
    {
        OnFaultedOverride();
    }

    public void DefaultOnFaulted()
    {
        base.OnFaulted();
    }

    #region helpers

    public static void InterceptAllOpenMethods(IMockCommunicationObject mco, List<string> methodsCalled)
    {
        mco.OnOpeningOverride = () =>
        {
            methodsCalled.Add("OnOpening");
            mco.DefaultOnOpening();
        };

        mco.OnOpenOverride = (TimeSpan t) =>
        {
            methodsCalled.Add("OnOpen");
            mco.DefaultOnOpen(t);
        };

        mco.OnBeginOpenOverride = (TimeSpan t, AsyncCallback c, object s) =>
        {
            methodsCalled.Add("OnBeginOpen");
            return mco.DefaultOnBeginOpen(t, c, s);
        };

        mco.OnOpenedOverride = () =>
        {
            methodsCalled.Add("OnOpened");
            mco.DefaultOnOpened();
        };
    }

    // This helper will override all the open methods of the MockCommunicationObject
    // and record their names into the provided list in the order they are called.
    public static void InterceptAllCloseMethods(IMockCommunicationObject mco, List<string> methodsCalled)
    {
        mco.OnClosingOverride = () =>
        {
            methodsCalled.Add("OnClosing");
            mco.DefaultOnClosing();
        };

        mco.OnCloseOverride = (TimeSpan t) =>
        {
            methodsCalled.Add("OnClose");
            mco.DefaultOnClose(t);
        };

        mco.OnBeginCloseOverride = (TimeSpan t, AsyncCallback c, object s) =>
        {
            methodsCalled.Add("OnBeginClose");
            return mco.DefaultOnBeginClose(t, c, s);
        };

        mco.OnClosedOverride = () =>
        {
            methodsCalled.Add("OnClosed");
            mco.DefaultOnClosed();
        };

        // The OnAbort is considered one of the methods associated with close.
        mco.OnAbortOverride = () =>
        {
            methodsCalled.Add("OnAbort");
            mco.DefaultOnAbort();
        };
    }

    // Intercepts all the events expected to fire during an open
    public static void InterceptAllOpenEvents(IMockCommunicationObject mco, List<string> eventsFired)
    {
        mco.Opening += (s, ea) => eventsFired.Add("Opening");
        mco.Opened += (s, ea) => eventsFired.Add("Opened");
    }

    // Intercepts all the events expected to fire during a close
    public static void InterceptAllCloseEvents(IMockCommunicationObject mco, List<string> eventsFired)
    {
        mco.Closing += (s, ea) => eventsFired.Add("Closing");
        mco.Closed += (s, ea) => eventsFired.Add("Closed");
    }

    // Intercepts all the open and close methods in MockCommunicationObject
    // and records the CommunicationState before and after the default code executes,
    public static void InterceptAllStateChanges(IMockCommunicationObject mco, CommunicationStateData data)
    {
        // Immediately capture the current state after initial creation
        data.StateAfterCreate = mco.State;

        mco.OnOpeningOverride = () =>
        {
            data.StateEnterOnOpening = mco.State;
            mco.DefaultOnOpening();
            data.StateLeaveOnOpening = mco.State;
        };

        mco.OnOpenOverride = (TimeSpan t) =>
        {
            data.StateEnterOnOpen = mco.State;
            mco.DefaultOnOpen(t);
            data.StateLeaveOnOpen = mco.State;
        };

        mco.OnBeginOpenOverride = (TimeSpan t, AsyncCallback c, object s) =>
        {
            data.StateEnterOnBeginOpen = mco.State;
            IAsyncResult result = mco.DefaultOnBeginOpen(t, c, s);
            data.StateLeaveOnBeginOpen = mco.State;
            return result;
        };

        mco.OnOpenedOverride = () =>
        {
            data.StateEnterOnOpened = mco.State;
            mco.DefaultOnOpened();
            data.StateLeaveOnOpened = mco.State;
        };

        mco.OnClosingOverride = () =>
        {
            data.StateEnterOnClosing = mco.State;
            mco.DefaultOnClosing();
            data.StateLeaveOnClosing = mco.State;
        };

        mco.OnCloseOverride = (TimeSpan t) =>
        {
            data.StateEnterOnClose = mco.State;
            mco.DefaultOnClose(t);
            data.StateLeaveOnClose = mco.State;
        };

        mco.OnBeginCloseOverride = (TimeSpan t, AsyncCallback c, object s) =>
        {
            data.StateEnterOnBeginClose = mco.State;
            IAsyncResult result = mco.DefaultOnBeginClose(t, c, s);
            data.StateLeaveOnBeginClose = mco.State;
            return result;
        };

        mco.OnClosedOverride = () =>
        {
            data.StateEnterOnClosed = mco.State;
            mco.DefaultOnClosed();
            data.StateLeaveOnClosed = mco.State;
        };

        #endregion helpers
    }
}

public class CommunicationStateData
{
    public CommunicationState StateAfterCreate { get; set; }
    public CommunicationState StateEnterOnOpening { get; set; }
    public CommunicationState StateLeaveOnOpening { get; set; }
    public CommunicationState StateEnterOnOpen { get; set; }
    public CommunicationState StateLeaveOnOpen { get; set; }
    public CommunicationState StateEnterOnBeginOpen { get; set; }
    public CommunicationState StateLeaveOnBeginOpen { get; set; }
    public CommunicationState StateEnterOnOpened { get; set; }
    public CommunicationState StateLeaveOnOpened { get; set; }

    public CommunicationState StateEnterOnClosing { get; set; }
    public CommunicationState StateLeaveOnClosing { get; set; }
    public CommunicationState StateEnterOnClose { get; set; }
    public CommunicationState StateLeaveOnClose { get; set; }
    public CommunicationState StateEnterOnBeginClose { get; set; }
    public CommunicationState StateLeaveOnBeginClose { get; set; }
    public CommunicationState StateEnterOnClosed { get; set; }
    public CommunicationState StateLeaveOnClosed { get; set; }
}


