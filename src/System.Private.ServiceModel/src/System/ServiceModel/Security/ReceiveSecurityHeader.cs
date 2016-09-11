// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace System.ServiceModel.Security
{
    internal abstract class ReceiveSecurityHeader : SecurityHeader
    {
        private bool _expectBasicTokens;
        private bool _expectSignedTokens;
        private bool _expectEndorsingTokens;
        private long _maxReceivedMessageSize = TransportDefaults.MaxReceivedMessageSize;
        private XmlDictionaryReaderQuotas _readerQuotas;
        private IList<SupportingTokenAuthenticatorSpecification> _supportingTokenAuthenticators;
        private ReadOnlyCollection<SecurityTokenResolver> _outOfBandTokenResolver;
        private SecurityTokenAuthenticator _derivedTokenAuthenticator;
        private bool _replayDetectionEnabled = false;
        private NonceCache _nonceCache;
        private TimeSpan _replayWindow;
        private TimeSpan _clockSkew;
        private Collection<SecurityToken> _basicTokens;
        private Collection<SecurityToken> _signedTokens;
        private Collection<SecurityToken> _endorsingTokens;
        private Collection<SecurityToken> _signedEndorsingTokens;
        private Dictionary<SecurityToken, ReadOnlyCollection<IAuthorizationPolicy>> _tokenPoliciesMapping;
        private bool _enforceDerivedKeyRequirement = true;

        protected ReceiveSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite,
            int headerIndex,
            MessageDirection direction)
            : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, direction)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        public override string Name
        {
            get { return this.StandardsManager.SecurityVersion.HeaderName.Value; }
        }

        public override string Namespace
        {
            get { return this.StandardsManager.SecurityVersion.HeaderNamespace.Value; }
        }

        public Collection<SecurityToken> BasicSupportingTokens
        {
            get
            {
                return _basicTokens;
            }
        }

        public Collection<SecurityToken> SignedSupportingTokens
        {
            get
            {
                return _signedTokens;
            }
        }

        public Collection<SecurityToken> EndorsingSupportingTokens
        {
            get
            {
                return _endorsingTokens;
            }
        }

        public Collection<SecurityToken> SignedEndorsingSupportingTokens
        {
            get
            {
                return _signedEndorsingTokens;
            }
        }

        public Message ProcessedMessage
        {
            get { return Message; }
        }

        public SecurityTokenAuthenticator DerivedTokenAuthenticator
        {
            get
            {
                return _derivedTokenAuthenticator;
            }
            set
            {
                ThrowIfProcessingStarted();
                _derivedTokenAuthenticator = value;
            }
        }

        public bool EnforceDerivedKeyRequirement
        {
            get
            {
                return _enforceDerivedKeyRequirement;
            }
            set
            {
                ThrowIfProcessingStarted();
                _enforceDerivedKeyRequirement = value;
            }
        }

        public bool ReplayDetectionEnabled
        {
            get { return _replayDetectionEnabled; }
            set
            {
                ThrowIfProcessingStarted();
                _replayDetectionEnabled = value;
            }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        public bool ExpectBasicTokens
        {
            get { return _expectBasicTokens; }
            set
            {
                ThrowIfProcessingStarted();
                _expectBasicTokens = value;
            }
        }

        public bool ExpectSignedTokens
        {
            get { return _expectSignedTokens; }
            set
            {
                ThrowIfProcessingStarted();
                _expectSignedTokens = value;
            }
        }

        public bool ExpectEndorsingTokens
        {
            get { return _expectEndorsingTokens; }
            set
            {
                ThrowIfProcessingStarted();
                _expectEndorsingTokens = value;
            }
        }

        internal long MaxReceivedMessageSize
        {
            get
            {
                return _maxReceivedMessageSize;
            }
            set
            {
                ThrowIfProcessingStarted();
                _maxReceivedMessageSize = value;
            }
        }

        internal XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return _readerQuotas; }
            set
            {
                ThrowIfProcessingStarted();

                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");

                _readerQuotas = value;
            }
        }

        public Dictionary<SecurityToken, ReadOnlyCollection<IAuthorizationPolicy>> SecurityTokenAuthorizationPoliciesMapping
        {
            get
            {
                if (_tokenPoliciesMapping == null)
                {
                    _tokenPoliciesMapping = new Dictionary<SecurityToken, ReadOnlyCollection<IAuthorizationPolicy>>();
                }
                return _tokenPoliciesMapping;
            }
        }

        public void ConfigureTransportBindingServerReceiveHeader(IList<SupportingTokenAuthenticatorSpecification> supportingTokenAuthenticators)
        {
            _supportingTokenAuthenticators = supportingTokenAuthenticators;
        }

        public void ConfigureOutOfBandTokenResolver(ReadOnlyCollection<SecurityTokenResolver> outOfBandResolvers)
        {
            if (outOfBandResolvers == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(outOfBandResolvers));
            if (outOfBandResolvers.Count == 0)
            {
                return;
            }
            _outOfBandTokenResolver = outOfBandResolvers;
        }

        Collection<SecurityToken> EnsureSupportingTokens(ref Collection<SecurityToken> list)
        {
            if (list == null)
                list = new Collection<SecurityToken>();
            return list;
        }

        void VerifySupportingToken(TokenTracker tracker)
        {
            if (tracker == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tracker");

            Contract.Assert(tracker._spec != null, "Supporting token trackers cannot have null specification.");

            SupportingTokenAuthenticatorSpecification spec = tracker._spec;

            if (tracker._token == null)
            {
                if (spec.IsTokenOptional)
                    return;
                else
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.SupportingTokenNotProvided, spec.TokenParameters, spec.SecurityTokenAttachmentMode)));
            }
            switch (spec.SecurityTokenAttachmentMode)
            {
                case SecurityTokenAttachmentMode.Endorsing:
                    if (!tracker._IsEndorsing)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.SupportingTokenIsNotEndorsing, spec.TokenParameters)));
                    }
                    if (this.EnforceDerivedKeyRequirement && spec.TokenParameters.RequireDerivedKeys && !spec.TokenParameters.HasAsymmetricKey && !tracker._IsDerivedFrom)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.SupportingSignatureIsNotDerivedFrom, spec.TokenParameters)));
                    }
                    EnsureSupportingTokens(ref _endorsingTokens).Add(tracker._token);
                    break;
                case SecurityTokenAttachmentMode.Signed:
                    if (!tracker._IsSigned && this.RequireMessageProtection)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.SupportingTokenIsNotSigned, spec.TokenParameters)));
                    }
                    EnsureSupportingTokens(ref _signedTokens).Add(tracker._token);
                    break;
                case SecurityTokenAttachmentMode.SignedEncrypted:
                    if (!tracker._IsSigned && this.RequireMessageProtection)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.SupportingTokenIsNotSigned, spec.TokenParameters)));
                    }
                    if (!tracker._IsEncrypted && this.RequireMessageProtection)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.SupportingTokenIsNotEncrypted, spec.TokenParameters)));
                    }
                    EnsureSupportingTokens(ref _basicTokens).Add(tracker._token);
                    break;
                case SecurityTokenAttachmentMode.SignedEndorsing:
                    if (!tracker._IsSigned && this.RequireMessageProtection)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.SupportingTokenIsNotSigned, spec.TokenParameters)));
                    }
                    if (!tracker._IsEndorsing)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.SupportingTokenIsNotEndorsing, spec.TokenParameters)));
                    }
                    if (this.EnforceDerivedKeyRequirement && spec.TokenParameters.RequireDerivedKeys && !spec.TokenParameters.HasAsymmetricKey && !tracker._IsDerivedFrom)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.SupportingSignatureIsNotDerivedFrom, spec.TokenParameters)));
                    }
                    EnsureSupportingTokens(ref _signedEndorsingTokens).Add(tracker._token);
                    break;

                default:
                    Contract.Assert(false, "Unknown token attachment mode " + spec.SecurityTokenAttachmentMode);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.Format(SR.UnknownTokenAttachmentMode, spec.SecurityTokenAttachmentMode)));
            }
        }

        // replay detection done if enableReplayDetection is set to true.
        public void SetTimeParameters(NonceCache nonceCache, TimeSpan replayWindow, TimeSpan clockSkew)
        {
            _nonceCache = nonceCache;
            _replayWindow = replayWindow;
            _clockSkew = clockSkew;
        }

        public void Process(TimeSpan timeout, ChannelBinding channelBinding, ExtendedProtectionPolicy extendedProtectionPolicy)
        {
            Contract.Assert(this.ReaderQuotas != null, "Reader quotas must be set before processing");

            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

        //    MessageProtectionOrder actualProtectionOrder = this.protectionOrder;
        //    bool wasProtectionOrderDowngraded = false;
        //    if (this.protectionOrder == MessageProtectionOrder.SignBeforeEncryptAndEncryptSignature)
        //    {
        //        if (this.RequiredEncryptionParts == null || !this.RequiredEncryptionParts.IsBodyIncluded)
        //        {
        //            // Let's downgrade for now. If after signature verification we find a header that 
        //            // is signed and encrypted, we will check for signature encryption too.
        //            actualProtectionOrder = MessageProtectionOrder.SignBeforeEncrypt;
        //            wasProtectionOrderDowngraded = true;
        //        }
        //    }

        //    this.channelBinding = channelBinding;
        //    this.extendedProtectionPolicy = extendedProtectionPolicy;
        //    this.orderTracker.SetRequiredProtectionOrder(actualProtectionOrder);

        //    SetProcessingStarted();
        //    this.timeoutHelper = new TimeoutHelper(timeout);
        //    this.Message = this.securityVerifiedMessage = new SecurityVerifiedMessage(this.Message, this);
        //    XmlDictionaryReader reader = CreateSecurityHeaderReader();
        //    reader.MoveToStartElement();
        //    if (reader.IsEmptyElement)
        //    {
        //        throw TraceUtility.ThrowHelperError(new MessageSecurityException(SR.Format(SR.SecurityHeaderIsEmpty)), this.Message);
        //    }
        //    if (this.RequireMessageProtection)
        //    {
        //        this.securityElementAttributes = XmlAttributeHolder.ReadAttributes(reader);
        //    }
        //    else
        //    {
        //        this.securityElementAttributes = XmlAttributeHolder.emptyArray;
        //    }
        //    reader.ReadStartElement();

        //    if (this.primaryTokenParameters != null)
        //    {
        //        this.primaryTokenTracker = new TokenTracker(null, this.outOfBandPrimaryToken, this.allowFirstTokenMismatch);
        //    }
        //    // universalTokenResolver is used for resolving tokens
        //    universalTokenResolver = new SecurityHeaderTokenResolver(this);
        //    // primary token resolver is used for resolving primary signature and decryption
        //    primaryTokenResolver = new SecurityHeaderTokenResolver(this);
        //    if (this.outOfBandPrimaryToken != null)
        //    {
        //        universalTokenResolver.Add(this.outOfBandPrimaryToken, SecurityTokenReferenceStyle.External, this.primaryTokenParameters);
        //        primaryTokenResolver.Add(this.outOfBandPrimaryToken, SecurityTokenReferenceStyle.External, this.primaryTokenParameters);
        //    }
        //    else if (this.outOfBandPrimaryTokenCollection != null)
        //    {
        //        for (int i = 0; i < this.outOfBandPrimaryTokenCollection.Count; ++i)
        //        {
        //            universalTokenResolver.Add(this.outOfBandPrimaryTokenCollection[i], SecurityTokenReferenceStyle.External, this.primaryTokenParameters);
        //            primaryTokenResolver.Add(this.outOfBandPrimaryTokenCollection[i], SecurityTokenReferenceStyle.External, this.primaryTokenParameters);
        //        }
        //    }
        //    if (this.wrappingToken != null)
        //    {
        //        universalTokenResolver.ExpectedWrapper = this.wrappingToken;
        //        universalTokenResolver.ExpectedWrapperTokenParameters = this.wrappingTokenParameters;
        //        primaryTokenResolver.ExpectedWrapper = this.wrappingToken;
        //        primaryTokenResolver.ExpectedWrapperTokenParameters = this.wrappingTokenParameters;
        //    }
        //    else if (expectedEncryptionToken != null)
        //    {
        //        universalTokenResolver.Add(expectedEncryptionToken, SecurityTokenReferenceStyle.External, expectedEncryptionTokenParameters);
        //        primaryTokenResolver.Add(expectedEncryptionToken, SecurityTokenReferenceStyle.External, expectedEncryptionTokenParameters);
        //    }

        //    if (this.outOfBandTokenResolver == null)
        //    {
        //        this.combinedUniversalTokenResolver = this.universalTokenResolver;
        //        this.combinedPrimaryTokenResolver = this.primaryTokenResolver;
        //    }
        //    else
        //    {
        //        this.combinedUniversalTokenResolver = new AggregateSecurityHeaderTokenResolver(this.universalTokenResolver, this.outOfBandTokenResolver);
        //        this.combinedPrimaryTokenResolver = new AggregateSecurityHeaderTokenResolver(this.primaryTokenResolver, this.outOfBandTokenResolver);
        //    }

        //    allowedAuthenticators = new List<SecurityTokenAuthenticator>();
        //    if (this.primaryTokenAuthenticator != null)
        //    {
        //        allowedAuthenticators.Add(this.primaryTokenAuthenticator);
        //    }
        //    if (this.DerivedTokenAuthenticator != null)
        //    {
        //        allowedAuthenticators.Add(this.DerivedTokenAuthenticator);
        //    }
        //    pendingSupportingTokenAuthenticator = null;
        //    int numSupportingTokensRequiringDerivation = 0;
        //    if (this.supportingTokenAuthenticators != null && this.supportingTokenAuthenticators.Count > 0)
        //    {
        //        this.supportingTokenTrackers = new List<TokenTracker>(this.supportingTokenAuthenticators.Count);
        //        for (int i = 0; i < this.supportingTokenAuthenticators.Count; ++i)
        //        {
        //            SupportingTokenAuthenticatorSpecification spec = this.supportingTokenAuthenticators[i];
        //            switch (spec.SecurityTokenAttachmentMode)
        //            {
        //                case SecurityTokenAttachmentMode.Endorsing:
        //                    this.hasEndorsingOrSignedEndorsingSupportingTokens = true;
        //                    break;
        //                case SecurityTokenAttachmentMode.Signed:
        //                    this.hasAtLeastOneSupportingTokenExpectedToBeSigned = true;
        //                    break;
        //                case SecurityTokenAttachmentMode.SignedEndorsing:
        //                    this.hasEndorsingOrSignedEndorsingSupportingTokens = true;
        //                    this.hasAtLeastOneSupportingTokenExpectedToBeSigned = true;
        //                    break;
        //                case SecurityTokenAttachmentMode.SignedEncrypted:
        //                    this.hasAtLeastOneSupportingTokenExpectedToBeSigned = true;
        //                    break;
        //            }

        //            if ((this.primaryTokenAuthenticator != null) && (this.primaryTokenAuthenticator.GetType().Equals(spec.TokenAuthenticator.GetType())))
        //            {
        //                pendingSupportingTokenAuthenticator = spec.TokenAuthenticator;
        //            }
        //            else
        //            {
        //                allowedAuthenticators.Add(spec.TokenAuthenticator);
        //            }
        //            if (spec.TokenParameters.RequireDerivedKeys && !spec.TokenParameters.HasAsymmetricKey &&
        //                (spec.SecurityTokenAttachmentMode == SecurityTokenAttachmentMode.Endorsing || spec.SecurityTokenAttachmentMode == SecurityTokenAttachmentMode.SignedEndorsing))
        //            {
        //                ++numSupportingTokensRequiringDerivation;
        //            }
        //            this.supportingTokenTrackers.Add(new TokenTracker(spec));
        //        }
        //    }

        //    if (this.DerivedTokenAuthenticator != null)
        //    {
        //        // we expect key derivation. Compute quotas for derived keys
        //        int maxKeyDerivationLengthInBits = this.AlgorithmSuite.DefaultEncryptionKeyDerivationLength >= this.AlgorithmSuite.DefaultSignatureKeyDerivationLength ?
        //            this.AlgorithmSuite.DefaultEncryptionKeyDerivationLength : this.AlgorithmSuite.DefaultSignatureKeyDerivationLength;
        //        this.maxDerivedKeyLength = maxKeyDerivationLengthInBits / 8;
        //        // the upper bound of derived keys is (1 for primary signature + 1 for encryption + supporting token signatures requiring derivation)*2
        //        // the multiplication by 2 is to take care of interop scenarios that may arise that require more derived keys than the lower bound.
        //        this.maxDerivedKeys = (1 + 1 + numSupportingTokensRequiringDerivation) * 2;
        //    }

        //    SecurityHeaderElementInferenceEngine engine = SecurityHeaderElementInferenceEngine.GetInferenceEngine(this.Layout);
        //    engine.ExecuteProcessingPasses(this, reader);
        //    if (this.RequireMessageProtection)
        //    {
        //        this.ElementManager.EnsureAllRequiredSecurityHeaderTargetsWereProtected();
        //        ExecuteMessageProtectionPass(this.hasAtLeastOneSupportingTokenExpectedToBeSigned);
        //        if (this.RequiredSignatureParts != null && this.SignatureToken == null)
        //        {
        //            throw TraceUtility.ThrowHelperError(new MessageSecurityException(SR.Format(SR.RequiredSignatureMissing)), this.Message);
        //        }
        //    }

        //    EnsureDecryptionComplete();

        //    this.signatureTracker.SetDerivationSourceIfRequired();
        //    this.encryptionTracker.SetDerivationSourceIfRequired();
        //    if (this.EncryptionToken != null)
        //    {
        //        if (wrappingToken != null)
        //        {
        //            if (!(this.EncryptionToken is WrappedKeySecurityToken) || ((WrappedKeySecurityToken)this.EncryptionToken).WrappingToken != this.wrappingToken)
        //            {
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.EncryptedKeyWasNotEncryptedWithTheRequiredEncryptingToken, this.wrappingToken)));
        //            }
        //        }
        //        else if (expectedEncryptionToken != null)
        //        {
        //            if (this.EncryptionToken != expectedEncryptionToken)
        //            {
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.MessageWasNotEncryptedWithTheRequiredEncryptingToken)));
        //            }
        //        }
        //        else if (this.SignatureToken != null && this.EncryptionToken != this.SignatureToken)
        //        {
        //            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.SignatureAndEncryptionTokenMismatch, this.SignatureToken, this.EncryptionToken)));
        //        }
        //    }

        //    // ensure that the primary signature was signed with derived keys if required
        //    if (this.EnforceDerivedKeyRequirement)
        //    {
        //        if (this.SignatureToken != null)
        //        {
        //            if (this.primaryTokenParameters != null)
        //            {
        //                if (this.primaryTokenParameters.RequireDerivedKeys && !this.primaryTokenParameters.HasAsymmetricKey && !this.primaryTokenTracker.IsDerivedFrom)
        //                {
        //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.PrimarySignatureWasNotSignedByDerivedKey, this.primaryTokenParameters)));
        //                }
        //            }
        //            else if (this.wrappingTokenParameters != null && this.wrappingTokenParameters.RequireDerivedKeys)
        //            {
        //                if (!this.signatureTracker.IsDerivedToken)
        //                {
        //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.PrimarySignatureWasNotSignedByDerivedWrappedKey, this.wrappingTokenParameters)));
        //                }
        //            }
        //        }

        //        // verify that the encryption is using key derivation
        //        if (this.EncryptionToken != null)
        //        {
        //            if (wrappingTokenParameters != null)
        //            {
        //                if (wrappingTokenParameters.RequireDerivedKeys && !this.encryptionTracker.IsDerivedToken)
        //                {
        //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.MessageWasNotEncryptedByDerivedWrappedKey, this.wrappingTokenParameters)));
        //                }
        //            }
        //            else if (expectedEncryptionTokenParameters != null)
        //            {
        //                if (expectedEncryptionTokenParameters.RequireDerivedKeys && !this.encryptionTracker.IsDerivedToken)
        //                {
        //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.MessageWasNotEncryptedByDerivedEncryptionToken, this.expectedEncryptionTokenParameters)));
        //                }
        //            }
        //            else if (primaryTokenParameters != null && !primaryTokenParameters.HasAsymmetricKey && primaryTokenParameters.RequireDerivedKeys && !this.encryptionTracker.IsDerivedToken)
        //            {
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.MessageWasNotEncryptedByDerivedEncryptionToken, this.primaryTokenParameters)));
        //            }
        //        }
        //    }

        //    if (wasProtectionOrderDowngraded && (this.BasicSupportingTokens != null) && (this.BasicSupportingTokens.Count > 0))
        //    {
        //        // Basic tokens are always signed and encrypted. So check if Signatures 
        //        // are encrypted as well.
        //        this.VerifySignatureEncryption();
        //    }

        //    // verify all supporting token parameters have their requirements met
        //    if (this.supportingTokenTrackers != null)
        //    {
        //        for (int i = 0; i < this.supportingTokenTrackers.Count; ++i)
        //        {
        //            VerifySupportingToken(this.supportingTokenTrackers[i]);
        //        }
        //    }

        //    if (this.replayDetectionEnabled)
        //    {
        //        if (this.timestamp == null)
        //        {
        //            throw TraceUtility.ThrowHelperError(new MessageSecurityException(
        //                SR.Format(SR.NoTimestampAvailableInSecurityHeaderToDoReplayDetection)), this.Message);
        //        }
        //        if (this.primarySignatureValue == null)
        //        {
        //            throw TraceUtility.ThrowHelperError(new MessageSecurityException(
        //                SR.Format(SR.NoSignatureAvailableInSecurityHeaderToDoReplayDetection)), this.Message);
        //        }

        //        AddNonce(this.nonceCache, this.primarySignatureValue);

        //        // if replay detection is on, redo creation range checks to ensure full coverage
        //        this.timestamp.ValidateFreshness(this.replayWindow, this.clockSkew);
        //    }

        //    if (this.ExpectSignatureConfirmation)
        //    {
        //        this.ElementManager.VerifySignatureConfirmationWasFound();
        //    }

        //    MarkHeaderAsUnderstood();
        }

    }

    class TokenTracker
    {
        public SecurityToken _token;
        public bool _IsDerivedFrom;
        public bool _IsSigned;
        public bool _IsEncrypted;
        public bool _IsEndorsing;
        public bool _AlreadyReadEndorsingSignature;
        private bool _allowFirstTokenMismatch;
        public SupportingTokenAuthenticatorSpecification _spec;

        public TokenTracker(SupportingTokenAuthenticatorSpecification spec)
            : this(spec, null, false)
        {
        }

        public TokenTracker(SupportingTokenAuthenticatorSpecification spec, SecurityToken token, bool allowFirstTokenMismatch)
        {
            _spec = spec;
            _token = token;
            _allowFirstTokenMismatch = allowFirstTokenMismatch;
        }

        public void RecordToken(SecurityToken token)
        {
            if (_token == null)
            {
                _token = token;
            }
            else if (_allowFirstTokenMismatch)
            {
                if (!AreTokensEqual(_token, token))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SR.Format(SR.MismatchInSecurityOperationToken)));
                }
                _token = token;
                _allowFirstTokenMismatch = false;
            }
            else if (!object.ReferenceEquals(_token, token))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SR.Format(SR.MismatchInSecurityOperationToken)));
            }
        }

        static bool AreTokensEqual(SecurityToken outOfBandToken, SecurityToken replyToken)
        {
            // we support the serialized reply token legacy feature only for X509 certificates.
            // in this case the thumbprint of the reply certificate must match the outofband certificate's thumbprint
            if ((outOfBandToken is X509SecurityToken) && (replyToken is X509SecurityToken))
            {
                byte[] outOfBandCertificateThumbprint = ((X509SecurityToken)outOfBandToken).Certificate.GetCertHash();
                byte[] replyCertificateThumbprint = ((X509SecurityToken)replyToken).Certificate.GetCertHash();
                return (CryptoHelper.IsEqual(outOfBandCertificateThumbprint, replyCertificateThumbprint));
            }
            else
            {
                return false;
            }
        }
    }

}
