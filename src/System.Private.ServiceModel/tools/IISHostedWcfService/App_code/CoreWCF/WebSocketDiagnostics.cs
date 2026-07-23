// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using System;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace WcfService
{
    // Diagnostics for the WebSocket Kestrel listeners on the SelfHostedCoreWcfService.
    //
    // Captures, in the Helix console log, the precise lifecycle of every TCP
    // connection that reaches the WebSocket endpoints (ports 8083/8084):
    //   * timestamped open / close / exception events tagged [WSDIAG]
    //   * connection-id correlation with Kestrel trace logging
    //   * byte-level outbound sniffing on the IDuplexPipe transport so we can
    //     prove whether the server ever wrote a WebSocket Close frame (first
    //     byte 0x88 = FIN+opcode Close, unmasked server-to-client) before
    //     Kestrel's transport sent TCP FIN. This isolates whether the
    //     "remote party closed without completing the close handshake" race
    //     on Helix (issue #5818) is caused by a missing server-side Close
    //     frame or by a Kestrel teardown that races the close-frame flush.
    internal sealed class WebSocketDiagnosticsConnectionMiddleware
    {
        internal const string Tag = "[WSDIAG]";
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

            IDuplexPipe original = connection.Transport;
            SniffingDuplexPipe sniff = new SniffingDuplexPipe(original, connId, n, sw);
            connection.Transport = sniff;

            CancellationTokenRegistration ctr = connection.ConnectionClosed.Register(() =>
            {
                Write($"connclosed-token #{n} id={connId} elapsedMs={sw.ElapsedMilliseconds} {sniff.SummaryLine()}");
            });

            try
            {
                await _next(connection);
                Write($"pipeline-completed #{n} id={connId} elapsedMs={sw.ElapsedMilliseconds} {sniff.SummaryLine()}");
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
                Write($"close #{n} id={connId} totalMs={sw.ElapsedMilliseconds} {sniff.SummaryLine()}");
            }
        }

        internal static void Write(string line)
        {
            Console.WriteLine($"{Tag} {DateTime.UtcNow:HH:mm:ss.fffffff} {line}");
        }

        private static string Sanitize(string s) =>
            s is null ? string.Empty : s.Replace("\r", " ").Replace("\n", " | ");
    }

    // Wraps the connection's IDuplexPipe so we can observe outbound bytes
    // written by the application. We do not modify any bytes; we only
    // timestamp + log opcode bytes of each outbound write so we can prove
    // whether a WebSocket Close frame was emitted before the socket's send
    // loop completed.
    internal sealed class SniffingDuplexPipe : IDuplexPipe
    {
        private readonly SniffingPipeWriter _output;
        private readonly IDuplexPipe _inner;

        public SniffingDuplexPipe(IDuplexPipe inner, string connId, long seq, Stopwatch sw)
        {
            _inner = inner;
            _output = new SniffingPipeWriter(inner.Output, connId, seq, sw);
        }

        public PipeReader Input => _inner.Input;
        public PipeWriter Output => _output;

        public string SummaryLine() => _output.SummaryLine();
    }

    internal sealed class SniffingPipeWriter : PipeWriter
    {
        private readonly PipeWriter _inner;
        private readonly string _connId;
        private readonly long _seq;
        private readonly Stopwatch _sw;

        private Memory<byte> _lastMemory;
        private long _totalBytes;
        private int _writeCount;
        private bool _closeFrameSeen;
        private long _closeFrameElapsedMs = -1;
        private byte _lastFirstByte;

        public SniffingPipeWriter(PipeWriter inner, string connId, long seq, Stopwatch sw)
        {
            _inner = inner;
            _connId = connId;
            _seq = seq;
            _sw = sw;
        }

        private void Inspect(ReadOnlySpan<byte> span, string source)
        {
            if (span.Length == 0) return;

            _writeCount++;
            _totalBytes += span.Length;
            _lastFirstByte = span[0];

            // Server-to-client frames are unmasked. WebSocket frame header
            // byte layout: FIN(1) RSV(3) Opcode(4). Close opcode = 0x8.
            // FIN+Close => 0x88.
            if ((span[0] & 0x0F) == 0x08 && (span[0] & 0x80) == 0x80)
            {
                if (!_closeFrameSeen)
                {
                    _closeFrameSeen = true;
                    _closeFrameElapsedMs = _sw.ElapsedMilliseconds;
                    int dumpLen = Math.Min(span.Length, 16);
                    string hex = Convert.ToHexString(span.Slice(0, dumpLen));
                    WebSocketDiagnosticsConnectionMiddleware.Write(
                        $"out-close-frame #{_seq} id={_connId} via={source} elapsedMs={_closeFrameElapsedMs} firstByte=0x{span[0]:X2} dump={hex}");
                }
            }
        }

        public override void Advance(int bytes)
        {
            if (bytes > 0 && _lastMemory.Length >= bytes)
            {
                Inspect(_lastMemory.Span.Slice(0, bytes), "GetMemory+Advance");
            }
            _inner.Advance(bytes);
        }

        public override Memory<byte> GetMemory(int sizeHint = 0)
        {
            _lastMemory = _inner.GetMemory(sizeHint);
            return _lastMemory;
        }

        public override Span<byte> GetSpan(int sizeHint = 0)
        {
            _lastMemory = _inner.GetMemory(sizeHint);
            return _lastMemory.Span;
        }

        public override ValueTask<FlushResult> WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default)
        {
            Inspect(source.Span, "WriteAsync");
            return _inner.WriteAsync(source, cancellationToken);
        }

        public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
            => _inner.FlushAsync(cancellationToken);

        public override void CancelPendingFlush() => _inner.CancelPendingFlush();

        public override void Complete(Exception exception = null)
        {
            WebSocketDiagnosticsConnectionMiddleware.Write(
                $"output-complete #{_seq} id={_connId} elapsedMs={_sw.ElapsedMilliseconds} totalBytes={_totalBytes} writes={_writeCount} closeFrameSent={_closeFrameSeen} ex={exception?.GetType().Name ?? "null"}");
            _inner.Complete(exception);
        }

        public string SummaryLine() =>
            $"writes={_writeCount} totalBytes={_totalBytes} closeFrameSent={_closeFrameSeen} closeFrameElapsedMs={_closeFrameElapsedMs} lastFirstByte=0x{_lastFirstByte:X2}";
    }
}
#endif
