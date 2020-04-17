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
    ///    <para>Represents a catch exception block.</para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeCatchClause
    {
        private CodeStatementCollection _statements;
        private CodeTypeReference _catchExceptionType;
        private string _localName;

        /// <devdoc>
        ///    <para>
        ///       Initializes an instance of <see cref='Microsoft.CodeDom.CodeCatchClause'/>.
        ///    </para>
        /// </devdoc>
        public CodeCatchClause()
        {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeCatchClause(string localName)
        {
            _localName = localName;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeCatchClause(string localName, CodeTypeReference catchExceptionType)
        {
            _localName = localName;
            _catchExceptionType = catchExceptionType;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeCatchClause(string localName, CodeTypeReference catchExceptionType, params CodeStatement[] statements)
        {
            _localName = localName;
            _catchExceptionType = catchExceptionType;
            Statements.AddRange(statements);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string LocalName
        {
            get
            {
                return (_localName == null) ? string.Empty : _localName;
            }
            set
            {
                _localName = value;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeTypeReference CatchExceptionType
        {
            get
            {
                if (_catchExceptionType == null)
                {
                    _catchExceptionType = new CodeTypeReference(typeof(System.Exception));
                }
                return _catchExceptionType;
            }
            set
            {
                _catchExceptionType = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the statements within the clause.
        ///    </para>
        /// </devdoc>
        public CodeStatementCollection Statements
        {
            get
            {
                if (_statements == null)
                {
                    _statements = new CodeStatementCollection();
                }
                return _statements;
            }
        }
    }
}
