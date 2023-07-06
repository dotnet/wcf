// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// The <see cref="WSFederationHttpBinding"/> is designed to send a WSTrust message to an STS and attach the token received as
    /// an IssuedToken in the message to a WCF RelyingParty.
    /// </summary>
    public class WSFederationHttpBinding : WSHttpBinding
    {
        /// <summary>
        /// Creates a <see cref="WSFederationHttpBinding"/> to send a WSTrust message to an STS and attach the token received as
        /// an IssuedToken in the message to a WCF RelyingParty.
        /// </summary>
        /// <param name="wsTrustTokenParameters">the <see cref="WSTrustTokenParameters"/> that describe the WSTrust request.</param>
        /// <remarks>The <see cref="SecurityMode"/> (for the RelyingParty), is set to <see cref="SecurityMode.TransportWithMessageCredential"/>.
        /// <para>The ClientCredentialType (for the RelyingParty), is set to: <see cref="MessageCredentialType.IssuedToken"/>.</para></remarks>
        /// <exception cref="ArgumentNullException">if <paramref name="wsTrustTokenParameters"/> is null.</exception>
        public WSFederationHttpBinding(WSTrustTokenParameters wsTrustTokenParameters)
            : base(SecurityMode.TransportWithMessageCredential)
        {
            WSTrustTokenParameters = wsTrustTokenParameters ?? throw new ArgumentNullException(nameof(wsTrustTokenParameters));
            Security.Message.ClientCredentialType = MessageCredentialType.IssuedToken;
        }

        /// <summary>
        /// The WSTrustTokenParameters describe the WsTrust request that will be sent to the STS.
        /// </summary>
        /// <remarks>see: http://docs.oasis-open.org/ws-sx/ws-trust/200512/ws-trust-1.3-os.html </remarks>
        public WSTrustTokenParameters WSTrustTokenParameters
        {
            get;
        }

        private SecurityBindingElement SecurityBindingElement { get; set; }

        /// <summary>
        /// Builds the <see cref="SecurityBindingElement"/> for the RelyingParty channel.
        /// </summary>
        /// <returns>a <see cref="SecurityBindingElement"/> for the RelyingParty channel.</returns>
        /// <remarks>Creates a new <see cref="TransportBindingElement"/> and adds <see cref="WSTrustTokenParameters"/> to the appropriate EndpointSupportingTokenParameters (Endorsing or Signed).
        /// <para>Sets: <see cref="SecurityBindingElement.MessageSecurityVersion"/> == <see cref="WSTrustTokenParameters.MessageSecurityVersion"/>.</para>
        /// <para>Sets: <see cref="SecurityBindingElement.IncludeTimestamp"/> == true.</para></remarks>
        protected override SecurityBindingElement CreateMessageSecurity()
        {
            WSTrustTokenParameters.RequireDerivedKeys = false;
            var result = new TransportSecurityBindingElement
            {
                IncludeTimestamp = true,
                MessageSecurityVersion = WSTrustTokenParameters.MessageSecurityVersion
            };

            if (WSTrustTokenParameters.KeyType == SecurityKeyType.BearerKey)
            {
                result.EndpointSupportingTokenParameters.Signed.Add(WSTrustTokenParameters);
            }
            else
            {
                result.EndpointSupportingTokenParameters.Endorsing.Add(WSTrustTokenParameters);
            }

            if (Security.Message.EstablishSecurityContext)
            {
                var securityContextWrappingElement = new TransportSecurityBindingElement
                {
                    IncludeTimestamp = true,
                    MessageSecurityVersion = WSTrustTokenParameters.MessageSecurityVersion
                };
                securityContextWrappingElement.LocalClientSettings.DetectReplays = false;

                var scParameters = new SecureConversationSecurityTokenParameters(result)
                {
                    RequireCancellation = true,
                    RequireDerivedKeys = false
                };
                securityContextWrappingElement.EndpointSupportingTokenParameters.Endorsing.Add(scParameters);

                result = securityContextWrappingElement;
            }

            SecurityBindingElement = result;
            return result;
        }

        /// <summary>
        /// Builds the <see cref="BindingElementCollection"/> for the channnel.
        /// </summary>
        /// <returns><see cref="BindingElementCollection"/> for the channel.</returns>
        /// <remarks>Adds a <see cref="WSFederationBindingElement"/> in first position to the <see cref="BindingElementCollection"/> returned from base.CreateBindingElements().</remarks>
        public override BindingElementCollection CreateBindingElements()
        {
            var bindingElementCollection = base.CreateBindingElements();
            bindingElementCollection.Insert(0, new WSFederationBindingElement(WSTrustTokenParameters, SecurityBindingElement));
            return bindingElementCollection;
        }
    }
}
