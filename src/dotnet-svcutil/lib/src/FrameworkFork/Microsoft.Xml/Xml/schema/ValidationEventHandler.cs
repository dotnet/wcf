//------------------------------------------------------------------------------
// <copyright file="ValidationEventHandler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright> 
// <owner current="true" primary="true">priyal</owner>                                                               
//------------------------------------------------------------------------------

namespace Microsoft.Xml.Schema {
				using System;
				using Microsoft.Xml;

    /// <include file='doc\ValidationEventHandler.uex' path='docs/doc[@for="ValidationEventHandler"]/*' />
    /// <devdoc>
    /// </devdoc>
    public delegate void ValidationEventHandler( object sender, ValidationEventArgs e );
}
