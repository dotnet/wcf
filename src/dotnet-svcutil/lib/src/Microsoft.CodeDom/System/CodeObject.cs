//------------------------------------------------------------------------------
// <copyright file="CodeObject.cs" company="Microsoft">
// 
// <OWNER>petes</OWNER>
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace Microsoft.CodeDom {

    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Runtime.InteropServices;

    /// <devdoc>
    ///    <para>
    ///       The base class for CodeDom objects
    ///    </para>
    /// </devdoc>
    [
       //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
        // Serializable,
    ]
    public class CodeObject {
        private IDictionary userData = null;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeObject() {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public IDictionary UserData {
            get {
                if (userData == null) {
                    userData = new ListDictionary();
                }
                return userData;
            }
        }
    }
}
