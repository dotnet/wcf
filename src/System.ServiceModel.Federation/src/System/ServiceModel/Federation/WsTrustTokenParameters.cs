// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable 1591

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using System.Xml;
using Microsoft.IdentityModel.Protocols.WsFed;

namespace System.ServiceModel.Federation
{
    public class WsTrustTokenParameters : IssuedSecurityTokenParameters
    {
        internal const bool DefaultEstablishSecurityContext = true;
        public static readonly bool DefaultCacheIssuedTokens = true;
        public static readonly int DefaultIssuedTokenRenewalThresholdPercentage = 60;
        public static readonly TimeSpan DefaultMaxIssuedTokenCachingTime = TimeSpan.MaxValue;
        public static readonly SecurityKeyType DefaultSecurityKeyType = SecurityKeyType.SymmetricKey;

        private TimeSpan _maxIssuedTokenCachingTime = DefaultMaxIssuedTokenCachingTime;
        private MessageSecurityVersion _messageSecurityVersion;
        private int _issuedTokenRenewalThresholdPercentage = DefaultIssuedTokenRenewalThresholdPercentage;
        private string _target;

        /// <summary>
        /// Values that are used to obtain a token from an IdentityProvider
        /// </summary>
        public WsTrustTokenParameters()
        {
            KeyType = DefaultSecurityKeyType;
            EstablishSecurityContext = DefaultEstablishSecurityContext;
            DefaultMessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;
            MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;
        }

        protected WsTrustTokenParameters(WsTrustTokenParameters other) : base(other)
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
            RequestContext = other.RequestContext;
            _target = other.Target;
            EstablishSecurityContext = other.EstablishSecurityContext;
        }

        protected override SecurityTokenParameters CloneCore()
        {
            return new WsTrustTokenParameters(this);
        }

        public ICollection<XmlElement> AdditionalRequestParameters { get; } = new Collection<XmlElement>();

        public bool CacheIssuedTokens { get; set; } = DefaultCacheIssuedTokens;

        public ICollection<ClaimType> ClaimTypes { get; } = new Collection<ClaimType>();

        /// <summary>
        /// Gets or sets the percentage of the issued token's lifetime at which it should be renewed instead of cached.
        /// </summary>
        public int IssuedTokenRenewalThresholdPercentage
        {
            get => _issuedTokenRenewalThresholdPercentage;
            set => _issuedTokenRenewalThresholdPercentage = (value <= 0 || value > 100)
                ? throw new ArgumentOutOfRangeException(nameof(value), $"IssuedTokenRenewalThresholdPercentage  must be greater than or equal to 1 and less than or equal to 100. Was: '{value}'")
                : value;
        }

        public int? KeySize { get; set; }

        /// <summary>
        /// Gets or sets the maximum time an issued token will be cached before renewing it.
        /// </summary>
        public TimeSpan MaxIssuedTokenCachingTime
        {
            get => _maxIssuedTokenCachingTime;
            set => _maxIssuedTokenCachingTime = value <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(value), "MaxIssuedTokenCachingTime must be greater than TimeSpan.Zero. Was: '{value}'") // TODO - Get exception messages from resources
                : value;
        }

        public MessageSecurityVersion MessageSecurityVersion
        {
            get => _messageSecurityVersion;
            set => _messageSecurityVersion = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string RequestContext { get; set; }

        public string Target
        {
            get => _target;
            set => _target = !string.IsNullOrEmpty(value) ? value : throw new ArgumentNullException(nameof(value));
        }

        public bool EstablishSecurityContext { get; set; }
    }
}
