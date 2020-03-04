// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema {
				using System;
				using Microsoft.Xml;


    using Microsoft.Xml.Serialization;

    /// <include file='doc\XmlSchemaContentProcessing.uex' path='docs/doc[@for="XmlSchemaContentProcessing"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public enum XmlSchemaContentProcessing {
        /// <include file='doc\XmlSchemaContentProcessing.uex' path='docs/doc[@for="XmlSchemaContentProcessing.None"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlIgnore]
        None,
        /// <include file='doc\XmlSchemaContentProcessing.uex' path='docs/doc[@for="XmlSchemaContentProcessing.XmlEnum"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlEnum("skip")]
        Skip,
        /// <include file='doc\XmlSchemaContentProcessing.uex' path='docs/doc[@for="XmlSchemaContentProcessing.XmlEnum1"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlEnum("lax")]
        Lax,
        /// <include file='doc\XmlSchemaContentProcessing.uex' path='docs/doc[@for="XmlSchemaContentProcessing.XmlEnum2"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlEnum("strict")]
        Strict
    }
}
