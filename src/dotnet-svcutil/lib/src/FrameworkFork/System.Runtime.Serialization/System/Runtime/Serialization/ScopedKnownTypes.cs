// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.Runtime.Serialization
{
    using System;
    using Microsoft.Xml;
    using DataContractDictionary = System.Collections.Generic.Dictionary<Microsoft.Xml.XmlQualifiedName, DataContract>;

    internal struct ScopedKnownTypes
    {
        internal DataContractDictionary[] dataContractDictionaries;
        private int _count;
        internal void Push(DataContractDictionary dataContractDictionary)
        {
            if (dataContractDictionaries == null)
                dataContractDictionaries = new DataContractDictionary[4];
            else if (_count == dataContractDictionaries.Length)
                Array.Resize<DataContractDictionary>(ref dataContractDictionaries, dataContractDictionaries.Length * 2);
            dataContractDictionaries[_count++] = dataContractDictionary;
        }

        internal void Pop()
        {
            _count--;
        }

        internal DataContract GetDataContract(XmlQualifiedName qname)
        {
            for (int i = (_count - 1); i >= 0; i--)
            {
                DataContractDictionary dataContractDictionary = dataContractDictionaries[i];
                DataContract dataContract;
                if (dataContractDictionary.TryGetValue(qname, out dataContract))
                    return dataContract;
            }
            return null;
        }
    }
}
