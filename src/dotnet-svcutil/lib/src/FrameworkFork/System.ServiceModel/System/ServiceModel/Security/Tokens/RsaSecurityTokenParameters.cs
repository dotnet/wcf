// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Security.Tokens
{
    using System.ServiceModel.Security;
    using System.ServiceModel;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;

    public class RsaSecurityTokenParameters : SecurityTokenParameters
    {
        protected RsaSecurityTokenParameters(RsaSecurityTokenParameters other)
            : base(other)
        {
            this.InclusionMode = SecurityTokenInclusionMode.Never;
        }

        public RsaSecurityTokenParameters()
            : base()
        {
            this.InclusionMode = SecurityTokenInclusionMode.Never;
        }

        internal protected override bool HasAsymmetricKey { get { return true; } }

        internal protected override bool SupportsClientAuthentication { get { return true; } }
        internal protected override bool SupportsServerAuthentication { get { return true; } }
        internal protected override bool SupportsClientWindowsIdentity { get { return false; } }

        protected override SecurityTokenParameters CloneCore()
        {
            return new RsaSecurityTokenParameters(this);
        }
    }
}
