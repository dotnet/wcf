// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime;
using System.Runtime.Serialization;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Channels
{
    public abstract class Message : IDisposable
    {
        internal const int InitialBufferSize = 1024;

        public abstract MessageHeaders Headers { get; } // must never return null

        protected bool IsDisposed
        {
            get { return State == MessageState.Closed; }
        }

        public virtual bool IsFault
        {
            get
            {
                if (IsDisposed)
                {
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                }

                return false;
            }
        }

        public virtual bool IsEmpty
        {
            get
            {
                if (IsDisposed)
                {
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                }

                return false;
            }
        }

        public abstract MessageProperties Properties { get; }

        public abstract MessageVersion Version { get; } // must never return null

        internal virtual RecycledMessageState RecycledMessageState
        {
            get { return null; }
        }

        public MessageState State { get; private set; }

        internal void BodyToString(XmlDictionaryWriter writer)
        {
            OnBodyToString(writer);
        }

        public void Close()
        {
            if (State != MessageState.Closed)
            {
                State = MessageState.Closed;
                OnClose();
            }
            else
            {
            }
        }

        public MessageBuffer CreateBufferedCopy(int maxBufferSize)
        {
            if (maxBufferSize < 0)
            {
                throw TraceUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(maxBufferSize), maxBufferSize,
                                                    SRP.ValueMustBeNonNegative), this);
            }

            switch (State)
            {
                case MessageState.Created:
                    State = MessageState.Copied;
                    break;
                case MessageState.Closed:
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                case MessageState.Copied:
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.MessageHasBeenCopied), this);
                case MessageState.Read:
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.MessageHasBeenRead), this);
                case MessageState.Written:
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.MessageHasBeenWritten), this);
                default:
                    Fx.Assert(SRP.InvalidMessageState);
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.InvalidMessageState), this);
            }
            return OnCreateBufferedCopy(maxBufferSize);
        }

        private static Type GetObjectType(object value)
        {
            return (value == null) ? typeof(object) : value.GetType();
        }

        static public Message CreateMessage(MessageVersion version, string action, object body)
        {
            return CreateMessage(version, action, body, DataContractSerializerDefaults.CreateSerializer(GetObjectType(body), int.MaxValue/*maxItems*/));
        }

        static public Message CreateMessage(MessageVersion version, string action, object body, XmlObjectSerializer serializer)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(version)));
            }

            if (serializer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(serializer)));
            }

            return new BodyWriterMessage(version, action, new XmlObjectSerializerBodyWriter(body, serializer));
        }

        static public Message CreateMessage(MessageVersion version, string action, XmlReader body)
        {
            return CreateMessage(version, action, XmlDictionaryReader.CreateDictionaryReader(body));
        }

        static public Message CreateMessage(MessageVersion version, string action, XmlDictionaryReader body)
        {
            if (body == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(body));
            }

            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(version));
            }

            return CreateMessage(version, action, new XmlReaderBodyWriter(body, version.Envelope));
        }

        static public Message CreateMessage(MessageVersion version, string action, BodyWriter body)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(version)));
            }

            if (body == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(body)));
            }

            return new BodyWriterMessage(version, action, body);
        }

        static internal Message CreateMessage(MessageVersion version, ActionHeader actionHeader, BodyWriter body)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(version)));
            }

            if (body == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(body)));
            }

            return new BodyWriterMessage(version, actionHeader, body);
        }

        static public Message CreateMessage(MessageVersion version, string action)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(version)));
            }

            return new BodyWriterMessage(version, action, EmptyBodyWriter.Value);
        }

        static internal Message CreateMessage(MessageVersion version, ActionHeader actionHeader)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(version)));
            }

            return new BodyWriterMessage(version, actionHeader, EmptyBodyWriter.Value);
        }

        static public Message CreateMessage(XmlReader envelopeReader, int maxSizeOfHeaders, MessageVersion version)
        {
            return CreateMessage(XmlDictionaryReader.CreateDictionaryReader(envelopeReader), maxSizeOfHeaders, version);
        }

        static public Message CreateMessage(XmlDictionaryReader envelopeReader, int maxSizeOfHeaders, MessageVersion version)
        {
            if (envelopeReader == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(envelopeReader)));
            }

            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(version)));
            }

            Message message = new StreamedMessage(envelopeReader, maxSizeOfHeaders, version);
            return message;
        }

        static public Message CreateMessage(MessageVersion version, FaultCode faultCode, string reason, string action)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(version)));
            }

            if (faultCode == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(faultCode)));
            }

            if (reason == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(reason)));
            }

            return CreateMessage(version, MessageFault.CreateFault(faultCode, reason), action);
        }

        static public Message CreateMessage(MessageVersion version, FaultCode faultCode, string reason, object detail, string action)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(version)));
            }

            if (faultCode == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(faultCode)));
            }

            if (reason == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(reason)));
            }

            return CreateMessage(version, MessageFault.CreateFault(faultCode, new FaultReason(reason), detail), action);
        }

        static public Message CreateMessage(MessageVersion version, MessageFault fault, string action)
        {
            if (fault == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(fault)));
            }

            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(version)));
            }

            return new BodyWriterMessage(version, action, new FaultBodyWriter(fault, version.Envelope));
        }

        internal Exception CreateMessageDisposedException()
        {
            return new ObjectDisposedException("", SRP.MessageClosed);
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        public T GetBody<T>()
        {
            XmlDictionaryReader reader = GetReaderAtBodyContents();   // This call will change the message state to Read.
            return OnGetBody<T>(reader);
        }

        protected virtual T OnGetBody<T>(XmlDictionaryReader reader)
        {
            return GetBodyCore<T>(reader, DataContractSerializerDefaults.CreateSerializer(typeof(T), int.MaxValue/*maxItems*/));
        }

        public T GetBody<T>(XmlObjectSerializer serializer)
        {
            if (serializer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(serializer)));
            }

            return GetBodyCore<T>(GetReaderAtBodyContents(), serializer);
        }

        private T GetBodyCore<T>(XmlDictionaryReader reader, XmlObjectSerializer serializer)
        {
            T value;
            using (reader)
            {
                value = (T)serializer.ReadObject(reader);
                ReadFromBodyContentsToEnd(reader);
            }
            return value;
        }

        internal virtual XmlDictionaryReader GetReaderAtHeader()
        {
            XmlBuffer buffer = new XmlBuffer(int.MaxValue);
            XmlDictionaryWriter writer = buffer.OpenSection(XmlDictionaryReaderQuotas.Max);
            WriteStartEnvelope(writer);
            MessageHeaders headers = Headers;
            for (int i = 0; i < headers.Count; i++)
            {
                headers.WriteHeader(i, writer);
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            buffer.CloseSection();
            buffer.Close();
            XmlDictionaryReader reader = buffer.GetReader(0);
            reader.ReadStartElement();
            reader.MoveToStartElement();
            return reader;
        }

        public XmlDictionaryReader GetReaderAtBodyContents()
        {
            EnsureReadMessageState();
            if (IsEmpty)
            {
                throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.MessageIsEmpty), this);
            }

            return OnGetReaderAtBodyContents();
        }

        internal void EnsureReadMessageState()
        {
            switch (State)
            {
                case MessageState.Created:
                    State = MessageState.Read;
                    break;
                case MessageState.Copied:
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.MessageHasBeenCopied), this);
                case MessageState.Read:
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.MessageHasBeenRead), this);
                case MessageState.Written:
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.MessageHasBeenWritten), this);
                case MessageState.Closed:
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                default:
                    Fx.Assert(SRP.InvalidMessageState);
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.InvalidMessageState), this);
            }
        }

        internal void InitializeReply(Message request)
        {
            UniqueId requestMessageID = request.Headers.MessageId;
            Headers.RelatesTo = requestMessageID ?? throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.RequestMessageDoesNotHaveAMessageID), request);
        }

        static internal bool IsFaultStartElement(XmlDictionaryReader reader, EnvelopeVersion version)
        {
            return reader.IsStartElement(XD.MessageDictionary.Fault, version.DictionaryNamespace);
        }

        protected virtual void OnBodyToString(XmlDictionaryWriter writer)
        {
            writer.WriteString(SRP.MessageBodyIsUnknown);
        }

        protected virtual MessageBuffer OnCreateBufferedCopy(int maxBufferSize)
        {
            return OnCreateBufferedCopy(maxBufferSize, XmlDictionaryReaderQuotas.Max);
        }

        internal MessageBuffer OnCreateBufferedCopy(int maxBufferSize, XmlDictionaryReaderQuotas quotas)
        {
            XmlBuffer msgBuffer = new XmlBuffer(maxBufferSize);
            XmlDictionaryWriter writer = msgBuffer.OpenSection(quotas);
            OnWriteMessage(writer);
            msgBuffer.CloseSection();
            msgBuffer.Close();
            return new DefaultMessageBuffer(this, msgBuffer);
        }

        protected virtual void OnClose()
        {
        }

        protected virtual XmlDictionaryReader OnGetReaderAtBodyContents()
        {
            XmlBuffer bodyBuffer = new XmlBuffer(int.MaxValue);
            XmlDictionaryWriter writer = bodyBuffer.OpenSection(XmlDictionaryReaderQuotas.Max);
            if (Version.Envelope != EnvelopeVersion.None)
            {
                OnWriteStartEnvelope(writer);
                OnWriteStartBody(writer);
            }
            OnWriteBodyContents(writer);
            if (Version.Envelope != EnvelopeVersion.None)
            {
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            bodyBuffer.CloseSection();
            bodyBuffer.Close();
            XmlDictionaryReader reader = bodyBuffer.GetReader(0);
            if (Version.Envelope != EnvelopeVersion.None)
            {
                reader.ReadStartElement();
                reader.ReadStartElement();
            }
            reader.MoveToContent();
            return reader;
        }

        protected virtual void OnWriteStartBody(XmlDictionaryWriter writer)
        {
            MessageDictionary messageDictionary = XD.MessageDictionary;
            writer.WriteStartElement(messageDictionary.Prefix.Value, messageDictionary.Body, Version.Envelope.DictionaryNamespace);
        }

        public void WriteBodyContents(XmlDictionaryWriter writer)
        {
            EnsureWriteMessageState(writer);
            OnWriteBodyContents(writer);
        }

        public Task WriteBodyContentsAsync(XmlDictionaryWriter writer)
        {
            WriteBodyContents(writer);
            return TaskHelpers.CompletedTask();
        }

        public IAsyncResult BeginWriteBodyContents(XmlDictionaryWriter writer, AsyncCallback callback, object state)
        {
            EnsureWriteMessageState(writer);
            return OnBeginWriteBodyContents(writer, callback, state);
        }

        public void EndWriteBodyContents(IAsyncResult result)
        {
            OnEndWriteBodyContents(result);
        }

        protected abstract void OnWriteBodyContents(XmlDictionaryWriter writer);

        protected virtual Task OnWriteBodyContentsAsync(XmlDictionaryWriter writer)
        {
            OnWriteBodyContents(writer);
            return TaskHelpers.CompletedTask();
        }

        protected virtual IAsyncResult OnBeginWriteBodyContents(XmlDictionaryWriter writer, AsyncCallback callback, object state)
        {
            return OnWriteBodyContentsAsync(writer).ToApm(callback, state);
        }

        protected virtual void OnEndWriteBodyContents(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        public void WriteStartEnvelope(XmlDictionaryWriter writer)
        {
            if (writer == null)
            {
                throw TraceUtility.ThrowHelperError(new ArgumentNullException(nameof(writer)), this);
            }

            OnWriteStartEnvelope(writer);
        }

        protected virtual void OnWriteStartEnvelope(XmlDictionaryWriter writer)
        {
            EnvelopeVersion envelopeVersion = Version.Envelope;
            if (envelopeVersion != EnvelopeVersion.None)
            {
                MessageDictionary messageDictionary = XD.MessageDictionary;
                writer.WriteStartElement(messageDictionary.Prefix.Value, messageDictionary.Envelope, envelopeVersion.DictionaryNamespace);
                WriteSharedHeaderPrefixes(writer);
            }
        }

        protected virtual void OnWriteStartHeaders(XmlDictionaryWriter writer)
        {
            EnvelopeVersion envelopeVersion = Version.Envelope;
            if (envelopeVersion != EnvelopeVersion.None)
            {
                MessageDictionary messageDictionary = XD.MessageDictionary;
                writer.WriteStartElement(messageDictionary.Prefix.Value, messageDictionary.Header, envelopeVersion.DictionaryNamespace);
            }
        }

        public override string ToString()
        {
            if (IsDisposed)
            {
                return base.ToString();
            }


            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                using (XmlWriter textWriter = XmlWriter.Create(stringWriter, settings))
                {
                    using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateDictionaryWriter(textWriter))
                    {
                        try
                        {
                            ToString(writer);
                            writer.Flush();
                            return stringWriter.ToString();
                        }
                        catch (XmlException e)
                        {
                            return SRP.Format(SRP.MessageBodyToStringError, e.GetType().ToString(), e.Message);
                        }
                    }
                }
            }
        }

        internal void ToString(XmlDictionaryWriter writer)
        {
            if (IsDisposed)
            {
                throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
            }

            if (Version.Envelope != EnvelopeVersion.None)
            {
                WriteStartEnvelope(writer);
                WriteStartHeaders(writer);
                MessageHeaders headers = Headers;
                for (int i = 0; i < headers.Count; i++)
                {
                    headers.WriteHeader(i, writer);
                }

                writer.WriteEndElement();
                MessageDictionary messageDictionary = XD.MessageDictionary;
                WriteStartBody(writer);
            }

            BodyToString(writer);

            if (Version.Envelope != EnvelopeVersion.None)
            {
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        public string GetBodyAttribute(string localName, string ns)
        {
            if (localName == null)
            {
                throw TraceUtility.ThrowHelperError(new ArgumentNullException(nameof(localName)), this);
            }

            if (ns == null)
            {
                throw TraceUtility.ThrowHelperError(new ArgumentNullException(nameof(ns)), this);
            }

            switch (State)
            {
                case MessageState.Created:
                    break;
                case MessageState.Copied:
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.MessageHasBeenCopied), this);
                case MessageState.Read:
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.MessageHasBeenRead), this);
                case MessageState.Written:
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.MessageHasBeenWritten), this);
                case MessageState.Closed:
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                default:
                    Fx.Assert(SRP.InvalidMessageState);
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.InvalidMessageState), this);
            }
            return OnGetBodyAttribute(localName, ns);
        }

        protected virtual string OnGetBodyAttribute(string localName, string ns)
        {
            return null;
        }

        internal void ReadFromBodyContentsToEnd(XmlDictionaryReader reader)
        {
            Message.ReadFromBodyContentsToEnd(reader, Version.Envelope);
        }

        private static void ReadFromBodyContentsToEnd(XmlDictionaryReader reader, EnvelopeVersion envelopeVersion)
        {
            if (envelopeVersion != EnvelopeVersion.None)
            {
                reader.ReadEndElement(); // </Body>
                reader.ReadEndElement(); // </Envelope>
            }
            reader.MoveToContent();
        }

        internal static bool ReadStartBody(XmlDictionaryReader reader, EnvelopeVersion envelopeVersion, out bool isFault, out bool isEmpty)
        {
            if (reader.IsEmptyElement)
            {
                reader.Read();
                isEmpty = true;
                isFault = false;
                reader.ReadEndElement();
                return false;
            }
            else
            {
                reader.Read();
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.MoveToContent();
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    isFault = IsFaultStartElement(reader, envelopeVersion);
                    isEmpty = false;
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    isEmpty = true;
                    isFault = false;
                    Message.ReadFromBodyContentsToEnd(reader, envelopeVersion);
                    return false;
                }
                else
                {
                    isEmpty = false;
                    isFault = false;
                }

                return true;
            }
        }

        public void WriteBody(XmlWriter writer)
        {
            WriteBody(XmlDictionaryWriter.CreateDictionaryWriter(writer));
        }

        public void WriteBody(XmlDictionaryWriter writer)
        {
            WriteStartBody(writer);
            WriteBodyContents(writer);
            writer.WriteEndElement();
        }

        public void WriteStartBody(XmlWriter writer)
        {
            WriteStartBody(XmlDictionaryWriter.CreateDictionaryWriter(writer));
        }

        public void WriteStartBody(XmlDictionaryWriter writer)
        {
            if (writer == null)
            {
                throw TraceUtility.ThrowHelperError(new ArgumentNullException(nameof(writer)), this);
            }

            OnWriteStartBody(writer);
        }

        internal void WriteStartHeaders(XmlDictionaryWriter writer)
        {
            OnWriteStartHeaders(writer);
        }

        public void WriteMessage(XmlWriter writer)
        {
            WriteMessage(XmlDictionaryWriter.CreateDictionaryWriter(writer));
        }

        public void WriteMessage(XmlDictionaryWriter writer)
        {
            EnsureWriteMessageState(writer);
            OnWriteMessage(writer);
        }

        public virtual Task WriteMessageAsync(XmlWriter writer)
        {
            WriteMessage(writer);
            return TaskHelpers.CompletedTask();
        }

        public virtual async Task WriteMessageAsync(XmlDictionaryWriter writer)
        {
            EnsureWriteMessageState(writer);
            await OnWriteMessageAsync(writer);
        }

        public virtual async Task OnWriteMessageAsync(XmlDictionaryWriter writer)
        {
            WriteMessagePreamble(writer);

            // We should call OnWriteBodyContentsAsync instead of WriteBodyContentsAsync here,
            // otherwise EnsureWriteMessageState would get called twice. Also see OnWriteMessage()
            // for the example.
            await OnWriteBodyContentsAsync(writer);
            await WriteMessagePostambleAsync(writer);
        }

        private void EnsureWriteMessageState(XmlDictionaryWriter writer)
        {
            if (writer == null)
            {
                throw TraceUtility.ThrowHelperError(new ArgumentNullException(nameof(writer)), this);
            }

            switch (State)
            {
                case MessageState.Created:
                    State = MessageState.Written;
                    break;
                case MessageState.Copied:
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.MessageHasBeenCopied), this);
                case MessageState.Read:
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.MessageHasBeenRead), this);
                case MessageState.Written:
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.MessageHasBeenWritten), this);
                case MessageState.Closed:
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                default:
                    Fx.Assert(SRP.InvalidMessageState);
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.InvalidMessageState), this);
            }
        }

        //  WriteMessageAsync
        public IAsyncResult BeginWriteMessage(XmlDictionaryWriter writer, AsyncCallback callback, object state)
        {
            EnsureWriteMessageState(writer);
            return OnBeginWriteMessage(writer, callback, state);
        }

        public void EndWriteMessage(IAsyncResult result)
        {
            OnEndWriteMessage(result);
        }

        //  OnWriteMessageAsync
        protected virtual void OnWriteMessage(XmlDictionaryWriter writer)
        {
            WriteMessagePreamble(writer);
            OnWriteBodyContents(writer);
            WriteMessagePostamble(writer);
        }

        internal void WriteMessagePreamble(XmlDictionaryWriter writer)
        {
            if (Version.Envelope != EnvelopeVersion.None)
            {
                OnWriteStartEnvelope(writer);

                MessageHeaders headers = Headers;
                int headersCount = headers.Count;
                if (headersCount > 0)
                {
                    OnWriteStartHeaders(writer);
                    for (int i = 0; i < headersCount; i++)
                    {
                        headers.WriteHeader(i, writer);
                    }
                    writer.WriteEndElement();
                }

                OnWriteStartBody(writer);
            }
        }

        internal void WriteMessagePostamble(XmlDictionaryWriter writer)
        {
            if (Version.Envelope != EnvelopeVersion.None)
            {
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        internal async Task WriteMessagePostambleAsync(XmlDictionaryWriter writer)
        {
            if (Version.Envelope != EnvelopeVersion.None)
            {
                await writer.WriteEndElementAsync();
                await writer.WriteEndElementAsync();
            }
        }

        protected virtual IAsyncResult OnBeginWriteMessage(XmlDictionaryWriter writer, AsyncCallback callback, object state)
        {
            return new OnWriteMessageAsyncResult(writer, this, callback, state);
        }

        protected virtual void OnEndWriteMessage(IAsyncResult result)
        {
            OnWriteMessageAsyncResult.End(result);
        }

        private void WriteSharedHeaderPrefixes(XmlDictionaryWriter writer)
        {
            MessageHeaders headers = Headers;
            int count = headers.Count;
            int prefixesWritten = 0;
            for (int i = 0; i < count; i++)
            {
                if (Version.Addressing == AddressingVersion.None && headers[i].Namespace == AddressingVersion.None.Namespace)
                {
                    continue;
                }

                IMessageHeaderWithSharedNamespace headerWithSharedNamespace = headers[i] as IMessageHeaderWithSharedNamespace;
                if (headerWithSharedNamespace != null)
                {
                    XmlDictionaryString prefix = headerWithSharedNamespace.SharedPrefix;
                    string prefixString = prefix.Value;
                    if (!((prefixString.Length == 1)))
                    {
                        Fx.Assert("Message.WriteSharedHeaderPrefixes: (prefixString.Length == 1) -- IMessageHeaderWithSharedNamespace must use a single lowercase letter prefix.");
                        throw TraceUtility.ThrowHelperError(new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "IMessageHeaderWithSharedNamespace must use a single lowercase letter prefix.")), this);
                    }

                    int prefixIndex = prefixString[0] - 'a';
                    if (!((prefixIndex >= 0 && prefixIndex < 26)))
                    {
                        Fx.Assert("Message.WriteSharedHeaderPrefixes: (prefixIndex >= 0 && prefixIndex < 26) -- IMessageHeaderWithSharedNamespace must use a single lowercase letter prefix.");
                        throw TraceUtility.ThrowHelperError(new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "IMessageHeaderWithSharedNamespace must use a single lowercase letter prefix.")), this);
                    }
                    int prefixBit = 1 << prefixIndex;
                    if ((prefixesWritten & prefixBit) == 0)
                    {
                        writer.WriteXmlnsAttribute(prefixString, headerWithSharedNamespace.SharedNamespace);
                        prefixesWritten |= prefixBit;
                    }
                }
            }
        }


        private class OnWriteMessageAsyncResult : ScheduleActionItemAsyncResult
        {
            private Message _message;
            private XmlDictionaryWriter _writer;

            public OnWriteMessageAsyncResult(XmlDictionaryWriter writer, Message message, AsyncCallback callback, object state)
                : base(callback, state)
            {
                Fx.Assert(message != null, "message should never be null");

                _message = message;
                _writer = writer;

                Schedule();
            }

            protected override void OnDoWork()
            {
                _message.OnWriteMessage(_writer);
            }
        }
    }

    internal class EmptyBodyWriter : BodyWriter
    {
        private static EmptyBodyWriter s_value;

        private EmptyBodyWriter()
            : base(true)
        {
        }

        public static EmptyBodyWriter Value
        {
            get
            {
                if (s_value == null)
                {
                    s_value = new EmptyBodyWriter();
                }

                return s_value;
            }
        }

        internal override bool IsEmpty
        {
            get { return true; }
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
        }
    }

    internal class FaultBodyWriter : BodyWriter
    {
        private MessageFault _fault;
        private EnvelopeVersion _version;

        public FaultBodyWriter(MessageFault fault, EnvelopeVersion version)
            : base(true)
        {
            _fault = fault;
            _version = version;
        }

        internal override bool IsFault
        {
            get { return true; }
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            _fault.WriteTo(writer, _version);
        }
    }

    internal class XmlObjectSerializerBodyWriter : BodyWriter
    {
        private object _body;
        private XmlObjectSerializer _serializer;

        public XmlObjectSerializerBodyWriter(object body, XmlObjectSerializer serializer)
            : base(true)
        {
            _body = body;
            _serializer = serializer;
        }

        private object ThisLock
        {
            get { return this; }
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            lock (ThisLock)
            {
                _serializer.WriteObject(writer, _body);
            }
        }
    }

    internal class XmlReaderBodyWriter : BodyWriter
    {
        private XmlDictionaryReader _reader;
        private bool _isFault;

        public XmlReaderBodyWriter(XmlDictionaryReader reader, EnvelopeVersion version)
            : base(false)
        {
            _reader = reader;
            if (reader.MoveToContent() != XmlNodeType.Element)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.InvalidReaderPositionOnCreateMessage, "reader"));
            }

            _isFault = Message.IsFaultStartElement(reader, version);
        }

        internal override bool IsFault
        {
            get
            {
                return _isFault;
            }
        }

        protected override BodyWriter OnCreateBufferedCopy(int maxBufferSize)
        {
            return OnCreateBufferedCopy(maxBufferSize, _reader.Quotas);
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            using (_reader)
            {
                XmlNodeType type = _reader.MoveToContent();
                while (!_reader.EOF && type != XmlNodeType.EndElement)
                {
                    if (type != XmlNodeType.Element)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.InvalidReaderPositionOnCreateMessage, "reader"));
                    }

                    writer.WriteNode(_reader, false);

                    type = _reader.MoveToContent();
                }
            }
        }
    }

    internal class BodyWriterMessage : Message
    {
        private MessageProperties _properties;
        private MessageHeaders _headers;

        private BodyWriterMessage(BodyWriter bodyWriter)
        {
            BodyWriter = bodyWriter;
        }

        public BodyWriterMessage(MessageVersion version, string action, BodyWriter bodyWriter)
            : this(bodyWriter)
        {
            _headers = new MessageHeaders(version);
            _headers.Action = action;
        }

        public BodyWriterMessage(MessageVersion version, ActionHeader actionHeader, BodyWriter bodyWriter)
            : this(bodyWriter)
        {
            _headers = new MessageHeaders(version);
            _headers.SetActionHeader(actionHeader);
        }

        public BodyWriterMessage(MessageHeaders headers, KeyValuePair<string, object>[] properties, BodyWriter bodyWriter)
            : this(bodyWriter)
        {
            _headers = new MessageHeaders(headers);
            _properties = new MessageProperties(properties);
        }

        public override bool IsFault
        {
            get
            {
                if (IsDisposed)
                {
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                }

                return BodyWriter.IsFault;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                if (IsDisposed)
                {
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                }

                return BodyWriter.IsEmpty;
            }
        }

        public override MessageHeaders Headers
        {
            get
            {
                if (IsDisposed)
                {
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                }

                return _headers;
            }
        }

        public override MessageProperties Properties
        {
            get
            {
                if (IsDisposed)
                {
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                }

                if (_properties == null)
                {
                    _properties = new MessageProperties();
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
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                }

                return _headers.MessageVersion;
            }
        }

        protected override MessageBuffer OnCreateBufferedCopy(int maxBufferSize)
        {
            BodyWriter bufferedBodyWriter;
            if (BodyWriter.IsBuffered)
            {
                bufferedBodyWriter = BodyWriter;
            }
            else
            {
                bufferedBodyWriter = BodyWriter.CreateBufferedCopy(maxBufferSize);
            }
            KeyValuePair<string, object>[] properties = new KeyValuePair<string, object>[Properties.Count];
            ((ICollection<KeyValuePair<string, object>>)Properties).CopyTo(properties, 0);
            return new BodyWriterMessageBuffer(_headers, properties, bufferedBodyWriter);
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

            if (ex != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ex);
            }

            BodyWriter = null;
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            BodyWriter.WriteBodyContents(writer);
        }

        protected override Task OnWriteBodyContentsAsync(XmlDictionaryWriter writer)
        {
            return BodyWriter.WriteBodyContentsAsync(writer);
        }

        protected override IAsyncResult OnBeginWriteMessage(XmlDictionaryWriter writer, AsyncCallback callback, object state)
        {
            return null;
            //WriteMessagePreamble(writer);
            //return new OnWriteMessageAsyncResult(writer, this, callback, state);
        }

        protected override void OnEndWriteMessage(IAsyncResult result)
        {
            //OnWriteMessageAsyncResult.End(result);
        }

        protected override IAsyncResult OnBeginWriteBodyContents(XmlDictionaryWriter writer, AsyncCallback callback, object state)
        {
            return BodyWriter.BeginWriteBodyContents(writer, callback, state);
        }

        protected override void OnEndWriteBodyContents(IAsyncResult result)
        {
            BodyWriter.EndWriteBodyContents(result);
        }

        protected override void OnBodyToString(XmlDictionaryWriter writer)
        {
            if (BodyWriter.IsBuffered)
            {
                BodyWriter.WriteBodyContents(writer);
            }
            else
            {
                writer.WriteString(SRP.MessageBodyIsStream);
            }
        }

        protected internal BodyWriter BodyWriter { get; private set; }
    }

    internal abstract class ReceivedMessage : Message
    {
        private bool _isFault;
        private bool _isEmpty;

        public override bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public override bool IsFault
        {
            get { return _isFault; }
        }

        protected static bool HasHeaderElement(XmlDictionaryReader reader, EnvelopeVersion envelopeVersion)
        {
            return reader.IsStartElement(XD.MessageDictionary.Header, envelopeVersion.DictionaryNamespace);
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            if (!_isEmpty)
            {
                using (XmlDictionaryReader bodyReader = OnGetReaderAtBodyContents())
                {
                    if (bodyReader.ReadState == ReadState.Error || bodyReader.ReadState == ReadState.Closed)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.MessageBodyReaderInvalidReadState, bodyReader.ReadState.ToString())));
                    }

                    while (bodyReader.NodeType != XmlNodeType.EndElement && !bodyReader.EOF)
                    {
                        writer.WriteNode(bodyReader, false);
                    }

                    ReadFromBodyContentsToEnd(bodyReader);
                }
            }
        }

        protected bool ReadStartBody(XmlDictionaryReader reader)
        {
            return Message.ReadStartBody(reader, Version.Envelope, out _isFault, out _isEmpty);
        }

        protected static EnvelopeVersion ReadStartEnvelope(XmlDictionaryReader reader)
        {
            EnvelopeVersion envelopeVersion;

            if (reader.IsStartElement(XD.MessageDictionary.Envelope, XD.Message12Dictionary.Namespace))
            {
                envelopeVersion = EnvelopeVersion.Soap12;
            }
            else if (reader.IsStartElement(XD.MessageDictionary.Envelope, XD.Message11Dictionary.Namespace))
            {
                envelopeVersion = EnvelopeVersion.Soap11;
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(SRP.MessageVersionUnknown));
            }

            if (reader.IsEmptyElement)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(SRP.MessageBodyMissing));
            }

            reader.Read();
            return envelopeVersion;
        }

        protected static void VerifyStartBody(XmlDictionaryReader reader, EnvelopeVersion version)
        {
            if (!reader.IsStartElement(XD.MessageDictionary.Body, version.DictionaryNamespace))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(SRP.MessageBodyMissing));
            }
        }
    }

    internal sealed class StreamedMessage : ReceivedMessage
    {
        private MessageHeaders _headers;
        private XmlAttributeHolder[] _envelopeAttributes;
        private XmlAttributeHolder[] _headerAttributes;
        private XmlAttributeHolder[] _bodyAttributes;
        private string _envelopePrefix;
        private string _headerPrefix;
        private string _bodyPrefix;
        private MessageProperties _properties;
        private XmlDictionaryReader _reader;
        private XmlDictionaryReaderQuotas _quotas;

        public StreamedMessage(XmlDictionaryReader reader, int maxSizeOfHeaders, MessageVersion desiredVersion)
        {
            _properties = new MessageProperties();
            if (reader.NodeType != XmlNodeType.Element)
            {
                reader.MoveToContent();
            }

            if (desiredVersion.Envelope == EnvelopeVersion.None)
            {
                _reader = reader;
                _headerAttributes = XmlAttributeHolder.emptyArray;
                _headers = new MessageHeaders(desiredVersion);
            }
            else
            {
                _envelopeAttributes = XmlAttributeHolder.ReadAttributes(reader, ref maxSizeOfHeaders);
                _envelopePrefix = reader.Prefix;
                EnvelopeVersion envelopeVersion = ReadStartEnvelope(reader);
                if (desiredVersion.Envelope != envelopeVersion)
                {
                    Exception versionMismatchException = new ArgumentException(SRP.Format(SRP.EncoderEnvelopeVersionMismatch, envelopeVersion, desiredVersion.Envelope), "reader");
                    throw TraceUtility.ThrowHelperError(
                        new CommunicationException(versionMismatchException.Message, versionMismatchException),
                        this);
                }

                if (HasHeaderElement(reader, envelopeVersion))
                {
                    _headerPrefix = reader.Prefix;
                    _headerAttributes = XmlAttributeHolder.ReadAttributes(reader, ref maxSizeOfHeaders);
                    _headers = new MessageHeaders(desiredVersion, reader, _envelopeAttributes, _headerAttributes, ref maxSizeOfHeaders);
                }
                else
                {
                    _headerAttributes = XmlAttributeHolder.emptyArray;
                    _headers = new MessageHeaders(desiredVersion);
                }

                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.MoveToContent();
                }

                _bodyPrefix = reader.Prefix;
                VerifyStartBody(reader, envelopeVersion);
                _bodyAttributes = XmlAttributeHolder.ReadAttributes(reader, ref maxSizeOfHeaders);
                if (ReadStartBody(reader))
                {
                    _reader = reader;
                }
                else
                {
                    _quotas = new XmlDictionaryReaderQuotas();
                    reader.Quotas.CopyTo(_quotas);
                    reader.Dispose();
                }
            }
        }

        public override MessageHeaders Headers
        {
            get
            {
                if (IsDisposed)
                {
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                }

                return _headers;
            }
        }

        public override MessageVersion Version
        {
            get
            {
                return _headers.MessageVersion;
            }
        }

        public override MessageProperties Properties
        {
            get
            {
                return _properties;
            }
        }

        protected override void OnBodyToString(XmlDictionaryWriter writer)
        {
            writer.WriteString(SRP.MessageBodyIsStream);
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
                _properties.Dispose();
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
                    _reader.Dispose();
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ex);
            }
        }

        protected override XmlDictionaryReader OnGetReaderAtBodyContents()
        {
            XmlDictionaryReader reader = _reader;
            _reader = null;
            return reader;
        }

        protected override MessageBuffer OnCreateBufferedCopy(int maxBufferSize)
        {
            if (_reader != null)
            {
                return OnCreateBufferedCopy(maxBufferSize, _reader.Quotas);
            }

            return OnCreateBufferedCopy(maxBufferSize, _quotas);
        }

        protected override void OnWriteStartBody(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement(_bodyPrefix, MessageStrings.Body, Version.Envelope.Namespace);
            XmlAttributeHolder.WriteAttributes(_bodyAttributes, writer);
        }

        protected override void OnWriteStartEnvelope(XmlDictionaryWriter writer)
        {
            EnvelopeVersion envelopeVersion = Version.Envelope;
            writer.WriteStartElement(_envelopePrefix, MessageStrings.Envelope, envelopeVersion.Namespace);
            XmlAttributeHolder.WriteAttributes(_envelopeAttributes, writer);
        }

        protected override void OnWriteStartHeaders(XmlDictionaryWriter writer)
        {
            EnvelopeVersion envelopeVersion = Version.Envelope;
            writer.WriteStartElement(_headerPrefix, MessageStrings.Header, envelopeVersion.Namespace);
            XmlAttributeHolder.WriteAttributes(_headerAttributes, writer);
        }

        protected override string OnGetBodyAttribute(string localName, string ns)
        {
            return XmlAttributeHolder.GetAttribute(_bodyAttributes, localName, ns);
        }
    }

    internal interface IBufferedMessageData
    {
        MessageEncoder MessageEncoder { get; }
        ArraySegment<byte> Buffer { get; }
        XmlDictionaryReaderQuotas Quotas { get; }
        void Close();
        void EnableMultipleUsers();
        XmlDictionaryReader GetMessageReader();
        void Open();
        void ReturnMessageState(RecycledMessageState messageState);
        RecycledMessageState TakeMessageState();
    }

    internal sealed class BufferedMessage : ReceivedMessage
    {
        private MessageHeaders _headers;
        private MessageProperties _properties;
        private RecycledMessageState _recycledMessageState;
        private XmlDictionaryReader _reader;
        private XmlAttributeHolder[] _bodyAttributes;

        public BufferedMessage(IBufferedMessageData messageData, RecycledMessageState recycledMessageState)
            : this(messageData, recycledMessageState, null, false)
        {
        }

        public BufferedMessage(IBufferedMessageData messageData, RecycledMessageState recycledMessageState, bool[] understoodHeaders, bool understoodHeadersModified)
        {
            bool throwing = true;
            try
            {
                _recycledMessageState = recycledMessageState;
                MessageData = messageData;
                _properties = recycledMessageState.TakeProperties();
                if (_properties == null)
                {
                    _properties = new MessageProperties();
                }

                XmlDictionaryReader reader = messageData.GetMessageReader();
                MessageVersion desiredVersion = messageData.MessageEncoder.MessageVersion;

                if (desiredVersion.Envelope == EnvelopeVersion.None)
                {
                    _reader = reader;
                    _headers = new MessageHeaders(desiredVersion);
                }
                else
                {
                    EnvelopeVersion envelopeVersion = ReadStartEnvelope(reader);
                    if (desiredVersion.Envelope != envelopeVersion)
                    {
                        Exception versionMismatchException = new ArgumentException(SRP.Format(SRP.EncoderEnvelopeVersionMismatch, envelopeVersion, desiredVersion.Envelope), "reader");
                        throw TraceUtility.ThrowHelperError(
                            new CommunicationException(versionMismatchException.Message, versionMismatchException),
                            this);
                    }

                    if (HasHeaderElement(reader, envelopeVersion))
                    {
                        _headers = recycledMessageState.TakeHeaders();
                        if (_headers == null)
                        {
                            _headers = new MessageHeaders(desiredVersion, reader, messageData, recycledMessageState, understoodHeaders, understoodHeadersModified);
                        }
                        else
                        {
                            _headers.Init(desiredVersion, reader, messageData, recycledMessageState, understoodHeaders, understoodHeadersModified);
                        }
                    }
                    else
                    {
                        _headers = new MessageHeaders(desiredVersion);
                    }

                    VerifyStartBody(reader, envelopeVersion);

                    int maxSizeOfAttributes = int.MaxValue;
                    _bodyAttributes = XmlAttributeHolder.ReadAttributes(reader, ref maxSizeOfAttributes);
                    if (maxSizeOfAttributes < int.MaxValue - 4096)
                    {
                        _bodyAttributes = null;
                    }

                    if (ReadStartBody(reader))
                    {
                        _reader = reader;
                    }
                    else
                    {
                        reader.Dispose();
                    }
                }
                throwing = false;
            }
            finally
            {
                if (throwing && MessageLogger.LoggingEnabled)
                {
                    MessageLogger.LogMessage(messageData.Buffer, MessageLoggingSource.Malformed);
                }
            }
        }

        public override MessageHeaders Headers
        {
            get
            {
                if (IsDisposed)
                {
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                }

                return _headers;
            }
        }

        internal IBufferedMessageData MessageData { get; private set; }

        public override MessageProperties Properties
        {
            get
            {
                if (IsDisposed)
                {
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                }

                return _properties;
            }
        }

        internal override RecycledMessageState RecycledMessageState
        {
            get { return _recycledMessageState; }
        }

        public override MessageVersion Version
        {
            get
            {
                return _headers.MessageVersion;
            }
        }

        protected override XmlDictionaryReader OnGetReaderAtBodyContents()
        {
            XmlDictionaryReader reader = _reader;
            _reader = null;
            return reader;
        }

        internal override XmlDictionaryReader GetReaderAtHeader()
        {
            if (!_headers.ContainsOnlyBufferedMessageHeaders)
            {
                return base.GetReaderAtHeader();
            }

            XmlDictionaryReader reader = MessageData.GetMessageReader();
            if (reader.NodeType != XmlNodeType.Element)
            {
                reader.MoveToContent();
            }

            reader.Read();
            if (HasHeaderElement(reader, _headers.MessageVersion.Envelope))
            {
                return reader;
            }

            return base.GetReaderAtHeader();
        }

        public XmlDictionaryReader GetBufferedReaderAtBody()
        {
            XmlDictionaryReader reader = MessageData.GetMessageReader();
            if (reader.NodeType != XmlNodeType.Element)
            {
                reader.MoveToContent();
            }

            if (Version.Envelope != EnvelopeVersion.None)
            {
                reader.Read();
                if (HasHeaderElement(reader, _headers.MessageVersion.Envelope))
                {
                    reader.Skip();
                }

                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.MoveToContent();
                }
            }
            return reader;
        }

        public XmlDictionaryReader GetMessageReader()
        {
            return MessageData.GetMessageReader();
        }

        protected override void OnBodyToString(XmlDictionaryWriter writer)
        {
            using (XmlDictionaryReader reader = GetBufferedReaderAtBody())
            {
                if (Version == MessageVersion.None)
                {
                    writer.WriteNode(reader, false);
                }
                else
                {
                    if (!reader.IsEmptyElement)
                    {
                        reader.ReadStartElement();
                        while (reader.NodeType != XmlNodeType.EndElement)
                        {
                            writer.WriteNode(reader, false);
                        }
                    }
                }
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
                _properties.Dispose();
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
                    _reader.Dispose();
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
                _recycledMessageState.ReturnHeaders(_headers);
                _recycledMessageState.ReturnProperties(_properties);
                MessageData.ReturnMessageState(_recycledMessageState);
                _recycledMessageState = null;
                MessageData.Close();
                MessageData = null;
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ex);
            }
        }

        protected override void OnWriteStartEnvelope(XmlDictionaryWriter writer)
        {
            using (XmlDictionaryReader reader = GetMessageReader())
            {
                reader.MoveToContent();
                EnvelopeVersion envelopeVersion = Version.Envelope;
                writer.WriteStartElement(reader.Prefix, MessageStrings.Envelope, envelopeVersion.Namespace);
                writer.WriteAttributes(reader, false);
            }
        }

        protected override void OnWriteStartHeaders(XmlDictionaryWriter writer)
        {
            using (XmlDictionaryReader reader = GetMessageReader())
            {
                reader.MoveToContent();
                EnvelopeVersion envelopeVersion = Version.Envelope;
                reader.Read();
                if (HasHeaderElement(reader, envelopeVersion))
                {
                    writer.WriteStartElement(reader.Prefix, MessageStrings.Header, envelopeVersion.Namespace);
                    writer.WriteAttributes(reader, false);
                }
                else
                {
                    writer.WriteStartElement(MessageStrings.Prefix, MessageStrings.Header, envelopeVersion.Namespace);
                }
            }
        }

        protected override void OnWriteStartBody(XmlDictionaryWriter writer)
        {
            using (XmlDictionaryReader reader = GetBufferedReaderAtBody())
            {
                writer.WriteStartElement(reader.Prefix, MessageStrings.Body, Version.Envelope.Namespace);
                writer.WriteAttributes(reader, false);
            }
        }

        protected override MessageBuffer OnCreateBufferedCopy(int maxBufferSize)
        {
            if (_headers.ContainsOnlyBufferedMessageHeaders)
            {
                KeyValuePair<string, object>[] properties = new KeyValuePair<string, object>[Properties.Count];
                ((ICollection<KeyValuePair<string, object>>)Properties).CopyTo(properties, 0);
                MessageData.EnableMultipleUsers();
                bool[] understoodHeaders = null;
                if (_headers.HasMustUnderstandBeenModified)
                {
                    understoodHeaders = new bool[_headers.Count];
                    for (int i = 0; i < _headers.Count; i++)
                    {
                        understoodHeaders[i] = _headers.IsUnderstood(i);
                    }
                }
                return new BufferedMessageBuffer(MessageData, properties, understoodHeaders, _headers.HasMustUnderstandBeenModified);
            }
            else
            {
                if (_reader != null)
                {
                    return OnCreateBufferedCopy(maxBufferSize, _reader.Quotas);
                }

                return OnCreateBufferedCopy(maxBufferSize, XmlDictionaryReaderQuotas.Max);
            }
        }

        protected override string OnGetBodyAttribute(string localName, string ns)
        {
            if (_bodyAttributes != null)
            {
                return XmlAttributeHolder.GetAttribute(_bodyAttributes, localName, ns);
            }

            using (XmlDictionaryReader reader = GetBufferedReaderAtBody())
            {
                return reader.GetAttribute(localName, ns);
            }
        }
    }

    internal struct XmlAttributeHolder
    {
        private string _value;

        public static XmlAttributeHolder[] emptyArray = new XmlAttributeHolder[0];

        public XmlAttributeHolder(string prefix, string localName, string ns, string value)
        {
            Prefix = prefix;
            LocalName = localName;
            NamespaceUri = ns;
            _value = value;
        }

        public string Prefix { get; }

        public string NamespaceUri { get; }

        public string LocalName { get; }

        public string Value
        {
            get { return _value; }
        }

        public void WriteTo(XmlWriter writer)
        {
            writer.WriteStartAttribute(Prefix, LocalName, NamespaceUri);
            writer.WriteString(_value);
            writer.WriteEndAttribute();
        }

        public static void WriteAttributes(XmlAttributeHolder[] attributes, XmlWriter writer)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                attributes[i].WriteTo(writer);
            }
        }

        public static XmlAttributeHolder[] ReadAttributes(XmlDictionaryReader reader)
        {
            int maxSizeOfHeaders = int.MaxValue;
            return ReadAttributes(reader, ref maxSizeOfHeaders);
        }

        public static XmlAttributeHolder[] ReadAttributes(XmlDictionaryReader reader, ref int maxSizeOfHeaders)
        {
            if (reader.AttributeCount == 0)
            {
                return emptyArray;
            }

            XmlAttributeHolder[] attributes = new XmlAttributeHolder[reader.AttributeCount];
            reader.MoveToFirstAttribute();
            for (int i = 0; i < attributes.Length; i++)
            {
                string ns = reader.NamespaceURI;
                string localName = reader.LocalName;
                string prefix = reader.Prefix;
                string value = string.Empty;
                while (reader.ReadAttributeValue())
                {
                    if (value.Length == 0)
                    {
                        value = reader.Value;
                    }
                    else
                    {
                        value += reader.Value;
                    }
                }
                Deduct(prefix, ref maxSizeOfHeaders);
                Deduct(localName, ref maxSizeOfHeaders);
                Deduct(ns, ref maxSizeOfHeaders);
                Deduct(value, ref maxSizeOfHeaders);
                attributes[i] = new XmlAttributeHolder(prefix, localName, ns, value);
                reader.MoveToNextAttribute();
            }
            reader.MoveToElement();
            return attributes;
        }

        private static void Deduct(string s, ref int maxSizeOfHeaders)
        {
            int byteCount = s.Length * sizeof(char);
            if (byteCount > maxSizeOfHeaders)
            {
                string message = SRP.XmlBufferQuotaExceeded;
                Exception inner = new QuotaExceededException(message);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(message, inner));
            }
            maxSizeOfHeaders -= byteCount;
        }

        public static string GetAttribute(XmlAttributeHolder[] attributes, string localName, string ns)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i].LocalName == localName && attributes[i].NamespaceUri == ns)
                {
                    return attributes[i].Value;
                }
            }

            return null;
        }
    }

    internal class RecycledMessageState
    {
        private MessageHeaders _recycledHeaders;
        private MessageProperties _recycledProperties;
        private UriCache _uriCache;
        private HeaderInfoCache _headerInfoCache;

        public HeaderInfoCache HeaderInfoCache
        {
            get
            {
                if (_headerInfoCache == null)
                {
                    _headerInfoCache = new HeaderInfoCache();
                }
                return _headerInfoCache;
            }
        }

        public UriCache UriCache
        {
            get
            {
                if (_uriCache == null)
                {
                    _uriCache = new UriCache();
                }

                return _uriCache;
            }
        }

        public MessageProperties TakeProperties()
        {
            MessageProperties taken = _recycledProperties;
            _recycledProperties = null;
            return taken;
        }

        public void ReturnProperties(MessageProperties properties)
        {
            if (properties.CanRecycle)
            {
                properties.Recycle();
                _recycledProperties = properties;
            }
        }

        public MessageHeaders TakeHeaders()
        {
            MessageHeaders taken = _recycledHeaders;
            _recycledHeaders = null;
            return taken;
        }

        public void ReturnHeaders(MessageHeaders headers)
        {
            if (headers.CanRecycle)
            {
                headers.Recycle(HeaderInfoCache);
                _recycledHeaders = headers;
            }
        }
    }

    internal class HeaderInfoCache
    {
        private const int maxHeaderInfos = 4;
        private HeaderInfo[] _headerInfos;
        private int _index;

        public MessageHeaderInfo TakeHeaderInfo(XmlDictionaryReader reader, string actor, bool mustUnderstand, bool relay, bool isRefParam)
        {
            if (_headerInfos != null)
            {
                int i = _index;
                for (; ; )
                {
                    HeaderInfo headerInfo = _headerInfos[i];
                    if (headerInfo != null)
                    {
                        if (headerInfo.Matches(reader, actor, mustUnderstand, relay, isRefParam))
                        {
                            _headerInfos[i] = null;
                            _index = (i + 1) % maxHeaderInfos;
                            return headerInfo;
                        }
                    }
                    i = (i + 1) % maxHeaderInfos;
                    if (i == _index)
                    {
                        break;
                    }
                }
            }

            return new HeaderInfo(reader, actor, mustUnderstand, relay, isRefParam);
        }

        public void ReturnHeaderInfo(MessageHeaderInfo headerInfo)
        {
            HeaderInfo headerInfoToReturn = headerInfo as HeaderInfo;
            if (headerInfoToReturn != null)
            {
                if (_headerInfos == null)
                {
                    _headerInfos = new HeaderInfo[maxHeaderInfos];
                }
                int i = _index;
                for (; ; )
                {
                    if (_headerInfos[i] == null)
                    {
                        break;
                    }
                    i = (i + 1) % maxHeaderInfos;
                    if (i == _index)
                    {
                        break;
                    }
                }
                _headerInfos[i] = headerInfoToReturn;
                _index = (i + 1) % maxHeaderInfos;
            }
        }

        internal class HeaderInfo : MessageHeaderInfo
        {
            private string _name;
            private string _ns;
            private string _actor;
            private bool _isReferenceParameter;
            private bool _mustUnderstand;
            private bool _relay;

            public HeaderInfo(XmlDictionaryReader reader, string actor, bool mustUnderstand, bool relay, bool isReferenceParameter)
            {
                _actor = actor;
                _mustUnderstand = mustUnderstand;
                _relay = relay;
                _isReferenceParameter = isReferenceParameter;
                _name = reader.LocalName;
                _ns = reader.NamespaceURI;
            }

            public override string Name
            {
                get { return _name; }
            }

            public override string Namespace
            {
                get { return _ns; }
            }

            public override bool IsReferenceParameter
            {
                get { return _isReferenceParameter; }
            }

            public override string Actor
            {
                get { return _actor; }
            }

            public override bool MustUnderstand
            {
                get { return _mustUnderstand; }
            }

            public override bool Relay
            {
                get { return _relay; }
            }

            public bool Matches(XmlDictionaryReader reader, string actor, bool mustUnderstand, bool relay, bool isRefParam)
            {
                return reader.IsStartElement(_name, _ns) &&
                    _actor == actor && _mustUnderstand == mustUnderstand && _relay == relay && _isReferenceParameter == isRefParam;
            }
        }
    }

    internal class UriCache
    {
        private const int MaxKeyLength = 128;
        private const int MaxEntries = 8;
        private Entry[] _entries;
        private int _count;

        public UriCache()
        {
            _entries = new Entry[MaxEntries];
        }

        public Uri CreateUri(string uriString)
        {
            Uri uri = Get(uriString);
            if (uri == null)
            {
                uri = new Uri(uriString);
                Set(uriString, uri);
            }
            return uri;
        }

        private Uri Get(string key)
        {
            if (key.Length > MaxKeyLength)
            {
                return null;
            }

            for (int i = _count - 1; i >= 0; i--)
            {
                if (_entries[i].Key == key)
                {
                    return _entries[i].Value;
                }
            }

            return null;
        }

        private void Set(string key, Uri value)
        {
            if (key.Length > MaxKeyLength)
            {
                return;
            }

            if (_count < _entries.Length)
            {
                _entries[_count++] = new Entry(key, value);
            }
            else
            {
                Array.Copy(_entries, 1, _entries, 0, _entries.Length - 1);
                _entries[_count - 1] = new Entry(key, value);
            }
        }

        internal struct Entry
        {
            private Uri _value;

            public Entry(string key, Uri value)
            {
                Key = key;
                _value = value;
            }

            public string Key { get; }

            public Uri Value
            {
                get { return _value; }
            }
        }
    }
}
