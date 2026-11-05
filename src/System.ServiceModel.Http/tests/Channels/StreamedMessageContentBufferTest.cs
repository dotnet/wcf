// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public static class StreamedMessageContentBufferTest
{
    [WcfTheory]
    [InlineData("success", false)]
    [InlineData("success", true)]
    [InlineData("fault", false)]
    [InlineData("fault", true)]
    [InlineData("cancel", false)]
    [InlineData("cancel", true)]
    public static async Task CopyToAsync_TextEncoder_ReturnsBufferAndLeavesDestinationOpen(string outcome, bool partial)
    {
        Exception failure = CreateFailure(outcome);
        MessageEncoder encoder = new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8)
            .CreateMessageEncoderFactory().Encoder;
        var manager = new CountingBufferManager();
        using Message message = Message.CreateMessage(MessageVersion.Soap11, "urn:request", new TestBodyWriter(partial, failure));
        using HttpContent content = CreateContent(message, encoder, manager);
        manager.Completion = Completion(content);
        using var destination = new TrackingStream();

        Exception exception = await Record.ExceptionAsync(() => content.CopyToAsync(destination).WaitAsync(TimeSpan.FromSeconds(15)));
        Assert.Same(failure, exception);
        Assert.False(destination.Disposed);
        if (failure == null)
        {
            using Message expectedMessage = Message.CreateMessage(MessageVersion.Soap11, (string)null, new TestBodyWriter(partial, null));
            using var expected = new MemoryStream();
            await encoder.WriteMessageAsync(expectedMessage, expected);
            Assert.Equal(expected.ToArray(), destination.ToArray());
        }

        await Completion(content).WaitAsync(TimeSpan.FromSeconds(15));
        Assert.Equal(1, manager.Rents);
        Assert.Equal(1, manager.Returns);
        Assert.Same(manager.RentedBuffer, manager.ReturnedBuffer);
        Assert.False(manager.CompletionObservedOnReturn);
        content.Dispose();
        content.Dispose();
        Assert.Equal(1, manager.Returns);
        Assert.False(destination.Disposed);
        destination.WriteByte(42);
    }

    [WcfTheory]
    [InlineData("success", false)]
    [InlineData("fault", false)]
    [InlineData("cancel", false)]
    [InlineData("success", true)]
    [InlineData("fault", true)]
    [InlineData("cancel", true)]
    public static async Task CopyToAsync_DisposeWhileEncoderPending_ReturnsBufferOnlyAfterEncoderEnds(string outcome, bool shadowBuffer)
    {
        Exception failure = CreateFailure(outcome);
        var encoder = new GatedEncoder(failure, shadowBuffer);
        var manager = new CountingBufferManager();
        using Message message = Message.CreateMessage(MessageVersion.Soap11, "urn:request", "request");
        using HttpContent content = CreateContent(message, encoder, manager);
        manager.Completion = Completion(content);
        using var destination = new TrackingStream();
        Task copy = content.CopyToAsync(destination);
        try
        {
            await encoder.Entered.Task.WaitAsync(TimeSpan.FromSeconds(15));
            content.Dispose();
            Assert.Equal(0, manager.Returns);
            Assert.False(Completion(content).IsCompleted);
            Assert.False(destination.Disposed);
        }
        finally
        {
            encoder.Release.TrySetResult(true);
            Exception exception = await Record.ExceptionAsync(() => copy.WaitAsync(TimeSpan.FromSeconds(15)));
            Assert.Same(failure, exception);
        }

        Assert.True(encoder.Ended);
        byte[] expected = shadowBuffer ? new byte[12288] : Array.Empty<byte>();
        if (!shadowBuffer && failure == null)
        {
            expected = new byte[] { 1, 2, 3 };
        }

        Assert.Equal(expected, destination.ToArray());
        Assert.Equal(outcome == "success" ? 1 : 0, destination.Flushes);
        await Completion(content).WaitAsync(TimeSpan.FromSeconds(15));
        Assert.Equal(1, manager.Returns);
        Assert.Same(manager.RentedBuffer, manager.ReturnedBuffer);
        Assert.False(manager.CompletionObservedOnReturn);
        content.Dispose();
        Assert.Equal(1, manager.Returns);
        Assert.False(destination.Disposed);
    }

    [WcfFact]
    public static async Task CopyToAsync_DestinationThrows_ReturnsBufferAndPreservesWriteError()
    {
        var failure = new IOException("Destination write failed");
        var manager = new CountingBufferManager();
        MessageEncoder encoder = new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8)
            .CreateMessageEncoderFactory().Encoder;
        using Message message = Message.CreateMessage(MessageVersion.Soap11, "urn:request", "request");
        using HttpContent content = CreateContent(message, encoder, manager);
        manager.Completion = Completion(content);
        using var destination = new TrackingStream(failure);

        HttpRequestException exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => content.CopyToAsync(destination).WaitAsync(TimeSpan.FromSeconds(15)));
        Assert.Same(failure, exception.InnerException);
        await Completion(content).WaitAsync(TimeSpan.FromSeconds(15));
        Assert.Equal(1, manager.Rents);
        Assert.Equal(1, manager.Returns);
        Assert.Same(manager.RentedBuffer, manager.ReturnedBuffer);
        Assert.False(manager.CompletionObservedOnReturn);
        content.Dispose();
        Assert.Equal(1, manager.Returns);
        Assert.False(destination.Disposed);
    }

    [WcfTheory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public static async Task BufferedWriteStream_Dispose_ReturnsOriginalBufferAndRespectsLeaveOpen(bool leaveOpen, bool asyncWrite)
    {
        var manager = new CountingBufferManager();
        using var destination = new TrackingStream();
        Type type = typeof(Message).Assembly.GetType("System.ServiceModel.Channels.BufferedWriteStream", throwOnError: true);
        using var stream = (Stream)Activator.CreateInstance(type, destination, manager, 8192, leaveOpen);
        if (asyncWrite)
        {
            await stream.WriteAsync(new byte[4096], 0, 4096);
            await stream.WriteAsync(new byte[8192], 0, 8192);
        }
        else
        {
            stream.Write(new byte[4096], 0, 4096);
            stream.Write(new byte[8192], 0, 8192);
        }

        Assert.Equal(0, manager.Returns);
        stream.Dispose();
        stream.Dispose();
        Assert.Equal(1, manager.Rents);
        Assert.Equal(1, manager.Returns);
        Assert.Same(manager.RentedBuffer, manager.ReturnedBuffer);
        Assert.Equal(!leaveOpen, destination.Disposed);
        Assert.Equal(0, destination.Flushes);
        Assert.Equal(new byte[12288], destination.ToArray());
    }

    private static Exception CreateFailure(string outcome) => outcome switch
    {
        "fault" => new InvalidOperationException("Body writer failed"),
        "cancel" => new OperationCanceledException(new CancellationToken(canceled: true)),
        _ => null
    };

    private static HttpContent CreateContent(Message message, MessageEncoder encoder, BufferManager manager)
    {
        Type type = typeof(BasicHttpBinding).Assembly.GetType("System.ServiceModel.Channels.StreamedMessageContent", throwOnError: true);
        return (HttpContent)Activator.CreateInstance(type, message, encoder, manager);
    }

    private static Task Completion(HttpContent content) => (Task)content.GetType().BaseType
        .GetProperty("WriteCompletionTask", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(content);

    private sealed class CountingBufferManager : BufferManager
    {
        public int Rents { get; private set; }
        public int Returns { get; private set; }
        public byte[] RentedBuffer { get; private set; }
        public byte[] ReturnedBuffer { get; private set; }
        public Task Completion { get; set; }
        public bool CompletionObservedOnReturn { get; private set; }

        public override byte[] TakeBuffer(int bufferSize)
        {
            Rents++;
            RentedBuffer = new byte[bufferSize];
            return RentedBuffer;
        }

        public override void ReturnBuffer(byte[] buffer)
        {
            Returns++;
            ReturnedBuffer = buffer;
            CompletionObservedOnReturn = Completion?.IsCompleted == true;
        }

        public override void Clear() => throw new NotSupportedException();
    }

    private sealed class TrackingStream : MemoryStream
    {
        private readonly Exception _writeFailure;

        public TrackingStream(Exception writeFailure = null) => _writeFailure = writeFailure;
        public bool Disposed { get; private set; }
        public int Flushes { get; private set; }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (_writeFailure != null)
            {
                return Task.FromException(_writeFailure);
            }

            return base.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override void Flush() => Flushes++;
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            Flushes++;
            return Task.CompletedTask;
        }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }
    }

    private sealed class TestBodyWriter : BodyWriter
    {
        private readonly bool _partial;
        private readonly Exception _failure;

        public TestBodyWriter(bool partial, Exception failure) : base(isBuffered: false)
        {
            _partial = partial;
            _failure = failure;
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            if (_partial)
            {
                writer.WriteElementString("prefix", new string('x', 1024));
                writer.Flush();
            }

            if (_failure != null)
            {
                throw _failure;
            }

            writer.WriteElementString("result", "success");
        }
    }

    private sealed class GatedEncoder : MessageEncoder
    {
        private readonly Exception _failure;
        private readonly bool _shadowBuffer;

        public GatedEncoder(Exception failure, bool shadowBuffer)
        {
            _failure = failure;
            _shadowBuffer = shadowBuffer;
        }

        public TaskCompletionSource<bool> Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource<bool> Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public bool Ended { get; private set; }
        public override string ContentType => "text/xml";
        public override string MediaType => "text/xml";
        public override MessageVersion MessageVersion => MessageVersion.Soap11;

        public override async ValueTask WriteMessageAsync(Message message, Stream stream)
        {
            try
            {
                if (_shadowBuffer)
                {
                    await stream.WriteAsync(new byte[4096], 0, 4096);
                    await stream.WriteAsync(new byte[8192], 0, 8192);
                }
                else
                {
                    await stream.WriteAsync(new byte[] { 1, 2, 3 }, 0, 3);
                }

                Entered.TrySetResult(true);
                await Release.Task;
                if (_failure != null)
                {
                    throw _failure;
                }

                await stream.FlushAsync();
            }
            finally
            {
                Ended = true;
            }
        }

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType) => throw new NotSupportedException();
        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType) => throw new NotSupportedException();
        public override void WriteMessage(Message message, Stream stream) => throw new NotSupportedException();
        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset) => throw new NotSupportedException();
    }
}
