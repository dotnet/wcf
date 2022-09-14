// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class InputChannel
    {
        internal static async Task<Message> HelpReceiveAsync(IAsyncInputChannel channel, TimeSpan timeout)
        {
            (bool success, Message message) = await channel.TryReceiveAsync(timeout);

            if (success)
            {
                return message;
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateReceiveTimedOutException(channel, timeout));
            }
        }

        private static Exception CreateReceiveTimedOutException(IInputChannel channel, TimeSpan timeout)
        {
            if (channel.LocalAddress != null)
            {
                return new TimeoutException(SRP.Format(SRP.ReceiveTimedOut, channel.LocalAddress.Uri.AbsoluteUri, timeout));
            }
            else
            {
                return new TimeoutException(SRP.Format(SRP.ReceiveTimedOutNoLocalAddress, timeout));
            }
        }
    }
}
