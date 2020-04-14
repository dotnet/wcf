// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema
{
    using System;
    using Microsoft.Xml;


    using System.Collections;
    using System.ComponentModel;
    using Microsoft.Xml.Serialization;

    /// <include file='doc\XmlSchemaDocumentation.uex' path='docs/doc[@for="XmlSchemaDocumentation"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class XmlSchemaDocumentation : XmlSchemaObject
    {
        private string _source;
        private string _language;
        private XmlNode[] _markup;
        private static XmlSchemaSimpleType s_languageType = DatatypeImplementation.GetSimpleTypeFromXsdType(new XmlQualifiedName("language", XmlReservedNs.NsXs));

        /// <include file='doc\XmlSchemaDocumentation.uex' path='docs/doc[@for="XmlSchemaDocumentation.Source"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlAttribute("source", DataType = "anyURI")]
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        /// <include file='doc\XmlSchemaDocumentation.uex' path='docs/doc[@for="XmlSchemaDocumentation.Language"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlAttribute("xml:lang")]
        public string Language
        {
            get { return _language; }
            set { _language = (string)s_languageType.Datatype.ParseValue(value, (XmlNameTable)null, (IXmlNamespaceResolver)null); }
        }

        /// <include file='doc\XmlSchemaDocumentation.uex' path='docs/doc[@for="XmlSchemaDocumentation.Markup"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlText(), XmlAnyElement]
        public XmlNode[] Markup
        {
            get { return _markup; }
            set { _markup = value; }
        }
    }
}
