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

        public IAsyncResult BeginGetToken(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        public SecurityToken EndGetToken(IAsyncResult result)
        {
            throw ExceptionHelper.PlatformNotSupported();
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

        internal protected class SecurityTokenAsyncResult : IAsyncResult
        {
            // $$$ private SecurityToken _token;
            private object _state;
            private ManualResetEvent _manualResetEvent;
            private object _thisLock = new object();

            public SecurityTokenAsyncResult(SecurityToken token, AsyncCallback callback, object state)
            {
                throw ExceptionHelper.PlatformNotSupported();

                // $$$
//                this.token = token;
//                this.state = state;

//                if (callback != null)
//                {
//                    try
//                    {
//                        callback(this);
//                    }
//#pragma warning suppress 56500
//                    catch (Exception e)
//                    {
//                        if (Fx.IsFatal(e))
//                            throw;

//                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(SR.Format(SR.AsyncCallbackException), e);
//                    }
//                }
            }

            public object AsyncState
            {
                get { return _state; }
            }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    if (_manualResetEvent != null)
                    {
                        return _manualResetEvent;
                    }

                    lock (_thisLock)
                    {
                        if (_manualResetEvent == null)
                        {
                            _manualResetEvent = new ManualResetEvent(true);
                        }
                    }
                    return _manualResetEvent;
                }
            }

            public bool CompletedSynchronously
            {
                get { return true; }
            }

            public bool IsCompleted
            {
                get { return true; }
            }

            public static SecurityToken End(IAsyncResult result)
            {
                throw ExceptionHelper.PlatformNotSupported();

                // $$$
                //if (result == null)
                //{
                //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("result");
                //}

                //SecurityTokenAsyncResult completedResult = result as SecurityTokenAsyncResult;
                //if (completedResult == null)
                //{
                //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.InvalidAsyncResult), "result"));
                //}

                //return completedResult.token;
            }
        }
    }
}
