// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IdentityModel.Policy;
    using System.IdentityModel.Tokens;
    using System.Runtime;
    using System.Runtime.Diagnostics;
    using System.Security.Authentication.ExtendedProtection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Security.Tokens;
    using System.Net;
    using System.Xml;
    using System.Threading.Tasks;

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
        private MessageVersion _messageVersion;
        private EndpointAddress _localAddress;
        private ChannelParameterCollection _channelParameters;
        private WebHeaderCollection _webHeaderCollection;

        public SecuritySessionSecurityTokenProvider()
            : base()
        {
            _standardsManager = SecurityStandardsManager.DefaultInstance;
            _keyEntropyMode = SecurityKeyEntropyMode.CombinedEntropy;
        }

        public WebHeaderCollection WebHeaders
        {
            get
            {
                return _webHeaderCollection;
            }
            set
            {
                this.CommunicationObject.ThrowIfDisposedOrImmutable();
                _webHeaderCollection = value;
            }
        }

        public SecurityAlgorithmSuite SecurityAlgorithmSuite
        {
            get
            {
                return _securityAlgorithmSuite;
            }
            set
            {
                this.CommunicationObject.ThrowIfDisposedOrImmutable();
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
                this.CommunicationObject.ThrowIfDisposedOrImmutable();
                SecurityKeyEntropyModeHelper.Validate(value);
                _keyEntropyMode = value;
            }
        }

        private MessageVersion MessageVersion
        {
            get
            {
                return _messageVersion;
            }
        }

        public EndpointAddress TargetAddress
        {
            get { return _targetAddress; }
            set
            {
                this.CommunicationObject.ThrowIfDisposedOrImmutable();
                _targetAddress = value;
            }
        }

        public EndpointAddress LocalAddress
        {
            get { return _localAddress; }
            set
            {
                this.CommunicationObject.ThrowIfDisposedOrImmutable();
                _localAddress = value;
            }
        }

        public Uri Via
        {
            get { return _via; }
            set
            {
                this.CommunicationObject.ThrowIfDisposedOrImmutable();
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
                this.CommunicationObject.ThrowIfDisposedOrImmutable();
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                _issuerBindingContext = value.Clone();
            }
        }

        public SecurityBindingElement BootstrapSecurityBindingElement
        {
            get { return _bootstrapSecurityBindingElement; }
            set
            {
                this.CommunicationObject.ThrowIfDisposedOrImmutable();
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
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
                this.CommunicationObject.ThrowIfDisposedOrImmutable();
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(value)));
                }
                if (!value.TrustDriver.IsSessionSupported)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.TrustDriverVersionDoesNotSupportSession, nameof(value)));
                }
                if (!value.SecureConversationDriver.IsSessionSupported)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.SecureConversationDriverVersionDoesNotSupportSession, nameof(value)));
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
                this.CommunicationObject.ThrowIfDisposedOrImmutable();
                _issuedTokenParameters = value;
            }
        }

        public Uri PrivacyNoticeUri
        {
            get { return _privacyNoticeUri; }
            set
            {
                this.CommunicationObject.ThrowIfDisposedOrImmutable();
                _privacyNoticeUri = value;
            }
        }

        public ChannelParameterCollection ChannelParameters
        {
            get { return _channelParameters; }
            set
            {
                this.CommunicationObject.ThrowIfDisposedOrImmutable();
                _channelParameters = value;
            }
        }

        public int PrivacyNoticeVersion
        {
            get { return _privacyNoticeVersion; }
            set
            {
                this.CommunicationObject.ThrowIfDisposedOrImmutable();
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.TargetAddressIsNotSet, this.GetType())));
            }
            if (this.IssuerBindingContext == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.IssuerBuildContextNotSet, this.GetType())));
            }
            if (this.IssuedSecurityTokenParameters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.IssuedSecurityTokenParametersNotSet, this.GetType())));
            }
            if (this.BootstrapSecurityBindingElement == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.BootstrapSecurityBindingElementNotSet, this.GetType())));
            }
            if (this.SecurityAlgorithmSuite == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SecurityAlgorithmSuiteNotSet, this.GetType())));
            }
            InitializeFactories();
            await _rstChannelFactory.OpenHelperAsync(timeoutHelper.RemainingTime());
            _sctUri = this.StandardsManager.SecureConversationDriver.TokenTypeUri;
        }

        public override void OnOpening()
        {
            base.OnOpening();
            if (this.IssuerBindingContext == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.IssuerBuildContextNotSet, this.GetType())));
            }
            if (this.BootstrapSecurityBindingElement == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.BootstrapSecurityBindingElementNotSet, this.GetType())));
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
            ISecurityCapabilities securityCapabilities = this.BootstrapSecurityBindingElement.GetProperty<ISecurityCapabilities>(this.IssuerBindingContext);
            SecurityCredentialsManager securityCredentials = this.IssuerBindingContext.BindingParameters.Find<SecurityCredentialsManager>();
            if (securityCredentials == null)
            {
                securityCredentials = ClientCredentials.CreateDefaultCredentials();
            }

            BindingContext context = this.IssuerBindingContext;
            _bootstrapSecurityBindingElement.ReaderQuotas = context.GetInnerProperty<XmlDictionaryReaderQuotas>();
            if (_bootstrapSecurityBindingElement.ReaderQuotas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.EncodingBindingElementDoesNotHandleReaderQuotas));
            }

            TransportBindingElement transportBindingElement = context.RemainingBindingElements.Find<TransportBindingElement>();
            if (transportBindingElement != null)
            {
                _bootstrapSecurityBindingElement.MaxReceivedMessageSize = transportBindingElement.MaxReceivedMessageSize;
            }

            SecurityProtocolFactory securityProtocolFactory = this.BootstrapSecurityBindingElement.CreateSecurityProtocolFactory<IRequestChannel>(this.IssuerBindingContext.Clone(), securityCredentials, false, this.IssuerBindingContext.Clone());

            if (_localAddress != null)
            {
                MessageFilter issueAndRenewFilter = new SessionActionFilter(_standardsManager, this.IssueResponseAction.Value, this.RenewResponseAction.Value);
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
                securityCapabilities, this.IssuerBindingContext, channelBuilder, securityProtocolFactory, innerChannelFactory);

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
            _messageVersion = securityChannelFactory.MessageVersion;
        }

        // token provider methods
        protected override Task<SecurityToken> GetTokenCoreAsync(TimeSpan timeout)
        {
            this.CommunicationObject.ThrowIfClosedOrNotOpen();
            return this.DoOperationAsync(SecuritySessionOperation.Issue, _targetAddress, _via, null, timeout);
        }

        protected override Task<SecurityToken> RenewTokenCoreAsync(TimeSpan timeout, SecurityToken tokenToBeRenewed)
        {
            this.CommunicationObject.ThrowIfClosedOrNotOpen();
            return this.DoOperationAsync(SecuritySessionOperation.Renew, _targetAddress, _via, tokenToBeRenewed, timeout);
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
                return this.CreateIssueRequest(target, out requestState);
            }
            else if (operation == SecuritySessionOperation.Renew)
            {
                return this.CreateRenewRequest(target, currentToken, out requestState);
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
                issuedToken = this.ProcessIssueResponse(reply, requestState);
            }
            else if (operation == SecuritySessionOperation.Renew)
            {
                issuedToken = this.ProcessRenewResponse(reply, requestState);
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("target");
            }

            if (operation == SecuritySessionOperation.Renew && currentToken == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("currentToken");
            }

            IAsyncRequestChannel channel = null;
            try
            {
                channel = this.CreateChannel(operation, target, via);

                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                await channel.OpenAsync(timeoutHelper.RemainingTime());
                object requestState;
                GenericXmlSecurityToken issuedToken;

                using (Message requestMessage = this.CreateRequest(operation, target, currentToken, out requestState))
                {
                    EventTraceActivity eventTraceActivity = null;

                    TraceUtility.ProcessOutgoingMessage(requestMessage, eventTraceActivity);

                    using (Message reply = await channel.RequestAsync(requestMessage, timeoutHelper.RemainingTime()))
                    {
                        if (reply == null)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(SR.FailToRecieveReplyFromNegotiation));
                        }

                        TraceUtility.ProcessIncomingMessage(reply, eventTraceActivity);
                        ThrowIfFault(reply, _targetAddress);
                        issuedToken = ProcessReply(reply, operation, requestState);
                        ValidateKeySize(issuedToken);
                    }
                }
                await channel.CloseAsync(timeoutHelper.RemainingTime());
                this.OnOperationSuccess(operation, target, issuedToken, currentToken);
                return issuedToken;
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                    throw;

                if (e is TimeoutException)
                {
                    e = new TimeoutException(SR.Format(SR.ClientSecuritySessionRequestTimeout, timeout), e);
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
            rst.KeySize = this.SecurityAlgorithmSuite.DefaultSymmetricKeyLength;
            rst.TokenType = _sctUri;
            if (this.KeyEntropyMode == SecurityKeyEntropyMode.ClientEntropy || this.KeyEntropyMode == SecurityKeyEntropyMode.CombinedEntropy)
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
                    message.Headers.ReplyTo = this.LocalAddress;
                }
                else
                {
                    message.Headers.ReplyTo = EndpointAddress.AnonymousAddress;
                }
            }

            if (_webHeaderCollection != null && _webHeaderCollection.Count > 0)
            {
                object prop = null;
                HttpRequestMessageProperty rmp = null;
                if (message.Properties.TryGetValue(HttpRequestMessageProperty.Name, out prop))
                {
                    rmp = prop as HttpRequestMessageProperty;
                }
                else
                {
                    rmp = new HttpRequestMessageProperty();
                    message.Properties.Add(HttpRequestMessageProperty.Name, rmp);
                }

                if (rmp != null && rmp.Headers != null)
                {
                    rmp.Headers.Add(_webHeaderCollection);
                }
            }
        }

        protected virtual Message CreateIssueRequest(EndpointAddress target, out object requestState)
        {
            this.CommunicationObject.ThrowIfClosedOrNotOpen();
            RequestSecurityToken rst = CreateRst(target, out requestState);
            rst.RequestType = this.StandardsManager.TrustDriver.RequestTypeIssue;
            rst.MakeReadOnly();
            Message result = Message.CreateMessage(this.MessageVersion, ActionHeader.Create(this.IssueAction, this.MessageVersion.Addressing), rst);
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
                if (this.StandardsManager.MessageSecurityVersion.TrustVersion == TrustVersion.WSTrustFeb2005)
                    rstr = this.StandardsManager.TrustDriver.CreateRequestSecurityTokenResponse(bodyReader);
                else if (this.StandardsManager.MessageSecurityVersion.TrustVersion == TrustVersion.WSTrust13)
                {
                    RequestSecurityTokenResponseCollection rstrc = this.StandardsManager.TrustDriver.CreateRequestSecurityTokenResponseCollection(bodyReader);
                    foreach (RequestSecurityTokenResponse rstrItem in rstrc.RstrCollection)
                    {
                        if (rstr != null)
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SR.MoreThanOneRSTRInRSTRC));

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

            GenericXmlSecurityToken issuedToken = rstr.GetIssuedToken(null, null, this.KeyEntropyMode, requestorEntropy, _sctUri, authorizationPolicies, this.SecurityAlgorithmSuite.DefaultSymmetricKeyLength, false);
            return issuedToken;
        }

        protected virtual GenericXmlSecurityToken ProcessIssueResponse(Message response, object requestState)
        {
            this.CommunicationObject.ThrowIfClosedOrNotOpen();
            return ExtractToken(response, requestState);
        }

        protected virtual Message CreateRenewRequest(EndpointAddress target, SecurityToken currentSessionToken, out object requestState)
        {
            this.CommunicationObject.ThrowIfClosedOrNotOpen();
            RequestSecurityToken rst = CreateRst(target, out requestState);
            rst.RequestType = this.StandardsManager.TrustDriver.RequestTypeRenew;
            rst.RenewTarget = this.IssuedSecurityTokenParameters.CreateKeyIdentifierClause(currentSessionToken, SecurityTokenReferenceStyle.External);
            rst.MakeReadOnly();
            Message result = Message.CreateMessage(this.MessageVersion, ActionHeader.Create(this.RenewAction, this.MessageVersion.Addressing), rst);
            SecurityMessageProperty supportingTokenProperty = new SecurityMessageProperty();
            supportingTokenProperty.OutgoingSupportingTokens.Add(new SupportingTokenSpecification(currentSessionToken, EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance, SecurityTokenAttachmentMode.Endorsing, this.IssuedSecurityTokenParameters));
            result.Properties.Security = supportingTokenProperty;
            PrepareRequest(result);
            return result;
        }

        protected virtual GenericXmlSecurityToken ProcessRenewResponse(Message response, object requestState)
        {
            this.CommunicationObject.ThrowIfClosedOrNotOpen();
            if (response.Headers.Action != this.RenewResponseAction.Value)
            {
                throw TraceUtility.ThrowHelperError(new SecurityNegotiationException(SR.Format(SR.InvalidRenewResponseAction, response.Headers.Action)), response);
            }

            return ExtractToken(response, requestState);
        }

        static protected void ThrowIfFault(Message message, EndpointAddress target)
        {
            SecurityUtils.ThrowIfNegotiationFault(message, target);
        }

        protected void ValidateKeySize(GenericXmlSecurityToken issuedToken)
        {
            this.CommunicationObject.ThrowIfClosedOrNotOpen();
            ReadOnlyCollection<SecurityKey> issuedKeys = issuedToken.SecurityKeys;
            if (issuedKeys != null && issuedKeys.Count == 1)
            {
                SymmetricSecurityKey symmetricKey = issuedKeys[0] as SymmetricSecurityKey;
                if (symmetricKey != null)
                {
                    if (this.SecurityAlgorithmSuite.IsSymmetricKeyLengthSupported(symmetricKey.KeySize))
                    {
                        return;
                    }
                    else
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SR.Format(SR.InvalidIssuedTokenKeySize, symmetricKey.KeySize)));
                    }
                }
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SR.CannotObtainIssuedTokenKeySize));
            }
        }

        internal class RequestChannelFactory : ChannelFactoryBase<IAsyncRequestChannel>
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

            protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return _serviceChannelFactory.BeginOpen(timeout, callback, state);
            }

            protected override void OnEndOpen(IAsyncResult result)
            {
                _serviceChannelFactory.EndOpen(result);
            }

            protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return new ChainedCloseAsyncResult(timeout, callback, state, base.OnBeginClose, base.OnEndClose, _serviceChannelFactory);
            }

            protected override void OnEndClose(IAsyncResult result)
            {
                ChainedCloseAsyncResult.End(result);
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
        }
    }
}
