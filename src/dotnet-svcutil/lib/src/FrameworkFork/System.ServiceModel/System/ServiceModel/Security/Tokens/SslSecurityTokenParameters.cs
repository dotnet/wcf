// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Security.Tokens
{
    using System.IdentityModel.Selectors;
    using System.ServiceModel.Channels;
    using System.ServiceModel;
    using System.IdentityModel.Tokens;
    using System.ServiceModel.Security;
    using System.Text;
    using System.Globalization;

    public class SslSecurityTokenParameters : SecurityTokenParameters
    {
        internal const bool defaultRequireClientCertificate = false;
        internal const bool defaultRequireCancellation = false;

        private bool _requireCancellation = defaultRequireCancellation;
        private bool _requireClientCertificate;
        private BindingContext _issuerBindingContext;

        protected SslSecurityTokenParameters(SslSecurityTokenParameters other)
            : base(other)
        {
            _requireClientCertificate = other._requireClientCertificate;
            _requireCancellation = other._requireCancellation;
            if (other._issuerBindingContext != null)
            {
                _issuerBindingContext = other._issuerBindingContext.Clone();
            }
        }

        public SslSecurityTokenParameters()
            : this(defaultRequireClientCertificate)
        {
            // empty
        }

        public SslSecurityTokenParameters(bool requireClientCertificate)
            : this(requireClientCertificate, defaultRequireCancellation)
        {
            // empty
        }

        public SslSecurityTokenParameters(bool requireClientCertificate, bool requireCancellation)
            : base()
        {
            _requireClientCertificate = requireClientCertificate;
            _requireCancellation = requireCancellation;
        }

        internal protected override bool HasAsymmetricKey { get { return false; } }

        public bool RequireCancellation
        {
            get
            {
                return _requireCancellation;
            }
            set
            {
                _requireCancellation = value;
            }
        }

        public bool RequireClientCertificate
        {
            get
            {
                return _requireClientCertificate;
            }
            set
            {
                _requireClientCertificate = value;
            }
        }

        internal BindingContext IssuerBindingContext
        {
            get
            {
                return _issuerBindingContext;
            }
            set
            {
                if (value != null)
                {
                    value = value.Clone();
                }
                _issuerBindingContext = value;
            }
        }

        internal protected override bool SupportsClientAuthentication { get { return _requireClientCertificate; } }
        internal protected override bool SupportsServerAuthentication { get { return true; } }
        internal protected override bool SupportsClientWindowsIdentity { get { return _requireClientCertificate; } }

        protected override SecurityTokenParameters CloneCore()
        {
            return new SslSecurityTokenParameters(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "RequireCancellation: {0}", this.RequireCancellation.ToString()));
            sb.Append(String.Format(CultureInfo.InvariantCulture, "RequireClientCertificate: {0}", this.RequireClientCertificate.ToString()));

            return sb.ToString();
        }
    }
}
