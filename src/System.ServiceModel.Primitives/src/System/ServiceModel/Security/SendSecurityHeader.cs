// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security.Tokens;
using System.Xml;

using IPrefixGenerator = System.IdentityModel.IPrefixGenerator;

namespace System.ServiceModel.Security
{
    internal abstract class SendSecurityHeader : SecurityHeader, IMessageHeaderWithSharedNamespace
    {
#pragma warning disable CS0649 // Field is never assign to
        private bool _encryptSignature;
        private bool _primarySignatureDone;
        private SignatureConfirmations _signatureValuesGenerated;
        private SignatureConfirmations _signatureConfirmationsToSend;
        private int _idCounter;
        private string _idPrefix;
        private MessagePartSpecification _signatureParts;
        private List<SecurityTokenParameters> _basicSupportingTokenParameters = null;
        private List<SecurityTokenParameters> _endorsingTokenParameters = null;
        private List<SecurityTokenParameters> _signedEndorsingTokenParameters = null;
        private List<SecurityTokenParameters> _signedTokenParameters = null;
        private byte[] _primarySignatureValue = null;
        private bool _shouldProtectTokens;
        private BufferManager _bufferManager;
        private SecurityProtocolCorrelationState _correlationState;
        private bool _signThenEncrypt = true;
#pragma warning restore CS0649 // Field is never assign to
        private static readonly string[] s_ids = new string[] { "_0", "_1", "_2", "_3", "_4", "_5", "_6", "_7", "_8", "_9" };

        protected SendSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite,
            MessageDirection transferDirection)
            : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, transferDirection)
        {
            ElementContainer = new SendSecurityHeaderElementContainer();
        }

        public SendSecurityHeaderElementContainer ElementContainer { get; }

        public BufferManager StreamBufferManager
        {
            get
            {
                if (_bufferManager == null)
                {
                    _bufferManager = BufferManager.CreateBufferManager(0, int.MaxValue);
                }

                return _bufferManager;
            }
            set
            {
                _bufferManager = value;
            }
        }

        protected SecurityAppliedMessage SecurityAppliedMessage
        {
            get { return (SecurityAppliedMessage)Message; }
        }

        public bool SignThenEncrypt
        {
            get { return _signThenEncrypt; }
            set
            {
                ThrowIfProcessingStarted();
                _signThenEncrypt = value;
            }
        }

        public bool ShouldProtectTokens
        {
            get { return _shouldProtectTokens; }
            set
            {
                ThrowIfProcessingStarted();
                _shouldProtectTokens = value;
            }
        }

        public void AddBasicSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
        {
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(token));
            }

            if (parameters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(parameters));
            }

            ThrowIfProcessingStarted();
            SendSecurityHeaderElement tokenElement = new SendSecurityHeaderElement(token.Id, new TokenElement(token, StandardsManager));
            tokenElement.MarkedForEncryption = true;
            ElementContainer.AddBasicSupportingToken(tokenElement);
            AddParameters(ref _basicSupportingTokenParameters, parameters);
        }

        public void AddEndorsingSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
        {
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(token));
            }

            if (parameters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(parameters));
            }

            ThrowIfProcessingStarted();
            ElementContainer.AddEndorsingSupportingToken(token);
            ShouldSignToHeader |= (!RequireMessageProtection) && (SecurityUtils.GetSecurityKey<AsymmetricSecurityKey>(token) != null);
            AddParameters(ref _endorsingTokenParameters, parameters);
        }

        public void AddSignedEndorsingSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
        {
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(token));
            }

            if (parameters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(parameters));
            }

            ThrowIfProcessingStarted();
            ElementContainer.AddSignedEndorsingSupportingToken(token);
            AddParameters(ref _signedEndorsingTokenParameters, parameters);
        }

        public void AddSignedSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
        {
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(token));
            }

            if (parameters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(parameters));
            }

            ThrowIfProcessingStarted();
            ElementContainer.AddSignedSupportingToken(token);
            AddParameters(ref _signedTokenParameters, parameters);
        }

        public bool EncryptPrimarySignature
        {
            get { return _encryptSignature; }
            set
            {
                ThrowIfProcessingStarted();
                if (value)
                {
                    throw ExceptionHelper.PlatformNotSupported();
                }

                _encryptSignature = value;
            }
        }

        protected bool ShouldUseStrTransformForToken(SecurityToken securityToken, int position, SecurityTokenAttachmentMode mode, out SecurityKeyIdentifierClause keyIdentifierClause)
        {
            keyIdentifierClause = null;
            return false;
        }

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedNamespace
        {
            get { return XD.UtilityDictionary.Namespace; }
        }

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedPrefix
        {
            get { return XD.UtilityDictionary.Prefix; }
        }

        public string IdPrefix
        {
            get { return _idPrefix; }
            set
            {
                ThrowIfProcessingStarted();
                _idPrefix = string.IsNullOrEmpty(value) || value == "_" ? null : value;
            }
        }

        protected internal SecurityTokenParameters SigningTokenParameters { get; }

        protected bool ShouldSignToHeader { get; private set; } = false;

        public override string Name
        {
            get { return StandardsManager.SecurityVersion.HeaderName.Value; }
        }

        public override string Namespace
        {
            get { return StandardsManager.SecurityVersion.HeaderNamespace.Value; }
        }

        public SecurityTimestamp Timestamp
        {
            get { return ElementContainer.Timestamp; }
        }

        private void AddParameters(ref List<SecurityTokenParameters> list, SecurityTokenParameters item)
        {
            if (list == null)
            {
                list = new List<SecurityTokenParameters>();
            }
            list.Add(item);
        }

        public abstract void ApplyBodySecurity(XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator);

        public abstract void ApplySecurityAndWriteHeaders(MessageHeaders headers, XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator);

        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            StandardsManager.SecurityVersion.WriteStartHeader(writer);
            WriteHeaderAttributes(writer, messageVersion);
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            if (ElementContainer.Timestamp != null && Layout != SecurityHeaderLayout.LaxTimestampLast)
            {
                StandardsManager.WSUtilitySpecificationVersion.WriteTimestamp(writer, ElementContainer.Timestamp);
            }
            if (ElementContainer.PrerequisiteToken != null)
            {
                StandardsManager.SecurityTokenSerializer.WriteToken(writer, ElementContainer.PrerequisiteToken);
            }
            if (ElementContainer.SourceSigningToken != null)
            {
                if (ShouldSerializeToken(SigningTokenParameters, MessageDirection))
                {
                    StandardsManager.SecurityTokenSerializer.WriteToken(writer, ElementContainer.SourceSigningToken);

                    // Implement Protect token 
                    // NOTE: The spec says sign the primary token if it is not included in the message. But we currently are not supporting it
                    // as we do not support STR-Transform for external references. Hence we can not sign the token which is external ie not in the message.
                    // This only affects the messages from service to client where 
                    // 1. allowSerializedSigningTokenOnReply is false.
                    // 2. SymmetricSecurityBindingElement with IssuedTokens binding where the issued token has a symmetric key.

                    if (ShouldProtectTokens)
                    {
                        WriteSecurityTokenReferencyEntry(writer, ElementContainer.SourceSigningToken, SigningTokenParameters);
                    }
                }
            }
            if (ElementContainer.DerivedSigningToken != null)
            {
                StandardsManager.SecurityTokenSerializer.WriteToken(writer, ElementContainer.DerivedSigningToken);
            }

            if (ElementContainer.WrappedEncryptionToken != null)
            {
                StandardsManager.SecurityTokenSerializer.WriteToken(writer, ElementContainer.WrappedEncryptionToken);
            }

            if (ElementContainer.DerivedEncryptionToken != null)
            {
                StandardsManager.SecurityTokenSerializer.WriteToken(writer, ElementContainer.DerivedEncryptionToken);
            }

            if (SignThenEncrypt)
            {
                if (ElementContainer.ReferenceList != null)
                {
                    ElementContainer.ReferenceList.WriteTo(writer, ServiceModelDictionaryManager.Instance);
                }
            }

            SecurityToken[] signedTokens = ElementContainer.GetSignedSupportingTokens();
            if (signedTokens != null)
            {
                for (int i = 0; i < signedTokens.Length; ++i)
                {
                    StandardsManager.SecurityTokenSerializer.WriteToken(writer, signedTokens[i]);
                    WriteSecurityTokenReferencyEntry(writer, signedTokens[i], _signedTokenParameters[i]);
                }
            }
            SendSecurityHeaderElement[] basicTokensXml = ElementContainer.GetBasicSupportingTokens();
            if (basicTokensXml != null)
            {
                for (int i = 0; i < basicTokensXml.Length; ++i)
                {
                    basicTokensXml[i].Item.WriteTo(writer, ServiceModelDictionaryManager.Instance);
                    if (SignThenEncrypt)
                    {
                        WriteSecurityTokenReferencyEntry(writer, null, _basicSupportingTokenParameters[i]);
                    }
                }
            }
            SecurityToken[] endorsingTokens = ElementContainer.GetEndorsingSupportingTokens();
            if (endorsingTokens != null)
            {
                for (int i = 0; i < endorsingTokens.Length; ++i)
                {
                    if (ShouldSerializeToken(_endorsingTokenParameters[i], MessageDirection))
                    {
                        StandardsManager.SecurityTokenSerializer.WriteToken(writer, endorsingTokens[i]);
                    }
                }
            }
            SecurityToken[] endorsingDerivedTokens = ElementContainer.GetEndorsingDerivedSupportingTokens();
            if (endorsingDerivedTokens != null)
            {
                for (int i = 0; i < endorsingDerivedTokens.Length; ++i)
                {
                    StandardsManager.SecurityTokenSerializer.WriteToken(writer, endorsingDerivedTokens[i]);
                }
            }
            SecurityToken[] signedEndorsingTokens = ElementContainer.GetSignedEndorsingSupportingTokens();
            if (signedEndorsingTokens != null)
            {
                for (int i = 0; i < signedEndorsingTokens.Length; ++i)
                {
                    StandardsManager.SecurityTokenSerializer.WriteToken(writer, signedEndorsingTokens[i]);
                    WriteSecurityTokenReferencyEntry(writer, signedEndorsingTokens[i], _signedEndorsingTokenParameters[i]);
                }
            }
            SecurityToken[] signedEndorsingDerivedTokens = ElementContainer.GetSignedEndorsingDerivedSupportingTokens();
            if (signedEndorsingDerivedTokens != null)
            {
                for (int i = 0; i < signedEndorsingDerivedTokens.Length; ++i)
                {
                    StandardsManager.SecurityTokenSerializer.WriteToken(writer, signedEndorsingDerivedTokens[i]);
                }
            }
            SendSecurityHeaderElement[] signatureConfirmations = ElementContainer.GetSignatureConfirmations();
            if (signatureConfirmations != null)
            {
                for (int i = 0; i < signatureConfirmations.Length; ++i)
                {
                    signatureConfirmations[i].Item.WriteTo(writer, ServiceModelDictionaryManager.Instance);
                }
            }
            if (ElementContainer.PrimarySignature != null && ElementContainer.PrimarySignature.Item != null)
            {
                ElementContainer.PrimarySignature.Item.WriteTo(writer, ServiceModelDictionaryManager.Instance);
            }
            SendSecurityHeaderElement[] endorsingSignatures = ElementContainer.GetEndorsingSignatures();
            if (endorsingSignatures != null)
            {
                for (int i = 0; i < endorsingSignatures.Length; ++i)
                {
                    endorsingSignatures[i].Item.WriteTo(writer, ServiceModelDictionaryManager.Instance);
                }
            }
            if (!SignThenEncrypt)
            {
                if (ElementContainer.ReferenceList != null)
                {
                    ElementContainer.ReferenceList.WriteTo(writer, ServiceModelDictionaryManager.Instance);
                }
            }
            if (ElementContainer.Timestamp != null && Layout == SecurityHeaderLayout.LaxTimestampLast)
            {
                StandardsManager.WSUtilitySpecificationVersion.WriteTimestamp(writer, ElementContainer.Timestamp);
            }
        }

        public void AddTimestamp(TimeSpan timestampValidityDuration)
        {
            DateTime now = DateTime.UtcNow;
            string id = RequireMessageProtection ? SecurityUtils.GenerateId() : GenerateId();
            AddTimestamp(new SecurityTimestamp(now, now + timestampValidityDuration, id));
        }

        public void AddTimestamp(SecurityTimestamp timestamp)
        {
            ThrowIfProcessingStarted();
            if (ElementContainer.Timestamp != null)
            {
                throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.TimestampAlreadySetForSecurityHeader), Message);
            }

            ElementContainer.Timestamp = timestamp ?? throw TraceUtility.ThrowHelperArgumentNull(nameof(timestamp), Message);
        }

        protected abstract void WriteSecurityTokenReferencyEntry(XmlDictionaryWriter writer, SecurityToken securityToken, SecurityTokenParameters securityTokenParameters);

        public Message SetupExecution()
        {
            ThrowIfProcessingStarted();
            SetProcessingStarted();

            bool signBody = false;
            if (ElementContainer.SourceSigningToken != null)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            bool encryptBody = false;
            if (ElementContainer.SourceEncryptionToken != null)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            SecurityAppliedMessage message = new SecurityAppliedMessage(Message, this, signBody, encryptBody);
            Message = message;
            return message;
        }

        protected virtual ISignatureValueSecurityElement[] CreateSignatureConfirmationElements(SignatureConfirmations signatureConfirmations)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                SRP.SignatureConfirmationNotSupported));
        }

        private void StartEncryption()
        {
            if (ElementContainer.SourceEncryptionToken == null)
            {
                return;
            }

            throw ExceptionHelper.PlatformNotSupported(); // Encrypting isn't supported
        }

        private void CompleteEncryption()
        {
            // No-op as encryption not supported
        }

        internal void StartSecurityApplication()
        {
            if (SignThenEncrypt)
            {
                StartSignature();
                StartEncryption();
            }
            else
            {
                throw ExceptionHelper.PlatformNotSupported(); // Encrypting can only come first when using message encryption which isn't supported
            }
        }

        internal void CompleteSecurityApplication()
        {
            if (SignThenEncrypt)
            {
                CompleteSignature();
                SignWithSupportingTokens();
                CompleteEncryption();
            }
            else
            {
                throw ExceptionHelper.PlatformNotSupported(); // Encrypting can only come first when using message encryption which isn't supported
            }

            if (_correlationState != null)
            {
                _correlationState.SignatureConfirmations = GetSignatureValues();
            }
        }

        public void RemoveSignatureEncryptionIfAppropriate()
        {
            // No-op as no support for encryption
        }

        public string GenerateId()
        {
            int id = _idCounter++;

            if (_idPrefix != null)
            {
                return _idPrefix + id;
            }

            if (id < s_ids.Length)
            {
                return s_ids[id];
            }
            else
            {
                return "_" + id;
            }
        }

        private SignatureConfirmations GetSignatureValues()
        {
            return _signatureValuesGenerated;
        }

        internal static bool ShouldSerializeToken(SecurityTokenParameters parameters, MessageDirection transferDirection)
        {
            switch (parameters.InclusionMode)
            {
                case SecurityTokenInclusionMode.AlwaysToInitiator:
                    return (transferDirection == MessageDirection.Output);
                case SecurityTokenInclusionMode.Once:
                case SecurityTokenInclusionMode.AlwaysToRecipient:
                    return (transferDirection == MessageDirection.Input);
                case SecurityTokenInclusionMode.Never:
                    return false;
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.UnsupportedTokenInclusionMode, parameters.InclusionMode)));
            }
        }

        protected internal SecurityTokenReferenceStyle GetTokenReferenceStyle(SecurityTokenParameters parameters)
        {
            return (ShouldSerializeToken(parameters, MessageDirection)) ? SecurityTokenReferenceStyle.Internal : SecurityTokenReferenceStyle.External;
        }

        private void StartSignature()
        {
            if (ElementContainer.SourceSigningToken == null)
            {
                return;
            }

            // determine the key identifier clause to use for the source
            SecurityTokenReferenceStyle sourceSigningKeyReferenceStyle = GetTokenReferenceStyle(SigningTokenParameters);
            SecurityKeyIdentifierClause sourceSigningKeyIdentifierClause = SigningTokenParameters.CreateKeyIdentifierClause(ElementContainer.SourceSigningToken, sourceSigningKeyReferenceStyle);
            if (sourceSigningKeyIdentifierClause == null)
            {
                throw TraceUtility.ThrowHelperError(new MessageSecurityException(SRP.TokenManagerCannotCreateTokenReference), Message);
            }

            SecurityToken signingToken;
            SecurityKeyIdentifierClause signingKeyIdentifierClause;

            // determine if a token needs to be derived
            if (SigningTokenParameters.RequireDerivedKeys && !SigningTokenParameters.HasAsymmetricKey)
            {
                // Derived keys not required for initial implementation
                throw ExceptionHelper.PlatformNotSupported();
            }
            else
            {
                signingToken = ElementContainer.SourceSigningToken;
                signingKeyIdentifierClause = sourceSigningKeyIdentifierClause;
            }

            SecurityKeyIdentifier signingKeyIdentifier = new SecurityKeyIdentifier(signingKeyIdentifierClause);

            if (_signatureConfirmationsToSend != null && _signatureConfirmationsToSend.Count > 0)
            {
                ISecurityElement[] signatureConfirmationElements;
                signatureConfirmationElements = CreateSignatureConfirmationElements(_signatureConfirmationsToSend);
                for (int i = 0; i < signatureConfirmationElements.Length; ++i)
                {
                    SendSecurityHeaderElement sigConfElement = new SendSecurityHeaderElement(signatureConfirmationElements[i].Id, signatureConfirmationElements[i]);
                    sigConfElement.MarkedForEncryption = _signatureConfirmationsToSend.IsMarkedForEncryption;
                    ElementContainer.AddSignatureConfirmation(sigConfElement);
                }
            }

            bool generateTargettablePrimarySignature = ((_endorsingTokenParameters != null) || (_signedEndorsingTokenParameters != null));
            StartPrimarySignatureCore(signingToken, signingKeyIdentifier, _signatureParts, generateTargettablePrimarySignature);
        }

        private void CompleteSignature()
        {
            ISignatureValueSecurityElement signedXml = CompletePrimarySignatureCore(
                ElementContainer.GetSignatureConfirmations(), ElementContainer.GetSignedEndorsingSupportingTokens(),
                ElementContainer.GetSignedSupportingTokens(), ElementContainer.GetBasicSupportingTokens(), true);
            if (signedXml == null)
            {
                return;
            }
            ElementContainer.PrimarySignature = new SendSecurityHeaderElement(signedXml.Id, signedXml);
            ElementContainer.PrimarySignature.MarkedForEncryption = _encryptSignature;
            AddGeneratedSignatureValue(signedXml.GetSignatureValue(), EncryptPrimarySignature);
            _primarySignatureDone = true;
            _primarySignatureValue = signedXml.GetSignatureValue();
        }

        protected abstract void StartPrimarySignatureCore(SecurityToken token, SecurityKeyIdentifier identifier, MessagePartSpecification signatureParts, bool generateTargettablePrimarySignature);

        protected abstract ISignatureValueSecurityElement CompletePrimarySignatureCore(SendSecurityHeaderElement[] signatureConfirmations,
            SecurityToken[] signedEndorsingTokens, SecurityToken[] signedTokens, SendSecurityHeaderElement[] basicTokens, bool isPrimarySignature);

        protected abstract ISignatureValueSecurityElement CreateSupportingSignature(SecurityToken token, SecurityKeyIdentifier identifier);

        protected abstract ISignatureValueSecurityElement CreateSupportingSignature(SecurityToken token, SecurityKeyIdentifier identifier, ISecurityElement primarySignature);

        private void SignWithSupportingToken(SecurityToken token, SecurityKeyIdentifierClause identifierClause)
        {
            if (token == null)
            {
                throw TraceUtility.ThrowHelperArgumentNull(nameof(token), Message);
            }
            if (identifierClause == null)
            {
                throw TraceUtility.ThrowHelperError(new MessageSecurityException(SRP.TokenManagerCannotCreateTokenReference), Message);
            }
            if (!RequireMessageProtection)
            {
                if (ElementContainer.Timestamp == null)
                {
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(
                        SRP.SigningWithoutPrimarySignatureRequiresTimestamp), Message);
                }
            }
            else
            {
                if (!_primarySignatureDone)
                {
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(
                        SRP.PrimarySignatureMustBeComputedBeforeSupportingTokenSignatures), Message);
                }
                if (ElementContainer.PrimarySignature.Item == null)
                {
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(
                        SRP.SupportingTokenSignaturesNotExpected), Message);
                }
            }

            SecurityKeyIdentifier identifier = new SecurityKeyIdentifier(identifierClause);
            ISignatureValueSecurityElement supportingSignature;
            if (!RequireMessageProtection)
            {
                supportingSignature = CreateSupportingSignature(token, identifier);
            }
            else
            {
                supportingSignature = CreateSupportingSignature(token, identifier, ElementContainer.PrimarySignature.Item);
            }
            AddGeneratedSignatureValue(supportingSignature.GetSignatureValue(), _encryptSignature);
            SendSecurityHeaderElement supportingSignatureElement = new SendSecurityHeaderElement(supportingSignature.Id, supportingSignature);
            supportingSignatureElement.MarkedForEncryption = _encryptSignature;
            ElementContainer.AddEndorsingSignature(supportingSignatureElement);
        }

        private void SignWithSupportingTokens()
        {
            SecurityToken[] endorsingTokens = ElementContainer.GetEndorsingSupportingTokens();
            if (endorsingTokens != null)
            {
                for (int i = 0; i < endorsingTokens.Length; ++i)
                {
                    SecurityToken source = endorsingTokens[i];
                    SecurityKeyIdentifierClause sourceKeyClause = _endorsingTokenParameters[i].CreateKeyIdentifierClause(source, GetTokenReferenceStyle(_endorsingTokenParameters[i]));
                    if (sourceKeyClause == null)
                    {
                        throw TraceUtility.ThrowHelperError(new MessageSecurityException(SRP.TokenManagerCannotCreateTokenReference), Message);
                    }

                    SecurityToken signingToken;
                    SecurityKeyIdentifierClause signingKeyClause;
                    if (_endorsingTokenParameters[i].RequireDerivedKeys && !_endorsingTokenParameters[i].HasAsymmetricKey)
                    {
                        throw ExceptionHelper.PlatformNotSupported();
                    }
                    else
                    {
                        signingToken = source;
                        signingKeyClause = sourceKeyClause;
                    }

                    SignWithSupportingToken(signingToken, signingKeyClause);
                }
            }

            SecurityToken[] signedEndorsingSupportingTokens = ElementContainer.GetSignedEndorsingSupportingTokens();
            if (signedEndorsingSupportingTokens != null)
            {
                for (int i = 0; i < signedEndorsingSupportingTokens.Length; ++i)
                {
                    SecurityToken source = signedEndorsingSupportingTokens[i];
                    SecurityKeyIdentifierClause sourceKeyClause = _signedEndorsingTokenParameters[i].CreateKeyIdentifierClause(source, GetTokenReferenceStyle(_signedEndorsingTokenParameters[i]));
                    if (sourceKeyClause == null)
                    {
                        throw TraceUtility.ThrowHelperError(new MessageSecurityException(SRP.TokenManagerCannotCreateTokenReference), Message);
                    }

                    SecurityToken signingToken;
                    SecurityKeyIdentifierClause signingKeyClause;
                    if (_signedEndorsingTokenParameters[i].RequireDerivedKeys && !_signedEndorsingTokenParameters[i].HasAsymmetricKey)
                    {
                        throw ExceptionHelper.PlatformNotSupported(); // Derived keys not supported initially
                    }
                    else
                    {
                        signingToken = source;
                        signingKeyClause = sourceKeyClause;
                    }
                    SignWithSupportingToken(signingToken, signingKeyClause);
                }
            }
        }

        private void AddGeneratedSignatureValue(byte[] signatureValue, bool wasEncrypted)
        {
            // cache outgoing signatures only on the client side
            if (MaintainSignatureConfirmationState && (_signatureConfirmationsToSend == null))
            {
                if (_signatureValuesGenerated == null)
                {
                    _signatureValuesGenerated = new SignatureConfirmations();
                }

                _signatureValuesGenerated.AddConfirmation(signatureValue, wasEncrypted);
            }
        }
    }

    internal class TokenElement : ISecurityElement
    {
        private SecurityStandardsManager _standardsManager;

        public TokenElement(SecurityToken token, SecurityStandardsManager standardsManager)
        {
            Token = token;
            _standardsManager = standardsManager;
        }

        public override bool Equals(object item)
        {
            TokenElement element = item as TokenElement;
            return (element != null && Token == element.Token && _standardsManager == element._standardsManager);
        }

        public override int GetHashCode()
        {
            return Token.GetHashCode() ^ _standardsManager.GetHashCode();
        }

        public bool HasId
        {
            get { return true; }
        }

        public string Id
        {
            get { return Token.Id; }
        }

        public SecurityToken Token { get; }

        public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            _standardsManager.SecurityTokenSerializer.WriteToken(writer, Token);
        }
    }
}
