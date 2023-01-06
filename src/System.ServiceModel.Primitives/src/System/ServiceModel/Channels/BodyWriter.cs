// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Channels
{
    public abstract class BodyWriter
    {
        private bool _canWrite;
        private object _thisLock;

        protected BodyWriter(bool isBuffered)
        {
            IsBuffered = isBuffered;
            _canWrite = true;
            if (!IsBuffered)
            {
                _thisLock = new object();
            }
        }

        public bool IsBuffered { get; }

        internal virtual bool IsEmpty
        {
            get { return false; }
        }

        internal virtual bool IsFault
        {
            get { return false; }
        }

        public BodyWriter CreateBufferedCopy(int maxBufferSize)
        {
            if (maxBufferSize < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(maxBufferSize), maxBufferSize,
                                                    SRP.ValueMustBeNonNegative));
            }

            if (IsBuffered)
            {
                return this;
            }
            else
            {
                lock (_thisLock)
                {
                    if (!_canWrite)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.BodyWriterCanOnlyBeWrittenOnce));
                    }

                    _canWrite = false;
                }
                BodyWriter bodyWriter = OnCreateBufferedCopy(maxBufferSize);
                if (!bodyWriter.IsBuffered)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.BodyWriterReturnedIsNotBuffered));
                }

                return bodyWriter;
            }
        }

        protected virtual BodyWriter OnCreateBufferedCopy(int maxBufferSize)
        {
            return OnCreateBufferedCopy(maxBufferSize, XmlDictionaryReaderQuotas.Max);
        }

        internal BodyWriter OnCreateBufferedCopy(int maxBufferSize, XmlDictionaryReaderQuotas quotas)
        {
            XmlBuffer buffer = new XmlBuffer(maxBufferSize);
            using (XmlDictionaryWriter writer = buffer.OpenSection(quotas))
            {
                writer.WriteStartElement("a");
                OnWriteBodyContents(writer);
                writer.WriteEndElement();
            }
            buffer.CloseSection();
            buffer.Close();
            return new BufferedBodyWriter(buffer);
        }

        protected abstract void OnWriteBodyContents(XmlDictionaryWriter writer);

        protected virtual Task OnWriteBodyContentsAsync(XmlDictionaryWriter writer)
        {
            OnWriteBodyContents(writer);
            return Task.CompletedTask;
        }

        protected virtual IAsyncResult OnBeginWriteBodyContents(XmlDictionaryWriter writer, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        protected virtual void OnEndWriteBodyContents(IAsyncResult result)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        private void EnsureWriteBodyContentsState(XmlDictionaryWriter writer)
        {
            if (writer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(writer)));
            }

            if (!IsBuffered)
            {
                lock (_thisLock)
                {
                    if (!_canWrite)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.BodyWriterCanOnlyBeWrittenOnce));
                    }

                    _canWrite = false;
                }
            }
        }

        public void WriteBodyContents(XmlDictionaryWriter writer)
        {
            EnsureWriteBodyContentsState(writer);
            OnWriteBodyContents(writer);
        }

        internal Task WriteBodyContentsAsync(XmlDictionaryWriter writer)
        {
            EnsureWriteBodyContentsState(writer);
            return OnWriteBodyContentsAsync(writer);
        }

        public IAsyncResult BeginWriteBodyContents(XmlDictionaryWriter writer, AsyncCallback callback, object state)
        {
            EnsureWriteBodyContentsState(writer);
            return OnBeginWriteBodyContents(writer, callback, state);
        }

        public void EndWriteBodyContents(IAsyncResult result)
        {
            OnEndWriteBodyContents(result);
        }

        internal class BufferedBodyWriter : BodyWriter
        {
            private XmlBuffer _buffer;

            public BufferedBodyWriter(XmlBuffer buffer)
                : base(true)
            {
                _buffer = buffer;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                XmlDictionaryReader reader = _buffer.GetReader(0);
                using (reader)
                {
                    reader.ReadStartElement();
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        writer.WriteNode(reader, false);
                    }
                    reader.ReadEndElement();
                }
            }
        }
    }
}
