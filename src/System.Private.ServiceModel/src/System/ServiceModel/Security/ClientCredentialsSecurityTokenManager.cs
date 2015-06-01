// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel
{
    public class ClientCredentialsSecurityTokenManager : SecurityTokenManager
    {
        private ClientCredentials _parent;

        public ClientCredentialsSecurityTokenManager(ClientCredentials clientCredentials)
        {
            if (clientCredentials == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("clientCredentials");
            }
            _parent = clientCredentials;
        }

        public ClientCredentials ClientCredentials
        {
            get { return _parent; }
        }


        private bool IsDigestAuthenticationScheme(SecurityTokenRequirement requirement)
        {
            if (requirement.Properties.ContainsKey(ServiceModelSecurityTokenRequirement.HttpAuthenticationSchemeProperty))
            {
                AuthenticationSchemes authScheme = (AuthenticationSchemes)requirement.Properties[ServiceModelSecurityTokenRequirement.HttpAuthenticationSchemeProperty];

                if (!authScheme.IsSingleton())
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("authScheme", string.Format(SR.HttpRequiresSingleAuthScheme, authScheme));
                }

                return (authScheme == AuthenticationSchemes.Digest);
            }
            else
            {
                return false;
            }
        }


        internal protected bool IsIssuedSecurityTokenRequirement(SecurityTokenRequirement requirement)
        {
            if (requirement != null && requirement.Properties.ContainsKey(ServiceModelSecurityTokenRequirement.IssuerAddressProperty))
            {
                // handle all issued token requirements except for spnego, tlsnego and secure conversation
                if (requirement.TokenType == ServiceModelSecurityTokenTypes.AnonymousSslnego || requirement.TokenType == ServiceModelSecurityTokenTypes.MutualSslnego
                    || requirement.TokenType == ServiceModelSecurityTokenTypes.SecureConversation || requirement.TokenType == ServiceModelSecurityTokenTypes.Spnego)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public override SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
        {
            throw ExceptionHelper.PlatformNotSupported("CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement) not supported");
        }

        public override SecurityTokenSerializer CreateSecurityTokenSerializer(SecurityTokenVersion version)
        {
            // not referenced anywhere in current code, but must implement abstract. 
            throw ExceptionHelper.PlatformNotSupported("CreateSecurityTokenSerializer(SecurityTokenVersion version) not supported");
        }

        public override SecurityTokenAuthenticator CreateSecurityTokenAuthenticator(SecurityTokenRequirement tokenRequirement, out SecurityTokenResolver outOfBandTokenResolver)
        {
            // not referenced anywhere in current code, but must implement abstract. 
            throw ExceptionHelper.PlatformNotSupported("CreateSecurityTokenAuthenticator(SecurityTokenRequirement tokenRequirement, out SecurityTokenResolver outOfBandTokenResolver) not supported");
        }
    }
}
