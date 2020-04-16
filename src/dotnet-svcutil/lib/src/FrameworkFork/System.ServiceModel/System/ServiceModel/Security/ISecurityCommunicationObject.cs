// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    internal interface ISecurityCommunicationObject
    {
        TimeSpan DefaultOpenTimeout { get; }
        TimeSpan DefaultCloseTimeout { get; }
        void OnAbort();
        IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state);
        IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state);
        void OnClose(TimeSpan timeout);
        void OnClosed();
        void OnClosing();
        void OnEndClose(IAsyncResult result);
        void OnEndOpen(IAsyncResult result);
        void OnFaulted();
        void OnOpen(TimeSpan timeout);
        void OnOpened();
        void OnOpening();
    }
}
