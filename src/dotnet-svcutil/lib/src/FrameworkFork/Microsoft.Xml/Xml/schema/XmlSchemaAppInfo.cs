// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema
{
    using System;
    using Microsoft.Xml;


    using System.Collections;
    using Microsoft.Xml.Serialization;

    /// <include file='doc\XmlSchemaAppInfo.uex' path='docs/doc[@for="XmlSchemaAppInfo"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class XmlSchemaAppInfo : XmlSchemaObject
    {
        private string _source;
        private XmlNode[] _markup;

        /// <include file='doc\XmlSchemaAppInfo.uex' path='docs/doc[@for="XmlSchemaAppInfo.Source"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlAttribute("source", DataType = "anyURI")]
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        /// <include file='doc\XmlSchemaAppInfo.uex' path='docs/doc[@for="XmlSchemaAppInfo.Markup"]/*' />
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
