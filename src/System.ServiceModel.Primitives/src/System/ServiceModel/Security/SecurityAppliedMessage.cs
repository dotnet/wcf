// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Xml;

using IPrefixGenerator = System.IdentityModel.IPrefixGenerator;

namespace System.ServiceModel.Security
{
    internal sealed class SecurityAppliedMessage : DelegatingMessage
    {
        private bool _bodyIdInserted;
        private string _bodyPrefix = MessageStrings.Prefix;
        private XmlBuffer _fullBodyBuffer;
        private XmlAttributeHolder[] _bodyAttributes;
        private bool _delayedApplicationHandled;
        private BodyState _state = BodyState.Created;
        private readonly SendSecurityHeader _securityHeader;
#pragma warning disable CS0649 // Field is never assign to
        private MemoryStream _startBodyFragment;
        private MemoryStream _endBodyFragment;
#pragma warning restore CS0649 // Field is never assign to
        private byte[] _fullBodyFragment;
        private int _fullBodyFragmentLength;

        public SecurityAppliedMessage(Message messageToProcess, SendSecurityHeader securityHeader, bool signBody, bool encryptBody)
            : base(messageToProcess)
        {
            Fx.Assert(!(messageToProcess is SecurityAppliedMessage), "SecurityAppliedMessage should not be wrapped");
            _securityHeader = securityHeader;
            BodyProtectionMode = MessagePartProtectionModeHelper.GetProtectionMode(signBody, encryptBody, securityHeader.SignThenEncrypt);
        }

        public string BodyId { get; private set; }

        public MessagePartProtectionMode BodyProtectionMode { get; }

        private Exception CreateBadStateException(string operation)
        {
            return new InvalidOperationException(SRP.Format(SRP.MessageBodyOperationNotValidInBodyState, operation, _state));
        }

        private void EnsureUniqueSecurityApplication()
        {
            if (_delayedApplicationHandled)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.DelayedSecurityApplicationAlreadyCompleted));
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
                InnerMessage.Close();
            }
            finally
            {
                _fullBodyBuffer = null;
                _bodyAttributes = null;
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
                    InnerMessage.WriteStartBody(writer);
                    return;
                case BodyState.Signed:
                case BodyState.EncryptedThenSigned:
                    XmlDictionaryReader reader = _fullBodyBuffer.GetReader(0);
                    writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                    writer.WriteAttributes(reader, false);
                    reader.Close();
                    return;
                case BodyState.SignedThenEncrypted:
                    writer.WriteStartElement(_bodyPrefix, XD.MessageDictionary.Body, Version.Envelope.DictionaryNamespace);
                    if (_bodyAttributes != null)
                    {
                        XmlAttributeHolder.WriteAttributes(_bodyAttributes, writer);
                    }
                    return;
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateBadStateException(nameof(OnWriteStartBody)));
            }
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            switch (_state)
            {
                case BodyState.Created:
                    InnerMessage.WriteBodyContents(writer);
                    return;
                case BodyState.Signed:
                case BodyState.EncryptedThenSigned:
                    XmlDictionaryReader reader = _fullBodyBuffer.GetReader(0);
                    reader.ReadStartElement();
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        writer.WriteNode(reader, false);
                    }

                    reader.ReadEndElement();
                    reader.Close();
                    return;
                case BodyState.Encrypted:
                case BodyState.SignedThenEncrypted:
                    throw ExceptionHelper.PlatformNotSupported();
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateBadStateException(nameof(OnWriteBodyContents)));
            }
        }

        protected override async Task OnWriteBodyContentsAsync(XmlDictionaryWriter writer)
        {
            switch (_state)
            {
                case BodyState.Created:
                    await InnerMessage.WriteBodyContentsAsync(writer);
                    return;
                case BodyState.Signed:
                case BodyState.EncryptedThenSigned:
                    XmlDictionaryReader reader = _fullBodyBuffer.GetReader(0);
                    reader.ReadStartElement();
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        await writer.WriteNodeAsync(reader, false);
                    }

                    reader.ReadEndElement();
                    reader.Close();
                    return;
                case BodyState.Encrypted:
                case BodyState.SignedThenEncrypted:
                    throw ExceptionHelper.PlatformNotSupported();
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateBadStateException(nameof(OnWriteBodyContentsAsync)));
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

            Headers.Add(_securityHeader);

            InnerMessage.WriteStartEnvelope(writer);

            Headers.RemoveAt(Headers.Count - 1);

            _securityHeader.ApplyBodySecurity(writer, prefixGenerator);

            InnerMessage.WriteStartHeaders(writer);
            _securityHeader.ApplySecurityAndWriteHeaders(Headers, writer, prefixGenerator);

            _securityHeader.RemoveSignatureEncryptionIfAppropriate();

            _securityHeader.CompleteSecurityApplication();
            _securityHeader.WriteHeader(writer, Version);
            writer.WriteEndElement();

            if (_fullBodyFragment != null)
            {
                ((IFragmentCapableXmlDictionaryWriter)writer).WriteFragment(_fullBodyFragment, 0, _fullBodyFragmentLength);
            }
            else
            {
                if (_startBodyFragment != null)
                {
                    ((IFragmentCapableXmlDictionaryWriter)writer).WriteFragment(_startBodyFragment.GetBuffer(), 0, (int)_startBodyFragment.Length);
                }
                else
                {
                    OnWriteStartBody(writer);
                }

                OnWriteBodyContents(writer);

                if (_endBodyFragment != null)
                {
                    ((IFragmentCapableXmlDictionaryWriter)writer).WriteFragment(_endBodyFragment.GetBuffer(), 0, (int)_endBodyFragment.Length);
                }
                else
                {
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        public override async Task OnWriteMessageAsync(XmlDictionaryWriter writer)
        {
            // For Kerb one shot, the channel binding will be need to be fished out of the message, cached and added to the
            // token before calling ISC.

            AttachChannelBindingTokenIfFound();

            EnsureUniqueSecurityApplication();

            MessagePrefixGenerator prefixGenerator = new MessagePrefixGenerator(writer);
            _securityHeader.StartSecurityApplication();

            Headers.Add(_securityHeader);

            InnerMessage.WriteStartEnvelope(writer);

            Headers.RemoveAt(Headers.Count - 1);

            _securityHeader.ApplyBodySecurity(writer, prefixGenerator);

            InnerMessage.WriteStartHeaders(writer);
            _securityHeader.ApplySecurityAndWriteHeaders(Headers, writer, prefixGenerator);

            _securityHeader.RemoveSignatureEncryptionIfAppropriate();

            _securityHeader.CompleteSecurityApplication();
            _securityHeader.WriteHeader(writer, Version);
            await writer.WriteEndElementAsync();

            if (_fullBodyFragment != null)
            {
                ((IFragmentCapableXmlDictionaryWriter)writer).WriteFragment(_fullBodyFragment, 0, _fullBodyFragmentLength);
            }
            else
            {
                if (_startBodyFragment != null)
                {
                    ((IFragmentCapableXmlDictionaryWriter)writer).WriteFragment(_startBodyFragment.GetBuffer(), 0, (int)_startBodyFragment.Length);
                }
                else
                {
                    OnWriteStartBody(writer);
                }

                await OnWriteBodyContentsAsync(writer);

                if (_endBodyFragment != null)
                {
                    ((IFragmentCapableXmlDictionaryWriter)writer).WriteFragment(_endBodyFragment.GetBuffer(), 0, (int)_endBodyFragment.Length);
                }
                else
                {
                    writer.WriteEndElement();
                }
            }

            await writer.WriteEndElementAsync();
        }

        private void AttachChannelBindingTokenIfFound()
        {
            // Only valid when using a TokenParameter of type KerberosSecurityTokenParameters which isn't currently supported
        }

        private void SetBodyId()
        {
            BodyId = InnerMessage.GetBodyAttribute(
                UtilityStrings.IdAttribute,
                _securityHeader.StandardsManager.IdManager.DefaultIdNamespaceUri);
            if (BodyId == null)
            {
                BodyId = _securityHeader.GenerateId();
                _bodyIdInserted = true;
            }
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

        public void WriteBodyToSignWithFragments(Stream stream, bool includeComments, string[] inclusivePrefixes, XmlDictionaryWriter writer)
        {
            IFragmentCapableXmlDictionaryWriter fragmentingWriter = (IFragmentCapableXmlDictionaryWriter)writer;

            SetBodyId();
            BufferedOutputStream fullBodyFragment = new BufferManagerOutputStream(SRP.XmlBufferQuotaExceeded, 1024, int.MaxValue, _securityHeader.StreamBufferManager);
            writer.StartCanonicalization(stream, includeComments, inclusivePrefixes);
            fragmentingWriter.StartFragment(fullBodyFragment, false);
            WriteStartInnerMessageWithId(writer);
            InnerMessage.WriteBodyContents(writer);
            writer.WriteEndElement();
            fragmentingWriter.EndFragment();
            writer.EndCanonicalization();

            _fullBodyFragment = fullBodyFragment.ToArray(out _fullBodyFragmentLength);

            _state = BodyState.Signed;
        }

        private void WriteInnerMessageWithId(XmlDictionaryWriter writer)
        {
            WriteStartInnerMessageWithId(writer);
            InnerMessage.WriteBodyContents(writer);
            writer.WriteEndElement();
        }

        private void WriteStartInnerMessageWithId(XmlDictionaryWriter writer)
        {
            InnerMessage.WriteStartBody(writer);
            if (_bodyIdInserted)
            {
                _securityHeader.StandardsManager.IdManager.WriteIdAttribute(writer, BodyId);
            }
        }

        private enum BodyState
        {
            Created,
            Signed,
            SignedThenEncrypted,
            EncryptedThenSigned,
            Encrypted,
            Disposed,
        }

        private sealed class MessagePrefixGenerator : IPrefixGenerator
        {
            private XmlWriter _writer;

            public MessagePrefixGenerator(XmlWriter writer)
            {
                _writer = writer;
            }

            public string GetPrefix(string namespaceUri, int depth, bool isForAttribute)
            {
                return _writer.LookupPrefix(namespaceUri);
            }
        }
    }
}
