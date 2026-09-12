// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public static class ByteStreamMessageEncoderTest
{
    private sealed class TrackingBufferManager : BufferManager
    {
        public byte[] LastReturnedBuffer { get; private set; }

        public int ReturnCount { get; private set; }

        public override void Clear()
        {
        }

        public override byte[] TakeBuffer(int bufferSize) => new byte[bufferSize];

        public override void ReturnBuffer(byte[] buffer)
        {
            LastReturnedBuffer = buffer;
            ReturnCount++;
        }
    }

    private static MessageEncoder CreateEncoder(XmlDictionaryReaderQuotas quotas = null)
    {
        ByteStreamMessageEncodingBindingElement element = new ByteStreamMessageEncodingBindingElement(quotas);
        return element.CreateMessageEncoderFactory().Encoder;
    }

    private static byte[] ReadAll(Stream stream)
    {
        using (MemoryStream copy = new MemoryStream())
        {
            stream.CopyTo(copy);
            return copy.ToArray();
        }
    }

    [WcfFact]
    public static void Encoder_AdvertisesRawMessageIdentity()
    {
        MessageEncoder encoder = CreateEncoder();

        Assert.Null(encoder.ContentType);
        Assert.Null(encoder.MediaType);
        Assert.Equal(MessageVersion.None, encoder.MessageVersion);
        Assert.Equal("ByteStreamMessageEncoder", encoder.ToString());
        Assert.True(encoder.IsContentTypeSupported(null));
        Assert.True(encoder.IsContentTypeSupported("application/custom"));
    }

    [WcfFact]
    public static void ReadMessage_StreamPreservesPayloadAndEncoder()
    {
        byte[] payload = new byte[] { 0x10, 0x20, 0x30, 0x40 };
        MessageEncoder encoder = CreateEncoder();
        using (MemoryStream stream = new MemoryStream(payload, false))
        using (Message message = encoder.ReadMessage(stream, int.MaxValue, "application/custom"))
        {
            Assert.Same(encoder, message.Properties.Encoder);
            Assert.Equal(payload, ReadAll(message.GetBody<Stream>()));
        }
    }

    [WcfFact]
    public static async Task ReadMessageAsync_StreamPreservesPayload()
    {
        byte[] payload = new byte[] { 0x10, 0x20, 0x30, 0x40 };
        MessageEncoder encoder = CreateEncoder();
        using (MemoryStream stream = new MemoryStream(payload, false))
        using (Message message = await encoder.ReadMessageAsync(stream, int.MaxValue, "application/custom"))
        {
            Assert.Equal(payload, ReadAll(message.GetBody<Stream>()));
        }
    }

    [WcfFact]
    public static void ReadMessage_BufferPreservesSegmentAndReturnsBuffer()
    {
        byte[] backingBuffer = new byte[] { 0xFF, 0x10, 0x20, 0x30, 0xFF };
        byte[] payload = new byte[] { 0x10, 0x20, 0x30 };
        TrackingBufferManager bufferManager = new TrackingBufferManager();
        MessageEncoder encoder = CreateEncoder();

        using (Message message = encoder.ReadMessage(
            new ArraySegment<byte>(backingBuffer, 1, payload.Length),
            bufferManager,
            "application/custom"))
        {
            Assert.Same(encoder, message.Properties.Encoder);
            Assert.Equal(payload, message.GetBody<byte[]>());
        }

        Assert.Equal(1, bufferManager.ReturnCount);
        Assert.Same(backingBuffer, bufferManager.LastReturnedBuffer);
    }

    [WcfFact]
    public static void WriteMessage_StreamWritesPayloadWithoutFraming()
    {
        byte[] payload = new byte[] { 0x10, 0x20, 0x30, 0x40 };
        MessageEncoder encoder = CreateEncoder();
        using (Message message = ByteStreamMessage.CreateMessage(new ArraySegment<byte>(payload)))
        using (MemoryStream stream = new MemoryStream())
        {
            encoder.WriteMessage(message, stream);

            Assert.Same(encoder, message.Properties.Encoder);
            Assert.Equal(payload, stream.ToArray());
        }
    }

    [WcfFact]
    public static async Task WriteMessageAsync_StreamWritesPayloadWithoutFraming()
    {
        byte[] payload = new byte[] { 0x10, 0x20, 0x30, 0x40 };
        MessageEncoder encoder = CreateEncoder();
        using (Message message = ByteStreamMessage.CreateMessage(new ArraySegment<byte>(payload)))
        using (MemoryStream stream = new MemoryStream())
        {
            await encoder.WriteMessageAsync(message, stream);

            Assert.Equal(payload, stream.ToArray());
        }
    }

    [WcfFact]
    public static void WriteMessage_BufferRespectsMessageOffset()
    {
        byte[] payload = new byte[] { 0x10, 0x20, 0x30, 0x40 };
        MessageEncoder encoder = CreateEncoder();
        BufferManager bufferManager = BufferManager.CreateBufferManager(1024 * 1024, 1024);
        ArraySegment<byte> encoded = default;

        try
        {
            using (Message message = ByteStreamMessage.CreateMessage(new ArraySegment<byte>(payload)))
            {
                encoded = encoder.WriteMessage(message, 1024, bufferManager, 5);
            }

            Assert.Equal(5, encoded.Offset);
            Assert.Equal(payload.Length, encoded.Count);

            byte[] actual = new byte[encoded.Count];
            Buffer.BlockCopy(encoded.Array, encoded.Offset, actual, 0, actual.Length);
            Assert.Equal(payload, actual);
        }
        finally
        {
            if (encoded.Array != null)
            {
                bufferManager.ReturnBuffer(encoded.Array);
            }

            bufferManager.Clear();
        }
    }

    [WcfFact]
    public static void WriteMessage_BufferEnforcesMaximumSize()
    {
        byte[] payload = new byte[] { 0x10, 0x20, 0x30, 0x40 };
        MessageEncoder encoder = CreateEncoder();
        BufferManager bufferManager = BufferManager.CreateBufferManager(1024 * 1024, 1024);

        try
        {
            using (Message message = ByteStreamMessage.CreateMessage(new ArraySegment<byte>(payload)))
            {
                Assert.Throws<QuotaExceededException>(() => encoder.WriteMessage(message, payload.Length - 1, bufferManager, 0));
            }
        }
        finally
        {
            bufferManager.Clear();
        }
    }

    [WcfFact]
    public static void ReadMessage_ValidatesArguments()
    {
        MessageEncoder encoder = CreateEncoder();
        BufferManager bufferManager = BufferManager.CreateBufferManager(1024, 1024);

        try
        {
            Assert.Throws<ArgumentNullException>(() => encoder.ReadMessage((Stream)null, int.MaxValue, null));
            Assert.Throws<ArgumentNullException>(() => encoder.ReadMessage(default(ArraySegment<byte>), bufferManager, null));
            Assert.Throws<ArgumentNullException>(() => encoder.ReadMessage(new ArraySegment<byte>(new byte[1]), null, null));
        }
        finally
        {
            bufferManager.Clear();
        }
    }

    [WcfFact]
    public static void WriteMessage_ValidatesArguments()
    {
        MessageEncoder encoder = CreateEncoder();
        BufferManager bufferManager = BufferManager.CreateBufferManager(1024, 1024);
        using (Message message = ByteStreamMessage.CreateMessage(new ArraySegment<byte>(new byte[] { 0x10 })))
        using (MemoryStream stream = new MemoryStream())
        {
            Assert.Throws<ArgumentNullException>(() => encoder.WriteMessage(null, stream));
            Assert.Throws<ArgumentNullException>(() => encoder.WriteMessage(message, (Stream)null));
            Assert.Throws<ArgumentNullException>(() => encoder.WriteMessage(null, 1024, bufferManager, 0));
            Assert.Throws<ArgumentNullException>(() => encoder.WriteMessage(message, 1024, null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => encoder.WriteMessage(message, -1, bufferManager, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => encoder.WriteMessage(message, 1024, bufferManager, -1));
        }

        bufferManager.Clear();
    }

    [WcfFact]
    public static void WriteMessage_RejectsMismatchedMessageVersion()
    {
        MessageEncoder encoder = CreateEncoder();
        using (Message message = Message.CreateMessage(MessageVersion.Soap11, "urn:test"))
        using (MemoryStream stream = new MemoryStream())
        {
            Assert.Throws<ProtocolException>(() => encoder.WriteMessage(message, stream));
        }
    }

    [WcfFact]
    public static void ReaderQuotas_AreAppliedToStreamedReads()
    {
        XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas
        {
            MaxDepth = 7,
            MaxStringContentLength = 128
        };
        MessageEncoder encoder = CreateEncoder(quotas);
        using (MemoryStream stream = new MemoryStream(new byte[] { 0x10 }, false))
        using (Message message = encoder.ReadMessage(stream, int.MaxValue, null))
        {
            XmlDictionaryReader reader = message.GetReaderAtBodyContents();

            Assert.Equal(7, reader.Quotas.MaxDepth);
            Assert.Equal(128, reader.Quotas.MaxStringContentLength);
        }
    }
}
