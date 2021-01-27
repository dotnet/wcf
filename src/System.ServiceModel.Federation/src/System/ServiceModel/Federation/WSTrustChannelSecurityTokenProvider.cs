// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IdentityModel.Security;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Runtime.Diagnostics;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Federation.System.Runtime;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.WsAddressing;
using Microsoft.IdentityModel.Protocols.WsPolicy;
using Microsoft.IdentityModel.Protocols.WsSecurity;
using Microsoft.IdentityModel.Protocols.WsTrust;
using SecurityToken = System.IdentityModel.Tokens.SecurityToken;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// <see cref="WSTrustChannelSecurityTokenProvider"/> has been designed to work with the <see cref="WSFederationHttpBinding"/> to send a WsTrust message to obtain a SecurityToken from an STS. The SecurityToken is
    /// added as an IssuedToken on the outbound WCF message.
    /// </summary>
    public class WSTrustChannelSecurityTokenProvider : SecurityTokenProvider, ICommunicationObject, ISecurityCommunicationObject
    {
        private const int DefaultPublicKeySize = 1024;
        private const string Namespace = "http://schemas.microsoft.com/ws/2006/05/servicemodel/securitytokenrequirement";
        private const string IssuedSecurityTokenParametersProperty = Namespace + "/IssuedSecurityTokenParameters";
        private const string SecurityAlgorithmSuiteProperty = Namespace + "/SecurityAlgorithmSuite";
        private const string SecurityBindingElementProperty = Namespace + "/SecurityBindingElement";
        private const string TargetAddressProperty = Namespace + "/TargetAddress";

        private SecurityKeyEntropyMode _keyEntropyMode;
        private readonly SecurityAlgorithmSuite _securityAlgorithmSuite;
        private WsSerializationContext _requestSerializationContext;
        private WrapperSecurityCommunicationObject _communicationObject;
        private EventTraceActivity _eventTraceActivity;

        /// <summary>
        /// Instantiates a <see cref="WSTrustChannelSecurityTokenProvider"/> that describe the parameters for a WSTrust request.
        /// </summary>
        /// <param name="tokenRequirement">the <see cref="SecurityTokenRequirement"/> that must contain a <see cref="WSTrustTokenParameters"/> as the <see cref="IssuedSecurityTokenParameters"/> property.</param>
        /// <exception cref="ArgumentNullException">thrown if <paramref name="tokenRequirement"/> is null.</exception>
        /// <exception cref="ArgumentException">thrown if <see cref="SecurityTokenRequirement.GetProperty{TValue}(string)"/> (IssuedSecurityTokenParameters) is not a <see cref="WSTrustTokenParameters"/>.</exception>
        public WSTrustChannelSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
        {
            SecurityTokenRequirement = tokenRequirement ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new ArgumentNullException(nameof(tokenRequirement)), EventLevel.Error);
            SecurityTokenRequirement.TryGetProperty(SecurityAlgorithmSuiteProperty, out _securityAlgorithmSuite);

            IssuedSecurityTokenParameters issuedSecurityTokenParameters = SecurityTokenRequirement.GetProperty<IssuedSecurityTokenParameters>(IssuedSecurityTokenParametersProperty);
            WSTrustTokenParameters = issuedSecurityTokenParameters as WSTrustTokenParameters;
            if (WSTrustTokenParameters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new ArgumentException(LogHelper.FormatInvariant("tokenRequirement.GetProperty<IssuedSecurityTokenParameters> must be of type: WSTrustTokenParameters. Was: '{0}.", issuedSecurityTokenParameters), nameof(tokenRequirement)), EventLevel.Error);
            }

            _communicationObject = new WrapperSecurityCommunicationObject(this);
        }

        private DateTime AddTicks(DateTime time, long ticks)
        {
            if (ticks > 0 && DateTime.MaxValue.Subtract(time).Ticks <= ticks)
                return DateTime.MaxValue;

            if (ticks < 0 && time.Subtract(DateTime.MinValue).Ticks <= -ticks)
                return DateTime.MinValue;

            return time.AddTicks(ticks);
        }

        /// <summary>
        /// Gets or sets the cached security token response.
        /// </summary>
        private WsTrustResponse CachedResponse
        {
            // FUTURE : At some point, it may be valuable to replace this with a cache (Microsoft.Extensions.Caching.Distributed.IDistributedCache, perhaps)
            //        so that caches can be shared between token providers. For the time being, this is just an in-memory WsTrustResponse since a
            //        WSTrustChannelSecurityTokenProvider will only ever use a single WsTrustRequest. If caches can be shared, though, then this would
            //        be replaced with a more full-featured cache allowing multiple providers to cache tokens in a single cache object.
            get;
            set;
        }

        private void CacheSecurityTokenResponse(WsTrustRequest request, WsTrustResponse response)
        {
            if (WSTrustTokenParameters.CacheIssuedTokens)
            {
                // If cached responses are stored in a shared cache in the future, that cache should be written
                // to here, possibly including serializing the WsTrustResponse if the cache stores byte[] (as
                // IDistributedCache does).
                CachedResponse = response;
            }
        }

        /// <summary>
        /// Returns a channel factory with the credentials that the user set the users credentials on.
        /// </summary>
        /// <returns></returns>
        internal virtual ChannelFactory<IRequestChannel> ChannelFactory { get; set; }

        internal ClientCredentials ClientCredentials { get; set; }

        /// <summary>
        /// Creates a <see cref="WsTrustRequest"/> from the <see cref="WSTrustTokenParameters"/>
        /// </summary>
        /// <returns></returns>
        protected virtual WsTrustRequest CreateWsTrustRequest()
        {
            EndpointAddress target = SecurityTokenRequirement.GetProperty<EndpointAddress>(TargetAddressProperty);

            int keySize;
            string keyType;

            switch (WSTrustTokenParameters.KeyType)
            {
                case SecurityKeyType.AsymmetricKey:
                    keySize = DefaultPublicKeySize;
                    keyType = _requestSerializationContext.TrustKeyTypes.PublicKey;
                    break;
                case SecurityKeyType.SymmetricKey:
                    keySize = _securityAlgorithmSuite.DefaultSymmetricKeyLength;
                    keyType = _requestSerializationContext.TrustKeyTypes.Symmetric;
                    break;
                case SecurityKeyType.BearerKey:
                    keySize = 0;
                    keyType = _requestSerializationContext.TrustKeyTypes.Bearer;
                    break;
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new NotSupportedException(LogHelper.FormatInvariant("KeyType is not supported: {0}", WSTrustTokenParameters.KeyType)), EventLevel.Error);
            }

            Entropy entropy = null;
            if (WSTrustTokenParameters.KeyType != SecurityKeyType.BearerKey &&
                (KeyEntropyMode == SecurityKeyEntropyMode.ClientEntropy || KeyEntropyMode == SecurityKeyEntropyMode.CombinedEntropy))
            {
                byte[] entropyBytes = new byte[keySize / 8];
                Psha1KeyGenerator.FillRandomBytes(entropyBytes);
                entropy = new Entropy(new BinarySecret(entropyBytes));
            }

            var trustRequest = new WsTrustRequest(_requestSerializationContext.TrustActions.Issue)
            {
                AppliesTo = new AppliesTo(new EndpointReference(target.Uri.OriginalString)),
                Context = RequestContext,
                KeySizeInBits = keySize,
                KeyType = keyType,
                WsTrustVersion = _requestSerializationContext.TrustVersion
            };

            if (SecurityTokenRequirement.TokenType != null)
            {
                trustRequest.TokenType = SecurityTokenRequirement.TokenType;
            }

            if (entropy != null)
            {
                trustRequest.Entropy = entropy;
                trustRequest.ComputedKeyAlgorithm = _requestSerializationContext.TrustKeyTypes.PSHA1;
            }

            return trustRequest;
        }

        private EventTraceActivity EventTraceActivity
        {
            get
            {
                if (_eventTraceActivity == null)
                {
                    _eventTraceActivity = EventTraceActivity.GetFromThreadOrCreate();
                }
                return _eventTraceActivity;
            }
        }

        private WsTrustResponse GetCachedResponse(WsTrustRequest request)
        {
            if (WSTrustTokenParameters.CacheIssuedTokens && CachedResponse != null)
            {
                // If cached responses are read from shared caches in the future, then that cache should be read here
                // and, if necessary, translated (perhaps via deserialization) into a WsTrustResponse.
                if (!IsWsTrustResponseExpired(CachedResponse))
                    return CachedResponse;
            }

            return null;
        }

        /// <summary>
        /// Gets the WsTrustVersion for the current MessageSecurityVersion.
        /// </summary>        /// <summary>
        /// Get a proof token from a WsTrust request/response pair based on section 4.4.3 of the WS-Trust 1.3 spec.
        /// How the proof token is retrieved depends on whether the requestor or issuer provide key material:
        /// Requestor   |   Issuer                  | Results
        /// -------------------------------------------------
        /// Entropy     | No key material           | No proof token returned, requestor entropy used
        /// Entropy     | Entropy                   | Computed key algorithm returned and key computed based on request and response entropy
        /// Entropy     | Rejects requestor entropy | Proof token in response used as key
        /// No entropy  | Issues key                | Proof token in response used as key
        /// No entropy  | No key material           | No proof token
        /// </summary>
        /// <param name="request">The WS-Trust request (RST).</param>
        /// <param name="response">The WS-Trust response (RSTR).</param>
        /// <returns>The proof token or null if there is no proof token.</returns>
        private BinarySecretSecurityToken GetProofToken(WsTrustRequest request, RequestSecurityTokenResponse response)
        {
            // According to the WS-Trust 1.3 spec, symmetric is the default key type
            string keyType = response.KeyType ?? request.KeyType ?? _requestSerializationContext.TrustKeyTypes.Symmetric;

            // Encrypted keys and encrypted entropy are not supported, currently, as they should
            // only be needed by unsupported message security scenarios.
            if (response.RequestedProofToken?.EncryptedKey != null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new NotSupportedException("Encrypted keys for proof tokens are not supported."), EventLevel.Error);

            // Bearer scenarios have no proof token
            if (string.Equals(keyType, _requestSerializationContext.TrustKeyTypes.Bearer, StringComparison.Ordinal))
            {
                if (response.RequestedProofToken != null || response.Entropy != null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException("Bearer key scenarios should not include a proof token or issuer entropy in the response."), EventLevel.Error);

                return null;
            }

            // If the response includes a proof token, use it as the security token's proof.
            // This scenario will occur if the request does not include entropy or if the issuer rejects the requestor's entropy.
            if (response.RequestedProofToken?.BinarySecret != null)
            {
                // Confirm that a computed key algorithm isn't also specified
                if (!string.IsNullOrEmpty(response.RequestedProofToken.ComputedKeyAlgorithm) || response.Entropy != null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException("An RSTR containing a proof token should not also have a computed key algorithm or issuer entropy."), EventLevel.Error);

                return new BinarySecretSecurityToken(response.RequestedProofToken.BinarySecret.Data);
            }
            // If the response includes a computed key algorithm, compute the proof token based on requestor and issuer entropy.
            // This scenario will occur if the requestor and issuer both provide key material.
            else if (response.RequestedProofToken?.ComputedKeyAlgorithm != null)
            {
                if (!string.Equals(keyType, _requestSerializationContext.TrustKeyTypes.Symmetric, StringComparison.Ordinal))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException("Computed key proof tokens are only supported with symmetric key types."), EventLevel.Error);

                if (string.Equals(response.RequestedProofToken.ComputedKeyAlgorithm, _requestSerializationContext.TrustKeyTypes.PSHA1, StringComparison.Ordinal))
                {
                    // Confirm that no encrypted entropy was provided as that is currently not supported.
                    // If we wish to support it in the future, most of the work will be in the WSTrust serializer;
                    // this code would just have to use protected key's .Secret property to get the key material.
                    if (response.Entropy?.ProtectedKey != null || request.Entropy?.ProtectedKey != null)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper( new NotSupportedException("Protected key entropy is not supported."), EventLevel.Error);

                    // Get issuer and requestor entropy
                    byte[] issuerEntropy = response.Entropy?.BinarySecret?.Data;
                    if (issuerEntropy == null)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException("Computed key proof tokens require issuer to supply key material via entropy."), EventLevel.Error);

                    byte[] requestorEntropy = request.Entropy?.BinarySecret?.Data;
                    if (requestorEntropy == null)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException("Computed key proof tokens require requestor to supply key material via entropy."), EventLevel.Error);

                    // Get key size
                    int keySizeInBits = response.KeySizeInBits ?? 0; // RSTR key size has precedence
                    if (keySizeInBits == 0)
                        keySizeInBits = request.KeySizeInBits ?? 0; // Followed by RST

                    if (keySizeInBits == 0)
                        keySizeInBits = _securityAlgorithmSuite?.DefaultSymmetricKeyLength ?? 0; // Symmetric keys should default to a length cooresponding to the algorithm in use

                    if (keySizeInBits == 0)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException("No key size provided."), EventLevel.Error);

                    return new BinarySecretSecurityToken(Psha1KeyGenerator.ComputeCombinedKey(issuerEntropy, requestorEntropy, keySizeInBits));
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new NotSupportedException("Only PSHA1 computed keys are supported."), EventLevel.Error);
                }
            }
            // If the response does not have a proof token or computed key value, but the request proposed entropy,
            // then the requestor's entropy is used as the proof token.
            else if (request.Entropy != null)
            {
                if (request.Entropy.ProtectedKey != null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new NotSupportedException("Protected key entropy is not supported."), EventLevel.Error);

                if (request.Entropy.BinarySecret != null)
                    return new BinarySecretSecurityToken(request.Entropy.BinarySecret.Data);
            }

            // If we get here, then no key material has been supplied (by either issuer or requestor), so there is no proof token.
            return null;
        }

        private GenericXmlSecurityKeyIdentifierClause GetSecurityKeyIdentifierForTokenReference(SecurityTokenReference securityTokenReference)
        {
            if (securityTokenReference == null)
                return null;

            return new GenericXmlSecurityKeyIdentifierClause(WsSecuritySerializer.CreateXmlElement(securityTokenReference));
        }

        /// <summary>
        /// Begins a WSTrust call to the STS to obtain a <see cref="SecurityToken"/> first checking if the token is available in the cache.
        /// </summary>
        /// <returns>A <see cref="IAsyncResult"/>.</returns>
        protected override IAsyncResult BeginGetTokenCore(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return GetTokenAsyncCore(timeout).ToApm(callback, state);
        }

        /// <summary>
        /// Completes a WSTrust call to the STS to obtain a <see cref="SecurityToken"/> first checking if the token is available in the cache.
        /// </summary>
        /// <returns>A <see cref="SecurityToken"/>.</returns>
        protected override SecurityToken EndGetTokenCore(IAsyncResult result)
        {
            return result.ToApmEnd<SecurityToken>();
        }

        private async Task<SecurityToken> GetTokenAsyncCore(TimeSpan timeout)
        {
            _communicationObject.ThrowIfClosedOrNotOpen();
            WsTrustRequest request = CreateWsTrustRequest();
            WsTrustResponse trustResponse = GetCachedResponse(request);

            if (trustResponse is null)
            {
                using (var memeoryStream = new MemoryStream())
                {
                    var writer = XmlDictionaryWriter.CreateTextWriter(memeoryStream, Encoding.UTF8);
                    var serializer = new WsTrustSerializer();
                    serializer.WriteRequest(writer, _requestSerializationContext.TrustVersion, request);
                    writer.Flush();
                    var reader = XmlDictionaryReader.CreateTextReader(memeoryStream.ToArray(), XmlDictionaryReaderQuotas.Max);
                    IRequestChannel channel = ChannelFactory.CreateChannel();
                    await Task.Factory.FromAsync(channel.BeginOpen, channel.EndOpen, null, TaskCreationOptions.None);
                    try
                    {
                        Message requestMessage = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, _requestSerializationContext.TrustActions.IssueRequest, reader);
                        Message reply = await Task.Factory.FromAsync(channel.BeginRequest, channel.EndRequest, requestMessage, null, TaskCreationOptions.None);
                        SecurityUtils.ThrowIfNegotiationFault(reply, channel.RemoteAddress);
                        trustResponse = serializer.ReadResponse(reply.GetReaderAtBodyContents());
                        CacheSecurityTokenResponse(request, trustResponse);
                    }
                    finally
                    {
                        await Task.Factory.FromAsync(channel.BeginClose, channel.EndClose, null, TaskCreationOptions.None);
                    }
                }
            }

            return CreateGenericXmlSecurityToken(request, trustResponse);
        }

        /// <summary>
        /// Makes a WSTrust call to the STS to obtain a <see cref="SecurityToken"/> first checking if the token is available in the cache.
        /// </summary>
        /// <returns>A <see cref="GenericXmlSecurityToken"/>.</returns>
        protected override SecurityToken GetTokenCore(TimeSpan timeout)
        {
            _communicationObject.ThrowIfClosedOrNotOpen();
            WsTrustRequest request = CreateWsTrustRequest();
            WsTrustResponse trustResponse = GetCachedResponse(request);

            if (trustResponse is null)
            {
                using (var memeoryStream = new MemoryStream())
                {
                    var writer = XmlDictionaryWriter.CreateTextWriter(memeoryStream, Encoding.UTF8);
                    var serializer = new WsTrustSerializer();
                    serializer.WriteRequest(writer, _requestSerializationContext.TrustVersion, request);
                    writer.Flush();
                    var reader = XmlDictionaryReader.CreateTextReader(memeoryStream.ToArray(), XmlDictionaryReaderQuotas.Max);
                    IRequestChannel channel = ChannelFactory.CreateChannel();
                    try
                    {
                        channel.Open();
                        Message reply = channel.Request(Message.CreateMessage(MessageVersion.Soap12WSAddressing10, _requestSerializationContext.TrustActions.IssueRequest, reader));
                        SecurityUtils.ThrowIfNegotiationFault(reply, channel.RemoteAddress);
                        trustResponse = serializer.ReadResponse(reply.GetReaderAtBodyContents());
                        CacheSecurityTokenResponse(request, trustResponse);
                    }
                    finally
                    {
                        channel.Close();
                    }
                }
            }

            return CreateGenericXmlSecurityToken(request, trustResponse);
        }

        private SecurityToken CreateGenericXmlSecurityToken(WsTrustRequest request, WsTrustResponse trustResponse)
        {
            // Create GenericXmlSecurityToken
            // Assumes that token is first and Saml2SecurityToken.
            RequestSecurityTokenResponse response = trustResponse.RequestSecurityTokenResponseCollection[0];

            // Get attached and unattached references
            GenericXmlSecurityKeyIdentifierClause internalSecurityKeyIdentifierClause = null;
            if (response.AttachedReference != null)
                internalSecurityKeyIdentifierClause = GetSecurityKeyIdentifierForTokenReference(response.AttachedReference);

            GenericXmlSecurityKeyIdentifierClause externalSecurityKeyIdentifierClause = null;
            if (response.UnattachedReference != null)
                externalSecurityKeyIdentifierClause = GetSecurityKeyIdentifierForTokenReference(response.UnattachedReference);

            // Get proof token
            IdentityModel.Tokens.SecurityToken proofToken = GetProofToken(request, response);

            // Get lifetime
            DateTime created = response.Lifetime?.Created ?? DateTime.UtcNow;
            DateTime expires = response.Lifetime?.Expires ?? created.AddDays(1);

            return new GenericXmlSecurityToken(response.RequestedSecurityToken.TokenElement,
                                               proofToken,
                                               created,
                                               expires,
                                               internalSecurityKeyIdentifierClause,
                                               externalSecurityKeyIdentifierClause,
                                               null);
        }

        private WsTrustVersion GetWsTrustVersion(MessageSecurityVersion messageSecurityVersion)
        {
            if (messageSecurityVersion.TrustVersion == TrustVersion.WSTrust13)
                return WsTrustVersion.Trust13;

            if (messageSecurityVersion.TrustVersion == TrustVersion.WSTrustFeb2005)
                return WsTrustVersion.TrustFeb2005;

            throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new NotSupportedException(LogHelper.FormatInvariant("Unsupported TrustVersion: '{0}'.", MessageSecurityVersion.TrustVersion)), EventLevel.Error);
        }

        private void InitializeKeyEntropyMode()
        {
            // Default to combined entropy unless another option is specified in the issuer's security binding element.
            // In previous versions of .NET WsTrust token providers, it was possible to set the default key entropy mode in client credentials.
            // That scenario does not seem to be needed in .NET Core WsTrust scenarios, so key entropy mode is simply being read from the issuer's
            // security binding element. If, in the future, it's necessary to change the default (if some scenarios don't have a security binding
            // element, for example), that could be done by adding a DefaultKeyEntropyMode property to WsTrustChannelCredentials and moving
            // the code that calculates KeyEntropyMode out to WSTrustChannelSecurityTokenManager since it can set this property
            // when it creates the provider and fall back to the credentials' default value if no security binding element is present.
            KeyEntropyMode = SecurityKeyEntropyMode.CombinedEntropy;
            SecurityBindingElement securityBindingElement = IssuerBinding?.CreateBindingElements().Find<SecurityBindingElement>();
            if (securityBindingElement != null)
            {
                KeyEntropyMode = securityBindingElement.KeyEntropyMode;
            }
        }

        /// <summary>
        /// Gets the issuer binding from the issued token parameters.
        /// </summary>
        ///
        internal Binding IssuerBinding
        {
            get => WSTrustTokenParameters?.IssuerBinding;
        }

        private bool IsWsTrustResponseExpired(WsTrustResponse response)
        {
            Lifetime responseLifetime = response?.RequestSecurityTokenResponseCollection?[0]?.Lifetime;

            if (responseLifetime == null || responseLifetime.Expires == null)
            {
                // A null lifetime could represent an invalid response or a valid response
                // with an unspecified expiration. Similarly, a response lifetime without an expiration
                // time represents an unspecified expiration. In any of these cases, err on the side of
                // retrieving a new response instead of possibly using an invalid or expired one.
                return true;
            }

            // If a response's lifetime doesn't specify a created time, conservatively assume the response was just created.
            DateTime fromTime = responseLifetime.Created?.ToUniversalTime() ?? DateTime.UtcNow;
            DateTime toTime = responseLifetime.Expires.Value.ToUniversalTime();

            long interval = toTime.Ticks - fromTime.Ticks;
            long effectiveInterval = (long)((WSTrustTokenParameters.IssuedTokenRenewalThresholdPercentage / (double)100) * interval);
            DateTime effectiveExpiration = AddTicks(fromTime, Math.Min(effectiveInterval, WSTrustTokenParameters.MaxIssuedTokenCachingTime.Ticks));

            return effectiveExpiration < DateTime.UtcNow;
        }

        /// <summary>
        /// Gets or sets the desired key entroy mode to use when making requests to the STS.
        /// </summary>
        internal SecurityKeyEntropyMode KeyEntropyMode
        {
            get => _keyEntropyMode;
            set
            {
                if (!Enum.IsDefined(typeof(SecurityKeyEntropyMode), value))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidEnumArgumentException(nameof(value), (int)value, typeof(SecurityKeyEntropyMode)), EventLevel.Error);

                _keyEntropyMode = value;
            }
        }

        internal MessageSecurityVersion MessageSecurityVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a string used as a context sent in WsTrustRequests that is useful for correlating requests.
        /// </summary>
        internal string RequestContext
        {
            get; private set;
        }

        /// <summary>
        /// Gets the <see cref="SecurityTokenRequirement"/>
        /// </summary>
        internal SecurityTokenRequirement SecurityTokenRequirement
        {
            get;
        }

        /// <summary>
        /// Set initial MessageSecurityVersion based on (in priority order):
        ///  <para>1. The message security version of the issuer binding's security binding element.</para>
        ///  <para>2. The provided DefaultMessageSecurityVersion from issued token parameters.</para>
        ///  <para>3. The message security version of the outer security binding element (from the security token requirement).</para>
        ///  <para>4. MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11.</para>
        /// </summary>
        private void SetInboundSerializationContext()
        {
            // WSTrustTokenParameters.MessageSecurityVersion can be checked directly instead of
            // extracting MessageSecurityVersion from the issuer binding, because the WSFederationHttpBinding
            // creates its security binding element using the MessageSecurityVersion from its WSTrustTokenParameters.
            MessageSecurityVersion messageSecurityVersion = WSTrustTokenParameters.MessageSecurityVersion;
            if (messageSecurityVersion == null)
                messageSecurityVersion = WSTrustTokenParameters.DefaultMessageSecurityVersion;

            if (messageSecurityVersion == null)
            {
                if (SecurityTokenRequirement.TryGetProperty(SecurityBindingElementProperty, out SecurityBindingElement outerSecurityBindingElement))
                    messageSecurityVersion = outerSecurityBindingElement.MessageSecurityVersion;
            }

            if (messageSecurityVersion == null)
                messageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11;

            MessageSecurityVersion = messageSecurityVersion;

            _requestSerializationContext = new WsSerializationContext(GetWsTrustVersion(messageSecurityVersion));
        }

        public override bool SupportsTokenCancellation => false;

        public override bool SupportsTokenRenewal => false;

        internal WSTrustTokenParameters WSTrustTokenParameters { get; }

        #region ISecurityCommunicationObject
        // This implementation is based on a combination of IssuanceTokenProviderBase<T> and CommunicationObjectSecurityTokenProvider
        // As this class is public, it's not possible to derive from the internal CommunicationObjectSecurityTokenProvider class so the
        // equivalent implementation has been provided inline in this class.
        void ISecurityCommunicationObject.OnAbort()
        {
            if (ChannelFactory != null && ChannelFactory.State == CommunicationState.Opened)
            {
                ChannelFactory.Abort();
                ChannelFactory = null;
            }
        }

        async Task ISecurityCommunicationObject.OnCloseAsync(TimeSpan timeout)
        {
            if (ChannelFactory != null && ChannelFactory.State == CommunicationState.Opened)
            {
                await Task.Factory.FromAsync(ChannelFactory.BeginClose, ChannelFactory.EndClose, timeout, null, TaskCreationOptions.None);
                ChannelFactory = null;
            }
        }

        async Task ISecurityCommunicationObject.OnOpenAsync(TimeSpan timeout)
        {
            InitializeKeyEntropyMode();
            SetInboundSerializationContext();
            RequestContext = string.IsNullOrEmpty(WSTrustTokenParameters.RequestContext) ? Guid.NewGuid().ToString() : WSTrustTokenParameters.RequestContext;
            var channelFactory = new ChannelFactory<IRequestChannel>(IssuerBinding, WSTrustTokenParameters.IssuerAddress);
            if (ClientCredentials != null)
            {
                channelFactory.Endpoint.EndpointBehaviors.Remove(typeof(ClientCredentials));
                channelFactory.Endpoint.EndpointBehaviors.Add(ClientCredentials.Clone());
            }

            await Task.Factory.FromAsync(channelFactory.BeginOpen, channelFactory.EndOpen, null, TaskCreationOptions.None);
            ChannelFactory = channelFactory;
        }

        void ISecurityCommunicationObject.OnClosed() { }

        void ISecurityCommunicationObject.OnClosing() { }

        void ISecurityCommunicationObject.OnFaulted() { }

        void ISecurityCommunicationObject.OnOpened()
        {
            SecurityTraceRecordHelper.TraceTokenProviderOpened(EventTraceActivity, this);
        }

        void ISecurityCommunicationObject.OnOpening() { }

        TimeSpan ISecurityCommunicationObject.DefaultOpenTimeout => ServiceDefaults.OpenTimeout;
        TimeSpan ISecurityCommunicationObject.DefaultCloseTimeout => ServiceDefaults.CloseTimeout;
        #endregion

        #region ICommunicationObject
        event EventHandler ICommunicationObject.Closed
        {
            add { _communicationObject.Closed += value; }
            remove { _communicationObject.Closed -= value; }
        }

        event EventHandler ICommunicationObject.Closing
        {
            add { _communicationObject.Closing += value; }
            remove { _communicationObject.Closing -= value; }
        }

        event EventHandler ICommunicationObject.Faulted
        {
            add { _communicationObject.Faulted += value; }
            remove { _communicationObject.Faulted -= value; }
        }

        event EventHandler ICommunicationObject.Opened
        {
            add { _communicationObject.Opened += value; }
            remove { _communicationObject.Opened -= value; }
        }

        event EventHandler ICommunicationObject.Opening
        {
            add { _communicationObject.Opening += value; }
            remove { _communicationObject.Opening -= value; }
        }

        CommunicationState ICommunicationObject.State
        {
            get { return _communicationObject.State; }
        }

        void ICommunicationObject.Abort()
        {
            _communicationObject.Abort();
        }

        void ICommunicationObject.Close()
        {
            _communicationObject.Close();
        }

        void ICommunicationObject.Close(TimeSpan timeout)
        {
            _communicationObject.Close(timeout);
        }

        IAsyncResult ICommunicationObject.BeginClose(AsyncCallback callback, object state)
        {
            return _communicationObject.BeginClose(callback, state);
        }

        IAsyncResult ICommunicationObject.BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _communicationObject.BeginClose(timeout, callback, state);
        }

        void ICommunicationObject.EndClose(IAsyncResult result)
        {
            _communicationObject.EndClose(result);
        }

        void ICommunicationObject.Open()
        {
            _communicationObject.Open();
        }

        void ICommunicationObject.Open(TimeSpan timeout)
        {
            _communicationObject.Open(timeout);
        }

        IAsyncResult ICommunicationObject.BeginOpen(AsyncCallback callback, object state)
        {
            return _communicationObject.BeginOpen(callback, state);
        }

        IAsyncResult ICommunicationObject.BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _communicationObject.BeginOpen(timeout, callback, state);
        }

        void ICommunicationObject.EndOpen(IAsyncResult result)
        {
            _communicationObject.EndOpen(result);
        }
        #endregion
    }
}
