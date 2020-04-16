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
    ///     Represents a try block, with any number of catch clauses and an
    ///     optionally finally block.
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeTryCatchFinallyStatement : CodeStatement
    {
        private CodeStatementCollection _tryStatments = new CodeStatementCollection();
        private CodeStatementCollection _finallyStatments = new CodeStatementCollection();
        private CodeCatchClauseCollection _catchClauses = new CodeCatchClauseCollection();

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeTryCatchFinallyStatement'/>.
        ///    </para>
        /// </devdoc>
        public CodeTryCatchFinallyStatement()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeTryCatchFinallyStatement'/> using the specified statements to try and catch
        ///       clauses.
        ///    </para>
        /// </devdoc>
        public CodeTryCatchFinallyStatement(CodeStatement[] tryStatements, CodeCatchClause[] catchClauses)
        {
            TryStatements.AddRange(tryStatements);
            CatchClauses.AddRange(catchClauses);
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeTryCatchFinallyStatement'/> using the specified statements to
        ///       try, catch clauses, and finally statements.
        ///    </para>
        /// </devdoc>
        public CodeTryCatchFinallyStatement(CodeStatement[] tryStatements, CodeCatchClause[] catchClauses, CodeStatement[] finallyStatements)
        {
            TryStatements.AddRange(tryStatements);
            CatchClauses.AddRange(catchClauses);
            FinallyStatements.AddRange(finallyStatements);
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the try statements to try.
        ///    </para>
        /// </devdoc>
        public CodeStatementCollection TryStatements
        {
            get
            {
                return _tryStatments;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the catch clauses to use.
        ///    </para>
        /// </devdoc>
        public CodeCatchClauseCollection CatchClauses
        {
            get
            {
                return _catchClauses;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the finally statements to use.
        ///    </para>
        /// </devdoc>
        public CodeStatementCollection FinallyStatements
        {
            get
            {
                return _finallyStatments;
            }
        }
    }
}
