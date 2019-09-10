//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace System.Runtime.Serialization
{
    using System;
    using Microsoft.CodeDom;
    using System.Reflection;
    using System.Collections.ObjectModel;

    public interface IDataContractSurrogate
    {
        Type GetDataContractType(Type type);
        object GetObjectToSerialize(object obj, Type targetType);
        object GetDeserializedObject(object obj, Type targetType);
        object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType);
        object GetCustomDataToExport(Type clrType, Type dataContractType);
        void GetKnownCustomDataTypes(Collection<Type> customDataTypes);
        Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData);
        CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit);
    }
}
