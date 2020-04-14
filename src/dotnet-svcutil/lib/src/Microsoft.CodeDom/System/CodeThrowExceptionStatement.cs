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
    ///       Represents
    ///       a statement that throws an exception.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeThrowExceptionStatement : CodeStatement
    {
        private CodeExpression _toThrow;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeThrowExceptionStatement'/>.
        ///    </para>
        /// </devdoc>
        public CodeThrowExceptionStatement()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeThrowExceptionStatement'/> using the specified statement.
        ///    </para>
        /// </devdoc>
        public CodeThrowExceptionStatement(CodeExpression toThrow)
        {
            ToThrow = toThrow;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the expression to throw.
        ///    </para>
        /// </devdoc>
        public CodeExpression ToThrow
        {
            get
            {
                return _toThrow;
            }
            set
            {
                _toThrow = value;
            }
        }
    }
}
