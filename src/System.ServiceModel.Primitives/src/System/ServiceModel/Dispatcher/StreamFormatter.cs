// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IO;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using BufferedReadStream = System.ServiceModel.Channels.BufferedReadStream;

namespace System.ServiceModel.Dispatcher
{
    internal class StreamFormatter
    {
        private string _partNS;
        private int _streamIndex;
        private bool _isRequest;
        private string _operationName;
        private const int returnValueIndex = -1;

        internal static StreamFormatter Create(MessageDescription messageDescription, string operationName, bool isRequest)
        {
            MessagePartDescription streamPart = ValidateAndGetStreamPart(messageDescription, isRequest, operationName);
            if (streamPart == null)
            {
                return null;
            }

            return new StreamFormatter(messageDescription, streamPart, operationName, isRequest);
        }

        private StreamFormatter(MessageDescription messageDescription, MessagePartDescription streamPart, string operationName, bool isRequest)
        {
            if ((object)streamPart == (object)messageDescription.Body.ReturnValue)
            {
                _streamIndex = returnValueIndex;
            }
            else
            {
                _streamIndex = streamPart.Index;
            }

            WrapperName = messageDescription.Body.WrapperName;
            WrapperNamespace = messageDescription.Body.WrapperNamespace;
            PartName = streamPart.Name;
            _partNS = streamPart.Namespace;
            _isRequest = isRequest;
            _operationName = operationName;
        }

        internal void Serialize(XmlDictionaryWriter writer, object[] parameters, object returnValue)
        {
            Stream streamValue = GetStreamAndWriteStartWrapperIfNecessary(writer, parameters, returnValue);
            var streamProvider = new OperationStreamProvider(streamValue);
            writer.WriteValue(streamProvider);
            WriteEndWrapperIfNecessary(writer);
        }

        internal async Task SerializeAsync(XmlDictionaryWriter writer, object[] parameters, object returnValue)
        {
            Stream streamValue = await GetStreamAndWriteStartWrapperIfNecessaryAsync(writer, parameters, returnValue);
            var streamProvider = new OperationStreamProvider(streamValue);
            await writer.WriteValueAsync(streamProvider);
            await WriteEndWrapperIfNecessaryAsync(writer);
        }

        private Stream GetStreamAndWriteStartWrapperIfNecessary(XmlDictionaryWriter writer, object[] parameters, object returnValue)
        {
            Stream streamValue = GetStreamValue(parameters, returnValue);
            if (streamValue == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(PartName);
            }

            if (WrapperName != null)
            {
                writer.WriteStartElement(WrapperName, WrapperNamespace);
            }

            writer.WriteStartElement(PartName, PartNamespace);
            return streamValue;
        }

        private async Task<Stream> GetStreamAndWriteStartWrapperIfNecessaryAsync(XmlDictionaryWriter writer, object[] parameters, object returnValue)
        {
            Stream streamValue = GetStreamValue(parameters, returnValue);
            if (streamValue == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(PartName);
            }

            if (WrapperName != null)
            {
                await writer.WriteStartElementAsync(null, WrapperName, WrapperNamespace);
            }

            await writer.WriteStartElementAsync(null, PartName, PartNamespace);
            return streamValue;
        }

        private void WriteEndWrapperIfNecessary(XmlDictionaryWriter writer)
        {
            writer.WriteEndElement();
            if (WrapperName != null)
            {
                writer.WriteEndElement();
            }
        }

        private async Task WriteEndWrapperIfNecessaryAsync(XmlDictionaryWriter writer)
        {
            await writer.WriteEndElementAsync();
            if (WrapperName != null)
            {
                await writer.WriteEndElementAsync();
            }
        }

        internal IAsyncResult BeginSerialize(XmlDictionaryWriter writer, object[] parameters, object returnValue, AsyncCallback callback, object state)
        {
            return new SerializeAsyncResult(this, writer, parameters, returnValue, callback, state);
        }

        public void EndSerialize(IAsyncResult result)
        {
            SerializeAsyncResult.End(result);
        }

        internal class SerializeAsyncResult : AsyncResult
        {
            private static AsyncCompletion s_handleEndSerialize = new AsyncCompletion(HandleEndSerialize);

            private StreamFormatter _streamFormatter;
            private XmlDictionaryWriter _writer;

            internal SerializeAsyncResult(StreamFormatter streamFormatter, XmlDictionaryWriter writer, object[] parameters, object returnValue,
                AsyncCallback callback, object state)
                : base(callback, state)
            {
                _streamFormatter = streamFormatter;
                _writer = writer;

                // As we use the Task-returning method for async operation,
                // we shouldn't get to this point. Throw exception just in case.
                throw ExceptionHelper.AsError(NotImplemented.ByDesign);
            }

            private static bool HandleEndSerialize(IAsyncResult result)
            {
                SerializeAsyncResult thisPtr = (SerializeAsyncResult)result.AsyncState;
                thisPtr._streamFormatter.WriteEndWrapperIfNecessary(thisPtr._writer);
                return true;
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<SerializeAsyncResult>(result);
            }
        }

        internal void Deserialize(object[] parameters, ref object retVal, Message message)
        {
            SetStreamValue(parameters, ref retVal, new MessageBodyStream(message, WrapperName, WrapperNamespace, PartName, PartNamespace, _isRequest));
        }

        internal string WrapperName { get; set; }

        internal string WrapperNamespace { get; set; }

        internal string PartName { get; }

        internal string PartNamespace
        {
            get { return _partNS; }
        }


        private Stream GetStreamValue(object[] parameters, object returnValue)
        {
            if (_streamIndex == returnValueIndex)
            {
                return (Stream)returnValue;
            }

            return (Stream)parameters[_streamIndex];
        }

        private void SetStreamValue(object[] parameters, ref object returnValue, Stream streamValue)
        {
            if (_streamIndex == returnValueIndex)
            {
                returnValue = streamValue;
            }
            else
            {
                parameters[_streamIndex] = streamValue;
            }
        }

        private static MessagePartDescription ValidateAndGetStreamPart(MessageDescription messageDescription, bool isRequest, string operationName)
        {
            MessagePartDescription part = GetStreamPart(messageDescription);
            if (part != null)
            {
                return part;
            }

            if (HasStream(messageDescription))
            {
                if (messageDescription.IsTypedMessage)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SFxInvalidStreamInTypedMessage, messageDescription.MessageName)));
                }
                else if (isRequest)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SFxInvalidStreamInRequest, operationName)));
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SFxInvalidStreamInResponse, operationName)));
                }
            }
            return null;
        }

        private static bool HasStream(MessageDescription messageDescription)
        {
            if (messageDescription.Body.ReturnValue != null && messageDescription.Body.ReturnValue.Type == typeof(Stream))
            {
                return true;
            }

            foreach (MessagePartDescription part in messageDescription.Body.Parts)
            {
                if (part.Type == typeof(Stream))
                {
                    return true;
                }
            }
            return false;
        }

        private static MessagePartDescription GetStreamPart(MessageDescription messageDescription)
        {
            if (OperationFormatter.IsValidReturnValue(messageDescription.Body.ReturnValue))
            {
                if (messageDescription.Body.Parts.Count == 0)
                {
                    if (messageDescription.Body.ReturnValue.Type == typeof(Stream))
                    {
                        return messageDescription.Body.ReturnValue;
                    }
                }
            }
            else
            {
                if (messageDescription.Body.Parts.Count == 1)
                {
                    if (messageDescription.Body.Parts[0].Type == typeof(Stream))
                    {
                        return messageDescription.Body.Parts[0];
                    }
                }
            }
            return null;
        }

        internal static bool IsStream(MessageDescription messageDescription)
        {
            return GetStreamPart(messageDescription) != null;
        }

        internal class MessageBodyStream : Stream
        {
            private Message _message;
            private XmlDictionaryReader _reader;
            private BufferedReadStream _bufferedReadStream;
            private long _position;
            private string _wrapperName, _wrapperNs;
            private string _elementName, _elementNs;
            private bool _isRequest;

            internal MessageBodyStream(Message message, string wrapperName, string wrapperNs, string elementName, string elementNs, bool isRequest)
            {
                _message = message;
                _position = 0;
                _wrapperName = wrapperName;
                _wrapperNs = wrapperNs;
                _elementName = elementName;
                _elementNs = elementNs;
                _isRequest = isRequest;
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                if (count == 0)
                {
                    return 0;
                }

                if (_reader == null)
                {
                    if (_message.Properties.ContainsKey(BufferedReadStream.BufferedReadStreamPropertyName))
                    {
                        _bufferedReadStream =
                            _message.Properties[BufferedReadStream.BufferedReadStreamPropertyName] as BufferedReadStream;
                    }
                }

                using (TaskHelpers.RunTaskContinuationsOnOurThreads())
                {
                    if (_bufferedReadStream != null)
                    {
                        await _bufferedReadStream.PreReadBufferAsync(cancellationToken);
                    }

                    return Read(buffer, offset, count);
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                EnsureStreamIsOpen();
                if (buffer == null)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentNullException(nameof(buffer)), _message);
                }

                if (offset < 0)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(offset), offset,
                                                    SRP.Format(SRP.ValueMustBeNonNegative)), _message);
                }

                if (count < 0)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(count), count,
                                                    SRP.Format(SRP.ValueMustBeNonNegative)), _message);
                }

                if (buffer.Length - offset < count)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentException(SRP.Format(SRP.SFxInvalidStreamOffsetLength, offset + count)), _message);
                }

                if (count == 0)
                {
                    return 0;
                }

                try
                {
                    if (_reader == null)
                    {
                        _reader = _message.GetReaderAtBodyContents();
                        if (_wrapperName != null)
                        {
                            _reader.MoveToContent();
                            _reader.ReadStartElement(_wrapperName, _wrapperNs);
                        }
                        _reader.MoveToContent();
                        if (_reader.NodeType == XmlNodeType.EndElement)
                        {
                            return 0;
                        }

                        _reader.ReadStartElement(_elementName, _elementNs);
                    }
                    if (_reader.MoveToContent() != XmlNodeType.Text)
                    {
                        Exhaust(_reader);
                        return 0;
                    }
                    int bytesRead = _reader.ReadContentAsBase64(buffer, offset, count);
                    _position += bytesRead;
                    if (bytesRead == 0)
                    {
                        Exhaust(_reader);
                    }
                    return bytesRead;
                }
                catch (Exception ex)
                {
                    if (Fx.IsFatal(ex))
                    {
                        throw;
                    }

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new IOException(SRP.Format(SRP.SFxStreamIOException), ex));
                }
            }

            private void EnsureStreamIsOpen()
            {
                if (_message.State == MessageState.Closed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(SRP.Format(
                        _isRequest ? SRP.SFxStreamRequestMessageClosed : SRP.SFxStreamResponseMessageClosed)));
                }
            }

            private static void Exhaust(XmlDictionaryReader reader)
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        // drain
                    }
                }
            }

            public override long Position
            {
                get
                {
                    EnsureStreamIsOpen();
                    return _position;
                }
                set { throw TraceUtility.ThrowHelperError(new NotSupportedException(), _message); }
            }

            protected override void Dispose(bool isDisposing)
            {
                _message.Close();
                if (_reader != null)
                {
                    _reader.Dispose();
                    _reader = null;
                }
                base.Dispose(isDisposing);
            }
            public override bool CanRead { get { return _message.State != MessageState.Closed; } }
            public override bool CanSeek { get { return false; } }
            public override bool CanWrite { get { return false; } }
            public override long Length
            {
                get
                {
                    throw TraceUtility.ThrowHelperError(new NotSupportedException(), _message);
                }
            }
            public override void Flush() { throw TraceUtility.ThrowHelperError(new NotSupportedException(), _message); }
            public override long Seek(long offset, SeekOrigin origin) { throw TraceUtility.ThrowHelperError(new NotSupportedException(), _message); }
            public override void SetLength(long value) { throw TraceUtility.ThrowHelperError(new NotSupportedException(), _message); }
            public override void Write(byte[] buffer, int offset, int count) { throw TraceUtility.ThrowHelperError(new NotSupportedException(), _message); }
        }

        internal class OperationStreamProvider : IStreamProvider
        {
            private Stream _stream;

            internal OperationStreamProvider(Stream stream)
            {
                _stream = stream;
            }

            public Stream GetStream()
            {
                return _stream;
            }
            public void ReleaseStream(Stream stream)
            {
                //Noop
            }
        }
    }
}
