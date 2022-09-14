// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.ServiceModel;

namespace System.IdentityModel.Selectors
{
    public class X509SecurityTokenAuthenticator : SecurityTokenAuthenticator
    {
        private X509CertificateValidator _validator;
        private bool _cloneHandle;

        public X509SecurityTokenAuthenticator()
            : this(X509CertificateValidator.ChainTrust)
        {
        }

        public X509SecurityTokenAuthenticator(X509CertificateValidator validator)
        {
            _validator = validator ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(validator));
            _cloneHandle = true;
        }

        protected override bool CanValidateTokenCore(SecurityToken token)
        {
            return token is X509SecurityToken;
        }

        protected override ReadOnlyCollection<IAuthorizationPolicy> ValidateTokenCore(SecurityToken token)
        {
            X509SecurityToken x509Token = (X509SecurityToken)token;
            _validator.Validate(x509Token.Certificate);
            X509CertificateClaimSet x509ClaimSet = new X509CertificateClaimSet(x509Token.Certificate, _cloneHandle);
            return SecurityUtils.CreateAuthorizationPolicies(x509ClaimSet, x509Token.ValidTo);
        }
    }
}

