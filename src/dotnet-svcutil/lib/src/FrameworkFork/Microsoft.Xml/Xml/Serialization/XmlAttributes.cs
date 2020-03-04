// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Serialization {
    using System;
    using System.Reflection;
    using System.Collections.Generic;
    using System.ComponentModel;

    internal enum XmlAttributeFlags {
        Enum = 0x1,
        Array = 0x2,
        Text = 0x4,
        ArrayItems = 0x8,
        Elements = 0x10,
        Attribute = 0x20,
        Root = 0x40,
        Type = 0x80,
        AnyElements = 0x100,
        AnyAttribute = 0x200,
        ChoiceIdentifier = 0x400,
        XmlnsDeclarations = 0x800,
    }

    /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class XmlAttributes {
        XmlElementAttributes xmlElements = new XmlElementAttributes();
        XmlArrayItemAttributes xmlArrayItems = new XmlArrayItemAttributes();
        XmlAnyElementAttributes xmlAnyElements = new XmlAnyElementAttributes();
        XmlArrayAttribute xmlArray;
        XmlAttributeAttribute xmlAttribute;
        XmlTextAttribute xmlText;
        XmlEnumAttribute xmlEnum;
        bool xmlIgnore;
        bool xmlns;
        object xmlDefaultValue = null;
        XmlRootAttribute xmlRoot;
        XmlTypeAttribute xmlType;
        XmlAnyAttributeAttribute xmlAnyAttribute;
        XmlChoiceIdentifierAttribute xmlChoiceIdentifier;
        static volatile Type ignoreAttributeType;


        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlAttributes"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlAttributes() {
        }

        internal XmlAttributeFlags XmlFlags {
            get { 
                XmlAttributeFlags flags = 0;
                if (xmlElements.Count > 0) flags |= XmlAttributeFlags.Elements;
                if (xmlArrayItems.Count > 0) flags |= XmlAttributeFlags.ArrayItems;
                if (xmlAnyElements.Count > 0) flags |= XmlAttributeFlags.AnyElements;
                if (xmlArray != null) flags |= XmlAttributeFlags.Array;
                if (xmlAttribute != null) flags |= XmlAttributeFlags.Attribute;
                if (xmlText != null) flags |= XmlAttributeFlags.Text;
                if (xmlEnum != null) flags |= XmlAttributeFlags.Enum;
                if (xmlRoot != null) flags |= XmlAttributeFlags.Root;
                if (xmlType != null) flags |= XmlAttributeFlags.Type;
                if (xmlAnyAttribute != null) flags |= XmlAttributeFlags.AnyAttribute;
                if (xmlChoiceIdentifier != null) flags |= XmlAttributeFlags.ChoiceIdentifier;
                if (xmlns) flags |= XmlAttributeFlags.XmlnsDeclarations;
                return flags;
            }
        }

        private static Type IgnoreAttribute {
            get {
                if (ignoreAttributeType == null) {
                    ignoreAttributeType = typeof(object).GetTypeInfo().Assembly.GetType("System.XmlIgnoreMemberAttribute");
                    if (ignoreAttributeType == null) {
                        ignoreAttributeType = typeof(XmlIgnoreAttribute);
                    }
                }
                return ignoreAttributeType;
            }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlAttributes1"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlAttributes(IEnumerable<Attribute> attributes) {
            // most generic <any/> matches everithig 
            XmlAnyElementAttribute wildcard = null;
            foreach(var attrib in attributes) { 
                if (attrib is XmlIgnoreAttribute || attrib is ObsoleteAttribute || attrib.GetType() == IgnoreAttribute) {
                    xmlIgnore = true;
                    break;
                }
                else if (attrib is XmlElementAttribute) {
                    this.xmlElements.Add((XmlElementAttribute)attrib);
                }
                else if (attrib is XmlArrayItemAttribute) {
                    this.xmlArrayItems.Add((XmlArrayItemAttribute)attrib);
                }
                else if (attrib is XmlAnyElementAttribute) {
                    XmlAnyElementAttribute any = (XmlAnyElementAttribute)attrib;
                    if ((any.Name == null || any.Name.Length == 0) && any.NamespaceSpecified && any.Namespace == null) {
                        // ignore duplicate wildcards
                        wildcard = any;
                    }
                    else {
                        this.xmlAnyElements.Add((XmlAnyElementAttribute)attrib);
                    }
                }
                else if (attrib is DefaultValueAttribute) {
                    this.xmlDefaultValue = ((DefaultValueAttribute)attrib).Value;
                }
                else if (attrib is XmlAttributeAttribute) {
                    this.xmlAttribute = (XmlAttributeAttribute)attrib;
                }
                else if (attrib is XmlArrayAttribute) {
                    this.xmlArray = (XmlArrayAttribute)attrib;
                }
                else if (attrib is XmlTextAttribute) {
                    this.xmlText = (XmlTextAttribute)attrib;
                }
                else if (attrib is XmlEnumAttribute) {
                    this.xmlEnum = (XmlEnumAttribute)attrib;
                }
                else if (attrib is XmlRootAttribute) {
                    this.xmlRoot = (XmlRootAttribute)attrib;
                }
                else if (attrib is XmlTypeAttribute) {
                    this.xmlType = (XmlTypeAttribute)attrib;
                }
                else if (attrib is XmlAnyAttributeAttribute) {
                    this.xmlAnyAttribute = (XmlAnyAttributeAttribute)attrib;
                }
                else if (attrib is XmlChoiceIdentifierAttribute) {
                    this.xmlChoiceIdentifier = (XmlChoiceIdentifierAttribute)attrib;
                }
                else if (attrib is XmlNamespaceDeclarationsAttribute) {
                    this.xmlns = true;
                }
            }
            if (xmlIgnore) {
                this.xmlElements.Clear();
                this.xmlArrayItems.Clear();
                this.xmlAnyElements.Clear();
                this.xmlDefaultValue = null;
                this.xmlAttribute = null;
                this.xmlArray = null;
                this.xmlText = null;
                this.xmlEnum = null;
                this.xmlType = null;
                this.xmlAnyAttribute = null;
                this.xmlChoiceIdentifier = null;
                this.xmlns = false;
            }
            else {
                if (wildcard != null) {
                    this.xmlAnyElements.Add(wildcard);
                }
            }
        }
        
        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlElements"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlElementAttributes XmlElements {
            get { return xmlElements; }
        }
        
        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlAttribute"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlAttributeAttribute XmlAttribute {
            get { return xmlAttribute; }
            set { xmlAttribute = value; }
        }
        
        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlEnum"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlEnumAttribute XmlEnum {
            get { return xmlEnum; }
            set { xmlEnum = value; }
        }
        
        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlText"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlTextAttribute XmlText {
            get { return xmlText; }
            set { xmlText = value; }
        }
        
        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlArray"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlArrayAttribute XmlArray {
            get { return xmlArray; }
            set { xmlArray = value; }
        }
        
        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlArrayItems"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlArrayItemAttributes XmlArrayItems {
            get { return xmlArrayItems; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlDefaultValue"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public object XmlDefaultValue {
            get { return xmlDefaultValue; }
            set { xmlDefaultValue = value; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlIgnore"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool XmlIgnore {
            get { return xmlIgnore; }
            set { xmlIgnore = value; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlType"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlTypeAttribute XmlType {
            get { return xmlType; }
            set { xmlType = value; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlRoot"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlRootAttribute XmlRoot {
            get { return xmlRoot; }
            set { xmlRoot = value; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlAnyElement"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlAnyElementAttributes XmlAnyElements {
            get { return xmlAnyElements; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlAnyAttribute"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlAnyAttributeAttribute XmlAnyAttribute {
            get { return xmlAnyAttribute; }
            set { xmlAnyAttribute = value; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlChoiceIdentifier"]/*' />
        public XmlChoiceIdentifierAttribute XmlChoiceIdentifier {
            get { return xmlChoiceIdentifier; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.Xmlns"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool Xmlns {
            get { return xmlns; }
            set { xmlns = value; }
        }

    }
}
