// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
