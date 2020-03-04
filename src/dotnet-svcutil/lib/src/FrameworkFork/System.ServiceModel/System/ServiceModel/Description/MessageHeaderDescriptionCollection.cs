// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections.ObjectModel;
using Microsoft.Xml;

namespace System.ServiceModel.Description
{
    public class MessageHeaderDescriptionCollection : KeyedCollection<XmlQualifiedName, MessageHeaderDescription>
    {
        internal MessageHeaderDescriptionCollection() : base(null, 4)
        {
        }

        protected override XmlQualifiedName GetKeyForItem(MessageHeaderDescription item)
        {
            return new XmlQualifiedName(item.Name, item.Namespace);
        }
    }
}
