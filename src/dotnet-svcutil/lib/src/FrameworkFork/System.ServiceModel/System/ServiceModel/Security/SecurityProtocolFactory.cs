// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security.Tokens;
using System.Globalization;

namespace System.ServiceModel.Security
{
    /*
     * See
     * http://xws/gxa/main/specs/security/security_profiles/SecurityProfiles.doc
     * for details on security protocols

     * Concrete implementations are required to me thread safe after
     * Open() is called;

     * instances of concrete protocol factories are scoped to a
     * channel/listener factory;

     * Each channel/listener factory must have a
     * SecurityProtocolFactory set on it before open/first use; the
     * factory instance cannot be changed once the factory is opened
     * or listening;

     * security protocol instances are scoped to a channel and will be
     * created by the Create calls on protocol factories;

     * security protocol instances are required to be thread-safe.

     * for typical subclasses, factory wide state and immutable
     * settings are expected to be on the ProtocolFactory itself while
     * channel-wide state is maintained internally in each security
     * protocol instance;

     * the security protocol instance set on a channel cannot be
     * changed; however, the protocol instance may change internal
     * state; this covers RM's SCT renego case; by keeping state
     * change internal to protocol instances, we get better
     * coordination with concurrent message security on channels;

     * the primary pivot in creating a security protocol instance is
     * initiator (client) vs. responder (server), NOT sender vs
     * receiver

     * Create calls for input and reply channels will contain the
     * listener-wide state (if any) created by the corresponding call
     * on the factory;

     */

    // Whether we need to add support for targetting different SOAP roles is tracked by 19144

    internal abstract class SecurityProtocolFactory : ISecurityCommunicationObject
    {
        internal const bool defaultAddTimestamp = true;
        internal const bool defaultDeriveKeys = true;
        internal const bool defaultDetectReplays = true;
        internal const string defaultMaxClockSkewString = "00:05:00";
        internal const string defaultReplayWindowString = "00:05:00";
        internal static readonly TimeSpan defaultMaxClockSkew = TimeSpan.Parse(defaultMaxClockSkewString, CultureInfo.InvariantCulture);
        internal static readonly TimeSpan defaultReplayWindow = TimeSpan.Parse(defaultReplayWindowString, CultureInfo.InvariantCulture);
        internal const int defaultMaxCachedNonces = 900000;
        internal const string defaultTimestampValidityDurationString = "00:05:00";
        internal static readonly TimeSpan defaultTimestampValidityDuration = TimeSpan.Parse(defaultTimestampValidityDurationString, CultureInfo.InvariantCulture);
        internal const SecurityHeaderLayout defaultSecurityHeaderLayout = SecurityHeaderLayout.Strict;

        private static ReadOnlyCollection<SupportingTokenAuthenticatorSpecification> s_emptyTokenAuthenticators;

        private bool _actAsInitiator;
        private bool _isDuplexReply;
        private bool _addTimestamp = defaultAddTimestamp;
        private bool _detectReplays = defaultDetectReplays;
        private bool _expectIncomingMessages;
        private bool _expectOutgoingMessages;
        private SecurityAlgorithmSuite _incomingAlgorithmSuite = SecurityAlgorithmSuite.Default;


        // per receiver protocol factory lists
        private ICollection<SupportingTokenAuthenticatorSpecification> _channelSupportingTokenAuthenticatorSpecification;
        private Dictionary<string, ICollection<SupportingTokenAuthenticatorSpecification>> _scopedSupportingTokenAuthenticatorSpecification;
        private Dictionary<string, MergedSupportingTokenAuthenticatorSpecification> _mergedSupportingTokenAuthenticatorsMap;

        private int _maxCachedNonces = defaultMaxCachedNonces;
        private TimeSpan _maxClockSkew = defaultMaxClockSkew;
        private NonceCache _nonceCache = null;
        private SecurityAlgorithmSuite _outgoingAlgorithmSuite = SecurityAlgorithmSuite.Default;
        private TimeSpan _replayWindow = defaultReplayWindow;
        private SecurityStandardsManager _standardsManager = SecurityStandardsManager.DefaultInstance;
        private SecurityTokenManager _securityTokenManager;
        private SecurityBindingElement _securityBindingElement;
        private string _requestReplyErrorPropertyName;
        private NonValidatingSecurityTokenAuthenticator<DerivedKeySecurityToken> _derivedKeyTokenAuthenticator;
        private TimeSpan _timestampValidityDuration = defaultTimestampValidityDuration;
        private AuditLogLocation _auditLogLocation;
        private bool _suppressAuditFailure;
        private SecurityHeaderLayout _securityHeaderLayout;
        private AuditLevel _serviceAuthorizationAuditLevel;
        private AuditLevel _messageAuthenticationAuditLevel;
        private bool _expectKeyDerivation;
        private bool _expectChannelBasicTokens;
        private bool _expectChannelSignedTokens;
        private bool _expectChannelEndorsingTokens;
        private bool _expectSupportingTokens;
        private Uri _listenUri;
        private MessageSecurityVersion _messageSecurityVersion;
        private WrapperSecurityCommunicationObject _communicationObject;
        private Uri _privacyNoticeUri;
        private int _privacyNoticeVersion;
        private ExtendedProtectionPolicy _extendedProtectionPolicy;
        private BufferManager _streamBufferManager = null;

        protected SecurityProtocolFactory()
        {
            _channelSupportingTokenAuthenticatorSpecification = new Collection<SupportingTokenAuthenticatorSpecification>();
            _scopedSupportingTokenAuthenticatorSpecification = new Dictionary<string, ICollection<SupportingTokenAuthenticatorSpecification>>();
            _communicationObject = new WrapperSecurityCommunicationObject(this);
        }

        internal SecurityProtocolFactory(SecurityProtocolFactory factory)
            : this()
        {
            if (factory == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("factory");
            }

            _actAsInitiator = factory._actAsInitiator;
            _addTimestamp = factory._addTimestamp;
            _detectReplays = factory._detectReplays;
            _incomingAlgorithmSuite = factory._incomingAlgorithmSuite;
            _maxCachedNonces = factory._maxCachedNonces;
            _maxClockSkew = factory._maxClockSkew;
            _outgoingAlgorithmSuite = factory._outgoingAlgorithmSuite;
            _replayWindow = factory._replayWindow;
            _channelSupportingTokenAuthenticatorSpecification = new Collection<SupportingTokenAuthenticatorSpecification>(new List<SupportingTokenAuthenticatorSpecification>(factory._channelSupportingTokenAuthenticatorSpecification));
            _scopedSupportingTokenAuthenticatorSpecification = new Dictionary<string, ICollection<SupportingTokenAuthenticatorSpecification>>(factory._scopedSupportingTokenAuthenticatorSpecification);
            _standardsManager = factory._standardsManager;
            _timestampValidityDuration = factory._timestampValidityDuration;
            _auditLogLocation = factory._auditLogLocation;
            _suppressAuditFailure = factory._suppressAuditFailure;
            _serviceAuthorizationAuditLevel = factory._serviceAuthorizationAuditLevel;
            _messageAuthenticationAuditLevel = factory._messageAuthenticationAuditLevel;
            if (factory._securityBindingElement != null)
            {
                _securityBindingElement = (SecurityBindingElement)factory._securityBindingElement.Clone();
            }
            _securityTokenManager = factory._securityTokenManager;
            _privacyNoticeUri = factory._privacyNoticeUri;
            _privacyNoticeVersion = factory._privacyNoticeVersion;
            _extendedProtectionPolicy = factory._extendedProtectionPolicy;
            _nonceCache = factory._nonceCache;
        }

        protected WrapperSecurityCommunicationObject CommunicationObject
        {
            get { return _communicationObject; }
        }

        // The ActAsInitiator value is set automatically on Open and
        // remains unchanged thereafter.  ActAsInitiator is true for
        // the initiator of the message exchange, such as the sender
        // of a datagram, sender of a request and sender of either leg
        // of a duplex exchange.
        public bool ActAsInitiator
        {
            get
            {
                return _actAsInitiator;
            }
        }

        public BufferManager StreamBufferManager
        {
            get
            {
                if (_streamBufferManager == null)
                {
                    _streamBufferManager = BufferManager.CreateBufferManager(0, int.MaxValue);
                }

                return _streamBufferManager;
            }
            set
            {
                _streamBufferManager = value;
            }
        }

        internal bool IsDuplexReply
        {
            get
            {
                return _isDuplexReply;
            }
            set
            {
                _isDuplexReply = value;
            }
        }

        public bool AddTimestamp
        {
            get
            {
                return _addTimestamp;
            }
            set
            {
                ThrowIfImmutable();
                _addTimestamp = value;
            }
        }

        public AuditLogLocation AuditLogLocation
        {
            get
            {
                return _auditLogLocation;
            }
            set
            {
                ThrowIfImmutable();
                AuditLogLocationHelper.Validate(value);
                _auditLogLocation = value;
            }
        }

        public bool SuppressAuditFailure
        {
            get
            {
                return _suppressAuditFailure;
            }
            set
            {
                ThrowIfImmutable();
                _suppressAuditFailure = value;
            }
        }

        public AuditLevel ServiceAuthorizationAuditLevel
        {
            get
            {
                return _serviceAuthorizationAuditLevel;
            }
            set
            {
                ThrowIfImmutable();
                AuditLevelHelper.Validate(value);
                _serviceAuthorizationAuditLevel = value;
            }
        }

        public AuditLevel MessageAuthenticationAuditLevel
        {
            get
            {
                return _messageAuthenticationAuditLevel;
            }
            set
            {
                ThrowIfImmutable();
                AuditLevelHelper.Validate(value);
                _messageAuthenticationAuditLevel = value;
            }
        }


        public bool DetectReplays
        {
            get
            {
                return _detectReplays;
            }
            set
            {
                ThrowIfImmutable();
                _detectReplays = value;
            }
        }

        public Uri PrivacyNoticeUri
        {
            get
            {
                return _privacyNoticeUri;
            }
            set
            {
                ThrowIfImmutable();
                _privacyNoticeUri = value;
            }
        }

        public int PrivacyNoticeVersion
        {
            get
            {
                return _privacyNoticeVersion;
            }
            set
            {
                ThrowIfImmutable();
                _privacyNoticeVersion = value;
            }
        }

        private static ReadOnlyCollection<SupportingTokenAuthenticatorSpecification> EmptyTokenAuthenticators
        {
            get
            {
                if (s_emptyTokenAuthenticators == null)
                {
                    s_emptyTokenAuthenticators = new ReadOnlyCollection<SupportingTokenAuthenticatorSpecification>(new SupportingTokenAuthenticatorSpecification[0]);
                }
                return s_emptyTokenAuthenticators;
            }
        }

        internal NonValidatingSecurityTokenAuthenticator<DerivedKeySecurityToken> DerivedKeyTokenAuthenticator
        {
            get
            {
                return _derivedKeyTokenAuthenticator;
            }
        }

        internal bool ExpectIncomingMessages
        {
            get
            {
                return _expectIncomingMessages;
            }
        }

        internal bool ExpectOutgoingMessages
        {
            get
            {
                return _expectOutgoingMessages;
            }
        }

        internal bool ExpectKeyDerivation
        {
            get { return _expectKeyDerivation; }
            set { _expectKeyDerivation = value; }
        }

        internal bool ExpectSupportingTokens
        {
            get { return _expectSupportingTokens; }
            set { _expectSupportingTokens = value; }
        }

        public SecurityAlgorithmSuite IncomingAlgorithmSuite
        {
            get
            {
                return _incomingAlgorithmSuite;
            }
            set
            {
                ThrowIfImmutable();
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                }
                _incomingAlgorithmSuite = value;
            }
        }

        protected bool IsReadOnly
        {
            get
            {
                return this.CommunicationObject.State != CommunicationState.Created;
            }
        }

        public int MaxCachedNonces
        {
            get
            {
                return _maxCachedNonces;
            }
            set
            {
                ThrowIfImmutable();
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                _maxCachedNonces = value;
            }
        }

        public TimeSpan MaxClockSkew
        {
            get
            {
                return _maxClockSkew;
            }
            set
            {
                ThrowIfImmutable();
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                _maxClockSkew = value;
            }
        }

        public NonceCache NonceCache
        {
            get
            {
                return _nonceCache;
            }
            set
            {
                ThrowIfImmutable();
                _nonceCache = value;
            }
        }

        public SecurityAlgorithmSuite OutgoingAlgorithmSuite
        {
            get
            {
                return _outgoingAlgorithmSuite;
            }
            set
            {
                ThrowIfImmutable();
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                }
                _outgoingAlgorithmSuite = value;
            }
        }

        public TimeSpan ReplayWindow
        {
            get
            {
                return _replayWindow;
            }
            set
            {
                ThrowIfImmutable();
                if (value <= TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", SRServiceModel.TimeSpanMustbeGreaterThanTimeSpanZero));
                }
                _replayWindow = value;
            }
        }

        public ICollection<SupportingTokenAuthenticatorSpecification> ChannelSupportingTokenAuthenticatorSpecification
        {
            get
            {
                return _channelSupportingTokenAuthenticatorSpecification;
            }
        }

        public Dictionary<string, ICollection<SupportingTokenAuthenticatorSpecification>> ScopedSupportingTokenAuthenticatorSpecification
        {
            get
            {
                return _scopedSupportingTokenAuthenticatorSpecification;
            }
        }

        public SecurityBindingElement SecurityBindingElement
        {
            get { return _securityBindingElement; }
            set
            {
                ThrowIfImmutable();
                if (value != null)
                {
                    value = (SecurityBindingElement)value.Clone();
                }
                _securityBindingElement = value;
            }
        }

        public SecurityTokenManager SecurityTokenManager
        {
            get { return _securityTokenManager; }
            set
            {
                ThrowIfImmutable();
                _securityTokenManager = value;
            }
        }

        public virtual bool SupportsDuplex
        {
            get
            {
                return false;
            }
        }

        public SecurityHeaderLayout SecurityHeaderLayout
        {
            get
            {
                return _securityHeaderLayout;
            }
            set
            {
                ThrowIfImmutable();
                _securityHeaderLayout = value;
            }
        }

        public virtual bool SupportsReplayDetection
        {
            get
            {
                return true;
            }
        }

        public virtual bool SupportsRequestReply
        {
            get
            {
                return true;
            }
        }

        public SecurityStandardsManager StandardsManager
        {
            get
            {
                return _standardsManager;
            }
            set
            {
                ThrowIfImmutable();
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                }
                _standardsManager = value;
            }
        }

        public TimeSpan TimestampValidityDuration
        {
            get
            {
                return _timestampValidityDuration;
            }
            set
            {
                ThrowIfImmutable();
                if (value <= TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", SRServiceModel.TimeSpanMustbeGreaterThanTimeSpanZero));
                }
                _timestampValidityDuration = value;
            }
        }

        public Uri ListenUri
        {
            get { return _listenUri; }
            set
            {
                ThrowIfImmutable();
                _listenUri = value;
            }
        }

        internal MessageSecurityVersion MessageSecurityVersion
        {
            get { return _messageSecurityVersion; }
        }

        // ISecurityCommunicationObject members
        public TimeSpan DefaultOpenTimeout
        {
            get { return ServiceDefaults.OpenTimeout; }
        }

        public TimeSpan DefaultCloseTimeout
        {
            get { return ServiceDefaults.CloseTimeout; }
        }

        public IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new OperationWithTimeoutAsyncResult(this.OnClose, timeout, callback, state);
        }

        public IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new OperationWithTimeoutAsyncResult(this.OnOpen, timeout, callback, state);
        }

        public void OnClosed()
        {
        }

        public void OnClosing()
        {
        }

        public void OnEndClose(IAsyncResult result)
        {
            OperationWithTimeoutAsyncResult.End(result);
        }

        public void OnEndOpen(IAsyncResult result)
        {
            OperationWithTimeoutAsyncResult.End(result);
        }

        public void OnFaulted()
        {
        }

        public void OnOpened()
        {
        }

        public void OnOpening()
        {
        }

        public virtual void OnAbort()
        {
            if (!_actAsInitiator)
            {
                foreach (SupportingTokenAuthenticatorSpecification spec in _channelSupportingTokenAuthenticatorSpecification)
                {
                    SecurityUtils.AbortTokenAuthenticatorIfRequired(spec.TokenAuthenticator);
                }
                foreach (string action in _scopedSupportingTokenAuthenticatorSpecification.Keys)
                {
                    ICollection<SupportingTokenAuthenticatorSpecification> supportingAuthenticators = _scopedSupportingTokenAuthenticatorSpecification[action];
                    foreach (SupportingTokenAuthenticatorSpecification spec in supportingAuthenticators)
                    {
                        SecurityUtils.AbortTokenAuthenticatorIfRequired(spec.TokenAuthenticator);
                    }
                }
            }
        }

        public virtual void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            if (!_actAsInitiator)
            {
                foreach (SupportingTokenAuthenticatorSpecification spec in _channelSupportingTokenAuthenticatorSpecification)
                {
                    SecurityUtils.CloseTokenAuthenticatorIfRequired(spec.TokenAuthenticator, timeoutHelper.RemainingTime());
                }
                foreach (string action in _scopedSupportingTokenAuthenticatorSpecification.Keys)
                {
                    ICollection<SupportingTokenAuthenticatorSpecification> supportingAuthenticators = _scopedSupportingTokenAuthenticatorSpecification[action];
                    foreach (SupportingTokenAuthenticatorSpecification spec in supportingAuthenticators)
                    {
                        SecurityUtils.CloseTokenAuthenticatorIfRequired(spec.TokenAuthenticator, timeoutHelper.RemainingTime());
                    }
                }
            }
        }

        public virtual object CreateListenerSecurityState()
        {
            return null;
        }

        public SecurityProtocol CreateSecurityProtocol(EndpointAddress target, Uri via, object listenerSecurityState, bool isReturnLegSecurityRequired, TimeSpan timeout)
        {
            ThrowIfNotOpen();
            SecurityProtocol securityProtocol = OnCreateSecurityProtocol(target, via, listenerSecurityState, timeout);
            if (securityProtocol == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRServiceModel.ProtocolFactoryCouldNotCreateProtocol));
            }
            return securityProtocol;
        }

        public virtual EndpointIdentity GetIdentityOfSelf()
        {
            return null;
        }

        public virtual T GetProperty<T>()
        {
            if (typeof(T) == typeof(Collection<ISecurityContextSecurityTokenCache>))
            {
                ThrowIfNotOpen();
                Collection<ISecurityContextSecurityTokenCache> result = new Collection<ISecurityContextSecurityTokenCache>();
                if (_channelSupportingTokenAuthenticatorSpecification != null)
                {
                    foreach (SupportingTokenAuthenticatorSpecification spec in _channelSupportingTokenAuthenticatorSpecification)
                    {
                        if (spec.TokenAuthenticator is ISecurityContextSecurityTokenCacheProvider)
                        {
                            result.Add(((ISecurityContextSecurityTokenCacheProvider)spec.TokenAuthenticator).TokenCache);
                        }
                    }
                }
                return (T)(object)(result);
            }
            else
            {
                return default(T);
            }
        }

        protected abstract SecurityProtocol OnCreateSecurityProtocol(EndpointAddress target, Uri via, object listenerSecurityState, TimeSpan timeout);

        private void VerifyTypeUniqueness(ICollection<SupportingTokenAuthenticatorSpecification> supportingTokenAuthenticators)
        {
            // its ok to go brute force here since we are dealing with a small number of authenticators
            foreach (SupportingTokenAuthenticatorSpecification spec in supportingTokenAuthenticators)
            {
                Type authenticatorType = spec.TokenAuthenticator.GetType();
                int numSkipped = 0;
                foreach (SupportingTokenAuthenticatorSpecification spec2 in supportingTokenAuthenticators)
                {
                    Type spec2AuthenticatorType = spec2.TokenAuthenticator.GetType();
                    if (object.ReferenceEquals(spec, spec2))
                    {
                        if (numSkipped > 0)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(string.Format(SRServiceModel.MultipleSupportingAuthenticatorsOfSameType, spec.TokenParameters.GetType())));
                        }
                        ++numSkipped;
                        continue;
                    }
                    else if (authenticatorType.IsAssignableFrom(spec2AuthenticatorType) || spec2AuthenticatorType.IsAssignableFrom(authenticatorType))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(string.Format(SRServiceModel.MultipleSupportingAuthenticatorsOfSameType, spec.TokenParameters.GetType())));
                    }
                }
            }
        }

        internal IList<SupportingTokenAuthenticatorSpecification> GetSupportingTokenAuthenticators(string action, out bool expectSignedTokens, out bool expectBasicTokens, out bool expectEndorsingTokens)
        {
            if (_mergedSupportingTokenAuthenticatorsMap != null && _mergedSupportingTokenAuthenticatorsMap.Count > 0)
            {
                if (action != null && _mergedSupportingTokenAuthenticatorsMap.ContainsKey(action))
                {
                    MergedSupportingTokenAuthenticatorSpecification mergedSpec = _mergedSupportingTokenAuthenticatorsMap[action];
                    expectSignedTokens = mergedSpec.ExpectSignedTokens;
                    expectBasicTokens = mergedSpec.ExpectBasicTokens;
                    expectEndorsingTokens = mergedSpec.ExpectEndorsingTokens;
                    return mergedSpec.SupportingTokenAuthenticators;
                }
                else if (_mergedSupportingTokenAuthenticatorsMap.ContainsKey(MessageHeaders.WildcardAction))
                {
                    MergedSupportingTokenAuthenticatorSpecification mergedSpec = _mergedSupportingTokenAuthenticatorsMap[MessageHeaders.WildcardAction];
                    expectSignedTokens = mergedSpec.ExpectSignedTokens;
                    expectBasicTokens = mergedSpec.ExpectBasicTokens;
                    expectEndorsingTokens = mergedSpec.ExpectEndorsingTokens;
                    return mergedSpec.SupportingTokenAuthenticators;
                }
            }
            expectSignedTokens = _expectChannelSignedTokens;
            expectBasicTokens = _expectChannelBasicTokens;
            expectEndorsingTokens = _expectChannelEndorsingTokens;
            // in case the channelSupportingTokenAuthenticators is empty return null so that its Count does not get accessed.
            return (Object.ReferenceEquals(_channelSupportingTokenAuthenticatorSpecification, EmptyTokenAuthenticators)) ? null : (IList<SupportingTokenAuthenticatorSpecification>)_channelSupportingTokenAuthenticatorSpecification;
        }

        private void MergeSupportingTokenAuthenticators(TimeSpan timeout)
        {
            if (_scopedSupportingTokenAuthenticatorSpecification.Count == 0)
            {
                _mergedSupportingTokenAuthenticatorsMap = null;
            }
            else
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                _expectSupportingTokens = true;
                _mergedSupportingTokenAuthenticatorsMap = new Dictionary<string, MergedSupportingTokenAuthenticatorSpecification>();
                foreach (string action in _scopedSupportingTokenAuthenticatorSpecification.Keys)
                {
                    ICollection<SupportingTokenAuthenticatorSpecification> scopedAuthenticators = _scopedSupportingTokenAuthenticatorSpecification[action];
                    if (scopedAuthenticators == null || scopedAuthenticators.Count == 0)
                    {
                        continue;
                    }
                    Collection<SupportingTokenAuthenticatorSpecification> mergedAuthenticators = new Collection<SupportingTokenAuthenticatorSpecification>();
                    bool expectSignedTokens = _expectChannelSignedTokens;
                    bool expectBasicTokens = _expectChannelBasicTokens;
                    bool expectEndorsingTokens = _expectChannelEndorsingTokens;
                    foreach (SupportingTokenAuthenticatorSpecification spec in _channelSupportingTokenAuthenticatorSpecification)
                    {
                        mergedAuthenticators.Add(spec);
                    }
                    foreach (SupportingTokenAuthenticatorSpecification spec in scopedAuthenticators)
                    {
                        SecurityUtils.OpenTokenAuthenticatorIfRequired(spec.TokenAuthenticator, timeoutHelper.RemainingTime());
                        mergedAuthenticators.Add(spec);
                        if (spec.SecurityTokenAttachmentMode == SecurityTokenAttachmentMode.Endorsing ||
                            spec.SecurityTokenAttachmentMode == SecurityTokenAttachmentMode.SignedEndorsing)
                        {
                            if (spec.TokenParameters.RequireDerivedKeys && !spec.TokenParameters.HasAsymmetricKey)
                            {
                                _expectKeyDerivation = true;
                            }
                        }
                        SecurityTokenAttachmentMode mode = spec.SecurityTokenAttachmentMode;
                        if (mode == SecurityTokenAttachmentMode.SignedEncrypted
                            || mode == SecurityTokenAttachmentMode.Signed
                            || mode == SecurityTokenAttachmentMode.SignedEndorsing)
                        {
                            expectSignedTokens = true;
                            if (mode == SecurityTokenAttachmentMode.SignedEncrypted)
                            {
                                expectBasicTokens = true;
                            }
                        }
                        if (mode == SecurityTokenAttachmentMode.Endorsing || mode == SecurityTokenAttachmentMode.SignedEndorsing)
                        {
                            expectEndorsingTokens = true;
                        }
                    }
                    VerifyTypeUniqueness(mergedAuthenticators);
                    MergedSupportingTokenAuthenticatorSpecification mergedSpec = new MergedSupportingTokenAuthenticatorSpecification();
                    mergedSpec.SupportingTokenAuthenticators = mergedAuthenticators;
                    mergedSpec.ExpectBasicTokens = expectBasicTokens;
                    mergedSpec.ExpectEndorsingTokens = expectEndorsingTokens;
                    mergedSpec.ExpectSignedTokens = expectSignedTokens;
                    _mergedSupportingTokenAuthenticatorsMap.Add(action, mergedSpec);
                }
            }
        }

        protected RecipientServiceModelSecurityTokenRequirement CreateRecipientSecurityTokenRequirement()
        {
            RecipientServiceModelSecurityTokenRequirement requirement = new RecipientServiceModelSecurityTokenRequirement();
            requirement.SecurityBindingElement = _securityBindingElement;
            requirement.SecurityAlgorithmSuite = this.IncomingAlgorithmSuite;
            requirement.ListenUri = _listenUri;
            requirement.MessageSecurityVersion = this.MessageSecurityVersion.SecurityTokenVersion;
            requirement.Properties[ServiceModelSecurityTokenRequirement.ExtendedProtectionPolicy] = _extendedProtectionPolicy;
            return requirement;
        }

        private RecipientServiceModelSecurityTokenRequirement CreateRecipientSecurityTokenRequirement(SecurityTokenParameters parameters, SecurityTokenAttachmentMode attachmentMode)
        {
            RecipientServiceModelSecurityTokenRequirement requirement = CreateRecipientSecurityTokenRequirement();

            requirement.KeyUsage = SecurityKeyUsage.Signature;
            requirement.Properties[ServiceModelSecurityTokenRequirement.MessageDirectionProperty] = MessageDirection.Input;
            requirement.Properties[ServiceModelSecurityTokenRequirement.SupportingTokenAttachmentModeProperty] = attachmentMode;
            requirement.Properties[ServiceModelSecurityTokenRequirement.ExtendedProtectionPolicy] = _extendedProtectionPolicy;
            return requirement;
        }

        private void AddSupportingTokenAuthenticators(SupportingTokenParameters supportingTokenParameters, bool isOptional, IList<SupportingTokenAuthenticatorSpecification> authenticatorSpecList)
        {
            for (int i = 0; i < supportingTokenParameters.Endorsing.Count; ++i)
            {
                SecurityTokenRequirement requirement = this.CreateRecipientSecurityTokenRequirement(supportingTokenParameters.Endorsing[i], SecurityTokenAttachmentMode.Endorsing);
                try
                {
                    System.IdentityModel.Selectors.SecurityTokenResolver resolver;
                    System.IdentityModel.Selectors.SecurityTokenAuthenticator authenticator = this.SecurityTokenManager.CreateSecurityTokenAuthenticator(requirement, out resolver);
                    SupportingTokenAuthenticatorSpecification authenticatorSpec = new SupportingTokenAuthenticatorSpecification(authenticator, resolver, SecurityTokenAttachmentMode.Endorsing, supportingTokenParameters.Endorsing[i], isOptional);
                    authenticatorSpecList.Add(authenticatorSpec);
                }
                catch (Exception e)
                {
                    if (!isOptional || Fx.IsFatal(e))
                    {
                        throw;
                    }
                }
            }
            for (int i = 0; i < supportingTokenParameters.SignedEndorsing.Count; ++i)
            {
                SecurityTokenRequirement requirement = this.CreateRecipientSecurityTokenRequirement(supportingTokenParameters.SignedEndorsing[i], SecurityTokenAttachmentMode.SignedEndorsing);
                try
                {
                    System.IdentityModel.Selectors.SecurityTokenResolver resolver;
                    System.IdentityModel.Selectors.SecurityTokenAuthenticator authenticator = this.SecurityTokenManager.CreateSecurityTokenAuthenticator(requirement, out resolver);
                    SupportingTokenAuthenticatorSpecification authenticatorSpec = new SupportingTokenAuthenticatorSpecification(authenticator, resolver, SecurityTokenAttachmentMode.SignedEndorsing, supportingTokenParameters.SignedEndorsing[i], isOptional);
                    authenticatorSpecList.Add(authenticatorSpec);
                }
                catch (Exception e)
                {
                    if (!isOptional || Fx.IsFatal(e))
                    {
                        throw;
                    }
                }
            }
            for (int i = 0; i < supportingTokenParameters.SignedEncrypted.Count; ++i)
            {
                SecurityTokenRequirement requirement = this.CreateRecipientSecurityTokenRequirement(supportingTokenParameters.SignedEncrypted[i], SecurityTokenAttachmentMode.SignedEncrypted);
                try
                {
                    System.IdentityModel.Selectors.SecurityTokenResolver resolver;
                    System.IdentityModel.Selectors.SecurityTokenAuthenticator authenticator = this.SecurityTokenManager.CreateSecurityTokenAuthenticator(requirement, out resolver);
                    SupportingTokenAuthenticatorSpecification authenticatorSpec = new SupportingTokenAuthenticatorSpecification(authenticator, resolver, SecurityTokenAttachmentMode.SignedEncrypted, supportingTokenParameters.SignedEncrypted[i], isOptional);
                    authenticatorSpecList.Add(authenticatorSpec);
                }
                catch (Exception e)
                {
                    if (!isOptional || Fx.IsFatal(e))
                    {
                        throw;
                    }
                }
            }
            for (int i = 0; i < supportingTokenParameters.Signed.Count; ++i)
            {
                SecurityTokenRequirement requirement = this.CreateRecipientSecurityTokenRequirement(supportingTokenParameters.Signed[i], SecurityTokenAttachmentMode.Signed);
                try
                {
                    System.IdentityModel.Selectors.SecurityTokenResolver resolver;
                    System.IdentityModel.Selectors.SecurityTokenAuthenticator authenticator = this.SecurityTokenManager.CreateSecurityTokenAuthenticator(requirement, out resolver);
                    SupportingTokenAuthenticatorSpecification authenticatorSpec = new SupportingTokenAuthenticatorSpecification(authenticator, resolver, SecurityTokenAttachmentMode.Signed, supportingTokenParameters.Signed[i], isOptional);
                    authenticatorSpecList.Add(authenticatorSpec);
                }
                catch (Exception e)
                {
                    if (!isOptional || Fx.IsFatal(e))
                    {
                        throw;
                    }
                }
            }
        }

        public virtual void OnOpen(TimeSpan timeout)
        {
            if (this.SecurityBindingElement == null)
            {
                this.OnPropertySettingsError("SecurityBindingElement", true);
            }
            if (this.SecurityTokenManager == null)
            {
                this.OnPropertySettingsError("SecurityTokenManager", true);
            }
            _messageSecurityVersion = _standardsManager.MessageSecurityVersion;
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            _expectOutgoingMessages = this.ActAsInitiator || this.SupportsRequestReply;
            _expectIncomingMessages = !this.ActAsInitiator || this.SupportsRequestReply;
            if (!_actAsInitiator)
            {
                AddSupportingTokenAuthenticators(_securityBindingElement.EndpointSupportingTokenParameters, false, (IList<SupportingTokenAuthenticatorSpecification>)_channelSupportingTokenAuthenticatorSpecification);
                // validate the token authenticator types and create a merged map if needed.
                if (!_channelSupportingTokenAuthenticatorSpecification.IsReadOnly)
                {
                    if (_channelSupportingTokenAuthenticatorSpecification.Count == 0)
                    {
                        _channelSupportingTokenAuthenticatorSpecification = EmptyTokenAuthenticators;
                    }
                    else
                    {
                        _expectSupportingTokens = true;
                        foreach (SupportingTokenAuthenticatorSpecification tokenAuthenticatorSpec in _channelSupportingTokenAuthenticatorSpecification)
                        {
                            SecurityUtils.OpenTokenAuthenticatorIfRequired(tokenAuthenticatorSpec.TokenAuthenticator, timeoutHelper.RemainingTime());
                            if (tokenAuthenticatorSpec.SecurityTokenAttachmentMode == SecurityTokenAttachmentMode.Endorsing
                                || tokenAuthenticatorSpec.SecurityTokenAttachmentMode == SecurityTokenAttachmentMode.SignedEndorsing)
                            {
                                if (tokenAuthenticatorSpec.TokenParameters.RequireDerivedKeys && !tokenAuthenticatorSpec.TokenParameters.HasAsymmetricKey)
                                {
                                    _expectKeyDerivation = true;
                                }
                            }
                            SecurityTokenAttachmentMode mode = tokenAuthenticatorSpec.SecurityTokenAttachmentMode;
                            if (mode == SecurityTokenAttachmentMode.SignedEncrypted
                                || mode == SecurityTokenAttachmentMode.Signed
                                || mode == SecurityTokenAttachmentMode.SignedEndorsing)
                            {
                                _expectChannelSignedTokens = true;
                                if (mode == SecurityTokenAttachmentMode.SignedEncrypted)
                                {
                                    _expectChannelBasicTokens = true;
                                }
                            }
                            if (mode == SecurityTokenAttachmentMode.Endorsing || mode == SecurityTokenAttachmentMode.SignedEndorsing)
                            {
                                _expectChannelEndorsingTokens = true;
                            }
                        }
                        _channelSupportingTokenAuthenticatorSpecification =
                            new ReadOnlyCollection<SupportingTokenAuthenticatorSpecification>((Collection<SupportingTokenAuthenticatorSpecification>)_channelSupportingTokenAuthenticatorSpecification);
                    }
                }
                VerifyTypeUniqueness(_channelSupportingTokenAuthenticatorSpecification);
                MergeSupportingTokenAuthenticators(timeoutHelper.RemainingTime());
            }

            if (this.DetectReplays)
            {
                if (!this.SupportsReplayDetection)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("DetectReplays", string.Format(SRServiceModel.SecurityProtocolCannotDoReplayDetection, this));
                }
                if (this.MaxClockSkew == TimeSpan.MaxValue || this.ReplayWindow == TimeSpan.MaxValue)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.NoncesCachedInfinitely));
                }

                // If DetectReplays is true and nonceCache is null then use the default InMemoryNonceCache. 
                if (_nonceCache == null)
                {
                    // The nonce needs to be cached for replayWindow + 2*clockSkew to eliminate replays
                    _nonceCache = new InMemoryNonceCache(this.ReplayWindow + this.MaxClockSkew + this.MaxClockSkew, this.MaxCachedNonces);
                }
            }

            _derivedKeyTokenAuthenticator = new NonValidatingSecurityTokenAuthenticator<DerivedKeySecurityToken>();
        }

        public void Open(bool actAsInitiator, TimeSpan timeout)
        {
            _actAsInitiator = actAsInitiator;
            _communicationObject.Open(timeout);
        }

        public IAsyncResult BeginOpen(bool actAsInitiator, TimeSpan timeout, AsyncCallback callback, object state)
        {
            _actAsInitiator = actAsInitiator;
            return this.CommunicationObject.BeginOpen(timeout, callback, state);
        }

        public void EndOpen(IAsyncResult result)
        {
            this.CommunicationObject.EndOpen(result);
        }

        public void Close(bool aborted, TimeSpan timeout)
        {
            if (aborted)
            {
                this.CommunicationObject.Abort();
            }
            else
            {
                this.CommunicationObject.Close(timeout);
            }
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.CommunicationObject.BeginClose(timeout, callback, state);
        }

        public void EndClose(IAsyncResult result)
        {
            this.CommunicationObject.EndClose(result);
        }

        internal void Open(string propertyName, bool requiredForForwardDirection, SecurityTokenAuthenticator authenticator, TimeSpan timeout)
        {
            if (authenticator != null)
            {
                SecurityUtils.OpenTokenAuthenticatorIfRequired(authenticator, timeout);
            }
            else
            {
                OnPropertySettingsError(propertyName, requiredForForwardDirection);
            }
        }

        internal void Open(string propertyName, bool requiredForForwardDirection, SecurityTokenProvider provider, TimeSpan timeout)
        {
            if (provider != null)
            {
                SecurityUtils.OpenTokenProviderIfRequired(provider, timeout);
            }
            else
            {
                OnPropertySettingsError(propertyName, requiredForForwardDirection);
            }
        }

        internal void OnPropertySettingsError(string propertyName, bool requiredForForwardDirection)
        {
            if (requiredForForwardDirection)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(
                    string.Format(SRServiceModel.PropertySettingErrorOnProtocolFactory, propertyName, this),
                    propertyName));
            }
            else if (_requestReplyErrorPropertyName == null)
            {
                _requestReplyErrorPropertyName = propertyName;
            }
        }

        private void ThrowIfReturnDirectionSecurityNotSupported()
        {
            if (_requestReplyErrorPropertyName != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(
                    string.Format(SRServiceModel.PropertySettingErrorOnProtocolFactory, _requestReplyErrorPropertyName, this),
                    _requestReplyErrorPropertyName));
            }
        }

        internal void ThrowIfImmutable()
        {
            _communicationObject.ThrowIfDisposedOrImmutable();
        }

        private void ThrowIfNotOpen()
        {
            _communicationObject.ThrowIfNotOpened();
        }
    }

    internal struct MergedSupportingTokenAuthenticatorSpecification
    {
        public Collection<SupportingTokenAuthenticatorSpecification> SupportingTokenAuthenticators;
        public bool ExpectSignedTokens;
        public bool ExpectEndorsingTokens;
        public bool ExpectBasicTokens;
    }
}
