// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Tokens;
using System.Runtime;
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
            SecurityToken token = await GetTokenCoreAsync(timeout);
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

            SecurityToken token = await RenewTokenCoreAsync(timeout, tokenToBeRenewed);
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

            await CancelTokenCoreAsync(timeout, token);
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

        // TODO: This class needs to be reworked to make these methods cooperate with the Task based methods. They are here without implementation to give
        // the contracts something to type forward to.
        public SecurityToken GetToken(TimeSpan timeout) { return default(SecurityToken); }
        public IAsyncResult BeginGetToken(TimeSpan timeout, AsyncCallback callback, object state) { return default(IAsyncResult); }
        public SecurityToken EndGetToken(IAsyncResult result) { return default(SecurityToken); }
        public SecurityToken RenewToken(TimeSpan timeout, SecurityToken tokenToBeRenewed) { return default(SecurityToken); }
        public IAsyncResult BeginRenewToken(TimeSpan timeout, SecurityToken tokenToBeRenewed, AsyncCallback callback, object state) { return default(IAsyncResult); }
        public SecurityToken EndRenewToken(IAsyncResult result) { return default(SecurityToken); }
        public void CancelToken(TimeSpan timeout, SecurityToken token) { }
        public IAsyncResult BeginCancelToken(TimeSpan timeout, SecurityToken token, AsyncCallback callback, object state) { return default(IAsyncResult); }
        public void EndCancelToken(IAsyncResult result) { }
        protected abstract SecurityToken GetTokenCore(TimeSpan timeout);
        protected virtual SecurityToken RenewTokenCore(TimeSpan timeout, SecurityToken tokenToBeRenewed) { return default(SecurityToken); }
        protected virtual void CancelTokenCore(TimeSpan timeout, SecurityToken token) { }
        protected virtual IAsyncResult BeginGetTokenCore(TimeSpan timeout, AsyncCallback callback, object state) { return default(IAsyncResult); }
        protected virtual SecurityToken EndGetTokenCore(IAsyncResult result) { return default(SecurityToken); }
        protected virtual IAsyncResult BeginRenewTokenCore(TimeSpan timeout, SecurityToken tokenToBeRenewed, AsyncCallback callback, object state) { return default(IAsyncResult); }
        protected virtual SecurityToken EndRenewTokenCore(IAsyncResult result) { return default(SecurityToken); }
        protected virtual IAsyncResult BeginCancelTokenCore(TimeSpan timeout, SecurityToken token, AsyncCallback callback, object state) { return default(IAsyncResult); }
        protected virtual void EndCancelTokenCore(IAsyncResult result) { }
    }
}
