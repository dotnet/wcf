// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using Microsoft.Xml;
    using System.ServiceModel.Description;
    using System.Collections;

    public class OneWayBindingElementImporter : IPolicyImportExtension
    {
        public OneWayBindingElementImporter()
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

            XmlElement oneWayAssertion = PolicyConversionContext.FindAssertion(context.GetBindingAssertions(),
                OneWayPolicyConstants.OneWay, OneWayPolicyConstants.Namespace, true);

            if (oneWayAssertion != null)
            {
                OneWayBindingElement bindingElement = new OneWayBindingElement();
                context.BindingElements.Add(bindingElement);

                for (int i = 0; i < oneWayAssertion.ChildNodes.Count; i++)
                {
                    XmlNode currentNode = oneWayAssertion.ChildNodes[i];
                    if (currentNode != null
                        && currentNode.NodeType == XmlNodeType.Element
                        && currentNode.NamespaceURI == OneWayPolicyConstants.Namespace
                        && currentNode.LocalName == OneWayPolicyConstants.PacketRoutable)
                    {
                        bindingElement.PacketRoutable = true;
                        break;
                    }
                }
            }
            else if (WsdlImporter.WSAddressingHelper.DetermineSupportedAddressingMode(importer, context) == SupportedAddressingMode.NonAnonymous)
            {
                context.BindingElements.Add(new OneWayBindingElement());
            }
        }
    }

    internal static class OneWayPolicyConstants
    {
        public const string OneWay = "OneWay";
        public const string PacketRoutable = "PacketRoutable";
        public const string Namespace = DotNetOneWayStrings.Namespace + "/policy";
        public const string Prefix = "ow";
    }
}
