// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.ServiceModel;

namespace System.IdentityModel.Selectors
{
    public abstract class SecurityTokenAuthenticator
    {
        protected SecurityTokenAuthenticator() { }

        public bool CanValidateToken(SecurityToken token)
        {
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(token));
            }
            return CanValidateTokenCore(token);
        }

        public ReadOnlyCollection<IAuthorizationPolicy> ValidateToken(SecurityToken token)
        {
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(token));
            }
            if (!CanValidateToken(token))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenValidationException(SRP.Format(SRP.CannotValidateSecurityTokenType, this, token.GetType())));
            }

            EventTraceActivity eventTraceActivity = null;
            string tokenType = null;

            if (WcfEventSource.Instance.TokenValidationStartedIsEnabled())
            {
                eventTraceActivity = eventTraceActivity ?? EventTraceActivity.GetFromThreadOrCreate();
                tokenType = tokenType ?? token.GetType().ToString();
                WcfEventSource.Instance.TokenValidationStarted(eventTraceActivity, tokenType, token.Id);
            }

            ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies = ValidateTokenCore(token);
            if (authorizationPolicies == null)
            {
                string errorMsg = SRP.Format(SRP.CannotValidateSecurityTokenType, this, token.GetType());
                if (WcfEventSource.Instance.TokenValidationFailureIsEnabled())
                {
                    eventTraceActivity = eventTraceActivity ?? EventTraceActivity.GetFromThreadOrCreate();
                    tokenType = tokenType ?? token.GetType().ToString();
                    WcfEventSource.Instance.TokenValidationFailure(eventTraceActivity, tokenType, token.Id, errorMsg);
                }

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenValidationException(errorMsg));
            }

            if (WcfEventSource.Instance.TokenValidationSuccessIsEnabled())
            {
                eventTraceActivity = eventTraceActivity ?? EventTraceActivity.GetFromThreadOrCreate();
                tokenType = tokenType ?? token.GetType().ToString();
                WcfEventSource.Instance.TokenValidationSuccess(eventTraceActivity, tokenType, token.Id);
            }

            return authorizationPolicies;
        }

        protected abstract bool CanValidateTokenCore(SecurityToken token);
        protected abstract ReadOnlyCollection<IAuthorizationPolicy> ValidateTokenCore(SecurityToken token);
    }
}
