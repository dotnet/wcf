// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
