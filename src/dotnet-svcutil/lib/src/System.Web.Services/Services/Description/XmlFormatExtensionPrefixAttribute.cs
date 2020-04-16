// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Web.Services.Configuration
{
    using System;

    /// <include file='doc\XmlFormatExtensionPrefixAttribute.uex' path='docs/doc[@for="XmlFormatExtensionPrefixAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class XmlFormatExtensionPrefixAttribute : Attribute
    {
        private string _prefix;
        private string _ns;

        /// <include file='doc\XmlFormatExtensionPrefixAttribute.uex' path='docs/doc[@for="XmlFormatExtensionPrefixAttribute.XmlFormatExtensionPrefixAttribute"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlFormatExtensionPrefixAttribute()
        {
        }

        /// <include file='doc\XmlFormatExtensionPrefixAttribute.uex' path='docs/doc[@for="XmlFormatExtensionPrefixAttribute.XmlFormatExtensionPrefixAttribute1"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlFormatExtensionPrefixAttribute(string prefix, string ns)
        {
            _prefix = prefix;
            _ns = ns;
        }

        /// <include file='doc\XmlFormatExtensionPrefixAttribute.uex' path='docs/doc[@for="XmlFormatExtensionPrefixAttribute.Prefix"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string Prefix
        {
            get { return _prefix == null ? string.Empty : _prefix; }
            set { _prefix = value; }
        }

        /// <include file='doc\XmlFormatExtensionPrefixAttribute.uex' path='docs/doc[@for="XmlFormatExtensionPrefixAttribute.Namespace"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string Namespace
        {
            get { return _ns == null ? string.Empty : _ns; }
            set { _ns = value; }
        }
    }
}

