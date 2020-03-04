// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Channels
{
    public interface IChannelListener : ICommunicationObject
    {
        Uri Uri { get; }

        T GetProperty<T>() where T : class;

        bool WaitForChannel(TimeSpan timeout);
        IAsyncResult BeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state);
        bool EndWaitForChannel(IAsyncResult result);
    }

    public interface IChannelListener<TChannel> : IChannelListener
        where TChannel : class, IChannel
    {
        TChannel AcceptChannel();
        TChannel AcceptChannel(TimeSpan timeout);
        IAsyncResult BeginAcceptChannel(AsyncCallback callback, object state);
        IAsyncResult BeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state);
        TChannel EndAcceptChannel(IAsyncResult result);
    }
}
