// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
