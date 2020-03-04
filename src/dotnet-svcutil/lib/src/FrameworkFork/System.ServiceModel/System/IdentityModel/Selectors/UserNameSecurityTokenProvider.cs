// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace System.IdentityModel.Selectors
{
    public class UserNameSecurityTokenProvider : SecurityTokenProvider
    {
        readonly UserNameSecurityToken _userNameToken;

        public UserNameSecurityTokenProvider(string userName, string password)
        {
            if (userName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("userName");
            }

            _userNameToken = new UserNameSecurityToken(userName, password);
        }

        protected override Task<SecurityToken> GetTokenCoreAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((SecurityToken)_userNameToken);
        }
    }
}
