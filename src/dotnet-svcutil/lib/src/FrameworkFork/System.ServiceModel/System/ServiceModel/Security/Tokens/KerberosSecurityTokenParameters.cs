// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Security.Tokens
{
    using System.ServiceModel.Security;
    using System.ServiceModel;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;

    public class KerberosSecurityTokenParameters : SecurityTokenParameters
    {
        protected KerberosSecurityTokenParameters(KerberosSecurityTokenParameters other)
            : base(other)
        {
            // empty
        }

        public KerberosSecurityTokenParameters()
            : base()
        {
            this.InclusionMode = SecurityTokenInclusionMode.Once;
        }

        internal protected override bool HasAsymmetricKey { get { return false; } }
        internal protected override bool SupportsClientAuthentication { get { return true; } }
        internal protected override bool SupportsServerAuthentication { get { return true; } }
        internal protected override bool SupportsClientWindowsIdentity { get { return true; } }

        protected override SecurityTokenParameters CloneCore()
        {
            return new KerberosSecurityTokenParameters(this);
        }
    }
}
