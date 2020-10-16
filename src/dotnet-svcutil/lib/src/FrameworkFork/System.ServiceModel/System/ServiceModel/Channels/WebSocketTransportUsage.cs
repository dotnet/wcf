// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    public enum WebSocketTransportUsage
    {
        /// <summary>
        /// Indicates WebSocket transport will be used for duplex service contracts only.
        /// </summary>
        WhenDuplex = 0,

        /// <summary>
        /// Indicates WebSocket transport will always be used.
        /// </summary>
        Always,

        /// <summary>
        /// Indicates WebSocket transport will never be used.
        /// </summary>
        Never
    }
}
