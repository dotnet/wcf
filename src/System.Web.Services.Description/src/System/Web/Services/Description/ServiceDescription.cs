// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Xml.Serialization;
using System.Xml.Schema;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Web.Services.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Web.Services.Protocols;

namespace System.Web.Services.Description
{
    [XmlRoot("definitions", Namespace = Namespace)]
    [XmlFormatExtensionPoint("Extensions")]
    public sealed class ServiceDescription : NamedItem
    {
        internal const string Prefix = "wsdl";
        private static XmlSerializer s_serializer;
        private static XmlSerializerNamespaces s_namespaces;
        private static XmlSchema s_schema = null;
        private static XmlSchema s_soapEncodingSchema = null;
        private static StringCollection s_warnings = new StringCollection();

        public const string Namespace = "http://schemas.xmlsoap.org/wsdl/";
        private const WsiProfiles SupportedClaims = WsiProfiles.BasicProfile1_1;

        private Types _types;
        private ImportCollection _imports;
        private MessageCollection _messages;
        private PortTypeCollection _portTypes;
        private BindingCollection _bindings;
        private ServiceCollection _services;
        private ServiceDescriptionFormatExtensionCollection _extensions;
        private string _retrievalUrl;
        private StringCollection _validationWarnings;

        private static void InstanceValidation(object sender, ValidationEventArgs args)
        {
            s_warnings.Add(SR.Format(SR.WsdlInstanceValidationDetails, args.Message, args.Exception.LineNumber.ToString(CultureInfo.InvariantCulture), args.Exception.LinePosition.ToString(CultureInfo.InvariantCulture)));
        }

        [XmlIgnore]
        public string RetrievalUrl
        {
            get { return _retrievalUrl == null ? string.Empty : _retrievalUrl; }
            set { _retrievalUrl = value; }
        }

        internal void SetParent(ServiceDescriptionCollection parent)
        {
            ServiceDescriptions = parent;
        }

        [XmlIgnore]
        public ServiceDescriptionCollection ServiceDescriptions { get; private set; }

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }

        [XmlElement("import")]
        public ImportCollection Imports
        {
            get { if (_imports == null) { _imports = new ImportCollection(this); } return _imports; }
        }

        [XmlElement("types")]
        public Types Types
        {
            get { if (_types == null) { _types = new Types(); } return _types; }
            set { _types = value; }
        }

        private bool ShouldSerializeTypes() { return Types.HasItems(); }

        [XmlElement("message")]
        public MessageCollection Messages
        {
            get { if (_messages == null) { _messages = new MessageCollection(this); } return _messages; }
        }

        [XmlElement("portType")]
        public PortTypeCollection PortTypes
        {
            get { if (_portTypes == null) { _portTypes = new PortTypeCollection(this); } return _portTypes; }
        }

        [XmlElement("binding")]
        public BindingCollection Bindings
        {
            get { if (_bindings == null) { _bindings = new BindingCollection(this); } return _bindings; }
        }

        [XmlElement("service")]
        public ServiceCollection Services
        {
            get { if (_services == null) { _services = new ServiceCollection(this); } return _services; }
        }

        [XmlAttribute("targetNamespace")]
        public string TargetNamespace { get; set; }

        public static XmlSchema Schema
        {
            get
            {
                if (s_schema == null)
                {
                    using (XmlTextReader reader = new XmlTextReader(new StringReader(Schemas.Wsdl)))
                    {
                        reader.DtdProcessing = DtdProcessing.Ignore;
                        s_schema = XmlSchema.Read(reader, null);
                    }
                }
                return s_schema;
            }
        }

        internal static XmlSchema SoapEncodingSchema
        {
            get
            {
                if (s_soapEncodingSchema == null)
                {
                    using (XmlTextReader reader = new XmlTextReader(new StringReader(Schemas.SoapEncoding)))
                    {
                        reader.DtdProcessing = DtdProcessing.Ignore;
                        s_soapEncodingSchema = XmlSchema.Read(reader, null);
                    }
                }
                return s_soapEncodingSchema;
            }
        }

        [XmlIgnore]
        public StringCollection ValidationWarnings
        {
            get
            {
                if (_validationWarnings == null)
                {
                    _validationWarnings = new StringCollection();
                }
                return _validationWarnings;
            }
        }

        internal void SetWarnings(StringCollection warnings)
        {
            _validationWarnings = warnings;
        }

        // This is a special serializer that hardwires to the generated
        // ServiceDescriptionSerializer. To regenerate the serializer
        // Turn on KEEPTEMPFILES 
        // Restart server
        // Run wsdl as follows
        //   wsdl <URL_FOR_VALID_ASMX_FILE>?wsdl
        // Goto windows temp dir (usually \winnt\temp)
        // and get the latest generated .cs file
        // Change namespace to 'System.Web.Services.Description'
        // Change class names to ServiceDescriptionSerializationWriter
        // and ServiceDescriptionSerializationReader
        // Make the classes internal
        // Ensure the public Write method is Write125_definitions (If not
        // change Serialize to call the new one)
        // Ensure the public Read method is Read126_definitions (If not
        // change Deserialize to call the new one)
        internal class ServiceDescriptionSerializer : XmlSerializer
        {
            protected override XmlSerializationReader CreateReader()
            {
                return new ServiceDescriptionSerializationReader();
            }
            protected override XmlSerializationWriter CreateWriter()
            {
                return new ServiceDescriptionSerializationWriter();
            }
            public override bool CanDeserialize(System.Xml.XmlReader xmlReader)
            {
                return xmlReader.IsStartElement("definitions", Namespace);
            }
            protected override void Serialize(Object objectToSerialize, XmlSerializationWriter writer)
            {
                ((ServiceDescriptionSerializationWriter)writer).Write125_definitions(objectToSerialize);
            }
            protected override object Deserialize(XmlSerializationReader reader)
            {
                return ((ServiceDescriptionSerializationReader)reader).Read125_definitions();
            }
        }

        [XmlIgnore]
        public static XmlSerializer Serializer
        {
            get
            {
                if (s_serializer == null)
                {
                    WebServicesSection config = WebServicesSection.Current;
                    XmlAttributeOverrides overrides = new XmlAttributeOverrides();
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("s", XmlSchema.Namespace);
                    WebServicesSection.LoadXmlFormatExtensions(config.GetAllFormatExtensionTypes(), overrides, ns);
                    s_namespaces = ns;
                    s_serializer = new ServiceDescriptionSerializer();
                    s_serializer.UnknownElement += new XmlElementEventHandler(RuntimeUtils.OnUnknownElement);
                }

                return s_serializer;
            }
        }

        internal ServiceDescription Next { get; set; }

        public static ServiceDescription Read(TextReader textReader)
        {
            return Read(textReader, false);
        }

        public static ServiceDescription Read(Stream stream)
        {
            return Read(stream, false);
        }

        public static ServiceDescription Read(XmlReader reader)
        {
            return Read(reader, false);
        }

        public static ServiceDescription Read(string fileName)
        {
            return Read(fileName, false);
        }

        public static ServiceDescription Read(TextReader textReader, bool validate)
        {
            XmlTextReader reader = new XmlTextReader(textReader);
            reader.WhitespaceHandling = WhitespaceHandling.Significant;
            reader.XmlResolver = null;
            reader.DtdProcessing = DtdProcessing.Prohibit;
            return Read(reader, validate);
        }

        public static ServiceDescription Read(Stream stream, bool validate)
        {
            XmlTextReader reader = new XmlTextReader(stream);
            reader.WhitespaceHandling = WhitespaceHandling.Significant;
            reader.XmlResolver = null;
            reader.DtdProcessing = DtdProcessing.Prohibit;
            return Read(reader, validate);
        }

        public static ServiceDescription Read(string fileName, bool validate)
        {
            StreamReader reader = new StreamReader(fileName, Encoding.Default, true);
            try
            {
                return Read(reader, validate);
            }
            finally
            {
                reader.Close();
            }
        }

        public static ServiceDescription Read(XmlReader reader, bool validate)
        {
            if (validate)
            {
                XmlReaderSettings readerSettings = new XmlReaderSettings();

                readerSettings.ValidationType = ValidationType.Schema;
                readerSettings.ValidationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints;

                readerSettings.Schemas.Add(Schema);
                readerSettings.Schemas.Add(SoapBinding.Schema);
                readerSettings.ValidationEventHandler += new ValidationEventHandler(InstanceValidation);
                s_warnings.Clear();
                XmlReader validatingReader = XmlReader.Create(reader, readerSettings);
                if (reader.ReadState != ReadState.Initial)
                {
                    //underlying reader has moved, so move validatingreader as well
                    validatingReader.Read();
                }
                ServiceDescription sd = (ServiceDescription)Serializer.Deserialize(validatingReader);
                sd.SetWarnings(s_warnings);
                return sd;
            }
            else
            {
                return (ServiceDescription)Serializer.Deserialize(reader);
            }
        }

        public static bool CanRead(XmlReader reader)
        {
            return Serializer.CanDeserialize(reader);
        }

        public void Write(string fileName)
        {
            StreamWriter writer = new StreamWriter(fileName);
            try
            {
                Write(writer);
            }
            finally
            {
                writer.Close();
            }
        }

        public void Write(TextWriter writer)
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(writer);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.Indentation = 2;
            Write(xmlWriter);
        }

        public void Write(Stream stream)
        {
            TextWriter writer = new StreamWriter(stream);
            Write(writer);
            writer.Flush();
        }

        public void Write(XmlWriter writer)
        {
            XmlSerializer serializer = Serializer;
            XmlSerializerNamespaces ns;
            if (Namespaces == null || Namespaces.Count == 0)
            {
                ns = new XmlSerializerNamespaces(s_namespaces);
                ns.Add(Prefix, Namespace);
                if (TargetNamespace != null && TargetNamespace.Length != 0)
                {
                    ns.Add("tns", TargetNamespace);
                }
                for (int i = 0; i < Types.Schemas.Count; i++)
                {
                    string tns = Types.Schemas[i].TargetNamespace;
                    if (tns != null && tns.Length > 0 && tns != TargetNamespace && tns != Namespace)
                    {
                        ns.Add("s" + i.ToString(CultureInfo.InvariantCulture), tns);
                    }
                }
                for (int i = 0; i < Imports.Count; i++)
                {
                    Import import = Imports[i];
                    if (import.Namespace.Length > 0)
                    {
                        ns.Add("i" + i.ToString(CultureInfo.InvariantCulture), import.Namespace);
                    }
                }
            }
            else
            {
                ns = Namespaces;
            }

            serializer.Serialize(writer, this, ns);
        }

        internal static WsiProfiles GetConformanceClaims(XmlElement documentation)
        {
            if (documentation == null)
            {
                return WsiProfiles.None;
            }

            WsiProfiles existingClaims = WsiProfiles.None;

            XmlNode child = documentation.FirstChild;
            while (child != null)
            {
                XmlNode sibling = child.NextSibling;
                if (child is XmlElement)
                {
                    XmlElement element = (XmlElement)child;
                    if (element.LocalName == Soap.Element.Claim && element.NamespaceURI == Soap.ConformanceClaim)
                    {
                        if (Soap.BasicProfile1_1 == element.GetAttribute(Soap.Attribute.ConformsTo))
                        {
                            existingClaims |= WsiProfiles.BasicProfile1_1;
                        }
                    }
                }
                child = sibling;
            }

            return existingClaims;
        }

        internal static void AddConformanceClaims(XmlElement documentation, WsiProfiles claims)
        {
            claims &= SupportedClaims;
            if (claims == WsiProfiles.None)
            {
                return;
            }

            // check already presend claims
            WsiProfiles existingClaims = GetConformanceClaims(documentation);
            claims &= ~existingClaims;
            if (claims == WsiProfiles.None)
            {
                return;
            }

            XmlDocument d = documentation.OwnerDocument;
            if ((claims & WsiProfiles.BasicProfile1_1) != 0)
            {
                XmlElement claim = d.CreateElement(Soap.ClaimPrefix, Soap.Element.Claim, Soap.ConformanceClaim);
                claim.SetAttribute(Soap.Attribute.ConformsTo, Soap.BasicProfile1_1);
                documentation.InsertBefore(claim, null);
            }
        }
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class Import : DocumentableItem
    {
        private string _ns;
        private string _location;
        private ServiceDescriptionFormatExtensionCollection _extensions;

        internal void SetParent(ServiceDescription parent)
        {
            ServiceDescription = parent;
        }

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }

        public ServiceDescription ServiceDescription { get; private set; }

        [XmlAttribute("namespace")]
        public string Namespace
        {
            get { return _ns == null ? string.Empty : _ns; }
            set { _ns = value; }
        }

        [XmlAttribute("location")]
        public string Location
        {
            get { return _location == null ? string.Empty : _location; }
            set { _location = value; }
        }
    }

    public abstract class DocumentableItem
    {
        private XmlDocument _parent;
        private string _documentation;
        private XmlElement _documentationElement;
        private XmlSerializerNamespaces _namespaces;

        [XmlIgnore]
        public string Documentation
        {
            get
            {
                if (_documentation != null)
                {
                    return _documentation;
                }

                if (_documentationElement == null)
                {
                    return string.Empty;
                }

                return _documentationElement.InnerXml;
            }
            set
            {
                _documentation = value;
                StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
                XmlTextWriter xmlWriter = new XmlTextWriter(writer);
                xmlWriter.WriteElementString(ServiceDescription.Prefix, "documentation", ServiceDescription.Namespace, value);
                using (XmlTextReader reader = new XmlTextReader(new StringReader(writer.ToString())))
                {
                    reader.DtdProcessing = DtdProcessing.Ignore;
                    Parent.Load(reader);
                }
                _documentationElement = _parent.DocumentElement;
                xmlWriter.Close();
            }
        }

        [XmlAnyElement("documentation", Namespace = ServiceDescription.Namespace)]
        public XmlElement DocumentationElement
        {
            get { return _documentationElement; }
            set
            {
                _documentationElement = value;
                _documentation = null;
            }
        }

        [XmlAnyAttribute]
        public System.Xml.XmlAttribute[] ExtensibleAttributes { get; set; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Namespaces
        {
            get
            {
                if (_namespaces == null)
                {
                    _namespaces = new XmlSerializerNamespaces();
                }

                return _namespaces;
            }
            set { _namespaces = value; }
        }

        [XmlIgnore]
        public abstract ServiceDescriptionFormatExtensionCollection Extensions { get; }

        internal XmlDocument Parent
        {
            get
            {
                if (_parent == null)
                {
                    _parent = new XmlDocument();
                }

                return _parent;
            }
        }
    }

    public abstract class NamedItem : DocumentableItem
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class Port : NamedItem
    {
        private ServiceDescriptionFormatExtensionCollection _extensions;

        internal void SetParent(Service parent)
        {
            Service = parent;
        }

        public Service Service { get; private set; }

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }

        [XmlAttribute("binding")]
        public XmlQualifiedName Binding { get; set; } = XmlQualifiedName.Empty;
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class Service : NamedItem
    {
        private ServiceDescriptionFormatExtensionCollection _extensions;
        private PortCollection _ports;

        internal void SetParent(ServiceDescription parent)
        {
            ServiceDescription = parent;
        }

        public ServiceDescription ServiceDescription { get; private set; }

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }

        [XmlElement("port")]
        public PortCollection Ports
        {
            get { if (_ports == null) { _ports = new PortCollection(this); } return _ports; }
        }
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class FaultBinding : MessageBinding
    {
        private ServiceDescriptionFormatExtensionCollection _extensions;

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }
    }

    public abstract class MessageBinding : NamedItem
    {
        internal void SetParent(OperationBinding parent)
        {
            OperationBinding = parent;
        }

        public OperationBinding OperationBinding { get; private set; }
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class InputBinding : MessageBinding
    {
        private ServiceDescriptionFormatExtensionCollection _extensions;

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class OutputBinding : MessageBinding
    {
        private ServiceDescriptionFormatExtensionCollection _extensions;

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class OperationBinding : NamedItem
    {
        private ServiceDescriptionFormatExtensionCollection _extensions;
        private FaultBindingCollection _faults;
        private InputBinding _input;
        private OutputBinding _output;

        internal void SetParent(Binding parent)
        {
            Binding = parent;
        }

        public Binding Binding { get; private set; }

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }

        [XmlElement("input")]
        public InputBinding Input
        {
            get { return _input; }
            set
            {
                if (_input != null)
                {
                    _input.SetParent(null);
                }
                _input = value;
                if (_input != null)
                {
                    _input.SetParent(this);
                }
            }
        }

        [XmlElement("output")]
        public OutputBinding Output
        {
            get { return _output; }
            set
            {
                if (_output != null)
                {
                    _output.SetParent(null);
                }
                _output = value;
                if (_output != null)
                {
                    _output.SetParent(this);
                }
            }
        }

        [XmlElement("fault")]
        public FaultBindingCollection Faults
        {
            get { if (_faults == null) { _faults = new FaultBindingCollection(this); } return _faults; }
        }
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class Binding : NamedItem
    {
        private ServiceDescriptionFormatExtensionCollection _extensions;
        private OperationBindingCollection _operations;
        private XmlQualifiedName _type = XmlQualifiedName.Empty;

        internal void SetParent(ServiceDescription parent)
        {
            ServiceDescription = parent;
        }

        public ServiceDescription ServiceDescription { get; private set; }

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }

        [XmlElement("operation")]
        public OperationBindingCollection Operations
        {
            get { if (_operations == null) { _operations = new OperationBindingCollection(this); } return _operations; }
        }

        [XmlAttribute("type")]
        public XmlQualifiedName Type
        {
            get
            {
                if ((object)_type == null)
                {
                    return XmlQualifiedName.Empty;
                }

                return _type;
            }
            set { _type = value; }
        }
    }

    public abstract class OperationMessage : NamedItem
    {
        internal void SetParent(Operation parent)
        {
            Operation = parent;
        }

        public Operation Operation { get; private set; }

        [XmlAttribute("message")]
        public XmlQualifiedName Message { get; set; } = XmlQualifiedName.Empty;
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class OperationFault : OperationMessage
    {
        private ServiceDescriptionFormatExtensionCollection _extensions;

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class OperationInput : OperationMessage
    {
        private ServiceDescriptionFormatExtensionCollection _extensions;

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class OperationOutput : OperationMessage
    {
        private ServiceDescriptionFormatExtensionCollection _extensions;

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class Operation : NamedItem
    {
        private OperationMessageCollection _messages;
        private OperationFaultCollection _faults;
        private ServiceDescriptionFormatExtensionCollection _extensions;

        internal void SetParent(PortType parent)
        {
            PortType = parent;
        }

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }

        public PortType PortType { get; private set; }

        [XmlAttribute("parameterOrder"), DefaultValue("")]
        public string ParameterOrderString
        {
            get
            {
                if (ParameterOrder == null)
                {
                    return string.Empty;
                }

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < ParameterOrder.Length; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(' ');
                    }

                    builder.Append(ParameterOrder[i]);
                }
                return builder.ToString();
            }
            set
            {
                if (value == null)
                {
                    ParameterOrder = null;
                }
                else
                {
                    ParameterOrder = value.Split(new char[] { ' ' });
                }
            }
        }

        [XmlIgnore]
        public string[] ParameterOrder { get; set; }

        [XmlElement("input", typeof(OperationInput)),
        XmlElement("output", typeof(OperationOutput))]
        public OperationMessageCollection Messages
        {
            get { if (_messages == null) { _messages = new OperationMessageCollection(this); } return _messages; }
        }

        [XmlElement("fault")]
        public OperationFaultCollection Faults
        {
            get { if (_faults == null) { _faults = new OperationFaultCollection(this); } return _faults; }
        }

        public bool IsBoundBy(OperationBinding operationBinding)
        {
            if (operationBinding.Name != Name)
            {
                return false;
            }

            OperationMessage input = Messages.Input;
            if (input != null)
            {
                if (operationBinding.Input == null)
                {
                    return false;
                }

                string portTypeInputName = GetMessageName(Name, input.Name, true);
                string bindingInputName = GetMessageName(operationBinding.Name, operationBinding.Input.Name, true);
                if (bindingInputName != portTypeInputName)
                {
                    return false;
                }
            }
            else if (operationBinding.Input != null)
            {
                return false;
            }

            OperationMessage output = Messages.Output;
            if (output != null)
            {
                if (operationBinding.Output == null)
                {
                    return false;
                }

                string portTypeOutputName = GetMessageName(Name, output.Name, false);
                string bindingOutputName = GetMessageName(operationBinding.Name, operationBinding.Output.Name, false);
                if (bindingOutputName != portTypeOutputName)
                {
                    return false;
                }
            }
            else if (operationBinding.Output != null)
            {
                return false;
            }

            return true;
        }

        private string GetMessageName(string operationName, string messageName, bool isInput)
        {
            if (messageName != null && messageName.Length > 0)
            {
                return messageName;
            }

            switch (Messages.Flow)
            {
                case OperationFlow.RequestResponse:
                    if (isInput)
                    {
                        return operationName + "Request";
                    }

                    return operationName + "Response";
                case OperationFlow.OneWay:
                    if (isInput)
                    {
                        return operationName;
                    }

                    Debug.Assert(isInput == true, "Oneway flow cannot have an output message");
                    return null;
                // Cases not supported
                case OperationFlow.SolicitResponse:
                    return null;
                case OperationFlow.Notification:
                    return null;
            }
            Debug.Assert(false, "Unknown message flow");
            return null;
        }
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class PortType : NamedItem
    {
        private OperationCollection _operations;
        private ServiceDescriptionFormatExtensionCollection _extensions;

        internal void SetParent(ServiceDescription parent)
        {
            ServiceDescription = parent;
        }

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }

        public ServiceDescription ServiceDescription { get; private set; }

        [XmlElement("operation")]
        public OperationCollection Operations
        {
            get { if (_operations == null) { _operations = new OperationCollection(this); } return _operations; }
        }
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class Message : NamedItem
    {
        private MessagePartCollection _parts;
        private ServiceDescriptionFormatExtensionCollection _extensions;

        internal void SetParent(ServiceDescription parent)
        {
            ServiceDescription = parent;
        }

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }

        public ServiceDescription ServiceDescription { get; private set; }

        [XmlElement("part")]
        public MessagePartCollection Parts
        {
            get { if (_parts == null) { _parts = new MessagePartCollection(this); } return _parts; }
        }

        public MessagePart[] FindPartsByName(string[] partNames)
        {
            MessagePart[] partArray = new MessagePart[partNames.Length];
            for (int i = 0; i < partNames.Length; i++)
            {
                partArray[i] = FindPartByName(partNames[i]);
            }
            return partArray;
        }

        public MessagePart FindPartByName(string partName)
        {
            for (int i = 0; i < _parts.Count; i++)
            {
                MessagePart part = _parts[i];
                if (part.Name == partName)
                {
                    return part;
                }
            }
            throw new ArgumentException(SR.Format(SR.MissingMessagePartForMessageFromNamespace3, partName, Name, ServiceDescription.TargetNamespace), nameof(partName));
        }
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class MessagePart : NamedItem
    {
        private XmlQualifiedName _type = XmlQualifiedName.Empty;
        private ServiceDescriptionFormatExtensionCollection _extensions;

        internal void SetParent(Message parent)
        {
            Message = parent;
        }

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }

        public Message Message { get; private set; }

        [XmlAttribute("element")]
        public XmlQualifiedName Element { get; set; } = XmlQualifiedName.Empty;

        [XmlAttribute("type")]
        public XmlQualifiedName Type
        {
            get
            {
                if ((object)_type == null)
                {
                    return XmlQualifiedName.Empty;
                }

                return _type;
            }
            set { _type = value; }
        }
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class Types : DocumentableItem
    {
        private XmlSchemas _schemas;
        private ServiceDescriptionFormatExtensionCollection _extensions;

        internal bool HasItems()
        {
            return (_schemas != null && _schemas.Count > 0) ||
                (_extensions != null && _extensions.Count > 0);
        }

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }

        [XmlElement("schema", typeof(XmlSchema), Namespace = XmlSchema.Namespace)]
        public XmlSchemas Schemas
        {
            get { if (_schemas == null) { _schemas = new XmlSchemas(); } return _schemas; }
        }
    }

    public sealed class ServiceDescriptionFormatExtensionCollection : ServiceDescriptionBaseCollection
    {
        public ServiceDescriptionFormatExtensionCollection(object parent) : base(parent) { }

        private ArrayList _handledElements;

        public object this[int index]
        {
            get { return (object)List[index]; }
            set { List[index] = value; }
        }

        public int Add(object extension)
        {
            return List.Add(extension);
        }

        public void Insert(int index, object extension)
        {
            List.Insert(index, extension);
        }

        public int IndexOf(object extension)
        {
            return List.IndexOf(extension);
        }

        public bool Contains(object extension)
        {
            return List.Contains(extension);
        }

        public void Remove(object extension)
        {
            List.Remove(extension);
        }

        public void CopyTo(object[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public object Find(Type type)
        {
            for (int i = 0; i < List.Count; i++)
            {
                object item = List[i];
                if (type.IsAssignableFrom(item.GetType()))
                {
                    ((ServiceDescriptionFormatExtension)item).Handled = true;
                    return item;
                }
            }
            return null;
        }

        public object[] FindAll(Type type)
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < List.Count; i++)
            {
                object item = List[i];
                if (type.IsAssignableFrom(item.GetType()))
                {
                    ((ServiceDescriptionFormatExtension)item).Handled = true;
                    list.Add(item);
                }
            }
            return (object[])list.ToArray(type);
        }

        public XmlElement Find(string name, string ns)
        {
            for (int i = 0; i < List.Count; i++)
            {
                XmlElement element = List[i] as XmlElement;
                if (element != null && element.LocalName == name && element.NamespaceURI == ns)
                {
                    SetHandled(element);
                    return element;
                }
            }

            return null;
        }

        public XmlElement[] FindAll(string name, string ns)
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < List.Count; i++)
            {
                XmlElement element = List[i] as XmlElement;
                if (element != null && element.LocalName == name && element.NamespaceURI == ns)
                {
                    SetHandled(element);
                    list.Add(element);
                }
            }

            return (XmlElement[])list.ToArray(typeof(XmlElement));
        }

        private void SetHandled(XmlElement element)
        {
            if (_handledElements == null)
            {
                _handledElements = new ArrayList();
            }

            if (!_handledElements.Contains(element))
            {
                _handledElements.Add(element);
            }
        }

        public bool IsHandled(object item)
        {
            return item is XmlElement ? IsHandled((XmlElement)item) : ((ServiceDescriptionFormatExtension)item).Handled;
        }

        public bool IsRequired(object item)
        {
            return item is XmlElement ? IsRequired((XmlElement)item) : ((ServiceDescriptionFormatExtension)item).Required;
        }

        private bool IsHandled(XmlElement element)
        {
            return _handledElements == null ? false : _handledElements.Contains(element);
        }

        private bool IsRequired(XmlElement element)
        {
            XmlAttribute requiredAttr = element.Attributes["required", ServiceDescription.Namespace];
            if (requiredAttr == null || requiredAttr.Value == null)
            {
                requiredAttr = element.Attributes["required"];
                if (requiredAttr == null || requiredAttr.Value == null)
                {
                    return false; // not required, by default
                }
            }

            return XmlConvert.ToBoolean(requiredAttr.Value);
        }

        protected override void SetParent(object value, object parent)
        {
            if (value is ServiceDescriptionFormatExtension extension)
            {
                extension.SetParent(parent);
            }
        }

        protected override void OnValidate(object value)
        {
            if (!(value is XmlElement || value is ServiceDescriptionFormatExtension))
            {
                throw new ArgumentException(SR.OnlyXmlElementsOrTypesDerivingFromServiceDescriptionFormatExtension0, nameof(value));
            }

            base.OnValidate(value);
        }
    }

    public abstract class ServiceDescriptionFormatExtension
    {
        internal void SetParent(object parent)
        {
            Parent = parent;
        }

        public object Parent { get; private set; }

        [XmlAttribute("required", Namespace = ServiceDescription.Namespace), DefaultValue(false)]
        public bool Required { get; set; }

        [XmlIgnore]
        public bool Handled { get; set; }
    }

    public enum OperationFlow
    {
        None,
        OneWay,
        Notification,
        RequestResponse,
        SolicitResponse,
    }

    public sealed class OperationMessageCollection : ServiceDescriptionBaseCollection
    {
        internal OperationMessageCollection(Operation operation) : base(operation) { }

        public OperationMessage this[int index]
        {
            get { return (OperationMessage)List[index]; }
            set { List[index] = value; }
        }

        public int Add(OperationMessage operationMessage)
        {
            return List.Add(operationMessage);
        }

        public void Insert(int index, OperationMessage operationMessage)
        {
            List.Insert(index, operationMessage);
        }

        public int IndexOf(OperationMessage operationMessage)
        {
            return List.IndexOf(operationMessage);
        }

        public bool Contains(OperationMessage operationMessage)
        {
            return List.Contains(operationMessage);
        }

        public void Remove(OperationMessage operationMessage)
        {
            List.Remove(operationMessage);
        }

        public void CopyTo(OperationMessage[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public OperationInput Input
        {
            get
            {
                for (int i = 0; i < List.Count; i++)
                {
                    OperationInput input = List[i] as OperationInput;
                    if (input != null)
                    {
                        return input;
                    }
                }
                return null;
            }
        }

        public OperationOutput Output
        {
            get
            {
                for (int i = 0; i < List.Count; i++)
                {
                    OperationOutput output = List[i] as OperationOutput;
                    if (output != null)
                    {
                        return output;
                    }
                }

                return null;
            }
        }

        public OperationFlow Flow
        {
            get
            {
                if (List.Count == 0)
                {
                    return OperationFlow.None;
                }
                else if (List.Count == 1)
                {
                    if (List[0] is OperationInput)
                    {
                        return OperationFlow.OneWay;
                    }
                    else
                    {
                        return OperationFlow.Notification;
                    }
                }
                else
                {
                    if (List[0] is OperationInput)
                    {
                        return OperationFlow.RequestResponse;
                    }
                    else
                    {
                        return OperationFlow.SolicitResponse;
                    }
                }
            }
        }

        protected override void SetParent(object value, object parent)
        {
            ((OperationMessage)value).SetParent((Operation)parent);
        }

        protected override void OnInsert(int index, object value)
        {
            if (Count > 1 || (Count == 1 && value.GetType() == List[0].GetType()))
            {
                throw new InvalidOperationException(SR.WebDescriptionTooManyMessages);
            }

            base.OnInsert(index, value);
        }

        protected override void OnSet(int index, object oldValue, object newValue)
        {
            if (oldValue.GetType() != newValue.GetType())
            {
                throw new InvalidOperationException(SR.WebDescriptionTooManyMessages);
            }

            base.OnSet(index, oldValue, newValue);
        }

        protected override void OnValidate(object value)
        {
            if (!(value is OperationInput || value is OperationOutput))
            {
                throw new ArgumentException(SR.OnlyOperationInputOrOperationOutputTypes, nameof(value));
            }

            base.OnValidate(value);
        }
    }

    public sealed class ImportCollection : ServiceDescriptionBaseCollection
    {
        internal ImportCollection(ServiceDescription serviceDescription) : base(serviceDescription) { }

        public Import this[int index]
        {
            get { return (Import)List[index]; }
            set { List[index] = value; }
        }

        public int Add(Import import)
        {
            return List.Add(import);
        }

        public void Insert(int index, Import import)
        {
            List.Insert(index, import);
        }

        public int IndexOf(Import import)
        {
            return List.IndexOf(import);
        }

        public bool Contains(Import import)
        {
            return List.Contains(import);
        }

        public void Remove(Import import)
        {
            List.Remove(import);
        }

        public void CopyTo(Import[] array, int index)
        {
            List.CopyTo(array, index);
        }

        protected override void SetParent(object value, object parent)
        {
            ((Import)value).SetParent((ServiceDescription)parent);
        }
    }

    public sealed class MessageCollection : ServiceDescriptionBaseCollection
    {
        internal MessageCollection(ServiceDescription serviceDescription) : base(serviceDescription) { }

        public Message this[int index]
        {
            get { return (Message)List[index]; }
            set { List[index] = value; }
        }

        public int Add(Message message)
        {
            return List.Add(message);
        }

        public void Insert(int index, Message message)
        {
            List.Insert(index, message);
        }

        public int IndexOf(Message message)
        {
            return List.IndexOf(message);
        }

        public bool Contains(Message message)
        {
            return List.Contains(message);
        }

        public void Remove(Message message)
        {
            List.Remove(message);
        }

        public void CopyTo(Message[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public Message this[string name]
        {
            get { return (Message)Table[name]; }
        }

        protected override string GetKey(object value)
        {
            return ((Message)value).Name;
        }

        protected override void SetParent(object value, object parent)
        {
            ((Message)value).SetParent((ServiceDescription)parent);
        }
    }

    public sealed class PortCollection : ServiceDescriptionBaseCollection
    {
        internal PortCollection(Service service) : base(service) { }

        public Port this[int index]
        {
            get { return (Port)List[index]; }
            set { List[index] = value; }
        }

        public int Add(Port port)
        {
            return List.Add(port);
        }

        public void Insert(int index, Port port)
        {
            List.Insert(index, port);
        }

        public int IndexOf(Port port)
        {
            return List.IndexOf(port);
        }

        public bool Contains(Port port)
        {
            return List.Contains(port);
        }

        public void Remove(Port port)
        {
            List.Remove(port);
        }

        public void CopyTo(Port[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public Port this[string name]
        {
            get { return (Port)Table[name]; }
        }

        protected override string GetKey(object value)
        {
            return ((Port)value).Name;
        }

        protected override void SetParent(object value, object parent)
        {
            ((Port)value).SetParent((Service)parent);
        }
    }

    public sealed class PortTypeCollection : ServiceDescriptionBaseCollection
    {
        internal PortTypeCollection(ServiceDescription serviceDescription) : base(serviceDescription) { }

        public PortType this[int index]
        {
            get { return (PortType)List[index]; }
            set { List[index] = value; }
        }

        public int Add(PortType portType)
        {
            return List.Add(portType);
        }

        public void Insert(int index, PortType portType)
        {
            List.Insert(index, portType);
        }

        public int IndexOf(PortType portType)
        {
            return List.IndexOf(portType);
        }

        public bool Contains(PortType portType)
        {
            return List.Contains(portType);
        }

        public void Remove(PortType portType)
        {
            List.Remove(portType);
        }

        public void CopyTo(PortType[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public PortType this[string name]
        {
            get { return (PortType)Table[name]; }
        }

        protected override string GetKey(object value)
        {
            return ((PortType)value).Name;
        }

        protected override void SetParent(object value, object parent)
        {
            ((PortType)value).SetParent((ServiceDescription)parent);
        }
    }

    public sealed class BindingCollection : ServiceDescriptionBaseCollection
    {
        internal BindingCollection(ServiceDescription serviceDescription) : base(serviceDescription) { }

        public Binding this[int index]
        {
            get { return (Binding)List[index]; }
            set { List[index] = value; }
        }

        public int Add(Binding binding)
        {
            return List.Add(binding);
        }

        public void Insert(int index, Binding binding)
        {
            List.Insert(index, binding);
        }

        public int IndexOf(Binding binding)
        {
            return List.IndexOf(binding);
        }

        public bool Contains(Binding binding)
        {
            return List.Contains(binding);
        }

        public void Remove(Binding binding)
        {
            List.Remove(binding);
        }

        public void CopyTo(Binding[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public Binding this[string name]
        {
            get { return (Binding)Table[name]; }
        }

        protected override string GetKey(object value)
        {
            return ((Binding)value).Name;
        }

        protected override void SetParent(object value, object parent)
        {
            ((Binding)value).SetParent((ServiceDescription)parent);
        }
    }

    public sealed class ServiceCollection : ServiceDescriptionBaseCollection
    {
        internal ServiceCollection(ServiceDescription serviceDescription) : base(serviceDescription) { }

        public Service this[int index]
        {
            get { return (Service)List[index]; }
            set { List[index] = value; }
        }

        public int Add(Service service)
        {
            return List.Add(service);
        }

        public void Insert(int index, Service service)
        {
            List.Insert(index, service);
        }

        public int IndexOf(Service service)
        {
            return List.IndexOf(service);
        }

        public bool Contains(Service service)
        {
            return List.Contains(service);
        }

        public void Remove(Service service)
        {
            List.Remove(service);
        }

        public void CopyTo(Service[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public Service this[string name]
        {
            get { return (Service)Table[name]; }
        }

        protected override string GetKey(object value)
        {
            return ((Service)value).Name;
        }

        protected override void SetParent(object value, object parent)
        {
            ((Service)value).SetParent((ServiceDescription)parent);
        }
    }

    public sealed class MessagePartCollection : ServiceDescriptionBaseCollection
    {
        internal MessagePartCollection(Message message) : base(message) { }

        public MessagePart this[int index]
        {
            get { return (MessagePart)List[index]; }
            set { List[index] = value; }
        }

        public int Add(MessagePart messagePart)
        {
            return List.Add(messagePart);
        }

        public void Insert(int index, MessagePart messagePart)
        {
            List.Insert(index, messagePart);
        }

        public int IndexOf(MessagePart messagePart)
        {
            return List.IndexOf(messagePart);
        }

        public bool Contains(MessagePart messagePart)
        {
            return List.Contains(messagePart);
        }

        public void Remove(MessagePart messagePart)
        {
            List.Remove(messagePart);
        }

        public void CopyTo(MessagePart[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public MessagePart this[string name]
        {
            get { return (MessagePart)Table[name]; }
        }

        protected override string GetKey(object value)
        {
            return ((MessagePart)value).Name;
        }

        protected override void SetParent(object value, object parent)
        {
            ((MessagePart)value).SetParent((Message)parent);
        }
    }

    public sealed class OperationBindingCollection : ServiceDescriptionBaseCollection
    {
        internal OperationBindingCollection(Binding binding) : base(binding) { }

        public OperationBinding this[int index]
        {
            get { return (OperationBinding)List[index]; }
            set { List[index] = value; }
        }

        public int Add(OperationBinding bindingOperation)
        {
            return List.Add(bindingOperation);
        }

        public void Insert(int index, OperationBinding bindingOperation)
        {
            List.Insert(index, bindingOperation);
        }

        public int IndexOf(OperationBinding bindingOperation)
        {
            return List.IndexOf(bindingOperation);
        }

        public bool Contains(OperationBinding bindingOperation)
        {
            return List.Contains(bindingOperation);
        }

        public void Remove(OperationBinding bindingOperation)
        {
            List.Remove(bindingOperation);
        }

        public void CopyTo(OperationBinding[] array, int index)
        {
            List.CopyTo(array, index);
        }

        protected override void SetParent(object value, object parent)
        {
            ((OperationBinding)value).SetParent((Binding)parent);
        }
    }

    public sealed class FaultBindingCollection : ServiceDescriptionBaseCollection
    {
        internal FaultBindingCollection(OperationBinding operationBinding) : base(operationBinding) { }

        public FaultBinding this[int index]
        {
            get { return (FaultBinding)List[index]; }
            set { List[index] = value; }
        }

        public int Add(FaultBinding bindingOperationFault)
        {
            return List.Add(bindingOperationFault);
        }

        public void Insert(int index, FaultBinding bindingOperationFault)
        {
            List.Insert(index, bindingOperationFault);
        }

        public int IndexOf(FaultBinding bindingOperationFault)
        {
            return List.IndexOf(bindingOperationFault);
        }

        public bool Contains(FaultBinding bindingOperationFault)
        {
            return List.Contains(bindingOperationFault);
        }

        public void Remove(FaultBinding bindingOperationFault)
        {
            List.Remove(bindingOperationFault);
        }

        public void CopyTo(FaultBinding[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public FaultBinding this[string name]
        {
            get { return (FaultBinding)Table[name]; }
        }

        protected override string GetKey(object value)
        {
            return ((FaultBinding)value).Name;
        }

        protected override void SetParent(object value, object parent)
        {
            ((FaultBinding)value).SetParent((OperationBinding)parent);
        }
    }

    public sealed class OperationCollection : ServiceDescriptionBaseCollection
    {
        internal OperationCollection(PortType portType) : base(portType) { }

        public Operation this[int index]
        {
            get { return (Operation)List[index]; }
            set { List[index] = value; }
        }

        public int Add(Operation operation)
        {
            return List.Add(operation);
        }

        public void Insert(int index, Operation operation)
        {
            List.Insert(index, operation);
        }

        public int IndexOf(Operation operation)
        {
            return List.IndexOf(operation);
        }

        public bool Contains(Operation operation)
        {
            return List.Contains(operation);
        }

        public void Remove(Operation operation)
        {
            List.Remove(operation);
        }

        public void CopyTo(Operation[] array, int index)
        {
            List.CopyTo(array, index);
        }

        protected override void SetParent(object value, object parent)
        {
            ((Operation)value).SetParent((PortType)parent);
        }
    }

    public sealed class OperationFaultCollection : ServiceDescriptionBaseCollection
    {
        internal OperationFaultCollection(Operation operation) : base(operation) { }

        public OperationFault this[int index]
        {
            get { return (OperationFault)List[index]; }
            set { List[index] = value; }
        }

        public int Add(OperationFault operationFaultMessage)
        {
            return List.Add(operationFaultMessage);
        }

        public void Insert(int index, OperationFault operationFaultMessage)
        {
            List.Insert(index, operationFaultMessage);
        }

        public int IndexOf(OperationFault operationFaultMessage)
        {
            return List.IndexOf(operationFaultMessage);
        }

        public bool Contains(OperationFault operationFaultMessage)
        {
            return List.Contains(operationFaultMessage);
        }

        public void Remove(OperationFault operationFaultMessage)
        {
            List.Remove(operationFaultMessage);
        }

        public void CopyTo(OperationFault[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public OperationFault this[string name]
        {
            get { return (OperationFault)Table[name]; }
        }

        protected override string GetKey(object value)
        {
            return ((OperationFault)value).Name;
        }

        protected override void SetParent(object value, object parent)
        {
            ((OperationFault)value).SetParent((Operation)parent);
        }
    }

    public abstract class ServiceDescriptionBaseCollection : CollectionBase
    {
        private Hashtable _table;
        private object _parent;

        internal ServiceDescriptionBaseCollection(object parent)
        {
            _parent = parent;
        }

        protected virtual IDictionary Table
        {
            get { if (_table == null) { _table = new Hashtable(); } return _table; }
        }

        protected virtual string GetKey(object value)
        {
            return null; // returning null means there is no key
        }

        protected virtual void SetParent(object value, object parent)
        {
            // default is that the item has no parent
        }

        protected override void OnInsertComplete(int index, object value)
        {
            AddValue(value);
        }

        protected override void OnRemove(int index, object value)
        {
            RemoveValue(value);
        }

        protected override void OnClear()
        {
            for (int i = 0; i < List.Count; i++)
            {
                RemoveValue(List[i]);
            }
        }

        protected override void OnSet(int index, object oldValue, object newValue)
        {
            RemoveValue(oldValue);
            AddValue(newValue);
        }

        private void AddValue(object value)
        {
            string key = GetKey(value);
            if (key != null)
            {
                try
                {
                    Table.Add(key, value);
                }
                catch (Exception e)
                {
                    if (e is ThreadAbortException || e is StackOverflowException || e is OutOfMemoryException)
                    {
                        throw;
                    }
                    if (Table[key] != null)
                    {
                        throw new ArgumentException(GetDuplicateMessage(value.GetType(), key), e.InnerException);
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
            SetParent(value, _parent);
        }

        private void RemoveValue(object value)
        {
            string key = GetKey(value);
            if (key != null)
            {
                Table.Remove(key);
            }

            SetParent(value, null);
        }

        private static string GetDuplicateMessage(Type type, string elemName)
        {
            string message;
            if (type == typeof(ServiceDescriptionFormatExtension))
            {
                message = SR.Format(SR.WebDuplicateFormatExtension, elemName);
            }
            else if (type == typeof(OperationMessage))
            {
                message = SR.Format(SR.WebDuplicateOperationMessage, elemName);
            }
            else if (type == typeof(Import))
            {
                message = SR.Format(SR.WebDuplicateImport, elemName);
            }
            else if (type == typeof(Message))
            {
                message = SR.Format(SR.WebDuplicateMessage, elemName);
            }
            else if (type == typeof(Port))
            {
                message = SR.Format(SR.WebDuplicatePort, elemName);
            }
            else if (type == typeof(PortType))
            {
                message = SR.Format(SR.WebDuplicatePortType, elemName);
            }
            else if (type == typeof(Binding))
            {
                message = SR.Format(SR.WebDuplicateBinding, elemName);
            }
            else if (type == typeof(Service))
            {
                message = SR.Format(SR.WebDuplicateService, elemName);
            }
            else if (type == typeof(MessagePart))
            {
                message = SR.Format(SR.WebDuplicateMessagePart, elemName);
            }
            else if (type == typeof(OperationBinding))
            {
                message = SR.Format(SR.WebDuplicateOperationBinding, elemName);
            }
            else if (type == typeof(FaultBinding))
            {
                message = SR.Format(SR.WebDuplicateFaultBinding, elemName);
            }
            else if (type == typeof(Operation))
            {
                message = SR.Format(SR.WebDuplicateOperation, elemName);
            }
            else if (type == typeof(OperationFault))
            {
                message = SR.Format(SR.WebDuplicateOperationFault, elemName);
            }
            else
            {
                message = SR.Format(SR.WebDuplicateUnknownElement, type, elemName);
            }

            return message;
        }
    }

    internal class Schemas
    {
        private Schemas() { }
        internal const string Wsdl = @"<?xml version='1.0' encoding='UTF-8' ?>
<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'
           xmlns:wsdl='http://schemas.xmlsoap.org/wsdl/'
           targetNamespace='http://schemas.xmlsoap.org/wsdl/'
           elementFormDefault='qualified' >

  <xs:complexType mixed='true' name='tDocumentation' >
    <xs:sequence>
      <xs:any minOccurs='0' maxOccurs='unbounded' processContents='lax' />
    </xs:sequence>
  </xs:complexType>

  <xs:complexType name='tDocumented' >
    <xs:annotation>
      <xs:documentation>
      This type is extended by  component types to allow them to be documented
      </xs:documentation>
    </xs:annotation>
    <xs:sequence>
      <xs:element name='documentation' type='wsdl:tDocumentation' minOccurs='0' />
    </xs:sequence>
  </xs:complexType>
 <!-- allow extensibility via elements and attributes on all elements swa124 -->
 <xs:complexType name='tExtensibleAttributesDocumented' abstract='true' >
    <xs:complexContent>
      <xs:extension base='wsdl:tDocumented' >
        <xs:annotation>
          <xs:documentation>
          This type is extended by component types to allow attributes from other namespaces to be added.
          </xs:documentation>
        </xs:annotation>
        <xs:sequence>
          <xs:any namespace='##other' minOccurs='0' maxOccurs='unbounded' processContents='lax' />
        </xs:sequence>
        <xs:anyAttribute namespace='##other' processContents='lax' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:complexType name='tExtensibleDocumented' abstract='true' >
    <xs:complexContent>
      <xs:extension base='wsdl:tDocumented' >
        <xs:annotation>
          <xs:documentation>
          This type is extended by component types to allow elements from other namespaces to be added.
          </xs:documentation>
        </xs:annotation>
        <xs:sequence>
          <xs:any namespace='##other' minOccurs='0' maxOccurs='unbounded' processContents='lax' />
        </xs:sequence>
        <xs:anyAttribute namespace='##other' processContents='lax' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- original wsdl removed as part of swa124 resolution
  <xs:complexType name='tExtensibleAttributesDocumented' abstract='true' >
    <xs:complexContent>
      <xs:extension base='wsdl:tDocumented' >
        <xs:annotation>
          <xs:documentation>
          This type is extended by component types to allow attributes from other namespaces to be added.
          </xs:documentation>
        </xs:annotation>
        <xs:anyAttribute namespace='##other' processContents='lax' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tExtensibleDocumented' abstract='true' >
    <xs:complexContent>
      <xs:extension base='wsdl:tDocumented' >
        <xs:annotation>
          <xs:documentation>
          This type is extended by component types to allow elements from other namespaces to be added.
          </xs:documentation>
        </xs:annotation>
        <xs:sequence>
          <xs:any namespace='##other' minOccurs='0' maxOccurs='unbounded' processContents='lax' />
        </xs:sequence>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
 -->
  <xs:element name='definitions' type='wsdl:tDefinitions' >
    <xs:key name='message' >
      <xs:selector xpath='wsdl:message' />
      <xs:field xpath='@name' />
    </xs:key>
    <xs:key name='portType' >
      <xs:selector xpath='wsdl:portType' />
      <xs:field xpath='@name' />
    </xs:key>
    <xs:key name='binding' >
      <xs:selector xpath='wsdl:binding' />
      <xs:field xpath='@name' />
    </xs:key>
    <xs:key name='service' >
      <xs:selector xpath='wsdl:service' />
      <xs:field xpath='@name' />
    </xs:key>
    <xs:key name='import' >
      <xs:selector xpath='wsdl:import' />
      <xs:field xpath='@namespace' />
    </xs:key>
  </xs:element>

  <xs:group name='anyTopLevelOptionalElement' >
    <xs:annotation>
      <xs:documentation>
      Any top level optional element allowed to appear more then once - any child of definitions element except wsdl:types. Any extensibility element is allowed in any place.
      </xs:documentation>
    </xs:annotation>
    <xs:choice>
      <xs:element name='import' type='wsdl:tImport' />
      <xs:element name='types' type='wsdl:tTypes' />
      <xs:element name='message'  type='wsdl:tMessage' >
        <xs:unique name='part' >
          <xs:selector xpath='wsdl:part' />
          <xs:field xpath='@name' />
        </xs:unique>
      </xs:element>
      <xs:element name='portType' type='wsdl:tPortType' />
      <xs:element name='binding'  type='wsdl:tBinding' />
      <xs:element name='service'  type='wsdl:tService' >
        <xs:unique name='port' >
          <xs:selector xpath='wsdl:port' />
          <xs:field xpath='@name' />
        </xs:unique>
      </xs:element>
    </xs:choice>
  </xs:group>

  <xs:complexType name='tDefinitions' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:sequence>
          <xs:group ref='wsdl:anyTopLevelOptionalElement'  minOccurs='0'   maxOccurs='unbounded' />
        </xs:sequence>
        <xs:attribute name='targetNamespace' type='xs:anyURI' use='optional' />
        <xs:attribute name='name' type='xs:NCName' use='optional' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tImport' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleAttributesDocumented' >
        <xs:attribute name='namespace' type='xs:anyURI' use='required' />
        <xs:attribute name='location' type='xs:anyURI' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tTypes' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' />
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tMessage' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:sequence>
          <xs:element name='part' type='wsdl:tPart' minOccurs='0' maxOccurs='unbounded' />
        </xs:sequence>
        <xs:attribute name='name' type='xs:NCName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tPart' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleAttributesDocumented' >
        <xs:attribute name='name' type='xs:NCName' use='required' />
        <xs:attribute name='element' type='xs:QName' use='optional' />
        <xs:attribute name='type' type='xs:QName' use='optional' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tPortType' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleAttributesDocumented' >
        <xs:sequence>
          <xs:element name='operation' type='wsdl:tOperation' minOccurs='0' maxOccurs='unbounded' />
        </xs:sequence>
        <xs:attribute name='name' type='xs:NCName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tOperation' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:sequence>
          <xs:choice>
            <xs:group ref='wsdl:request-response-or-one-way-operation' />
            <xs:group ref='wsdl:solicit-response-or-notification-operation' />
          </xs:choice>
        </xs:sequence>
        <xs:attribute name='name' type='xs:NCName' use='required' />
        <xs:attribute name='parameterOrder' type='xs:NMTOKENS' use='optional' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:group name='request-response-or-one-way-operation' >
    <xs:sequence>
      <xs:element name='input' type='wsdl:tParam' />
      <xs:sequence minOccurs='0' >
        <xs:element name='output' type='wsdl:tParam' />
        <xs:element name='fault' type='wsdl:tFault' minOccurs='0' maxOccurs='unbounded' />
      </xs:sequence>
    </xs:sequence>
  </xs:group>

  <xs:group name='solicit-response-or-notification-operation' >
    <xs:sequence>
      <xs:element name='output' type='wsdl:tParam' />
      <xs:sequence minOccurs='0' >
        <xs:element name='input' type='wsdl:tParam' />
        <xs:element name='fault' type='wsdl:tFault' minOccurs='0' maxOccurs='unbounded' />
      </xs:sequence>
    </xs:sequence>
  </xs:group>

  <xs:complexType name='tParam' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleAttributesDocumented' >
        <xs:attribute name='name' type='xs:NCName' use='optional' />
        <xs:attribute name='message' type='xs:QName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tFault' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleAttributesDocumented' >
        <xs:attribute name='name' type='xs:NCName'  use='required' />
        <xs:attribute name='message' type='xs:QName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tBinding' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:sequence>
          <xs:element name='operation' type='wsdl:tBindingOperation' minOccurs='0' maxOccurs='unbounded' />
        </xs:sequence>
        <xs:attribute name='name' type='xs:NCName' use='required' />
        <xs:attribute name='type' type='xs:QName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tBindingOperationMessage' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:attribute name='name' type='xs:NCName' use='optional' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tBindingOperationFault' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:attribute name='name' type='xs:NCName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tBindingOperation' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:sequence>
          <xs:element name='input' type='wsdl:tBindingOperationMessage' minOccurs='0' />
          <xs:element name='output' type='wsdl:tBindingOperationMessage' minOccurs='0' />
          <xs:element name='fault' type='wsdl:tBindingOperationFault' minOccurs='0' maxOccurs='unbounded' />
        </xs:sequence>
        <xs:attribute name='name' type='xs:NCName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tService' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:sequence>
          <xs:element name='port' type='wsdl:tPort' minOccurs='0' maxOccurs='unbounded' />
        </xs:sequence>
        <xs:attribute name='name' type='xs:NCName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tPort' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:attribute name='name' type='xs:NCName' use='required' />
        <xs:attribute name='binding' type='xs:QName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:attribute name='arrayType' type='xs:string' />
  <xs:attribute name='required' type='xs:boolean' />
  <xs:complexType name='tExtensibilityElement' abstract='true' >
    <xs:attribute ref='wsdl:required' use='optional' />
  </xs:complexType>

</xs:schema>";

        internal const string Soap = @"<?xml version='1.0' encoding='UTF-8' ?>
<xs:schema xmlns:soap='http://schemas.xmlsoap.org/wsdl/soap/' xmlns:wsdl='http://schemas.xmlsoap.org/wsdl/' targetNamespace='http://schemas.xmlsoap.org/wsdl/soap/' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xs:import namespace='http://schemas.xmlsoap.org/wsdl/' />
  <xs:simpleType name='encodingStyle'>
    <xs:annotation>
      <xs:documentation>
      'encodingStyle' indicates any canonicalization conventions followed in the contents of the containing element.  For example, the value 'http://schemas.xmlsoap.org/soap/encoding/' indicates the pattern described in SOAP specification
      </xs:documentation>
    </xs:annotation>
    <xs:list itemType='xs:anyURI' />
  </xs:simpleType>
  <xs:element name='binding' type='soap:tBinding' />
  <xs:complexType name='tBinding'>
    <xs:complexContent mixed='false'>
      <xs:extension base='wsdl:tExtensibilityElement'>
        <xs:attribute name='transport' type='xs:anyURI' use='required' />
        <xs:attribute name='style' type='soap:tStyleChoice' use='optional' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:simpleType name='tStyleChoice'>
    <xs:restriction base='xs:string'>
      <xs:enumeration value='rpc' />
      <xs:enumeration value='document' />
    </xs:restriction>
  </xs:simpleType>
  <xs:element name='operation' type='soap:tOperation' />
  <xs:complexType name='tOperation'>
    <xs:complexContent mixed='false'>
      <xs:extension base='wsdl:tExtensibilityElement'>
        <xs:attribute name='soapAction' type='xs:anyURI' use='optional' />
        <xs:attribute name='style' type='soap:tStyleChoice' use='optional' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:element name='body' type='soap:tBody' />
  <xs:attributeGroup name='tBodyAttributes'>
    <xs:attribute name='encodingStyle' type='soap:encodingStyle' use='optional' />
    <xs:attribute name='use' type='soap:useChoice' use='optional' />
    <xs:attribute name='namespace' type='xs:anyURI' use='optional' />
  </xs:attributeGroup>
  <xs:complexType name='tBody'>
    <xs:complexContent mixed='false'>
      <xs:extension base='wsdl:tExtensibilityElement'>
        <xs:attribute name='parts' type='xs:NMTOKENS' use='optional' />
        <xs:attributeGroup ref='soap:tBodyAttributes' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:simpleType name='useChoice'>
    <xs:restriction base='xs:string'>
      <xs:enumeration value='literal' />
      <xs:enumeration value='encoded' />
    </xs:restriction>
  </xs:simpleType>
  <xs:element name='fault' type='soap:tFault' />
  <xs:complexType name='tFaultRes' abstract='true'>
    <xs:complexContent mixed='false'>
      <xs:restriction base='soap:tBody'>
        <xs:attribute ref='wsdl:required' use='optional' />
        <xs:attribute name='parts' type='xs:NMTOKENS' use='prohibited' />
        <xs:attributeGroup ref='soap:tBodyAttributes' />
      </xs:restriction>
    </xs:complexContent>
  </xs:complexType>
  <xs:complexType name='tFault'>
    <xs:complexContent mixed='false'>
      <xs:extension base='soap:tFaultRes'>
        <xs:attribute name='name' type='xs:NCName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:element name='header' type='soap:tHeader' />
  <xs:attributeGroup name='tHeaderAttributes'>
    <xs:attribute name='message' type='xs:QName' use='required' />
    <xs:attribute name='part' type='xs:NMTOKEN' use='required' />
    <xs:attribute name='use' type='soap:useChoice' use='required' />
    <xs:attribute name='encodingStyle' type='soap:encodingStyle' use='optional' />
    <xs:attribute name='namespace' type='xs:anyURI' use='optional' />
  </xs:attributeGroup>
  <xs:complexType name='tHeader'>
    <xs:complexContent mixed='false'>
      <xs:extension base='wsdl:tExtensibilityElement'>
        <xs:sequence>
          <xs:element minOccurs='0' maxOccurs='unbounded' ref='soap:headerfault' />
        </xs:sequence>
        <xs:attributeGroup ref='soap:tHeaderAttributes' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:element name='headerfault' type='soap:tHeaderFault' />
  <xs:complexType name='tHeaderFault'>
    <xs:attributeGroup ref='soap:tHeaderAttributes' />
  </xs:complexType>
  <xs:element name='address' type='soap:tAddress' />
  <xs:complexType name='tAddress'>
    <xs:complexContent mixed='false'>
      <xs:extension base='wsdl:tExtensibilityElement'>
        <xs:attribute name='location' type='xs:anyURI' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
</xs:schema>";

        internal const string WebRef = @"<?xml version='1.0' encoding='UTF-8' ?>
<xs:schema xmlns:tns='http://microsoft.com/webReference/' elementFormDefault='qualified' targetNamespace='http://microsoft.com/webReference/' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xs:simpleType name='options'>
    <xs:list>
      <xs:simpleType>
        <xs:restriction base='xs:string'>
          <xs:enumeration value='properties' />
          <xs:enumeration value='newAsync' />
          <xs:enumeration value='oldAsync' />
          <xs:enumeration value='order' />
          <xs:enumeration value='enableDataBinding' />
        </xs:restriction>
      </xs:simpleType>
    </xs:list>
  </xs:simpleType>
  <xs:simpleType name='style'>
    <xs:restriction base='xs:string'>
      <xs:enumeration value='client' />
      <xs:enumeration value='server' />
      <xs:enumeration value='serverInterface' />
    </xs:restriction>
  </xs:simpleType>
  <xs:complexType name='webReferenceOptions'>
    <xs:all>
      <xs:element minOccurs='0' default='oldAsync' name='codeGenerationOptions' type='tns:options' />
      <xs:element minOccurs='0' default='client' name='style' type='tns:style' />
      <xs:element minOccurs='0' default='false' name='verbose' type='xs:boolean' />
      <xs:element minOccurs='0' name='schemaImporterExtensions'>
        <xs:complexType>
          <xs:sequence>
            <xs:element minOccurs='0' maxOccurs='unbounded' name='type' type='xs:string' />
          </xs:sequence>
        </xs:complexType>
      </xs:element>
    </xs:all>
  </xs:complexType>
  <xs:element name='webReferenceOptions' type='tns:webReferenceOptions' />
  <xs:complexType name='wsdlParameters'>
    <xs:all>
      <xs:element minOccurs='0' name='appSettingBaseUrl' type='xs:string' />
      <xs:element minOccurs='0' name='appSettingUrlKey' type='xs:string' />
      <xs:element minOccurs='0' name='domain' type='xs:string' />
      <xs:element minOccurs='0' name='out' type='xs:string' />
      <xs:element minOccurs='0' name='password' type='xs:string' />
      <xs:element minOccurs='0' name='proxy' type='xs:string' />
      <xs:element minOccurs='0' name='proxydomain' type='xs:string' />
      <xs:element minOccurs='0' name='proxypassword' type='xs:string' />
      <xs:element minOccurs='0' name='proxyusername' type='xs:string' />
      <xs:element minOccurs='0' name='username' type='xs:string' />
      <xs:element minOccurs='0' name='namespace' type='xs:string' />
      <xs:element minOccurs='0' name='language' type='xs:string' />
      <xs:element minOccurs='0' name='protocol' type='xs:string' />
      <xs:element minOccurs='0' name='nologo' type='xs:boolean' />
      <xs:element minOccurs='0' name='parsableerrors' type='xs:boolean' />
      <xs:element minOccurs='0' name='sharetypes' type='xs:boolean' />
      <xs:element minOccurs='0' name='webReferenceOptions' type='tns:webReferenceOptions' />
      <xs:element minOccurs='0' name='documents'>
        <xs:complexType>
          <xs:sequence>
            <xs:element minOccurs='0' maxOccurs='unbounded' name='document' type='xs:string' />
          </xs:sequence>
        </xs:complexType>
      </xs:element>
    </xs:all>
  </xs:complexType>
  <xs:element name='wsdlParameters' type='tns:wsdlParameters' />
</xs:schema>";

        internal const string SoapEncoding = @"<?xml version='1.0' encoding='UTF-8' ?>
<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'
           xmlns:tns='http://schemas.xmlsoap.org/soap/encoding/'
           targetNamespace='http://schemas.xmlsoap.org/soap/encoding/' >

 <xs:attribute name='root' >
   <xs:simpleType>
     <xs:restriction base='xs:boolean'>
       <xs:pattern value='0|1' />
     </xs:restriction>
   </xs:simpleType>
 </xs:attribute>

  <xs:attributeGroup name='commonAttributes' >
    <xs:attribute name='id' type='xs:ID' />
    <xs:attribute name='href' type='xs:anyURI' />
    <xs:anyAttribute namespace='##other' processContents='lax' />
  </xs:attributeGroup>

  <xs:simpleType name='arrayCoordinate' >
    <xs:restriction base='xs:string' />
  </xs:simpleType>

  <xs:attribute name='arrayType' type='xs:string' />
  <xs:attribute name='offset' type='tns:arrayCoordinate' />

  <xs:attributeGroup name='arrayAttributes' >
    <xs:attribute ref='tns:arrayType' />
    <xs:attribute ref='tns:offset' />
  </xs:attributeGroup>

  <xs:attribute name='position' type='tns:arrayCoordinate' />

  <xs:attributeGroup name='arrayMemberAttributes' >
    <xs:attribute ref='tns:position' />
  </xs:attributeGroup>

  <xs:group name='Array' >
    <xs:sequence>
      <xs:any namespace='##any' minOccurs='0' maxOccurs='unbounded' processContents='lax' />
    </xs:sequence>
  </xs:group>

  <xs:element name='Array' type='tns:Array' />
  <xs:complexType name='Array' >
    <xs:group ref='tns:Array' minOccurs='0' />
    <xs:attributeGroup ref='tns:arrayAttributes' />
    <xs:attributeGroup ref='tns:commonAttributes' />
  </xs:complexType>
  <xs:element name='Struct' type='tns:Struct' />
  <xs:group name='Struct' >
    <xs:sequence>
      <xs:any namespace='##any' minOccurs='0' maxOccurs='unbounded' processContents='lax' />
    </xs:sequence>
  </xs:group>

  <xs:complexType name='Struct' >
    <xs:group ref='tns:Struct' minOccurs='0' />
    <xs:attributeGroup ref='tns:commonAttributes'/>
  </xs:complexType>

  <xs:simpleType name='base64' >
    <xs:restriction base='xs:base64Binary' />
  </xs:simpleType>

  <xs:element name='duration' type='tns:duration' />
  <xs:complexType name='duration' >
    <xs:simpleContent>
      <xs:extension base='xs:duration' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='dateTime' type='tns:dateTime' />
  <xs:complexType name='dateTime' >
    <xs:simpleContent>
      <xs:extension base='xs:dateTime' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='NOTATION' type='tns:NOTATION' />
  <xs:complexType name='NOTATION' >
    <xs:simpleContent>
      <xs:extension base='xs:QName' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='time' type='tns:time' />
  <xs:complexType name='time' >
    <xs:simpleContent>
      <xs:extension base='xs:time' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='date' type='tns:date' />
  <xs:complexType name='date' >
    <xs:simpleContent>
      <xs:extension base='xs:date' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='gYearMonth' type='tns:gYearMonth' />
  <xs:complexType name='gYearMonth' >
    <xs:simpleContent>
      <xs:extension base='xs:gYearMonth' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='gYear' type='tns:gYear' />
  <xs:complexType name='gYear' >
    <xs:simpleContent>
      <xs:extension base='xs:gYear' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='gMonthDay' type='tns:gMonthDay' />
  <xs:complexType name='gMonthDay' >
    <xs:simpleContent>
      <xs:extension base='xs:gMonthDay' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='gDay' type='tns:gDay' />
  <xs:complexType name='gDay' >
    <xs:simpleContent>
      <xs:extension base='xs:gDay' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='gMonth' type='tns:gMonth' />
  <xs:complexType name='gMonth' >
    <xs:simpleContent>
      <xs:extension base='xs:gMonth' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='boolean' type='tns:boolean' />
  <xs:complexType name='boolean' >
    <xs:simpleContent>
      <xs:extension base='xs:boolean' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='base64Binary' type='tns:base64Binary' />
  <xs:complexType name='base64Binary' >
    <xs:simpleContent>
      <xs:extension base='xs:base64Binary' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='hexBinary' type='tns:hexBinary' />
  <xs:complexType name='hexBinary' >
    <xs:simpleContent>
     <xs:extension base='xs:hexBinary' >
       <xs:attributeGroup ref='tns:commonAttributes' />
     </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='float' type='tns:float' />
  <xs:complexType name='float' >
    <xs:simpleContent>
      <xs:extension base='xs:float' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='double' type='tns:double' />
  <xs:complexType name='double' >
    <xs:simpleContent>
      <xs:extension base='xs:double' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='anyURI' type='tns:anyURI' />
  <xs:complexType name='anyURI' >
    <xs:simpleContent>
      <xs:extension base='xs:anyURI' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='QName' type='tns:QName' />
  <xs:complexType name='QName' >
    <xs:simpleContent>
      <xs:extension base='xs:QName' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='string' type='tns:string' />
  <xs:complexType name='string' >
    <xs:simpleContent>
      <xs:extension base='xs:string' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='normalizedString' type='tns:normalizedString' />
  <xs:complexType name='normalizedString' >
    <xs:simpleContent>
      <xs:extension base='xs:normalizedString' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='token' type='tns:token' />
  <xs:complexType name='token' >
    <xs:simpleContent>
      <xs:extension base='xs:token' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='language' type='tns:language' />
  <xs:complexType name='language' >
    <xs:simpleContent>
      <xs:extension base='xs:language' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='Name' type='tns:Name' />
  <xs:complexType name='Name' >
    <xs:simpleContent>
      <xs:extension base='xs:Name' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='NMTOKEN' type='tns:NMTOKEN' />
  <xs:complexType name='NMTOKEN' >
    <xs:simpleContent>
      <xs:extension base='xs:NMTOKEN' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='NCName' type='tns:NCName' />
  <xs:complexType name='NCName' >
    <xs:simpleContent>
      <xs:extension base='xs:NCName' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='NMTOKENS' type='tns:NMTOKENS' />
  <xs:complexType name='NMTOKENS' >
    <xs:simpleContent>
      <xs:extension base='xs:NMTOKENS' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='ID' type='tns:ID' />
  <xs:complexType name='ID' >
    <xs:simpleContent>
      <xs:extension base='xs:ID' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='IDREF' type='tns:IDREF' />
  <xs:complexType name='IDREF' >
    <xs:simpleContent>
      <xs:extension base='xs:IDREF' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='ENTITY' type='tns:ENTITY' />
  <xs:complexType name='ENTITY' >
    <xs:simpleContent>
      <xs:extension base='xs:ENTITY' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='IDREFS' type='tns:IDREFS' />
  <xs:complexType name='IDREFS' >
    <xs:simpleContent>
      <xs:extension base='xs:IDREFS' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='ENTITIES' type='tns:ENTITIES' />
  <xs:complexType name='ENTITIES' >
    <xs:simpleContent>
      <xs:extension base='xs:ENTITIES' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='decimal' type='tns:decimal' />
  <xs:complexType name='decimal' >
    <xs:simpleContent>
      <xs:extension base='xs:decimal' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='integer' type='tns:integer' />
  <xs:complexType name='integer' >
    <xs:simpleContent>
      <xs:extension base='xs:integer' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='nonPositiveInteger' type='tns:nonPositiveInteger' />
  <xs:complexType name='nonPositiveInteger' >
    <xs:simpleContent>
      <xs:extension base='xs:nonPositiveInteger' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='negativeInteger' type='tns:negativeInteger' />
  <xs:complexType name='negativeInteger' >
    <xs:simpleContent>
      <xs:extension base='xs:negativeInteger' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='long' type='tns:long' />
  <xs:complexType name='long' >
    <xs:simpleContent>
      <xs:extension base='xs:long' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='int' type='tns:int' />
  <xs:complexType name='int' >
    <xs:simpleContent>
      <xs:extension base='xs:int' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='short' type='tns:short' />
  <xs:complexType name='short' >
    <xs:simpleContent>
      <xs:extension base='xs:short' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='byte' type='tns:byte' />
  <xs:complexType name='byte' >
    <xs:simpleContent>
      <xs:extension base='xs:byte' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='nonNegativeInteger' type='tns:nonNegativeInteger' />
  <xs:complexType name='nonNegativeInteger' >
    <xs:simpleContent>
      <xs:extension base='xs:nonNegativeInteger' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='unsignedLong' type='tns:unsignedLong' />
  <xs:complexType name='unsignedLong' >
    <xs:simpleContent>
      <xs:extension base='xs:unsignedLong' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='unsignedInt' type='tns:unsignedInt' />
  <xs:complexType name='unsignedInt' >
    <xs:simpleContent>
      <xs:extension base='xs:unsignedInt' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='unsignedShort' type='tns:unsignedShort' />
  <xs:complexType name='unsignedShort' >
    <xs:simpleContent>
      <xs:extension base='xs:unsignedShort' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='unsignedByte' type='tns:unsignedByte' />
  <xs:complexType name='unsignedByte' >
    <xs:simpleContent>
      <xs:extension base='xs:unsignedByte' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='positiveInteger' type='tns:positiveInteger' />
  <xs:complexType name='positiveInteger' >
    <xs:simpleContent>
      <xs:extension base='xs:positiveInteger' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='anyType' />
</xs:schema>";
    }
}
