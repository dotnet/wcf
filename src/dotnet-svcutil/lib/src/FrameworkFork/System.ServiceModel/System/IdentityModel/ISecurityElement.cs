// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Xml;

namespace System.IdentityModel
{
    internal interface ISecurityElement
    {
        bool HasId { get; }

        string Id { get; }

        void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager);
    }
}
