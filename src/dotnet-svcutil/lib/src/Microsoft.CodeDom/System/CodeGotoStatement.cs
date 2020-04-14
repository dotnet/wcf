// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.CodeDom
{
    using System.Diagnostics;
    using System;
    using Microsoft.Win32;
    using System.Collections;
    using System.Runtime.InteropServices;

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeGotoStatement : CodeStatement
    {
        private string _label;

        public CodeGotoStatement()
        {
        }

        public CodeGotoStatement(string label)
        {
            Label = label;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string Label
        {
            get
            {
                return _label;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");

                _label = value;
            }
        }
    }
}
