// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Web.Services.Description;
using System.Xml.Serialization;

namespace System.Web.Services.Configuration
{
    internal sealed class WebServicesSection {
        public WebServicesSection() { }

        public static WebServicesSection Current {
            get {
                return new WebServicesSection();
            }
        }

        internal Type[] GetAllFormatExtensionTypes() {
            return _defaultFormatTypes;
        }

        private static XmlFormatExtensionPointAttribute GetExtensionPointAttribute(Type type) {
            object[] attrs = type.GetCustomAttributes(typeof(XmlFormatExtensionPointAttribute), false);
            if (attrs.Length == 0)
            {
                throw new ArgumentException(SR.Format(SR.TheSyntaxOfTypeMayNotBeExtended1, type.FullName), nameof(type));
            }

            return (XmlFormatExtensionPointAttribute)attrs[0];
        }

        internal static void LoadXmlFormatExtensions(Type[] extensionTypes, XmlAttributeOverrides overrides, XmlSerializerNamespaces namespaces) {
            Hashtable table = new Hashtable();
            table.Add(typeof(ServiceDescription), new XmlAttributes());
            table.Add(typeof(Import), new XmlAttributes());
            table.Add(typeof(Port), new XmlAttributes());
            table.Add(typeof(Service), new XmlAttributes());
            table.Add(typeof(FaultBinding), new XmlAttributes());
            table.Add(typeof(InputBinding), new XmlAttributes());
            table.Add(typeof(OutputBinding), new XmlAttributes());
            table.Add(typeof(OperationBinding), new XmlAttributes());
            table.Add(typeof(Binding), new XmlAttributes());
            table.Add(typeof(OperationFault), new XmlAttributes());
            table.Add(typeof(OperationInput), new XmlAttributes());
            table.Add(typeof(OperationOutput), new XmlAttributes());
            table.Add(typeof(Operation), new XmlAttributes());
            table.Add(typeof(PortType), new XmlAttributes());
            table.Add(typeof(Message), new XmlAttributes());
            table.Add(typeof(MessagePart), new XmlAttributes());
            table.Add(typeof(Types), new XmlAttributes());
            Hashtable extensions = new Hashtable();
            foreach (Type extensionType in extensionTypes) {
                if (extensions[extensionType] != null) {
                    continue;
                }
                extensions.Add(extensionType, extensionType);
                object[] attrs = extensionType.GetCustomAttributes(typeof(XmlFormatExtensionAttribute), false);
                if (attrs.Length == 0) {
                    throw new ArgumentException(SR.Format(SR.RequiredXmlFormatExtensionAttributeIsMissing1, extensionType.FullName), nameof(extensionTypes));
                }
                XmlFormatExtensionAttribute extensionAttr = (XmlFormatExtensionAttribute)attrs[0];
                foreach (Type extensionPointType in extensionAttr.ExtensionPoints) {
                    XmlAttributes xmlAttrs = (XmlAttributes)table[extensionPointType];
                    if (xmlAttrs == null) {
                        xmlAttrs = new XmlAttributes();
                        table.Add(extensionPointType, xmlAttrs);
                    }
                    XmlElementAttribute xmlAttr = new XmlElementAttribute(extensionAttr.ElementName, extensionType);
                    xmlAttr.Namespace = extensionAttr.Namespace;
                    xmlAttrs.XmlElements.Add(xmlAttr);
                }
                attrs = extensionType.GetCustomAttributes(typeof(XmlFormatExtensionPrefixAttribute), false);
                string[] prefixes = new string[attrs.Length];
                Hashtable nsDefs = new Hashtable();
                for (int i = 0; i < attrs.Length; i++) {
                    XmlFormatExtensionPrefixAttribute prefixAttr = (XmlFormatExtensionPrefixAttribute)attrs[i];
                    prefixes[i] = prefixAttr.Prefix;
                    nsDefs.Add(prefixAttr.Prefix, prefixAttr.Namespace);
                }
                Array.Sort(prefixes, InvariantComparer.Default);
                for (int i = 0; i < prefixes.Length; i++) {
                    namespaces.Add(prefixes[i], (string)nsDefs[prefixes[i]]);
                }
            }
            foreach (Type extensionPointType in table.Keys) {
                XmlFormatExtensionPointAttribute attr = GetExtensionPointAttribute(extensionPointType);
                XmlAttributes xmlAttrs = (XmlAttributes)table[extensionPointType];
                if (attr.AllowElements) {
                    xmlAttrs.XmlAnyElements.Add(new XmlAnyElementAttribute());
                }
                overrides.Add(extensionPointType, attr.MemberName, xmlAttrs);
            }
        }

        private Type[] _defaultFormatTypes = new Type[] {
                                                   typeof(HttpAddressBinding),
                                                   typeof(HttpBinding),
                                                   typeof(HttpOperationBinding),
                                                   typeof(HttpUrlEncodedBinding),
                                                   typeof(HttpUrlReplacementBinding),
                                                   typeof(MimeContentBinding),
                                                   typeof(MimeXmlBinding),
                                                   typeof(MimeMultipartRelatedBinding),
                                                   typeof(MimeTextBinding),
                                                   typeof(SoapBinding),
                                                   typeof(SoapOperationBinding),
                                                   typeof(SoapBodyBinding),
                                                   typeof(SoapFaultBinding),
                                                   typeof(SoapHeaderBinding),
                                                   typeof(SoapAddressBinding),
                                                   typeof(Soap12Binding),
                                                   typeof(Soap12OperationBinding),
                                                   typeof(Soap12BodyBinding),
                                                   typeof(Soap12FaultBinding),
                                                   typeof(Soap12HeaderBinding),
                                                   typeof(Soap12AddressBinding) };
    }
}
