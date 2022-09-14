// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Threading.Tasks;

namespace System.ServiceModel.Security
{
    internal class TransportSecurityProtocol : SecurityProtocol
    {
        public TransportSecurityProtocol(TransportSecurityProtocolFactory factory, EndpointAddress target, Uri via)
            : base(factory, target, via)
        {
        }

        public override async Task<Message> SecureOutgoingMessageAsync(Message message, TimeSpan timeout)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));
            }
            CommunicationObject.ThrowIfClosedOrNotOpen();
            string actor = string.Empty; // message.Version.Envelope.UltimateDestinationActor;
            try
            {
                if (SecurityProtocolFactory.ActAsInitiator)
                {
                    message = await SecureOutgoingMessageAtInitiatorAsync(message, actor, timeout);
                }
                else
                {
                    throw ExceptionHelper.PlatformNotSupported();
                }
                base.OnOutgoingMessageSecured(message);
            }
            catch
            {
                base.OnSecureOutgoingMessageFailure(message);
                throw;
            }

            return message;
        }

        protected virtual async Task<Message> SecureOutgoingMessageAtInitiatorAsync(Message message, string actor, TimeSpan timeout)
        {
            IList<SupportingTokenSpecification> supportingTokens = await TryGetSupportingTokensAsync(SecurityProtocolFactory, Target, Via, message, timeout);
            SetUpDelayedSecurityExecution(ref message, actor, supportingTokens);
            return message;
        }

        internal void SetUpDelayedSecurityExecution(ref Message message, string actor,
            IList<SupportingTokenSpecification> supportingTokens)
        {
            SendSecurityHeader securityHeader = CreateSendSecurityHeaderForTransportProtocol(message, actor, SecurityProtocolFactory);
            AddSupportingTokens(securityHeader, supportingTokens);
            message = securityHeader.SetupExecution();
        }

        public sealed override void VerifyIncomingMessage(ref Message message, TimeSpan timeout)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));
            }
            CommunicationObject.ThrowIfClosedOrNotOpen();
            try
            {
                VerifyIncomingMessageCore(ref message, timeout);
            }
            catch (MessageSecurityException e)
            {
                base.OnVerifyIncomingMessageFailure(message, e);
                throw;
            }
            catch (Exception e)
            {
                // Always immediately rethrow fatal exceptions.
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                base.OnVerifyIncomingMessageFailure(message, e);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.MessageSecurityVerificationFailed, e));
            }
        }

        protected void AttachRecipientSecurityProperty(Message message, IList<SecurityToken> basicTokens, IList<SecurityToken> endorsingTokens,
           IList<SecurityToken> signedEndorsingTokens, IList<SecurityToken> signedTokens, Dictionary<SecurityToken, ReadOnlyCollection<IAuthorizationPolicy>> tokenPoliciesMapping)
        {
            SecurityMessageProperty security = SecurityMessageProperty.GetOrCreate(message);
            AddSupportingTokenSpecification(security, basicTokens, endorsingTokens, signedEndorsingTokens, signedTokens, tokenPoliciesMapping);
            security.ServiceSecurityContext = new ServiceSecurityContext(security.GetInitiatorTokenAuthorizationPolicies());
        }

        protected virtual void VerifyIncomingMessageCore(ref Message message, TimeSpan timeout)
        {
            TransportSecurityProtocolFactory factory = (TransportSecurityProtocolFactory)SecurityProtocolFactory;
            string actor = string.Empty; // message.Version.Envelope.UltimateDestinationActor;

            ReceiveSecurityHeader securityHeader = factory.StandardsManager.TryCreateReceiveSecurityHeader(message, actor,
                factory.IncomingAlgorithmSuite, (factory.ActAsInitiator) ? MessageDirection.Output : MessageDirection.Input);
            bool expectBasicTokens;
            bool expectEndorsingTokens;
            bool expectSignedTokens;
            IList<SupportingTokenAuthenticatorSpecification> supportingAuthenticators = factory.GetSupportingTokenAuthenticators(message.Headers.Action,
                out expectSignedTokens, out expectBasicTokens, out expectEndorsingTokens);
            if (securityHeader == null)
            {
                bool expectSupportingTokens = expectEndorsingTokens || expectSignedTokens || expectBasicTokens;
                if ((factory.ActAsInitiator && (!factory.AddTimestamp || factory.SecurityBindingElement.EnableUnsecuredResponse))
                    || (!factory.ActAsInitiator && !factory.AddTimestamp && !expectSupportingTokens))
                {
                    return;
                }
                else
                {
                    if (String.IsNullOrEmpty(actor))
                    {
                        throw System.ServiceModel.Diagnostics.TraceUtility.ThrowHelperError(new MessageSecurityException(
                            SRP.UnableToFindSecurityHeaderInMessageNoActor), message);
                    }
                    else
                    {
                        throw System.ServiceModel.Diagnostics.TraceUtility.ThrowHelperError(new MessageSecurityException(
                            SRP.Format(SRP.UnableToFindSecurityHeaderInMessage, actor)), message);
                    }
                }
            }

            securityHeader.RequireMessageProtection = false;
            securityHeader.ExpectBasicTokens = expectBasicTokens;
            securityHeader.ExpectSignedTokens = expectSignedTokens;
            securityHeader.ExpectEndorsingTokens = expectEndorsingTokens;
            securityHeader.MaxReceivedMessageSize = factory.SecurityBindingElement.MaxReceivedMessageSize;
            securityHeader.ReaderQuotas = factory.SecurityBindingElement.ReaderQuotas;

            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            if (!factory.ActAsInitiator)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
            securityHeader.ReplayDetectionEnabled = factory.DetectReplays;
            securityHeader.SetTimeParameters(factory.NonceCache, factory.ReplayWindow, factory.MaxClockSkew);
            securityHeader.Process(timeoutHelper.RemainingTime(), SecurityUtils.GetChannelBindingFromMessage(message), factory.ExtendedProtectionPolicy);
            message = securityHeader.ProcessedMessage;
            if (!factory.ActAsInitiator)
            {
                AttachRecipientSecurityProperty(message, securityHeader.BasicSupportingTokens, securityHeader.EndorsingSupportingTokens, securityHeader.SignedEndorsingSupportingTokens,
                    securityHeader.SignedSupportingTokens, securityHeader.SecurityTokenAuthorizationPoliciesMapping);
            }

            base.OnIncomingMessageVerified(message);
        }
    }
}
