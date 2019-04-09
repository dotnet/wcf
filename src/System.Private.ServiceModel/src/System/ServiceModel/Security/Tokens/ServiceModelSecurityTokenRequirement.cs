// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Text;
using System.ServiceModel.Channels;
using System.IdentityModel.Selectors;
using System.Globalization;

namespace System.ServiceModel.Security.Tokens
{
    public abstract class ServiceModelSecurityTokenRequirement : SecurityTokenRequirement
    {
        protected const string Namespace = "http://schemas.microsoft.com/ws/2006/05/servicemodel/securitytokenrequirement";
        private const string securityAlgorithmSuiteProperty = Namespace + "/SecurityAlgorithmSuite";
        private const string securityBindingElementProperty = Namespace + "/SecurityBindingElement";
        private const string issuerAddressProperty = Namespace + "/IssuerAddress";
        private const string issuerBindingProperty = Namespace + "/IssuerBinding";
        private const string secureConversationSecurityBindingElementProperty = Namespace + "/SecureConversationSecurityBindingElement";
        private const string supportSecurityContextCancellationProperty = Namespace + "/SupportSecurityContextCancellation";
        private const string messageSecurityVersionProperty = Namespace + "/MessageSecurityVersion";
        private const string defaultMessageSecurityVersionProperty = Namespace + "/DefaultMessageSecurityVersion";
        private const string issuerBindingContextProperty = Namespace + "/IssuerBindingContext";
        private const string transportSchemeProperty = Namespace + "/TransportScheme";
        private const string isInitiatorProperty = Namespace + "/IsInitiator";
        private const string targetAddressProperty = Namespace + "/TargetAddress";
        private const string viaProperty = Namespace + "/Via";
        private const string listenUriProperty = Namespace + "/ListenUri";
        private const string auditLogLocationProperty = Namespace + "/AuditLogLocation";
        private const string suppressAuditFailureProperty = Namespace + "/SuppressAuditFailure";
        private const string messageAuthenticationAuditLevelProperty = Namespace + "/MessageAuthenticationAuditLevel";
        private const string isOutOfBandTokenProperty = Namespace + "/IsOutOfBandToken";
        private const string preferSslCertificateAuthenticatorProperty = Namespace + "/PreferSslCertificateAuthenticator";

        // the following properties dont have top level OM properties but are part of the property bag
        private const string supportingTokenAttachmentModeProperty = Namespace + "/SupportingTokenAttachmentMode";
        private const string messageDirectionProperty = Namespace + "/MessageDirection";
        private const string httpAuthenticationSchemeProperty = Namespace + "/HttpAuthenticationScheme";
        private const string issuedSecurityTokenParametersProperty = Namespace + "/IssuedSecurityTokenParameters";
        private const string privacyNoticeUriProperty = Namespace + "/PrivacyNoticeUri";
        private const string privacyNoticeVersionProperty = Namespace + "/PrivacyNoticeVersion";
        private const string duplexClientLocalAddressProperty = Namespace + "/DuplexClientLocalAddress";
        private const string endpointFilterTableProperty = Namespace + "/EndpointFilterTable";
        private const string channelParametersCollectionProperty = Namespace + "/ChannelParametersCollection";
        private const string extendedProtectionPolicy = Namespace + "/ExtendedProtectionPolicy";

        private const bool defaultSupportSecurityContextCancellation = false;

        protected ServiceModelSecurityTokenRequirement()
            : base()
        {
            Properties[SupportSecurityContextCancellationProperty] = defaultSupportSecurityContextCancellation;
        }

        static public string SecurityAlgorithmSuiteProperty { get { return securityAlgorithmSuiteProperty; } }
        static public string SecurityBindingElementProperty { get { return securityBindingElementProperty; } }
        static public string IssuerAddressProperty { get { return issuerAddressProperty; } }
        static public string IssuerBindingProperty { get { return issuerBindingProperty; } }
        static public string SecureConversationSecurityBindingElementProperty { get { return secureConversationSecurityBindingElementProperty; } }
        static public string SupportSecurityContextCancellationProperty { get { return supportSecurityContextCancellationProperty; } }
        static public string MessageSecurityVersionProperty { get { return messageSecurityVersionProperty; } }
        static internal string DefaultMessageSecurityVersionProperty { get { return defaultMessageSecurityVersionProperty; } }
        static public string IssuerBindingContextProperty { get { return issuerBindingContextProperty; } }
        static public string TransportSchemeProperty { get { return transportSchemeProperty; } }
        static public string IsInitiatorProperty { get { return isInitiatorProperty; } }
        static public string TargetAddressProperty { get { return targetAddressProperty; } }
        static public string ViaProperty { get { return viaProperty; } }
        static public string ListenUriProperty { get { return listenUriProperty; } }
        static public string AuditLogLocationProperty { get { return auditLogLocationProperty; } }
        static public string SuppressAuditFailureProperty { get { return suppressAuditFailureProperty; } }
        static public string MessageAuthenticationAuditLevelProperty { get { return messageAuthenticationAuditLevelProperty; } }
        static public string IsOutOfBandTokenProperty { get { return isOutOfBandTokenProperty; } }
        static public string PreferSslCertificateAuthenticatorProperty { get { return preferSslCertificateAuthenticatorProperty; } }

        static public string SupportingTokenAttachmentModeProperty { get { return supportingTokenAttachmentModeProperty; } }
        static public string MessageDirectionProperty { get { return messageDirectionProperty; } }
        static public string HttpAuthenticationSchemeProperty { get { return httpAuthenticationSchemeProperty; } }
        static public string IssuedSecurityTokenParametersProperty { get { return issuedSecurityTokenParametersProperty; } }
        static public string PrivacyNoticeUriProperty { get { return privacyNoticeUriProperty; } }
        static public string PrivacyNoticeVersionProperty { get { return privacyNoticeVersionProperty; } }
        static public string DuplexClientLocalAddressProperty { get { return duplexClientLocalAddressProperty; } }
        static public string EndpointFilterTableProperty { get { return endpointFilterTableProperty; } }
        static public string ChannelParametersCollectionProperty { get { return channelParametersCollectionProperty; } }
        static public string ExtendedProtectionPolicy { get { return extendedProtectionPolicy; } }

        public bool IsInitiator
        {
            get
            {
                return GetPropertyOrDefault(IsInitiatorProperty, false);
            }
        }

        public SecurityAlgorithmSuite SecurityAlgorithmSuite
        {
            get
            {
                return GetPropertyOrDefault<SecurityAlgorithmSuite>(SecurityAlgorithmSuiteProperty, null);
            }
            set
            {
                Properties[SecurityAlgorithmSuiteProperty] = value;
            }
        }

        public SecurityBindingElement SecurityBindingElement
        {
            get
            {
                return GetPropertyOrDefault<SecurityBindingElement>(SecurityBindingElementProperty, null);
            }
            set
            {
                Properties[SecurityBindingElementProperty] = value;
            }
        }

        public EndpointAddress IssuerAddress
        {
            get
            {
                return GetPropertyOrDefault<EndpointAddress>(IssuerAddressProperty, null);
            }
            set
            {
                Properties[IssuerAddressProperty] = value;
            }
        }

        public Binding IssuerBinding
        {
            get
            {
                return GetPropertyOrDefault<Binding>(IssuerBindingProperty, null);
            }
            set
            {
                Properties[IssuerBindingProperty] = value;
            }
        }

        public SecurityBindingElement SecureConversationSecurityBindingElement
        {
            get
            {
                return GetPropertyOrDefault<SecurityBindingElement>(SecureConversationSecurityBindingElementProperty, null);
            }
            set
            {
                Properties[SecureConversationSecurityBindingElementProperty] = value;
            }
        }

        public SecurityTokenVersion MessageSecurityVersion
        {
            get
            {
                return GetPropertyOrDefault<SecurityTokenVersion>(MessageSecurityVersionProperty, null);
            }
            set
            {
                Properties[MessageSecurityVersionProperty] = value;
            }
        }

        internal MessageSecurityVersion DefaultMessageSecurityVersion
        {
            get
            {
                MessageSecurityVersion messageSecurityVersion;
                return (TryGetProperty(DefaultMessageSecurityVersionProperty, out messageSecurityVersion)) ? messageSecurityVersion : null;
            }
            set
            {
                Properties[DefaultMessageSecurityVersionProperty] = (object)value;
            }
        }

        public string TransportScheme
        {
            get
            {
                return GetPropertyOrDefault<string>(TransportSchemeProperty, null);
            }
            set
            {
                Properties[TransportSchemeProperty] = value;
            }
        }

        internal bool SupportSecurityContextCancellation
        {
            get
            {
                return GetPropertyOrDefault(SupportSecurityContextCancellationProperty, defaultSupportSecurityContextCancellation);
            }
            set
            {
                Properties[SupportSecurityContextCancellationProperty] = value;
            }
        }

        internal EndpointAddress DuplexClientLocalAddress
        {
            get
            {
                return GetPropertyOrDefault<EndpointAddress>(duplexClientLocalAddressProperty, null);
            }
            set
            {
                Properties[duplexClientLocalAddressProperty] = value;
            }
        }

        internal TValue GetPropertyOrDefault<TValue>(string propertyName, TValue defaultValue)
        {
            TValue result;
            if (!TryGetProperty(propertyName, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        internal string InternalToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0}:", GetType().ToString()));
            foreach (string propertyName in Properties.Keys)
            {
                object propertyValue = Properties[propertyName];
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "PropertyName: {0}", propertyName));
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "PropertyValue: {0}", propertyValue));
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "---"));
            }
            return sb.ToString().Trim();
        }
    }
}
