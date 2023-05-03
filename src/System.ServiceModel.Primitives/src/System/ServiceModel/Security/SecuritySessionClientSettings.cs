// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security.Tokens;
using System.Net;
using System.Xml;
using System.Globalization;
using System.Threading.Tasks;

namespace System.ServiceModel.Security
{
    // This class is named Settings since the only public APIs are for
    // settings; however, this class also manages all functionality
    // for session channels through internal APIs

    internal static class SecuritySessionClientSettings
    {
        internal const string DefaultKeyRenewalIntervalString = "10:00:00";
        internal const string DefaultKeyRolloverIntervalString = "00:05:00";

        internal static readonly TimeSpan s_defaultKeyRenewalInterval = TimeSpan.Parse(DefaultKeyRenewalIntervalString, CultureInfo.InvariantCulture);
        internal static readonly TimeSpan s_defaultKeyRolloverInterval = TimeSpan.Parse(DefaultKeyRolloverIntervalString, CultureInfo.InvariantCulture);
        internal const bool DefaultTolerateTransportFailures = true;
    }

    internal sealed class SecuritySessionClientSettings<TChannel> : IChannelSecureConversationSessionSettings, ISecurityCommunicationObject
    {
        private SecurityProtocolFactory _sessionProtocolFactory;
        private TimeSpan _keyRenewalInterval;
        private TimeSpan _keyRolloverInterval;
        private bool _tolerateTransportFailures;
        private WrapperSecurityCommunicationObject _communicationObject;
        private SecurityStandardsManager _standardsManager;
        private SecurityTokenParameters _issuedTokenParameters;
        private int _issuedTokenRenewalThreshold;
        private object _thisLock = new object();

        public SecuritySessionClientSettings()
        {
            _keyRenewalInterval = SecuritySessionClientSettings.s_defaultKeyRenewalInterval;
            _keyRolloverInterval = SecuritySessionClientSettings.s_defaultKeyRolloverInterval;
            _tolerateTransportFailures = SecuritySessionClientSettings.DefaultTolerateTransportFailures;
            _communicationObject = new WrapperSecurityCommunicationObject(this);
        }

        private IChannelFactory InnerChannelFactory { get; set; }

        internal ChannelBuilder ChannelBuilder { get; set; }

        private SecurityChannelFactory<TChannel> SecurityChannelFactory { get; set; }

        public SecurityProtocolFactory SessionProtocolFactory
        {
            get
            {
                return _sessionProtocolFactory;
            }
            set
            {
                _communicationObject.ThrowIfDisposedOrImmutable();
                _sessionProtocolFactory = value;
            }
        }

        public TimeSpan KeyRenewalInterval
        {
            get
            {
                return _keyRenewalInterval;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), SRP.TimeSpanMustbeGreaterThanTimeSpanZero));
                }
                _communicationObject.ThrowIfDisposedOrImmutable();
                _keyRenewalInterval = value;
            }
        }

        public TimeSpan KeyRolloverInterval
        {
            get
            {
                return _keyRolloverInterval;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), SRP.TimeSpanMustbeGreaterThanTimeSpanZero));
                }
                _communicationObject.ThrowIfDisposedOrImmutable();
                _keyRolloverInterval = value;
            }
        }

        public bool TolerateTransportFailures
        {
            get
            {
                return _tolerateTransportFailures;
            }
            set
            {
                _communicationObject.ThrowIfDisposedOrImmutable();
                _tolerateTransportFailures = value;
            }
        }

        public bool CanRenewSession { get; set; } = true;

        public SecurityTokenParameters IssuedSecurityTokenParameters
        {
            get
            {
                return _issuedTokenParameters;
            }
            set
            {
                _communicationObject.ThrowIfDisposedOrImmutable();
                _issuedTokenParameters = value;
            }
        }

        public SecurityStandardsManager SecurityStandardsManager
        {
            get
            {
                return _standardsManager;
            }
            set
            {
                _communicationObject.ThrowIfDisposedOrImmutable();
                _standardsManager = value;
            }
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

        internal IChannelFactory CreateInnerChannelFactory()
        {
            if (ChannelBuilder.CanBuildChannelFactory<IDuplexSessionChannel>())
            {
                return ChannelBuilder.BuildChannelFactory<IDuplexSessionChannel>();
            }
            else if (ChannelBuilder.CanBuildChannelFactory<IDuplexChannel>())
            {
                return ChannelBuilder.BuildChannelFactory<IDuplexChannel>();
            }
            else if (ChannelBuilder.CanBuildChannelFactory<IRequestChannel>())
            {
                return ChannelBuilder.BuildChannelFactory<IRequestChannel>();
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }
        }

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

        public Task OnCloseAsync(TimeSpan timeout)
        {
            if (_sessionProtocolFactory != null)
            {
                return _sessionProtocolFactory.CloseAsync(false, timeout);
            }

            return Task.CompletedTask;
        }

        public void OnAbort()
        {
            if (_sessionProtocolFactory != null)
            {
                _sessionProtocolFactory.CloseAsync(true, TimeSpan.Zero).Wait();
            }
        }

        public Task OnOpenAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            if (_sessionProtocolFactory == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SecuritySessionProtocolFactoryShouldBeSetBeforeThisOperation));
            }
            if (_standardsManager == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SecurityStandardsManagerNotSet, GetType().ToString())));
            }
            if (_issuedTokenParameters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.IssuedSecurityTokenParametersNotSet, GetType())));
            }
            if (_keyRenewalInterval < _keyRolloverInterval)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.KeyRolloverGreaterThanKeyRenewal));
            }
            _issuedTokenRenewalThreshold = _sessionProtocolFactory.SecurityBindingElement.LocalClientSettings.CookieRenewalThresholdPercentage;
            ConfigureSessionProtocolFactory();
            return _sessionProtocolFactory.OpenAsync(true, timeoutHelper.RemainingTime());
        }

        internal Task CloseAsync(TimeSpan timeout)
        {
            return ((IAsyncCommunicationObject)_communicationObject).CloseAsync(timeout);
        }

        internal void Abort()
        {
            _communicationObject.Abort();
        }

        internal Task OpenAsync(SecurityChannelFactory<TChannel> securityChannelFactory,
            IChannelFactory innerChannelFactory, ChannelBuilder channelBuilder, TimeSpan timeout)
        {
            SecurityChannelFactory = securityChannelFactory;
            InnerChannelFactory = innerChannelFactory;
            ChannelBuilder = channelBuilder;
            return ((IAsyncCommunicationObject)_communicationObject).OpenAsync(timeout);
        }

        internal TChannel OnCreateChannel(EndpointAddress remoteAddress, Uri via)
        {
            if (typeof(TChannel) == typeof(IRequestSessionChannel))
            {
                return (TChannel)((object)(new SecurityRequestSessionChannel(this, remoteAddress, via)));
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return (TChannel)((object)(new ClientSecurityDuplexSessionChannel(this, remoteAddress, via)));
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.Format(SRP.ChannelTypeNotSupported, typeof(TChannel)), nameof(TChannel)));
            }
        }

        private void ConfigureSessionProtocolFactory()
        {
            if (_sessionProtocolFactory is SessionSymmetricTransportSecurityProtocolFactory)
            {
                SessionSymmetricTransportSecurityProtocolFactory transport = (SessionSymmetricTransportSecurityProtocolFactory)_sessionProtocolFactory;
                transport.AddTimestamp = true;
                transport.SecurityTokenParameters.RequireDerivedKeys = false;
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }
        }

        private abstract class ClientSecuritySessionChannel : ChannelBase
        {
            private ChannelParameterCollection _channelParameters;
            private SecurityToken _currentSessionToken;
            private SecurityToken _previousSessionToken;
            private DateTime _keyRenewalTime;
            private DateTime _keyRolloverTime;
            private SecurityProtocol _securityProtocol;
            private SecurityTokenProvider _sessionTokenProvider;
            private bool _isKeyRenewalOngoing = false;
            private InterruptibleWaitObject _keyRenewalCompletedEvent;
            private bool _sentClose;
            private bool _receivedClose;
            private volatile bool _isOutputClosed;
            private volatile bool _isInputClosed;
            private InterruptibleWaitObject _inputSessionClosedHandle = new InterruptibleWaitObject(false);
            private bool _isCompositeDuplexConnection;
            private Message _closeResponse;
            private InterruptibleWaitObject _outputSessionCloseHandle = new InterruptibleWaitObject(true);

            protected ClientSecuritySessionChannel(SecuritySessionClientSettings<TChannel> settings, EndpointAddress to, Uri via)
                : base(settings.SecurityChannelFactory)
            {
                Settings = settings;
                RemoteAddress = to;
                Via = via;
                _keyRenewalCompletedEvent = new InterruptibleWaitObject(false);
                MessageVersion = settings.SecurityChannelFactory.MessageVersion;
                _channelParameters = new ChannelParameterCollection(this);
                InitializeChannelBinder();
                SupportsAsyncOpenClose = true;
            }

            protected SecuritySessionClientSettings<TChannel> Settings { get; }

            protected IClientReliableChannelBinder ChannelBinder { get; private set; }

            public EndpointAddress RemoteAddress { get; }

            public Uri Via { get; }

            protected bool SendCloseHandshake { get; private set; } = false;

            protected EndpointAddress InternalLocalAddress
            {
                get
                {
                    if (ChannelBinder != null)
                    {
                        return ChannelBinder.LocalAddress;
                    }

                    return null;
                }
            }

            protected virtual bool CanDoSecurityCorrelation
            {
                get
                {
                    return false;
                }
            }

            public MessageVersion MessageVersion { get; }

            protected bool IsInputClosed
            {
                get { return _isInputClosed; }
            }

            protected bool IsOutputClosed
            {
                get { return _isOutputClosed; }
            }

            protected abstract bool ExpectClose
            {
                get;
            }

            protected abstract string SessionId
            {
                get;
            }

            public override T GetProperty<T>()
            {
                if (typeof(T) == typeof(ChannelParameterCollection))
                {
                    return _channelParameters as T;
                }

                if (typeof(T) == typeof(FaultConverter) && (ChannelBinder != null))
                {
                    return new SecurityChannelFaultConverter(ChannelBinder.Channel) as T;
                }

                T result = base.GetProperty<T>();
                if ((result == null) && (ChannelBinder != null) && (ChannelBinder.Channel != null))
                {
                    result = ChannelBinder.Channel.GetProperty<T>();
                }

                return result;
            }

            protected abstract void InitializeSession(SecurityToken sessionToken);

            private void InitializeSecurityState(SecurityToken sessionToken)
            {
                InitializeSession(sessionToken);
                _currentSessionToken = sessionToken;
                _previousSessionToken = null;
                List<SecurityToken> incomingSessionTokens = new List<SecurityToken>(1);
                incomingSessionTokens.Add(sessionToken);
                ((IInitiatorSecuritySessionProtocol)_securityProtocol).SetIdentityCheckAuthenticator(new GenericXmlSecurityTokenAuthenticator());
                ((IInitiatorSecuritySessionProtocol)_securityProtocol).SetIncomingSessionTokens(incomingSessionTokens);
                ((IInitiatorSecuritySessionProtocol)_securityProtocol).SetOutgoingSessionToken(sessionToken);
                if (CanDoSecurityCorrelation)
                {
                    ((IInitiatorSecuritySessionProtocol)_securityProtocol).ReturnCorrelationState = true;
                }
                _keyRenewalTime = GetKeyRenewalTime(sessionToken);
            }

            private void SetupSessionTokenProvider()
            {
                InitiatorServiceModelSecurityTokenRequirement requirement = new InitiatorServiceModelSecurityTokenRequirement();
                Settings.IssuedSecurityTokenParameters.InitializeSecurityTokenRequirement(requirement);
                requirement.KeyUsage = SecurityKeyUsage.Signature;
                requirement.SupportSecurityContextCancellation = true;
                requirement.SecurityAlgorithmSuite = Settings.SessionProtocolFactory.OutgoingAlgorithmSuite;
                requirement.SecurityBindingElement = Settings.SessionProtocolFactory.SecurityBindingElement;
                requirement.TargetAddress = RemoteAddress;
                requirement.Via = Via;
                requirement.MessageSecurityVersion = Settings.SessionProtocolFactory.MessageSecurityVersion.SecurityTokenVersion;

                if (_channelParameters != null)
                {
                    requirement.Properties[ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty] = _channelParameters;
                }
                //requirement.Properties[ServiceModelSecurityTokenRequirement.PrivacyNoticeVersionProperty] = Settings.SessionProtocolFactory.PrivacyNoticeVersion;
                if (ChannelBinder.LocalAddress != null)
                {
                    requirement.DuplexClientLocalAddress = ChannelBinder.LocalAddress;
                }
                _sessionTokenProvider = Settings.SessionProtocolFactory.SecurityTokenManager.CreateSecurityTokenProvider(requirement);
            }

            private async Task OpenCoreAsync(SecurityToken sessionToken, TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                _securityProtocol = Settings.SessionProtocolFactory.CreateSecurityProtocol(RemoteAddress, Via, null, true, timeoutHelper.RemainingTime());
                if (!(_securityProtocol is IInitiatorSecuritySessionProtocol))
                {
                    Fx.Assert("Security protocol must be IInitiatorSecuritySessionProtocol.");
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.ProtocolMisMatch, nameof(IInitiatorSecuritySessionProtocol), GetType().ToString())));
                }
                await _securityProtocol.OpenAsync(timeoutHelper.RemainingTime());
                await ChannelBinder.OpenAsync(timeoutHelper.RemainingTime());
                InitializeSecurityState(sessionToken);
            }

            protected override void OnFaulted()
            {
                AbortCore();
                _inputSessionClosedHandle.Fault(this);
                _keyRenewalCompletedEvent.Fault(this);
                _outputSessionCloseHandle.Fault(this);
                base.OnFaulted();
            }

            protected override void OnOpen(TimeSpan timeout)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            protected override void OnEndOpen(IAsyncResult result)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            protected internal override async Task OnOpenAsync(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                SetupSessionTokenProvider();
                await SecurityUtils.OpenTokenProviderIfRequiredAsync(_sessionTokenProvider, timeoutHelper.RemainingTime());
                using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ?
                    ServiceModelActivity.CreateBoundedActivity() : null)
                {
                    SecurityToken sessionToken = await _sessionTokenProvider.GetTokenAsync(timeoutHelper.RemainingTime());
                    // Token was issued, do send cancel on close;
                    SendCloseHandshake = true;
                    await OpenCoreAsync(sessionToken, timeoutHelper.RemainingTime());
                }
            }

            private void InitializeChannelBinder()
            {
                ChannelBuilder channelBuilder = Settings.ChannelBuilder;
                TolerateFaultsMode faultMode = Settings.TolerateTransportFailures ? TolerateFaultsMode.Always : TolerateFaultsMode.Never;
                if (channelBuilder.CanBuildChannelFactory<IDuplexSessionChannel>())
                {
                    ChannelBinder = ClientReliableChannelBinder<IDuplexSessionChannel>.CreateBinder(RemoteAddress, Via, (IChannelFactory<IDuplexSessionChannel>)(object)Settings.InnerChannelFactory,
                        MaskingMode.None, faultMode, _channelParameters, DefaultCloseTimeout, DefaultSendTimeout);
                }
                else if (channelBuilder.CanBuildChannelFactory<IDuplexChannel>())
                {
                    ChannelBinder = ClientReliableChannelBinder<IDuplexChannel>.CreateBinder(RemoteAddress, Via, (IChannelFactory<IDuplexChannel>)(object)Settings.InnerChannelFactory,
                        MaskingMode.None, faultMode, _channelParameters, DefaultCloseTimeout, DefaultSendTimeout);
                    _isCompositeDuplexConnection = true;
                }
                else if (channelBuilder.CanBuildChannelFactory<IRequestChannel>())
                {
                    ChannelBinder = ClientReliableChannelBinder<IRequestChannel>.CreateBinder(RemoteAddress, Via, (IChannelFactory<IRequestChannel>)(object)Settings.InnerChannelFactory,
                        MaskingMode.None, faultMode, _channelParameters, DefaultCloseTimeout, DefaultSendTimeout);
                }
                else if (channelBuilder.CanBuildChannelFactory<IRequestSessionChannel>())
                {
                    ChannelBinder = ClientReliableChannelBinder<IRequestSessionChannel>.CreateBinder(RemoteAddress, Via, (IChannelFactory<IRequestSessionChannel>)(object)Settings.InnerChannelFactory,
                        MaskingMode.None, faultMode, _channelParameters, DefaultCloseTimeout, DefaultSendTimeout);
                }
                ChannelBinder.Faulted += OnInnerFaulted;
            }

            private void OnInnerFaulted(IReliableChannelBinder sender, Exception exception)
            {
                Fault(exception);
            }

            protected virtual bool OnCloseResponseReceived()
            {
                bool setInputSessionClosedHandle = false;
                bool isCloseResponseExpected = false;
                lock (ThisLock)
                {
                    isCloseResponseExpected = _sentClose;
                    if (isCloseResponseExpected && !_isInputClosed)
                    {
                        _isInputClosed = true;
                        setInputSessionClosedHandle = true;
                    }
                }
                if (!isCloseResponseExpected)
                {
                    Fault(new ProtocolException(SRP.UnexpectedSecuritySessionCloseResponse));
                    return false;
                }
                if (setInputSessionClosedHandle)
                {
                    _inputSessionClosedHandle.Set();
                }
                return true;
            }

            protected virtual bool OnCloseReceived()
            {
                if (!ExpectClose)
                {
                    Fault(new ProtocolException(SRP.UnexpectedSecuritySessionClose));
                    return false;
                }
                bool setInputSessionClosedHandle = false;
                lock (ThisLock)
                {
                    if (!_isInputClosed)
                    {
                        _isInputClosed = true;
                        _receivedClose = true;
                        setInputSessionClosedHandle = true;
                    }
                }
                if (setInputSessionClosedHandle)
                {
                    _inputSessionClosedHandle.Set();
                }
                return true;
            }

            private Message PrepareCloseMessage()
            {
                SecurityToken tokenToClose;
                lock (ThisLock)
                {
                    tokenToClose = _currentSessionToken;
                }
                RequestSecurityToken rst = new RequestSecurityToken(Settings.SecurityStandardsManager);
                rst.RequestType = Settings.SecurityStandardsManager.TrustDriver.RequestTypeClose;
                rst.CloseTarget = Settings.IssuedSecurityTokenParameters.CreateKeyIdentifierClause(tokenToClose, SecurityTokenReferenceStyle.External);
                rst.MakeReadOnly();
                Message closeMessage = Message.CreateMessage(MessageVersion, ActionHeader.Create(Settings.SecurityStandardsManager.SecureConversationDriver.CloseAction, MessageVersion.Addressing), rst);

                RequestReplyCorrelator.PrepareRequest(closeMessage);

                if (InternalLocalAddress != null)
                {
                    closeMessage.Headers.ReplyTo = InternalLocalAddress;
                }
                else
                {
                    if (closeMessage.Version.Addressing == AddressingVersion.WSAddressing10)
                    {
                        closeMessage.Headers.ReplyTo = null;
                    }
                    else if (closeMessage.Version.Addressing == AddressingVersion.WSAddressingAugust2004)
                    {
                        closeMessage.Headers.ReplyTo = EndpointAddress.AnonymousAddress;
                    }
                    else
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new ProtocolException(SRP.Format(SRP.AddressingVersionNotSupported, closeMessage.Version.Addressing)));
                    }
                }
                if (TraceUtility.PropagateUserActivity || TraceUtility.ShouldPropagateActivity)
                {
                    TraceUtility.AddAmbientActivityToMessage(closeMessage);
                }
                return closeMessage;
            }

            protected async Task<SecurityProtocolCorrelationState> SendCloseMessageAsync(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                SecurityProtocolCorrelationState closeCorrelationState;
                Message closeMessage = PrepareCloseMessage();
                try
                {
                    (closeCorrelationState, closeMessage) = await _securityProtocol.SecureOutgoingMessageAsync(closeMessage, timeoutHelper.RemainingTime(), null);
                    await ChannelBinder.SendAsync(closeMessage, timeoutHelper.RemainingTime());
                }
                finally
                {
                    closeMessage.Close();
                }

                return closeCorrelationState;
            }

            protected async Task SendCloseResponseMessageAsync(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

                Message message = null;
                try
                {
                    message = _closeResponse;
                    SecurityProtocolCorrelationState dummy;
                    (dummy, message) = await _securityProtocol.SecureOutgoingMessageAsync(message, timeoutHelper.RemainingTime(), null);
                    await ChannelBinder.SendAsync(message, timeoutHelper.RemainingTime());
                }
                finally
                {
                    message.Close();
                }
            }

            private MessageFault GetProtocolFault(ref Message message, out bool isKeyRenewalFault, out bool isSessionAbortedFault)
            {
                isKeyRenewalFault = false;
                isSessionAbortedFault = false;
                MessageFault result = null;
                using (MessageBuffer buffer = message.CreateBufferedCopy(int.MaxValue))
                {
                    message = buffer.CreateMessage();
                    Message copy = buffer.CreateMessage();
                    MessageFault fault = MessageFault.CreateFault(copy, TransportDefaults.MaxSecurityFaultSize);
                    if (fault.Code.IsSenderFault)
                    {
                        FaultCode subCode = fault.Code.SubCode;
                        if (subCode != null)
                        {
                            SecurityStandardsManager standardsManager = _securityProtocol.SecurityProtocolFactory.StandardsManager;
                            SecureConversationDriver scDriver = standardsManager.SecureConversationDriver;
                            if (subCode.Namespace == scDriver.Namespace.Value && subCode.Name == scDriver.RenewNeededFaultCode.Value)
                            {
                                result = fault;
                                isKeyRenewalFault = true;
                            }
                            else if (subCode.Namespace == DotNetSecurityStrings.Namespace && subCode.Name == DotNetSecurityStrings.SecuritySessionAbortedFault)
                            {
                                result = fault;
                                isSessionAbortedFault = true;
                            }
                        }
                    }
                }
                return result;
            }

            private void ProcessKeyRenewalFault()
            {
                lock (ThisLock)
                {
                    _keyRenewalTime = DateTime.UtcNow;
                }
            }

            private void ProcessSessionAbortedFault(MessageFault sessionAbortedFault)
            {
                Fault(new FaultException(sessionAbortedFault));
            }

            private void ProcessCloseResponse(Message response)
            {
                // the close message may have been received by the channel after the channel factory has been closed
                if (response.Headers.Action != Settings.SecurityStandardsManager.SecureConversationDriver.CloseResponseAction.Value)
                {
                    throw TraceUtility.ThrowHelperError(new MessageSecurityException(SRP.Format(SRP.InvalidCloseResponseAction, response.Headers.Action)), response);
                }

                RequestSecurityTokenResponse rstr = null;
                XmlDictionaryReader bodyReader = response.GetReaderAtBodyContents();
                using (bodyReader)
                {
                    if (Settings.SecurityStandardsManager.MessageSecurityVersion.TrustVersion == TrustVersion.WSTrustFeb2005)
                    {
                        rstr = Settings.SecurityStandardsManager.TrustDriver.CreateRequestSecurityTokenResponse(bodyReader);
                    }
                    else if (Settings.SecurityStandardsManager.MessageSecurityVersion.TrustVersion == TrustVersion.WSTrust13)
                    {
                        RequestSecurityTokenResponseCollection rstrc = Settings.SecurityStandardsManager.TrustDriver.CreateRequestSecurityTokenResponseCollection(bodyReader);
                        foreach (RequestSecurityTokenResponse rstrItem in rstrc.RstrCollection)
                        {
                            if (rstr != null)
                            {
                                // More than one RSTR is found. So throw an exception.
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.MoreThanOneRSTRInRSTRC));
                            }
                            rstr = rstrItem;
                        }
                    }
                    else
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
                    }

                    response.ReadFromBodyContentsToEnd(bodyReader);
                }

                if (!rstr.IsRequestedTokenClosed)
                {
                    throw TraceUtility.ThrowHelperError(new MessageSecurityException(SRP.SessionTokenWasNotClosed), response);
                }
            }

            private void PrepareReply(Message request, Message reply)
            {
                if (request.Headers.ReplyTo != null)
                {
                    request.Headers.ReplyTo.ApplyTo(reply);
                }
                else if (request.Headers.From != null)
                {
                    request.Headers.From.ApplyTo(reply);
                }

                if (request.Headers.MessageId != null)
                {
                    reply.Headers.RelatesTo = request.Headers.MessageId;
                }

                TraceUtility.CopyActivity(request, reply);
                if (TraceUtility.PropagateUserActivity || TraceUtility.ShouldPropagateActivity)
                {
                    TraceUtility.AddActivityHeader(reply);
                }
            }

            private bool DoesSkiClauseMatchSigningToken(SecurityContextKeyIdentifierClause skiClause, Message request)
            {
                if (SessionId == null)
                {
                    return false;
                }

                return (skiClause.ContextId.ToString() == SessionId);
            }

            private void ProcessCloseMessage(Message message)
            {
                RequestSecurityToken rst;
                XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
                using (bodyReader)
                {
                    rst = Settings.SecurityStandardsManager.TrustDriver.CreateRequestSecurityToken(bodyReader);
                    message.ReadFromBodyContentsToEnd(bodyReader);
                }

                if (rst.RequestType != null && rst.RequestType != Settings.SecurityStandardsManager.TrustDriver.RequestTypeClose)
                {
                    throw TraceUtility.ThrowHelperWarning(new MessageSecurityException(SRP.Format(SRP.InvalidRstRequestType, rst.RequestType)), message);
                }

                if (rst.CloseTarget == null)
                {
                    throw TraceUtility.ThrowHelperWarning(new MessageSecurityException(SRP.NoCloseTargetSpecified), message);
                }

                SecurityContextKeyIdentifierClause sctSkiClause = rst.CloseTarget as SecurityContextKeyIdentifierClause;
                if (sctSkiClause == null || !DoesSkiClauseMatchSigningToken(sctSkiClause, message))
                {
                    throw TraceUtility.ThrowHelperWarning(new MessageSecurityException(SRP.Format(SRP.BadCloseTarget, rst.CloseTarget)), message);
                }

                // prepare the close response
                RequestSecurityTokenResponse rstr = new RequestSecurityTokenResponse(Settings.SecurityStandardsManager);
                rstr.Context = rst.Context;
                rstr.IsRequestedTokenClosed = true;
                rstr.MakeReadOnly();
                Message response = null;
                if (Settings.SecurityStandardsManager.MessageSecurityVersion.TrustVersion == TrustVersion.WSTrustFeb2005)
                {
                    response = Message.CreateMessage(message.Version, ActionHeader.Create(Settings.SecurityStandardsManager.SecureConversationDriver.CloseResponseAction, message.Version.Addressing), rstr);
                }
                else if (Settings.SecurityStandardsManager.MessageSecurityVersion.TrustVersion == TrustVersion.WSTrust13)
                {
                    List<RequestSecurityTokenResponse> rstrList = new List<RequestSecurityTokenResponse>();
                    rstrList.Add(rstr);
                    RequestSecurityTokenResponseCollection rstrCollection = new RequestSecurityTokenResponseCollection(rstrList, Settings.SecurityStandardsManager);
                    response = Message.CreateMessage(message.Version, ActionHeader.Create(Settings.SecurityStandardsManager.SecureConversationDriver.CloseResponseAction, message.Version.Addressing), rstrCollection);
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
                }
                PrepareReply(message, response);
                _closeResponse = response;
            }

            private bool ShouldWrapException(Exception e)
            {
                return ((e is FormatException) || (e is XmlException));
            }

            protected Message ProcessIncomingMessage(Message message, TimeSpan timeout, SecurityProtocolCorrelationState correlationState, out MessageFault protocolFault)
            {
                protocolFault = null;
                lock (ThisLock)
                {
                    DoKeyRolloverIfNeeded();
                }

                try
                {
                    VerifyIncomingMessage(ref message, timeout, correlationState);

                    string action = message.Headers.Action;

                    if (action == Settings.SecurityStandardsManager.SecureConversationDriver.CloseResponseAction.Value)
                    {
                        ProcessCloseResponse(message);
                        OnCloseResponseReceived();
                    }
                    else if (action == Settings.SecurityStandardsManager.SecureConversationDriver.CloseAction.Value)
                    {
                        ProcessCloseMessage(message);
                        OnCloseReceived();
                    }
                    else if (action == DotNetSecurityStrings.SecuritySessionFaultAction)
                    {
                        bool isKeyRenewalFault;
                        bool isSessionAbortedFault;
                        protocolFault = GetProtocolFault(ref message, out isKeyRenewalFault, out isSessionAbortedFault);
                        if (isKeyRenewalFault)
                        {
                            ProcessKeyRenewalFault();
                        }
                        else if (isSessionAbortedFault)
                        {
                            ProcessSessionAbortedFault(protocolFault);
                        }
                        else
                        {
                            return message;
                        }
                    }
                    else
                    {
                        return message;
                    }
                }
                catch (Exception e)
                {
                    if ((e is CommunicationException) || (e is TimeoutException) || (Fx.IsFatal(e)) || !ShouldWrapException(e))
                    {
                        throw;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SRP.MessageSecurityVerificationFailed, e));
                }

                message.Close();
                return null;
            }

            protected Message ProcessRequestContext(RequestContext requestContext, TimeSpan timeout, SecurityProtocolCorrelationState correlationState)
            {
                if (requestContext == null)
                {
                    return null;
                }
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                Message message = requestContext.RequestMessage;
                Message unverifiedMessage = message;
                try
                {
                    Exception faultException = null;
                    try
                    {
                        MessageFault dummyProtocolFault;
                        return ProcessIncomingMessage(message, timeoutHelper.RemainingTime(), correlationState, out dummyProtocolFault);
                    }
                    catch (MessageSecurityException e)
                    {
                        // if the message is an unsecured security fault from the other party over the same connection then fault the session
                        if (!_isCompositeDuplexConnection)
                        {
                            if (unverifiedMessage.IsFault)
                            {
                                MessageFault fault = MessageFault.CreateFault(unverifiedMessage, TransportDefaults.MaxSecurityFaultSize);
                                if (SecurityUtils.IsSecurityFault(fault, Settings._sessionProtocolFactory.StandardsManager))
                                {
                                    faultException = SecurityUtils.CreateSecurityFaultException(fault);
                                }
                            }
                            else
                            {
                                faultException = e;
                            }
                        }
                    }
                    if (faultException != null)
                    {
                        Fault(faultException);
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(faultException);
                    }
                    return null;
                }
                finally
                {
                    requestContext.Close(timeoutHelper.RemainingTime());
                }
            }

            /// <summary>
            /// This method removes the previous session key when the key rollover time is past.
            /// It must be called within a lock
            /// </summary>
            private void DoKeyRolloverIfNeeded()
            {
                if (DateTime.UtcNow >= _keyRolloverTime && _previousSessionToken != null)
                {
                    // forget the previous session token 
                    _previousSessionToken = null;
                    List<SecurityToken> incomingTokens = new List<SecurityToken>(1);
                    incomingTokens.Add(_currentSessionToken);
                    ((IInitiatorSecuritySessionProtocol)_securityProtocol).SetIncomingSessionTokens(incomingTokens);
                }
            }

            private DateTime GetKeyRenewalTime(SecurityToken token)
            {
                TimeSpan tokenValidityInterval = TimeSpan.FromTicks((long)(((token.ValidTo.Ticks - token.ValidFrom.Ticks) * Settings._issuedTokenRenewalThreshold) / 100));
                DateTime keyRenewalTime1 = TimeoutHelper.Add(token.ValidFrom, tokenValidityInterval);
                DateTime keyRenewalTime2 = TimeoutHelper.Add(token.ValidFrom, Settings._keyRenewalInterval);
                if (keyRenewalTime1 < keyRenewalTime2)
                {
                    return keyRenewalTime1;
                }
                else
                {
                    return keyRenewalTime2;
                }
            }

            /// <summary>
            /// This method returns true if key renewal is needed.
            /// It must be called within a lock
            /// </summary>
            private bool IsKeyRenewalNeeded()
            {
                return DateTime.UtcNow >= _keyRenewalTime;
            }

            /// <summary>
            /// When the new session token is obtained, mark the current token as previous and remove it
            /// after KeyRolloverTime. Mark the new current as pending and update the next key renewal time
            /// </summary>
            private void UpdateSessionTokens(SecurityToken newToken)
            {
                lock (ThisLock)
                {
                    _previousSessionToken = _currentSessionToken;
                    _keyRolloverTime = TimeoutHelper.Add(DateTime.UtcNow, Settings.KeyRolloverInterval);
                    _currentSessionToken = newToken;
                    _keyRenewalTime = GetKeyRenewalTime(newToken);
                    List<SecurityToken> incomingTokens = new List<SecurityToken>(2);
                    incomingTokens.Add(_previousSessionToken);
                    incomingTokens.Add(_currentSessionToken);
                    ((IInitiatorSecuritySessionProtocol)_securityProtocol).SetIncomingSessionTokens(incomingTokens);
                    ((IInitiatorSecuritySessionProtocol)_securityProtocol).SetOutgoingSessionToken(_currentSessionToken);
                }
            }

            private async Task RenewKeyAsync(TimeSpan timeout)
            {
                if (!Settings.CanRenewSession)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new SessionKeyExpiredException(SRP.SessionKeyRenewalNotSupported));
                }

                bool startKeyRenewal;
                lock (ThisLock)
                {
                    if (!_isKeyRenewalOngoing)
                    {
                        _isKeyRenewalOngoing = true;
                        _keyRenewalCompletedEvent.Reset();
                        startKeyRenewal = true;
                    }
                    else
                    {
                        startKeyRenewal = false;
                    }
                }
                if (startKeyRenewal == true)
                {
                    try
                    {
                        using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ?
                            ServiceModelActivity.CreateBoundedActivity() : null)
                        {
                            SecurityToken renewedToken = await _sessionTokenProvider.RenewTokenAsync(timeout, _currentSessionToken);
                            UpdateSessionTokens(renewedToken);
                        }
                    }
                    finally
                    {
                        lock (ThisLock)
                        {
                            _isKeyRenewalOngoing = false;
                            _keyRenewalCompletedEvent.Set();
                        }
                    }
                }
                else
                {
                    await _keyRenewalCompletedEvent.WaitAsync(timeout);
                    lock (ThisLock)
                    {
                        if (IsKeyRenewalNeeded())
                        {
                            // the key renewal attempt failed. Throw an exception to the user
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new SessionKeyExpiredException(SRP.UnableToRenewSessionKey));
                        }
                    }
                }
            }

            private bool CheckIfKeyRenewalNeeded()
            {
                bool doKeyRenewal = false;
                lock (ThisLock)
                {
                    doKeyRenewal = IsKeyRenewalNeeded();
                    DoKeyRolloverIfNeeded();
                }
                return doKeyRenewal;
            }

            protected async Task<(SecurityProtocolCorrelationState, Message)> SecureOutgoingMessageAsync(Message message, TimeSpan timeout)
            {
                bool doKeyRenewal = CheckIfKeyRenewalNeeded();
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                if (doKeyRenewal)
                {
                    await RenewKeyAsync(timeoutHelper.RemainingTime());
                }
                return await _securityProtocol.SecureOutgoingMessageAsync(message, timeoutHelper.RemainingTime(), null);
            }

            protected void VerifyIncomingMessage(ref Message message, TimeSpan timeout, SecurityProtocolCorrelationState correlationState)
            {
                _securityProtocol.VerifyIncomingMessage(ref message, timeout, correlationState);
            }

            protected virtual void AbortCore()
            {
                if (ChannelBinder != null)
                {
                    ChannelBinder.Abort();
                }
                if (_sessionTokenProvider != null)
                {
                    SecurityUtils.AbortTokenProviderIfRequired(_sessionTokenProvider);
                }
            }

            protected virtual async Task CloseCoreAsync(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

                try
                {
                    if (ChannelBinder != null)
                    {
                        await ChannelBinder.CloseAsync(timeoutHelper.RemainingTime());
                    }
                    if (_sessionTokenProvider != null)
                    {
                        SecurityUtils.CloseTokenProviderIfRequired(_sessionTokenProvider, timeoutHelper.RemainingTime());
                    }
                    _keyRenewalCompletedEvent.Abort(this);
                    _inputSessionClosedHandle.Abort(this);
                }
                catch (CommunicationObjectAbortedException)
                {
                    if (State != CommunicationState.Closed)
                    {
                        throw;
                    }
                }
            }

            protected async Task<Message> ReceiveInternalAsync(TimeSpan timeout, SecurityProtocolCorrelationState correlationState)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                while (!_isInputClosed)
                {
                    (bool success, RequestContext requestContext) = await ChannelBinder.TryReceiveAsync(timeoutHelper.RemainingTime());
                    if (success)
                    {
                        if (requestContext == null)
                        {
                            return null;
                        }

                        Message message = ProcessRequestContext(requestContext, timeoutHelper.RemainingTime(), correlationState);

                        if (message != null)
                        {
                            return message;
                        }
                    }

                    if (timeoutHelper.RemainingTime() == TimeSpan.Zero)
                    {
                        // we timed out
                        break;
                    }
                }

                return null;
            }

            protected async Task<(bool, bool)> CloseSessionAsync(TimeSpan timeout)
            {
                bool wasAborted;
                using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ?
                    ServiceModelActivity.CreateBoundedActivity() : null)
                {
                    if (DiagnosticUtility.ShouldUseActivity)
                    {
                        ServiceModelActivity.Start(activity, SRP.ActivitySecurityClose, ActivityType.SecuritySetup);
                    }

                    TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                    wasAborted = false;
                    try
                    {
                        await CloseOutputSessionAsync(timeoutHelper.RemainingTime());
                        bool sessionClosed = await _inputSessionClosedHandle.WaitAsync(timeoutHelper.RemainingTime(), false);
                        return (sessionClosed, wasAborted);
                    }
                    catch (CommunicationObjectAbortedException)
                    {
                        if (State != CommunicationState.Closed)
                        {
                            throw;
                        }
                        wasAborted = true;
                    }
                    return (false, wasAborted);
                }
            }

            private void DetermineCloseMessageToSend(out bool sendClose, out bool sendCloseResponse)
            {
                sendClose = false;
                sendCloseResponse = false;
                lock (ThisLock)
                {
                    if (!_isOutputClosed)
                    {
                        _isOutputClosed = true;
                        if (_receivedClose)
                        {
                            sendCloseResponse = true;
                        }
                        else
                        {
                            sendClose = true;
                            _sentClose = true;
                        }
                        _outputSessionCloseHandle.Reset();
                    }
                }
            }

            protected virtual async Task<SecurityProtocolCorrelationState> CloseOutputSessionAsync(TimeSpan timeout)
            {
                // TODO: Make async for real
                ThrowIfFaulted();
                if (!SendCloseHandshake)
                {
                    return null;
                }
                bool sendClose;
                bool sendCloseResponse;
                DetermineCloseMessageToSend(out sendClose, out sendCloseResponse);
                if (sendClose || sendCloseResponse)
                {
                    try
                    {
                        if (sendClose)
                        {
                            return await SendCloseMessageAsync(timeout);
                        }
                        else
                        {
                            await SendCloseResponseMessageAsync(timeout);
                            return null;
                        }
                    }
                    finally
                    {
                        _outputSessionCloseHandle.Set();
                    }
                }
                else
                {
                    return null;
                }
            }

            protected void CheckOutputOpen()
            {
                ThrowIfClosedOrNotOpen();
                lock (ThisLock)
                {
                    if (_isOutputClosed)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new CommunicationException(SRP.OutputNotExpected));
                    }
                }
            }

            protected override void OnAbort()
            {
                AbortCore();
                _inputSessionClosedHandle.Abort(this);
                _keyRenewalCompletedEvent.Abort(this);
                _outputSessionCloseHandle.Abort(this);
            }

            protected internal override async Task OnCloseAsync(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                if (SendCloseHandshake)
                {
                    bool wasAborted, wasSessionClosed;
                    (wasSessionClosed, wasAborted) = await CloseSessionAsync(timeout);
                    if (wasAborted)
                    {
                        return;
                    }

                    if (!wasSessionClosed)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new TimeoutException(SRP.Format(SRP.ClientSecurityCloseTimeout, timeout)));
                    }

                    // wait for any concurrent output session close to finish
                    try
                    {
                        if (!await _outputSessionCloseHandle.WaitAsync(timeoutHelper.RemainingTime(), false))
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new TimeoutException(SRP.Format(SRP.ClientSecurityOutputSessionCloseTimeout, timeoutHelper.OriginalTimeout)));
                        }
                    }
                    catch (CommunicationObjectAbortedException)
                    {
                        if (State == CommunicationState.Closed)
                        {
                            return;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                await CloseCoreAsync(timeoutHelper.RemainingTime());
            }

            protected override void OnClose(TimeSpan timeout)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            protected override void OnEndClose(IAsyncResult result)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            protected class SoapSecurityOutputSession : ISecureConversationSession, IOutputSession
            {
                private ClientSecuritySessionChannel _channel;
                private UniqueId _sessionId;
                private SecurityKeyIdentifierClause _sessionTokenIdentifier;
                private SecurityStandardsManager _standardsManager;

                public SoapSecurityOutputSession(ClientSecuritySessionChannel channel)
                {
                    _channel = channel;
                }

                internal void Initialize(SecurityToken sessionToken, SecuritySessionClientSettings<TChannel> settings)
                {
                    if (sessionToken == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(sessionToken));
                    }
                    if (settings == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(settings));
                    }
                    Claim identityClaim = SecurityUtils.GetPrimaryIdentityClaim(((GenericXmlSecurityToken)sessionToken).AuthorizationPolicies);
                    if (identityClaim != null)
                    {
                        RemoteIdentity = EndpointIdentity.CreateIdentity(identityClaim);
                    }
                    _standardsManager = settings.SessionProtocolFactory.StandardsManager;
                    _sessionId = GetSessionId(sessionToken, _standardsManager);
                    _sessionTokenIdentifier = settings.IssuedSecurityTokenParameters.CreateKeyIdentifierClause(sessionToken,
                        SecurityTokenReferenceStyle.External);
                }

                private UniqueId GetSessionId(SecurityToken sessionToken, SecurityStandardsManager standardsManager)
                {
                    GenericXmlSecurityToken gxt = sessionToken as GenericXmlSecurityToken;
                    if (gxt == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.SessionTokenIsNotGenericXmlToken, sessionToken, typeof(GenericXmlSecurityToken))));
                    }
                    return standardsManager.SecureConversationDriver.GetSecurityContextTokenId(XmlDictionaryReader.CreateDictionaryReader(new XmlNodeReader(gxt.TokenXml)));
                }

                public string Id
                {
                    get
                    {
                        if (_sessionId == null)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.ChannelMustBeOpenedToGetSessionId));
                        }
                        return _sessionId.ToString();
                    }
                }

                public EndpointIdentity RemoteIdentity { get; private set; }

                public void WriteSessionTokenIdentifier(XmlDictionaryWriter writer)
                {
                    _channel.ThrowIfDisposedOrNotOpen();
                    _standardsManager.SecurityTokenSerializer.WriteKeyIdentifierClause(writer, _sessionTokenIdentifier);
                }

                public bool TryReadSessionTokenIdentifier(XmlReader reader)
                {
                    _channel.ThrowIfDisposedOrNotOpen();
                    if (!_standardsManager.SecurityTokenSerializer.CanReadKeyIdentifierClause(reader))
                    {
                        return false;
                    }
                    SecurityContextKeyIdentifierClause incomingTokenIdentifier =
                        _standardsManager.SecurityTokenSerializer.ReadKeyIdentifierClause(reader) as SecurityContextKeyIdentifierClause;
                    return incomingTokenIdentifier != null && incomingTokenIdentifier.Matches(_sessionId, null);
                }
            }
        }

        private abstract class ClientSecuritySimplexSessionChannel : ClientSecuritySessionChannel
        {
            private SoapSecurityOutputSession _outputSession;

            protected ClientSecuritySimplexSessionChannel(SecuritySessionClientSettings<TChannel> settings, EndpointAddress to, Uri via)
                : base(settings, to, via)
            {
                _outputSession = new SoapSecurityOutputSession(this);
            }

            public IOutputSession Session
            {
                get
                {
                    return _outputSession;
                }
            }

            protected override bool ExpectClose
            {
                get { return false; }
            }

            protected override string SessionId
            {
                get { return Session.Id; }
            }

            protected override void InitializeSession(SecurityToken sessionToken)
            {
                _outputSession.Initialize(sessionToken, Settings);
            }
        }

        private sealed class SecurityRequestSessionChannel : ClientSecuritySimplexSessionChannel, IAsyncRequestSessionChannel
        {
            public SecurityRequestSessionChannel(SecuritySessionClientSettings<TChannel> settings, EndpointAddress to, Uri via)
                : base(settings, to, via)
            {
            }

            protected override bool CanDoSecurityCorrelation
            {
                get
                {
                    return true;
                }
            }

            protected override async Task<SecurityProtocolCorrelationState> CloseOutputSessionAsync(TimeSpan timeout)
            {
                ThrowIfFaulted();
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                SecurityProtocolCorrelationState correlationState = await base.CloseOutputSessionAsync(timeoutHelper.RemainingTime());

                Message message = await ReceiveInternalAsync(timeoutHelper.RemainingTime(), correlationState);

                if (message != null)
                {
                    using (message)
                    {
                        ProtocolException error = ProtocolException.ReceiveShutdownReturnedNonNull(message);
                        throw TraceUtility.ThrowHelperWarning(error, message);
                    }
                }
                return null;
            }

            public Task<Message> RequestAsync(Message message)
            {
                return RequestAsync(message, DefaultSendTimeout);
            }

            Message IRequestChannel.Request(Message message) => ((IRequestChannel)this).Request(message, DefaultSendTimeout);

            Message IRequestChannel.Request(Message message, TimeSpan timeout) => RequestAsync(message, timeout).WaitForCompletionNoSpin();

            IAsyncResult IRequestChannel.BeginRequest(Message message, AsyncCallback callback, object state) => ((IRequestChannel)this).BeginRequest(message, DefaultSendTimeout, callback, state);

            IAsyncResult IRequestChannel.BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state) => RequestAsync(message, timeout).ToApm(callback, state);

            Message IRequestChannel.EndRequest(IAsyncResult result) => result.ToApmEnd<Message>();

            private Message ProcessReply(Message reply, TimeSpan timeout, SecurityProtocolCorrelationState correlationState)
            {
                if (reply == null)
                {
                    return null;
                }
                Message unverifiedReply = reply;
                Message processedReply = null;
                MessageFault protocolFault = null;
                Exception faultException = null;
                try
                {
                    processedReply = ProcessIncomingMessage(reply, timeout, correlationState, out protocolFault);
                }
                catch (MessageSecurityException)
                {
                    if (unverifiedReply.IsFault)
                    {
                        MessageFault fault = MessageFault.CreateFault(unverifiedReply, TransportDefaults.MaxSecurityFaultSize);
                        if (SecurityUtils.IsSecurityFault(fault, Settings._standardsManager))
                        {
                            faultException = SecurityUtils.CreateSecurityFaultException(fault);
                        }
                    }
                    if (faultException == null)
                    {
                        throw;
                    }
                }
                if (faultException != null)
                {
                    Fault(faultException);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(faultException);
                }
                if (processedReply == null && protocolFault != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.SecuritySessionFaultReplyWasSent, new FaultException(protocolFault)));
                }
                return processedReply;
            }

            public async Task<Message> RequestAsync(Message message, TimeSpan timeout)
            {
                ThrowIfFaulted();
                CheckOutputOpen();
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                SecurityProtocolCorrelationState correlationState;
                await TaskHelpers.EnsureDefaultTaskScheduler();
                (correlationState, message) = await SecureOutgoingMessageAsync(message, timeoutHelper.RemainingTime());
                Message reply = await ChannelBinder.RequestAsync(message, timeoutHelper.RemainingTime());
                return ProcessReply(reply, timeoutHelper.RemainingTime(), correlationState);
            }
        }

        private class ClientSecurityDuplexSessionChannel : ClientSecuritySessionChannel, IAsyncDuplexSessionChannel
        {
            private SoapSecurityClientDuplexSession _session;
            private InputQueue<Message> _queue;

            public ClientSecurityDuplexSessionChannel(SecuritySessionClientSettings<TChannel> settings, EndpointAddress to, Uri via)
                : base(settings, to, via)
            {
                _session = new SoapSecurityClientDuplexSession(this);
                _queue = TraceUtility.CreateInputQueue<Message>();
            }

            public EndpointAddress LocalAddress
            {
                get
                {
                    return base.InternalLocalAddress;
                }
            }

            IDuplexSession ISessionChannel<IDuplexSession>.Session
            {
                get
                {
                    return _session;
                }
            }

            IAsyncDuplexSession ISessionChannel<IAsyncDuplexSession>.Session
            {
                get
                {
                    return _session;
                }
            }

            protected override bool ExpectClose
            {
                get { return true; }
            }

            protected override string SessionId
            {
                get { return _session.Id; }
            }

            public Message Receive() => ReceiveAsync().GetAwaiter().GetResult();

            public Task<Message> ReceiveAsync() => ReceiveAsync(DefaultReceiveTimeout);

            public Message Receive(TimeSpan timeout) => ReceiveAsync(timeout).GetAwaiter().GetResult();

            public Task<Message> ReceiveAsync(TimeSpan timeout) => InputChannel.HelpReceiveAsync(this, timeout);

            public IAsyncResult BeginReceive(AsyncCallback callback, object state) => ReceiveAsync().ToApm(callback, state);

            public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state) => ReceiveAsync(timeout).ToApm(callback, state);

            public Message EndReceive(IAsyncResult result) => result.ToApmEnd<Message>();

            public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state) => TryReceiveAsync(timeout).ToApm(callback, state);

            public bool EndTryReceive(IAsyncResult result, out Message message)
            {
                bool success;
                (success, message) = result.ToApmEnd<(bool, Message)>();
                return success;
            }

            protected override void OnOpened()
            {
                base.OnOpened();
                StartReceiving();
            }

            public bool TryReceive(TimeSpan timeout, out Message message)
            {
                bool success;
                (success, message) = TryReceiveAsync(timeout).GetAwaiter().GetResult();
                return success;
            }

            public async Task<(bool, Message)> TryReceiveAsync(TimeSpan timeout)
            {
                ThrowIfFaulted();
                await TaskHelpers.EnsureDefaultTaskScheduler();
                (bool wasDequeued, Message message) = await _queue.TryDequeueAsync(timeout);
                if (message == null)
                {
                    // the channel could have faulted, shutting down the input queue
                    ThrowIfFaulted();
                }
                return (wasDequeued, message);
            }

            public void Send(Message message) => SendAsync(message).WaitForCompletionNoSpin();

            public Task SendAsync(Message message) => SendAsync(message, DefaultSendTimeout);

            public void Send(Message message, TimeSpan timeout) => SendAsync(message, timeout).WaitForCompletionNoSpin();

            public async Task SendAsync(Message message, TimeSpan timeout)
            {
                ThrowIfFaulted();
                CheckOutputOpen();
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                SecurityProtocolCorrelationState dummy;
                await TaskHelpers.EnsureDefaultTaskScheduler();
                (dummy, message) = await SecureOutgoingMessageAsync(message, timeoutHelper.RemainingTime());
                await ChannelBinder.SendAsync(message, timeoutHelper.RemainingTime());
            }

            public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state) => SendAsync(message).ToApm(callback, state);

            public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state) => SendAsync(message, timeout).ToApm(callback, state);

            public void EndSend(IAsyncResult result) => result.ToApmEnd();

            protected override void InitializeSession(SecurityToken sessionToken)
            {
                _session.Initialize(sessionToken, Settings);
            }

            private async void StartReceiving()
            {
                while (true)
                {
                    // no need to receive anymore if in the closed state
                    if (State == CommunicationState.Closed || State == CommunicationState.Faulted || IsInputClosed)
                    {
                        return;
                    }
                    try
                    {
                        Message message = await ReceiveInternalAsync(TimeSpan.MaxValue, null);
                        if (message != null)
                        {
                            ActionItem.Schedule(Fx.ThunkCallback<object>(state =>
                            {
                                try
                                {
                                    _queue.EnqueueAndDispatch(message);
                                }
                                catch (Exception e)
                                {
                                    if (Fx.IsFatal(e))
                                    {
                                        throw;
                                    }
                                }
                            }), null);
                        }
                    }
                    catch (CommunicationException)
                    {
                        // BeginReceive failed. ignore the exception and start another receive
                    }
                    catch (TimeoutException)
                    {
                        // BeginReceive failed. ignore the exception and start another receive
                    }
                }
            }

            protected override void AbortCore()
            {
                try
                {
                    _queue.Dispose();
                }
                catch (CommunicationException)
                {
                }
                catch (TimeoutException)
                {
                }
                base.AbortCore();
            }

            public bool WaitForMessage(TimeSpan timeout) => WaitForMessageAsync(timeout).GetAwaiter().GetResult();

            public Task<bool> WaitForMessageAsync(TimeSpan timeout)
            {
                return _queue.WaitForItemAsync(timeout);
            }

            public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state) => WaitForMessageAsync(timeout).ToApm(callback, state);

            public bool EndWaitForMessage(IAsyncResult result) => result.ToApmEnd<bool>();

            protected override void OnFaulted()
            {
                _queue.Shutdown(() => GetPendingException());
                base.OnFaulted();
            }

            protected override bool OnCloseResponseReceived()
            {
                if (base.OnCloseResponseReceived())
                {
                    _queue.Shutdown();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            protected override bool OnCloseReceived()
            {
                if (base.OnCloseReceived())
                {
                    _queue.Shutdown();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private class SoapSecurityClientDuplexSession : SoapSecurityOutputSession, IDuplexSession, IAsyncDuplexSession
            {
                private ClientSecurityDuplexSessionChannel _channel;
                private bool _initialized = false;

                public SoapSecurityClientDuplexSession(ClientSecurityDuplexSessionChannel channel)
                    : base(channel)
                {
                    _channel = channel;
                }

                internal new void Initialize(SecurityToken sessionToken, SecuritySessionClientSettings<TChannel> settings)
                {
                    base.Initialize(sessionToken, settings);
                    _initialized = true;
                }

                private void CheckInitialized()
                {
                    if (!_initialized)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.ChannelNotOpen));
                    }
                }

                public void CloseOutputSession() => CloseOutputSessionAsync().GetAwaiter().GetResult();

                public Task CloseOutputSessionAsync()
                {
                    return CloseOutputSessionAsync(_channel.DefaultCloseTimeout);
                }

                public void CloseOutputSession(TimeSpan timeout) => CloseOutputSessionAsync(timeout).GetAwaiter().GetResult();

                public async Task CloseOutputSessionAsync(TimeSpan timeout)
                {
                    CheckInitialized();
                    _channel.ThrowIfFaulted();
                    _channel.ThrowIfNotOpened();
                    Exception pendingException = null;
                    try
                    {
                        await _channel.CloseOutputSessionAsync(timeout);
                    }
                    catch (CommunicationObjectAbortedException)
                    {
                        if (_channel.State != CommunicationState.Closed)
                        {
                            throw;
                        }
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                        {
                            throw;
                        }

                        pendingException = e;
                    }
                    if (pendingException != null)
                    {
                        _channel.Fault(pendingException);
                        throw pendingException;
                    }
                }

                public IAsyncResult BeginCloseOutputSession(AsyncCallback callback, object state) => CloseOutputSessionAsync().ToApm(callback, state);

                public IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state) => CloseOutputSessionAsync(timeout).ToApm(callback, state);

                public void EndCloseOutputSession(IAsyncResult result) => result.ToApmEnd();
            }
        }
    }
}
