// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Xml;

namespace System.ServiceModel.Security
{
    internal abstract class SignatureTargetIdManager
    {
        protected SignatureTargetIdManager()
        {
        }

        public abstract string DefaultIdNamespacePrefix { get; }

        public abstract string DefaultIdNamespaceUri { get; }

        public abstract string ExtractId(XmlDictionaryReader reader);

        public abstract void WriteIdAttribute(XmlDictionaryWriter writer, string id);
    }
}
