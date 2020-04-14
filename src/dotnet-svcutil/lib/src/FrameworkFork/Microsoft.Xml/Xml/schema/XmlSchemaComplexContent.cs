// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema
{
    using System;
    using Microsoft.Xml;


    using Microsoft.Xml.Serialization;

    /// <include file='doc\XmlSchemaComplexContent.uex' path='docs/doc[@for="XmlSchemaComplexContent"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class XmlSchemaComplexContent : XmlSchemaContentModel
    {
        private XmlSchemaContent _content;
        private bool _isMixed;
        private bool _hasMixedAttribute;

        /// <include file='doc\XmlSchemaComplexContent.uex' path='docs/doc[@for="XmlSchemaComplexContent.IsMixed"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlAttribute("mixed")]
        public bool IsMixed
        {
            get { return _isMixed; }
            set { _isMixed = value; _hasMixedAttribute = true; }
        }

        /// <include file='doc\XmlSchemaComplexContent.uex' path='docs/doc[@for="XmlSchemaComplexContent.Content"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlElement("restriction", typeof(XmlSchemaComplexContentRestriction)),
         XmlElement("extension", typeof(XmlSchemaComplexContentExtension))]
        public override XmlSchemaContent Content
        {
            get { return _content; }
            set { _content = value; }
        }

        [XmlIgnore]
        internal bool HasMixedAttribute
        {
            get { return _hasMixedAttribute; }
        }
    }
}
