// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Security
{
    public class SspiSecurityTokenProvider : SecurityTokenProvider
    {
        internal const bool DefaultAllowNtlm = true;
        internal const bool DefaultExtractWindowsGroupClaims = true;
        internal const bool DefaultAllowUnauthenticatedCallers = false;
        private readonly SspiSecurityToken _token;

        // client side ctor
        public SspiSecurityTokenProvider(NetworkCredential credential, bool allowNtlm, TokenImpersonationLevel impersonationLevel)
        {
            _token = new SspiSecurityToken(impersonationLevel, allowNtlm, credential);
        }

        // service side ctor
        public SspiSecurityTokenProvider(NetworkCredential credential, bool extractGroupsForWindowsAccounts, bool allowUnauthenticatedCallers)
        {
            _token = new SspiSecurityToken(credential, extractGroupsForWindowsAccounts, allowUnauthenticatedCallers);
        }

        protected override Task<SecurityToken> GetTokenCoreAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<SecurityToken>(_token);
        }
    }
}
