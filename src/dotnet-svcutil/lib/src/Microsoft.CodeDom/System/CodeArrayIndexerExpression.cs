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
    ///       Represents an array indexer expression.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeArrayIndexerExpression : CodeExpression
    {
        private CodeExpression _targetObject;
        private CodeExpressionCollection _indices;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeArrayIndexerExpression()
        {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeArrayIndexerExpression(CodeExpression targetObject, params CodeExpression[] indices)
        {
            _targetObject = targetObject;
            _indices = new CodeExpressionCollection();
            _indices.AddRange(indices);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
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
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeExpressionCollection Indices
        {
            get
            {
                if (_indices == null)
                {
                    _indices = new CodeExpressionCollection();
                }
                return _indices;
            }
        }
    }
}
