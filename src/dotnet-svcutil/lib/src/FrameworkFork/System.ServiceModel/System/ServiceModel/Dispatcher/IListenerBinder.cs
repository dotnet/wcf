// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    internal interface IListenerBinder
    {
        IChannelListener Listener { get; }
        MessageVersion MessageVersion { get; }

        IChannelBinder Accept(TimeSpan timeout);
        IAsyncResult BeginAccept(TimeSpan timeout, AsyncCallback callback, object state);
        IChannelBinder EndAccept(IAsyncResult result);
    }
}
