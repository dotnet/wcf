// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Serialization
{
    using System;

    /// <include file='doc\SoapIgnoreAttribute.uex' path='docs/doc[@for="SoapIgnoreAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class SoapIgnoreAttribute : System.Attribute
    {
        /// <include file='doc\SoapIgnoreAttribute.uex' path='docs/doc[@for="SoapIgnoreAttribute.SoapIgnoreAttribute"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public SoapIgnoreAttribute()
        {
        }
    }
}
