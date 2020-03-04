// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
