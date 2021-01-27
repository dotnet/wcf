// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
