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
    ///       Represents a snippet block of code.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeSnippetCompileUnit : CodeCompileUnit
    {
        private string _value;
        private CodeLinePragma _linePragma;

        public CodeSnippetCompileUnit()
        {
        }

        public CodeSnippetCompileUnit(string value)
        {
            Value = value;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the snippet
        ///       text of the code block to represent.
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

        /// <devdoc>
        ///    <para>
        ///       The line the code block starts on.
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
    }
}
