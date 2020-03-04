// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.Runtime.Serialization
{
    public interface ISerializationSurrogateProvider
    {
        Type GetSurrogateType(Type type);
        object GetObjectToSerialize(object obj, Type targetType);
        object GetDeserializedObject(object obj, Type targetType);
    }
}
