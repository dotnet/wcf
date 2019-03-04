// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Security
{
    internal class AcceleratedTokenProvider :
        NegotiationTokenProvider<AcceleratedTokenProviderState>
    {
        internal const SecurityKeyEntropyMode defaultKeyEntropyMode = SecurityKeyEntropyMode.CombinedEntropy;
        private SecurityKeyEntropyMode _keyEntropyMode = defaultKeyEntropyMode;
        private SecurityBindingElement _bootstrapSecurityBindingElement;
        private ChannelParameterCollection _channelParameters;

        public AcceleratedTokenProvider()
            : base()
        {
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

        public ChannelParameterCollection ChannelParameters
        {
            get { return _channelParameters; }
            set
            {
                this.CommunicationObject.ThrowIfDisposedOrImmutable();
                _channelParameters = value;
            }
        }

        // SC/Trust workshop change to turn off context
        protected override bool IsMultiLegNegotiation
        {
            get { return false; }
        }

        public override XmlDictionaryString RequestSecurityTokenAction
        {
            get
            {
                return this.StandardsManager.SecureConversationDriver.IssueAction;
            }
        }

        public override XmlDictionaryString RequestSecurityTokenResponseAction
        {
            get
            {
                return this.StandardsManager.SecureConversationDriver.IssueResponseAction;
            }
        }

        public override Task OnOpenAsync(TimeSpan timeout)
        {
            if (this.BootstrapSecurityBindingElement == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.BootstrapSecurityBindingElementNotSet, this.GetType())));
            }

            return base.OnOpenAsync(timeout);
        }

        public override void OnOpening()
        {
            base.OnOpening();
            if (this.BootstrapSecurityBindingElement == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.BootstrapSecurityBindingElementNotSet, this.GetType())));
            }
        }

        public override Task OnCloseAsync(TimeSpan timeout)
        {
            return base.OnCloseAsync(timeout);
        }

        public override void OnAbort()
        {
            base.OnAbort();
        }

        protected override IChannelFactory<IAsyncRequestChannel> GetNegotiationChannelFactory(IChannelFactory<IAsyncRequestChannel> transportChannelFactory, ChannelBuilder channelBuilder)
        {
            ISecurityCapabilities securityCapabilities = _bootstrapSecurityBindingElement.GetProperty<ISecurityCapabilities>(this.IssuerBindingContext);
            SecurityCredentialsManager securityCredentials = this.IssuerBindingContext.BindingParameters.Find<SecurityCredentialsManager>();
            if (securityCredentials == null)
            {
                securityCredentials = ClientCredentials.CreateDefaultCredentials();
            }

            _bootstrapSecurityBindingElement.ReaderQuotas = this.IssuerBindingContext.GetInnerProperty<XmlDictionaryReaderQuotas>();
            if (_bootstrapSecurityBindingElement.ReaderQuotas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.EncodingBindingElementDoesNotHandleReaderQuotas));
            }

            TransportBindingElement transportBindingElement = this.IssuerBindingContext.RemainingBindingElements.Find<TransportBindingElement>();
            if (transportBindingElement != null)
            {
                _bootstrapSecurityBindingElement.MaxReceivedMessageSize = transportBindingElement.MaxReceivedMessageSize;
            }

            SecurityProtocolFactory securityProtocolFactory = _bootstrapSecurityBindingElement.CreateSecurityProtocolFactory<IAsyncRequestChannel>(this.IssuerBindingContext.Clone(), securityCredentials, false, this.IssuerBindingContext.Clone());
            return new SecurityChannelFactory<IAsyncRequestChannel>(
                securityCapabilities, this.IssuerBindingContext, channelBuilder, securityProtocolFactory, transportChannelFactory);
        }

        protected override IAsyncRequestChannel CreateClientChannel(EndpointAddress target, Uri via)
        {
            IAsyncRequestChannel result = base.CreateClientChannel(target, via);
            if (_channelParameters != null)
            {
                _channelParameters.PropagateChannelParameters(result);
            }

            return result;
        }

        protected override Task<AcceleratedTokenProviderState> CreateNegotiationStateAsync(EndpointAddress target, Uri via, TimeSpan timeout)
        {
            byte[] keyEntropy;
            if (_keyEntropyMode == SecurityKeyEntropyMode.ClientEntropy || _keyEntropyMode == SecurityKeyEntropyMode.CombinedEntropy)
            {
                keyEntropy = new byte[this.SecurityAlgorithmSuite.DefaultSymmetricKeyLength / 8];
                IdentityModel.CryptoHelper.FillRandomBytes(keyEntropy);
            }
            else
            {
                keyEntropy = null;
            }

            return Task.FromResult(new AcceleratedTokenProviderState(keyEntropy));
        }

        protected override BodyWriter GetFirstOutgoingMessageBody(AcceleratedTokenProviderState negotiationState, out MessageProperties messageProperties)
        {
            messageProperties = null;
            RequestSecurityToken rst = new RequestSecurityToken(this.StandardsManager);
            rst.Context = negotiationState.Context;
            rst.KeySize = this.SecurityAlgorithmSuite.DefaultSymmetricKeyLength;
            rst.TokenType = this.SecurityContextTokenUri;
            byte[] requestorEntropy = negotiationState.GetRequestorEntropy();
            if (requestorEntropy != null)
            {
                rst.SetRequestorEntropy(requestorEntropy);
            }
            rst.MakeReadOnly();
            return rst;
        }

        protected override BodyWriter GetNextOutgoingMessageBody(Message incomingMessage, AcceleratedTokenProviderState negotiationState)
        {
            ThrowIfFault(incomingMessage, this.TargetAddress);
            if (incomingMessage.Headers.Action != RequestSecurityTokenResponseAction.Value)
            {
                throw TraceUtility.ThrowHelperError(new SecurityNegotiationException(SR.Format(SR.InvalidActionForNegotiationMessage, incomingMessage.Headers.Action)), incomingMessage);
            }
            // get the claims corresponding to the server
            SecurityMessageProperty serverContextProperty = incomingMessage.Properties.Security;
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
            XmlDictionaryReader bodyReader = incomingMessage.GetReaderAtBodyContents();
            using (bodyReader)
            {
                if (this.StandardsManager.MessageSecurityVersion.TrustVersion == TrustVersion.WSTrustFeb2005)
                    rstr = RequestSecurityTokenResponse.CreateFrom(this.StandardsManager, bodyReader);
                else if (this.StandardsManager.MessageSecurityVersion.TrustVersion == TrustVersion.WSTrust13)
                {
                    RequestSecurityTokenResponseCollection rstrc = this.StandardsManager.TrustDriver.CreateRequestSecurityTokenResponseCollection(bodyReader);

                    foreach (RequestSecurityTokenResponse rstrItem in rstrc.RstrCollection)
                    {
                        if (rstr != null)
                        {
                            // More than one RSTR is found. So throw an exception.
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SR.MoreThanOneRSTRInRSTRC));
                        }
                        rstr = rstrItem;
                    }
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
                }

                incomingMessage.ReadFromBodyContentsToEnd(bodyReader);
            }
            if (rstr.Context != negotiationState.Context)
            {
                throw TraceUtility.ThrowHelperError(new SecurityNegotiationException(SR.BadSecurityNegotiationContext), incomingMessage);
            }
            byte[] keyEntropy = negotiationState.GetRequestorEntropy();
            GenericXmlSecurityToken serviceToken = rstr.GetIssuedToken(null, null, _keyEntropyMode, keyEntropy, this.SecurityContextTokenUri, authorizationPolicies, this.SecurityAlgorithmSuite.DefaultSymmetricKeyLength, false);
            negotiationState.SetServiceToken(serviceToken);
            return null;
        }
    }
}
