// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Xml;
using System.IO;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Text;
using System.Web.Services.Configuration;

namespace System.Web.Services.Description {
    [XmlFormatExtension("binding", Namespace, typeof(Binding))]
    [XmlFormatExtensionPrefix("soap", Namespace)]
    [XmlFormatExtensionPrefix("soapenc", "http://schemas.xmlsoap.org/soap/encoding/")]
    public class SoapBinding : ServiceDescriptionFormatExtension {
        private string _transport;
        private static XmlSchema s_schema = null;

        public const string Namespace = "http://schemas.xmlsoap.org/wsdl/soap/";

        public const string HttpTransport = "http://schemas.xmlsoap.org/soap/http";

        [XmlAttribute("transport")]
        public string Transport {
            get { return _transport == null ? string.Empty : _transport; }
            set { _transport = value; }
        }

        [XmlAttribute("style"), DefaultValue(SoapBindingStyle.Document)]
        public SoapBindingStyle Style { get; set; } = SoapBindingStyle.Document;

        public static XmlSchema Schema {
            get {
                if (s_schema == null) {
                    using (XmlTextReader reader = new XmlTextReader(new StringReader(Schemas.Soap)))
                    {
                        reader.DtdProcessing = DtdProcessing.Ignore;
                        s_schema = XmlSchema.Read(reader, null);
                    }
                }
                return s_schema;
            }
        }
    }

    public enum SoapBindingStyle {
        [XmlIgnore]
        Default,
        [XmlEnum("document")]
        Document,
        [XmlEnum("rpc")]
        Rpc,
    }

    [XmlFormatExtension("operation", SoapBinding.Namespace, typeof(OperationBinding))]
    public class SoapOperationBinding : ServiceDescriptionFormatExtension {
        private string _soapAction;

        [XmlAttribute("soapAction")]
        public string SoapAction {
            get { return _soapAction == null ? string.Empty : _soapAction; }
            set { _soapAction = value; }
        }

        [XmlAttribute("style"), DefaultValue(SoapBindingStyle.Default)]
        public SoapBindingStyle Style { get; set; }
    }

    [XmlFormatExtension("body", SoapBinding.Namespace, typeof(InputBinding), typeof(OutputBinding), typeof(MimePart))]
    public class SoapBodyBinding : ServiceDescriptionFormatExtension {
        private string _ns;
        private string _encoding;

        [XmlAttribute("use"), DefaultValue(SoapBindingUse.Default)]
        public SoapBindingUse Use { get; set; }

        [XmlAttribute("namespace"), DefaultValue("")]
        public string Namespace {
            get { return _ns == null ? string.Empty : _ns; }
            set { _ns = value; }
        }

        [XmlAttribute("encodingStyle"), DefaultValue("")]
        public string Encoding {
            get { return _encoding == null ? string.Empty : _encoding; }
            set { _encoding = value; }
        }

        [XmlAttribute("parts")]
        public string PartsString {
            get {
                if (Parts == null)
                {
                    return null;
                }

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < Parts.Length; i++) {
                    if (i > 0)
                    {
                        builder.Append(' ');
                    }

                    builder.Append(Parts[i]);
                }

                return builder.ToString();
            }
            set {
                if (value == null)
                {
                    Parts = null;
                }
                else
                {
                    Parts = value.Split(new char[] { ' ' });
                }
            }
        }

        [XmlIgnore]
        public string[] Parts { get; set; }
    }

    public enum SoapBindingUse {
        [XmlIgnore]
        Default,
        [XmlEnum("encoded")]
        Encoded,
        [XmlEnum("literal")]
        Literal,
    }

    [XmlFormatExtension("fault", SoapBinding.Namespace, typeof(FaultBinding))]
    public class SoapFaultBinding : ServiceDescriptionFormatExtension {
        private string _ns;
        private string _encoding;

        [XmlAttribute("use"), DefaultValue(SoapBindingUse.Default)]
        public SoapBindingUse Use { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("namespace")]
        public string Namespace {
            get { return _ns == null ? string.Empty : _ns; }
            set { _ns = value; }
        }

        [XmlAttribute("encodingStyle"), DefaultValue("")]
        public string Encoding {
            get { return _encoding == null ? string.Empty : _encoding; }
            set { _encoding = value; }
        }
    }

    [XmlFormatExtension("header", SoapBinding.Namespace, typeof(InputBinding), typeof(OutputBinding))]
    public class SoapHeaderBinding : ServiceDescriptionFormatExtension {
        private string _encoding;
        private string _ns;

        [XmlIgnore]
        public bool MapToProperty { get; set; }

        [XmlAttribute("message")]
        public XmlQualifiedName Message { get; set; } = XmlQualifiedName.Empty;

        [XmlAttribute("part")]
        public string Part { get; set; }

        [XmlAttribute("use"), DefaultValue(SoapBindingUse.Default)]
        public SoapBindingUse Use { get; set; }

        [XmlAttribute("encodingStyle"), DefaultValue("")]
        public string Encoding {
            get { return _encoding == null ? string.Empty : _encoding; }
            set { _encoding = value; }
        }

        [XmlAttribute("namespace"), DefaultValue("")]
        public string Namespace {
            get { return _ns == null ? string.Empty : _ns; }
            set { _ns = value; }
        }

        [XmlElement("headerfault")]
        public SoapHeaderFaultBinding Fault { get; set; }
    }

    public class SoapHeaderFaultBinding : ServiceDescriptionFormatExtension {
        private string _encoding;
        private string _ns;

        [XmlAttribute("message")]
        public XmlQualifiedName Message { get; set; } = XmlQualifiedName.Empty;

        [XmlAttribute("part")]
        public string Part { get; set; }

        [XmlAttribute("use"), DefaultValue(SoapBindingUse.Default)]
        public SoapBindingUse Use { get; set; }

        [XmlAttribute("encodingStyle"), DefaultValue("")]
        public string Encoding {
            get { return _encoding == null ? string.Empty : _encoding; }
            set { _encoding = value; }
        }

        [XmlAttribute("namespace"), DefaultValue("")]
        public string Namespace {
            get { return _ns == null ? string.Empty : _ns; }
            set { _ns = value; }
        }
    }

    [XmlFormatExtension("address", SoapBinding.Namespace, typeof(Port))]
    public class SoapAddressBinding : ServiceDescriptionFormatExtension {
        private string _location;

        [XmlAttribute("location")]
        public string Location {
            get { return _location == null ? string.Empty : _location; }
            set { _location = value; }
        }
    }
}
