// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.CodeDom {

    using System.Diagnostics;
    using System;
    using Microsoft.Win32;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <devdoc>
    ///    <para>
    ///       Represents a
    ///       snippet member of a class.
    ///    </para>
    /// </devdoc>
    [
       //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
        // Serializable,
    ]
    public class CodeSnippetTypeMember : CodeTypeMember {
        private string text;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeSnippetTypeMember'/>.
        ///    </para>
        /// </devdoc>
        public CodeSnippetTypeMember() {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeSnippetTypeMember'/>.
        ///    </para>
        /// </devdoc>
        public CodeSnippetTypeMember(string text) {
            Text = text;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the code for the class member.
        ///    </para>
        /// </devdoc>
        public string Text {
            get {
                return (text == null) ? string.Empty : text;
            }
            set {
                text = value;
            }
        }
    }
}

