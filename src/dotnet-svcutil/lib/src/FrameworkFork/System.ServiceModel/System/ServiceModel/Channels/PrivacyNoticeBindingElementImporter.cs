// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using Microsoft.Xml;
    using System.ServiceModel.Description;

    internal static class PrivacyNoticePolicyStrings
    {
        public const string PrivacyNoticeName = "PrivacyNotice";
        public const string PrivacyNoticeVersionAttributeName = "Version";
        public const string PrivacyNoticeNamespace = "http://schemas.xmlsoap.org/ws/2005/05/identity";
        public const string PrivacyNoticePrefix = "ic";
    }

    public sealed class PrivacyNoticeBindingElementImporter : IPolicyImportExtension
    {
        void IPolicyImportExtension.ImportPolicy(MetadataImporter importer, PolicyConversionContext policyContext)
        {
            if (policyContext == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("policyContext");

            XmlElement privacyNoticeAssertion = PolicyConversionContext.FindAssertion(policyContext.GetBindingAssertions(),
                PrivacyNoticePolicyStrings.PrivacyNoticeName, PrivacyNoticePolicyStrings.PrivacyNoticeNamespace, true);
            if (privacyNoticeAssertion != null)
            {
                PrivacyNoticeBindingElement settings =
                    policyContext.BindingElements.Find<PrivacyNoticeBindingElement>();

                if (null == settings)
                {
                    settings = new PrivacyNoticeBindingElement();
                    policyContext.BindingElements.Add(settings);
                }

                settings.Url = new Uri(privacyNoticeAssertion.InnerText);
                string versionString = privacyNoticeAssertion.GetAttribute(PrivacyNoticePolicyStrings.PrivacyNoticeVersionAttributeName, PrivacyNoticePolicyStrings.PrivacyNoticeNamespace);
                if (string.IsNullOrEmpty(versionString))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.CannotImportPrivacyNoticeElementWithoutVersionAttribute));
                }

                int version = 0;
                if (!Int32.TryParse(versionString, out version))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.PrivacyNoticeElementVersionAttributeInvalid));
                }
                settings.Version = version;
            }
        }
    }
}

