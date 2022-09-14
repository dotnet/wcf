// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.Threading.Tasks;

namespace System.ServiceModel.Security
{
    internal class InitiatorSessionSymmetricTransportSecurityProtocol : TransportSecurityProtocol, IInitiatorSecuritySessionProtocol
    {
        private SecurityToken _outgoingSessionToken;
        private List<SecurityToken> _incomingSessionTokens;
        private bool _requireDerivedKeys;

        public InitiatorSessionSymmetricTransportSecurityProtocol(SessionSymmetricTransportSecurityProtocolFactory factory, EndpointAddress target, Uri via) : base(factory, target, via)
        {
            if (factory.ActAsInitiator != true)
            {
                Fx.Assert("This protocol can only be used at the initiator.");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.ProtocolMustBeInitiator, nameof(InitiatorSessionSymmetricTransportSecurityProtocol))));
            }

            _requireDerivedKeys = factory.SecurityTokenParameters.RequireDerivedKeys;
        }

        private SessionSymmetricTransportSecurityProtocolFactory Factory
        {
            get { return (SessionSymmetricTransportSecurityProtocolFactory)SecurityProtocolFactory; }
        }

        private object ThisLock { get; } = new object();

        public bool ReturnCorrelationState
        {
            get => false;
            set { }
        }

        public SecurityToken GetOutgoingSessionToken()
        {
            lock (ThisLock)
            {
                return _outgoingSessionToken;
            }
        }

        public void SetIdentityCheckAuthenticator(SecurityTokenAuthenticator authenticator)
        {
        }

        public void SetOutgoingSessionToken(SecurityToken token)
        {
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(token));
            }
            lock (ThisLock)
            {
                _outgoingSessionToken = token;
                if (_requireDerivedKeys)
                {
                    throw ExceptionHelper.PlatformNotSupported();
                }
            }
        }

        public List<SecurityToken> GetIncomingSessionTokens()
        {
            lock (ThisLock)
            {
                return _incomingSessionTokens;
            }
        }

        public void SetIncomingSessionTokens(List<SecurityToken> tokens)
        {
            if (tokens == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokens));
            }
            lock (ThisLock)
            {
                _incomingSessionTokens = new List<SecurityToken>(tokens);
            }
        }

        private void GetTokensForOutgoingMessages(out SecurityToken signingToken, out SecurityToken sourceToken, out SecurityTokenParameters tokenParameters)
        {
            lock (ThisLock)
            {
                if (_requireDerivedKeys)
                {
                    throw ExceptionHelper.PlatformNotSupported();
                }
                else
                {
                    signingToken = _outgoingSessionToken;
                    sourceToken = null;
                }
            }
            tokenParameters = Factory.GetTokenParameters();
        }

        internal void SetupDelayedSecurityExecution(string actor, ref Message message, SecurityToken signingToken, SecurityToken sourceToken, SecurityTokenParameters tokenParameters,
            IList<SupportingTokenSpecification> supportingTokens)
        {
            SendSecurityHeader securityHeader = CreateSendSecurityHeaderForTransportProtocol(message, actor, Factory);
            securityHeader.RequireMessageProtection = false;
            if (sourceToken != null)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            AddSupportingTokens(securityHeader, supportingTokens);
            securityHeader.AddEndorsingSupportingToken(signingToken, tokenParameters);
            message = securityHeader.SetupExecution();
        }

        protected override async Task<Message> SecureOutgoingMessageAtInitiatorAsync(Message message, string actor, TimeSpan timeout)
        {
            SecurityToken signingToken;
            SecurityToken sourceToken;
            SecurityTokenParameters tokenParameters;
            GetTokensForOutgoingMessages(out signingToken, out sourceToken, out tokenParameters);
            IList<SupportingTokenSpecification> supportingTokens;
            supportingTokens = await TryGetSupportingTokensAsync(SecurityProtocolFactory, Target, Via, message, timeout);
            SetupDelayedSecurityExecution(actor, ref message, signingToken, sourceToken, tokenParameters, supportingTokens);
            return message;
        }
    }
}
