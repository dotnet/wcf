// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Selectors;
using System.ServiceModel.Description;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// <see cref="WSTrustChannelClientCredentials"/> are designed to work with <see cref="WSFederationHttpBinding"/> to that will send a WsTrust message to obtain a token from an STS and add the token as
    /// an issued token when communicating with a WCF relying party.
    /// </summary>
    public class WSTrustChannelClientCredentials : ClientCredentials
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public WSTrustChannelClientCredentials()
            : base()
        {
        }

        /// <summary>
        /// Creates a shallow copy of 'other'.
        /// </summary>
        /// <param name="other">The WSTrustChannelClientCredentials to copy.</param>
        protected WSTrustChannelClientCredentials(WSTrustChannelClientCredentials other)
            : base(other)
        {
            ClientCredentials = other.ClientCredentials;
            SecurityTokenManager = other.SecurityTokenManager;
        }

        /// <summary>
        /// Crates an instance of <see cref="WSTrustChannelClientCredentials"/> with specifying a <see cref="ClientCredentials"/>
        /// </summary>
        /// <param name="clientCredentials">The <see cref="SecurityTokenManager"/> from this parameter will be used to
        /// create the <see cref="SecurityTokenProvider"/> in the case the <see cref="System.ServiceModel.Security.Tokens.SecurityTokenParameters"/> in the channel are not a <see cref="WSTrustTokenParameters"/></param>
        public WSTrustChannelClientCredentials(ClientCredentials clientCredentials)
            : base(clientCredentials)
        {
            ClientCredentials = clientCredentials;
        }

        /// <summary>
        ///  The client credentials from BindingParameters passed by ChannelFactory. There might be
        ///  other credentials configured on this instance so used as a fallback.
        /// </summary>
        public ClientCredentials ClientCredentials { get; private set; }

        /// <summary>
        /// Creates a shallow clone of this.
        /// </summary>
        protected override ClientCredentials CloneCore()
        {
            return new WSTrustChannelClientCredentials(this);
        }

        /// <summary>
        /// Returns a <see cref="SecurityTokenManager"/> to use on this channel.
        /// </summary>
        /// <remarks>The <see cref="SecurityTokenManager"/> is responsible to return the <see cref="SecurityTokenProvider"/> to obtain the issued token.
        /// <para>If <see cref="ClientCredentials"/> was passed to the constructor, then that <see cref="SecurityTokenManager"/> will be used to
        /// create the <see cref="SecurityTokenProvider"/> in the case the <see cref="System.ServiceModel.Security.Tokens.SecurityTokenParameters"/> in the channel are not a <see cref="WSTrustTokenParameters"/></para></remarks>
        /// <returns>An instance of <see cref="WSTrustChannelSecurityTokenManager" />.</returns>
        public override SecurityTokenManager CreateSecurityTokenManager()
        {
            if (ClientCredentials != null)
                SecurityTokenManager = ClientCredentials.CreateSecurityTokenManager();
    
            return new WSTrustChannelSecurityTokenManager((WSTrustChannelClientCredentials)Clone());
        }

        internal SecurityTokenManager SecurityTokenManager { get; private set; }
    }
}
