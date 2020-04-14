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
    ///    <para>
    ///       Represents a snippet expression.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeSnippetExpression : CodeExpression
    {
        private string _value;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeSnippetExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeSnippetExpression()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeSnippetExpression'/> using the specified snippet
        ///       expression.
        ///    </para>
        /// </devdoc>
        public CodeSnippetExpression(string value)
        {
            Value = value;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the snippet expression.
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
