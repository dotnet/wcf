// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Runtime;

namespace System.ServiceModel.Channels
{
    public abstract class StreamUpgradeInitiator
    {
        protected StreamUpgradeInitiator()
        {
        }

        public abstract string GetNextUpgrade();

        public abstract Stream InitiateUpgrade(Stream stream);
        public abstract IAsyncResult BeginInitiateUpgrade(Stream stream, AsyncCallback callback, object state);
        public abstract Stream EndInitiateUpgrade(IAsyncResult result);

        internal virtual IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CompletedAsyncResult(callback, state);
        }

        internal virtual void EndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        internal virtual IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CompletedAsyncResult(callback, state);
        }

        internal virtual void EndClose(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        internal virtual void Open(TimeSpan timeout)
        {
        }

        internal virtual void Close(TimeSpan timeout)
        {
        }
    }
}
