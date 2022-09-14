// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace System.ServiceModel.Channels
{
    public abstract class MessageBuffer : IDisposable
    {
        public abstract int BufferSize { get; }

        void IDisposable.Dispose()
        {
            Close();
        }

        public abstract void Close();

        public virtual void WriteMessage(Stream stream)
        {
            if (stream == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(stream)));
            }

            Message message = CreateMessage();
            using (message)
            {
                XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(stream, XD.Dictionary, null, false);
                using (writer)
                {
                    message.WriteMessage(writer);
                }
            }
        }

        public virtual string MessageContentType
        {
            get { return BinaryEncodingString.Binary; }
        }

        public abstract Message CreateMessage();

        internal Exception CreateBufferDisposedException()
        {
            return new ObjectDisposedException("", SRP.MessageBufferIsClosed);
        }
    }

    internal class DefaultMessageBuffer : MessageBuffer
    {
        private XmlBuffer _msgBuffer;
        private KeyValuePair<string, object>[] _properties;
        private bool[] _understoodHeaders;
        private bool _closed;
        private MessageVersion _version;
        private Uri _to;
        private string _action;
        private bool _isNullMessage;

        public DefaultMessageBuffer(Message message, XmlBuffer msgBuffer)
        {
            _msgBuffer = msgBuffer;
            _version = message.Version;
            _isNullMessage = message is NullMessage;

            _properties = new KeyValuePair<string, object>[message.Properties.Count];
            ((ICollection<KeyValuePair<string, object>>)message.Properties).CopyTo(_properties, 0);
            _understoodHeaders = new bool[message.Headers.Count];
            for (int i = 0; i < _understoodHeaders.Length; ++i)
            {
                _understoodHeaders[i] = message.Headers.IsUnderstood(i);
            }

            if (_version == MessageVersion.None)
            {
                _to = message.Headers.To;
                _action = message.Headers.Action;
            }
        }

        private object ThisLock
        {
            get { return _msgBuffer; }
        }

        public override int BufferSize
        {
            get { return _msgBuffer.BufferSize; }
        }

        public override void Close()
        {
            lock (ThisLock)
            {
                if (_closed)
                {
                    return;
                }

                _closed = true;
                for (int i = 0; i < _properties.Length; i++)
                {
                    IDisposable disposable = _properties[i].Value as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

        public override Message CreateMessage()
        {
            if (_closed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateBufferDisposedException());
            }

            Message msg;
            if (_isNullMessage)
            {
                msg = new NullMessage();
            }
            else
            {
                msg = Message.CreateMessage(_msgBuffer.GetReader(0), int.MaxValue, _version);
            }

            lock (ThisLock)
            {
                msg.Properties.CopyProperties(_properties);
            }

            for (int i = 0; i < _understoodHeaders.Length; ++i)
            {
                if (_understoodHeaders[i])
                {
                    msg.Headers.AddUnderstood(i);
                }
            }

            if (_to != null)
            {
                msg.Headers.To = _to;
            }

            if (_action != null)
            {
                msg.Headers.Action = _action;
            }

            return msg;
        }
    }

    internal class BufferedMessageBuffer : MessageBuffer
    {
        private IBufferedMessageData _messageData;
        private KeyValuePair<string, object>[] _properties;
        private bool _closed;
        private bool[] _understoodHeaders;
        private bool _understoodHeadersModified;

        public BufferedMessageBuffer(IBufferedMessageData messageData,
            KeyValuePair<string, object>[] properties, bool[] understoodHeaders, bool understoodHeadersModified)
        {
            _messageData = messageData;
            _properties = properties;
            _understoodHeaders = understoodHeaders;
            _understoodHeadersModified = understoodHeadersModified;
            messageData.Open();
        }

        public override int BufferSize
        {
            get
            {
                lock (ThisLock)
                {
                    if (_closed)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateBufferDisposedException());
                    }

                    return _messageData.Buffer.Count;
                }
            }
        }

        public override void WriteMessage(Stream stream)
        {
            if (stream == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(stream)));
            }

            lock (ThisLock)
            {
                if (_closed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateBufferDisposedException());
                }

                ArraySegment<byte> buffer = _messageData.Buffer;
                stream.Write(buffer.Array, buffer.Offset, buffer.Count);
            }
        }

        public override string MessageContentType
        {
            get
            {
                lock (ThisLock)
                {
                    if (_closed)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateBufferDisposedException());
                    }

                    return _messageData.MessageEncoder.ContentType;
                }
            }
        }

        private object ThisLock { get; } = new object();

        public override void Close()
        {
            lock (ThisLock)
            {
                if (!_closed)
                {
                    _closed = true;
                    _messageData.Close();
                    _messageData = null;
                }
            }
        }

        public override Message CreateMessage()
        {
            lock (ThisLock)
            {
                if (_closed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateBufferDisposedException());
                }

                RecycledMessageState recycledMessageState = _messageData.TakeMessageState();
                if (recycledMessageState == null)
                {
                    recycledMessageState = new RecycledMessageState();
                }

                BufferedMessage bufferedMessage = new BufferedMessage(_messageData, recycledMessageState, _understoodHeaders, _understoodHeadersModified);
                bufferedMessage.Properties.CopyProperties(_properties);
                _messageData.Open();
                return bufferedMessage;
            }
        }
    }

    internal class BodyWriterMessageBuffer : MessageBuffer
    {
        private object _thisLock = new object();

        public BodyWriterMessageBuffer(MessageHeaders headers,
            KeyValuePair<string, object>[] properties, BodyWriter bodyWriter)
        {
            BodyWriter = bodyWriter;
            Headers = new MessageHeaders(headers);
            Properties = properties;
        }

        protected object ThisLock
        {
            get { return _thisLock; }
        }

        public override int BufferSize
        {
            get { return 0; }
        }

        public override void Close()
        {
            lock (ThisLock)
            {
                if (!Closed)
                {
                    Closed = true;
                    BodyWriter = null;
                    Headers = null;
                    Properties = null;
                }
            }
        }

        public override Message CreateMessage()
        {
            lock (ThisLock)
            {
                if (Closed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateBufferDisposedException());
                }

                return new BodyWriterMessage(Headers, Properties, BodyWriter);
            }
        }

        protected BodyWriter BodyWriter { get; private set; }

        protected MessageHeaders Headers { get; private set; }

        protected KeyValuePair<string, object>[] Properties { get; private set; }

        protected bool Closed { get; private set; }
    }
}
