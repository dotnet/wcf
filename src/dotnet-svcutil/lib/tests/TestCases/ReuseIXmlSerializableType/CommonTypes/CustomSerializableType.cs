using System;
using System.Globalization;
using System.Numerics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonTypes
{
    public struct CustomSerializableType : IXmlSerializable
    {
        public BigInteger TestValue { get; set; }

        public XmlSchema? GetSchema()
        {
            XmlSchema schema = new XmlSchema()
            {
                Id = "CustomSerializableTypeSchema"
            };
            XmlSchemaSimpleType schemaSimpleType1 = new XmlSchemaSimpleType();
            schemaSimpleType1.Name = "BigIntegerString";
            XmlSchemaSimpleType schemaSimpleType2 = schemaSimpleType1;
            XmlSchemaSimpleTypeRestriction simpleTypeRestriction = new XmlSchemaSimpleTypeRestriction()
            {
                BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
            };
            schemaSimpleType2.Content = (XmlSchemaSimpleTypeContent) simpleTypeRestriction;
            schema.Items.Add((XmlSchemaObject) schemaSimpleType2);
            return schema;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            reader.ReadStartElement();
            this.TestValue = BigInteger.Parse(reader.ReadContentAsString());
            reader.ReadEndElement();
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("BigIntegerString");
            writer.WriteValue(this.TestValue.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }
    }
}
