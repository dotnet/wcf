// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema
{
    using System;
    using Microsoft.Xml;


    using Microsoft.Xml.Serialization;

    /// <include file='doc\XmlSchemaRedefine.uex' path='docs/doc[@for="XmlSchemaRedefine"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class XmlSchemaRedefine : XmlSchemaExternal
    {
        private XmlSchemaObjectCollection _items = new XmlSchemaObjectCollection();
        private XmlSchemaObjectTable _attributeGroups = new XmlSchemaObjectTable();
        private XmlSchemaObjectTable _types = new XmlSchemaObjectTable();
        private XmlSchemaObjectTable _groups = new XmlSchemaObjectTable();


        /// <include file='doc\XmlSchemaRedefine.uex' path='docs/doc[@for="XmlSchemaRedefine.XmlSchemaRedefine"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlSchemaRedefine()
        {
            Compositor = Compositor.Redefine;
        }

        /// <include file='doc\XmlSchemaRedefine.uex' path='docs/doc[@for="XmlSchemaRedefine.Items"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlElement("annotation", typeof(XmlSchemaAnnotation)),
         XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroup)),
         XmlElement("complexType", typeof(XmlSchemaComplexType)),
         XmlElement("group", typeof(XmlSchemaGroup)),
         XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
        public XmlSchemaObjectCollection Items
        {
            get { return _items; }
        }

        /// <include file='doc\XmlSchemaRedefine.uex' path='docs/doc[@for="XmlSchemaRedefine.AttributeGroups"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlIgnore]
        public XmlSchemaObjectTable AttributeGroups
        {
            get { return _attributeGroups; }
        }

        /// <include file='doc\XmlSchemaRedefine.uex' path='docs/doc[@for="XmlSchemaRedefine.SchemaTypes"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlIgnore]
        public XmlSchemaObjectTable SchemaTypes
        {
            get { return _types; }
        }

        /// <include file='doc\XmlSchemaRedefine.uex' path='docs/doc[@for="XmlSchemaRedefine.Groups"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlIgnore]
        public XmlSchemaObjectTable Groups
        {
            get { return _groups; }
        }

        internal override void AddAnnotation(XmlSchemaAnnotation annotation)
        {
            _items.Add(annotation);
        }
    }
}
