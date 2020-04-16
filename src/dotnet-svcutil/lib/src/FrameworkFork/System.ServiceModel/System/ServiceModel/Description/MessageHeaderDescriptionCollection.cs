// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
