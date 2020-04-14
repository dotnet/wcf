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
    ///       Represents an event member.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeMemberEvent : CodeTypeMember
    {
        private CodeTypeReference _type;
        private CodeTypeReference _privateImplements = null;
        private CodeTypeReferenceCollection _implementationTypes = null;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeMemberEvent()
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the member field type.
        ///    </para>
        /// </devdoc>
        public CodeTypeReference Type
        {
            get
            {
                if (_type == null)
                {
                    _type = new CodeTypeReference("");
                }
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeTypeReference PrivateImplementationType
        {
            get
            {
                return _privateImplements;
            }
            set
            {
                _privateImplements = value;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeTypeReferenceCollection ImplementationTypes
        {
            get
            {
                if (_implementationTypes == null)
                {
                    _implementationTypes = new CodeTypeReferenceCollection();
                }
                return _implementationTypes;
            }
        }
    }
}
