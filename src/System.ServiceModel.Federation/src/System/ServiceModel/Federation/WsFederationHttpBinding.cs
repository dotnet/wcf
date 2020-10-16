// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable 1591

using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Federation
{
    public class WSFederationHttpBinding : WSHttpBinding
    {
        // binding is always TransportWithMessageCredentialy
        public WSFederationHttpBinding(WSTrustTokenParameters wsTrustTokenParameters) : base(SecurityMode.TransportWithMessageCredential)
        {
            WSTrustTokenParameters = wsTrustTokenParameters ?? throw new ArgumentNullException(nameof(wsTrustTokenParameters));
            Security.Message.ClientCredentialType = MessageCredentialType.IssuedToken;
        }

        public WSTrustTokenParameters WSTrustTokenParameters
        {
            get;
        }

        private SecurityBindingElement SecurityBindingElement { get; set; }

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

            if (WSTrustTokenParameters.EstablishSecurityContext)
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

        public override BindingElementCollection CreateBindingElements()
        {
            var bindingElementCollection = base.CreateBindingElements();
            bindingElementCollection.Insert(0, new WSFederationBindingElement(WSTrustTokenParameters, SecurityBindingElement));
            return bindingElementCollection;
        }


        protected override TransportBindingElement GetTransport()
        {
            var transportBindingElement = base.GetTransport();
            return transportBindingElement;
        }
    }
}
