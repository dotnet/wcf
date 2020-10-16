// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal static class TransportSecurityHelpers
    {
        // used for HTTP (from HttpChannelUtilities.GetCredential)
        public static async Task<NetworkCredential> GetSspiCredentialAsync(SecurityTokenProviderContainer tokenProvider,
            OutWrapper<TokenImpersonationLevel> impersonationLevelWrapper, OutWrapper<AuthenticationLevel> authenticationLevelWrapper,
            CancellationToken cancellationToken)
        {
            OutWrapper<bool> dummyExtractWindowsGroupClaimsWrapper = new OutWrapper<bool>();
            OutWrapper<bool> allowNtlmWrapper = new OutWrapper<bool>();
            NetworkCredential result = await GetSspiCredentialAsync(tokenProvider.TokenProvider as SspiSecurityTokenProvider,
                dummyExtractWindowsGroupClaimsWrapper, impersonationLevelWrapper, allowNtlmWrapper, cancellationToken);
            authenticationLevelWrapper.Value = allowNtlmWrapper.Value ?
                AuthenticationLevel.MutualAuthRequested : AuthenticationLevel.MutualAuthRequired;
            return result;
        }

        // used by client WindowsStream security (from InitiateUpgrade)
        public static async Task<NetworkCredential> GetSspiCredentialAsync(SspiSecurityTokenProvider tokenProvider,
            OutWrapper<TokenImpersonationLevel> impersonationLevel, OutWrapper<bool> allowNtlm, CancellationToken cancellationToken)
        {
            OutWrapper<bool> dummyExtractWindowsGroupClaimsWrapper = new OutWrapper<bool>();
            return await GetSspiCredentialAsync(tokenProvider,
                dummyExtractWindowsGroupClaimsWrapper, impersonationLevel, allowNtlm, cancellationToken);
        }

        // used by server WindowsStream security (from Open)
        public static NetworkCredential GetSspiCredential(SecurityTokenManager credentialProvider,
            SecurityTokenRequirement sspiTokenRequirement, TimeSpan timeout,
            out bool extractGroupsForWindowsAccounts)
        {
            extractGroupsForWindowsAccounts = TransportDefaults.ExtractGroupsForWindowsAccounts;
            NetworkCredential result = null;

            if (credentialProvider != null)
            {
                SecurityTokenProvider tokenProvider = credentialProvider.CreateSecurityTokenProvider(sspiTokenRequirement);
                if (tokenProvider != null)
                {
                    TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                    SecurityUtils.OpenTokenProviderIfRequired(tokenProvider, timeoutHelper.RemainingTime());
                    bool success = false;
                    try
                    {
                        OutWrapper<TokenImpersonationLevel> dummyImpersonationLevelWrapper = new OutWrapper<TokenImpersonationLevel>();
                        OutWrapper<bool> dummyAllowNtlmWrapper = new OutWrapper<bool>();
                        OutWrapper<bool> extractGroupsForWindowsAccountsWrapper = new OutWrapper<bool>();
                        result = GetSspiCredentialAsync((SspiSecurityTokenProvider)tokenProvider, extractGroupsForWindowsAccountsWrapper,
                            dummyImpersonationLevelWrapper, dummyAllowNtlmWrapper, timeoutHelper.GetCancellationToken()).GetAwaiter().GetResult();

                        success = true;
                    }
                    finally
                    {
                        if (!success)
                        {
                            SecurityUtils.AbortTokenProviderIfRequired(tokenProvider);
                        }
                    }
                    SecurityUtils.CloseTokenProviderIfRequired(tokenProvider, timeoutHelper.RemainingTime());
                }
            }

            return result;
        }

        // core Cred lookup code
        public static async Task<NetworkCredential> GetSspiCredentialAsync(SspiSecurityTokenProvider tokenProvider,
            OutWrapper<bool> extractGroupsForWindowsAccounts,
            OutWrapper<TokenImpersonationLevel> impersonationLevelWrapper,
            OutWrapper<bool> allowNtlmWrapper,
            CancellationToken cancellationToken)
        {
            NetworkCredential credential = null;
            extractGroupsForWindowsAccounts.Value = TransportDefaults.ExtractGroupsForWindowsAccounts;
            impersonationLevelWrapper.Value = TokenImpersonationLevel.Identification;
            allowNtlmWrapper.Value = ConnectionOrientedTransportDefaults.AllowNtlm;

            if (tokenProvider != null)
            {
                SspiSecurityToken token = await TransportSecurityHelpers.GetTokenAsync<SspiSecurityToken>(tokenProvider, cancellationToken);
                if (token != null)
                {
                    extractGroupsForWindowsAccounts.Value = token.ExtractGroupsForWindowsAccounts;
                    impersonationLevelWrapper.Value = token.ImpersonationLevel;
                    allowNtlmWrapper.Value = token.AllowNtlm;
                    if (token.NetworkCredential != null)
                    {
                        credential = token.NetworkCredential;
                        SecurityUtils.FixNetworkCredential(ref credential);
                    }
                }
            }

            // Initialize to the default value if no token provided. A partial trust app should not have access to the
            // default network credentials but should be able to provide credentials. The DefaultNetworkCredentials
            // getter will throw under partial trust.
            if (credential == null)
            {
                credential = CredentialCache.DefaultNetworkCredentials;
            }

            return credential;
        }

        internal static SecurityTokenRequirement CreateSspiTokenRequirement(string transportScheme, Uri listenUri)
        {
            RecipientServiceModelSecurityTokenRequirement tokenRequirement = new RecipientServiceModelSecurityTokenRequirement();
            tokenRequirement.TransportScheme = transportScheme;
            tokenRequirement.RequireCryptographicToken = false;
            tokenRequirement.ListenUri = listenUri;
            tokenRequirement.TokenType = ServiceModelSecurityTokenTypes.SspiCredential;
            return tokenRequirement;
        }

        internal static SecurityTokenRequirement CreateSspiTokenRequirement(EndpointAddress target, Uri via, string transportScheme)
        {
            InitiatorServiceModelSecurityTokenRequirement sspiTokenRequirement = new InitiatorServiceModelSecurityTokenRequirement();
            sspiTokenRequirement.TokenType = ServiceModelSecurityTokenTypes.SspiCredential;
            sspiTokenRequirement.RequireCryptographicToken = false;
            sspiTokenRequirement.TransportScheme = transportScheme;
            sspiTokenRequirement.TargetAddress = target;
            sspiTokenRequirement.Via = via;
            return sspiTokenRequirement;
        }

        public static SspiSecurityTokenProvider GetSspiTokenProvider(
            SecurityTokenManager tokenManager, EndpointAddress target, Uri via, string transportScheme, AuthenticationSchemes authenticationScheme, ChannelParameterCollection channelParameters)
        {
            if (tokenManager != null)
            {
                SecurityTokenRequirement sspiRequirement = CreateSspiTokenRequirement(target, via, transportScheme);
                sspiRequirement.Properties[ServiceModelSecurityTokenRequirement.HttpAuthenticationSchemeProperty] = authenticationScheme;
                if (channelParameters != null)
                {
                    sspiRequirement.Properties[ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty] = channelParameters;
                }
                SspiSecurityTokenProvider tokenProvider = tokenManager.CreateSecurityTokenProvider(sspiRequirement) as SspiSecurityTokenProvider;
                return tokenProvider;
            }
            return null;
        }

        public static SspiSecurityTokenProvider GetSspiTokenProvider(
            SecurityTokenManager tokenManager, EndpointAddress target, Uri via, string transportScheme,
            out IdentityVerifier identityVerifier)
        {
            identityVerifier = null;
            if (tokenManager != null)
            {
                SspiSecurityTokenProvider tokenProvider =
                    tokenManager.CreateSecurityTokenProvider(CreateSspiTokenRequirement(target, via, transportScheme)) as SspiSecurityTokenProvider;

                if (tokenProvider != null)
                {
                    identityVerifier = IdentityVerifier.CreateDefault();
                }

                return tokenProvider;
            }
            return null;
        }

        public static SecurityTokenProvider GetDigestTokenProvider(
            SecurityTokenManager tokenManager, EndpointAddress target, Uri via,
            string transportScheme, AuthenticationSchemes authenticationScheme, ChannelParameterCollection channelParameters)
        {
            if (tokenManager != null)
            {
                InitiatorServiceModelSecurityTokenRequirement digestTokenRequirement =
                    new InitiatorServiceModelSecurityTokenRequirement();
                digestTokenRequirement.TokenType = ServiceModelSecurityTokenTypes.SspiCredential;
                digestTokenRequirement.TargetAddress = target;
                digestTokenRequirement.Via = via;
                digestTokenRequirement.RequireCryptographicToken = false;
                digestTokenRequirement.TransportScheme = transportScheme;
                digestTokenRequirement.Properties[ServiceModelSecurityTokenRequirement.HttpAuthenticationSchemeProperty] = authenticationScheme;
                if (channelParameters != null)
                {
                    digestTokenRequirement.Properties[ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty] = channelParameters;
                }
                return tokenManager.CreateSecurityTokenProvider(digestTokenRequirement) as SspiSecurityTokenProvider;
            }
            return null;
        }

        public static SecurityTokenProvider GetCertificateTokenProvider(
            SecurityTokenManager tokenManager, EndpointAddress target, Uri via, string transportScheme, ChannelParameterCollection channelParameters)
        {
            if (tokenManager != null)
            {
                InitiatorServiceModelSecurityTokenRequirement certificateTokenRequirement =
                    new InitiatorServiceModelSecurityTokenRequirement();
                certificateTokenRequirement.TokenType = SecurityTokenTypes.X509Certificate;
                certificateTokenRequirement.TargetAddress = target;
                certificateTokenRequirement.Via = via;
                certificateTokenRequirement.RequireCryptographicToken = false;
                certificateTokenRequirement.TransportScheme = transportScheme;
                if (channelParameters != null)
                {
                    certificateTokenRequirement.Properties[ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty] = channelParameters;
                }
                return tokenManager.CreateSecurityTokenProvider(certificateTokenRequirement);
            }
            return null;
        }

        private static async Task<T> GetTokenAsync<T>(SecurityTokenProvider tokenProvider, CancellationToken cancellationToken)
            where T : SecurityToken
        {
            SecurityToken result = await tokenProvider.GetTokenAsync(cancellationToken);
            if ((result != null) && !(result is T))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(
                    SRServiceModel.InvalidTokenProvided, tokenProvider.GetType(), typeof(T))));
            }
            return result as T;
        }

        public static async Task<NetworkCredential> GetUserNameCredentialAsync(SecurityTokenProviderContainer tokenProvider, CancellationToken cancellationToken)
        {
            NetworkCredential result = null;

            if (tokenProvider != null && tokenProvider.TokenProvider != null)
            {
                UserNameSecurityToken token = await GetTokenAsync<UserNameSecurityToken>(tokenProvider.TokenProvider, cancellationToken);
                if (token != null)
                {
                    result = new NetworkCredential(token.UserName, token.Password);
                }
            }

            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.NoUserNameTokenProvided));
            }

            return result;
        }

        public static SecurityTokenProvider GetUserNameTokenProvider(
            SecurityTokenManager tokenManager, EndpointAddress target, Uri via, string transportScheme, AuthenticationSchemes authenticationScheme,
            ChannelParameterCollection channelParameters)
        {
            SecurityTokenProvider result = null;
            if (tokenManager != null)
            {
                SecurityTokenRequirement usernameRequirement = CreateUserNameTokenRequirement(target, via, transportScheme);
                usernameRequirement.Properties[ServiceModelSecurityTokenRequirement.HttpAuthenticationSchemeProperty] = authenticationScheme;
                if (channelParameters != null)
                {
                    usernameRequirement.Properties[ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty] = channelParameters;
                }
                result = tokenManager.CreateSecurityTokenProvider(usernameRequirement);
            }
            return result;
        }

        public static Uri GetListenUri(Uri baseAddress, string relativeAddress)
        {
            Uri fullUri = baseAddress;

            // Ensure that baseAddress Path does end with a slash if we have a relative address
            if (!string.IsNullOrEmpty(relativeAddress))
            {
                if (!baseAddress.AbsolutePath.EndsWith("/", StringComparison.Ordinal))
                {
                    UriBuilder uriBuilder = new UriBuilder(baseAddress);
                    FixIpv6Hostname(uriBuilder, baseAddress);
                    uriBuilder.Path = uriBuilder.Path + "/";
                    baseAddress = uriBuilder.Uri;
                }

                fullUri = new Uri(baseAddress, relativeAddress);
            }

            return fullUri;
        }

        private static InitiatorServiceModelSecurityTokenRequirement CreateUserNameTokenRequirement(
            EndpointAddress target, Uri via, string transportScheme)
        {
            InitiatorServiceModelSecurityTokenRequirement usernameRequirement = new InitiatorServiceModelSecurityTokenRequirement();
            usernameRequirement.RequireCryptographicToken = false;
            usernameRequirement.TokenType = SecurityTokenTypes.UserName;
            usernameRequirement.TargetAddress = target;
            usernameRequirement.Via = via;
            usernameRequirement.TransportScheme = transportScheme;
            return usernameRequirement;
        }

        // Originally: TcpChannelListener.FixIpv6Hostname
        private static void FixIpv6Hostname(UriBuilder uriBuilder, Uri originalUri)
        {
            if (originalUri.HostNameType == UriHostNameType.IPv6)
            {
                string ipv6Host = originalUri.DnsSafeHost;
                uriBuilder.Host = string.Concat("[", ipv6Host, "]");
            }
        }
    }

    internal static class HttpTransportSecurityHelpers
    {
        private static Dictionary<string, int> s_targetNameCounter = new Dictionary<string, int>();

        public static bool AddIdentityMapping(Uri via, EndpointAddress target)
        {
            // On Desktop, we do mutual auth when the EndpointAddress has an identity. We need
            // support from HttpClient before any functionality can be added here. 
            return false;
        }

        public static void RemoveIdentityMapping(Uri via, EndpointAddress target, bool validateState)
        {
            // On Desktop, we do mutual auth when the EndpointAddress has an identity. We need
            // support from HttpClient before any functionality can be added here. 
        }

        private static Dictionary<HttpRequestMessage, string> s_serverCertMap = new Dictionary<HttpRequestMessage, string>();

        public static void AddServerCertMapping(HttpRequestMessage request, EndpointAddress to)
        {
            Fx.Assert(request.RequestUri.Scheme == UriEx.UriSchemeHttps,
                "Wrong URI scheme for AddServerCertMapping().");
            X509CertificateEndpointIdentity remoteCertificateIdentity = to.Identity as X509CertificateEndpointIdentity;
            if (remoteCertificateIdentity != null)
            {
                // The following condition should have been validated when the channel was created.
                Fx.Assert(remoteCertificateIdentity.Certificates.Count <= 1,
                    "HTTPS server certificate identity contains multiple certificates");
                AddServerCertMapping(request, remoteCertificateIdentity.Certificates[0].Thumbprint);
            }
        }

        private static void AddServerCertMapping(HttpRequestMessage request, string thumbprint)
        {
            lock (s_serverCertMap)
            {
                s_serverCertMap.Add(request, thumbprint);
            }
        }

        public static void SetServerCertificateValidationCallback(ServiceModelHttpMessageHandler handler)
        {
            if (!handler.SupportsClientCertificates)
            {
                throw ExceptionHelper.PlatformNotSupported("Server certificate validation not supported yet");
            }
            handler.ServerCertificateValidationCallback =
                ChainValidator(handler.ServerCertificateValidationCallback);
        }

        private static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ChainValidator(Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> previousValidator)
        {
            if (previousValidator == null)
            {
                return OnValidateServerCertificate;
            }

            Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> chained =
                (request, certificate, chain, sslPolicyErrors) =>
            {
                bool valid = OnValidateServerCertificate(request, certificate, chain, sslPolicyErrors);
                if (valid)
                {
                    return previousValidator(request, certificate, chain, sslPolicyErrors);
                }
                return false;
            };
            return chained;
        }

        private static bool OnValidateServerCertificate(HttpRequestMessage request, X509Certificate2 certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (request != null)
            {
                string thumbprint;
                lock (s_serverCertMap)
                {
                    s_serverCertMap.TryGetValue(request, out thumbprint);
                }
                if (thumbprint != null)
                {
                    try
                    {
                        ValidateServerCertificate(certificate, thumbprint);
                    }
                    catch (SecurityNegotiationException)
                    {
                        return false;
                    }
                }
            }

            return (sslPolicyErrors == SslPolicyErrors.None);
        }

        public static void RemoveServerCertMapping(HttpRequestMessage request)
        {
            lock (s_serverCertMap)
            {
                s_serverCertMap.Remove(request);
            }
        }

        private static void ValidateServerCertificate(X509Certificate2 certificate, string thumbprint)
        {
            string certHashString = certificate.Thumbprint;
            if (!thumbprint.Equals(certHashString))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new SecurityNegotiationException(string.Format(SRServiceModel.HttpsServerCertThumbprintMismatch,
                    certificate.Subject, certHashString, thumbprint)));
            }
        }
    }

    internal static class AuthenticationLevelHelper
    {
        internal static string ToString(AuthenticationLevel authenticationLevel)
        {
            if (authenticationLevel == AuthenticationLevel.MutualAuthRequested)
            {
                return "mutualAuthRequested";
            }
            if (authenticationLevel == AuthenticationLevel.MutualAuthRequired)
            {
                return "mutualAuthRequired";
            }
            if (authenticationLevel == AuthenticationLevel.None)
            {
                return "none";
            }

            Fx.Assert("unknown authentication level");
            return authenticationLevel.ToString();
        }
    }
}
