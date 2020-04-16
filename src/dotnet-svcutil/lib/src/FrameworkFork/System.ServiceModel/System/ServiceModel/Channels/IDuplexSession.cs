// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
