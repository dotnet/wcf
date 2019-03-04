// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Security
{
    internal interface ISecurityCommunicationObject
    {
        TimeSpan DefaultOpenTimeout { get; }
        TimeSpan DefaultCloseTimeout { get; }
        void OnAbort();
        Task OnCloseAsync(TimeSpan timeout);
        void OnClosed();
        void OnClosing();
        void OnFaulted();
        Task OnOpenAsync(TimeSpan timeout);
        void OnOpened();
        void OnOpening();
    }
}
