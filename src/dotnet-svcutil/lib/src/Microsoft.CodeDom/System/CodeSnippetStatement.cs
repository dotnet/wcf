// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeDom
{
    using System.Diagnostics;
    using System;
    using Microsoft.Win32;
    using System.Collections;
    using System.Runtime.InteropServices;

    /// <devdoc>
    ///    <para>
    ///       Represents a snippet statement.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeSnippetStatement : CodeStatement
    {
        private string _value;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeSnippetStatement'/>.
        ///    </para>
        /// </devdoc>
        public CodeSnippetStatement()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeSnippetStatement'/> using the specified snippet
        ///       of code.
        ///    </para>
        /// </devdoc>
        public CodeSnippetStatement(string value)
        {
            Value = value;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the snippet statement.
        ///    </para>
        /// </devdoc>
        public string Value
        {
            get
            {
                return (_value == null) ? string.Empty : _value;
            }
            set
            {
                _value = value;
            }
        }
    }
}
