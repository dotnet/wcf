// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public static class ByteStreamMessageTest
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

    private static byte[] ReadAll(Stream stream)
    {
        using (MemoryStream copy = new MemoryStream())
        {
            stream.CopyTo(copy);
            return copy.ToArray();
        }
    }

    [WcfFact]
    public static void CreateMessage_ArraySegmentWithOffset_ReadsEntireSegment()
    {
        byte[] payload = new byte[] { 0x10, 0x20, 0x30, 0x40 };
        const int Offset = 3;
        byte[] backingBuffer = new byte[Offset + payload.Length + 2];
        Buffer.BlockCopy(payload, 0, backingBuffer, Offset, payload.Length);

        using (Message message = ByteStreamMessage.CreateMessage(
            new ArraySegment<byte>(backingBuffer, Offset, payload.Length)))
        {
            XmlDictionaryReader reader = message.GetReaderAtBodyContents();
            byte[] actual = new byte[payload.Length];

            int bytesRead = reader.ReadContentAsBase64(actual, 0, actual.Length);

            Assert.Equal(payload.Length, bytesRead);
            Assert.Equal(payload, actual);
            Assert.Equal(0, reader.ReadContentAsBase64(actual, 0, actual.Length));
        }
    }

    [WcfFact]
    public static void CreateMessage_Stream_GetBodyReturnsOriginalStream()
    {
        byte[] payload = new byte[] { 0x10, 0x20, 0x30, 0x40 };
        using (MemoryStream stream = new MemoryStream(payload, false))
        using (Message message = ByteStreamMessage.CreateMessage(stream))
        {
            Stream body = message.GetBody<Stream>();

            Assert.Same(stream, body);
            Assert.Equal(payload, ReadAll(body));
        }
    }

    [WcfFact]
    public static void CreateMessage_ArraySegment_GetBodyReturnsCopyOfSegment()
    {
        byte[] payload = new byte[] { 0x10, 0x20, 0x30, 0x40 };
        byte[] backingBuffer = new byte[] { 0xFF, 0xFF, 0x10, 0x20, 0x30, 0x40, 0xFF };

        using (Message message = ByteStreamMessage.CreateMessage(new ArraySegment<byte>(backingBuffer, 2, payload.Length)))
        {
            byte[] body = message.GetBody<byte[]>();

            Assert.NotSame(backingBuffer, body);
            Assert.Equal(payload, body);
        }
    }

    [WcfFact]
    public static void CreateMessage_Stream_GetBodyAsByteArrayThrows()
    {
        using (MemoryStream stream = new MemoryStream(new byte[] { 0x10 }, false))
        using (Message message = ByteStreamMessage.CreateMessage(stream))
        {
            Assert.Throws<InvalidOperationException>(() => message.GetBody<byte[]>());
        }
    }

    [WcfFact]
    public static void CreateMessage_RejectsNullInputs()
    {
        Assert.Throws<ArgumentNullException>(() => ByteStreamMessage.CreateMessage((Stream)null));
        Assert.Throws<ArgumentNullException>(() => ByteStreamMessage.CreateMessage(default(ArraySegment<byte>)));
    }

    [WcfFact]
    public static void GetBody_RejectsUnsupportedType()
    {
        using (Message message = ByteStreamMessage.CreateMessage(new ArraySegment<byte>(new byte[] { 0x10 })))
        {
            Assert.Throws<NotSupportedException>(() => message.GetBody<string>());
        }
    }

    [WcfFact]
    public static void CreateBufferedCopy_FromStreamCopiesPayload()
    {
        byte[] payload = new byte[] { 0x10, 0x20, 0x30, 0x40 };
        using (MemoryStream stream = new MemoryStream(payload, false))
        using (Message message = ByteStreamMessage.CreateMessage(stream))
        {
            MessageBuffer buffer = message.CreateBufferedCopy(1024);
            try
            {
                Assert.Equal(payload.Length, buffer.BufferSize);

                using (Message copy = buffer.CreateMessage())
                {
                    Assert.Equal(payload, copy.GetBody<byte[]>());
                }
            }
            finally
            {
                buffer.Close();
            }
        }
    }

    [WcfFact]
    public static void BufferedCopy_ReturnsBufferAfterLastOwnerCloses()
    {
        byte[] backingBuffer = new byte[] { 0xFF, 0x10, 0x20, 0x30, 0xFF };
        byte[] payload = new byte[] { 0x10, 0x20, 0x30 };
        TrackingBufferManager bufferManager = new TrackingBufferManager();
        Message message = ByteStreamMessage.CreateMessage(new ArraySegment<byte>(backingBuffer, 1, payload.Length), bufferManager);
        MessageBuffer buffer = null;

        try
        {
            buffer = message.CreateBufferedCopy(1024);
            message.Close();

            using (Message first = buffer.CreateMessage())
            using (Message second = buffer.CreateMessage())
            {
                Assert.Equal(payload, first.GetBody<byte[]>());
                Assert.Equal(payload, second.GetBody<byte[]>());
            }

            Assert.Equal(0, bufferManager.ReturnCount);

            buffer.Close();
            Assert.Equal(1, bufferManager.ReturnCount);
            Assert.Same(backingBuffer, bufferManager.LastReturnedBuffer);

            buffer.Close();
            Assert.Equal(1, bufferManager.ReturnCount);
        }
        finally
        {
            message.Close();
            buffer?.Close();
        }
    }

    [WcfFact]
    public static void ClosingExtractedStreamTwice_DoesNotReleaseBufferOwnedByMessageBuffer()
    {
        byte[] backingBuffer = new byte[] { 0xFF, 0x10, 0x20, 0x30, 0xFF };
        byte[] payload = new byte[] { 0x10, 0x20, 0x30 };
        TrackingBufferManager bufferManager = new TrackingBufferManager();
        Message message = ByteStreamMessage.CreateMessage(new ArraySegment<byte>(backingBuffer, 1, payload.Length), bufferManager);
        MessageBuffer buffer = null;

        try
        {
            buffer = message.CreateBufferedCopy(1024);
            message.Close();

            using (Message copy = buffer.CreateMessage())
            {
                Stream body = copy.GetBody<Stream>();
                body.Close();
                body.Close();
            }

            Assert.Equal(0, bufferManager.ReturnCount);

            using (Message copy = buffer.CreateMessage())
            {
                Assert.Equal(payload, copy.GetBody<byte[]>());
            }

            buffer.Close();
            Assert.Equal(1, bufferManager.ReturnCount);
        }
        finally
        {
            message.Close();
            buffer?.Close();
        }
    }

    [WcfFact]
    public static void Reader_ReadsAcrossChunkBoundaries()
    {
        byte[] payload = new byte[] { 0x10, 0x20, 0x30, 0x40 };
        using (Message message = ByteStreamMessage.CreateMessage(new ArraySegment<byte>(payload)))
        {
            XmlDictionaryReader reader = message.GetReaderAtBodyContents();
            byte[] destination = new byte[6];

            Assert.Equal(2, reader.ReadContentAsBase64(destination, 1, 2));
            Assert.Equal(2, reader.ReadContentAsBase64(destination, 3, 2));
            Assert.Equal(0, reader.ReadContentAsBase64(destination, 0, destination.Length));
            Assert.Equal(new byte[] { 0, 0x10, 0x20, 0x30, 0x40, 0 }, destination);
        }
    }

    [WcfFact]
    public static void Reader_RejectsInvalidBase64BufferRanges()
    {
        using (Message message = ByteStreamMessage.CreateMessage(new ArraySegment<byte>(new byte[] { 0x10 })))
        {
            XmlDictionaryReader reader = message.GetReaderAtBodyContents();
            byte[] destination = new byte[3];

            Assert.Throws<ArgumentNullException>(() => reader.ReadContentAsBase64(null, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => reader.ReadContentAsBase64(destination, -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => reader.ReadContentAsBase64(destination, destination.Length, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => reader.ReadContentAsBase64(destination, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => reader.ReadContentAsBase64(destination, 2, 2));
            Assert.Equal(0, reader.ReadContentAsBase64(destination, 0, 0));
        }
    }
}
