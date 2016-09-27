// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

public class MockChannelFactory<TChannel> : ChannelFactoryBase<TChannel>, IMockCommunicationObject where TChannel : IChannel
{
    public MockChannelFactory(BindingContext context, MessageEncoderFactory encoderFactory)
            : base(context.Binding)
    {
        MessageEncoderFactory = encoderFactory;

        OnCreateChannelOverride = DefaultOnCreateChannel;

        OpenAsyncResult = new MockAsyncResult();
        CloseAsyncResult = new MockAsyncResult();

        // Each overrideable method has a delegate property that
        // can be set to override it, please a default handler.

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

    public Func<EndpointAddress, Uri, IChannel> OnCreateChannelOverride { get; set; }

    public MessageEncoderFactory MessageEncoderFactory { get; set; }

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

    protected override TChannel OnCreateChannel(EndpointAddress address, Uri via)
    {
        return (TChannel) OnCreateChannelOverride(address, via);
    }

    public IChannel DefaultOnCreateChannel(EndpointAddress address, Uri via)
    {
        // Default is a mock IRequestChannel.
        // If callers want something else, supply a delegate to OnCreateChannelOverride
        // that creates the TChannel needed, and don't call this default.
        return new MockRequestChannel(this, MessageEncoderFactory, address, via);
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
        base.OnAbort();
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
    }

    protected override void OnClose(TimeSpan timeout)
    {
        OnCloseOverride(timeout);
    }

    public void DefaultOnClose(TimeSpan timeout)
    {
        base.OnClose(timeout);
    }

    protected override void OnEndClose(IAsyncResult result)
    {
        OnEndCloseOverride(result);
    }

    public void DefaultOnEndClose(IAsyncResult result)
    {
        ((MockAsyncResult)result).Complete();
    }

    protected override void OnEndOpen(IAsyncResult result)
    {
        OnEndOpenOverride(result);
    }

    public void DefaultOnEndOpen(IAsyncResult result)
    {
        ((MockAsyncResult)result).Complete();
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