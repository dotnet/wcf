// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    using System;

    using System.IdentityModel.Claims;
    using Microsoft.Xml;

    public class SpnEndpointIdentity : EndpointIdentity
    {
        private static TimeSpan s_spnLookupTime = TimeSpan.FromMinutes(1);

        // Double-checked locking pattern requires volatile for read/write synchronization
        private volatile bool _hasSpnSidBeenComputed;

        private Object _thisLock = new Object();

        private static Object s_typeLock = new Object();

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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value.Ticks, SRServiceModel.ValueMustBeNonNegative));
                }
                s_spnLookupTime = value;
            }
        }

        public SpnEndpointIdentity(string spnName)
        {
            if (spnName == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("spnName");

            base.Initialize(Claim.CreateSpnClaim(spnName));
        }

        public SpnEndpointIdentity(Claim identity)
        {
            if (identity == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("identity");

            // PreSharp Bug: Parameter 'identity.ResourceType' to this public method must be validated: A null-dereference can occur here.
#pragma warning disable 56506 // Claim.ClaimType will never return null
            if (!identity.ClaimType.Equals(ClaimTypes.Spn))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(string.Format(SRServiceModel.UnrecognizedClaimTypeForIdentity, identity.ClaimType, ClaimTypes.Spn));

            base.Initialize(identity);
        }

        internal override void WriteContentsTo(XmlDictionaryWriter writer)
        {
            if (writer == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");

            writer.WriteElementString(XD.AddressingDictionary.Spn, XD.AddressingDictionary.IdentityExtensionNamespace, (string)this.IdentityClaim.Resource);
        }
    }
}
