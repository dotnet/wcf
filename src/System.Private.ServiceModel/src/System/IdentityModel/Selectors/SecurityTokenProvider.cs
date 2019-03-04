// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Tokens;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace System.IdentityModel.Selectors
{
    public abstract class SecurityTokenProvider
    {
        protected SecurityTokenProvider() { }

        public virtual bool SupportsTokenRenewal
        {
            get { return false; }
        }

        public virtual bool SupportsTokenCancellation
        {
            get { return false; }
        }

        public async Task<SecurityToken> GetTokenAsync(TimeSpan timeout)
        {
            SecurityToken token = await this.GetTokenCoreAsync(timeout);
            if (token == null)
            {
                throw Fx.Exception.AsError(new SecurityTokenException(SR.Format(SR.TokenProviderUnableToGetToken, this)));
            }

            return token;
        }

        public async Task<SecurityToken> RenewTokenAsync(TimeSpan timeout, SecurityToken tokenToBeRenewed)
        {
            if (tokenToBeRenewed == null)
            {
                throw Fx.Exception.ArgumentNull(nameof(tokenToBeRenewed));
            }

            SecurityToken token = await this.RenewTokenCoreAsync(timeout, tokenToBeRenewed);
            if (token == null)
            {
                throw Fx.Exception.AsError(new SecurityTokenException(SR.Format(SR.TokenProviderUnableToRenewToken, this)));
            }

            return token;
        }

        public async Task CancelTokenAsync(TimeSpan timeout, SecurityToken token)
        {
            if (token == null)
            {
                throw Fx.Exception.ArgumentNull(nameof(token));
            }

            await this.CancelTokenCoreAsync(timeout, token);
        }

        // protected methods
        protected abstract Task<SecurityToken> GetTokenCoreAsync(TimeSpan timeout);

        protected virtual Task<SecurityToken> RenewTokenCoreAsync(TimeSpan timeout, SecurityToken tokenToBeRenewed)
        {
            throw Fx.Exception.AsError(new NotSupportedException(SR.Format(SR.TokenRenewalNotSupported, this)));
        }

        protected virtual Task CancelTokenCoreAsync(TimeSpan timeout, SecurityToken token)
        {
            throw Fx.Exception.AsError(new NotSupportedException(SR.Format(SR.TokenCancellationNotSupported, this)));
        }
    }
}
