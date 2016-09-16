// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens;
using System.IO;
using System.Runtime;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.Xml;
using IPrefixGenerator = System.IdentityModel.IPrefixGenerator;
using ISecurityElement = System.IdentityModel.ISecurityElement;

namespace System.ServiceModel.Security
{
    sealed class SecurityAppliedMessage : DelegatingMessage
    {
        private string _bodyId;
        private bool _bodyIdInserted;
        private string _bodyPrefix = MessageStrings.Prefix;
        private XmlBuffer _fullBodyBuffer;
        private ISecurityElement _encryptedBodyContent;
        private XmlAttributeHolder[] _bodyAttributes;
        private bool _delayedApplicationHandled;
        private readonly MessagePartProtectionMode _bodyProtectionMode;
        private BodyState _state = BodyState.Created;
        private readonly SendSecurityHeader _securityHeader;
        private MemoryStream _startBodyFragment;
        private MemoryStream _endBodyFragment;
        private byte[] _fullBodyFragment;
       // private int _fullBodyFragmentLength;

        public SecurityAppliedMessage(Message messageToProcess, SendSecurityHeader securityHeader, bool signBody, bool encryptBody)
            : base(messageToProcess)
        {
            Fx.Assert(!(messageToProcess is SecurityAppliedMessage), "SecurityAppliedMessage should not be wrapped");
            _securityHeader = securityHeader;
            _bodyProtectionMode = MessagePartProtectionModeHelper.GetProtectionMode(signBody, encryptBody, securityHeader.SignThenEncrypt);
        }

        public string BodyId
        {
            get { return _bodyId; }
        }

        public MessagePartProtectionMode BodyProtectionMode
        {
            get { return _bodyProtectionMode; }
        }

        internal byte[] PrimarySignatureValue
        {
            get { return _securityHeader.PrimarySignatureValue; }
        }

        Exception CreateBadStateException(string operation)
        {
            return new InvalidOperationException(SR.Format(SR.MessageBodyOperationNotValidInBodyState,
                operation, _state));
        }

        void EnsureUniqueSecurityApplication()
        {
            if (_delayedApplicationHandled)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.DelayedSecurityApplicationAlreadyCompleted)));
            }
            _delayedApplicationHandled = true;
        }

        protected override void OnBodyToString(XmlDictionaryWriter writer)
        {
            if (_state == BodyState.Created || _fullBodyFragment != null)
            {
                base.OnBodyToString(writer);
            }
            else
            {
                OnWriteBodyContents(writer);
            }
        }

        protected override void OnClose()
        {
            try
            {
                this.InnerMessage.Close();
            }
            finally
            {
                _fullBodyBuffer = null;
                _bodyAttributes = null;
                _encryptedBodyContent = null;
                _state = BodyState.Disposed;
            }
        }

        protected override void OnWriteStartBody(XmlDictionaryWriter writer)
        {
            if (_startBodyFragment != null || _fullBodyFragment != null)
            {
                WriteStartInnerMessageWithId(writer);
                return;
            }

            switch (_state)
            {
                case BodyState.Created:
                case BodyState.Encrypted:
                    this.InnerMessage.WriteStartBody(writer);
                    return;
                case BodyState.Signed:
                case BodyState.EncryptedThenSigned:
                    XmlDictionaryReader reader = _fullBodyBuffer.GetReader(0);
                    writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                    writer.WriteAttributes(reader, false);
                    reader.Dispose();
                    return;
                case BodyState.SignedThenEncrypted:
                    writer.WriteStartElement(_bodyPrefix, XD.MessageDictionary.Body, this.Version.Envelope.DictionaryNamespace);
                    if (_bodyAttributes != null)
                    {
                        XmlAttributeHolder.WriteAttributes(_bodyAttributes, writer);
                    }
                    return;
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateBadStateException("OnWriteStartBody"));
            }
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            switch (_state)
            {
                case BodyState.Created:
                    this.InnerMessage.WriteBodyContents(writer);
                    return;
                case BodyState.Signed:
                case BodyState.EncryptedThenSigned:
                    using (XmlDictionaryReader reader = _fullBodyBuffer.GetReader(0))
                    {
                        reader.ReadStartElement();
                        while (reader.NodeType != XmlNodeType.EndElement)
                            writer.WriteNode(reader, false);
                        reader.ReadEndElement();
                    }
                    return;
                case BodyState.Encrypted:
                case BodyState.SignedThenEncrypted:
                    _encryptedBodyContent.WriteTo(writer, ServiceModelDictionaryManager.Instance);
                    break;
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateBadStateException("OnWriteBodyContents"));
            }
        }

        protected override void OnWriteMessage(XmlDictionaryWriter writer)
        {
            // For Kerb one shot, the channel binding will be need to be fished out of the message, cached and added to the
            // token before calling ISC.

            AttachChannelBindingTokenIfFound();

            EnsureUniqueSecurityApplication();

            MessagePrefixGenerator prefixGenerator = new MessagePrefixGenerator(writer);
            _securityHeader.StartSecurityApplication();

            this.Headers.Add(_securityHeader);

            this.InnerMessage.WriteStartEnvelope(writer);

            this.Headers.RemoveAt(this.Headers.Count - 1);

            _securityHeader.ApplyBodySecurity(writer, prefixGenerator);

            this.InnerMessage.WriteStartHeaders(writer);
            _securityHeader.ApplySecurityAndWriteHeaders(this.Headers, writer, prefixGenerator);

            _securityHeader.RemoveSignatureEncryptionIfAppropriate();

            _securityHeader.CompleteSecurityApplication();
            _securityHeader.WriteHeader(writer, this.Version);
            writer.WriteEndElement();

            if (_fullBodyFragment != null)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                //((IFragmentCapableXmlDictionaryWriter)writer).WriteFragment(this.fullBodyFragment, 0, _fullBodyFragmentLength);
            }
            else
            {
                if (_startBodyFragment != null)
                {
                    throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                    //((IFragmentCapableXmlDictionaryWriter)writer).WriteFragment(this.startBodyFragment.GetBuffer(), 0, (int)this.startBodyFragment.Length);
                }
                else
                {
                    OnWriteStartBody(writer);
                }

                OnWriteBodyContents(writer);

                if (_endBodyFragment != null)
                {
                    throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                    //((IFragmentCapableXmlDictionaryWriter)writer).WriteFragment(_endBodyFragment.GetBuffer(), 0, (int)this.endBodyFragment.Length);
                }
                else
                {
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        void AttachChannelBindingTokenIfFound()
        {
            ChannelBindingMessageProperty cbmp = null;
            ChannelBindingMessageProperty.TryGet(this.InnerMessage, out cbmp);

            if (cbmp != null)
            {
                if (_securityHeader.ElementContainer != null && _securityHeader.ElementContainer.EndorsingSupportingTokens != null)
                {
                    foreach (SecurityToken token in _securityHeader.ElementContainer.EndorsingSupportingTokens)
                    {
                        ProviderBackedSecurityToken pbst = token as ProviderBackedSecurityToken;
                        if (pbst != null)
                        {
                            pbst.ChannelBinding = cbmp.ChannelBinding;
                        }
                    }
                }
            }
        }

        void SetBodyId()
        {
            _bodyId = this.InnerMessage.GetBodyAttribute(
                UtilityStrings.IdAttribute,
                _securityHeader.StandardsManager.IdManager.DefaultIdNamespaceUri);
            if (_bodyId == null)
            {
                _bodyId = _securityHeader.GenerateId();
                _bodyIdInserted = true;
            }
        }

        public void WriteBodyToEncrypt(EncryptedData encryptedData, SymmetricAlgorithm algorithm)
        {
            encryptedData.Id = _securityHeader.GenerateId();

            BodyContentHelper helper = new BodyContentHelper();
            XmlDictionaryWriter encryptingWriter = helper.CreateWriter();
            this.InnerMessage.WriteBodyContents(encryptingWriter);
            encryptedData.SetUpEncryption(algorithm, helper.ExtractResult());
            _encryptedBodyContent = encryptedData;

            _state = BodyState.Encrypted;
        }

        public void WriteBodyToEncryptThenSign(Stream canonicalStream, EncryptedData encryptedData, SymmetricAlgorithm algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //encryptedData.Id = this.securityHeader.GenerateId();
            //SetBodyId();

            //XmlDictionaryWriter encryptingWriter = XmlDictionaryWriter.CreateTextWriter(Stream.Null);
            //// The XmlSerializer body formatter would add a
            //// document declaration to the body fragment when a fresh writer 
            //// is provided. Hence, insert a dummy element here and capture 
            //// the body contents as a fragment.
            //encryptingWriter.WriteStartElement("a");
            //MemoryStream ms = new MemoryStream();
            //((IFragmentCapableXmlDictionaryWriter)encryptingWriter).StartFragment(ms, true);

            //this.InnerMessage.WriteBodyContents(encryptingWriter);
            //((IFragmentCapableXmlDictionaryWriter)encryptingWriter).EndFragment();
            //encryptingWriter.WriteEndElement();
            //ms.Flush();
            //encryptedData.SetUpEncryption(algorithm, new ArraySegment<byte>(ms.GetBuffer(), 0, (int) ms.Length));

            //this.fullBodyBuffer = new XmlBuffer(int.MaxValue);
            //XmlDictionaryWriter canonicalWriter = this.fullBodyBuffer.OpenSection(XmlDictionaryReaderQuotas.Max);

            //canonicalWriter.StartCanonicalization(canonicalStream, false, null);
            //WriteStartInnerMessageWithId(canonicalWriter);
            //encryptedData.WriteTo(canonicalWriter, ServiceModelDictionaryManager.Instance);
            //canonicalWriter.WriteEndElement();
            //canonicalWriter.EndCanonicalization();
            //canonicalWriter.Flush();

            //this.fullBodyBuffer.CloseSection();
            //this.fullBodyBuffer.Close();

            //this.state = BodyState.EncryptedThenSigned;
        }

        public void WriteBodyToSign(Stream canonicalStream)
        {
            SetBodyId();

            _fullBodyBuffer = new XmlBuffer(int.MaxValue);
            XmlDictionaryWriter canonicalWriter = _fullBodyBuffer.OpenSection(XmlDictionaryReaderQuotas.Max);
            canonicalWriter.StartCanonicalization(canonicalStream, false, null);
            WriteInnerMessageWithId(canonicalWriter);
            canonicalWriter.EndCanonicalization();
            canonicalWriter.Flush();
            _fullBodyBuffer.CloseSection();
            _fullBodyBuffer.Close();

            _state = BodyState.Signed;
        }

        public void WriteBodyToSignThenEncrypt(Stream canonicalStream, EncryptedData encryptedData, SymmetricAlgorithm algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //XmlBuffer buffer = new XmlBuffer(int.MaxValue);
            //XmlDictionaryWriter fragmentingWriter = buffer.OpenSection(XmlDictionaryReaderQuotas.Max);
            //WriteBodyToSignThenEncryptWithFragments(canonicalStream, false, null, encryptedData, algorithm, fragmentingWriter);
            //((IFragmentCapableXmlDictionaryWriter)fragmentingWriter).WriteFragment(this.startBodyFragment.GetBuffer(), 0, (int)this.startBodyFragment.Length);
            //((IFragmentCapableXmlDictionaryWriter)fragmentingWriter).WriteFragment(this.endBodyFragment.GetBuffer(), 0, (int)this.endBodyFragment.Length);
            //buffer.CloseSection();
            //buffer.Close();

            //this.startBodyFragment = null;
            //this.endBodyFragment = null;

            //XmlDictionaryReader reader = buffer.GetReader(0);
            //reader.MoveToContent();
            //this.bodyPrefix = reader.Prefix;
            //if (reader.HasAttributes)
            //{
            //    this.bodyAttributes = XmlAttributeHolder.ReadAttributes(reader);
            //}
            //reader.Close();
        }

        public void WriteBodyToSignThenEncryptWithFragments(
            Stream stream, bool includeComments, string[] inclusivePrefixes,
            EncryptedData encryptedData, SymmetricAlgorithm algorithm, XmlDictionaryWriter writer)
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //IFragmentCapableXmlDictionaryWriter fragmentingWriter = (IFragmentCapableXmlDictionaryWriter) writer;

            //SetBodyId();
            //encryptedData.Id = this.securityHeader.GenerateId();

            //this.startBodyFragment = new MemoryStream();
            //BufferedOutputStream bodyContentFragment = new BufferManagerOutputStream(SR.XmlBufferQuotaExceeded, 1024, int.MaxValue, this.securityHeader.StreamBufferManager);
            //this.endBodyFragment = new MemoryStream();

            //writer.StartCanonicalization(stream, includeComments, inclusivePrefixes);

            //fragmentingWriter.StartFragment(this.startBodyFragment, false);
            //WriteStartInnerMessageWithId(writer);
            //fragmentingWriter.EndFragment();

            //fragmentingWriter.StartFragment(bodyContentFragment, true);
            //this.InnerMessage.WriteBodyContents(writer);
            //fragmentingWriter.EndFragment();

            //fragmentingWriter.StartFragment(this.endBodyFragment, false);
            //writer.WriteEndElement();
            //fragmentingWriter.EndFragment();

            //writer.EndCanonicalization();

            //int bodyLength;
            //byte[] bodyBuffer = bodyContentFragment.ToArray(out bodyLength);

            //encryptedData.SetUpEncryption(algorithm, new ArraySegment<byte>(bodyBuffer, 0, bodyLength));
            //this.encryptedBodyContent = encryptedData;

            //this.state = BodyState.SignedThenEncrypted;
        }

        public void WriteBodyToSignWithFragments(Stream stream, bool includeComments, string[] inclusivePrefixes, XmlDictionaryWriter writer)
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //IFragmentCapableXmlDictionaryWriter fragmentingWriter = (IFragmentCapableXmlDictionaryWriter) writer;

            //SetBodyId();
            //BufferedOutputStream fullBodyFragment = new BufferManagerOutputStream(SR.XmlBufferQuotaExceeded, 1024, int.MaxValue, this.securityHeader.StreamBufferManager);
            //writer.StartCanonicalization(stream, includeComments, inclusivePrefixes);
            //fragmentingWriter.StartFragment(fullBodyFragment, false);
            //WriteStartInnerMessageWithId(writer);
            //this.InnerMessage.WriteBodyContents(writer);
            //writer.WriteEndElement();
            //fragmentingWriter.EndFragment();
            //writer.EndCanonicalization();

            //this.fullBodyFragment = fullBodyFragment.ToArray(out this.fullBodyFragmentLength);

            //this.state = BodyState.Signed;
        }

        void WriteInnerMessageWithId(XmlDictionaryWriter writer)
        {
            WriteStartInnerMessageWithId(writer);
            this.InnerMessage.WriteBodyContents(writer);
            writer.WriteEndElement();
        }

        void WriteStartInnerMessageWithId(XmlDictionaryWriter writer)
        {
            this.InnerMessage.WriteStartBody(writer);
            if (_bodyIdInserted)
            {
                _securityHeader.StandardsManager.IdManager.WriteIdAttribute(writer, _bodyId);
            }
        }

        enum BodyState
        {
            Created,
            Signed,
            SignedThenEncrypted,
            EncryptedThenSigned,
            Encrypted,
            Disposed,
        }

        struct BodyContentHelper
        {
            MemoryStream stream;
            XmlDictionaryWriter writer;

            public XmlDictionaryWriter CreateWriter()
            {
                this.stream = new MemoryStream();
                this.writer = XmlDictionaryWriter.CreateTextWriter(stream);
                return this.writer;
            }

            public ArraySegment<byte> ExtractResult()
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

                //this.writer.Flush();
                //return new ArraySegment<byte>(this.stream.GetBuffer(), 0, (int) this.stream.Length);
            }
        }

        sealed class MessagePrefixGenerator : IPrefixGenerator
        {
            XmlWriter writer;

            public MessagePrefixGenerator(XmlWriter writer)
            {
                this.writer = writer;
            }

            public string GetPrefix(string namespaceUri, int depth, bool isForAttribute)
            {
                return this.writer.LookupPrefix(namespaceUri);
            }
        }
    }
}

