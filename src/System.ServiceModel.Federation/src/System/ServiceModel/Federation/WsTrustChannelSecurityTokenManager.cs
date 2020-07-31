// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable 1591

using System.IdentityModel.Selectors;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens.Saml;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// WsTrustChannelSecurityTokenProvider uses WsTrust to obtain a token from an IdentityProvider
    /// </summary>
    public class WsTrustChannelSecurityTokenManager : ClientCredentialsSecurityTokenManager
    {
        private WsTrustChannelClientCredentials _wsTrustChannelClientCredentials;

        /// <summary>
        ///
        /// </summary>
        /// <param name="wsTrustChannelClientCredentials"></param>
        public WsTrustChannelSecurityTokenManager(WsTrustChannelClientCredentials wsTrustChannelClientCredentials)
            : base(wsTrustChannelClientCredentials)
        {
            _wsTrustChannelClientCredentials = wsTrustChannelClientCredentials ?? throw LogHelper.LogArgumentNullException(nameof(wsTrustChannelClientCredentials));
        }

        /// <summary>
        /// Make use of this extensibility point for returning a custom SecurityTokenProvider when SAML tokens are specified in the tokenRequirement
        /// </summary>
        /// <param name="tokenRequirement">A SecurityTokenRequirement  </param>
        /// <returns>The appropriate SecurityTokenProvider</returns>
        public override SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
        {
            if (tokenRequirement == null)
                throw LogHelper.LogArgumentNullException(nameof(tokenRequirement));

            // TODO - we should check the value of the IssuedTokenType on WsTrustTokenParameters
            if (Saml2Constants.OasisWssSaml2TokenProfile11.Equals(tokenRequirement.TokenType) ||
                Saml2Constants.Saml2TokenProfile11.Equals(tokenRequirement.TokenType) ||
                SamlConstants.OasisWssSamlTokenProfile11.Equals(tokenRequirement.TokenType) ||
                tokenRequirement.TokenType is null) // Treat unspecified token types as being SAML
            {
                // pass issuedtokenRequirements
                return new WsTrustChannelSecurityTokenProvider(tokenRequirement)
                {
                    ClientCredentials = _wsTrustChannelClientCredentials.ClientCredentials
                };
            }
            // If the original ChannelFactory had a ClientCredentials instance, defer to that
            else if (_wsTrustChannelClientCredentials.SecurityTokenManager != null)
            {
                return _wsTrustChannelClientCredentials.SecurityTokenManager.CreateSecurityTokenProvider(tokenRequirement);
            }
            // This means ClientCredentials was replaced with WsTrustChannelClientCredentials in the ChannelFactory so defer
            // to base class to create other token providers.
            else
            {
                return base.CreateSecurityTokenProvider(tokenRequirement);
            }
        }
    }
}
