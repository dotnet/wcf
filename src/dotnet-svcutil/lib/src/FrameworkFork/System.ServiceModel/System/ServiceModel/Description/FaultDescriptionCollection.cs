// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace System.ServiceModel.Description
{
    public class FaultDescriptionCollection : Collection<FaultDescription>
    {
        internal FaultDescriptionCollection()
        {
        }

        public FaultDescription Find(string action)
        {
            foreach (FaultDescription description in this)
            {
                if (description != null && action == description.Action)
                    return description;
            }

            return null;
        }

        public Collection<FaultDescription> FindAll(string action)
        {
            Collection<FaultDescription> descriptions = new Collection<FaultDescription>();
            foreach (FaultDescription description in this)
            {
                if (description != null && action == description.Action)
                    descriptions.Add(description);
            }

            return descriptions;
        }
    }
}
