// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IO;
using System.Runtime;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal class ByteStreamMessageEncoder : MessageEncoder
    {
        private readonly string _maxSentMessageSizeExceededResourceString;
        private readonly XmlDictionaryReaderQuotas _quotas;
        private readonly XmlDictionaryReaderQuotas _bufferedReadReaderQuotas;

        // Specifies if this encoder produces Messages that provide a body reader (with the
        // Message.GetReaderAtBodyContents() method) positioned on content.
        private readonly bool _moveBodyReaderToContent;

        public ByteStreamMessageEncoder(XmlDictionaryReaderQuotas quotas, bool moveBodyReaderToContent)
        {
            _quotas = new XmlDictionaryReaderQuotas();
            quotas.CopyTo(_quotas);

            _bufferedReadReaderQuotas = EncoderHelpers.GetBufferedReadQuotas(_quotas);

            _maxSentMessageSizeExceededResourceString = SRP.MaxSentMessageSizeExceeded;
            _moveBodyReaderToContent = moveBodyReaderToContent;
        }

        public override string ContentType => null;

        public override string MediaType => null;

        public override MessageVersion MessageVersion => MessageVersion.None;

        public override bool IsContentTypeSupported(string contentType) => true;

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            if (stream == null)
            {
                throw Fx.Exception.ArgumentNull(nameof(stream));
            }

            Message message = ByteStreamMessage.CreateMessage(stream, _quotas, _moveBodyReaderToContent);
            message.Properties.Encoder = this;

            return message;
        }

        public override ValueTask<Message> ReadMessageAsync(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            return new ValueTask<Message>(ReadMessage(stream, maxSizeOfHeaders, contentType));
        }

        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            if (buffer.Array == null)
            {
                throw Fx.Exception.ArgumentNull("buffer.Array");
            }

            if (bufferManager == null)
            {
                throw Fx.Exception.ArgumentNull(nameof(bufferManager));
            }

            ByteStreamBufferedMessageData messageData = new ByteStreamBufferedMessageData(buffer, bufferManager);

            Message message = ByteStreamMessage.CreateMessage(messageData, _bufferedReadReaderQuotas, _moveBodyReaderToContent);
            message.Properties.Encoder = this;

            return message;
        }

        public override void WriteMessage(Message message, Stream stream)
        {
            if (message == null)
            {
                throw Fx.Exception.ArgumentNull(nameof(message));
            }

            if (stream == null)
            {
                throw Fx.Exception.ArgumentNull(nameof(stream));
            }

            ThrowIfMismatchedMessageVersion(message);
            message.Properties.Encoder = this;

            using (XmlDictionaryWriter writer = new XmlByteStreamWriter(stream, false))
            {
                message.WriteMessage(writer);
                writer.Flush();
            }
        }

        public override async ValueTask WriteMessageAsync(Message message, Stream stream)
        {
            if (message == null)
            {
                throw Fx.Exception.ArgumentNull(nameof(message));
            }

            if (stream == null)
            {
                throw Fx.Exception.ArgumentNull(nameof(stream));
            }

            ThrowIfMismatchedMessageVersion(message);
            message.Properties.Encoder = this;

            using (XmlDictionaryWriter writer = new XmlByteStreamWriter(stream, false))
            {
                await message.WriteMessageAsync(writer);
                await writer.FlushAsync();
            }
        }

        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            if (message == null)
            {
                throw Fx.Exception.ArgumentNull(nameof(message));
            }

            if (bufferManager == null)
            {
                throw Fx.Exception.ArgumentNull(nameof(bufferManager));
            }

            if (maxMessageSize < 0)
            {
                throw Fx.Exception.ArgumentOutOfRange(nameof(maxMessageSize), maxMessageSize, SRP.Format(SRP.ArgumentOutOfMinRange, 0));
            }

            if (messageOffset < 0)
            {
                throw Fx.Exception.ArgumentOutOfRange(nameof(messageOffset), messageOffset, SRP.Format(SRP.ArgumentOutOfMinRange, 0));
            }

            ThrowIfMismatchedMessageVersion(message);
            message.Properties.Encoder = this;

            ArraySegment<byte> messageBuffer;

            using (BufferManagerOutputStream stream = new BufferManagerOutputStream(_maxSentMessageSizeExceededResourceString, 0, maxMessageSize, bufferManager))
            {
                stream.Skip(messageOffset);
                using (XmlDictionaryWriter writer = new XmlByteStreamWriter(stream, true))
                {
                    message.WriteMessage(writer);
                    writer.Flush();
                    byte[] bytes = stream.ToArray(out int size);
                    messageBuffer = new ArraySegment<byte>(bytes, messageOffset, size - messageOffset);
                }
            }

            return messageBuffer;
        }

        public override string ToString() => ByteStreamMessageUtility.EncoderName;
    }
}
