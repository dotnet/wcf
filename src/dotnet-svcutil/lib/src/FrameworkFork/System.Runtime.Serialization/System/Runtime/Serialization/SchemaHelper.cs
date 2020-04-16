// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Serialization
{
    using System;
    using Microsoft.Xml;
    using Microsoft.Xml.Schema;
    using System.Collections;
    using System.Collections.Generic;
    using SchemaObjectDictionary = System.Collections.Generic.Dictionary<Microsoft.Xml.XmlQualifiedName, SchemaObjectInfo>;

    internal class SchemaObjectInfo
    {
        internal XmlSchemaType type;
        internal XmlSchemaElement element;
        internal XmlSchema schema;
        internal List<XmlSchemaType> knownTypes;

        internal SchemaObjectInfo(XmlSchemaType type, XmlSchemaElement element, XmlSchema schema, List<XmlSchemaType> knownTypes)
        {
            this.type = type;
            this.element = element;
            this.schema = schema;
            this.knownTypes = knownTypes;
        }
    }

    internal static class SchemaHelper
    {
        internal static bool NamespacesEqual(string ns1, string ns2)
        {
            if (ns1 == null || ns1.Length == 0)
                return (ns2 == null || ns2.Length == 0);
            else
                return ns1 == ns2;
        }

        internal static XmlSchemaType GetSchemaType(SchemaObjectDictionary schemaInfo, XmlQualifiedName typeName)
        {
            SchemaObjectInfo schemaObjectInfo;
            if (schemaInfo.TryGetValue(typeName, out schemaObjectInfo))
            {
                return schemaObjectInfo.type;
            }
            return null;
        }

        internal static XmlSchema GetSchemaWithType(SchemaObjectDictionary schemaInfo, XmlSchemaSet schemas, XmlQualifiedName typeName)
        {
            SchemaObjectInfo schemaObjectInfo;
            if (schemaInfo.TryGetValue(typeName, out schemaObjectInfo))
            {
                if (schemaObjectInfo.schema != null)
                    return schemaObjectInfo.schema;
            }
            ICollection currentSchemas = schemas.Schemas();
            string ns = typeName.Namespace;
            foreach (XmlSchema schema in currentSchemas)
            {
                if (NamespacesEqual(ns, schema.TargetNamespace))
                {
                    return schema;
                }
            }
            return null;
        }

        internal static XmlSchemaElement GetSchemaElement(SchemaObjectDictionary schemaInfo, XmlQualifiedName elementName)
        {
            SchemaObjectInfo schemaObjectInfo;
            if (schemaInfo.TryGetValue(elementName, out schemaObjectInfo))
            {
                return schemaObjectInfo.element;
            }
            return null;
        }

        internal static XmlSchema GetSchemaWithGlobalElementDeclaration(XmlSchemaElement element, XmlSchemaSet schemas)
        {
            ICollection currentSchemas = schemas.Schemas();
            foreach (XmlSchema schema in currentSchemas)
            {
                foreach (XmlSchemaObject schemaObject in schema.Items)
                {
                    XmlSchemaElement schemaElement = schemaObject as XmlSchemaElement;
                    if (schemaElement == null)
                        continue;

                    if (schemaElement == element)
                    {
                        return schema;
                    }
                }
            }
            return null;
        }

        internal static XmlQualifiedName GetGlobalElementDeclaration(XmlSchemaSet schemas, XmlQualifiedName typeQName, out bool isNullable)
        {
            ICollection currentSchemas = schemas.Schemas();
            string ns = typeQName.Namespace;
            if (ns == null)
                ns = string.Empty;
            isNullable = false;
            foreach (XmlSchema schema in currentSchemas)
            {
                foreach (XmlSchemaObject schemaObject in schema.Items)
                {
                    XmlSchemaElement schemaElement = schemaObject as XmlSchemaElement;
                    if (schemaElement == null)
                        continue;

                    if (schemaElement.SchemaTypeName.Equals(typeQName))
                    {
                        isNullable = schemaElement.IsNillable;
                        return new XmlQualifiedName(schemaElement.Name, schema.TargetNamespace);
                    }
                }
            }
            return null;
        }
    }
}
