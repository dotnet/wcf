// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel;
using System.Security.Cryptography.Xml;
using System.Text;
using System.IdentityModel.Tokens;
using System.IO;
using System.Runtime;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.Xml;
using IPrefixGenerator = System.IdentityModel.IPrefixGenerator;
using ISecurityElement = System.IdentityModel.ISecurityElement;
using ISignatureValueSecurityElement = System.IdentityModel.ISignatureValueSecurityElement;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
    internal class WSSecurityOneDotZeroSendSecurityHeader : SendSecurityHeader
    {
        private HashStream _hashStream;
        private SignedXml _signedXml;

        private KeyedHashAlgorithm _signingKey;
        private MessagePartSpecification _effectiveSignatureParts;

        // For Transport Security we have to sign the 'To' header with the 
        // supporting tokens.
        private Stream _toHeaderStream = null;
        private string _toHeaderId = null;

        public WSSecurityOneDotZeroSendSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite,
            MessageDirection direction)
            : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, direction)
        {
        }

        protected string EncryptionAlgorithm
        {
            get { return AlgorithmSuite.DefaultEncryptionAlgorithm; }
        }

        protected XmlDictionaryString EncryptionAlgorithmDictionaryString
        {
            get { return AlgorithmSuite.DefaultEncryptionAlgorithmDictionaryString; }
        }

        private void AddEncryptionReference(MessageHeader header, string headerId, IPrefixGenerator prefixGenerator, bool sign,
            out MemoryStream plainTextStream, out string encryptedDataId)
        {
            throw new PlatformNotSupportedException();
        }

        private void AddSignatureReference(SecurityToken token, int position, SecurityTokenAttachmentMode mode)
        {
            SecurityKeyIdentifierClause keyIdentifierClause = null;
            bool strTransformEnabled = ShouldUseStrTransformForToken(token, position, mode, out keyIdentifierClause);
            AddTokenSignatureReference(token, keyIdentifierClause, strTransformEnabled);
        }

        private void AddPrimaryTokenSignatureReference(SecurityToken token, SecurityTokenParameters securityTokenParameters)
        {
            return;
        }

        // Given a token and useStarTransform value this method adds apporopriate reference accordingly.
        // 1. If strTransform is disabled, it adds a reference to the token's id. 
        // 2. Else if strtransform is enabled it adds a reference the security token's keyIdentifier's id.
        private void AddTokenSignatureReference(SecurityToken token, SecurityKeyIdentifierClause keyIdentifierClause, bool strTransformEnabled)
        {
            throw new PlatformNotSupportedException();
        }

        private void AddSignatureReference(SendSecurityHeaderElement[] elements)
        {
            if (elements != null)
            {
                for (int i = 0; i < elements.Length; ++i)
                {
                    SecurityKeyIdentifierClause keyIdentifierClause = null;
                    TokenElement signedEncryptedTokenElement = elements[i].Item as TokenElement;

                    // signedEncryptedTokenElement can either be a TokenElement ( in SignThenEncrypt case) or EncryptedData ( in !SignThenEncryptCase)
                    // STR-Transform does not make sense in !SignThenEncrypt case .
                    // note: signedEncryptedTokenElement can also be SignatureConfirmation but we do not care about it here.
                    bool useStrTransform = signedEncryptedTokenElement != null
                                           && SignThenEncrypt
                                           && ShouldUseStrTransformForToken(signedEncryptedTokenElement.Token,
                                                                                 i,
                                                                                 SecurityTokenAttachmentMode.SignedEncrypted,
                                                                                 out keyIdentifierClause);

                    if (!useStrTransform && elements[i].Id == null)
                    {
                        throw TraceUtility.ThrowHelperError(new MessageSecurityException(SRP.ElementToSignMustHaveId), Message);
                    }

                    MemoryStream stream = new MemoryStream();
                    XmlDictionaryWriter utf8Writer = TakeUtf8Writer();
                    utf8Writer.StartCanonicalization(stream, false, null);
                    elements[i].Item.WriteTo(utf8Writer, ServiceModelDictionaryManager.Instance);
                    utf8Writer.EndCanonicalization();
                    stream.Position = 0;
                    if (useStrTransform)
                    {
                        throw new PlatformNotSupportedException("StrTransform not supported yet");
                    }
                    else
                    {
                        AddReference("#" + elements[i].Id, stream);
                    }
                }
            }
        }

        private void AddSignatureReference(SecurityToken[] tokens, SecurityTokenAttachmentMode mode)
        {
            if (tokens != null)
            {
                for (int i = 0; i < tokens.Length; ++i)
                {
                    AddSignatureReference(tokens[i], i, mode);
                }
            }
        }

        private string GetSignatureHash(MessageHeader header, string headerId, IPrefixGenerator prefixGenerator, XmlDictionaryWriter writer, out byte[] hash)
        {
            HashStream hashStream = TakeHashStream();
            XmlDictionaryWriter effectiveWriter;
            XmlBuffer canonicalBuffer = null;

            if (writer.CanCanonicalize)
            {
                effectiveWriter = writer;
            }
            else
            {
                canonicalBuffer = new XmlBuffer(int.MaxValue);
                effectiveWriter = canonicalBuffer.OpenSection(XmlDictionaryReaderQuotas.Max);
            }

            effectiveWriter.StartCanonicalization(hashStream, false, null);

            header.WriteStartHeader(effectiveWriter, Version);
            if (headerId == null)
            {
                headerId = GenerateId();
                StandardsManager.IdManager.WriteIdAttribute(effectiveWriter, headerId);
            }
            header.WriteHeaderContents(effectiveWriter, Version);
            effectiveWriter.WriteEndElement();
            effectiveWriter.EndCanonicalization();
            effectiveWriter.Flush();

            if (!ReferenceEquals(effectiveWriter, writer))
            {
                Fx.Assert(canonicalBuffer != null, "Canonical buffer cannot be null.");
                canonicalBuffer.CloseSection();
                canonicalBuffer.Close();
                XmlDictionaryReader dicReader = canonicalBuffer.GetReader(0);
                writer.WriteNode(dicReader, false);
                dicReader.Close();
            }

            hash = hashStream.FlushHashAndGetValue();

            return headerId;
        }

        private string GetSignatureStream(MessageHeader header, string headerId, IPrefixGenerator prefixGenerator, XmlDictionaryWriter writer, out Stream stream)
        {
            stream = new MemoryStream();
            XmlDictionaryWriter effectiveWriter;
            XmlBuffer canonicalBuffer = null;

            if (writer.CanCanonicalize)
            {
                effectiveWriter = writer;
            }
            else
            {
                canonicalBuffer = new XmlBuffer(int.MaxValue);
                effectiveWriter = canonicalBuffer.OpenSection(XmlDictionaryReaderQuotas.Max);
            }

            effectiveWriter.StartCanonicalization(stream, false, null);

            header.WriteStartHeader(effectiveWriter, Version);
            if (headerId == null)
            {
                headerId = GenerateId();
                StandardsManager.IdManager.WriteIdAttribute(effectiveWriter, headerId);
            }
            header.WriteHeaderContents(effectiveWriter, Version);
            effectiveWriter.WriteEndElement();
            effectiveWriter.EndCanonicalization();
            effectiveWriter.Flush();

            if (!ReferenceEquals(effectiveWriter, writer))
            {
                Fx.Assert(canonicalBuffer != null, "Canonical buffer cannot be null.");
                canonicalBuffer.CloseSection();
                canonicalBuffer.Close();
                XmlDictionaryReader dicReader = canonicalBuffer.GetReader(0);
                writer.WriteNode(dicReader, false);
                dicReader.Close();
            }

            stream.Position = 0;

            return headerId;
        }

        private void AddReference(string id, Stream contents)
        {
            var reference = new Reference(contents);
            reference.Uri = id;
            reference.DigestMethod = AlgorithmSuite.DefaultDigestAlgorithm;
            reference.AddTransform(new XmlDsigExcC14NTransform());
            _signedXml.AddReference(reference);
        }

        private void AddSignatureReference(MessageHeader header, string headerId, IPrefixGenerator prefixGenerator, XmlDictionaryWriter writer)
        {
            // No transforms added to Reference as the digest value has already been calculated
            byte[] hashValue;
            headerId = GetSignatureHash(header, headerId, prefixGenerator, writer, out hashValue);
            var reference = new Reference();
            reference.DigestMethod = AlgorithmSuite.DefaultDigestAlgorithm;
            reference.DigestValue = hashValue;
            reference.Id = headerId;
            _signedXml.AddReference(reference);
        }

        private void ApplySecurityAndWriteHeader(MessageHeader header, string headerId, XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator)
        {
            if (!RequireMessageProtection && ShouldSignToHeader)
            {
                if ((header.Name == XD.AddressingDictionary.To.Value) &&
                    (header.Namespace == Message.Version.Addressing.Namespace))
                {
                    if (_toHeaderStream == null)
                    {
                        Stream headerStream;
                        headerId = GetSignatureStream(header, headerId, prefixGenerator, writer, out headerStream);
                        _toHeaderStream = headerStream;
                        _toHeaderId = headerId;
                    }
                    else
                    {
                        // More than one 'To' header is specified in the message.
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.TransportSecuredMessageHasMoreThanOneToHeader));
                    }

                    return;
                }
            }

            MessagePartProtectionMode protectionMode = GetProtectionMode(header);
            switch (protectionMode)
            {
                case MessagePartProtectionMode.None:
                    header.WriteHeader(writer, Version);
                    return;
                case MessagePartProtectionMode.Sign:
                    AddSignatureReference(header, headerId, prefixGenerator, writer);
                    return;
                case MessagePartProtectionMode.SignThenEncrypt:
                case MessagePartProtectionMode.Encrypt:
                case MessagePartProtectionMode.EncryptThenSign:
                    throw ExceptionHelper.PlatformNotSupported();
                default:
                    Fx.Assert("Invalid MessagePartProtectionMode");
                    return;
            }
        }

        public override void ApplySecurityAndWriteHeaders(MessageHeaders headers, XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator)
        {
            string[] headerIds;
            if (RequireMessageProtection || ShouldSignToHeader)
            {
                headerIds = headers.GetHeaderAttributes(UtilityStrings.IdAttribute,
                    StandardsManager.IdManager.DefaultIdNamespaceUri);
            }
            else
            {
                headerIds = null;
            }
            for (int i = 0; i < headers.Count; i++)
            {
                MessageHeader header = headers.GetMessageHeader(i);
                if (Version.Addressing == AddressingVersion.None && header.Namespace == AddressingVersion.None.Namespace)
                {
                    continue;
                }

                if (header != this)
                {
                    ApplySecurityAndWriteHeader(header, headerIds == null ? null : headerIds[i], writer, prefixGenerator);
                }
            }
        }

        private static bool CanCanonicalizeAndFragment(XmlDictionaryWriter writer)
        {
            if (!writer.CanCanonicalize)
            {
                return false;
            }
            IFragmentCapableXmlDictionaryWriter fragmentingWriter = writer as IFragmentCapableXmlDictionaryWriter;
            return fragmentingWriter != null && fragmentingWriter.CanFragment;
        }

        public override void ApplyBodySecurity(XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator)
        {
            SecurityAppliedMessage message = SecurityAppliedMessage;
            switch (message.BodyProtectionMode)
            {
                case MessagePartProtectionMode.None:
                    return;
                case MessagePartProtectionMode.Sign:
                    var ms = new MemoryStream();
                    if (CanCanonicalizeAndFragment(writer))
                    {
                        message.WriteBodyToSignWithFragments(ms, false, null, writer);
                    }
                    else
                    {
                        message.WriteBodyToSign(ms);
                    }

                    ms.Position = 0;
                    AddReference("#" + message.BodyId, ms);
                    return;
                case MessagePartProtectionMode.SignThenEncrypt:
                    throw new PlatformNotSupportedException();
                case MessagePartProtectionMode.Encrypt:
                    throw new PlatformNotSupportedException();
                case MessagePartProtectionMode.EncryptThenSign:
                    throw new PlatformNotSupportedException();
                default:
                    Fx.Assert("Invalid MessagePartProtectionMode");
                    return;
            }
        }

        protected override ISignatureValueSecurityElement CompletePrimarySignatureCore(
            SendSecurityHeaderElement[] signatureConfirmations,
            SecurityToken[] signedEndorsingTokens,
            SecurityToken[] signedTokens,
            SendSecurityHeaderElement[] basicTokens, bool isPrimarySignature)
        {
            if (_signedXml == null)
            {
                return null;
            }

            SecurityTimestamp timestamp = Timestamp;
            if (timestamp != null)
            {
                if (timestamp.Id == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.TimestampToSignHasNoId));
                }

                var buffer = new byte[64];
                var ms = new MemoryStream();
                StandardsManager.WSUtilitySpecificationVersion.WriteTimestampCanonicalForm(
                    ms, timestamp, buffer);
                ms.Position = 0;
                AddReference("#" + timestamp.Id, ms);
                var reference = new Reference(ms);
            }

            if ((ShouldSignToHeader) && (_signingKey != null || _signedXml.SigningKey != null) && (Version.Addressing != AddressingVersion.None))
            {
                if (_toHeaderStream != null)
                {
                    AddReference("#" + _toHeaderId, _toHeaderStream);
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.TransportSecurityRequireToHeader));
                }
            }

            AddSignatureReference(signatureConfirmations);
            if (isPrimarySignature && ShouldProtectTokens)
            {
                AddPrimaryTokenSignatureReference(ElementContainer.SourceSigningToken, SigningTokenParameters);
            }

            if (RequireMessageProtection)
            {
                throw new PlatformNotSupportedException(nameof(RequireMessageProtection));
            }

            if (_signedXml.SignedInfo.References.Count == 0)
            {
                throw TraceUtility.ThrowHelperError(new MessageSecurityException(SRP.NoPartsOfMessageMatchedPartsToSign), Message);
            }
            try
            {
                if (_signingKey != null)
                {
                    _signedXml.ComputeSignature(_signingKey);
                }
                else
                {
                    _signedXml.ComputeSignature();
                }

                return new SignatureValue(_signedXml.Signature);
            }
            finally
            {
                _hashStream = null;
                _signingKey = null;
                _signedXml = null;
                _effectiveSignatureParts = null;
            }
        }

        internal class SignatureValue : ISignatureValueSecurityElement
        {
            private Signature _signature;

            public SignatureValue(Signature signature)
            {
                _signature = signature;
            }

            public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
            {
                _signature.GetXml().WriteTo(writer);
            }

            public bool HasId
            {
                get { return true; }
            }

            public string Id
            {
                get { return _signature.Id; }
            }

            public byte[] GetSignatureValue()
            {
                return _signature.SignatureValue;
            }
        }

        private HashStream TakeHashStream()
        {
            HashStream hashStream = null;
            if (_hashStream == null)
            {
                _hashStream = hashStream = new HashStream(CryptoHelper.CreateHashAlgorithm(AlgorithmSuite.DefaultDigestAlgorithm));
            }
            else
            {
                hashStream = _hashStream;
                ;
                hashStream.Reset();
            }
            return hashStream;
        }

        private XmlDictionaryWriter TakeUtf8Writer()
        {
            throw new PlatformNotSupportedException();
        }

        private MessagePartProtectionMode GetProtectionMode(MessageHeader header)
        {
            if (!RequireMessageProtection)
            {
                return MessagePartProtectionMode.None;
            }
            bool sign = _signedXml != null && _effectiveSignatureParts.IsHeaderIncluded(header);
            bool encrypt = false;
            return MessagePartProtectionModeHelper.GetProtectionMode(sign, encrypt, SignThenEncrypt);
        }

        protected override void StartPrimarySignatureCore(SecurityToken token,
            SecurityKeyIdentifier keyIdentifier,
            MessagePartSpecification signatureParts,
            bool generateTargettableSignature)
        {
            SecurityAlgorithmSuite suite = AlgorithmSuite;
            string canonicalizationAlgorithm = suite.DefaultCanonicalizationAlgorithm;
            if (canonicalizationAlgorithm != SecurityAlgorithms.ExclusiveC14n)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new MessageSecurityException(SRP.Format(SRP.UnsupportedCanonicalizationAlgorithm, suite.DefaultCanonicalizationAlgorithm)));
            }
            string signatureAlgorithm;
            XmlDictionaryString signatureAlgorithmDictionaryString;
            SecurityKey signatureKey;
            suite.GetSignatureAlgorithmAndKey(token, out signatureAlgorithm, out signatureKey, out signatureAlgorithmDictionaryString);
            AsymmetricAlgorithm asymmetricAlgorithm = null;
            GetSigningAlgorithm(signatureKey, signatureAlgorithm, out _signingKey, out asymmetricAlgorithm);

            _signedXml = new SignedXml();
            _signedXml.SignedInfo.CanonicalizationMethod = canonicalizationAlgorithm;
            _signedXml.SignedInfo.SignatureMethod = signatureAlgorithm;
            _signedXml.SigningKey = asymmetricAlgorithm;
            if (keyIdentifier != null)
            {
                var stream = new MemoryStream();
                using (var xmlWriter = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
                {
                    StandardsManager.SecurityTokenSerializer.WriteKeyIdentifier(xmlWriter, keyIdentifier);
                }

                stream.Position = 0;
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);
                var keyInfo = new KeyInfo();
                keyInfo.LoadXml(doc.DocumentElement);
                _signedXml.KeyInfo = keyInfo;
            }

            if (generateTargettableSignature)
            {
                _signedXml.Signature.Id = GenerateId();
            }
            _effectiveSignatureParts = signatureParts;
        }

        private void GetSigningAlgorithm(SecurityKey signatureKey, string algorithmName, out KeyedHashAlgorithm symmetricAlgorithm, out AsymmetricAlgorithm asymmetricAlgorithm)
        {
            symmetricAlgorithm = null;
            asymmetricAlgorithm = null;
            SymmetricSecurityKey symmetricKey = signatureKey as SymmetricSecurityKey;
            if (symmetricKey != null)
            {
                _signingKey = symmetricKey.GetKeyedHashAlgorithm(algorithmName);
                if (_signingKey == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        SRP.Format(SRP.UnableToCreateKeyedHashAlgorithm, symmetricKey, algorithmName)));
                }
            }
            else
            {
                AsymmetricSecurityKey asymmetricKey = signatureKey as AsymmetricSecurityKey;
                if (asymmetricKey == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        SRP.Format(SRP.UnknownICryptoType, _signingKey)));
                }

                asymmetricAlgorithm = asymmetricKey.GetAsymmetricAlgorithm(algorithmName, privateKey: true);
                if (asymmetricAlgorithm == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        SRP.Format(SRP.UnableToCreateHashAlgorithmFromAsymmetricCrypto, algorithmName,
                            asymmetricKey)));
                }
            }
        }

        protected override ISignatureValueSecurityElement CreateSupportingSignature(SecurityToken token, SecurityKeyIdentifier identifier)
        {
            StartPrimarySignatureCore(token, identifier, MessagePartSpecification.NoParts, false);
            return CompletePrimarySignatureCore(null, null, null, null, false);
        }

        protected override ISignatureValueSecurityElement CreateSupportingSignature(SecurityToken token, SecurityKeyIdentifier identifier, ISecurityElement elementToSign)
        {
            string signatureAlgorithm;
            XmlDictionaryString signatureAlgorithmDictionaryString;
            SecurityKey signatureKey;
            AlgorithmSuite.GetSignatureAlgorithmAndKey(token, out signatureAlgorithm, out signatureKey, out signatureAlgorithmDictionaryString);

            SignedXml signedXml = new SignedXml();
            SignedInfo signedInfo = signedXml.SignedInfo;
            signedInfo.CanonicalizationMethod = AlgorithmSuite.DefaultCanonicalizationAlgorithm;
            signedInfo.SignatureMethod = signatureAlgorithm;

            if (elementToSign.Id == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.ElementToSignMustHaveId));
            }

            MemoryStream stream = new MemoryStream();
            XmlDictionaryWriter utf8Writer = TakeUtf8Writer();
            utf8Writer.StartCanonicalization(stream, false, null);
            elementToSign.WriteTo(utf8Writer, ServiceModelDictionaryManager.Instance);
            utf8Writer.EndCanonicalization();
            stream.Position = 0;
            AddReference("#" + elementToSign.Id, stream);

            AsymmetricAlgorithm asymmetricAlgorithm = null;
            KeyedHashAlgorithm keyedHashAlgorithm = null;
            GetSigningAlgorithm(signatureKey, signatureAlgorithm, out keyedHashAlgorithm, out asymmetricAlgorithm);
            if (keyedHashAlgorithm != null)
            {
                signedXml.ComputeSignature(keyedHashAlgorithm);
            }
            else
            {
                signedXml.SigningKey = asymmetricAlgorithm;
                signedXml.ComputeSignature();
            }

            SetKeyInfo(signedXml, identifier);
            return new SignatureValue(signedXml.Signature);
        }

        private void SetKeyInfo(SignedXml signedXml, SecurityKeyIdentifier identifier)
        {
            if (identifier != null)
            {
                var stream = new MemoryStream();
                using (var xmlWriter = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
                {
                    StandardsManager.SecurityTokenSerializer.WriteKeyIdentifier(xmlWriter, identifier);
                }

                stream.Position = 0;
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);
                var keyInfo = new KeyInfo();
                keyInfo.LoadXml(doc.DocumentElement);
                signedXml.KeyInfo = keyInfo;
            }
        }

        protected override void WriteSecurityTokenReferencyEntry(XmlDictionaryWriter writer, SecurityToken securityToken, SecurityTokenParameters securityTokenParameters)
        {
            return;
        }
    }

    internal class WrappedXmlDictionaryWriter : XmlDictionaryWriter
    {
        private XmlDictionaryWriter _innerWriter;
        private int _index;
        private bool _insertId;
        private bool _isStrReferenceElement;
        private string _id;

        public WrappedXmlDictionaryWriter(XmlDictionaryWriter writer, string id)
        {
            _innerWriter = writer;
            _index = 0;
            _insertId = false;
            _isStrReferenceElement = false;
            _id = id;
        }

        public override void WriteStartAttribute(string prefix, string localName, string namespaceUri)
        {
            if (_isStrReferenceElement && _insertId && localName == XD.UtilityDictionary.IdAttribute.Value)
            {
                // This means the serializer is already writing the Id out, so we don't write it again.
                _insertId = false;
            }
            _innerWriter.WriteStartAttribute(prefix, localName, namespaceUri);
        }

        public override void WriteStartElement(string prefix, string localName, string namespaceUri)
        {
            if (_isStrReferenceElement && _insertId)
            {
                if (_id != null)
                {
                    _innerWriter.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, _id);
                }

                _isStrReferenceElement = false;
                _insertId = false;
            }

            _index++;

            if (_index == 1 && localName == XD.SecurityJan2004Dictionary.SecurityTokenReference.Value)
            {
                _insertId = true;
                _isStrReferenceElement = true;
            }

            _innerWriter.WriteStartElement(prefix, localName, namespaceUri);
        }

        // Below methods simply call into innerWritter
        public override void Close()
        {
            _innerWriter.Close();
        }

        public override void Flush()
        {
            _innerWriter.Flush();
        }

        public override string LookupPrefix(string ns)
        {
            return _innerWriter.LookupPrefix(ns);
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            _innerWriter.WriteBase64(buffer, index, count);
        }

        public override void WriteCData(string text)
        {
            _innerWriter.WriteCData(text);
        }

        public override void WriteCharEntity(char ch)
        {
            _innerWriter.WriteCharEntity(ch);
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            _innerWriter.WriteChars(buffer, index, count);
        }

        public override void WriteComment(string text)
        {
            _innerWriter.WriteComment(text);
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            _innerWriter.WriteDocType(name, pubid, sysid, subset);
        }

        public override void WriteEndAttribute()
        {
            _innerWriter.WriteEndAttribute();
        }

        public override void WriteEndDocument()
        {
            _innerWriter.WriteEndDocument();
        }

        public override void WriteEndElement()
        {
            _innerWriter.WriteEndElement();
        }

        public override void WriteEntityRef(string name)
        {
            _innerWriter.WriteEntityRef(name);
        }

        public override void WriteFullEndElement()
        {
            _innerWriter.WriteFullEndElement();
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            _innerWriter.WriteProcessingInstruction(name, text);
        }

        public override void WriteRaw(string data)
        {
            _innerWriter.WriteRaw(data);
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            _innerWriter.WriteRaw(buffer, index, count);
        }

        public override void WriteStartDocument(bool standalone)
        {
            _innerWriter.WriteStartDocument(standalone);
        }

        public override void WriteStartDocument()
        {
            _innerWriter.WriteStartDocument();
        }

        public override WriteState WriteState
        {
            get { return _innerWriter.WriteState; }
        }

        public override void WriteString(string text)
        {
            _innerWriter.WriteString(text);
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            _innerWriter.WriteSurrogateCharEntity(lowChar, highChar);
        }

        public override void WriteWhitespace(string ws)
        {
            _innerWriter.WriteWhitespace(ws);
        }
    }
}

