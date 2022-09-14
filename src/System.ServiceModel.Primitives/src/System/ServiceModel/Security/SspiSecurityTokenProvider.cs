// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Runtime;

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

        protected override SecurityToken GetTokenCore(TimeSpan timeout)
        {
            return _token;
        }

        protected override IAsyncResult BeginGetTokenCore(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CompletedAsyncResult<SecurityToken>(GetTokenCore(timeout), callback, state);
        }

        protected override SecurityToken EndGetTokenCore(IAsyncResult result)
        {
            return CompletedAsyncResult<SecurityToken>.End(result);
        }
    }
}
