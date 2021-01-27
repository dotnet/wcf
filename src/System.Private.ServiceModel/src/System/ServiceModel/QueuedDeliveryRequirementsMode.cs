// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    public enum QueuedDeliveryRequirementsMode
    {
        Allowed,
        Required,
        NotAllowed,
    }

    internal static class QueuedDeliveryRequirementsModeHelper
    {
        static public bool IsDefined(QueuedDeliveryRequirementsMode x)
        {
            return
                x == QueuedDeliveryRequirementsMode.Allowed ||
                x == QueuedDeliveryRequirementsMode.Required ||
                x == QueuedDeliveryRequirementsMode.NotAllowed ||
                false;
        }
    }
}
