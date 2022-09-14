// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security.Tokens;
using System.Xml;

using SignatureResourcePool = System.IdentityModel.SignatureResourcePool;

namespace System.ServiceModel.Security
{
    internal abstract class ReceiveSecurityHeader : SecurityHeader
    {
        // client->server symmetric binding case: only primaryTokenAuthenticator is set
        // server->client symmetric binding case: only primary token is set
        // asymmetric binding case: primaryTokenAuthenticator and wrapping token is set

#pragma warning disable CS0649 // Field is never assign to
        private SecurityTokenAuthenticator _primaryTokenAuthenticator;
        private bool _allowFirstTokenMismatch;
        private SecurityToken _outOfBandPrimaryToken;
        private IList<SecurityToken> _outOfBandPrimaryTokenCollection;
        private SecurityTokenParameters _primaryTokenParameters;
        private TokenTracker _primaryTokenTracker;
        private SecurityToken _wrappingToken;
        private SecurityTokenParameters _wrappingTokenParameters;
        private SecurityTokenAuthenticator _derivedTokenAuthenticator;
        // assumes that the caller has done the check for uniqueness of types
        private IList<SupportingTokenAuthenticatorSpecification> _supportingTokenAuthenticators;
        private ChannelBinding _channelBinding;
        private ExtendedProtectionPolicy _extendedProtectionPolicy;

        private bool _expectBasicTokens;
        private bool _expectSignedTokens;
        private bool _expectEndorsingTokens;
        private bool _expectSignatureConfirmation;
        // maps from token to wire form (for basic and signed), and also tracks operations done
        // maps from supporting token parameter to the operations done for that token type
        private List<TokenTracker> _supportingTokenTrackers;

        private List<SecurityTokenAuthenticator> _allowedAuthenticators;

        private SecurityTokenAuthenticator _pendingSupportingTokenAuthenticator;
        private Collection<SecurityToken> _signedEndorsingTokens;
        private Dictionary<SecurityToken, ReadOnlyCollection<IAuthorizationPolicy>> _tokenPoliciesMapping;
        private SecurityTimestamp _timestamp;
        private SecurityHeaderTokenResolver _universalTokenResolver;
        private SecurityHeaderTokenResolver _primaryTokenResolver;
        private ReadOnlyCollection<SecurityTokenResolver> _outOfBandTokenResolver;
        private SecurityTokenResolver _combinedPrimaryTokenResolver;

        private XmlAttributeHolder[] _securityElementAttributes;
        private OrderTracker _orderTracker = new OrderTracker();
        private OperationTracker _signatureTracker = new OperationTracker();
        private OperationTracker _encryptionTracker = new OperationTracker();
        private int _maxDerivedKeys;
        private int _numDerivedKeys;
        private bool _enforceDerivedKeyRequirement = true;

        private NonceCache _nonceCache;
        private TimeSpan _replayWindow;
        private TimeSpan _clockSkew;
        private TimeoutHelper _timeoutHelper;
        private long _maxReceivedMessageSize = TransportDefaults.MaxReceivedMessageSize;
        private XmlDictionaryReaderQuotas _readerQuotas;
        private MessageProtectionOrder _protectionOrder;
        private bool _hasEndorsingOrSignedEndorsingSupportingTokens;
        private SignatureResourcePool _resourcePool;
        private bool _replayDetectionEnabled = false;
#pragma warning restore CS0649 // Field is never assign to

        private const int AppendPosition = -1;

        protected ReceiveSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite,
            int headerIndex,
            MessageDirection direction)
            : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, direction)
        {
            HeaderIndex = headerIndex;
            ElementManager = new ReceiveSecurityHeaderElementManager(this);
        }

        public Collection<SecurityToken> BasicSupportingTokens { get; }

        public Collection<SecurityToken> SignedSupportingTokens { get; }

        public Collection<SecurityToken> EndorsingSupportingTokens { get; }

        public Collection<SecurityToken> SignedEndorsingSupportingTokens
        {
            get
            {
                return _signedEndorsingTokens;
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

        public byte[] PrimarySignatureValue { get; }

        public SecurityToken EncryptionToken
        {
            get { return _encryptionTracker.Token; }
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

        public ReceiveSecurityHeaderElementManager ElementManager { get; }

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

        public bool ReplayDetectionEnabled
        {
            get { return _replayDetectionEnabled; }
            set
            {
                ThrowIfProcessingStarted();
                _replayDetectionEnabled = value;
            }
        }

        public bool ExpectSignatureConfirmation
        {
            get { return _expectSignatureConfirmation; }
            set
            {
                ThrowIfProcessingStarted();
                _expectSignatureConfirmation = value;
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

        public SecurityTokenResolver CombinedUniversalTokenResolver { get; private set; }

        internal int HeaderIndex { get; }

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

                _readerQuotas = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
            }
        }

        public override string Name
        {
            get { return StandardsManager.SecurityVersion.HeaderName.Value; }
        }

        public override string Namespace
        {
            get { return StandardsManager.SecurityVersion.HeaderNamespace.Value; }
        }

        public Message ProcessedMessage
        {
            get { return Message; }
        }

        protected SignatureResourcePool ResourcePool
        {
            get
            {
                if (_resourcePool == null)
                {
                    _resourcePool = new SignatureResourcePool();
                }
                return _resourcePool;
            }
        }

        internal SecurityVerifiedMessage SecurityVerifiedMessage { get; private set; }

        public SecurityToken SignatureToken
        {
            get { return _signatureTracker.Token; }
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

        public int MaxDerivedKeyLength { get; private set; }

        internal XmlDictionaryReader CreateSecurityHeaderReader()
        {
            return SecurityVerifiedMessage.GetReaderAtSecurityHeader();
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            XmlDictionaryReader securityHeaderReader = GetReaderAtSecurityHeader();
            securityHeaderReader.ReadStartElement();
            for (int i = 0; i < ElementManager.Count; ++i)
            {
                ReceiveSecurityHeaderEntry entry;
                ElementManager.GetElementEntry(i, out entry);
                XmlDictionaryReader reader = null;
                if (entry._encrypted)
                {
                    reader = ElementManager.GetReader(i, false);
                    writer.WriteNode(reader, false);
                    reader.Close();
                    securityHeaderReader.Skip();
                }
                else
                {
                    writer.WriteNode(securityHeaderReader, false);
                }
            }
            securityHeaderReader.Close();
        }

        private XmlDictionaryReader GetReaderAtSecurityHeader()
        {
            XmlDictionaryReader reader = SecurityVerifiedMessage.GetReaderAtFirstHeader();
            for (int i = 0; i < HeaderIndex; ++i)
            {
                reader.Skip();
            }

            return reader;
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
            Fx.Assert(ReaderQuotas != null, "Reader quotas must be set before processing");
            MessageProtectionOrder actualProtectionOrder = _protectionOrder;
            bool wasProtectionOrderDowngraded = false;
            if (_protectionOrder == MessageProtectionOrder.SignBeforeEncryptAndEncryptSignature)
            {
                throw ExceptionHelper.PlatformNotSupported(); // No support for message encryption
            }

            _channelBinding = channelBinding;
            _extendedProtectionPolicy = extendedProtectionPolicy;
            _orderTracker.SetRequiredProtectionOrder(actualProtectionOrder);

            SetProcessingStarted();
            _timeoutHelper = new TimeoutHelper(timeout);
            Message = SecurityVerifiedMessage = new SecurityVerifiedMessage(Message, this);
            XmlDictionaryReader reader = CreateSecurityHeaderReader();
            reader.MoveToStartElement();
            if (reader.IsEmptyElement)
            {
                throw TraceUtility.ThrowHelperError(new MessageSecurityException(SRP.SecurityHeaderIsEmpty), Message);
            }
            if (RequireMessageProtection)
            {
                _securityElementAttributes = XmlAttributeHolder.ReadAttributes(reader);
            }
            else
            {
                _securityElementAttributes = XmlAttributeHolder.emptyArray;
            }
            reader.ReadStartElement();

            if (_primaryTokenParameters != null)
            {
                _primaryTokenTracker = new TokenTracker(null, _outOfBandPrimaryToken, _allowFirstTokenMismatch);
            }
            // universalTokenResolver is used for resolving tokens
            _universalTokenResolver = new SecurityHeaderTokenResolver(this);
            // primary token resolver is used for resolving primary signature and decryption
            _primaryTokenResolver = new SecurityHeaderTokenResolver(this);
            if (_outOfBandPrimaryToken != null)
            {
                _universalTokenResolver.Add(_outOfBandPrimaryToken, SecurityTokenReferenceStyle.External, _primaryTokenParameters);
                _primaryTokenResolver.Add(_outOfBandPrimaryToken, SecurityTokenReferenceStyle.External, _primaryTokenParameters);
            }
            else if (_outOfBandPrimaryTokenCollection != null)
            {
                for (int i = 0; i < _outOfBandPrimaryTokenCollection.Count; ++i)
                {
                    _universalTokenResolver.Add(_outOfBandPrimaryTokenCollection[i], SecurityTokenReferenceStyle.External, _primaryTokenParameters);
                    _primaryTokenResolver.Add(_outOfBandPrimaryTokenCollection[i], SecurityTokenReferenceStyle.External, _primaryTokenParameters);
                }
            }
            if (_wrappingToken != null)
            {
                _universalTokenResolver.ExpectedWrapper = _wrappingToken;
                _universalTokenResolver.ExpectedWrapperTokenParameters = _wrappingTokenParameters;
                _primaryTokenResolver.ExpectedWrapper = _wrappingToken;
                _primaryTokenResolver.ExpectedWrapperTokenParameters = _wrappingTokenParameters;
            }

            if (_outOfBandTokenResolver == null)
            {
                CombinedUniversalTokenResolver = _universalTokenResolver;
                _combinedPrimaryTokenResolver = _primaryTokenResolver;
            }
            else
            {
                CombinedUniversalTokenResolver = new AggregateSecurityHeaderTokenResolver(_universalTokenResolver, _outOfBandTokenResolver);
                _combinedPrimaryTokenResolver = new AggregateSecurityHeaderTokenResolver(_primaryTokenResolver, _outOfBandTokenResolver);
            }

            _allowedAuthenticators = new List<SecurityTokenAuthenticator>();
            if (_primaryTokenAuthenticator != null)
            {
                _allowedAuthenticators.Add(_primaryTokenAuthenticator);
            }
            if (DerivedTokenAuthenticator != null)
            {
                _allowedAuthenticators.Add(DerivedTokenAuthenticator);
            }
            _pendingSupportingTokenAuthenticator = null;
            int numSupportingTokensRequiringDerivation = 0;
            if (_supportingTokenAuthenticators != null && _supportingTokenAuthenticators.Count > 0)
            {
                _supportingTokenTrackers = new List<TokenTracker>(_supportingTokenAuthenticators.Count);
                for (int i = 0; i < _supportingTokenAuthenticators.Count; ++i)
                {
                    SupportingTokenAuthenticatorSpecification spec = _supportingTokenAuthenticators[i];
                    switch (spec.SecurityTokenAttachmentMode)
                    {
                        case SecurityTokenAttachmentMode.Endorsing:
                            _hasEndorsingOrSignedEndorsingSupportingTokens = true;
                            break;
                        case SecurityTokenAttachmentMode.Signed:
                            break;
                        case SecurityTokenAttachmentMode.SignedEndorsing:
                            _hasEndorsingOrSignedEndorsingSupportingTokens = true;
                            break;
                        case SecurityTokenAttachmentMode.SignedEncrypted:
                            break;
                    }

                    if ((_primaryTokenAuthenticator != null) && (_primaryTokenAuthenticator.GetType().Equals(spec.TokenAuthenticator.GetType())))
                    {
                        _pendingSupportingTokenAuthenticator = spec.TokenAuthenticator;
                    }
                    else
                    {
                        _allowedAuthenticators.Add(spec.TokenAuthenticator);
                    }
                    if (spec.TokenParameters.RequireDerivedKeys && !spec.TokenParameters.HasAsymmetricKey &&
                        (spec.SecurityTokenAttachmentMode == SecurityTokenAttachmentMode.Endorsing || spec.SecurityTokenAttachmentMode == SecurityTokenAttachmentMode.SignedEndorsing))
                    {
                        ++numSupportingTokensRequiringDerivation;
                    }
                    _supportingTokenTrackers.Add(new TokenTracker(spec));
                }
            }

            if (DerivedTokenAuthenticator != null)
            {
                // we expect key derivation. Compute quotas for derived keys
                int maxKeyDerivationLengthInBits = AlgorithmSuite.DefaultEncryptionKeyDerivationLength >= AlgorithmSuite.DefaultSignatureKeyDerivationLength ?
                    AlgorithmSuite.DefaultEncryptionKeyDerivationLength : AlgorithmSuite.DefaultSignatureKeyDerivationLength;
                MaxDerivedKeyLength = maxKeyDerivationLengthInBits / 8;
                // the upper bound of derived keys is (1 for primary signature + 1 for encryption + supporting token signatures requiring derivation)*2
                // the multiplication by 2 is to take care of interop scenarios that may arise that require more derived keys than the lower bound.
                _maxDerivedKeys = (1 + 1 + numSupportingTokensRequiringDerivation) * 2;
            }

            SecurityHeaderElementInferenceEngine engine = SecurityHeaderElementInferenceEngine.GetInferenceEngine(Layout);
            engine.ExecuteProcessingPasses(this, reader);
            if (RequireMessageProtection)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            EnsureDecryptionComplete();

            _signatureTracker.SetDerivationSourceIfRequired();
            _encryptionTracker.SetDerivationSourceIfRequired();
            if (EncryptionToken != null)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            // ensure that the primary signature was signed with derived keys if required
            if (EnforceDerivedKeyRequirement)
            {
                if (SignatureToken != null)
                {
                    if (_primaryTokenParameters != null)
                    {
                        if (_primaryTokenParameters.RequireDerivedKeys && !_primaryTokenParameters.HasAsymmetricKey && !_primaryTokenTracker.IsDerivedFrom)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SRP.Format(SRP.PrimarySignatureWasNotSignedByDerivedKey, _primaryTokenParameters)));
                        }
                    }
                    else if (_wrappingTokenParameters != null && _wrappingTokenParameters.RequireDerivedKeys)
                    {
                        if (!_signatureTracker.IsDerivedToken)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SRP.Format(SRP.PrimarySignatureWasNotSignedByDerivedWrappedKey, _wrappingTokenParameters)));
                        }
                    }
                }

                // verify that the encryption is using key derivation
                if (EncryptionToken != null)
                {
                    throw ExceptionHelper.PlatformNotSupported();
                }
            }

            if (wasProtectionOrderDowngraded && (BasicSupportingTokens != null) && (BasicSupportingTokens.Count > 0))
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            // verify all supporting token parameters have their requirements met
            if (_supportingTokenTrackers != null && _supportingTokenTrackers.Count > 0)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            if (_replayDetectionEnabled)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            if (ExpectSignatureConfirmation)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            MarkHeaderAsUnderstood();
        }

        protected abstract void EnsureDecryptionComplete();

        internal void ExecuteFullPass(XmlDictionaryReader reader)
        {
            bool primarySignatureFound = !RequireMessageProtection;
            int position = 0;
            while (reader.IsStartElement())
            {
                if (IsReaderAtSignature(reader))
                {
                    throw ExceptionHelper.PlatformNotSupported();
                }
                else if (IsReaderAtReferenceList(reader))
                {
                    throw ExceptionHelper.PlatformNotSupported();
                }
                else if (StandardsManager.WSUtilitySpecificationVersion.IsReaderAtTimestamp(reader))
                {
                    ReadTimestamp(reader);
                }
                else if (IsReaderAtEncryptedKey(reader))
                {
                    throw ExceptionHelper.PlatformNotSupported();
                }
                else if (IsReaderAtEncryptedData(reader))
                {
                    throw ExceptionHelper.PlatformNotSupported();
                }
                else if (StandardsManager.SecurityVersion.IsReaderAtSignatureConfirmation(reader))
                {
                    throw ExceptionHelper.PlatformNotSupported();
                }
                else if (IsReaderAtSecurityTokenReference(reader))
                {
                    throw ExceptionHelper.PlatformNotSupported();
                }
                else
                {
                    ReadToken(reader, AppendPosition, null, null, null, _timeoutHelper.RemainingTime());
                }
                position++;
            }

            reader.ReadEndElement(); // wsse:Security
            reader.Close();
        }

        internal void EnsureDerivedKeyLimitNotReached()
        {
            ++_numDerivedKeys;
            if (_numDerivedKeys > _maxDerivedKeys)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SRP.Format(SRP.DerivedKeyLimitExceeded, _maxDerivedKeys)));
            }
        }

        protected TokenTracker GetSupportingTokenTracker(SecurityTokenAuthenticator tokenAuthenticator, out SupportingTokenAuthenticatorSpecification spec)
        {
            spec = null;
            if (_supportingTokenAuthenticators == null)
            {
                return null;
            }

            for (int i = 0; i < _supportingTokenAuthenticators.Count; ++i)
            {
                if (_supportingTokenAuthenticators[i].TokenAuthenticator == tokenAuthenticator)
                {
                    spec = _supportingTokenAuthenticators[i];
                    return _supportingTokenTrackers[i];
                }
            }
            return null;
        }

        private void ReadTimestamp(XmlDictionaryReader reader)
        {
            if (_timestamp != null)
            {
                throw TraceUtility.ThrowHelperError(new MessageSecurityException(SRP.DuplicateTimestampInSecurityHeader), Message);
            }
            bool expectTimestampToBeSigned = RequireMessageProtection || _hasEndorsingOrSignedEndorsingSupportingTokens;
            string expectedDigestAlgorithm = expectTimestampToBeSigned ? AlgorithmSuite.DefaultDigestAlgorithm : null;
            SignatureResourcePool resourcePool = expectTimestampToBeSigned ? ResourcePool : null;
            _timestamp = StandardsManager.WSUtilitySpecificationVersion.ReadTimestamp(reader, expectedDigestAlgorithm, resourcePool);
            _timestamp.ValidateRangeAndFreshness(_replayWindow, _clockSkew);
            ElementManager.AppendTimestamp(_timestamp);
        }

        private bool IsPrimaryToken(SecurityToken token)
        {
            bool result = (token == _outOfBandPrimaryToken
                || (_primaryTokenTracker != null && token == _primaryTokenTracker.Token));
            if (!result && _outOfBandPrimaryTokenCollection != null)
            {
                for (int i = 0; i < _outOfBandPrimaryTokenCollection.Count; ++i)
                {
                    if (_outOfBandPrimaryTokenCollection[i] == token)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        private void ReadToken(XmlDictionaryReader reader, int position, byte[] decryptedBuffer,
            SecurityToken encryptionToken, string idInEncryptedForm, TimeSpan timeout)
        {
            Fx.Assert((position == AppendPosition) == (decryptedBuffer == null), "inconsistent position, decryptedBuffer parameters");
            Fx.Assert((position == AppendPosition) == (encryptionToken == null), "inconsistent position, encryptionToken parameters");
            string localName = reader.LocalName;
            string namespaceUri = reader.NamespaceURI;
            string valueType = reader.GetAttribute(XD.SecurityJan2004Dictionary.ValueType, null);

            SecurityTokenAuthenticator usedTokenAuthenticator;
            SecurityToken token = ReadToken(reader, CombinedUniversalTokenResolver, _allowedAuthenticators, out usedTokenAuthenticator);
            if (token == null)
            {
                throw TraceUtility.ThrowHelperError(new MessageSecurityException(SRP.Format(SRP.TokenManagerCouldNotReadToken, localName, namespaceUri, valueType)), Message);
            }

            DerivedKeySecurityToken derivedKeyToken = token as DerivedKeySecurityToken;
            if (derivedKeyToken != null)
            {
                EnsureDerivedKeyLimitNotReached();
                derivedKeyToken.InitializeDerivedKey(MaxDerivedKeyLength);
            }

            if (usedTokenAuthenticator == _primaryTokenAuthenticator)
            {
                _allowedAuthenticators.Remove(usedTokenAuthenticator);
            }

            ReceiveSecurityHeaderBindingModes mode;
            TokenTracker supportingTokenTracker = null;
            if (usedTokenAuthenticator == _primaryTokenAuthenticator)
            {
                // this is the primary token. Add to resolver as such
                _universalTokenResolver.Add(token, SecurityTokenReferenceStyle.Internal, _primaryTokenParameters);
                _primaryTokenResolver.Add(token, SecurityTokenReferenceStyle.Internal, _primaryTokenParameters);
                if (_pendingSupportingTokenAuthenticator != null)
                {
                    _allowedAuthenticators.Add(_pendingSupportingTokenAuthenticator);
                    _pendingSupportingTokenAuthenticator = null;
                }

                _primaryTokenTracker.RecordToken(token);
                mode = ReceiveSecurityHeaderBindingModes.Primary;
            }
            else if (usedTokenAuthenticator == DerivedTokenAuthenticator)
            {
                if (token is DerivedKeySecurityTokenStub)
                {
                    if (Layout == SecurityHeaderLayout.Strict)
                    {
                        DerivedKeySecurityTokenStub tmpToken = (DerivedKeySecurityTokenStub)token;
                        throw TraceUtility.ThrowHelperError(new MessageSecurityException(
                            SRP.Format(SRP.UnableToResolveKeyInfoClauseInDerivedKeyToken, tmpToken.TokenToDeriveIdentifier)), Message);
                    }
                }
                else
                {
                    AddDerivedKeyTokenToResolvers(token);
                }

                mode = ReceiveSecurityHeaderBindingModes.Unknown;
            }
            else
            {
                SupportingTokenAuthenticatorSpecification supportingTokenSpec;
                supportingTokenTracker = GetSupportingTokenTracker(usedTokenAuthenticator, out supportingTokenSpec);
                if (supportingTokenTracker == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SRP.Format(SRP.UnknownTokenAuthenticatorUsedInTokenProcessing, usedTokenAuthenticator)));
                }

                if (supportingTokenTracker.Token != null)
                {
                    supportingTokenTracker = new TokenTracker(supportingTokenSpec);
                    _supportingTokenTrackers.Add(supportingTokenTracker);
                }

                supportingTokenTracker.RecordToken(token);
                if (encryptionToken != null)
                {
                    supportingTokenTracker.IsEncrypted = true;
                }

                bool isBasic;
                bool isSignedButNotBasic;
                SecurityTokenAttachmentModeHelper.Categorize(supportingTokenSpec.SecurityTokenAttachmentMode, out isBasic, out isSignedButNotBasic, out mode);
                if (isBasic)
                {
                    if (!ExpectBasicTokens)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SRP.BasicTokenNotExpected));
                    }

                    // only basic tokens have to be part of the reference list. Encrypted Saml tokens dont for example
                    if (RequireMessageProtection && encryptionToken != null)
                    {
                        throw ExceptionHelper.PlatformNotSupported();
                    }
                }

                if (isSignedButNotBasic && !ExpectSignedTokens)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SRP.SignedSupportingTokenNotExpected));
                }

                _universalTokenResolver.Add(token, SecurityTokenReferenceStyle.Internal, supportingTokenSpec.TokenParameters);
            }

            if (position == AppendPosition)
            {
                ElementManager.AppendToken(token, mode, supportingTokenTracker);
            }
            else
            {
                ElementManager.SetTokenAfterDecryption(position, token, mode, decryptedBuffer, supportingTokenTracker);
            }
        }

        private SecurityToken GetRootToken(SecurityToken token)
        {
            if (token is DerivedKeySecurityToken)
            {
                return ((DerivedKeySecurityToken)token).TokenToDerive;
            }
            else
            {
                return token;
            }
        }

        private SecurityToken ReadToken(XmlReader reader, SecurityTokenResolver tokenResolver, IList<SecurityTokenAuthenticator> allowedTokenAuthenticators, out SecurityTokenAuthenticator usedTokenAuthenticator)
        {
            SecurityToken token = StandardsManager.SecurityTokenSerializer.ReadToken(reader, tokenResolver);
            if (token is DerivedKeySecurityTokenStub)
            {
                if (DerivedTokenAuthenticator == null)
                {
                    // No Authenticator registered for DerivedKeySecurityToken
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(
                        SRP.Format(SRP.UnableToFindTokenAuthenticator, typeof(DerivedKeySecurityToken))));
                }

                // This is just the stub. Nothing to Validate. Set the usedTokenAuthenticator to 
                // DerivedKeySecurityTokenAuthenticator.
                usedTokenAuthenticator = DerivedTokenAuthenticator;
                return token;
            }

            for (int i = 0; i < allowedTokenAuthenticators.Count; ++i)
            {
                SecurityTokenAuthenticator tokenAuthenticator = allowedTokenAuthenticators[i];
                if (tokenAuthenticator.CanValidateToken(token))
                {
                    ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies;
                    authorizationPolicies = tokenAuthenticator.ValidateToken(token);
                    SecurityTokenAuthorizationPoliciesMapping.Add(token, authorizationPolicies);
                    usedTokenAuthenticator = tokenAuthenticator;
                    return token;
                }
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(
                SRP.Format(SRP.UnableToFindTokenAuthenticator, token.GetType())));
        }

        private void AddDerivedKeyTokenToResolvers(SecurityToken token)
        {
            _universalTokenResolver.Add(token);
            // add it to the primary token resolver only if its root is primary
            SecurityToken rootToken = GetRootToken(token);
            if (IsPrimaryToken(rootToken))
            {
                _primaryTokenResolver.Add(token);
            }
        }

        protected abstract bool IsReaderAtEncryptedKey(XmlDictionaryReader reader);
        protected abstract bool IsReaderAtEncryptedData(XmlDictionaryReader reader);
        protected abstract bool IsReaderAtReferenceList(XmlDictionaryReader reader);
        protected abstract bool IsReaderAtSignature(XmlDictionaryReader reader);
        protected abstract bool IsReaderAtSecurityTokenReference(XmlDictionaryReader reader);

        private void MarkHeaderAsUnderstood()
        {
            // header decryption does not reorder or delete headers
            MessageHeaderInfo header = Message.Headers[HeaderIndex];
            Fx.Assert(header.Name == Name && header.Namespace == Namespace && header.Actor == Actor, "security header index mismatch");
            Message.Headers.UnderstoodHeaders.Add(header);
        }

        private struct OrderTracker
        {
            private static readonly ReceiverProcessingOrder[] s_stateTransitionTableOnDecrypt = new ReceiverProcessingOrder[]
                {
                    ReceiverProcessingOrder.Decrypt, ReceiverProcessingOrder.VerifyDecrypt, ReceiverProcessingOrder.Decrypt,
                    ReceiverProcessingOrder.Mixed, ReceiverProcessingOrder.VerifyDecrypt, ReceiverProcessingOrder.Mixed
                };
            private static readonly ReceiverProcessingOrder[] s_stateTransitionTableOnVerify = new ReceiverProcessingOrder[]
                {
                    ReceiverProcessingOrder.Verify, ReceiverProcessingOrder.Verify, ReceiverProcessingOrder.DecryptVerify,
                    ReceiverProcessingOrder.DecryptVerify, ReceiverProcessingOrder.Mixed, ReceiverProcessingOrder.Mixed
                };

            private const int MaxAllowedWrappedKeys = 1;

            private int _referenceListCount;
            private ReceiverProcessingOrder _state;
#pragma warning disable CS0649 // Field is never assign to
            private int _signatureCount;
            private int _unencryptedSignatureCount;
#pragma warning restore CS0649 // Field is never assign to
            private MessageProtectionOrder _protectionOrder;
            private bool _enforce;

            public bool AllSignaturesEncrypted
            {
                get { return _unencryptedSignatureCount == 0; }
            }

            public bool EncryptBeforeSignMode
            {
                get { return _enforce && _protectionOrder == MessageProtectionOrder.EncryptBeforeSign; }
            }

            public bool EncryptBeforeSignOrderRequirementMet
            {
                get { return _state != ReceiverProcessingOrder.DecryptVerify && _state != ReceiverProcessingOrder.Mixed; }
            }

            public bool PrimarySignatureDone
            {
                get { return _signatureCount > 0; }
            }

            public bool SignBeforeEncryptOrderRequirementMet
            {
                get { return _state != ReceiverProcessingOrder.VerifyDecrypt && _state != ReceiverProcessingOrder.Mixed; }
            }

            private void EnforceProtectionOrder()
            {
                switch (_protectionOrder)
                {
                    case MessageProtectionOrder.SignBeforeEncryptAndEncryptSignature:
                        throw ExceptionHelper.PlatformNotSupported();
                    case MessageProtectionOrder.SignBeforeEncrypt:
                        if (!SignBeforeEncryptOrderRequirementMet)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(
                                SRP.Format(SRP.MessageProtectionOrderMismatch, _protectionOrder)));
                        }
                        break;
                    case MessageProtectionOrder.EncryptBeforeSign:
                        throw ExceptionHelper.PlatformNotSupported();
                    default:
                        Fx.Assert("");
                        break;
                }
            }

            public void OnProcessReferenceList()
            {
                Fx.Assert(_enforce, "OrderTracker should have 'enforce' set to true.");
                if (_referenceListCount > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(
                        SRP.AtMostOneReferenceListIsSupportedWithDefaultPolicyCheck));
                }

                _referenceListCount++;
                _state = s_stateTransitionTableOnDecrypt[(int)_state];
                if (_enforce)
                {
                    EnforceProtectionOrder();
                }
            }

            public void SetRequiredProtectionOrder(MessageProtectionOrder protectionOrder)
            {
                _protectionOrder = protectionOrder;
                _enforce = true;
            }

            private enum ReceiverProcessingOrder : int
            {
                None = 0,
                Verify = 1,
                Decrypt = 2,
                DecryptVerify = 3,
                VerifyDecrypt = 4,
                Mixed = 5
            }
        }

        private struct OperationTracker
        {
            private bool _isDerivedToken;

            public MessagePartSpecification Parts { get; set; }

            public SecurityToken Token { get; private set; }

            public bool IsDerivedToken
            {
                get { return _isDerivedToken; }
            }

            public void RecordToken(SecurityToken token)
            {
                if (Token == null)
                {
                    Token = token;
                }
                else if (!ReferenceEquals(Token, token))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.MismatchInSecurityOperationToken));
                }
            }

            public void SetDerivationSourceIfRequired()
            {
                DerivedKeySecurityToken derivedKeyToken = Token as DerivedKeySecurityToken;
                if (derivedKeyToken != null)
                {
                    Token = derivedKeyToken.TokenToDerive;
                    _isDerivedToken = true;
                }
            }
        }
    }

    internal class TokenTracker
    {
        private bool _allowFirstTokenMismatch;
        public SupportingTokenAuthenticatorSpecification spec;

        public SecurityToken Token { get; set; }
        public bool IsDerivedFrom { get; set; }
        public bool IsSigned { get; set; }
        public bool IsEncrypted { get; set; }
        public bool IsEndorsing { get; set; }
        public bool AlreadyReadEndorsingSignature { get; set; }

        public TokenTracker(SupportingTokenAuthenticatorSpecification spec)
            : this(spec, null, false)
        {
        }

        public TokenTracker(SupportingTokenAuthenticatorSpecification spec, SecurityToken token, bool allowFirstTokenMismatch)
        {
            this.spec = spec;
            Token = token;
            _allowFirstTokenMismatch = allowFirstTokenMismatch;
        }

        public void RecordToken(SecurityToken token)
        {
            if (Token == null)
            {
                Token = token;
            }
            else if (_allowFirstTokenMismatch)
            {
                if (!AreTokensEqual(Token, token))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.MismatchInSecurityOperationToken));
                }
                Token = token;
                _allowFirstTokenMismatch = false;
            }
            else if (!object.ReferenceEquals(Token, token))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.MismatchInSecurityOperationToken));
            }
        }

        private static bool AreTokensEqual(SecurityToken outOfBandToken, SecurityToken replyToken)
        {
            // we support the serialized reply token legacy feature only for X509 certificates.
            // in this case the thumbprint of the reply certificate must match the outofband certificate's thumbprint
            if ((outOfBandToken is X509SecurityToken) && (replyToken is X509SecurityToken))
            {
                byte[] outOfBandCertificateThumbprint = ((X509SecurityToken)outOfBandToken).Certificate.GetCertHash();
                byte[] replyCertificateThumbprint = ((X509SecurityToken)replyToken).Certificate.GetCertHash();
                return (SecurityUtils.IsEqual(outOfBandCertificateThumbprint, replyCertificateThumbprint));
            }
            else
            {
                return false;
            }
        }
    }

    internal class AggregateSecurityHeaderTokenResolver : AggregateTokenResolver
    {
        private SecurityHeaderTokenResolver _tokenResolver;

        public AggregateSecurityHeaderTokenResolver(SecurityHeaderTokenResolver tokenResolver, ReadOnlyCollection<SecurityTokenResolver> outOfBandTokenResolvers) :
            base(outOfBandTokenResolvers)
        {
            _tokenResolver = tokenResolver ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenResolver));
        }

        protected override bool TryResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
        {
            bool resolved = false;
            key = null;

            resolved = _tokenResolver.TryResolveSecurityKey(keyIdentifierClause, false, out key);

            if (!resolved)
            {
                resolved = base.TryResolveSecurityKeyCore(keyIdentifierClause, out key);
            }

            if (!resolved)
            {
                resolved = SecurityUtils.TryCreateKeyFromIntrinsicKeyClause(keyIdentifierClause, this, out key);
            }

            return resolved;
        }

        protected override bool TryResolveTokenCore(SecurityKeyIdentifier keyIdentifier, out SecurityToken token)
        {
            bool resolved = false;
            token = null;

            resolved = _tokenResolver.TryResolveToken(keyIdentifier, false, false, out token);

            if (!resolved)
            {
                resolved = base.TryResolveTokenCore(keyIdentifier, out token);
            }

            if (!resolved)
            {
                for (int i = 0; i < keyIdentifier.Count; ++i)
                {
                    if (TryResolveTokenFromIntrinsicKeyClause(keyIdentifier[i], out token))
                    {
                        resolved = true;
                        break;
                    }
                }
            }

            return resolved;
        }

        private bool TryResolveTokenFromIntrinsicKeyClause(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityToken token)
        {
            token = null;
            if (keyIdentifierClause is X509RawDataKeyIdentifierClause)
            {
                token = new X509SecurityToken(new X509Certificate2(((X509RawDataKeyIdentifierClause)keyIdentifierClause).GetX509RawData()), false);
                return true;
            }
            else if (keyIdentifierClause is EncryptedKeyIdentifierClause)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            return false;
        }

        protected override bool TryResolveTokenCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityToken token)
        {
            bool resolved = false;
            token = null;

            resolved = _tokenResolver.TryResolveToken(keyIdentifierClause, false, false, out token);

            if (!resolved)
            {
                resolved = base.TryResolveTokenCore(keyIdentifierClause, out token);
            }

            if (!resolved)
            {
                resolved = TryResolveTokenFromIntrinsicKeyClause(keyIdentifierClause, out token);
            }

            return resolved;
        }
    }
}
