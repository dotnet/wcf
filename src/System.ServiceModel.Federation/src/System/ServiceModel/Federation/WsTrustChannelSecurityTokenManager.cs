// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Tracing;
using System.IdentityModel.Selectors;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// WSTrustChannelSecurityTokenProvider uses WsTrust to obtain a token from an IdentityProvider
    /// </summary>
    public class WSTrustChannelSecurityTokenManager : ClientCredentialsSecurityTokenManager
    {
        private const string Namespace = "http://schemas.microsoft.com/ws/2006/05/servicemodel/securitytokenrequirement";
        private const string IssuedSecurityTokenParametersProperty = Namespace + "/IssuedSecurityTokenParameters";

        private WSTrustChannelClientCredentials _wsTrustChannelClientCredentials;

        /// <summary>
        /// Instantiates a <see cref="WSTrustChannelSecurityTokenManager"/>.
        /// </summary>
        /// <param name="wsTrustChannelClientCredentials"> the WSTrustChannelClientCredentials that can serve up a SecurityTokenProvider to use.</param>
        public WSTrustChannelSecurityTokenManager(WSTrustChannelClientCredentials wsTrustChannelClientCredentials)
            : base(wsTrustChannelClientCredentials)
        {
            _wsTrustChannelClientCredentials = wsTrustChannelClientCredentials ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new ArgumentNullException(nameof(wsTrustChannelClientCredentials)), EventLevel.Error);
        }

        /// <summary>
        /// Make use of this extensibility point for returning a custom SecurityTokenProvider when SAML tokens are specified in the tokenRequirement
        /// </summary>
        /// <param name="tokenRequirement">A SecurityTokenRequirement  </param>
        /// <returns>The appropriate SecurityTokenProvider</returns>
        public override SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
        {
            if (tokenRequirement == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new ArgumentNullException(nameof(tokenRequirement)), EventLevel.Error);

            // If the token requirement includes an issued security token parameter of type
            // WSTrustTokenParameters, then tokens should be provided by a WSTrustChannelSecurityTokenProvider.
            if (tokenRequirement.TryGetProperty(IssuedSecurityTokenParametersProperty, out SecurityTokenParameters issuedSecurityTokenParameters) &&
                issuedSecurityTokenParameters is WSTrustTokenParameters)
            {
                // pass issuedtokenRequirements
                return new WSTrustChannelSecurityTokenProvider(tokenRequirement)
                {
                    ClientCredentials = _wsTrustChannelClientCredentials.ClientCredentials
                };
            }
            // If the original ChannelFactory had a ClientCredentials instance, defer to that
            else if (_wsTrustChannelClientCredentials.SecurityTokenManager != null)
            {
                return _wsTrustChannelClientCredentials.SecurityTokenManager.CreateSecurityTokenProvider(tokenRequirement);
            }
            // This means ClientCredentials was replaced with WSTrustChannelClientCredentials in the ChannelFactory so defer
            // to base class to create other token providers.
            else
            {
                return base.CreateSecurityTokenProvider(tokenRequirement);
            }
        }
    }
}
