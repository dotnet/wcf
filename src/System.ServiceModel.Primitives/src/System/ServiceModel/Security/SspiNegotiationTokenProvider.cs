// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net;
using System.Runtime;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Security
{
    internal abstract class SspiNegotiationTokenProvider : NegotiationTokenProvider<SspiNegotiationTokenProviderState>
    {
        private bool _negotiateTokenOnOpen;
        private SecurityBindingElement _securityBindingElement;

        protected SspiNegotiationTokenProvider()
            : base()
        {
        }
        protected SspiNegotiationTokenProvider(SecurityBindingElement securityBindingElement) : base()
        {
            _securityBindingElement = securityBindingElement;
        }


        public bool NegotiateTokenOnOpen
        {
            get
            {
                return _negotiateTokenOnOpen;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _negotiateTokenOnOpen = value;
            }
        }

        // SspiNegotiationTokenProvider abstract methods
        protected abstract ReadOnlyCollection<IAuthorizationPolicy> ValidateSspiNegotiation(ISspiNegotiation sspiNegotiation);
        public abstract XmlDictionaryString NegotiationValueType { get; }

        public override async Task OnOpenAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            EnsureEndpointAddressDoesNotRequireEncryption(TargetAddress);
            await base.OnOpenAsync(timeoutHelper.RemainingTime());
            if (_negotiateTokenOnOpen)
            {
                await DoNegotiationAsync(timeoutHelper.RemainingTime());
            }
        }

        protected override IChannelFactory<IRequestChannel> GetNegotiationChannelFactory(IChannelFactory<IRequestChannel> transportChannelFactory, ChannelBuilder channelBuilder)
        {
            return transportChannelFactory;
        }

        // helper methods
        private void ValidateIncomingBinaryNegotiation(BinaryNegotiation incomingNego)
        {
            incomingNego.Validate(NegotiationValueType);
        }

        private static void AddToDigest(IncrementalHash negotiationDigest, Stream stream)
        {
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            CanonicalizationDriver canonicalizer = new CanonicalizationDriver();
            canonicalizer.SetInput(stream);
            byte[] canonicalizedData = canonicalizer.GetBytes();
            lock (negotiationDigest)
            {
                negotiationDigest.AppendData(canonicalizedData);
            }
        }

        internal static void AddToDigest(SspiNegotiationTokenProviderState sspiState, RequestSecurityToken rst)
        {
            MemoryStream stream = new MemoryStream();
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(stream);
            rst.WriteTo(writer);
            writer.Flush();
            AddToDigest(sspiState.NegotiationDigest, stream);
        }

        private void AddToDigest(SspiNegotiationTokenProviderState sspiState, RequestSecurityTokenResponse rstr, bool wasReceived, bool isFinalRstr)
        {
            MemoryStream stream = new MemoryStream();
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(stream);
            if (!wasReceived)
            {
                rstr.WriteTo(writer);
            }
            else
            {
                if (!isFinalRstr)
                {
                    rstr.RequestSecurityTokenResponseXml.WriteTo(writer);
                }
                else
                {
                    XmlElement rstrClone = (XmlElement)rstr.RequestSecurityTokenResponseXml.CloneNode(true);
                    List<XmlNode> nodesToRemove = new List<XmlNode>(2);
                    for (int i = 0; i < rstrClone.ChildNodes.Count; ++i)
                    {
                        XmlNode child = rstrClone.ChildNodes[i];
                        if (StandardsManager.TrustDriver.IsRequestedSecurityTokenElement(child.LocalName, child.NamespaceURI))
                        {
                            nodesToRemove.Add(child);
                        }
                        else if (StandardsManager.TrustDriver.IsRequestedProofTokenElement(child.LocalName, child.NamespaceURI))
                        {
                            nodesToRemove.Add(child);
                        }
                    }
                    for (int i = 0; i < nodesToRemove.Count; ++i)
                    {
                        rstrClone.RemoveChild(nodesToRemove[i]);
                    }
                    rstrClone.WriteTo(writer);
                }
            }
            writer.Flush();
            AddToDigest(sspiState.NegotiationDigest, stream);
        }

        private static bool IsCorrectAuthenticator(SspiNegotiationTokenProviderState sspiState, byte[] proofKey, byte[] serverAuthenticator)
        {
            byte[] negotiationHash;
            lock (sspiState.NegotiationDigest)
            {
                negotiationHash = sspiState.NegotiationDigest.GetHashAndReset();
            }

            byte[] label = Encoding.UTF8.GetBytes(TrustApr2004Strings.CombinedHashLabel);
            Psha1DerivedKeyGenerator generator = new Psha1DerivedKeyGenerator(proofKey);
            byte[] clientAuthenticator = generator.GenerateDerivedKey(label, negotiationHash, SecurityNegotiationConstants.NegotiationAuthenticatorSize, 0);
            if (clientAuthenticator.Length != serverAuthenticator.Length)
            {
                return false;
            }
            for (int i = 0; i < clientAuthenticator.Length; ++i)
            {
                if (clientAuthenticator[i] != serverAuthenticator[i])
                {
                    return false;
                }
            }
            return true;
        }

        private BodyWriter PrepareRstr(SspiNegotiationTokenProviderState sspiState, byte[] outgoingBlob)
        {
            RequestSecurityTokenResponse rstr = new RequestSecurityTokenResponse(StandardsManager);
            rstr.Context = sspiState.Context;
            rstr.SetBinaryNegotiation(new BinaryNegotiation(NegotiationValueType, outgoingBlob));
            rstr.MakeReadOnly();
            AddToDigest(sspiState, rstr, false, false);
            return rstr;
        }

        protected override BodyWriter GetFirstOutgoingMessageBody(SspiNegotiationTokenProviderState sspiState, out MessageProperties messageProperties)
        {
            messageProperties = null;

            RequestSecurityToken rst = new RequestSecurityToken(StandardsManager, false);
            rst.Context = sspiState.Context;
            rst.TokenType = StandardsManager.SecureConversationDriver.TokenTypeUri;
            rst.KeySize = SecurityAlgorithmSuite.DefaultSymmetricKeyLength;

            // Delay GetOutgoingBlob()'s first call until channel binding is available.
            rst.OnGetBinaryNegotiation = (channelBinding) =>
            {
                byte[] outgoingBlob = sspiState.SspiNegotiation.GetOutgoingBlob(null, channelBinding, null);

                if (outgoingBlob == null && sspiState.SspiNegotiation.IsCompleted == false)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SRP.NoBinaryNegoToSend));
                }

                rst.SetBinaryNegotiation(new BinaryNegotiation(NegotiationValueType, outgoingBlob));
                AddToDigest(sspiState, rst);
                rst.MakeReadOnly();
            };

            return rst;
        }

        protected override BodyWriter GetNextOutgoingMessageBody(Message incomingMessage, SspiNegotiationTokenProviderState sspiState)
        {
            try
            {
                ThrowIfFault(incomingMessage, TargetAddress);
            }
            catch (FaultException fault)
            {
                if (fault.Code.IsSenderFault)
                {
                    if (fault.Code.SubCode.Name == TrustApr2004Strings.FailedAuthenticationFaultCode ||
                        fault.Code.SubCode.Name == TrustFeb2005Strings.FailedAuthenticationFaultCode)
                    {
                        throw TraceUtility.ThrowHelperError(new SecurityNegotiationException(SRP.AuthenticationOfClientFailed, fault), incomingMessage);
                    }

                    throw TraceUtility.ThrowHelperError(new SecurityNegotiationException(SRP.FailedSspiNegotiation, fault), incomingMessage);
                }
                else
                {
                    throw;
                }
            }

            RequestSecurityTokenResponse negotiationRstr = null;
            RequestSecurityTokenResponse authenticatorRstr = null;
            XmlDictionaryReader bodyReader = incomingMessage.GetReaderAtBodyContents();
            using (bodyReader)
            {
                if (StandardsManager.TrustDriver.IsAtRequestSecurityTokenResponseCollection(bodyReader))
                {
                    RequestSecurityTokenResponseCollection rstrCollection = StandardsManager.TrustDriver.CreateRequestSecurityTokenResponseCollection(bodyReader);
                    using (IEnumerator<RequestSecurityTokenResponse> enumerator = rstrCollection.RstrCollection.GetEnumerator())
                    {
                        enumerator.MoveNext();
                        negotiationRstr = enumerator.Current;
                        if (enumerator.MoveNext())
                        {
                            authenticatorRstr = enumerator.Current;
                        }
                    }
                    if (authenticatorRstr == null)
                    {
                        throw TraceUtility.ThrowHelperError(new SecurityNegotiationException(SRP.AuthenticatorNotPresentInRSTRCollection), incomingMessage);
                    }
                    else if (authenticatorRstr.Context != negotiationRstr.Context)
                    {
                        throw TraceUtility.ThrowHelperError(new SecurityNegotiationException(SRP.RSTRAuthenticatorHasBadContext), incomingMessage);
                    }
                    AddToDigest(sspiState, negotiationRstr, true, true);
                }
                else if (StandardsManager.TrustDriver.IsAtRequestSecurityTokenResponse(bodyReader))
                {
                    negotiationRstr = RequestSecurityTokenResponse.CreateFrom(StandardsManager, bodyReader);
                    AddToDigest(sspiState, negotiationRstr, true, false);
                }
                else
                {
                    throw TraceUtility.ThrowHelperError(new SecurityNegotiationException(SRP.Format(SRP.FailToReceiveReplyFromNegotiation)), incomingMessage);
                }
                incomingMessage.ReadFromBodyContentsToEnd(bodyReader);
            }

            if (negotiationRstr.Context != sspiState.Context)
            {
                throw TraceUtility.ThrowHelperError(new SecurityNegotiationException(SRP.BadSecurityNegotiationContext), incomingMessage);
            }

            BinaryNegotiation incomingBinaryNego = negotiationRstr.GetBinaryNegotiation();
            byte[] incomingBlob;
            if (incomingBinaryNego != null)
            {
                ValidateIncomingBinaryNegotiation(incomingBinaryNego);
                incomingBlob = incomingBinaryNego.GetNegotiationData();
            }
            else
            {
                incomingBlob = null;
            }

            BodyWriter nextMessageBody;
            if (incomingBlob == null && sspiState.SspiNegotiation.IsCompleted == false)
            {
                throw TraceUtility.ThrowHelperError(new SecurityNegotiationException(SRP.NoBinaryNegoToReceive), incomingMessage);
            }
            else if (incomingBlob == null && sspiState.SspiNegotiation.IsCompleted == true)
            {
                OnNegotiationComplete(sspiState, negotiationRstr, authenticatorRstr);
                nextMessageBody = null;
            }
            else
            {
                byte[] outgoingBlob = sspiState.SspiNegotiation.GetOutgoingBlob(
                    incomingBlob,
                    SecurityUtils.GetChannelBindingFromMessage(incomingMessage),
                    null);

                if (outgoingBlob == null && sspiState.SspiNegotiation.IsCompleted == false)
                {
                    throw TraceUtility.ThrowHelperError(new SecurityNegotiationException(SRP.NoBinaryNegoToSend), incomingMessage);
                }
                else if (outgoingBlob == null && sspiState.SspiNegotiation.IsCompleted == true)
                {
                    OnNegotiationComplete(sspiState, negotiationRstr, authenticatorRstr);
                    nextMessageBody = null;
                }
                else
                {
                    nextMessageBody = PrepareRstr(sspiState, outgoingBlob);
                }
            }
            return nextMessageBody;
        }

        private void OnNegotiationComplete(SspiNegotiationTokenProviderState sspiState, RequestSecurityTokenResponse negotiationRstr, RequestSecurityTokenResponse authenticatorRstr)
        {
            ISspiNegotiation sspiNegotiation = sspiState.SspiNegotiation;
            ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies = ValidateSspiNegotiation(sspiNegotiation);

            SecurityTokenResolver tokenResolver = new SspiSecurityTokenResolver(sspiNegotiation);
            GenericXmlSecurityToken serviceToken = negotiationRstr.GetIssuedToken(tokenResolver, EmptyReadOnlyCollection<SecurityTokenAuthenticator>.Instance,
                SecurityKeyEntropyMode.ServerEntropy, null, SecurityContextTokenUri, authorizationPolicies, 0, false);
            if (serviceToken == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SRP.NoServiceTokenReceived));
            }

            WrappedKeySecurityToken wrappedToken = serviceToken.ProofToken as WrappedKeySecurityToken;
            if (wrappedToken == null || wrappedToken.WrappingAlgorithm != sspiNegotiation.KeyEncryptionAlgorithm)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SRP.ProofTokenWasNotWrappedCorrectly));
            }

            byte[] proofKey = wrappedToken.GetWrappedKey();
            if (authenticatorRstr == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SRP.RSTRAuthenticatorNotPresent));
            }

            byte[] serverAuthenticator = authenticatorRstr.GetAuthenticator();
            if (serverAuthenticator == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SRP.RSTRAuthenticatorNotPresent));
            }

            if (!IsCorrectAuthenticator(sspiState, proofKey, serverAuthenticator))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SRP.RSTRAuthenticatorIncorrect));
            }

            sspiState.SetServiceToken(serviceToken);
        }

        private class SspiSecurityTokenResolver : SecurityTokenResolver, ISspiNegotiationInfo
        {
            private ISspiNegotiation _sspiNegotiation;

            public SspiSecurityTokenResolver(ISspiNegotiation sspiNegotiation)
            {
                _sspiNegotiation = sspiNegotiation;
            }

            public ISspiNegotiation SspiNegotiation
            {
                get { return _sspiNegotiation; }
            }

            protected override bool TryResolveTokenCore(SecurityKeyIdentifier keyIdentifier, out SecurityToken token)
            {
                token = null;
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotImplementedException());
            }

            protected override bool TryResolveTokenCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityToken token)
            {
                token = null;
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotImplementedException());
            }

            protected override bool TryResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
            {
                key = null;
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotImplementedException());
            }
        }
    }

    internal static class SecurityNegotiationConstants
    {
        internal const int NegotiationAuthenticatorSize = 256;
    }

    internal class SspiIssuanceChannelParameter
    {
        public SspiIssuanceChannelParameter(bool getTokenOnOpen, NetworkCredential credential)
        {
            GetTokenOnOpen = getTokenOnOpen;
            Credential = credential;
        }

        public bool GetTokenOnOpen { get; }

        public NetworkCredential Credential { get; }
    }
}
