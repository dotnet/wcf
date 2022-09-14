// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    public interface IRequestChannel : IChannel
    {
        EndpointAddress RemoteAddress { get; }
        Uri Via { get; }

        Message Request(Message message);
        Message Request(Message message, TimeSpan timeout);
        IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state);
        IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state);
        Message EndRequest(IAsyncResult result);
    }

    // Internal interface to allow async implementation using async/await. We should consider adding these to the
    // public contract for IRequestChannel in the future.
    internal interface IAsyncRequestChannel : IRequestChannel, IAsyncCommunicationObject
    {
        Task<Message> RequestAsync(Message message);
        Task<Message> RequestAsync(Message message, TimeSpan timeout);
    }
}
