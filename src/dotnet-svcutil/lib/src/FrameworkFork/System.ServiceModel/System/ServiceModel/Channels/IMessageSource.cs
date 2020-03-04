// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    public interface IMessageSource
    {
        Task<Message> ReceiveAsync(TimeSpan timeout);
        Message Receive(TimeSpan timeout);

        Task<bool> WaitForMessageAsync(TimeSpan timeout);
        bool WaitForMessage(TimeSpan timeout);
    }
}
