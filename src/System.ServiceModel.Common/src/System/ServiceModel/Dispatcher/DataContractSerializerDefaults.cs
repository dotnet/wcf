// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;

namespace System.ServiceModel.Dispatcher
{
    internal static class DataContractSerializerDefaults
    {
        internal const bool IgnoreExtensionDataObject = false;
        internal const int MaxItemsInObjectGraph = int.MaxValue;

        internal static DataContractSerializer CreateSerializer(Type type, int maxItems)
        {
            return CreateSerializer(type, null, maxItems);
        }

        internal static DataContractSerializer CreateSerializer(Type type, IList<Type> knownTypes, int maxItems)
        {
            return new DataContractSerializer(
                type,
                knownTypes);
        }

        internal static DataContractSerializer CreateSerializer(Type type, string rootName, string rootNs, int maxItems)
        {
            return CreateSerializer(type, null, rootName, rootNs, maxItems);
        }

        internal static DataContractSerializer CreateSerializer(Type type, IList<Type> knownTypes, string rootName, string rootNs, int maxItems)
        {
            XmlDictionary dictionary = new XmlDictionary(2);
            return new DataContractSerializer(
                type,
                dictionary.Add(rootName),
                dictionary.Add(rootNs),
                knownTypes);
        }

        internal static DataContractSerializer CreateSerializer(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNs, int maxItems)
        {
            return CreateSerializer(type, null, rootName, rootNs, maxItems);
        }

        internal static DataContractSerializer CreateSerializer(Type type, IList<Type> knownTypes, XmlDictionaryString rootName, XmlDictionaryString rootNs, int maxItems)
        {
            return new DataContractSerializer(
                type,
                rootName,
                rootNs,
                knownTypes);
        }
    }
}
