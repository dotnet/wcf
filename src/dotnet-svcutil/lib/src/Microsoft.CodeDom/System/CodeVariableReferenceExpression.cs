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
    ///       Represents a reference to a field.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeVariableReferenceExpression : CodeExpression
    {
        private string _variableName;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeVariableReferenceExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeVariableReferenceExpression()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeArgumentReferenceExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeVariableReferenceExpression(string variableName)
        {
            _variableName = variableName;
        }


        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string VariableName
        {
            get
            {
                return (_variableName == null) ? string.Empty : _variableName;
            }
            set
            {
                _variableName = value;
            }
        }
    }
}
