// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.MsmqIntegration
{
    // Mirrors System.Messaging.AcknowledgeTypes. Values correspond to
    // the MQMSG_ACKNOWLEDGMENT_* flags consumed by MSMQ's native API.
    [Flags]
    public enum AcknowledgeTypes
    {
        None = 0,
        PositiveArrival = 1,
        PositiveReceive = 2,
        NegativeArrival = 4,
        NotAcknowledgeReachQueue = 4,
        NegativeReceive = 8,
        NotAcknowledgeReceive = 12,
        FullReachQueue = 5,
        FullReceive = 14,
    }
}
