// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Xml;


namespace System.Runtime.Serialization
{
    public abstract class DataContractResolver
    {
        public abstract bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace);
        public abstract Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver);
    }
}
