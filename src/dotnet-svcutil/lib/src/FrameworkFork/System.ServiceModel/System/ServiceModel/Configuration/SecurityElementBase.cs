// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Configuration
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IdentityModel.Tokens;
    using System.Runtime;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.ServiceModel.Security.Tokens;
    using Microsoft.Xml;
    using System.Linq;

    public partial class SecurityElementBase : BindingElementExtensionElement
    {
        // if you add another variable, make sure to adjust: CopyFrom and UnMerge methods.
        SecurityBindingElement failedSecurityBindingElement = null;
        bool willX509IssuerReferenceAssertionBeWritten;
        SecurityKeyType templateKeyType = IssuedSecurityTokenParameters.defaultKeyType;

        internal SecurityElementBase()
        {
        }

        internal bool HasImportFailed { get { return this.failedSecurityBindingElement != null; } }

        internal bool IsSecurityElementBootstrap { get; set; } // Used in serialization path to optimize Xml representation

        public override Type BindingElementType
        {
            get { return typeof(SecurityBindingElement); }
        }
        protected internal override BindingElement CreateBindingElement()
        {
            throw new NotImplementedException();
        }

        static bool AreTokenParametersMatching(SecurityTokenParameters p1, SecurityTokenParameters p2, bool skipRequireDerivedKeysComparison, bool exactMessageSecurityVersion)
        {
            if (p1 == null || p2 == null)
                return false;

            if (p1.GetType() != p2.GetType())
                return false;

            if (p1.InclusionMode != p2.InclusionMode)
                return false;

            if (skipRequireDerivedKeysComparison == false && p1.RequireDerivedKeys != p2.RequireDerivedKeys)
                return false;

            if (p1.ReferenceStyle != p2.ReferenceStyle)
                return false;

            // mutual ssl and anonymous ssl differ in the client cert requirement
            if (p1 is SslSecurityTokenParameters)
            {
                if (((SslSecurityTokenParameters)p1).RequireClientCertificate != ((SslSecurityTokenParameters)p2).RequireClientCertificate)
                    return false;
            }
            else if (p1 is SecureConversationSecurityTokenParameters)
            {
                SecureConversationSecurityTokenParameters sc1 = (SecureConversationSecurityTokenParameters)p1;
                SecureConversationSecurityTokenParameters sc2 = (SecureConversationSecurityTokenParameters)p2;

                if (sc1.RequireCancellation != sc2.RequireCancellation)
                    return false;

                if (sc1.CanRenewSession != sc2.CanRenewSession)
                    return false;


                if (!AreBindingsMatching(sc1.BootstrapSecurityBindingElement, sc2.BootstrapSecurityBindingElement, exactMessageSecurityVersion))
                    return false;
            }
            else if (p1 is IssuedSecurityTokenParameters)
            {
                if (((IssuedSecurityTokenParameters)p1).KeyType != ((IssuedSecurityTokenParameters)p2).KeyType)
                    return false;
            }

            return true;
        }

        static bool AreTokenParameterCollectionsMatching(Collection<SecurityTokenParameters> c1, Collection<SecurityTokenParameters> c2, bool exactMessageSecurityVersion)
        {
            if (c1.Count != c2.Count)
                return false;

            for (int i = 0; i < c1.Count; i++)
                if (!AreTokenParametersMatching(c1[i], c2[i], true, exactMessageSecurityVersion))
                    return false;

            return true;
        }

        internal static bool AreBindingsMatching(SecurityBindingElement b1, SecurityBindingElement b2)
        {
            return AreBindingsMatching(b1, b2, true);
        }

        internal static bool AreBindingsMatching(SecurityBindingElement b1, SecurityBindingElement b2, bool exactMessageSecurityVersion)
        {
            if (b1 == null || b2 == null)
                return b1 == b2;

            if (b1.GetType() != b2.GetType())
                return false;

            if (b1.MessageSecurityVersion != b2.MessageSecurityVersion)
            {
                // exactMessageSecurityVersion meant that BSP mismatch could be ignored
                if (exactMessageSecurityVersion)
                    return false;

                if (b1.MessageSecurityVersion.SecurityVersion != b2.MessageSecurityVersion.SecurityVersion
                 || b1.MessageSecurityVersion.TrustVersion != b2.MessageSecurityVersion.TrustVersion
                 || b1.MessageSecurityVersion.SecureConversationVersion != b2.MessageSecurityVersion.SecureConversationVersion
                 || b1.MessageSecurityVersion.SecurityPolicyVersion != b2.MessageSecurityVersion.SecurityPolicyVersion)
                {
                    return false;
                }
            }

            if (b1.SecurityHeaderLayout != b2.SecurityHeaderLayout)
                return false;

            if (b1.DefaultAlgorithmSuite != b2.DefaultAlgorithmSuite)
                return false;

            if (b1.IncludeTimestamp != b2.IncludeTimestamp)
                return false;

            if (b1.SecurityHeaderLayout != b2.SecurityHeaderLayout)
                return false;

            if (b1.KeyEntropyMode != b2.KeyEntropyMode)
                return false;

            if (!AreTokenParameterCollectionsMatching(b1.EndpointSupportingTokenParameters.Endorsing, b2.EndpointSupportingTokenParameters.Endorsing, exactMessageSecurityVersion))
                return false;

            if (!AreTokenParameterCollectionsMatching(b1.EndpointSupportingTokenParameters.SignedEncrypted, b2.EndpointSupportingTokenParameters.SignedEncrypted, exactMessageSecurityVersion))
                return false;

            if (!AreTokenParameterCollectionsMatching(b1.EndpointSupportingTokenParameters.Signed, b2.EndpointSupportingTokenParameters.Signed, exactMessageSecurityVersion))
                return false;

            if (!AreTokenParameterCollectionsMatching(b1.EndpointSupportingTokenParameters.SignedEndorsing, b2.EndpointSupportingTokenParameters.SignedEndorsing, exactMessageSecurityVersion))
                return false;

            if (b1.OperationSupportingTokenParameters.Count != b2.OperationSupportingTokenParameters.Count)
                return false;

            foreach (KeyValuePair<string, SupportingTokenParameters> operation1 in b1.OperationSupportingTokenParameters)
            {
                if (!b2.OperationSupportingTokenParameters.ContainsKey(operation1.Key))
                    return false;

                SupportingTokenParameters stp2 = b2.OperationSupportingTokenParameters[operation1.Key];

                if (!AreTokenParameterCollectionsMatching(operation1.Value.Endorsing, stp2.Endorsing, exactMessageSecurityVersion))
                    return false;

                if (!AreTokenParameterCollectionsMatching(operation1.Value.SignedEncrypted, stp2.SignedEncrypted, exactMessageSecurityVersion))
                    return false;

                if (!AreTokenParameterCollectionsMatching(operation1.Value.Signed, stp2.Signed, exactMessageSecurityVersion))
                    return false;

                if (!AreTokenParameterCollectionsMatching(operation1.Value.SignedEndorsing, stp2.SignedEndorsing, exactMessageSecurityVersion))
                    return false;
            }

            SymmetricSecurityBindingElement ssbe1 = b1 as SymmetricSecurityBindingElement;
            if (ssbe1 != null)
            {
                SymmetricSecurityBindingElement ssbe2 = (SymmetricSecurityBindingElement)b2;

                if (ssbe1.MessageProtectionOrder != ssbe2.MessageProtectionOrder)
                    return false;

                if (!AreTokenParametersMatching(ssbe1.ProtectionTokenParameters, ssbe2.ProtectionTokenParameters, false, exactMessageSecurityVersion))
                    return false;
            }

            AsymmetricSecurityBindingElement asbe1 = b1 as AsymmetricSecurityBindingElement;
            if (asbe1 != null)
            {
                AsymmetricSecurityBindingElement asbe2 = (AsymmetricSecurityBindingElement)b2;

                if (asbe1.MessageProtectionOrder != asbe2.MessageProtectionOrder)
                    return false;

                if (asbe1.RequireSignatureConfirmation != asbe2.RequireSignatureConfirmation)
                    return false;

                if (!AreTokenParametersMatching(asbe1.InitiatorTokenParameters, asbe2.InitiatorTokenParameters, true, exactMessageSecurityVersion)
                    || !AreTokenParametersMatching(asbe1.RecipientTokenParameters, asbe2.RecipientTokenParameters, true, exactMessageSecurityVersion))
                    return false;
            }

            return true;
        }
    }
}
