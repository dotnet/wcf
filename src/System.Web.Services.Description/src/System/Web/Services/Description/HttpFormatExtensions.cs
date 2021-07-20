// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Xml.Serialization;
using System.Web.Services.Configuration;

namespace System.Web.Services.Description {
    [XmlFormatExtension("address", HttpBinding.Namespace, typeof(Port))]
    public sealed class HttpAddressBinding : ServiceDescriptionFormatExtension {
        private string _location;

        [XmlAttribute("location")]
        public string Location {
            get { return _location == null ? string.Empty : _location; }
            set { _location = value; }
        }
    }

    [XmlFormatExtension("binding", Namespace, typeof(Binding))]
    [XmlFormatExtensionPrefix("http", Namespace)]
    public sealed class HttpBinding : ServiceDescriptionFormatExtension {
        public const string Namespace = "http://schemas.xmlsoap.org/wsdl/http/";

        [XmlAttribute("verb")]
        public string Verb { get; set; }
    }

    [XmlFormatExtension("operation", HttpBinding.Namespace, typeof(OperationBinding))]
    public sealed class HttpOperationBinding : ServiceDescriptionFormatExtension {
        private string _location;

        [XmlAttribute("location")]
        public string Location {
            get { return _location == null ? string.Empty : _location; }
            set { _location = value; }
        }
    }

    [XmlFormatExtension("urlEncoded", HttpBinding.Namespace, typeof(InputBinding))]
    public sealed class HttpUrlEncodedBinding : ServiceDescriptionFormatExtension {
    }

    [XmlFormatExtension("urlReplacement", HttpBinding.Namespace, typeof(InputBinding))]
    public sealed class HttpUrlReplacementBinding : ServiceDescriptionFormatExtension {
    }
}
