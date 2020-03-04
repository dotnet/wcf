// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace System.IdentityModel.Claims
{
    [DataContract(Namespace = XsiConstants.Namespace)]
    public class DefaultClaimSet : ClaimSet
    {
        [DataMember(Name = "Issuer")]
        private ClaimSet _issuer;
        [DataMember(Name = "Claims")]
        private IList<Claim> _claims;

        public DefaultClaimSet(params Claim[] claims)
        {
            Initialize(this, claims);
        }

        public DefaultClaimSet(IList<Claim> claims)
        {
            Initialize(this, claims);
        }

        public DefaultClaimSet(ClaimSet issuer, params Claim[] claims)
        {
            Initialize(issuer, claims);
        }

        public DefaultClaimSet(ClaimSet issuer, IList<Claim> claims)
        {
            Initialize(issuer, claims);
        }

        public override Claim this[int index]
        {
            get { return _claims[index]; }
        }

        public override int Count
        {
            get { return _claims.Count; }
        }

        public override ClaimSet Issuer
        {
            get { return _issuer; }
        }

        public override bool ContainsClaim(Claim claim)
        {
            if (claim == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("claim");

            for (int i = 0; i < _claims.Count; ++i)
            {
                if (claim.Equals(_claims[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public override IEnumerable<Claim> FindClaims(string claimType, string right)
        {
            bool anyClaimType = (claimType == null);
            bool anyRight = (right == null);

            for (int i = 0; i < _claims.Count; ++i)
            {
                Claim claim = _claims[i];
                if ((claim != null) &&
                    (anyClaimType || claimType == claim.ClaimType) &&
                    (anyRight || right == claim.Right))
                {
                    yield return claim;
                }
            }
        }

        public override IEnumerator<Claim> GetEnumerator()
        {
            return _claims.GetEnumerator();
        }

        protected void Initialize(ClaimSet issuer, IList<Claim> claims)
        {
            if (issuer == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("issuer");
            if (claims == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("claims");

            _issuer = issuer;
            _claims = claims;
        }

        public override string ToString()
        {
            return SecurityUtils.ClaimSetToString(this);
        }
    }
}
