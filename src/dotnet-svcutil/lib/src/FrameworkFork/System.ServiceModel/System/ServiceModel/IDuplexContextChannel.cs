// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel
{
    public interface IDuplexContextChannel : IContextChannel
    {
        bool AutomaticInputSessionShutdown { get; set; }
        InstanceContext CallbackInstance { get; set; }

        IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state);
        void EndCloseOutputSession(IAsyncResult result);
        void CloseOutputSession(TimeSpan timeout);
    }
}
