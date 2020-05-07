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
    ///       Represents a delegate creation expression.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeDelegateCreateExpression : CodeExpression
    {
        private CodeTypeReference _delegateType;
        private CodeExpression _targetObject;
        private string _methodName;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeDelegateCreateExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeDelegateCreateExpression()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeDelegateCreateExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeDelegateCreateExpression(CodeTypeReference delegateType, CodeExpression targetObject, string methodName)
        {
            _delegateType = delegateType;
            _targetObject = targetObject;
            _methodName = methodName;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the delegate type.
        ///    </para>
        /// </devdoc>
        public CodeTypeReference DelegateType
        {
            get
            {
                if (_delegateType == null)
                {
                    _delegateType = new CodeTypeReference("");
                }
                return _delegateType;
            }
            set
            {
                _delegateType = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the target object.
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
        ///       Gets or sets the method name.
        ///    </para>
        /// </devdoc>
        public string MethodName
        {
            get
            {
                return (_methodName == null) ? string.Empty : _methodName;
            }
            set
            {
                _methodName = value;
            }
        }
    }
}
