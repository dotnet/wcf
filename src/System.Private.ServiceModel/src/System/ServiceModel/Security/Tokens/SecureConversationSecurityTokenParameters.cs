// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Globalization;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using System.Text;

namespace System.ServiceModel.Security.Tokens
{
    public class SecureConversationSecurityTokenParameters : SecurityTokenParameters
    {
        internal const bool defaultRequireCancellation = true;
        internal const bool defaultCanRenewSession = true;

        private SecurityBindingElement _bootstrapSecurityBindingElement;
        private bool _requireCancellation;
        private bool _canRenewSession = defaultCanRenewSession;
        private BindingContext _issuerBindingContext;

        protected SecureConversationSecurityTokenParameters(SecureConversationSecurityTokenParameters other)
            : base(other)
        {
            _requireCancellation = other._requireCancellation;
            _canRenewSession = other._canRenewSession;
            if (other._bootstrapSecurityBindingElement != null)
                _bootstrapSecurityBindingElement = (SecurityBindingElement)other._bootstrapSecurityBindingElement.Clone();

            if (other._issuerBindingContext != null)
                _issuerBindingContext = other._issuerBindingContext.Clone();
        }

        public SecureConversationSecurityTokenParameters()
        {
            // empty
        }

        public SecureConversationSecurityTokenParameters(SecurityBindingElement bootstrapSecurityBindingElement)
        {
            _bootstrapSecurityBindingElement = bootstrapSecurityBindingElement;
        }

        internal protected override bool HasAsymmetricKey { get { return false; } }

        public SecurityBindingElement BootstrapSecurityBindingElement
        {
            get
            {
                return _bootstrapSecurityBindingElement;
            }
            set
            {
                _bootstrapSecurityBindingElement = value;
            }
        }

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

        private ISecurityCapabilities BootstrapSecurityCapabilities
        {
            get
            {
                return _bootstrapSecurityBindingElement.GetIndividualProperty<ISecurityCapabilities>();
            }
        }

        internal protected override bool SupportsClientAuthentication
        {
            get
            {
                return this.BootstrapSecurityCapabilities == null ? false : this.BootstrapSecurityCapabilities.SupportsClientAuthentication;
            }
        }

        internal protected override bool SupportsServerAuthentication
        {
            get
            {
                return this.BootstrapSecurityCapabilities == null ? false : this.BootstrapSecurityCapabilities.SupportsServerAuthentication;
            }
        }

        internal protected override bool SupportsClientWindowsIdentity
        {
            get
            {
                return this.BootstrapSecurityCapabilities == null ? false : this.BootstrapSecurityCapabilities.SupportsClientWindowsIdentity;
            }
        }

        protected override SecurityTokenParameters CloneCore()
        {
            return new SecureConversationSecurityTokenParameters(this);
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
            requirement.TokenType = ServiceModelSecurityTokenTypes.SecureConversation;
            requirement.KeyType = SecurityKeyType.SymmetricKey;
            requirement.RequireCryptographicToken = true;
            requirement.Properties[ServiceModelSecurityTokenRequirement.SupportSecurityContextCancellationProperty] = this.RequireCancellation;
            requirement.Properties[ServiceModelSecurityTokenRequirement.SecureConversationSecurityBindingElementProperty] = this.BootstrapSecurityBindingElement;
            requirement.Properties[ServiceModelSecurityTokenRequirement.IssuerBindingContextProperty] = this.IssuerBindingContext.Clone();
            requirement.Properties[ServiceModelSecurityTokenRequirement.IssuedSecurityTokenParametersProperty] = this.Clone();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "RequireCancellation: {0}", _requireCancellation.ToString()));
            if (_bootstrapSecurityBindingElement == null)
            {
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "BootstrapSecurityBindingElement: null"));
            }
            else
            {
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "BootstrapSecurityBindingElement:"));
                sb.AppendLine("  " + this.BootstrapSecurityBindingElement.ToString().Trim().Replace("\n", "\n  "));
            }

            return sb.ToString().Trim();
        }
    }
}
