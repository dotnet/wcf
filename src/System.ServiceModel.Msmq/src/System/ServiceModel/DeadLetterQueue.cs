// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel
{
    public enum DeadLetterQueue
    {
        None,
        System,
        Custom
    }

    internal static class DeadLetterQueueHelper
    {
        internal static bool IsDefined(DeadLetterQueue value)
        {
            return value == DeadLetterQueue.None
                || value == DeadLetterQueue.System
                || value == DeadLetterQueue.Custom;
        }
    }
}
