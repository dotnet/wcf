// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Web.Services.Description
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.ComponentModel;
    using Microsoft.Xml;
    using Microsoft.Xml.Schema;
    using Microsoft.Xml.Serialization;
    using System.Collections;
    using System.Collections.Specialized;

    /// <include file='doc\WebReferenceOptions.uex' path='docs/doc[@for="WebReferenceOptions"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlType("webReferenceOptions", Namespace = WebReferenceOptions.TargetNamespace)]
    [XmlRoot("webReferenceOptions", Namespace = WebReferenceOptions.TargetNamespace)]
    public class WebReferenceOptions
    {
        public const string TargetNamespace = "http://microsoft.com/webReference/";
        private static XmlSchema s_schema = null;
        private CodeGenerationOptions _codeGenerationOptions = CodeGenerationOptions.GenerateOldAsync;
        private ServiceDescriptionImportStyle _style = ServiceDescriptionImportStyle.Client;
        private StringCollection _schemaImporterExtensions;
        private bool _verbose;

        /// <include file='doc\WebReferenceOptions.uex' path='docs/doc[@for="WebReferenceOptions.CodeGenerationOptions"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlElement("codeGenerationOptions")]
        [DefaultValue(CodeGenerationOptions.GenerateOldAsync)]
        public CodeGenerationOptions CodeGenerationOptions
        {
            get
            {
                return _codeGenerationOptions;
            }
            set
            {
                _codeGenerationOptions = value;
            }
        }

        /// <include file='doc\WebReferenceOptions.uex' path='docs/doc[@for="WebReferenceOptions.SchemaImporterExtensions"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlArray("schemaImporterExtensions")]
        [XmlArrayItem("type")]
        public StringCollection SchemaImporterExtensions
        {
            get
            {
                if (_schemaImporterExtensions == null)
                    _schemaImporterExtensions = new StringCollection();
                return _schemaImporterExtensions;
            }
        }

        /// <include file='doc\WebReferenceOptions.uex' path='docs/doc[@for="WebReferenceOptions.Style"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [DefaultValue(ServiceDescriptionImportStyle.Client)]
        [XmlElement("style")]
        public ServiceDescriptionImportStyle Style
        {
            get
            {
                return _style;
            }
            set
            {
                _style = value;
            }
        }

        /// <include file='doc\WebReferenceOptions.uex' path='docs/doc[@for="WebReferenceOptions.Verbose"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlElement("verbose")]
        public bool Verbose
        {
            get
            {
                return _verbose;
            }
            set
            {
                _verbose = value;
            }
        }

        /// <include file='doc\WebReferenceOptions.uex' path='docs/doc[@for="WebReferenceOptions.Schema"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static XmlSchema Schema
        {
            get
            {
                if (s_schema == null)
                {
                    s_schema = XmlSchema.Read(new StringReader(Schemas.WebRef), null);
                }
                return s_schema;
            }
        }

        /// <include file='doc\WebReferenceOptions.uex' path='docs/doc[@for="WebReferenceOptions.Read"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static WebReferenceOptions Read(TextReader reader, ValidationEventHandler validationEventHandler)
        {
            XmlTextReader readerNew = new XmlTextReader(reader);
            readerNew.XmlResolver = null;
            readerNew.DtdProcessing = DtdProcessing.Prohibit;
            return Read(readerNew, validationEventHandler);
        }

        /// <include file='doc\WebReferenceOptions.uex' path='docs/doc[@for="WebReferenceOptions.Read1"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static WebReferenceOptions Read(Stream stream, ValidationEventHandler validationEventHandler)
        {
            XmlTextReader readerNew = new XmlTextReader(stream);
            readerNew.XmlResolver = null;
            readerNew.DtdProcessing = DtdProcessing.Prohibit;
            return Read(readerNew, validationEventHandler);
        }

        /// <include file='doc\WebReferenceOptions.uex' path='docs/doc[@for="XmlSchema.Read2"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static WebReferenceOptions Read(XmlReader xmlReader, ValidationEventHandler validationEventHandler)
        {
#pragma warning disable 0618
            XmlValidatingReader validatingReader = new XmlValidatingReader(xmlReader);
#pragma warning restore 0618
            validatingReader.ValidationType = ValidationType.Schema;
            if (validationEventHandler != null)
            {
                validatingReader.ValidationEventHandler += validationEventHandler;
            }
            else
            {
                validatingReader.ValidationEventHandler += new ValidationEventHandler(SchemaValidationHandler);
            }
            validatingReader.Schemas.Add(Schema);
            webReferenceOptionsSerializer ser = new webReferenceOptionsSerializer();
            try
            {
                return (WebReferenceOptions)ser.Deserialize(validatingReader);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                validatingReader.Close();
            }
        }

        private static void SchemaValidationHandler(object sender, ValidationEventArgs args)
        {
            if (args.Severity != XmlSeverityType.Error)
                return;
            throw new InvalidOperationException(string.Format(ResWebServices.WsdlInstanceValidationDetails, args.Message, args.Exception.LineNumber.ToString(CultureInfo.InvariantCulture), args.Exception.LinePosition.ToString(CultureInfo.InvariantCulture)));
        }
    }

    internal class WebReferenceOptionsSerializationWriter : XmlSerializationWriter
    {
        private string Write1_CodeGenerationOptions(Microsoft.Xml.Serialization.CodeGenerationOptions v)
        {
            string s = null;
            switch (v)
            {
                case Microsoft.Xml.Serialization.CodeGenerationOptions.@GenerateProperties: s = @"properties"; break;
                case Microsoft.Xml.Serialization.CodeGenerationOptions.@GenerateNewAsync: s = @"newAsync"; break;
                case Microsoft.Xml.Serialization.CodeGenerationOptions.@GenerateOldAsync: s = @"oldAsync"; break;
                case Microsoft.Xml.Serialization.CodeGenerationOptions.@GenerateOrder: s = @"order"; break;
                case Microsoft.Xml.Serialization.CodeGenerationOptions.@EnableDataBinding: s = @"enableDataBinding"; break;
                default:
                    s = FromEnum(((System.Int64)v), new string[] { @"properties",
                    @"newAsync",
                    @"oldAsync",
                    @"order",
                    @"enableDataBinding" }, new System.Int64[] { (long)Microsoft.Xml.Serialization.CodeGenerationOptions.@GenerateProperties,
                    (long)Microsoft.Xml.Serialization.CodeGenerationOptions.@GenerateNewAsync,
                    (long)Microsoft.Xml.Serialization.CodeGenerationOptions.@GenerateOldAsync,
                    (long)Microsoft.Xml.Serialization.CodeGenerationOptions.@GenerateOrder,
                    (long)Microsoft.Xml.Serialization.CodeGenerationOptions.@EnableDataBinding }, @"System.Xml.Serialization.CodeGenerationOptions"); break;
            }
            return s;
        }

        private string Write2_ServiceDescriptionImportStyle(System.Web.Services.Description.ServiceDescriptionImportStyle v)
        {
            string s = null;
            switch (v)
            {
                case System.Web.Services.Description.ServiceDescriptionImportStyle.@Client: s = @"client"; break;
                case System.Web.Services.Description.ServiceDescriptionImportStyle.@Server: s = @"server"; break;
                case System.Web.Services.Description.ServiceDescriptionImportStyle.@ServerInterface: s = @"serverInterface"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"System.Web.Services.Description.ServiceDescriptionImportStyle");
            }
            return s;
        }

        private void Write4_WebReferenceOptions(string n, string ns, WebReferenceOptions o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(WebReferenceOptions))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o);
            if (needType) WriteXsiType(@"webReferenceOptions", @"http://microsoft.com/webReference/");
            if (((CodeGenerationOptions)o.@CodeGenerationOptions) != (CodeGenerationOptions.@GenerateOldAsync))
            {
                WriteElementString(@"codeGenerationOptions", @"http://microsoft.com/webReference/", Write1_CodeGenerationOptions(((CodeGenerationOptions)o.@CodeGenerationOptions)));
            }
            {
                System.Collections.Specialized.StringCollection a = (System.Collections.Specialized.StringCollection)((System.Collections.Specialized.StringCollection)o.@SchemaImporterExtensions);
                if (a != null)
                {
                    WriteStartElement(@"schemaImporterExtensions", @"http://microsoft.com/webReference/");
                    for (int ia = 0; ia < a.Count; ia++)
                    {
                        WriteNullableStringLiteral(@"type", @"http://microsoft.com/webReference/", ((System.String)a[ia]));
                    }
                    WriteEndElement();
                }
            }
            if (((System.Web.Services.Description.ServiceDescriptionImportStyle)o.@Style) != System.Web.Services.Description.ServiceDescriptionImportStyle.@Client)
            {
                WriteElementString(@"style", @"http://microsoft.com/webReference/", Write2_ServiceDescriptionImportStyle(((System.Web.Services.Description.ServiceDescriptionImportStyle)o.@Style)));
            }
            WriteElementStringRaw(@"verbose", @"http://microsoft.com/webReference/", Microsoft.Xml.XmlConvert.ToString((System.Boolean)((System.Boolean)o.@Verbose)));
            WriteEndElement(o);
        }

        protected override void InitCallbacks()
        {
        }

        internal void Write5_webReferenceOptions(object o)
        {
            WriteStartDocument();
            if (o == null)
            {
                WriteNullTagLiteral(@"webReferenceOptions", @"http://microsoft.com/webReference/");
                return;
            }
            TopLevelElement();
            Write4_WebReferenceOptions(@"webReferenceOptions", @"http://microsoft.com/webReference/", ((System.Web.Services.Description.WebReferenceOptions)o), true, false);
        }
    }

    internal class WebReferenceOptionsSerializationReader : XmlSerializationReader
    {
        private System.Collections.Hashtable _CodeGenerationOptionsValues;

        internal System.Collections.Hashtable CodeGenerationOptionsValues
        {
            get
            {
                if ((object)_CodeGenerationOptionsValues == null)
                {
                    System.Collections.Hashtable h = new System.Collections.Hashtable();
                    h.Add(@"properties", (long)Microsoft.Xml.Serialization.CodeGenerationOptions.@GenerateProperties);
                    h.Add(@"newAsync", (long)Microsoft.Xml.Serialization.CodeGenerationOptions.@GenerateNewAsync);
                    h.Add(@"oldAsync", (long)Microsoft.Xml.Serialization.CodeGenerationOptions.@GenerateOldAsync);
                    h.Add(@"order", (long)Microsoft.Xml.Serialization.CodeGenerationOptions.@GenerateOrder);
                    h.Add(@"enableDataBinding", (long)Microsoft.Xml.Serialization.CodeGenerationOptions.@EnableDataBinding);
                    _CodeGenerationOptionsValues = h;
                }
                return _CodeGenerationOptionsValues;
            }
        }

        private Microsoft.Xml.Serialization.CodeGenerationOptions Read1_CodeGenerationOptions(string s)
        {
            return (Microsoft.Xml.Serialization.CodeGenerationOptions)ToEnum(s, CodeGenerationOptionsValues, @"System.Xml.Serialization.CodeGenerationOptions");
        }

        private System.Web.Services.Description.ServiceDescriptionImportStyle Read2_ServiceDescriptionImportStyle(string s)
        {
            switch (s)
            {
                case @"client": return System.Web.Services.Description.ServiceDescriptionImportStyle.@Client;
                case @"server": return System.Web.Services.Description.ServiceDescriptionImportStyle.@Server;
                case @"serverInterface": return System.Web.Services.Description.ServiceDescriptionImportStyle.@ServerInterface;
                default: throw CreateUnknownConstantException(s, typeof(System.Web.Services.Description.ServiceDescriptionImportStyle));
            }
        }

        private System.Web.Services.Description.WebReferenceOptions Read4_WebReferenceOptions(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id1_webReferenceOptions && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            System.Web.Services.Description.WebReferenceOptions o;
            o = new System.Web.Services.Description.WebReferenceOptions();
            System.Collections.Specialized.StringCollection a_1 = (System.Collections.Specialized.StringCollection)o.@SchemaImporterExtensions;
            bool[] paramsRead = new bool[4];
            while (Reader.MoveToNextAttribute())
            {
                if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o);
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations0 = 0;
            int readerCount0 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id3_codeGenerationOptions && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if (Reader.IsEmptyElement)
                        {
                            Reader.Skip();
                        }
                        else
                        {
                            o.@CodeGenerationOptions = Read1_CodeGenerationOptions(Reader.ReadElementString());
                        }
                        paramsRead[0] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id4_schemaImporterExtensions && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if (!ReadNull())
                        {
                            System.Collections.Specialized.StringCollection a_1_0 = (System.Collections.Specialized.StringCollection)o.@SchemaImporterExtensions;
                            if (((object)(a_1_0) == null) || (Reader.IsEmptyElement))
                            {
                                Reader.Skip();
                            }
                            else
                            {
                                Reader.ReadStartElement();
                                Reader.MoveToContent();
                                int whileIterations1 = 0;
                                int readerCount1 = ReaderCount;
                                while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
                                {
                                    if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                                    {
                                        if (((object)Reader.LocalName == (object)_id5_type && (object)Reader.NamespaceURI == (object)_id2_Item))
                                        {
                                            if (ReadNull())
                                            {
                                                a_1_0.Add(null);
                                            }
                                            else
                                            {
                                                a_1_0.Add(Reader.ReadElementString());
                                            }
                                        }
                                        else
                                        {
                                            UnknownNode(null, @"http://microsoft.com/webReference/:type");
                                        }
                                    }
                                    else
                                    {
                                        UnknownNode(null, @"http://microsoft.com/webReference/:type");
                                    }
                                    Reader.MoveToContent();
                                    CheckReaderCount(ref whileIterations1, ref readerCount1);
                                }
                                ReadEndElement();
                            }
                        }
                    }
                    else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id6_style && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if (Reader.IsEmptyElement)
                        {
                            Reader.Skip();
                        }
                        else
                        {
                            o.@Style = Read2_ServiceDescriptionImportStyle(Reader.ReadElementString());
                        }
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id7_verbose && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        {
                            o.@Verbose = Microsoft.Xml.XmlConvert.ToBoolean(Reader.ReadElementString());
                        }
                        paramsRead[3] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://microsoft.com/webReference/:codeGenerationOptions, http://microsoft.com/webReference/:schemaImporterExtensions, http://microsoft.com/webReference/:style, http://microsoft.com/webReference/:verbose");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://microsoft.com/webReference/:codeGenerationOptions, http://microsoft.com/webReference/:schemaImporterExtensions, http://microsoft.com/webReference/:style, http://microsoft.com/webReference/:verbose");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations0, ref readerCount0);
            }
            ReadEndElement();
            return o;
        }

        protected override void InitCallbacks()
        {
        }

        internal object Read5_webReferenceOptions()
        {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
            {
                if (((object)Reader.LocalName == (object)_id1_webReferenceOptions && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o = Read4_WebReferenceOptions(true, true);
                }
                else
                {
                    throw CreateUnknownNodeException();
                }
            }
            else
            {
                UnknownNode(null, @"http://microsoft.com/webReference/:webReferenceOptions");
            }
            return (object)o;
        }

        private string _id2_Item;
        private string _id5_type;
        private string _id4_schemaImporterExtensions;
        private string _id3_codeGenerationOptions;
        private string _id6_style;
        private string _id7_verbose;
        private string _id1_webReferenceOptions;

        protected override void InitIDs()
        {
            _id2_Item = Reader.NameTable.Add(@"http://microsoft.com/webReference/");
            _id5_type = Reader.NameTable.Add(@"type");
            _id4_schemaImporterExtensions = Reader.NameTable.Add(@"schemaImporterExtensions");
            _id3_codeGenerationOptions = Reader.NameTable.Add(@"codeGenerationOptions");
            _id6_style = Reader.NameTable.Add(@"style");
            _id7_verbose = Reader.NameTable.Add(@"verbose");
            _id1_webReferenceOptions = Reader.NameTable.Add(@"webReferenceOptions");
        }
    }

    internal sealed class webReferenceOptionsSerializer : XmlSerializer
    {
        protected override XmlSerializationReader CreateReader()
        {
            return new WebReferenceOptionsSerializationReader();
        }
        protected override XmlSerializationWriter CreateWriter()
        {
            return new WebReferenceOptionsSerializationWriter();
        }
        public override System.Boolean CanDeserialize(Microsoft.Xml.XmlReader xmlReader)
        {
            return true;
        }

        protected override void Serialize(System.Object objectToSerialize, XmlSerializationWriter writer)
        {
            ((WebReferenceOptionsSerializationWriter)writer).Write5_webReferenceOptions(objectToSerialize);
        }
        protected override System.Object Deserialize(XmlSerializationReader reader)
        {
            return ((WebReferenceOptionsSerializationReader)reader).Read5_webReferenceOptions();
        }
    }
}

