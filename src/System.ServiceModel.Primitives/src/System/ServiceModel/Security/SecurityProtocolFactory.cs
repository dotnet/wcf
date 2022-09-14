// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Selectors;
using System.Runtime;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.Globalization;
using System.Threading.Tasks;

namespace System.ServiceModel.Security
{
    /*
     * Concrete implementations are required to be thread safe after
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
        private bool _addTimestamp = defaultAddTimestamp;
        private bool _detectReplays = defaultDetectReplays;
        private bool _expectIncomingMessages;
        private bool _expectOutgoingMessages;
        private SecurityAlgorithmSuite _incomingAlgorithmSuite = SecurityAlgorithmSuite.Default;

        // per receiver protocol factory lists
        private ICollection<SupportingTokenAuthenticatorSpecification> _channelSupportingTokenAuthenticatorSpecification;

        private int _maxCachedNonces = defaultMaxCachedNonces;
        private TimeSpan _maxClockSkew = defaultMaxClockSkew;
        private NonceCache _nonceCache = null;
        private SecurityAlgorithmSuite _outgoingAlgorithmSuite = SecurityAlgorithmSuite.Default;
        private TimeSpan _replayWindow = defaultReplayWindow;
        private SecurityStandardsManager _standardsManager = SecurityStandardsManager.DefaultInstance;
        private SecurityTokenManager _securityTokenManager;
        private SecurityBindingElement _securityBindingElement;
        private string _requestReplyErrorPropertyName;
        private TimeSpan _timestampValidityDuration = defaultTimestampValidityDuration;
        private SecurityHeaderLayout _securityHeaderLayout;
#pragma warning disable CS0649 // Field is never assign to
        private bool _expectChannelBasicTokens;
        private bool _expectChannelSignedTokens;
        private bool _expectChannelEndorsingTokens;
#pragma warning restore CS0649 // Field is never assign to
        private BufferManager _streamBufferManager = null;

        protected SecurityProtocolFactory()
        {
            _channelSupportingTokenAuthenticatorSpecification = new Collection<SupportingTokenAuthenticatorSpecification>();
            //scopedSupportingTokenAuthenticatorSpecification = new Dictionary<string, ICollection<SupportingTokenAuthenticatorSpecification>>();
            CommunicationObject = new WrapperSecurityCommunicationObject(this);
        }

        internal SecurityProtocolFactory(SecurityProtocolFactory factory) : this()
        {
            if (factory == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(factory));
            }

            ActAsInitiator = factory.ActAsInitiator;
            _addTimestamp = factory._addTimestamp;
            _detectReplays = factory._detectReplays;
            _incomingAlgorithmSuite = factory._incomingAlgorithmSuite;
            _maxCachedNonces = factory._maxCachedNonces;
            _maxClockSkew = factory._maxClockSkew;
            _outgoingAlgorithmSuite = factory._outgoingAlgorithmSuite;
            _replayWindow = factory._replayWindow;
            _channelSupportingTokenAuthenticatorSpecification = new Collection<SupportingTokenAuthenticatorSpecification>(new List<SupportingTokenAuthenticatorSpecification>(factory._channelSupportingTokenAuthenticatorSpecification));
            _standardsManager = factory._standardsManager;
            _timestampValidityDuration = factory._timestampValidityDuration;
            _securityBindingElement = (SecurityBindingElement)factory._securityBindingElement?.Clone();
            _securityTokenManager = factory._securityTokenManager;
            _nonceCache = factory._nonceCache;
        }

        protected WrapperSecurityCommunicationObject CommunicationObject { get; }

        // The ActAsInitiator value is set automatically on Open and
        // remains unchanged thereafter.  ActAsInitiator is true for
        // the initiator of the message exchange, such as the sender
        // of a datagram, sender of a request and sender of either leg
        // of a duplex exchange.
        public bool ActAsInitiator { get; private set; }

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

        public ExtendedProtectionPolicy ExtendedProtectionPolicy { get; set; }

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

        private static ReadOnlyCollection<SupportingTokenAuthenticatorSpecification> EmptyTokenAuthenticators
        {
            get
            {
                if (s_emptyTokenAuthenticators == null)
                {
                    s_emptyTokenAuthenticators = Array.AsReadOnly(new SupportingTokenAuthenticatorSpecification[0]);
                }
                return s_emptyTokenAuthenticators;
            }
        }

        internal bool ExpectKeyDerivation { get; set; }

        internal bool ExpectSupportingTokens { get; set; }

        public SecurityAlgorithmSuite IncomingAlgorithmSuite
        {
            get
            {
                return _incomingAlgorithmSuite;
            }
            set
            {
                ThrowIfImmutable();
                _incomingAlgorithmSuite = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(value)));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
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
                _outgoingAlgorithmSuite = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(value)));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), SRP.TimeSpanMustbeGreaterThanTimeSpanZero));
                }
                _replayWindow = value;
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
                _standardsManager = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(value)));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), SRP.TimeSpanMustbeGreaterThanTimeSpanZero));
                }
                _timestampValidityDuration = value;
            }
        }

        internal MessageSecurityVersion MessageSecurityVersion { get; private set; }

        // ISecurityCommunicationObject members
        public TimeSpan DefaultOpenTimeout => ServiceDefaults.OpenTimeout;

        public TimeSpan DefaultCloseTimeout => ServiceDefaults.CloseTimeout;

        public void OnClosed()
        {
        }

        public void OnClosing()
        {
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
            if (!ActAsInitiator)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
        }

        public virtual Task OnCloseAsync(TimeSpan timeout)
        {
            if (!ActAsInitiator)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
            return Task.CompletedTask;
        }

        public SecurityProtocol CreateSecurityProtocol(EndpointAddress target, Uri via, object listenerSecurityState, bool isReturnLegSecurityRequired, TimeSpan timeout)
        {
            ThrowIfNotOpen();
            SecurityProtocol securityProtocol = OnCreateSecurityProtocol(target, via, listenerSecurityState, timeout);
            if (securityProtocol == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.ProtocolFactoryCouldNotCreateProtocol));
            }
            return securityProtocol;
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

        internal IList<SupportingTokenAuthenticatorSpecification> GetSupportingTokenAuthenticators(string action, out bool expectSignedTokens, out bool expectBasicTokens, out bool expectEndorsingTokens)
        {
            expectSignedTokens = _expectChannelSignedTokens;
            expectBasicTokens = _expectChannelBasicTokens;
            expectEndorsingTokens = _expectChannelEndorsingTokens;
            // in case the channelSupportingTokenAuthenticators is empty return null so that its Count does not get accessed.
            return (Object.ReferenceEquals(_channelSupportingTokenAuthenticatorSpecification, EmptyTokenAuthenticators)) ? null : (IList<SupportingTokenAuthenticatorSpecification>)_channelSupportingTokenAuthenticatorSpecification;
        }

        public virtual Task OnOpenAsync(TimeSpan timeout)
        {
            if (SecurityBindingElement == null)
            {
                OnPropertySettingsError(nameof(SecurityBindingElement), true);
            }

            if (SecurityTokenManager == null)
            {
                OnPropertySettingsError(nameof(SecurityTokenManager), true);
            }

            MessageSecurityVersion = _standardsManager.MessageSecurityVersion;
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            _expectOutgoingMessages = ActAsInitiator || SupportsRequestReply;
            _expectIncomingMessages = !ActAsInitiator || SupportsRequestReply;
            if (!ActAsInitiator)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            if (DetectReplays)
            {
                if (!SupportsReplayDetection)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(nameof(DetectReplays), SRP.Format(SRP.SecurityProtocolCannotDoReplayDetection, this));
                }
                if (MaxClockSkew == TimeSpan.MaxValue || ReplayWindow == TimeSpan.MaxValue)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.NoncesCachedInfinitely));
                }

                // If DetectReplays is true and nonceCache is null then use the default InMemoryNonceCache. 
                if (_nonceCache == null)
                {
                    // The nonce needs to be cached for replayWindow + 2*clockSkew to eliminate replays
                    _nonceCache = new InMemoryNonceCache(ReplayWindow + MaxClockSkew + MaxClockSkew, MaxCachedNonces);
                }
            }

            //derivedKeyTokenAuthenticator = new NonValidatingSecurityTokenAuthenticator<DerivedKeySecurityToken>();
            return Task.CompletedTask;
        }

        public Task OpenAsync(bool actAsInitiator, TimeSpan timeout)
        {
            ActAsInitiator = actAsInitiator;
            return ((IAsyncCommunicationObject)CommunicationObject).OpenAsync(timeout);
        }

        public Task CloseAsync(bool aborted, TimeSpan timeout)
        {
            if (aborted)
            {
                CommunicationObject.Abort();
                return Task.CompletedTask;
            }
            else
            {
                return ((IAsyncCommunicationObject)CommunicationObject).CloseAsync(timeout);
            }
        }

        internal void OnPropertySettingsError(string propertyName, bool requiredForForwardDirection)
        {
            if (requiredForForwardDirection)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(
                    SRP.Format(SRP.PropertySettingErrorOnProtocolFactory, propertyName, this),
                    propertyName));
            }
            else if (_requestReplyErrorPropertyName == null)
            {
                _requestReplyErrorPropertyName = propertyName;
            }
        }

        internal void ThrowIfImmutable()
        {
            CommunicationObject.ThrowIfDisposedOrImmutable();
        }

        private void ThrowIfNotOpen()
        {
            CommunicationObject.ThrowIfNotOpened();
        }
    }
}
