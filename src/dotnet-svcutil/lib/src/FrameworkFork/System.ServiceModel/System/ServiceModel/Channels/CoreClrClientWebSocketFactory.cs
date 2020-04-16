// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.Net.WebSockets;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class CoreClrClientWebSocketFactory : ClientWebSocketFactory
    {
        public override async Task<WebSocket> CreateWebSocketAsync(Uri address, WebHeaderCollection headers, ICredentials credentials,
            WebSocketTransportSettings settings, TimeoutHelper timeoutHelper)
        {
            ClientWebSocket webSocket = new ClientWebSocket();
            webSocket.Options.Credentials = credentials;
            if (!string.IsNullOrEmpty(settings.SubProtocol))
            {
                webSocket.Options.AddSubProtocol(settings.SubProtocol);
            }

            webSocket.Options.KeepAliveInterval = settings.KeepAliveInterval;
            foreach (var headerObj in headers)
            {
                var header = headerObj as string;
                webSocket.Options.SetRequestHeader(header, headers[header]);
            }

            var cancelToken = await timeoutHelper.GetCancellationTokenAsync();
            await webSocket.ConnectAsync(address, cancelToken);
            return webSocket;
        }
    }
}
