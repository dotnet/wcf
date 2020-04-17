// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.IO;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.ServiceModel.Diagnostics;
using System.Text;
using Microsoft.Xml;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace System.ServiceModel.Channels
{
    internal class TextMessageEncoderFactory : MessageEncoderFactory
    {
        private TextMessageEncoder _messageEncoder;
        internal static ContentEncoding[] Soap11Content = GetContentEncodingMap(MessageVersion.Soap11WSAddressing10);
        internal static ContentEncoding[] Soap12Content = GetContentEncodingMap(MessageVersion.Soap12WSAddressing10);
        internal static ContentEncoding[] SoapNoneContent = GetContentEncodingMap(MessageVersion.None);
        internal const string Soap11MediaType = "text/xml";
        internal const string Soap12MediaType = "application/soap+xml";
        private const string XmlMediaType = "application/xml";

        public TextMessageEncoderFactory(MessageVersion version, Encoding writeEncoding, int maxReadPoolSize, int maxWritePoolSize, XmlDictionaryReaderQuotas quotas)
        {
            _messageEncoder = new TextMessageEncoder(version, writeEncoding, maxReadPoolSize, maxWritePoolSize, quotas);
        }

        public override MessageEncoder Encoder
        {
            get { return _messageEncoder; }
        }

        public override MessageVersion MessageVersion
        {
            get { return _messageEncoder.MessageVersion; }
        }

        public int MaxWritePoolSize
        {
            get { return _messageEncoder.MaxWritePoolSize; }
        }

        public int MaxReadPoolSize
        {
            get { return _messageEncoder.MaxReadPoolSize; }
        }

        public static Encoding[] GetSupportedEncodings()
        {
            Encoding[] supported = TextEncoderDefaults.SupportedEncodings;
            Encoding[] enc = new Encoding[supported.Length];
            Array.Copy(supported, enc, supported.Length);
            return enc;
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get
            {
                return _messageEncoder.ReaderQuotas;
            }
        }

        internal static string GetMediaType(MessageVersion version)
        {
            string mediaType = null;
            if (version.Envelope == EnvelopeVersion.Soap12)
            {
                mediaType = TextMessageEncoderFactory.Soap12MediaType;
            }
            else if (version.Envelope == EnvelopeVersion.Soap11)
            {
                mediaType = TextMessageEncoderFactory.Soap11MediaType;
            }
            else if (version.Envelope == EnvelopeVersion.None)
            {
                mediaType = TextMessageEncoderFactory.XmlMediaType;
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    string.Format(SRServiceModel.EnvelopeVersionNotSupported, version.Envelope)));
            }
            return mediaType;
        }

        internal static string GetContentType(string mediaType, Encoding encoding)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}; charset={1}", mediaType, TextEncoderDefaults.EncodingToCharSet(encoding));
        }

        private static ContentEncoding[] GetContentEncodingMap(MessageVersion version)
        {
            Encoding[] readEncodings = TextMessageEncoderFactory.GetSupportedEncodings();
            string media = GetMediaType(version);
            ContentEncoding[] map = new ContentEncoding[readEncodings.Length];
            for (int i = 0; i < readEncodings.Length; i++)
            {
                ContentEncoding contentEncoding = new ContentEncoding();
                contentEncoding.contentType = GetContentType(media, readEncodings[i]);
                contentEncoding.encoding = readEncodings[i];
                map[i] = contentEncoding;
            }
            return map;
        }

        internal static Encoding GetEncodingFromContentType(string contentType, ContentEncoding[] contentMap)
        {
            if (contentType == null)
            {
                return null;
            }

            // Check for known/expected content types
            for (int i = 0; i < contentMap.Length; i++)
            {
                if (contentMap[i].contentType == contentType)
                {
                    return contentMap[i].encoding;
                }
            }

            // then some heuristic matches (since System.Mime.ContentType is a performance hit)
            // start by looking for a parameter. 

            // If none exists, we don't have an encoding
            int semiColonIndex = contentType.IndexOf(';');
            if (semiColonIndex == -1)
            {
                return null;
            }

            // optimize for charset being the first parameter
            int charsetValueIndex = -1;

            // for Indigo scenarios, we'll have "; charset=", so check for the c
            if ((contentType.Length > semiColonIndex + 11) // need room for parameter + charset + '=' 
                && contentType[semiColonIndex + 2] == 'c'
                && string.Compare("charset=", 0, contentType, semiColonIndex + 2, 8, StringComparison.OrdinalIgnoreCase) == 0)
            {
                charsetValueIndex = semiColonIndex + 10;
            }
            else
            {
                // look for charset= somewhere else in the message
                int paramIndex = contentType.IndexOf("charset=", semiColonIndex + 1, StringComparison.OrdinalIgnoreCase);
                if (paramIndex != -1)
                {
                    // validate there's only whitespace or semi-colons beforehand
                    for (int i = paramIndex - 1; i >= semiColonIndex; i--)
                    {
                        if (contentType[i] == ';')
                        {
                            charsetValueIndex = paramIndex + 8;
                            break;
                        }

                        if (contentType[i] == '\n')
                        {
                            if (i == semiColonIndex || contentType[i - 1] != '\r')
                            {
                                break;
                            }

                            i--;
                            continue;
                        }

                        if (contentType[i] != ' '
                            && contentType[i] != '\t')
                        {
                            break;
                        }
                    }
                }
            }

            string charSet;
            Encoding enc;

            // we have a possible charset value. If it's easy to parse, do so
            if (charsetValueIndex != -1)
            {
                // get the next semicolon
                semiColonIndex = contentType.IndexOf(';', charsetValueIndex);
                if (semiColonIndex == -1)
                {
                    charSet = contentType.Substring(charsetValueIndex);
                }
                else
                {
                    charSet = contentType.Substring(charsetValueIndex, semiColonIndex - charsetValueIndex);
                }

                // and some minimal quote stripping
                if (charSet.Length > 2 && charSet[0] == '"' && charSet[charSet.Length - 1] == '"')
                {
                    charSet = charSet.Substring(1, charSet.Length - 2);
                }

                if (TryGetEncodingFromCharSet(charSet, out enc))
                {
                    return enc;
                }
            }

            // our quick heuristics failed. fall back to System.Net
            try
            {
                MediaTypeHeaderValue parsedContentType = MediaTypeHeaderValue.Parse(contentType);
                charSet = parsedContentType.CharSet;
            }
            catch (FormatException e)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ProtocolException(SRServiceModel.EncoderBadContentType, e));
            }

            if (TryGetEncodingFromCharSet(charSet, out enc))
                return enc;

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ProtocolException(string.Format(SRServiceModel.EncoderUnrecognizedCharSet, charSet)));
        }

        internal static bool TryGetEncodingFromCharSet(string charSet, out Encoding encoding)
        {
            encoding = null;
            if (charSet == null || charSet.Length == 0)
                return true;

            return TextEncoderDefaults.TryGetEncoding(charSet, out encoding);
        }

        internal class ContentEncoding
        {
            internal string contentType;
            internal Encoding encoding;
        }

        internal class TextMessageEncoder : MessageEncoder
        {
            private int _maxReadPoolSize;
            private int _maxWritePoolSize;

            // Double-checked locking pattern requires volatile for read/write synchronization
            private volatile SynchronizedPool<UTF8BufferedMessageData> _bufferedReaderPool;
            private volatile SynchronizedPool<TextBufferedMessageWriter> _bufferedWriterPool;
            private volatile SynchronizedPool<RecycledMessageState> _recycledStatePool;

            private object _thisLock;
            private string _contentType;
            private string _mediaType;
            private Encoding _writeEncoding;
            private MessageVersion _version;
            private bool _optimizeWriteForUTF8;
            private const int maxPooledXmlReadersPerMessage = 2;
            private XmlDictionaryReaderQuotas _readerQuotas;
            private XmlDictionaryReaderQuotas _bufferedReadReaderQuotas;
            private ContentEncoding[] _contentEncodingMap;

            public TextMessageEncoder(MessageVersion version, Encoding writeEncoding, int maxReadPoolSize, int maxWritePoolSize, XmlDictionaryReaderQuotas quotas)
            {
                if (version == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("version");
                if (writeEncoding == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writeEncoding");

                TextEncoderDefaults.ValidateEncoding(writeEncoding);
                _writeEncoding = writeEncoding;
                _optimizeWriteForUTF8 = IsUTF8Encoding(writeEncoding);

                _thisLock = new object();

                _version = version;
                _maxReadPoolSize = maxReadPoolSize;
                _maxWritePoolSize = maxWritePoolSize;

                _readerQuotas = new XmlDictionaryReaderQuotas();
                quotas.CopyTo(_readerQuotas);
                _bufferedReadReaderQuotas = EncoderHelpers.GetBufferedReadQuotas(_readerQuotas);

                _mediaType = TextMessageEncoderFactory.GetMediaType(version);
                _contentType = TextMessageEncoderFactory.GetContentType(_mediaType, writeEncoding);
                if (version.Envelope == EnvelopeVersion.Soap12)
                {
                    _contentEncodingMap = TextMessageEncoderFactory.Soap12Content;
                }
                else if (version.Envelope == EnvelopeVersion.Soap11)
                {
                    // public profile does not allow SOAP1.1/WSA1.0. However, the EnvelopeVersion 1.1 is supported. Need to know what the implications are here
                    // but I think that it's not necessary to have here since we're a sender in N only. 
                    _contentEncodingMap = TextMessageEncoderFactory.Soap11Content;
                }
                else if (version.Envelope == EnvelopeVersion.None)
                {
                    _contentEncodingMap = TextMessageEncoderFactory.SoapNoneContent;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        string.Format(SRServiceModel.EnvelopeVersionNotSupported, version.Envelope)));
                }
            }

            private static bool IsUTF8Encoding(Encoding encoding)
            {
                return encoding.WebName == "utf-8";
            }

            public override string ContentType
            {
                get { return _contentType; }
            }

            public int MaxWritePoolSize
            {
                get { return _maxWritePoolSize; }
            }

            public int MaxReadPoolSize
            {
                get { return _maxReadPoolSize; }
            }

            public XmlDictionaryReaderQuotas ReaderQuotas
            {
                get
                {
                    return _readerQuotas;
                }
            }

            public override string MediaType
            {
                get { return _mediaType; }
            }

            public override MessageVersion MessageVersion
            {
                get { return _version; }
            }

            private object ThisLock
            {
                get { return _thisLock; }
            }


            internal override bool IsCharSetSupported(string charSet)
            {
                Encoding tmp;
                return TextEncoderDefaults.TryGetEncoding(charSet, out tmp);
            }

            public override bool IsContentTypeSupported(string contentType)
            {
                if (contentType == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contentType");
                }

                if (base.IsContentTypeSupported(contentType))
                {
                    return true;
                }

                // we support a few extra content types for "none"
                if (MessageVersion == MessageVersion.None)
                {
                    const string rss1MediaType = "text/xml";
                    const string rss2MediaType = "application/rss+xml";
                    const string atomMediaType = "application/atom+xml";
                    const string htmlMediaType = "text/html";

                    if (IsContentTypeSupported(contentType, rss1MediaType, rss1MediaType))
                    {
                        return true;
                    }
                    if (IsContentTypeSupported(contentType, rss2MediaType, rss2MediaType))
                    {
                        return true;
                    }
                    if (IsContentTypeSupported(contentType, htmlMediaType, atomMediaType))
                    {
                        return true;
                    }
                    if (IsContentTypeSupported(contentType, atomMediaType, atomMediaType))
                    {
                        return true;
                    }
                    // application/xml checked by base method
                }

                return false;
            }

            public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
            {
                if (bufferManager == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("bufferManager"));

                if (WcfEventSource.Instance.TextMessageDecodingStartIsEnabled())
                {
                    WcfEventSource.Instance.TextMessageDecodingStart();
                }

                Message message;

                UTF8BufferedMessageData messageData = TakeBufferedReader();
                messageData.Encoding = GetEncodingFromContentType(contentType, _contentEncodingMap);
                messageData.Open(buffer, bufferManager);
                RecycledMessageState messageState = messageData.TakeMessageState();
                if (messageState == null)
                    messageState = new RecycledMessageState();
                message = new BufferedMessage(messageData, messageState);

                message.Properties.Encoder = this;

                if (WcfEventSource.Instance.MessageReadByEncoderIsEnabled())
                {
                    WcfEventSource.Instance.MessageReadByEncoder(
                        EventTraceActivityHelper.TryExtractActivity(message, true),
                        buffer.Count,
                        this);
                }

                if (MessageLogger.LogMessagesAtTransportLevel)
                    MessageLogger.LogMessage(ref message, MessageLoggingSource.TransportReceive);

                return message;
            }

            public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
            {
                if (stream == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("stream"));

                if (WcfEventSource.Instance.TextMessageDecodingStartIsEnabled())
                {
                    WcfEventSource.Instance.TextMessageDecodingStart();
                }

                XmlReader reader = TakeStreamedReader(stream, GetEncodingFromContentType(contentType, _contentEncodingMap));
                Message message = Message.CreateMessage(reader, maxSizeOfHeaders, _version);
                message.Properties.Encoder = this;

                if (WcfEventSource.Instance.StreamedMessageReadByEncoderIsEnabled())
                {
                    WcfEventSource.Instance.StreamedMessageReadByEncoder(EventTraceActivityHelper.TryExtractActivity(message, true));
                }

                if (MessageLogger.LogMessagesAtTransportLevel)
                    MessageLogger.LogMessage(ref message, MessageLoggingSource.TransportReceive);
                return message;
            }

            public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
            {
                return WriteMessageAsync(message, maxMessageSize, bufferManager, messageOffset).WaitForCompletion();
            }

            public override void WriteMessage(Message message, Stream stream)
            {
                WriteMessageAsyncInternal(message, stream).WaitForCompletion();
            }

            public override Task<ArraySegment<byte>> WriteMessageAsync(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
            {
                if (message == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("message"));
                if (bufferManager == null)
                    throw TraceUtility.ThrowHelperError(new ArgumentNullException("bufferManager"), message);
                if (maxMessageSize < 0)
                    throw TraceUtility.ThrowHelperError(new ArgumentOutOfRangeException("maxMessageSize", maxMessageSize,
                                                                SRServiceModel.ValueMustBeNonNegative), message);
                if (messageOffset < 0 || messageOffset > maxMessageSize)
                    throw TraceUtility.ThrowHelperError(new ArgumentOutOfRangeException("messageOffset", messageOffset,
                                                    string.Format(SRServiceModel.ValueMustBeInRange, 0, maxMessageSize)), message);

                ThrowIfMismatchedMessageVersion(message);

                EventTraceActivity eventTraceActivity = null;
                if (WcfEventSource.Instance.TextMessageEncodingStartIsEnabled())
                {
                    eventTraceActivity = EventTraceActivityHelper.TryExtractActivity(message);
                    WcfEventSource.Instance.TextMessageEncodingStart(eventTraceActivity);
                }

                message.Properties.Encoder = this;
                TextBufferedMessageWriter messageWriter = TakeBufferedWriter();

                ArraySegment<byte> messageData = messageWriter.WriteMessage(message, bufferManager, messageOffset, maxMessageSize);
                ReturnMessageWriter(messageWriter);

                if (WcfEventSource.Instance.MessageWrittenByEncoderIsEnabled())
                {
                    WcfEventSource.Instance.MessageWrittenByEncoder(
                        eventTraceActivity ?? EventTraceActivityHelper.TryExtractActivity(message),
                        messageData.Count,
                        this);
                }

                if (MessageLogger.LogMessagesAtTransportLevel)
                {
                    XmlDictionaryReader xmlDictionaryReader = XmlDictionaryReader.CreateTextReader(messageData.Array, messageData.Offset, messageData.Count, XmlDictionaryReaderQuotas.Max);
                    MessageLogger.LogMessage(ref message, xmlDictionaryReader, MessageLoggingSource.TransportSend);
                }

                return Task.FromResult(messageData);
            }

            private async Task WriteMessageAsyncInternal(Message message, Stream stream)
            {
                await TaskHelpers.EnsureDefaultTaskScheduler();
                await WriteMessageAsync(message, stream);
            }

            public override async Task WriteMessageAsync(Message message, Stream stream)
            {
                if (message == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("message"));
                if (stream == null)
                    throw TraceUtility.ThrowHelperError(new ArgumentNullException("stream"), message);
                ThrowIfMismatchedMessageVersion(message);

                EventTraceActivity eventTraceActivity = null;
                if (WcfEventSource.Instance.TextMessageEncodingStartIsEnabled())
                {
                    eventTraceActivity = EventTraceActivityHelper.TryExtractActivity(message);
                    WcfEventSource.Instance.TextMessageEncodingStart(eventTraceActivity);
                }

                message.Properties.Encoder = this;
                XmlDictionaryWriter xmlWriter = TakeStreamedWriter(stream);
                if (_optimizeWriteForUTF8)
                {
                    await message.WriteMessageAsync(xmlWriter);
                }
                else
                {
                    xmlWriter.WriteStartDocument();
                    await message.WriteMessageAsync(xmlWriter);
                    xmlWriter.WriteEndDocument();
                }

                xmlWriter.Flush();
                ReturnStreamedWriter(xmlWriter);

                if (WcfEventSource.Instance.StreamedMessageWrittenByEncoderIsEnabled())
                {
                    WcfEventSource.Instance.StreamedMessageWrittenByEncoder(eventTraceActivity ?? EventTraceActivityHelper.TryExtractActivity(message));
                }

                if (MessageLogger.LogMessagesAtTransportLevel)
                    MessageLogger.LogMessage(ref message, MessageLoggingSource.TransportSend);
            }

            public override IAsyncResult BeginWriteMessage(Message message, Stream stream, AsyncCallback callback, object state)
            {
                return this.WriteMessageAsync(message, stream).ToApm(callback, state);
            }

            public override void EndWriteMessage(IAsyncResult result)
            {
                result.ToApmEnd();
            }


            private XmlDictionaryWriter TakeStreamedWriter(Stream stream)
            {
                return XmlDictionaryWriter.CreateTextWriter(stream, _writeEncoding, false);
            }

            private void ReturnStreamedWriter(XmlWriter xmlWriter)
            {
                Contract.Assert(xmlWriter != null, "xmlWriter MUST NOT be null");
                xmlWriter.Flush();
                xmlWriter.Dispose();
            }

            private TextBufferedMessageWriter TakeBufferedWriter()
            {
                if (_bufferedWriterPool == null)
                {
                    lock (ThisLock)
                    {
                        if (_bufferedWriterPool == null)
                        {
                            _bufferedWriterPool = new SynchronizedPool<TextBufferedMessageWriter>(_maxWritePoolSize);
                        }
                    }
                }

                TextBufferedMessageWriter messageWriter = _bufferedWriterPool.Take();
                if (messageWriter == null)
                {
                    messageWriter = new TextBufferedMessageWriter(this);
                    if (WcfEventSource.Instance.WritePoolMissIsEnabled())
                    {
                        WcfEventSource.Instance.WritePoolMiss(messageWriter.GetType().Name);
                    }
                }
                return messageWriter;
            }

            private void ReturnMessageWriter(TextBufferedMessageWriter messageWriter)
            {
                _bufferedWriterPool.Return(messageWriter);
            }

            private XmlReader TakeStreamedReader(Stream stream, Encoding enc)
            {
                return XmlDictionaryReader.CreateTextReader(stream, _readerQuotas);
            }


            private XmlDictionaryWriter CreateWriter(Stream stream)
            {
                return XmlDictionaryWriter.CreateTextWriter(stream, _writeEncoding, false);
            }

            private UTF8BufferedMessageData TakeBufferedReader()
            {
                if (_bufferedReaderPool == null)
                {
                    lock (ThisLock)
                    {
                        if (_bufferedReaderPool == null)
                        {
                            _bufferedReaderPool = new SynchronizedPool<UTF8BufferedMessageData>(_maxReadPoolSize);
                        }
                    }
                }
                UTF8BufferedMessageData messageData = _bufferedReaderPool.Take();
                if (messageData == null)
                {
                    messageData = new UTF8BufferedMessageData(this, maxPooledXmlReadersPerMessage);
                    if (WcfEventSource.Instance.ReadPoolMissIsEnabled())
                    {
                        WcfEventSource.Instance.ReadPoolMiss(messageData.GetType().Name);
                    }
                }
                return messageData;
            }

            private void ReturnBufferedData(UTF8BufferedMessageData messageData)
            {
                _bufferedReaderPool.Return(messageData);
            }

            private SynchronizedPool<RecycledMessageState> RecycledStatePool
            {
                get
                {
                    if (_recycledStatePool == null)
                    {
                        lock (ThisLock)
                        {
                            if (_recycledStatePool == null)
                            {
                                _recycledStatePool = new SynchronizedPool<RecycledMessageState>(_maxReadPoolSize);
                            }
                        }
                    }
                    return _recycledStatePool;
                }
            }


            private static readonly byte[] s_xmlDeclarationStartText = { (byte)'<', (byte)'?', (byte)'x', (byte)'m', (byte)'l' };
            private static readonly byte[] s_version10Text = { (byte)'v', (byte)'e', (byte)'r', (byte)'s', (byte)'i', (byte)'o', (byte)'n', (byte)'=', (byte)'"', (byte)'1', (byte)'.', (byte)'0', (byte)'"' };
            private static readonly byte[] s_encodingText = { (byte)'e', (byte)'n', (byte)'c', (byte)'o', (byte)'d', (byte)'i', (byte)'n', (byte)'g', (byte)'=' };

            internal class UTF8BufferedMessageData : BufferedMessageData
            {
                private TextMessageEncoder _messageEncoder;
                private Encoding _encoding;

                private const int additionalNodeSpace = 1024;

                public UTF8BufferedMessageData(TextMessageEncoder messageEncoder, int maxReaderPoolSize)
                    : base(messageEncoder.RecycledStatePool)
                {
                    _messageEncoder = messageEncoder;
                }

                internal Encoding Encoding
                {
                    set
                    {
                        _encoding = value;
                    }
                }

                public override MessageEncoder MessageEncoder
                {
                    get { return _messageEncoder; }
                }

                public override XmlDictionaryReaderQuotas Quotas
                {
                    get { return _messageEncoder._bufferedReadReaderQuotas; }
                }

                protected override void OnClosed()
                {
                    _messageEncoder.ReturnBufferedData(this);
                }

                protected override XmlDictionaryReader TakeXmlReader()
                {
                    ArraySegment<byte> buffer = this.Buffer;
                    return XmlDictionaryReader.CreateTextReader(buffer.Array, buffer.Offset, buffer.Count, this.Quotas);
                }

                protected override void ReturnXmlReader(XmlDictionaryReader xmlReader)
                {
                    Contract.Assert(xmlReader != null, "xmlReader MUST NOT be null");
                    xmlReader.Dispose();
                }
            }

            internal class TextBufferedMessageWriter : BufferedMessageWriter
            {
                private TextMessageEncoder _messageEncoder;

                public TextBufferedMessageWriter(TextMessageEncoder messageEncoder)
                {
                    _messageEncoder = messageEncoder;
                }

                protected override void OnWriteStartMessage(XmlDictionaryWriter writer)
                {
                    if (!_messageEncoder._optimizeWriteForUTF8)
                        writer.WriteStartDocument();
                }

                protected override void OnWriteEndMessage(XmlDictionaryWriter writer)
                {
                    if (!_messageEncoder._optimizeWriteForUTF8)
                        writer.WriteEndDocument();
                }

                protected override XmlDictionaryWriter TakeXmlWriter(Stream stream)
                {
                    if (_messageEncoder._optimizeWriteForUTF8)
                    {
                        return XmlDictionaryWriter.CreateTextWriter(stream, _messageEncoder._writeEncoding, false);
                    }
                    else
                    {
                        return _messageEncoder.CreateWriter(stream);
                    }
                }

                protected override void ReturnXmlWriter(XmlDictionaryWriter writer)
                {
                    Contract.Assert(writer != null, "writer MUST NOT be null");
                    writer.Flush();
                    writer.Dispose();
                }
            }
        }
    }
}
