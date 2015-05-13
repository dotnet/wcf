// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Channels
{
    internal interface IChannelAcceptor<TChannel> : ICommunicationObject
        where TChannel : class, IChannel
    {
        TChannel AcceptChannel(TimeSpan timeout);
        IAsyncResult BeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state);
        TChannel EndAcceptChannel(IAsyncResult result);

        bool WaitForChannel(TimeSpan timeout);
        IAsyncResult BeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state);
        bool EndWaitForChannel(IAsyncResult result);
    }
}
