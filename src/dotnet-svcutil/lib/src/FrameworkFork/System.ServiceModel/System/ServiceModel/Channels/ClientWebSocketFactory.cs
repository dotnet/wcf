// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Runtime;
using System.ServiceModel;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    // In Win8 (and above), a client web socket can simply be created in 2 steps:
    // 1. create a HttpWebRequest with the Uri = "ws://server_address"
    // 2. create a client WebSocket with WebSocket.CreateClientWebSocket(stream_requested_from_the_HttpWebRequest)
    // On pre-Win8, the WebSocket.CreateClientWebSocket method doesn't work, so users needs to provide a factory for step #2.
    // WCF will internally create the HttpWebRequest from step #1 and will call the web socket factory for step #2.
    // A factory can also be used in Win8 (and above), if the user desires to use his own WebSocket implementation.
    public abstract class ClientWebSocketFactory
    {
        public abstract Task<WebSocket> CreateWebSocketAsync(Uri address, WebHeaderCollection headers, ICredentials credentials, WebSocketTransportSettings settings, TimeoutHelper timeoutHelper);

        public static ClientWebSocketFactory GetFactory()
        {
            return new CoreClrClientWebSocketFactory();
        }
    }
}
