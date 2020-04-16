// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
