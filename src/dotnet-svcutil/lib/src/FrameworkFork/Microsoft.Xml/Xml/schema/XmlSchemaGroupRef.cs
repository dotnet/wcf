// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema
{
    using System;
    using Microsoft.Xml;


    using Microsoft.Xml.Serialization;

    /// <include file='doc\XmlSchemaGroupRef.uex' path='docs/doc[@for="XmlSchemaGroupRef"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class XmlSchemaGroupRef : XmlSchemaParticle
    {
        private XmlQualifiedName _refName = XmlQualifiedName.Empty;
        private XmlSchemaGroupBase _particle;
        private XmlSchemaGroup _refined;

        /// <include file='doc\XmlSchemaGroupRef.uex' path='docs/doc[@for="XmlSchemaGroupRef.RefName"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlAttribute("ref")]
        public XmlQualifiedName RefName
        {
            get { return _refName; }
            set { _refName = (value == null ? XmlQualifiedName.Empty : value); }
        }

        /// <include file='doc\XmlSchemaGroupRef.uex' path='docs/doc[@for="XmlSchemaGroupRef.Particle"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlIgnore]
        public XmlSchemaGroupBase Particle
        {
            get { return _particle; }
        }

        internal void SetParticle(XmlSchemaGroupBase value)
        {
            _particle = value;
        }

        [XmlIgnore]
        internal XmlSchemaGroup Redefined
        {
            get { return _refined; }
            set { _refined = value; }
        }
    }
}
