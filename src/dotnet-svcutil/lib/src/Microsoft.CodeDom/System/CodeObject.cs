// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.CodeDom
{
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
    public class CodeObject
    {
        private IDictionary _userData = null;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeObject()
        {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public IDictionary UserData
        {
            get
            {
                if (_userData == null)
                {
                    _userData = new ListDictionary();
                }
                return _userData;
            }
        }
    }
}
