// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel;
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

        public SecurityToken GetToken(TimeSpan timeout)
        {
            SecurityToken token = this.GetTokenCore(timeout);
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.TokenProviderUnableToGetToken, this)));
            }
            return token;
        }

        public async Task<SecurityToken> GetTokenAsync(CancellationToken cancellationToken)
        {
            SecurityToken token = await this.GetTokenCoreAsync(cancellationToken);
            if (token == null)
            {
                throw Fx.Exception.AsError(new SecurityTokenException(SR.Format(SR.TokenProviderUnableToGetToken, this)));
            }
            return token;
        }

        public async Task<SecurityToken> RenewTokenAsync(CancellationToken cancellationToken, SecurityToken tokenToBeRenewed)
        {
            if (tokenToBeRenewed == null)
            {
                throw Fx.Exception.ArgumentNull("tokenToBeRenewed");
            }
            SecurityToken token = await this.RenewTokenCoreAsync(cancellationToken, tokenToBeRenewed);
            if (token == null)
            {
                throw Fx.Exception.AsError(new SecurityTokenException(SR.Format(SR.TokenProviderUnableToRenewToken, this)));
            }
            return token;
        }

        public async Task CancelTokenAsync(CancellationToken cancellationToken, SecurityToken securityToken)
        {
            if (securityToken == null)
            {
                throw Fx.Exception.ArgumentNull("token");
            }
            await this.CancelTokenCoreAsync(cancellationToken, securityToken);
        }

        // protected methods
        protected abstract SecurityToken GetTokenCore(TimeSpan timeout);

        protected abstract Task<SecurityToken> GetTokenCoreAsync(CancellationToken cancellationToken);

        protected virtual Task<SecurityToken> RenewTokenCoreAsync(CancellationToken cancellationToken, SecurityToken tokenToBeRenewed)
        {
            throw Fx.Exception.AsError(new NotSupportedException(SR.Format(SR.TokenRenewalNotSupported, this)));
        }

        protected virtual Task CancelTokenCoreAsync(CancellationToken cancellationToken, SecurityToken token)
        {
            throw Fx.Exception.AsError(new NotSupportedException(SR.Format(SR.TokenCancellationNotSupported, this)));
        }
    }
}
