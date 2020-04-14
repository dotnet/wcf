// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Serialization
{
    using System;

    /// <include file='doc\SoapIncludeAttribute.uex' path='docs/doc[@for="SoapIncludeAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true)]
    public class SoapIncludeAttribute : System.Attribute
    {
        private Type _type;

        /// <include file='doc\SoapIncludeAttribute.uex' path='docs/doc[@for="SoapIncludeAttribute.SoapIncludeAttribute"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public SoapIncludeAttribute(Type type)
        {
            _type = type;
        }

        /// <include file='doc\SoapIncludeAttribute.uex' path='docs/doc[@for="SoapIncludeAttribute.Type"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}
