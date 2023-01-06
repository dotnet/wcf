// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Xml;

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
