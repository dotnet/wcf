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
        private BindingContext _issuerBindingContext;
        private ChannelProtectionRequirements _bootstrapProtectionRequirements;

        protected SecureConversationSecurityTokenParameters(SecureConversationSecurityTokenParameters other)
            : base(other)
        {
            RequireCancellation = other.RequireCancellation;
            CanRenewSession = other.CanRenewSession;
            if (other.BootstrapSecurityBindingElement != null)
            {
                BootstrapSecurityBindingElement = (SecurityBindingElement)other.BootstrapSecurityBindingElement.Clone();
            }

            if (other._issuerBindingContext != null)
            {
                _issuerBindingContext = other._issuerBindingContext.Clone();
            }
        }

        public SecureConversationSecurityTokenParameters()
            : this(null, defaultRequireCancellation, null)
        {
            // empty
        }

        public SecureConversationSecurityTokenParameters(SecurityBindingElement bootstrapSecurityBindingElement)
        {
            BootstrapSecurityBindingElement = bootstrapSecurityBindingElement;
        }

        public SecureConversationSecurityTokenParameters(SecurityBindingElement bootstrapSecurityBindingElement, bool requireCancellation, ChannelProtectionRequirements bootstrapProtectionRequirements)
            : this(bootstrapSecurityBindingElement, requireCancellation, defaultCanRenewSession, null)
        {
            // empty
        }

        public SecureConversationSecurityTokenParameters(SecurityBindingElement bootstrapSecurityBindingElement, bool requireCancellation, bool canRenewSession, ChannelProtectionRequirements bootstrapProtectionRequirements) : base()
        {
            BootstrapSecurityBindingElement = bootstrapSecurityBindingElement;
            CanRenewSession = canRenewSession;
            if (bootstrapProtectionRequirements != null)
            {
                _bootstrapProtectionRequirements = new ChannelProtectionRequirements(bootstrapProtectionRequirements);
            }
            else
            {
                _bootstrapProtectionRequirements = new ChannelProtectionRequirements();
                _bootstrapProtectionRequirements.IncomingEncryptionParts.AddParts(new MessagePartSpecification(true));
                _bootstrapProtectionRequirements.IncomingSignatureParts.AddParts(new MessagePartSpecification(true));
                _bootstrapProtectionRequirements.OutgoingEncryptionParts.AddParts(new MessagePartSpecification(true));
                _bootstrapProtectionRequirements.OutgoingSignatureParts.AddParts(new MessagePartSpecification(true));
            }
            RequireCancellation = requireCancellation;
        }

        internal protected override bool HasAsymmetricKey { get { return false; } }

        public SecurityBindingElement BootstrapSecurityBindingElement { get; set; }

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
                return BootstrapSecurityBindingElement.GetIndividualProperty<ISecurityCapabilities>();
            }
        }

        public bool RequireCancellation { get; set; }

        public bool CanRenewSession { get; set; } = defaultCanRenewSession;

        internal protected override bool SupportsClientAuthentication
        {
            get
            {
                return BootstrapSecurityCapabilities == null ? false : BootstrapSecurityCapabilities.SupportsClientAuthentication;
            }
        }

        internal protected override bool SupportsServerAuthentication
        {
            get
            {
                return BootstrapSecurityCapabilities == null ? false : BootstrapSecurityCapabilities.SupportsServerAuthentication;
            }
        }

        internal protected override bool SupportsClientWindowsIdentity
        {
            get
            {
                return BootstrapSecurityCapabilities == null ? false : BootstrapSecurityCapabilities.SupportsClientWindowsIdentity;
            }
        }

        protected override SecurityTokenParameters CloneCore()
        {
            return new SecureConversationSecurityTokenParameters(this);
        }

        internal protected override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
        {
            if (token is GenericXmlSecurityToken)
            {
                return base.CreateGenericXmlTokenKeyIdentifierClause(token, referenceStyle);
            }
            else
            {
                return CreateKeyIdentifierClause<SecurityContextKeyIdentifierClause, LocalIdKeyIdentifierClause>(token, referenceStyle);
            }
        }

        protected internal override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
        {
            requirement.TokenType = ServiceModelSecurityTokenTypes.SecureConversation;
            requirement.KeyType = SecurityKeyType.SymmetricKey;
            requirement.RequireCryptographicToken = true;
            requirement.Properties[ServiceModelSecurityTokenRequirement.SupportSecurityContextCancellationProperty] = RequireCancellation;
            requirement.Properties[ServiceModelSecurityTokenRequirement.SecureConversationSecurityBindingElementProperty] = BootstrapSecurityBindingElement;
            requirement.Properties[ServiceModelSecurityTokenRequirement.IssuerBindingContextProperty] = IssuerBindingContext.Clone();
            requirement.Properties[ServiceModelSecurityTokenRequirement.IssuedSecurityTokenParametersProperty] = Clone();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "RequireCancellation: {0}", RequireCancellation.ToString()));
            if (BootstrapSecurityBindingElement == null)
            {
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "BootstrapSecurityBindingElement: null"));
            }
            else
            {
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "BootstrapSecurityBindingElement:"));
                sb.AppendLine("  " + BootstrapSecurityBindingElement.ToString().Trim().Replace("\n", "\n  "));
            }

            return sb.ToString().Trim();
        }
    }
}
