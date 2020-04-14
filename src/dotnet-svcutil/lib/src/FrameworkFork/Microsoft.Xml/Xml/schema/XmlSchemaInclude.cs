// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema
{
    using System;
    using Microsoft.Xml;


    using Microsoft.Xml.Serialization;

    /// <include file='doc\XmlSchemaInclude.uex' path='docs/doc[@for="XmlSchemaInclude"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class XmlSchemaInclude : XmlSchemaExternal
    {
        private XmlSchemaAnnotation _annotation;

        /// <include file='doc\XmlSchemaInclude.uex' path='docs/doc[@for="XmlSchemaInclude.XmlSchemaInclude"]/*' />
        public XmlSchemaInclude()
        {
            Compositor = Compositor.Include;
        }
        /// <include file='doc\XmlSchemaInclude.uex' path='docs/doc[@for="XmlSchemaInclude.Annotation"]/*' />
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
