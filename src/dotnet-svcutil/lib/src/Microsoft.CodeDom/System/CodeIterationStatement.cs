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
    ///       Represents a simple for loop.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeIterationStatement : CodeStatement
    {
        private CodeStatement _initStatement;
        private CodeExpression _testExpression;
        private CodeStatement _incrementStatement;
        private CodeStatementCollection _statements = new CodeStatementCollection();

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeIterationStatement'/>.
        ///    </para>
        /// </devdoc>
        public CodeIterationStatement()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeIterationStatement'/>.
        ///    </para>
        /// </devdoc>
        public CodeIterationStatement(CodeStatement initStatement, CodeExpression testExpression, CodeStatement incrementStatement, params CodeStatement[] statements)
        {
            InitStatement = initStatement;
            TestExpression = testExpression;
            IncrementStatement = incrementStatement;
            Statements.AddRange(statements);
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the loop initialization statement.
        ///    </para>
        /// </devdoc>
        public CodeStatement InitStatement
        {
            get
            {
                return _initStatement;
            }
            set
            {
                _initStatement = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the expression to test for.
        ///    </para>
        /// </devdoc>
        public CodeExpression TestExpression
        {
            get
            {
                return _testExpression;
            }
            set
            {
                _testExpression = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the per loop cycle increment statement.
        ///    </para>
        /// </devdoc>
        public CodeStatement IncrementStatement
        {
            get
            {
                return _incrementStatement;
            }
            set
            {
                _incrementStatement = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the statements to be executed within the loop.
        ///    </para>
        /// </devdoc>
        public CodeStatementCollection Statements
        {
            get
            {
                return _statements;
            }
        }
    }
}
