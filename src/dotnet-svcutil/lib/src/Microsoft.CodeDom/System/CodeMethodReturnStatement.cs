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
    ///       Represents a return statement.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeMethodReturnStatement : CodeStatement
    {
        private CodeExpression _expression;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeMethodReturnStatement'/>.
        ///    </para>
        /// </devdoc>
        public CodeMethodReturnStatement()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeMethodReturnStatement'/> using the specified expression.
        ///    </para>
        /// </devdoc>
        public CodeMethodReturnStatement(CodeExpression expression)
        {
            Expression = expression;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the expression that indicates the return statement.
        ///    </para>
        /// </devdoc>
        public CodeExpression Expression
        {
            get
            {
                return _expression;
            }
            set
            {
                _expression = value;
            }
        }
    }
}
