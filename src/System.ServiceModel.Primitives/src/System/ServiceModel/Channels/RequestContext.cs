// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.ServiceModel.Channels
{
    public abstract class RequestContext : IDisposable
    {
        public abstract Message RequestMessage { get; }

        public abstract void Abort();

        public abstract void Close();
        public abstract void Close(TimeSpan timeout);

        public abstract void Reply(Message message);
        public abstract void Reply(Message message, TimeSpan timeout);
        public abstract IAsyncResult BeginReply(Message message, AsyncCallback callback, object state);
        public abstract IAsyncResult BeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state);
        public abstract void EndReply(IAsyncResult result);

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
