// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Runtime;
    using System.ServiceModel.Description;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security.Tokens;
    using System.Text;
    using Microsoft.Xml;

    internal class WSSecurityPolicy12 : WSSecurityPolicy
    {
        public const string WsspNamespace = @"http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702";

        public const string SignedEncryptedSupportingTokensName = "SignedEncryptedSupportingTokens";
        public const string RequireImpliedDerivedKeysName = "RequireImpliedDerivedKeys";
        public const string RequireExplicitDerivedKeysName = "RequireExplicitDerivedKeys";

        public override string WsspNamespaceUri
        {
            get { return WSSecurityPolicy12.WsspNamespace; }
        }

        public override bool IsSecurityVersionSupported(MessageSecurityVersion version)
        {
            return version == MessageSecurityVersion.WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10 ||
                version == MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12 ||
                version == MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;
        }

        public override MessageSecurityVersion GetSupportedMessageSecurityVersion(SecurityVersion version)
        {
            return (version == SecurityVersion.WSSecurity10) ? MessageSecurityVersion.WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10 : MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;
        }

        public override TrustDriver TrustDriver
        {
            get
            {
                throw new NotImplementedException();
                // TODO:
                // return new WSTrustDec2005.DriverDec2005(new SecurityStandardsManager(MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12, WSSecurityTokenSerializer.DefaultInstance));
            }
        }

        public override XmlElement CreateWsspHttpsTokenAssertion(MetadataExporter exporter, HttpsTransportBindingElement httpsBinding)
        {
            Fx.Assert(httpsBinding != null, "httpsBinding must not be null.");
            Fx.Assert(httpsBinding.AuthenticationScheme.IsSingleton(), "authenticationScheme must be a singleton value for security-mode TransportWithMessageCredential.");

            XmlElement result = CreateWsspAssertion(WSSecurityPolicy.HttpsTokenName);
            if (httpsBinding.RequireClientCertificate ||
                httpsBinding.AuthenticationScheme == AuthenticationSchemes.Basic ||
                httpsBinding.AuthenticationScheme == AuthenticationSchemes.Digest)
            {
                XmlElement policy = CreateWspPolicyWrapper(exporter);
                if (httpsBinding.RequireClientCertificate)
                {
                    policy.AppendChild(CreateWsspAssertion(WSSecurityPolicy.RequireClientCertificateName));
                }
                if (httpsBinding.AuthenticationScheme == AuthenticationSchemes.Basic)
                {
                    policy.AppendChild(CreateWsspAssertion(WSSecurityPolicy.HttpBasicAuthenticationName));
                }
                else if (httpsBinding.AuthenticationScheme == AuthenticationSchemes.Digest)
                {
                    policy.AppendChild(CreateWsspAssertion(WSSecurityPolicy.HttpDigestAuthenticationName));
                }
                result.AppendChild(policy);
            }
            return result;
        }

        public override bool TryImportWsspHttpsTokenAssertion(MetadataImporter importer, ICollection<XmlElement> assertions, HttpsTransportBindingElement httpsBinding)
        {
            if (assertions == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("assertions");
            }

            bool result = true;
            XmlElement assertion;

            if (TryImportWsspAssertion(assertions, HttpsTokenName, out assertion))
            {
                XmlElement policyElement = null;
                foreach (XmlNode node in assertion.ChildNodes)
                {
                    if (node is XmlElement && node.LocalName == WSSecurityPolicy.PolicyName && (node.NamespaceURI == WSSecurityPolicy.WspNamespace || node.NamespaceURI == WSSecurityPolicy.Wsp15Namespace))
                    {
                        policyElement = (XmlElement)node;
                        break;
                    }
                }

                if (policyElement != null)
                {
                    foreach (XmlNode node in policyElement.ChildNodes)
                    {
                        if (node is XmlElement && node.NamespaceURI == this.WsspNamespaceUri)
                        {
                            if (node.LocalName == WSSecurityPolicy.RequireClientCertificateName)
                            {
                                httpsBinding.RequireClientCertificate = true;
                            }
                            else if (node.LocalName == WSSecurityPolicy.HttpBasicAuthenticationName)
                            {
                                httpsBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                            }
                            else if (node.LocalName == WSSecurityPolicy.HttpDigestAuthenticationName)
                            {
                                httpsBinding.AuthenticationScheme = AuthenticationSchemes.Digest;
                            }
                        }
                    }
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        public override Collection<XmlElement> CreateWsspSupportingTokensAssertion(MetadataExporter exporter, Collection<SecurityTokenParameters> signed, Collection<SecurityTokenParameters> signedEncrypted, Collection<SecurityTokenParameters> endorsing, Collection<SecurityTokenParameters> signedEndorsing, Collection<SecurityTokenParameters> optionalSigned, Collection<SecurityTokenParameters> optionalSignedEncrypted, Collection<SecurityTokenParameters> optionalEndorsing, Collection<SecurityTokenParameters> optionalSignedEndorsing, AddressingVersion addressingVersion)
        {
            Collection<XmlElement> supportingTokenAssertions = new Collection<XmlElement>();

            // Signed Supporting Tokens
            XmlElement supportingTokenAssertion = CreateWsspSignedSupportingTokensAssertion(exporter, signed, optionalSigned);
            if (supportingTokenAssertion != null)
                supportingTokenAssertions.Add(supportingTokenAssertion);

            // Signed Encrypted Supporting Tokens
            supportingTokenAssertion = CreateWsspSignedEncryptedSupportingTokensAssertion(exporter, signedEncrypted, optionalSignedEncrypted);
            if (supportingTokenAssertion != null)
                supportingTokenAssertions.Add(supportingTokenAssertion);

            // Endorsing Supporting Tokens.
            supportingTokenAssertion = CreateWsspEndorsingSupportingTokensAssertion(exporter, endorsing, optionalEndorsing, addressingVersion);
            if (supportingTokenAssertion != null)
                supportingTokenAssertions.Add(supportingTokenAssertion);

            // Signed Endorsing Supporting Tokens.
            supportingTokenAssertion = CreateWsspSignedEndorsingSupportingTokensAssertion(exporter, signedEndorsing, optionalSignedEndorsing, addressingVersion);
            if (supportingTokenAssertion != null)
                supportingTokenAssertions.Add(supportingTokenAssertion);

            return supportingTokenAssertions;
        }

        private XmlElement CreateWsspMustNotSendAmendAssertion()
        {
            XmlElement result = CreateWsspAssertion(MustNotSendAmendName);
            return result;
        }

        private XmlElement CreateWsspMustNotSendRenewAssertion()
        {
            XmlElement result = CreateWsspAssertion(MustNotSendRenewName);
            return result;
        }

        public virtual bool TryImportWsspMustNotSendAmendAssertion(ICollection<XmlElement> assertions)
        {
            TryImportWsspAssertion(assertions, MustNotSendAmendName);
            return true;
        }

        public virtual bool TryImportWsspMustNotSendRenewAssertion(ICollection<XmlElement> assertions, out bool canRenewSession)
        {
            canRenewSession = !TryImportWsspAssertion(assertions, MustNotSendRenewName);
            return true;
        }

        private XmlElement CreateWsspSignedSupportingTokensAssertion(MetadataExporter exporter, Collection<SecurityTokenParameters> signed, Collection<SecurityTokenParameters> optionalSigned)
        {
            XmlElement result;

            if ((signed == null || signed.Count == 0)
                && (optionalSigned == null || optionalSigned.Count == 0))
            {
                result = null;
            }
            else
            {
                XmlElement policy = CreateWspPolicyWrapper(exporter);

                if (signed != null)
                {
                    foreach (SecurityTokenParameters p in signed)
                    {
                        policy.AppendChild(CreateTokenAssertion(exporter, p));
                    }
                }
                if (optionalSigned != null)
                {
                    foreach (SecurityTokenParameters p in optionalSigned)
                    {
                        policy.AppendChild(CreateTokenAssertion(exporter, p, true));
                    }
                }

                result = CreateWsspAssertion(SignedSupportingTokensName);
                result.AppendChild(policy);
            }

            return result;
        }

        private XmlElement CreateWsspSignedEncryptedSupportingTokensAssertion(MetadataExporter exporter, Collection<SecurityTokenParameters> signedEncrypted, Collection<SecurityTokenParameters> optionalSignedEncrypted)
        {
            XmlElement result;

            if ((signedEncrypted == null || signedEncrypted.Count == 0)
                && (optionalSignedEncrypted == null || optionalSignedEncrypted.Count == 0))
            {
                result = null;
            }
            else
            {
                XmlElement policy = CreateWspPolicyWrapper(exporter);

                if (signedEncrypted != null)
                {
                    foreach (SecurityTokenParameters p in signedEncrypted)
                    {
                        policy.AppendChild(CreateTokenAssertion(exporter, p));
                    }
                }
                if (optionalSignedEncrypted != null)
                {
                    foreach (SecurityTokenParameters p in optionalSignedEncrypted)
                    {
                        policy.AppendChild(CreateTokenAssertion(exporter, p, true));
                    }
                }

                result = CreateWsspAssertion(SignedEncryptedSupportingTokensName);
                result.AppendChild(policy);
            }

            return result;
        }

        public override bool TryImportWsspSupportingTokensAssertion(MetadataImporter importer, PolicyConversionContext policyContext, ICollection<XmlElement> assertions, Collection<SecurityTokenParameters> signed, Collection<SecurityTokenParameters> signedEncrypted, Collection<SecurityTokenParameters> endorsing, Collection<SecurityTokenParameters> signedEndorsing, Collection<SecurityTokenParameters> optionalSigned, Collection<SecurityTokenParameters> optionalSignedEncrypted, Collection<SecurityTokenParameters> optionalEndorsing, Collection<SecurityTokenParameters> optionalSignedEndorsing)
        {
            XmlElement assertion;

            if (!TryImportWsspSignedSupportingTokensAssertion(
                importer,
                policyContext,
                assertions,
                signed,
                optionalSigned,
                out assertion)
                && assertion != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.UnsupportedSecurityPolicyAssertion, assertion.OuterXml)));
            }

            if (!TryImportWsspSignedEncryptedSupportingTokensAssertion(
                importer,
                policyContext,
                assertions,
                signedEncrypted,
                optionalSignedEncrypted,
                out assertion)
                && assertion != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.UnsupportedSecurityPolicyAssertion, assertion.OuterXml)));
            }

            if (!TryImportWsspEndorsingSupportingTokensAssertion(
                importer,
                policyContext,
                assertions,
                endorsing,
                optionalEndorsing,
                out assertion)
                && assertion != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.UnsupportedSecurityPolicyAssertion, assertion.OuterXml)));
            }

            if (!TryImportWsspSignedEndorsingSupportingTokensAssertion(
                importer,
                policyContext,
                assertions,
                signedEndorsing,
                optionalSignedEndorsing,
                out assertion)
                && assertion != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.UnsupportedSecurityPolicyAssertion, assertion.OuterXml)));
            }

            return true;
        }

        private bool TryImportWsspSignedSupportingTokensAssertion(MetadataImporter importer, PolicyConversionContext policyContext, ICollection<XmlElement> assertions, Collection<SecurityTokenParameters> signed, Collection<SecurityTokenParameters> optionalSigned, out XmlElement assertion)
        {
            throw new NotImplementedException();
        }

        private bool TryImportWsspSignedEncryptedSupportingTokensAssertion(MetadataImporter importer, PolicyConversionContext policyContext, ICollection<XmlElement> assertions, Collection<SecurityTokenParameters> signedEncrypted, Collection<SecurityTokenParameters> optionalSignedEncrypted, out XmlElement assertion)
        {
            throw new NotImplementedException();
        }

        public override XmlElement CreateWsspTrustAssertion(MetadataExporter exporter, SecurityKeyEntropyMode keyEntropyMode)
        {
            return CreateWsspTrustAssertion(Trust13Name, exporter, keyEntropyMode);
        }

        public override bool TryImportWsspTrustAssertion(MetadataImporter importer, ICollection<XmlElement> assertions, SecurityBindingElement binding, out XmlElement assertion)
        {
            return TryImportWsspTrustAssertion(Trust13Name, importer, assertions, binding, out assertion);
        }
    }
}
