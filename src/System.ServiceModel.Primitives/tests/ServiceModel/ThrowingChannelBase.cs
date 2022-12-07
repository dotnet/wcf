// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel.Channels;

/// <summary>
/// Base channel class that uses an ChannelMessageInterceptor
/// </summary>
class ThrowingChannelBase<TChannel> : ChannelBase where TChannel : class, IChannel
{
    private readonly Exception _exception;

    protected ThrowingChannelBase(ChannelManagerBase manager, Exception exception, TChannel innerChannel) : base(manager)
    {
        if (innerChannel == null)
        {
            throw new ArgumentException("ThrowingChannelBase requires a non-null inner channel.", "innerChannel");
        }

        _exception = exception;
        InnerChannel = innerChannel;
    }

    protected TChannel InnerChannel { get; private set; }

    public override T GetProperty<T>()
    {
        T baseProperty = base.GetProperty<T>();
        if (baseProperty != null)
        {
            return baseProperty;
        }

        return InnerChannel.GetProperty<T>();
    }

    protected override void OnAbort() => InnerChannel.Abort();

    protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state) => throw _exception;

    protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state) => InnerChannel.BeginOpen(timeout, callback, state);

    protected override void OnClose(TimeSpan timeout) => throw _exception;

    protected override void OnEndClose(IAsyncResult result) => InnerChannel.EndClose(result);

    protected override void OnEndOpen(IAsyncResult result) => InnerChannel.EndOpen(result);

    protected override void OnOpen(TimeSpan timeout) => InnerChannel.Open(timeout);
}
