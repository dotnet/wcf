// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema
{
    using System;
    using Microsoft.Xml;


    using Microsoft.Xml.Serialization;
    using System.Diagnostics;

    /// <include file='doc\XmlSchemaSimpleType.uex' path='docs/doc[@for="XmlSchemaSimpleType"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class XmlSchemaSimpleType : XmlSchemaType
    {
        private XmlSchemaSimpleTypeContent _content;

        /// <include file='doc\XmlSchemaSimpleType.uex' path='docs/doc[@for="XmlSchemaSimpleType.Content"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>

        public XmlSchemaSimpleType()
        {
            Debug.Assert(SchemaContentType == XmlSchemaContentType.TextOnly);
        }

        /// <include file='doc\XmlSchemaSimpleType.uex' path='docs/doc[@for="XmlSchemaSimpleType.Content1"]/*' />
        [XmlElement("restriction", typeof(XmlSchemaSimpleTypeRestriction)),
        XmlElement("list", typeof(XmlSchemaSimpleTypeList)),
        XmlElement("union", typeof(XmlSchemaSimpleTypeUnion))]
        public XmlSchemaSimpleTypeContent Content
        {
            get { return _content; }
            set { _content = value; }
        }

        internal override XmlQualifiedName DerivedFrom
        {
            get
            {
                if (_content == null)
                {
                    // type derived from anyType
                    return XmlQualifiedName.Empty;
                }
                if (_content is XmlSchemaSimpleTypeRestriction)
                {
                    return ((XmlSchemaSimpleTypeRestriction)_content).BaseTypeName;
                }
                return XmlQualifiedName.Empty;
            }
        }

        internal override XmlSchemaObject Clone()
        {
            XmlSchemaSimpleType newSimpleType = (XmlSchemaSimpleType)MemberwiseClone();
            if (_content != null)
            {
                newSimpleType.Content = (XmlSchemaSimpleTypeContent)_content.Clone();
            }
            return newSimpleType;
        }
    }
}

