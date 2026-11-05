// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public static class MessageEncoderBufferTest
{
    [WcfTheory]
    [InlineData(0, 8192, false)]
    [InlineData(8191, 8192, false)]
    [InlineData(8192, 8192, false)]
    [InlineData(8193, 16384, false)]
    [InlineData(16384, 16384, true)]
    [InlineData(65535, 65536, true)]
    [InlineData(65536, 65536, false)]
    [InlineData(65537, 131072, false)]
    [InlineData(1048576, 1048576, false)]
    public static async Task BufferMessageStreamAsync_UnknownLength_OnlyGrowsWhenMoreDataExists(int length, int expectedCapacity, bool fragmented)
    {
        byte[] payload = CreatePayload(length);
        var manager = new TrackingBufferManager();
        using var stream = new UnknownLengthStream(payload, fragmented ? 137 : int.MaxValue);
        ArraySegment<byte> result = await BufferAsync(stream, manager, 2 * 1024 * 1024, CancellationToken.None);
        try
        {
            Assert.Equal(payload, result.ToArray());
            Assert.Equal(expectedCapacity, result.Array.Length);
            Assert.Equal(expectedCapacity, manager.Rents[manager.Rents.Count - 1]);
            Assert.Single(manager.Outstanding);
            Assert.Contains(result.Array, manager.Outstanding);
            Assert.True(stream.Disposed);
        }
        finally
        {
            manager.ReturnBuffer(result.Array);
        }

        Assert.Empty(manager.Outstanding);
    }

    [WcfFact]
    public static async Task BufferMessageStreamAsync_OversizedRental_DoesNotGrowAtEof()
    {
        byte[] payload = CreatePayload(16384);
        var manager = new TrackingBufferManager { OversizeFirstRental = true };
        using var stream = new UnknownLengthStream(payload, 311);
        ArraySegment<byte> result = await BufferAsync(stream, manager, 65536, CancellationToken.None);
        try
        {
            Assert.Equal(payload, result.ToArray());
            Assert.Single(manager.Rents);
            Assert.Equal(16384, result.Array.Length);
        }
        finally
        {
            manager.ReturnBuffer(result.Array);
        }

        Assert.Empty(manager.Outstanding);
    }

    [WcfTheory]
    [InlineData(8191, 8192, false)]
    [InlineData(8192, 8192, true)]
    [InlineData(8193, 8192, true)]
    [InlineData(8193, 8193, true)]
    [InlineData(8194, 8193, true)]
    [InlineData(8999, 9000, false)]
    public static async Task BufferMessageStreamAsync_Quota_PreservesLimitAndReturnsBuffer(int length, int maximum, bool exceedsQuota)
    {
        byte[] payload = CreatePayload(length);
        var manager = new TrackingBufferManager();
        using var stream = new UnknownLengthStream(payload, int.MaxValue);
        if (exceedsQuota)
        {
            CommunicationException exception = await Assert.ThrowsAsync<CommunicationException>(
                () => BufferAsync(stream, manager, maximum, CancellationToken.None));
            Assert.IsType<QuotaExceededException>(exception.InnerException);
            Assert.Equal(maximum, stream.BytesRead);
            Assert.False(stream.Disposed);
        }
        else
        {
            ArraySegment<byte> result = await BufferAsync(stream, manager, maximum, CancellationToken.None);
            try
            {
                Assert.Equal(payload, result.ToArray());
                Assert.True(stream.Disposed);
            }
            finally
            {
                manager.ReturnBuffer(result.Array);
            }
        }

        Assert.Empty(manager.Outstanding);
    }

    [WcfTheory]
    [InlineData(0, false)]
    [InlineData(8192, false)]
    [InlineData(8193, false)]
    [InlineData(0, true)]
    [InlineData(8192, true)]
    [InlineData(8193, true)]
    public static async Task BufferMessageStreamAsync_ReadFails_PreservesErrorAndReturnsBuffer(int failAfter, bool cancel)
    {
        using var cancellation = new CancellationTokenSource();
        var failure = new IOException("Response read failed");
        var manager = new TrackingBufferManager();
        using var stream = new UnknownLengthStream(CreatePayload(20000), int.MaxValue)
        {
            FailAfter = failAfter,
            Failure = failure,
            Cancellation = cancel ? cancellation : null
        };
        Exception exception = await Record.ExceptionAsync(() => BufferAsync(stream, manager, 65536, cancellation.Token));
        if (cancel)
        {
            OperationCanceledException canceled = Assert.IsAssignableFrom<OperationCanceledException>(exception);
            Assert.Equal(cancellation.Token, canceled.CancellationToken);
        }
        else
        {
            Assert.Same(failure, exception);
        }

        Assert.Empty(manager.Outstanding);
        Assert.False(stream.Disposed);
    }

    [WcfFact]
    public static async Task BufferMessageStreamAsync_GrowthRentFails_ReturnsCurrentBuffer()
    {
        var failure = new InvalidOperationException("Growth rent failed");
        var manager = new TrackingBufferManager { GrowthFailure = failure };
        using var stream = new UnknownLengthStream(CreatePayload(8193), int.MaxValue);

        Exception exception = await Record.ExceptionAsync(() => BufferAsync(stream, manager, 65536, CancellationToken.None));

        Assert.Same(failure, exception);
        Assert.Empty(manager.Outstanding);
        Assert.False(stream.Disposed);
    }

    private static byte[] CreatePayload(int length)
    {
        byte[] payload = new byte[length];
        for (int i = 0; i < payload.Length; i++)
        {
            payload[i] = (byte)(i * 31 + i / 256);
        }

        return payload;
    }

    private static Task<ArraySegment<byte>> BufferAsync(Stream stream, BufferManager manager, int maximum, CancellationToken token)
    {
        MessageEncoder encoder = new TextMessageEncodingBindingElement().CreateMessageEncoderFactory().Encoder;
        MethodInfo method = typeof(MessageEncoder).GetMethod("BufferMessageStreamAsync", BindingFlags.Instance | BindingFlags.NonPublic);
        Task<ArraySegment<byte>> pending = (Task<ArraySegment<byte>>)method.Invoke(encoder, new object[] { stream, manager, maximum, token });
        return pending.WaitAsync(TimeSpan.FromSeconds(15));
    }

    private sealed class TrackingBufferManager : BufferManager
    {
        public List<int> Rents { get; } = new();
        public HashSet<byte[]> Outstanding { get; } = new();
        public bool OversizeFirstRental { get; set; }
        public Exception GrowthFailure { get; set; }

        public override byte[] TakeBuffer(int bufferSize)
        {
            if (Rents.Count > 0 && GrowthFailure != null)
            {
                throw GrowthFailure;
            }

            var buffer = new byte[OversizeFirstRental && Rents.Count == 0 ? bufferSize * 2 : bufferSize];
            Rents.Add(bufferSize);
            Assert.True(Outstanding.Add(buffer));
            return buffer;
        }

        public override void ReturnBuffer(byte[] buffer) => Assert.True(Outstanding.Remove(buffer), "Unknown or duplicate buffer return");
        public override void Clear() => Assert.Empty(Outstanding);
    }

    private sealed class UnknownLengthStream : MemoryStream
    {
        private readonly int _fragmentSize;

        public UnknownLengthStream(byte[] payload, int fragmentSize) : base(payload) => _fragmentSize = fragmentSize;
        public int BytesRead { get; private set; }
        public bool Disposed { get; private set; }
        public int FailAfter { get; set; } = int.MaxValue;
        public Exception Failure { get; set; }
        public CancellationTokenSource Cancellation { get; set; }
        public override bool CanSeek => false;
        public override long Length => throw new NotSupportedException();

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await Task.Yield();
            if (BytesRead >= FailAfter)
            {
                if (Cancellation != null)
                {
                    Cancellation.Cancel();
                    cancellationToken.ThrowIfCancellationRequested();
                }

                throw Failure;
            }

            int read = await base.ReadAsync(buffer, offset, Math.Min(count, Math.Min(_fragmentSize, FailAfter - BytesRead)), cancellationToken);
            BytesRead += read;
            return read;
        }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }
    }
}
