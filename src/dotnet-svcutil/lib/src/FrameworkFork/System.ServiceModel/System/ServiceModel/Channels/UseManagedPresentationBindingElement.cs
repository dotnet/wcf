// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System.ServiceModel.Description;
    using Microsoft.Xml;

    public sealed class UseManagedPresentationBindingElement : BindingElement, IPolicyExportExtension
    {
        public UseManagedPresentationBindingElement()
        {
        }

        public override BindingElement Clone()
        {
            return new UseManagedPresentationBindingElement();
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            return context.GetInnerProperty<T>();
        }

        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            if (context.BindingElements != null)
            {
                UseManagedPresentationBindingElement settings =
                    context.BindingElements.Find<UseManagedPresentationBindingElement>();

                if (settings != null)
                {
                    XmlDocument doc = new XmlDocument();

                    // UseUseManagedPresentation assertion
                    XmlElement assertion = doc.CreateElement(UseManagedPresentationPolicyStrings.UseManagedPresentationPrefix,
                                                              UseManagedPresentationPolicyStrings.RequireFederatedIdentityProvisioningName,
                                                              UseManagedPresentationPolicyStrings.UseManagedPresentationNamespace);

                    context.GetBindingAssertions().Add(assertion);
                }
            }
        }
    }
}
