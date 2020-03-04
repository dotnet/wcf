// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
