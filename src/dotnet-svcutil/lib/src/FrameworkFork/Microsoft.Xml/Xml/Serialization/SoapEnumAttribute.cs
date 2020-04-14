// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Serialization
{
    using System;

    /// <include file='doc\SoapEnumAttribute.uex' path='docs/doc[@for="SoapEnumAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.Field)]
    public class SoapEnumAttribute : System.Attribute
    {
        private string _name;

        /// <include file='doc\SoapEnumAttribute.uex' path='docs/doc[@for="SoapEnumAttribute.SoapEnumAttribute"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public SoapEnumAttribute()
        {
        }

        /// <include file='doc\SoapEnumAttribute.uex' path='docs/doc[@for="SoapEnumAttribute.SoapEnumAttribute1"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public SoapEnumAttribute(string name)
        {
            _name = name;
        }

        /// <include file='doc\SoapEnumAttribute.uex' path='docs/doc[@for="SoapEnumAttribute.Name"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string Name
        {
            get { return _name == null ? string.Empty : _name; }
            set { _name = value; }
        }
    }
}
