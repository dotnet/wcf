// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Serialization
{
    using System;

    /// <include file='doc\SoapSchemaMember.uex' path='docs/doc[@for="SoapSchemaMember"]/*' />
    /// <internalonly/>
    public class SoapSchemaMember
    {
        private string _memberName;
        private XmlQualifiedName _type = XmlQualifiedName.Empty;

        /// <include file='doc\SoapSchemaMember.uex' path='docs/doc[@for="SoapSchemaMember.MemberType"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlQualifiedName MemberType
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <include file='doc\SoapSchemaMember.uex' path='docs/doc[@for="SoapSchemaMember.MemberName"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string MemberName
        {
            get { return _memberName == null ? string.Empty : _memberName; }
            set { _memberName = value; }
        }
    }
}
