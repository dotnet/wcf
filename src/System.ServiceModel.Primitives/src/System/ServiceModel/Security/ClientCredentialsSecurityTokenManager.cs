// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public class ClientCredentialsSecurityTokenManager : SecurityTokenManager
    {
        public ClientCredentialsSecurityTokenManager(ClientCredentials clientCredentials)
        {
            ClientCredentials = clientCredentials ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(clientCredentials));
        }

        public ClientCredentials ClientCredentials { get; }

        private string GetServicePrincipalName(InitiatorServiceModelSecurityTokenRequirement initiatorRequirement)
        {
            EndpointAddress targetAddress = initiatorRequirement.TargetAddress;
            if (targetAddress == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.TokenRequirementDoesNotSpecifyTargetAddress, initiatorRequirement));
            }
            IdentityVerifier identityVerifier;
            SecurityBindingElement securityBindingElement = initiatorRequirement.SecurityBindingElement;
            if (securityBindingElement != null)
            {
                identityVerifier = securityBindingElement.LocalClientSettings.IdentityVerifier;
            }
            else
            {
                identityVerifier = IdentityVerifier.CreateDefault();
            }
            EndpointIdentity identity;
            identityVerifier.TryGetIdentity(targetAddress, out identity);
            return SecurityUtils.GetSpnFromIdentity(identity, targetAddress);
        }

        private bool IsDigestAuthenticationScheme(SecurityTokenRequirement requirement)
        {
            if (requirement.Properties.ContainsKey(ServiceModelSecurityTokenRequirement.HttpAuthenticationSchemeProperty))
            {
                AuthenticationSchemes authScheme = (AuthenticationSchemes)requirement.Properties[ServiceModelSecurityTokenRequirement.HttpAuthenticationSchemeProperty];

                if (!authScheme.IsSingleton() && authScheme != AuthenticationSchemes.IntegratedWindowsAuthentication)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("authScheme", string.Format(SRP.HttpRequiresSingleAuthScheme, authScheme));
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
            if (tokenRequirement == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenRequirement));
            }

            SecurityTokenProvider result = null;
            if (tokenRequirement is RecipientServiceModelSecurityTokenRequirement && tokenRequirement.TokenType == SecurityTokenTypes.X509Certificate && tokenRequirement.KeyUsage == SecurityKeyUsage.Exchange)
            {
                // this is the uncorrelated duplex case
                if (ClientCredentials.ClientCertificate.Certificate == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.ClientCertificateNotProvidedOnClientCredentials)));
                }
                result = new X509SecurityTokenProvider(ClientCredentials.ClientCertificate.Certificate, ClientCredentials.ClientCertificate.CloneCertificate);
            }
            else if (tokenRequirement is InitiatorServiceModelSecurityTokenRequirement)
            {
                InitiatorServiceModelSecurityTokenRequirement initiatorRequirement = tokenRequirement as InitiatorServiceModelSecurityTokenRequirement;
                string tokenType = initiatorRequirement.TokenType;
                if (IsIssuedSecurityTokenRequirement(initiatorRequirement))
                {
                    throw ExceptionHelper.PlatformNotSupported("CreateSecurityTokenProvider (IsIssuedSecurityTokenRequirement(initiatorRequirement)");
                }
                else if (tokenType == SecurityTokenTypes.X509Certificate)
                {
                    if (initiatorRequirement.Properties.ContainsKey(SecurityTokenRequirement.KeyUsageProperty) && initiatorRequirement.KeyUsage == SecurityKeyUsage.Exchange)
                    {
                        throw ExceptionHelper.PlatformNotSupported("CreateSecurityTokenProvider X509Certificate - SecurityKeyUsage.Exchange");
                    }
                    else
                    {
                        if (ClientCredentials.ClientCertificate.Certificate == null)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.ClientCertificateNotProvidedOnClientCredentials)));
                        }
                        result = new X509SecurityTokenProvider(ClientCredentials.ClientCertificate.Certificate, ClientCredentials.ClientCertificate.CloneCertificate);
                    }
                }
                else if (tokenType == SecurityTokenTypes.Kerberos)
                {
                    string spn = GetServicePrincipalName(initiatorRequirement);
                    result = new KerberosSecurityTokenProviderWrapper(
                        new KerberosSecurityTokenProvider(spn, ClientCredentials.Windows.AllowedImpersonationLevel, SecurityUtils.GetNetworkCredentialOrDefault(ClientCredentials.Windows.ClientCredential)));
                }
                else if (tokenType == SecurityTokenTypes.UserName)
                {
                    if (ClientCredentials.UserName.UserName == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.UserNamePasswordNotProvidedOnClientCredentials));
                    }
                    result = new UserNameSecurityTokenProvider(ClientCredentials.UserName.UserName, ClientCredentials.UserName.Password);
                }
                else if (tokenType == ServiceModelSecurityTokenTypes.SspiCredential)
                {
                    if (IsDigestAuthenticationScheme(initiatorRequirement))
                    {
                        result = new SspiSecurityTokenProvider(SecurityUtils.GetNetworkCredentialOrDefault(ClientCredentials.HttpDigest.ClientCredential), true, TokenImpersonationLevel.Delegation);
                    }
                    else
                    {
#pragma warning disable 618   // to disable AllowNtlm obsolete warning.      
                        result = new SspiSecurityTokenProvider(SecurityUtils.GetNetworkCredentialOrDefault(ClientCredentials.Windows.ClientCredential),

                            ClientCredentials.Windows.AllowNtlm,
                            ClientCredentials.Windows.AllowedImpersonationLevel);
#pragma warning restore 618
                    }
                }
                else if (tokenType == ServiceModelSecurityTokenTypes.SecureConversation)
                {
                    result = CreateSecureConversationSecurityTokenProvider(initiatorRequirement);
                }
            }

            if ((result == null) && !tokenRequirement.IsOptionalToken)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.SecurityTokenManagerCannotCreateProviderForRequirement, tokenRequirement)));
            }

            return result;
        }

        public override SecurityTokenSerializer CreateSecurityTokenSerializer(SecurityTokenVersion version)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(version));
            }

            MessageSecurityTokenVersion wsVersion = version as MessageSecurityTokenVersion;
            if (wsVersion != null)
            {
                return new WSSecurityTokenSerializer(wsVersion.SecurityVersion, wsVersion.TrustVersion, wsVersion.SecureConversationVersion, wsVersion.EmitBspRequiredAttributes, null, null, null);
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.SecurityTokenManagerCannotCreateSerializerForVersion, version)));
            }
        }

        private SecurityTokenProvider CreateSecureConversationSecurityTokenProvider(InitiatorServiceModelSecurityTokenRequirement initiatorRequirement)
        {
            EndpointAddress targetAddress = initiatorRequirement.TargetAddress;
            if (targetAddress == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.TokenRequirementDoesNotSpecifyTargetAddress, initiatorRequirement));
            }
            SecurityBindingElement securityBindingElement = initiatorRequirement.SecurityBindingElement;
            if (securityBindingElement == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.TokenProviderRequiresSecurityBindingElement, initiatorRequirement));
            }
            LocalClientSecuritySettings localClientSettings = securityBindingElement.LocalClientSettings;
            BindingContext issuerBindingContext = initiatorRequirement.GetProperty<BindingContext>(ServiceModelSecurityTokenRequirement.IssuerBindingContextProperty);
            ChannelParameterCollection channelParameters = initiatorRequirement.GetPropertyOrDefault<ChannelParameterCollection>(ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty, null);
            bool isSessionMode = initiatorRequirement.SupportSecurityContextCancellation;
            if (isSessionMode)
            {
                SecuritySessionSecurityTokenProvider sessionTokenProvider = new SecuritySessionSecurityTokenProvider();
                sessionTokenProvider.BootstrapSecurityBindingElement = SecurityUtils.GetIssuerSecurityBindingElement(initiatorRequirement);
                sessionTokenProvider.IssuedSecurityTokenParameters = initiatorRequirement.GetProperty<SecurityTokenParameters>(ServiceModelSecurityTokenRequirement.IssuedSecurityTokenParametersProperty);
                sessionTokenProvider.IssuerBindingContext = issuerBindingContext;
                sessionTokenProvider.KeyEntropyMode = securityBindingElement.KeyEntropyMode;
                sessionTokenProvider.SecurityAlgorithmSuite = initiatorRequirement.SecurityAlgorithmSuite;
                sessionTokenProvider.StandardsManager = SecurityUtils.CreateSecurityStandardsManager(initiatorRequirement, this);
                sessionTokenProvider.TargetAddress = targetAddress;
                sessionTokenProvider.Via = initiatorRequirement.GetPropertyOrDefault<Uri>(InitiatorServiceModelSecurityTokenRequirement.ViaProperty, null);
                Uri privacyNoticeUri;
                if (initiatorRequirement.TryGetProperty(ServiceModelSecurityTokenRequirement.PrivacyNoticeUriProperty, out privacyNoticeUri))
                {
                    sessionTokenProvider.PrivacyNoticeUri = privacyNoticeUri;
                }

                int privacyNoticeVersion;
                if (initiatorRequirement.TryGetProperty(ServiceModelSecurityTokenRequirement.PrivacyNoticeVersionProperty, out privacyNoticeVersion))
                {
                    sessionTokenProvider.PrivacyNoticeVersion = privacyNoticeVersion;
                }

                EndpointAddress localAddress;
                if (initiatorRequirement.TryGetProperty(ServiceModelSecurityTokenRequirement.DuplexClientLocalAddressProperty, out localAddress))
                {
                    sessionTokenProvider.LocalAddress = localAddress;
                }

                sessionTokenProvider.ChannelParameters = channelParameters;
                return sessionTokenProvider;
            }
            else
            {
                AcceleratedTokenProvider acceleratedTokenProvider = new AcceleratedTokenProvider();
                acceleratedTokenProvider.IssuerAddress = initiatorRequirement.IssuerAddress;
                acceleratedTokenProvider.BootstrapSecurityBindingElement = SecurityUtils.GetIssuerSecurityBindingElement(initiatorRequirement);
                acceleratedTokenProvider.CacheServiceTokens = localClientSettings.CacheCookies;
                acceleratedTokenProvider.IssuerBindingContext = issuerBindingContext;
                acceleratedTokenProvider.KeyEntropyMode = securityBindingElement.KeyEntropyMode;
                acceleratedTokenProvider.MaxServiceTokenCachingTime = localClientSettings.MaxCookieCachingTime;
                acceleratedTokenProvider.SecurityAlgorithmSuite = initiatorRequirement.SecurityAlgorithmSuite;
                acceleratedTokenProvider.ServiceTokenValidityThresholdPercentage = localClientSettings.CookieRenewalThresholdPercentage;
                acceleratedTokenProvider.StandardsManager = SecurityUtils.CreateSecurityStandardsManager(initiatorRequirement, this);
                acceleratedTokenProvider.TargetAddress = targetAddress;
                acceleratedTokenProvider.Via = initiatorRequirement.GetPropertyOrDefault<Uri>(InitiatorServiceModelSecurityTokenRequirement.ViaProperty, null);
                return acceleratedTokenProvider;
            }
        }

        private X509SecurityTokenAuthenticator CreateServerX509TokenAuthenticator()
        {
            return new X509SecurityTokenAuthenticator(ClientCredentials.ServiceCertificate.Authentication.GetCertificateValidator());
        }

        private X509SecurityTokenAuthenticator CreateServerSslX509TokenAuthenticator()
        {
            if (ClientCredentials.ServiceCertificate.SslCertificateAuthentication != null)
            {
                return new X509SecurityTokenAuthenticator(ClientCredentials.ServiceCertificate.SslCertificateAuthentication.GetCertificateValidator());
            }

            return CreateServerX509TokenAuthenticator();
        }

        public override SecurityTokenAuthenticator CreateSecurityTokenAuthenticator(SecurityTokenRequirement tokenRequirement, out SecurityTokenResolver outOfBandTokenResolver)
        {
            if (tokenRequirement == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenRequirement));
            }

            outOfBandTokenResolver = null;
            SecurityTokenAuthenticator result = null;

            InitiatorServiceModelSecurityTokenRequirement initiatorRequirement = tokenRequirement as InitiatorServiceModelSecurityTokenRequirement;
            if (initiatorRequirement != null)
            {
                string tokenType = initiatorRequirement.TokenType;
                if (IsIssuedSecurityTokenRequirement(initiatorRequirement))
                {
                    throw ExceptionHelper.PlatformNotSupported("CreateSecurityTokenAuthenticator : GenericXmlSecurityTokenAuthenticator");
                }
                else if (tokenType == SecurityTokenTypes.X509Certificate)
                {
                    if (initiatorRequirement.IsOutOfBandToken)
                    {
                        // when the client side soap security asks for a token authenticator, its for doing
                        // identity checks on the out of band server certificate
                        result = new X509SecurityTokenAuthenticator(X509CertificateValidator.None);
                    }
                    else if (initiatorRequirement.PreferSslCertificateAuthenticator)
                    {
                        result = CreateServerSslX509TokenAuthenticator();
                    }
                    else
                    {
                        result = CreateServerX509TokenAuthenticator();
                    }
                }
                else if (tokenType == SecurityTokenTypes.Rsa)
                {
                    throw ExceptionHelper.PlatformNotSupported("CreateSecurityTokenAuthenticator : SecurityTokenTypes.Rsa");
                }
                else if (tokenType == SecurityTokenTypes.Kerberos)
                {
                    throw ExceptionHelper.PlatformNotSupported("CreateSecurityTokenAuthenticator : SecurityTokenTypes.Kerberos");
                }
                else if (tokenType == ServiceModelSecurityTokenTypes.SecureConversation
                    || tokenType == ServiceModelSecurityTokenTypes.MutualSslnego
                    || tokenType == ServiceModelSecurityTokenTypes.AnonymousSslnego
                    || tokenType == ServiceModelSecurityTokenTypes.Spnego)
                {
                    throw ExceptionHelper.PlatformNotSupported("CreateSecurityTokenAuthenticator : GenericXmlSecurityTokenAuthenticator");
                }
            }
            else if ((tokenRequirement is RecipientServiceModelSecurityTokenRequirement) && tokenRequirement.TokenType == SecurityTokenTypes.X509Certificate)
            {
                // uncorrelated duplex case
                result = CreateServerX509TokenAuthenticator();
            }

            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.SecurityTokenManagerCannotCreateAuthenticatorForRequirement, tokenRequirement)));
            }

            return result;
        }
    }

    internal class KerberosSecurityTokenProviderWrapper : CommunicationObjectSecurityTokenProvider
    {
        private KerberosSecurityTokenProvider _innerProvider;

        public KerberosSecurityTokenProviderWrapper(KerberosSecurityTokenProvider innerProvider)
        {
            _innerProvider = innerProvider;
        }

        protected override SecurityToken GetTokenCore(TimeSpan timeout)
        {
            return new KerberosRequestorSecurityToken(_innerProvider.ServicePrincipalName,
                _innerProvider.TokenImpersonationLevel, _innerProvider.NetworkCredential,
                SecurityUniqueId.Create().Value);
        }

        internal Task<SecurityToken> GetTokenAsync(TimeSpan timeout, ChannelBinding channelbinding)
        {
            return Task.FromResult(GetTokenCore(timeout));
        }

        internal override Task<SecurityToken> GetTokenCoreInternalAsync(TimeSpan timeout)
        {
            return GetTokenAsync(timeout, null);
        }
    }
}
