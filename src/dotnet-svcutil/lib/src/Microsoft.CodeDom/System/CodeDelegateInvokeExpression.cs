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
    ///       Represents an
    ///       expression that invokes a delegate.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeDelegateInvokeExpression : CodeExpression
    {
        private CodeExpression _targetObject;
        private CodeExpressionCollection _parameters = new CodeExpressionCollection();

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeDelegateInvokeExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeDelegateInvokeExpression()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeDelegateInvokeExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeDelegateInvokeExpression(CodeExpression targetObject)
        {
            TargetObject = targetObject;
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeDelegateInvokeExpression'/>
        ///       .
        ///    </para>
        /// </devdoc>
        public CodeDelegateInvokeExpression(CodeExpression targetObject, params CodeExpression[] parameters)
        {
            TargetObject = targetObject;
            Parameters.AddRange(parameters);
        }

        /// <devdoc>
        ///    <para>
        ///       The
        ///       delegate's target object.
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
        ///       The
        ///       delegate parameters.
        ///    </para>
        /// </devdoc>
        public CodeExpressionCollection Parameters
        {
            get
            {
                return _parameters;
            }
        }
    }
}
