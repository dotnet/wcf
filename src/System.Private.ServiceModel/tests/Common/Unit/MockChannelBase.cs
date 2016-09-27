// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

public class MockChannelBase : ChannelBase, IMockCommunicationObject
{
    private readonly EndpointAddress _address;
    private readonly MessageEncoder _encoder;
    private readonly ChannelManagerBase _manager;

    public MockChannelBase(ChannelManagerBase manager, MessageEncoderFactory encoderFactory, EndpointAddress address)
    : base(manager)
    {
        _address = address;
        _manager = manager;
        _encoder = encoderFactory.CreateSessionEncoder();

        OpenAsyncResult = new MockAsyncResult();
        CloseAsyncResult = new MockAsyncResult();

        GetEndpointPropertyOverride = DefaultGetEndpointProperty;

        // CommunicationObject overrides
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

    public Func<EndpointAddress> GetEndpointPropertyOverride { get; set; }

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

    public EndpointAddress RemoteAddress
    {
        get { return DefaultGetEndpointProperty(); }
    }

    public EndpointAddress DefaultGetEndpointProperty()
    {
        return _address;
    }

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
}
