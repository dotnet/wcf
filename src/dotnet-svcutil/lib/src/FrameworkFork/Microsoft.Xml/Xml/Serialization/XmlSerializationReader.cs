// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Serialization {

    using System.IO;
    using System;
    using System.Security;
    using System.Collections;
    using System.Reflection;
    using System.Text;
    using Microsoft.Xml;
    using Microsoft.Xml.Schema;
    using System.ComponentModel;
    using System.Globalization;
    using Microsoft.CodeDom.Compiler;
    using System.Diagnostics;
    using System.Threading;
    // using System.Configuration;
    //using Microsoft.Xml.Serialization.Configuration;

    /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader"]/*' />
    ///<internalonly/>
    public abstract class XmlSerializationReader : XmlSerializationGeneratedCode {
        XmlReader r;
        XmlCountingReader countingReader;
        XmlDocument d;
        Hashtable callbacks;
        Hashtable types;
        Hashtable typesReverse;
        XmlDeserializationEvents events;
        Hashtable targets;
        Hashtable referencedTargets;
        ArrayList targetsWithoutIds;
        ArrayList fixups;
        ArrayList collectionFixups;
        bool soap12;
        bool isReturnValue;
        bool decodeName = true;

        string schemaNsID;
        string schemaNs1999ID;
        string schemaNs2000ID;
        string schemaNonXsdTypesNsID;
        string instanceNsID;
        string instanceNs2000ID;
        string instanceNs1999ID;
        string soapNsID;
        string soap12NsID;
        string schemaID;
        string wsdlNsID;
        string wsdlArrayTypeID;
        string nullID;
        string nilID;
        string typeID;
        string arrayTypeID;
        string itemTypeID;
        string arraySizeID;
        string arrayID;
        string urTypeID;
        string stringID;
        string intID;
        string booleanID;
        string shortID;
        string longID;
        string floatID;
        string doubleID;
        string decimalID;
        string dateTimeID;
        string qnameID;
        string dateID;
        string timeID;
        string hexBinaryID;
        string base64BinaryID;
        string base64ID;
        string unsignedByteID;
        string byteID;
        string unsignedShortID;
        string unsignedIntID;
        string unsignedLongID;
        string oldDecimalID;
        string oldTimeInstantID;

        string anyURIID;
        string durationID;
        string ENTITYID;
        string ENTITIESID;
        string gDayID;
        string gMonthID;
        string gMonthDayID;
        string gYearID;
        string gYearMonthID;
        string IDID;
        string IDREFID;
        string IDREFSID;
        string integerID;
        string languageID;
        string NameID;
        string NCNameID;
        string NMTOKENID;
        string NMTOKENSID;
        string negativeIntegerID;
        string nonPositiveIntegerID;
        string nonNegativeIntegerID;
        string normalizedStringID;
        string NOTATIONID;
        string positiveIntegerID;
        string tokenID;

        string charID;
        string guidID;

        static bool checkDeserializeAdvances;

        static XmlSerializationReader()
        {

        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.InitIDs"]/*' />
        protected abstract void InitIDs();

        // this method must be called before any generated deserialization methods are called
        internal void Init(XmlReader r, XmlDeserializationEvents events, string encodingStyle, TempAssembly tempAssembly) {
            this.events = events;
            if (checkDeserializeAdvances)
            {
                this.countingReader = new XmlCountingReader(r);
                this.r = this.countingReader;
            }
            else 
                this.r = r;
            this.d = null;
            this.soap12 = (encodingStyle == Soap12.Encoding);
            Init(tempAssembly);

            schemaNsID = r.NameTable.Add(XmlSchema.Namespace);
            schemaNs2000ID = r.NameTable.Add("http://www.w3.org/2000/10/XMLSchema");
            schemaNs1999ID = r.NameTable.Add("http://www.w3.org/1999/XMLSchema");
            schemaNonXsdTypesNsID = r.NameTable.Add(UrtTypes.Namespace);
            instanceNsID = r.NameTable.Add(XmlSchema.InstanceNamespace);
            instanceNs2000ID = r.NameTable.Add("http://www.w3.org/2000/10/XMLSchema-instance");
            instanceNs1999ID = r.NameTable.Add("http://www.w3.org/1999/XMLSchema-instance");
            soapNsID = r.NameTable.Add(Soap.Encoding);
            soap12NsID = r.NameTable.Add(Soap12.Encoding);
            schemaID = r.NameTable.Add("schema");
            wsdlNsID = r.NameTable.Add(Wsdl.Namespace);
            wsdlArrayTypeID = r.NameTable.Add(Wsdl.ArrayType);
            nullID = r.NameTable.Add("null");
            nilID = r.NameTable.Add("nil");
            typeID = r.NameTable.Add("type");
            arrayTypeID = r.NameTable.Add("arrayType");
            itemTypeID = r.NameTable.Add("itemType");
            arraySizeID = r.NameTable.Add("arraySize");
            arrayID = r.NameTable.Add("Array");
            urTypeID = r.NameTable.Add(Soap.UrType);
            InitIDs();
        }

        /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.DecodeName"]/*' />
        protected bool DecodeName {
            get {
                return decodeName;
            }
            set {
                decodeName = value;
            }
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.Reader"]/*' />
        protected XmlReader Reader {
            get {
                return r;
            }
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReaderCount"]/*' />
        protected int ReaderCount {
            get {
                return checkDeserializeAdvances ? countingReader.AdvanceCount : 0;
            }
        }
        
        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.Document"]/*' />
        protected XmlDocument Document {
            get {
                if (d == null) {
                    d = new XmlDocument(r.NameTable);
                    d.SetBaseURI(r.BaseURI);
                }
                return d;
            }
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ResolveDynamicAssembly"]/*' />
        ///<internalonly/>
        protected static Assembly ResolveDynamicAssembly(string assemblyFullName){
            return DynamicAssemblies.Get(assemblyFullName);
        }
        
        void InitPrimitiveIDs() {
            if (tokenID != null) return;
            object ns = r.NameTable.Add(XmlSchema.Namespace);
            object ns2 = r.NameTable.Add(UrtTypes.Namespace);
            
            stringID = r.NameTable.Add("string");
            intID = r.NameTable.Add("int");
            booleanID = r.NameTable.Add("boolean");
            shortID = r.NameTable.Add("short");
            longID = r.NameTable.Add("long");
            floatID = r.NameTable.Add("float");
            doubleID = r.NameTable.Add("double");
            decimalID = r.NameTable.Add("decimal");
            dateTimeID = r.NameTable.Add("dateTime");
            qnameID = r.NameTable.Add("QName");
            dateID = r.NameTable.Add("date");
            timeID = r.NameTable.Add("time");
            hexBinaryID = r.NameTable.Add("hexBinary");
            base64BinaryID = r.NameTable.Add("base64Binary");
            unsignedByteID = r.NameTable.Add("unsignedByte");
            byteID = r.NameTable.Add("byte");
            unsignedShortID = r.NameTable.Add("unsignedShort");
            unsignedIntID = r.NameTable.Add("unsignedInt");
            unsignedLongID = r.NameTable.Add("unsignedLong");
            oldDecimalID = r.NameTable.Add("decimal");
            oldTimeInstantID = r.NameTable.Add("timeInstant");
            charID = r.NameTable.Add("char");
            guidID = r.NameTable.Add("guid");
            base64ID = r.NameTable.Add("base64");

            anyURIID = r.NameTable.Add("anyURI");
            durationID = r.NameTable.Add("duration");
            ENTITYID = r.NameTable.Add("ENTITY");
            ENTITIESID = r.NameTable.Add("ENTITIES");
            gDayID = r.NameTable.Add("gDay");
            gMonthID = r.NameTable.Add("gMonth");
            gMonthDayID = r.NameTable.Add("gMonthDay");
            gYearID = r.NameTable.Add("gYear");
            gYearMonthID = r.NameTable.Add("gYearMonth");
            IDID = r.NameTable.Add("ID");
            IDREFID = r.NameTable.Add("IDREF");
            IDREFSID = r.NameTable.Add("IDREFS");
            integerID = r.NameTable.Add("integer");
            languageID = r.NameTable.Add("language");
            NameID = r.NameTable.Add("Name");
            NCNameID = r.NameTable.Add("NCName");
            NMTOKENID = r.NameTable.Add("NMTOKEN");
            NMTOKENSID = r.NameTable.Add("NMTOKENS");
            negativeIntegerID = r.NameTable.Add("negativeInteger");
            nonNegativeIntegerID = r.NameTable.Add("nonNegativeInteger");
            nonPositiveIntegerID = r.NameTable.Add("nonPositiveInteger");
            normalizedStringID = r.NameTable.Add("normalizedString");
            NOTATIONID = r.NameTable.Add("NOTATION");
            positiveIntegerID = r.NameTable.Add("positiveInteger");
            tokenID = r.NameTable.Add("token");
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.GetXsiType"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected XmlQualifiedName GetXsiType() {
            string type = r.GetAttribute(typeID, instanceNsID);
            if (type == null) {
                type = r.GetAttribute(typeID, instanceNs2000ID);
                if (type == null) {
                    type = r.GetAttribute(typeID, instanceNs1999ID);
                    if (type == null)
                        return null;
                }
            }
            return ToXmlQualifiedName(type, false);
        }

        // throwOnUnknown flag controls whether this method throws an exception or just returns 
        // null if typeName.Namespace is unknown. the method still throws if typeName.Namespace
        // is recognized but typeName.Name isn't.
        Type GetPrimitiveType(XmlQualifiedName typeName, bool throwOnUnknown) {
            InitPrimitiveIDs();

            if ((object)typeName.Namespace == (object)schemaNsID || (object)typeName.Namespace == (object)soapNsID || (object)typeName.Namespace == (object)soap12NsID) {
                if ((object) typeName.Name == (object) stringID ||
                    (object) typeName.Name == (object) anyURIID ||
                    (object) typeName.Name == (object) durationID ||
                    (object) typeName.Name == (object) ENTITYID ||
                    (object) typeName.Name == (object) ENTITIESID ||
                    (object) typeName.Name == (object) gDayID ||
                    (object) typeName.Name == (object) gMonthID ||
                    (object) typeName.Name == (object) gMonthDayID ||
                    (object) typeName.Name == (object) gYearID ||
                    (object) typeName.Name == (object) gYearMonthID ||
                    (object) typeName.Name == (object) IDID ||
                    (object) typeName.Name == (object) IDREFID ||
                    (object) typeName.Name == (object) IDREFSID ||
                    (object) typeName.Name == (object) integerID ||
                    (object) typeName.Name == (object) languageID ||
                    (object) typeName.Name == (object) NameID ||
                    (object) typeName.Name == (object) NCNameID ||
                    (object) typeName.Name == (object) NMTOKENID ||
                    (object) typeName.Name == (object) NMTOKENSID ||
                    (object) typeName.Name == (object) negativeIntegerID ||
                    (object) typeName.Name == (object) nonPositiveIntegerID ||
                    (object) typeName.Name == (object) nonNegativeIntegerID ||
                    (object) typeName.Name == (object) normalizedStringID ||
                    (object) typeName.Name == (object) NOTATIONID ||
                    (object) typeName.Name == (object) positiveIntegerID ||
                    (object) typeName.Name == (object) tokenID)
                    return typeof(string);
                else if ((object) typeName.Name == (object) intID)
                    return typeof(int);
                else if ((object) typeName.Name == (object) booleanID)
                    return typeof(bool);
                else if ((object) typeName.Name == (object) shortID)
                    return typeof(short);
                else if ((object) typeName.Name == (object) longID)
                    return typeof(long);
                else if ((object) typeName.Name == (object) floatID)
                    return typeof(float);
                else if ((object) typeName.Name == (object) doubleID)
                    return typeof(double);
                else if ((object) typeName.Name == (object) decimalID)
                    return typeof(decimal);
                else if ((object) typeName.Name == (object) dateTimeID)
                    return typeof(DateTime);
                else if ((object) typeName.Name == (object) qnameID)
                    return typeof(XmlQualifiedName);
                else if ((object) typeName.Name == (object) dateID)
                    return typeof(DateTime);
                else if ((object) typeName.Name == (object) timeID)
                    return typeof(DateTime);
                else if ((object) typeName.Name == (object) hexBinaryID)
                    return typeof(byte[]);
                else if ((object)typeName.Name == (object)base64BinaryID)
                    return typeof(byte[]);
                else if ((object)typeName.Name == (object)unsignedByteID)
                    return typeof(byte);
                else if ((object) typeName.Name == (object) byteID)
                    return typeof(SByte);
                else if ((object) typeName.Name == (object) unsignedShortID)
                    return typeof(UInt16);
                else if ((object) typeName.Name == (object) unsignedIntID)
                    return typeof(UInt32);
                else if ((object) typeName.Name == (object) unsignedLongID)
                    return typeof(UInt64);
                else
                    throw CreateUnknownTypeException(typeName);
            } 
            else if ((object) typeName.Namespace == (object) schemaNs2000ID || (object) typeName.Namespace == (object) schemaNs1999ID) {
                if ((object) typeName.Name == (object) stringID ||
                    (object) typeName.Name == (object) anyURIID ||
                    (object) typeName.Name == (object) durationID ||
                    (object) typeName.Name == (object) ENTITYID ||
                    (object) typeName.Name == (object) ENTITIESID ||
                    (object) typeName.Name == (object) gDayID ||
                    (object) typeName.Name == (object) gMonthID ||
                    (object) typeName.Name == (object) gMonthDayID ||
                    (object) typeName.Name == (object) gYearID ||
                    (object) typeName.Name == (object) gYearMonthID ||
                    (object) typeName.Name == (object) IDID ||
                    (object) typeName.Name == (object) IDREFID ||
                    (object) typeName.Name == (object) IDREFSID ||
                    (object) typeName.Name == (object) integerID ||
                    (object) typeName.Name == (object) languageID ||
                    (object) typeName.Name == (object) NameID ||
                    (object) typeName.Name == (object) NCNameID ||
                    (object) typeName.Name == (object) NMTOKENID ||
                    (object) typeName.Name == (object) NMTOKENSID ||
                    (object) typeName.Name == (object) negativeIntegerID ||
                    (object) typeName.Name == (object) nonPositiveIntegerID ||
                    (object) typeName.Name == (object) nonNegativeIntegerID ||
                    (object) typeName.Name == (object) normalizedStringID ||
                    (object) typeName.Name == (object) NOTATIONID ||
                    (object) typeName.Name == (object) positiveIntegerID ||
                    (object) typeName.Name == (object) tokenID)
                    return typeof(string);
                else if ((object) typeName.Name == (object) intID)
                    return typeof(int);
                else if ((object) typeName.Name == (object) booleanID)
                    return typeof(bool);
                else if ((object) typeName.Name == (object) shortID)
                    return typeof(short);
                else if ((object) typeName.Name == (object) longID)
                    return typeof(long);
                else if ((object) typeName.Name == (object) floatID)
                    return typeof(float);
                else if ((object) typeName.Name == (object) doubleID)
                    return typeof(double);
                else if ((object) typeName.Name == (object) oldDecimalID)
                    return typeof(decimal);
                else if ((object) typeName.Name == (object) oldTimeInstantID)
                    return typeof(DateTime);
                else if ((object) typeName.Name == (object) qnameID)
                    return typeof(XmlQualifiedName);
                else if ((object) typeName.Name == (object) dateID)
                    return typeof(DateTime);
                else if ((object) typeName.Name == (object) timeID)
                    return typeof(DateTime);
                else if ((object) typeName.Name == (object) hexBinaryID)
                    return typeof(byte[]);
                else if ((object) typeName.Name == (object) byteID)
                    return typeof(SByte);
                else if ((object) typeName.Name == (object) unsignedShortID)
                    return typeof(UInt16);
                else if ((object) typeName.Name == (object) unsignedIntID)
                    return typeof(UInt32);
                else if ((object) typeName.Name == (object) unsignedLongID)
                    return typeof(UInt64);
                else
                    throw CreateUnknownTypeException(typeName);
            }
            else if ((object) typeName.Namespace == (object) schemaNonXsdTypesNsID) {
                if ((object) typeName.Name == (object) charID)
                    return typeof(char);
                else if ((object) typeName.Name == (object) guidID)
                    return typeof(Guid);
                else
                    throw CreateUnknownTypeException(typeName);
            }
            else if (throwOnUnknown)
                throw CreateUnknownTypeException(typeName);
            else
                return null;
        }

        bool IsPrimitiveNamespace(string ns) {
            return (object) ns == (object) schemaNsID ||
                   (object) ns == (object) schemaNonXsdTypesNsID ||
                   (object) ns == (object) soapNsID ||
                   (object) ns == (object) soap12NsID ||
                   (object) ns == (object) schemaNs2000ID ||
                   (object) ns == (object) schemaNs1999ID;
        }

        private string ReadStringValue(){
            if (r.IsEmptyElement){
                r.Skip();
                return string.Empty;
            }
            r.ReadStartElement();
            string retVal = r.ReadString();
            ReadEndElement();
            return retVal;
        }

        private XmlQualifiedName ReadXmlQualifiedName(){
            string s;
            bool isEmpty = false;
            if (r.IsEmptyElement) {
                s = string.Empty;
                isEmpty = true;
            }
            else{
                r.ReadStartElement();
                s = r.ReadString();
            }
            XmlQualifiedName retVal = ToXmlQualifiedName(s);
            if (isEmpty)
                r.Skip();
            else
                ReadEndElement();
            return retVal;
        }

        private byte[] ReadByteArray(bool isBase64) {
            ArrayList list = new ArrayList();
            const   int MAX_ALLOC_SIZE = 64*1024;
            int     currentSize = 1024;
            byte[]  buffer;
            int     bytes = -1;
            int     offset = 0;
            int     total = 0;
            buffer = new byte[currentSize];
            list.Add(buffer);
            while (bytes != 0) {
                if (offset == buffer.Length) {
                    currentSize = Math.Min(currentSize*2, MAX_ALLOC_SIZE);
                    buffer = new byte[currentSize];
                    offset = 0;              
                    list.Add(buffer);
                }
                if (isBase64) {
                    bytes = r.ReadElementContentAsBase64(buffer, offset, buffer.Length-offset);
                }
                else {
                    bytes = r.ReadElementContentAsBinHex(buffer, offset, buffer.Length-offset);
                }
                offset += bytes;
                total += bytes;
            }

            byte[] result = new byte[total];
            offset = 0;
            foreach (byte[] block in list) {
                currentSize = Math.Min(block.Length, total);
                if (currentSize > 0) {
                    Buffer.BlockCopy(block, 0, result, offset, currentSize);
                    offset += currentSize;
                    total -= currentSize;
                }
            }
            list.Clear();
            return result;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadTypedPrimitive"]/*' />
        protected object ReadTypedPrimitive(XmlQualifiedName type) {
            return ReadTypedPrimitive(type, false);
        }

        private object ReadTypedPrimitive(XmlQualifiedName type, bool elementCanBeType) {
            InitPrimitiveIDs();
            object value = null;
            if (!IsPrimitiveNamespace(type.Namespace) || (object)type.Name == (object)urTypeID) 
                return ReadXmlNodes(elementCanBeType);

            if ((object)type.Namespace == (object)schemaNsID || (object)type.Namespace == (object)soapNsID || (object)type.Namespace == (object)soap12NsID) {
                if ((object) type.Name == (object) stringID ||
                    (object) type.Name == (object) normalizedStringID)
                    value = ReadStringValue();
                else if ((object) type.Name == (object) anyURIID ||
                    (object) type.Name == (object) durationID ||
                    (object) type.Name == (object) ENTITYID ||
                    (object) type.Name == (object) ENTITIESID ||
                    (object) type.Name == (object) gDayID ||
                    (object) type.Name == (object) gMonthID ||
                    (object) type.Name == (object) gMonthDayID ||
                    (object) type.Name == (object) gYearID ||
                    (object) type.Name == (object) gYearMonthID ||
                    (object) type.Name == (object) IDID ||
                    (object) type.Name == (object) IDREFID ||
                    (object) type.Name == (object) IDREFSID ||
                    (object) type.Name == (object) integerID ||
                    (object) type.Name == (object) languageID ||
                    (object) type.Name == (object) NameID ||
                    (object) type.Name == (object) NCNameID ||
                    (object) type.Name == (object) NMTOKENID ||
                    (object) type.Name == (object) NMTOKENSID ||
                    (object) type.Name == (object) negativeIntegerID ||
                    (object) type.Name == (object) nonPositiveIntegerID ||
                    (object) type.Name == (object) nonNegativeIntegerID ||
                    (object) type.Name == (object) NOTATIONID ||
                    (object) type.Name == (object) positiveIntegerID ||
                    (object) type.Name == (object) tokenID)
                    value = CollapseWhitespace(ReadStringValue());
                else if ((object) type.Name == (object) intID)
                    value = XmlConvert.ToInt32(ReadStringValue());
                else if ((object) type.Name == (object) booleanID)
                    value = XmlConvert.ToBoolean(ReadStringValue());
                else if ((object) type.Name == (object) shortID)
                    value = XmlConvert.ToInt16(ReadStringValue());
                else if ((object) type.Name == (object) longID)
                    value = XmlConvert.ToInt64(ReadStringValue());
                else if ((object)type.Name == (object)floatID)
                    value = XmlConvert.ToSingle(ReadStringValue());
                else if ((object)type.Name == (object)doubleID)
                    value = XmlConvert.ToDouble(ReadStringValue());
                else if ((object)type.Name == (object)decimalID)
                    value = XmlConvert.ToDecimal(ReadStringValue());
                else if ((object)type.Name == (object)dateTimeID)
                    value = ToDateTime(ReadStringValue());
                else if ((object) type.Name == (object) qnameID)
                    value = ReadXmlQualifiedName();
                else if ((object) type.Name == (object) dateID)
                    value = ToDate(ReadStringValue());
                else if ((object) type.Name == (object) timeID)
                    value = ToTime(ReadStringValue());
                else if ((object) type.Name == (object) unsignedByteID)
                    value = XmlConvert.ToByte(ReadStringValue());
                else if ((object) type.Name == (object) byteID)
                    value = XmlConvert.ToSByte(ReadStringValue());
                else if ((object) type.Name == (object) unsignedShortID)
                    value = XmlConvert.ToUInt16(ReadStringValue());
                else if ((object) type.Name == (object) unsignedIntID)
                    value = XmlConvert.ToUInt32(ReadStringValue());
                else if ((object) type.Name == (object) unsignedLongID)
                    value = XmlConvert.ToUInt64(ReadStringValue());
                else if ((object) type.Name == (object) hexBinaryID)
                    value = ToByteArrayHex(false);
                else if ((object) type.Name == (object) base64BinaryID)
                    value = ToByteArrayBase64(false);
                else if ((object) type.Name == (object)base64ID && ((object)type.Namespace == (object)soapNsID || (object)type.Namespace == (object)soap12NsID))
                    value = ToByteArrayBase64(false);
                else
                    value = ReadXmlNodes(elementCanBeType);
            }
            else if ((object) type.Namespace == (object) schemaNs2000ID || (object) type.Namespace == (object) schemaNs1999ID) {
                if ((object) type.Name == (object) stringID ||
                    (object) type.Name == (object) normalizedStringID)
                    value = ReadStringValue();
                else if ((object) type.Name == (object) anyURIID ||
                    (object) type.Name == (object) anyURIID ||
                    (object) type.Name == (object) durationID ||
                    (object) type.Name == (object) ENTITYID ||
                    (object) type.Name == (object) ENTITIESID ||
                    (object) type.Name == (object) gDayID ||
                    (object) type.Name == (object) gMonthID ||
                    (object) type.Name == (object) gMonthDayID ||
                    (object) type.Name == (object) gYearID ||
                    (object) type.Name == (object) gYearMonthID ||
                    (object) type.Name == (object) IDID ||
                    (object) type.Name == (object) IDREFID ||
                    (object) type.Name == (object) IDREFSID ||
                    (object) type.Name == (object) integerID ||
                    (object) type.Name == (object) languageID ||
                    (object) type.Name == (object) NameID ||
                    (object) type.Name == (object) NCNameID ||
                    (object) type.Name == (object) NMTOKENID ||
                    (object) type.Name == (object) NMTOKENSID ||
                    (object) type.Name == (object) negativeIntegerID ||
                    (object) type.Name == (object) nonPositiveIntegerID ||
                    (object) type.Name == (object) nonNegativeIntegerID ||
                    (object) type.Name == (object) NOTATIONID ||
                    (object) type.Name == (object) positiveIntegerID ||
                    (object) type.Name == (object) tokenID)
                    value = CollapseWhitespace(ReadStringValue());
                else if ((object) type.Name == (object) intID)
                    value = XmlConvert.ToInt32(ReadStringValue());
                else if ((object) type.Name == (object) booleanID)
                    value = XmlConvert.ToBoolean(ReadStringValue());
                else if ((object) type.Name == (object) shortID)
                    value = XmlConvert.ToInt16(ReadStringValue());
                else if ((object) type.Name == (object) longID)
                    value = XmlConvert.ToInt64(ReadStringValue());
                else if ((object)type.Name == (object)floatID)
                    value = XmlConvert.ToSingle(ReadStringValue());
                else if ((object)type.Name == (object)doubleID)
                    value = XmlConvert.ToDouble(ReadStringValue());
                else if ((object)type.Name == (object) oldDecimalID)
                    value = XmlConvert.ToDecimal(ReadStringValue());
                else if ((object)type.Name == (object) oldTimeInstantID)
                    value = ToDateTime(ReadStringValue());
                else if ((object) type.Name == (object) qnameID)
                    value = ReadXmlQualifiedName();
                else if ((object) type.Name == (object) dateID)
                    value = ToDate(ReadStringValue());
                else if ((object) type.Name == (object) timeID)
                    value = ToTime(ReadStringValue());
                else if ((object) type.Name == (object) unsignedByteID)
                    value = XmlConvert.ToByte(ReadStringValue());
                else if ((object) type.Name == (object) byteID)
                    value = XmlConvert.ToSByte(ReadStringValue());
                else if ((object) type.Name == (object) unsignedShortID)
                    value = XmlConvert.ToUInt16(ReadStringValue());
                else if ((object) type.Name == (object) unsignedIntID)
                    value = XmlConvert.ToUInt32(ReadStringValue());
                else if ((object) type.Name == (object) unsignedLongID)
                    value = XmlConvert.ToUInt64(ReadStringValue());
                else
                    value = ReadXmlNodes(elementCanBeType);
            }
            else if ((object) type.Namespace == (object) schemaNonXsdTypesNsID) {
                if ((object) type.Name == (object) charID)
                    value = ToChar(ReadStringValue());
                else if ((object) type.Name == (object) guidID)
                    value = new Guid(CollapseWhitespace(ReadStringValue()));
                else
                    value = ReadXmlNodes(elementCanBeType);
            }
            else
                value = ReadXmlNodes(elementCanBeType);
            return value;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadTypedNull"]/*' />
        protected object ReadTypedNull(XmlQualifiedName type) {
            InitPrimitiveIDs();
            object value = null;
            if (!IsPrimitiveNamespace(type.Namespace) || (object)type.Name == (object)urTypeID) {
                return null;
            }

            if ((object)type.Namespace == (object)schemaNsID || (object)type.Namespace == (object)soapNsID || (object)type.Namespace == (object)soap12NsID) {
                if ((object) type.Name == (object) stringID ||
                    (object) type.Name == (object) anyURIID ||
                    (object) type.Name == (object) durationID ||
                    (object) type.Name == (object) ENTITYID ||
                    (object) type.Name == (object) ENTITIESID ||
                    (object) type.Name == (object) gDayID ||
                    (object) type.Name == (object) gMonthID ||
                    (object) type.Name == (object) gMonthDayID ||
                    (object) type.Name == (object) gYearID ||
                    (object) type.Name == (object) gYearMonthID ||
                    (object) type.Name == (object) IDID ||
                    (object) type.Name == (object) IDREFID ||
                    (object) type.Name == (object) IDREFSID ||
                    (object) type.Name == (object) integerID ||
                    (object) type.Name == (object) languageID ||
                    (object) type.Name == (object) NameID ||
                    (object) type.Name == (object) NCNameID ||
                    (object) type.Name == (object) NMTOKENID ||
                    (object) type.Name == (object) NMTOKENSID ||
                    (object) type.Name == (object) negativeIntegerID ||
                    (object) type.Name == (object) nonPositiveIntegerID ||
                    (object) type.Name == (object) nonNegativeIntegerID ||
                    (object) type.Name == (object) normalizedStringID ||
                    (object) type.Name == (object) NOTATIONID ||
                    (object) type.Name == (object) positiveIntegerID ||
                    (object) type.Name == (object) tokenID)
                    value = null;
                else if ((object) type.Name == (object) intID) {
                    value = default(Nullable<int>);
                }
                else if ((object) type.Name == (object) booleanID)
                    value = default(Nullable<bool>);
                else if ((object) type.Name == (object) shortID)
                    value = default(Nullable<Int16>);
                        else if ((object) type.Name == (object) longID)
                    value = default(Nullable<long>);
                else if ((object)type.Name == (object)floatID)
                    value = default(Nullable<float>);
                else if ((object)type.Name == (object)doubleID)
                    value = default(Nullable<double>);
                else if ((object)type.Name == (object)decimalID)
                    value = default(Nullable<decimal>);
                else if ((object)type.Name == (object)dateTimeID)
                    value = default(Nullable<DateTime>);
                        else if ((object) type.Name == (object) qnameID)
                    value = null;
                else if ((object) type.Name == (object) dateID)
                    value = default(Nullable<DateTime>);
                        else if ((object) type.Name == (object) timeID)
                    value = default(Nullable<DateTime>);
                        else if ((object) type.Name == (object) unsignedByteID)
                    value = default(Nullable<byte>);
                else if ((object) type.Name == (object) byteID)
                    value = default(Nullable<SByte>);
                        else if ((object) type.Name == (object) unsignedShortID)
                    value = default(Nullable<UInt16>);
                        else if ((object) type.Name == (object) unsignedIntID)
                    value = default(Nullable<UInt32>);
                        else if ((object) type.Name == (object) unsignedLongID)
                    value = default(Nullable<UInt64>);
                        else if ((object) type.Name == (object) hexBinaryID)
                    value = null;
                else if ((object) type.Name == (object) base64BinaryID)
                    value = null;
                else if ((object) type.Name == (object)base64ID && ((object)type.Namespace == (object)soapNsID || (object)type.Namespace == (object)soap12NsID))
                    value = null;
                else
                    value = null;
            }
            else if ((object) type.Namespace == (object) schemaNonXsdTypesNsID) {
                if ((object) type.Name == (object) charID)
                    value = default(Nullable<char>);
                else if ((object) type.Name == (object) guidID)
                    value = default(Nullable<Guid>);
                        else
                    value = null;
            }
            else
                value = null;
            return value;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.IsXmlnsAttribute"]/*' />
        protected bool IsXmlnsAttribute(string name) {
            if (!name.StartsWith("xmlns", StringComparison.Ordinal)) return false;
            if (name.Length == 5) return true;
            return name[5] == ':';
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.IsArrayTypeAttribute"]/*' />
        protected void ParseWsdlArrayType(XmlAttribute attr) {
            if ((object)attr.LocalName == (object)wsdlArrayTypeID && (object)attr.NamespaceURI == (object)wsdlNsID ) {

                int colon = attr.Value.LastIndexOf(':');
                if (colon < 0) {
                    attr.Value = r.LookupNamespace("") + ":" + attr.Value;
                }
                else {
                    attr.Value = r.LookupNamespace(attr.Value.Substring(0, colon)) + ":" + attr.Value.Substring(colon + 1);
                }
            }
            return;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.IsReturnValue"]/*' />
        protected bool IsReturnValue {
            // value only valid for soap 1.1
            get { return isReturnValue && !soap12; }
            set { isReturnValue = value; }
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadNull"]/*' />
        protected bool ReadNull() {
            if (!GetNullAttr()) return false;
            if (r.IsEmptyElement) {
                r.Skip();
                return true;
            }
            r.ReadStartElement();
            int whileIterations = 0;
            int readerCount = ReaderCount;
            while (r.NodeType != XmlNodeType.EndElement)
            {
                UnknownNode(null);
                CheckReaderCount(ref whileIterations, ref readerCount);
            }
            ReadEndElement();
            return true;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.GetNullAttr"]/*' />
        protected bool GetNullAttr() {
            string isNull = r.GetAttribute(nilID, instanceNsID);
            if(isNull == null)
                isNull = r.GetAttribute(nullID, instanceNsID);
            if (isNull == null) {
                isNull = r.GetAttribute(nullID, instanceNs2000ID);
                if (isNull == null)
                    isNull = r.GetAttribute(nullID, instanceNs1999ID);
            }
            if (isNull == null || !XmlConvert.ToBoolean(isNull)) return false;
            return true;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadNullableString"]/*' />
        protected string ReadNullableString() {
            if (ReadNull()) return null;
            return r.ReadElementString();
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadNullableQualifiedName"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected XmlQualifiedName ReadNullableQualifiedName() {
            if (ReadNull()) return null;
            return ReadElementQualifiedName();
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadElementQualifiedName"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected XmlQualifiedName ReadElementQualifiedName() {
            if (r.IsEmptyElement) {
                XmlQualifiedName empty = new XmlQualifiedName(string.Empty, r.LookupNamespace(""));
                r.Skip();
                return empty;
            }
            XmlQualifiedName qname = ToXmlQualifiedName(CollapseWhitespace(r.ReadString()));
            r.ReadEndElement();
            return qname;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadXmlDocument"]/*' />
        protected XmlDocument ReadXmlDocument(bool wrapped) {
            XmlNode n = ReadXmlNode(wrapped);
            if (n == null)
                return null;
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.ImportNode(n, true));
            return doc;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.CollapseWhitespace"]/*' />
        protected string CollapseWhitespace(string value) {
            if (value == null)
                return null;
            return value.Trim();
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadXmlNode"]/*' />
        protected XmlNode ReadXmlNode(bool wrapped) {
            XmlNode node = null;
            if (wrapped) {
                if (ReadNull()) return null;
                r.ReadStartElement();
                r.MoveToContent();
                if (r.NodeType != XmlNodeType.EndElement)
                    node = Document.ReadNode(r);
                int whileIterations = 0;
                int readerCount = ReaderCount;
                while (r.NodeType != XmlNodeType.EndElement)
                {
                    UnknownNode(null);
                    CheckReaderCount(ref whileIterations, ref readerCount);
                }
                r.ReadEndElement();
            }
            else {
                node = Document.ReadNode(r);
            }
            return node;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ToByteArrayBase64"]/*' />
        protected static byte[] ToByteArrayBase64(string value) {
            return XmlCustomFormatter.ToByteArrayBase64(value);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ToByteArrayBase641"]/*' />
        protected byte[] ToByteArrayBase64(bool isNull) {
            if (isNull) {
                return null;
            }
            return ReadByteArray(true); //means use Base64
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ToByteArrayHex"]/*' />
        protected static byte[] ToByteArrayHex(string value) {
            return XmlCustomFormatter.ToByteArrayHex(value);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ToByteArrayHex1"]/*' />
        protected byte[] ToByteArrayHex(bool isNull) {
            if (isNull) {
                return null;
            }
            return ReadByteArray(false); //means use BinHex
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.GetArrayLength"]/*' />
        protected int GetArrayLength(string name, string ns) {
            if (GetNullAttr()) return 0;
            string arrayType = r.GetAttribute(arrayTypeID, soapNsID);
            SoapArrayInfo arrayInfo = ParseArrayType(arrayType);
            if (arrayInfo.dimensions != 1) throw new InvalidOperationException(ResXml.GetString(ResXml.XmlInvalidArrayDimentions, CurrentTag()));
            XmlQualifiedName qname = ToXmlQualifiedName(arrayInfo.qname, false);
            if (qname.Name != name) throw new InvalidOperationException(ResXml.GetString(ResXml.XmlInvalidArrayTypeName, qname.Name, name, CurrentTag()));
            if (qname.Namespace != ns) throw new InvalidOperationException(ResXml.GetString(ResXml.XmlInvalidArrayTypeNamespace, qname.Namespace, ns, CurrentTag()));
            return arrayInfo.length;
        }

        struct SoapArrayInfo {
            /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.qname;"]/*' />
            public string qname;
            /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.dimensions;"]/*' />
            public int dimensions;
            /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.length;"]/*' />
            public int length;
            public int jaggedDimensions;
        }
 
        private SoapArrayInfo ParseArrayType(string value) {
            if (value == null) {
                throw new ArgumentNullException(ResXml.GetString(ResXml.XmlMissingArrayType, CurrentTag()));
            }

            if (value.Length == 0) {
                throw new ArgumentException(ResXml.GetString(ResXml.XmlEmptyArrayType, CurrentTag()), "value");
            }
 
            char[] chars = value.ToCharArray();
            int charsLength = chars.Length;
        
            SoapArrayInfo soapArrayInfo = new SoapArrayInfo(); 
 
            // Parse backwards to get length first, then optional dimensions, then qname.
            int pos = charsLength - 1;
 
            // Must end with ]
            if (chars[pos] != ']') {
                throw new ArgumentException(ResXml.GetString(ResXml.XmlInvalidArraySyntax), "value");
            }
            pos--;   
 
            // Find [
            while (pos != -1 && chars[pos] != '[') {
                if (chars[pos] == ',')
                    throw new ArgumentException(ResXml.GetString(ResXml.XmlInvalidArrayDimentions, CurrentTag()), "value");
                pos--;
            }
            if (pos == -1) {
                throw new ArgumentException(ResXml.GetString(ResXml.XmlMismatchedArrayBrackets), "value");
            }
 
            int len = charsLength - pos - 2;
            if (len > 0) {
                string lengthString = new String(chars, pos + 1, len);
                try {
                    soapArrayInfo.length = Int32.Parse(lengthString, CultureInfo.InvariantCulture);
                }
                catch (Exception e) {
                    if (/*e is ThreadAbortException || e is StackOverflowException ||*/ e is OutOfMemoryException) {
                        throw;
                    }
                    throw new ArgumentException(ResXml.GetString(ResXml.XmlInvalidArrayLength, lengthString), "value");
                }
            }
            else {
                soapArrayInfo.length = -1;
            }

            pos--;         

            soapArrayInfo.jaggedDimensions = 0;
            while (pos != -1 && chars[pos] == ']') {
                pos--;
                if (pos < 0)
                    throw new ArgumentException(ResXml.GetString(ResXml.XmlMismatchedArrayBrackets), "value");
                if (chars[pos] == ',')
                    throw new ArgumentException(ResXml.GetString(ResXml.XmlInvalidArrayDimentions, CurrentTag()), "value");
                else if (chars[pos] != '[')
                    throw new ArgumentException(ResXml.GetString(ResXml.XmlInvalidArraySyntax), "value");
                pos--;
                soapArrayInfo.jaggedDimensions++;
            }

            soapArrayInfo.dimensions = 1;
 
            // everything else is qname - validation of qnames?
            soapArrayInfo.qname = new String(chars, 0, pos + 1);
            return soapArrayInfo;
        }

        private SoapArrayInfo ParseSoap12ArrayType(string itemType, string arraySize) {
            SoapArrayInfo soapArrayInfo = new SoapArrayInfo(); 

            if (itemType != null && itemType.Length > 0)
                soapArrayInfo.qname = itemType;
            else
                soapArrayInfo.qname = "";

            string[] dimensions;
            if (arraySize != null && arraySize.Length > 0)
                dimensions = arraySize.Split(null);
            else
                dimensions = new string[0];

            soapArrayInfo.dimensions = 0;
            soapArrayInfo.length = -1;
            for (int i = 0; i < dimensions.Length; i++) {
                if (dimensions[i].Length > 0) {
                    if (dimensions[i] == "*") {
                        soapArrayInfo.dimensions++;
                    }
                    else {
                        try {
                            soapArrayInfo.length = Int32.Parse(dimensions[i], CultureInfo.InvariantCulture);
                            soapArrayInfo.dimensions++;
                        }
                        catch (Exception e) {
                            if (/*e is ThreadAbortException || e is StackOverflowException ||*/ e is OutOfMemoryException) {
                                throw;
                            }
                            throw new ArgumentException(ResXml.GetString(ResXml.XmlInvalidArrayLength, dimensions[i]), "value");
                        }
                    }
                }
            }
            if (soapArrayInfo.dimensions == 0)
                soapArrayInfo.dimensions = 1; // default is 1D even if no arraySize is specified

            return soapArrayInfo;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ToDateTime"]/*' />
        protected static DateTime ToDateTime(string value) {
            return XmlCustomFormatter.ToDateTime(value);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ToDate"]/*' />
        protected static DateTime ToDate(string value) {
            return XmlCustomFormatter.ToDate(value);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ToTime"]/*' />
        protected static DateTime ToTime(string value) {
            return XmlCustomFormatter.ToTime(value);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ToChar"]/*' />
        protected static char ToChar(string value) {
            return XmlCustomFormatter.ToChar(value);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ToEnum"]/*' />
        protected static long ToEnum(string value, Hashtable h, string typeName) {
            return XmlCustomFormatter.ToEnum(value, h, typeName, true);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ToXmlName"]/*' />
        protected static string ToXmlName(string value) {
            return XmlCustomFormatter.ToXmlName(value);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ToXmlNCName"]/*' />
        protected static string ToXmlNCName(string value) {
            return XmlCustomFormatter.ToXmlNCName(value);
        }
        
        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ToXmlNmToken"]/*' />
        protected static string ToXmlNmToken(string value) {
            return XmlCustomFormatter.ToXmlNmToken(value);
        }
        
        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ToXmlNmTokens"]/*' />
        protected static string ToXmlNmTokens(string value) {
            return XmlCustomFormatter.ToXmlNmTokens(value);
        }
        
        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ToXmlQualifiedName"]/*' />
        protected XmlQualifiedName ToXmlQualifiedName(string value) {
            return ToXmlQualifiedName(value, DecodeName);
        }

        internal XmlQualifiedName ToXmlQualifiedName(string value, bool decodeName) {
            int colon = value == null ? -1 : value.LastIndexOf(':');
            string prefix = colon < 0 ? null : value.Substring(0, colon);
            string localName = value.Substring(colon + 1);

            if (decodeName) {
                prefix = XmlConvert.DecodeName(prefix);
                localName = XmlConvert.DecodeName(localName);
            }
            if (prefix == null || prefix.Length == 0) {
                return new XmlQualifiedName(r.NameTable.Add(value), r.LookupNamespace(String.Empty));
            }
            else {
                string ns = r.LookupNamespace(prefix);
                if (ns == null) {
                    // Namespace prefix '{0}' is not defined.
                    throw new InvalidOperationException(ResXml.GetString(ResXml.XmlUndefinedAlias, prefix));
                }
                return new XmlQualifiedName(r.NameTable.Add(localName), ns);
            }
        }
        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.UnknownAttribute"]/*' />
        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.UnknownAttribute"]/*' />
        protected void UnknownAttribute(object o, XmlAttribute attr) {
            UnknownAttribute(o, attr, null);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.UnknownAttribute1"]/*' />
        protected void UnknownAttribute(object o, XmlAttribute attr, string qnames) {
            if (events.OnUnknownAttribute != null) {
                int lineNumber, linePosition;
                GetCurrentPosition(out lineNumber, out linePosition);
                XmlAttributeEventArgs e = new XmlAttributeEventArgs(attr, lineNumber, linePosition, o, qnames);
                events.OnUnknownAttribute(events.sender, e);
            }
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.UnknownAttribute"]/*' />
        protected void UnknownElement(object o, XmlElement elem) {
            UnknownElement(o, elem, null);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.UnknownElement1"]/*' />
        protected void UnknownElement(object o, XmlElement elem, string qnames) {
            if (events.OnUnknownElement != null) {
                int lineNumber, linePosition;
                GetCurrentPosition(out lineNumber, out linePosition);
                XmlElementEventArgs e = new XmlElementEventArgs(elem, lineNumber, linePosition, o, qnames);
                events.OnUnknownElement(events.sender, e);
            }
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.UnknownNode"]/*' />
        protected void UnknownNode(object o) {
            UnknownNode(o, null);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.UnknownNode1"]/*' />
        protected void UnknownNode(object o, string qnames) {
            if (r.NodeType == XmlNodeType.None || r.NodeType == XmlNodeType.Whitespace) {
                r.Read();
                return;
            }
            if (r.NodeType == XmlNodeType.EndElement)
                return;
            if (events.OnUnknownNode != null) {
                UnknownNode(Document.ReadNode(r), o, qnames);
            }
            else if (r.NodeType == XmlNodeType.Attribute && events.OnUnknownAttribute == null) {
                return;
            }
            else if (r.NodeType == XmlNodeType.Element && events.OnUnknownElement == null) {
                r.Skip();
                return;
            }
            else {
                UnknownNode(Document.ReadNode(r), o, qnames);
            }
        }

        void UnknownNode(XmlNode unknownNode, object o, string qnames) {
            if (unknownNode == null)
                return;
            if (unknownNode.NodeType != XmlNodeType.None && unknownNode.NodeType != XmlNodeType.Whitespace && events.OnUnknownNode != null) {
                int lineNumber, linePosition;
                GetCurrentPosition(out lineNumber, out linePosition);
                XmlNodeEventArgs e = new XmlNodeEventArgs(unknownNode, lineNumber, linePosition, o);
                events.OnUnknownNode(events.sender, e);
            }
            if (unknownNode.NodeType == XmlNodeType.Attribute) {
                UnknownAttribute(o, (XmlAttribute)unknownNode, qnames);
            }
            else if (unknownNode.NodeType == XmlNodeType.Element) {
                UnknownElement(o, (XmlElement)unknownNode, qnames);
            }
        }


        void GetCurrentPosition(out int lineNumber, out int linePosition){
            if (Reader is IXmlLineInfo){
                IXmlLineInfo lineInfo = (IXmlLineInfo)Reader;
                lineNumber = lineInfo.LineNumber;
                linePosition = lineInfo.LinePosition;
            }
            else
                lineNumber = linePosition = -1;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.UnreferencedObject"]/*' />
        protected void UnreferencedObject(string id, object o) {
            if (events.OnUnreferencedObject != null) {
                UnreferencedObjectEventArgs e = new UnreferencedObjectEventArgs(o, id);
                events.OnUnreferencedObject(events.sender, e);
            }
        }

        string CurrentTag() {
            switch (r.NodeType) {
                case XmlNodeType.Element:
                    return "<" + r.LocalName + " xmlns='" + r.NamespaceURI + "'>";
                case XmlNodeType.EndElement:
                    return ">";
                case XmlNodeType.Text:
                    return r.Value;
                case XmlNodeType.CDATA:
                    return "CDATA";
                case XmlNodeType.Comment:
                    return "<--";
                case XmlNodeType.ProcessingInstruction:
                    return "<?";
                default:
                    return "(unknown)";
            }
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.CreateUnknownTypeException"]/*' />
        protected Exception CreateUnknownTypeException(XmlQualifiedName type) {
            return new InvalidOperationException(ResXml.GetString(ResXml.XmlUnknownType, type.Name, type.Namespace, CurrentTag()));
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.CreateReadOnlyCollectionException"]/*' />
        protected Exception CreateReadOnlyCollectionException(string name) {
            return new InvalidOperationException(ResXml.GetString(ResXml.XmlReadOnlyCollection, name));
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.CreateAbstractTypeException"]/*' />
        protected Exception CreateAbstractTypeException(string name, string ns) {
            return new InvalidOperationException(ResXml.GetString(ResXml.XmlAbstractType, name, ns, CurrentTag()));
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.CreateInaccessibleConstructorException"]/*' />
        protected Exception CreateInaccessibleConstructorException(string typeName) {
            return new InvalidOperationException(ResXml.GetString(ResXml.XmlConstructorInaccessible, typeName));
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.CreateCtorHasSecurityException"]/*' />
        protected Exception CreateCtorHasSecurityException(string typeName) {
            return new InvalidOperationException(ResXml.GetString(ResXml.XmlConstructorHasSecurityAttributes, typeName));
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.CreateUnknownNodeException"]/*' />
        protected Exception CreateUnknownNodeException() {
            return new InvalidOperationException(ResXml.GetString(ResXml.XmlUnknownNode, CurrentTag()));
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.CreateUnknownConstantException"]/*' />
        protected Exception CreateUnknownConstantException(string value, Type enumType) {
            return new InvalidOperationException(ResXml.GetString(ResXml.XmlUnknownConstant, value, enumType.Name));
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.CreateInvalidCastException"]/*' />
        protected Exception CreateInvalidCastException(Type type, object value) {
            return CreateInvalidCastException(type, value, null);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.CreateInvalidCastException1"]/*' />
        protected Exception CreateInvalidCastException(Type type, object value, string id) {
            if (value == null)
                return new InvalidCastException(ResXml.GetString(ResXml.XmlInvalidNullCast, type.FullName));
            else if (id == null)
                return new InvalidCastException(ResXml.GetString(ResXml.XmlInvalidCast, value.GetType().FullName, type.FullName));
            else
                return new InvalidCastException(ResXml.GetString(ResXml.XmlInvalidCastWithId, value.GetType().FullName, type.FullName, id));

        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.CreateBadDerivationException"]/*' />
        protected Exception CreateBadDerivationException(string xsdDerived, string nsDerived, string xsdBase, string nsBase, string clrDerived, string clrBase) {
            return new InvalidOperationException(ResXml.GetString(ResXml.XmlSerializableBadDerivation, xsdDerived, nsDerived, xsdBase, nsBase, clrDerived, clrBase));
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.CreateMissingIXmlSerializableType"]/*' />
        protected Exception CreateMissingIXmlSerializableType(string name, string ns, string clrType) {
            return new InvalidOperationException(ResXml.GetString(ResXml.XmlSerializableMissingClrType, name, ns, typeof(XmlIncludeAttribute).Name, clrType));
            //XmlSerializableMissingClrType= Type '{0}' from namespace '{1}' doesnot have corresponding IXmlSerializable type. Please consider adding {2} to '{3}'.
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.EnsureArrayIndex"]/*' />
        protected Array EnsureArrayIndex(Array a, int index, Type elementType) {
            if (a == null) return Array.CreateInstance(elementType, 32);
            if (index < a.Length) return a;
            Array b = Array.CreateInstance(elementType, a.Length * 2);
            Array.Copy(a, b, index);
            return b;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ShrinkArray"]/*' />
        protected Array ShrinkArray(Array a, int length, Type elementType, bool isNullable) {
            if (a == null) {
                if (isNullable) return null;
                return Array.CreateInstance(elementType, 0);
            }
            if (a.Length == length) return a;
            Array b = Array.CreateInstance(elementType, length);
            Array.Copy(a, b, length);
            return b;
        } 

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadString"]/*' />
        protected string ReadString(string value) {
            return ReadString(value, false);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadString1"]/*' />
        protected string ReadString(string value, bool trim) {
            string str = r.ReadString();
            if (str != null && trim)
                str = str.Trim();
            if (value == null || value.Length == 0)
                return str;
            return value + str;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadSerializable"]/*' />
        protected IXmlSerializable ReadSerializable(IXmlSerializable serializable) {
            return ReadSerializable(serializable, false);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadSerializable"]/*' />
        protected IXmlSerializable ReadSerializable(IXmlSerializable serializable, bool wrappedAny)
        {
            string name = null;
            string ns = null;

            if (wrappedAny) {
                name = r.LocalName;
                ns = r.NamespaceURI;
                r.Read();
                r.MoveToContent();
            }
            serializable.ReadXml(r);

            if (wrappedAny) {
                while (r.NodeType == XmlNodeType.Whitespace) r.Skip();
                if (r.NodeType == XmlNodeType.None) r.Skip();
                if (r.NodeType == XmlNodeType.EndElement && r.LocalName == name && r.NamespaceURI == ns) {
                    Reader.Read();
                }
            }
            return serializable;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadReference"]/*' />
        protected bool ReadReference(out string fixupReference) {
            string href = soap12 ? r.GetAttribute("ref", Soap12.Encoding) : r.GetAttribute("href");
            if (href == null) {
                fixupReference = null;
                return false;
            }
            if (!soap12) {
                // soap 1.1 href starts with '#'; soap 1.2 ref does not
                if (!href.StartsWith("#", StringComparison.Ordinal)) throw new InvalidOperationException(ResXml.GetString(ResXml.XmlMissingHref, href));
                fixupReference = href.Substring(1);
            }
            else
                fixupReference = href;

            if (r.IsEmptyElement) {
                r.Skip();
            }
            else {
                r.ReadStartElement();
                ReadEndElement();
            }
            return true;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.AddTarget"]/*' />
        protected void AddTarget(string id, object o) {
            if (id == null) {
                if (targetsWithoutIds == null) 
                    targetsWithoutIds = new ArrayList();
                if (o != null) 
                    targetsWithoutIds.Add(o);
            }
            else {
                if (targets == null) targets = new Hashtable();
                if (!targets.Contains(id))
                    targets.Add(id, o);
            }
        }



        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.AddFixup"]/*' />
        protected void AddFixup(Fixup fixup) {
            if (fixups == null) fixups = new ArrayList();
            fixups.Add(fixup);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.AddFixup2"]/*' />
        protected void AddFixup(CollectionFixup fixup) {
            if (collectionFixups == null) collectionFixups = new ArrayList();
            collectionFixups.Add(fixup);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.GetTarget"]/*' />
        protected object GetTarget(string id) {
            object target = targets != null ? targets[id] : null;
            if (target == null) {
                throw new InvalidOperationException(ResXml.GetString(ResXml.XmlInvalidHref, id));
            }
            Referenced(target);
            return target;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.Referenced"]/*' />
        protected void Referenced(object o) {
            if (o == null) return;
            if (referencedTargets == null) referencedTargets = new Hashtable();
            referencedTargets[o] = o;
        }

        void HandleUnreferencedObjects() {
            if (targets != null) {
                foreach (DictionaryEntry target in targets) {
                    if (referencedTargets == null || !referencedTargets.Contains(target.Value)) {
                        UnreferencedObject((string)target.Key, target.Value);
                    }
                }
            }
            if (targetsWithoutIds != null) {
                foreach (object o in targetsWithoutIds) {
                    if (referencedTargets == null || !referencedTargets.Contains(o)) {
                        UnreferencedObject(null, o);
                    }
                }
            }
        }

        void DoFixups() {
            if (fixups == null) return;
            for (int i = 0; i < fixups.Count; i++) {
                Fixup fixup = (Fixup)fixups[i];
                fixup.Callback(fixup);
            }

            if (collectionFixups == null) return;
            for (int i = 0; i < collectionFixups.Count; i++) {
                CollectionFixup collectionFixup = (CollectionFixup)collectionFixups[i];
                collectionFixup.Callback(collectionFixup.Collection, collectionFixup.CollectionItems);
            }
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.FixupArrayRefs"]/*' />
        protected void FixupArrayRefs(object fixup) {
            Fixup f = (Fixup)fixup;
            Array array = (Array)f.Source;
            for (int i = 0; i < array.Length; i++) {
                string id = f.Ids[i];
                if (id == null) continue;
                object o = GetTarget(id);
                try {
                    array.SetValue(o, i);
                }
                catch (InvalidCastException) {
                    throw new InvalidOperationException(ResXml.GetString(ResXml.XmlInvalidArrayRef, id, o.GetType().FullName, i.ToString()));
                }
            }
        }

        object ReadArray(string typeName, string typeNs) {
            SoapArrayInfo arrayInfo;
            Type fallbackElementType = null;
            if (soap12) {
                string itemType = r.GetAttribute(itemTypeID, soap12NsID);
                string arraySize = r.GetAttribute(arraySizeID, soap12NsID);
                Type arrayType = (Type)types[new XmlQualifiedName(typeName, typeNs)];
                // no indication that this is an array?
                if (itemType == null && arraySize == null && (arrayType == null || !arrayType.IsArray))
                    return null;

                arrayInfo = ParseSoap12ArrayType(itemType, arraySize);
                if (arrayType != null)
                    fallbackElementType = TypeScope.GetArrayElementType(arrayType, null);
            }
            else {
                string arrayType = r.GetAttribute(arrayTypeID, soapNsID);
                if (arrayType == null) 
                    return null;

                arrayInfo = ParseArrayType(arrayType);
            }

            if (arrayInfo.dimensions != 1) throw new InvalidOperationException(ResXml.GetString(ResXml.XmlInvalidArrayDimentions, CurrentTag()));

            // NOTE: don't use the array size that is specified since an evil client might pass
            // a number larger than the actual number of items in an attempt to harm the server.

            XmlQualifiedName qname;
            bool isPrimitive;
            Type elementType = null;
            XmlQualifiedName urTypeName = new XmlQualifiedName(urTypeID, schemaNsID);
            if (arrayInfo.qname.Length > 0) {
                qname = ToXmlQualifiedName(arrayInfo.qname, false);
                elementType = (Type)types[qname];
            }
            else
                qname = urTypeName;
            
            // try again if the best we could come up with was object
            if (soap12 && elementType == typeof(object))
                elementType = null;
            
            if (elementType == null) {
                if (!soap12) {
                    elementType = GetPrimitiveType(qname, true);
                    isPrimitive = true;
                }
                else {
                    // try it as a primitive
                    if (qname != urTypeName)
                        elementType = GetPrimitiveType(qname, false);
                    if (elementType != null) {
                        isPrimitive = true;
                    }
                    else {
                        // still nothing: go with fallback type or object
                        if (fallbackElementType == null) {
                            elementType = typeof(object);
                            isPrimitive = false;
                        }
                        else {
                            elementType = fallbackElementType;
                            XmlQualifiedName newQname = (XmlQualifiedName)typesReverse[elementType];
                            if (newQname == null) {
                                newQname = XmlSerializationWriter.GetPrimitiveTypeNameInternal(elementType);
                                isPrimitive = true;
                            }
                            else
                                isPrimitive = elementType.GetTypeInfo().IsPrimitive;
                            if (newQname != null) qname = newQname;
                        }
                    }
                }
            }
            else
                isPrimitive = elementType.GetTypeInfo().IsPrimitive;

            if (!soap12 && arrayInfo.jaggedDimensions > 0) {
                for (int i = 0; i < arrayInfo.jaggedDimensions; i++)
                    elementType = elementType.MakeArrayType();
            }

            if (r.IsEmptyElement) {
                r.Skip();
                return Array.CreateInstance(elementType, 0);
            }

            r.ReadStartElement();
            r.MoveToContent();

            int arrayLength = 0;
            Array array = null;

            if (elementType.GetTypeInfo().IsValueType) {
                if (!isPrimitive && !elementType.GetTypeInfo().IsEnum) {
                    throw new NotSupportedException(ResXml.GetString(ResXml.XmlRpcArrayOfValueTypes, elementType.FullName));
                }
                // CONSIDER, erikc, we could have specialized read functions here
                // for primitives, which would avoid boxing.
                int whileIterations = 0;
                int readerCount = ReaderCount;
                while (r.NodeType != XmlNodeType.EndElement) {
                    array = EnsureArrayIndex(array, arrayLength, elementType);
                    array.SetValue(ReadReferencedElement(qname.Name, qname.Namespace), arrayLength);
                    arrayLength++;
                    r.MoveToContent();
                    CheckReaderCount(ref whileIterations, ref readerCount);
                }
                array = ShrinkArray(array, arrayLength, elementType, false);
            }
            else {
                string type;
                string typens;
                string[] ids = null;
                int idsLength = 0;

                int whileIterations = 0;
                int readerCount = ReaderCount;
                while (r.NodeType != XmlNodeType.EndElement) {
                    array = EnsureArrayIndex(array, arrayLength, elementType);
                    ids = (string[])EnsureArrayIndex(ids, idsLength, typeof(string));
                    // CONSIDER: i'm not sure it's correct to let item name take precedence over arrayType
                    if (r.NamespaceURI.Length != 0){
                        type = r.LocalName;
                        if ((object)r.NamespaceURI == (object)soapNsID)
                            typens = XmlSchema.Namespace;
                        else
                            typens = r.NamespaceURI;
                    }
                    else {
                        type = qname.Name;
                        typens = qname.Namespace;                        
                    }
                    array.SetValue(ReadReferencingElement(type, typens, out ids[idsLength]), arrayLength);
                    arrayLength++;
                    idsLength++;
                    // CONSIDER, erikc, sparse arrays, perhaps?
                    r.MoveToContent();
                    CheckReaderCount(ref whileIterations, ref readerCount);
                }

                // special case for soap 1.2: try to get a better fit than object[] when no metadata is known
                // this applies in the doc/enc/bare case
                if (soap12 && elementType == typeof(object)) {
                    Type itemType = null;
                    for (int i = 0; i < arrayLength; i++) {
                        object currItem = array.GetValue(i);
                        if (currItem != null) {
                            Type currItemType = currItem.GetType();
                            if (currItemType.GetTypeInfo().IsValueType) {
                                itemType = null;
                                break;
                            }
                            if (itemType == null || currItemType.IsAssignableFrom(itemType)) {
                                itemType = currItemType;
                            }
                            else if (!itemType.IsAssignableFrom(currItemType)) {
                                itemType = null;
                                break;
                            }
                        }
                    }
                    if (itemType != null)
                        elementType = itemType;
                }
                ids = (string[])ShrinkArray(ids, idsLength, typeof(string), false);
                array = ShrinkArray(array, arrayLength, elementType, false);
                Fixup fixupArray = new Fixup(array, new XmlSerializationFixupCallback(this.FixupArrayRefs), ids);
                AddFixup(fixupArray);
            }

            // CONSIDER, erikc, check to see if the specified array length was right, perhaps?

            ReadEndElement();
            return array;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.InitCallbacks"]/*' />
        protected abstract void InitCallbacks();

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadReferencedElements"]/*' />
        protected void ReadReferencedElements() {
            r.MoveToContent();
            string dummy;
            int whileIterations = 0;
            int readerCount = ReaderCount;
            while (r.NodeType != XmlNodeType.EndElement && r.NodeType != XmlNodeType.None) {
                ReadReferencingElement(null, null, true, out dummy);
                r.MoveToContent();
                CheckReaderCount(ref whileIterations, ref readerCount);
            }
            DoFixups();

            HandleUnreferencedObjects();
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadReferencedElement"]/*' />
        protected object ReadReferencedElement() {
            return ReadReferencedElement(null, null);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadReferencedElement1"]/*' />
        protected object ReadReferencedElement(string name, string ns) {
            string dummy;
            return ReadReferencingElement(name, ns, out dummy);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadReferencingElement"]/*' />
        protected object ReadReferencingElement(out string fixupReference) {
            return ReadReferencingElement(null, null, out fixupReference);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadReferencingElement1"]/*' />
        protected object ReadReferencingElement(string name, string ns, out string fixupReference) {
            return ReadReferencingElement(name, ns, false, out fixupReference);
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadReferencingElement2"]/*' />
        protected object ReadReferencingElement(string name, string ns, bool elementCanBeType, out string fixupReference) {
            object o = null;

            if (callbacks == null) {
                callbacks = new Hashtable();
                types = new Hashtable();
                XmlQualifiedName urType = new XmlQualifiedName(urTypeID, r.NameTable.Add(XmlSchema.Namespace));
                types.Add(urType, typeof(object));
                typesReverse = new Hashtable();
                typesReverse.Add(typeof(object), urType);
                InitCallbacks();
            }

            r.MoveToContent();

            if (ReadReference(out fixupReference)) return null;

            if (ReadNull()) return null;

            string id = soap12 ? r.GetAttribute("id", Soap12.Encoding) : r.GetAttribute("id", null);

            if ((o = ReadArray(name, ns)) == null) {
                XmlQualifiedName typeId = GetXsiType();
                if (typeId == null) {
                    if (name == null)
                        typeId = new XmlQualifiedName(r.NameTable.Add(r.LocalName), r.NameTable.Add(r.NamespaceURI));
                    else
                        typeId = new XmlQualifiedName(r.NameTable.Add(name), r.NameTable.Add(ns));
                }
                XmlSerializationReadCallback callback = (XmlSerializationReadCallback)callbacks[typeId];
                if (callback != null) {
                    o = callback();
                }
                else
                    o = ReadTypedPrimitive(typeId, elementCanBeType);
            }

            AddTarget(id, o);

            return o;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.AddReadCallback"]/*' />
        protected void AddReadCallback(string name, string ns, Type type, XmlSerializationReadCallback read) {
            XmlQualifiedName typeName = new XmlQualifiedName(r.NameTable.Add(name), r.NameTable.Add(ns));
            callbacks[typeName] = read;
            types[typeName] = type;
            typesReverse[type] = typeName;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.ReadEndElement"]/*' />
        protected void ReadEndElement() {
            while (r.NodeType == XmlNodeType.Whitespace) r.Skip();
            if (r.NodeType == XmlNodeType.None) r.Skip();
            else r.ReadEndElement();
        }

        object ReadXmlNodes(bool elementCanBeType) {

            ArrayList xmlNodeList = new ArrayList();
            string elemLocalName = Reader.LocalName;
            string elemNs = Reader.NamespaceURI;
            string elemName = Reader.Name;
            string xsiTypeName = null;
            string xsiTypeNs = null;
            int skippableNodeCount = 0;
            int lineNumber = -1, linePosition=-1;
            XmlNode unknownNode = null;
            if(Reader.NodeType == XmlNodeType.Attribute){
                XmlAttribute attr = Document.CreateAttribute(elemName, elemNs);
                attr.Value = Reader.Value;
                unknownNode = attr;
            }
            else
                unknownNode = Document.CreateElement(elemName, elemNs);
            GetCurrentPosition(out lineNumber, out linePosition);
            XmlElement unknownElement = unknownNode as XmlElement;

            while (Reader.MoveToNextAttribute()) {
                if (IsXmlnsAttribute(Reader.Name) || (Reader.Name == "id" && (!soap12 || Reader.NamespaceURI == Soap12.Encoding)))
                    skippableNodeCount++;
                if ( (object)Reader.LocalName == (object)typeID &&
                     ( (object)Reader.NamespaceURI == (object)instanceNsID ||
                       (object)Reader.NamespaceURI == (object)instanceNs2000ID ||
                       (object)Reader.NamespaceURI == (object)instanceNs1999ID
                     )
                   ){
                    string value = Reader.Value;
                    int colon = value.LastIndexOf(':');
                    xsiTypeName = (colon >= 0) ? value.Substring(colon+1) : value;
                    xsiTypeNs = Reader.LookupNamespace((colon >= 0) ? value.Substring(0, colon) : "");
                }
                XmlAttribute xmlAttribute = (XmlAttribute)Document.ReadNode(r);
                xmlNodeList.Add(xmlAttribute);
                if (unknownElement != null) unknownElement.SetAttributeNode(xmlAttribute);
            }

            // If the node is referenced (or in case of paramStyle = bare) and if xsi:type is not
            // specified then the element name is used as the type name. Reveal this to the user
            // by adding an extra attribute node "xsi:type" with value as the element name.
            if(elementCanBeType && xsiTypeName == null){
                xsiTypeName = elemLocalName;
                xsiTypeNs = elemNs;
                XmlAttribute xsiTypeAttribute = Document.CreateAttribute(typeID, instanceNsID);
                xsiTypeAttribute.Value = elemName;
                xmlNodeList.Add(xsiTypeAttribute);
            }
            if( xsiTypeName == Soap.UrType &&
                ( (object)xsiTypeNs == (object)schemaNsID ||
                  (object)xsiTypeNs == (object)schemaNs1999ID ||
                  (object)xsiTypeNs == (object)schemaNs2000ID
                )
               )
                skippableNodeCount++;
            
            
            Reader.MoveToElement();
            if (Reader.IsEmptyElement) {
                Reader.Skip();
            }
            else {
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations = 0;
                int readerCount = ReaderCount;
                while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement) {
                    XmlNode xmlNode = Document.ReadNode(r);
                    xmlNodeList.Add(xmlNode);
                    if (unknownElement != null) unknownElement.AppendChild(xmlNode);
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations, ref readerCount);
                }
                ReadEndElement();

            }


            if(xmlNodeList.Count <= skippableNodeCount)
                return new object();

            XmlNode[] childNodes =  (XmlNode[])xmlNodeList.ToArray(typeof(XmlNode));

            UnknownNode(unknownNode, null, null);
            return childNodes;
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReader.CheckReaderCount"]/*' />
        protected void CheckReaderCount(ref int whileIterations, ref int readerCount)
        {
            if (checkDeserializeAdvances)
            {
                whileIterations++;
                if ((whileIterations & 0x80) == 0x80)
                {
                    if (readerCount == ReaderCount)
                        throw new InvalidOperationException(ResXml.GetString(ResXml.XmlInternalErrorReaderAdvance));
                    readerCount = ReaderCount;
                }
            }
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="Fixup"]/*' />
        ///<internalonly/>
        protected class Fixup {
            XmlSerializationFixupCallback callback;
            object source;
            string[] ids;

            /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="Fixup.Fixup1"]/*' />
            public Fixup(object o, XmlSerializationFixupCallback callback, int count) 
                : this (o, callback, new string[count]) {
            }

            /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="Fixup.Fixup2"]/*' />
            public Fixup(object o, XmlSerializationFixupCallback callback, string[] ids) {
                this.callback = callback;
                this.Source = o;
                this.ids = ids;
            }

            /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="Fixup.Callback"]/*' />
            public XmlSerializationFixupCallback Callback {
                get { return callback; }
            }

            /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="Fixup.Source"]/*' />
            public object Source {
                get { return source; }
                set { source = value; }
            }

            /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="Fixup.Ids"]/*' />
            public string[] Ids {
                get { return ids; }
            }
        }

        /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="CollectionFixup"]/*' />
        protected class CollectionFixup {
            XmlSerializationCollectionFixupCallback callback;
            object collection;
            object collectionItems;

            /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="CollectionFixup.CollectionFixup"]/*' />
            public CollectionFixup(object collection, XmlSerializationCollectionFixupCallback callback, object collectionItems) {
                this.callback = callback;
                this.collection = collection;
                this.collectionItems = collectionItems;
            }

            /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="CollectionFixup.Callback"]/*' />
            public XmlSerializationCollectionFixupCallback Callback {
                get { return callback; }
            }

            /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="CollectionFixup.Collection"]/*' />
            public object Collection {
                get { return collection; }
            }

            /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="CollectionFixup.CollectionItems"]/*' />
            public object CollectionItems {
                get { return collectionItems; }
            }
        }
    }

    /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationFixupCallback"]/*' />
    ///<internalonly/>
    public delegate void XmlSerializationFixupCallback(object fixup);


    /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationCollectionFixupCallback"]/*' />
    ///<internalonly/>
    public delegate void XmlSerializationCollectionFixupCallback(object collection, object collectionItems);

    /// <include file='doc\XmlSerializationReader.uex' path='docs/doc[@for="XmlSerializationReadCallback"]/*' />
    ///<internalonly/>
    public delegate object XmlSerializationReadCallback();

    internal class XmlSerializationReaderCodeGen : XmlSerializationCodeGen {
        Hashtable idNames = new Hashtable();
        Hashtable enums;
        Hashtable createMethods = new Hashtable();
        int nextCreateMethodNumber = 0;
        int nextIdNumber = 0;
        int nextWhileLoopIndex = 0;

        internal Hashtable Enums {
            get {
                if (enums == null) {
                    enums = new Hashtable();
                }
                return enums;
            }
        }

        class CreateCollectionInfo {
            string name;
            TypeDesc td;

            internal CreateCollectionInfo(string name, TypeDesc td) {
                this.name = name;
                this.td = td;
            }
            internal string Name {
                get { return name; }
            }

            internal TypeDesc TypeDesc {
                get { return td; }
            }
        }

        internal XmlSerializationReaderCodeGen(IndentedWriter writer, TypeScope[] scopes, string access, string className) : base(writer, scopes, access, className) {
        }
    }
}
