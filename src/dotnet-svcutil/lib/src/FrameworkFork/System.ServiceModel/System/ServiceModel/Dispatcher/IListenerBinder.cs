// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
