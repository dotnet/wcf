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
    public class CodeEventReferenceExpression : CodeExpression
    {
        private CodeExpression _targetObject;
        private string _eventName;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeEventReferenceExpression()
        {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeEventReferenceExpression(CodeExpression targetObject, string eventName)
        {
            _targetObject = targetObject;
            _eventName = eventName;
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
        public string EventName
        {
            get
            {
                return (_eventName == null) ? string.Empty : _eventName;
            }
            set
            {
                _eventName = value;
            }
        }
    }
}
