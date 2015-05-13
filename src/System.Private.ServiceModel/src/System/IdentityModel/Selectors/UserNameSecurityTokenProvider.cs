// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IdentityModel.Tokens;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace System.IdentityModel.Selectors
{
    public class UserNameSecurityTokenProvider : SecurityTokenProvider
    {
        private UserNameSecurityToken _userNameToken;

        public UserNameSecurityTokenProvider(string userName, string password)
        {
            if (userName == null)
            {
                throw Fx.Exception.ArgumentNull("userName");
            }

            _userNameToken = new UserNameSecurityToken(userName, password);
        }

        protected override Task<SecurityToken> GetTokenCoreAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<SecurityToken>(_userNameToken);
        }
    }
}
