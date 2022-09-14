// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Threading.Tasks;

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

    public interface IAsyncOutputChannel : IOutputChannel, IAsyncCommunicationObject
    {
        Task SendAsync(Message message);
        Task SendAsync(Message message, TimeSpan timeout);
    }
}
