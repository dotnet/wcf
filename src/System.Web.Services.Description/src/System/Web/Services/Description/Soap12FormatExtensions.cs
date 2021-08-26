// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Web.Services.Configuration;

namespace System.Web.Services.Description
{
    [XmlFormatExtension("binding", Namespace, typeof(Binding))]
    [XmlFormatExtensionPrefix("soap12", Namespace)]
    public sealed class Soap12Binding : SoapBinding {
        public new const string Namespace = "http://schemas.xmlsoap.org/wsdl/soap12/";
        public new const string HttpTransport = "http://schemas.xmlsoap.org/soap/http";
    }

    [XmlFormatExtension("operation", Soap12Binding.Namespace, typeof(OperationBinding))]
    public sealed class Soap12OperationBinding : SoapOperationBinding {
        [XmlAttribute("soapActionRequired"), DefaultValue(false)]
        public bool SoapActionRequired { get; set; }

        internal Soap12OperationBinding DuplicateBySoapAction { get; set; }

        internal Soap12OperationBinding DuplicateByRequestElement { get; set; }
    }

    [XmlFormatExtension("body", Soap12Binding.Namespace, typeof(InputBinding), typeof(OutputBinding), typeof(MimePart))]
    public sealed class Soap12BodyBinding : SoapBodyBinding {
    }

    [XmlFormatExtension("fault", Soap12Binding.Namespace, typeof(FaultBinding))]
    public sealed class Soap12FaultBinding : SoapFaultBinding {
    }

    [XmlFormatExtension("header", Soap12Binding.Namespace, typeof(InputBinding), typeof(OutputBinding))]
    public sealed class Soap12HeaderBinding : SoapHeaderBinding {
    }

    [XmlFormatExtension("address", Soap12Binding.Namespace, typeof(Port))]
    public sealed class Soap12AddressBinding : SoapAddressBinding {
    }
}
