// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        private ImportOptions _options;
        private CodeCompileUnit _codeCompileUnit;
        private DataContractSet _dataContractSet;

        private static readonly XmlQualifiedName[] s_emptyTypeNameArray = new XmlQualifiedName[0];

        private static readonly XmlSchemaElement[] s_emptyElementArray = new XmlSchemaElement[0];
        private XmlQualifiedName[] _singleTypeNameArray;
        private XmlSchemaElement[] _singleElementArray;

        public XsdDataContractImporter()
        {
        }

        public XsdDataContractImporter(CodeCompileUnit codeCompileUnit)
        {
            _codeCompileUnit = codeCompileUnit;
        }

        public ImportOptions Options
        {
            get { return _options; }
            set { _options = value; }
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
            if (_codeCompileUnit == null)
                _codeCompileUnit = new CodeCompileUnit();
            return _codeCompileUnit;
        }


        private DataContractSet DataContractSet
        {
            get
            {
                if (_dataContractSet == null)
                {
                    _dataContractSet = Options == null ? new DataContractSet(null, null, null) :
                                                        new DataContractSet(Options.DataContractSurrogate, Options.ReferencedTypes, Options.ReferencedCollectionTypes);
                }
                return _dataContractSet;
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

            InternalImport(schemas, typeNames, s_emptyElementArray, s_emptyTypeNameArray);
        }

        public void Import(XmlSchemaSet schemas, XmlQualifiedName typeName)
        {
            if (schemas == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));

            if (typeName == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));

            SingleTypeNameArray[0] = typeName;
            InternalImport(schemas, SingleTypeNameArray, s_emptyElementArray, s_emptyTypeNameArray);
        }

        public XmlQualifiedName Import(XmlSchemaSet schemas, XmlSchemaElement element)
        {
            if (schemas == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));

            if (element == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("element"));

            SingleTypeNameArray[0] = null;
            SingleElementArray[0] = element;
            InternalImport(schemas, s_emptyTypeNameArray, SingleElementArray, SingleTypeNameArray/*filled on return*/);
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

            return InternalCanImport(schemas, typeNames, s_emptyElementArray, s_emptyTypeNameArray);
        }

        public bool CanImport(XmlSchemaSet schemas, XmlQualifiedName typeName)
        {
            if (schemas == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));

            if (typeName == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));

            return InternalCanImport(schemas, new XmlQualifiedName[] { typeName }, s_emptyElementArray, s_emptyTypeNameArray);
        }

        public bool CanImport(XmlSchemaSet schemas, XmlSchemaElement element)
        {
            if (schemas == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));

            if (element == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("element"));

            SingleTypeNameArray[0] = null;
            SingleElementArray[0] = element;
            return InternalCanImport(schemas, s_emptyTypeNameArray, SingleElementArray, SingleTypeNameArray);
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
                    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRSerialization.TypeHasNotBeenImported, typeName.Name, typeName.Namespace)));
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
                    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRSerialization.TypeHasNotBeenImported, typeName.Name, typeName.Namespace)));
            }

            CodeExporter codeExporter = new CodeExporter(DataContractSet, Options, GetCodeCompileUnit());
            return codeExporter.GetKnownTypeReferences(dataContract);
        }

        private XmlQualifiedName[] SingleTypeNameArray
        {
            get
            {
                if (_singleTypeNameArray == null)
                    _singleTypeNameArray = new XmlQualifiedName[1];
                return _singleTypeNameArray;
            }
        }

        private XmlSchemaElement[] SingleElementArray
        {
            get
            {
                if (_singleElementArray == null)
                    _singleElementArray = new XmlSchemaElement[1];
                return _singleElementArray;
            }
        }

        private void InternalImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames, ICollection<XmlSchemaElement> elements, XmlQualifiedName[] elementTypeNames/*filled on return*/)
        {
            DataContractSet oldValue = (_dataContractSet == null) ? null : new DataContractSet(_dataContractSet);
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
                _dataContractSet = oldValue;
                TraceImportError(ex);
                throw;
            }
        }

        private bool ImportXmlDataType
        {
            get
            {
                return this.Options == null ? false : this.Options.ImportXmlType;
            }
        }

        private void TraceImportError(Exception exception)
        {
        }

        private bool InternalCanImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames, ICollection<XmlSchemaElement> elements, XmlQualifiedName[] elementTypeNames)
        {
            DataContractSet oldValue = (_dataContractSet == null) ? null : new DataContractSet(_dataContractSet);
            try
            {
                SchemaImporter schemaImporter = new SchemaImporter(schemas, typeNames, elements, elementTypeNames, DataContractSet, ImportXmlDataType);
                schemaImporter.Import();
                return true;
            }
            catch (InvalidDataContractException)
            {
                _dataContractSet = oldValue;
                return false;
            }
            catch (Exception ex)
            {
                if (DiagnosticUtility.IsFatal(ex))
                {
                    throw;
                }
                _dataContractSet = oldValue;
                TraceImportError(ex);
                throw;
            }
        }
    }
}
