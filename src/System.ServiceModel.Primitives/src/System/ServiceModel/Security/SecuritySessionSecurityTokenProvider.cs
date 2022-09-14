// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security.Tokens;
using System.Net;
using System.Xml;
using System.Threading.Tasks;

namespace System.ServiceModel.Security
{
    internal class SecuritySessionSecurityTokenProvider : CommunicationObjectSecurityTokenProvider
    {
        private static readonly MessageOperationFormatter s_operationFormatter = new MessageOperationFormatter();

        private BindingContext _issuerBindingContext;
        private SecurityChannelFactory<IAsyncRequestChannel> _rstChannelFactory;
        private SecurityAlgorithmSuite _securityAlgorithmSuite;
        private SecurityStandardsManager _standardsManager;
        private Object _thisLock = new Object();
        private SecurityKeyEntropyMode _keyEntropyMode;
        private SecurityTokenParameters _issuedTokenParameters;
        private bool _requiresManualReplyAddressing;
        private EndpointAddress _targetAddress;
        private SecurityBindingElement _bootstrapSecurityBindingElement;
        private Uri _via;
        private string _sctUri;
        private Uri _privacyNoticeUri;
        private int _privacyNoticeVersion;
        private EndpointAddress _localAddress;
        private ChannelParameterCollection _channelParameters;

        public SecuritySessionSecurityTokenProvider()
            : base()
        {
            _standardsManager = SecurityStandardsManager.DefaultInstance;
            _keyEntropyMode = SecurityKeyEntropyMode.CombinedEntropy;
        }

        public SecurityAlgorithmSuite SecurityAlgorithmSuite
        {
            get
            {
                return _securityAlgorithmSuite;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _securityAlgorithmSuite = value;
            }
        }

        public SecurityKeyEntropyMode KeyEntropyMode
        {
            get
            {
                return _keyEntropyMode;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                SecurityKeyEntropyModeHelper.Validate(value);
                _keyEntropyMode = value;
            }
        }

        private MessageVersion MessageVersion { get; set; }

        public EndpointAddress TargetAddress
        {
            get { return _targetAddress; }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _targetAddress = value;
            }
        }

        public EndpointAddress LocalAddress
        {
            get { return _localAddress; }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _localAddress = value;
            }
        }

        public Uri Via
        {
            get { return _via; }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _via = value;
            }
        }

        public BindingContext IssuerBindingContext
        {
            get
            {
                return _issuerBindingContext;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }
                _issuerBindingContext = value.Clone();
            }
        }

        public SecurityBindingElement BootstrapSecurityBindingElement
        {
            get { return _bootstrapSecurityBindingElement; }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }
                _bootstrapSecurityBindingElement = (SecurityBindingElement)value.Clone();
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
                CommunicationObject.ThrowIfDisposedOrImmutable();
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(value)));
                }
                if (!value.TrustDriver.IsSessionSupported)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.TrustDriverVersionDoesNotSupportSession, nameof(value)));
                }
                if (!value.SecureConversationDriver.IsSessionSupported)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.SecureConversationDriverVersionDoesNotSupportSession, nameof(value)));
                }
                _standardsManager = value;
            }
        }

        public SecurityTokenParameters IssuedSecurityTokenParameters
        {
            get
            {
                return _issuedTokenParameters;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _issuedTokenParameters = value;
            }
        }

        public Uri PrivacyNoticeUri
        {
            get { return _privacyNoticeUri; }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _privacyNoticeUri = value;
            }
        }

        public ChannelParameterCollection ChannelParameters
        {
            get { return _channelParameters; }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _channelParameters = value;
            }
        }

        public int PrivacyNoticeVersion
        {
            get { return _privacyNoticeVersion; }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _privacyNoticeVersion = value;
            }
        }

        public virtual XmlDictionaryString IssueAction
        {
            get
            {
                return _standardsManager.SecureConversationDriver.IssueAction;
            }
        }

        public virtual XmlDictionaryString IssueResponseAction
        {
            get
            {
                return _standardsManager.SecureConversationDriver.IssueResponseAction;
            }
        }

        public virtual XmlDictionaryString RenewAction
        {
            get
            {
                return _standardsManager.SecureConversationDriver.RenewAction;
            }
        }

        public virtual XmlDictionaryString RenewResponseAction
        {
            get
            {
                return _standardsManager.SecureConversationDriver.RenewResponseAction;
            }
        }

        public virtual XmlDictionaryString CloseAction
        {
            get
            {
                return _standardsManager.SecureConversationDriver.CloseAction;
            }
        }

        public virtual XmlDictionaryString CloseResponseAction
        {
            get
            {
                return _standardsManager.SecureConversationDriver.CloseResponseAction;
            }
        }

        // ISecurityCommunicationObject methods
        public override void OnAbort()
        {
            if (_rstChannelFactory != null)
            {
                _rstChannelFactory.Abort();
                _rstChannelFactory = null;
            }
            FreeCredentialsHandle();
        }

        public override async Task OnOpenAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            if (_targetAddress == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.TargetAddressIsNotSet, GetType())));
            }
            if (IssuerBindingContext == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.IssuerBuildContextNotSet, GetType())));
            }
            if (IssuedSecurityTokenParameters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.IssuedSecurityTokenParametersNotSet, GetType())));
            }
            if (BootstrapSecurityBindingElement == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.BootstrapSecurityBindingElementNotSet, GetType())));
            }
            if (SecurityAlgorithmSuite == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SecurityAlgorithmSuiteNotSet, GetType())));
            }
            InitializeFactories();
            await _rstChannelFactory.OpenHelperAsync(timeoutHelper.RemainingTime());
            _sctUri = StandardsManager.SecureConversationDriver.TokenTypeUri;
        }

        public override void OnOpening()
        {
            base.OnOpening();
            if (IssuerBindingContext == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.IssuerBuildContextNotSet, GetType())));
            }
            if (BootstrapSecurityBindingElement == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.BootstrapSecurityBindingElementNotSet, GetType())));
            }
        }

        public override async Task OnCloseAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            if (_rstChannelFactory != null)
            {
                await _rstChannelFactory.CloseHelperAsync(timeoutHelper.RemainingTime());
                _rstChannelFactory = null;
            }
            FreeCredentialsHandle();
        }

        private void FreeCredentialsHandle()
        {
        }

        private void InitializeFactories()
        {
            ISecurityCapabilities securityCapabilities = BootstrapSecurityBindingElement.GetProperty<ISecurityCapabilities>(IssuerBindingContext);
            SecurityCredentialsManager securityCredentials = IssuerBindingContext.BindingParameters.Find<SecurityCredentialsManager>();
            if (securityCredentials == null)
            {
                securityCredentials = ClientCredentials.CreateDefaultCredentials();
            }

            BindingContext context = IssuerBindingContext;
            _bootstrapSecurityBindingElement.ReaderQuotas = context.GetInnerProperty<XmlDictionaryReaderQuotas>();
            if (_bootstrapSecurityBindingElement.ReaderQuotas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.EncodingBindingElementDoesNotHandleReaderQuotas));
            }

            TransportBindingElement transportBindingElement = context.RemainingBindingElements.Find<TransportBindingElement>();
            if (transportBindingElement != null)
            {
                _bootstrapSecurityBindingElement.MaxReceivedMessageSize = transportBindingElement.MaxReceivedMessageSize;
            }

            SecurityProtocolFactory securityProtocolFactory = BootstrapSecurityBindingElement.CreateSecurityProtocolFactory<IRequestChannel>(IssuerBindingContext.Clone(), securityCredentials, false, IssuerBindingContext.Clone());

            if (_localAddress != null)
            {
                MessageFilter issueAndRenewFilter = new SessionActionFilter(_standardsManager, IssueResponseAction.Value, RenewResponseAction.Value);
                context.BindingParameters.Add(new LocalAddressProvider(_localAddress, issueAndRenewFilter));
            }

            ChannelBuilder channelBuilder = new ChannelBuilder(context, true);
            IChannelFactory innerChannelFactory;
            // if the underlying transport does not support request/reply, wrap it inside
            // a service channel factory.
            if (channelBuilder.CanBuildChannelFactory<IRequestChannel>())
            {
                innerChannelFactory = channelBuilder.BuildChannelFactory<IRequestChannel>();
                _requiresManualReplyAddressing = true;
            }
            else
            {
                ClientRuntime clientRuntime = new ClientRuntime("RequestSecuritySession", NamingHelper.DefaultNamespace);
                clientRuntime.UseSynchronizationContext = false;
                clientRuntime.AddTransactionFlowProperties = false;
                clientRuntime.ValidateMustUnderstand = false;
                ServiceChannelFactory serviceChannelFactory = ServiceChannelFactory.BuildChannelFactory(channelBuilder, clientRuntime);

                ClientOperation issueOperation = new ClientOperation(serviceChannelFactory.ClientRuntime, "Issue", IssueAction.Value);
                issueOperation.Formatter = s_operationFormatter;
                serviceChannelFactory.ClientRuntime.Operations.Add(issueOperation);

                ClientOperation renewOperation = new ClientOperation(serviceChannelFactory.ClientRuntime, "Renew", RenewAction.Value);
                renewOperation.Formatter = s_operationFormatter;
                serviceChannelFactory.ClientRuntime.Operations.Add(renewOperation);
                innerChannelFactory = new RequestChannelFactory(serviceChannelFactory);
                _requiresManualReplyAddressing = false;
            }

            SecurityChannelFactory<IAsyncRequestChannel> securityChannelFactory = new SecurityChannelFactory<IAsyncRequestChannel>(
                securityCapabilities, IssuerBindingContext, channelBuilder, securityProtocolFactory, innerChannelFactory);

            // attach the ExtendedProtectionPolicy to the securityProtcolFactory so it will be 
            // available when building the channel.
            if (transportBindingElement != null)
            {
                if (securityChannelFactory.SecurityProtocolFactory != null)
                {
                    securityChannelFactory.SecurityProtocolFactory.ExtendedProtectionPolicy = transportBindingElement.GetProperty<ExtendedProtectionPolicy>(context);
                }
            }

            _rstChannelFactory = securityChannelFactory;
            MessageVersion = securityChannelFactory.MessageVersion;
        }

        // token provider methods
        protected override SecurityToken GetTokenCore(TimeSpan timeout)
        {
            CommunicationObject.ThrowIfClosedOrNotOpen();
            return DoOperationAsync(SecuritySessionOperation.Issue, _targetAddress, _via, null, timeout).GetAwaiter().GetResult();
        }

        internal override Task<SecurityToken> GetTokenCoreInternalAsync(TimeSpan timeout)
        {
            CommunicationObject.ThrowIfClosedOrNotOpen();
            return DoOperationAsync(SecuritySessionOperation.Issue, _targetAddress, _via, null, timeout);
        }

        internal override Task<SecurityToken> RenewTokenCoreInternalAsync(TimeSpan timeout, SecurityToken tokenToBeRenewed)
        {
            CommunicationObject.ThrowIfClosedOrNotOpen();
            return DoOperationAsync(SecuritySessionOperation.Renew, _targetAddress, _via, tokenToBeRenewed, timeout);
        }

        private IAsyncRequestChannel CreateChannel(SecuritySessionOperation operation, EndpointAddress target, Uri via)
        {
            IChannelFactory<IAsyncRequestChannel> cf;
            if (operation == SecuritySessionOperation.Issue || operation == SecuritySessionOperation.Renew)
            {
                cf = _rstChannelFactory;
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }

            IAsyncRequestChannel channel;
            if (via != null)
            {
                channel = cf.CreateChannel(target, via);
            }
            else
            {
                channel = cf.CreateChannel(target);
            }

            if (_channelParameters != null)
            {
                _channelParameters.PropagateChannelParameters(channel);
            }

            return channel;
        }

        private Message CreateRequest(SecuritySessionOperation operation, EndpointAddress target, SecurityToken currentToken, out object requestState)
        {
            if (operation == SecuritySessionOperation.Issue)
            {
                return CreateIssueRequest(target, out requestState);
            }
            else if (operation == SecuritySessionOperation.Renew)
            {
                return CreateRenewRequest(target, currentToken, out requestState);
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }
        }

        private GenericXmlSecurityToken ProcessReply(Message reply, SecuritySessionOperation operation, object requestState)
        {
            ThrowIfFault(reply, _targetAddress);
            GenericXmlSecurityToken issuedToken = null;
            if (operation == SecuritySessionOperation.Issue)
            {
                issuedToken = ProcessIssueResponse(reply, requestState);
            }
            else if (operation == SecuritySessionOperation.Renew)
            {
                issuedToken = ProcessRenewResponse(reply, requestState);
            }

            return issuedToken;
        }

        private void OnOperationSuccess(SecuritySessionOperation operation, EndpointAddress target, SecurityToken issuedToken, SecurityToken currentToken)
        {
        }

        private void OnOperationFailure(SecuritySessionOperation operation, EndpointAddress target, SecurityToken currentToken, Exception e, IChannel channel)
        {
            if (channel != null)
            {
                channel.Abort();
            }
        }

        private async Task<SecurityToken> DoOperationAsync(SecuritySessionOperation operation, EndpointAddress target, Uri via, SecurityToken currentToken, TimeSpan timeout)
        {
            if (target == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(target));
            }

            if (operation == SecuritySessionOperation.Renew && currentToken == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(currentToken));
            }

            IAsyncRequestChannel channel = null;
            try
            {
                channel = CreateChannel(operation, target, via);

                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                await channel.OpenAsync(timeoutHelper.RemainingTime());
                object requestState;
                GenericXmlSecurityToken issuedToken;

                using (Message requestMessage = CreateRequest(operation, target, currentToken, out requestState))
                {
                    EventTraceActivity eventTraceActivity = null;

                    TraceUtility.ProcessOutgoingMessage(requestMessage, eventTraceActivity);

                    using (Message reply = await channel.RequestAsync(requestMessage, timeoutHelper.RemainingTime()))
                    {
                        if (reply == null)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(SRP.FailToReceiveReplyFromNegotiation));
                        }

                        TraceUtility.ProcessIncomingMessage(reply, eventTraceActivity);
                        ThrowIfFault(reply, _targetAddress);
                        issuedToken = ProcessReply(reply, operation, requestState);
                        ValidateKeySize(issuedToken);
                    }
                }
                await channel.CloseAsync(timeoutHelper.RemainingTime());
                OnOperationSuccess(operation, target, issuedToken, currentToken);
                return issuedToken;
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                if (e is TimeoutException)
                {
                    e = new TimeoutException(SRP.Format(SRP.ClientSecuritySessionRequestTimeout, timeout), e);
                }

                OnOperationFailure(operation, target, currentToken, e, channel);
                throw;
            }
        }

        private byte[] GenerateEntropy(int entropySize)
        {
            byte[] result = Fx.AllocateByteArray(entropySize / 8);
            IdentityModel.CryptoHelper.FillRandomBytes(result);
            return result;
        }

        private RequestSecurityToken CreateRst(EndpointAddress target, out object requestState)
        {
            RequestSecurityToken rst = new RequestSecurityToken(_standardsManager);
            rst.KeySize = SecurityAlgorithmSuite.DefaultSymmetricKeyLength;
            rst.TokenType = _sctUri;
            if (KeyEntropyMode == SecurityKeyEntropyMode.ClientEntropy || KeyEntropyMode == SecurityKeyEntropyMode.CombinedEntropy)
            {
                byte[] entropy = GenerateEntropy(rst.KeySize);
                rst.SetRequestorEntropy(entropy);
                requestState = entropy;
            }
            else
            {
                requestState = null;
            }

            return rst;
        }

        private void PrepareRequest(Message message)
        {
            RequestReplyCorrelator.PrepareRequest(message);
            if (_requiresManualReplyAddressing)
            {
                if (_localAddress != null)
                {
                    message.Headers.ReplyTo = LocalAddress;
                }
                else
                {
                    message.Headers.ReplyTo = EndpointAddress.AnonymousAddress;
                }
            }
        }

        protected virtual Message CreateIssueRequest(EndpointAddress target, out object requestState)
        {
            CommunicationObject.ThrowIfClosedOrNotOpen();
            RequestSecurityToken rst = CreateRst(target, out requestState);
            rst.RequestType = StandardsManager.TrustDriver.RequestTypeIssue;
            rst.MakeReadOnly();
            Message result = Message.CreateMessage(MessageVersion, ActionHeader.Create(IssueAction, MessageVersion.Addressing), rst);
            PrepareRequest(result);
            return result;
        }

        private GenericXmlSecurityToken ExtractToken(Message response, object requestState)
        {
            // get the claims corresponding to the server
            SecurityMessageProperty serverContextProperty = response.Properties.Security;
            ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies;
            if (serverContextProperty != null && serverContextProperty.ServiceSecurityContext != null)
            {
                authorizationPolicies = serverContextProperty.ServiceSecurityContext.AuthorizationPolicies;
            }
            else
            {
                authorizationPolicies = EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance;
            }

            RequestSecurityTokenResponse rstr = null;
            XmlDictionaryReader bodyReader = response.GetReaderAtBodyContents();
            using (bodyReader)
            {
                if (StandardsManager.MessageSecurityVersion.TrustVersion == TrustVersion.WSTrustFeb2005)
                {
                    rstr = StandardsManager.TrustDriver.CreateRequestSecurityTokenResponse(bodyReader);
                }
                else if (StandardsManager.MessageSecurityVersion.TrustVersion == TrustVersion.WSTrust13)
                {
                    RequestSecurityTokenResponseCollection rstrc = StandardsManager.TrustDriver.CreateRequestSecurityTokenResponseCollection(bodyReader);
                    foreach (RequestSecurityTokenResponse rstrItem in rstrc.RstrCollection)
                    {
                        if (rstr != null)
                        {
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

            byte[] requestorEntropy;
            if (requestState != null)
            {
                requestorEntropy = (byte[])requestState;
            }
            else
            {
                requestorEntropy = null;
            }

            GenericXmlSecurityToken issuedToken = rstr.GetIssuedToken(null, null, KeyEntropyMode, requestorEntropy, _sctUri, authorizationPolicies, SecurityAlgorithmSuite.DefaultSymmetricKeyLength, false);
            return issuedToken;
        }

        protected virtual GenericXmlSecurityToken ProcessIssueResponse(Message response, object requestState)
        {
            CommunicationObject.ThrowIfClosedOrNotOpen();
            return ExtractToken(response, requestState);
        }

        protected virtual Message CreateRenewRequest(EndpointAddress target, SecurityToken currentSessionToken, out object requestState)
        {
            CommunicationObject.ThrowIfClosedOrNotOpen();
            RequestSecurityToken rst = CreateRst(target, out requestState);
            rst.RequestType = StandardsManager.TrustDriver.RequestTypeRenew;
            rst.RenewTarget = IssuedSecurityTokenParameters.CreateKeyIdentifierClause(currentSessionToken, SecurityTokenReferenceStyle.External);
            rst.MakeReadOnly();
            Message result = Message.CreateMessage(MessageVersion, ActionHeader.Create(RenewAction, MessageVersion.Addressing), rst);
            SecurityMessageProperty supportingTokenProperty = new SecurityMessageProperty();
            supportingTokenProperty.OutgoingSupportingTokens.Add(new SupportingTokenSpecification(currentSessionToken, EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance, SecurityTokenAttachmentMode.Endorsing, IssuedSecurityTokenParameters));
            result.Properties.Security = supportingTokenProperty;
            PrepareRequest(result);
            return result;
        }

        protected virtual GenericXmlSecurityToken ProcessRenewResponse(Message response, object requestState)
        {
            CommunicationObject.ThrowIfClosedOrNotOpen();
            if (response.Headers.Action != RenewResponseAction.Value)
            {
                throw TraceUtility.ThrowHelperError(new SecurityNegotiationException(SRP.Format(SRP.InvalidRenewResponseAction, response.Headers.Action)), response);
            }

            return ExtractToken(response, requestState);
        }

        protected static void ThrowIfFault(Message message, EndpointAddress target)
        {
            SecurityUtils.ThrowIfNegotiationFault(message, target);
        }

        protected void ValidateKeySize(GenericXmlSecurityToken issuedToken)
        {
            CommunicationObject.ThrowIfClosedOrNotOpen();
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SRP.CannotObtainIssuedTokenKeySize));
            }
        }

        internal class RequestChannelFactory : ChannelFactoryBase<IAsyncRequestChannel>, IChannelFactory<IRequestChannel>
        {
            private ServiceChannelFactory _serviceChannelFactory;

            public RequestChannelFactory(ServiceChannelFactory serviceChannelFactory)
            {
                _serviceChannelFactory = serviceChannelFactory;
            }

            protected override IAsyncRequestChannel OnCreateChannel(EndpointAddress address, Uri via)
            {
                return _serviceChannelFactory.CreateChannel<IAsyncRequestChannel>(address, via);
            }

            protected internal override Task OnOpenAsync(TimeSpan timeout)
            {
                return _serviceChannelFactory.OpenHelperAsync(timeout);
            }

            protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return _serviceChannelFactory.BeginOpen(timeout, callback, state);
            }

            protected override void OnEndOpen(IAsyncResult result)
            {
                _serviceChannelFactory.EndOpen(result);
            }

            protected internal override async Task OnCloseAsync(TimeSpan timeout)
            {
                await base.OnCloseAsync(timeout);
                await _serviceChannelFactory.CloseHelperAsync(timeout);
            }

            protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return OnCloseAsync(timeout).ToApm(callback, state);
            }

            protected override void OnEndClose(IAsyncResult result)
            {
                result.ToApmEnd();
            }

            protected override void OnClose(TimeSpan timeout)
            {
                base.OnClose(timeout);
                _serviceChannelFactory.Close(timeout);
            }

            protected override void OnOpen(TimeSpan timeout)
            {
                _serviceChannelFactory.Open(timeout);
            }

            protected override void OnAbort()
            {
                _serviceChannelFactory.Abort();
                base.OnAbort();
            }

            public override T GetProperty<T>()
            {
                return _serviceChannelFactory.GetProperty<T>();
            }

            IRequestChannel IChannelFactory<IRequestChannel>.CreateChannel(EndpointAddress to)
            {
                return CreateChannel(to);
            }

            IRequestChannel IChannelFactory<IRequestChannel>.CreateChannel(EndpointAddress to, Uri via)
            {
                return CreateChannel(to, via);
            }
        }
    }
}
