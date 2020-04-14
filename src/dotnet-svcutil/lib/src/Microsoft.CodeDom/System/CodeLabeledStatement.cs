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
    public class CodeLabeledStatement : CodeStatement
    {
        private string _label;
        private CodeStatement _statement;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeLabeledStatement()
        {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeLabeledStatement(string label)
        {
            _label = label;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeLabeledStatement(string label, CodeStatement statement)
        {
            _label = label;
            _statement = statement;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string Label
        {
            get
            {
                return (_label == null) ? string.Empty : _label;
            }
            set
            {
                _label = value;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeStatement Statement
        {
            get
            {
                return _statement;
            }
            set
            {
                _statement = value;
            }
        }
    }
}
