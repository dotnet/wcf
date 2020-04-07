// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
 
namespace System.Runtime.Serialization
{
    using System;
    using Microsoft.CodeDom;
    using System.Collections.Generic;
    using System.Security;
    using Microsoft.Xml;
    using Microsoft.Xml.Serialization;
    using Microsoft.Xml.Schema;

    public class XsdDataContractImporter
    {
        ImportOptions options;
        CodeCompileUnit codeCompileUnit;
        DataContractSet dataContractSet;

        static readonly XmlQualifiedName[] emptyTypeNameArray = new XmlQualifiedName[0];

        static readonly XmlSchemaElement[] emptyElementArray = new XmlSchemaElement[0];
        XmlQualifiedName[] singleTypeNameArray;
        XmlSchemaElement[] singleElementArray;

        public XsdDataContractImporter()
        {
        }

        public XsdDataContractImporter(CodeCompileUnit codeCompileUnit)
        {
            this.codeCompileUnit = codeCompileUnit;
        }

        public ImportOptions Options
        {
            get { return options; }
            set { options = value; }
        }


        public CodeCompileUnit CodeCompileUnit
        {
            get
            {
                return GetCodeCompileUnit();
            }
        }

        private CodeCompileUnit GetCodeCompileUnit()
        {
            if (codeCompileUnit == null)
                codeCompileUnit = new CodeCompileUnit();
            return codeCompileUnit;
        }


        private DataContractSet DataContractSet
        {
            get
            {

                if (dataContractSet == null)
                {
                    dataContractSet = Options == null ? new DataContractSet(null, null, null) :
                                                        new DataContractSet(Options.DataContractSurrogate, Options.ReferencedTypes, Options.ReferencedCollectionTypes);
                }
                return dataContractSet;
            }
        }

        public void Import(XmlSchemaSet schemas)
        {
            if (schemas == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));

            InternalImport(schemas, null, null, null);
        }

        public void Import(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames)
        {
            if (schemas == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));

            if (typeNames == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeNames"));

            InternalImport(schemas, typeNames, emptyElementArray, emptyTypeNameArray);
        }

        public void Import(XmlSchemaSet schemas, XmlQualifiedName typeName)
        {
            if (schemas == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));

            if (typeName == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));

            SingleTypeNameArray[0] = typeName;
            InternalImport(schemas, SingleTypeNameArray, emptyElementArray, emptyTypeNameArray);
        }

        public XmlQualifiedName Import(XmlSchemaSet schemas, XmlSchemaElement element)
        {
            if (schemas == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));

            if (element == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("element"));

            SingleTypeNameArray[0] = null;
            SingleElementArray[0] = element;
            InternalImport(schemas, emptyTypeNameArray, SingleElementArray, SingleTypeNameArray/*filled on return*/);
            return SingleTypeNameArray[0];
        }

        public bool CanImport(XmlSchemaSet schemas)
        {
            if (schemas == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));

            return InternalCanImport(schemas, null, null, null);
        }

        public bool CanImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames)
        {
            if (schemas == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));

            if (typeNames == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeNames"));

            return InternalCanImport(schemas, typeNames, emptyElementArray, emptyTypeNameArray);
        }

        public bool CanImport(XmlSchemaSet schemas, XmlQualifiedName typeName)
        {
            if (schemas == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));

            if (typeName == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));

            return InternalCanImport(schemas, new XmlQualifiedName[] { typeName }, emptyElementArray, emptyTypeNameArray);
        }

        public bool CanImport(XmlSchemaSet schemas, XmlSchemaElement element)
        {
            if (schemas == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));

            if (element == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("element"));

            SingleTypeNameArray[0] = null;
            SingleElementArray[0] = element;
            return InternalCanImport(schemas, emptyTypeNameArray, SingleElementArray, SingleTypeNameArray);
        }

        public CodeTypeReference GetCodeTypeReference(XmlQualifiedName typeName)
        {
            DataContract dataContract = FindDataContract(typeName);
            CodeExporter codeExporter = new CodeExporter(DataContractSet, Options, GetCodeCompileUnit());
            return codeExporter.GetCodeTypeReference(dataContract);
        }

        public CodeTypeReference GetCodeTypeReference(XmlQualifiedName typeName, XmlSchemaElement element)
        {
            if (element == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("element"));
            if (typeName == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
            DataContract dataContract = FindDataContract(typeName);
            CodeExporter codeExporter = new CodeExporter(DataContractSet, Options, GetCodeCompileUnit());
#pragma warning disable 56506 // Code Exporter will never be null
            return codeExporter.GetElementTypeReference(dataContract, element.IsNillable);
        }

        internal DataContract FindDataContract(XmlQualifiedName typeName)
        {
            if (typeName == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));

            DataContract dataContract = DataContract.GetBuiltInDataContract(typeName.Name, typeName.Namespace);
            if (dataContract == null)
            {
                dataContract = DataContractSet[typeName];
                if (dataContract == null)
                    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRSerialization.Format(SRSerialization.TypeHasNotBeenImported, typeName.Name, typeName.Namespace)));
            }
            return dataContract;
        }

        public ICollection<CodeTypeReference> GetKnownTypeReferences(XmlQualifiedName typeName)
        {
            if (typeName == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));

            DataContract dataContract = DataContract.GetBuiltInDataContract(typeName.Name, typeName.Namespace);
            if (dataContract == null)
            {
                dataContract = DataContractSet[typeName];
                if (dataContract == null)
                    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRSerialization.Format(SRSerialization.TypeHasNotBeenImported, typeName.Name, typeName.Namespace)));
            }

            CodeExporter codeExporter = new CodeExporter(DataContractSet, Options, GetCodeCompileUnit());
            return codeExporter.GetKnownTypeReferences(dataContract);
        }

        XmlQualifiedName[] SingleTypeNameArray
        {
            get
            {
                if (singleTypeNameArray == null)
                    singleTypeNameArray = new XmlQualifiedName[1];
                return singleTypeNameArray;
            }
        }

        XmlSchemaElement[] SingleElementArray
        {
            get
            {
                if (singleElementArray == null)
                    singleElementArray = new XmlSchemaElement[1];
                return singleElementArray;
            }
        }

        void InternalImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames, ICollection<XmlSchemaElement> elements, XmlQualifiedName[] elementTypeNames/*filled on return*/)
        {
            DataContractSet oldValue = (dataContractSet == null) ? null : new DataContractSet(dataContractSet);
            try
            {
                SchemaImporter schemaImporter = new SchemaImporter(schemas, typeNames, elements, elementTypeNames/*filled on return*/, DataContractSet, ImportXmlDataType);
                schemaImporter.Import();

                CodeExporter codeExporter = new CodeExporter(DataContractSet, Options, GetCodeCompileUnit());
                codeExporter.Export();
            }
            catch (Exception ex)
            {
                if (DiagnosticUtility.IsFatal(ex))
                {
                    throw;
                }
                dataContractSet = oldValue;
                TraceImportError(ex);
                throw;
            }
        }

        bool ImportXmlDataType
        {
            get
            {
                return this.Options == null ? false : this.Options.ImportXmlType;
            }
        }

        void TraceImportError(Exception exception)
        {

        }

        bool InternalCanImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames, ICollection<XmlSchemaElement> elements, XmlQualifiedName[] elementTypeNames)
        {
            DataContractSet oldValue = (dataContractSet == null) ? null : new DataContractSet(dataContractSet);
            try
            {
                SchemaImporter schemaImporter = new SchemaImporter(schemas, typeNames, elements, elementTypeNames, DataContractSet, ImportXmlDataType);
                schemaImporter.Import();
                return true;
            }
            catch (InvalidDataContractException)
            {
                dataContractSet = oldValue;
                return false;
            }
            catch (Exception ex)
            {
                if (DiagnosticUtility.IsFatal(ex))
                {
                    throw;
                }
                dataContractSet = oldValue;
                TraceImportError(ex);
                throw;
            }
        }
    }
}
