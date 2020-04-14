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
    ///       Represents a statement.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeStatement : CodeObject
    {
        private CodeLinePragma _linePragma;

        // Optionally Serializable
        // [OptionalField]  // Not available in DNX (NetCore)
        private CodeDirectiveCollection _startDirectives = null;
        // [OptionalField]  // Not available in DNX (NetCore)
        private CodeDirectiveCollection _endDirectives = null;


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

        public CodeDirectiveCollection StartDirectives
        {
            get
            {
                if (_startDirectives == null)
                {
                    _startDirectives = new CodeDirectiveCollection();
                }
                return _startDirectives;
            }
        }

        public CodeDirectiveCollection EndDirectives
        {
            get
            {
                if (_endDirectives == null)
                {
                    _endDirectives = new CodeDirectiveCollection();
                }
                return _endDirectives;
            }
        }
    }
}
