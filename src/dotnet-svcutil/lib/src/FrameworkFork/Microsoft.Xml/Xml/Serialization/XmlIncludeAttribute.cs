// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Serialization {

    using System;

    /// <include file='doc\XmlIncludeAttribute.uex' path='docs/doc[@for="XmlIncludeAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple=true)]
    public class XmlIncludeAttribute : System.Attribute {
        Type type;

        /// <include file='doc\XmlIncludeAttribute.uex' path='docs/doc[@for="XmlIncludeAttribute.XmlIncludeAttribute"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlIncludeAttribute(Type type) {
            this.type = type;
        }

        /// <include file='doc\XmlIncludeAttribute.uex' path='docs/doc[@for="XmlIncludeAttribute.Type"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public Type Type {
            get { return type; }
            set { type = value; }
        }
    }
}
