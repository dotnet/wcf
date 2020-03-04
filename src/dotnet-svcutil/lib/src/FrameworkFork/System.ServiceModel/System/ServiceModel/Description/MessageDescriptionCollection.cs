// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections.ObjectModel;

namespace System.ServiceModel.Description
{
    public class MessageDescriptionCollection : Collection<MessageDescription>
    {
        internal MessageDescriptionCollection()
        {
        }

        public MessageDescription Find(string action)
        {
            foreach (MessageDescription description in this)
            {
                if (description != null && action == description.Action)
                    return description;
            }

            return null;
        }

        public Collection<MessageDescription> FindAll(string action)
        {
            Collection<MessageDescription> descriptions = new Collection<MessageDescription>();
            foreach (MessageDescription description in this)
            {
                if (description != null && action == description.Action)
                    descriptions.Add(description);
            }

            return descriptions;
        }
    }
}
