// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IO;
using System.Runtime;
using System.Xml;

namespace System.ServiceModel.Channels
{
    public static class ByteStreamMessage
    {
        public static Message CreateMessage(Stream stream)
        {
            if (stream == null)
            {
                throw Fx.Exception.ArgumentNull(nameof(stream));
            }

            return CreateMessage(stream, XmlDictionaryReaderQuotas.Max, true);
        }

        public static Message CreateMessage(ArraySegment<byte> buffer)
        {
            return CreateMessage(buffer, null);
        }

        public static Message CreateMessage(ArraySegment<byte> buffer, BufferManager bufferManager)
        {
            if (buffer.Array == null)
            {
                throw Fx.Exception.ArgumentNull("buffer.Array", SRP.Format(SRP.ArgumentPropertyShouldNotBeNullError, "buffer.Array"));
            }

            ByteStreamBufferedMessageData data = new ByteStreamBufferedMessageData(buffer, bufferManager);

            // moveBodyReaderToContent is true, for consistency with the other implementations of Message (including the Message base class itself)
            return CreateMessage(data, XmlDictionaryReaderQuotas.Max, true);
        }

        internal static Message CreateMessage(Stream stream, XmlDictionaryReaderQuotas quotas, bool moveBodyReaderToContent)
        {
            return new InternalByteStreamMessage(stream, quotas, moveBodyReaderToContent);
        }

        internal static Message CreateMessage(ByteStreamBufferedMessageData bufferedMessageData, XmlDictionaryReaderQuotas quotas, bool moveBodyReaderToContent)
        {
            return new InternalByteStreamMessage(bufferedMessageData, quotas, moveBodyReaderToContent);
        }

        internal static bool IsInternalByteStreamMessage(Message message)
        {
            Fx.Assert(message != null, "message should not be null");
            return message is InternalByteStreamMessage;
        }

        internal class InternalByteStreamMessage : Message
        {
            private BodyWriter _bodyWriter;
            private readonly MessageHeaders _headers;
            private readonly MessageProperties _properties;
            private XmlByteStreamReader _reader;
            private bool _moveBodyReaderToContent;

            public InternalByteStreamMessage(ByteStreamBufferedMessageData bufferedMessageData, XmlDictionaryReaderQuotas quotas, bool moveBodyReaderToContent)
            {
                // Assign both writer and reader here so that we can CreateBufferedCopy without the need to
                // abstract between a streamed or buffered message. We're protected here by the state on Message
                // preventing both a read/write.

                quotas = ByteStreamMessageUtility.EnsureQuotas(quotas);

                _bodyWriter = new ByteStreamBufferedBodyWriter(bufferedMessageData);
                _headers = new MessageHeaders(MessageVersion.None);
                _properties = new MessageProperties();
                _reader = new XmlBufferedByteStreamReader(bufferedMessageData, quotas);
                _moveBodyReaderToContent = moveBodyReaderToContent;
            }

            public InternalByteStreamMessage(Stream stream, XmlDictionaryReaderQuotas quotas, bool moveBodyReaderToContent)
            {
                // Assign both writer and reader here so that we can CreateBufferedCopy without the need to
                // abstract between a streamed or buffered message. We're protected here by the state on Message
                // preventing both a read/write on the same stream.

                quotas = ByteStreamMessageUtility.EnsureQuotas(quotas);

                _bodyWriter = StreamedBodyWriter.Create(stream);
                _headers = new MessageHeaders(MessageVersion.None);
                _properties = new MessageProperties();
                _reader = XmlStreamedByteStreamReader.Create(stream, quotas);
                _moveBodyReaderToContent = moveBodyReaderToContent;
            }

            private InternalByteStreamMessage(ByteStreamBufferedMessageData messageData, MessageHeaders headers, MessageProperties properties, XmlDictionaryReaderQuotas quotas, bool moveBodyReaderToContent)
            {
                _headers = new MessageHeaders(headers);
                _properties = new MessageProperties(properties);
                _bodyWriter = new ByteStreamBufferedBodyWriter(messageData);
                _reader = new XmlBufferedByteStreamReader(messageData, quotas);
                _moveBodyReaderToContent = moveBodyReaderToContent;
            }

            public override MessageHeaders Headers
            {
                get
                {
                    if (IsDisposed)
                    {
                        throw Fx.Exception.ObjectDisposed(SRP.Format(SRP.ObjectDisposed, "message"));
                    }

                    return _headers;
                }
            }

            public override bool IsEmpty
            {
                get
                {
                    if (IsDisposed)
                    {
                        throw Fx.Exception.ObjectDisposed(SRP.Format(SRP.ObjectDisposed, "message"));
                    }

                    return false;
                }
            }

            public override bool IsFault
            {
                get
                {
                    if (IsDisposed)
                    {
                        throw Fx.Exception.ObjectDisposed(SRP.Format(SRP.ObjectDisposed, "message"));
                    }

                    return false;
                }
            }

            public override MessageProperties Properties
            {
                get
                {
                    if (IsDisposed)
                    {
                        throw Fx.Exception.ObjectDisposed(SRP.Format(SRP.ObjectDisposed, "message"));
                    }

                    return _properties;
                }
            }

            public override MessageVersion Version
            {
                get
                {
                    if (IsDisposed)
                    {
                        throw Fx.Exception.ObjectDisposed(SRP.Format(SRP.ObjectDisposed, "message"));
                    }

                    return MessageVersion.None;
                }
            }

            protected override void OnBodyToString(XmlDictionaryWriter writer)
            {
                if (_bodyWriter.IsBuffered)
                {
                    _bodyWriter.WriteBodyContents(writer);
                }
                else
                {
                    writer.WriteString(SRP.MessageBodyIsStream);
                }
            }

            protected override void OnClose()
            {
                Exception ex = null;
                try
                {
                    base.OnClose();
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    ex = e;
                }

                try
                {
                    if (_properties != null)
                    {
                        _properties.Dispose();
                    }
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    if (ex == null)
                    {
                        ex = e;
                    }
                }

                try
                {
                    if (_reader != null)
                    {
                        _reader.Close();
                    }
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    if (ex == null)
                    {
                        ex = e;
                    }
                }

                if (ex != null)
                {
                    throw Fx.Exception.AsError(ex);
                }

                _bodyWriter = null;
            }

            protected override MessageBuffer OnCreateBufferedCopy(int maxBufferSize)
            {
                ByteStreamBufferedBodyWriter bufferedBodyWriter;
                if (_bodyWriter.IsBuffered)
                {
                    // Can hand this off in buffered case without making a new one.
                    bufferedBodyWriter = (ByteStreamBufferedBodyWriter)_bodyWriter;
                }
                else
                {
                    bufferedBodyWriter = (ByteStreamBufferedBodyWriter)_bodyWriter.CreateBufferedCopy(maxBufferSize);
                }

                // Protected by Message state to be called only once.
                _bodyWriter = null;
                return new ByteStreamMessageBuffer(bufferedBodyWriter.MessageData, _headers, _properties, _reader.Quotas, _moveBodyReaderToContent);
            }

            protected override T OnGetBody<T>(XmlDictionaryReader reader)
            {
                Fx.Assert(reader is XmlByteStreamReader, "reader should be XmlByteStreamReader");
                if (IsDisposed)
                {
                    throw Fx.Exception.ObjectDisposed(SRP.Format(SRP.ObjectDisposed, "message"));
                }

                Type typeT = typeof(T);
                if (typeof(Stream) == typeT)
                {
                    Stream stream = (reader as XmlByteStreamReader).ToStream();
                    reader.Close();
                    return (T)(object)stream;
                }
                else if (typeof(byte[]) == typeT)
                {
                    byte[] buffer = (reader as XmlByteStreamReader).ToByteArray();
                    reader.Close();
                    return (T)(object)buffer;
                }

                throw Fx.Exception.AsError(
                    new NotSupportedException(SRP.Format(SRP.ByteStreamMessageGetTypeNotSupported, typeT.FullName)));
            }

            protected override XmlDictionaryReader OnGetReaderAtBodyContents()
            {
                XmlDictionaryReader r = _reader;
                _reader = null;

                if ((r != null) && _moveBodyReaderToContent)
                {
                    r.MoveToContent();
                }

                return r;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                _bodyWriter.WriteBodyContents(writer);
            }

            internal class ByteStreamBufferedBodyWriter : BodyWriter
            {
                public ByteStreamBufferedBodyWriter(ByteStreamBufferedMessageData bufferedMessageData)
                    : base(true)
                {
                    MessageData = bufferedMessageData;
                }

                internal ByteStreamBufferedMessageData MessageData { get; }

                protected override BodyWriter OnCreateBufferedCopy(int maxBufferSize)
                {
                    // Never called because when copying a Buffered message, we simply hand off the existing BodyWriter
                    // to the new message.
                    Fx.Assert(false, "This is never called");
                    return null;
                }

                protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
                {
                    writer.WriteStartElement(ByteStreamMessageUtility.StreamElementName, string.Empty);
                    writer.WriteBase64(MessageData.Buffer.Array, MessageData.Buffer.Offset, MessageData.Buffer.Count);
                    writer.WriteEndElement();
                }
            }

            internal abstract class StreamedBodyWriter : BodyWriter
            {
                private StreamedBodyWriter()
                    : base(false)
                {
                }

                public static StreamedBodyWriter Create(Stream stream) => new StreamBasedStreamedBodyWriter(stream);

                // OnCreateBufferedCopy / OnWriteBodyContents can only be called once - protected by state on Message (either copied or written once)
                protected override BodyWriter OnCreateBufferedCopy(int maxBufferSize)
                {
                    using (BufferManagerOutputStream bufferedStream = new BufferManagerOutputStream(SRP.MaxReceivedMessageSizeExceeded, maxBufferSize))
                    {
                        using (XmlDictionaryWriter writer = new XmlByteStreamWriter(bufferedStream, true))
                        {
                            OnWriteBodyContents(writer);
                            writer.Flush();
                            byte[] bytesArray = bufferedStream.ToArray(out int size);
                            ByteStreamBufferedMessageData bufferedMessageData = new ByteStreamBufferedMessageData(new ArraySegment<byte>(bytesArray, 0, size));
                            return new ByteStreamBufferedBodyWriter(bufferedMessageData);
                        }
                    }
                }

                // OnCreateBufferedCopy / OnWriteBodyContents can only be called once - protected by state on Message (either copied or written once)
                protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
                {
                    writer.WriteStartElement(ByteStreamMessageUtility.StreamElementName, string.Empty);
                    writer.WriteValue(new ByteStreamStreamProvider(GetStream()));
                    writer.WriteEndElement();
                }

                protected abstract Stream GetStream();

                internal class ByteStreamStreamProvider : IStreamProvider
                {
                    private readonly Stream _stream;

                    internal ByteStreamStreamProvider(Stream stream)
                    {
                        _stream = stream;
                    }

                    public Stream GetStream() => _stream;

                    public void ReleaseStream(Stream stream)
                    {
                        //Noop
                    }
                }

                internal class StreamBasedStreamedBodyWriter : StreamedBodyWriter
                {
                    private readonly Stream _stream;

                    public StreamBasedStreamedBodyWriter(Stream stream)
                    {
                        _stream = stream;
                    }

                    protected override Stream GetStream() => _stream;
                }
            }

            internal class ByteStreamMessageBuffer : MessageBuffer
            {
                private bool _closed;
                private MessageHeaders _headers;
                private ByteStreamBufferedMessageData _messageData;
                private MessageProperties _properties;
                private XmlDictionaryReaderQuotas _quotas;
                private bool _moveBodyReaderToContent;

                public ByteStreamMessageBuffer(ByteStreamBufferedMessageData messageData, MessageHeaders headers, MessageProperties properties, XmlDictionaryReaderQuotas quotas, bool moveBodyReaderToContent)
                    : base()
                {
                    _messageData = messageData;
                    _headers = new MessageHeaders(headers);
                    _properties = new MessageProperties(properties);
                    _quotas = new XmlDictionaryReaderQuotas();
                    quotas.CopyTo(_quotas);
                    _moveBodyReaderToContent = moveBodyReaderToContent;

                    _messageData.Open();
                }

                public override int BufferSize => _messageData.Buffer.Count;

                private object ThisLock { get; } = new object();

                public override void Close()
                {
                    lock (ThisLock)
                    {
                        if (!_closed)
                        {
                            _closed = true;
                            _headers = null;

                            if (_properties != null)
                            {
                                _properties.Dispose();
                                _properties = null;
                            }

                            _messageData.Close();
                            _messageData = null;
                            _quotas = null;
                        }
                    }
                }

                public override Message CreateMessage()
                {
                    lock (ThisLock)
                    {
                        if (_closed)
                        {
                            throw Fx.Exception.ObjectDisposed(SRP.Format(SRP.ObjectDisposed, "message"));
                        }

                        return new InternalByteStreamMessage(_messageData, _headers, _properties, _quotas, _moveBodyReaderToContent);
                    }
                }
            }
        }
    }
}
