// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace Extensibility.MessageEncoder.Tests
{
    internal class CustomTextMessageEncoder : System.ServiceModel.Channels.MessageEncoder
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
            int messageLength;
            using (MemoryStream stream = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(stream, _writerSettings))
                {
                    message.WriteMessage(writer);
                }

                stream.TryGetBuffer(out messageBuffer);
                messageLength = (int)stream.Position;
            }

            int totalLength = messageLength + messageOffset;
            byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
            Array.Copy(messageBuffer.Array, 0, totalBytes, messageOffset, messageLength);

            ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
            return byteArray;
        }
    }
}
