// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
