// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Claims;

namespace System.ServiceModel
{
    public class SpnEndpointIdentity : EndpointIdentity
    {
        private static TimeSpan s_spnLookupTime = TimeSpan.FromMinutes(1);

        public SpnEndpointIdentity(string spnName)
        {
            if (spnName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(spnName));
            }

            base.Initialize(Claim.CreateSpnClaim(spnName));
        }

        public SpnEndpointIdentity(Claim identity)
        {
            if (identity == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(identity));
            }

            if (!identity.ClaimType.Equals(ClaimTypes.Spn))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.UnrecognizedClaimTypeForIdentity, identity.ClaimType, ClaimTypes.Spn));
            }

            base.Initialize(identity);
        }

        public static TimeSpan SpnLookupTime
        {
            get
            {
                return s_spnLookupTime;
            }
            set
            {
                if (value.Ticks < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new ArgumentOutOfRangeException(nameof(value), value.Ticks, SRP.Format(SRP.ValueMustBeNonNegative)));
                }
                s_spnLookupTime = value;
            }
        }
    }
}
