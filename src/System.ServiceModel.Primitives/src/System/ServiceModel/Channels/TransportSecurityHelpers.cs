// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net;
using System.Net.Security;
using System.Runtime;
using System.Security.Principal;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal static class TransportSecurityHelpers
    {
        // used for HTTP (from HttpChannelUtilities.GetCredential)
        public static async ValueTask<(NetworkCredential credential, TokenImpersonationLevel impersonationLevel, AuthenticationLevel authenticationLevel)> GetSspiCredentialAsync(
            SecurityTokenProviderContainer tokenProvider, TimeSpan timeout)
        {
            var result = await GetSspiCredentialCoreAsync(tokenProvider.TokenProvider as SspiSecurityTokenProvider, timeout);
            AuthenticationLevel authenticationLevel =  result.allowNtlm ? AuthenticationLevel.MutualAuthRequested : AuthenticationLevel.MutualAuthRequired;
            return (result.credential, result.impersonationLevel, authenticationLevel);
        }

        // core Cred lookup code
        public static async ValueTask<(NetworkCredential credential, bool extractGroupsForWindowsAccounts, TokenImpersonationLevel impersonationLevel, bool allowNtlm)> GetSspiCredentialCoreAsync(
            SspiSecurityTokenProvider tokenProvider, TimeSpan timeout)
        {
            (NetworkCredential credential,
             bool extractGroupsForWindowsAccounts,
             TokenImpersonationLevel impersonationLevel,
             bool allowNtlm) result =
             (null, TransportDefaults.ExtractGroupsForWindowsAccounts, TokenImpersonationLevel.Identification, SspiSecurityTokenProvider.DefaultAllowNtlm);

            if (tokenProvider != null)
            {
                SspiSecurityToken token = await TransportSecurityHelpers.GetTokenAsync<SspiSecurityToken>(tokenProvider, timeout);
                if (token != null)
                {
                    result.extractGroupsForWindowsAccounts = token.ExtractGroupsForWindowsAccounts;
                    result.impersonationLevel = token.ImpersonationLevel;
                    result.allowNtlm = token.AllowNtlm;
                    if (token.NetworkCredential != null)
                    {
                        result.credential = token.NetworkCredential;
                        SecurityUtils.FixNetworkCredential(ref result.credential);
                    }
                }
            }

            // Initialize to the default value if no token provided. A partial trust app should not have access to the
            // default network credentials but should be able to provide credentials. The DefaultNetworkCredentials
            // getter will throw under partial trust.
            if (result.credential == null)
            {
                result.credential = CredentialCache.DefaultNetworkCredentials;
            }

            return result;
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

        private static async Task<T> GetTokenAsync<T>(SecurityTokenProvider tokenProvider, TimeSpan timeout)
            where T : SecurityToken
        {
            SecurityToken result = await tokenProvider.GetTokenAsync(timeout);
            if ((result != null) && !(result is T))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(
                    SRP.InvalidTokenProvided, tokenProvider.GetType(), typeof(T))));
            }
            return result as T;
        }

        public static async Task<NetworkCredential> GetUserNameCredentialAsync(SecurityTokenProviderContainer tokenProvider, TimeSpan timeout)
        {
            NetworkCredential result = null;

            if (tokenProvider != null && tokenProvider.TokenProvider != null)
            {
                UserNameSecurityToken token = await GetTokenAsync<UserNameSecurityToken>(tokenProvider.TokenProvider, timeout);
                if (token != null)
                {
                    result = new NetworkCredential(token.UserName, token.Password);
                }
            }

            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.NoUserNameTokenProvided));
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
