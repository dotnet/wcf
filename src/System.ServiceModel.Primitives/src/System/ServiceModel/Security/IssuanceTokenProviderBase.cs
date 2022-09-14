// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Security
{
    // IssuanceTokenProviderBase is a base class for token providers that fetch tokens from
    // another party.
    // This class manages caching of tokens, async messaging, concurrency
    internal abstract class IssuanceTokenProviderBase<T> : CommunicationObjectSecurityTokenProvider
        where T : IssuanceTokenProviderState
    {
        internal const string defaultClientMaxTokenCachingTimeString = "10675199.02:48:05.4775807";
        internal const bool defaultClientCacheTokens = true;
        internal const int defaultServiceTokenValidityThresholdPercentage = 60;

        // if an issuer is explicitly specified it will be used otherwise target is the issuer
        private EndpointAddress _issuerAddress;
        // the target service's address and via
        private EndpointAddress _targetAddress;
        private Uri _via = null;

        // This controls whether the token provider caches the service tokens it obtains
        private bool _cacheServiceTokens = defaultClientCacheTokens;
        // This is a fudge factor that controls how long the client can use a service token
        private int _serviceTokenValidityThresholdPercentage = defaultServiceTokenValidityThresholdPercentage;
        // the maximum time that the client is willing to cache service tokens
        private TimeSpan _maxServiceTokenCachingTime;

        private SecurityStandardsManager _standardsManager;
        private SecurityAlgorithmSuite _algorithmSuite;
        private ChannelProtectionRequirements _applicationProtectionRequirements;
        private SecurityToken _cachedToken;
        private string _sctUri;

        protected IssuanceTokenProviderBase()
            : base()
        {
            _cacheServiceTokens = defaultClientCacheTokens;
            _serviceTokenValidityThresholdPercentage = defaultServiceTokenValidityThresholdPercentage;
            _maxServiceTokenCachingTime = DefaultClientMaxTokenCachingTime;
            _standardsManager = null;
        }

        // settings
        public EndpointAddress IssuerAddress
        {
            get
            {
                return _issuerAddress;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _issuerAddress = value;
            }
        }

        public EndpointAddress TargetAddress
        {
            get
            {
                return _targetAddress;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _targetAddress = value;
            }
        }

        public bool CacheServiceTokens
        {
            get
            {
                return _cacheServiceTokens;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _cacheServiceTokens = value;
            }
        }

        internal static TimeSpan DefaultClientMaxTokenCachingTime
        {
            get
            {
                Fx.Assert(TimeSpan.Parse(defaultClientMaxTokenCachingTimeString, CultureInfo.InvariantCulture) == TimeSpan.MaxValue, "TimeSpan value not correct");
                return TimeSpan.MaxValue;
            }
        }

        public int ServiceTokenValidityThresholdPercentage
        {
            get
            {
                return _serviceTokenValidityThresholdPercentage;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                if (value <= 0 || value > 100)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), SRP.Format(SRP.ValueMustBeInRange, 1, 100)));
                }
                _serviceTokenValidityThresholdPercentage = value;
            }
        }

        public SecurityAlgorithmSuite SecurityAlgorithmSuite
        {
            get
            {
                return _algorithmSuite;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _algorithmSuite = value;
            }
        }

        public TimeSpan MaxServiceTokenCachingTime
        {
            get
            {
                return _maxServiceTokenCachingTime;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                if (value <= TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), SRP.TimeSpanMustbeGreaterThanTimeSpanZero));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value, SRP.SFxTimeoutOutOfRangeTooBig));
                }

                _maxServiceTokenCachingTime = value;
            }
        }


        public SecurityStandardsManager StandardsManager
        {
            get
            {
                if (_standardsManager == null)
                {
                    return SecurityStandardsManager.DefaultInstance;
                }

                return _standardsManager;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _standardsManager = value;
            }
        }

        public ChannelProtectionRequirements ApplicationProtectionRequirements
        {
            get
            {
                return _applicationProtectionRequirements;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _applicationProtectionRequirements = value;
            }
        }

        public Uri Via
        {
            get
            {
                return _via;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _via = value;
            }
        }

        public override bool SupportsTokenCancellation
        {
            get
            {
                return true;
            }
        }

        protected Object ThisLock { get; } = new Object();

        protected virtual bool IsMultiLegNegotiation
        {
            get { return true; }
        }

        protected abstract MessageVersion MessageVersion
        {
            get;
        }

        protected abstract bool RequiresManualReplyAddressing
        {
            get;
        }

        public abstract XmlDictionaryString RequestSecurityTokenAction
        {
            get;
        }

        public abstract XmlDictionaryString RequestSecurityTokenResponseAction
        {
            get;
        }

        protected string SecurityContextTokenUri
        {
            get
            {
                ThrowIfCreated();
                return _sctUri;
            }
        }

        protected void ThrowIfCreated()
        {
            CommunicationState state = CommunicationObject.State;
            if (state == CommunicationState.Created)
            {
                Exception e = new InvalidOperationException(SRP.Format(SRP.CommunicationObjectCannotBeUsed, GetType().ToString(), state.ToString()));
                throw TraceUtility.ThrowHelperError(e, Guid.Empty, this);
            }
        }

        protected void ThrowIfClosedOrCreated()
        {
            CommunicationObject.ThrowIfClosed();
            ThrowIfCreated();
        }

        // ISecurityCommunicationObject methods
        public override Task OnOpenAsync(TimeSpan timeout)
        {
            if (_targetAddress == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.TargetAddressIsNotSet, GetType())));
            }

            if (SecurityAlgorithmSuite == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SecurityAlgorithmSuiteNotSet, GetType())));
            }

            _sctUri = StandardsManager.SecureConversationDriver.TokenTypeUri;
            return Task.CompletedTask;
        }

        // helper methods
        protected void EnsureEndpointAddressDoesNotRequireEncryption(EndpointAddress target)
        {
            if (ApplicationProtectionRequirements == null
                  || ApplicationProtectionRequirements.OutgoingEncryptionParts == null)
            {
                return;
            }
            MessagePartSpecification channelEncryptionParts = ApplicationProtectionRequirements.OutgoingEncryptionParts.ChannelParts;
            if (channelEncryptionParts == null)
            {
                return;
            }
            for (int i = 0; i < _targetAddress.Headers.Count; ++i)
            {
                AddressHeader header = target.Headers[i];
                if (channelEncryptionParts.IsHeaderIncluded(header.Name, header.Namespace))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SRP.Format(SRP.SecurityNegotiationCannotProtectConfidentialEndpointHeader, target, header.Name, header.Namespace)));
                }
            }
        }

        private DateTime GetServiceTokenEffectiveExpirationTime(SecurityToken serviceToken)
        {
            // if the token never expires, return the max date time
            // else return effective expiration time
            if (serviceToken.ValidTo.ToUniversalTime() >= SecurityUtils.MaxUtcDateTime)
            {
                return serviceToken.ValidTo;
            }

            TimeSpan interval = serviceToken.ValidTo.ToUniversalTime() - serviceToken.ValidFrom.ToUniversalTime();
            long serviceTokenTicksInterval = interval.Ticks;
            long effectiveTicksInterval = Convert.ToInt64((double)ServiceTokenValidityThresholdPercentage / 100.0 * (double)serviceTokenTicksInterval, NumberFormatInfo.InvariantInfo);
            DateTime effectiveExpirationTime = TimeoutHelper.Add(serviceToken.ValidFrom.ToUniversalTime(), new TimeSpan(effectiveTicksInterval));
            DateTime maxCachingTime = TimeoutHelper.Add(serviceToken.ValidFrom.ToUniversalTime(), MaxServiceTokenCachingTime);
            if (effectiveExpirationTime <= maxCachingTime)
            {
                return effectiveExpirationTime;
            }
            else
            {
                return maxCachingTime;
            }
        }

        private bool IsServiceTokenTimeValid(SecurityToken serviceToken)
        {
            DateTime effectiveExpirationTime = GetServiceTokenEffectiveExpirationTime(serviceToken);
            return (DateTime.UtcNow <= effectiveExpirationTime);
        }

        private SecurityToken GetCurrentServiceToken()
        {
            if (CacheServiceTokens && _cachedToken != null && IsServiceTokenTimeValid(_cachedToken))
            {
                return _cachedToken;
            }
            else
            {
                return null;
            }
        }

        static protected void ThrowIfFault(Message message, EndpointAddress target)
        {
            SecurityUtils.ThrowIfNegotiationFault(message, target);
        }

        protected override SecurityToken GetTokenCore(TimeSpan timeout)
        {
            CommunicationObject.ThrowIfClosedOrNotOpen();
            SecurityToken result;
            lock (ThisLock)
            {
                result = GetCurrentServiceToken();
            }

            if (result == null)
            {
                return DoNegotiationAsync(timeout).GetAwaiter().GetResult();
            }

            return result;
        }

        internal override Task<SecurityToken> GetTokenCoreInternalAsync(TimeSpan timeout)
        {
            CommunicationObject.ThrowIfClosedOrNotOpen();
            SecurityToken result;
            lock (ThisLock)
            {
                result = GetCurrentServiceToken();
            }

            if (result == null)
            {
                return DoNegotiationAsync(timeout);
            }

            return Task.FromResult(result);
        }

        internal override Task CancelTokenCoreInternalAsync(TimeSpan timeout, SecurityToken token)
        {
            if (CacheServiceTokens)
            {
                lock (ThisLock)
                {
                    if (object.ReferenceEquals(token, _cachedToken))
                    {
                        _cachedToken = null;
                    }
                }
            }

            return Task.CompletedTask;
        }

        // Negotiation state creation methods
        protected abstract Task<T> CreateNegotiationStateAsync(EndpointAddress target, Uri via, TimeSpan timeout);

        // Negotiation message processing methods
        protected abstract BodyWriter GetFirstOutgoingMessageBody(T negotiationState, out MessageProperties properties);
        protected abstract BodyWriter GetNextOutgoingMessageBody(Message incomingMessage, T negotiationState);
        protected abstract Task InitializeChannelFactoriesAsync(EndpointAddress target, TimeSpan timeout);
        protected abstract IAsyncRequestChannel CreateClientChannel(EndpointAddress target, Uri via);

        private void PrepareRequest(Message nextMessage)
        {
            PrepareRequest(nextMessage, null);
        }

        private void PrepareRequest(Message nextMessage, RequestSecurityToken rst)
        {
            if (rst != null && !rst.IsReadOnly)
            {
                rst.Message = nextMessage;
            }

            RequestReplyCorrelator.PrepareRequest(nextMessage);
            if (RequiresManualReplyAddressing)
            {
                // if we are on HTTP, we need to explicitly add a reply-to header for interop
                nextMessage.Headers.ReplyTo = EndpointAddress.AnonymousAddress;
            }
        }

        /*
        *   Negotiation consists of the following steps (some may be async in the async case):
        *   1. Create negotiation state 
        *   2. Initialize channel factories 
        *   3. Create an channel 
        *   4. Open the channel
        *   5. Create the next message to send to server
        *   6. Send the message and get reply 
        *   8. Process incoming message and get next outgoing message.
        *   9. If no outgoing message, then negotiation is over. Go to step 11.
        *   10. Goto step 6
        *   11. Close the IAsyncRequest channel and complete
        */
        protected async Task<SecurityToken> DoNegotiationAsync(TimeSpan timeout)
        {
            ThrowIfClosedOrCreated();
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            IAsyncRequestChannel rstChannel = null;
            T negotiationState = null;
            TimeSpan timeLeft = timeout;
            int legs = 1;
            try
            {
                negotiationState = await CreateNegotiationStateAsync(_targetAddress, _via, timeoutHelper.RemainingTime());
                InitializeNegotiationState(negotiationState);
                await InitializeChannelFactoriesAsync(negotiationState.RemoteAddress, timeoutHelper.RemainingTime());
                rstChannel = CreateClientChannel(negotiationState.RemoteAddress, _via);
                await rstChannel.OpenAsync(timeoutHelper.RemainingTime());
                Message nextOutgoingMessage = null;
                Message incomingMessage = null;
                SecurityToken serviceToken = null;
                for (; ; )
                {
                    nextOutgoingMessage = GetNextOutgoingMessage(incomingMessage, negotiationState);
                    if (incomingMessage != null)
                    {
                        incomingMessage.Close();
                    }

                    if (nextOutgoingMessage != null)
                    {
                        using (nextOutgoingMessage)
                        {
                            timeLeft = timeoutHelper.RemainingTime();
                            incomingMessage = await rstChannel.RequestAsync(nextOutgoingMessage, timeLeft);
                            if (incomingMessage == null)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(SRP.FailToReceiveReplyFromNegotiation));
                            }
                        }
                        legs += 2;
                    }
                    else
                    {
                        if (!negotiationState.IsNegotiationCompleted)
                        {
                            throw TraceUtility.ThrowHelperError(new SecurityNegotiationException(SRP.NoNegotiationMessageToSend), incomingMessage);
                        }

                        try
                        {
                            rstChannel.Close(timeoutHelper.RemainingTime());
                        }
                        catch (CommunicationException)
                        {
                            rstChannel.Abort();
                        }
                        catch (TimeoutException)
                        {
                            rstChannel.Abort();
                        }

                        rstChannel = null;
                        ValidateAndCacheServiceToken(negotiationState);
                        serviceToken = negotiationState.ServiceToken;
                        break;
                    }
                }
                return serviceToken;
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                if (e is TimeoutException)
                {
                    e = new TimeoutException(SRP.Format(SRP.ClientSecurityNegotiationTimeout, timeout, legs, timeLeft), e);
                }

                EndpointAddress temp = (negotiationState == null) ? null : negotiationState.RemoteAddress;
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(WrapExceptionIfRequired(e, temp, _issuerAddress));
            }
            finally
            {
                Cleanup(rstChannel, negotiationState);
            }
        }

        private void InitializeNegotiationState(T negotiationState)
        {
            negotiationState.TargetAddress = _targetAddress;
            if (negotiationState.Context == null && IsMultiLegNegotiation)
            {
                negotiationState.Context = SecurityUtils.GenerateId();
            }

            if (IssuerAddress != null)
            {
                negotiationState.RemoteAddress = IssuerAddress;
            }
            else
            {
                negotiationState.RemoteAddress = negotiationState.TargetAddress;
            }
        }

        private Message GetNextOutgoingMessage(Message incomingMessage, T negotiationState)
        {
            BodyWriter nextMessageBody;
            MessageProperties nextMessageProperties = null;
            if (incomingMessage == null)
            {
                nextMessageBody = GetFirstOutgoingMessageBody(negotiationState, out nextMessageProperties);
            }
            else
            {
                nextMessageBody = GetNextOutgoingMessageBody(incomingMessage, negotiationState);
            }

            if (nextMessageBody != null)
            {
                Message nextMessage;
                if (incomingMessage == null)
                {
                    nextMessage = Message.CreateMessage(MessageVersion, ActionHeader.Create(RequestSecurityTokenAction, MessageVersion.Addressing), nextMessageBody);
                }
                else
                {
                    nextMessage = Message.CreateMessage(MessageVersion, ActionHeader.Create(RequestSecurityTokenResponseAction, MessageVersion.Addressing), nextMessageBody);
                }

                if (nextMessageProperties != null)
                {
                    nextMessage.Properties.CopyProperties(nextMessageProperties);
                }

                PrepareRequest(nextMessage, nextMessageBody as RequestSecurityToken);
                return nextMessage;
            }
            else
            {
                return null;
            }
        }

        private void Cleanup(IChannel rstChannel, T negotiationState)
        {
            if (negotiationState != null)
            {
                negotiationState.Dispose();
            }

            if (rstChannel != null)
            {
                rstChannel.Abort();
            }
        }

        protected virtual void ValidateKeySize(GenericXmlSecurityToken issuedToken)
        {
            if (SecurityAlgorithmSuite == null)
            {
                return;
            }

            ReadOnlyCollection<SecurityKey> issuedKeys = issuedToken.SecurityKeys;
            if (issuedKeys != null && issuedKeys.Count == 1)
            {
                SymmetricSecurityKey symmetricKey = issuedKeys[0] as SymmetricSecurityKey;
                if (symmetricKey != null)
                {
                    if (SecurityAlgorithmSuite.IsSymmetricKeyLengthSupported(symmetricKey.KeySize))
                    {
                        return;
                    }
                    else
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SRP.Format(SRP.InvalidIssuedTokenKeySize, symmetricKey.KeySize)));
                    }
                }
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SRP.Format(SRP.CannotObtainIssuedTokenKeySize)));
            }
        }

        private static bool ShouldWrapException(Exception e)
        {
            return (e is ComponentModel.Win32Exception
                || e is XmlException
                || e is InvalidOperationException
                || e is ArgumentException
                || e is QuotaExceededException
                || e is System.Security.SecurityException
                || e is System.Security.Cryptography.CryptographicException
                || e is SecurityTokenException);
        }

        private static Exception WrapExceptionIfRequired(Exception e, EndpointAddress targetAddress, EndpointAddress issuerAddress)
        {
            if (ShouldWrapException(e))
            {
                Uri targetUri;
                if (targetAddress != null)
                {
                    targetUri = targetAddress.Uri;
                }
                else
                {
                    targetUri = null;
                }

                Uri issuerUri;
                if (issuerAddress != null)
                {
                    issuerUri = issuerAddress.Uri;
                }
                else
                {
                    issuerUri = targetUri;
                }

                // => issuerUri != null
                if (targetUri != null)
                {
                    e = new SecurityNegotiationException(SRP.Format(SRP.SoapSecurityNegotiationFailedForIssuerAndTarget, issuerUri, targetUri), e);
                }
                else
                {
                    e = new SecurityNegotiationException(SRP.SoapSecurityNegotiationFailed, e);
                }
            }
            return e;
        }

        private void ValidateAndCacheServiceToken(T negotiationState)
        {
            ValidateKeySize(negotiationState.ServiceToken);
            lock (ThisLock)
            {
                if (CacheServiceTokens)
                {
                    _cachedToken = negotiationState.ServiceToken;
                }
            }
        }
    }
}
