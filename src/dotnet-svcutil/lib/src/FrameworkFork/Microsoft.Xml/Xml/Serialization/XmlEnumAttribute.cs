// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Serialization
{
    using System;

    /// <include file='doc\XmlEnumAttribute.uex' path='docs/doc[@for="XmlEnumAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.Field)]
    public class XmlEnumAttribute : System.Attribute
    {
        private string _name;

        /// <include file='doc\XmlEnumAttribute.uex' path='docs/doc[@for="XmlEnumAttribute.XmlEnumAttribute"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlEnumAttribute()
        {
        }

        /// <include file='doc\XmlEnumAttribute.uex' path='docs/doc[@for="XmlEnumAttribute.XmlEnumAttribute1"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlEnumAttribute(string name)
        {
            _name = name;
        }

        /// <include file='doc\XmlEnumAttribute.uex' path='docs/doc[@for="XmlEnumAttribute.Name"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
