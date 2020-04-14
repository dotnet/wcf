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
    ///    <para> Represents a comment.</para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeComment : CodeObject
    {
        private string _text;
        private bool _docComment = false;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeComment'/>.
        ///    </para>
        /// </devdoc>
        public CodeComment()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeComment'/> with the specified text as
        ///       contents.
        ///    </para>
        /// </devdoc>
        public CodeComment(string text)
        {
            Text = text;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeComment(string text, bool docComment)
        {
            Text = text;
            _docComment = docComment;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool DocComment
        {
            get
            {
                return _docComment;
            }
            set
            {
                _docComment = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or setes
        ///       the text of the comment.
        ///    </para>
        /// </devdoc>
        public string Text
        {
            get
            {
                return (_text == null) ? string.Empty : _text;
            }
            set
            {
                _text = value;
            }
        }
    }
}
