// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel;
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

        #region GetToken
        public SecurityToken GetToken(TimeSpan timeout)
        {
            SecurityToken token = GetTokenCore(timeout);
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SRP.Format(SRP.TokenProviderUnableToGetToken, this)));
            }

            return token;
        }

        public IAsyncResult BeginGetToken(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return BeginGetTokenCore(timeout, callback, state);
        }

        public SecurityToken EndGetToken(IAsyncResult result)
        {
            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(result));
            }

            SecurityToken token = EndGetTokenCore(result);
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SRP.Format(SRP.TokenProviderUnableToGetToken, this)));
            }

            return token;
        }

        public async Task<SecurityToken> GetTokenAsync(TimeSpan timeout)
        {
            SecurityToken token = await GetTokenCoreAsync(timeout);
            if (token == null)
            {
                throw Fx.Exception.AsError(new SecurityTokenException(SRP.Format(SRP.TokenProviderUnableToGetToken, this)));
            }

            return token;
        }

        protected abstract SecurityToken GetTokenCore(TimeSpan timeout);

        protected virtual IAsyncResult BeginGetTokenCore(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return GetTokenCoreInternalAsync(timeout).ToApm(callback, state);
        }

        protected virtual SecurityToken EndGetTokenCore(IAsyncResult result)
        {
            return result.ToApmEnd<SecurityToken>();
        }


        protected virtual Task<SecurityToken> GetTokenCoreAsync(TimeSpan timeout)
        {
            return Task<SecurityToken>.Factory.FromAsync(BeginGetTokenCore, EndGetTokenCore, timeout, null);
        }

        // If external concrete implementation overrides GetTokenCoreAsync and calls base.GetTokenCoreAsync, this will call into base class implementation in GetTokenCoreInternalAsync.
        // This pattern prevents a cycle of GetTokenCoreAsync wrapping {Begin|End}GetTokenCore and {Begin|End}GetTokenCore wrapping GetTokenCoreAsync.
        internal virtual Task<SecurityToken> GetTokenCoreInternalAsync(TimeSpan timeout)
        {
            return Task.FromResult(GetTokenCore(timeout));
        }
        #endregion // GetToken

        #region RenewToken
        public SecurityToken RenewToken(TimeSpan timeout, SecurityToken tokenToBeRenewed)
        {
            if (tokenToBeRenewed == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenToBeRenewed));
            }

            SecurityToken token = RenewTokenCore(timeout, tokenToBeRenewed);
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SRP.Format(SRP.TokenProviderUnableToRenewToken, this)));
            }

            return token;
        }

        public IAsyncResult BeginRenewToken(TimeSpan timeout, SecurityToken tokenToBeRenewed, AsyncCallback callback, object state)
        {
            if (tokenToBeRenewed == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenToBeRenewed));
            }

            return BeginRenewTokenCore(timeout, tokenToBeRenewed, callback, state);
        }

        public SecurityToken EndRenewToken(IAsyncResult result)
        {
            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(result));
            }

            SecurityToken token = EndRenewTokenCore(result);
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SRP.Format(SRP.TokenProviderUnableToRenewToken, this)));
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
                throw Fx.Exception.AsError(new SecurityTokenException(SRP.Format(SRP.TokenProviderUnableToRenewToken, this)));
            }

            return token;
        }

        protected virtual SecurityToken RenewTokenCore(TimeSpan timeout, SecurityToken tokenToBeRenewed)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.TokenRenewalNotSupported, this)));
        }

        protected virtual IAsyncResult BeginRenewTokenCore(TimeSpan timeout, SecurityToken tokenToBeRenewed, AsyncCallback callback, object state)
        {
            return RenewTokenCoreInternalAsync(timeout, tokenToBeRenewed).ToApm(callback, state);
        }

        protected virtual SecurityToken EndRenewTokenCore(IAsyncResult result)
        {
            return result.ToApmEnd<SecurityToken>();
        }

        protected virtual Task<SecurityToken> RenewTokenCoreAsync(TimeSpan timeout, SecurityToken tokenToBeRenewed)
        {
            return Task<SecurityToken>.Factory.FromAsync(BeginRenewTokenCore, EndRenewTokenCore, timeout, tokenToBeRenewed, null);
        }

        internal virtual Task<SecurityToken> RenewTokenCoreInternalAsync(TimeSpan timeout, SecurityToken tokenToBeRenewed)
        {
            return Task.FromResult(RenewTokenCore(timeout, tokenToBeRenewed));
        }
        #endregion // RenewToken

        #region CancelToken
        public void CancelToken(TimeSpan timeout, SecurityToken token)
        {
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(token));
            }

            CancelTokenCore(timeout, token);
        }

        public IAsyncResult BeginCancelToken(TimeSpan timeout, SecurityToken token, AsyncCallback callback, object state)
        {
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(token));
            }

            return BeginCancelTokenCore(timeout, token, callback, state);
        }

        public void EndCancelToken(IAsyncResult result)
        {
            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(result));
            }

            EndCancelTokenCore(result);
        }

        public async Task CancelTokenAsync(TimeSpan timeout, SecurityToken token)
        {
            if (token == null)
            {
                throw Fx.Exception.ArgumentNull(nameof(token));
            }

            await CancelTokenCoreAsync(timeout, token);
        }

        protected virtual void CancelTokenCore(TimeSpan timeout, SecurityToken token)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.TokenCancellationNotSupported, this)));
        }

        protected virtual IAsyncResult BeginCancelTokenCore(TimeSpan timeout, SecurityToken token, AsyncCallback callback, object state)
        {
            return CancelTokenCoreInternalAsync(timeout, token).ToApm(callback, state);
        }

        protected virtual void EndCancelTokenCore(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected virtual Task CancelTokenCoreAsync(TimeSpan timeout, SecurityToken token)
        {
            return Task.Factory.FromAsync(BeginCancelTokenCore, EndCancelTokenCore, timeout, token, null);
        }

        internal virtual Task CancelTokenCoreInternalAsync(TimeSpan timeout, SecurityToken token)
        {
            CancelTokenCore(timeout, token);
            return Task.CompletedTask;
        }
        #endregion // CancelToken
    }
}
