// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Security.Tokens
{
    using System.IdentityModel.Tokens;
    using System.ServiceModel;
    using System.IdentityModel.Selectors;
    using System.ServiceModel.Security;
    using System.Text;
    using System.Globalization;

    public class X509SecurityTokenParameters : SecurityTokenParameters
    {
        internal const X509KeyIdentifierClauseType defaultX509ReferenceStyle = X509KeyIdentifierClauseType.Any;

        private X509KeyIdentifierClauseType _x509ReferenceStyle;

        protected X509SecurityTokenParameters(X509SecurityTokenParameters other)
            : base(other)
        {
            _x509ReferenceStyle = other._x509ReferenceStyle;
        }

        public X509SecurityTokenParameters()
            : this(X509SecurityTokenParameters.defaultX509ReferenceStyle, SecurityTokenParameters.defaultInclusionMode)
        {
            // empty
        }

        public X509SecurityTokenParameters(X509KeyIdentifierClauseType x509ReferenceStyle)
            : this(x509ReferenceStyle, SecurityTokenParameters.defaultInclusionMode)
        {
            // empty
        }

        public X509SecurityTokenParameters(X509KeyIdentifierClauseType x509ReferenceStyle, SecurityTokenInclusionMode inclusionMode)
            : this(x509ReferenceStyle, inclusionMode, SecurityTokenParameters.defaultRequireDerivedKeys)
        {
        }

        internal X509SecurityTokenParameters(X509KeyIdentifierClauseType x509ReferenceStyle, SecurityTokenInclusionMode inclusionMode,
            bool requireDerivedKeys)
            : base()
        {
            this.X509ReferenceStyle = x509ReferenceStyle;
            this.InclusionMode = inclusionMode;
            this.RequireDerivedKeys = requireDerivedKeys;
        }

        internal protected override bool HasAsymmetricKey { get { return true; } }

        public X509KeyIdentifierClauseType X509ReferenceStyle
        {
            get
            {
                return _x509ReferenceStyle;
            }
            set
            {
                X509SecurityTokenReferenceStyleHelper.Validate(value);
                _x509ReferenceStyle = value;
            }
        }

        internal protected override bool SupportsClientAuthentication { get { return true; } }
        internal protected override bool SupportsServerAuthentication { get { return true; } }
        internal protected override bool SupportsClientWindowsIdentity { get { return true; } }

        protected override SecurityTokenParameters CloneCore()
        {
            return new X509SecurityTokenParameters(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());

            sb.Append(String.Format(CultureInfo.InvariantCulture, "X509ReferenceStyle: {0}", _x509ReferenceStyle.ToString()));

            return sb.ToString();
        }
    }
}
