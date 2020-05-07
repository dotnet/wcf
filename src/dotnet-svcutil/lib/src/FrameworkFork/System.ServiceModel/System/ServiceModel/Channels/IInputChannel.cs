// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.Collections;

namespace System.ServiceModel.Channels
{
    public interface IInputChannel : IChannel
    {
        EndpointAddress LocalAddress { get; }

        Message Receive();
        Message Receive(TimeSpan timeout);
        IAsyncResult BeginReceive(AsyncCallback callback, object state);
        IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state);
        Message EndReceive(IAsyncResult result);

        bool TryReceive(TimeSpan timeout, out Message message);
        IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state);
        bool EndTryReceive(IAsyncResult result, out Message message);

        bool WaitForMessage(TimeSpan timeout);
        IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state);
        bool EndWaitForMessage(IAsyncResult result);
    }
}
