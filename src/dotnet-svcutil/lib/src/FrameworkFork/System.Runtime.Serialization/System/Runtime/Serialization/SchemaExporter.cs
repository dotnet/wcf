// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.Runtime.Serialization
{
    using System;
    using System.Linq;
    using Microsoft.Xml;
    using Microsoft.Xml.Schema;
    using Microsoft.Xml.Serialization;
    using System.Reflection;
    using System.Security;

    internal class SchemaExporter
    {
        internal static void GetXmlTypeInfo(Type type, out XmlQualifiedName stableName, out XmlSchemaType xsdType, out bool hasRoot)
        {
            if (IsSpecialXmlType(type, out stableName, out xsdType, out hasRoot))
                return;
            XmlSchemaSet schemas = null;
            InvokeSchemaProviderMethod(type, schemas, out stableName, out xsdType, out hasRoot);
            if (stableName.Name == null || stableName.Name.Length == 0)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SRSerialization.Format(SRSerialization.InvalidXmlDataContractName, DataContract.GetClrTypeFullName(type))));
        }

        internal static long GetDefaultEnumValue(bool isFlags, int index)
        {
            return isFlags ? (long)Math.Pow(2, index) : index;
        }

        private static bool InvokeSchemaProviderMethod(Type clrType, XmlSchemaSet schemas, out XmlQualifiedName stableName, out XmlSchemaType xsdType, out bool hasRoot)
        {
            xsdType = null;
            hasRoot = true;
            object[] attrs = clrType.GetTypeInfo().GetCustomAttributes(Globals.TypeOfXmlSchemaProviderAttribute, false).ToArray();
            if (attrs == null || attrs.Length == 0)
            {
                stableName = DataContract.GetDefaultStableName(clrType);
                return false;
            }

            XmlSchemaProviderAttribute provider = (XmlSchemaProviderAttribute)attrs[0];
            if (provider.IsAny)
            {
                xsdType = CreateAnyElementType();
                hasRoot = false;
            }
            string methodName = provider.MethodName;
            if (methodName == null || methodName.Length == 0)
            {
                if (!provider.IsAny)
                    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SRSerialization.Format(SRSerialization.InvalidGetSchemaMethod, DataContract.GetClrTypeFullName(clrType))));
                stableName = DataContract.GetDefaultStableName(clrType);
            }
            else
            {
                MethodInfo getMethod = clrType.GetMethod(methodName,  /*BindingFlags.DeclaredOnly |*/ BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, new Type[] { typeof(XmlSchemaSet) });
                if (getMethod == null)
                    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SRSerialization.Format(SRSerialization.MissingGetSchemaMethod, DataContract.GetClrTypeFullName(clrType), methodName)));

                if (!(Globals.TypeOfXmlQualifiedName.IsAssignableFrom(getMethod.ReturnType)))
                    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SRSerialization.Format(SRSerialization.InvalidReturnTypeOnGetSchemaMethod, DataContract.GetClrTypeFullName(clrType), methodName, DataContract.GetClrTypeFullName(getMethod.ReturnType), DataContract.GetClrTypeFullName(Globals.TypeOfXmlQualifiedName))));

                object typeInfo = getMethod.Invoke(null, new object[] { schemas });

                if (provider.IsAny)
                {
                    if (typeInfo != null)
                        throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SRSerialization.Format(SRSerialization.InvalidNonNullReturnValueByIsAny, DataContract.GetClrTypeFullName(clrType), methodName)));
                    stableName = DataContract.GetDefaultStableName(clrType);
                }
                else if (typeInfo == null)
                {
                    xsdType = CreateAnyElementType();
                    hasRoot = false;
                    stableName = DataContract.GetDefaultStableName(clrType);
                }
                else
                {
                    stableName = (XmlQualifiedName)typeInfo;
                }
            }
            return true;
        }

        private static XmlSchemaComplexType CreateAnyElementType()
        {
            XmlSchemaComplexType anyElementType = new XmlSchemaComplexType();
            anyElementType.IsMixed = false;
            anyElementType.Particle = new XmlSchemaSequence();
            XmlSchemaAny any = new XmlSchemaAny();
            any.MinOccurs = 0;
            any.ProcessContents = XmlSchemaContentProcessing.Lax;
            ((XmlSchemaSequence)anyElementType.Particle).Items.Add(any);
            return anyElementType;
        }

        private static XmlSchemaComplexType CreateAnyType()
        {
            XmlSchemaComplexType anyType = new XmlSchemaComplexType();
            anyType.IsMixed = true;
            anyType.Particle = new XmlSchemaSequence();
            XmlSchemaAny any = new XmlSchemaAny();
            any.MinOccurs = 0;
            any.MaxOccurs = Decimal.MaxValue;
            any.ProcessContents = XmlSchemaContentProcessing.Lax;
            ((XmlSchemaSequence)anyType.Particle).Items.Add(any);
            anyType.AnyAttribute = new XmlSchemaAnyAttribute();
            return anyType;
        }

        internal static bool IsSpecialXmlType(Type type, out XmlQualifiedName typeName, out XmlSchemaType xsdType, out bool hasRoot)
        {
            xsdType = null;
            hasRoot = true;
            if (type == Globals.TypeOfXmlElement || type == Globals.TypeOfXmlNodeArray)
            {
                string name = null;
                if (type == Globals.TypeOfXmlElement)
                {
                    xsdType = CreateAnyElementType();
                    name = "XmlElement";
                    hasRoot = false;
                }
                else
                {
                    xsdType = CreateAnyType();
                    name = "ArrayOfXmlNode";
                    hasRoot = true;
                }
                typeName = new XmlQualifiedName(name, DataContract.GetDefaultStableNamespace(type));
                return true;
            }
            typeName = null;
            return false;
        }

        // Property is not stored in a local because XmlSchemaAny is mutable.
        // The schema export process should not expose objects that may be modified later.
        internal static XmlSchemaAny ISerializableWildcardElement
        {
            get
            {
                XmlSchemaAny iSerializableWildcardElement = new XmlSchemaAny();
                iSerializableWildcardElement.MinOccurs = 0;
                iSerializableWildcardElement.MaxOccursString = Globals.OccursUnbounded;
                iSerializableWildcardElement.Namespace = "##local";
                iSerializableWildcardElement.ProcessContents = XmlSchemaContentProcessing.Skip;
                return iSerializableWildcardElement;
            }
        }

        // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        static XmlQualifiedName anytypeQualifiedName;
        internal static XmlQualifiedName AnytypeQualifiedName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical anytypeQualifiedName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (anytypeQualifiedName == null)
                    anytypeQualifiedName = new XmlQualifiedName(Globals.AnyTypeLocalName, Globals.SchemaNamespace);
                return anytypeQualifiedName;
            }
        }

        // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        static XmlQualifiedName stringQualifiedName;
        internal static XmlQualifiedName StringQualifiedName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical stringQualifiedName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (stringQualifiedName == null)
                    stringQualifiedName = new XmlQualifiedName(Globals.StringLocalName, Globals.SchemaNamespace);
                return stringQualifiedName;
            }
        }

        // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        static XmlQualifiedName defaultEnumBaseTypeName;
        internal static XmlQualifiedName DefaultEnumBaseTypeName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical defaultEnumBaseTypeName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (defaultEnumBaseTypeName == null)
                    defaultEnumBaseTypeName = new XmlQualifiedName(Globals.IntLocalName, Globals.SchemaNamespace);
                return defaultEnumBaseTypeName;
            }
        }

         // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        static XmlQualifiedName enumerationValueAnnotationName;
        internal static XmlQualifiedName EnumerationValueAnnotationName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical enumerationValueAnnotationName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (enumerationValueAnnotationName == null)
                    enumerationValueAnnotationName = new XmlQualifiedName(Globals.EnumerationValueLocalName, Globals.SerializationNamespace);
                return enumerationValueAnnotationName;
            }
        }

         // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        static XmlQualifiedName surrogateDataAnnotationName;
        internal static XmlQualifiedName SurrogateDataAnnotationName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical surrogateDataAnnotationName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (surrogateDataAnnotationName == null)
                    surrogateDataAnnotationName = new XmlQualifiedName(Globals.SurrogateDataLocalName, Globals.SerializationNamespace);
                return surrogateDataAnnotationName;
            }
        }

        // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        static XmlQualifiedName defaultValueAnnotation;
        internal static XmlQualifiedName DefaultValueAnnotation
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical defaultValueAnnotation field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (defaultValueAnnotation == null)
                    defaultValueAnnotation = new XmlQualifiedName(Globals.DefaultValueLocalName, Globals.SerializationNamespace);
                return defaultValueAnnotation;
            }
        }

        // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        static XmlQualifiedName actualTypeAnnotationName;
        internal static XmlQualifiedName ActualTypeAnnotationName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical actualTypeAnnotationName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (actualTypeAnnotationName == null)
                    actualTypeAnnotationName = new XmlQualifiedName(Globals.ActualTypeLocalName, Globals.SerializationNamespace);
                return actualTypeAnnotationName;
            }
        }

        // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        static XmlQualifiedName isDictionaryAnnotationName;
        internal static XmlQualifiedName IsDictionaryAnnotationName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical isDictionaryAnnotationName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (isDictionaryAnnotationName == null)
                    isDictionaryAnnotationName = new XmlQualifiedName(Globals.IsDictionaryLocalName, Globals.SerializationNamespace);
                return isDictionaryAnnotationName;
            }
        }

        // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        static XmlQualifiedName isValueTypeName;
        internal static XmlQualifiedName IsValueTypeName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical isValueTypeName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (isValueTypeName == null)
                    isValueTypeName = new XmlQualifiedName(Globals.IsValueTypeLocalName, Globals.SerializationNamespace);
                return isValueTypeName;
            }
        }

        // Property is not stored in a local because XmlSchemaAttribute is mutable.
        // The schema export process should not expose objects that may be modified later.
        internal static XmlSchemaAttribute ISerializableFactoryTypeAttribute
        {
            get
            {
                XmlSchemaAttribute iSerializableFactoryTypeAttribute = new XmlSchemaAttribute();
                iSerializableFactoryTypeAttribute.RefName = new XmlQualifiedName(Globals.ISerializableFactoryTypeLocalName, Globals.SerializationNamespace);
                return iSerializableFactoryTypeAttribute;
            }
        }

    }
}

