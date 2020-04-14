// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema
{
    using System;
    using Microsoft.Xml;


    using Microsoft.Xml.Serialization;

    /// <include file='doc\XmlSchemaImport.uex' path='docs/doc[@for="XmlSchemaImport"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class XmlSchemaImport : XmlSchemaExternal
    {
        private string _ns;
        private XmlSchemaAnnotation _annotation;

        /// <include file='doc\XmlSchemaImport.uex' path='docs/doc[@for="XmlSchemaImport.XmlSchemaImport"]/*' />
        public XmlSchemaImport()
        {
            Compositor = Compositor.Import;
        }

        /// <include file='doc\XmlSchemaImport.uex' path='docs/doc[@for="XmlSchemaImport.Namespace"]/*' />
        [XmlAttribute("namespace", DataType = "anyURI")]
        public string Namespace
        {
            get { return _ns; }
            set { _ns = value; }
        }

        /// <include file='doc\XmlSchemaImport.uex' path='docs/doc[@for="XmlSchemaImport.Annotation"]/*' />
        [XmlElement("annotation", typeof(XmlSchemaAnnotation))]
        public XmlSchemaAnnotation Annotation
        {
            get { return _annotation; }
            set { _annotation = value; }
        }

        internal override void AddAnnotation(XmlSchemaAnnotation annotation)
        {
            _annotation = annotation;
        }
    }
}
