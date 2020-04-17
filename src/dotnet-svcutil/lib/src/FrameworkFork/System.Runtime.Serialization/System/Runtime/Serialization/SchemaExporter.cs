// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(string.Format(SRSerialization.InvalidXmlDataContractName, DataContract.GetClrTypeFullName(type))));
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
                    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(string.Format(SRSerialization.InvalidGetSchemaMethod, DataContract.GetClrTypeFullName(clrType))));
                stableName = DataContract.GetDefaultStableName(clrType);
            }
            else
            {
                MethodInfo getMethod = clrType.GetMethod(methodName,  /*BindingFlags.DeclaredOnly |*/ BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, new Type[] { typeof(XmlSchemaSet) });
                if (getMethod == null)
                    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(string.Format(SRSerialization.MissingGetSchemaMethod, DataContract.GetClrTypeFullName(clrType), methodName)));

                if (!(Globals.TypeOfXmlQualifiedName.IsAssignableFrom(getMethod.ReturnType)))
                    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(string.Format(SRSerialization.InvalidReturnTypeOnGetSchemaMethod, DataContract.GetClrTypeFullName(clrType), methodName, DataContract.GetClrTypeFullName(getMethod.ReturnType), DataContract.GetClrTypeFullName(Globals.TypeOfXmlQualifiedName))));

                object typeInfo = getMethod.Invoke(null, new object[] { schemas });

                if (provider.IsAny)
                {
                    if (typeInfo != null)
                        throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(string.Format(SRSerialization.InvalidNonNullReturnValueByIsAny, DataContract.GetClrTypeFullName(clrType), methodName)));
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
        private static XmlQualifiedName s_anytypeQualifiedName;
        internal static XmlQualifiedName AnytypeQualifiedName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical anytypeQualifiedName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (s_anytypeQualifiedName == null)
                    s_anytypeQualifiedName = new XmlQualifiedName(Globals.AnyTypeLocalName, Globals.SchemaNamespace);
                return s_anytypeQualifiedName;
            }
        }

        // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        private static XmlQualifiedName s_stringQualifiedName;
        internal static XmlQualifiedName StringQualifiedName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical stringQualifiedName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (s_stringQualifiedName == null)
                    s_stringQualifiedName = new XmlQualifiedName(Globals.StringLocalName, Globals.SchemaNamespace);
                return s_stringQualifiedName;
            }
        }

        // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        private static XmlQualifiedName s_defaultEnumBaseTypeName;
        internal static XmlQualifiedName DefaultEnumBaseTypeName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical defaultEnumBaseTypeName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (s_defaultEnumBaseTypeName == null)
                    s_defaultEnumBaseTypeName = new XmlQualifiedName(Globals.IntLocalName, Globals.SchemaNamespace);
                return s_defaultEnumBaseTypeName;
            }
        }

        // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        private static XmlQualifiedName s_enumerationValueAnnotationName;
        internal static XmlQualifiedName EnumerationValueAnnotationName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical enumerationValueAnnotationName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (s_enumerationValueAnnotationName == null)
                    s_enumerationValueAnnotationName = new XmlQualifiedName(Globals.EnumerationValueLocalName, Globals.SerializationNamespace);
                return s_enumerationValueAnnotationName;
            }
        }

        // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        private static XmlQualifiedName s_surrogateDataAnnotationName;
        internal static XmlQualifiedName SurrogateDataAnnotationName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical surrogateDataAnnotationName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (s_surrogateDataAnnotationName == null)
                    s_surrogateDataAnnotationName = new XmlQualifiedName(Globals.SurrogateDataLocalName, Globals.SerializationNamespace);
                return s_surrogateDataAnnotationName;
            }
        }

        // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        private static XmlQualifiedName s_defaultValueAnnotation;
        internal static XmlQualifiedName DefaultValueAnnotation
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical defaultValueAnnotation field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (s_defaultValueAnnotation == null)
                    s_defaultValueAnnotation = new XmlQualifiedName(Globals.DefaultValueLocalName, Globals.SerializationNamespace);
                return s_defaultValueAnnotation;
            }
        }

        // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        private static XmlQualifiedName s_actualTypeAnnotationName;
        internal static XmlQualifiedName ActualTypeAnnotationName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical actualTypeAnnotationName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (s_actualTypeAnnotationName == null)
                    s_actualTypeAnnotationName = new XmlQualifiedName(Globals.ActualTypeLocalName, Globals.SerializationNamespace);
                return s_actualTypeAnnotationName;
            }
        }

        // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        private static XmlQualifiedName s_isDictionaryAnnotationName;
        internal static XmlQualifiedName IsDictionaryAnnotationName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical isDictionaryAnnotationName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (s_isDictionaryAnnotationName == null)
                    s_isDictionaryAnnotationName = new XmlQualifiedName(Globals.IsDictionaryLocalName, Globals.SerializationNamespace);
                return s_isDictionaryAnnotationName;
            }
        }

        // TODO:  [Fx.Tag.SecurityNote(Critical = "Static fields are marked SecurityCritical or readonly to prevent"
        //     + " data from being modified or leaked to other components in appdomain.")]
        [SecurityCritical]
        private static XmlQualifiedName s_isValueTypeName;
        internal static XmlQualifiedName IsValueTypeName
        {
            // TODO:  [Fx.Tag.SecurityNote(Critical = "Fetches the critical isValueTypeName field.",
            // Safe = "Get-only properties only needs to be protected for write; initialized in getter if null.")]
            [SecuritySafeCritical]
            get
            {
                if (s_isValueTypeName == null)
                    s_isValueTypeName = new XmlQualifiedName(Globals.IsValueTypeLocalName, Globals.SerializationNamespace);
                return s_isValueTypeName;
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

