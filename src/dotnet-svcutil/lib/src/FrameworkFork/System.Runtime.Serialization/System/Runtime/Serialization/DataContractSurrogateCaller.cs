// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Serialization
{
    using System;
    using Microsoft.CodeDom;
    using System.Reflection;
    using System.Collections.ObjectModel;

    internal static class DataContractSurrogateCaller
    {
        internal static Type GetDataContractType(IDataContractSurrogate surrogate, Type type)
        {
            if (DataContract.GetBuiltInDataContract(type) != null)
                return type;
            Type dcType = surrogate.GetDataContractType(type);
            if (dcType == null)
                return type;
            return dcType;
        }

        internal static Type GetDataContractType(ISerializationSurrogateProvider surrogateProvider, Type type)
        {
            if (DataContract.GetBuiltInDataContract(type) != null)
                return type;
            return surrogateProvider.GetSurrogateType(type) ?? type;
        }

        internal static object GetObjectToSerialize(ISerializationSurrogateProvider surrogateProvider, object obj, Type objType, Type membertype)
        {
            if (obj == null)
                return null;
            if (DataContract.GetBuiltInDataContract(objType) != null)
                return obj;
            return surrogateProvider.GetObjectToSerialize(obj, membertype);
        }

        internal static object GetDeserializedObject(ISerializationSurrogateProvider surrogateProvider, object obj, Type objType, Type memberType)
        {
            if (obj == null)
                return null;
            if (DataContract.GetBuiltInDataContract(objType) != null)
                return obj;
            return surrogateProvider.GetDeserializedObject(obj, memberType);
        }

        internal static object GetCustomDataToExport(IDataContractSurrogate surrogate, MemberInfo memberInfo, Type dataContractType)
        {
            return surrogate.GetCustomDataToExport(memberInfo, dataContractType);
        }

        internal static object GetCustomDataToExport(IDataContractSurrogate surrogate, Type clrType, Type dataContractType)
        {
            if (DataContract.GetBuiltInDataContract(clrType) != null)
                return null;
            return surrogate.GetCustomDataToExport(clrType, dataContractType);
        }

        internal static void GetKnownCustomDataTypes(IDataContractSurrogate surrogate, Collection<Type> customDataTypes)
        {
            surrogate.GetKnownCustomDataTypes(customDataTypes);
        }

        internal static Type GetReferencedTypeOnImport(IDataContractSurrogate surrogate, string typeName, string typeNamespace, object customData)
        {
            if (DataContract.GetBuiltInDataContract(typeName, typeNamespace) != null)
                return null;
            return surrogate.GetReferencedTypeOnImport(typeName, typeNamespace, customData);
        }

        internal static CodeTypeDeclaration ProcessImportedType(IDataContractSurrogate surrogate, CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
        {
            return surrogate.ProcessImportedType(typeDeclaration, compileUnit);
        }
    }
}

