// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Web.Services.Description
{
    using Microsoft.Xml.Serialization;
    using System.Web.Services.Configuration;

    /// <include file='doc\HttpFormatExtensions.uex' path='docs/doc[@for="HttpAddressBinding"]/*' />
    [XmlFormatExtension("address", HttpBinding.Namespace, typeof(Port))]
    public sealed class HttpAddressBinding : ServiceDescriptionFormatExtension
    {
        private string _location;

        /// <include file='doc\HttpFormatExtensions.uex' path='docs/doc[@for="HttpAddressBinding.Location"]/*' />
        [XmlAttribute("location")]
        public string Location
        {
            get { return _location == null ? string.Empty : _location; }
            set { _location = value; }
        }
    }

    /// <include file='doc\HttpFormatExtensions.uex' path='docs/doc[@for="HttpBinding"]/*' />
    [XmlFormatExtension("binding", HttpBinding.Namespace, typeof(Binding))]
    [XmlFormatExtensionPrefix("http", HttpBinding.Namespace)]
    public sealed class HttpBinding : ServiceDescriptionFormatExtension
    {
        private string _verb;

        /// <include file='doc\HttpFormatExtensions.uex' path='docs/doc[@for="HttpBinding.Namespace"]/*' />
        public const string Namespace = "http://schemas.xmlsoap.org/wsdl/http/";

        /// <include file='doc\HttpFormatExtensions.uex' path='docs/doc[@for="HttpBinding.Verb"]/*' />
        [XmlAttribute("verb")]
        public string Verb
        {
            get { return _verb; }
            set { _verb = value; }
        }
    }

    /// <include file='doc\HttpFormatExtensions.uex' path='docs/doc[@for="HttpOperationBinding"]/*' />
    [XmlFormatExtension("operation", HttpBinding.Namespace, typeof(OperationBinding))]
    public sealed class HttpOperationBinding : ServiceDescriptionFormatExtension
    {
        private string _location;

        /// <include file='doc\HttpFormatExtensions.uex' path='docs/doc[@for="HttpOperationBinding.Location"]/*' />
        [XmlAttribute("location")]
        public string Location
        {
            get { return _location == null ? string.Empty : _location; }
            set { _location = value; }
        }
    }

    /// <include file='doc\HttpFormatExtensions.uex' path='docs/doc[@for="HttpUrlEncodedBinding"]/*' />
    [XmlFormatExtension("urlEncoded", HttpBinding.Namespace, typeof(InputBinding))]
    public sealed class HttpUrlEncodedBinding : ServiceDescriptionFormatExtension
    {
    }

    /// <include file='doc\HttpFormatExtensions.uex' path='docs/doc[@for="HttpUrlReplacementBinding"]/*' />
    [XmlFormatExtension("urlReplacement", HttpBinding.Namespace, typeof(InputBinding))]
    public sealed class HttpUrlReplacementBinding : ServiceDescriptionFormatExtension
    {
    }
}

