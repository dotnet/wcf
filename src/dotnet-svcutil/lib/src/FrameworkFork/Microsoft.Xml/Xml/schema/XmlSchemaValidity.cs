//------------------------------------------------------------------------------
// <copyright file="XmlSchemaValidity.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">priyal</owner> 
//------------------------------------------------------------------------------

namespace Microsoft.Xml.Schema {
				using System;
				using Microsoft.Xml;


    /// <include file='doc\XmlSchemaValidity.uex' path='docs/doc[@for="XmlSchemaValidity"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public enum XmlSchemaValidity
    {
        /// <include file='doc\XmlSchemaValidity.uex' path='docs/doc[@for="XmlSchemaValidity.NotKnown"]/*' />
        NotKnown,
        /// <include file='doc\XmlSchemaValidity.uex' path='docs/doc[@for="XmlSchemaValidity.Valid"]/*' />
        Valid,
        /// <include file='doc\XmlSchemaValidity.uex' path='docs/doc[@for="XmlSchemaValidity.Invalid"]/*' />
        Invalid,
    }
}
