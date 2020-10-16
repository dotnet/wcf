// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using Microsoft.Xml;
    using System.ServiceModel.Description;
    using System.Collections;

    public class CompositeDuplexBindingElementImporter : IPolicyImportExtension
    {
        public CompositeDuplexBindingElementImporter()
        {
        }

        void IPolicyImportExtension.ImportPolicy(MetadataImporter importer, PolicyConversionContext context)
        {
            if (importer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("importer");
            }

            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            XmlElement compositeDuplexAssertion = PolicyConversionContext.FindAssertion(context.GetBindingAssertions(),
                TransportPolicyConstants.CompositeDuplex, TransportPolicyConstants.CompositeDuplexNamespace, true);

            if (compositeDuplexAssertion != null
                || WsdlImporter.WSAddressingHelper.DetermineSupportedAddressingMode(importer, context) == SupportedAddressingMode.NonAnonymous)
            {
                context.BindingElements.Add(new CompositeDuplexBindingElement());
            }
        }
    }
}
