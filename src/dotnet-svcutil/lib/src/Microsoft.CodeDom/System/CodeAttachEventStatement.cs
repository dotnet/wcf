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
    ///       Represents a event attach statement.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeAttachEventStatement : CodeStatement
    {
        private CodeEventReferenceExpression _eventRef;
        private CodeExpression _listener;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeAttachEventStatement'/>.
        ///    </para>
        /// </devdoc>
        public CodeAttachEventStatement()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='Microsoft.CodeDom.CodeAttachEventStatement'/> class using the specified arguments.
        ///    </para>
        /// </devdoc>
        public CodeAttachEventStatement(CodeEventReferenceExpression eventRef, CodeExpression listener)
        {
            _eventRef = eventRef;
            _listener = listener;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeAttachEventStatement(CodeExpression targetObject, string eventName, CodeExpression listener)
        {
            _eventRef = new CodeEventReferenceExpression(targetObject, eventName);
            _listener = listener;
        }

        /// <devdoc>
        ///    <para>
        ///       The event to attach a listener to.
        ///    </para>
        /// </devdoc>
        public CodeEventReferenceExpression Event
        {
            get
            {
                if (_eventRef == null)
                {
                    return new CodeEventReferenceExpression();
                }
                return _eventRef;
            }
            set
            {
                _eventRef = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       The new listener.
        ///    </para>
        /// </devdoc>
        public CodeExpression Listener
        {
            get
            {
                return _listener;
            }
            set
            {
                _listener = value;
            }
        }
    }
}
