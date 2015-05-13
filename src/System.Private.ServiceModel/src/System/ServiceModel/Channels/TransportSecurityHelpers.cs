// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Channels
{
    internal static class HttpTransportSecurityHelpers
    {
        private static Dictionary<string, int> s_targetNameCounter = new Dictionary<string, int>();
    }


    internal static class TransportSecurityHelpers
    {
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
            if (!String.IsNullOrEmpty(relativeAddress))
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
