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
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeDirectionExpression : CodeExpression
    {
        private CodeExpression _expression;
        private FieldDirection _direction = FieldDirection.In;


        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeDirectionExpression()
        {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeDirectionExpression(FieldDirection direction, CodeExpression expression)
        {
            _expression = expression;
            _direction = direction;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeExpression Expression
        {
            get
            {
                return _expression;
            }
            set
            {
                _expression = value;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public FieldDirection Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }
    }
}
