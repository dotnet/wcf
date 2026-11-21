// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IO;
using System.Runtime;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal abstract class XmlByteStreamReader : XmlDictionaryReader
    {
        private string _base64StringValue;
        private NameTable _nameTable;
        private bool _readBase64AsString;

        protected ReaderPosition _position;
        protected XmlDictionaryReaderQuotas _quotas;

        protected XmlByteStreamReader(XmlDictionaryReaderQuotas quotas)
        {
            _quotas = quotas;
            _position = ReaderPosition.None;
        }

        public override int AttributeCount => 0;

        public override string BaseURI => string.Empty;

        public override bool CanCanonicalize => false;

        public override bool CanReadBinaryContent => true;

        public override bool CanReadValueChunk => false;

        public override bool CanResolveEntity => false;

        public override int Depth => _position == ReaderPosition.Content ? 1 : 0;

        public override bool EOF => _position == ReaderPosition.EOF;

        public override bool HasAttributes => false;

        public override bool HasValue => _position == ReaderPosition.Content;

        public override bool IsDefault => false;

        public override bool IsEmptyElement => false;

        public override string LocalName => _position == ReaderPosition.StartElement ? ByteStreamMessageUtility.StreamElementName : null;

        public override void MoveToStartElement()
        {
            base.MoveToStartElement();
            _position = ReaderPosition.StartElement;
        }

        public override XmlNameTable NameTable
        {
            get
            {
                if (_nameTable == null)
                {
                    _nameTable = new NameTable();
                    _nameTable.Add(ByteStreamMessageUtility.StreamElementName);
                }

                return _nameTable;
            }
        }

        public override string NamespaceURI => string.Empty;

        public override XmlNodeType NodeType
        {
            get
            {
                switch (_position)
                {
                    case ReaderPosition.StartElement:
                        return XmlNodeType.Element;
                    case ReaderPosition.Content:
                        return XmlNodeType.Text;
                    case ReaderPosition.EndElement:
                        return XmlNodeType.EndElement;
                    default:
                        // and StreamPosition.EOF
                        return XmlNodeType.None;
                }
            }
        }

        public override string Prefix => string.Empty;

        public override XmlDictionaryReaderQuotas Quotas => _quotas;

        public override ReadState ReadState
        {
            get
            {
                switch (_position)
                {
                    case ReaderPosition.None:
                        return ReadState.Initial;
                    case ReaderPosition.StartElement:
                    case ReaderPosition.Content:
                    case ReaderPosition.EndElement:
                        return ReadState.Interactive;
                    case ReaderPosition.EOF:
                        return ReadState.Closed;
                    default:
                        Fx.Assert("Unknown ReadState hit in XmlByteStreamReader");
                        return ReadState.Error;
                }
            }
        }

        public override string Value
        {
            get
            {
                switch (_position)
                {
                    case ReaderPosition.Content:
                        if (!_readBase64AsString)
                        {
                            _base64StringValue = Convert.ToBase64String(ReadContentAsBase64());
                            _readBase64AsString = true;
                        }
                        return _base64StringValue;

                    default:
                        return string.Empty;
                }
            }
        }

        public override void Close()
        {
            if (!IsClosed)
            {
                try
                {
                    OnClose();
                }
                finally
                {
                    _position = ReaderPosition.EOF;
                    IsClosed = true;
                }
            }
        }

        protected bool IsClosed { get; private set; }

        protected virtual void OnClose()
        {
        }

        public override string GetAttribute(int i) => throw Fx.Exception.ArgumentOutOfRange(nameof(i), i, SRP.ArgumentNotInSetOfValidValues);

        public override string GetAttribute(string name, string namespaceURI) => null;

        public override string GetAttribute(string name) => null;

        public override string LookupNamespace(string prefix)
        {
            if (prefix == string.Empty)
            {
                return string.Empty;
            }
            else if (prefix == "xml")
            {
                return ByteStreamMessageUtility.XmlNamespace;
            }
            else if (prefix == "xmlns")
            {
                return ByteStreamMessageUtility.XmlNamespaceNamespace;
            }
            else
            {
                return null;
            }
        }

        public override bool MoveToAttribute(string name, string ns) => false;

        public override bool MoveToAttribute(string name) => false;

        public override bool MoveToElement()
        {
            if (_position == ReaderPosition.None)
            {
                _position = ReaderPosition.StartElement;
                return true;
            }

            return false;
        }

        public override bool MoveToFirstAttribute() => false;

        public override bool MoveToNextAttribute() => false;

        public override bool Read()
        {
            switch (_position)
            {
                case ReaderPosition.None:
                    _position = ReaderPosition.StartElement;
                    return true;
                case ReaderPosition.StartElement:
                    _position = ReaderPosition.Content;
                    return true;
                case ReaderPosition.Content:
                    _position = ReaderPosition.EndElement;
                    return true;
                case ReaderPosition.EndElement:
                    _position = ReaderPosition.EOF;
                    return false;
                case ReaderPosition.EOF:
                    return false;
                default:
                    // we should never get here
                    // it means we managed to get into some unknown position in the stream
                    Fx.Assert(false, "Unknown read position in XmlByteStreamReader");
                    return false;
            }
        }

        public override bool ReadAttributeValue() => false;

        public override abstract int ReadContentAsBase64(byte[] buffer, int index, int count);

        public override int ReadContentAsBinHex(byte[] buffer, int index, int count) => throw Fx.Exception.AsError(new NotSupportedException());

        public override void ResolveEntity() => throw Fx.Exception.AsError(new NotSupportedException());

        public byte[] ToByteArray()
        {
            if (IsClosed)
            {
                throw Fx.Exception.AsError(
                    new ObjectDisposedException(SRP.XmlReaderClosed));
            }

            return OnToByteArray();
        }

        protected abstract byte[] OnToByteArray();

        public Stream ToStream()
        {
            if (IsClosed)
            {
                throw Fx.Exception.AsError(
                    new ObjectDisposedException(SRP.XmlReaderClosed));
            }

            return OnToStream();
        }

        protected abstract Stream OnToStream();

        protected void EnsureInContent()
        {
            // This method is only being called from XmlByteStreamReader.ReadContentAsBase64.
            // We don't block if the position is None or StartElement since we want our XmlByteStreamReader
            // to be a little bit smarter when people just to access the content of the ByteStreamMessage.
            if (_position == ReaderPosition.EndElement
             || _position == ReaderPosition.EOF)
            {
                throw Fx.Exception.AsError(
                    new InvalidOperationException(SRP.Format(SRP.ByteStreamReaderNotInByteStream, ByteStreamMessageUtility.StreamElementName)));
            }
            else
            {
                _position = ReaderPosition.Content;
            }
        }

        protected enum ReaderPosition
        {
            None,
            StartElement,
            Content,
            EndElement,
            EOF
        }
    }
}
