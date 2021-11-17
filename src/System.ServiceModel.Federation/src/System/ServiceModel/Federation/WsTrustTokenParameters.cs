// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.Xml;
using Microsoft.IdentityModel.Protocols.WsTrust;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// <see cref="WSTrustTokenParameters"/> is designed to be used with the <see cref="WSFederationHttpBinding"/> to send a WSTrust message to an STS and attach the token received as
    /// an IssuedToken in the message to a WCF RelyingParty.
    /// </summary>
    public class WSTrustTokenParameters : IssuedSecurityTokenParameters
    {
        public static readonly bool DefaultCacheIssuedTokens = true;
        public static readonly int DefaultIssuedTokenRenewalThresholdPercentage = 60;
        public static readonly TimeSpan DefaultMaxIssuedTokenCachingTime = TimeSpan.MaxValue;
        public static readonly SecurityKeyType DefaultSecurityKeyType = SecurityKeyType.SymmetricKey;

        private TimeSpan _maxIssuedTokenCachingTime = DefaultMaxIssuedTokenCachingTime;
        private MessageSecurityVersion _messageSecurityVersion;
        private int _issuedTokenRenewalThresholdPercentage = DefaultIssuedTokenRenewalThresholdPercentage;

        /// <summary>
        /// Instantiates a <see cref="wsTrustTokenParameters"/> that describe the parameters for a WSTrust request.
        /// </summary>
        /// <remarks><see cref="KeyType"/> == <see cref="SecurityKeyType.SymmetricKey"/>.
        /// <para><see cref="MessageSecurityVersion"/> == <see cref="MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10"/></para></remarks>
        public WSTrustTokenParameters()
        {
            KeyType = DefaultSecurityKeyType;
            DefaultMessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;
            MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;
        }

        /// <summary>
        /// Creates a shallow copy of 'other'.
        /// </summary>
        /// <param name="other">The WSTrustTokenParameters to copy.</param>
        protected WSTrustTokenParameters(WSTrustTokenParameters other)
            : base(other)
        {
            foreach (var parameter in other.AdditionalRequestParameters)
                AdditionalRequestParameters.Add(parameter);

            CacheIssuedTokens = other.CacheIssuedTokens;
            foreach (var claimType in ClaimTypes)
                ClaimTypes.Add(claimType);

            _issuedTokenRenewalThresholdPercentage = other.IssuedTokenRenewalThresholdPercentage;
            KeySize = other.KeySize;
            _maxIssuedTokenCachingTime = other.MaxIssuedTokenCachingTime;
            _messageSecurityVersion = other.MessageSecurityVersion;
        }

        /// <summary>
        /// Creates a shallow clone of this.
        /// </summary>
        protected override SecurityTokenParameters CloneCore()
        {
            return new WSTrustTokenParameters(this);
        }

        /// <summary>
        /// Allows the addition of custom <see cref="XmlElement"/>s to the WSTrust request
        /// <para>see: http://docs.oasis-open.org/ws-sx/ws-trust/200512/ws-trust-1.3-os.html </para>
        /// </summary>
        public ICollection<XmlElement> AdditionalRequestParameters { get; } = new Collection<XmlElement>();

        /// <summary>
        /// Gets or set a bool that controls if tokens received from that STS should be cached.
        /// </summary>
        public bool CacheIssuedTokens { get; set; } = DefaultCacheIssuedTokens;

        /// <summary>
        /// Allows the addition of <see cref="Claims"/> to the WSTrust request
        /// <para>see: http://docs.oasis-open.org/ws-sx/ws-trust/200512/ws-trust-1.3-os.html </para>
        /// </summary>
        public ICollection<Claims> ClaimTypes { get; } = new Collection<Claims>();

        /// <summary>
        /// Gets or sets the percentage of the issued token's lifetime at which it should be renewed instead of cached.
        /// </summary>
        public int IssuedTokenRenewalThresholdPercentage
        {
            get => _issuedTokenRenewalThresholdPercentage;
            set => _issuedTokenRenewalThresholdPercentage = (value <= 0 || value > 100)
                ? throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new ArgumentOutOfRangeException(nameof(value), LogHelper.FormatInvariant(SR.GetResourceString(SR.IssuedTokenRenewalThresholdPercentageIncorrect), value)), EventLevel.Error)
                : value;
        }

        /// <summary>
        /// Gets or sets the keysize to request from the STS
        /// </summary>
        public int? KeySize { get; set; }

        /// <summary>
        /// Gets or sets the maximum time an issued token will be cached before renewing it.
        /// </summary>
        public TimeSpan MaxIssuedTokenCachingTime
        {
            get => _maxIssuedTokenCachingTime;
            set => _maxIssuedTokenCachingTime = value <= TimeSpan.Zero
                ? throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new ArgumentOutOfRangeException(nameof(value), LogHelper.FormatInvariant(SR.GetResourceString(SR.MaxIssuedTokenCachingTimeMustBeGreaterThanTimeSpanZero), value)), EventLevel.Error)
                : value;
        }

        /// <summary>
        /// Gets or sets the <see cref="MessageSecurityVersion"/> to use.
        /// </summary>
        public MessageSecurityVersion MessageSecurityVersion
        {
            get => _messageSecurityVersion;
            set => _messageSecurityVersion = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new ArgumentNullException(nameof(value)), EventLevel.Error);
        }

        /// <summary>
        /// Gets or sets a string to put in the WSTrust request as a 'context' that helps in tracking request / response pairs.
        /// <para>see: http://docs.oasis-open.org/ws-sx/ws-trust/200512/ws-trust-1.3-os.html </para>
        /// </summary>
        public string RequestContext { get; set; }

        public static WSTrustTokenParameters CreateWSFederationTokenParameters(Binding issuerBinding, EndpointAddress issuerAddress)
        {
            return new WSTrustTokenParameters
            {
                IssuerAddress = issuerAddress,
                IssuerBinding = issuerBinding,
                KeyType = SecurityKeyType.SymmetricKey,
                TokenType = Saml2Constants.OasisWssSaml2TokenProfile11,
                MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10
            };
        }

        public static WSTrustTokenParameters CreateWS2007FederationTokenParameters(Binding issuerBinding, EndpointAddress issuerAddress)
        {
            return new WSTrustTokenParameters
            {
                IssuerAddress = issuerAddress,
                IssuerBinding = issuerBinding,
                KeyType = SecurityKeyType.SymmetricKey,
                TokenType = Saml2Constants.OasisWssSaml2TokenProfile11,
                MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10
            };
        }
    }
}
