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
    ///       Represents a namespace import into the current namespace.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeNamespaceImport : CodeObject
    {
        private string _nameSpace;
        private CodeLinePragma _linePragma;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeNamespaceImport'/>.
        ///    </para>
        /// </devdoc>
        public CodeNamespaceImport()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeNamespaceImport'/> using the specified namespace
        ///       to import.
        ///    </para>
        /// </devdoc>
        public CodeNamespaceImport(string nameSpace)
        {
            Namespace = nameSpace;
        }

        /// <devdoc>
        ///    <para>
        ///       The line the statement occurs on.
        ///    </para>
        /// </devdoc>
        public CodeLinePragma LinePragma
        {
            get
            {
                return _linePragma;
            }
            set
            {
                _linePragma = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the namespace to import.
        ///    </para>
        /// </devdoc>
        public string Namespace
        {
            get
            {
                return (_nameSpace == null) ? string.Empty : _nameSpace;
            }
            set
            {
                _nameSpace = value;
            }
        }
    }
}
