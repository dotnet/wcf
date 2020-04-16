// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using Microsoft.Xml;
    using Microsoft.Xml.Serialization;

    [XmlRoot(MetadataStrings.MetadataExchangeStrings.Metadata, Namespace = MetadataStrings.MetadataExchangeStrings.Namespace)]
    public class MetadataSet : IXmlSerializable
    {
        private Collection<MetadataSection> _sections = new Collection<MetadataSection>();
        private Collection<XmlAttribute> _attributes = new Collection<XmlAttribute>();

        internal ServiceMetadataExtension.WriteFilter WriteFilter;

        public MetadataSet()
        {
        }

        public MetadataSet(IEnumerable<MetadataSection> sections)
            : this()
        {
            if (sections != null)
                foreach (MetadataSection section in sections)
                    _sections.Add(section);
        }

        [XmlElement(MetadataStrings.MetadataExchangeStrings.MetadataSection, Namespace = MetadataStrings.MetadataExchangeStrings.Namespace)]
        public Collection<MetadataSection> MetadataSections
        {
            get { return _sections; }
        }

        [XmlAnyAttribute]
        public Collection<XmlAttribute> Attributes
        {
            get { return _attributes; }
        }

        //Reader should write the <Metadata> element
        public void WriteTo(XmlWriter writer)
        {
            WriteMetadataSet(writer, true);
        }

        //Reader is on the <Metadata> element
        public static MetadataSet ReadFrom(XmlReader reader)
        {
            if (reader == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");

            MetadataSetSerializer xs = new MetadataSetSerializer();
            return (MetadataSet)xs.Deserialize(reader);
        }

        Microsoft.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        //Reader in on the <Metadata> element
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");

            MetadataSetSerializer xs = new MetadataSetSerializer();
            xs.ProcessOuterElement = false;

            MetadataSet metadataSet = (MetadataSet)xs.Deserialize(reader);

            _sections = metadataSet.MetadataSections;
            _attributes = metadataSet.Attributes;
        }

        //Reader has just written the <Metadata> element can still write attribs here
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            WriteMetadataSet(writer, false);
        }

        private void WriteMetadataSet(XmlWriter writer, bool processOuterElement)
        {
            if (writer == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");

            if (this.WriteFilter != null)
            {
                ServiceMetadataExtension.WriteFilter filter = this.WriteFilter.CloneWriteFilter();
                filter.Writer = writer;
                writer = filter;
            }
            MetadataSetSerializer xs = new MetadataSetSerializer();
            xs.ProcessOuterElement = processOuterElement;

            xs.Serialize(writer, this);
        }
    }

#pragma warning disable

    /* The Following code is a generated XmlSerializer.  It was created by:
     *      (*) Removing the IXmlSerializable from MetadataSet
     *      (*) Changing typeof(WsdlNS.ServiceDescription) and typeof(XsdNS.XmlSchema) to typeof(string) and typeof(int) on the [XmlElement] attribute on 
     *          MetadataSection.Metadata
     *      (*) running "sgen /a:System.ServiceModel.dll /t:System.ServiceModel.Description.MetadataSet /k" to generate the code 
     *      (*) Revert the above changes.
     * 
     * and then doing the following to fix it up:
     * 
     *      (*) Change the classes from public to internal
     *      (*) Add ProcessOuterElement to MetadataSetSerializer, XmlSerializationReaderMetadataSet, and XmlSerializationWriterMetadataSet
                       private bool processOuterElement = true;
     
                       public bool ProcessOuterElement
                       {
                           get { return processOuterElement; }
                           set { processOuterElement = value; }
                       }
     *      (*) Set XmlSerializationWriterMetadataSet.ProcessOuterElement with MetadataSetSerializer.ProcessOuterElement
     *          in MetadataSetSerializer.Serialize 
     *          ((XmlSerializationWriterMetadataSet)writer).ProcessOuterElement = this.processOuterElement;
     * 
     *      (*) Set XmlSerializationReaderMetadataSet.ProcessOuterElement with MetadataSetSerializer.ProcessOuterElement
     *          in MetadataSetSerializer.Deserialize 
     *          ((XmlSerializationReaderMetadataSet)reader).ProcessOuterElement = this.processOuterElement;
     *      (*) wrap anything in XmlSerializationWriterMetadataSet.Write*_Metadata or 
     *          XmlSerializationWriterMetadataSet.Write*_MetadataSet that outputs the outer
     *          element with "if(processOuterElement) { ... }"
     *      (*) Add "!processOuterElement ||" to checks for name and namespace of the outer element
     *          in XmlSerializationReaderMetadataSet.Read*_Metadata and XmlSerializationReaderMetadataSet.Read*_MetadataSet.
     *      (*) In XmlSerializationReaderMetadataSet.Read*_MetadataSection change the if clause writing the XmlSchema from
     *          
     *          o.@Metadata = Reader.ReadElementString();
     *          to
                o.@Metadata = Microsoft.Xml.Schema.XmlSchema.Read(this.Reader, null);
                if (this.Reader.NodeType == XmlNodeType.EndElement)
                    ReadEndElement();
     * 
     * 
     *      (*) In XmlSerializationWriterMetadataSet Write*_MetadataSection change
     *
     *          else if (o.@Metadata is global::System.Int32) {
     *              WriteElementString(@"schema", @"http://www.w3.org/2001/XMLSchema", ((global::System.Int32)o.@Metadata));
     *          }
     *          to
     * 
                else if (o.@Metadata is global::Microsoft.Xml.Schema.XmlSchema)
                {
                    ((global::Microsoft.Xml.Schema.XmlSchema)o.@Metadata).Write(this.Writer);
                }       
     * 
     *      (*) In XmlSerializationReaderMetadataSet.Read*_MetadataSection change 
     *          
     *          o.@Metadata = Reader.ReadElementString();
     *          to
     *          o.@Metadata = System.Web.Services.Description.ServiceDescription.Read(this.Reader);
     * 
     * 
     *      (*) In XmlSerializationWriterMetadataSet Write*_MetadataSection change
     *
     *          if (o.@Metadata is global::System.String) {
     *              WriteElementString(@"definitions", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.String)o.@Metadata));
     *          }
     *          to
     * 
                if (o.@Metadata is global::System.Web.Services.Description.ServiceDescription) {
                    ((global::System.Web.Services.Description.ServiceDescription)o.@Metadata).Write(this.Writer);
                }         
     * 
     *      (*) In XmlSerializationWriterMetadataSet Write*_MetadataSet add 
     *
                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(MetadataStrings.MetadataExchangeStrings.Prefix, MetadataStrings.MetadataExchangeStrings.Namespace);
                WriteNamespaceDeclarations(xmlSerializerNamespaces);
     *          
     *          immediately before 'if (needType) WriteXsiType(@"MetadataSet", @"http://schemas.xmlsoap.org/ws/2004/09/mex");'
     * 
     *      (*) In XmlSerializationWriterMetadataSet Write*_MetadataSection replace  
     *          WriteStartElement(n, ns, o, false, null);
     *          with
     * 
                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(string.Empty, string.Empty);

                WriteStartElement(n, ns, o, true, xmlSerializerNamespaces);
     *          
     *      (*) In XmlSerializationWriterMetadataSet Write*_XmlSchema replace              
     *          WriteStartElement(n, ns, o, false, o.@Namespaces);
     *          with 
     *          WriteStartElement(n, ns, o, true, o.@Namespaces);
     * 
     *       (*) Make sure you keep the #pragmas surrounding this block.
     * 
     *      (*) Make sure to replace all exception throw with standard throw using DiagnosticUtility.ExceptionUtility.ThrowHelperError;
     *          change:
     *
     *          throw CreateUnknownTypeException(*);
     *          to
     *          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnknownTypeException(*));
     *          
     *          throw CreateUnknownNodeException();
     *          to
     *          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnknownNodeException());
     * 
     *          throw CreateInvalidAnyTypeException(elem);
     *          to
     *          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateInvalidAnyTypeException(elem));
     * 
     *          throw CreateInvalidEnumValueException(*);
     *          to
     *          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateInvalidEnumValueException(*));
     * 
     *          throw CreateUnknownConstantException(*);
     *          to
     *          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnknownConstantException(*));
     *
     */

    internal class XmlSerializationWriterMetadataSet : Microsoft.Xml.Serialization.XmlSerializationWriter
    {
        private bool _processOuterElement = true;
        public bool ProcessOuterElement
        {
            get { return _processOuterElement; }
            set { _processOuterElement = value; }
        }

        public void Write68_Metadata(object o)
        {
            if (_processOuterElement)
            {
                WriteStartDocument();
                if (o == null)
                {
                    WriteNullTagLiteral(@"Metadata", @"http://schemas.xmlsoap.org/ws/2004/09/mex");
                    return;
                }
                TopLevelElement();
            }
            Write67_MetadataSet(@"Metadata", @"http://schemas.xmlsoap.org/ws/2004/09/mex", ((global::System.ServiceModel.Description.MetadataSet)o), true, false);
        }

        private void Write67_MetadataSet(string n, string ns, global::System.ServiceModel.Description.MetadataSet o, bool isNullable, bool needType)
        {
            if (_processOuterElement)
            {
                if ((object)o == null)
                {
                    if (isNullable) WriteNullTagLiteral(n, ns);
                    return;
                }
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.ServiceModel.Description.MetadataSet))
                {
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnknownTypeException(o));
                }
            }
            if (_processOuterElement)
            {
                WriteStartElement(n, ns, o, false, null);
            }

            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(MetadataStrings.MetadataExchangeStrings.Prefix, MetadataStrings.MetadataExchangeStrings.Namespace);
            WriteNamespaceDeclarations(xmlSerializerNamespaces);

            if (needType) WriteXsiType(@"MetadataSet", @"http://schemas.xmlsoap.org/ws/2004/09/mex");
            {
                global::System.Collections.ObjectModel.Collection<global::Microsoft.Xml.XmlAttribute> a = (global::System.Collections.ObjectModel.Collection<global::Microsoft.Xml.XmlAttribute>)o.@Attributes;
                if (a != null)
                {
                    for (int i = 0; i < ((System.Collections.ICollection)a).Count; i++)
                    {
                        global::Microsoft.Xml.XmlAttribute ai = (global::Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            {
                global::System.Collections.ObjectModel.Collection<global::System.ServiceModel.Description.MetadataSection> a = (global::System.Collections.ObjectModel.Collection<global::System.ServiceModel.Description.MetadataSection>)o.@MetadataSections;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write66_MetadataSection(@"MetadataSection", @"http://schemas.xmlsoap.org/ws/2004/09/mex", ((global::System.ServiceModel.Description.MetadataSection)a[ia]), false, false);
                    }
                }
            }
            if (_processOuterElement)
            {
                WriteEndElement(o);
            }
        }

        private void Write66_MetadataSection(string n, string ns, global::System.ServiceModel.Description.MetadataSection o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.ServiceModel.Description.MetadataSection))
                {
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnknownTypeException(o));
                }
            }


            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(string.Empty, string.Empty);

            WriteStartElement(n, ns, o, true, xmlSerializerNamespaces);
            if (needType) WriteXsiType(@"MetadataSection", @"http://schemas.xmlsoap.org/ws/2004/09/mex");
            {
                global::System.Collections.ObjectModel.Collection<global::Microsoft.Xml.XmlAttribute> a = (global::System.Collections.ObjectModel.Collection<global::Microsoft.Xml.XmlAttribute>)o.@Attributes;
                if (a != null)
                {
                    for (int i = 0; i < ((System.Collections.ICollection)a).Count; i++)
                    {
                        global::Microsoft.Xml.XmlAttribute ai = (global::Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"Dialect", @"", ((global::System.String)o.@Dialect));
            WriteAttribute(@"Identifier", @"", ((global::System.String)o.@Identifier));
            {
                if (o.@Metadata is global::System.Web.Services.Description.ServiceDescription)
                {
                    ((global::System.Web.Services.Description.ServiceDescription)o.@Metadata).Write(this.Writer);
                }
                else if (o.@Metadata is global::Microsoft.Xml.Schema.XmlSchema)
                {
                    ((global::Microsoft.Xml.Schema.XmlSchema)o.@Metadata).Write(this.Writer);
                }
                else if (o.@Metadata is global::System.ServiceModel.Description.MetadataSet)
                {
                    Write67_MetadataSet(@"Metadata", @"http://schemas.xmlsoap.org/ws/2004/09/mex", ((global::System.ServiceModel.Description.MetadataSet)o.@Metadata), false, false);
                }
                else if (o.@Metadata is global::System.ServiceModel.Description.MetadataLocation)
                {
                    Write65_MetadataLocation(@"Location", @"http://schemas.xmlsoap.org/ws/2004/09/mex", ((global::System.ServiceModel.Description.MetadataLocation)o.@Metadata), false, false);
                }
                else if (o.@Metadata is global::System.ServiceModel.Description.MetadataReference)
                {
                    WriteSerializable((Microsoft.Xml.Serialization.IXmlSerializable)((global::System.ServiceModel.Description.MetadataReference)o.@Metadata), @"MetadataReference", @"http://schemas.xmlsoap.org/ws/2004/09/mex", false, true);
                }
                else if (o.@Metadata is Microsoft.Xml.XmlElement)
                {
                    Microsoft.Xml.XmlElement elem = (Microsoft.Xml.XmlElement)o.@Metadata;
                    if ((elem) is Microsoft.Xml.XmlNode || elem == null)
                    {
                        WriteElementLiteral((Microsoft.Xml.XmlNode)elem, @"", null, false, true);
                    }
                    else
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateInvalidAnyTypeException(elem));
                    }
                }
                else
                {
                    if (o.@Metadata != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnknownTypeException(o.@Metadata));
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write65_MetadataLocation(string n, string ns, global::System.ServiceModel.Description.MetadataLocation o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.ServiceModel.Description.MetadataLocation))
                {
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnknownTypeException(o));
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"MetadataLocation", @"http://schemas.xmlsoap.org/ws/2004/09/mex");
            {
                WriteValue(((global::System.String)o.@Location));
            }
            WriteEndElement(o);
        }

        protected override void InitCallbacks()
        {
        }
    }

    internal class XmlSerializationReaderMetadataSet : Microsoft.Xml.Serialization.XmlSerializationReader
    {
        private bool _processOuterElement = true;
        public bool ProcessOuterElement
        {
            get { return _processOuterElement; }
            set { _processOuterElement = value; }
        }

        public object Read68_Metadata()
        {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
            {
                if (!_processOuterElement || (((object)Reader.LocalName == (object)_id1_Metadata && (object)Reader.NamespaceURI == (object)_id2_Item)))
                {
                    o = Read67_MetadataSet(true, true);
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnknownNodeException());
                }
            }
            else
            {
                UnknownNode(null, @"http://schemas.xmlsoap.org/ws/2004/09/mex:Metadata");
            }
            return (object)o;
        }

        private global::System.ServiceModel.Description.MetadataSet Read67_MetadataSet(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (!_processOuterElement || (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id3_MetadataSet && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item)))
                {
                }
                else
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType));
            }
            if (isNull) return null;
            global::System.ServiceModel.Description.MetadataSet o;
            o = new global::System.ServiceModel.Description.MetadataSet();
            global::System.Collections.ObjectModel.Collection<global::System.ServiceModel.Description.MetadataSection> a_0 = (global::System.Collections.ObjectModel.Collection<global::System.ServiceModel.Description.MetadataSection>)o.@MetadataSections;
            global::System.Collections.ObjectModel.Collection<global::Microsoft.Xml.XmlAttribute> a_1 = (global::System.Collections.ObjectModel.Collection<global::Microsoft.Xml.XmlAttribute>)o.@Attributes;
            bool[] paramsRead = new bool[2];
            while (Reader.MoveToNextAttribute())
            {
                if (!IsXmlnsAttribute(Reader.Name))
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1.Add(attr);
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
                    if (((object)Reader.LocalName == (object)_id4_MetadataSection && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if ((object)(a_0) == null) Reader.Skip(); else a_0.Add(Read66_MetadataSection(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://schemas.xmlsoap.org/ws/2004/09/mex:MetadataSection");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/ws/2004/09/mex:MetadataSection");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations0, ref readerCount0);
            }
            ReadEndElement();
            return o;
        }

        private global::System.ServiceModel.Description.MetadataSection Read66_MetadataSection(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id4_MetadataSection && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType));
            }
            if (isNull) return null;
            global::System.ServiceModel.Description.MetadataSection o;
            o = new global::System.ServiceModel.Description.MetadataSection();
            global::System.Collections.ObjectModel.Collection<global::Microsoft.Xml.XmlAttribute> a_0 = (global::System.Collections.ObjectModel.Collection<global::Microsoft.Xml.XmlAttribute>)o.@Attributes;
            bool[] paramsRead = new bool[4];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id5_Dialect && (object)Reader.NamespaceURI == (object)_id6_Item))
                {
                    o.@Dialect = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id7_Identifier && (object)Reader.NamespaceURI == (object)_id6_Item))
                {
                    o.@Identifier = Reader.Value;
                    paramsRead[2] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_0.Add(attr);
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
            int whileIterations1 = 0;
            int readerCount1 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id1_Metadata && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@Metadata = Read67_MetadataSet(false, true);
                        paramsRead[3] = true;
                    }
                    else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id8_schema && (object)Reader.NamespaceURI == (object)_id9_Item))
                    {
                        o.@Metadata = Microsoft.Xml.Schema.XmlSchema.Read(this.Reader, null);
                        if (this.Reader.NodeType == XmlNodeType.EndElement)
                            ReadEndElement();
                        paramsRead[3] = true;
                    }
                    else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id10_definitions && (object)Reader.NamespaceURI == (object)_id11_Item))
                    {
                        {
                            o.@Metadata = System.Web.Services.Description.ServiceDescription.Read(this.Reader);
                        }
                        paramsRead[3] = true;
                    }
                    else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id12_MetadataReference && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@Metadata = (global::System.ServiceModel.Description.MetadataReference)ReadSerializable((Microsoft.Xml.Serialization.IXmlSerializable)System.Activator.CreateInstance(typeof(global::System.ServiceModel.Description.MetadataReference), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null, new object[0], null));
                        paramsRead[3] = true;
                    }
                    else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id13_Location && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@Metadata = Read65_MetadataLocation(false, true);
                        paramsRead[3] = true;
                    }
                    else
                    {
                        o.@Metadata = (global::Microsoft.Xml.XmlElement)ReadXmlNode(false);
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/ws/2004/09/mex:Metadata, http://www.w3.org/2001/XMLSchema:schema, http://schemas.xmlsoap.org/wsdl/:definitions, http://schemas.xmlsoap.org/ws/2004/09/mex:MetadataReference, http://schemas.xmlsoap.org/ws/2004/09/mex:Location");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations1, ref readerCount1);
            }
            ReadEndElement();
            return o;
        }

        private global::System.ServiceModel.Description.MetadataLocation Read65_MetadataLocation(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id14_MetadataLocation && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType));
            }
            if (isNull) return null;
            global::System.ServiceModel.Description.MetadataLocation o;
            o = new global::System.ServiceModel.Description.MetadataLocation();
            bool[] paramsRead = new bool[1];
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
            int whileIterations2 = 0;
            int readerCount2 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                string tmp = null;
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Text ||
                Reader.NodeType == Microsoft.Xml.XmlNodeType.CDATA ||
                Reader.NodeType == Microsoft.Xml.XmlNodeType.Whitespace ||
                Reader.NodeType == Microsoft.Xml.XmlNodeType.SignificantWhitespace)
                {
                    tmp = ReadString(tmp, false);
                    o.@Location = tmp;
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations2, ref readerCount2);
            }
            ReadEndElement();
            return o;
        }

        protected override void InitCallbacks()
        {
        }

        private string _id60_documentation;
        private string _id22_targetNamespace;
        private string _id10_definitions;
        private string _id65_lang;
        private string _id31_attribute;
        private string _id47_ref;
        private string _id4_MetadataSection;
        private string _id54_refer;
        private string _id83_union;
        private string _id127_Item;
        private string _id53_XmlSchemaKeyref;
        private string _id27_import;
        private string _id75_all;
        private string _id128_XmlSchemaSimpleContent;
        private string _id139_XmlSchemaInclude;
        private string _id78_namespace;
        private string _id18_attributeFormDefault;
        private string _id100_XmlSchemaFractionDigitsFacet;
        private string _id32_attributeGroup;
        private string _id64_XmlSchemaDocumentation;
        private string _id93_maxLength;
        private string _id49_type;
        private string _id86_XmlSchemaSimpleTypeRestriction;
        private string _id96_length;
        private string _id104_XmlSchemaLengthFacet;
        private string _id17_XmlSchema;
        private string _id134_public;
        private string _id77_XmlSchemaAnyAttribute;
        private string _id24_id;
        private string _id71_simpleContent;
        private string _id51_key;
        private string _id67_XmlSchemaKey;
        private string _id80_XmlSchemaAttribute;
        private string _id126_Item;
        private string _id23_version;
        private string _id121_XmlSchemaGroupRef;
        private string _id90_maxInclusive;
        private string _id116_memberTypes;
        private string _id20_finalDefault;
        private string _id120_any;
        private string _id112_XmlSchemaMaxExclusiveFacet;
        private string _id15_EndpointReference;
        private string _id45_name;
        private string _id122_XmlSchemaSequence;
        private string _id73_sequence;
        private string _id82_XmlSchemaSimpleType;
        private string _id48_substitutionGroup;
        private string _id111_XmlSchemaMinInclusiveFacet;
        private string _id7_Identifier;
        private string _id113_XmlSchemaSimpleTypeList;
        private string _id41_default;
        private string _id125_extension;
        private string _id16_Item;
        private string _id1000_Item;
        private string _id124_XmlSchemaComplexContent;
        private string _id72_complexContent;
        private string _id11_Item;
        private string _id25_include;
        private string _id34_simpleType;
        private string _id91_minExclusive;
        private string _id94_pattern;
        private string _id2_Item;
        private string _id95_enumeration;
        private string _id114_itemType;
        private string _id115_XmlSchemaSimpleTypeUnion;
        private string _id59_XmlSchemaAnnotation;
        private string _id28_notation;
        private string _id84_list;
        private string _id39_abstract;
        private string _id103_XmlSchemaWhiteSpaceFacet;
        private string _id110_XmlSchemaMaxInclusiveFacet;
        private string _id55_selector;
        private string _id43_fixed;
        private string _id57_XmlSchemaXPath;
        private string _id118_XmlSchemaAll;
        private string _id56_field;
        private string _id119_XmlSchemaChoice;
        private string _id123_XmlSchemaAny;
        private string _id132_XmlSchemaGroup;
        private string _id35_element;
        private string _id129_Item;
        private string _id30_annotation;
        private string _id44_form;
        private string _id21_elementFormDefault;
        private string _id98_totalDigits;
        private string _id88_maxExclusive;
        private string _id42_final;
        private string _id46_nillable;
        private string _id9_Item;
        private string _id61_appinfo;
        private string _id38_maxOccurs;
        private string _id70_mixed;
        private string _id87_base;
        private string _id13_Location;
        private string _id12_MetadataReference;
        private string _id97_whiteSpace;
        private string _id29_group;
        private string _id92_minLength;
        private string _id99_fractionDigits;
        private string _id137_schemaLocation;
        private string _id26_redefine;
        private string _id101_value;
        private string _id63_source;
        private string _id89_minInclusive;
        private string _id133_XmlSchemaNotation;
        private string _id52_keyref;
        private string _id33_complexType;
        private string _id135_system;
        private string _id50_unique;
        private string _id74_choice;
        private string _id66_Item;
        private string _id105_XmlSchemaEnumerationFacet;
        private string _id107_XmlSchemaMaxLengthFacet;
        private string _id36_XmlSchemaElement;
        private string _id106_XmlSchemaPatternFacet;
        private string _id37_minOccurs;
        private string _id130_Item;
        private string _id68_XmlSchemaUnique;
        private string _id131_XmlSchemaAttributeGroup;
        private string _id40_block;
        private string _id81_use;
        private string _id85_restriction;
        private string _id1_Metadata;
        private string _id69_XmlSchemaComplexType;
        private string _id117_XmlSchemaAttributeGroupRef;
        private string _id138_XmlSchemaRedefine;
        private string _id6_Item;
        private string _id102_XmlSchemaTotalDigitsFacet;
        private string _id58_xpath;
        private string _id5_Dialect;
        private string _id14_MetadataLocation;
        private string _id3_MetadataSet;
        private string _id79_processContents;
        private string _id76_anyAttribute;
        private string _id19_blockDefault;
        private string _id136_XmlSchemaImport;
        private string _id109_XmlSchemaMinExclusiveFacet;
        private string _id108_XmlSchemaMinLengthFacet;
        private string _id8_schema;
        private string _id62_XmlSchemaAppInfo;

        protected override void InitIDs()
        {
            _id60_documentation = Reader.NameTable.Add(@"documentation");
            _id22_targetNamespace = Reader.NameTable.Add(@"targetNamespace");
            _id10_definitions = Reader.NameTable.Add(@"definitions");
            _id65_lang = Reader.NameTable.Add(@"lang");
            _id31_attribute = Reader.NameTable.Add(@"attribute");
            _id47_ref = Reader.NameTable.Add(@"ref");
            _id4_MetadataSection = Reader.NameTable.Add(@"MetadataSection");
            _id54_refer = Reader.NameTable.Add(@"refer");
            _id83_union = Reader.NameTable.Add(@"union");
            _id127_Item = Reader.NameTable.Add(@"XmlSchemaComplexContentRestriction");
            _id53_XmlSchemaKeyref = Reader.NameTable.Add(@"XmlSchemaKeyref");
            _id27_import = Reader.NameTable.Add(@"import");
            _id75_all = Reader.NameTable.Add(@"all");
            _id128_XmlSchemaSimpleContent = Reader.NameTable.Add(@"XmlSchemaSimpleContent");
            _id139_XmlSchemaInclude = Reader.NameTable.Add(@"XmlSchemaInclude");
            _id78_namespace = Reader.NameTable.Add(@"namespace");
            _id18_attributeFormDefault = Reader.NameTable.Add(@"attributeFormDefault");
            _id100_XmlSchemaFractionDigitsFacet = Reader.NameTable.Add(@"XmlSchemaFractionDigitsFacet");
            _id32_attributeGroup = Reader.NameTable.Add(@"attributeGroup");
            _id64_XmlSchemaDocumentation = Reader.NameTable.Add(@"XmlSchemaDocumentation");
            _id93_maxLength = Reader.NameTable.Add(@"maxLength");
            _id49_type = Reader.NameTable.Add(@"type");
            _id86_XmlSchemaSimpleTypeRestriction = Reader.NameTable.Add(@"XmlSchemaSimpleTypeRestriction");
            _id96_length = Reader.NameTable.Add(@"length");
            _id104_XmlSchemaLengthFacet = Reader.NameTable.Add(@"XmlSchemaLengthFacet");
            _id17_XmlSchema = Reader.NameTable.Add(@"XmlSchema");
            _id134_public = Reader.NameTable.Add(@"public");
            _id77_XmlSchemaAnyAttribute = Reader.NameTable.Add(@"XmlSchemaAnyAttribute");
            _id24_id = Reader.NameTable.Add(@"id");
            _id71_simpleContent = Reader.NameTable.Add(@"simpleContent");
            _id51_key = Reader.NameTable.Add(@"key");
            _id67_XmlSchemaKey = Reader.NameTable.Add(@"XmlSchemaKey");
            _id80_XmlSchemaAttribute = Reader.NameTable.Add(@"XmlSchemaAttribute");
            _id126_Item = Reader.NameTable.Add(@"XmlSchemaComplexContentExtension");
            _id23_version = Reader.NameTable.Add(@"version");
            _id121_XmlSchemaGroupRef = Reader.NameTable.Add(@"XmlSchemaGroupRef");
            _id90_maxInclusive = Reader.NameTable.Add(@"maxInclusive");
            _id116_memberTypes = Reader.NameTable.Add(@"memberTypes");
            _id20_finalDefault = Reader.NameTable.Add(@"finalDefault");
            _id120_any = Reader.NameTable.Add(@"any");
            _id112_XmlSchemaMaxExclusiveFacet = Reader.NameTable.Add(@"XmlSchemaMaxExclusiveFacet");
            _id15_EndpointReference = Reader.NameTable.Add(@"EndpointReference");
            _id45_name = Reader.NameTable.Add(@"name");
            _id122_XmlSchemaSequence = Reader.NameTable.Add(@"XmlSchemaSequence");
            _id73_sequence = Reader.NameTable.Add(@"sequence");
            _id82_XmlSchemaSimpleType = Reader.NameTable.Add(@"XmlSchemaSimpleType");
            _id48_substitutionGroup = Reader.NameTable.Add(@"substitutionGroup");
            _id111_XmlSchemaMinInclusiveFacet = Reader.NameTable.Add(@"XmlSchemaMinInclusiveFacet");
            _id7_Identifier = Reader.NameTable.Add(@"Identifier");
            _id113_XmlSchemaSimpleTypeList = Reader.NameTable.Add(@"XmlSchemaSimpleTypeList");
            _id41_default = Reader.NameTable.Add(@"default");
            _id125_extension = Reader.NameTable.Add(@"extension");
            _id16_Item = Reader.NameTable.Add(@"http://schemas.xmlsoap.org/ws/2004/08/addressing");
            _id1000_Item = Reader.NameTable.Add(@"http://www.w3.org/2005/08/addressing");
            _id124_XmlSchemaComplexContent = Reader.NameTable.Add(@"XmlSchemaComplexContent");
            _id72_complexContent = Reader.NameTable.Add(@"complexContent");
            _id11_Item = Reader.NameTable.Add(@"http://schemas.xmlsoap.org/wsdl/");
            _id25_include = Reader.NameTable.Add(@"include");
            _id34_simpleType = Reader.NameTable.Add(@"simpleType");
            _id91_minExclusive = Reader.NameTable.Add(@"minExclusive");
            _id94_pattern = Reader.NameTable.Add(@"pattern");
            _id2_Item = Reader.NameTable.Add(@"http://schemas.xmlsoap.org/ws/2004/09/mex");
            _id95_enumeration = Reader.NameTable.Add(@"enumeration");
            _id114_itemType = Reader.NameTable.Add(@"itemType");
            _id115_XmlSchemaSimpleTypeUnion = Reader.NameTable.Add(@"XmlSchemaSimpleTypeUnion");
            _id59_XmlSchemaAnnotation = Reader.NameTable.Add(@"XmlSchemaAnnotation");
            _id28_notation = Reader.NameTable.Add(@"notation");
            _id84_list = Reader.NameTable.Add(@"list");
            _id39_abstract = Reader.NameTable.Add(@"abstract");
            _id103_XmlSchemaWhiteSpaceFacet = Reader.NameTable.Add(@"XmlSchemaWhiteSpaceFacet");
            _id110_XmlSchemaMaxInclusiveFacet = Reader.NameTable.Add(@"XmlSchemaMaxInclusiveFacet");
            _id55_selector = Reader.NameTable.Add(@"selector");
            _id43_fixed = Reader.NameTable.Add(@"fixed");
            _id57_XmlSchemaXPath = Reader.NameTable.Add(@"XmlSchemaXPath");
            _id118_XmlSchemaAll = Reader.NameTable.Add(@"XmlSchemaAll");
            _id56_field = Reader.NameTable.Add(@"field");
            _id119_XmlSchemaChoice = Reader.NameTable.Add(@"XmlSchemaChoice");
            _id123_XmlSchemaAny = Reader.NameTable.Add(@"XmlSchemaAny");
            _id132_XmlSchemaGroup = Reader.NameTable.Add(@"XmlSchemaGroup");
            _id35_element = Reader.NameTable.Add(@"element");
            _id129_Item = Reader.NameTable.Add(@"XmlSchemaSimpleContentExtension");
            _id30_annotation = Reader.NameTable.Add(@"annotation");
            _id44_form = Reader.NameTable.Add(@"form");
            _id21_elementFormDefault = Reader.NameTable.Add(@"elementFormDefault");
            _id98_totalDigits = Reader.NameTable.Add(@"totalDigits");
            _id88_maxExclusive = Reader.NameTable.Add(@"maxExclusive");
            _id42_final = Reader.NameTable.Add(@"final");
            _id46_nillable = Reader.NameTable.Add(@"nillable");
            _id9_Item = Reader.NameTable.Add(@"http://www.w3.org/2001/XMLSchema");
            _id61_appinfo = Reader.NameTable.Add(@"appinfo");
            _id38_maxOccurs = Reader.NameTable.Add(@"maxOccurs");
            _id70_mixed = Reader.NameTable.Add(@"mixed");
            _id87_base = Reader.NameTable.Add(@"base");
            _id13_Location = Reader.NameTable.Add(@"Location");
            _id12_MetadataReference = Reader.NameTable.Add(@"MetadataReference");
            _id97_whiteSpace = Reader.NameTable.Add(@"whiteSpace");
            _id29_group = Reader.NameTable.Add(@"group");
            _id92_minLength = Reader.NameTable.Add(@"minLength");
            _id99_fractionDigits = Reader.NameTable.Add(@"fractionDigits");
            _id137_schemaLocation = Reader.NameTable.Add(@"schemaLocation");
            _id26_redefine = Reader.NameTable.Add(@"redefine");
            _id101_value = Reader.NameTable.Add(@"value");
            _id63_source = Reader.NameTable.Add(@"source");
            _id89_minInclusive = Reader.NameTable.Add(@"minInclusive");
            _id133_XmlSchemaNotation = Reader.NameTable.Add(@"XmlSchemaNotation");
            _id52_keyref = Reader.NameTable.Add(@"keyref");
            _id33_complexType = Reader.NameTable.Add(@"complexType");
            _id135_system = Reader.NameTable.Add(@"system");
            _id50_unique = Reader.NameTable.Add(@"unique");
            _id74_choice = Reader.NameTable.Add(@"choice");
            _id66_Item = Reader.NameTable.Add(@"http://www.w3.org/XML/1998/namespace");
            _id105_XmlSchemaEnumerationFacet = Reader.NameTable.Add(@"XmlSchemaEnumerationFacet");
            _id107_XmlSchemaMaxLengthFacet = Reader.NameTable.Add(@"XmlSchemaMaxLengthFacet");
            _id36_XmlSchemaElement = Reader.NameTable.Add(@"XmlSchemaElement");
            _id106_XmlSchemaPatternFacet = Reader.NameTable.Add(@"XmlSchemaPatternFacet");
            _id37_minOccurs = Reader.NameTable.Add(@"minOccurs");
            _id130_Item = Reader.NameTable.Add(@"XmlSchemaSimpleContentRestriction");
            _id68_XmlSchemaUnique = Reader.NameTable.Add(@"XmlSchemaUnique");
            _id131_XmlSchemaAttributeGroup = Reader.NameTable.Add(@"XmlSchemaAttributeGroup");
            _id40_block = Reader.NameTable.Add(@"block");
            _id81_use = Reader.NameTable.Add(@"use");
            _id85_restriction = Reader.NameTable.Add(@"restriction");
            _id1_Metadata = Reader.NameTable.Add(@"Metadata");
            _id69_XmlSchemaComplexType = Reader.NameTable.Add(@"XmlSchemaComplexType");
            _id117_XmlSchemaAttributeGroupRef = Reader.NameTable.Add(@"XmlSchemaAttributeGroupRef");
            _id138_XmlSchemaRedefine = Reader.NameTable.Add(@"XmlSchemaRedefine");
            _id6_Item = Reader.NameTable.Add(@"");
            _id102_XmlSchemaTotalDigitsFacet = Reader.NameTable.Add(@"XmlSchemaTotalDigitsFacet");
            _id58_xpath = Reader.NameTable.Add(@"xpath");
            _id5_Dialect = Reader.NameTable.Add(@"Dialect");
            _id14_MetadataLocation = Reader.NameTable.Add(@"MetadataLocation");
            _id3_MetadataSet = Reader.NameTable.Add(@"MetadataSet");
            _id79_processContents = Reader.NameTable.Add(@"processContents");
            _id76_anyAttribute = Reader.NameTable.Add(@"anyAttribute");
            _id19_blockDefault = Reader.NameTable.Add(@"blockDefault");
            _id136_XmlSchemaImport = Reader.NameTable.Add(@"XmlSchemaImport");
            _id109_XmlSchemaMinExclusiveFacet = Reader.NameTable.Add(@"XmlSchemaMinExclusiveFacet");
            _id108_XmlSchemaMinLengthFacet = Reader.NameTable.Add(@"XmlSchemaMinLengthFacet");
            _id8_schema = Reader.NameTable.Add(@"schema");
            _id62_XmlSchemaAppInfo = Reader.NameTable.Add(@"XmlSchemaAppInfo");
        }
    }

    internal abstract class XmlSerializer1 : Microsoft.Xml.Serialization.XmlSerializer
    {
        protected override Microsoft.Xml.Serialization.XmlSerializationReader CreateReader()
        {
            return new XmlSerializationReaderMetadataSet();
        }
        protected override Microsoft.Xml.Serialization.XmlSerializationWriter CreateWriter()
        {
            return new XmlSerializationWriterMetadataSet();
        }
    }

    internal sealed class MetadataSetSerializer : XmlSerializer1
    {
        private bool _processOuterElement = true;
        public bool ProcessOuterElement
        {
            get { return _processOuterElement; }
            set { _processOuterElement = value; }
        }

        public override System.Boolean CanDeserialize(Microsoft.Xml.XmlReader xmlReader)
        {
            return xmlReader.IsStartElement(@"Metadata", @"http://schemas.xmlsoap.org/ws/2004/09/mex");
        }

        protected override void Serialize(object objectToSerialize, Microsoft.Xml.Serialization.XmlSerializationWriter writer)
        {
            ((XmlSerializationWriterMetadataSet)writer).ProcessOuterElement = _processOuterElement;
            ((XmlSerializationWriterMetadataSet)writer).Write68_Metadata(objectToSerialize);
        }

        protected override object Deserialize(Microsoft.Xml.Serialization.XmlSerializationReader reader)
        {
            ((XmlSerializationReaderMetadataSet)reader).ProcessOuterElement = _processOuterElement;
            return ((XmlSerializationReaderMetadataSet)reader).Read68_Metadata();
        }
    }

    internal class XmlSerializerContract : global::Microsoft.Xml.Serialization.XmlSerializerImplementation
    {
        public override global::Microsoft.Xml.Serialization.XmlSerializationReader Reader { get { return new XmlSerializationReaderMetadataSet(); } }
        public override global::Microsoft.Xml.Serialization.XmlSerializationWriter Writer { get { return new XmlSerializationWriterMetadataSet(); } }
        private System.Collections.Hashtable _readMethods = null;
        public override System.Collections.Hashtable ReadMethods
        {
            get
            {
                if (_readMethods == null)
                {
                    System.Collections.Hashtable _tmp = new System.Collections.Hashtable();
                    _tmp[@"System.ServiceModel.Description.MetadataSet:http://schemas.xmlsoap.org/ws/2004/09/mex:Metadata:True:"] = @"Read68_Metadata";
                    if (_readMethods == null) _readMethods = _tmp;
                }
                return _readMethods;
            }
        }
        private System.Collections.Hashtable _writeMethods = null;
        public override System.Collections.Hashtable WriteMethods
        {
            get
            {
                if (_writeMethods == null)
                {
                    System.Collections.Hashtable _tmp = new System.Collections.Hashtable();
                    _tmp[@"System.ServiceModel.Description.MetadataSet:http://schemas.xmlsoap.org/ws/2004/09/mex:Metadata:True:"] = @"Write68_Metadata";
                    if (_writeMethods == null) _writeMethods = _tmp;
                }
                return _writeMethods;
            }
        }
        private System.Collections.Hashtable _typedSerializers = null;
        public override System.Collections.Hashtable TypedSerializers
        {
            get
            {
                if (_typedSerializers == null)
                {
                    System.Collections.Hashtable _tmp = new System.Collections.Hashtable();
                    _tmp.Add(@"System.ServiceModel.Description.MetadataSet:http://schemas.xmlsoap.org/ws/2004/09/mex:Metadata:True:", new MetadataSetSerializer());
                    if (_typedSerializers == null) _typedSerializers = _tmp;
                }
                return _typedSerializers;
            }
        }
        public override System.Boolean CanSerialize(System.Type type)
        {
            if (type == typeof(global::System.ServiceModel.Description.MetadataSet)) return true;
            return false;
        }
        public override Microsoft.Xml.Serialization.XmlSerializer GetSerializer(System.Type type)
        {
            if (type == typeof(global::System.ServiceModel.Description.MetadataSet)) return new MetadataSetSerializer();
            return null;
        }
    }

    // end generated code
#pragma warning restore
}
