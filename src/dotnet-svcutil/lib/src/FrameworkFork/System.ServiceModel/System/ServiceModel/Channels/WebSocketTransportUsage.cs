// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
