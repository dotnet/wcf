// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal static class ConnectionUtilities
    {
        internal static async ValueTask CloseNoThrowAsync(IConnection connection, TimeSpan timeout)
        {
            bool success = false;
            try
            {
                await connection.CloseAsync(timeout);
                success = true;
            }
            catch (TimeoutException)
            {
            }
            catch (CommunicationException)
            {
            }
            finally
            {
                if (!success)
                {
                    connection.Abort();
                }
            }
        }
    }
}
