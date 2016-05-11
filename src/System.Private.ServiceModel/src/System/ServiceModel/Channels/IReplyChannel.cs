// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Channels
{
    public interface IReplyChannel : IChannel
    {
        EndpointAddress LocalAddress { get; }

        RequestContext ReceiveRequest();
        RequestContext ReceiveRequest(TimeSpan timeout);
        IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state);
        IAsyncResult BeginReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state);
        RequestContext EndReceiveRequest(IAsyncResult result);

        bool TryReceiveRequest(TimeSpan timeout, out RequestContext context);
        IAsyncResult BeginTryReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state);
        bool EndTryReceiveRequest(IAsyncResult result, out RequestContext context);

        bool WaitForRequest(TimeSpan timeout);
        IAsyncResult BeginWaitForRequest(TimeSpan timeout, AsyncCallback callback, object state);
        bool EndWaitForRequest(IAsyncResult result);
    }
}
