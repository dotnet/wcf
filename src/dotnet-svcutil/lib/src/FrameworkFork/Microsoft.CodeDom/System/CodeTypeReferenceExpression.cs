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
    ///       Represents a reference to a type.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeTypeReferenceExpression : CodeExpression
    {
        private CodeTypeReference _type;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeTypeReferenceExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeTypeReferenceExpression()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeTypeReferenceExpression'/> using the specified type.
        ///    </para>
        /// </devdoc>
        public CodeTypeReferenceExpression(CodeTypeReference type)
        {
            Type = type;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeTypeReferenceExpression(string type)
        {
            Type = new CodeTypeReference(type);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeTypeReferenceExpression(Type type)
        {
            Type = new CodeTypeReference(type);
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the type to reference.
        ///    </para>
        /// </devdoc>
        public CodeTypeReference Type
        {
            get
            {
                if (_type == null)
                {
                    _type = new CodeTypeReference("");
                }
                return _type;
            }
            set
            {
                _type = value;
            }
        }
    }
}
