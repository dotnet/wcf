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
    ///       Represents a reference to a property.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodePropertyReferenceExpression : CodeExpression
    {
        private CodeExpression _targetObject;
        private string _propertyName;
        private CodeExpressionCollection _parameters = new CodeExpressionCollection();

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodePropertyReferenceExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodePropertyReferenceExpression()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodePropertyReferenceExpression'/> using the specified target object and property
        ///       name.
        ///    </para>
        /// </devdoc>
        public CodePropertyReferenceExpression(CodeExpression targetObject, string propertyName)
        {
            TargetObject = targetObject;
            PropertyName = propertyName;
        }

        /// <devdoc>
        ///    <para>
        ///       The target object containing the property this <see cref='Microsoft.CodeDom.CodePropertyReferenceExpression'/> references.
        ///    </para>
        /// </devdoc>
        public CodeExpression TargetObject
        {
            get
            {
                return _targetObject;
            }
            set
            {
                _targetObject = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       The name of the property to reference.
        ///    </para>
        /// </devdoc>
        public string PropertyName
        {
            get
            {
                return (_propertyName == null) ? string.Empty : _propertyName;
            }
            set
            {
                _propertyName = value;
            }
        }
    }
}
