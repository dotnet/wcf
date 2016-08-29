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

namespace System.ServiceModel.Security
{
    internal abstract class SendSecurityHeader : SecurityHeader, IMessageHeaderWithSharedNamespace
    {
        private bool _basicTokenEncrypted;
        private SendSecurityHeaderElementContainer _elementContainer;
        private bool _primarySignatureDone;
        bool _encryptSignature;
        private SignatureConfirmations _signatureValuesGenerated;
        private SignatureConfirmations _signatureConfirmationsToSend;
        private int _idCounter;
        private string _idPrefix;
        private bool _hasSignedTokens;
        private bool _hasEncryptedTokens;
        private MessagePartSpecification _signatureParts;
        private MessagePartSpecification _encryptionParts;
        private SecurityTokenParameters _signingTokenParameters;
        private SecurityTokenParameters _encryptingTokenParameters;
        private List<SecurityToken> _basicTokens = null;
        private List<SecurityTokenParameters> _basicSupportingTokenParameters = null;
        private List<SecurityTokenParameters> _endorsingTokenParameters = null;
        private List<SecurityTokenParameters> _signedEndorsingTokenParameters = null;
        private List<SecurityTokenParameters> _signedTokenParameters = null;
        private SecurityToken _encryptingToken;
        private bool _skipKeyInfoForEncryption;
        private byte[] _primarySignatureValue = null;
        private bool _shouldProtectTokens;
        private BufferManager _bufferManager;
        private bool _shouldSignToHeader = false;
        private SecurityProtocolCorrelationState _correlationState;
        private bool _signThenEncrypt = true;
        private static readonly string[] s_ids = new string[] { "_0", "_1", "_2", "_3", "_4", "_5", "_6", "_7", "_8", "_9" };

        protected SendSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite,
            MessageDirection transferDirection)
            : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, transferDirection)
        {
            _elementContainer = new SendSecurityHeaderElementContainer();
        }

        public SendSecurityHeaderElementContainer ElementContainer
        {
            get { return _elementContainer; }
        }

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

        public MessagePartSpecification EncryptionParts
        {
            get { return _encryptionParts; }
            set
            {
                ThrowIfProcessingStarted();
                if (value == null)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentNullException("value"), this.Message);
                }
                if (!value.IsReadOnly)
                {
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(
                        SR.Format(SR.MessagePartSpecificationMustBeImmutable)), this.Message);
                }
                _encryptionParts = value;
            }
        }

        public bool EncryptPrimarySignature
        {
            get { return _encryptSignature; }
            set
            {
                ThrowIfProcessingStarted();
                _encryptSignature = value;
            }
        }

        internal byte[] PrimarySignatureValue
        {
            get { return _primarySignatureValue; }
        }

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedNamespace
        {
            get { return XD.UtilityDictionary.Namespace; }
        }

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedPrefix
        {
            get { return XD.UtilityDictionary.Prefix; }
        }

        public override string Name
        {
            get { return this.StandardsManager.SecurityVersion.HeaderName.Value; }
        }

        public override string Namespace
        {
            get { return this.StandardsManager.SecurityVersion.HeaderNamespace.Value; }
        }

        protected SecurityAppliedMessage SecurityAppliedMessage
        {
            get { return (SecurityAppliedMessage)this.Message; }
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

        public MessagePartSpecification SignatureParts
        {
            get { return _signatureParts; }
            set
            {
                ThrowIfProcessingStarted();
                if (value == null)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentNullException("value"), this.Message);
                }
                if (!value.IsReadOnly)
                {
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(
                        SR.Format(SR.MessagePartSpecificationMustBeImmutable)), this.Message);
                }
                _signatureParts = value;
            }
        }

        public SecurityTimestamp Timestamp
        {
            get { return _elementContainer.Timestamp; }
        }

        public bool HasSignedTokens
        {
            get
            {
                return this._hasSignedTokens;
            }
        }

        public bool HasEncryptedTokens
        {
            get
            {
                return this._hasEncryptedTokens;
            }
        }

        void AddParameters(ref List<SecurityTokenParameters> list, SecurityTokenParameters item)
        {
            if (list == null)
            {
                list = new List<SecurityTokenParameters>();
            }
            list.Add(item);
        }

        public abstract void ApplyBodySecurity(XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator);

        public abstract void ApplySecurityAndWriteHeaders(MessageHeaders headers, XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator);

        protected virtual bool HasSignedEncryptedMessagePart
        {
            get { return false; }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            throw ExceptionHelper.PlatformNotSupported();
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

        public void AddTimestamp(TimeSpan timestampValidityDuration)
        {
            DateTime now = DateTime.UtcNow;
            string id = this.RequireMessageProtection ? SecurityUtils.GenerateId() : GenerateId();
            AddTimestamp(new SecurityTimestamp(now, now + timestampValidityDuration, id));
        }

        public void AddTimestamp(SecurityTimestamp timestamp)
        {
            ThrowIfProcessingStarted();
            if (_elementContainer.Timestamp != null)
            {
                throw TraceUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.TimestampAlreadySetForSecurityHeader)), this.Message);
            }
            if (timestamp == null)
            {
                throw TraceUtility.ThrowHelperArgumentNull("timestamp", this.Message);
            }

            _elementContainer.Timestamp = timestamp;
        }

        public void AddBasicSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
        {
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
            if (parameters == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
            ThrowIfProcessingStarted();
            SendSecurityHeaderElement tokenElement = new SendSecurityHeaderElement(token.Id, new TokenElement(token, this.StandardsManager));
            tokenElement.MarkedForEncryption = true;
            _elementContainer.AddBasicSupportingToken(tokenElement);
            _hasEncryptedTokens = true;
            _hasSignedTokens = true;
            AddParameters(ref _basicSupportingTokenParameters, parameters);
            if (_basicTokens == null)
            {
                _basicTokens = new List<SecurityToken>();
            }

            //  We maintain a list of the basic tokens for the SignThenEncrypt case as we will 
            //  need this token to write STR entry on OnWriteHeaderContents. 
            _basicTokens.Add(token);

        }

        public void AddEndorsingSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
        {
            throw ExceptionHelper.PlatformNotSupported();

            // Issue #31 in progress

            //if (token == null)
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
            //if (parameters == null)
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
            //ThrowIfProcessingStarted();
            //this.elementContainer.AddEndorsingSupportingToken(token);
            //// The ProviderBackedSecurityToken was added for the ChannelBindingToken (CBT) effort for win7.  
            //// We can assume the key is of type symmetric key.
            ////
            //// Asking for the key type from the token will cause the ProviderBackedSecurityToken 
            //// to attempt to resolve the token and the nego will start.  
            ////
            //// We don't want that.  
            //// We want to defer the nego until after the CBT is available in SecurityAppliedMessage.OnWriteMessage.
            //if (!(token is ProviderBackedSecurityToken))
            //{
            //    this.shouldSignToHeader |= (!this.RequireMessageProtection) && (SecurityUtils.GetSecurityKey<AsymmetricSecurityKey>(token) != null);
            //}
            //this.AddParameters(ref this.endorsingTokenParameters, parameters);
        }

        public void AddSignedEndorsingSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
        {
            throw ExceptionHelper.PlatformNotSupported();

            // Issue #31 in progress
            //if (token == null)
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
            //if (parameters == null)
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
            //ThrowIfProcessingStarted();
            //this.elementContainer.AddSignedEndorsingSupportingToken(token);
            //hasSignedTokens = true;
            //this.shouldSignToHeader |= (!this.RequireMessageProtection) && (SecurityUtils.GetSecurityKey<AsymmetricSecurityKey>(token) != null);
            //this.AddParameters(ref this.signedEndorsingTokenParameters, parameters);
        }

        public void AddSignedSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
        {
            throw ExceptionHelper.PlatformNotSupported();

            // Issue #31 in progress
            //if (token == null)
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
            //if (parameters == null)
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
            //ThrowIfProcessingStarted();
            //this.elementContainer.AddSignedSupportingToken(token);
            //hasSignedTokens = true;
            //this.AddParameters(ref this.signedTokenParameters, parameters);
        }

        public void RemoveSignatureEncryptionIfAppropriate()
        {
            if (this.SignThenEncrypt &&
                this.EncryptPrimarySignature &&
                (this.SecurityAppliedMessage.BodyProtectionMode != MessagePartProtectionMode.SignThenEncrypt) &&
                (_basicSupportingTokenParameters == null || _basicSupportingTokenParameters.Count == 0) &&
                (_signatureConfirmationsToSend == null || _signatureConfirmationsToSend.Count == 0 || !_signatureConfirmationsToSend.IsMarkedForEncryption) &&
                !this.HasSignedEncryptedMessagePart)
            {
                _encryptSignature = false;
            }
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

        SignatureConfirmations GetSignatureValues()
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.Format(SR.UnsupportedTokenInclusionMode, parameters.InclusionMode)));
            }
        }

        protected abstract void WriteSecurityTokenReferencyEntry(XmlDictionaryWriter writer, SecurityToken securityToken, SecurityTokenParameters securityTokenParameters);

        public Message SetupExecution()
        {
            ThrowIfProcessingStarted();
            SetProcessingStarted();

            bool signBody = false;
            if (_elementContainer.SourceSigningToken != null)
            {
                if (_signatureParts == null)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentNullException("SignatureParts"), this.Message);
                }
                signBody = _signatureParts.IsBodyIncluded;
            }

            bool encryptBody = false;
            if (_elementContainer.SourceEncryptionToken != null)
            {
                if (_encryptionParts == null)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentNullException("EncryptionParts"), this.Message);
                }
                encryptBody = _encryptionParts.IsBodyIncluded;
            }

            SecurityAppliedMessage message = new SecurityAppliedMessage(this.Message, this, signBody, encryptBody);
            this.Message = message;
            return message;
        }

        protected internal SecurityTokenReferenceStyle GetTokenReferenceStyle(SecurityTokenParameters parameters)
        {
            return (ShouldSerializeToken(parameters, this.MessageDirection)) ? SecurityTokenReferenceStyle.Internal : SecurityTokenReferenceStyle.External;
        }

        void StartSignature()
        {
            if (_elementContainer.SourceSigningToken == null)
            {
                return;
            }

            // determine the key identifier clause to use for the source
            SecurityTokenReferenceStyle sourceSigningKeyReferenceStyle = GetTokenReferenceStyle(_signingTokenParameters);
            SecurityKeyIdentifierClause sourceSigningKeyIdentifierClause = _signingTokenParameters.CreateKeyIdentifierClause(_elementContainer.SourceSigningToken, sourceSigningKeyReferenceStyle);
            if (sourceSigningKeyIdentifierClause == null)
            {
                throw TraceUtility.ThrowHelperError(new MessageSecurityException(SR.Format(SR.TokenManagerCannotCreateTokenReference)), this.Message);
            }

            SecurityToken signingToken;
            SecurityKeyIdentifierClause signingKeyIdentifierClause;

            // determine if a token needs to be derived
            if (_signingTokenParameters.RequireDerivedKeys && !_signingTokenParameters.HasAsymmetricKey)
            {
                string derivationAlgorithm = this.AlgorithmSuite.GetSignatureKeyDerivationAlgorithm(_elementContainer.SourceSigningToken, this.StandardsManager.MessageSecurityVersion.SecureConversationVersion);
                string expectedDerivationAlgorithm = SecurityUtils.GetKeyDerivationAlgorithm(this.StandardsManager.MessageSecurityVersion.SecureConversationVersion);
                if (derivationAlgorithm == expectedDerivationAlgorithm)
                {
                    DerivedKeySecurityToken derivedSigningToken = new DerivedKeySecurityToken(-1, 0, this.AlgorithmSuite.GetSignatureKeyDerivationLength(_elementContainer.SourceSigningToken, this.StandardsManager.MessageSecurityVersion.SecureConversationVersion), null, DerivedKeySecurityToken.DefaultNonceLength, _elementContainer.SourceSigningToken,
                        sourceSigningKeyIdentifierClause, derivationAlgorithm, GenerateId());
                    signingToken = _elementContainer.DerivedSigningToken = derivedSigningToken;
                    signingKeyIdentifierClause = new LocalIdKeyIdentifierClause(signingToken.Id, signingToken.GetType());
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.Format(SR.UnsupportedCryptoAlgorithm, derivationAlgorithm)));
                }
            }
            else
            {
                signingToken = _elementContainer.SourceSigningToken;
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
                    _elementContainer.AddSignatureConfirmation(sigConfElement);
                }
            }

            bool generateTargettablePrimarySignature = ((_endorsingTokenParameters != null) || (_signedEndorsingTokenParameters != null));
            this.StartPrimarySignatureCore(signingToken, signingKeyIdentifier, _signatureParts, generateTargettablePrimarySignature);
        }

        void CompleteSignature()
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //ISignatureValueSecurityElement signedXml = this.CompletePrimarySignatureCore(
            //    _elementContainer.GetSignatureConfirmations(), _elementContainer.GetSignedEndorsingSupportingTokens(),
            //    _elementContainer.GetSignedSupportingTokens(), _elementContainer.GetBasicSupportingTokens(), true);
            //if (signedXml == null)
            //{
            //    return;
            //}
            //_elementContainer.PrimarySignature = new SendSecurityHeaderElement(signedXml.Id, signedXml);
            //_elementContainer.PrimarySignature.MarkedForEncryption = _encryptSignature;
            //AddGeneratedSignatureValue(signedXml.GetSignatureValue(), this.EncryptPrimarySignature);
            //this.primarySignatureDone = true;
            //this.primarySignatureValue = signedXml.GetSignatureValue();
        }

        protected abstract void StartPrimarySignatureCore(SecurityToken token, SecurityKeyIdentifier identifier, MessagePartSpecification signatureParts, bool generateTargettablePrimarySignature);

        protected abstract ISignatureValueSecurityElement CompletePrimarySignatureCore(SendSecurityHeaderElement[] signatureConfirmations,
           SecurityToken[] signedEndorsingTokens, SecurityToken[] signedTokens, SendSecurityHeaderElement[] basicTokens, bool isPrimarySignature);


        protected abstract ISignatureValueSecurityElement CreateSupportingSignature(SecurityToken token, SecurityKeyIdentifier identifier);

        protected abstract ISignatureValueSecurityElement CreateSupportingSignature(SecurityToken token, SecurityKeyIdentifier identifier, ISecurityElement primarySignature);

        protected abstract void StartEncryptionCore(SecurityToken token, SecurityKeyIdentifier keyIdentifier);

        protected abstract ISecurityElement CompleteEncryptionCore(SendSecurityHeaderElement primarySignature,
            SendSecurityHeaderElement[] basicTokens, SendSecurityHeaderElement[] signatureConfirmations, SendSecurityHeaderElement[] endorsingSignatures);

        void SignWithSupportingToken(SecurityToken token, SecurityKeyIdentifierClause identifierClause)
        {
            if (token == null)
            {
                throw TraceUtility.ThrowHelperArgumentNull("token", this.Message);
            }
            if (identifierClause == null)
            {
                throw TraceUtility.ThrowHelperError(new MessageSecurityException(SR.Format(SR.TokenManagerCannotCreateTokenReference)), this.Message);
            }
            if (!this.RequireMessageProtection)
            {
                if (_elementContainer.Timestamp == null)
                {
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(
                        SR.Format(SR.SigningWithoutPrimarySignatureRequiresTimestamp)), this.Message);
                }
            }
            else
            {
                if (!_primarySignatureDone)
                {
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(
                        SR.Format(SR.PrimarySignatureMustBeComputedBeforeSupportingTokenSignatures)), this.Message);
                }
                if (_elementContainer.PrimarySignature.Item == null)
                {
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(
                        SR.Format(SR.SupportingTokenSignaturesNotExpected)), this.Message);
                }
            }

            SecurityKeyIdentifier identifier = new SecurityKeyIdentifier(identifierClause);
            ISignatureValueSecurityElement supportingSignature;
            if (!this.RequireMessageProtection)
            {
                supportingSignature = CreateSupportingSignature(token, identifier);
            }
            else
            {
                supportingSignature = CreateSupportingSignature(token, identifier, _elementContainer.PrimarySignature.Item);
            }
            AddGeneratedSignatureValue(supportingSignature.GetSignatureValue(), _encryptSignature);
            SendSecurityHeaderElement supportingSignatureElement = new SendSecurityHeaderElement(supportingSignature.Id, supportingSignature);
            supportingSignatureElement.MarkedForEncryption = _encryptSignature;
            _elementContainer.AddEndorsingSignature(supportingSignatureElement);
        }

        void SignWithSupportingTokens()
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //SecurityToken[] endorsingTokens = _elementContainer.GetEndorsingSupportingTokens();
            //if (endorsingTokens != null)
            //{
            //    for (int i = 0; i < endorsingTokens.Length; ++i)
            //    {
            //        SecurityToken source = endorsingTokens[i];
            //        SecurityKeyIdentifierClause sourceKeyClause = _endorsingTokenParameters[i].CreateKeyIdentifierClause(source, GetTokenReferenceStyle(endorsingTokenParameters[i]));
            //        if (sourceKeyClause == null)
            //        {
            //            throw TraceUtility.ThrowHelperError(new MessageSecurityException(SR.Format(SR.TokenManagerCannotCreateTokenReference)), this.Message);
            //        }
            //        SecurityToken signingToken;
            //        SecurityKeyIdentifierClause signingKeyClause;
            //        if (_endorsingTokenParameters[i].RequireDerivedKeys && !_endorsingTokenParameters[i].HasAsymmetricKey)
            //        {
            //            string derivationAlgorithm = SecurityUtils.GetKeyDerivationAlgorithm(this.StandardsManager.MessageSecurityVersion.SecureConversationVersion);
            //            DerivedKeySecurityToken dkt = new DerivedKeySecurityToken(-1, 0,
            //                this.AlgorithmSuite.GetSignatureKeyDerivationLength(source, this.StandardsManager.MessageSecurityVersion.SecureConversationVersion), null,
            //                DerivedKeySecurityToken.DefaultNonceLength, source, sourceKeyClause, derivationAlgorithm, GenerateId());
            //            signingToken = dkt;
            //            signingKeyClause = new LocalIdKeyIdentifierClause(dkt.Id, dkt.GetType());
            //            _elementContainer.AddEndorsingDerivedSupportingToken(dkt);
            //        }
            //        else
            //        {
            //            signingToken = source;
            //            signingKeyClause = sourceKeyClause;
            //        }
            //        SignWithSupportingToken(signingToken, signingKeyClause);
            //    }
            //}
            //SecurityToken[] signedEndorsingSupportingTokens = _elementContainer.GetSignedEndorsingSupportingTokens();
            //if (signedEndorsingSupportingTokens != null)
            //{
            //    for (int i = 0; i < signedEndorsingSupportingTokens.Length; ++i)
            //    {
            //        SecurityToken source = signedEndorsingSupportingTokens[i];
            //        SecurityKeyIdentifierClause sourceKeyClause = _signedEndorsingTokenParameters[i].CreateKeyIdentifierClause(source, GetTokenReferenceStyle(_signedEndorsingTokenParameters[i]));
            //        if (sourceKeyClause == null)
            //        {
            //            throw TraceUtility.ThrowHelperError(new MessageSecurityException(SR.Format(SR.TokenManagerCannotCreateTokenReference)), this.Message);
            //        }
            //        SecurityToken signingToken;
            //        SecurityKeyIdentifierClause signingKeyClause;
            //        if (_signedEndorsingTokenParameters[i].RequireDerivedKeys && !_signedEndorsingTokenParameters[i].HasAsymmetricKey)
            //        {
            //            string derivationAlgorithm = SecurityUtils.GetKeyDerivationAlgorithm(this.StandardsManager.MessageSecurityVersion.SecureConversationVersion);
            //            DerivedKeySecurityToken dkt = new DerivedKeySecurityToken(-1, 0,
            //                this.AlgorithmSuite.GetSignatureKeyDerivationLength(source, this.StandardsManager.MessageSecurityVersion.SecureConversationVersion), null,
            //                DerivedKeySecurityToken.DefaultNonceLength, source, sourceKeyClause, derivationAlgorithm, GenerateId());
            //            signingToken = dkt;
            //            signingKeyClause = new LocalIdKeyIdentifierClause(dkt.Id, dkt.GetType());
            //            _elementContainer.AddSignedEndorsingDerivedSupportingToken(dkt);
            //        }
            //        else
            //        {
            //            signingToken = source;
            //            signingKeyClause = sourceKeyClause;
            //        }
            //        SignWithSupportingToken(signingToken, signingKeyClause);
            //    }
            //}
        }

        protected virtual ISignatureValueSecurityElement[] CreateSignatureConfirmationElements(SignatureConfirmations signatureConfirmations)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                SR.Format(SR.SignatureConfirmationNotSupported)));
        }

        void StartEncryption()
        {
            if (_elementContainer.SourceEncryptionToken == null)
            {
                return;
            }
            // determine the key identifier clause to use for the source
            SecurityTokenReferenceStyle sourceEncryptingKeyReferenceStyle = GetTokenReferenceStyle(_encryptingTokenParameters);
            bool encryptionTokenSerialized = sourceEncryptingKeyReferenceStyle == SecurityTokenReferenceStyle.Internal;
            SecurityKeyIdentifierClause sourceEncryptingKeyIdentifierClause = _encryptingTokenParameters.CreateKeyIdentifierClause(_elementContainer.SourceEncryptionToken, sourceEncryptingKeyReferenceStyle);
            if (sourceEncryptingKeyIdentifierClause == null)
            {
                throw TraceUtility.ThrowHelperError(new MessageSecurityException(SR.Format(SR.TokenManagerCannotCreateTokenReference)), this.Message);
            }
            SecurityToken sourceToken;
            SecurityKeyIdentifierClause sourceTokenIdentifierClause;

            // if the source token cannot do symmetric crypto, create a wrapped key
            if (!SecurityUtils.HasSymmetricSecurityKey(_elementContainer.SourceEncryptionToken))
            {
                int keyLength = Math.Max(128, this.AlgorithmSuite.DefaultSymmetricKeyLength);
                CryptoHelper.ValidateSymmetricKeyLength(keyLength, this.AlgorithmSuite);
                byte[] key = new byte[keyLength / 8];
                CryptoHelper.FillRandomBytes(key);
                string keyWrapAlgorithm;
                XmlDictionaryString keyWrapAlgorithmDictionaryString;
                this.AlgorithmSuite.GetKeyWrapAlgorithm(_elementContainer.SourceEncryptionToken, out keyWrapAlgorithm, out keyWrapAlgorithmDictionaryString);
                WrappedKeySecurityToken wrappedKey = new WrappedKeySecurityToken(GenerateId(), key, keyWrapAlgorithm, keyWrapAlgorithmDictionaryString,
                    _elementContainer.SourceEncryptionToken, new SecurityKeyIdentifier(sourceEncryptingKeyIdentifierClause));
                _elementContainer.WrappedEncryptionToken = wrappedKey;
                sourceToken = wrappedKey;
                sourceTokenIdentifierClause = new LocalIdKeyIdentifierClause(wrappedKey.Id, wrappedKey.GetType());
                encryptionTokenSerialized = true;
            }
            else
            {
                sourceToken = _elementContainer.SourceEncryptionToken;
                sourceTokenIdentifierClause = sourceEncryptingKeyIdentifierClause;
            }

            // determine if a key needs to be derived
            SecurityKeyIdentifierClause encryptingKeyIdentifierClause;
            // determine if a token needs to be derived
            if (_encryptingTokenParameters.RequireDerivedKeys)
            {
                string derivationAlgorithm = this.AlgorithmSuite.GetEncryptionKeyDerivationAlgorithm(sourceToken, this.StandardsManager.MessageSecurityVersion.SecureConversationVersion);
                string expectedDerivationAlgorithm = SecurityUtils.GetKeyDerivationAlgorithm(this.StandardsManager.MessageSecurityVersion.SecureConversationVersion);
                if (derivationAlgorithm == expectedDerivationAlgorithm)
                {
                    DerivedKeySecurityToken derivedEncryptingToken = new DerivedKeySecurityToken(-1, 0,
                        this.AlgorithmSuite.GetEncryptionKeyDerivationLength(sourceToken, this.StandardsManager.MessageSecurityVersion.SecureConversationVersion), null, DerivedKeySecurityToken.DefaultNonceLength, sourceToken, sourceTokenIdentifierClause, derivationAlgorithm, GenerateId());
                    _encryptingToken = _elementContainer.DerivedEncryptionToken = derivedEncryptingToken;
                    encryptingKeyIdentifierClause = new LocalIdKeyIdentifierClause(derivedEncryptingToken.Id, derivedEncryptingToken.GetType());
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.Format(SR.UnsupportedCryptoAlgorithm, derivationAlgorithm)));
                }
            }
            else
            {
                _encryptingToken = sourceToken;
                encryptingKeyIdentifierClause = sourceTokenIdentifierClause;
            }

            _skipKeyInfoForEncryption = encryptionTokenSerialized && this.EncryptedKeyContainsReferenceList && (_encryptingToken is WrappedKeySecurityToken) && this._signThenEncrypt;
            SecurityKeyIdentifier identifier;
            if (_skipKeyInfoForEncryption)
            {
                identifier = null;
            }
            else
            {
                identifier = new SecurityKeyIdentifier(encryptingKeyIdentifierClause);
            }

            StartEncryptionCore(_encryptingToken, identifier);
        }

        void CompleteEncryption()
        {
            ISecurityElement referenceList = CompleteEncryptionCore(
                _elementContainer.PrimarySignature,
                _elementContainer.GetBasicSupportingTokens(),
                _elementContainer.GetSignatureConfirmations(),
                _elementContainer.GetEndorsingSignatures());

            if (referenceList == null)
            {
                // null out all the encryption fields since there is no encryption needed
                _elementContainer.SourceEncryptionToken = null;
                _elementContainer.WrappedEncryptionToken = null;
                _elementContainer.DerivedEncryptionToken = null;
                return;
            }

            if (_skipKeyInfoForEncryption)
            {
                WrappedKeySecurityToken wrappedKeyToken = _encryptingToken as WrappedKeySecurityToken;
                wrappedKeyToken.EnsureEncryptedKeySetUp();
                wrappedKeyToken.EncryptedKey.ReferenceList = (ReferenceList)referenceList;
            }
            else
            {
                _elementContainer.ReferenceList = referenceList;
            }
            _basicTokenEncrypted = true;
        }

        internal void StartSecurityApplication()
        {
            if (this.SignThenEncrypt)
            {
                StartSignature();
                StartEncryption();
            }
            else
            {
                StartEncryption();
                StartSignature();
            }
        }

        internal void CompleteSecurityApplication()
        {
            if (this.SignThenEncrypt)
            {
                CompleteSignature();
                SignWithSupportingTokens();
                CompleteEncryption();
            }
            else
            {
                CompleteEncryption();
                CompleteSignature();
                SignWithSupportingTokens();
            }

            if (_correlationState != null)
            {
                _correlationState.SignatureConfirmations = GetSignatureValues();
            }
        }

        void AddGeneratedSignatureValue(byte[] signatureValue, bool wasEncrypted)
        {
            // cache outgoing signatures only on the client side
            if (this.MaintainSignatureConfirmationState && (_signatureConfirmationsToSend == null))
            {
                if (_signatureValuesGenerated == null)
                {
                    _signatureValuesGenerated = new SignatureConfirmations();
                }
                _signatureValuesGenerated.AddConfirmation(signatureValue, wasEncrypted);
            }
        }
    }

    class TokenElement : ISecurityElement
    {
        SecurityStandardsManager standardsManager;
        SecurityToken token;

        public TokenElement(SecurityToken token, SecurityStandardsManager standardsManager)
        {
            this.token = token;
            this.standardsManager = standardsManager;
        }

        public override bool Equals(object item)
        {
            TokenElement element = item as TokenElement;
            return (element != null && this.token == element.token && this.standardsManager == element.standardsManager);
        }

        public override int GetHashCode()
        {
            return token.GetHashCode() ^ standardsManager.GetHashCode();
        }

        public bool HasId
        {
            get { return true; }
        }

        public string Id
        {
            get { return token.Id; }
        }

        public SecurityToken Token
        {
            get { return token; }
        }

        public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            standardsManager.SecurityTokenSerializer.WriteToken(writer, token);
        }
    }
}
