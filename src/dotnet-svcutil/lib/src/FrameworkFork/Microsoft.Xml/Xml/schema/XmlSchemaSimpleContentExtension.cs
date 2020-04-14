// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema
{
    using System;
    using Microsoft.Xml;


    using Microsoft.Xml.Serialization;

    /// <include file='doc\XmlSchemaSimpleContentExtension.uex' path='docs/doc[@for="XmlSchemaSimpleContentExtension"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class XmlSchemaSimpleContentExtension : XmlSchemaContent
    {
        private XmlSchemaObjectCollection _attributes = new XmlSchemaObjectCollection();
        private XmlSchemaAnyAttribute _anyAttribute;
        private XmlQualifiedName _baseTypeName = XmlQualifiedName.Empty;

        /// <include file='doc\XmlSchemaSimpleContentExtension.uex' path='docs/doc[@for="XmlSchemaSimpleContentExtension.BaseTypeName"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlAttribute("base")]
        public XmlQualifiedName BaseTypeName
        {
            get { return _baseTypeName; }
            set { _baseTypeName = (value == null ? XmlQualifiedName.Empty : value); }
        }

        /// <include file='doc\XmlSchemaSimpleContentExtension.uex' path='docs/doc[@for="XmlSchemaSimpleContentExtension.Attributes"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlElement("attribute", typeof(XmlSchemaAttribute)),
         XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroupRef))]
        public XmlSchemaObjectCollection Attributes
        {
            get { return _attributes; }
        }

        /// <include file='doc\XmlSchemaSimpleContentExtension.uex' path='docs/doc[@for="XmlSchemaSimpleContentExtension.AnyAttribute"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlElement("anyAttribute")]
        public XmlSchemaAnyAttribute AnyAttribute
        {
            get { return _anyAttribute; }
            set { _anyAttribute = value; }
        }

        internal void SetAttributes(XmlSchemaObjectCollection newAttributes)
        {
            _attributes = newAttributes;
        }
    }
}
