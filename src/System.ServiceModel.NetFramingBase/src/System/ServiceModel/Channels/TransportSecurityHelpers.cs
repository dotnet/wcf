// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Principal;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal static class WindowsStreamTransportSecurityHelpers
    {
        //// used for HTTP (from HttpChannelUtilities.GetCredential)
        //public static async ValueTask<(NetworkCredential credential, TokenImpersonationLevel impersonationLevel, AuthenticationLevel authenticationLevel)> GetSspiCredentialAsync(
        //    SecurityTokenProviderContainer tokenProvider, TimeSpan timeout)
        //{
        //    var result = await GetSspiCredentialCoreAsync(tokenProvider.TokenProvider as SspiSecurityTokenProvider, timeout);
        //    AuthenticationLevel authenticationLevel = result.allowNtlm ? AuthenticationLevel.MutualAuthRequested : AuthenticationLevel.MutualAuthRequired;
        //    return (result.credential, result.impersonationLevel, authenticationLevel);
        //}

        // used by client WindowsStream security (from InitiateUpgrade)
        public static async ValueTask<(NetworkCredential credential, TokenImpersonationLevel impersonationLevel, bool allowNtlm)> GetSspiCredentialAsync(SecurityTokenProvider tokenProvider, TimeSpan timeout)
        {
            var result = await GetSspiCredentialCoreAsync(tokenProvider, timeout);
            return (result.credential, result.impersonationLevel, result.allowNtlm);
        }

        // core Cred lookup code
        public static async ValueTask<(NetworkCredential credential, bool extractGroupsForWindowsAccounts, TokenImpersonationLevel impersonationLevel, bool allowNtlm)> GetSspiCredentialCoreAsync(
            SecurityTokenProvider tokenProvider, TimeSpan timeout)
        {
            (NetworkCredential credential,
             bool extractGroupsForWindowsAccounts,
             TokenImpersonationLevel impersonationLevel,
             bool allowNtlm) result =
             (null, NFTransportDefaults.ExtractGroupsForWindowsAccounts, TokenImpersonationLevel.Identification, ConnectionOrientedTransportDefaults.AllowNtlm);

            if (tokenProvider != null)
            {
                SspiSecurityToken token = await WindowsStreamTransportSecurityHelpers.GetTokenAsync<SspiSecurityToken>(tokenProvider, timeout);
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

        public static SecurityTokenProvider GetSspiTokenProvider(
            SecurityTokenManager tokenManager, EndpointAddress target, Uri via, string transportScheme)
        {
            return tokenManager?.CreateSecurityTokenProvider(CreateSspiTokenRequirement(target, via, transportScheme));
        }

        private static async Task<T> GetTokenAsync<T>(SecurityTokenProvider tokenProvider, TimeSpan timeout)
            where T : SecurityToken
        {
            SecurityToken result = await tokenProvider.GetTokenAsync(timeout);
            if ((result != null) && !(result is T))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(
                    SR.InvalidTokenProvided, tokenProvider.GetType(), typeof(T))));
            }
            return result as T;
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
}
