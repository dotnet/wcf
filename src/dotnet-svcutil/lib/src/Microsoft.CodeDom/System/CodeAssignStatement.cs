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
    ///       Represents a simple assignment statement.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeAssignStatement : CodeStatement
    {
        private CodeExpression _left;
        private CodeExpression _right;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeAssignStatement'/>.
        ///    </para>
        /// </devdoc>
        public CodeAssignStatement()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeAssignStatement'/> that represents the
        ///       specified assignment values.
        ///    </para>
        /// </devdoc>
        public CodeAssignStatement(CodeExpression left, CodeExpression right)
        {
            Left = left;
            Right = right;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the variable to be assigned to.
        ///    </para>
        /// </devdoc>
        public CodeExpression Left
        {
            get
            {
                return _left;
            }
            set
            {
                _left = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the value to assign.
        ///    </para>
        /// </devdoc>
        public CodeExpression Right
        {
            get
            {
                return _right;
            }
            set
            {
                _right = value;
            }
        }
    }
}
