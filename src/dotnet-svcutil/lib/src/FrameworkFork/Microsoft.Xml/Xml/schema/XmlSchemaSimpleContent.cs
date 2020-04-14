// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema
{
    using System;
    using Microsoft.Xml;


    using Microsoft.Xml.Serialization;

    /// <include file='doc\XmlSchemaSimpleContent.uex' path='docs/doc[@for="XmlSchemaSimpleContent"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class XmlSchemaSimpleContent : XmlSchemaContentModel
    {
        private XmlSchemaContent _content;

        /// <include file='doc\XmlSchemaSimpleContent.uex' path='docs/doc[@for="XmlSchemaSimpleContent.Content"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlElement("restriction", typeof(XmlSchemaSimpleContentRestriction)),
         XmlElement("extension", typeof(XmlSchemaSimpleContentExtension))]
        public override XmlSchemaContent Content
        {
            get { return _content; }
            set { _content = value; }
        }
    }
}
