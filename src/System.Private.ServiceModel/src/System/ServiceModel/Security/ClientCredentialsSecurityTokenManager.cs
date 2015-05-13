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
            return this.CreateSecurityTokenProvider(tokenRequirement, false);
        }

        internal SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement, bool disableInfoCard)
        {
            if (tokenRequirement == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenRequirement");
            }

            SecurityTokenProvider result = null;

            // Where CardSpace is not supported, CardSpaceTryCreateSecurityTokenProviderStub(...) always returns false  
            if (tokenRequirement is RecipientServiceModelSecurityTokenRequirement && tokenRequirement.TokenType == SecurityTokenTypes.X509Certificate && tokenRequirement.KeyUsage == SecurityKeyUsage.Exchange)
            {
                throw ExceptionHelper.PlatformNotSupported("uncorrelated duplex case - not supported");
            }
            else if (tokenRequirement is InitiatorServiceModelSecurityTokenRequirement)
            {
                InitiatorServiceModelSecurityTokenRequirement initiatorRequirement = tokenRequirement as InitiatorServiceModelSecurityTokenRequirement;

                // initiatorRequirement will never be null due to the preceding 'is' validation.
                string tokenType = initiatorRequirement.TokenType;
                if (IsIssuedSecurityTokenRequirement(initiatorRequirement))
                {
                    throw ExceptionHelper.PlatformNotSupported("Security tokens are not supported for security. ");
                }
                else if (tokenType == SecurityTokenTypes.X509Certificate)
                {
                    throw ExceptionHelper.PlatformNotSupported("X509Certificates are not supported for security. ");
                }
                else if (tokenType == SecurityTokenTypes.Kerberos)
                {
                    throw ExceptionHelper.PlatformNotSupported("Kerberos is not supported for security yet. ");
                }
                else if (tokenType == SecurityTokenTypes.UserName)
                {
                    if (_parent.UserName.UserName == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.UserNamePasswordNotProvidedOnClientCredentials));
                    }
                    result = new UserNameSecurityTokenProvider(_parent.UserName.UserName, _parent.UserName.Password);
                }
                else if (tokenType == ServiceModelSecurityTokenTypes.SspiCredential)
                {
                    if (IsDigestAuthenticationScheme(initiatorRequirement))
                    {
                        result = new SspiSecurityTokenProvider(SecurityUtils.GetNetworkCredentialOrDefault(_parent.HttpDigest.ClientCredential), true, _parent.HttpDigest.AllowedImpersonationLevel);
                    }
                    else
                    {
                        throw ExceptionHelper.PlatformNotSupported("NTLM is not supported for security yet. ");
                    }
                }
                else if (tokenType == ServiceModelSecurityTokenTypes.Spnego)
                {
                    throw ExceptionHelper.PlatformNotSupported("Spnego is not supported for security yet. ");
                }
                else if (tokenType == ServiceModelSecurityTokenTypes.MutualSslnego)
                {
                    throw ExceptionHelper.PlatformNotSupported("MutualSslnego is not supported for security yet. ");
                    //result = CreateTlsnegoTokenProvider(initiatorRequirement, true);
                }
                else if (tokenType == ServiceModelSecurityTokenTypes.AnonymousSslnego)
                {
                    throw ExceptionHelper.PlatformNotSupported("AnonymousSslnego is not supported for security yet. ");
                }
                else if (tokenType == ServiceModelSecurityTokenTypes.SecureConversation)
                {
                    throw ExceptionHelper.PlatformNotSupported("SecureConversation is not supported for security yet. ");
                }
            }

            if ((result == null) && !tokenRequirement.IsOptionalToken)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new NotSupportedException(string.Format(SR.SecurityTokenManagerCannotCreateProviderForRequirement, tokenRequirement)));
            }

            return result;
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
