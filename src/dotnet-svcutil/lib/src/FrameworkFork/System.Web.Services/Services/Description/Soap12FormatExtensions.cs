// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Web.Services.Description
{
    using System.ComponentModel;
    using System.Web.Services.Configuration;
    using Microsoft.Xml;
    using Microsoft.Xml.Serialization;

    /// <include file='doc\Soap12FormatExtensions.uex' path='docs/doc[@for="Soap12Binding"]/*' />
    [XmlFormatExtension("binding", Soap12Binding.Namespace, typeof(Binding))]
    [XmlFormatExtensionPrefix("soap12", Soap12Binding.Namespace)]
    public sealed class Soap12Binding : SoapBinding
    {
        /// <include file='doc\Soap12FormatExtensions.uex' path='docs/doc[@for="Soap12Binding.Namespace"]/*' />
        public new const string Namespace = "http://schemas.xmlsoap.org/wsdl/soap12/";
        /// <include file='doc\Soap12FormatExtensions.uex' path='docs/doc[@for="Soap12Binding.HttpTransport"]/*' />
        public new const string HttpTransport = "http://schemas.xmlsoap.org/soap/http";
    }

    /// <include file='doc\Soap12FormatExtensions.uex' path='docs/doc[@for="Soap12OperationBinding"]/*' />
    [XmlFormatExtension("operation", Soap12Binding.Namespace, typeof(OperationBinding))]
    public sealed class Soap12OperationBinding : SoapOperationBinding
    {
        private bool _soapActionRequired;

        /// <include file='doc\Soap12FormatExtensions.uex' path='docs/doc[@for="Soap12OperationBinding.SoapActionRequired"]/*' />
        [XmlAttribute("soapActionRequired"), DefaultValue(false)]
        public bool SoapActionRequired
        {
            get { return _soapActionRequired; }
            set { _soapActionRequired = value; }
        }
    }

    /// <include file='doc\Soap12FormatExtensions.uex' path='docs/doc[@for="Soap12BodyBinding"]/*' />
    [XmlFormatExtension("body", Soap12Binding.Namespace, typeof(InputBinding), typeof(OutputBinding), typeof(MimePart))]
    public sealed class Soap12BodyBinding : SoapBodyBinding
    {
    }

    /// <include file='doc\Soap12FormatExtensions.uex' path='docs/doc[@for="Soap12FaultBinding"]/*' />
    [XmlFormatExtension("fault", Soap12Binding.Namespace, typeof(FaultBinding))]
    public sealed class Soap12FaultBinding : SoapFaultBinding
    {
    }

    /// <include file='doc\Soap12FormatExtensions.uex' path='docs/doc[@for="Soap12HeaderBinding"]/*' />
    [XmlFormatExtension("header", Soap12Binding.Namespace, typeof(InputBinding), typeof(OutputBinding))]
    public sealed class Soap12HeaderBinding : SoapHeaderBinding
    {
    }

    /// <include file='doc\Soap12FormatExtensions.uex' path='docs/doc[@for="Soap12AddressBinding"]/*' />
    [XmlFormatExtension("address", Soap12Binding.Namespace, typeof(Port))]
    public sealed class Soap12AddressBinding : SoapAddressBinding
    {
    }
}
