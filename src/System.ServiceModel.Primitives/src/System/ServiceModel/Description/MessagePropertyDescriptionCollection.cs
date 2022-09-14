// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.ObjectModel;

namespace System.ServiceModel.Description
{
    public class MessagePropertyDescriptionCollection : KeyedCollection<string, MessagePropertyDescription>
    {
        internal MessagePropertyDescriptionCollection() : base(null, 4)
        {
        }

        protected override string GetKeyForItem(MessagePropertyDescription item)
        {
            return item.Name;
        }
    }
}
