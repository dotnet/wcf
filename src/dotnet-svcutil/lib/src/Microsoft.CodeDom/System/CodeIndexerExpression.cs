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
    public class CodeIndexerExpression : CodeExpression
    {
        private CodeExpression _targetObject;
        private CodeExpressionCollection _indices;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeIndexerExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeIndexerExpression()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeIndexerExpression'/> using the specified target
        ///       object and index.
        ///    </para>
        /// </devdoc>
        public CodeIndexerExpression(CodeExpression targetObject, params CodeExpression[] indices)
        {
            _targetObject = targetObject;
            _indices = new CodeExpressionCollection();
            _indices.AddRange(indices);
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the target object.
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
        ///       Gets or sets
        ///       the index.
        ///    </para>
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
