// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
