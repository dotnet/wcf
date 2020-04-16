// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Web.Services.Description
{
    using Microsoft.Xml;
    using System.IO;
    using Microsoft.Xml.Schema;
    using Microsoft.Xml.Serialization;
    using System.ComponentModel;
    using System.Text;
    using System.Web.Services.Configuration;

    /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBinding"]/*' />
    [XmlFormatExtension("binding", SoapBinding.Namespace, typeof(Binding))]
    [XmlFormatExtensionPrefix("soap", SoapBinding.Namespace)]
    [XmlFormatExtensionPrefix("soapenc", "http://schemas.xmlsoap.org/soap/encoding/")]
    public class SoapBinding : ServiceDescriptionFormatExtension
    {
        private SoapBindingStyle _style = SoapBindingStyle.Document;
        private string _transport;
        private static XmlSchema s_schema = null;

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBinding.Namespace"]/*' />
        public const string Namespace = "http://schemas.xmlsoap.org/wsdl/soap/";
        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBinding.HttpTransport"]/*' />
        public const string HttpTransport = "http://schemas.xmlsoap.org/soap/http";

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBinding.Transport"]/*' />
        [XmlAttribute("transport")]
        public string Transport
        {
            get { return _transport == null ? string.Empty : _transport; }
            set { _transport = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBinding.Style"]/*' />
        [XmlAttribute("style"), DefaultValue(SoapBindingStyle.Document)]
        public SoapBindingStyle Style
        {
            get { return _style; }
            set { _style = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapFormatExtensions.Schema"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static XmlSchema Schema
        {
            get
            {
                if (s_schema == null)
                {
                    s_schema = XmlSchema.Read(new StringReader(Schemas.Soap), null);
                }
                return s_schema;
            }
        }
    }

    /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBindingStyle"]/*' />
    public enum SoapBindingStyle
    {
        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBindingStyle.Default"]/*' />
        [XmlIgnore]
        Default,
        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBindingStyle.Document"]/*' />
        [XmlEnum("document")]
        Document,
        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBindingStyle.Rpc"]/*' />
        [XmlEnum("rpc")]
        Rpc,
    }

    /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapOperationBinding"]/*' />
    [XmlFormatExtension("operation", SoapBinding.Namespace, typeof(OperationBinding))]
    public class SoapOperationBinding : ServiceDescriptionFormatExtension
    {
        private string _soapAction;
        private SoapBindingStyle _style;

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapOperationBinding.SoapAction"]/*' />
        [XmlAttribute("soapAction")]
        public string SoapAction
        {
            get { return _soapAction == null ? string.Empty : _soapAction; }
            set { _soapAction = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapOperationBinding.Style"]/*' />
        [XmlAttribute("style"), DefaultValue(SoapBindingStyle.Default)]
        public SoapBindingStyle Style
        {
            get { return _style; }
            set { _style = value; }
        }
    }

    /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBodyBinding"]/*' />
    [XmlFormatExtension("body", SoapBinding.Namespace, typeof(InputBinding), typeof(OutputBinding), typeof(MimePart))]
    public class SoapBodyBinding : ServiceDescriptionFormatExtension
    {
        private SoapBindingUse _use;
        private string _ns;
        private string _encoding;
        private string[] _parts;

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBodyBinding.Use"]/*' />
        [XmlAttribute("use"), DefaultValue(SoapBindingUse.Default)]
        public SoapBindingUse Use
        {
            get { return _use; }
            set { _use = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBodyBinding.Namespace"]/*' />
        [XmlAttribute("namespace"), DefaultValue("")]
        public string Namespace
        {
            get { return _ns == null ? string.Empty : _ns; }
            set { _ns = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBodyBinding.Encoding"]/*' />
        [XmlAttribute("encodingStyle"), DefaultValue("")]
        public string Encoding
        {
            get { return _encoding == null ? string.Empty : _encoding; }
            set { _encoding = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBodyBinding.PartsString"]/*' />
        [XmlAttribute("parts")]
        public string PartsString
        {
            get
            {
                if (_parts == null)
                    return null;
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < _parts.Length; i++)
                {
                    if (i > 0) builder.Append(' ');
                    builder.Append(_parts[i]);
                }
                return builder.ToString();
            }
            set
            {
                if (value == null)
                    _parts = null;
                else
                    _parts = value.Split(new char[] { ' ' });
            }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBodyBinding.Parts"]/*' />
        [XmlIgnore]
        public string[] Parts
        {
            get { return _parts; }
            set { _parts = value; }
        }
    }

    /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBindingUse"]/*' />
    public enum SoapBindingUse
    {
        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBindingUse.Default"]/*' />
        [XmlIgnore]
        Default,
        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBindingUse.Encoded"]/*' />
        [XmlEnum("encoded")]
        Encoded,
        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapBindingUse.Literal"]/*' />
        [XmlEnum("literal")]
        Literal,
    }

    /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapFaultBinding"]/*' />
    [XmlFormatExtension("fault", SoapBinding.Namespace, typeof(FaultBinding))]
    public class SoapFaultBinding : ServiceDescriptionFormatExtension
    {
        private SoapBindingUse _use;
        private string _ns;
        private string _encoding;
        private string _name;

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapFaultBinding.Use"]/*' />
        [XmlAttribute("use"), DefaultValue(SoapBindingUse.Default)]
        public SoapBindingUse Use
        {
            get { return _use; }
            set { _use = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapFaultBinding.Use"]/*' />
        [XmlAttribute("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapFaultBinding.Namespace"]/*' />
        [XmlAttribute("namespace")]
        public string Namespace
        {
            get { return _ns == null ? string.Empty : _ns; }
            set { _ns = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapFaultBinding.Encoding"]/*' />
        [XmlAttribute("encodingStyle"), DefaultValue("")]
        public string Encoding
        {
            get { return _encoding == null ? string.Empty : _encoding; }
            set { _encoding = value; }
        }
    }

    /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapHeaderBinding"]/*' />
    [XmlFormatExtension("header", SoapBinding.Namespace, typeof(InputBinding), typeof(OutputBinding))]
    public class SoapHeaderBinding : ServiceDescriptionFormatExtension
    {
        private XmlQualifiedName _message = XmlQualifiedName.Empty;
        private string _part;
        private SoapBindingUse _use;
        private string _encoding;
        private string _ns;
        private bool _mapToProperty;
        private SoapHeaderFaultBinding _fault;

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapHeaderBinding.MapToProperty"]/*' />
        [XmlIgnore]
        public bool MapToProperty
        {
            get { return _mapToProperty; }
            set { _mapToProperty = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapHeaderBinding.Message"]/*' />
        [XmlAttribute("message")]
        public XmlQualifiedName Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapHeaderBinding.Part"]/*' />
        [XmlAttribute("part")]
        public string Part
        {
            get { return _part; }
            set { _part = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapHeaderBinding.Use"]/*' />
        [XmlAttribute("use"), DefaultValue(SoapBindingUse.Default)]
        public SoapBindingUse Use
        {
            get { return _use; }
            set { _use = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapHeaderBinding.Encoding"]/*' />
        [XmlAttribute("encodingStyle"), DefaultValue("")]
        public string Encoding
        {
            get { return _encoding == null ? string.Empty : _encoding; }
            set { _encoding = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapHeaderBinding.Namespace"]/*' />
        [XmlAttribute("namespace"), DefaultValue("")]
        public string Namespace
        {
            get { return _ns == null ? string.Empty : _ns; }
            set { _ns = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapHeaderBinding.Fault"]/*' />
        [XmlElement("headerfault")]
        public SoapHeaderFaultBinding Fault
        {
            get { return _fault; }
            set { _fault = value; }
        }
    }

    /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapHeaderFaultBinding"]/*' />
    public class SoapHeaderFaultBinding : ServiceDescriptionFormatExtension
    {
        private XmlQualifiedName _message = XmlQualifiedName.Empty;
        private string _part;
        private SoapBindingUse _use;
        private string _encoding;
        private string _ns;

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapHeaderFaultBinding.Message"]/*' />
        [XmlAttribute("message")]
        public XmlQualifiedName Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapHeaderFaultBinding.Part"]/*' />
        [XmlAttribute("part")]
        public string Part
        {
            get { return _part; }
            set { _part = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapHeaderFaultBinding.Use"]/*' />
        [XmlAttribute("use"), DefaultValue(SoapBindingUse.Default)]
        public SoapBindingUse Use
        {
            get { return _use; }
            set { _use = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapHeaderFaultBinding.Encoding"]/*' />
        [XmlAttribute("encodingStyle"), DefaultValue("")]
        public string Encoding
        {
            get { return _encoding == null ? string.Empty : _encoding; }
            set { _encoding = value; }
        }

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapHeaderFaultBinding.Namespace"]/*' />
        [XmlAttribute("namespace"), DefaultValue("")]
        public string Namespace
        {
            get { return _ns == null ? string.Empty : _ns; }
            set { _ns = value; }
        }
    }

    /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapAddressBinding"]/*' />
    [XmlFormatExtension("address", SoapBinding.Namespace, typeof(Port))]
    public class SoapAddressBinding : ServiceDescriptionFormatExtension
    {
        private string _location;

        /// <include file='doc\SoapFormatExtensions.uex' path='docs/doc[@for="SoapAddressBinding.Location"]/*' />
        [XmlAttribute("location")]
        public string Location
        {
            get { return _location == null ? string.Empty : _location; }
            set { _location = value; }
        }
    }
}

