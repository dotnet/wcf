// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema
{
    using System;
    using Microsoft.Xml;


    using Microsoft.Xml.Serialization;

    /// <include file='doc\XmlSchemaSimpleContentRestriction.uex' path='docs/doc[@for="XmlSchemaSimpleContentRestriction"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class XmlSchemaSimpleContentRestriction : XmlSchemaContent
    {
        private XmlQualifiedName _baseTypeName = XmlQualifiedName.Empty;
        private XmlSchemaSimpleType _baseType;
        private XmlSchemaObjectCollection _facets = new XmlSchemaObjectCollection();
        private XmlSchemaObjectCollection _attributes = new XmlSchemaObjectCollection();
        private XmlSchemaAnyAttribute _anyAttribute;

        /// <include file='doc\XmlSchemaSimpleContentRestriction.uex' path='docs/doc[@for="XmlSchemaSimpleContentRestriction.BaseTypeName"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlAttribute("base")]
        public XmlQualifiedName BaseTypeName
        {
            get { return _baseTypeName; }
            set { _baseTypeName = (value == null ? XmlQualifiedName.Empty : value); }
        }

        /// <include file='doc\XmlSchemaSimpleContentRestriction.uex' path='docs/doc[@for="XmlSchemaSimpleContentRestriction.BaseType"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
        public XmlSchemaSimpleType BaseType
        {
            get { return _baseType; }
            set { _baseType = value; }
        }

        /// <include file='doc\XmlSchemaSimpleContentRestriction.uex' path='docs/doc[@for="XmlSchemaSimpleContentRestriction.Facets"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlElement("length", typeof(XmlSchemaLengthFacet)),
         XmlElement("minLength", typeof(XmlSchemaMinLengthFacet)),
         XmlElement("maxLength", typeof(XmlSchemaMaxLengthFacet)),
         XmlElement("pattern", typeof(XmlSchemaPatternFacet)),
         XmlElement("enumeration", typeof(XmlSchemaEnumerationFacet)),
         XmlElement("maxInclusive", typeof(XmlSchemaMaxInclusiveFacet)),
         XmlElement("maxExclusive", typeof(XmlSchemaMaxExclusiveFacet)),
         XmlElement("minInclusive", typeof(XmlSchemaMinInclusiveFacet)),
         XmlElement("minExclusive", typeof(XmlSchemaMinExclusiveFacet)),
         XmlElement("totalDigits", typeof(XmlSchemaTotalDigitsFacet)),
         XmlElement("fractionDigits", typeof(XmlSchemaFractionDigitsFacet)),
         XmlElement("whiteSpace", typeof(XmlSchemaWhiteSpaceFacet))]
        public XmlSchemaObjectCollection Facets
        {
            get { return _facets; }
        }

        /// <include file='doc\XmlSchemaSimpleContentRestriction.uex' path='docs/doc[@for="XmlSchemaSimpleContentRestriction.Attributes"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlElement("attribute", typeof(XmlSchemaAttribute)),
         XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroupRef))]
        public XmlSchemaObjectCollection Attributes
        {
            get { return _attributes; }
        }

        /// <include file='doc\XmlSchemaSimpleContentRestriction.uex' path='docs/doc[@for="XmlSchemaSimpleContentRestriction.AnyAttribute"]/*' />
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

