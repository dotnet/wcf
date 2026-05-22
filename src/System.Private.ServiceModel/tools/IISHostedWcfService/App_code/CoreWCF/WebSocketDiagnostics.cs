// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace WcfService
{
    // Diagnostics for the WebSocket Kestrel listeners on the SelfHostedCoreWcfService.
    //
    // The goal is to capture, in the Helix console log, the precise lifecycle of
    // every TCP connection that reaches the WebSocket endpoints (ports 8083/8084):
    //   * timestamped open / close / exception events
    //   * connection-id correlation with Kestrel's trace logging
    //   * any uncaught exception that escapes the pipeline (which typically
    //     produces the "remote party closed without completing the close
    //     handshake" race observed in issue #5818 on Helix Linux Open queues)
    //
    // Pair this middleware with Trace-level filters for
    //   Microsoft.AspNetCore.Server.Kestrel.Connections
    //   Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets
    //   Microsoft.AspNetCore.WebSockets
    // so byte counts, TCP FIN/RST events, and WebSocket close-frame events
    // appear alongside the WSDIAG lifecycle stamps. All WSDIAG output is
    // prefixed so it can be grep'd easily from the Helix console.log.
    internal sealed class WebSocketDiagnosticsConnectionMiddleware
    {
        private const string Tag = "[WSDIAG]";
        private static long s_seq;

        private readonly ConnectionDelegate _next;
        private readonly string _listener;

        public WebSocketDiagnosticsConnectionMiddleware(ConnectionDelegate next, string listener)
        {
            _next = next;
            _listener = listener;
        }

        public async Task OnConnectionAsync(ConnectionContext connection)
        {
            long n = Interlocked.Increment(ref s_seq);
            Stopwatch sw = Stopwatch.StartNew();
            string connId = connection.ConnectionId;
            string remote = connection.RemoteEndPoint?.ToString() ?? "?";
            string local = connection.LocalEndPoint?.ToString() ?? "?";

            Write($"open #{n} listener={_listener} id={connId} remote={remote} local={local}");

            CancellationTokenRegistration ctr = connection.ConnectionClosed.Register(() =>
            {
                Write($"connclosed-token #{n} id={connId} elapsedMs={sw.ElapsedMilliseconds}");
            });

            try
            {
                await _next(connection);
                Write($"pipeline-completed #{n} id={connId} elapsedMs={sw.ElapsedMilliseconds}");
            }
            catch (Exception ex)
            {
                Write($"pipeline-exception #{n} id={connId} elapsedMs={sw.ElapsedMilliseconds} type={ex.GetType().FullName} msg={Sanitize(ex.Message)}");
                Write($"pipeline-exception-stack #{n} id={connId} {Sanitize(ex.ToString())}");
                throw;
            }
            finally
            {
                ctr.Dispose();
                Write($"close #{n} id={connId} totalMs={sw.ElapsedMilliseconds}");
            }
        }

        private static void Write(string line)
        {
            // Single Console.WriteLine call to keep the line atomic across threads.
            Console.WriteLine($"{Tag} {DateTime.UtcNow:HH:mm:ss.fffffff} {line}");
        }

        private static string Sanitize(string s) =>
            s is null ? string.Empty : s.Replace("\r", " ").Replace("\n", " | ");
    }
}
#endif
