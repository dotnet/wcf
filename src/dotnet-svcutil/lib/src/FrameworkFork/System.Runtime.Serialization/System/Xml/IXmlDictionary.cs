// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml
{
    public interface IXmlDictionary
    {
        bool TryLookup(string value, out XmlDictionaryString result);
        bool TryLookup(int key, out XmlDictionaryString result);
        bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result);
    }
}
