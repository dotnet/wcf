// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;

namespace System.ServiceModel.Channels
{
    // Minimal subset of JsonGlobals constants used by the Web HTTP encoder factories.
    // Mirrors CoreWCF.Runtime.JsonGlobals.
    internal static class JsonGlobals
    {
        public const string ApplicationJsonMediaType = "application/json";
        public const string TextJsonMediaType = "text/json";

        // XML dictionary string constants used by DataContractJsonSerializer-emitted XML.
        public const string RootString = "root";
        public const string ItemString = "item";
        public const string TypeString = "type";
        public const string ObjectString = "object";
        public const string DString = "d";
        public const string NullString = "null";

        // Pre-created dictionary strings.
        public static readonly XmlDictionaryString RootDictionaryString =
            new XmlDictionary().Add(RootString);
        public static readonly XmlDictionaryString ItemDictionaryString =
            new XmlDictionary().Add(ItemString);
        public static readonly XmlDictionaryString DDictionaryString =
            new XmlDictionary().Add(DString);
    }
}
