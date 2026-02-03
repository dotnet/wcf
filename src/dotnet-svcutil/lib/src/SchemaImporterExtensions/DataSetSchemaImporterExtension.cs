// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeDom;
using Microsoft.CodeDom.Compiler;
using Microsoft.Xml;
using Microsoft.Xml.Schema;
using Microsoft.Xml.Serialization;
using Microsoft.Xml.Serialization.Advanced;

using System;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    /// <summary>
    /// Minimal schema importer extension that recognizes the common msdata annotations used to represent
    /// ADO.NET DataSet/DataTable in WSDL/XSD and maps them to System.Data types.
    ///
    /// This intentionally avoids any dependency on System.Data internals and does not attempt to generate
    /// typed datasets; it only returns the CLR type name to use.
    /// </summary>
    public sealed class DataSetSchemaImporterExtension : SchemaImporterExtension
    {
        private const string MsDataNamespace = "urn:schemas-microsoft-com:xml-msdata";
        private const string DiffgramNamespace = "urn:schemas-microsoft-com:xml-diffgram-v1";

        // Common msdata attributes seen in DataSet/DataTable schema exports.
        private const string AttrIsDataSet = "IsDataSet";
        private const string AttrDataType = "DataType";
        private static readonly string[] s_dataTableHints =
        {
            // Typed DataTable hints (a subset; we only need a signal, not full fidelity).
            "GeneratorDataTableName",
            "GeneratorUserTableName",
            "GeneratorTableClassName",
            "GeneratorTableVarName",
        };

        // Keep strings to avoid compile-time references to System.Data assemblies.
        private const string DataSetClrTypeName = "System.Data.DataSet";
        private const string DataTableClrTypeName = "System.Data.DataTable";

        public override string ImportSchemaType(
            string name,
            string ns,
            XmlSchemaObject context,
            XmlSchemas schemas,
            XmlSchemaImporter importer,
            CodeCompileUnit compileUnit,
            CodeNamespace mainNamespace,
            CodeGenerationOptions options,
            CodeDomProvider codeProvider)
        {
            // Some import paths call this overload. We can still decide based on the schema context annotations.
            if (LooksLikeDataSetWrapperType(null, context))
            {
                return DataSetClrTypeName;
            }

            if (IsDataSetAnnotated(context))
            {
                return DataSetClrTypeName;
            }

            if (IsDataTableAnnotated(context))
            {
                return DataTableClrTypeName;
            }

            return null;
        }

        public override string ImportSchemaType(
            XmlSchemaType type,
            XmlSchemaObject context,
            XmlSchemas schemas,
            XmlSchemaImporter importer,
            CodeCompileUnit compileUnit,
            CodeNamespace mainNamespace,
            CodeGenerationOptions options,
            CodeDomProvider codeProvider)
        {
            // Heuristic fallback for schemas that don't include msdata annotations but use the classic
            // "xsd:schema" + "xsd:any" wrapper pattern produced by DataSet exports.
            // Example:
            //   <xsd:sequence>
            //     <xsd:element ref="xsd:schema" />
            //     <xsd:any />
            //   </xsd:sequence>
            if (LooksLikeDataSetWrapperType(type, context))
            {
                return DataSetClrTypeName;
            }

            // Prefer context (often an element) and fall back to the type itself.
            if (IsDataSetAnnotated(context) || IsDataSetAnnotated(type))
            {
                return DataSetClrTypeName;
            }

            if (IsDataTableAnnotated(context) || IsDataTableAnnotated(type))
            {
                return DataTableClrTypeName;
            }

            return null;
        }

        public override string ImportAnyElement(
            XmlSchemaAny any,
            bool mixed,
            XmlSchemas schemas,
            XmlSchemaImporter importer,
            CodeCompileUnit compileUnit,
            CodeNamespace mainNamespace,
            CodeGenerationOptions options,
            CodeDomProvider codeProvider)
        {
            // Some WSDLs carry msdata annotations directly on xs:any.
            if (IsDataSetAnnotated(any))
            {
                return DataSetClrTypeName;
            }

            if (IsDataTableAnnotated(any))
            {
                return DataTableClrTypeName;
            }

            return null;
        }

        private static bool LooksLikeDataSetWrapperType(XmlSchemaType type, XmlSchemaObject context)
        {
            // First, try the provided type.
            XmlSchemaComplexType complexType = type as XmlSchemaComplexType;

            // If the type isn't directly provided (or isn't complex), try deriving it from the context.
            if (complexType == null)
            {
                XmlSchemaElement element = context as XmlSchemaElement;
                if (element != null)
                {
                    complexType = element.SchemaType as XmlSchemaComplexType;
                }
            }

            if (complexType == null)
            {
                return false;
            }

            XmlSchemaSequence sequence = complexType.Particle as XmlSchemaSequence;
            if (sequence == null || sequence.Items == null || sequence.Items.Count == 0)
            {
                return false;
            }

            bool hasSchemaRef = false;
            bool hasSchemaAny = false;
            bool hasAny = false;

            // Ensure we only match the narrow "schema + any" wrapper pattern.
            // If there are other particles, bail out.
            for (int i = 0; i < sequence.Items.Count; i++)
            {
                XmlSchemaObject item = sequence.Items[i];

                XmlSchemaElement itemElement = item as XmlSchemaElement;
                if (itemElement != null)
                {
                    if (!itemElement.RefName.IsEmpty &&
                        itemElement.RefName.Name == "schema" &&
                        itemElement.RefName.Namespace == XmlSchema.Namespace)
                    {
                        hasSchemaRef = true;
                        continue;
                    }

                    // Another common wrapper uses an explicit diffgram element reference instead of xs:any.
                    //   <xs:element ref="diffgr:diffgram" />
                    if (!itemElement.RefName.IsEmpty &&
                        itemElement.RefName.Name == "diffgram" &&
                        itemElement.RefName.Namespace == DiffgramNamespace)
                    {
                        hasAny = true;
                        continue;
                    }

                    // Any other element means it's not the standard DataSet wrapper.
                    return false;
                }

                XmlSchemaAny itemAny = item as XmlSchemaAny;
                if (itemAny != null)
                {
                    // Another common DataSet wrapper is:
                    //   <xs:any namespace="http://www.w3.org/2001/XMLSchema" />
                    //   <xs:any namespace="urn:schemas-microsoft-com:xml-diffgram-v1" />
                    // (or a second <xs:any/> without explicit namespace)
                    if (itemAny.Namespace == XmlSchema.Namespace)
                    {
                        hasSchemaAny = true;
                    }
                    else
                    {
                        hasAny = true;
                    }

                    continue;
                }

                // Unknown particle.
                return false;
            }

            return (hasSchemaRef || hasSchemaAny) && hasAny;
        }

        private static bool IsDataSetAnnotated(XmlSchemaObject schemaObject)
        {
            var value = GetMsDataValue(schemaObject, AttrIsDataSet);
            if (value != null && value.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Some schemas use msdata:DataType="System.Data.DataSet" instead.
            var dataType = GetMsDataValue(schemaObject, AttrDataType);
            if (dataType != null &&
                (dataType.Equals(DataSetClrTypeName, StringComparison.OrdinalIgnoreCase) ||
                 dataType.EndsWith(".DataSet", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            return false;
        }

        private static bool IsDataTableAnnotated(XmlSchemaObject schemaObject)
        {
            // DataTable does not have a single canonical marker, so we use a small set of common hints.
            // We also explicitly avoid mapping to DataTable when IsDataSet==true.
            if (IsDataSetAnnotated(schemaObject))
            {
                return false;
            }

            // Some schemas use msdata:DataType="System.Data.DataTable".
            var dataType = GetMsDataValue(schemaObject, AttrDataType);
            if (dataType != null &&
                (dataType.Equals(DataTableClrTypeName, StringComparison.OrdinalIgnoreCase) ||
                 dataType.EndsWith(".DataTable", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            foreach (var hint in s_dataTableHints)
            {
                if (GetMsDataValue(schemaObject, hint) != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetMsDataValue(XmlSchemaObject schemaObject, string localName)
        {
            var fromAttributes = GetMsDataUnhandledAttributeValue(schemaObject, localName);
            if (fromAttributes != null)
            {
                return fromAttributes;
            }

            return GetMsDataAppInfoValue(schemaObject, localName);
        }

        private static string GetMsDataUnhandledAttributeValue(XmlSchemaObject schemaObject, string localName)
        {
            XmlSchemaAnnotated annotated = schemaObject as XmlSchemaAnnotated;
            if (annotated == null)
                return null;

            XmlAttribute[] attributes = annotated.UnhandledAttributes;
            if (attributes == null || attributes.Length == 0)
            {
                return null;
            }

            for (int i = 0; i < attributes.Length; i++)
            {
                XmlAttribute attr = attributes[i];
                if (attr == null)
                {
                    continue;
                }

                if (attr.NamespaceURI == MsDataNamespace && attr.LocalName == localName)
                {
                    return attr.Value;
                }
            }

            return null;
        }

        private static string GetMsDataAppInfoValue(XmlSchemaObject schemaObject, string localName)
        {
            XmlSchemaAnnotated annotated = schemaObject as XmlSchemaAnnotated;
            if (annotated == null)
            {
                return null;
            }

            XmlSchemaAnnotation annotation = annotated.Annotation;
            if (annotation == null)
            {
                return null;
            }

            if (annotation.Items == null)
            {
                return null;
            }

            for (int i = 0; i < annotation.Items.Count; i++)
            {
                XmlSchemaAppInfo appInfo = annotation.Items[i] as XmlSchemaAppInfo;
                if (appInfo == null || appInfo.Markup == null)
                {
                    continue;
                }

                for (int j = 0; j < appInfo.Markup.Length; j++)
                {
                    XmlNode node = appInfo.Markup[j];
                    if (node == null)
                    {
                        continue;
                    }

                    // Match element form: <msdata:IsDataSet>true</msdata:IsDataSet>
                    XmlElement element = node as XmlElement;
                    if (element != null)
                    {
                        if (element.NamespaceURI == MsDataNamespace && element.LocalName == localName)
                        {
                            return element.InnerText;
                        }

                        // Some appinfo blocks put msdata annotations as attributes on arbitrary elements.
                        if (element.Attributes != null)
                        {
                            for (int k = 0; k < element.Attributes.Count; k++)
                            {
                                XmlAttribute attr = element.Attributes[k];
                                if (attr != null && attr.NamespaceURI == MsDataNamespace && attr.LocalName == localName)
                                {
                                    return attr.Value;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Generic fallback: scan attributes, if any.
                        if (node.Attributes != null)
                        {
                            for (int k = 0; k < node.Attributes.Count; k++)
                            {
                                XmlAttribute attr = node.Attributes[k];
                                if (attr != null && attr.NamespaceURI == MsDataNamespace && attr.LocalName == localName)
                                {
                                    return attr.Value;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
