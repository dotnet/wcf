// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;

namespace System.ServiceModel.Security
{
    public class SecurityTokenSpecification
    {
        private SecurityToken _token;
        private ReadOnlyCollection<IAuthorizationPolicy> _tokenPolicies;

        public SecurityTokenSpecification(SecurityToken token, ReadOnlyCollection<IAuthorizationPolicy> tokenPolicies)
        {
            _token = token;
            if (tokenPolicies == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenPolicies");
            }
            _tokenPolicies = tokenPolicies;
        }

        public SecurityToken SecurityToken
        {
            get { return _token; }
        }

        public ReadOnlyCollection<IAuthorizationPolicy> SecurityTokenPolicies
        {
            get { return _tokenPolicies; }
        }
    }
}
