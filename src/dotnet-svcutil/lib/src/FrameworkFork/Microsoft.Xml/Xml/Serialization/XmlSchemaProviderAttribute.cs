// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.


namespace Microsoft.Xml.Serialization {
    using System;
    using Microsoft.Xml.Schema;

    /// <include file='doc\XmlSchemaProviderAttribute.uex' path='docs/doc[@for="XmlSchemaProviderAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public sealed class XmlSchemaProviderAttribute : System.Attribute {
        string methodName;
        bool any;
        
        /// <include file='doc\XmlSchemaProviderAttribute.uex' path='docs/doc[@for="XmlSchemaProviderAttribute.XmlSchemaProviderAttribute"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlSchemaProviderAttribute(string methodName) {
            this.methodName = methodName;
        }
        
        /// <include file='doc\XmlSchemaProviderAttribute.uex' path='docs/doc[@for="XmlSchemaProviderAttribute.MethodName"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string MethodName {
            get { return methodName; }
        }

        /// <include file='doc\XmlSchemaProviderAttribute.uex' path='docs/doc[@for="XmlSchemaProviderAttribute.IsAny"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool IsAny {
            get { return any; }
            set {  any = value; }
        }
    }
}
