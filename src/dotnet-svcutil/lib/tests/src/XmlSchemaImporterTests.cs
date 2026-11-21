// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Tools.ServiceModel.Svcutil.XmlSerializer;
using Xunit;

namespace SvcutilTest
{
    public class XmlSchemaImporterTests
    {
        [Fact]
        public void ImportSchemaType_UsesSpecialMapping_ForWildcardTypeWithImplicitAnyTypeBase()
        {
            XmlSchema schema = LoadSchema(@"<?xml version=""1.0"" encoding=""utf-8""?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema""
           targetNamespace=""urn:repro:wildcard""
           elementFormDefault=""qualified"">
  <xs:complexType name=""FlexibleContentType"">
    <xs:sequence>
      <xs:any minOccurs=""0"" maxOccurs=""1"" processContents=""lax""/>
    </xs:sequence>
  </xs:complexType>
</xs:schema>");

            XmlSchemas schemas = CreateSchemas(schema);
            XmlSchemaComplexType type = GetComplexType(schemas, "FlexibleContentType", "urn:repro:wildcard");
            Assert.NotNull(type);
            Assert.NotNull(type.BaseXmlSchemaType);
            Assert.Null(type.ContentModel);

            XmlSchemaImporter importer = new XmlSchemaImporter(schemas);

            XmlTypeMapping mapping = importer.ImportSchemaType(new XmlQualifiedName("FlexibleContentType", "urn:repro:wildcard"));

            Assert.IsType<SpecialMapping>(mapping.Accessor.Mapping);
        }

        [Fact]
        public void ImportSchemaType_DoesNotUseSpecialMapping_ForWildcardTypeWithExplicitInheritance()
        {
            XmlSchema schema = LoadSchema(@"<?xml version=""1.0"" encoding=""utf-8""?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema""
           targetNamespace=""urn:repro:wildcard""
           elementFormDefault=""qualified"">
  <xs:complexType name=""BaseType"">
    <xs:sequence />
  </xs:complexType>

  <xs:complexType name=""DerivedWildcardType"">
    <xs:complexContent>
      <xs:extension base=""tns:BaseType"" xmlns:tns=""urn:repro:wildcard"">
        <xs:sequence>
          <xs:any minOccurs=""0"" maxOccurs=""1"" processContents=""lax""/>
        </xs:sequence>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
</xs:schema>");

            XmlSchemas schemas = CreateSchemas(schema);
            XmlSchemaComplexType type = GetComplexType(schemas, "DerivedWildcardType", "urn:repro:wildcard");
            Assert.NotNull(type);
            Assert.NotNull(type.ContentModel);
            XmlSchemaImporter importer = new XmlSchemaImporter(schemas);

            XmlTypeMapping mapping = importer.ImportSchemaType(new XmlQualifiedName("DerivedWildcardType", "urn:repro:wildcard"));

            Assert.IsNotType<SpecialMapping>(mapping.Accessor.Mapping);
            Assert.IsType<StructMapping>(mapping.Accessor.Mapping);
        }

        private static XmlSchemas CreateSchemas(XmlSchema schema)
        {
            XmlSchemas schemas = new XmlSchemas();
            schemas.Add(schema);
            schemas.Compile(null, true);
            return schemas;
        }

        private static XmlSchemaComplexType GetComplexType(XmlSchemas schemas, string typeName, string typeNamespace)
        {
            return (XmlSchemaComplexType)schemas.Find(new XmlQualifiedName(typeName, typeNamespace), typeof(XmlSchemaType));
        }

        private static XmlSchema LoadSchema(string schemaText)
        {
            using (StringReader reader = new StringReader(schemaText))
            {
                return XmlSchema.Read(reader, ValidationCallback);
            }
        }

        private static void ValidationCallback(object sender, ValidationEventArgs e)
        {
            throw new InvalidOperationException(e.Message, e.Exception);
        }
    }
}
