// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace System.ServiceModel.Channels
{
    public interface IDuplexSession : IInputSession, IOutputSession
    {
        void CloseOutputSession();
        void CloseOutputSession(TimeSpan timeout);
        IAsyncResult BeginCloseOutputSession(AsyncCallback callback, object state);
        IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state);
        void EndCloseOutputSession(IAsyncResult result);
    }
}
