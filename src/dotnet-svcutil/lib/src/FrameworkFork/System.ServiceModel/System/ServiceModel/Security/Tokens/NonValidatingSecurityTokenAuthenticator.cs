// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections.ObjectModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IdentityModel.Policy;

namespace System.ServiceModel.Security.Tokens
{
    internal class NonValidatingSecurityTokenAuthenticator<TTokenType> : SecurityTokenAuthenticator
    {
        public NonValidatingSecurityTokenAuthenticator()
            : base()
        { }

        protected override bool CanValidateTokenCore(SecurityToken token)
        {
            return (token is TTokenType);
        }

        protected override ReadOnlyCollection<IAuthorizationPolicy> ValidateTokenCore(SecurityToken token)
        {
            return EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance;
        }
    }
}
