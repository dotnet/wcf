// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Channels
{
    // Bindings can implement this interface in order to advertise capabilities 
    // that DeliveryRequirementsAttribute consumes
    public interface IBindingDeliveryCapabilities
    {
        // This bool gives an assurance: "true" means you get Ordered, "false" means you may not
        // [DeliveryRequirements(RequireOrderedDelivery)] consumes this
        bool AssuresOrderedDelivery { get; }

        // Is this binding a queue (in the transacted-receive sense)
        // [DeliveryRequirements(QueuedDeliveryRequirements)] consumes this
        bool QueuedDelivery { get; }
    }
}
