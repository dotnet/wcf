// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace System.ServiceModel
{
    public interface ICommunicationObject
    {
        CommunicationState State { get; }

        event EventHandler Closed;
        event EventHandler Closing;
        event EventHandler Faulted;
        event EventHandler Opened;
        event EventHandler Opening;

        void Abort();

        void Close();
        void Close(TimeSpan timeout);
        IAsyncResult BeginClose(AsyncCallback callback, object state);
        IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state);
        void EndClose(IAsyncResult result);

        void Open();
        void Open(TimeSpan timeout);
        IAsyncResult BeginOpen(AsyncCallback callback, object state);
        IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state);
        void EndOpen(IAsyncResult result);
    }

    public interface IAsyncCommunicationObject : ICommunicationObject
    {
        Task CloseAsync(TimeSpan timeout);
        Task OpenAsync(TimeSpan timeout);
    }
}
