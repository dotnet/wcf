// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.MsmqIntegration
{
    // Mirrors System.Messaging.MessagePriority. Higher-priority
    // messages are delivered ahead of lower-priority ones within a
    // single queue. The default at the MSMQ level is Normal.
    public enum MessagePriority
    {
        Lowest = 0,
        VeryLow = 1,
        Low = 2,
        Normal = 3,
        AboveNormal = 4,
        High = 5,
        VeryHigh = 6,
        Highest = 7,
    }
}
