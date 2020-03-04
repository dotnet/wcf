// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections.ObjectModel;
using Microsoft.Xml;

namespace System.ServiceModel.Description
{
    public class MessagePartDescriptionCollection : KeyedCollection<XmlQualifiedName, MessagePartDescription>
    {
        internal MessagePartDescriptionCollection()
            : base(null, 4)
        {
        }

        protected override XmlQualifiedName GetKeyForItem(MessagePartDescription item)
        {
            return new XmlQualifiedName(item.Name, item.Namespace);
        }
    }
}
