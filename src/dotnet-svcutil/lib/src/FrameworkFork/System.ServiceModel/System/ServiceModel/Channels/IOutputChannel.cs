// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;

namespace System.ServiceModel.Channels
{
    public interface IOutputChannel : IChannel
    {
        EndpointAddress RemoteAddress { get; }
        Uri Via { get; }

        void Send(Message message);
        void Send(Message message, TimeSpan timeout);
        IAsyncResult BeginSend(Message message, AsyncCallback callback, object state);
        IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state);
        void EndSend(IAsyncResult result);
    }
}
