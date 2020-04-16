// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Web.Services.Configuration
{
    using System;

    /// <include file='doc\XmlFormatExtensionAttribute.uex' path='docs/doc[@for="XmlFormatExtensionAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class XmlFormatExtensionAttribute : Attribute
    {
        private Type[] _types;
        private string _name;
        private string _ns;

        /// <include file='doc\XmlFormatExtensionAttribute.uex' path='docs/doc[@for="XmlFormatExtensionAttribute.XmlFormatExtensionAttribute"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlFormatExtensionAttribute()
        {
        }

        /// <include file='doc\XmlFormatExtensionAttribute.uex' path='docs/doc[@for="XmlFormatExtensionAttribute.XmlFormatExtensionAttribute2"]/*' />
        public XmlFormatExtensionAttribute(string elementName, string ns, Type extensionPoint1) : this(elementName, ns, new Type[] { extensionPoint1 })
        {
        }
        /// <include file='doc\XmlFormatExtensionAttribute.uex' path='docs/doc[@for="XmlFormatExtensionAttribute.XmlFormatExtensionAttribute3"]/*' />
        public XmlFormatExtensionAttribute(string elementName, string ns, Type extensionPoint1, Type extensionPoint2) : this(elementName, ns, new Type[] { extensionPoint1, extensionPoint2 })
        {
        }
        /// <include file='doc\XmlFormatExtensionAttribute.uex' path='docs/doc[@for="XmlFormatExtensionAttribute.XmlFormatExtensionAttribute4"]/*' />
        public XmlFormatExtensionAttribute(string elementName, string ns, Type extensionPoint1, Type extensionPoint2, Type extensionPoint3) : this(elementName, ns, new Type[] { extensionPoint1, extensionPoint2, extensionPoint3 })
        {
        }
        /// <include file='doc\XmlFormatExtensionAttribute.uex' path='docs/doc[@for="XmlFormatExtensionAttribute.XmlFormatExtensionAttribute5"]/*' />
        public XmlFormatExtensionAttribute(string elementName, string ns, Type extensionPoint1, Type extensionPoint2, Type extensionPoint3, Type extensionPoint4) : this(elementName, ns, new Type[] { extensionPoint1, extensionPoint2, extensionPoint3, extensionPoint4 })
        {
        }

        /// <include file='doc\XmlFormatExtensionAttribute.uex' path='docs/doc[@for="XmlFormatExtensionAttribute.XmlFormatExtensionAttribute1"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlFormatExtensionAttribute(string elementName, string ns, Type[] extensionPoints)
        {
            _name = elementName;
            _ns = ns;
            _types = extensionPoints;
        }

        /// <include file='doc\XmlFormatExtensionAttribute.uex' path='docs/doc[@for="XmlFormatExtensionAttribute.ExtensionPoints"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public Type[] ExtensionPoints
        {
            get { return _types == null ? new Type[0] : _types; }
            set { _types = value; }
        }

        /// <include file='doc\XmlFormatExtensionAttribute.uex' path='docs/doc[@for="XmlFormatExtensionAttribute.Namespace"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string Namespace
        {
            get { return _ns == null ? string.Empty : _ns; }
            set { _ns = value; }
        }

        /// <include file='doc\XmlFormatExtensionAttribute.uex' path='docs/doc[@for="XmlFormatExtensionAttribute.ElementName"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string ElementName
        {
            get { return _name == null ? string.Empty : _name; }
            set { _name = value; }
        }
    }
}

