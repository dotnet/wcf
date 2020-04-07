// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.


namespace System.ServiceModel.Security.Tokens
{
    using System.IdentityModel.Selectors;
    using System.ServiceModel.Channels;
    using System.ServiceModel;
    using System.IdentityModel.Tokens;
    using System.ServiceModel.Security;
    using System.Text;
    using System.Globalization;

    public class SspiSecurityTokenParameters : SecurityTokenParameters
    {
        internal const bool defaultRequireCancellation = false;

        bool requireCancellation = defaultRequireCancellation;
        BindingContext issuerBindingContext;


        protected SspiSecurityTokenParameters(SspiSecurityTokenParameters other)
            : base(other)
        {
            this.requireCancellation = other.requireCancellation;
            if (other.issuerBindingContext != null)
            {
                this.issuerBindingContext = other.issuerBindingContext.Clone();
            }
        }


        public SspiSecurityTokenParameters()
            : this(defaultRequireCancellation)
        {
            // empty
        }

        public SspiSecurityTokenParameters(bool requireCancellation)
            : base()
        {
            this.requireCancellation = requireCancellation;
        }

        internal protected override bool HasAsymmetricKey { get { return false; } }

        public bool RequireCancellation
        {
            get
            {
                return this.requireCancellation;
            }
            set
            {
                this.requireCancellation = value;
            }
        }

        internal BindingContext IssuerBindingContext
        {
            get
            {
                return this.issuerBindingContext;
            }
            set
            {
                if (value != null)
                {
                    value = value.Clone();
                }
                this.issuerBindingContext = value;
            }
        }

        internal protected override bool SupportsClientAuthentication { get { return true; } }
        internal protected override bool SupportsServerAuthentication { get { return true; } }
        internal protected override bool SupportsClientWindowsIdentity { get { return true; } }

        protected override SecurityTokenParameters CloneCore()
        {
            return new SspiSecurityTokenParameters(this);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());

            sb.Append(String.Format(CultureInfo.InvariantCulture, "RequireCancellation: {0}", this.RequireCancellation.ToString()));

            return sb.ToString();
        }
    }
}
