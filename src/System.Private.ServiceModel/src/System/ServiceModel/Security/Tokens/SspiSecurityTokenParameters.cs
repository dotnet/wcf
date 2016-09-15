// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using System.Text;
using System.Globalization;

namespace System.ServiceModel.Security.Tokens
{
    public class SspiSecurityTokenParameters : SecurityTokenParameters
    {
        internal const bool defaultRequireCancellation = false;

        private bool _requireCancellation = defaultRequireCancellation;
        private BindingContext _issuerBindingContext;

        protected SspiSecurityTokenParameters(SspiSecurityTokenParameters other)
            : base(other)
        {
            _requireCancellation = other._requireCancellation;
            if (other._issuerBindingContext != null)
            {
                _issuerBindingContext = other._issuerBindingContext.Clone();
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

        internal protected override bool SupportsClientAuthentication { get { return true; } }
        internal protected override bool SupportsServerAuthentication { get { return true; } }
        internal protected override bool SupportsClientWindowsIdentity { get { return true; } }

        protected override SecurityTokenParameters CloneCore()
        {
            return new SspiSecurityTokenParameters(this);
        }

        internal protected override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
        {
            if (token is GenericXmlSecurityToken)
                return base.CreateGenericXmlTokenKeyIdentifierClause(token, referenceStyle);
            else
                return this.CreateKeyIdentifierClause<SecurityContextKeyIdentifierClause, LocalIdKeyIdentifierClause>(token, referenceStyle);
        }

        protected internal override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
        {
            requirement.TokenType = ServiceModelSecurityTokenTypes.Spnego;
            requirement.RequireCryptographicToken = true;
            requirement.KeyType = SecurityKeyType.SymmetricKey;
            requirement.Properties[ServiceModelSecurityTokenRequirement.SupportSecurityContextCancellationProperty] = this.RequireCancellation;
            if (this.IssuerBindingContext != null)
            {
                requirement.Properties[ServiceModelSecurityTokenRequirement.IssuerBindingContextProperty] = this.IssuerBindingContext.Clone();
            }
            requirement.Properties[ServiceModelSecurityTokenRequirement.IssuedSecurityTokenParametersProperty] = this.Clone();
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

