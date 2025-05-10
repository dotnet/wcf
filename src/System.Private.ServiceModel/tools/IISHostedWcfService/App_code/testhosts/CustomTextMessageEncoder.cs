// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF.Channels;
#else
using System;
using System.IO;
using System.ServiceModel.Channels;
#endif
using System.Text;
using System.Xml;

namespace WcfService
{
    internal class CustomTextMessageEncoder : MessageEncoder
    {
        private CustomTextMessageEncoderFactory _factory;
        private XmlWriterSettings _writerSettings;
        private string _contentType;

        public CustomTextMessageEncoder(CustomTextMessageEncoderFactory factory)
        {
            _factory = factory;

            _writerSettings = new XmlWriterSettings();
            _writerSettings.Encoding = Encoding.GetEncoding(factory.CharSet);
            _contentType = string.Format("{0}; charset={1}",
                _factory.MediaType, _writerSettings.Encoding.WebName);
        }

        public override string ContentType
        {
            get
            {
                return _contentType;
            }
        }

        public override string MediaType
        {
            get
            {
                return _factory.MediaType;
            }
        }

        public override MessageVersion MessageVersion
        {
            get
            {
                return _factory.MessageVersion;
            }
        }

#if NET
        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            XmlReader reader = XmlReader.Create(new MemoryStream(buffer.Array, buffer.Offset, buffer.Count));
            Message message = Message.CreateMessage(reader, int.MaxValue, MessageVersion);
            bufferManager.ReturnBuffer(buffer.Array);

            return message;
        }

        public override async Task<Message> ReadMessageAsync(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                Async = true
            };

            XmlReader reader = XmlReader.Create(stream, settings);
            Message message = Message.CreateMessage(reader, maxSizeOfHeaders, MessageVersion);
            return await Task.FromResult(message);
        }

        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            MemoryStream memoryStream = new MemoryStream();
            XmlWriter xmlWriter = XmlWriter.Create(memoryStream, _writerSettings);

            message.WriteMessage(xmlWriter);
            xmlWriter.Flush();
            xmlWriter.Dispose();

            int messageLength = (int)memoryStream.Length;
            int totalLength = messageLength + messageOffset;

            byte[] totalBuffer = bufferManager.TakeBuffer(totalLength);

            ArraySegment<byte> messageBytes = new ArraySegment<byte>(memoryStream.GetBuffer(), 0, messageLength);
            Buffer.BlockCopy(messageBytes.Array, 0, totalBuffer, messageOffset, messageLength);

            return new ArraySegment<byte>(totalBuffer, messageOffset, messageLength);
        }

        public override async Task WriteMessageAsync(Message message, Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Async = true,
                CloseOutput = false
            };

            await using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
            {
                message.WriteMessage(xmlWriter);
                await xmlWriter.FlushAsync();
            }
        }
#else
        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            XmlReader reader = XmlReader.Create(stream);
            return Message.CreateMessage(reader, maxSizeOfHeaders, MessageVersion);
        }

        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            byte[] msgContents = new byte[buffer.Count];
            Array.Copy(buffer.Array, buffer.Offset, msgContents, 0, msgContents.Length);
            bufferManager.ReturnBuffer(buffer.Array);

            MemoryStream stream = new MemoryStream(msgContents);
            return ReadMessage(stream, int.MaxValue);
        }

        public override void WriteMessage(Message message, Stream stream)
        {
            using (XmlWriter writer = XmlWriter.Create(stream, _writerSettings))
            {
                message.WriteMessage(writer);
            }
        }

        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            ArraySegment<byte> messageBuffer;
            byte[] writeBuffer = null;

            int messageLength;
            using (MemoryStream stream = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(stream, _writerSettings))
                {
                    message.WriteMessage(writer);
                }

                // TryGetBuffer is the preferred path but requires 4.6
                //stream.TryGetBuffer(out messageBuffer);
                writeBuffer = stream.ToArray();
                messageBuffer = new ArraySegment<byte>(writeBuffer);

                messageLength = (int)stream.Position;
            }

            int totalLength = messageLength + messageOffset;
            byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
            Array.Copy(messageBuffer.Array, 0, totalBytes, messageOffset, messageLength);

            ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
            return byteArray;
        }
#endif
    }
}
