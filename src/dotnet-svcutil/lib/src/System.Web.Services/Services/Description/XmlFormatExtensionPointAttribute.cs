// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Web.Services.Configuration
{
    using System;

    /// <include file='doc\XmlFormatExtensionPointAttribute.uex' path='docs/doc[@for="XmlFormatExtensionPointAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class XmlFormatExtensionPointAttribute : Attribute
    {
        private string _name;
        private bool _allowElements = true;

        /// <include file='doc\XmlFormatExtensionPointAttribute.uex' path='docs/doc[@for="XmlFormatExtensionPointAttribute.XmlFormatExtensionPointAttribute"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlFormatExtensionPointAttribute(string memberName)
        {
            _name = memberName;
        }

        /// <include file='doc\XmlFormatExtensionPointAttribute.uex' path='docs/doc[@for="XmlFormatExtensionPointAttribute.MemberName"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string MemberName
        {
            get { return _name == null ? string.Empty : _name; }
            set { _name = value; }
        }

        /// <include file='doc\XmlFormatExtensionPointAttribute.uex' path='docs/doc[@for="XmlFormatExtensionPointAttribute.AllowElements"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool AllowElements
        {
            get { return _allowElements; }
            set { _allowElements = value; }
        }
    }
}

