// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Net;
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

public static class StreamedRequestTest
{
    [WcfTheory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public static async Task ReadAsStreamAsync_WriterThrows_PropagatesFailure(bool partial, bool synchronous)
    {
        var failure = new InvalidOperationException("Streamed request writer failed");
        using var handler = new ReaderHandler(true, synchronous);
        Exception exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => SendAsync(handler, new TestWriter(partial, failure)));
        Assert.Same(failure, exception);
        Assert.False(handler.ReadToEnd);
    }

    [WcfTheory]
    [InlineData(false)]
    [InlineData(true)]
    public static async Task CopyToAsync_WriterThrows_PropagatesFailure(bool partial)
    {
        var failure = new InvalidOperationException("Streamed request writer failed");
        using var handler = new ReaderHandler(false, false);
        Exception exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => SendAsync(handler, new TestWriter(partial, failure)));
        Assert.Same(failure, exception);
    }

    [WcfTheory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public static async Task ReadAsStreamAsync_WriterCanceled_DoesNotReturnReply(bool partial, bool synchronous)
    {
        using var handler = new ReaderHandler(true, synchronous);
        var failure = new OperationCanceledException("Streamed request writer canceled");
        OperationCanceledException exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => SendAsync(handler, new TestWriter(partial, failure)));
        Assert.Same(failure, exception);
        Assert.False(handler.ReadToEnd);
    }

    [WcfTheory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public static async Task ReadAsStreamAsync_WriterSucceeds_ReturnsCompleteBody(bool partial, bool synchronous)
    {
        using var handler = new ReaderHandler(true, synchronous);
        await SendAsync(handler, new TestWriter(partial, null));
        Assert.True(handler.ReadToEnd);
        Assert.Contains("<result>success</result>", handler.Body);
        if (partial)
        {
            Assert.Contains(new string('x', 32768), handler.Body);
        }
    }

    [WcfFact]
    public static async Task ReadAsStreamAsync_EmptyFlush_DoesNotCompleteReader()
    {
        using var handler = new ReaderHandler(true, false);
        var writer = new GatedWriter();
        Task operation = SendAsync(handler, writer);
        try
        {
            await writer.Entered.Task.WaitAsync(TimeSpan.FromSeconds(15));
            Assert.False(handler.ReadToEnd);
        }
        finally
        {
            writer.Release.TrySetResult(true);
            await operation;
        }

        Assert.Contains("<result>success</result>", handler.Body);
    }

    [WcfFact]
    public static async Task ReadAsStreamAsync_AbortWhileWriterPending_CancelsRequest()
    {
        using var handler = new ReaderHandler(true, false);
        var writer = new GatedWriter();
        var channelOpened = new TaskCompletionSource<IRequestChannel>(TaskCreationOptions.RunContinuationsAsynchronously);
        Task operation = SendAsync(handler, writer, channel => channelOpened.TrySetResult(channel));
        try
        {
            await writer.Entered.Task.WaitAsync(TimeSpan.FromSeconds(15));
            IRequestChannel channel = await channelOpened.Task;
            channel.Abort();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => operation);
            Assert.False(handler.ReadToEnd);
        }
        finally
        {
            writer.Release.TrySetResult(true);
        }
    }

    [WcfFact]
    public static async Task ReadAsStreamAsync_DisposeWhileWriting_UnblocksWriter()
    {
        var writer = new BlockingWriter();
        using var handler = new ReaderHandler(writer);
        await SendAsync(handler, writer);
        Assert.True(writer.Exited.Task.IsCompletedSuccessfully);
    }

    [WcfTheory]
    [InlineData(false)]
    [InlineData(true)]
    public static async Task ReadAsStreamAsync_ProducerCompletedBeforeRead_PreservesOutcome(bool throwing)
    {
        InvalidOperationException failure = throwing ? new InvalidOperationException("Late reader writer failure") : null;
        var encoder = new CompletionEncoder(failure);
        using Message message = Message.CreateMessage(MessageVersion.Soap11, "urn:request", "request");
        Type type = typeof(BasicHttpBinding).Assembly.GetType("System.ServiceModel.Channels.StreamedMessageContent", throwOnError: true);
        using var content = (HttpContent)Activator.CreateInstance(type, message, encoder, BufferManager.CreateBufferManager(32768, 8192));
        Task completion = (Task)type.BaseType.GetProperty("WriteCompletionTask", BindingFlags.Instance | BindingFlags.NonPublic)
            .GetValue(content);
        using Stream input = await content.ReadAsStreamAsync();
        await encoder.Entered.Task.WaitAsync(TimeSpan.FromSeconds(15));
        encoder.Release.SetResult(true);
        await completion.WaitAsync(TimeSpan.FromSeconds(15));

        Assert.True(input.CanRead);
        using var output = new MemoryStream();
        if (throwing)
        {
            Assert.Same(failure, Assert.Throws<InvalidOperationException>(() => input.CopyTo(output)));
            Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(() => input.CopyToAsync(output)));
        }
        else
        {
            input.CopyTo(output);
            await input.CopyToAsync(output);
            Assert.Equal(0, output.Length);
        }
    }

    private static async Task SendAsync(ReaderHandler handler, BodyWriter writer, Action<IRequestChannel> channelOpened = null)
    {
        var binding = new BasicHttpBinding
        {
            TransferMode = TransferMode.StreamedRequest,
            UseDefaultWebProxy = false
        };
        var parameters = new BindingParameterCollection
        {
            new Func<HttpClientHandler, HttpMessageHandler>(inner =>
            {
                handler.InnerHandler = inner;
                return handler;
            })
        };
        IChannelFactory<IRequestChannel> factory = binding.BuildChannelFactory<IRequestChannel>(parameters);
        IRequestChannel channel = null;
        try
        {
            factory.Open();
            channel = factory.CreateChannel(new EndpointAddress("http://localhost/streamed-request-test"));
            channel.Open();
            channelOpened?.Invoke(channel);
            using Message request = Message.CreateMessage(MessageVersion.Soap11, "urn:request", writer);
            using Message reply = await Task<Message>.Factory.FromAsync(
                (callback, state) => channel.BeginRequest(request, TimeSpan.FromSeconds(15), callback, state),
                channel.EndRequest, null).WaitAsync(TimeSpan.FromSeconds(30));
        }
        finally
        {
            channel?.Abort();
            factory.Abort();
        }
    }

    private sealed class ReaderHandler : DelegatingHandler
    {
        private readonly bool _detached;
        private readonly bool _synchronous;
        private readonly BlockingWriter _blockedWriter;

        public ReaderHandler(bool detached, bool synchronous)
        {
            _detached = detached;
            _synchronous = synchronous;
        }

        public ReaderHandler(BlockingWriter blockedWriter) => _blockedWriter = blockedWriter;

        public bool ReadToEnd { get; private set; }
        public string Body { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using var body = new MemoryStream();
            if (_blockedWriter != null)
            {
                using Stream input = await request.Content.ReadAsStreamAsync(cancellationToken);
                var buffer = new byte[1];
                for (int count = 0; count < 8192 && !_blockedWriter.Entered.Task.IsCompleted; count++)
                {
                    Assert.Equal(1, await input.ReadAsync(buffer, cancellationToken));
                }

                Assert.True(_blockedWriter.Entered.Task.IsCompleted);
                request.Content.Dispose();
                await _blockedWriter.Exited.Task.WaitAsync(TimeSpan.FromSeconds(15));
            }
            else if (_detached)
            {
                using Stream input = await request.Content.ReadAsStreamAsync(cancellationToken);
                if (_synchronous)
                {
                    await Task.Run(() => input.CopyTo(body), cancellationToken);
                }
                else
                {
                    await input.CopyToAsync(body, cancellationToken);
                }
            }
            else
            {
                await request.Content.CopyToAsync(body, cancellationToken);
            }

            ReadToEnd = true;
            Body = Encoding.UTF8.GetString(body.ToArray());
            using var responseBody = new MemoryStream();
            using Message reply = Message.CreateMessage(MessageVersion.Soap11, (string)null, (object)"reply");
            new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8)
                .CreateMessageEncoderFactory().Encoder.WriteMessage(reply, responseBody);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(responseBody.ToArray()),
                RequestMessage = request
            };
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml")
            {
                CharSet = "utf-8"
            };
            return response;
        }
    }

    private sealed class TestWriter : BodyWriter
    {
        private readonly bool _partial;
        private readonly Exception _failure;

        public TestWriter(bool partial, Exception failure) : base(isBuffered: false)
        {
            _partial = partial;
            _failure = failure;
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            if (_partial)
            {
                writer.WriteElementString("prefix", new string('x', 32768));
                writer.Flush();
            }

            if (_failure != null)
            {
                throw _failure;
            }

            writer.WriteElementString("result", "success");
        }
    }

    private sealed class GatedWriter : BodyWriter
    {
        public GatedWriter() : base(isBuffered: false)
        {
        }

        public TaskCompletionSource<bool> Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource<bool> Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer) => throw new NotSupportedException();

        protected override async Task OnWriteBodyContentsAsync(XmlDictionaryWriter writer)
        {
            await writer.FlushAsync();
            await writer.FlushAsync();
            Entered.TrySetResult(true);
            await Release.Task;
            writer.WriteElementString("result", "success");
        }
    }

    private sealed class BlockingWriter : BodyWriter
    {
        public BlockingWriter() : base(isBuffered: false)
        {
        }

        public TaskCompletionSource<bool> Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource<bool> Exited { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer) => throw new NotSupportedException();

        protected override Task OnWriteBodyContentsAsync(XmlDictionaryWriter writer) =>
            Task.Run(() =>
            {
                Entered.TrySetResult(true);
                try
                {
                    writer.WriteString(new string('x', 32768));
                    writer.Flush();
                }
                finally
                {
                    Exited.TrySetResult(true);
                }
            });
    }

    private sealed class CompletionEncoder : MessageEncoder
    {
        private readonly Exception _failure;

        public CompletionEncoder(Exception failure) => _failure = failure;
        public TaskCompletionSource<bool> Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource<bool> Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public override string ContentType => "text/xml";
        public override string MediaType => "text/xml";
        public override MessageVersion MessageVersion => MessageVersion.Soap11;

        public override ValueTask WriteMessageAsync(Message message, Stream stream) => new(WriteAsync());

        private async Task WriteAsync()
        {
            Entered.TrySetResult(true);
            await Release.Task;
            if (_failure != null)
            {
                throw _failure;
            }
        }

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType) => throw new NotSupportedException();
        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType) => throw new NotSupportedException();
        public override void WriteMessage(Message message, Stream stream) => throw new NotSupportedException();
        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset) => throw new NotSupportedException();
    }
}
